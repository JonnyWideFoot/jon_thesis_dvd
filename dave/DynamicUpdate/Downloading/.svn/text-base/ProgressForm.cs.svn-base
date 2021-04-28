using System;
using System.Drawing;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace UoB.DynamicUpdateEngine.Downloading
{

	/// <summary>
	/// Summary description for WebDownloadForm.
	/// </summary>
	/// 
	public class ProgressForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox outputGroupBox;
		private System.Windows.Forms.Label downloadProgressLbl;
		private System.Windows.Forms.Label bytesDownloadedLbl;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox m_Lines;
		private System.Windows.Forms.Label m_URL;
		private ArrayList m_OutputLines;

		public ProgressForm()
		{
			InitializeComponent();
			m_OutputLines = new ArrayList();
			SetControlState( false );
			progressBar.Minimum = 0;
			progressBar.Maximum = 0;
			progressBar.Value = 0;
		}


		public void SetControlState( bool enable )
		{
			for( int i = 0; i < Controls.Count; i++ )
			{
				Controls[i].Enabled = enable;
			}	
			m_Lines.Enabled = true;										 
		}

		public void SetProgress( int bytesSoFar, int totalBytes )
		{
			if ( totalBytes != -1 )
			{
				progressBar.Visible = true;
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

		public void SetReportText( string text )
		{
			m_OutputLines.Insert(0,text);
			m_Lines.Lines = (string[])m_OutputLines.ToArray(typeof(string));
		}

		public void SetURLText( string text )
		{
			m_URL.Text = text;
			m_OutputLines.Insert(0,text);
			m_Lines.Lines = (string[])m_OutputLines.ToArray(typeof(string));
		}

		public void SetComplete()
		{
			if ( !progressBar.Visible )
			{
				// set to 100%
				progressBar.Visible = true;
				progressBar.Minimum = 0;
				progressBar.Value = progressBar.Maximum = 1;
			}
		}


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
			this.m_URL = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.m_Lines = new System.Windows.Forms.TextBox();
			this.outputGroupBox.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// outputGroupBox
			// 
			this.outputGroupBox.Controls.Add(this.progressBar);
			this.outputGroupBox.Controls.Add(this.bytesDownloadedLbl);
			this.outputGroupBox.Controls.Add(this.downloadProgressLbl);
			this.outputGroupBox.Enabled = false;
			this.outputGroupBox.Location = new System.Drawing.Point(8, 56);
			this.outputGroupBox.Name = "outputGroupBox";
			this.outputGroupBox.Size = new System.Drawing.Size(408, 80);
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
			this.progressBar.Size = new System.Drawing.Size(392, 24);
			this.progressBar.TabIndex = 0;
			// 
			// bytesDownloadedLbl
			// 
			this.bytesDownloadedLbl.Location = new System.Drawing.Point(8, 32);
			this.bytesDownloadedLbl.Name = "bytesDownloadedLbl";
			this.bytesDownloadedLbl.Size = new System.Drawing.Size(144, 23);
			this.bytesDownloadedLbl.TabIndex = 2;
			this.bytesDownloadedLbl.Text = "Bytes";
			// 
			// downloadProgressLbl
			// 
			this.downloadProgressLbl.Location = new System.Drawing.Point(8, 16);
			this.downloadProgressLbl.Name = "downloadProgressLbl";
			this.downloadProgressLbl.Size = new System.Drawing.Size(328, 23);
			this.downloadProgressLbl.TabIndex = 1;
			this.downloadProgressLbl.Text = "# / #";
			// 
			// m_URL
			// 
			this.m_URL.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_URL.Location = new System.Drawing.Point(3, 16);
			this.m_URL.Name = "m_URL";
			this.m_URL.Size = new System.Drawing.Size(402, 21);
			this.m_URL.TabIndex = 5;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.m_URL);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(408, 40);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "File To Download :";
			// 
			// m_Lines
			// 
			this.m_Lines.Location = new System.Drawing.Point(8, 144);
			this.m_Lines.Multiline = true;
			this.m_Lines.Name = "m_Lines";
			this.m_Lines.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.m_Lines.Size = new System.Drawing.Size(408, 176);
			this.m_Lines.TabIndex = 9;
			this.m_Lines.Text = "";
			this.m_Lines.WordWrap = false;
			// 
			// ProgressForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(424, 325);
			this.Controls.Add(this.m_Lines);
			this.Controls.Add(this.outputGroupBox);
			this.Controls.Add(this.groupBox1);
			this.Name = "ProgressForm";
			this.Text = "Dynamic Update";
			this.outputGroupBox.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
