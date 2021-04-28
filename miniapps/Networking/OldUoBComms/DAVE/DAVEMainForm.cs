using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using TD.SandBar;
using DockingSuite;

using UoB.Research.DynamicUpdate;
using UoB.Research.Modelling.Alignment;
using UoB.Research;
using UoB.Research.FileIO;
using UoB.Research.FileIO.PDB;
using UoB.Research.FileIO.Tra;
using UoB.Research.Modelling.Structure;
using UoB.Comms;
using UoB.Comms.ServerSide;
using UoB.DAVE.Documents;
using UoB.DAVE.Dialog;
using UoB.Generic;
using UoB.Generic.Builder;
using UoB.Generic.Documents;
using UoB.Generic.Reporting;
using UoB.Generic.ToolWindows;
using UoB.Generic.TraInterface;
using UoB.Research.Modelling.Builder;
using UoB.Research.Interop;
using UoB.Generic.Interop;
using UoB.Generic.DialogBoxes;
using UoB.Research.Dendrogram;
using UoB.Generic.Dendro_Render;
using UoB.Research.Modelling.ForceField;

namespace UoB.DAVE
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class DAVEMainForm : System.Windows.Forms.Form
	{
		private TD.SandBar.ToolBarContainer leftSandBarDock;
		private TD.SandBar.ToolBarContainer rightSandBarDock;
		private TD.SandBar.ToolBarContainer bottomSandBarDock;
		private TD.SandBar.ToolBarContainer topSandBarDock;
		private TD.SandBar.SandBarManager sandBarManager;
		private TD.SandBar.ToolBar toolBar_Main;
		private TD.SandBar.MenuButtonItem menu_Help_About;
		private TD.SandBar.MenuButtonItem menu_File_New;
		private TD.SandBar.MenuButtonItem menu_File_OpenFromFile;
		private TD.SandBar.MenuButtonItem menu_File_OpenFromDownload;
		private TD.SandBar.MenuButtonItem menu_File_OpenFromCache;
		private TD.SandBar.MenuButtonItem menu_File_Recent1;
		private TD.SandBar.MenuButtonItem menu_File_Recent2;
		private TD.SandBar.MenuButtonItem menu_File_Recent3;
		private TD.SandBar.MenuButtonItem menu_File_Recent4;
		private TD.SandBar.MenuButtonItem menu_File_RecentMore;
		private TD.SandBar.MenuButtonItem menu_File_Recent5;
		private TD.SandBar.MenuButtonItem menu_File_Recent6;
		private TD.SandBar.MenuButtonItem menu_File_Recent7;
		private TD.SandBar.MenuButtonItem menu_File_Recent8;
		private TD.SandBar.MenuButtonItem menu_File_Recent9;
		private TD.SandBar.MenuButtonItem menu_File_Exit;
		private TD.SandBar.MenuButtonItem menu_File_Remote;
		private TD.SandBar.ButtonItem button_Main_New;
		private TD.SandBar.DropDownMenuItem button_Main_OpenDropdown;
		private TD.SandBar.MenuButtonItem button_Main_FromFile;
		private TD.SandBar.MenuButtonItem button_Main_FromDownload;
		private TD.SandBar.MenuButtonItem button_Main_FromCache;
		private TD.SandBar.MenuBarItem menu_File;
		private TD.SandBar.MenuBarItem menu_Comms;
		private TD.SandBar.MenuButtonItem menu_Comms_Listening;
		private TD.SandBar.MenuButtonItem menu_Window_Cascade;
		private TD.SandBar.MenuButtonItem menuButtonItem1;
		private TD.SandBar.MenuButtonItem menuButtonItem2;
		private TD.SandBar.ToolBar toolBar_ParticleSystem;
		private TD.SandBar.ToolBar toolBar_MDIActiveDoc;
		private TD.SandBar.LabelItem label_ActiveMDIDoc;
		private TD.SandBar.MenuBar menuBar_Main;
		private TD.SandBar.ButtonItem button_PartSys_TreeView;
		private TD.SandBar.ButtonItem button_PartSys_DrawMode;
		private TD.SandBar.ToolBar toolBar_Trajectory;
		private TD.SandBar.ButtonItem buttonItem_TraScroller;
		private TD.SandBar.ToolBar toolBar_DataManager;
		private TD.SandBar.ButtonItem buttonItem_DataManager;
		private TD.SandBar.ButtonItem button_PS_Ramachandran;
		private TD.SandBar.ButtonItem button_PS_Camera;
		private TD.SandBar.MenuButtonItem menu_File_Save;
		private TD.SandBar.ButtonItem buttonItem_TraDataBox;
		private TD.SandBar.ButtonItem buttonItem_SpawnFileView;
		private TD.SandBar.ButtonItem buttonItem_SpawnDefaultViewer;
		private TD.SandBar.MenuBarItem menu_Builder;
		private TD.SandBar.MenuButtonItem menu_Builder_RebuildAtoms;
		private TD.SandBar.MenuButtonItem menu_Interop_ExternalText;
		private TD.SandBar.MenuButtonItem menu_Interop_LaunchDefaultViewer;
		private TD.SandBar.MenuButtonItem menu_Interop_Minimise;
		private TD.SandBar.MenuButtonItem menu_Builder_HomologyBuild;
		private TD.SandBar.MenuBarItem menu_Interop;
		private TD.SandBar.MenuBarItem menu_Window;
		private TD.SandBar.MenuBarItem menu_Help;
		private TD.SandBar.MenuBarItem menu_Tools;
		private TD.SandBar.MenuButtonItem menu_Tools_SequenceBox;
		private TD.SandBar.MenuButtonItem menu_Tools_Alignment;
		private TD.SandBar.MenuButtonItem menu_Tools_Rotate;
		private TD.SandBar.MenuButtonItem menu_Tools_AlignmentView;
		private TD.SandBar.MenuButtonItem menu_Builder_ApplyMirror;
		private TD.SandBar.MenuButtonItem menu_Tools_AlignCurrentTRAEtoTarget;
		private TD.SandBar.MenuButtonItem menu_File_BrowseDir; // set by the Thread.CurrentThread accessor in the Main() function]
		
		private DockingSuite.DockHost m_DockHost_Right;
		private DockingSuite.DockHost m_DockHost_Bottom;
		private DockingSuite.DockPanel m_DocPanel_RightTop;
		//		#if DEBUG
		//		private DockingSuite.DockPanel m_DocPanel_RightBottom = null;
		//		#endif

		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.OpenFileDialog openFileDialog;

		private UoBInit m_UoBInit = UoBInit.Instance;
		private ServerComms m_Comms;
		private RecentFileListManager m_RecentFiles;
		private ArrayList m_Documents;
		private Document m_ActiveDocument;
		private object m_DocLock = new object();
		private ArrayList m_ToolList = new ArrayList();

		private object m_PreloadLock = new object();
		private static Thread daveThread = null;
		private static DAVEMainForm daveMainForm = null;

		static void AssertIsCurrentVersion()
		{
			// Assert that we are using the current version
			try
			{
				string assemblyPath = Assembly.GetEntryAssembly().Location;
				DirectoryInfo di = new DirectoryInfo( assemblyPath );
				di = di.Parent; // gets rid of teh dave.exe bit on the end
				string updaterFileName = di.FullName + @"\DynamicUpdate.exe";
				if( File.Exists(updaterFileName) )
				{
					DynamicUpdater du = new DynamicUpdater( di );
					if( du.UpdateRequired )
					{
						if( DialogResult.Yes == MessageBox.Show(daveMainForm,"You are not using the current version of DAVE, would you like to upgrade? This will cause DAVE to exit.","Upgrade?",MessageBoxButtons.YesNo,MessageBoxIcon.Question) )
						{
							try
							{			
								Process p = new Process();
								p.StartInfo.FileName = updaterFileName;
								p.StartInfo.Arguments = Process.GetCurrentProcess().Id.ToString() + " " + di.FullName + "\\";
								p.Start();
								return; // we cant run the prog, its going to be overwritten
							}
							catch
							{
							}
						}
					}
				}
			}
			catch
			{
			}
		}

		private static bool DetectDaveProcess( string forwardFilename )
		{
			Process p = RunningInstance();
			if( p == null )
			{
				return false;
			}
			else
			{
				// get the DAVE class and ask it to open the filename that this instance was started with ...
				ProcessModule pm = p.MainModule;
				//pm.
				return true;
			}
		}

		private static Process RunningInstance() 
		{ 
			Process current = Process.GetCurrentProcess(); 
			Process[] processes = Process.GetProcessesByName (current.ProcessName); 

			//Loop through the running processes in with the same name 
			foreach (Process process in processes) 
			{ 
				//Ignore the current process 
				if (process.Id != current.Id) 
				{ 
					//Make sure that the process is running from the exe file. 
					if (Assembly.GetExecutingAssembly().Location.
						Replace("/", "\\") == current.MainModule.FileName) 
 
					{  
						//Return the other process instance.  
						return process; 
 
					}  
				}  
			} 
			//No other instance was found, return null.  
			return null;  
		}


		[STAThread]
		static void Main( string[] args ) 
		{
			// the thread used to invoke the application is 
			Thread.CurrentThread.Name = "DAVE MainThread";
			// used to assert the current version and terminate the main thread if it isnt
			daveThread = Thread.CurrentThread;

			// used for long filenames containing spaces
			StringBuilder concatFilename = new StringBuilder();
			for( int i = 0; i < args.Length; i++ )
			{
				concatFilename.Append(args[i]);
				if( i != (args.Length - 1) ) concatFilename.Append(' ');
			}
			string openName = concatFilename.ToString();


			// Preliminary checks
			//AssertRegistryState(); // Will be used to manage registry associations
			//if( DetectDaveProcess( openName ) ) // multi process suppression
			//{
			//	return; // exit this process, the message has been passed on ...
			//}
	
			ThreadStart updateThreadStart = new ThreadStart( AssertIsCurrentVersion );
			Thread updateThread = new Thread( updateThreadStart );
			updateThread.Name = "Dynamic WebUpdate Thread";
			updateThread.Start();			

			// Currently a mini-hack for correct forcefield initialisation
			FFManager ffMan = FFManager.Instance;
			ffMan.FinaliseStage2();

			// launch dave
			daveMainForm = new DAVEMainForm();
			if( File.Exists( openName ) )
			{
				daveMainForm.OpenFileName( openName );
			}
			Application.Run( daveMainForm );			     			
		}

//		private static void AssertRegistryState()
//		{
//			RegistryKey rk = Registry.ClassesRoot;
//			RegistryKey ext = rk.CreateSubKey( ".pdb" );
//			ext.SetValue( "", "DAVE Associated FileType" );
//		}

		public DAVEMainForm()
		{
			m_Documents = new ArrayList(5);
			m_RecentFiles = new RecentFileListManager("DAVE",10);

			m_Comms = new ServerComms();
			m_Comms.ConnectionReceived += new ConnectionReceived(m_Comms_ConnectionReceived);

			InitializeComponent();

			WindowState = FormWindowState.Maximized;
			SetupToolBarTagging(); // allows for dynamic enabling/disabling of different toolbars when the active document type changes
			UpdateRecentFiles(); // the recent files class takes care of the 1-10 recent files list and records info in UoBInit
			UpdateActiveMDIChild(); // Initialise the relevent toolBars
//
//			// setup commonly used tool windows
//			#if DEBUG // doesnt tell the user very much, but i want it ...
//				m_DocPanel_RightBottom = new DockingSuite.DockPanel();
//				m_DockHost_Right.Controls.Add(this.m_DocPanel_RightBottom);
//				m_DocPanel_RightBottom.AutoHide = false;
//				m_DocPanel_RightBottom.DockedHeight = 128;
//				m_DocPanel_RightBottom.DockedWidth = 0;
//				m_DocPanel_RightBottom.Location = new System.Drawing.Point(4, 315);
//				m_DocPanel_RightBottom.Name = "m_DocPanel_RightBottom";
//				m_DocPanel_RightBottom.SelectedTab = null;
//				m_DocPanel_RightBottom.Size = new System.Drawing.Size(188, 128);
//				m_DocPanel_RightBottom.TabIndex = 1;
//				m_DocPanel_RightBottom.Text = "Docked Panel";
//				SimpleReporter sr = new SimpleReporter(); // will now catch any Trace.Write() commands to show the user
//				SetupToolWindow(sr, m_DocPanel_RightBottom);
//			#endif

			PSTreeView window2 = new PSTreeView();
			SetupToolWindow(window2, m_DocPanel_RightTop);	
			PS_Selections window1 = new PS_Selections();
			SetupToolWindow(window1, m_DocPanel_RightTop);	

			Trace.WriteLine( "Now initialising DAVE class on - " + Thread.CurrentThread.Name );
		}


		#region DAVE Processes

		private void DAVEMainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if ( m_Comms != null )
			{
				m_Comms.FinaliseAndExit(); 
			}
		}

		private void SetupRecentFile( TD.SandBar.MenuButtonItem menuItem, int fileNum )
		{
			string fileName = m_RecentFiles.recentFileList[fileNum - 1];
			if ( fileName == "" || !File.Exists( fileName ) )
			{
				menuItem.Enabled = false;
				menuItem.Text = fileNum.ToString() + " - No File";
			}
			else
			{
				menuItem.Enabled = true;
				menuItem.Text = fileNum.ToString() + " - " + fileName;
			}
		}

		private void New()
		{
			Document doc = new Document();
			AddDocument( doc );
		}

		private void TriggerFileOpenDialog()
		{
			openFileDialog.Title = "DAVE Open File Dialog"; 
			//openFileDialog.InitialDirectory =  ???; 
			openFileDialog.Filter = "All DAVE Readable Files (*.TRA,*.PDB,*.PH,*.ALIGN)|*.TRA;*.PDB;*.PH;*.ALIGN|PDB Files (*.PDB)|*.PDB|TRA Files (*.TRA)|*.TRA|PH Files (Dendrogram) (*.PH)|*.PH|ALIGN Files (DAVE Alignment Definitions)|*.ALIGN|All files (*.*)|*.*"; 
			openFileDialog.FilterIndex = 0; 
			openFileDialog.RestoreDirectory = true; 

			if(openFileDialog.ShowDialog() == DialogResult.OK) 
			{ 
				Trace.WriteLine("Opening File : " + openFileDialog.FileName + " ...");
				Trace.Indent();
				Trace.WriteLine("");
				
				OpenFileName( openFileDialog.FileName );
				UpdateRecentFiles();
				
				Trace.WriteLine("");
				Trace.Unindent();
				Trace.WriteLine("File Open Complete");
				Trace.WriteLine("");
			} 
		}

		private void TriggerBuilder()
		{
			Trace.WriteLine("Initialising Model Builder");
			Builder builder = new Builder();
			//builder.MdiParent = this;
			builder.ModelReady += new ParticleSystemEvent( ParticleSystemEventHandler );
			builder.ShowDialog(this);
		}

		// builder ending callbacks
		private void OpenParticleSystemInNewWindow( ParticleSystem ps )
		{
			PSViewDoc viewDoc = new PSViewDoc( ps );
			viewDoc.AddMember( ps );
			AddDocument( viewDoc );
		}

		private void ParticleSystemEventHandler( ParticleSystem ps )
		{
			ParticleSystemEvent pe = new ParticleSystemEvent( OpenParticleSystemInNewWindow );
			Invoke( pe, new object[] { ps } );
		}
		// end builder callbacks

		public void OpenFileName( string fileName )
		{
			if ( !File.Exists(fileName) )
			{
				MessageBox.Show("File no longer exists at that location");
				return;
			}
			m_RecentFiles.AddRecentFile(fileName);
			UpdateRecentFiles();

			string filetype = Path.GetExtension( fileName ).ToUpper();

			lock(m_PreloadLock)
			{
				switch (filetype)
				{
					case ".PDB":
						PDB pdbFile = new PDB( fileName, true );
						PDBViewDoc viewDoc = new PDBViewDoc( pdbFile );
						AddDocument( viewDoc );
						break;
					case ".TRA":
						Tra tra = new Tra( fileName ); // only loads position info, no trajectory info ...
						TrajReadin tr = new TrajReadin( tra, this );
						TraViewDoc view = new TraViewDoc( tra );
						AddDocument(view);
						break;
					case ".PH":
						DendroTree tree = new DendroTree( fileName );
						DendroViewDoc doc = new DendroViewDoc( tree );
						AddDocument( doc );
						doc.Begin();
						break;	
					case ".ALI": // 8.3 file-naming hack for align files upon file association launch
						goto ALIGNJUMP;
					case ".ALIGN":
						ALIGNJUMP:
							try
							{
								AlignFile ali = new AlignFile( fileName );
								ModelViewDoc document = new ModelViewDoc( ali[0].ModelStore );
								document.AddMember( ali[0].Models ); // add the models that we have found, so alignment can be viewed
								AddDocument( document );
								CallToolWindow( typeof(Model_EquivView), null );
							}
							catch( Exception ex )
							{
								MessageBox.Show(this,"Error during align file parseing : " + ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
							}
						break;
					default:
						if( DialogResult.Yes == MessageBox.Show( this, "Unknown file extension selected, would you like to open this as a PDB file ?\r\n" + fileName + "\r\nDetected filetype : " + filetype, "FileOpen Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1 ) )
						{
							PDB pdbFileDef = new PDB( fileName, true );
							PDBViewDoc viewDocDef = new PDBViewDoc( pdbFileDef );
							viewDocDef.AddMember( pdbFileDef );
							AddDocument( viewDocDef );
						}
						break;
				}
			}
		}

		private void UpdateRecentFiles()
		{
			SetupRecentFile(menu_File_Recent1, 1);
			SetupRecentFile(menu_File_Recent2, 2);
			SetupRecentFile(menu_File_Recent3, 3);
			SetupRecentFile(menu_File_Recent4, 4);
			SetupRecentFile(menu_File_Recent5, 5);
			SetupRecentFile(menu_File_Recent6, 6);
			SetupRecentFile(menu_File_Recent7, 7);
			SetupRecentFile(menu_File_Recent8, 8);
			SetupRecentFile(menu_File_Recent9, 9);
		}


		#endregion

		#region Connectivity

		private ConnectionReceived d_ConnectionRecieved;
		private void m_Comms_ConnectionReceived(ConnectionManager cm)
		{
			// should be in the connection socket reader thread
			Debug.WriteLine("DAVE says : Connection manager obtained - Now in thread - " + Thread.CurrentThread.Name);
			d_ConnectionRecieved = new ConnectionReceived( AddConnection );
			Invoke(d_ConnectionRecieved, new object[1] { cm } );
		}

		public void AddConnection( ConnectionManager cm )
		{
			//			// Must be called from the main thread via an invoke statement
			//			Debug.WriteLine("AddConnection() called in BriefCaseManager : Now in thread - " + Thread.CurrentThread.Name);
			//			
			//			lock ( m_Documents )
			//			{
			//				AddCase( new BriefCase( new CommunicationsWrapper( cm ) ) );
			//			}
		}


		#endregion

		#region DAVE MDI and ToolBar Windowing Setup

		// Called once at application startup :
		// Adds a Type object to each menu bar to state which object they apply to
		private void SetupToolBarTagging()
		{
			TD.SandBar.ToolBar[] toolBars = sandBarManager.GetToolBars();

#if DEBUG
#else
					toolBar_MDIActiveDoc.Visible = false;
#endif

			for ( int i = 0; i < toolBars.Length; i++ )
			{
				toolBarButtonActivation(toolBars[i], IsMainToolBar(toolBars[i])) ; 
			}
			// Now set the relevant toolbar tags
			this.toolBar_ParticleSystem.Tag = typeof(UoB.Generic.PS_Render.ParticleSystemDrawWrapper);
			this.toolBar_Trajectory.Tag = typeof(UoB.Research.Modelling.Structure.PS_PositionStore);
			this.toolBar_DataManager.Tag = typeof(UoB.Research.Data.DataManager);
		}

		private bool IsMainToolBar( TD.SandBar.ToolBar toolbar )
		{
			// Ensure that the following are active
			// 0 = menuBar_Main
			// 1 = toolBar_Main
			// 2 = toolBar_MDIActiveDoc

			if ( toolbar == menuBar_Main ) return true;
			if ( toolbar == toolBar_Main ) return true;
#if DEBUG
			if ( toolbar == toolBar_MDIActiveDoc )  return true;
#endif

			return false;
		}


		// Called once per new ToolWindow Required - Control must implement the ITool Interface
		private void SetupToolWindow(ITool tool, DockPanel panel)
		{
			m_ToolList.Add( tool );

			DockingSuite.DockControl dc = new DockControl();
			dc.Parent = panel;
			dc.Text = tool.Text;

			tool.Parent = dc;
			tool.Dock = DockStyle.Fill;
			tool.Show();

			EnsureVisible( tool );
		}
		
		private void SetupToolWindow( ITool tool )
		{
			m_ToolList.Add( tool );
			Size size = tool.Size;

			DockingSuite.DockControl dc = new DockControl();
			dc.Text = tool.Text;
			dc.Size = size;
			tool.Parent = dc;
			tool.Dock = DockStyle.Fill;

			DockingSuite.DockPanel dp = new DockPanel();
			dp.Parent = m_DockHost_Bottom;
			dc.Parent = dp;
			dp.Pop();

//			 
//			int a = 600;
//			a = a << 16;
//			int b = 600;
//			a += b;
//
//			Form f = new Form();
//			f.Size = new Size(100,100);
//			f.Show();
//
//			f.Width = 200;
//
//			SendMessage(f.Handle,5,0,a); // WM_Size
//			SendMessage(f.Handle,15,0,0);  // WM_Paint

			//dc.EnsureVisible( m_DockHost_Right );
			//dp.Pop();

			//dc.Width = size.Width;
			//dc.Height = size.Height;
		}

		// Called once per new document created
		public void AddDocument(Document doc)
		{
			// All cases must be created by the main thread via an invoke statement
			lock(m_DocLock)
			{
				m_Documents.Add( doc );
			}
			doc.MdiParent = this;
			doc.Closed += new EventHandler(doc_Closed);
			doc.Show();
		}

		private void doc_Closed(object sender, EventArgs e)
		{
			lock(m_DocLock)
			{
				Document d = (Document) sender;
				m_Documents.Remove( d );
				d.Closed -= new EventHandler(doc_Closed);
			}
		}

		// Enables / Disables a given toolbar - called in DAVEMainForm_MdiChildActivate()
		private void toolBarButtonActivation( TD.SandBar.ToolBar toolBar, bool activate )
		{
			for( int i = 0; i < toolBar.Buttons.Count; i++ )
			{
				toolBar.Buttons[i].Enabled = activate;
			}
		}

		private void DAVEMainForm_MdiChildActivate(object sender, System.EventArgs e)
		{
			UpdateActiveMDIChild();
		}

		// Main Function - must enable/disable toolbars depending on the document and 
		// call AttachToDocument() on all toolWindows
		// Function is also called once at Application startup

		private void UpdateCallToolRefresh()
		{
			foreach ( ITool tool in m_ToolList )
			{
				tool.AttachToDocument( m_ActiveDocument );
			}
		}

		public void UpdateActiveMDIChild()
		{
			// Test for null : i.e. all documents have been closed
			if( ActiveMdiChild == null ) 
			{
				m_ActiveDocument = null;
				label_ActiveMDIDoc.Text = @"Active MDI Doc : N/A";
				UpdateCallToolRefresh();				
				return;
			}

			// We can have MDI documents that are not "Root Documents" and dont derive from Document, these are not "True" Children and cannot be set to the m_ActiveChild
			if( 
				!UoB.Research.Tools.CommonTools.derivesFrom(typeof(Document),ActiveMdiChild) 
				) return; // the active window is not a "DAVE Document"

			// above call tells us that this cast is valid
			m_ActiveDocument = (Document) ActiveMdiChild;
			menu_File_Save.Enabled = m_ActiveDocument.canSave;
			label_ActiveMDIDoc.Text = "Active MDI Doc : " + m_ActiveDocument.Text;

			TD.SandBar.ToolBar[] toolBars = sandBarManager.GetToolBars();
			for ( int i = 0; i < toolBars.Length; i++ )
			{
				if ( IsMainToolBar( toolBars[i] ) )
				{
					toolBarButtonActivation(toolBars[i], true); 
					continue;
				}
				else
				{
					toolBarButtonActivation(toolBars[i], false); 
				}
				if ( toolBars[i].Tag != null )
				{
					for ( int j = 0; j < m_ActiveDocument.MemberCount; j++ )
					{
						try
						{
							if ( (Type) toolBars[i].Tag == m_ActiveDocument[j].GetType() )
							{
								toolBarButtonActivation(toolBars[i], true); 
								break;
							}
						}
						catch
						{
						}
					}
				}
			}

			UpdateCallToolRefresh();
		}


		// Has our tool been created yet and added to the m_ToolList arrayList
		private bool isToolPresent( Type type, out int index )
		{
			for ( int i = 0; i < m_ToolList.Count; i++ )
			{
				if ( m_ToolList[i].GetType() == type )
				{
					index = i;
					return true;
				}
			}
			index = -1;
			return false;
		}


		#endregion

		#region Menu events

		private void menu_Help_About_Activate(object sender, System.EventArgs e)
		{
			UoB.DAVE.Dialog.aboutBox about = new UoB.DAVE.Dialog.aboutBox();
			about.ShowDialog();
		}

		private void Exit_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void cascade_Click(object sender, System.EventArgs e)
		{
			LayoutMdi(MdiLayout.Cascade);
		}

		private void tileHorisontal_Click(object sender, System.EventArgs e)
		{
			LayoutMdi(MdiLayout.TileHorizontal);		
		}

		private void tileVertical_Click(object sender, System.EventArgs e)
		{
			LayoutMdi(MdiLayout.TileVertical);		
		}

		private void menu_Comms_Listening_Activate(object sender, System.EventArgs e)
		{
			if ( menu_Comms_Listening.Checked )
			{
				menu_Comms_Listening.Checked = false;
				m_Comms.listening = false;
			}
			else
			{
				menu_Comms_Listening.Checked = true;
				m_Comms.listening = true;
			}
		}

		private void menu_File_New_Activate(object sender, System.EventArgs e)
		{
			New();		
		}

		private void menu_File_OpenFromCache_Activate(object sender, System.EventArgs e)
		{
			PDBCacheOpenWindow w = new PDBCacheOpenWindow();
			if (w.ShowDialog(this) == DialogResult.OK)
			{
				foreach ( string fileName in w.FileList )
				{
					OpenFileName( fileName );
				}
			}	
		}

		private void menu_File_OpenFile_Click(object sender, System.EventArgs e)
		{
			TriggerFileOpenDialog();		
		}

		private void menu_File_Recent_Click(object sender, System.EventArgs e)
		{
			TD.SandBar.MenuButtonItem menuItem = (TD.SandBar.MenuButtonItem) sender;
			int fileID = int.Parse( menuItem.Text[0].ToString() );
			string fileName = m_RecentFiles.recentFileList[fileID - 1];
			Trace.WriteLine("Opening File : " + fileName + " ...");
			OpenFileName( fileName );
			UpdateRecentFiles();
		}

		private void menu_File_OpenFromDownload_Activate(object sender, System.EventArgs e)
		{
			openDownloadTool();		
		}

		private void EnsureVisible( ITool tool )
		{
			DockingSuite.DockHost hostToUse = m_DockHost_Right;
			try
			{
				DockControl dc = (DockControl) tool.Parent;
				dc.EnsureVisible( hostToUse );
			}
			catch
			{
				MessageBox.Show( "Ensure visible on tool control has failed!" );
			}
		}

		private void toolBar_ParticleSystem_ButtonClick(object sender, TD.SandBar.ToolBarItemEventArgs e)
		{
			switch( toolBar_ParticleSystem.Buttons.IndexOf(e.Item) )
			{
				case 0: // PartSys Drawing Mode Selection
					CallToolWindow( typeof(PS_Selections), m_DocPanel_RightTop );
					break;
				case 1: // PartSys TreeView
					CallToolWindow( typeof(PSTreeView), m_DocPanel_RightTop );
					break;
				case 2: // Ramachandran Plot
					CallToolWindow( typeof(PS_Ramachandran), m_DocPanel_RightTop );
					break;
				case 3: // PS_CameraPerspective
					CallToolWindow( typeof(PS_CameraPerspective), m_DocPanel_RightTop );
					break;
				default:
					break;
			}
		}

		#endregion

		#region Downloading Tool Window

		private void openDownloadTool()
		{
			// cant use this as we have to subscribe to the FileDone event
			// CallToolWindow( typeof(WebDownloadForm), m_DocPanel_RightTop );

			int index;
			if ( isToolPresent( typeof( WebDownloadForm ), out index ) )
			{
				WebDownloadForm tool = (WebDownloadForm) m_ToolList[index];
				EnsureVisible( tool );
			}
			else
			{
				WebDownloadForm window = new WebDownloadForm();
				window.FileDoneCallback += new FileDoneHandler( funcFileDoneHandler );
				SetupToolWindow(window, m_DocPanel_RightTop);		
			}
		}

		private void funcFileDoneHandler( string fileName )
		{
			StringEvent se = new StringEvent( OpenFileName );
			Invoke( se, new object[] { fileName } );
		}

		#endregion
		
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DAVEMainForm));
			this.sandBarManager = new TD.SandBar.SandBarManager();
			this.bottomSandBarDock = new TD.SandBar.ToolBarContainer();
			this.leftSandBarDock = new TD.SandBar.ToolBarContainer();
			this.rightSandBarDock = new TD.SandBar.ToolBarContainer();
			this.topSandBarDock = new TD.SandBar.ToolBarContainer();
			this.toolBar_DataManager = new TD.SandBar.ToolBar();
			this.buttonItem_TraDataBox = new TD.SandBar.ButtonItem();
			this.buttonItem_DataManager = new TD.SandBar.ButtonItem();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.toolBar_Trajectory = new TD.SandBar.ToolBar();
			this.buttonItem_TraScroller = new TD.SandBar.ButtonItem();
			this.menuBar_Main = new TD.SandBar.MenuBar();
			this.menu_File = new TD.SandBar.MenuBarItem();
			this.menu_File_New = new TD.SandBar.MenuButtonItem();
			this.menu_File_OpenFromFile = new TD.SandBar.MenuButtonItem();
			this.menu_File_OpenFromDownload = new TD.SandBar.MenuButtonItem();
			this.menu_File_OpenFromCache = new TD.SandBar.MenuButtonItem();
			this.menu_File_Save = new TD.SandBar.MenuButtonItem();
			this.menu_File_Recent1 = new TD.SandBar.MenuButtonItem();
			this.menu_File_Recent2 = new TD.SandBar.MenuButtonItem();
			this.menu_File_Recent3 = new TD.SandBar.MenuButtonItem();
			this.menu_File_Recent4 = new TD.SandBar.MenuButtonItem();
			this.menu_File_RecentMore = new TD.SandBar.MenuButtonItem();
			this.menu_File_Recent5 = new TD.SandBar.MenuButtonItem();
			this.menu_File_Recent6 = new TD.SandBar.MenuButtonItem();
			this.menu_File_Recent7 = new TD.SandBar.MenuButtonItem();
			this.menu_File_Recent8 = new TD.SandBar.MenuButtonItem();
			this.menu_File_Recent9 = new TD.SandBar.MenuButtonItem();
			this.menu_File_Exit = new TD.SandBar.MenuButtonItem();
			this.menu_Tools = new TD.SandBar.MenuBarItem();
			this.menu_Tools_SequenceBox = new TD.SandBar.MenuButtonItem();
			this.menu_Tools_Alignment = new TD.SandBar.MenuButtonItem();
			this.menu_Tools_AlignmentView = new TD.SandBar.MenuButtonItem();
			this.menu_Tools_Rotate = new TD.SandBar.MenuButtonItem();
			this.menu_Tools_AlignCurrentTRAEtoTarget = new TD.SandBar.MenuButtonItem();
			this.menu_Interop = new TD.SandBar.MenuBarItem();
			this.menu_Interop_ExternalText = new TD.SandBar.MenuButtonItem();
			this.menu_Interop_LaunchDefaultViewer = new TD.SandBar.MenuButtonItem();
			this.menu_File_Remote = new TD.SandBar.MenuButtonItem();
			this.menu_Interop_Minimise = new TD.SandBar.MenuButtonItem();
			this.menu_Builder = new TD.SandBar.MenuBarItem();
			this.menu_Builder_HomologyBuild = new TD.SandBar.MenuButtonItem();
			this.menu_Builder_RebuildAtoms = new TD.SandBar.MenuButtonItem();
			this.menu_Builder_ApplyMirror = new TD.SandBar.MenuButtonItem();
			this.menu_Comms = new TD.SandBar.MenuBarItem();
			this.menu_Comms_Listening = new TD.SandBar.MenuButtonItem();
			this.menu_Window = new TD.SandBar.MenuBarItem();
			this.menu_Window_Cascade = new TD.SandBar.MenuButtonItem();
			this.menuButtonItem1 = new TD.SandBar.MenuButtonItem();
			this.menuButtonItem2 = new TD.SandBar.MenuButtonItem();
			this.menu_Help = new TD.SandBar.MenuBarItem();
			this.menu_Help_About = new TD.SandBar.MenuButtonItem();
			this.toolBar_Main = new TD.SandBar.ToolBar();
			this.button_Main_New = new TD.SandBar.ButtonItem();
			this.button_Main_OpenDropdown = new TD.SandBar.DropDownMenuItem();
			this.button_Main_FromFile = new TD.SandBar.MenuButtonItem();
			this.button_Main_FromDownload = new TD.SandBar.MenuButtonItem();
			this.button_Main_FromCache = new TD.SandBar.MenuButtonItem();
			this.buttonItem_SpawnFileView = new TD.SandBar.ButtonItem();
			this.buttonItem_SpawnDefaultViewer = new TD.SandBar.ButtonItem();
			this.toolBar_ParticleSystem = new TD.SandBar.ToolBar();
			this.button_PartSys_DrawMode = new TD.SandBar.ButtonItem();
			this.button_PartSys_TreeView = new TD.SandBar.ButtonItem();
			this.button_PS_Ramachandran = new TD.SandBar.ButtonItem();
			this.button_PS_Camera = new TD.SandBar.ButtonItem();
			this.toolBar_MDIActiveDoc = new TD.SandBar.ToolBar();
			this.label_ActiveMDIDoc = new TD.SandBar.LabelItem();
			this.m_DockHost_Right = new DockingSuite.DockHost();
			this.m_DocPanel_RightTop = new DockingSuite.DockPanel();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.m_DockHost_Bottom = new DockingSuite.DockHost();
			this.menu_File_BrowseDir = new TD.SandBar.MenuButtonItem();
			this.topSandBarDock.SuspendLayout();
			this.m_DockHost_Right.SuspendLayout();
			this.SuspendLayout();
			// 
			// sandBarManager
			// 
			this.sandBarManager.BottomContainer = this.bottomSandBarDock;
			this.sandBarManager.LeftContainer = this.leftSandBarDock;
			this.sandBarManager.OwnerForm = this;
			this.sandBarManager.RightContainer = this.rightSandBarDock;
			this.sandBarManager.TopContainer = this.topSandBarDock;
			// 
			// bottomSandBarDock
			// 
			this.bottomSandBarDock.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bottomSandBarDock.Location = new System.Drawing.Point(0, 493);
			this.bottomSandBarDock.Manager = this.sandBarManager;
			this.bottomSandBarDock.Name = "bottomSandBarDock";
			this.bottomSandBarDock.Size = new System.Drawing.Size(728, 0);
			this.bottomSandBarDock.TabIndex = 2;
			// 
			// leftSandBarDock
			// 
			this.leftSandBarDock.Dock = System.Windows.Forms.DockStyle.Left;
			this.leftSandBarDock.Location = new System.Drawing.Point(0, 50);
			this.leftSandBarDock.Manager = this.sandBarManager;
			this.leftSandBarDock.Name = "leftSandBarDock";
			this.leftSandBarDock.Size = new System.Drawing.Size(0, 443);
			this.leftSandBarDock.TabIndex = 0;
			// 
			// rightSandBarDock
			// 
			this.rightSandBarDock.Dock = System.Windows.Forms.DockStyle.Right;
			this.rightSandBarDock.Location = new System.Drawing.Point(728, 50);
			this.rightSandBarDock.Manager = this.sandBarManager;
			this.rightSandBarDock.Name = "rightSandBarDock";
			this.rightSandBarDock.Size = new System.Drawing.Size(0, 443);
			this.rightSandBarDock.TabIndex = 1;
			// 
			// topSandBarDock
			// 
			this.topSandBarDock.Controls.Add(this.toolBar_DataManager);
			this.topSandBarDock.Controls.Add(this.toolBar_Trajectory);
			this.topSandBarDock.Controls.Add(this.menuBar_Main);
			this.topSandBarDock.Controls.Add(this.toolBar_Main);
			this.topSandBarDock.Controls.Add(this.toolBar_ParticleSystem);
			this.topSandBarDock.Controls.Add(this.toolBar_MDIActiveDoc);
			this.topSandBarDock.Dock = System.Windows.Forms.DockStyle.Top;
			this.topSandBarDock.Location = new System.Drawing.Point(0, 0);
			this.topSandBarDock.Manager = this.sandBarManager;
			this.topSandBarDock.Name = "topSandBarDock";
			this.topSandBarDock.Size = new System.Drawing.Size(728, 50);
			this.topSandBarDock.TabIndex = 3;
			// 
			// toolBar_DataManager
			// 
			this.toolBar_DataManager.Buttons.AddRange(new TD.SandBar.ToolbarItemBase[] {
																						   this.buttonItem_TraDataBox,
																						   this.buttonItem_DataManager});
			this.toolBar_DataManager.Closable = false;
			this.toolBar_DataManager.DockLine = 1;
			this.toolBar_DataManager.DrawActionsButton = false;
			this.toolBar_DataManager.Guid = new System.Guid("99fcf52c-8a04-40a7-b027-9c6959f1daaf");
			this.toolBar_DataManager.ImageList = this.imageList;
			this.toolBar_DataManager.Location = new System.Drawing.Point(119, 24);
			this.toolBar_DataManager.Name = "toolBar_DataManager";
			this.toolBar_DataManager.Size = new System.Drawing.Size(58, 26);
			this.toolBar_DataManager.TabIndex = 6;
			this.toolBar_DataManager.Text = "DataManager";
			this.toolBar_DataManager.ButtonClick += new TD.SandBar.ToolBar.ButtonClickEventHandler(this.toolBar_DataManager_ButtonClick);
			// 
			// buttonItem_TraDataBox
			// 
			this.buttonItem_TraDataBox.ImageIndex = 5;
			this.buttonItem_TraDataBox.ToolTipText = "Data Box";
			// 
			// buttonItem_DataManager
			// 
			this.buttonItem_DataManager.ImageIndex = 4;
			this.buttonItem_DataManager.ToolTipText = "Data Manager";
			// 
			// imageList
			// 
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// toolBar_Trajectory
			// 
			this.toolBar_Trajectory.Buttons.AddRange(new TD.SandBar.ToolbarItemBase[] {
																						  this.buttonItem_TraScroller});
			this.toolBar_Trajectory.Closable = false;
			this.toolBar_Trajectory.DockLine = 1;
			this.toolBar_Trajectory.DockOffset = 1;
			this.toolBar_Trajectory.DrawActionsButton = false;
			this.toolBar_Trajectory.Guid = new System.Guid("99fcf52c-8a04-40a7-b027-9c6959f1daaf");
			this.toolBar_Trajectory.ImageList = this.imageList;
			this.toolBar_Trajectory.Location = new System.Drawing.Point(285, 24);
			this.toolBar_Trajectory.Name = "toolBar_Trajectory";
			this.toolBar_Trajectory.Size = new System.Drawing.Size(35, 26);
			this.toolBar_Trajectory.TabIndex = 5;
			this.toolBar_Trajectory.Text = "Trajectory";
			this.toolBar_Trajectory.ButtonClick += new TD.SandBar.ToolBar.ButtonClickEventHandler(this.toolBar_Trajectory_ButtonClick);
			// 
			// buttonItem_TraScroller
			// 
			this.buttonItem_TraScroller.ImageIndex = 8;
			this.buttonItem_TraScroller.ToolTipText = "Trajectory Scroller";
			// 
			// menuBar_Main
			// 
			this.menuBar_Main.Buttons.AddRange(new TD.SandBar.ToolbarItemBase[] {
																					this.menu_File,
																					this.menu_Tools,
																					this.menu_Interop,
																					this.menu_Builder,
																					this.menu_Comms,
																					this.menu_Window,
																					this.menu_Help});
			this.menuBar_Main.Guid = new System.Guid("0e71cc8d-ae2f-4104-be68-15f3bb6c1fe7");
			this.menuBar_Main.ImageList = this.imageList;
			this.menuBar_Main.Location = new System.Drawing.Point(2, 0);
			this.menuBar_Main.Name = "menuBar_Main";
			this.menuBar_Main.Size = new System.Drawing.Size(726, 24);
			this.menuBar_Main.TabIndex = 0;
			// 
			// menu_File
			// 
			this.menu_File.MenuItems.AddRange(new TD.SandBar.MenuButtonItem[] {
																				  this.menu_File_New,
																				  this.menu_File_OpenFromFile,
																				  this.menu_File_OpenFromDownload,
																				  this.menu_File_OpenFromCache,
																				  this.menu_File_BrowseDir,
																				  this.menu_File_Save,
																				  this.menu_File_Recent1,
																				  this.menu_File_Recent2,
																				  this.menu_File_Recent3,
																				  this.menu_File_Recent4,
																				  this.menu_File_RecentMore,
																				  this.menu_File_Exit});
			this.menu_File.Text = "&File";
			// 
			// menu_File_New
			// 
			this.menu_File_New.ImageIndex = 0;
			this.menu_File_New.Text = "&New";
			this.menu_File_New.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_New_Activate);
			// 
			// menu_File_OpenFromFile
			// 
			this.menu_File_OpenFromFile.BeginGroup = true;
			this.menu_File_OpenFromFile.ImageIndex = 1;
			this.menu_File_OpenFromFile.Text = "Open From &File";
			this.menu_File_OpenFromFile.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_OpenFile_Click);
			// 
			// menu_File_OpenFromDownload
			// 
			this.menu_File_OpenFromDownload.ImageIndex = 2;
			this.menu_File_OpenFromDownload.Text = "Open From &Download";
			this.menu_File_OpenFromDownload.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_OpenFromDownload_Activate);
			// 
			// menu_File_OpenFromCache
			// 
			this.menu_File_OpenFromCache.Text = "Open From &Cache";
			this.menu_File_OpenFromCache.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_OpenFromCache_Activate);
			// 
			// menu_File_Save
			// 
			this.menu_File_Save.BeginGroup = true;
			this.menu_File_Save.Enabled = false;
			this.menu_File_Save.ImageIndex = 3;
			this.menu_File_Save.Text = "Save";
			this.menu_File_Save.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_Save_Activate);
			// 
			// menu_File_Recent1
			// 
			this.menu_File_Recent1.BeginGroup = true;
			this.menu_File_Recent1.Text = "1 - ";
			this.menu_File_Recent1.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_Recent_Click);
			// 
			// menu_File_Recent2
			// 
			this.menu_File_Recent2.Text = "2 - ";
			this.menu_File_Recent2.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_Recent_Click);
			// 
			// menu_File_Recent3
			// 
			this.menu_File_Recent3.Text = "3 - ";
			this.menu_File_Recent3.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_Recent_Click);
			// 
			// menu_File_Recent4
			// 
			this.menu_File_Recent4.Text = "4 - ";
			this.menu_File_Recent4.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_Recent_Click);
			// 
			// menu_File_RecentMore
			// 
			this.menu_File_RecentMore.MenuItems.AddRange(new TD.SandBar.MenuButtonItem[] {
																							 this.menu_File_Recent5,
																							 this.menu_File_Recent6,
																							 this.menu_File_Recent7,
																							 this.menu_File_Recent8,
																							 this.menu_File_Recent9});
			this.menu_File_RecentMore.Text = "More";
			// 
			// menu_File_Recent5
			// 
			this.menu_File_Recent5.Text = "5 - ";
			this.menu_File_Recent5.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_Recent_Click);
			// 
			// menu_File_Recent6
			// 
			this.menu_File_Recent6.Text = "6 - ";
			this.menu_File_Recent6.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_Recent_Click);
			// 
			// menu_File_Recent7
			// 
			this.menu_File_Recent7.Text = "7 - ";
			this.menu_File_Recent7.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_Recent_Click);
			// 
			// menu_File_Recent8
			// 
			this.menu_File_Recent8.Text = "8 - ";
			this.menu_File_Recent8.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_Recent_Click);
			// 
			// menu_File_Recent9
			// 
			this.menu_File_Recent9.Text = "9 - ";
			this.menu_File_Recent9.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_Recent_Click);
			// 
			// menu_File_Exit
			// 
			this.menu_File_Exit.BeginGroup = true;
			this.menu_File_Exit.Text = "&Exit";
			this.menu_File_Exit.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.Exit_Click);
			// 
			// menu_Tools
			// 
			this.menu_Tools.MenuItems.AddRange(new TD.SandBar.MenuButtonItem[] {
																				   this.menu_Tools_SequenceBox,
																				   this.menu_Tools_Alignment,
																				   this.menu_Tools_AlignmentView,
																				   this.menu_Tools_Rotate,
																				   this.menu_Tools_AlignCurrentTRAEtoTarget});
			this.menu_Tools.Text = "Tools";
			// 
			// menu_Tools_SequenceBox
			// 
			this.menu_Tools_SequenceBox.Text = "SequenceBox";
			this.menu_Tools_SequenceBox.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_Tools_SequenceBox_Activate);
			// 
			// menu_Tools_Alignment
			// 
			this.menu_Tools_Alignment.Text = "Alignment";
			this.menu_Tools_Alignment.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_Tools_Alignment_Activate);
			// 
			// menu_Tools_AlignmentView
			// 
			this.menu_Tools_AlignmentView.Text = "Alignment View";
			this.menu_Tools_AlignmentView.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_Tools_AlignmentView_Activate);
			// 
			// menu_Tools_Rotate
			// 
			this.menu_Tools_Rotate.Text = "Continual Rotation";
			this.menu_Tools_Rotate.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_Tools_Rotate_Activate);
			// 
			// menu_Tools_AlignCurrentTRAEtoTarget
			// 
			this.menu_Tools_AlignCurrentTRAEtoTarget.Text = "Align Current TRAE to Target";
			this.menu_Tools_AlignCurrentTRAEtoTarget.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_Tools_AlignCurrentTRAEtoTarget_Activate);
			// 
			// menu_Interop
			// 
			this.menu_Interop.MenuItems.AddRange(new TD.SandBar.MenuButtonItem[] {
																					 this.menu_Interop_ExternalText,
																					 this.menu_Interop_LaunchDefaultViewer,
																					 this.menu_File_Remote,
																					 this.menu_Interop_Minimise});
			this.menu_Interop.Text = "Interop";
			// 
			// menu_Interop_ExternalText
			// 
			this.menu_Interop_ExternalText.ImageIndex = 5;
			this.menu_Interop_ExternalText.Text = "Launch External Text Viewer";
			this.menu_Interop_ExternalText.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_ExternalText_Activate);
			// 
			// menu_Interop_LaunchDefaultViewer
			// 
			this.menu_Interop_LaunchDefaultViewer.ImageIndex = 8;
			this.menu_Interop_LaunchDefaultViewer.Text = "Launch Default PDB Viewer";
			this.menu_Interop_LaunchDefaultViewer.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_LaunchDefaultViewer_Activate);
			// 
			// menu_File_Remote
			// 
			this.menu_File_Remote.BeginGroup = true;
			this.menu_File_Remote.Enabled = false;
			this.menu_File_Remote.Text = "&Remote Connection Request";
			// 
			// menu_Interop_Minimise
			// 
			this.menu_Interop_Minimise.BeginGroup = true;
			this.menu_Interop_Minimise.Text = "Invoke Energy Minimisation";
			this.menu_Interop_Minimise.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_Minimise_Activate);
			// 
			// menu_Builder
			// 
			this.menu_Builder.MenuItems.AddRange(new TD.SandBar.MenuButtonItem[] {
																					 this.menu_Builder_HomologyBuild,
																					 this.menu_Builder_RebuildAtoms,
																					 this.menu_Builder_ApplyMirror});
			this.menu_Builder.Text = "Builder";
			// 
			// menu_Builder_HomologyBuild
			// 
			this.menu_Builder_HomologyBuild.Icon = ((System.Drawing.Icon)(resources.GetObject("menu_Builder_HomologyBuild.Icon")));
			this.menu_Builder_HomologyBuild.Text = "Homology Builder";
			this.menu_Builder_HomologyBuild.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_HomologyBuild_Activate);
			// 
			// menu_Builder_RebuildAtoms
			// 
			this.menu_Builder_RebuildAtoms.Text = "Rebuild Atoms";
			this.menu_Builder_RebuildAtoms.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_Builder_RebuildAtoms_Activate);
			// 
			// menu_Builder_ApplyMirror
			// 
			this.menu_Builder_ApplyMirror.Text = "Mirror Current Protein";
			this.menu_Builder_ApplyMirror.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_Builder_ApplyMirror_Activate);
			// 
			// menu_Comms
			// 
			this.menu_Comms.MenuItems.AddRange(new TD.SandBar.MenuButtonItem[] {
																				   this.menu_Comms_Listening});
			this.menu_Comms.Text = "&Comms";
			// 
			// menu_Comms_Listening
			// 
			this.menu_Comms_Listening.Checked = true;
			this.menu_Comms_Listening.Text = "&Listening";
			this.menu_Comms_Listening.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_Comms_Listening_Activate);
			// 
			// menu_Window
			// 
			this.menu_Window.MdiWindowList = true;
			this.menu_Window.MenuItems.AddRange(new TD.SandBar.MenuButtonItem[] {
																					this.menu_Window_Cascade,
																					this.menuButtonItem1,
																					this.menuButtonItem2});
			this.menu_Window.Text = "&Window";
			// 
			// menu_Window_Cascade
			// 
			this.menu_Window_Cascade.Text = "&Cascade";
			this.menu_Window_Cascade.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.cascade_Click);
			// 
			// menuButtonItem1
			// 
			this.menuButtonItem1.Text = "Tile &Horisontally";
			this.menuButtonItem1.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.tileHorisontal_Click);
			// 
			// menuButtonItem2
			// 
			this.menuButtonItem2.Text = "Tile &Vertically";
			this.menuButtonItem2.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.tileVertical_Click);
			// 
			// menu_Help
			// 
			this.menu_Help.MenuItems.AddRange(new TD.SandBar.MenuButtonItem[] {
																				  this.menu_Help_About});
			this.menu_Help.Text = "&Help";
			// 
			// menu_Help_About
			// 
			this.menu_Help_About.ImageIndex = 6;
			this.menu_Help_About.Text = "&About";
			this.menu_Help_About.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_Help_About_Activate);
			// 
			// toolBar_Main
			// 
			this.toolBar_Main.Buttons.AddRange(new TD.SandBar.ToolbarItemBase[] {
																					this.button_Main_New,
																					this.button_Main_OpenDropdown,
																					this.buttonItem_SpawnFileView,
																					this.buttonItem_SpawnDefaultViewer});
			this.toolBar_Main.Closable = false;
			this.toolBar_Main.DockLine = 1;
			this.toolBar_Main.DrawActionsButton = false;
			this.toolBar_Main.Guid = new System.Guid("d50c38ae-a793-403d-b29a-971f8eccff26");
			this.toolBar_Main.ImageList = this.imageList;
			this.toolBar_Main.Location = new System.Drawing.Point(2, 24);
			this.toolBar_Main.Name = "toolBar_Main";
			this.toolBar_Main.Size = new System.Drawing.Size(115, 26);
			this.toolBar_Main.TabIndex = 1;
			this.toolBar_Main.Text = "Main Toolbar";
			this.toolBar_Main.ButtonClick += new TD.SandBar.ToolBar.ButtonClickEventHandler(this.toolBar_Main_ButtonClick);
			// 
			// button_Main_New
			// 
			this.button_Main_New.ImageIndex = 0;
			// 
			// button_Main_OpenDropdown
			// 
			this.button_Main_OpenDropdown.ImageIndex = 1;
			this.button_Main_OpenDropdown.MenuItems.AddRange(new TD.SandBar.MenuButtonItem[] {
																								 this.button_Main_FromFile,
																								 this.button_Main_FromDownload,
																								 this.button_Main_FromCache});
			this.button_Main_OpenDropdown.Text = "";
			// 
			// button_Main_FromFile
			// 
			this.button_Main_FromFile.ImageIndex = 1;
			this.button_Main_FromFile.Text = "From &File";
			// 
			// button_Main_FromDownload
			// 
			this.button_Main_FromDownload.ImageIndex = 2;
			this.button_Main_FromDownload.Text = "From &Download";
			this.button_Main_FromDownload.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.button_Main_FromDownload_Activate);
			// 
			// button_Main_FromCache
			// 
			this.button_Main_FromCache.Text = "From &Cache";
			this.button_Main_FromCache.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_OpenFromCache_Activate);
			// 
			// buttonItem_SpawnFileView
			// 
			this.buttonItem_SpawnFileView.ImageIndex = 5;
			this.buttonItem_SpawnFileView.ToolTipText = "View File In NotePad";
			// 
			// buttonItem_SpawnDefaultViewer
			// 
			this.buttonItem_SpawnDefaultViewer.ImageIndex = 8;
			this.buttonItem_SpawnDefaultViewer.ToolTipText = "Spawn the default file viewer";
			// 
			// toolBar_ParticleSystem
			// 
			this.toolBar_ParticleSystem.Buttons.AddRange(new TD.SandBar.ToolbarItemBase[] {
																							  this.button_PartSys_DrawMode,
																							  this.button_PartSys_TreeView,
																							  this.button_PS_Ramachandran,
																							  this.button_PS_Camera});
			this.toolBar_ParticleSystem.Closable = false;
			this.toolBar_ParticleSystem.DockLine = 1;
			this.toolBar_ParticleSystem.DockOffset = 1;
			this.toolBar_ParticleSystem.DrawActionsButton = false;
			this.toolBar_ParticleSystem.Guid = new System.Guid("99fcf52c-8a04-40a7-b027-9c6959f1daaf");
			this.toolBar_ParticleSystem.ImageList = this.imageList;
			this.toolBar_ParticleSystem.Location = new System.Drawing.Point(179, 24);
			this.toolBar_ParticleSystem.Name = "toolBar_ParticleSystem";
			this.toolBar_ParticleSystem.Size = new System.Drawing.Size(104, 26);
			this.toolBar_ParticleSystem.TabIndex = 2;
			this.toolBar_ParticleSystem.Text = "Particle System";
			this.toolBar_ParticleSystem.ButtonClick += new TD.SandBar.ToolBar.ButtonClickEventHandler(this.toolBar_ParticleSystem_ButtonClick);
			// 
			// button_PartSys_DrawMode
			// 
			this.button_PartSys_DrawMode.ImageIndex = 8;
			this.button_PartSys_DrawMode.ToolTipText = "ParticleSystem DrawMode Selection";
			// 
			// button_PartSys_TreeView
			// 
			this.button_PartSys_TreeView.ImageIndex = 10;
			this.button_PartSys_TreeView.ToolTipText = "ParticleSystem TreeView";
			// 
			// button_PS_Ramachandran
			// 
			this.button_PS_Ramachandran.ImageIndex = 8;
			this.button_PS_Ramachandran.ToolTipText = "Ramachandran";
			// 
			// button_PS_Camera
			// 
			this.button_PS_Camera.ImageIndex = 8;
			this.button_PS_Camera.ToolTipText = "Camera";
			// 
			// toolBar_MDIActiveDoc
			// 
			this.toolBar_MDIActiveDoc.Buttons.AddRange(new TD.SandBar.ToolbarItemBase[] {
																							this.label_ActiveMDIDoc});
			this.toolBar_MDIActiveDoc.Closable = false;
			this.toolBar_MDIActiveDoc.DockLine = 1;
			this.toolBar_MDIActiveDoc.DockOffset = 5;
			this.toolBar_MDIActiveDoc.DrawActionsButton = false;
			this.toolBar_MDIActiveDoc.Guid = new System.Guid("95a82358-c2fa-4410-a2af-e2c1e6e695a7");
			this.toolBar_MDIActiveDoc.Location = new System.Drawing.Point(322, 24);
			this.toolBar_MDIActiveDoc.Name = "toolBar_MDIActiveDoc";
			this.toolBar_MDIActiveDoc.Size = new System.Drawing.Size(107, 26);
			this.toolBar_MDIActiveDoc.TabIndex = 4;
			this.toolBar_MDIActiveDoc.Text = "Active MDI Document Label";
			// 
			// label_ActiveMDIDoc
			// 
			this.label_ActiveMDIDoc.Text = "Active MDI Doc : ";
			// 
			// m_DockHost_Right
			// 
			this.m_DockHost_Right.Controls.Add(this.m_DocPanel_RightTop);
			this.m_DockHost_Right.Dock = System.Windows.Forms.DockStyle.Right;
			this.m_DockHost_Right.Location = new System.Drawing.Point(536, 50);
			this.m_DockHost_Right.Name = "m_DockHost_Right";
			this.m_DockHost_Right.Size = new System.Drawing.Size(192, 443);
			this.m_DockHost_Right.TabIndex = 5;
			// 
			// m_DocPanel_RightTop
			// 
			this.m_DocPanel_RightTop.AutoHide = false;
			this.m_DocPanel_RightTop.DockedHeight = 443;
			this.m_DocPanel_RightTop.DockedWidth = 0;
			this.m_DocPanel_RightTop.Location = new System.Drawing.Point(4, 0);
			this.m_DocPanel_RightTop.Name = "m_DocPanel_RightTop";
			this.m_DocPanel_RightTop.SelectedTab = null;
			this.m_DocPanel_RightTop.Size = new System.Drawing.Size(188, 443);
			this.m_DocPanel_RightTop.TabIndex = 0;
			this.m_DocPanel_RightTop.Text = "Docked Panel";
			// 
			// m_DockHost_Bottom
			// 
			this.m_DockHost_Bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.m_DockHost_Bottom.Location = new System.Drawing.Point(0, 477);
			this.m_DockHost_Bottom.Name = "m_DockHost_Bottom";
			this.m_DockHost_Bottom.Size = new System.Drawing.Size(536, 16);
			this.m_DockHost_Bottom.TabIndex = 6;
			// 
			// menu_File_BrowseDir
			// 
			this.menu_File_BrowseDir.BeginGroup = true;
			this.menu_File_BrowseDir.ImageIndex = 10;
			this.menu_File_BrowseDir.Text = "Browse Directory";
			this.menu_File_BrowseDir.Activate += new TD.SandBar.MenuButtonItem.ActivateEventHandler(this.menu_File_BrowseDir_Activate);
			// 
			// DAVEMainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(728, 493);
			this.Controls.Add(this.m_DockHost_Bottom);
			this.Controls.Add(this.m_DockHost_Right);
			this.Controls.Add(this.leftSandBarDock);
			this.Controls.Add(this.rightSandBarDock);
			this.Controls.Add(this.bottomSandBarDock);
			this.Controls.Add(this.topSandBarDock);
			this.IsMdiContainer = true;
			this.Name = "DAVEMainForm";
			this.Text = "DAVE - Dynamic Atomic Visualisation Environment";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.DAVEMainForm_Closing);
			this.MdiChildActivate += new System.EventHandler(this.DAVEMainForm_MdiChildActivate);
			this.topSandBarDock.ResumeLayout(false);
			this.m_DockHost_Right.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void button_Main_FromDownload_Activate(object sender, System.EventArgs e)
		{
			openDownloadTool();
		}

		private void toolBar_Main_ButtonClick(object sender, TD.SandBar.ToolBarItemEventArgs e)
		{
			switch( toolBar_Main.Buttons.IndexOf(e.Item) )
			{
				case 0:
					New();
					break;
				case 1: // Open DrowDown Box
					TriggerFileOpenDialog();
					break;
				case 2:
					ExternalTextView();
					break;
				case 3:
					ExternalDefaultViewer();
					break;
				default:
					break;
			}		
		}

		private void toolBar_Trajectory_ButtonClick(object sender, TD.SandBar.ToolBarItemEventArgs e)
		{
			switch( toolBar_Trajectory.Buttons.IndexOf(e.Item) )
			{
				case 0:
					CallToolWindow( typeof( Tra_Scroll ), m_DocPanel_RightTop );
					break;
				default:
					break;
			}
		}

		private void toolBar_DataManager_ButtonClick(object sender, TD.SandBar.ToolBarItemEventArgs e)
		{
			switch( toolBar_DataManager.Buttons.IndexOf(e.Item) )
			{
				case 0:
					CallToolWindow( typeof( TraDataBox ), null );
					break;
				case 1:
					CallToolWindow( typeof( DM_Manager ), null );
					break;
				default:
					break;
			}		
		}

		private void menu_File_HomologyBuild_Activate(object sender, System.EventArgs e)
		{
			TriggerBuilder();		
		}

		private void menu_File_Save_Activate(object sender, System.EventArgs e)
		{
			m_ActiveDocument.Save();
		}

		private void menu_File_ExternalText_Activate(object sender, System.EventArgs e)
		{
			ExternalTextView();
		}

		private void ExternalTextView()
		{
			string tempPath = m_UoBInit.DefaultSharedPath + "temp/";
			if( !Directory.Exists( tempPath ) )
			{
				Directory.CreateDirectory( tempPath );
			}

			if ( m_ActiveDocument != null )
			{
				if ( m_ActiveDocument is PDBViewDoc )
				{
					PDBViewDoc d = (PDBViewDoc) m_ActiveDocument;
					if ( d.PDBFile != null )
					{
						string filePath = d.PDBFile.FullFilePath;
						if ( !File.Exists( filePath ) )
						{
							if( DialogResult.Yes == MessageBox.Show( this, "The stored filename no longer exists, would you like DAVE to write and open a temporary file based on the current ParticleSystem?", "Import Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) )
							{
								filePath = tempPath + d.particleSystem.Name + ".pdb";
								PDB.SaveNew( filePath, d.particleSystem );
							}
							else
							{
								return;
							}
						}
						ViewTextFile(filePath);					
					} 
					else
					{
						MessageBox.Show("No PDB file is associated with the active document");
					}
				}
				else if ( m_ActiveDocument is PSViewDoc )
				{
					PSViewDoc d = (PSViewDoc) m_ActiveDocument;
					if ( d.particleSystem != null )
					{
						string filePath = tempPath + d.particleSystem.Name + ".pdb";
						PDB.SaveNew( filePath, d.particleSystem );
						ViewTextFile(filePath);	
					}
				}
				else if ( m_ActiveDocument is TraViewDoc )
				{
					TraViewDoc d = (TraViewDoc) m_ActiveDocument;
					if ( d.Trajectory.particleSystem != null )
					{
						if( DialogResult.OK == MessageBox.Show(this,"Tra files are not text-based, however DAVE will generate a PDB style file for the current entry being displayed in the viewer.","Tra Output",MessageBoxButtons.OKCancel, MessageBoxIcon.Information) )
						{
							string filePath = tempPath + d.Trajectory.particleSystem.Name + ".pdb";
							PDB.SaveNew( filePath, d.Trajectory.particleSystem );
							ViewTextFile(filePath);	   
						}
					}
				}	
				else if ( m_ActiveDocument is DirectoryViewDoc )
				{
					DirectoryViewDoc d = (DirectoryViewDoc) m_ActiveDocument;
					if ( d.CurrentFile != null )
					{
						if( d.CurrentViewItem.IsTextbased )
						{
							ViewTextFile( d.CurrentFile.FullName );
						}
						else
						{
							if( DialogResult.OK == MessageBox.Show(this,"This file type is binary and not text-based. DAVE can generate a PDB style file for the current entry being displayed in the viewer. Is that OK?","Tra Output",MessageBoxButtons.OKCancel, MessageBoxIcon.Information) )
							{
								string filePath = tempPath + d.CurrentViewItem.Name + ".pdb";
								PDB.SaveNew( filePath, d.CurrentViewItem.particleSystem );
								ViewTextFile(filePath);	
							}
						}
					}
					else
					{
						MessageBox.Show("No file is currently being viewed");
					}
				}	
				else
				{
					MessageBox.Show("The current document type does not support text viewing");
				}
			}
			else
			{
				MessageBox.Show("A document must be active");
			}
		}

		private void ViewTextFile( string filePath )
		{
			Process p  = new Process();
			p.StartInfo.FileName = "NotePad.exe";
			if( m_UoBInit.ContainsKey( "TextEditor" ) )
			{
				string textEditorFileName = m_UoBInit.ValueOf( "TextEditor" );
				if( File.Exists( textEditorFileName ) )
				{
					p.StartInfo.FileName = textEditorFileName;
				}
			}
			p.StartInfo.Arguments = filePath;
			p.Start();
			Trace.WriteLine("Launched external editor for : " + filePath );
		}

		private void ViewPDBFile( string filePath, string fileName )
		{
			if( m_UoBInit.ContainsKey( "ExternalPDBViewer" ) )
			{
				string PDBViewerName = m_UoBInit.ValueOf( "ExternalPDBViewer" );
				if( File.Exists( PDBViewerName ) )
				{
					Process p  = new Process();
					p.StartInfo.FileName = PDBViewerName;
					FileInfo fi = new FileInfo( filePath );
					Directory.SetCurrentDirectory( filePath );
					p.StartInfo.Arguments = fileName;
					p.Start();
					Trace.WriteLine("Launched ini-File PDB Viewer for : " + filePath );
					return; // all done for the UoBIni method
				}
				else
				{
					// roll through to the default method...
				}
			}

			// default method
			Process pDef  = new Process();
			pDef.StartInfo.FileName = filePath;
			pDef.Start();
			Trace.WriteLine("Launched System-Default external viewer for : " + filePath );
		}

		private void ExternalDefaultViewer()
		{
			string tempPath = m_UoBInit.DefaultSharedPath + "temp/";
			if( !Directory.Exists( tempPath ) )
			{
				Directory.CreateDirectory( tempPath );
			}

			if ( m_ActiveDocument != null )
			{
				if ( m_ActiveDocument is PDBViewDoc )
				{
					PDBViewDoc d = (PDBViewDoc) m_ActiveDocument;
					if ( d.PDBFile != null )
					{
						FileInfo fi = new FileInfo(d.PDBFile.FullFilePath);
						string fileName = fi.Name;
						string filePath = fi.DirectoryName + @"/" + fileName;
						if ( !fi.Exists )
						{
							if( DialogResult.Yes == MessageBox.Show( this, "The stored filename no longer exists, would you like DAVE to write and open a temporary file based on the current ParticleSystem?", "Import Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) )
							{
								filePath = tempPath;
								fileName = d.particleSystem.Name + ".pdb";
								PDB.SaveNew( filePath + fileName, d.particleSystem );
							}
							else
							{
								return;
							}
						}
						ViewPDBFile(filePath, fileName);					
					} 
					else
					{
						MessageBox.Show("No PDB file is associated with the active document");
					}
				}
				else if ( m_ActiveDocument is PSViewDoc )
				{
					PSViewDoc d = (PSViewDoc) m_ActiveDocument;
					if ( d.particleSystem != null )
					{
						string fileName = d.particleSystem.Name + ".pdb";
						PDB.SaveNew( tempPath + fileName, d.particleSystem );
						ViewPDBFile(tempPath, fileName);
					}
				}
				else if ( m_ActiveDocument is TraViewDoc )
				{
					TraViewDoc d = (TraViewDoc) m_ActiveDocument;
					if ( d.Trajectory.particleSystem != null )
					{
						string fileName = d.Trajectory.particleSystem.Name + ".pdb";
						PDB.SaveNew( tempPath + fileName, d.Trajectory.particleSystem );
						ViewPDBFile(tempPath, fileName);   
					}
				}
				else if ( m_ActiveDocument is DirectoryViewDoc )
				{
					DirectoryViewDoc d = (DirectoryViewDoc) m_ActiveDocument;
					if ( d.CurrentFile != null )
					{
						if( d.CurrentViewItem.IsTextbased )
						{
							ViewPDBFile( d.CurrentFile.FullName, d.CurrentFile.Name );
						}
						else
						{
							string fileName = d.CurrentViewItem.Name + ".pdb";
							string filePath = tempPath + fileName;
							PDB.SaveNew( filePath, d.CurrentViewItem.particleSystem );
							ViewPDBFile( filePath, fileName );	
						}
					}
					else
					{
						MessageBox.Show("No file is currently being viewed");
					}
				}			
				else
				{
					MessageBox.Show("The current document type does not support text viewing");
				}
			}
			else
			{
				MessageBox.Show("A document must be active");
			}
		}

		private void menu_File_LaunchDefaultViewer_Activate(object sender, System.EventArgs e)
		{
			ExternalDefaultViewer();
		}

		private void menu_File_Minimise_Activate(object sender, System.EventArgs e)
		{
			ParticleSystem p = null;

			if ( m_ActiveDocument != null )
			{
				for( int i = 0; i < m_ActiveDocument.MemberCount; i++ )
				{
					if( m_ActiveDocument[i] is ParticleSystem )
					{
						p = (ParticleSystem) m_ActiveDocument[i];
					}
				}
			}
			else
			{
				MessageBox.Show("A document must be active");
				return;
			}
			if ( p == null )
			{
				MessageBox.Show("No ParticleSystem was found for the minimisation process.");
				return;
			}

			MinimizationInterface min = new MinimizationInterface( p );
			min.FileDone += new StringEvent( funcFileDoneHandler );
			min.MdiParent = this;
			min.Show();
		}

		private void menu_Builder_RebuildAtoms_Activate(object sender, System.EventArgs e)
		{
			ParticleSystem p = null;

			if ( m_ActiveDocument != null )
			{
				for( int i = 0; i < m_ActiveDocument.MemberCount; i++ )
				{
					if( m_ActiveDocument[i] is ParticleSystem )
					{
						p = (ParticleSystem) m_ActiveDocument[i];
					}
				}
			}
			else
			{
				MessageBox.Show("A document must be active");
				return;
			}
			if ( p == null )
			{
				MessageBox.Show("No ParticleSystem was found for the rebuilding process.");
				return;
			}

			AtomRebuild rebuilder = new AtomRebuild( p );
			rebuilder.ShowDialog( this );

		}

		private void menu_Tools_SequenceBox_Activate(object sender, System.EventArgs e)
		{
			CallToolWindow( typeof( PS_SequenceBox ), m_DocPanel_RightTop );		
		}

		private void menu_Tools_Alignment_Activate(object sender, System.EventArgs e)
		{
			PSAlign align = new PSAlign( m_Documents );
			if( align.ShowDialog() == DialogResult.OK  )
			{
				ModelViewDoc viewDoc = new ModelViewDoc( align.SystemDefinition.ModelStore );
				viewDoc.AddMember( align.SystemDefinition.Models ); // add the models that we have found, so alignment can be viewed
				AddDocument( viewDoc );
			}
		}

		private void menu_Tools_Rotate_Activate(object sender, System.EventArgs e)
		{
			CallToolWindow( typeof(PS_Rotation), m_DocPanel_RightTop );
		}

		private void CallToolWindow( Type theToolType, DockPanel panel)
		{
			int index;
			if ( isToolPresent( theToolType, out index ) )
			{
				EnsureVisible( (ITool) m_ToolList[index] );
			}
			else
			{
				ITool tool = (ITool) Activator.CreateInstance( theToolType );
				tool.AttachToDocument( m_ActiveDocument ); 
				// only needs to be done once, subsequent attachments are done automaticallu when the active doc is changed
				if( panel != null )
				{
					SetupToolWindow(tool,panel);	
				}
				else
				{
					SetupToolWindow(tool);
				}
			}
		}

		private void menu_Tools_AlignCurrentTRAEtoTarget_Activate(object sender, System.EventArgs e)
		{
			if( m_ActiveDocument != null )
			{
				TraViewDoc doc = m_ActiveDocument as TraViewDoc;
				if( doc != null )
				{
					Tra trafile = doc.Trajectory;
					PSAlignManager aligner = new PSAlignManager(AlignmentMethod.Geometric,trafile.particleSystem.MemberAt(0).Count);
					aligner.ResetPSMolContainers(trafile,0,trafile.PositionDefinitions.Position);
					aligner.PerformAlignment();
					PSViewDoc view = new PSViewDoc( aligner.SystemDefinition.particleSystem );
					AddDocument(view);
				}
				else
				{
					MessageBox.Show( this, "No Trajectory is available in the current document.", "No System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
				}
			}
			else
			{
				MessageBox.Show( this, "No Document is currently active.", "No System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
			}
		}

		private void menu_Tools_AlignmentView_Activate(object sender, System.EventArgs e)
		{
			CallToolWindow( typeof(Model_EquivView), null );
		}

		private void menu_Builder_ApplyMirror_Activate(object sender, System.EventArgs e)
		{
			if ( m_ActiveDocument != null )
			{
				for( int i = 0; i < m_ActiveDocument.MemberCount; i++ )
				{
					if( m_ActiveDocument[i].GetType() == typeof(ParticleSystem) )
					{
						ParticleSystem ps = (ParticleSystem)m_ActiveDocument[i];
						ps.MirrorInXPlane();
						return;
					}
				}
			}
			else
			{
				MessageBox.Show( this, "No valid particle system is currently active.", "No System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
			}
		}

		private void menu_File_BrowseDir_Activate(object sender, System.EventArgs e)
		{
			DirectoryViewDoc viewDoc = new DirectoryViewDoc( this);
			AddDocument( viewDoc );
			viewDoc.ShowDirectorySelection();
		}

	}
}