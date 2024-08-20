using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Diagnostics;


namespace FolderMove
{
    public partial class FolderMove : Form
    {

        private const string SettingsFileName = "FolderOrganizerSettings.xml"; //�ҷ��Դ� ���� ����ϴ� ����

        public FolderMove()
        {
            InitializeComponent();
            LoadSettings(); //�����Ҷ� ���� �����ߴ� ���ϰ�� �ҷ�����
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

        private void browsebutton_Click(object sender, EventArgs e) // ��������ư
        {
            using (var fbd = new FolderBrowserDialog()) // ������ ����
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                //Ž���⿡�� Ȯ�δ����� + null �̳� ������ �ƴҰ��
                {
                    string selectedPath = fbd.SelectedPath; // path �� string�� ����
                    if (!Pathdisplay.Items.Contains(selectedPath)) //pathdisplay �� ���̴� ��ΰ� ���õ� ��ο� �ٸ����
                    {
                        Pathdisplay.Items.Add(selectedPath);
                    }
                    Pathdisplay.SelectedItem = selectedPath; // combobox �� ���õ��׸��� ���� ���õ� ��η�
                    SaveSettings();
                    UpdateFolderList(selectedPath); // textbox �� ������� ����
                }
            }
        }

        private async void btnOrganize_Click(object sender, EventArgs e) // ������ư �۵�
        {
            string rootPath = Pathdisplay.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(rootPath)) // 0�̳� ��ĭ
            {
                MessageBox.Show("������ �������ּ���.", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            organizebtn.Enabled = false;
            progressBar1.Value = 0;

            try // �õ�
            {
                await OrganizeFoldersAsync(rootPath);
                MessageBox.Show("���� ������ �Ϸ�Ǿ����ϴ�.", "�Ϸ�", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) //���ܿ��� ĳġ
            {
                MessageBox.Show($"���� ���� �� ���� �߻�: {ex.Message}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally // �õ� ���������
            {
                organizebtn.Enabled = true;
                progressBar1.Value = 100;
            }
        }

        private async Task OrganizeFoldersAsync(string rootPath)
        {
            string[] directories = Directory.GetDirectories(rootPath); // ���丮 ���ͼ� �迭�� ����
            int totalDirectories = directories.Length; //���丮�迭�� ����
            int processedDirectories = 0; // ���൵ 0

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

                        if (!Directory.Exists(newCategoryPath)) // [ī�װ�] �̸��� ������ ��������������
                        {
                            Directory.CreateDirectory(newCategoryPath); // [ī�װ�] �̸��� ���� ���ο� ���� �ۼ�
                        }

                        string newPath = Path.Combine(newCategoryPath, dirName); // ��� ����

                        if (Directory.Exists(newPath)) // ���� [ī�װ�] �̸��� ������ ������
                        {
                            int counter = 1; // ī���� ���̰�
                            string tempPath = newPath;
                            while (Directory.Exists(tempPath)) // [ī�װ�] (����) �� �̸��� �õ��ؼ� ����
                            {
                                tempPath = Path.Combine(newCategoryPath, $"{dirName} ({counter})");
                                counter++;
                            }
                            newPath = tempPath; // Ȯ���� �̸��� ����
                        }

                        try
                        {
                            TryMoveDirectory(dir, newPath); // �̵��Լ� �õ�
                        }
                        catch (Exception ex) // ����
                        {
                            // ���� �α� �Ǵ� ó��
                            Console.WriteLine($"���� '{dirName}' �̵� �� ����: {ex.Message}");
                        }
                    }

                    // ����� ǥ��
                    processedDirectories++;
                    int progressPercentage = (int)((processedDirectories / (double)totalDirectories) * 100);
                    UpdateProgressBar(progressPercentage);
                }
            });

            await UpdateFolderListAsync(rootPath);
        }

