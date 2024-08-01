namespace FolderMove
{
    partial class FolderMove
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            browsebutton = new Button();
            Pathdisplay = new ComboBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            progressBar1 = new ProgressBar();
            tableLayoutPanel3 = new TableLayoutPanel();
            organizebtn = new Button();
            openbtn = new Button();
            FolderListBox = new TextBox();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.Controls.Add(browsebutton, 1, 0);
            tableLayoutPanel1.Controls.Add(Pathdisplay, 0, 0);
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(879, 34);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // browsebutton
            // 
            browsebutton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            browsebutton.Location = new Point(832, 4);
            browsebutton.Name = "browsebutton";
            browsebutton.Size = new Size(44, 26);
            browsebutton.TabIndex = 0;
            browsebutton.Text = "...";
            browsebutton.UseVisualStyleBackColor = true;
            browsebutton.Click += browsebutton_Click;
            // 
            // Pathdisplay
            // 
            Pathdisplay.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            Pathdisplay.FormattingEnabled = true;
            Pathdisplay.Location = new Point(3, 5);
            Pathdisplay.Name = "Pathdisplay";
            Pathdisplay.Size = new Size(823, 23);
            Pathdisplay.TabIndex = 1;
            Pathdisplay.SelectedIndexChanged += Pathdisplay_SelectedIndexChanged;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(progressBar1, 0, 1);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel3, 0, 2);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel1, 0, 0);
            tableLayoutPanel2.Controls.Add(FolderListBox, 0, 2);
            tableLayoutPanel2.Location = new Point(23, 28);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 4;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel2.Size = new Size(885, 604);
            tableLayoutPanel2.TabIndex = 2;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            progressBar1.Location = new Point(6, 43);
            progressBar1.Margin = new Padding(6, 3, 6, 3);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(873, 23);
            progressBar1.TabIndex = 3;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel3.ColumnCount = 3;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            tableLayoutPanel3.Controls.Add(organizebtn, 2, 0);
            tableLayoutPanel3.Controls.Add(openbtn, 1, 0);
            tableLayoutPanel3.Location = new Point(3, 567);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(879, 34);
            tableLayoutPanel3.TabIndex = 3;
            // 
            // organizebtn
            // 
            organizebtn.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            organizebtn.Location = new Point(792, 5);
            organizebtn.Name = "organizebtn";
            organizebtn.Size = new Size(84, 24);
            organizebtn.TabIndex = 0;
            organizebtn.Text = "정리하기";
            organizebtn.UseVisualStyleBackColor = true;
            organizebtn.Click += organizebtn_Click;
            // 
            // openbtn
            // 
            openbtn.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            openbtn.Location = new Point(702, 5);
            openbtn.Name = "openbtn";
            openbtn.Size = new Size(84, 24);
            openbtn.TabIndex = 0;
            openbtn.Text = "폴더열기";
            openbtn.UseVisualStyleBackColor = true;
            openbtn.Click += openbtn_Click;
            // 
            // FolderListBox
            // 
            FolderListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            FolderListBox.BackColor = SystemColors.Window;
            FolderListBox.Location = new Point(6, 73);
            FolderListBox.Margin = new Padding(6, 3, 6, 3);
            FolderListBox.Multiline = true;
            FolderListBox.Name = "FolderListBox";
            FolderListBox.ReadOnly = true;
            FolderListBox.ScrollBars = ScrollBars.Both;
            FolderListBox.Size = new Size(873, 488);
            FolderListBox.TabIndex = 2;
            FolderListBox.WordWrap = false;
            // 
            // FolderMove
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(931, 655);
            Controls.Add(tableLayoutPanel2);
            MinimumSize = new Size(300, 300);
            Name = "FolderMove";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private TableLayoutPanel tableLayoutPanel1;
        private Button browsebutton;
        private ComboBox Pathdisplay;
        private TableLayoutPanel tableLayoutPanel2;
        private Button organizebtn;
        private TextBox FolderListBox;
        private TableLayoutPanel tableLayoutPanel3;
        private Button openbtn;
        private ProgressBar progressBar1;
    }
}
