using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

using UoB.Research;
using UoB.Research.Primitives;
using UoB.Generic.OpenGLView;
using UoB.Generic.PS_Render;
using UoB.Research.AtomicRepresentations;
using UoB.Research.PDB;
using UoB.Research.Detection;
using UoB.Research.Alignment;

namespace DatabaseViewer
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private PDB m_PDBFile = null;
		private PDB m_PDBFile2 = null;
		private GLView view = new GLView();
		private GLView view2 = new GLView();
		private ParticleSystemDrawWrapper m_DrawWrapper = null;
		private ParticleSystemDrawWrapper m_DrawWrapper2 = null;
		private ParticleSystem m_PartSys = null;
		private ParticleSystem m_PartSys2 = null;
		private DataSet ds = null;
		private DataTable dt = null;

		// Paths For All Modes
		private string m_RootPath = @"C:\_Gen Ig Database 29.07.04\";
		private string m_ToAddPath;
		private string m_DatabasePath;
		private string m_PDBPath;
		private string m_SourcePDBPath;
		private string m_AlignOutputPath;

		// Builder : Mol Selection State Trackers
		private bool editingValues = false;
		private bool m_DimerDefineMode = false;

		private int startIndex1_Available = 0;
		private int endIndex1_Available = 0;
		private int startIndex1 = 0;
		private int endIndex1 = 0;
		private PSMolContainer currentMol1 = null;

		private int startIndex2_Available = 0;
		private int endIndex2_Available = 0;
		private int startIndex2 = 0;
		private int endIndex2 = 0;


		private System.Windows.Forms.Button button_prev;
		private System.Windows.Forms.Button button_Next;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button button_Launch;
		private System.Windows.Forms.Button button_text;
		private System.Windows.Forms.Button button_GetFromDir;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.TextBox text_CommentBlock;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button_AddDefinition;
		private System.Windows.Forms.ComboBox combo_SegTypes;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button button_RemoveRow;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.NumericUpDown numeric_End1;
		private System.Windows.Forms.NumericUpDown numeric_Start1;
		private System.Windows.Forms.ListBox listBox_Monomer1;
		private System.Windows.Forms.ListBox listBox_Monomer2;
		private System.Windows.Forms.ListBox listBox_FileList;
		private System.Windows.Forms.NumericUpDown numeric_End2;
		private System.Windows.Forms.NumericUpDown numeric_Start2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button button_DoneWithFile;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Label label_Monomer1ChainIDStart;
		private System.Windows.Forms.Label label_Monomer1ChainIDEnd;
		private System.Windows.Forms.Label label_Monomer2ChainIDStart;
		private System.Windows.Forms.Label label_Monomer2ChainIDEnd;
		private System.Windows.Forms.Button button_AutoDomain;
		private System.Windows.Forms.NumericUpDown numeric_AutoDomain;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button button_FABFullAuto;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox check_SetCurrentChains01;
		private System.Windows.Forms.Button button_UpdateComment;
		private System.Windows.Forms.Button button_PDBWeb;
		private System.Windows.Forms.NumericUpDown numeric_EndLimitMol1;
		private System.Windows.Forms.NumericUpDown numeric_StartLimitMol1;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.NumericUpDown numeric_EndLimitMol2;
		private System.Windows.Forms.NumericUpDown numeric_StartLimitMol2;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Panel panel_Full;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.DataGrid dataGrid;
		private System.Windows.Forms.Panel panel_BuilderToolList;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.Panel panel_BuilderBody;
		private System.Windows.Forms.Panel panel_BuilderViewSlot;
		private System.Windows.Forms.Button button_Save;
		private System.Windows.Forms.Panel panel_View_DataBase;
		private System.Windows.Forms.Panel panel_View_Bottom;
		private System.Windows.Forms.Panel panel_View_GL1;
		private System.Windows.Forms.Panel panel_View_GL2;
		private System.Windows.Forms.Splitter splitter4;
		private System.Windows.Forms.Splitter splitter5;
		private System.Windows.Forms.Splitter splitter3;
		private System.Windows.Forms.Panel panel_Build_DataBase;
		private System.Windows.Forms.Button button_GetViews;
		private System.Windows.Forms.Button button_NextRow;
		private PSMolContainer currentMol2 = null;

		private void tempValidateFunction()
		{

			DataRowCollection IGRows = ds.Tables["IGDomains"].Rows;
			DataRowCollection PDBRows = ds.Tables["PDBParents"].Rows;

			FASTA fasta = new FASTA();

			for( int i = 0; i < IGRows.Count; i++ )
			{
				string parent = (string)IGRows[i]["ParentID"];
				for( int j = 0; j < PDBRows.Count; j++ )
				{
					string PDBID = (string)PDBRows[j]["SourcePDBFileID"];
					if( parent == PDBID )
					{
						float resolution = (float)PDBRows[j]["Resolution"];
						if( resolution < 0.0f )// nmr structures are -999.0
						{
							break;
						}
						else if( resolution < 2.5f )
						{
							break;
						}
						else
						{
							// write a FASTALine
							fasta.AddSequence( (string)IGRows[i]["DatabaseID"], (string)IGRows[i]["Sequence"] );
							break;
						}
					}
				}

			}

			fasta.WriteFile( this.m_AlignOutputPath + "out.ali" );
		}

		public Form1()
		{

			m_ToAddPath = m_RootPath + @"ToAdd\";			
			m_PDBPath = m_RootPath + @"PDB\";
			m_SourcePDBPath = m_RootPath + @"SourcePDB\";
			m_DatabasePath = m_RootPath + @"DataBase\";
			m_AlignOutputPath = m_RootPath + @"Align\";

            InitializeComponent();

			WindowState = FormWindowState.Maximized;
			
			m_DrawWrapper = new ParticleSystemDrawWrapper( null, view );
			m_DrawWrapper2 = new ParticleSystemDrawWrapper( null, view2 );

			view.Parent = panel_BuilderViewSlot;
			view.Dock = DockStyle.Fill;

			view2.Parent = panel_View_GL2;
			view2.Dock = DockStyle.Fill;

			view.LinkPerspective( view2 );
			view2.LinkPerspective( view );

            // Setup the seg box
			string[] segNames = Enum.GetNames( typeof(SegReason) );
			for( int i = 0; i < segNames.Length; i++ )
			{
				combo_SegTypes.Items.Add( segNames[i] );
			}
			combo_SegTypes.SelectedIndex = 0;
			try
			{
				combo_SegTypes.SelectedIndex = 4;
			}
			catch
			{
			}

			populateFileList();

			ds = new DataSet("IGDatabase");
			ds.ReadXmlSchema( m_DatabasePath + "IGschema.xsd" );
			ds.ReadXml( m_DatabasePath + "IGData.xml" );
            dt = ds.Tables[0];
			dataGrid.DataSource = ds;

			//tempValidateFunction();

		}

		private void populateFileList()
		{
			DirectoryInfo di = new DirectoryInfo( m_ToAddPath );
			FileInfo[] files = di.GetFiles("*.pdb");

			listBox_FileList.BeginUpdate();
			listBox_FileList.Items.Clear();

			if( files.Length > 0 )
			{
				foreach( FileInfo fi in files )
				{
					listBox_FileList.Items.Add(fi);
				}
				listBox_FileList.SelectedIndex = 0;
			}

			listBox_FileList.EndUpdate();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			this.button_prev = new System.Windows.Forms.Button();
			this.button_Next = new System.Windows.Forms.Button();
			this.listBox_FileList = new System.Windows.Forms.ListBox();
			this.button_PDBWeb = new System.Windows.Forms.Button();
			this.button_UpdateComment = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.check_SetCurrentChains01 = new System.Windows.Forms.CheckBox();
			this.label10 = new System.Windows.Forms.Label();
			this.button_FABFullAuto = new System.Windows.Forms.Button();
			this.numeric_AutoDomain = new System.Windows.Forms.NumericUpDown();
			this.label8 = new System.Windows.Forms.Label();
			this.button_AutoDomain = new System.Windows.Forms.Button();
			this.label9 = new System.Windows.Forms.Label();
			this.button_DoneWithFile = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label15 = new System.Windows.Forms.Label();
			this.numeric_EndLimitMol2 = new System.Windows.Forms.NumericUpDown();
			this.numeric_StartLimitMol2 = new System.Windows.Forms.NumericUpDown();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.numeric_EndLimitMol1 = new System.Windows.Forms.NumericUpDown();
			this.numeric_StartLimitMol1 = new System.Windows.Forms.NumericUpDown();
			this.label13 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.label_Monomer2ChainIDEnd = new System.Windows.Forms.Label();
			this.label_Monomer2ChainIDStart = new System.Windows.Forms.Label();
			this.label_Monomer1ChainIDEnd = new System.Windows.Forms.Label();
			this.label_Monomer1ChainIDStart = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.numeric_End1 = new System.Windows.Forms.NumericUpDown();
			this.listBox_Monomer1 = new System.Windows.Forms.ListBox();
			this.numeric_Start1 = new System.Windows.Forms.NumericUpDown();
			this.numeric_End2 = new System.Windows.Forms.NumericUpDown();
			this.numeric_Start2 = new System.Windows.Forms.NumericUpDown();
			this.listBox_Monomer2 = new System.Windows.Forms.ListBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.button_RemoveRow = new System.Windows.Forms.Button();
			this.combo_SegTypes = new System.Windows.Forms.ComboBox();
			this.button_AddDefinition = new System.Windows.Forms.Button();
			this.text_CommentBlock = new System.Windows.Forms.TextBox();
			this.button_GetFromDir = new System.Windows.Forms.Button();
			this.button_text = new System.Windows.Forms.Button();
			this.button_Launch = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel_Full = new System.Windows.Forms.Panel();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.panel_BuilderBody = new System.Windows.Forms.Panel();
			this.panel_BuilderViewSlot = new System.Windows.Forms.Panel();
			this.dataGrid = new System.Windows.Forms.DataGrid();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.panel_BuilderToolList = new System.Windows.Forms.Panel();
			this.button_Save = new System.Windows.Forms.Button();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.panel_View_DataBase = new System.Windows.Forms.Panel();
			this.panel_View_Bottom = new System.Windows.Forms.Panel();
			this.panel_View_GL1 = new System.Windows.Forms.Panel();
			this.panel_View_GL2 = new System.Windows.Forms.Panel();
			this.splitter4 = new System.Windows.Forms.Splitter();
			this.splitter5 = new System.Windows.Forms.Splitter();
			this.splitter3 = new System.Windows.Forms.Splitter();
			this.panel_Build_DataBase = new System.Windows.Forms.Panel();
			this.button_GetViews = new System.Windows.Forms.Button();
			this.button_NextRow = new System.Windows.Forms.Button();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numeric_AutoDomain)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numeric_EndLimitMol2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numeric_StartLimitMol2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numeric_EndLimitMol1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numeric_StartLimitMol1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numeric_End1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numeric_Start1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numeric_End2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numeric_Start2)).BeginInit();
			this.panel_Full.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.panel_BuilderBody.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
			this.panel_BuilderToolList.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.panel_View_Bottom.SuspendLayout();
			this.panel_View_GL1.SuspendLayout();
			this.panel_Build_DataBase.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_prev
			// 
			this.button_prev.Location = new System.Drawing.Point(8, 8);
			this.button_prev.Name = "button_prev";
			this.button_prev.Size = new System.Drawing.Size(24, 23);
			this.button_prev.TabIndex = 0;
			this.button_prev.Text = "<";
			this.button_prev.Click += new System.EventHandler(this.button_prev_Click);
			// 
			// button_Next
			// 
			this.button_Next.Location = new System.Drawing.Point(40, 8);
			this.button_Next.Name = "button_Next";
			this.button_Next.Size = new System.Drawing.Size(24, 23);
			this.button_Next.TabIndex = 1;
			this.button_Next.Text = ">";
			this.button_Next.Click += new System.EventHandler(this.button_Next_Click);
			// 
			// listBox_FileList
			// 
			this.listBox_FileList.Location = new System.Drawing.Point(144, 8);
			this.listBox_FileList.Name = "listBox_FileList";
			this.listBox_FileList.Size = new System.Drawing.Size(184, 95);
			this.listBox_FileList.TabIndex = 3;
			this.listBox_FileList.SelectedIndexChanged += new System.EventHandler(this.listBox_FileList_SelectedIndexChanged);
			// 
			// button_PDBWeb
			// 
			this.button_PDBWeb.Location = new System.Drawing.Point(8, 72);
			this.button_PDBWeb.Name = "button_PDBWeb";
			this.button_PDBWeb.Size = new System.Drawing.Size(120, 23);
			this.button_PDBWeb.TabIndex = 39;
			this.button_PDBWeb.Text = "PDB on Web";
			this.button_PDBWeb.Click += new System.EventHandler(this.button_PDBWeb_Click);
			// 
			// button_UpdateComment
			// 
			this.button_UpdateComment.Location = new System.Drawing.Point(168, 680);
			this.button_UpdateComment.Name = "button_UpdateComment";
			this.button_UpdateComment.Size = new System.Drawing.Size(152, 23);
			this.button_UpdateComment.TabIndex = 38;
			this.button_UpdateComment.Text = "Update Row Comment";
			this.button_UpdateComment.Click += new System.EventHandler(this.button_UpdateComment_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.check_SetCurrentChains01);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.button_FABFullAuto);
			this.groupBox2.Controls.Add(this.numeric_AutoDomain);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.button_AutoDomain);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Location = new System.Drawing.Point(8, 384);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(320, 160);
			this.groupBox2.TabIndex = 37;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "FAB Automation";
			// 
			// check_SetCurrentChains01
			// 
			this.check_SetCurrentChains01.Checked = true;
			this.check_SetCurrentChains01.CheckState = System.Windows.Forms.CheckState.Checked;
			this.check_SetCurrentChains01.Location = new System.Drawing.Point(8, 128);
			this.check_SetCurrentChains01.Name = "check_SetCurrentChains01";
			this.check_SetCurrentChains01.Size = new System.Drawing.Size(192, 24);
			this.check_SetCurrentChains01.TabIndex = 49;
			this.check_SetCurrentChains01.Text = "AutoSet Current chains";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(8, 88);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(304, 32);
			this.label10.TabIndex = 48;
			this.label10.Text = "Automatically perform auto for both pairs in an FAB fragment and add to database " +
				"- use with caution";
			// 
			// button_FABFullAuto
			// 
			this.button_FABFullAuto.Location = new System.Drawing.Point(208, 128);
			this.button_FABFullAuto.Name = "button_FABFullAuto";
			this.button_FABFullAuto.Size = new System.Drawing.Size(96, 23);
			this.button_FABFullAuto.TabIndex = 46;
			this.button_FABFullAuto.Text = "FAB Full Auto";
			this.button_FABFullAuto.Click += new System.EventHandler(this.button_FABFullAuto_Click);
			// 
			// numeric_AutoDomain
			// 
			this.numeric_AutoDomain.Location = new System.Drawing.Point(48, 64);
			this.numeric_AutoDomain.Maximum = new System.Decimal(new int[] {
																			   4,
																			   0,
																			   0,
																			   0});
			this.numeric_AutoDomain.Name = "numeric_AutoDomain";
			this.numeric_AutoDomain.Size = new System.Drawing.Size(72, 20);
			this.numeric_AutoDomain.TabIndex = 44;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(8, 16);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(304, 48);
			this.label8.TabIndex = 45;
			this.label8.Tag = "";
			this.label8.Text = "Monomer uses index to identify the domain in a string and Dimer uses that domain " +
				"with AutoDomain Pairing in the second strand";
			// 
			// button_AutoDomain
			// 
			this.button_AutoDomain.Location = new System.Drawing.Point(128, 64);
			this.button_AutoDomain.Name = "button_AutoDomain";
			this.button_AutoDomain.TabIndex = 43;
			this.button_AutoDomain.Text = "Auto";
			this.button_AutoDomain.Click += new System.EventHandler(this.button_AutoDomain_Click);
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(8, 64);
			this.label9.Name = "label9";
			this.label9.TabIndex = 47;
			this.label9.Text = "Index : ";
			// 
			// button_DoneWithFile
			// 
			this.button_DoneWithFile.Location = new System.Drawing.Point(8, 680);
			this.button_DoneWithFile.Name = "button_DoneWithFile";
			this.button_DoneWithFile.Size = new System.Drawing.Size(152, 23);
			this.button_DoneWithFile.TabIndex = 36;
			this.button_DoneWithFile.Text = "Done With File";
			this.button_DoneWithFile.Click += new System.EventHandler(this.button_DoneWithFile_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label15);
			this.groupBox1.Controls.Add(this.numeric_EndLimitMol2);
			this.groupBox1.Controls.Add(this.numeric_StartLimitMol2);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.Controls.Add(this.label12);
			this.groupBox1.Controls.Add(this.numeric_EndLimitMol1);
			this.groupBox1.Controls.Add(this.numeric_StartLimitMol1);
			this.groupBox1.Controls.Add(this.label13);
			this.groupBox1.Controls.Add(this.label14);
			this.groupBox1.Controls.Add(this.label_Monomer2ChainIDEnd);
			this.groupBox1.Controls.Add(this.label_Monomer2ChainIDStart);
			this.groupBox1.Controls.Add(this.label_Monomer1ChainIDEnd);
			this.groupBox1.Controls.Add(this.label_Monomer1ChainIDStart);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.numeric_End1);
			this.groupBox1.Controls.Add(this.listBox_Monomer1);
			this.groupBox1.Controls.Add(this.numeric_Start1);
			this.groupBox1.Controls.Add(this.numeric_End2);
			this.groupBox1.Controls.Add(this.numeric_Start2);
			this.groupBox1.Controls.Add(this.listBox_Monomer2);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(8, 136);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(320, 240);
			this.groupBox1.TabIndex = 35;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Define the molecules";
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(224, 8);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(104, 32);
			this.label15.TabIndex = 53;
			this.label15.Text = "AutoRange Sequence Limiters";
			// 
			// numeric_EndLimitMol2
			// 
			this.numeric_EndLimitMol2.Location = new System.Drawing.Point(248, 208);
			this.numeric_EndLimitMol2.Name = "numeric_EndLimitMol2";
			this.numeric_EndLimitMol2.Size = new System.Drawing.Size(56, 20);
			this.numeric_EndLimitMol2.TabIndex = 52;
			this.numeric_EndLimitMol2.ValueChanged += new System.EventHandler(this.numeric_RangeLimiters_ValueChanged);
			// 
			// numeric_StartLimitMol2
			// 
			this.numeric_StartLimitMol2.Location = new System.Drawing.Point(248, 168);
			this.numeric_StartLimitMol2.Name = "numeric_StartLimitMol2";
			this.numeric_StartLimitMol2.Size = new System.Drawing.Size(56, 20);
			this.numeric_StartLimitMol2.TabIndex = 51;
			this.numeric_StartLimitMol2.ValueChanged += new System.EventHandler(this.numeric_RangeLimiters_ValueChanged);
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(248, 152);
			this.label11.Name = "label11";
			this.label11.TabIndex = 49;
			this.label11.Tag = "";
			this.label11.Text = "Between";
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(248, 192);
			this.label12.Name = "label12";
			this.label12.TabIndex = 50;
			this.label12.Tag = "";
			this.label12.Text = "And";
			// 
			// numeric_EndLimitMol1
			// 
			this.numeric_EndLimitMol1.Location = new System.Drawing.Point(248, 96);
			this.numeric_EndLimitMol1.Name = "numeric_EndLimitMol1";
			this.numeric_EndLimitMol1.Size = new System.Drawing.Size(56, 20);
			this.numeric_EndLimitMol1.TabIndex = 47;
			this.numeric_EndLimitMol1.ValueChanged += new System.EventHandler(this.numeric_RangeLimiters_ValueChanged);
			// 
			// numeric_StartLimitMol1
			// 
			this.numeric_StartLimitMol1.Location = new System.Drawing.Point(248, 56);
			this.numeric_StartLimitMol1.Name = "numeric_StartLimitMol1";
			this.numeric_StartLimitMol1.Size = new System.Drawing.Size(56, 20);
			this.numeric_StartLimitMol1.TabIndex = 46;
			this.numeric_StartLimitMol1.ValueChanged += new System.EventHandler(this.numeric_RangeLimiters_ValueChanged);
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(248, 40);
			this.label13.Name = "label13";
			this.label13.TabIndex = 44;
			this.label13.Tag = "";
			this.label13.Text = "Between";
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(248, 80);
			this.label14.Name = "label14";
			this.label14.TabIndex = 45;
			this.label14.Tag = "";
			this.label14.Text = "And";
			// 
			// label_Monomer2ChainIDEnd
			// 
			this.label_Monomer2ChainIDEnd.Location = new System.Drawing.Point(208, 208);
			this.label_Monomer2ChainIDEnd.Name = "label_Monomer2ChainIDEnd";
			this.label_Monomer2ChainIDEnd.TabIndex = 42;
			// 
			// label_Monomer2ChainIDStart
			// 
			this.label_Monomer2ChainIDStart.Location = new System.Drawing.Point(208, 168);
			this.label_Monomer2ChainIDStart.Name = "label_Monomer2ChainIDStart";
			this.label_Monomer2ChainIDStart.TabIndex = 41;
			// 
			// label_Monomer1ChainIDEnd
			// 
			this.label_Monomer1ChainIDEnd.Location = new System.Drawing.Point(208, 96);
			this.label_Monomer1ChainIDEnd.Name = "label_Monomer1ChainIDEnd";
			this.label_Monomer1ChainIDEnd.TabIndex = 40;
			// 
			// label_Monomer1ChainIDStart
			// 
			this.label_Monomer1ChainIDStart.Location = new System.Drawing.Point(208, 56);
			this.label_Monomer1ChainIDStart.Name = "label_Monomer1ChainIDStart";
			this.label_Monomer1ChainIDStart.TabIndex = 39;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 128);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(240, 16);
			this.label5.TabIndex = 38;
			this.label5.Text = "Chains Present In Monomer 2";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(240, 16);
			this.label1.TabIndex = 37;
			this.label1.Text = "Chains Present In Monomer 1";
			// 
			// numeric_End1
			// 
			this.numeric_End1.Location = new System.Drawing.Point(144, 96);
			this.numeric_End1.Name = "numeric_End1";
			this.numeric_End1.Size = new System.Drawing.Size(56, 20);
			this.numeric_End1.TabIndex = 23;
			this.numeric_End1.ValueChanged += new System.EventHandler(this.setRange);
			// 
			// listBox_Monomer1
			// 
			this.listBox_Monomer1.Location = new System.Drawing.Point(8, 40);
			this.listBox_Monomer1.Name = "listBox_Monomer1";
			this.listBox_Monomer1.Size = new System.Drawing.Size(128, 82);
			this.listBox_Monomer1.TabIndex = 15;
			this.listBox_Monomer1.SelectedIndexChanged += new System.EventHandler(this.listBox_Monomer1_SelectedIndexChanged);
			// 
			// numeric_Start1
			// 
			this.numeric_Start1.Location = new System.Drawing.Point(144, 56);
			this.numeric_Start1.Name = "numeric_Start1";
			this.numeric_Start1.Size = new System.Drawing.Size(56, 20);
			this.numeric_Start1.TabIndex = 22;
			this.numeric_Start1.ValueChanged += new System.EventHandler(this.setRange);
			// 
			// numeric_End2
			// 
			this.numeric_End2.Location = new System.Drawing.Point(144, 208);
			this.numeric_End2.Name = "numeric_End2";
			this.numeric_End2.Size = new System.Drawing.Size(56, 20);
			this.numeric_End2.TabIndex = 34;
			this.numeric_End2.ValueChanged += new System.EventHandler(this.setRange);
			// 
			// numeric_Start2
			// 
			this.numeric_Start2.Location = new System.Drawing.Point(144, 168);
			this.numeric_Start2.Name = "numeric_Start2";
			this.numeric_Start2.Size = new System.Drawing.Size(56, 20);
			this.numeric_Start2.TabIndex = 33;
			this.numeric_Start2.ValueChanged += new System.EventHandler(this.setRange);
			// 
			// listBox_Monomer2
			// 
			this.listBox_Monomer2.Location = new System.Drawing.Point(8, 152);
			this.listBox_Monomer2.Name = "listBox_Monomer2";
			this.listBox_Monomer2.Size = new System.Drawing.Size(128, 82);
			this.listBox_Monomer2.TabIndex = 30;
			this.listBox_Monomer2.SelectedIndexChanged += new System.EventHandler(this.listBox_Monomer2_SelectedIndexChanged);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(144, 152);
			this.label7.Name = "label7";
			this.label7.TabIndex = 31;
			this.label7.Tag = "";
			this.label7.Text = "Between";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(144, 192);
			this.label6.Name = "label6";
			this.label6.TabIndex = 32;
			this.label6.Tag = "";
			this.label6.Text = "And";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(144, 40);
			this.label2.Name = "label2";
			this.label2.TabIndex = 20;
			this.label2.Tag = "";
			this.label2.Text = "Between";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(144, 80);
			this.label3.Name = "label3";
			this.label3.TabIndex = 21;
			this.label3.Tag = "";
			this.label3.Text = "And";
			// 
			// button_RemoveRow
			// 
			this.button_RemoveRow.Location = new System.Drawing.Point(168, 648);
			this.button_RemoveRow.Name = "button_RemoveRow";
			this.button_RemoveRow.Size = new System.Drawing.Size(152, 23);
			this.button_RemoveRow.TabIndex = 28;
			this.button_RemoveRow.Text = "Remove Row";
			this.button_RemoveRow.Click += new System.EventHandler(this.button_RemoveRow_Click);
			// 
			// combo_SegTypes
			// 
			this.combo_SegTypes.Location = new System.Drawing.Point(136, 112);
			this.combo_SegTypes.Name = "combo_SegTypes";
			this.combo_SegTypes.Size = new System.Drawing.Size(184, 21);
			this.combo_SegTypes.TabIndex = 25;
			this.combo_SegTypes.SelectedIndexChanged += new System.EventHandler(this.combo_SegTypes_SelectedIndexChanged);
			// 
			// button_AddDefinition
			// 
			this.button_AddDefinition.Location = new System.Drawing.Point(8, 648);
			this.button_AddDefinition.Name = "button_AddDefinition";
			this.button_AddDefinition.Size = new System.Drawing.Size(152, 23);
			this.button_AddDefinition.TabIndex = 24;
			this.button_AddDefinition.Text = "Add Definition To Database";
			this.button_AddDefinition.Click += new System.EventHandler(this.button_AddDefinition_Click);
			// 
			// text_CommentBlock
			// 
			this.text_CommentBlock.Location = new System.Drawing.Point(8, 552);
			this.text_CommentBlock.Multiline = true;
			this.text_CommentBlock.Name = "text_CommentBlock";
			this.text_CommentBlock.Size = new System.Drawing.Size(320, 88);
			this.text_CommentBlock.TabIndex = 19;
			this.text_CommentBlock.Text = "Comment :";
			// 
			// button_GetFromDir
			// 
			this.button_GetFromDir.Location = new System.Drawing.Point(72, 8);
			this.button_GetFromDir.Name = "button_GetFromDir";
			this.button_GetFromDir.Size = new System.Drawing.Size(56, 23);
			this.button_GetFromDir.TabIndex = 13;
			this.button_GetFromDir.Text = "Get ->";
			this.button_GetFromDir.Click += new System.EventHandler(this.button_GetFromDir_Click);
			// 
			// button_text
			// 
			this.button_text.Location = new System.Drawing.Point(72, 40);
			this.button_text.Name = "button_text";
			this.button_text.Size = new System.Drawing.Size(56, 23);
			this.button_text.TabIndex = 9;
			this.button_text.Text = "Text";
			this.button_text.Click += new System.EventHandler(this.button_text_Click);
			// 
			// button_Launch
			// 
			this.button_Launch.Location = new System.Drawing.Point(8, 40);
			this.button_Launch.Name = "button_Launch";
			this.button_Launch.Size = new System.Drawing.Size(56, 23);
			this.button_Launch.TabIndex = 8;
			this.button_Launch.Text = "Launch";
			this.button_Launch.Click += new System.EventHandler(this.button_Launch_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 112);
			this.label4.Name = "label4";
			this.label4.TabIndex = 26;
			this.label4.Text = "Segregation Type";
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter1.Location = new System.Drawing.Point(861, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 917);
			this.splitter1.TabIndex = 8;
			this.splitter1.TabStop = false;
			// 
			// panel_Full
			// 
			this.panel_Full.Controls.Add(this.tabControl1);
			this.panel_Full.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Full.Location = new System.Drawing.Point(0, 0);
			this.panel_Full.Name = "panel_Full";
			this.panel_Full.Size = new System.Drawing.Size(864, 917);
			this.panel_Full.TabIndex = 0;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.ItemSize = new System.Drawing.Size(49, 18);
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(864, 917);
			this.tabControl1.TabIndex = 1;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.panel_BuilderBody);
			this.tabPage1.Controls.Add(this.splitter2);
			this.tabPage1.Controls.Add(this.panel_BuilderToolList);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(856, 891);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Building";
			// 
			// panel_BuilderBody
			// 
			this.panel_BuilderBody.Controls.Add(this.panel_Build_DataBase);
			this.panel_BuilderBody.Controls.Add(this.splitter3);
			this.panel_BuilderBody.Controls.Add(this.panel_BuilderViewSlot);
			this.panel_BuilderBody.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_BuilderBody.Location = new System.Drawing.Point(339, 0);
			this.panel_BuilderBody.Name = "panel_BuilderBody";
			this.panel_BuilderBody.Size = new System.Drawing.Size(517, 891);
			this.panel_BuilderBody.TabIndex = 42;
			// 
			// panel_BuilderViewSlot
			// 
			this.panel_BuilderViewSlot.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel_BuilderViewSlot.Location = new System.Drawing.Point(0, 0);
			this.panel_BuilderViewSlot.Name = "panel_BuilderViewSlot";
			this.panel_BuilderViewSlot.Size = new System.Drawing.Size(517, 704);
			this.panel_BuilderViewSlot.TabIndex = 2;
			// 
			// dataGrid
			// 
			this.dataGrid.DataMember = "";
			this.dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid.Location = new System.Drawing.Point(0, 0);
			this.dataGrid.Name = "dataGrid";
			this.dataGrid.ReadOnly = true;
			this.dataGrid.Size = new System.Drawing.Size(517, 184);
			this.dataGrid.TabIndex = 0;
			// 
			// splitter2
			// 
			this.splitter2.Location = new System.Drawing.Point(336, 0);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(3, 891);
			this.splitter2.TabIndex = 41;
			this.splitter2.TabStop = false;
			// 
			// panel_BuilderToolList
			// 
			this.panel_BuilderToolList.Controls.Add(this.button_Save);
			this.panel_BuilderToolList.Controls.Add(this.combo_SegTypes);
			this.panel_BuilderToolList.Controls.Add(this.button_GetFromDir);
			this.panel_BuilderToolList.Controls.Add(this.button_text);
			this.panel_BuilderToolList.Controls.Add(this.button_Launch);
			this.panel_BuilderToolList.Controls.Add(this.label4);
			this.panel_BuilderToolList.Controls.Add(this.button_prev);
			this.panel_BuilderToolList.Controls.Add(this.button_Next);
			this.panel_BuilderToolList.Controls.Add(this.listBox_FileList);
			this.panel_BuilderToolList.Controls.Add(this.button_PDBWeb);
			this.panel_BuilderToolList.Controls.Add(this.button_RemoveRow);
			this.panel_BuilderToolList.Controls.Add(this.button_AddDefinition);
			this.panel_BuilderToolList.Controls.Add(this.text_CommentBlock);
			this.panel_BuilderToolList.Controls.Add(this.button_UpdateComment);
			this.panel_BuilderToolList.Controls.Add(this.groupBox2);
			this.panel_BuilderToolList.Controls.Add(this.button_DoneWithFile);
			this.panel_BuilderToolList.Controls.Add(this.groupBox1);
			this.panel_BuilderToolList.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel_BuilderToolList.Location = new System.Drawing.Point(0, 0);
			this.panel_BuilderToolList.Name = "panel_BuilderToolList";
			this.panel_BuilderToolList.Size = new System.Drawing.Size(336, 891);
			this.panel_BuilderToolList.TabIndex = 40;
			// 
			// button_Save
			// 
			this.button_Save.Location = new System.Drawing.Point(168, 712);
			this.button_Save.Name = "button_Save";
			this.button_Save.Size = new System.Drawing.Size(96, 24);
			this.button_Save.TabIndex = 40;
			this.button_Save.Text = "SAVE";
			this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.splitter4);
			this.tabPage2.Controls.Add(this.panel_View_Bottom);
			this.tabPage2.Controls.Add(this.panel_View_DataBase);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(856, 891);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "View and Edit";
			// 
			// panel_View_DataBase
			// 
			this.panel_View_DataBase.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel_View_DataBase.Location = new System.Drawing.Point(0, 0);
			this.panel_View_DataBase.Name = "panel_View_DataBase";
			this.panel_View_DataBase.Size = new System.Drawing.Size(856, 208);
			this.panel_View_DataBase.TabIndex = 0;
			// 
			// panel_View_Bottom
			// 
			this.panel_View_Bottom.Controls.Add(this.splitter5);
			this.panel_View_Bottom.Controls.Add(this.panel_View_GL2);
			this.panel_View_Bottom.Controls.Add(this.panel_View_GL1);
			this.panel_View_Bottom.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_View_Bottom.Location = new System.Drawing.Point(0, 208);
			this.panel_View_Bottom.Name = "panel_View_Bottom";
			this.panel_View_Bottom.Size = new System.Drawing.Size(856, 683);
			this.panel_View_Bottom.TabIndex = 1;
			// 
			// panel_View_GL1
			// 
			this.panel_View_GL1.Controls.Add(this.button_NextRow);
			this.panel_View_GL1.Controls.Add(this.button_GetViews);
			this.panel_View_GL1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel_View_GL1.Location = new System.Drawing.Point(0, 0);
			this.panel_View_GL1.Name = "panel_View_GL1";
			this.panel_View_GL1.Size = new System.Drawing.Size(656, 683);
			this.panel_View_GL1.TabIndex = 0;
			// 
			// panel_View_GL2
			// 
			this.panel_View_GL2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_View_GL2.Location = new System.Drawing.Point(656, 0);
			this.panel_View_GL2.Name = "panel_View_GL2";
			this.panel_View_GL2.Size = new System.Drawing.Size(200, 683);
			this.panel_View_GL2.TabIndex = 1;
			// 
			// splitter4
			// 
			this.splitter4.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter4.Location = new System.Drawing.Point(0, 208);
			this.splitter4.Name = "splitter4";
			this.splitter4.Size = new System.Drawing.Size(856, 3);
			this.splitter4.TabIndex = 2;
			this.splitter4.TabStop = false;
			// 
			// splitter5
			// 
			this.splitter5.Location = new System.Drawing.Point(656, 0);
			this.splitter5.Name = "splitter5";
			this.splitter5.Size = new System.Drawing.Size(3, 683);
			this.splitter5.TabIndex = 2;
			this.splitter5.TabStop = false;
			// 
			// splitter3
			// 
			this.splitter3.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter3.Location = new System.Drawing.Point(0, 704);
			this.splitter3.Name = "splitter3";
			this.splitter3.Size = new System.Drawing.Size(517, 3);
			this.splitter3.TabIndex = 3;
			this.splitter3.TabStop = false;
			// 
			// panel_Build_DataBase
			// 
			this.panel_Build_DataBase.Controls.Add(this.dataGrid);
			this.panel_Build_DataBase.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Build_DataBase.Location = new System.Drawing.Point(0, 707);
			this.panel_Build_DataBase.Name = "panel_Build_DataBase";
			this.panel_Build_DataBase.Size = new System.Drawing.Size(517, 184);
			this.panel_Build_DataBase.TabIndex = 4;
			// 
			// button_GetViews
			// 
			this.button_GetViews.Location = new System.Drawing.Point(496, 8);
			this.button_GetViews.Name = "button_GetViews";
			this.button_GetViews.TabIndex = 0;
			this.button_GetViews.Text = "Get Views";
			this.button_GetViews.Click += new System.EventHandler(this.DataGridClicked);
			// 
			// button_NextRow
			// 
			this.button_NextRow.Location = new System.Drawing.Point(576, 8);
			this.button_NextRow.Name = "button_NextRow";
			this.button_NextRow.Size = new System.Drawing.Size(72, 24);
			this.button_NextRow.TabIndex = 1;
			this.button_NextRow.Text = "Next";
			this.button_NextRow.Click += new System.EventHandler(this.button_NextRow_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(864, 917);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel_Full);
			this.Name = "Form1";
			this.Text = "Form1";
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numeric_AutoDomain)).EndInit();
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numeric_EndLimitMol2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numeric_StartLimitMol2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numeric_EndLimitMol1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numeric_StartLimitMol1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numeric_End1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numeric_Start1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numeric_End2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numeric_Start2)).EndInit();
			this.panel_Full.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.panel_BuilderBody.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
			this.panel_BuilderToolList.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.panel_View_Bottom.ResumeLayout(false);
			this.panel_View_GL1.ResumeLayout(false);
			this.panel_Build_DataBase.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void button_prev_Click(object sender, System.EventArgs e)
		{
			if( listBox_FileList.SelectedIndex == 0 )
			{
				listBox_FileList.SetSelected( listBox_FileList.Items.Count -1, true );
			}
			else
			{
				listBox_FileList.SelectedIndex--;
			}
		}

		private void button_Next_Click(object sender, System.EventArgs e)
		{
			if( listBox_FileList.SelectedIndex == listBox_FileList.Items.Count -1 )
			{
				listBox_FileList.SetSelected( 0, true );
			}
			else
			{
				listBox_FileList.SelectedIndex++;
			}
		}

		private void listBox_FileList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			FileInfo fi = (FileInfo) listBox_FileList.Items[ listBox_FileList.SelectedIndex ];
			m_PDBFile = new PDB( fi.FullName, true);
			m_PartSys = m_PDBFile.particleSystem;
			m_DrawWrapper.particleSystem = m_PartSys;
			m_DrawWrapper.globalDisplayMode = AtomDisplayMode.Backbone;
			//m_DrawWrapper.globalDisplayMode = AtomDisplayMode.CAlphaTrace;
			//m_DrawWrapper.globalDrawMode = AtomDrawStyle.Ribbon;

			currentMol1 = null;
			currentMol2 = null;

			// now we want to extract the information on the available peptides
			listBox_Monomer1.Items.Clear();
			listBox_Monomer2.Items.Clear();
			PSMolContainer[] mols = m_PDBFile.particleSystem.Members;
			for( int i = 0; i < mols.Length; i++ )
			{
				listBox_Monomer1.Items.Add( mols[i] );
				listBox_Monomer2.Items.Add( mols[i] );
			}

			// predict the segType

			if( m_PartSys.Members.Length == 1 )
			{
				combo_SegTypes.SelectedIndex = 0; // single in crystal (or NMR)
				if( m_PartSys.Members[0].Count > 250 )
				{
					// then it is a multiple domain chain and prob a bead on string
					combo_SegTypes.SelectedIndex = 1;
				}
			}
			else if( m_PartSys.Members.Length == 2 )
			{
				bool zeroIsSolvent = (m_PartSys.Members[0] is Solvent);
				bool oneIsSolvent = (m_PartSys.Members[1] is Solvent);
				if( zeroIsSolvent )
				{
					combo_SegTypes.SelectedIndex = 0; // single in crystal (or NMR)
					if( m_PartSys.Members[1].Count > 250 )
					{
						// then it is a multiple domain chain and prob a bead on string
						combo_SegTypes.SelectedIndex = 1;
					}
				}
				else if ( oneIsSolvent )
				{
					combo_SegTypes.SelectedIndex = 0; // single in crystal (or NMR)
					if( m_PartSys.Members[0].Count > 250 )
					{
						// then it is a multiple domain chain and prob a bead on string
						combo_SegTypes.SelectedIndex = 1;
					}
				}
				else
				{
					// if the sequences are the same, then we prob have a crystal dimer
					string s1 = m_PartSys.Members[0].MonomerString;
					string s2 = m_PartSys.Members[1].MonomerString;
					if( s1 == s2 )
					{
						combo_SegTypes.SelectedIndex = 2; // crystal dimer
					}
				}
			}
		}

		private void button_Launch_Click(object sender, System.EventArgs e)
		{
			FileInfo fi = (FileInfo) listBox_FileList.Items[ listBox_FileList.SelectedIndex ];

			UoBInit init = UoBInit.Instance;
			string path = init.DefaultSharedPath + "interop/" + "weblabviewer/";
			string outPath = init.DefaultSharedPath + "temp/" + "weblab_ribbonview.wvc";
			UoB.Research.Tools.InputFile.Create( path + "weblab_ribbonview.twvc",
				outPath,
				new string[] { "filename" },
				new string[] { fi.FullName } );

			Process p = new Process();
			p.StartInfo.FileName = outPath;
			//p.StartInfo.Arguments = fi.FullName;
			//p.StartInfo.FileName = fi.FullName;
			p.Start();
		}

		private void button_text_Click(object sender, System.EventArgs e)
		{
			FileInfo fi = (FileInfo) listBox_FileList.Items[ listBox_FileList.SelectedIndex ];
			Process p = new Process();
			p.StartInfo.FileName = @"C:\Program Files\TextPad 4\TextPad.exe";
			p.StartInfo.Arguments = fi.FullName;
			p.Start();
		}

		private void button_GetFromDir_Click(object sender, System.EventArgs e)
		{
			folderBrowserDialog.SelectedPath = m_ToAddPath;
			if( folderBrowserDialog.ShowDialog() == DialogResult.OK )
			{
				m_ToAddPath = this.folderBrowserDialog.SelectedPath;
				populateFileList();
			}																	
		}

		private void listBox_Monomer1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			currentMol1 = (PSMolContainer) listBox_Monomer1.SelectedItem;

			startIndex1_Available = startIndex1 = 0;
			endIndex1_Available = endIndex1 = currentMol1.Count - 1;

			editingValues = true;
			numeric_StartLimitMol1.Minimum = numeric_Start1.Minimum = startIndex1_Available;
			numeric_StartLimitMol1.Maximum = numeric_Start1.Maximum = endIndex1_Available;
			numeric_EndLimitMol1.Minimum = numeric_End1.Minimum = startIndex1_Available;
			numeric_EndLimitMol1.Maximum = numeric_End1.Maximum = endIndex1_Available;
			numeric_StartLimitMol1.Value = numeric_Start1.Value = startIndex1_Available;
			numeric_EndLimitMol1.Value = numeric_End1.Value = endIndex1_Available;
			editingValues = false;
			setRange( null, null );
			// the value changes raise the setRange event below ...
			// colours will therefore be updated
		}

		private void listBox_Monomer2_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			currentMol2 = (PSMolContainer) listBox_Monomer2.SelectedItem;

			startIndex2_Available = startIndex2 = 0;
			endIndex2_Available = endIndex2 = currentMol2.Count - 1;

			editingValues = true;
			numeric_StartLimitMol2.Minimum = numeric_Start2.Minimum = startIndex2_Available;
			numeric_StartLimitMol2.Maximum = numeric_Start2.Maximum = endIndex2_Available;
			numeric_EndLimitMol2.Minimum = numeric_End2.Minimum = startIndex2_Available;
			numeric_EndLimitMol2.Maximum = numeric_End2.Maximum = endIndex2_Available;
			numeric_StartLimitMol2.Value = numeric_Start2.Value = startIndex2_Available;
			numeric_EndLimitMol2.Value = numeric_End2.Value = endIndex2_Available;
			editingValues = false;
			setRange( null, null );
			// the value changes raise the setRange event below ...
			// colours will therefore be updated
		}

		private void setRange(object sender, System.EventArgs e)
		{
			if( editingValues )
			{
				// while editing, values may not yet be valid for the repaint
				return;
			}
			startIndex1 = (int) numeric_Start1.Value;
			endIndex1 = (int) numeric_End1.Value;
			startIndex2 = (int) numeric_Start2.Value;
			endIndex2 = (int) numeric_End2.Value;

			if( currentMol1 != null )
			{
				label_Monomer1ChainIDStart.Text = currentMol1[ startIndex1 ].ResidueNumber.ToString();
				label_Monomer1ChainIDEnd.Text = currentMol1[ endIndex1 ].ResidueNumber.ToString();
			}
			else
			{
				label_Monomer1ChainIDStart.Text = "IsNull";
				label_Monomer1ChainIDEnd.Text = "IsNull";
			}
			if( currentMol2 != null )
			{
				label_Monomer2ChainIDStart.Text = currentMol2[ startIndex2 ].ResidueNumber.ToString();
				label_Monomer2ChainIDEnd.Text = currentMol2[ endIndex2 ].ResidueNumber.ToString();
			}
			else
			{
				label_Monomer2ChainIDStart.Text = "IsNull";
				label_Monomer2ChainIDEnd.Text = "IsNull";
			}
			
			// now we have a valid range, set the colours in the viewer
			m_DrawWrapper.colourEditingInProgress = true;
			m_DrawWrapper.setColourDefaults();

			if( currentMol1 != null )
			{
				for( int i = ( startIndex1 - startIndex1_Available ); i < ( (endIndex1_Available - startIndex1_Available + 1) - (endIndex1_Available - endIndex1) ); i++ )
				{
					if( i >= currentMol1.Count )
					{
						if (currentMol1 is Solvent )
						{
						}
						else
						{
							throw new Exception("Assumption failure");
						}
						break;
					}
					m_DrawWrapper.setColour( currentMol1[i].AtomIndexes, UoB.Research.Primitives.Colour.FromName("Green") );
				}
			}

			if( currentMol2 != null && m_DimerDefineMode )
			{
				for( int i = ( startIndex2 - startIndex2_Available ); i < ( (endIndex2_Available - startIndex2_Available + 1) - (endIndex2_Available - endIndex2) ); i++ )
				{
					if( i >= currentMol2.Count )
					{
						if (currentMol2 is Solvent )
						{
						}
						else
						{
							throw new Exception("Assumption failure");
						}
						break;
					}
					m_DrawWrapper.setColour( currentMol2[i].AtomIndexes, UoB.Research.Primitives.Colour.FromName("Orange") );
				}
			}

			m_DrawWrapper.colourEditingInProgress = false;
		}

		private void button_AddDefinition_Click(object sender, System.EventArgs e)
		{
			// IsValid,Filename,SourcePDBFileID,DatabaseID,ExperimentalMethod,SourceHeader,SourceChainID,WasTruncated,AAStart,AAEnd,Sequence,ReasonForSegregation,Comments
			
			FileInfo fi = new FileInfo( m_PDBFile.fullFilePath );
			string PDBID = fi.Name.Split( new char[] { '.' } )[0];

			if ( m_DimerDefineMode )
			{
				
				DataRow row = dt.NewRow();

				ParticleSystem cloneSystem1 = new ParticleSystem( PDBID + '_' + currentMol1.ChainID + '_' + label_Monomer1ChainIDStart.Text  + '-' + label_Monomer1ChainIDEnd.Text );
				PolyPeptide p1 = new PolyPeptide( currentMol1.ChainID );

				ParticleSystem cloneSystem2 = new ParticleSystem( PDBID + '_' + currentMol2.ChainID + '_' + label_Monomer2ChainIDStart.Text  + '-' + label_Monomer2ChainIDEnd.Text );
				PolyPeptide p2 = new PolyPeptide( currentMol2.ChainID );

				cloneSystem1.BeginEditing();
				for( int i = ( startIndex1 - startIndex1_Available ); i < ( (endIndex1_Available - startIndex1_Available + 1) - (endIndex1_Available - endIndex1) ); i++ )
				{
					p1.addMolecule( (AminoAcid) currentMol1[i].Clone() );
				}
				cloneSystem1.AddMolContainer( p1 );
				cloneSystem1.EndEditing(true,true);

				cloneSystem2.BeginEditing();
				for( int i = ( startIndex2 - startIndex2_Available ); i < ( (endIndex2_Available - startIndex2_Available + 1) - (endIndex2_Available - endIndex2) ); i++ )
				{
					p2.addMolecule( (AminoAcid) currentMol2[i].Clone() );
				}
				cloneSystem2.AddMolContainer( p2 );
				cloneSystem2.EndEditing(true,true);

				string saveName1 = m_DatabasePath + "PDBFiles/" + cloneSystem1.Name + ".pdb";
				PDB.WriteFile( saveName1, cloneSystem1 );

				string saveName2 = m_DatabasePath + "PDBFiles/" + cloneSystem2.Name + ".pdb";
				PDB.WriteFile( saveName2, cloneSystem2 );


				row["IsValid"] = true;
				row["Filename"] = saveName1;		
				row["SourcePDBFileID"] = PDBID;
				row["DatabaseID"] = cloneSystem1.Name;
				row["DimerPartnerDBID"] = cloneSystem2.Name;
				row["ExperimentalMethod"] = m_PDBFile.info.ResolutionMethod;
				row["SourceHeader"] = m_PDBFile.info.TitleBlock;
				char chainID1 = currentMol1.ChainID;
				if( chainID1 == ' ' )
				{
					chainID1 = '-';
				}
				row["SourceChainID"] = chainID1;
				row["WasTruncated"] = ( !(( startIndex1 == startIndex1_Available ) && ( endIndex1 == endIndex1_Available )) );
				row["AAStart"] = int.Parse( label_Monomer1ChainIDStart.Text );
				row["AAEnd"] = int.Parse( label_Monomer1ChainIDEnd.Text );
				row["Sequence"] = currentMol1.MonomerString.Substring( (startIndex1 - startIndex1_Available), ( endIndex1 - startIndex1 + 1 ) );
				row["ReasonForSegregation"] = (SegReason) Enum.Parse( typeof( SegReason ), (string)combo_SegTypes.SelectedItem, true );
				row["Comments"] = text_CommentBlock.Text;

				dt.Rows.Add(row);
				row = dt.NewRow();

							
				row["IsValid"] = true;
				row["Filename"] = saveName2;		
				row["SourcePDBFileID"] = PDBID;
				row["DatabaseID"] = cloneSystem2.Name;
				row["DimerPartnerDBID"] = cloneSystem1.Name;
				row["ExperimentalMethod"] = m_PDBFile.info.ResolutionMethod;
				row["SourceHeader"] = m_PDBFile.info.TitleBlock;
				char chainID2 = currentMol2.ChainID;
				if( chainID2 == ' ' )
				{
					chainID2 = '-';
				}
				row["SourceChainID"] = chainID2;
				row["WasTruncated"] = ( !(( startIndex2 == startIndex2_Available ) && ( endIndex2 == endIndex2_Available )) );
				row["AAStart"] = int.Parse( label_Monomer2ChainIDStart.Text );
				row["AAEnd"] = int.Parse( label_Monomer2ChainIDEnd.Text );
				row["Sequence"] = currentMol2.MonomerString.Substring( (startIndex2 - startIndex2_Available), ( endIndex2 - startIndex2 + 1 ) );
				row["ReasonForSegregation"] = (SegReason) Enum.Parse( typeof( SegReason ), (string)combo_SegTypes.SelectedItem, true );
				row["Comments"] = text_CommentBlock.Text;

				dt.Rows.Add(row);

			}
			else
			{
				DataRow row = dt.NewRow(); 

				ParticleSystem cloneSystem = new ParticleSystem( PDBID + '_' + currentMol1.ChainID + '_' + label_Monomer1ChainIDStart.Text  + '-' + label_Monomer1ChainIDEnd.Text );
				PolyPeptide p = new PolyPeptide( currentMol1.ChainID );

				cloneSystem.BeginEditing();
				for( int i = ( startIndex1 - startIndex1_Available ); i < ( (endIndex1_Available - startIndex1_Available + 1) - (endIndex1_Available - endIndex1) ); i++ )
				{
					p.addMolecule( (AminoAcid) currentMol1[i].Clone() );
				}
				cloneSystem.AddMolContainer( p );
				cloneSystem.EndEditing(true,true);

				string saveName = m_DatabasePath + "PDBFiles/" + cloneSystem.Name + ".pdb";
				PDB.WriteFile( saveName, cloneSystem );
			
				row["IsValid"] = true;
				row["Filename"] = saveName;		
				row["SourcePDBFileID"] = PDBID;
				row["DatabaseID"] = cloneSystem.Name;
				row["ExperimentalMethod"] = m_PDBFile.info.ResolutionMethod;
				row["SourceHeader"] = m_PDBFile.info.TitleBlock;
				char chainID = currentMol1.ChainID;
				if( chainID == ' ' )
				{
					chainID = '-';
				}
				row["SourceChainID"] = chainID;
				row["WasTruncated"] = ( !(( startIndex1 == startIndex1_Available ) && ( endIndex1 == endIndex1_Available )) );
				row["AAStart"] = int.Parse( label_Monomer1ChainIDStart.Text );
				row["AAEnd"] = int.Parse( label_Monomer1ChainIDEnd.Text );
				row["Sequence"] = currentMol1.MonomerString.Substring( (startIndex1 - startIndex1_Available), ( endIndex1 - startIndex1 + 1 ) );
				row["ReasonForSegregation"] = (SegReason) Enum.Parse( typeof( SegReason ), (string)combo_SegTypes.SelectedItem, true );
				row["Comments"] = text_CommentBlock.Text;

				dt.Rows.Add(row);
			}


			dt.AcceptChanges();

			ds.WriteXmlSchema( m_DatabasePath + "IGschema.xsd" );
			ds.WriteXml( m_DatabasePath + "IGData.xml" );
		}

		private void button_Save_Click(object sender, System.EventArgs e)
		{
			ds.WriteXmlSchema( m_DatabasePath + "IGschema.xsd" );
			ds.WriteXml( m_DatabasePath + "IGData.xml" );
		}

		private void button_RemoveRow_Click(object sender, System.EventArgs e)
		{
			if( DialogResult.Yes == MessageBox.Show( this, "Are you sure you want to remove?", "Remove?", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) )
			{
				DataRowCollection rc = dt.Rows;
				DataRow currentRow = rc[dataGrid.CurrentCell.RowNumber];
				currentRow.Delete();
				ds.AcceptChanges();
			}
		}

		private void button_DoneWithFile_Click(object sender, System.EventArgs e)
		{
			FileInfo fi = (FileInfo) listBox_FileList.Items[ listBox_FileList.SelectedIndex ];
			fi.MoveTo( m_DatabasePath + "UsedOriginal/" + fi.Name );
			populateFileList();
		}

		private Regex m_Regex = new Regex( "true" ,RegexOptions.IgnoreCase | RegexOptions.Compiled );
		private void combo_SegTypes_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if( m_Regex.IsMatch( (string) combo_SegTypes.SelectedItem ) )
			{
				SetMol2DefsActive( true );
			}
			else
			{
				SetMol2DefsActive( false );
			}
		}

		private void SetMol2DefsActive( bool active )
		{
			m_DimerDefineMode = active;
			listBox_Monomer2.Enabled = active;
			numeric_Start2.Enabled = active;
			numeric_End2.Enabled = active;
		}

		private void button_AutoDomain_Click(object sender, System.EventArgs e)
		{
			if( currentMol1 == null )
			{
				MessageBox.Show( "Molecule1 is null");
				return;
			}
			//try
			//{
				IGDetector detect = new IGDetector( (PolyPeptide) currentMol1 );
				int length = (int)numeric_EndLimitMol1.Value - (int)numeric_StartLimitMol1.Value + 1;
				DetectionMatch[] matches = detect.Matches( (int)numeric_StartLimitMol1.Value, length );

				int index = (int) numeric_AutoDomain.Value;
				if( index >= matches.Length )
				{
					MessageBox.Show( "InvalidIndex on mol 1");
					return;
				}

				DetectionMatch d = matches[ index ];
				numeric_Start1.Value = d.StartIndex;
				numeric_End1.Value = d.EndIndex;

				if( m_DimerDefineMode )
				{
					if( currentMol2 == null )
					{
						MessageBox.Show( "Molecule2 is null");
						return;
					}

					IGDetector detect2 = new IGDetector( (PolyPeptide) currentMol2 );
					int length2 = (int)numeric_EndLimitMol2.Value - (int)numeric_StartLimitMol2.Value + 1;
					DetectionMatch[] matches2 = detect2.Matches( (int)numeric_StartLimitMol2.Value , length2 );

					float lowestValue = float.MaxValue;
					int idOfLowest = -1;
					for( int i = 0; i < matches2.Length; i++ )
					{
						float distanceBetweenPair = Position.distanceBetween( matches[ index ].MatchCenter, matches2[i].MatchCenter );
						if( distanceBetweenPair == 0.0f )
						{
							// we are working within a single chain and its the same domain
							continue;
						}
						if( distanceBetweenPair < lowestValue )
						{
							idOfLowest = i;
							lowestValue = distanceBetweenPair;                         
						}
					}
					
					DetectionMatch mol1Match = matches[ index ];
					numeric_Start1.Value = mol1Match.StartIndex;
					numeric_End1.Value = mol1Match.EndIndex;
					DetectionMatch mol2Match = matches2[ idOfLowest ];
					numeric_Start2.Value = mol2Match.StartIndex;
					numeric_End2.Value = mol2Match.EndIndex;

				}
			//}
			//catch( Exception ex )
			//{
			//	MessageBox.Show( ex.ToString() );
			//}
		}

		private void button_FABFullAuto_Click(object sender, System.EventArgs e)
		{
			if( check_SetCurrentChains01.Checked )
			{
				// sometimes there is a peptide as the 1st chain, so if there is, we should ignore it
				PSMolContainer[] mols = m_PartSys.Members;
				int firstIDUsed = -1; //null
				for( int i = 0; i < mols.Length; i++ )
				{
					if( mols[i].Count > 199 ) 
						// 199 arbitary for 2 domains, antigens in position ) are often big, and we dont want em
					{
						firstIDUsed = i;
						listBox_Monomer1.SelectedIndex = i;
						break;
					}
				}
				for( int i = firstIDUsed + 1; i < mols.Length; i++ )
				{
					if( mols[i].Count > 150 )
					{
						listBox_Monomer2.SelectedIndex = i;
						break;
					}
				}
			}

			combo_SegTypes.SelectedIndex = 4;
			numeric_AutoDomain.Value = 0;
			button_AutoDomain_Click(null,null);
			button_AddDefinition_Click(null,null);
			numeric_AutoDomain.Value = 1;
			button_AutoDomain_Click(null,null);
			button_AddDefinition_Click(null,null);   
         	button_DoneWithFile_Click(null,null);	
		}

		private void button_UpdateComment_Click(object sender, System.EventArgs e)
		{
			DataRowCollection rc = dt.Rows;
			DataRow currentRow = rc[dataGrid.CurrentCell.RowNumber];
			currentRow["Comments"] = text_CommentBlock.Text;
			ds.AcceptChanges();
		}

		private const string urlRoot = @"http://pdbbeta.rcsb.org/pdb/explore.do?structureId=";
        private void button_PDBWeb_Click(object sender, System.EventArgs e)
		{
			string url = urlRoot + m_PDBFile.Name.Substring(0,4);
            System.Diagnostics.Process p = new Process();
			p.StartInfo.FileName = url;
			p.Start();
		}

		private void numeric_RangeLimiters_ValueChanged(object sender, System.EventArgs e)
		{
			if( editingValues )
			{
				// while editing, values may not yet be valid for the repaint
				return;
			}

			//m_DrawWrapper.visibleEditingInProgress = true;
			if( currentMol1 != null )
			{
				//m_DrawWrapper.setVisible( false, 0, currentMol1.Count -1, currentMol1 );
				//m_DrawWrapper.setVisible( true, (int)numeric_StartLimitMol1.Value, (int)numeric_EndLimitMol1.Value, currentMol1);
			}

			if( currentMol2 != null && m_DimerDefineMode )
			{
				//m_DrawWrapper.setVisible( false, 0, currentMol2.Count -1, currentMol2 );
				//m_DrawWrapper.setVisible( true, (int)numeric_StartLimitMol2.Value, (int)numeric_EndLimitMol2.Value, currentMol2);
			}
			//m_DrawWrapper.visibleEditingInProgress = false;
		}

		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			switch( ((TabControl) sender ).SelectedIndex )
			{
				case 0: // builder
					view.Parent = panel_BuilderViewSlot;
					dataGrid.Parent = panel_Build_DataBase;
					break;
				case 1:
					view.Parent = panel_View_GL1;
					dataGrid.Parent = panel_View_DataBase;
					break;
				default: 
					break;
			}
		}

		private int m_RowIncrementer = -1;
		private void button_NextRow_Click(object sender, System.EventArgs e)
		{
			selectRow( ++m_RowIncrementer );
		}

		private void DataGridClicked(object sender, System.EventArgs e)
		{
			m_RowIncrementer = dataGrid.CurrentCell.RowNumber;
			selectRow( dataGrid.CurrentCell.RowNumber );
		}

		private void selectRow( int index )
		{
			DataRowCollection rc = dt.Rows;
			DataRow currentRow = rc[index];
			
			string fileName = m_SourcePDBPath + (string)currentRow["SourcePDBFileID"] + ".pdb";
			if( m_PDBFile != null )
			{
				if( fileName != m_PDBFile.fullFilePath )
				{
					m_PDBFile = new PDB( fileName, true );
					m_DrawWrapper.particleSystem = m_PDBFile.particleSystem;
				}
			}
			else
			{
				m_PDBFile = new PDB( fileName, true );
				m_DrawWrapper.particleSystem = m_PDBFile.particleSystem;
			}

			string path = m_PDBPath + (string)currentRow["FileName"];
			if( m_PDBFile2 != null )
			{
				if( path != m_PDBFile2.fullFilePath )
				{
					m_PDBFile2 = new PDB( path, true );
					m_DrawWrapper2.particleSystem = m_PDBFile2.particleSystem;
				}
			}
			else
			{
				m_PDBFile2 = new PDB( path, true );
				m_DrawWrapper2.particleSystem = m_PDBFile2.particleSystem;
			}

		}
	}
}
