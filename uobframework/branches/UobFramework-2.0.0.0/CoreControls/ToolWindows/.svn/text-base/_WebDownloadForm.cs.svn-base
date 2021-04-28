using System;
using System.Drawing;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using UoB.Core;
using UoB.Core.Download;
using UoB.CoreControls.Documents;
using UoB.CoreControls.ToolWindows;

// original code found at
// http://www.thecodeproject.com/csharp/webdownload.asp#xx240293xx

namespace UoB.CoreControls.ToolWindows
{

	public delegate void FileDoneHandler( string Filename );


	/// <summary>
	/// Summary description for WebDownloadForm.
	/// </summary>
	/// 
	public class WebDownloadForm : System.Windows.Forms.UserControl, ITool
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox outputGroupBox;
		private System.Windows.Forms.Button downloadBtn;
		private System.Windows.Forms.Label downloadProgressLbl;
		private System.Windows.Forms.Label bytesDownloadedLbl;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.TextBox PDBID;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label_InfoDisplay;
		private System.Windows.Forms.GroupBox groupBox_DownloadType;
		private System.Windows.Forms.RadioButton radio_PDB1;
		private System.Windows.Forms.RadioButton radio_PDB2;
		private System.Windows.Forms.RadioButton radio_ASTRAL;

		// my members
		public event FileDoneHandler FileDoneCallback;
		private System.Windows.Forms.Label m_URL;
		private CoreIni m_UoBInfo;
		private string m_CacheDir;
		private string m_PDB1_Pre;
		private string m_PDB1_Post;
		private string m_PDB2_Pre;
		private string m_PDB2_Post;
		private string m_Astral_Pre;
		private string m_Astral_Post;

		public WebDownloadForm()
		{
			InitializeComponent();
			AssertCacheDir();
			Text = "Web Download";
			m_UoBInfo = CoreIni.Instance;
            getURLStrings();
			doConcat(this, null);
		}

		private void AssertCacheDir()
		{
			m_CacheDir = CoreIni.Instance.DefaultSharedPath + @"PDB/";
			if(!Directory.Exists(m_CacheDir))
			{
				Directory.CreateDirectory(m_CacheDir);
			}
		}

		private void getURLStrings()
		{
			getSingleURLString(
				"PDB1DL_PreString",
				"PDB1DL_PostString",
				out m_PDB1_Pre, 
				out m_PDB1_Post,
				@"http://www.rcsb.org/pdb/cgi/export.cgi/DAVEGetFile.pdb?format=PDB&pdbId=",
				@"&compression=None"
				);

			getSingleURLString(
				"PDB2DL_PreString",
				"PDB2DL_PostString",
				out m_PDB2_Pre, 
				out m_PDB2_Post,
				@"http://144.16.71.2/cgi-bin/pdbid.pl?pdbId=",
				@""
				);

			getSingleURLString(
				"AstralDL_PreString",
				"AstralDL_PostString",
				out m_Astral_Pre, 
				out m_Astral_Post,
				@"http://astral.berkeley.edu/pdbstyle.cgi?id=",
				@"&output=text"
				);
		}

