using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Diagnostics;


namespace FolderMove
{
    public partial class FolderMove : Form
    {

        private const string SettingsFileName = "FolderOrganizerSettings.xml"; //불러왔던 폴더 기억하는 파일

        public FolderMove()
        {
            InitializeComponent();
            LoadSettings(); //실행할때 전에 선택했던 파일경로 불러오기
            //InitializeFolderListTextBox();
        }

        /*private void InitializeFolderListTextBox()
        {
            FolderListBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Bottom,
                Height = 150
            };
            this.Controls.Add(FolderListBox);
        }*/

        private void browsebutton_Click(object sender, EventArgs e) // 브라우저버튼
        {
            using (var fbd = new FolderBrowserDialog()) // 브라우저 열기
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                //탐색기에서 확인누르기 + null 이나 공백이 아닐경우
                {
                    string selectedPath = fbd.SelectedPath; // path 를 string에 저장
                    if (!Pathdisplay.Items.Contains(selectedPath)) //pathdisplay 에 보이는 경로가 선택된 경로와 다를경우
                    {
                        Pathdisplay.Items.Add(selectedPath);
                    }
                    Pathdisplay.SelectedItem = selectedPath; // combobox 에 선택된항목을 현재 선택된 경로로
                    SaveSettings();
                    UpdateFolderList(selectedPath); // textbox 에 폴더목록 갱신
                }
            }
        } 

        private async void btnOrganize_Click(object sender, EventArgs e) // 정리버튼 작동
        {
            string rootPath = Pathdisplay.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(rootPath)) // 0이나 빈칸
            {
                MessageBox.Show("폴더를 선택해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            organizebtn.Enabled = false;
            progressBar1.Value = 0;

            try // 시도
            {
                await OrganizeFoldersAsync(rootPath);
                MessageBox.Show("폴더 정리가 완료되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) //예외오류 캐치
            {
                MessageBox.Show($"폴더 정리 중 오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally // 시도 정상종료시
            {
                organizebtn.Enabled = true;
                progressBar1.Value = 100;
            }
        }

        private async Task OrganizeFoldersAsync(string rootPath)
        {
            string[] directories = Directory.GetDirectories(rootPath); // 디렉토리 얻어와서 배열에 저장
            int totalDirectories = directories.Length; //디렉토리배열의 길이
            int processedDirectories = 0; // 진행도 0

            await Task.Run(() =>
            {
                foreach (string dir in directories)
                {
                    string dirName = new DirectoryInfo(dir).Name;
                    Match match = Regex.Match(dirName, @"\[(.*?)\]");

                    if (match.Success)
                    {
                        string category = match.Groups[1].Value;
                        string categoryWithBrackets = $"[{category}]";
                        string newCategoryPath = Path.Combine(rootPath, categoryWithBrackets);

                        if (!Directory.Exists(newCategoryPath)) // [카테고리] 이름의 폴더가 존재하지않으면
                        {
                            Directory.CreateDirectory(newCategoryPath); // [카테고리] 이름을 가진 새로운 폴더 작성
                        }

                        string newPath = Path.Combine(newCategoryPath, dirName); // 경로 조합

                        if (Directory.Exists(newPath)) // 만약 [카테고리] 이름의 폴더가 있으면
                        {
                            int counter = 1; // 카운터 붙이고
                            string tempPath = newPath;
                            while (Directory.Exists(tempPath)) // [카테고리] (숫자) 의 이름을 시도해서 생성
                            {
                                tempPath = Path.Combine(newCategoryPath, $"{dirName} ({counter})");
                                counter++;
                            }
                            newPath = tempPath; // 확정된 이름을 선언
                        }

                        try
                        {
                            TryMoveDirectory(dir, newPath); // 이동함수 시도
                        }
                        catch (Exception ex) // 예외
                        {
                            // 에러 로깅 또는 처리
                            Console.WriteLine($"폴더 '{dirName}' 이동 중 오류: {ex.Message}");
                        }
                    }

                    // 진행바 표시
                    processedDirectories++;
                    int progressPercentage = (int)((processedDirectories / (double)totalDirectories) * 100);
                    UpdateProgressBar(progressPercentage);
                }
            });

            await UpdateFolderListAsync(rootPath);
        }

        private bool TryMoveDirectory(string sourceDir, string destDir, int maxRetries = 3) // 최대시도횟수 3으로설정
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try // 시도
                {
                    Directory.Move(sourceDir, destDir);
                    return true; // 성공시 true 반환
                }
                catch (IOException) // IO예외발생시
                {
                    if (i == maxRetries - 1) throw; // throw : 예외를 던지고 종료
                    Task.Delay(1000).Wait();
                }
            }
            return false;
        }

        private void UpdateProgressBar(int value)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(UpdateProgressBar), value);
            }
            else
            {
                progressBar1.Value = value;
            }
        }

