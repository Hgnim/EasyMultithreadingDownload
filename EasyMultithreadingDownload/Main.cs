using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Xml;

namespace EasyMultithreadingDownload
{
	public partial class Main : Form
	{
		static readonly string version = "1.1.0.20240418";
		public Main()
		{
			InitializeComponent();
		}

		static class Config
		{
			static public readonly string tempPath = Path.GetTempPath() + "emd/";
			static public readonly string dataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/EasyMultithreadingDownload/";
			static public readonly string dataFile = dataPath + "data.xml";
			static public readonly string aria2cEXE = tempPath + "aria2c.exe";
		}
		static class DataConfig
		{
			//public static bool shownMoreDownloads = true;
		}
		#region 主窗体事件处理
		private void Main_Load(object sender, EventArgs e)
		{
			if (!Directory.Exists(Config.tempPath)) Directory.CreateDirectory(Config.tempPath);
			if (!Directory.Exists(Config.dataPath)) Directory.CreateDirectory(Config.dataPath);
			if (!File.Exists(Config.aria2cEXE))
				OutputResource("aria2c.exe");
			if (!File.Exists(Config.dataFile))
			{
				XmlTextWriter xmlWriter = new(Config.dataFile, System.Text.Encoding.GetEncoding("utf-8")) { Formatting = System.Xml.Formatting.Indented };

				xmlWriter.WriteRaw("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
				xmlWriter.WriteStartElement("EasyMultithreadingDownload_Data");

				xmlWriter.WriteStartElement("windows");
				xmlWriter.WriteAttributeString("width", "-1");
				xmlWriter.WriteAttributeString("height", "-1");
				xmlWriter.WriteAttributeString("x", "-1");
				xmlWriter.WriteAttributeString("y", "-1");
				xmlWriter.WriteEndElement();

				/*xmlWriter.WriteStartElement("shown");
				xmlWriter.WriteAttributeString("more_downloads", "-1");
				xmlWriter.WriteEndElement();*/
				xmlWriter.WriteStartElement("more_data");
				xmlWriter.WriteAttributeString("save_dir_path", "null");
				xmlWriter.WriteEndElement();

				xmlWriter.WriteFullEndElement();
				xmlWriter.Close();

				SaveDirBox.Text = KnownFolders.Downloads.Path;
			}
			else
			{
				XmlDocument xmlDoc = new();
				XmlNode xmlRoot = xmlDoc.SelectSingleNode("null")!;
				xmlDoc.Load(Config.dataFile);
				xmlRoot = xmlDoc.SelectSingleNode("EasyMultithreadingDownload_Data")!;
				XmlNodeList xmlNL = xmlRoot.ChildNodes;
				foreach (XmlNode xn in xmlNL)
				{
					XmlElement xmlE = (XmlElement)xn;
					switch (xmlE.Name)
					{
						case "windows":
							int[] size = [int.Parse(xmlE.GetAttribute("width")), int.Parse(xmlE.GetAttribute("height"))];
							int[] loc = [int.Parse(xmlE.GetAttribute("x")), int.Parse(xmlE.GetAttribute("y"))];
							if (size[0] >= this.MinimumSize.Width && size[1] >= this.MinimumSize.Height /*&&
								size[0] <= this.MaximumSize.Width && size[1] <= this.MaximumSize.Height*/)
								this.Size = new Size(size[0], size[1]);
							if (loc[0] >= 0 && loc[1] >= 0 &&
								loc[0] < Screen.PrimaryScreen!.Bounds.Width && loc[1] < Screen.PrimaryScreen.Bounds.Height)
								this.Location = new Point(loc[0], loc[1]);
							break;
						/*case "shown":
							if (xmlE.GetAttribute("more_downloads") == "0")
								DataConfig.shownMoreDownloads = false;
							else
								DataConfig.shownMoreDownloads = true;
							break;*/
						case "more_data":
								 SaveDirBox.Text = xmlE.GetAttribute("save_dir_path");break;
							
					}
				}
			}

			//SaveDirBox.Text = KnownFolders.Downloads.Path;
			Debug.WriteLine("started");
		}
		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			Data_Save();
		}
		#endregion
		private void DirSelect_Click(object sender, EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = SaveDirBox.Text;

			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)//用户点确认件才保存选择的路径数据
			{
				SaveDirBox.Text = folderBrowserDialog1.SelectedPath;
			}
		}