        private bool TryMoveDirectory(string sourceDir, string destDir, int maxRetries = 3) // �ִ�õ�Ƚ�� 3���μ���
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try // �õ�
                {
                    Directory.Move(sourceDir, destDir);
                    return true; // ������ true ��ȯ
                }
                catch (IOException) // IO���ܹ߻���
                {
                    if (i == maxRetries - 1) throw; // throw : ���ܸ� ������ ����
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
            if (Directory.Exists(path)) // ��� ����
            {
                string[] directories = await Task.Run(() => Directory.GetDirectories(path));
                StringBuilder sb = new StringBuilder(); // ���ڿ��� ���۰���
                int foldercount = directories.Length;
                sb.AppendLine($"���� �� ���� ����: {foldercount}");
                foreach (string dir in directories)
                {
                    sb.AppendLine(new DirectoryInfo(dir).Name); // �����̸��� �� �ٿ� �߰�
                }
                UpdateFolderListBoxText(sb.ToString()); // ����Ʈ ������Ʈ
            }
            else
            {
                UpdateFolderListBoxText("���õ� ��ΰ� �������� �ʽ��ϴ�.");
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
            if (Pathdisplay.SelectedItem == null) // ���þȵ������
            {
                MessageBox.Show("������ �������ּ���.", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string selectedPath = Pathdisplay.SelectedItem.ToString();

            try
            {
                Process.Start("explorer.exe",selectedPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("������ �� �� �����ϴ�: " + ex.Message, "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OrganizeFolders(string rootPath) // �����Լ�
        {
            string[] directories = Directory.GetDirectories(rootPath); // rootPate �μ��� ��� ������
            int totalDirectories = directories.Length; // ���丮 �迭�� ����
            int processedDirectories = 0; // ���α׷��� ��


            foreach (string dir in directories) // directories �� dir �� ���ȭ�� ��� ���������� �ݺ�
            {
                string dirName = new DirectoryInfo(dir).Name; // ���丮�� �̸���ȯ
                Match match = Regex.Match(dirName, @"\[(.*?)\]"); // ���丮���� [] ���̿��ִ� �����ν�

                if (match.Success) // �����ϸ�
                {
                    string category = match.Groups[1].Value; // ���ȣ�� ������ ī�װ� �̸� ����
                    string categoryWithBrackets = $"[{category}]"; // �����̸�����
                    string newCategoryPath = Path.Combine(rootPath, categoryWithBrackets); // ��������

                    if (!Directory.Exists(newCategoryPath))
                    {
                        Directory.CreateDirectory(newCategoryPath);
                    }

                    string newPath = Path.Combine(newCategoryPath, dirName);

                    // �̹� ���� �̸��� ������ �����ϴ� ��� ó��
                    if (Directory.Exists(newPath))
                    {
                        int counter = 1;
                        string tempPath = newPath;
                        while (Directory.Exists(tempPath))
                        {
                            tempPath = Path.Combine(newCategoryPath, $"{dirName} ({counter})");
                            // [aa] �ߺ��� [aa] 1 �̷������� ����
                            counter++;
                        }
                        newPath = tempPath;
                    }
                    Trace.WriteLine(dirName);
                    Directory.Move(dir, newPath); // ���λ����� ��η� �̵�
                }

                // ���� ���� ������Ʈ
                processedDirectories++;
                int progressPercentage = (int)((processedDirectories / (double)totalDirectories) * 100);
                this.Invoke(new Action(() => progressBar1.Value = progressPercentage));
            }

            MessageBox.Show("���� ������ �Ϸ�Ǿ����ϴ�.", "�Ϸ�", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // �۾� �Ϸ� �� 100%�� ����
            this.Invoke(new Action(() => progressBar1.Value = 100));


        }

        private void SaveSettings() // ����
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
                string[] directories = Directory.GetDirectories(path); // path�� ���丮 ���� �� �迭�� ����
                StringBuilder sb = new StringBuilder(); // Append, Replace, Insert ������ ���氡���� ���ڿ������
                int foldercount = directories.Length;
                sb.AppendLine("���� �� ���� ����: " + foldercount); // AppendLine = ���ο� �� �߰�
                foreach (string dir in directories)
                {
                    sb.AppendLine(new DirectoryInfo(dir).Name); // ��θ� ���ο� �ٿ� �߰�
                }
                FolderListBox.Text = sb.ToString();
            }
            else
            {
                FolderListBox.Text = "���õ� ��ΰ� �������� �ʽ��ϴ�.";
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