        private async Task UpdateFolderListAsync(string path)
        {
            if (Directory.Exists(path))
            {
                string[] directories = await Task.Run(() => Directory.GetDirectories(path));
                StringBuilder sb = new StringBuilder();
                int foldercount = directories.Length;
                sb.AppendLine($"폴더 및 파일 개수: {foldercount}");
                foreach (string dir in directories)
                {
                    sb.AppendLine(new DirectoryInfo(dir).Name);
                }
                UpdateFolderListBoxText(sb.ToString());
            }
            else
            {
                UpdateFolderListBoxText("선택된 경로가 존재하지 않습니다.");
            }
        }

        private void UpdateFolderListBoxText(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateFolderListBoxText), text);
            }
            else
            {
                FolderListBox.Text = text;
            }
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            if (Pathdisplay.SelectedItem == null) // 선택안됐을경우
            {
                MessageBox.Show("폴더를 선택해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string selectedPath = Pathdisplay.SelectedItem.ToString();

            try
            {
                Process.Start("explorer.exe",selectedPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("폴더를 열 수 없습니다: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveSettings() // 저장
        {
            List<string> paths = new List<string>();
            foreach (var item in Pathdisplay.Items)
            {
                paths.Add(item.ToString());
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
            using (TextWriter writer = new StreamWriter(SettingsFileName))
            {
                serializer.Serialize(writer, paths);
            }
        }

        private void LoadSettings()
        {
            if (File.Exists(SettingsFileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
                using (TextReader reader = new StreamReader(SettingsFileName))
                {
                    List<string> paths = (List<string>)serializer.Deserialize(reader);
                    Pathdisplay.Items.AddRange(paths.ToArray());
                }
            }
        }

        private void UpdateFolderList(string path)
        {
            if (Directory.Exists(path))
            {
                string[] directories = Directory.GetDirectories(path); // path의 디렉토리 얻어온 뒤 배열에 저장
                StringBuilder sb = new StringBuilder(); // Append, Replace, Insert 등으로 변경가능한 문자열만들기
                int foldercount = directories.Length;
                sb.AppendLine("폴더 및 파일 개수: " + foldercount); // AppendLine = 새로운 줄 추가
                foreach (string dir in directories)
                {
                    sb.AppendLine(new DirectoryInfo(dir).Name); // 경로를 새로운 줄에 추가
                }
                FolderListBox.Text = sb.ToString();
            }
            else
            {
                FolderListBox.Text = "선택된 경로가 존재하지 않습니다.";
            }
        }

        private void Pathdisplay_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Pathdisplay.SelectedItem != null)
            {
                UpdateFolderList(Pathdisplay.SelectedItem.ToString());
            }
        }

        private void refreshbutton_clicked(object sender, EventArgs e)
        {
            if (Pathdisplay.SelectedItem != null)
            {
                UpdateFolderList(Pathdisplay.SelectedItem.ToString());
            }
        }
    }
}
