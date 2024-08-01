using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Diagnostics;

namespace FolderMove
{
    public partial class FolderMove : MetroFramework.Forms.MetroForm
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

        private void browsebutton_Click(object sender, EventArgs e)
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


        private void organizebtn_Click(object sender, EventArgs e) // 정리 시작버튼
        {
            if (Pathdisplay.SelectedItem == null) // 선택안됐을경우
            {
                MessageBox.Show("폴더를 선택해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string selectedPath = Pathdisplay.SelectedItem.ToString();
            OrganizeFolders(selectedPath);
            UpdateFolderList(selectedPath);
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


        private void OrganizeFolders(string rootPath) // 정리함수
        {
            string[] directories = Directory.GetDirectories(rootPath); // rootPate 인수에 경로 얻어오기
            int totalDirectories = directories.Length; // 디렉토리 배열의 길이
            int processedDirectories = 0; // 프로그레스 바

            /*try
            {*/
                foreach (string dir in directories) // directories 를 dir 로 사용화여 요소 끝날때까지 반복
                {
                    string dirName = new DirectoryInfo(dir).Name; // 디렉토리의 이름반환
                    Match match = Regex.Match(dirName, @"\[(.*?)\]"); // 디렉토리에서 [] 사이에있는 글자인식

                    if (match.Success) // 성공하면
                    {
                        string category = match.Groups[1].Value; // 대괄호를 포함한 카테고리 이름 생성
                        string categoryWithBrackets = $"[{category}]"; // 폴더이름지정
                        string newCategoryPath = Path.Combine(rootPath, categoryWithBrackets); // 폴더조합

                        if (!Directory.Exists(newCategoryPath))
                        {
                            Directory.CreateDirectory(newCategoryPath);
                        }

                        string newPath = Path.Combine(newCategoryPath, dirName);

                        // 이미 같은 이름의 폴더가 존재하는 경우 처리
                        if (Directory.Exists(newPath))
                        {
                            int counter = 1;
                            string tempPath = newPath;
                            while (Directory.Exists(tempPath))
                            {
                                tempPath = Path.Combine(newCategoryPath, $"{dirName} ({counter})");
                                // [aa] 중복시 [aa] 1 이런식으로 생성
                                counter++;
                            }
                            newPath = tempPath;
                        }

                        Directory.Move(dir, newPath); // 새로생성된 경로로 이동
                    }
                }

                MessageBox.Show("폴더 정리가 완료되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // 진행 상태 업데이트
                /*processedDirectories++;
                int progressPercentage = (int)((processedDirectories / (double)totalDirectories) * 100);
                this.Invoke(new Action(() => progressBar1.Value = progressPercentage));
            }
            
            catch (IOException ex)
            {
                MessageBox.Show("폴더 또는 파일이 사용중 입니다","오류",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"권한 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            // 작업 완료 후 100%로 설정
            this.Invoke(new Action(() => progressBar1.Value = 100));
            */
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
    }
}
