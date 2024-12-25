namespace EasyMultithreadingDownload
{
	partial class Main
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
			UrlBox = new TextBox();
			SaveDirBox = new TextBox();
			DirSelect = new Button();
			folderBrowserDialog1 = new FolderBrowserDialog();
			label1 = new Label();
			label2 = new Label();
			StartButton = new Button();
			DownloadProgressBar = new ProgressBar();
			ProgressText = new Label();
			gy = new Label();
			SuspendLayout();
			// 
			// UrlBox
			// 
			UrlBox.AllowDrop = true;
			UrlBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			UrlBox.Font = new Font("Microsoft YaHei UI", 10F);
			UrlBox.Location = new Point(65, 4);
			UrlBox.MaxLength = 99999;
			UrlBox.Multiline = true;
			UrlBox.Name = "UrlBox";
			UrlBox.PlaceholderText = "在此填入文件下载地址";
			UrlBox.ScrollBars = ScrollBars.Vertical;
			UrlBox.Size = new Size(365, 24);
			UrlBox.TabIndex = 0;
			UrlBox.WordWrap = false;
			UrlBox.DragDrop += UrlBox_DragDrop;
			UrlBox.DragEnter += UrlBox_DragEnter;
			// 
			// SaveDirBox
			// 
			SaveDirBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			SaveDirBox.Font = new Font("Microsoft YaHei UI", 10F);
			SaveDirBox.Location = new Point(65, 34);
			SaveDirBox.Name = "SaveDirBox";
			SaveDirBox.PlaceholderText = "在此填入文件保存路径";
			SaveDirBox.Size = new Size(296, 24);
			SaveDirBox.TabIndex = 1;
			// 
			// DirSelect
			// 
			DirSelect.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			DirSelect.Location = new Point(361, 35);
			DirSelect.Name = "DirSelect";
			DirSelect.Size = new Size(69, 23);
			DirSelect.TabIndex = 2;
			DirSelect.Text = "选择位置";
			DirSelect.UseVisualStyleBackColor = true;
			DirSelect.Click += DirSelect_Click;
			// 
			// folderBrowserDialog1
			// 
			folderBrowserDialog1.Description = "选择文件保存的位置";
			// 
			// label1
			// 
			label1.Anchor = AnchorStyles.Left;
			label1.AutoSize = true;
			label1.Font = new Font("Microsoft YaHei UI", 10F);
			label1.Location = new Point(-1, 7);
			label1.Name = "label1";
			label1.Size = new Size(68, 20);
			label1.TabIndex = 3;
			label1.Text = "下载地址:";
			// 
			// label2
			// 
			label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			label2.AutoSize = true;
			label2.Font = new Font("Microsoft YaHei UI", 10F);
			label2.Location = new Point(-1, 36);
			label2.Name = "label2";
			label2.Size = new Size(68, 20);
			label2.TabIndex = 4;
			label2.Text = "保存路径:";
			// 
			// StartButton
			// 
			StartButton.Anchor = AnchorStyles.Bottom;
			StartButton.Font = new Font("Microsoft YaHei UI", 11F);
			StartButton.Location = new Point(167, 64);
			StartButton.Name = "StartButton";
			StartButton.Size = new Size(101, 28);
			StartButton.TabIndex = 5;
			StartButton.Text = "下载";
			StartButton.UseVisualStyleBackColor = true;
			StartButton.Click += StartButton_Click;
			// 
			// DownloadProgressBar
			// 
			DownloadProgressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			DownloadProgressBar.Location = new Point(4, 61);
			DownloadProgressBar.Name = "DownloadProgressBar";
			DownloadProgressBar.Size = new Size(426, 20);
			DownloadProgressBar.TabIndex = 6;
			DownloadProgressBar.Visible = false;
			// 
			// ProgressText
			// 
			ProgressText.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			ProgressText.BackColor = Color.Transparent;
			ProgressText.Location = new Point(4, 81);
			ProgressText.Name = "ProgressText";
			ProgressText.Size = new Size(426, 20);
			ProgressText.TabIndex = 7;
			ProgressText.TextAlign = ContentAlignment.TopCenter;
			ProgressText.Visible = false;
			// 
			// gy
			// 
			gy.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			gy.AutoSize = true;
			gy.Location = new Point(398, 78);
			gy.Name = "gy";
			gy.Size = new Size(32, 17);
			gy.TabIndex = 8;
			gy.Text = "关于";
			gy.Click += Gy_Click;
			// 
			// Main
			// 
			AutoScaleDimensions = new SizeF(7F, 17F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(434, 98);
			Controls.Add(DownloadProgressBar);
			Controls.Add(ProgressText);
			Controls.Add(StartButton);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(DirSelect);
			Controls.Add(SaveDirBox);
			Controls.Add(UrlBox);
			Controls.Add(gy);
			Icon = (Icon)resources.GetObject("$this.Icon");
			MaximizeBox = false;
			MinimumSize = new Size(317, 137);
			Name = "Main";
			Text = "简易多线程下载器";
			FormClosing += Main_FormClosing;
			Load += Main_Load;
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private TextBox UrlBox;
		private TextBox SaveDirBox;
		private Button DirSelect;
		private FolderBrowserDialog folderBrowserDialog1;
		private Label label1;
		private Label label2;
		private Button StartButton;
		private ProgressBar DownloadProgressBar;
		private Label ProgressText;
		private Label gy;
	}
}
