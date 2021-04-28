using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using UoB.Core;
using UoB.Core.Sequence;
using UoB.Core.Structure.Builder;
using UoB.CoreControls.ToolWindows;
using UoB.Core.FileIO.PDB;
using UoB.Core.Primitives;
using UoB.Core.Structure;
using UoB.Core.ForceField;
using UoB.CoreControls.OpenGLView;
using UoB.CoreControls.PS_Render;

namespace UoB.CoreControls.Builder
{
	/// <summary>
	/// Summary description for Builder.
	/// </summary>
	public class Builder : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox check_RotamerMinimisation;
		private System.Windows.Forms.CheckBox check_SaveFile;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button button_Build;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label_Selected;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button button_AddSelection;
		private System.Windows.Forms.TextBox box_SaveName;
		private System.Windows.Forms.Button button_ClearSelections;
		private System.Windows.Forms.CheckBox check_OutputAll;
		private System.Windows.Forms.Button button_SaveTo;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.TabPage tab_Selections;
		private System.Windows.Forms.Panel panel_ViewSplitRight;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Splitter splitter_ViewRight;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TabPage tab_Builder;
		private System.Windows.Forms.TabPage tab_SystemInput;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radio_Three;
		private System.Windows.Forms.RadioButton radio_One;
		private System.Windows.Forms.RadioButton radio_FASTAAlignment;
		private System.Windows.Forms.Button button_Validate;
		private System.Windows.Forms.Button button_LoadFile;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox group_GetTemplatePS;
		private System.Windows.Forms.TextBox text_PSTemplateFileName;
		private System.Windows.Forms.TextBox text_PSName;
		private System.Windows.Forms.Button button_ChangeView_PSTemplate;
		private System.Windows.Forms.GroupBox group_UserInputBox;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Panel panel_Input_PDBInfo;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel_Input_TreeView;
		private System.Windows.Forms.Button button_Restore;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox check_OpenInViewer;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.TabControl TabCollection;
		private System.Windows.Forms.Panel panel_Info_GLView;
		private System.Windows.Forms.Panel panel_Info_PSTreeView;
		private System.Windows.Forms.Panel panel_Info_PDBInfo;
		private System.Windows.Forms.ListBox list_Selections;
		private System.Windows.Forms.TabPage tab_PSViewing;
		private System.Windows.Forms.Button button_SetViewToModel;
		private System.Windows.Forms.Button button_SetViewToTemplate;
		private System.Windows.Forms.TextBox box_Selection_Thread;
		private System.Windows.Forms.TextBox box_Selection_Template;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox box_ChainID;
		private System.Windows.Forms.Button button_GetFromChainID;
		private System.Windows.Forms.TextBox box_Input_UserBox;
		private System.Windows.Forms.TextBox box_Input_Thread;
		private System.Windows.Forms.TextBox box_Input_Template;
		private System.Windows.Forms.Button button_DeleteCurrentSelection;
		private System.Windows.Forms.TextBox text_Builder_PSName;
		private System.Windows.Forms.GroupBox group_RebuildOptions;
		private System.Windows.Forms.RadioButton radio_Rebuild_AllAtoms;
		private System.Windows.Forms.RadioButton radio_Rebuild_HeavyPolarAromatic;
		private System.Windows.Forms.RadioButton radio_Rebuild_HeavyPolar;
		private System.Windows.Forms.RadioButton radio_Rebuild_HeavyOnly;
		private System.Windows.Forms.TextBox text_Builder_Filename;
		private System.Windows.Forms.Button button_Builder_Load;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.GroupBox group_ViewerCallback;
		private System.Windows.Forms.RadioButton radio_builder_ModelAndTemplate;
		private System.Windows.Forms.RadioButton radio_builder_templateonly;
		private System.Windows.Forms.CheckBox check_Builder_PerformRebuild;
		private System.Windows.Forms.RadioButton radio_Builder_ModelOnly;
		private System.Windows.Forms.RadioButton radio_builder_ModelTemplateOverlay;
		private System.Windows.Forms.RadioButton radio_Builder_NoCallback;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox box_Selection_Resultant;
		private System.Windows.Forms.Panel panel_Selection_PSView;


		private UoB.CoreControls.ToolWindows.PDB_InfoView pdB_InfoView1;
		private PSTreeView psTreeView1;
		private GLView view;
		private PS_Builder m_Builder;
		public event ParticleSystemEvent ModelReady;
		private ParticleSystemDrawWrapper psDrawWrapper;

		private bool m_CurrentlyShowingTemplate = true; // the current view state of the info tab, false means that the model was being displayed

		// keeps track of the current selections
		private int templateStart = -1;
		private int templateLength = -1;
		private int threadStart = -1;
		private System.Windows.Forms.CheckBox check_Builder_LeaveRestInPlace;
		private int threadLength = -1;