		private void getSingleURLString( string preKey, string postKey, out string preValue, out string postValue, string defaultPreValue, string defaultPostValue )
		{
			if ( m_UoBInfo.ContainsKey( preKey ) )
			{
				preValue = m_UoBInfo.ValueOf( preKey );				
			}
			else
			{
				preValue = defaultPreValue;
				m_UoBInfo.AddDefinition( preKey, preValue );
			}

			if ( m_UoBInfo.ContainsKey( postKey ) )
			{
				postValue = m_UoBInfo.ValueOf( postKey );				
			}
			else
			{
				postValue = defaultPostValue;
				m_UoBInfo.AddDefinition( postKey, postValue );
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.outputGroupBox = new System.Windows.Forms.GroupBox();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.bytesDownloadedLbl = new System.Windows.Forms.Label();
			this.downloadProgressLbl = new System.Windows.Forms.Label();
			this.downloadBtn = new System.Windows.Forms.Button();
			this.PDBID = new System.Windows.Forms.TextBox();
			this.m_URL = new System.Windows.Forms.Label();
			this.label_InfoDisplay = new System.Windows.Forms.Label();
			this.groupBox_DownloadType = new System.Windows.Forms.GroupBox();
			this.radio_ASTRAL = new System.Windows.Forms.RadioButton();
			this.radio_PDB2 = new System.Windows.Forms.RadioButton();
			this.radio_PDB1 = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.outputGroupBox.SuspendLayout();
			this.groupBox_DownloadType.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// outputGroupBox
			// 
			this.outputGroupBox.Controls.Add(this.progressBar);
			this.outputGroupBox.Controls.Add(this.bytesDownloadedLbl);
			this.outputGroupBox.Controls.Add(this.downloadProgressLbl);
			this.outputGroupBox.Enabled = false;
			this.outputGroupBox.Location = new System.Drawing.Point(8, 264);
			this.outputGroupBox.Name = "outputGroupBox";
			this.outputGroupBox.Size = new System.Drawing.Size(160, 80);
			this.outputGroupBox.TabIndex = 2;
			this.outputGroupBox.TabStop = false;
			this.outputGroupBox.Text = "Current Progress";
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(8, 48);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(144, 24);
			this.progressBar.TabIndex = 0;
			// 
			// bytesDownloadedLbl
			// 
			this.bytesDownloadedLbl.Location = new System.Drawing.Point(8, 32);
			this.bytesDownloadedLbl.Name = "bytesDownloadedLbl";
			this.bytesDownloadedLbl.Size = new System.Drawing.Size(144, 23);
			this.bytesDownloadedLbl.TabIndex = 2;
			this.bytesDownloadedLbl.Text = "Bytes Downloaded";
			// 
			// downloadProgressLbl
			// 
			this.downloadProgressLbl.Location = new System.Drawing.Point(8, 16);
			this.downloadProgressLbl.Name = "downloadProgressLbl";
			this.downloadProgressLbl.Size = new System.Drawing.Size(144, 23);
			this.downloadProgressLbl.TabIndex = 1;
			this.downloadProgressLbl.Text = "# / #";
			// 
			// downloadBtn
			// 
			this.downloadBtn.Location = new System.Drawing.Point(8, 8);
			this.downloadBtn.Name = "downloadBtn";
			this.downloadBtn.Size = new System.Drawing.Size(80, 23);
			this.downloadBtn.TabIndex = 3;
			this.downloadBtn.Text = "Download";
			this.downloadBtn.Click += new System.EventHandler(this.downloadBtn_Click);
			// 
			// PDBID
			// 
			this.PDBID.Location = new System.Drawing.Point(96, 8);
			this.PDBID.Name = "PDBID";
			this.PDBID.Size = new System.Drawing.Size(72, 20);
			this.PDBID.TabIndex = 4;
			this.PDBID.Text = "1BW8";
			this.PDBID.TextChanged += new System.EventHandler(this.doConcat);
			// 
			// m_URL
			// 
			this.m_URL.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_URL.Location = new System.Drawing.Point(3, 16);
			this.m_URL.Name = "m_URL";
			this.m_URL.Size = new System.Drawing.Size(154, 93);
			this.m_URL.TabIndex = 5;
			// 
			// label_InfoDisplay
			// 
			this.label_InfoDisplay.BackColor = System.Drawing.SystemColors.Info;
			this.label_InfoDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_InfoDisplay.Location = new System.Drawing.Point(8, 352);
			this.label_InfoDisplay.Name = "label_InfoDisplay";
			this.label_InfoDisplay.Size = new System.Drawing.Size(160, 40);
			this.label_InfoDisplay.TabIndex = 6;
			this.label_InfoDisplay.Text = "InfoDisplay ...";
			// 
			// groupBox_DownloadType
			// 
			this.groupBox_DownloadType.Controls.Add(this.radio_ASTRAL);
			this.groupBox_DownloadType.Controls.Add(this.radio_PDB2);
			this.groupBox_DownloadType.Controls.Add(this.radio_PDB1);
			this.groupBox_DownloadType.Location = new System.Drawing.Point(8, 40);
			this.groupBox_DownloadType.Name = "groupBox_DownloadType";
			this.groupBox_DownloadType.Size = new System.Drawing.Size(160, 96);
			this.groupBox_DownloadType.TabIndex = 7;
			this.groupBox_DownloadType.TabStop = false;
			this.groupBox_DownloadType.Text = "Download Type";
			// 
			// radio_ASTRAL
			// 
			this.radio_ASTRAL.Location = new System.Drawing.Point(8, 64);
			this.radio_ASTRAL.Name = "radio_ASTRAL";
			this.radio_ASTRAL.Size = new System.Drawing.Size(144, 24);
			this.radio_ASTRAL.TabIndex = 2;
			this.radio_ASTRAL.Text = "ASTRAL";
			this.radio_ASTRAL.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
			// 
			// radio_PDB2
			// 
			this.radio_PDB2.Location = new System.Drawing.Point(8, 40);
			this.radio_PDB2.Name = "radio_PDB2";
			this.radio_PDB2.Size = new System.Drawing.Size(144, 24);
			this.radio_PDB2.TabIndex = 1;
			this.radio_PDB2.Text = "PDB Source 2";
			this.radio_PDB2.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
			// 
			// radio_PDB1
			// 
			this.radio_PDB1.Checked = true;
			this.radio_PDB1.Location = new System.Drawing.Point(8, 16);
			this.radio_PDB1.Name = "radio_PDB1";
			this.radio_PDB1.Size = new System.Drawing.Size(144, 24);
			this.radio_PDB1.TabIndex = 0;
			this.radio_PDB1.TabStop = true;
			this.radio_PDB1.Text = "PDB Source 1";
			this.radio_PDB1.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.m_URL);
			this.groupBox1.Location = new System.Drawing.Point(8, 144);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(160, 112);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "File To Download :";
			// 
			// WebDownloadForm
			// 
			this.Controls.Add(this.groupBox_DownloadType);
			this.Controls.Add(this.label_InfoDisplay);
			this.Controls.Add(this.PDBID);
			this.Controls.Add(this.downloadBtn);
			this.Controls.Add(this.outputGroupBox);
			this.Controls.Add(this.groupBox1);
			this.Name = "WebDownloadForm";
			this.Size = new System.Drawing.Size(176, 400);
			this.outputGroupBox.ResumeLayout(false);
			this.groupBox_DownloadType.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		private void DownloadProgressCallback ( int bytesSoFar, int totalBytes )
		{
			label_InfoDisplay.Text = "Download in progress ...";

			if ( totalBytes != -1 )
			{
				progressBar.Minimum = 0;
				progressBar.Maximum = totalBytes;
				progressBar.Value = bytesSoFar;
				downloadProgressLbl.Text = 
					bytesSoFar.ToString("#,##0") 
					+ " / " + 
					totalBytes.ToString("#,##0");
			}
			else
			{
				progressBar.Visible = false;
				downloadProgressLbl.Text = 
					bytesSoFar.ToString("#,##0") 
					+ " / Unknown";
			}
		}

		private void DownloadCompleteCallback ( byte[] dataDownloaded )
		{
			if ( !progressBar.Visible )
			{
				progressBar.Visible = true;
				progressBar.Minimum = 0;
				progressBar.Value = progressBar.Maximum = 1;
			}

			downloadBtn.Enabled = true;
			PDBID.Enabled = true;
			groupBox_DownloadType.Enabled = true;

			if ( dataDownloaded.Length != 0 )
			{
				FileDoneCallback( SaveFile(dataDownloaded, PDBID.Text + ".pdb") );
			}
			else
			{
				label_InfoDisplay.Text = "ERROR! - Downloaded size is 0 bytes !";
			}

		}

		private string SaveFile( byte[] dataDownloaded, string fileName )
		{
			Stream myStream;
			string fullName = m_CacheDir + fileName;
			myStream = File.OpenWrite(fullName);
			myStream.Write(dataDownloaded,0,dataDownloaded.Length);
			myStream.Close();
			return fullName;
		}

		private bool ValidateProteinID()
		{
			string IDToTest = PDBID.Text;

			if ( radio_PDB1.Checked == true || radio_PDB2.Checked == true )
			{
				if ( IDToTest.Length != 4 ) return false;
			}
			else
			if ( radio_ASTRAL.Checked == true )
			{
				if ( IDToTest.Length == 5 )
				{
					return true;
				}
				if ( IDToTest.Length != 7  ) 
				{
						return false;
				}
				else
				{
					if ( IDToTest[6] != '_' )
					{
						return false;
					}
				}
			}
			return true;
		}

		private void downloadBtn_Click(object sender, System.EventArgs e)
		{
			if ( !ValidateProteinID() )
			{
				MessageBox.Show("ID in the box is not valid for this download type");
				return;
			}

			foreach ( string theFile in Directory.GetFiles(m_CacheDir,"*.pdb"))
			{
				if(theFile == m_CacheDir + PDBID.Text + ".pdb")
				{

					if(	MessageBox.Show(
						this, 
						"File has already been downloaded and cached! Re-Download ?", 
						"User Confirmation ..",
						System.Windows.Forms.MessageBoxButtons.YesNo,
						System.Windows.Forms.MessageBoxIcon.Question) 
						== DialogResult.No) 
					{


						FileStream fileStream = File.OpenRead( theFile );
						Byte[] Buffer = new byte[fileStream.Length];
						fileStream.Read(Buffer,0,(int)fileStream.Length);
						fileStream.Close();
						FileDoneCallback( SaveFile(Buffer, PDBID.Text + ".pdb") );
						label_InfoDisplay.Text = "Opened a cached file. Waiting for next download request ...";
						return;
					} 
					else 
					{
						break;
					};
				}
			}

			if ( this.m_URL.Text != "" )
			{
				this.outputGroupBox.Enabled = true;
				groupBox_DownloadType.Enabled = false;

				this.progressBar.Minimum = 0;
				this.progressBar.Maximum = 0;
				this.progressBar.Value = 0;

				DownloadThread dl = new DownloadThread();
				dl.DownloadUrl = this.m_URL.Text;
				dl.CompleteCallback += new DownloadCompleteHandler( DownloadCompleteCallback );
				dl.ProgressCallback += new IntProgressEvent( DownloadProgressCallback );

				System.Threading.Thread t = new System.Threading.Thread( 
					new System.Threading.ThreadStart(
					dl.Download ));
				t.Start();
				label_InfoDisplay.Text = "Download being initiated";
				PDBID.Enabled = false;
				downloadBtn.Enabled = false;
			}
			else
			{
				MessageBox.Show("Please enter a path ..");
			}
		}

		private void doConcat(object sender, System.EventArgs e)
		{
			if ( radio_PDB1.Checked == true )
			{
				m_URL.Text = m_PDB1_Pre + PDBID.Text + m_PDB1_Post;
			}
			else
			if ( radio_PDB2.Checked == true )
			{
				m_URL.Text = m_PDB2_Pre + PDBID.Text + m_PDB2_Post;
			}
			else
			if ( radio_ASTRAL.Checked == true )
			{
				m_URL.Text = m_Astral_Pre + PDBID.Text + m_Astral_Post;
			}
		}

		#region ITool Members

		public void AttachToDocument(Document doc)
		{
			// None
		}

		#endregion

		private void radio_CheckedChanged(object sender, System.EventArgs e)
		{
			doConcat(this, null);		
		}

	}
}
