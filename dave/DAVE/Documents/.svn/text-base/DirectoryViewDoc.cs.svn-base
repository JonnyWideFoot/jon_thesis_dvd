using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

using UoB.Core.Structure;
using UoB.CoreControls.OpenGLView;
using UoB.CoreControls.PS_Render;
using UoB.CoreControls.Controls;
using UoB.CoreControls.Documents;
using UoB.CoreControls.ToolWindows;
using UoB.CoreControls.TraInterface;
using UoB.Core;
using UoB.Core.FileIO;
using UoB.Core.FileIO.PDB;
using UoB.Core.FileIO.Tra;

using UoB.DAVE;

namespace UoB.DAVE.Documents
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public sealed class DirectoryViewDoc : Document
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panel_Viewer;
		private System.Windows.Forms.Panel panel_Bottom;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.Splitter splitter_Bottom;
		private System.Windows.Forms.Panel panel1;
		
		private UoB.CoreControls.ToolWindows.Tra_Scroll_Extended tra_Scroll_Extended;
		private UoB.CoreControls.Controls.DirectoryFileSelection directoryFileSelection;
		
		private DAVEMainForm m_Parent;
		private GLView m_Viewer;
		private ArrayList m_AddedMemberItems = new ArrayList();
		private ParticleSystemDrawWrapper m_DrawWrapper;
		private BaseFileType_Structural m_FileType_Structural = null;

		public DirectoryViewDoc( DAVEMainForm parent ) : base()
		{	
			m_Parent = parent;

			InitializeComponent();
			Size = new Size(600,480);
			Text = "Directory Browser";

			m_Viewer = new GLView();
			m_Viewer.Parent = panel_Viewer;
			m_Viewer.Dock = DockStyle.Fill;
			m_DrawWrapper = new ParticleSystemDrawWrapper( null, m_Viewer );
			AddMember( m_DrawWrapper ); // needed by monitoring tool windows that check which members are present for interaction ...
				
			directoryFileSelection.ActiveFileChange += new UpdateEvent(SetCurrentViewFile);
			directoryFileSelection.CurrentDirectory = null;
		}

		public FileInfo CurrentFile
		{
			get
			{
				return directoryFileSelection.CurrentFile;
			}
		}

		public BaseFileType_Structural CurrentViewItem
		{
			get
			{
				return m_FileType_Structural;
			}
		}

		public DirectoryInfo CurrentDirectory
		{
			get
			{
				return directoryFileSelection.CurrentDirectory;
			}
		}

		public void ShowDirectorySelection()
		{
			directoryFileSelection.ShowDirectorySelection();
		}

		private void SetCurrentViewFile()
		{
			FileInfo currentFile = directoryFileSelection.CurrentFile;

			// remove all the file specific associated members
			for( int i = 0; i < m_AddedMemberItems.Count; i++ )
			{
				RemoveMember( m_AddedMemberItems[i] );
			}
			m_AddedMemberItems.Clear();

			if( currentFile == null )
			{
				OpenNull();
			}
			else if( currentFile.Extension.ToUpper() == ".PDB" )
			{
				OpenPDB( currentFile.FullName );
			}
			else if( currentFile.Extension.ToUpper() == ".TRA" )
			{
                OpenTra( currentFile.FullName );
			}
			else
			{
				if( DialogResult.Yes == MessageBox.Show(Parent,"File extension not recognised! Try to open as PDB file?","File Extension",MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation) )
				{
					OpenPDB( currentFile.FullName );
				}
				else
				{
					OpenNull();
				}
			}

			for( int i = 0; i < m_AddedMemberItems.Count; i++ )
			{
				AddMember( m_AddedMemberItems[i] );
			}

			m_Parent.UpdateActiveMDIChild(); // triggers toolbars and menus to update
		}

		private void OpenNull()
		{
			m_DrawWrapper.particleSystem = null;
			tra_Scroll_Extended.AttachTo( null );

			m_FileType_Structural = null;
		}

		private void OpenTra( string fileName )
		{
			Tra tra = new Tra( fileName ); // only loads position info, no trajectory info ...
			TrajReadin tr = new TrajReadin( tra, this );

			m_DrawWrapper.particleSystem = tra.particleSystem;
			tra_Scroll_Extended.AttachTo( tra.PositionDefinitions );

			m_AddedMemberItems.Add( tra );
			m_AddedMemberItems.Add( tra.particleSystem );
			m_AddedMemberItems.Add( tra.DataStore );
			m_AddedMemberItems.Add( tra.SequenceInfo );

			m_FileType_Structural = tra;
		}

		private void OpenPDB( string fileName )
		{
			PDB file = new PDB( fileName, true );

			m_DrawWrapper.particleSystem = file.particleSystem;
			tra_Scroll_Extended.AttachTo( file.PositionDefinitions );

			m_AddedMemberItems.Add( file );
			m_AddedMemberItems.Add( file.SequenceInfo );
			m_AddedMemberItems.Add( file.particleSystem );
			m_AddedMemberItems.Add( file.PositionDefinitions );

			m_FileType_Structural = file;
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
			this.tra_Scroll_Extended = new UoB.CoreControls.ToolWindows.Tra_Scroll_Extended();
			this.panel_Viewer = new System.Windows.Forms.Panel();
			this.panel_Bottom = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.splitter_Bottom = new System.Windows.Forms.Splitter();
			this.directoryFileSelection = new UoB.CoreControls.Controls.DirectoryFileSelection();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.panel_Bottom.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tra_Scroll_Extended
			// 
			this.tra_Scroll_Extended.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tra_Scroll_Extended.Location = new System.Drawing.Point(0, 0);
			this.tra_Scroll_Extended.Name = "tra_Scroll_Extended";
			this.tra_Scroll_Extended.Size = new System.Drawing.Size(208, 64);
			this.tra_Scroll_Extended.TabIndex = 4;
			// 
			// panel_Viewer
			// 
			this.panel_Viewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Viewer.Location = new System.Drawing.Point(0, 0);
			this.panel_Viewer.Name = "panel_Viewer";
			this.panel_Viewer.Size = new System.Drawing.Size(528, 349);
			this.panel_Viewer.TabIndex = 2;
			// 
			// panel_Bottom
			// 
			this.panel_Bottom.Controls.Add(this.panel1);
			this.panel_Bottom.Controls.Add(this.directoryFileSelection);
			this.panel_Bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel_Bottom.Location = new System.Drawing.Point(0, 285);
			this.panel_Bottom.Name = "panel_Bottom";
			this.panel_Bottom.Size = new System.Drawing.Size(528, 64);
			this.panel_Bottom.TabIndex = 5;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.tra_Scroll_Extended);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(320, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(208, 64);
			this.panel1.TabIndex = 8;
			// 
			// splitter_Bottom
			// 
			this.splitter_Bottom.Location = new System.Drawing.Point(0, 0);
			this.splitter_Bottom.Name = "splitter_Bottom";
			this.splitter_Bottom.TabIndex = 0;
			this.splitter_Bottom.TabStop = false;
			// 
			// directoryFileSelection
			// 
			this.directoryFileSelection.CurrentDirectory = null;
			this.directoryFileSelection.Dock = System.Windows.Forms.DockStyle.Left;
			this.directoryFileSelection.FileIndex = -1;
			this.directoryFileSelection.Location = new System.Drawing.Point(0, 0);
			this.directoryFileSelection.Name = "directoryFileSelection";
			this.directoryFileSelection.Size = new System.Drawing.Size(320, 64);
			this.directoryFileSelection.TabIndex = 6;
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter2.Location = new System.Drawing.Point(0, 282);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(528, 3);
			this.splitter2.TabIndex = 6;
			this.splitter2.TabStop = false;
			// 
			// DirectoryViewDoc
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(528, 349);
			this.Controls.Add(this.splitter2);
			this.Controls.Add(this.panel_Bottom);
			this.Controls.Add(this.panel_Viewer);
			this.Name = "DirectoryViewDoc";
			this.Text = "Form1";
			this.panel_Bottom.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