		public Builder()
		{
			InitializeComponent();

			m_Builder = new PS_Builder();
			view = new GLView();
			view.Dock = DockStyle.Fill;
			view.Parent = panel_Info_GLView;
			psDrawWrapper = new ParticleSystemDrawWrapper( null, view );

			pdB_InfoView1 = new PDB_InfoView();
			pdB_InfoView1.Dock = DockStyle.Fill;
			psTreeView1 = new PSTreeView();
			psTreeView1.Dock = DockStyle.Fill;

			layoutFormInformation();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Builder));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.check_Builder_LeaveRestInPlace = new System.Windows.Forms.CheckBox();
			this.check_Builder_PerformRebuild = new System.Windows.Forms.CheckBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.label8 = new System.Windows.Forms.Label();
			this.check_OutputAll = new System.Windows.Forms.CheckBox();
			this.check_RotamerMinimisation = new System.Windows.Forms.CheckBox();
			this.button_Build = new System.Windows.Forms.Button();
			this.button_SaveTo = new System.Windows.Forms.Button();
			this.check_SaveFile = new System.Windows.Forms.CheckBox();
			this.box_SaveName = new System.Windows.Forms.TextBox();
			this.check_OpenInViewer = new System.Windows.Forms.CheckBox();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.button_AddSelection = new System.Windows.Forms.Button();
			this.box_Selection_Thread = new System.Windows.Forms.TextBox();
			this.box_Selection_Template = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.button_ClearSelections = new System.Windows.Forms.Button();
			this.label_Selected = new System.Windows.Forms.Label();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.TabCollection = new System.Windows.Forms.TabControl();
			this.tab_Builder = new System.Windows.Forms.TabPage();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.button_Builder_Load = new System.Windows.Forms.Button();
			this.text_Builder_Filename = new System.Windows.Forms.TextBox();
			this.text_Builder_PSName = new System.Windows.Forms.TextBox();
			this.group_RebuildOptions = new System.Windows.Forms.GroupBox();
			this.radio_Rebuild_HeavyOnly = new System.Windows.Forms.RadioButton();
			this.radio_Rebuild_HeavyPolar = new System.Windows.Forms.RadioButton();
			this.radio_Rebuild_HeavyPolarAromatic = new System.Windows.Forms.RadioButton();
			this.radio_Rebuild_AllAtoms = new System.Windows.Forms.RadioButton();
			this.group_ViewerCallback = new System.Windows.Forms.GroupBox();
			this.radio_Builder_NoCallback = new System.Windows.Forms.RadioButton();
			this.radio_builder_ModelTemplateOverlay = new System.Windows.Forms.RadioButton();
			this.radio_builder_templateonly = new System.Windows.Forms.RadioButton();
			this.radio_builder_ModelAndTemplate = new System.Windows.Forms.RadioButton();
			this.radio_Builder_ModelOnly = new System.Windows.Forms.RadioButton();
			this.tab_SystemInput = new System.Windows.Forms.TabPage();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.box_Input_Template = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.box_ChainID = new System.Windows.Forms.TextBox();
			this.button_GetFromChainID = new System.Windows.Forms.Button();
			this.panel_Input_TreeView = new System.Windows.Forms.Panel();
			this.panel_Input_PDBInfo = new System.Windows.Forms.Panel();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.box_Input_Thread = new System.Windows.Forms.TextBox();
			this.group_UserInputBox = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.button_Restore = new System.Windows.Forms.Button();
			this.box_Input_UserBox = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radio_Three = new System.Windows.Forms.RadioButton();
			this.radio_One = new System.Windows.Forms.RadioButton();
			this.radio_FASTAAlignment = new System.Windows.Forms.RadioButton();
			this.button_Validate = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.group_GetTemplatePS = new System.Windows.Forms.GroupBox();
			this.button_ChangeView_PSTemplate = new System.Windows.Forms.Button();
			this.text_PSTemplateFileName = new System.Windows.Forms.TextBox();
			this.button_LoadFile = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.text_PSName = new System.Windows.Forms.TextBox();
			this.tab_PSViewing = new System.Windows.Forms.TabPage();
			this.panel_Info_GLView = new System.Windows.Forms.Panel();
			this.button_SetViewToModel = new System.Windows.Forms.Button();
			this.button_SetViewToTemplate = new System.Windows.Forms.Button();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel_ViewSplitRight = new System.Windows.Forms.Panel();
			this.panel_Info_PSTreeView = new System.Windows.Forms.Panel();
			this.splitter_ViewRight = new System.Windows.Forms.Splitter();
			this.panel_Info_PDBInfo = new System.Windows.Forms.Panel();
			this.tab_Selections = new System.Windows.Forms.TabPage();
			this.panel_Selection_PSView = new System.Windows.Forms.Panel();
			this.label9 = new System.Windows.Forms.Label();
			this.box_Selection_Resultant = new System.Windows.Forms.TextBox();
			this.button_DeleteCurrentSelection = new System.Windows.Forms.Button();
			this.list_Selections = new System.Windows.Forms.ListBox();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.TabCollection.SuspendLayout();
			this.tab_Builder.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.group_RebuildOptions.SuspendLayout();
			this.group_ViewerCallback.SuspendLayout();
			this.tab_SystemInput.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.group_UserInputBox.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.group_GetTemplatePS.SuspendLayout();
			this.tab_PSViewing.SuspendLayout();
			this.panel_Info_GLView.SuspendLayout();
			this.panel_ViewSplitRight.SuspendLayout();
			this.tab_Selections.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.check_Builder_LeaveRestInPlace);
			this.groupBox1.Controls.Add(this.check_Builder_PerformRebuild);
			this.groupBox1.Controls.Add(this.checkBox1);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.check_OutputAll);
			this.groupBox1.Controls.Add(this.check_RotamerMinimisation);
			this.groupBox1.Controls.Add(this.button_Build);
			this.groupBox1.Location = new System.Drawing.Point(8, 144);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(504, 216);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Structure Overlay and Build Options";
			// 
			// check_Builder_LeaveRestInPlace
			// 
			this.check_Builder_LeaveRestInPlace.Enabled = false;
			this.check_Builder_LeaveRestInPlace.Location = new System.Drawing.Point(8, 80);
			this.check_Builder_LeaveRestInPlace.Name = "check_Builder_LeaveRestInPlace";
			this.check_Builder_LeaveRestInPlace.Size = new System.Drawing.Size(480, 24);
			this.check_Builder_LeaveRestInPlace.TabIndex = 23;
			this.check_Builder_LeaveRestInPlace.Text = "Keep Explicit Water and HeteroMolecules in Model";
			// 
			// check_Builder_PerformRebuild
			// 
			this.check_Builder_PerformRebuild.Checked = true;
			this.check_Builder_PerformRebuild.CheckState = System.Windows.Forms.CheckState.Checked;
			this.check_Builder_PerformRebuild.Location = new System.Drawing.Point(8, 56);
			this.check_Builder_PerformRebuild.Name = "check_Builder_PerformRebuild";
			this.check_Builder_PerformRebuild.Size = new System.Drawing.Size(304, 24);
			this.check_Builder_PerformRebuild.TabIndex = 22;
			this.check_Builder_PerformRebuild.Text = "Perform preliminatry atom rebuild";
			this.check_Builder_PerformRebuild.CheckedChanged += new System.EventHandler(this.check_Builder_PerformRebuild_CheckedChanged);
			// 
			// checkBox1
			// 
			this.checkBox1.Enabled = false;
			this.checkBox1.Location = new System.Drawing.Point(8, 152);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(480, 24);
			this.checkBox1.TabIndex = 21;
			this.checkBox1.Text = "Spawn Energy Minimisation";
			// 
			// label8
			// 
			this.label8.BackColor = System.Drawing.SystemColors.Info;
			this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label8.Location = new System.Drawing.Point(8, 16);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(272, 32);
			this.label8.TabIndex = 20;
			this.label8.Text = "Building requires additional information to be provided. See above tabs ...";
			// 
			// check_OutputAll
			// 
			this.check_OutputAll.Checked = true;
			this.check_OutputAll.CheckState = System.Windows.Forms.CheckState.Checked;
			this.check_OutputAll.Enabled = false;
			this.check_OutputAll.Location = new System.Drawing.Point(8, 104);
			this.check_OutputAll.Name = "check_OutputAll";
			this.check_OutputAll.Size = new System.Drawing.Size(480, 24);
			this.check_OutputAll.TabIndex = 18;
			this.check_OutputAll.Text = "Output All Template, Not Just Aligned";
			// 
			// check_RotamerMinimisation
			// 
			this.check_RotamerMinimisation.Checked = true;
			this.check_RotamerMinimisation.CheckState = System.Windows.Forms.CheckState.Checked;
			this.check_RotamerMinimisation.Enabled = false;
			this.check_RotamerMinimisation.Location = new System.Drawing.Point(8, 128);
			this.check_RotamerMinimisation.Name = "check_RotamerMinimisation";
			this.check_RotamerMinimisation.Size = new System.Drawing.Size(480, 24);
			this.check_RotamerMinimisation.TabIndex = 3;
			this.check_RotamerMinimisation.Text = "Perform Rotamer Minimisation";
			// 
			// button_Build
			// 
			this.button_Build.Location = new System.Drawing.Point(8, 184);
			this.button_Build.Name = "button_Build";
			this.button_Build.Size = new System.Drawing.Size(72, 23);
			this.button_Build.TabIndex = 2;
			this.button_Build.Text = "Build";
			this.button_Build.Click += new System.EventHandler(this.button_Build_Click);
			// 
			// button_SaveTo
			// 
			this.button_SaveTo.Image = ((System.Drawing.Image)(resources.GetObject("button_SaveTo.Image")));
			this.button_SaveTo.Location = new System.Drawing.Point(464, 72);
			this.button_SaveTo.Name = "button_SaveTo";
			this.button_SaveTo.Size = new System.Drawing.Size(32, 23);
			this.button_SaveTo.TabIndex = 19;
			this.button_SaveTo.Click += new System.EventHandler(this.button_SaveTo_Click);
			// 
			// check_SaveFile
			// 
			this.check_SaveFile.Location = new System.Drawing.Point(8, 72);
			this.check_SaveFile.Name = "check_SaveFile";
			this.check_SaveFile.Size = new System.Drawing.Size(128, 24);
			this.check_SaveFile.TabIndex = 13;
			this.check_SaveFile.Text = "Save Model To File : ";
			// 
			// box_SaveName
			// 
			this.box_SaveName.Location = new System.Drawing.Point(136, 72);
			this.box_SaveName.Name = "box_SaveName";
			this.box_SaveName.Size = new System.Drawing.Size(272, 20);
			this.box_SaveName.TabIndex = 12;
			this.box_SaveName.Text = "c:\\DAVEBuilder_01.pdb";
			// 
			// check_OpenInViewer
			// 
			this.check_OpenInViewer.Checked = true;
			this.check_OpenInViewer.CheckState = System.Windows.Forms.CheckState.Checked;
			this.check_OpenInViewer.Location = new System.Drawing.Point(8, 96);
			this.check_OpenInViewer.Name = "check_OpenInViewer";
			this.check_OpenInViewer.Size = new System.Drawing.Size(304, 24);
			this.check_OpenInViewer.TabIndex = 14;
			this.check_OpenInViewer.Text = "Callback to viewer for file open on model completion";
			this.check_OpenInViewer.CheckedChanged += new System.EventHandler(this.check_OpenInViewer_CheckedChanged);
			// 
			// button_AddSelection
			// 
			this.button_AddSelection.Location = new System.Drawing.Point(240, 32);
			this.button_AddSelection.Name = "button_AddSelection";
			this.button_AddSelection.Size = new System.Drawing.Size(96, 23);
			this.button_AddSelection.TabIndex = 14;
			this.button_AddSelection.Text = "Add Selection";
			this.button_AddSelection.Click += new System.EventHandler(this.button_AddSelection_Click);
			// 
			// box_Selection_Thread
			// 
			this.box_Selection_Thread.BackColor = System.Drawing.Color.White;
			this.box_Selection_Thread.Location = new System.Drawing.Point(352, 232);
			this.box_Selection_Thread.Multiline = true;
			this.box_Selection_Thread.Name = "box_Selection_Thread";
			this.box_Selection_Thread.ReadOnly = true;
			this.box_Selection_Thread.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.box_Selection_Thread.Size = new System.Drawing.Size(496, 136);
			this.box_Selection_Thread.TabIndex = 23;
			this.box_Selection_Thread.Text = "";
			this.box_Selection_Thread.KeyDown += new System.Windows.Forms.KeyEventHandler(this.box_KeyDown);
			this.box_Selection_Thread.MouseMove += new System.Windows.Forms.MouseEventHandler(this.box_MouseUp);
			// 
			// box_Selection_Template
			// 
			this.box_Selection_Template.BackColor = System.Drawing.Color.White;
			this.box_Selection_Template.Location = new System.Drawing.Point(352, 72);
			this.box_Selection_Template.Multiline = true;
			this.box_Selection_Template.Name = "box_Selection_Template";
			this.box_Selection_Template.ReadOnly = true;
			this.box_Selection_Template.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.box_Selection_Template.Size = new System.Drawing.Size(496, 136);
			this.box_Selection_Template.TabIndex = 9;
			this.box_Selection_Template.Text = "";
			this.box_Selection_Template.KeyDown += new System.Windows.Forms.KeyEventHandler(this.box_KeyDown);
			this.box_Selection_Template.MouseUp += new System.Windows.Forms.MouseEventHandler(this.box_Selection_Template_MouseUp);
			this.box_Selection_Template.MouseMove += new System.Windows.Forms.MouseEventHandler(this.box_MouseUp);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(352, 56);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(496, 24);
			this.label5.TabIndex = 7;
			this.label5.Text = "Template Sequence";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(352, 216);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(496, 16);
			this.label4.TabIndex = 17;
			this.label4.Text = "Replacement Sequence :";
			// 
			// button_ClearSelections
			// 
			this.button_ClearSelections.Location = new System.Drawing.Point(240, 96);
			this.button_ClearSelections.Name = "button_ClearSelections";
			this.button_ClearSelections.Size = new System.Drawing.Size(96, 23);
			this.button_ClearSelections.TabIndex = 19;
			this.button_ClearSelections.Text = "Clear";
			this.button_ClearSelections.Click += new System.EventHandler(this.button_ClearSelections_Click);
			// 
			// label_Selected
			// 
			this.label_Selected.BackColor = System.Drawing.SystemColors.Info;
			this.label_Selected.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label_Selected.Location = new System.Drawing.Point(352, 8);
			this.label_Selected.Name = "label_Selected";
			this.label_Selected.Size = new System.Drawing.Size(496, 40);
			this.label_Selected.TabIndex = 16;
			this.label_Selected.Text = "Selection :";
			// 
			// TabCollection
			// 
			this.TabCollection.Controls.Add(this.tab_Builder);
			this.TabCollection.Controls.Add(this.tab_SystemInput);
			this.TabCollection.Controls.Add(this.tab_PSViewing);
			this.TabCollection.Controls.Add(this.tab_Selections);
			this.TabCollection.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TabCollection.Location = new System.Drawing.Point(0, 0);
			this.TabCollection.Name = "TabCollection";
			this.TabCollection.SelectedIndex = 0;
			this.TabCollection.Size = new System.Drawing.Size(864, 565);
			this.TabCollection.TabIndex = 18;
			this.TabCollection.SelectedIndexChanged += new System.EventHandler(this.TabCollection_SelectedIndexChanged);
			// 
			// tab_Builder
			// 
			this.tab_Builder.Controls.Add(this.groupBox5);
			this.tab_Builder.Controls.Add(this.groupBox1);
			this.tab_Builder.Controls.Add(this.group_RebuildOptions);
			this.tab_Builder.Controls.Add(this.group_ViewerCallback);
			this.tab_Builder.Location = new System.Drawing.Point(4, 22);
			this.tab_Builder.Name = "tab_Builder";
			this.tab_Builder.Size = new System.Drawing.Size(856, 539);
			this.tab_Builder.TabIndex = 0;
			this.tab_Builder.Text = "Builder";
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.button_Builder_Load);
			this.groupBox5.Controls.Add(this.box_SaveName);
			this.groupBox5.Controls.Add(this.text_Builder_Filename);
			this.groupBox5.Controls.Add(this.check_SaveFile);
			this.groupBox5.Controls.Add(this.button_SaveTo);
			this.groupBox5.Controls.Add(this.text_Builder_PSName);
			this.groupBox5.Controls.Add(this.check_OpenInViewer);
			this.groupBox5.Location = new System.Drawing.Point(8, 8);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(504, 128);
			this.groupBox5.TabIndex = 3;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Load \\ Save Options";
			// 
			// button_Builder_Load
			// 
			this.button_Builder_Load.Location = new System.Drawing.Point(416, 24);
			this.button_Builder_Load.Name = "button_Builder_Load";
			this.button_Builder_Load.TabIndex = 22;
			this.button_Builder_Load.Text = "Load";
			this.button_Builder_Load.Click += new System.EventHandler(this.button_LoadFile_Click);
			// 
			// text_Builder_Filename
			// 
			this.text_Builder_Filename.BackColor = System.Drawing.Color.White;
			this.text_Builder_Filename.Location = new System.Drawing.Point(8, 40);
			this.text_Builder_Filename.Name = "text_Builder_Filename";
			this.text_Builder_Filename.ReadOnly = true;
			this.text_Builder_Filename.Size = new System.Drawing.Size(400, 20);
			this.text_Builder_Filename.TabIndex = 21;
			this.text_Builder_Filename.Text = "Filename : ";
			// 
			// text_Builder_PSName
			// 
			this.text_Builder_PSName.BackColor = System.Drawing.Color.White;
			this.text_Builder_PSName.Location = new System.Drawing.Point(8, 16);
			this.text_Builder_PSName.Name = "text_Builder_PSName";
			this.text_Builder_PSName.ReadOnly = true;
			this.text_Builder_PSName.Size = new System.Drawing.Size(400, 20);
			this.text_Builder_PSName.TabIndex = 1;
			this.text_Builder_PSName.Text = "Name : ";
			// 
			// group_RebuildOptions
			// 
			this.group_RebuildOptions.Controls.Add(this.radio_Rebuild_HeavyOnly);
			this.group_RebuildOptions.Controls.Add(this.radio_Rebuild_HeavyPolar);
			this.group_RebuildOptions.Controls.Add(this.radio_Rebuild_HeavyPolarAromatic);
			this.group_RebuildOptions.Controls.Add(this.radio_Rebuild_AllAtoms);
			this.group_RebuildOptions.Location = new System.Drawing.Point(520, 160);
			this.group_RebuildOptions.Name = "group_RebuildOptions";
			this.group_RebuildOptions.Size = new System.Drawing.Size(328, 120);
			this.group_RebuildOptions.TabIndex = 4;
			this.group_RebuildOptions.TabStop = false;
			this.group_RebuildOptions.Text = "Rebuild Options";
			// 
			// radio_Rebuild_HeavyOnly
			// 
			this.radio_Rebuild_HeavyOnly.Location = new System.Drawing.Point(8, 88);
			this.radio_Rebuild_HeavyOnly.Name = "radio_Rebuild_HeavyOnly";
			this.radio_Rebuild_HeavyOnly.Size = new System.Drawing.Size(136, 24);
			this.radio_Rebuild_HeavyOnly.TabIndex = 4;
			this.radio_Rebuild_HeavyOnly.Text = "Heavy Atoms Only";
			// 
			// radio_Rebuild_HeavyPolar
			// 
			this.radio_Rebuild_HeavyPolar.Location = new System.Drawing.Point(8, 64);
			this.radio_Rebuild_HeavyPolar.Name = "radio_Rebuild_HeavyPolar";
			this.radio_Rebuild_HeavyPolar.Size = new System.Drawing.Size(256, 24);
			this.radio_Rebuild_HeavyPolar.TabIndex = 3;
			this.radio_Rebuild_HeavyPolar.Text = "Heavy, Polar Hydrogens";
			// 
			// radio_Rebuild_HeavyPolarAromatic
			// 
			this.radio_Rebuild_HeavyPolarAromatic.Location = new System.Drawing.Point(8, 40);
			this.radio_Rebuild_HeavyPolarAromatic.Name = "radio_Rebuild_HeavyPolarAromatic";
			this.radio_Rebuild_HeavyPolarAromatic.Size = new System.Drawing.Size(256, 24);
			this.radio_Rebuild_HeavyPolarAromatic.TabIndex = 2;
			this.radio_Rebuild_HeavyPolarAromatic.Text = "Heavy, Polar Hydrogens, Aromatic Hydrogens";
			// 
			// radio_Rebuild_AllAtoms
			// 
			this.radio_Rebuild_AllAtoms.Checked = true;
			this.radio_Rebuild_AllAtoms.Location = new System.Drawing.Point(8, 16);
			this.radio_Rebuild_AllAtoms.Name = "radio_Rebuild_AllAtoms";
			this.radio_Rebuild_AllAtoms.Size = new System.Drawing.Size(256, 24);
			this.radio_Rebuild_AllAtoms.TabIndex = 1;
			this.radio_Rebuild_AllAtoms.TabStop = true;
			this.radio_Rebuild_AllAtoms.Text = "All Atoms";
			// 
			// group_ViewerCallback
			// 
			this.group_ViewerCallback.Controls.Add(this.radio_Builder_NoCallback);
			this.group_ViewerCallback.Controls.Add(this.radio_builder_ModelTemplateOverlay);
			this.group_ViewerCallback.Controls.Add(this.radio_builder_templateonly);
			this.group_ViewerCallback.Controls.Add(this.radio_builder_ModelAndTemplate);
			this.group_ViewerCallback.Controls.Add(this.radio_Builder_ModelOnly);
			this.group_ViewerCallback.Location = new System.Drawing.Point(520, 8);
			this.group_ViewerCallback.Name = "group_ViewerCallback";
			this.group_ViewerCallback.Size = new System.Drawing.Size(328, 144);
			this.group_ViewerCallback.TabIndex = 23;
			this.group_ViewerCallback.TabStop = false;
			this.group_ViewerCallback.Text = "Viewer Callback Options";
			// 
			// radio_Builder_NoCallback
			// 
			this.radio_Builder_NoCallback.Location = new System.Drawing.Point(8, 16);
			this.radio_Builder_NoCallback.Name = "radio_Builder_NoCallback";
			this.radio_Builder_NoCallback.Size = new System.Drawing.Size(256, 24);
			this.radio_Builder_NoCallback.TabIndex = 6;
			this.radio_Builder_NoCallback.Text = "No Viewer Callback";
			// 
			// radio_builder_ModelTemplateOverlay
			// 
			this.radio_builder_ModelTemplateOverlay.Enabled = false;
			this.radio_builder_ModelTemplateOverlay.Location = new System.Drawing.Point(8, 112);
			this.radio_builder_ModelTemplateOverlay.Name = "radio_builder_ModelTemplateOverlay";
			this.radio_builder_ModelTemplateOverlay.Size = new System.Drawing.Size(256, 24);
			this.radio_builder_ModelTemplateOverlay.TabIndex = 5;
			this.radio_builder_ModelTemplateOverlay.Text = "Model : Template Overlay";
			// 
			// radio_builder_templateonly
			// 
			this.radio_builder_templateonly.Location = new System.Drawing.Point(8, 64);
			this.radio_builder_templateonly.Name = "radio_builder_templateonly";
			this.radio_builder_templateonly.Size = new System.Drawing.Size(256, 24);
			this.radio_builder_templateonly.TabIndex = 4;
			this.radio_builder_templateonly.Text = "Template Only (?)";
			// 
			// radio_builder_ModelAndTemplate
			// 
			this.radio_builder_ModelAndTemplate.Location = new System.Drawing.Point(8, 88);
			this.radio_builder_ModelAndTemplate.Name = "radio_builder_ModelAndTemplate";
			this.radio_builder_ModelAndTemplate.Size = new System.Drawing.Size(256, 24);
			this.radio_builder_ModelAndTemplate.TabIndex = 3;
			this.radio_builder_ModelAndTemplate.Text = "Model and Template";
			// 
			// radio_Builder_ModelOnly
			// 
			this.radio_Builder_ModelOnly.Checked = true;
			this.radio_Builder_ModelOnly.Location = new System.Drawing.Point(8, 40);
			this.radio_Builder_ModelOnly.Name = "radio_Builder_ModelOnly";
			this.radio_Builder_ModelOnly.Size = new System.Drawing.Size(256, 24);
			this.radio_Builder_ModelOnly.TabIndex = 2;
			this.radio_Builder_ModelOnly.TabStop = true;
			this.radio_Builder_ModelOnly.Text = "Model Only";
			// 
			// tab_SystemInput
			// 
			this.tab_SystemInput.Controls.Add(this.groupBox6);
			this.tab_SystemInput.Controls.Add(this.panel_Input_TreeView);
			this.tab_SystemInput.Controls.Add(this.panel_Input_PDBInfo);
			this.tab_SystemInput.Controls.Add(this.groupBox3);
			this.tab_SystemInput.Controls.Add(this.group_UserInputBox);
			this.tab_SystemInput.Controls.Add(this.group_GetTemplatePS);
			this.tab_SystemInput.Location = new System.Drawing.Point(4, 22);
			this.tab_SystemInput.Name = "tab_SystemInput";
			this.tab_SystemInput.Size = new System.Drawing.Size(856, 539);
			this.tab_SystemInput.TabIndex = 3;
			this.tab_SystemInput.Text = "System Input";
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.box_Input_Template);
			this.groupBox6.Controls.Add(this.label3);
			this.groupBox6.Controls.Add(this.box_ChainID);
			this.groupBox6.Controls.Add(this.button_GetFromChainID);
			this.groupBox6.Location = new System.Drawing.Point(272, 8);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(576, 144);
			this.groupBox6.TabIndex = 42;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Current Builder Template Sequence";
			// 
			// box_Input_Template
			// 
			this.box_Input_Template.Location = new System.Drawing.Point(8, 40);
			this.box_Input_Template.Multiline = true;
			this.box_Input_Template.Name = "box_Input_Template";
			this.box_Input_Template.Size = new System.Drawing.Size(560, 96);
			this.box_Input_Template.TabIndex = 16;
			this.box_Input_Template.Text = "";
			this.box_Input_Template.KeyDown += new System.Windows.Forms.KeyEventHandler(this.box_KeyDown);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 16);
			this.label3.TabIndex = 14;
			this.label3.Text = "Chain ID";
			// 
			// box_ChainID
			// 
			this.box_ChainID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.box_ChainID.Location = new System.Drawing.Point(56, 16);
			this.box_ChainID.MaxLength = 1;
			this.box_ChainID.Name = "box_ChainID";
			this.box_ChainID.Size = new System.Drawing.Size(48, 20);
			this.box_ChainID.TabIndex = 13;
			this.box_ChainID.Text = "";
			this.box_ChainID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.box_ChainID_KeyPress);
			// 
			// button_GetFromChainID
			// 
			this.button_GetFromChainID.Location = new System.Drawing.Point(112, 16);
			this.button_GetFromChainID.Name = "button_GetFromChainID";
			this.button_GetFromChainID.Size = new System.Drawing.Size(48, 24);
			this.button_GetFromChainID.TabIndex = 15;
			this.button_GetFromChainID.Text = "Get";
			// 
			// panel_Input_TreeView
			// 
			this.panel_Input_TreeView.Location = new System.Drawing.Point(8, 264);
			this.panel_Input_TreeView.Name = "panel_Input_TreeView";
			this.panel_Input_TreeView.Size = new System.Drawing.Size(256, 272);
			this.panel_Input_TreeView.TabIndex = 41;
			// 
			// panel_Input_PDBInfo
			// 
			this.panel_Input_PDBInfo.Location = new System.Drawing.Point(8, 128);
			this.panel_Input_PDBInfo.Name = "panel_Input_PDBInfo";
			this.panel_Input_PDBInfo.Size = new System.Drawing.Size(256, 128);
			this.panel_Input_PDBInfo.TabIndex = 40;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.box_Input_Thread);
			this.groupBox3.Location = new System.Drawing.Point(272, 424);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(576, 112);
			this.groupBox3.TabIndex = 39;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Current Builder Thread Sequence";
			// 
			// box_Input_Thread
			// 
			this.box_Input_Thread.BackColor = System.Drawing.Color.White;
			this.box_Input_Thread.Location = new System.Drawing.Point(8, 16);
			this.box_Input_Thread.Multiline = true;
			this.box_Input_Thread.Name = "box_Input_Thread";
			this.box_Input_Thread.ReadOnly = true;
			this.box_Input_Thread.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.box_Input_Thread.Size = new System.Drawing.Size(560, 88);
			this.box_Input_Thread.TabIndex = 0;
			this.box_Input_Thread.Text = "";
			this.box_Input_Thread.KeyDown += new System.Windows.Forms.KeyEventHandler(this.box_KeyDown);
			// 
			// group_UserInputBox
			// 
			this.group_UserInputBox.Controls.Add(this.label7);
			this.group_UserInputBox.Controls.Add(this.button_Restore);
			this.group_UserInputBox.Controls.Add(this.box_Input_UserBox);
			this.group_UserInputBox.Controls.Add(this.groupBox2);
			this.group_UserInputBox.Controls.Add(this.button_Validate);
			this.group_UserInputBox.Controls.Add(this.label2);
			this.group_UserInputBox.Location = new System.Drawing.Point(272, 160);
			this.group_UserInputBox.Name = "group_UserInputBox";
			this.group_UserInputBox.Size = new System.Drawing.Size(576, 256);
			this.group_UserInputBox.TabIndex = 38;
			this.group_UserInputBox.TabStop = false;
			this.group_UserInputBox.Text = "User Input";
			// 
			// label7
			// 
			this.label7.BackColor = System.Drawing.SystemColors.Info;
			this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label7.Location = new System.Drawing.Point(264, 200);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(232, 48);
			this.label7.TabIndex = 37;
			this.label7.Text = "FASTA Alignment : This will automatically add the relevent selections under the \"" +
				"Selection Control\"  tab";
			// 
			// button_Restore
			// 
			this.button_Restore.Location = new System.Drawing.Point(512, 192);
			this.button_Restore.Name = "button_Restore";
			this.button_Restore.Size = new System.Drawing.Size(56, 23);
			this.button_Restore.TabIndex = 36;
			this.button_Restore.Text = "Restore";
			// 
			// box_Input_UserBox
			// 
			this.box_Input_UserBox.Location = new System.Drawing.Point(8, 16);
			this.box_Input_UserBox.Multiline = true;
			this.box_Input_UserBox.Name = "box_Input_UserBox";
			this.box_Input_UserBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.box_Input_UserBox.Size = new System.Drawing.Size(560, 128);
			this.box_Input_UserBox.TabIndex = 31;
			this.box_Input_UserBox.Text = "";
			this.box_Input_UserBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.box_KeyDown);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.radio_Three);
			this.groupBox2.Controls.Add(this.radio_One);
			this.groupBox2.Controls.Add(this.radio_FASTAAlignment);
			this.groupBox2.Location = new System.Drawing.Point(8, 152);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(248, 96);
			this.groupBox2.TabIndex = 34;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "InputType";
			// 
			// radio_Three
			// 
			this.radio_Three.Location = new System.Drawing.Point(8, 40);
			this.radio_Three.Name = "radio_Three";
			this.radio_Three.Size = new System.Drawing.Size(232, 24);
			this.radio_Three.TabIndex = 21;
			this.radio_Three.Text = "WhiteSpace Delimited Three Letter Code";
			// 
			// radio_One
			// 
			this.radio_One.Checked = true;
			this.radio_One.Location = new System.Drawing.Point(8, 16);
			this.radio_One.Name = "radio_One";
			this.radio_One.Size = new System.Drawing.Size(232, 24);
			this.radio_One.TabIndex = 20;
			this.radio_One.TabStop = true;
			this.radio_One.Text = "Single Letter Code";
			// 
			// radio_FASTAAlignment
			// 
			this.radio_FASTAAlignment.Enabled = false;
			this.radio_FASTAAlignment.Location = new System.Drawing.Point(8, 64);
			this.radio_FASTAAlignment.Name = "radio_FASTAAlignment";
			this.radio_FASTAAlignment.Size = new System.Drawing.Size(232, 24);
			this.radio_FASTAAlignment.TabIndex = 24;
			this.radio_FASTAAlignment.Text = "FASTA Alignment";
			// 
			// button_Validate
			// 
			this.button_Validate.Location = new System.Drawing.Point(512, 160);
			this.button_Validate.Name = "button_Validate";
			this.button_Validate.Size = new System.Drawing.Size(56, 23);
			this.button_Validate.TabIndex = 32;
			this.button_Validate.Text = "Validate";
			this.button_Validate.Click += new System.EventHandler(this.button_Validate_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(352, 160);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(168, 16);
			this.label2.TabIndex = 35;
			this.label2.Text = "Validate and Submit to Builder";
			// 
			// group_GetTemplatePS
			// 
			this.group_GetTemplatePS.Controls.Add(this.button_ChangeView_PSTemplate);
			this.group_GetTemplatePS.Controls.Add(this.text_PSTemplateFileName);
			this.group_GetTemplatePS.Controls.Add(this.button_LoadFile);
			this.group_GetTemplatePS.Controls.Add(this.label1);
			this.group_GetTemplatePS.Controls.Add(this.text_PSName);
			this.group_GetTemplatePS.Location = new System.Drawing.Point(8, 8);
			this.group_GetTemplatePS.Name = "group_GetTemplatePS";
			this.group_GetTemplatePS.Size = new System.Drawing.Size(256, 112);
			this.group_GetTemplatePS.TabIndex = 37;
			this.group_GetTemplatePS.TabStop = false;
			this.group_GetTemplatePS.Text = "Template";
			// 
			// button_ChangeView_PSTemplate
			// 
			this.button_ChangeView_PSTemplate.Location = new System.Drawing.Point(168, 16);
			this.button_ChangeView_PSTemplate.Name = "button_ChangeView_PSTemplate";
			this.button_ChangeView_PSTemplate.TabIndex = 39;
			this.button_ChangeView_PSTemplate.Text = "View ^";
			this.button_ChangeView_PSTemplate.Click += new System.EventHandler(this.button_ChangeView_PSTemplate_Click);
			// 
			// text_PSTemplateFileName
			// 
			this.text_PSTemplateFileName.BackColor = System.Drawing.Color.White;
			this.text_PSTemplateFileName.Location = new System.Drawing.Point(8, 80);
			this.text_PSTemplateFileName.Name = "text_PSTemplateFileName";
			this.text_PSTemplateFileName.ReadOnly = true;
			this.text_PSTemplateFileName.Size = new System.Drawing.Size(232, 20);
			this.text_PSTemplateFileName.TabIndex = 37;
			this.text_PSTemplateFileName.Text = "Filename : ";
			this.text_PSTemplateFileName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.box_KeyDown);
			// 
			// button_LoadFile
			// 
			this.button_LoadFile.Image = ((System.Drawing.Image)(resources.GetObject("button_LoadFile.Image")));
			this.button_LoadFile.Location = new System.Drawing.Point(128, 16);
			this.button_LoadFile.Name = "button_LoadFile";
			this.button_LoadFile.Size = new System.Drawing.Size(32, 23);
			this.button_LoadFile.TabIndex = 35;
			this.button_LoadFile.Click += new System.EventHandler(this.button_LoadFile_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(192, 16);
			this.label1.TabIndex = 36;
			this.label1.Text = "Load from file :";
			// 
			// text_PSName
			// 
			this.text_PSName.BackColor = System.Drawing.Color.White;
			this.text_PSName.Location = new System.Drawing.Point(8, 48);
			this.text_PSName.Name = "text_PSName";
			this.text_PSName.ReadOnly = true;
			this.text_PSName.Size = new System.Drawing.Size(232, 20);
			this.text_PSName.TabIndex = 38;
			this.text_PSName.Text = "Name : ";
			this.text_PSName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.box_KeyDown);
			// 
			// tab_PSViewing
			// 
			this.tab_PSViewing.Controls.Add(this.panel_Info_GLView);
			this.tab_PSViewing.Controls.Add(this.splitter1);
			this.tab_PSViewing.Controls.Add(this.panel_ViewSplitRight);
			this.tab_PSViewing.Location = new System.Drawing.Point(4, 22);
			this.tab_PSViewing.Name = "tab_PSViewing";
			this.tab_PSViewing.Size = new System.Drawing.Size(856, 539);
			this.tab_PSViewing.TabIndex = 1;
			this.tab_PSViewing.Text = "PS Viewing";
			// 
			// panel_Info_GLView
			// 
			this.panel_Info_GLView.Controls.Add(this.button_SetViewToModel);
			this.panel_Info_GLView.Controls.Add(this.button_SetViewToTemplate);
			this.panel_Info_GLView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Info_GLView.Location = new System.Drawing.Point(0, 0);
			this.panel_Info_GLView.Name = "panel_Info_GLView";
			this.panel_Info_GLView.Size = new System.Drawing.Size(581, 539);
			this.panel_Info_GLView.TabIndex = 2;
			// 
			// button_SetViewToModel
			// 
			this.button_SetViewToModel.Location = new System.Drawing.Point(88, 8);
			this.button_SetViewToModel.Name = "button_SetViewToModel";
			this.button_SetViewToModel.TabIndex = 1;
			this.button_SetViewToModel.Text = "Model";
			this.button_SetViewToModel.Click += new System.EventHandler(this.button_SetViewToModel_Click);
			// 
			// button_SetViewToTemplate
			// 
			this.button_SetViewToTemplate.Location = new System.Drawing.Point(8, 8);
			this.button_SetViewToTemplate.Name = "button_SetViewToTemplate";
			this.button_SetViewToTemplate.TabIndex = 0;
			this.button_SetViewToTemplate.Text = "Template";
			this.button_SetViewToTemplate.Click += new System.EventHandler(this.button_SetViewToTemplate_Click);
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter1.Location = new System.Drawing.Point(581, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 539);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// panel_ViewSplitRight
			// 
			this.panel_ViewSplitRight.Controls.Add(this.panel_Info_PSTreeView);
			this.panel_ViewSplitRight.Controls.Add(this.splitter_ViewRight);
			this.panel_ViewSplitRight.Controls.Add(this.panel_Info_PDBInfo);
			this.panel_ViewSplitRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel_ViewSplitRight.Location = new System.Drawing.Point(584, 0);
			this.panel_ViewSplitRight.Name = "panel_ViewSplitRight";
			this.panel_ViewSplitRight.Size = new System.Drawing.Size(272, 539);
			this.panel_ViewSplitRight.TabIndex = 0;
			// 
			// panel_Info_PSTreeView
			// 
			this.panel_Info_PSTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Info_PSTreeView.Location = new System.Drawing.Point(0, 179);
			this.panel_Info_PSTreeView.Name = "panel_Info_PSTreeView";
			this.panel_Info_PSTreeView.Size = new System.Drawing.Size(272, 360);
			this.panel_Info_PSTreeView.TabIndex = 2;
			// 
			// splitter_ViewRight
			// 
			this.splitter_ViewRight.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter_ViewRight.Location = new System.Drawing.Point(0, 176);
			this.splitter_ViewRight.Name = "splitter_ViewRight";
			this.splitter_ViewRight.Size = new System.Drawing.Size(272, 3);
			this.splitter_ViewRight.TabIndex = 1;
			this.splitter_ViewRight.TabStop = false;
			// 
			// panel_Info_PDBInfo
			// 
			this.panel_Info_PDBInfo.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel_Info_PDBInfo.Location = new System.Drawing.Point(0, 0);
			this.panel_Info_PDBInfo.Name = "panel_Info_PDBInfo";
			this.panel_Info_PDBInfo.Size = new System.Drawing.Size(272, 176);
			this.panel_Info_PDBInfo.TabIndex = 0;
			// 
			// tab_Selections
			// 
			this.tab_Selections.Controls.Add(this.panel_Selection_PSView);
			this.tab_Selections.Controls.Add(this.label9);
			this.tab_Selections.Controls.Add(this.box_Selection_Resultant);
			this.tab_Selections.Controls.Add(this.button_DeleteCurrentSelection);
			this.tab_Selections.Controls.Add(this.list_Selections);
			this.tab_Selections.Controls.Add(this.label6);
			this.tab_Selections.Controls.Add(this.label_Selected);
			this.tab_Selections.Controls.Add(this.button_ClearSelections);
			this.tab_Selections.Controls.Add(this.button_AddSelection);
			this.tab_Selections.Controls.Add(this.box_Selection_Template);
			this.tab_Selections.Controls.Add(this.label4);
			this.tab_Selections.Controls.Add(this.box_Selection_Thread);
			this.tab_Selections.Controls.Add(this.label5);
			this.tab_Selections.Location = new System.Drawing.Point(4, 22);
			this.tab_Selections.Name = "tab_Selections";
			this.tab_Selections.Size = new System.Drawing.Size(856, 539);
			this.tab_Selections.TabIndex = 2;
			this.tab_Selections.Text = "Selection Control";
			// 
			// panel_Selection_PSView
			// 
			this.panel_Selection_PSView.Location = new System.Drawing.Point(8, 216);
			this.panel_Selection_PSView.Name = "panel_Selection_PSView";
			this.panel_Selection_PSView.Size = new System.Drawing.Size(328, 312);
			this.panel_Selection_PSView.TabIndex = 28;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(352, 376);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(488, 16);
			this.label9.TabIndex = 27;
			this.label9.Text = "Resultant Sequence Following Replacements";
			// 
			// box_Selection_Resultant
			// 
			this.box_Selection_Resultant.BackColor = System.Drawing.Color.White;
			this.box_Selection_Resultant.Location = new System.Drawing.Point(352, 392);
			this.box_Selection_Resultant.Multiline = true;
			this.box_Selection_Resultant.Name = "box_Selection_Resultant";
			this.box_Selection_Resultant.ReadOnly = true;
			this.box_Selection_Resultant.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.box_Selection_Resultant.Size = new System.Drawing.Size(496, 136);
			this.box_Selection_Resultant.TabIndex = 26;
			this.box_Selection_Resultant.Text = "";
			// 
			// button_DeleteCurrentSelection
			// 
			this.button_DeleteCurrentSelection.Location = new System.Drawing.Point(240, 64);
			this.button_DeleteCurrentSelection.Name = "button_DeleteCurrentSelection";
			this.button_DeleteCurrentSelection.Size = new System.Drawing.Size(96, 23);
			this.button_DeleteCurrentSelection.TabIndex = 25;
			this.button_DeleteCurrentSelection.Text = "Delete Current";
			this.button_DeleteCurrentSelection.Click += new System.EventHandler(this.button_DeleteCurrentSelection_Click);
			// 
			// list_Selections
			// 
			this.list_Selections.Location = new System.Drawing.Point(8, 32);
			this.list_Selections.Name = "list_Selections";
			this.list_Selections.ScrollAlwaysVisible = true;
			this.list_Selections.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.list_Selections.Size = new System.Drawing.Size(224, 173);
			this.list_Selections.TabIndex = 24;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(16, 8);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(96, 24);
			this.label6.TabIndex = 18;
			this.label6.Text = "Selections :";
			// 
			// Builder
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(864, 565);
			this.Controls.Add(this.TabCollection);
			this.Name = "Builder";
			this.Text = "Builder";
			this.groupBox1.ResumeLayout(false);
			this.TabCollection.ResumeLayout(false);
			this.tab_Builder.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.group_RebuildOptions.ResumeLayout(false);
			this.group_ViewerCallback.ResumeLayout(false);
			this.tab_SystemInput.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.group_UserInputBox.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.group_GetTemplatePS.ResumeLayout(false);
			this.tab_PSViewing.ResumeLayout(false);
			this.panel_Info_GLView.ResumeLayout(false);
			this.panel_ViewSplitRight.ResumeLayout(false);
			this.tab_Selections.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void button_LoadFile_Click(object sender, System.EventArgs e)
		{
			openFileDialog.Title = "Builder Open File Dialog"; 
			//openFileDialog.InitialDirectory =  ???; 
			openFileDialog.Filter = "All DAVE Readable Files (*.TRA;*.PDB)|*.TRA;*.PDB|PDB Files (*.PDB)|*.PDB|TRA files (*.TRA)|*.TRA|All files (*.*)|*.*"; 
			openFileDialog.FilterIndex = 0; 
			openFileDialog.RestoreDirectory = true; 

			if(openFileDialog.ShowDialog() == DialogResult.OK) 
			{ 
				Trace.WriteLine("Opening Builder Template File : " + openFileDialog.FileName + " ...");
				
				string[] split = openFileDialog.FileName.Split('.');
				string filetype = split[split.Length-1].ToUpper();

				string filePath = "";

				PDB pdbFile;
				switch (filetype)
				{
					case "PDB":
						pdbFile = new PDB( openFileDialog.FileName, true );
						m_Builder.TemplateSystem = pdbFile.particleSystem;
						psTreeView1.AttactToParticleSystem( m_Builder.TemplateSystem );
						pdB_InfoView1.AttactToPDBInfo( (PDBInfo) pdbFile.ExtendedInformation );
						filePath = openFileDialog.FileName;
						break;
					case "TRA":
						MessageBox.Show("Tra files are not yet supported in the builder ...");
						break;
					default:
						if( DialogResult.Yes == MessageBox.Show( this, "Unknown file extension selected, would you like to open this as a PDB file ?", "FileOpen Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1 ) )
						{
							pdbFile = new PDB( openFileDialog.FileName, true );
							m_Builder.TemplateSystem = pdbFile.particleSystem;
							psTreeView1.AttactToParticleSystem( m_Builder.TemplateSystem );
							pdB_InfoView1.AttactToPDBInfo( pdbFile.ExtendedInformation );
							filePath = openFileDialog.FileName;
						}
						break;
				}
                layoutFormInformation();
				psDrawWrapper.particleSystem = m_Builder.TemplateSystem;

				// just a bit of mildly pointless reporting
				string name = "Name : " + m_Builder.TemplateSystem.Name;
				string filename = "Filename : " + filePath;
				text_Builder_PSName.Text = name;
				text_PSName.Text = name; 
				text_PSTemplateFileName.Text = filename;
				text_Builder_Filename.Text = filename;
			} 

			TabCollection.SelectedIndex = 1;
		}

		private void layoutFormInformation()
		{
			// this functionis used by button_Validate_Click at the end - check before modifying
			layoutSequenceInfo();
			layoutSelectionInfo();
		}

		private void layoutSequenceInfo()
		{	
			// Populate sequence information boxes
			string templateMonomerList;
			string threadMonomerList;
			char chainID;
			m_Builder.GetCurrentSequenceInfo( out chainID, out templateMonomerList, out threadMonomerList );
			box_ChainID.Text = chainID.ToString();
			box_Input_Template.Text = templateMonomerList;
			box_Selection_Template.Text = templateMonomerList;
			box_Input_Thread.Text = threadMonomerList;
			box_Selection_Thread.Text = threadMonomerList;
		}

		private void layoutSelectionInfo()
		{
			// populate the selection information box
			list_Selections.Items.Clear();

			string[] selections = m_Builder.SelectionsStrings;

			for( int i = 0; i < selections.Length; i++ )
			{
				list_Selections.Items.Add( m_Builder.SelectionsStrings[i] );
			}

			box_Selection_Resultant.Text = m_Builder.SequenceFollowingBuild;
		}

		private void button_Build_Click(object sender, System.EventArgs e)
		{
			if( check_Builder_PerformRebuild.Checked )
			{
				try
				{
					RebuildMode mode = RebuildMode.AllAtoms;
					if( radio_Rebuild_AllAtoms.Checked )
					{
						mode = RebuildMode.AllAtoms;
					}
					else if( radio_Rebuild_HeavyPolarAromatic.Checked )
					{
						mode = RebuildMode.PolarAndAromatic;
					} 
					else if( radio_Rebuild_HeavyPolar.Checked )
					{
						mode = RebuildMode.PolarHydrogens;
					}
					else if( radio_Rebuild_HeavyOnly.Checked )
					{
						mode = RebuildMode.HeavyAtomsOnly;
					}
					else
					{
						throw new Exception("How have i got here ??? : \"button_Rebuild_Click()\" ");
					}
					m_Builder.RebuildTemplate( mode, false, true, false, false, false );
				}
				catch( BuilderException ex )
				{
					MessageBox.Show( ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error );
					return;
				}
			}

			try
			{
				Trace.WriteLine("Performing Model Build");
				m_Builder.BuildModel( check_Builder_LeaveRestInPlace.Checked, true, false );
			}
			catch( BuilderException ex )
			{
				MessageBox.Show( ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error );
				return;
			}

			if( check_RotamerMinimisation.Checked )
			{
				try
				{
					m_Builder.OptimiseRotamers();
				}
				catch( BuilderException ex )
				{
					MessageBox.Show( ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error );
					return;
				}
			}

			if( check_SaveFile.Checked )
			{
				try
				{
					PDB.SaveNew( box_SaveName.Text, m_Builder.ModelSystem );
				}
				catch
				{
					MessageBox.Show("PDB file write error, it was not saved ...");
				}
			}

			if( this.check_OpenInViewer.Checked )
			{
				if( radio_builder_ModelAndTemplate.Checked )
				{
					ModelReady( m_Builder.ModelSystem );
					ModelReady( m_Builder.TemplateSystem );
				}
				else if( radio_Builder_ModelOnly.Checked )
				{
					ModelReady( m_Builder.ModelSystem );
				}
				else if( radio_Builder_NoCallback.Checked )
				{
					// no nothing
				}
				else if ( radio_builder_templateonly.Checked )
				{
                    ModelReady( m_Builder.TemplateSystem );
				} 
				else if ( radio_builder_ModelTemplateOverlay.Checked )
				{
					throw new Exception("CODE EXCEPTION : Builder in generic - reached unimplemented code region!");
				}
				else
				{
					throw new Exception("CODE EXCEPTION : Builder in generic - reached invalid code!");
				}
			}

			TabCollection.SelectedIndex = 2; // the viewing tab
			m_CurrentlyShowingTemplate = false;
			setViewerPS();	
		}

		private void button_GetFromChainID_Click(object sender, System.EventArgs e)
		{
			try 
			{
				m_Builder.SetCurrentChainID( box_ChainID.Text[0] );
				layoutSequenceInfo();
			}
			catch( BuilderException ex )
			{
				MessageBox.Show(ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
		}

		private void box_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			TextBox tb = (TextBox)sender;
			if ( e.KeyCode == Keys.A && e.Modifiers == Keys.Control )
			{
				tb.SelectAll();
			}
		}

		private void box_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            // When a mouse up event occurs in the template and threading sequence boxes, 
			// then the selection profile shoudl be updated	
			templateStart = box_Selection_Template.SelectionStart;
			templateLength = box_Selection_Template.SelectionLength;
			threadStart = box_Selection_Thread.SelectionStart;
			threadLength = box_Selection_Thread.SelectionLength;

			StringBuilder selectionString = new StringBuilder(100);

			selectionString.Append( "Template Chosen From : " + (templateStart+1).ToString() + " To : " + ((int)(templateStart + templateLength)).ToString() + " Length : " + templateLength.ToString() + "\r\n");
			selectionString.Append( "Thread Chosen From : " + (threadStart+1).ToString() + " To : " + ((int)(threadStart + threadLength)).ToString() + " Length : " + threadLength.ToString() + "\r\n");

			label_Selected.Text = selectionString.ToString();
		}

		private void button_AddSelection_Click(object sender, System.EventArgs e)
		{
			int tempEnd = templateStart + templateLength;
			int threadEnd = threadStart + threadLength;

			try 
			{
				m_Builder.NewSelection( templateStart, tempEnd, threadStart, threadEnd );
				layoutSelectionInfo();
			}
			catch ( BuilderException ex )
			{
				MessageBox.Show( ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error );
			}
		}

		private void button_ClearSelections_Click(object sender, System.EventArgs e)
		{
			m_Builder.ClearSelections();
			layoutSelectionInfo();
		}

		private void button_Validate_Click(object sender, System.EventArgs e)
		{
			string newThread = box_Input_UserBox.Text.ToUpper();

			SequenceInputTypes seqType = SequenceInputTypes.SingleLetter;

			if( radio_One.Checked )
			{
				seqType = SequenceInputTypes.SingleLetter;
			} 
			else if ( radio_Three.Checked )
			{
				seqType = SequenceInputTypes.ThreeLetter;
			}
			else if ( radio_FASTAAlignment.Checked )
			{
				seqType = SequenceInputTypes.FASTA;
			}
			else
			{
				throw new Exception("Odd codeerror in Builder Validation initiation - there is no allowed radio button checked");
			}

			try
			{
				MessageBox.Show( m_Builder.ValidateNewThreadString( ref newThread, seqType ) ); 
				// the function returns a report string, so show a MsgBox
			}
			catch( BuilderException ex )
			{
				MessageBox.Show( ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error );
			}

			layoutFormInformation(); // both the sequence and selection data will change, we effectively start from scratch
		}

		private void button_Restore_Click(object sender, System.EventArgs e)
		{
			m_Builder.UndoThreadSequenceChange();
			layoutSequenceInfo();
		}

		private void button_SaveTo_Click(object sender, System.EventArgs e)
		{
			if( saveFileDialog.ShowDialog() == DialogResult.OK )
			{
				box_SaveName.Text = saveFileDialog.FileName;
				check_SaveFile.Checked = true;
			}
		}

		private void TabCollection_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			// when the user selects the tabs, the viewing components must be docked to the correct panel

			switch( TabCollection.SelectedIndex )
			{
				case 0 : // the builder main options
					// nothing needs to be done
					break;
				case 1: // System setup
					// both the templates PDB info pane and the treeview need to be displayed
					//panel_Input_PDBInfo
					//panel_Input_TreeView
					pdB_InfoView1.Parent = panel_Input_PDBInfo;
					psTreeView1.Parent = panel_Input_TreeView;

					break;
				case 2: // tab_PSViewing
					//panel_Info_GLView
					//panel_Info_PDBInfo
					//panel_Info_PSTreeView
					view.Parent = panel_Info_GLView;
					pdB_InfoView1.Parent = panel_Info_PDBInfo;
					psTreeView1.Parent = panel_Info_PSTreeView;

					break;
				case 3 : // Selection Control
                    view.Parent = panel_Selection_PSView;
					break;
				default:
					break;
			}							
		}

		private void button_ChangeView_PSTemplate_Click(object sender, System.EventArgs e)
		{
			TabCollection.SelectedIndex = 2; // the viewing tab
			m_CurrentlyShowingTemplate = true;
			setViewerPS();	
		}

		private void button_SetViewToTemplate_Click(object sender, System.EventArgs e)
		{
			m_CurrentlyShowingTemplate = true;
            setViewerPS();		
		}

		private void button_SetViewToModel_Click(object sender, System.EventArgs e)
		{
			m_CurrentlyShowingTemplate = false;            
			setViewerPS();	
		}

		private void setViewerPS()
		{
			if ( m_CurrentlyShowingTemplate )
			{
				psDrawWrapper.particleSystem = m_Builder.TemplateSystem;
				psTreeView1.AttactToParticleSystem( m_Builder.TemplateSystem );
			}
			else
			{
				psDrawWrapper.particleSystem = m_Builder.ModelSystem;
				psTreeView1.AttactToParticleSystem( m_Builder.ModelSystem );
			}
		}

		private void button_DeleteCurrentSelection_Click(object sender, System.EventArgs e)
		{
			try
			{
				// this is a real hack, but i cant find a way to convert list_Selections.SelectedIndices yo an int array other than this
				int count = list_Selections.SelectedIndices.Count;
				int[] ints = new int[ count ];
				for ( int i = 0; i < count; i++ )
				{
					ints[i] = list_Selections.SelectedIndices[i];
				}
				m_Builder.RemoveSelection( ints );
				layoutSelectionInfo();
			}
			catch( BuilderException ex )
			{
				MessageBox.Show( ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error );
			}
		}

		private void box_ChainID_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			box_ChainID.Text = e.KeyChar.ToString();
		}

		private void check_Builder_PerformRebuild_CheckedChanged(object sender, System.EventArgs e)
		{
			group_RebuildOptions.Enabled = check_Builder_PerformRebuild.Checked;
			radio_Rebuild_AllAtoms.Enabled = check_Builder_PerformRebuild.Checked;
			radio_Rebuild_HeavyOnly.Enabled = check_Builder_PerformRebuild.Checked;
			radio_Rebuild_HeavyPolar.Enabled = check_Builder_PerformRebuild.Checked;
			radio_Rebuild_HeavyPolarAromatic.Enabled = check_Builder_PerformRebuild.Checked;
		}

		private void check_OpenInViewer_CheckedChanged(object sender, System.EventArgs e)
		{
			group_ViewerCallback.Enabled = check_OpenInViewer.Checked;
			radio_builder_ModelAndTemplate.Enabled = check_OpenInViewer.Checked;
			radio_Builder_ModelOnly.Enabled = check_OpenInViewer.Checked;
			radio_builder_templateonly.Enabled = check_OpenInViewer.Checked;
			//radio_builder_ModelTemplateOverlay.Enabled = check_OpenInViewer.Checked;
			// not currently in use
		}

		private void box_Selection_Template_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			psDrawWrapper.BeginSelectionEdit();

			if( !psDrawWrapper.GetSelectionLockState() )
			{
				psDrawWrapper.SetSelectionLock(this,true);
			}

			PolyPeptide p = (PolyPeptide) m_Builder.TemplateSystem.MemberWithID( m_Builder.currentTemplateChainID );

			if( psDrawWrapper.SelectionCount == 1 )
			{
				psDrawWrapper.AddSelection( p, templateStart, templateLength );
				AminoAcidSelection aas = (AminoAcidSelection) psDrawWrapper.Selections[1];
				aas.Colour1 = Colour.FromKnownColor( KnownColor.Green );
				aas.ColourMode = SelectionColourMode.Single;
			}
			else
			{
                AminoAcidSelection aas = (AminoAcidSelection) psDrawWrapper.Selections[1];
				aas.setBounds( templateStart, templateLength );
			}

			psDrawWrapper.EndSelectionEdit();
		}
	}
}