		private void StartButton_Click(object sender, EventArgs e)
		{
			if (!Directory.Exists(SaveDirBox.Text))
			{
				MessageBox.Show("保存路径错误！", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			UrlBox.Enabled = false;
			SaveDirBox.Enabled = false;
			DirSelect.Enabled = false;
			gy.Visible = false;
			StartButton.Visible = false;
			ProgressText.Visible = true;
			DownloadProgressBar.Visible = true;
			TaskbarProgressbar.TaskbarManager.SetProgressState(TaskbarProgressbar.TaskbarProgressBarState.Normal);

			List<int> overCode = [];//0;
			List<string> outfilePath = [];//"--";
			List<string> urlList = []; int fileNumProgress;
			Thread downloadThread = new(() =>
			{
				List<int> outfilePathCode = [];//-1;
				try
				{
					if (UrlBox.Text.IndexOf("\r\n") != -1)
					{
						string[] urls = UrlBox.Text.Split("\r\n");
						for(int i = 0; i < urls.Length; i++)
						{
							if (urls[i] != "")
							{
								urlList.Add(urls[i]);
							}
						}
					}else urlList.Add(UrlBox.Text);

					for (fileNumProgress = 0; fileNumProgress < urlList.Count; fileNumProgress++) {
						try
						{
							overCode.Add(0);
							outfilePathCode.Add(-1);
							outfilePath.Add("--");
							Process process = new()
							{
								StartInfo = new ProcessStartInfo
								{
									FileName = Config.aria2cEXE,
									Arguments = " -x 16 -s 32 -d \"" + SaveDirBox.Text + "\" \"" + urlList[fileNumProgress] + "\"",
									UseShellExecute = false,
									RedirectStandardOutput = true,
									CreateNoWindow = true
								}
							};

							process.Start();
							{
								string outputLine;


								while (!process.StandardOutput.EndOfStream)
								{
									outputLine = process.StandardOutput.ReadLine()!;
									Debug.WriteLine(outputLine);
									switch (outputLine)
									{
										case "Status Legend:":
											overCode[fileNumProgress] = -1;
											goto restart;
										case "Download Results:":
											outfilePathCode[fileNumProgress] = 0;
											goto restart;
									}
									if (overCode[fileNumProgress] == -1)
									{
										if (outputLine.IndexOf("OK") != -1)
											overCode[fileNumProgress] = 1;
									}
									else if (outfilePathCode[fileNumProgress] > -1 && outfilePathCode[fileNumProgress] < 3)
									{
										if (outfilePathCode[fileNumProgress] == 2)
										{
											outfilePath[fileNumProgress] = "";
											string[] sp = outputLine.Split("|");
											for (int i = 3; i < sp.Length; i++)
												outfilePath[fileNumProgress] += sp[i];
										}
										outfilePathCode[fileNumProgress]++;
									}
									else if (outputLine!.IndexOf("CN:") != -1 && outputLine.IndexOf("DL:") != -1 && outputLine.IndexOf("]") != -1)
									{
										Thread uiThread = new(new ParameterizedThreadStart(uiChange!));
										void uiChange(object outputStr)
										{
											string o = outputStr.ToString()!.Split("]")[0];
											string[] s = o.Split(" ");
											string time = "-";
											int progressInt;
											try
											{
												progressInt=int.Parse(s[1].Split("(")[1].Split("%")[0]);
											}
											catch { progressInt= DownloadProgressBar.Value; }

											if (outputStr.ToString()!.IndexOf("ETA") != -1)
												time = s[4].Split(":")[1];
											this.Invoke(new Action(() =>
											{
												ProgressText.Text =
													"文件数:" + (fileNumProgress + 1).ToString() + "/" + urlList.Count.ToString() +
												  "  进度:" + s[1] +
												  "  速度:" + s[3].Split(":")[1] +
												"/s  剩余时间:" + time;
												DownloadProgressBar.Value = progressInt;
											}));
											TaskbarProgressbar.TaskbarManager.SetProgressValue(progressInt, 100);
										}
										uiThread.Start(outputLine);
									}
restart:;
								}
							}
						}
						catch { overCode[fileNumProgress] = -2; }
					}
				}
				catch { overCode.Add(-3);//初始错误代号
										 }
			});
			downloadThread.Start();
			while (downloadThread.IsAlive)
			{
				Application.DoEvents();
			}

			#region 信息输出
			if (overCode[0] == -3)
				failOutput();
			else
			{
				if (urlList.Count == 1)
				{
					if (overCode[0] == 1)
					{
						ProgressText.Text = "已完成!";
						DownloadProgressBar.Value = 100;
						TaskbarProgressbar.TaskbarManager.SetProgressValue(100, 100);
						MessageBox.Show("下载完成!文件保存于" + outfilePath[0], this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					else failOutput();
				}
				else
				{
					ProgressText.Text = "批量下载已完成!";
					DownloadProgressBar.Value = 100;
					string outputStr="批量下载已完成!";
					for(int i = 0;i<urlList.Count; i++)
					{
						outputStr += "\r\n";
						if (overCode[i] == 1) outputStr += "[下载成功] " + outfilePath[i];
						else outputStr += "[下载失败] " + urlList[i];//outfilePath[i] ;
					}
					MessageBox.Show(outputStr,this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			void failOutput()
			{
				ProgressText.Text = "失败!";
				TaskbarProgressbar.TaskbarManager.SetProgressState(TaskbarProgressbar.TaskbarProgressBarState.Error);
				MessageBox.Show("下载失败!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			#endregion

			
			//UrlBox.Clear();
			UrlBox.Enabled = true;
			UrlBox.Focus();UrlBox.SelectAll();
			SaveDirBox.Enabled = true;
			DirSelect.Enabled = true;
			ProgressText.Visible = false;
			DownloadProgressBar.Visible = false;
			TaskbarProgressbar.TaskbarManager.SetProgressState(TaskbarProgressbar.TaskbarProgressBarState.NoProgress);
			ProgressText.Text = null;
			DownloadProgressBar.Value = 0;
			TaskbarProgressbar.TaskbarManager.SetProgressValue(0, 100);
			StartButton.Visible = true;
			gy.Visible = true;
		}

		#region 信息拖拽处理
		private void UrlBox_DragEnter(object sender, DragEventArgs e)
		{			
			if (e.Data!.GetDataPresent(DataFormats.Text))//验证拖拽到数据格式，只允许拖拽链接文本
				e.Effect = DragDropEffects.Link;
			else
				e.Effect = DragDropEffects.None;
		}

		private void UrlBox_DragDrop(object sender, DragEventArgs e)
		{
			if (UrlBox.Text != "") UrlBox.Text += "\r\n";
			UrlBox.Text += e.Data!.GetData(DataFormats.Text)!.ToString();

		}
		#endregion

		private void Gy_Click(object sender, EventArgs e)
		{
			MessageBox.Show("程序名:EasyMultithreadingDownload(EMD)" +
						"\r\n别名:简易多线程下载器" +
						"\r\n版本:V" + version +
						"\r\nCopyright (C) 2024 Hgnim, All rights reserved." +
						"\r\nGithub: https://github.com/Hgnim/EasyMultithreadingDownload", "关于");
		}


		static void OutputResource(String path)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			string resPath = $"{assembly.FullName!.Split(',')[0]}.{path}";
			Stream stream = assembly.GetManifestResourceStream(resPath)!;
			Stream outFile = File.Create(Config.tempPath + path);
			stream.CopyTo(outFile);
			outFile.Close();
			stream.Close();
		}
		void Data_Save()
		{
			XmlDocument xmlDoc = new();
			XmlNodeList xmlNL;
			XmlElement xmlEle;
			xmlDoc.Load(Config.dataFile);
			xmlNL = xmlDoc.SelectSingleNode("EasyMultithreadingDownload_Data")!.ChildNodes;
			foreach (XmlNode xn in xmlNL)
			{
				xmlEle = (XmlElement)xn;
				switch (xmlEle.Name)
				{
					case "windows":
						xmlEle.SetAttribute("width", this.Size.Width.ToString());
						xmlEle.SetAttribute("height", this.Size.Height.ToString());
						xmlEle.SetAttribute("x", this.Location.X.ToString());
						xmlEle.SetAttribute("y", this.Location.Y.ToString());
						break;
						case "more_data":
						xmlEle.SetAttribute("save_dir_path", SaveDirBox.Text);break;
						/*case "shown":
							xmlEle.SetAttribute("more_downloads", Convert.ToInt32(DataConfig.shownMoreDownloads).ToString());
							break;*/
				}
			}
			xmlDoc.Save(Config.dataFile);

		}
	}
	public static class TaskbarProgressbar
	{
		/// <summary>
		/// Represents an instance of the Windows taskbar
		/// </summary>
		public static class TaskbarManager
		{
			/// <summary>
			/// Sets the handle of the window whose taskbar button will be used
			/// to display progress.
			/// </summary>
			private static IntPtr ownerHandle = IntPtr.Zero;

			static TaskbarManager()
			{
				var currentProcess = Process.GetCurrentProcess();
				if (currentProcess != null && currentProcess.MainWindowHandle != IntPtr.Zero)
					ownerHandle = currentProcess.MainWindowHandle;
			}

			/// <summary>
			/// Indicates whether this feature is supported on the current platform.
			/// </summary>
			private static bool IsPlatformSupported
			{
				get { return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.CompareTo(new Version(6, 1)) >= 0; }
			}

			/// <summary>
			/// Displays or updates a progress bar hosted in a taskbar button of the main application window 
			/// to show the specific percentage completed of the full operation.
			/// </summary>
			/// <param name="currentValue">An application-defined value that indicates the proportion of the operation that has been completed at the time the method is called.</param>
			/// <param name="maximumValue">An application-defined value that specifies the value currentValue will have when the operation is complete.</param>
			public static void SetProgressValue(int currentValue, int maximumValue)
			{
				if (IsPlatformSupported && ownerHandle != IntPtr.Zero)
					TaskbarList.Instance.SetProgressValue(
						ownerHandle,
						Convert.ToUInt32(currentValue),
						Convert.ToUInt32(maximumValue));
			}

			/// <summary>
			/// Displays or updates a progress bar hosted in a taskbar button of the given window handle 
			/// to show the specific percentage completed of the full operation.
			/// </summary>
			/// <param name="windowHandle">The handle of the window whose associated taskbar button is being used as a progress indicator.
			/// This window belong to a calling process associated with the button's application and must be already loaded.</param>
			/// <param name="currentValue">An application-defined value that indicates the proportion of the operation that has been completed at the time the method is called.</param>
			/// <param name="maximumValue">An application-defined value that specifies the value currentValue will have when the operation is complete.</param>
			public static void SetProgressValue(int currentValue, int maximumValue, IntPtr windowHandle)
			{
				if (IsPlatformSupported)
					TaskbarList.Instance.SetProgressValue(
						windowHandle,
						Convert.ToUInt32(currentValue),
						Convert.ToUInt32(maximumValue));
			}

			/// <summary>
			/// Sets the type and state of the progress indicator displayed on a taskbar button of the main application window.
			/// </summary>
			/// <param name="state">Progress state of the progress button</param>
			public static void SetProgressState(TaskbarProgressBarState state)
			{
				if (IsPlatformSupported && ownerHandle != IntPtr.Zero)
					TaskbarList.Instance.SetProgressState(ownerHandle, (TaskbarProgressBarStatus)state);
			}

			/// <summary>
			/// Sets the type and state of the progress indicator displayed on a taskbar button 
			/// of the given window handle 
			/// </summary>
			/// <param name="windowHandle">The handle of the window whose associated taskbar button is being used as a progress indicator.
			/// This window belong to a calling process associated with the button's application and must be already loaded.</param>
			/// <param name="state">Progress state of the progress button</param>
			public static void SetProgressState(TaskbarProgressBarState state, IntPtr windowHandle)
			{
				if (IsPlatformSupported)
					TaskbarList.Instance.SetProgressState(windowHandle, (TaskbarProgressBarStatus)state);
			}
		}


		/// <summary>
		/// Represents the thumbnail progress bar state.
		/// </summary>
		public enum TaskbarProgressBarState
		{
			/// <summary>
			/// No progress is displayed.
			/// </summary>
			NoProgress = 0,

			/// <summary>
			/// The progress is indeterminate (marquee).
			/// </summary>
			Indeterminate = 0x1,

			/// <summary>
			/// Normal progress is displayed.
			/// </summary>
			Normal = 0x2,

			/// <summary>
			/// An error occurred (red).
			/// </summary>
			Error = 0x4,

			/// <summary>
			/// The operation is paused (yellow).
			/// </summary>
			Paused = 0x8
		}

		/// <summary>
		/// Provides internal access to the functions provided by the ITaskbarList4 interface,
		/// without being forced to refer to it through another singleton.
		/// </summary>
		internal static class TaskbarList
		{
			private static object _syncLock = new object();

			private static ITaskbarList4? _taskbarList;
			internal static ITaskbarList4 Instance
			{
				get
				{
					if (_taskbarList == null)
					{
						lock (_syncLock)
						{
							if (_taskbarList == null)
							{
								_taskbarList = (ITaskbarList4)new CTaskbarList();
								_taskbarList.HrInit();
							}
						}
					}

					return _taskbarList;
				}
			}
		}

		[GuidAttribute("56FDF344-FD6D-11d0-958A-006097C9A090")]
		[ClassInterfaceAttribute(ClassInterfaceType.None)]
		[ComImportAttribute()]
		internal class CTaskbarList { }


		[ComImportAttribute()]
		[GuidAttribute("c43dc798-95d1-4bea-9030-bb99e2983a1a")]
		[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface ITaskbarList4
		{
			// ITaskbarList
			[PreserveSig]
			void HrInit();
			[PreserveSig]
			void AddTab(IntPtr hwnd);
			[PreserveSig]
			void DeleteTab(IntPtr hwnd);
			[PreserveSig]
			void ActivateTab(IntPtr hwnd);
			[PreserveSig]
			void SetActiveAlt(IntPtr hwnd);

			// ITaskbarList2
			[PreserveSig]
			void MarkFullscreenWindow(
				IntPtr hwnd,
				[MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

			// ITaskbarList3
			[PreserveSig]
			void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
			[PreserveSig]
			void SetProgressState(IntPtr hwnd, TaskbarProgressBarStatus tbpFlags);
			[PreserveSig]
			void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
			[PreserveSig]
			void UnregisterTab(IntPtr hwndTab);
			[PreserveSig]
			void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
			[PreserveSig]
			void SetTabActive(IntPtr hwndTab, IntPtr hwndInsertBefore, uint dwReserved);
			[PreserveSig]
			HResult ThumbBarAddButtons(
				IntPtr hwnd,
				uint cButtons,
				[MarshalAs(UnmanagedType.LPArray)] ThumbButton[] pButtons);
			[PreserveSig]
			HResult ThumbBarUpdateButtons(
				IntPtr hwnd,
				uint cButtons,
				[MarshalAs(UnmanagedType.LPArray)] ThumbButton[] pButtons);
			[PreserveSig]
			void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
			[PreserveSig]
			void SetOverlayIcon(
			  IntPtr hwnd,
			  IntPtr hIcon,
			  [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);
			[PreserveSig]
			void SetThumbnailTooltip(
				IntPtr hwnd,
				[MarshalAs(UnmanagedType.LPWStr)] string pszTip);
			[PreserveSig]
			void SetThumbnailClip(
				IntPtr hwnd,
				IntPtr prcClip);

			// ITaskbarList4
			void SetTabProperties(IntPtr hwndTab, SetTabPropertiesOption stpFlags);
		}

		internal enum TaskbarProgressBarStatus
		{
			NoProgress = 0,
			Indeterminate = 0x1,
			Normal = 0x2,
			Error = 0x4,
			Paused = 0x8
		}

		internal enum ThumbButtonMask
		{
			Bitmap = 0x1,
			Icon = 0x2,
			Tooltip = 0x4,
			THB_FLAGS = 0x8
		}

		[Flags]
		internal enum ThumbButtonOptions
		{
			Enabled = 0x00000000,
			Disabled = 0x00000001,
			DismissOnClick = 0x00000002,
			NoBackground = 0x00000004,
			Hidden = 0x00000008,
			NonInteractive = 0x00000010
		}

		internal enum SetTabPropertiesOption
		{
			None = 0x0,
			UseAppThumbnailAlways = 0x1,
			UseAppThumbnailWhenActive = 0x2,
			UseAppPeekAlways = 0x4,
			UseAppPeekWhenActive = 0x8
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		internal struct ThumbButton
		{
			/// <summary>
			/// WPARAM value for a THUMBBUTTON being clicked.
			/// </summary>
			internal const int Clicked = 0x1800;

			[MarshalAs(UnmanagedType.U4)]
			internal ThumbButtonMask Mask;
			internal uint Id;
			internal uint Bitmap;
			internal IntPtr Icon;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			internal string Tip;
			[MarshalAs(UnmanagedType.U4)]
			internal ThumbButtonOptions Flags;
		}

		/// <summary>
		/// HRESULT Wrapper
		/// </summary>
		public enum HResult
		{
			/// <summary>
			/// S_OK
			/// </summary>
			Ok = 0x0000,

			/// <summary>
			/// S_FALSE
			/// </summary>
			False = 0x0001,

			/// <summary>
			/// E_INVALIDARG
			/// </summary>
			InvalidArguments = unchecked((int)0x80070057),

			/// <summary>
			/// E_OUTOFMEMORY
			/// </summary>
			OutOfMemory = unchecked((int)0x8007000E),

			/// <summary>
			/// E_NOINTERFACE
			/// </summary>
			NoInterface = unchecked((int)0x80004002),

			/// <summary>
			/// E_FAIL
			/// </summary>
			Fail = unchecked((int)0x80004005),

			/// <summary>
			/// E_ELEMENTNOTFOUND
			/// </summary>
			ElementNotFound = unchecked((int)0x80070490),

			/// <summary>
			/// TYPE_E_ELEMENTNOTFOUND
			/// </summary>
			TypeElementNotFound = unchecked((int)0x8002802B),

			/// <summary>
			/// NO_OBJECT
			/// </summary>
			NoObject = unchecked((int)0x800401E5),

			/// <summary>
			/// Win32 Error code: ERROR_CANCELLED
			/// </summary>
			Win32ErrorCanceled = 1223,

			/// <summary>
			/// ERROR_CANCELLED
			/// </summary>
			Canceled = unchecked((int)0x800704C7),

			/// <summary>
			/// The requested resource is in use
			/// </summary>
			ResourceInUse = unchecked((int)0x800700AA),

			/// <summary>
			/// The requested resources is read-only.
			/// </summary>
			AccessDenied = unchecked((int)0x80030005)
		}
	}
}
