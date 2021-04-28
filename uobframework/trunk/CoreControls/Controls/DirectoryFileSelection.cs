using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Text;

using UoB.Core;

namespace UoB.CoreControls.Controls
{
	/// <summary>
	/// Summary description for DirectoryFileSelection.
	/// </summary>
	public class DirectoryFileSelection : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Button button_Previous;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.Button button_Next;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button button_SelDir;
		private System.Windows.Forms.Label label_Report;
		private System.Windows.Forms.TextBox text_Filter;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;

		public event UpdateEvent ActiveFileChange;
		public event UpdateEvent ActiveDirChange;

		private StringBuilder m_LabelBuilder = null;

		private DirectoryInfo m_DI = null;
		private FileInfo[] m_FIList = null;
		private FileInfo m_CurrentFile = null;
		
		private int m_Index = -1;
		private bool num_Index_Setup = false;
		private CoreIni m_UoBIni = null;
		private System.Windows.Forms.NumericUpDown num_Index;
		private static readonly string iniKey = "BrowseDir";

		public DirectoryFileSelection()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			folderBrowserDialog.ShowNewFolderButton = false;
			
			try
			{
				m_UoBIni = CoreIni.Instance;
				if ( m_UoBIni.ContainsKey( iniKey ) )
				{
					folderBrowserDialog.SelectedPath = m_UoBIni.ValueOf( iniKey );				
				}
				else
				{
					m_UoBIni.AddDefinition( iniKey, @"c:\" );
				}
			}
			catch
			{
				// dont really care
			}

			ActiveDirChange = new UpdateEvent( UpdateControl );
			ActiveFileChange = new UpdateEvent( UpdateControl );

			m_LabelBuilder = new StringBuilder();
		}

		public void ShowDirectorySelection()
		{
			if( DialogResult.OK == folderBrowserDialog.ShowDialog( this.Parent ) )
			{
				CurrentDirectory = new DirectoryInfo( folderBrowserDialog.SelectedPath );
				try
				{
					m_UoBIni.AddDefinition( iniKey, folderBrowserDialog.SelectedPath );
				}
				catch
				{
					// dont care
				}
			}
		}

		public DirectoryInfo CurrentDirectory
		{
			get
			{
				return m_DI;
			}
			set
			{
                m_DI = value;
				if( m_DI != null && !m_DI.Exists )
				{
					// we can cope with a null, but the error should be flagged
					throw new Exception("The directory specified does not exist");
				}
				ReobtainFileList();		
				ActiveDirChange();
			}
		}

		public void ReobtainFileList()
		{
			if( m_DI == null )
			{
				m_FIList = null;
				FileIndex = -1;
				return;
			}

			// ArrayList : Hack because i cant get GetFiles() to do an OR filter - how ??
			string[] filters = text_Filter.Text.Split(','); 
			ArrayList ar = new ArrayList();
			for( int i = 0; i < filters.Length; i++ )
			{
				ar.AddRange( m_DI.GetFiles("*." + filters[i]) );
			}
			m_FIList = (FileInfo[]) ar.ToArray(typeof(FileInfo));	

			if( m_FIList.Length > 0 )
			{
				FileIndex = 0; // "external" index set causes correct functions to fire
			}
			else
			{
				FileIndex = -1; // "external" index set causes correct functions to fire
			}	
		}

		public FileInfo[] CurrentFileList
		{
			get
			{
				return m_FIList;
			}
		}

		public FileInfo CurrentFile
		{
			get
			{
				return m_CurrentFile;
			}
		}

		public int FileIndex
		{
			get
			{
				return m_Index;
			}
			set
			{
				if( m_FIList == null )
				{
					value = -1; // anything else doesnt make sense
				}
				if( value == -1 )
				{
					// the null state
					m_CurrentFile = null;
					m_Index = -1;
					UpdateControl();
					ActiveFileChange();

					num_Index_Setup = true; // stop event firing, see event
						num_Index.Enabled = false;
						num_Index.Minimum = 0;
						num_Index.Value = 0;
						num_Index.Maximum = 0;
					num_Index_Setup = false;
				}
				else if( value < 0 || value >= m_FIList.Length )
				{
					throw new Exception("The index given is outside the available bounds");
				}
				else
				{
					m_CurrentFile = m_FIList[value];
					m_Index = value;
					UpdateControl();
					ActiveFileChange();

					num_Index_Setup = true; // stop event firing, see event
						num_Index.Enabled = true;
						num_Index.Minimum = 1;
						num_Index.Value = value + 1;
						num_Index.Maximum = m_FIList.Length;
					num_Index_Setup = false;
				}				
			}
		}


		private void UpdateControl()
		{
			m_LabelBuilder.Remove(0,m_LabelBuilder.Length);

			// set up the label
			if( m_DI == null )
			{
				m_LabelBuilder.Append( "No Directory Selected" );
				button_Previous.Enabled = false;
				button_Next.Enabled = false;
			}
			else 
			{
				m_LabelBuilder.Append( "Dir : " );
				m_LabelBuilder.Append( m_DI.FullName );
				m_LabelBuilder.Append( "\r\n" );
				if( m_FIList == null || m_FIList.Length == 0 )
				{
					m_LabelBuilder.Append( "No files available..." );
				}
				else
				{
					m_LabelBuilder.Append( "File : " );
					m_LabelBuilder.Append( (m_Index + 1).ToString() );
					m_LabelBuilder.Append( " / " );
					m_LabelBuilder.Append( m_FIList.Length );
					m_LabelBuilder.Append( "\r\n" );
					if( m_CurrentFile == null )
					{
						m_LabelBuilder.Append( "Current file is null");
					}
					else
					{
						m_LabelBuilder.Append( "Current file : " );
						m_LabelBuilder.Append( m_CurrentFile.Name );
					}
				}
			}

			// set button states
			if( m_Index == -1 )
			{
				button_Previous.Enabled = false;
				button_Next.Enabled = false;
			}
			else if ( m_FIList.Length == 1 )
			{
				button_Previous.Enabled = false;
				button_Next.Enabled = false;
			}
			else if( m_Index == 0 )
			{
				button_Previous.Enabled = false;
				button_Next.Enabled = true;	
			}
			else if( m_Index == m_FIList.Length -1 )
			{
				button_Next.Enabled = false;
				button_Previous.Enabled = true;
			}
			else
			{
				button_Previous.Enabled = true;
				button_Next.Enabled = true;
			}

			// print the label
			label_Report.Text = m_LabelBuilder.ToString();
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DirectoryFileSelection));
			this.button_Previous = new System.Windows.Forms.Button();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.button_Next = new System.Windows.Forms.Button();
			this.button_SelDir = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.text_Filter = new System.Windows.Forms.TextBox();
			this.label_Report = new System.Windows.Forms.Label();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.num_Index = new System.Windows.Forms.NumericUpDown();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.num_Index)).BeginInit();
			this.SuspendLayout();
			// 
			// button_Previous
			// 
			this.button_Previous.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_Previous.ImageIndex = 0;
			this.button_Previous.ImageList = this.imageList;
			this.button_Previous.Location = new System.Drawing.Point(4, 28);
			this.button_Previous.Name = "button_Previous";
			this.button_Previous.Size = new System.Drawing.Size(24, 23);
			this.button_Previous.TabIndex = 0;
			this.button_Previous.Click += new System.EventHandler(this.button_Previous_Click);
			// 
			// imageList
			// 
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// button_Next
			// 
			this.button_Next.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_Next.ImageIndex = 3;
			this.button_Next.ImageList = this.imageList;
			this.button_Next.Location = new System.Drawing.Point(92, 28);
			this.button_Next.Name = "button_Next";
			this.button_Next.Size = new System.Drawing.Size(24, 23);
			this.button_Next.TabIndex = 1;
			this.button_Next.Click += new System.EventHandler(this.button_Next_Click);
			// 
			// button_SelDir
			// 
			this.button_SelDir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_SelDir.ImageIndex = 4;
			this.button_SelDir.ImageList = this.imageList;
			this.button_SelDir.Location = new System.Drawing.Point(92, 4);
			this.button_SelDir.Name = "button_SelDir";
			this.button_SelDir.Size = new System.Drawing.Size(24, 23);
			this.button_SelDir.TabIndex = 2;
			this.button_SelDir.Click += new System.EventHandler(this.button_SelDir_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.num_Index);
			this.panel1.Controls.Add(this.text_Filter);
			this.panel1.Controls.Add(this.button_Next);
			this.panel1.Controls.Add(this.button_SelDir);
			this.panel1.Controls.Add(this.button_Previous);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(120, 56);
			this.panel1.TabIndex = 4;
			// 
			// text_Filter
			// 
			this.text_Filter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.text_Filter.Location = new System.Drawing.Point(4, 4);
			this.text_Filter.Name = "text_Filter";
			this.text_Filter.ReadOnly = true;
			this.text_Filter.Size = new System.Drawing.Size(84, 20);
			this.text_Filter.TabIndex = 3;
			this.text_Filter.Text = "PDB,TRA";
			this.text_Filter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label_Report
			// 
			this.label_Report.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label_Report.Location = new System.Drawing.Point(120, 0);
			this.label_Report.Name = "label_Report";
			this.label_Report.Size = new System.Drawing.Size(200, 56);
			this.label_Report.TabIndex = 5;
			this.label_Report.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// num_Index
			// 
			this.num_Index.Location = new System.Drawing.Point(32, 28);
			this.num_Index.Name = "num_Index";
			this.num_Index.Size = new System.Drawing.Size(60, 20);
			this.num_Index.TabIndex = 4;
			this.num_Index.ValueChanged += new System.EventHandler(this.num_Index_ValueChanged);
			// 
			// DirectoryFileSelection
			// 
			this.Controls.Add(this.label_Report);
			this.Controls.Add(this.panel1);
			this.Name = "DirectoryFileSelection";
			this.Size = new System.Drawing.Size(320, 56);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.num_Index)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void button_Previous_Click(object sender, System.EventArgs e)
		{
			 FileIndex--;		
		}

		private void button_SelDir_Click(object sender, System.EventArgs e)
		{
			ShowDirectorySelection();
		}

		private void button_Next_Click(object sender, System.EventArgs e)
		{
            FileIndex++;		
		}

		private void num_Index_ValueChanged(object sender, System.EventArgs e)
		{
			if( !num_Index_Setup )
			{
				FileIndex = (int)num_Index.Value - 1;
			}		
		}
	}
}
