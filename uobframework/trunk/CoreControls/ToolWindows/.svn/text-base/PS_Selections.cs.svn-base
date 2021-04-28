using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

using UoB.CoreControls.PS_Render;
using UoB.Core.Structure;
using UoB.Core.Primitives;
using UoB.CoreControls.OpenGLView;
using UoB.CoreControls.Documents;
using UoB.Core;

namespace UoB.CoreControls.ToolWindows
{
	/// <summary>
	/// Summary description for AtomSelections.
	/// </summary>
	public class PS_Selections : System.Windows.Forms.UserControl, ITool
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Panel panel_Colour1;
		private System.Windows.Forms.Panel panel_Colour2;
		private System.Windows.Forms.ColorDialog colorDialog;
		private System.Windows.Forms.Button button_ColourOptions;
		private System.Windows.Forms.Panel panel_ColourA;
		private System.Windows.Forms.Panel panel_ColourB;
		private System.Windows.Forms.Panel panel_ColourC;
		private System.Windows.Forms.Panel panel_ColourD;
		private System.Windows.Forms.Panel panel_ColourE;
		private System.Windows.Forms.Panel panel_ColourF;
		private System.Windows.Forms.ContextMenu menu_Colour;
		private System.Windows.Forms.Button button_SelectionOptions;
		private System.Windows.Forms.ComboBox combo_DrawStyle;
		private System.Windows.Forms.ListView list_Drawtype;
		private System.Windows.Forms.ContextMenu menu_Selections;
		private System.Windows.Forms.Button button_DemoteSelection;
		private System.Windows.Forms.Button button_PromoteSelection;
		private System.Windows.Forms.GroupBox group_Colour;
		private System.Windows.Forms.GroupBox group_Selections;
		private System.Windows.Forms.ComboBox combo_ChainIDs;
		private System.Windows.Forms.MenuItem menu_Colour_Apply1;
		private System.Windows.Forms.MenuItem menu_Colour_ApplyGradient;
		private System.Windows.Forms.MenuItem menu_Selections_AddNew;
		private System.Windows.Forms.MenuItem menu_Selections_Remove;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.TextBox text_SelName;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menu_Selections_Common_NoSolvent;
		private System.Windows.Forms.MenuItem menu_Selections_Common_GradientChains;
		private System.Windows.Forms.MenuItem menu_Selections_InvertAllAtom;
		private System.Windows.Forms.GroupBox group_DrawModes;
		private System.Windows.Forms.Button button_ResetName;
		private System.Windows.Forms.ComboBox combo_SelectionList;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.MenuItem menu_Colour_ApplyDefault;
		private System.Windows.Forms.CheckBox check_EasyViewMode;
		private System.Windows.Forms.MenuItem menu_RemoveAllSels;
		private System.Windows.Forms.MenuItem menu_InvertSelection;
		private System.Windows.Forms.MenuItem menu_CopyAndInvertCurrentSelection;
		private System.Windows.Forms.MenuItem menu_ToggleEnabled;
		private System.Windows.Forms.MenuItem menu_Selections_GradandnotSolv;
		private System.Windows.Forms.MenuItem menu_Selections_BackboneToAll;
		private UoB.CoreControls.Controls.PolymerRange m_MolRange;
		private System.Windows.Forms.MenuItem menu_Selections_SaveDefFile;
		private System.Windows.Forms.MenuItem menu_Selections_LoadDefFile;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.GroupBox CenterControl;
		private System.Windows.Forms.Label label1;	
		private System.Windows.Forms.CheckBox centering_AutoRecenter;
		private System.Windows.Forms.Button centering_CenterPS;
		private System.Windows.Forms.NumericUpDown centering_ResidueID;
		private System.Windows.Forms.ComboBox centering_CenteringChainID;
		private System.Windows.Forms.ComboBox centering_ModeSelect;
		private System.Windows.Forms.GroupBox group_DrawControl;
		private System.Windows.Forms.CheckBox check_HB;
		private System.Windows.Forms.CheckBox check_ForceVector;
		private System.Windows.Forms.CheckBox check_ExtraLines;

		private UpdateEvent m_SelectionListUpdate;
		private UpdateEvent m_FocusUpdate;
		private UpdateEvent m_DrawExtrasUpdate;
		private ParticleSystemDrawWrapper m_DrawWrapper;
		private int m_ChainColourCounter = 0;
		private bool m_SetupOccuring = false;
		private bool m_SelectionListSetupOccuring = false;
		private FocusDefinition m_FocDef = null;
        private MenuItem menu_Selections_SaveDefFileDAVEDef;
        private MenuItem menu_Selections_SaveDefFileFileDef;
        private FileInfo m_FileInfo = null;
				
		private readonly int[] typeFlags = new int[] { 1, 2, 4, 8 }; // int representations of the flagged Enum
		
		public PS_Selections()
		{
			Init();
			setupButtons(); // for the null state
		}

		public PS_Selections( ParticleSystemDrawWrapper drawWrapper ) // setup in static mode
		{
			Init();
			PSDrawWrapper = drawWrapper; // will set up the buttons too, and also take care of event subscription
		}

		private void Init()
		{
			// pre form init
			Text = "PS DrawStyle";
			m_SelectionListUpdate = new UpdateEvent(setupButtons);
			m_FocusUpdate = new UpdateEvent( LoadPSCentering );
			m_DrawExtrasUpdate = new UpdateEvent( LoadDrawExtras );

			InitializeComponent();

			// post form init
			toolTip.SetToolTip( text_SelName, "Input a name for the selection, or leave blank for autoname" );
			toolTip.SetToolTip( button_DemoteSelection, "Selection order is important, the higher up the list, the higher the priority of the settings");
			toolTip.SetToolTip( button_PromoteSelection, "Selection order is important, the higher up the list, the higher the priority of the settings");
			toolTip.SetToolTip( combo_SelectionList, "Checked status denotes that changes are applied to those selections" );	

			m_SetupOccuring = true;
				centering_CenteringChainID.ValueMember = "ChainID"; // just show the chainID in the box
				SetupCentering();
			m_SetupOccuring = false;
		}

		public void AttachToDocument( Document doc )
		{
			// get the wrapper
			m_DrawWrapper = null;
			if ( doc != null )
			{
                for (int i = 0; i < doc.MemberCount; i++)
                {
                    if (doc[i].GetType() == typeof(ParticleSystemDrawWrapper))
                    {
                        PSDrawWrapper = (ParticleSystemDrawWrapper)doc[i];
                        // Important to use the "external" version
                        // setupButtons(); is called by setting PSDrawWrapper
                        break; // only one drawWrapper should ever be present, so dont check any more tools
                    }
                }
                for( int i = 0; i < doc.MemberCount; i++ )
				{
                    FileInfo fi = doc[i] as System.IO.FileInfo;
                    if (fi != null)
                    {
                        m_FileInfo = fi;
                        break;
                    }
				}
			}
			if ( m_DrawWrapper == null )
			{
				setupButtons();
			}

            // only allow this if we know the filename
            menu_Selections_SaveDefFileFileDef.Enabled = (m_FileInfo != null);
		}
		
		private void setupButtons()
		{
			if( m_DrawWrapper == null || m_DrawWrapper.particleSystem == null || m_DrawWrapper.GetSelectionLockState() )
			{
				for( int i = 0; i < Controls.Count; i++ )
				{
					Controls[i].Enabled = false;
				}
				for( int i = 0; i < list_Drawtype.Items.Count; i++ )
				{
					list_Drawtype.Items[i].BackColor = System.Drawing.SystemColors.InactiveBorder;
				}
				return; // no more stuff is relevent
			}
			else
			{
				for( int i = 0; i < Controls.Count; i++ )
				{
					Controls[i].Enabled = true;
				}
				for( int i = 0; i < list_Drawtype.Items.Count; i++ )
				{
					list_Drawtype.Items[i].BackColor = System.Drawing.SystemColors.Window;
				}

				// get available PSMolContainers
				PSMolContainer[] mols = m_DrawWrapper.particleSystem.Members;
				combo_ChainIDs.Items.Clear();
				centering_CenteringChainID.Items.Clear();
				for( int i = 0; i < mols.Length; i++ )
				{
					combo_ChainIDs.Items.Add( mols[i] );
					centering_CenteringChainID.Items.Add( mols[i] );
				}

				// get current selections
				PopulateSelectionList();
				// this then calls the events to allow other button setup
			}
		}

		private void PopulateSelectionList()
		{
			m_SelectionListSetupOccuring = true;

			if( m_DrawWrapper.GetSelectionLockState() )
			{
				combo_SelectionList.DataSource = null;
				combo_SelectionList.Items.Add( "Selection lock in place" );		
				setupButtons();		
			}
			else
			{
				Selection[] sels = m_DrawWrapper.Selections;
				combo_SelectionList.DataSource = sels;
				combo_SelectionList.DisplayMember = "Name";
				combo_SelectionList.ValueMember = "Name";
				combo_SelectionList.SelectedIndex = m_DrawWrapper.CurrentSelectionIndex;
			}

			m_SelectionListSetupOccuring = false;
		}

		private void getCurrentSelectionInfo()
		{
			m_SetupOccuring = true;

			Selection s = m_DrawWrapper.CurrentSelection;
			panel_Colour1.BackColor = s.Colour1.color;
			panel_Colour2.BackColor = s.Colour2.color;

			for( int i = 0; i < typeFlags.Length; i++ )
			{
				list_Drawtype.Items[i].Checked = ( ( s.DrawStyle & (AtomDrawStyle)typeFlags[i] ) > 0 );
			}

			combo_DrawStyle.SelectedIndex = (int) s.DisplayMode;

			// sel range setup
			combo_ChainIDs.SelectedIndex = -1;
			combo_ChainIDs.Enabled = false;
			if( s is GlobalSelection )
			{
				text_SelName.Text = "Global Selection";
				m_MolRange.Polymer = null;
			}
			else if( s is AminoAcidSelection )
			{
				combo_ChainIDs.Enabled = true;
				for( int i = 0; i < combo_ChainIDs.Items.Count; i++ )
				{
					AminoAcidSelection selAA = (AminoAcidSelection)s;

					if( selAA.Molecule == combo_ChainIDs.Items[i] )
					{

						text_SelName.Text = selAA.Name;

						combo_ChainIDs.SelectedIndex = i;
						m_MolRange.Polymer = selAA.Molecule;
						m_MolRange.SetRange( selAA.Start, selAA.End - selAA.Start + 1);
						break;
					}
				}
			}
			else if( s is Selection_CAlphaEquiv )
			{								
				text_SelName.Text = "C-Alpha Equivelencies";
				m_MolRange.Polymer = null;
			}
			else
			{
			}
			m_SetupOccuring = false;
		}

		public ParticleSystemDrawWrapper PSDrawWrapper
		{
			get
			{
				return m_DrawWrapper;
			}
			set
			{
				if( m_DrawWrapper != null )
				{
					m_DrawWrapper.SelectionUpdated -= m_SelectionListUpdate;
				}
				if( m_FocDef != null )
				{
					m_FocDef.FocusUpdate -= m_FocusUpdate;
				}

				m_DrawWrapper = value;

				if( m_DrawWrapper != null )
				{
					m_DrawWrapper.SelectionUpdated += m_SelectionListUpdate;
					if( m_DrawWrapper.particleSystem != null )
					{
						m_FocDef = m_DrawWrapper.particleSystem.SystemFocus;
						if( m_FocDef != null ) // add if the draw wrapper is ok, it always should be
						{
							m_FocDef.FocusUpdate += m_FocusUpdate;
						}
					}
				}
				
				setupButtons();
				LoadPSCentering(); // not called by setupButtons
				LoadDrawExtras();
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Lines");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Ball and Stick");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("CPK");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("Ribbon");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PS_Selections));
            this.group_DrawModes = new System.Windows.Forms.GroupBox();
            this.combo_DrawStyle = new System.Windows.Forms.ComboBox();
            this.list_Drawtype = new System.Windows.Forms.ListView();
            this.panel_Colour1 = new System.Windows.Forms.Panel();
            this.panel_Colour2 = new System.Windows.Forms.Panel();
            this.button_ColourOptions = new System.Windows.Forms.Button();
            this.group_Colour = new System.Windows.Forms.GroupBox();
            this.panel_ColourF = new System.Windows.Forms.Panel();
            this.panel_ColourE = new System.Windows.Forms.Panel();
            this.panel_ColourD = new System.Windows.Forms.Panel();
            this.panel_ColourB = new System.Windows.Forms.Panel();
            this.panel_ColourA = new System.Windows.Forms.Panel();
            this.panel_ColourC = new System.Windows.Forms.Panel();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.group_Selections = new System.Windows.Forms.GroupBox();
            this.combo_SelectionList = new System.Windows.Forms.ComboBox();
            this.button_PromoteSelection = new System.Windows.Forms.Button();
            this.button_DemoteSelection = new System.Windows.Forms.Button();
            this.button_SelectionOptions = new System.Windows.Forms.Button();
            this.check_EasyViewMode = new System.Windows.Forms.CheckBox();
            this.text_SelName = new System.Windows.Forms.TextBox();
            this.combo_ChainIDs = new System.Windows.Forms.ComboBox();
            this.menu_Colour = new System.Windows.Forms.ContextMenu();
            this.menu_Colour_ApplyDefault = new System.Windows.Forms.MenuItem();
            this.menu_Colour_Apply1 = new System.Windows.Forms.MenuItem();
            this.menu_Colour_ApplyGradient = new System.Windows.Forms.MenuItem();
            this.menu_Selections = new System.Windows.Forms.ContextMenu();
            this.menu_Selections_AddNew = new System.Windows.Forms.MenuItem();
            this.menu_ToggleEnabled = new System.Windows.Forms.MenuItem();
            this.menu_Selections_Remove = new System.Windows.Forms.MenuItem();
            this.menu_RemoveAllSels = new System.Windows.Forms.MenuItem();
            this.menu_Selections_InvertAllAtom = new System.Windows.Forms.MenuItem();
            this.menu_InvertSelection = new System.Windows.Forms.MenuItem();
            this.menu_CopyAndInvertCurrentSelection = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menu_Selections_SaveDefFile = new System.Windows.Forms.MenuItem();
            this.menu_Selections_LoadDefFile = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menu_Selections_Common_NoSolvent = new System.Windows.Forms.MenuItem();
            this.menu_Selections_Common_GradientChains = new System.Windows.Forms.MenuItem();
            this.menu_Selections_GradandnotSolv = new System.Windows.Forms.MenuItem();
            this.menu_Selections_BackboneToAll = new System.Windows.Forms.MenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.centering_AutoRecenter = new System.Windows.Forms.CheckBox();
            this.centering_CenterPS = new System.Windows.Forms.Button();
            this.check_HB = new System.Windows.Forms.CheckBox();
            this.check_ForceVector = new System.Windows.Forms.CheckBox();
            this.check_ExtraLines = new System.Windows.Forms.CheckBox();
            this.button_ResetName = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_MolRange = new UoB.CoreControls.Controls.PolymerRange();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.CenterControl = new System.Windows.Forms.GroupBox();
            this.centering_ResidueID = new System.Windows.Forms.NumericUpDown();
            this.centering_CenteringChainID = new System.Windows.Forms.ComboBox();
            this.centering_ModeSelect = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.group_DrawControl = new System.Windows.Forms.GroupBox();
            this.menu_Selections_SaveDefFileDAVEDef = new System.Windows.Forms.MenuItem();
            this.menu_Selections_SaveDefFileFileDef = new System.Windows.Forms.MenuItem();
            this.group_DrawModes.SuspendLayout();
            this.group_Colour.SuspendLayout();
            this.group_Selections.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.CenterControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.centering_ResidueID)).BeginInit();
            this.group_DrawControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // group_DrawModes
            // 
            this.group_DrawModes.Controls.Add(this.combo_DrawStyle);
            this.group_DrawModes.Controls.Add(this.list_Drawtype);
            this.group_DrawModes.Location = new System.Drawing.Point(8, 224);
            this.group_DrawModes.Name = "group_DrawModes";
            this.group_DrawModes.Size = new System.Drawing.Size(168, 120);
            this.group_DrawModes.TabIndex = 0;
            this.group_DrawModes.TabStop = false;
            this.group_DrawModes.Text = "Draw Modes";
            // 
            // combo_DrawStyle
            // 
            this.combo_DrawStyle.Items.AddRange(new object[] {
            "All Atoms",
            "Heavy Atoms",
            "BackBone Only",
            "C-Alpha Trace",
            "Invisible"});
            this.combo_DrawStyle.Location = new System.Drawing.Point(8, 92);
            this.combo_DrawStyle.Name = "combo_DrawStyle";
            this.combo_DrawStyle.Size = new System.Drawing.Size(152, 21);
            this.combo_DrawStyle.TabIndex = 12;
            this.combo_DrawStyle.SelectedIndexChanged += new System.EventHandler(this.combo_DrawStyle_SelectedIndexChanged);
            // 
            // list_Drawtype
            // 
            this.list_Drawtype.CheckBoxes = true;
            listViewItem5.StateImageIndex = 0;
            listViewItem5.Tag = "1";
            listViewItem6.StateImageIndex = 0;
            listViewItem6.Tag = "2";
            listViewItem7.StateImageIndex = 0;
            listViewItem7.Tag = "4";
            listViewItem8.StateImageIndex = 0;
            listViewItem8.Tag = "8";
            this.list_Drawtype.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8});
            this.list_Drawtype.Location = new System.Drawing.Point(8, 16);
            this.list_Drawtype.Name = "list_Drawtype";
            this.list_Drawtype.Scrollable = false;
            this.list_Drawtype.Size = new System.Drawing.Size(152, 72);
            this.list_Drawtype.TabIndex = 13;
            this.list_Drawtype.UseCompatibleStateImageBehavior = false;
            this.list_Drawtype.View = System.Windows.Forms.View.SmallIcon;
            this.list_Drawtype.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.list_Drawtype_ItemCheck);
            // 
            // panel_Colour1
            // 
            this.panel_Colour1.BackColor = System.Drawing.Color.Red;
            this.panel_Colour1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_Colour1.Location = new System.Drawing.Point(16, 40);
            this.panel_Colour1.Name = "panel_Colour1";
            this.panel_Colour1.Size = new System.Drawing.Size(64, 24);
            this.panel_Colour1.TabIndex = 1;
            this.panel_Colour1.Tag = "1";
            this.panel_Colour1.Click += new System.EventHandler(this.panel_Colour_Click);
            // 
            // panel_Colour2
            // 
            this.panel_Colour2.BackColor = System.Drawing.Color.Blue;
            this.panel_Colour2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_Colour2.Location = new System.Drawing.Point(88, 40);
            this.panel_Colour2.Name = "panel_Colour2";
            this.panel_Colour2.Size = new System.Drawing.Size(64, 24);
            this.panel_Colour2.TabIndex = 2;
            this.panel_Colour2.Tag = "2";
            this.panel_Colour2.Click += new System.EventHandler(this.panel_Colour_Click);
            // 
            // button_ColourOptions
            // 
            this.button_ColourOptions.Location = new System.Drawing.Point(40, 72);
            this.button_ColourOptions.Name = "button_ColourOptions";
            this.button_ColourOptions.Size = new System.Drawing.Size(88, 24);
            this.button_ColourOptions.TabIndex = 4;
            this.button_ColourOptions.Text = "< Options >";
            this.button_ColourOptions.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button_ColourOptions_MouseUp);
            // 
            // group_Colour
            // 
            this.group_Colour.Controls.Add(this.panel_ColourF);
            this.group_Colour.Controls.Add(this.panel_ColourE);
            this.group_Colour.Controls.Add(this.panel_ColourD);
            this.group_Colour.Controls.Add(this.panel_ColourB);
            this.group_Colour.Controls.Add(this.panel_ColourA);
            this.group_Colour.Controls.Add(this.panel_Colour1);
            this.group_Colour.Controls.Add(this.panel_Colour2);
            this.group_Colour.Controls.Add(this.button_ColourOptions);
            this.group_Colour.Controls.Add(this.panel_ColourC);
            this.group_Colour.Location = new System.Drawing.Point(8, 488);
            this.group_Colour.Name = "group_Colour";
            this.group_Colour.Size = new System.Drawing.Size(168, 104);
            this.group_Colour.TabIndex = 5;
            this.group_Colour.TabStop = false;
            this.group_Colour.Text = "Colour Control";
            // 
            // panel_ColourF
            // 
            this.panel_ColourF.BackColor = System.Drawing.Color.Aqua;
            this.panel_ColourF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_ColourF.Location = new System.Drawing.Point(136, 16);
            this.panel_ColourF.Name = "panel_ColourF";
            this.panel_ColourF.Size = new System.Drawing.Size(16, 16);
            this.panel_ColourF.TabIndex = 10;
            this.panel_ColourF.Tag = "5";
            this.panel_ColourF.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel_CacheColourMouseUp);
            // 
            // panel_ColourE
            // 
            this.panel_ColourE.BackColor = System.Drawing.Color.Magenta;
            this.panel_ColourE.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_ColourE.Location = new System.Drawing.Point(112, 16);
            this.panel_ColourE.Name = "panel_ColourE";
            this.panel_ColourE.Size = new System.Drawing.Size(16, 16);
            this.panel_ColourE.TabIndex = 9;
            this.panel_ColourE.Tag = "4";
            this.panel_ColourE.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel_CacheColourMouseUp);
            // 
            // panel_ColourD
            // 
            this.panel_ColourD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.panel_ColourD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_ColourD.Location = new System.Drawing.Point(88, 16);
            this.panel_ColourD.Name = "panel_ColourD";
            this.panel_ColourD.Size = new System.Drawing.Size(16, 16);
            this.panel_ColourD.TabIndex = 8;
            this.panel_ColourD.Tag = "3";
            this.panel_ColourD.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel_CacheColourMouseUp);
            // 
            // panel_ColourB
            // 
            this.panel_ColourB.BackColor = System.Drawing.Color.Blue;
            this.panel_ColourB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_ColourB.Location = new System.Drawing.Point(40, 16);
            this.panel_ColourB.Name = "panel_ColourB";
            this.panel_ColourB.Size = new System.Drawing.Size(16, 16);
            this.panel_ColourB.TabIndex = 6;
            this.panel_ColourB.Tag = "1";
            this.panel_ColourB.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel_CacheColourMouseUp);
            // 
            // panel_ColourA
            // 
            this.panel_ColourA.BackColor = System.Drawing.Color.Red;
            this.panel_ColourA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_ColourA.Location = new System.Drawing.Point(16, 16);
            this.panel_ColourA.Name = "panel_ColourA";
            this.panel_ColourA.Size = new System.Drawing.Size(16, 16);
            this.panel_ColourA.TabIndex = 5;
            this.panel_ColourA.Tag = "0";
            this.panel_ColourA.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel_CacheColourMouseUp);
            // 
            // panel_ColourC
            // 
            this.panel_ColourC.BackColor = System.Drawing.Color.Yellow;
            this.panel_ColourC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_ColourC.Location = new System.Drawing.Point(64, 16);
            this.panel_ColourC.Name = "panel_ColourC";
            this.panel_ColourC.Size = new System.Drawing.Size(16, 16);
            this.panel_ColourC.TabIndex = 7;
            this.panel_ColourC.Tag = "2";
            this.panel_ColourC.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel_CacheColourMouseUp);
            // 
            // group_Selections
            // 
            this.group_Selections.Controls.Add(this.combo_SelectionList);
            this.group_Selections.Controls.Add(this.button_PromoteSelection);
            this.group_Selections.Controls.Add(this.button_DemoteSelection);
            this.group_Selections.Controls.Add(this.button_SelectionOptions);
            this.group_Selections.Controls.Add(this.check_EasyViewMode);
            this.group_Selections.Location = new System.Drawing.Point(8, 128);
            this.group_Selections.Name = "group_Selections";
            this.group_Selections.Size = new System.Drawing.Size(168, 96);
            this.group_Selections.TabIndex = 7;
            this.group_Selections.TabStop = false;
            this.group_Selections.Text = "Selections";
            // 
            // combo_SelectionList
            // 
            this.combo_SelectionList.Location = new System.Drawing.Point(8, 16);
            this.combo_SelectionList.Name = "combo_SelectionList";
            this.combo_SelectionList.Size = new System.Drawing.Size(152, 21);
            this.combo_SelectionList.TabIndex = 18;
            this.combo_SelectionList.SelectedIndexChanged += new System.EventHandler(this.combo_SelectionList_SelectedIndexChanged);
            // 
            // button_PromoteSelection
            // 
            this.button_PromoteSelection.Location = new System.Drawing.Point(136, 40);
            this.button_PromoteSelection.Name = "button_PromoteSelection";
            this.button_PromoteSelection.Size = new System.Drawing.Size(24, 24);
            this.button_PromoteSelection.TabIndex = 15;
            this.button_PromoteSelection.Text = ">";
            this.button_PromoteSelection.Click += new System.EventHandler(this.button_PromoteSelection_Click);
            // 
            // button_DemoteSelection
            // 
            this.button_DemoteSelection.Location = new System.Drawing.Point(8, 40);
            this.button_DemoteSelection.Name = "button_DemoteSelection";
            this.button_DemoteSelection.Size = new System.Drawing.Size(24, 24);
            this.button_DemoteSelection.TabIndex = 14;
            this.button_DemoteSelection.Text = "<";
            this.button_DemoteSelection.Click += new System.EventHandler(this.button_DemoteSelection_Click);
            // 
            // button_SelectionOptions
            // 
            this.button_SelectionOptions.Location = new System.Drawing.Point(40, 40);
            this.button_SelectionOptions.Name = "button_SelectionOptions";
            this.button_SelectionOptions.Size = new System.Drawing.Size(88, 24);
            this.button_SelectionOptions.TabIndex = 10;
            this.button_SelectionOptions.Text = "< Options >";
            this.button_SelectionOptions.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button_SelectionOptions_MouseUp);
            // 
            // check_EasyViewMode
            // 
            this.check_EasyViewMode.Location = new System.Drawing.Point(8, 64);
            this.check_EasyViewMode.Name = "check_EasyViewMode";
            this.check_EasyViewMode.Size = new System.Drawing.Size(152, 24);
            this.check_EasyViewMode.TabIndex = 19;
            this.check_EasyViewMode.Text = "Selection EasyView";
            this.check_EasyViewMode.CheckedChanged += new System.EventHandler(this.check_EasyViewMode_CheckedChanged);
            // 
            // text_SelName
            // 
            this.text_SelName.Location = new System.Drawing.Point(8, 40);
            this.text_SelName.Name = "text_SelName";
            this.text_SelName.Size = new System.Drawing.Size(120, 20);
            this.text_SelName.TabIndex = 16;
            // 
            // combo_ChainIDs
            // 
            this.combo_ChainIDs.Location = new System.Drawing.Point(8, 16);
            this.combo_ChainIDs.Name = "combo_ChainIDs";
            this.combo_ChainIDs.Size = new System.Drawing.Size(152, 21);
            this.combo_ChainIDs.TabIndex = 11;
            this.combo_ChainIDs.SelectedIndexChanged += new System.EventHandler(this.combo_ChainIDs_SelectedIndexChanged);
            // 
            // menu_Colour
            // 
            this.menu_Colour.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_Colour_ApplyDefault,
            this.menu_Colour_Apply1,
            this.menu_Colour_ApplyGradient});
            // 
            // menu_Colour_ApplyDefault
            // 
            this.menu_Colour_ApplyDefault.Index = 0;
            this.menu_Colour_ApplyDefault.Text = "Apply ForceField Default";
            this.menu_Colour_ApplyDefault.Click += new System.EventHandler(this.menu_Colour_Apply2_Click);
            // 
            // menu_Colour_Apply1
            // 
            this.menu_Colour_Apply1.Index = 1;
            this.menu_Colour_Apply1.Text = "Apply Colour 1";
            this.menu_Colour_Apply1.Click += new System.EventHandler(this.menu_Colour_Apply1_Click);
            // 
            // menu_Colour_ApplyGradient
            // 
            this.menu_Colour_ApplyGradient.Index = 2;
            this.menu_Colour_ApplyGradient.Text = "Apply Gradient";
            this.menu_Colour_ApplyGradient.Click += new System.EventHandler(this.menu_Colour_ApplyGradient_Click);
            // 
            // menu_Selections
            // 
            this.menu_Selections.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_Selections_AddNew,
            this.menu_ToggleEnabled,
            this.menu_Selections_Remove,
            this.menu_RemoveAllSels,
            this.menu_Selections_InvertAllAtom,
            this.menuItem4,
            this.menu_Selections_LoadDefFile,
            this.menu_Selections_SaveDefFile,
            this.menu_Selections_SaveDefFileDAVEDef,
            this.menu_Selections_SaveDefFileFileDef,
            this.menuItem1,
            this.menu_Selections_Common_NoSolvent,
            this.menu_Selections_Common_GradientChains,
            this.menu_Selections_GradandnotSolv,
            this.menu_Selections_BackboneToAll});
            // 
            // menu_Selections_AddNew
            // 
            this.menu_Selections_AddNew.Index = 0;
            this.menu_Selections_AddNew.Text = "Add New Selection";
            this.menu_Selections_AddNew.Click += new System.EventHandler(this.menu_Selections_AddNew_Click);
            // 
            // menu_ToggleEnabled
            // 
            this.menu_ToggleEnabled.Index = 1;
            this.menu_ToggleEnabled.Text = "Enabled-State on Current Selection";
            this.menu_ToggleEnabled.Click += new System.EventHandler(this.menu_ToggleEnabled_Click);
            // 
            // menu_Selections_Remove
            // 
            this.menu_Selections_Remove.Index = 2;
            this.menu_Selections_Remove.Text = "Remove Current Selection";
            this.menu_Selections_Remove.Click += new System.EventHandler(this.menu_Selections_Remove_Click);
            // 
            // menu_RemoveAllSels
            // 
            this.menu_RemoveAllSels.Index = 3;
            this.menu_RemoveAllSels.Text = "Remove All Selections";
            this.menu_RemoveAllSels.Click += new System.EventHandler(this.menu_RemoveAllSels_Click);
            // 
            // menu_Selections_InvertAllAtom
            // 
            this.menu_Selections_InvertAllAtom.Index = 4;
            this.menu_Selections_InvertAllAtom.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menu_InvertSelection,
            this.menu_CopyAndInvertCurrentSelection});
            this.menu_Selections_InvertAllAtom.Text = "Invert Selection";
            // 
            // menu_InvertSelection
            // 
            this.menu_InvertSelection.Index = 0;
            this.menu_InvertSelection.Text = "Toggle Inverted : Within Current Chain";
            this.menu_InvertSelection.Click += new System.EventHandler(this.menu_InvertSelection_Click);
            // 
            // menu_CopyAndInvertCurrentSelection
            // 
            this.menu_CopyAndInvertCurrentSelection.Index = 1;
            this.menu_CopyAndInvertCurrentSelection.Text = "Copy and Toggle Inverted: Within Current Chain";
            this.menu_CopyAndInvertCurrentSelection.Click += new System.EventHandler(this.menu_CopyAndInvertCurrentSelection_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 5;
            this.menuItem4.Text = "-";
            // 
            // menu_Selections_SaveDefFile
            // 
            this.menu_Selections_SaveDefFile.Index = 7;
            this.menu_Selections_SaveDefFile.Text = "Save Def File ...";
            this.menu_Selections_SaveDefFile.Click += new System.EventHandler(this.menu_Selections_SaveDefFile_Click);
            // 
            // menu_Selections_LoadDefFile
            // 
            this.menu_Selections_LoadDefFile.Index = 6;
            this.menu_Selections_LoadDefFile.Text = "Load Def File ...";
            this.menu_Selections_LoadDefFile.Click += new System.EventHandler(this.menu_Selections_LoadDefFile_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 10;
            this.menuItem1.Text = "-";
            // 
            // menu_Selections_Common_NoSolvent
            // 
            this.menu_Selections_Common_NoSolvent.Index = 11;
            this.menu_Selections_Common_NoSolvent.Text = "Common : Dont Show Solvent";
            this.menu_Selections_Common_NoSolvent.Click += new System.EventHandler(this.menu_Selections_Common_NoSolvent_Click);
            // 
            // menu_Selections_Common_GradientChains
            // 
            this.menu_Selections_Common_GradientChains.Index = 12;
            this.menu_Selections_Common_GradientChains.Text = "Common : Gradient All Chains";
            this.menu_Selections_Common_GradientChains.Click += new System.EventHandler(this.menu_Selections_Common_GradientChains_Click);
            // 
            // menu_Selections_GradandnotSolv
            // 
            this.menu_Selections_GradandnotSolv.Index = 13;
            this.menu_Selections_GradandnotSolv.Text = "Common : Both Gradient Chains and No Solvent";
            this.menu_Selections_GradandnotSolv.Click += new System.EventHandler(this.menu_GradandnotSolv_Click);
            // 
            // menu_Selections_BackboneToAll
            // 
            this.menu_Selections_BackboneToAll.Index = 14;
            this.menu_Selections_BackboneToAll.Text = "Common : Apply backbone-only to all selections";
            this.menu_Selections_BackboneToAll.Click += new System.EventHandler(this.menu_Selections_BackboneToAll_Click);
            // 
            // toolTip
            // 
            this.toolTip.ShowAlways = true;
            // 
            // centering_AutoRecenter
            // 
            this.centering_AutoRecenter.BackColor = System.Drawing.SystemColors.Control;
            this.centering_AutoRecenter.Location = new System.Drawing.Point(32, 16);
            this.centering_AutoRecenter.Name = "centering_AutoRecenter";
            this.centering_AutoRecenter.Size = new System.Drawing.Size(16, 24);
            this.centering_AutoRecenter.TabIndex = 4;
            this.centering_AutoRecenter.Text = "checkBox1";
            this.toolTip.SetToolTip(this.centering_AutoRecenter, "Click here to perform a re-centering per timestep....");
            this.centering_AutoRecenter.UseVisualStyleBackColor = false;
            this.centering_AutoRecenter.CheckedChanged += new System.EventHandler(this.centering_AutoRecenter_CheckedChanged);
            // 
            // centering_CenterPS
            // 
            this.centering_CenterPS.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.centering_CenterPS.Image = ((System.Drawing.Image)(resources.GetObject("centering_CenterPS.Image")));
            this.centering_CenterPS.Location = new System.Drawing.Point(8, 16);
            this.centering_CenterPS.Name = "centering_CenterPS";
            this.centering_CenterPS.Size = new System.Drawing.Size(24, 24);
            this.centering_CenterPS.TabIndex = 3;
            this.toolTip.SetToolTip(this.centering_CenterPS, "Click to recenter the ParticleSystem");
            this.centering_CenterPS.Click += new System.EventHandler(this.centering_CenterPS_Click);
            // 
            // check_HB
            // 
            this.check_HB.Location = new System.Drawing.Point(8, 16);
            this.check_HB.Name = "check_HB";
            this.check_HB.Size = new System.Drawing.Size(40, 16);
            this.check_HB.TabIndex = 0;
            this.check_HB.Text = "HB";
            this.toolTip.SetToolTip(this.check_HB, "Hydrogen Bonding View Control");
            this.check_HB.CheckedChanged += new System.EventHandler(this.check_HB_CheckedChanged);
            // 
            // check_ForceVector
            // 
            this.check_ForceVector.Location = new System.Drawing.Point(56, 16);
            this.check_ForceVector.Name = "check_ForceVector";
            this.check_ForceVector.Size = new System.Drawing.Size(40, 16);
            this.check_ForceVector.TabIndex = 1;
            this.check_ForceVector.Text = "FV";
            this.toolTip.SetToolTip(this.check_ForceVector, "Force Vector View Control");
            this.check_ForceVector.CheckedChanged += new System.EventHandler(this.check_ForceVector_CheckedChanged);
            // 
            // check_ExtraLines
            // 
            this.check_ExtraLines.Location = new System.Drawing.Point(104, 16);
            this.check_ExtraLines.Name = "check_ExtraLines";
            this.check_ExtraLines.Size = new System.Drawing.Size(40, 16);
            this.check_ExtraLines.TabIndex = 2;
            this.check_ExtraLines.Text = "EL";
            this.toolTip.SetToolTip(this.check_ExtraLines, "Aditional Line Matrix View Control");
            this.check_ExtraLines.CheckedChanged += new System.EventHandler(this.check_ExtraLines_CheckedChanged);
            // 
            // button_ResetName
            // 
            this.button_ResetName.Location = new System.Drawing.Point(128, 40);
            this.button_ResetName.Name = "button_ResetName";
            this.button_ResetName.Size = new System.Drawing.Size(32, 23);
            this.button_ResetName.TabIndex = 17;
            this.button_ResetName.Text = "<-";
            this.button_ResetName.Click += new System.EventHandler(this.button_ResetName_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.m_MolRange);
            this.groupBox1.Controls.Add(this.button_ResetName);
            this.groupBox1.Controls.Add(this.text_SelName);
            this.groupBox1.Controls.Add(this.combo_ChainIDs);
            this.groupBox1.Location = new System.Drawing.Point(8, 344);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(168, 144);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Chain Definition";
            // 
            // m_MolRange
            // 
            this.m_MolRange.Location = new System.Drawing.Point(8, 64);
            this.m_MolRange.Name = "m_MolRange";
            this.m_MolRange.Polymer = null;
            this.m_MolRange.Size = new System.Drawing.Size(152, 72);
            this.m_MolRange.TabIndex = 18;
            this.m_MolRange.RangeUpdated += new UoB.Core.Primitives.RangeChange(this.m_MolRange_RangeUpdated);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "DAVE Selections Def File|*.DSD";
            this.openFileDialog.Title = "Open Selection Definitions File...";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "DAVE Selections Def File|*.DSD";
            this.saveFileDialog.Title = "Save Selection Definitions File...";
            // 
            // CenterControl
            // 
            this.CenterControl.Controls.Add(this.centering_ResidueID);
            this.CenterControl.Controls.Add(this.centering_CenteringChainID);
            this.CenterControl.Controls.Add(this.centering_ModeSelect);
            this.CenterControl.Controls.Add(this.label1);
            this.CenterControl.Controls.Add(this.centering_AutoRecenter);
            this.CenterControl.Controls.Add(this.centering_CenterPS);
            this.CenterControl.Location = new System.Drawing.Point(8, 8);
            this.CenterControl.Name = "CenterControl";
            this.CenterControl.Size = new System.Drawing.Size(168, 72);
            this.CenterControl.TabIndex = 9;
            this.CenterControl.TabStop = false;
            this.CenterControl.Text = "Centering Control";
            // 
            // centering_ResidueID
            // 
            this.centering_ResidueID.Location = new System.Drawing.Point(96, 44);
            this.centering_ResidueID.Name = "centering_ResidueID";
            this.centering_ResidueID.Size = new System.Drawing.Size(64, 20);
            this.centering_ResidueID.TabIndex = 14;
            this.centering_ResidueID.ValueChanged += new System.EventHandler(this.centering_ResidueID_ValueChanged);
            // 
            // centering_CenteringChainID
            // 
            this.centering_CenteringChainID.Location = new System.Drawing.Point(36, 44);
            this.centering_CenteringChainID.Name = "centering_CenteringChainID";
            this.centering_CenteringChainID.Size = new System.Drawing.Size(56, 21);
            this.centering_CenteringChainID.TabIndex = 13;
            this.centering_CenteringChainID.SelectedIndexChanged += new System.EventHandler(this.centering_CenteringChainID_SelectedIndexChanged);
            // 
            // centering_ModeSelect
            // 
            this.centering_ModeSelect.Location = new System.Drawing.Point(56, 16);
            this.centering_ModeSelect.Name = "centering_ModeSelect";
            this.centering_ModeSelect.Size = new System.Drawing.Size(104, 21);
            this.centering_ModeSelect.TabIndex = 12;
            this.centering_ModeSelect.SelectedIndexChanged += new System.EventHandler(this.centering_ModeSelect_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 16);
            this.label1.TabIndex = 10;
            this.label1.Text = "ID : ";
            // 
            // group_DrawControl
            // 
            this.group_DrawControl.Controls.Add(this.check_ExtraLines);
            this.group_DrawControl.Controls.Add(this.check_ForceVector);
            this.group_DrawControl.Controls.Add(this.check_HB);
            this.group_DrawControl.Location = new System.Drawing.Point(8, 88);
            this.group_DrawControl.Name = "group_DrawControl";
            this.group_DrawControl.Size = new System.Drawing.Size(168, 40);
            this.group_DrawControl.TabIndex = 10;
            this.group_DrawControl.TabStop = false;
            this.group_DrawControl.Text = "Aditional Draw Control";
            // 
            // menu_Selections_SaveDefFileDAVEDef
            // 
            this.menu_Selections_SaveDefFileDAVEDef.Index = 8;
            this.menu_Selections_SaveDefFileDAVEDef.Text = "Save As DAVE Default ...";
            this.menu_Selections_SaveDefFileDAVEDef.Click += new System.EventHandler(this.menu_Selections_SaveDefFileDAVEDef_Click);
            // 
            // menu_Selections_SaveDefFileFileDef
            // 
            this.menu_Selections_SaveDefFileFileDef.Index = 9;
            this.menu_Selections_SaveDefFileFileDef.Text = "Save As Filename Default ...";
            this.menu_Selections_SaveDefFileFileDef.Click += new System.EventHandler(this.menu_Selections_SaveDefFileFileDef_Click);
            // 
            // PS_Selections
            // 
            this.Controls.Add(this.group_DrawControl);
            this.Controls.Add(this.CenterControl);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.group_Selections);
            this.Controls.Add(this.group_Colour);
            this.Controls.Add(this.group_DrawModes);
            this.Name = "PS_Selections";
            this.Size = new System.Drawing.Size(184, 552);
            this.group_DrawModes.ResumeLayout(false);
            this.group_Colour.ResumeLayout(false);
            this.group_Selections.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.CenterControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.centering_ResidueID)).EndInit();
            this.group_DrawControl.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion


		private void panel_Colour_Click(object sender, System.EventArgs e)
		{
			Panel p = (Panel) sender;

			colorDialog.Color = panel_Colour1.BackColor;

			if (colorDialog.ShowDialog(this) == DialogResult.OK)
			{
				p.BackColor =  colorDialog.Color;

				// update the cache if the colour is not present ...
				if( !(
					panel_ColourA.BackColor == colorDialog.Color ||
					panel_ColourB.BackColor == colorDialog.Color ||
					panel_ColourC.BackColor == colorDialog.Color ||
					panel_ColourD.BackColor == colorDialog.Color ||
					panel_ColourE.BackColor == colorDialog.Color ||
					panel_ColourF.BackColor == colorDialog.Color ) )
				{
					panel_ColourF.BackColor = panel_ColourE.BackColor;
					panel_ColourE.BackColor = panel_ColourD.BackColor;
					panel_ColourD.BackColor = panel_ColourC.BackColor;
					panel_ColourC.BackColor = panel_ColourB.BackColor;
					panel_ColourB.BackColor = panel_ColourA.BackColor;
					panel_ColourA.BackColor = colorDialog.Color;
				}
			}
			int colourID = int.Parse((string)p.Tag);

			m_DrawWrapper.BeginSelectionEdit();
			Selection s = m_DrawWrapper.CurrentSelection;
			if( colourID == 1 )
			{
				s.Colour1 = Colour.FromColor( panel_Colour1.BackColor );
			}
			else if ( colourID == 2 )
			{
				s.Colour2 = Colour.FromColor( panel_Colour2.BackColor );
			}
			m_DrawWrapper.EndSelectionEdit();
		}

		private void panel_CacheColourMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_DrawWrapper.BeginSelectionEdit();
			Selection s = m_DrawWrapper.CurrentSelection;
			Panel p;
			if( e.Button == MouseButtons.Left )
			{
				p = panel_Colour1;
			}
			else if( e.Button == MouseButtons.Right )
			{
				p = panel_Colour2;
			}
			else
			{
				return; // centre mouse button if present
			}

			Panel p_Cache = (Panel) sender;
			p.BackColor = p_Cache.BackColor;

			if( e.Button == MouseButtons.Left )
			{
				s.Colour1.Red = p.BackColor.R;
				s.Colour1.Green = p.BackColor.G;
				s.Colour1.Blue = p.BackColor.B;
			}
			else if( e.Button == MouseButtons.Right )
			{
				s.Colour2.Red = p.BackColor.R;
				s.Colour2.Green = p.BackColor.G;
				s.Colour2.Blue = p.BackColor.B;
			}

			m_DrawWrapper.EndSelectionEdit();
		}

		private void button_DemoteSelection_Click(object sender, System.EventArgs e)
		{
			m_DrawWrapper.BeginSelectionEdit();
			m_DrawWrapper.DemoteCurrentSelection();
			m_DrawWrapper.EndSelectionEdit();
		}

		private void button_PromoteSelection_Click(object sender, System.EventArgs e)
		{
			m_DrawWrapper.BeginSelectionEdit();
			m_DrawWrapper.PromoteCurrentSelection();
			m_DrawWrapper.EndSelectionEdit();
		}

		private void button_ColourOptions_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Point position = new Point( group_Colour.Left + button_ColourOptions.Left, 
				group_Colour.Top + button_ColourOptions.Bottom );

			menu_Colour.Show(this,position);  // showing the context menu
		}

		private void button_SelectionOptions_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Point position = new Point( group_Selections.Left + button_SelectionOptions.Left, 
				group_Selections.Top + button_SelectionOptions.Bottom );

			menu_Selections.Show(this,position);  // showing the context menu
		}

		private void menu_Selections_AddNew_Click(object sender, System.EventArgs e)
		{
			m_DrawWrapper.BeginSelectionEdit();
			PSMolContainer mol = (PSMolContainer)combo_ChainIDs.Items[0];
			m_DrawWrapper.AddSelection( mol, 0, mol.Count );
			m_DrawWrapper.EndSelectionEdit();
		}

		private void combo_ChainIDs_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if ( !m_SetupOccuring )
			{
				if ( combo_SelectionList.SelectedIndex >= 0 )
				{
					m_DrawWrapper.BeginSelectionEdit();
					PSMolContainer mol = (PSMolContainer) combo_ChainIDs.Items[combo_ChainIDs.SelectedIndex];
					Selection s = m_DrawWrapper.CurrentSelection;
					if( !(s is GlobalSelection) )
					{
						AminoAcidSelection molSel = (AminoAcidSelection) m_DrawWrapper.CurrentSelection;;
						molSel.Molecule = mol; 
						m_MolRange.Polymer = mol; // setup the range boxes
					}
					m_DrawWrapper.EndSelectionEdit();
				}
			}
		}

		private void combo_SelectionList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if ( !m_SelectionListSetupOccuring ) // set in the populate selection box function
			{
				m_DrawWrapper.CurrentSelectionIndex = combo_SelectionList.SelectedIndex;
			}
			getCurrentSelectionInfo();	
		}

		private void list_Drawtype_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
		{
			if( !m_SetupOccuring )
			{
				m_DrawWrapper.BeginSelectionEdit();
				Selection s = m_DrawWrapper.CurrentSelection;
				AtomDrawStyle setting = AtomDrawStyle.None;

				for( int i = 0; i < list_Drawtype.Items.Count; i++ )
				{
					bool isChecked;
					if( i == e.Index )
					{
						if( e.NewValue == CheckState.Checked ) // anoying checked state thingy, so no one line bool
						{
							isChecked = true;
						}
						else
						{
							isChecked = false;
						}
					}
					else
					{
						isChecked = list_Drawtype.Items[i].Checked;
					}

					if( isChecked )
					{
						setting = setting | (AtomDrawStyle) int.Parse( (string) list_Drawtype.Items[i].Tag );
					}
				}	

				s.DrawStyle = setting;
				m_DrawWrapper.EndSelectionEdit();
			}
		}

		private void combo_DrawStyle_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if ( !m_SetupOccuring )
			{
				if( m_DrawWrapper != null )
				{
					m_DrawWrapper.BeginSelectionEdit();
					Selection s = m_DrawWrapper.CurrentSelection;
					s.DisplayMode = (AtomDisplayMode) combo_DrawStyle.SelectedIndex;
					m_DrawWrapper.EndSelectionEdit();
				}
			}
		}

		private void m_MolRange_RangeUpdated(object sender, UoB.Core.Primitives.IntRange_EventFire range)
		{
			if ( !m_SetupOccuring )
			{
				// as the m_MolRange molecule is null if it is the global selection, we can
				// definatly make the cast to a AminoAcidSelection... but check anyway
				if( m_DrawWrapper.CurrentSelection is AminoAcidSelection )
				{
					m_DrawWrapper.BeginSelectionEdit();
					AminoAcidSelection s = (AminoAcidSelection) m_DrawWrapper.CurrentSelection;
					s.setBounds( range.RangeStart, range.RangeLength );
					m_DrawWrapper.EndSelectionEdit();
				}				
			}		
		}

		private void menu_Colour_Apply1_Click(object sender, System.EventArgs e)
		{
			m_DrawWrapper.BeginSelectionEdit();
			Selection s = m_DrawWrapper.CurrentSelection;
			s.Colour1 = Colour.FromColor( panel_Colour1.BackColor );
			s.ColourMode = SelectionColourMode.Single;
			m_DrawWrapper.EndSelectionEdit();
		}

		private void menu_Colour_ApplyGradient_Click(object sender, System.EventArgs e)
		{
			m_DrawWrapper.BeginSelectionEdit();
			Selection s = m_DrawWrapper.CurrentSelection;
			s.Colour1 = Colour.FromColor( panel_Colour1.BackColor );
			s.Colour2 = Colour.FromColor( panel_Colour2.BackColor );
			s.ColourMode = SelectionColourMode.Gradient;
			m_DrawWrapper.EndSelectionEdit();
		}

		private void menu_Colour_Apply2_Click(object sender, System.EventArgs e)
		{
			m_DrawWrapper.BeginSelectionEdit();
			Selection s = m_DrawWrapper.CurrentSelection;
			s.ColourMode = SelectionColourMode.ForceFieldDefault;
			m_DrawWrapper.EndSelectionEdit();
		}

		private void menu_Selections_Remove_Click(object sender, System.EventArgs e)
		{
			m_DrawWrapper.BeginSelectionEdit();
			m_DrawWrapper.RemoveCurrentSelection();
			m_DrawWrapper.EndSelectionEdit();
		}

		private void menu_RemoveAllSels_Click(object sender, System.EventArgs e)
		{
			m_DrawWrapper.BeginSelectionEdit();
			int initialSelCount = m_DrawWrapper.SelectionCount;
			for( int i = 1; i < initialSelCount; i++ ) // 1 = dont remove global, you cant anyway, but dont try
			{
				m_DrawWrapper.CurrentSelectionIndex = m_DrawWrapper.SelectionCount - 1; // doesnt really matter, from the end
				m_DrawWrapper.RemoveCurrentSelection();
			}
			m_DrawWrapper.EndSelectionEdit();
		}

		private void check_EasyViewMode_CheckedChanged(object sender, System.EventArgs e)
		{
			m_DrawWrapper.EasyViewMode = check_EasyViewMode.Checked;
			// no need for end selection edit, all internal
		}

		private void button_ResetName_Click(object sender, System.EventArgs e)
		{
			m_DrawWrapper.BeginSelectionEdit();
			Selection s = m_DrawWrapper.CurrentSelection;
			s.Name = text_SelName.Text;
			m_DrawWrapper.EndSelectionEdit();
		}

		private void menu_Selections_Common_NoSolvent_Click(object sender, System.EventArgs e)
		{
			
			ParticleSystem ps = m_DrawWrapper.particleSystem;

			PSMolContainer[] mols = ps.Members;
			bool solventNullified = false;
			for( int i = 0; i < mols.Length; i++ )
			{
				if( mols[i] is Solvent )
				{
					m_DrawWrapper.AddSelection( mols[i], 0, mols[i].Count );
					Selection solvent = m_DrawWrapper.getSelection( m_DrawWrapper.SelectionCount -1 );
					solvent.DisplayMode = AtomDisplayMode.Invisible;
					solvent.DrawStyle = AtomDrawStyle.None;
					solventNullified = true;
				}
			}

			if( solventNullified )
			{
				m_DrawWrapper.BeginSelectionEdit();
				// selections were added above, thats fine
				m_DrawWrapper.EndSelectionEdit();
			}
			else
			{
				MessageBox.Show(this,"No solvent was found in the current system","Custom View Error",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
			}
		}

		private void menu_Selections_Common_GradientChains_Click(object sender, System.EventArgs e)
		{
			m_DrawWrapper.BeginSelectionEdit();
			if( m_DrawWrapper.SelectionCount > 1 )
			{
				if( DialogResult.Yes == MessageBox.Show(this,"Selections are present, these will be deleted. Is this OK ?","Selection query",MessageBoxButtons.YesNo,MessageBoxIcon.Question ) )
				{
					// first remove all others
					int initialSelCount = m_DrawWrapper.SelectionCount;
					for( int i = 1; i < initialSelCount; i++ ) // 1 = dont remove global, you cant anyway, but dont try
					{
						m_DrawWrapper.CurrentSelectionIndex = m_DrawWrapper.SelectionCount - 1; // doesnt really matter, from the end
						m_DrawWrapper.RemoveCurrentSelection();
					}
				}
				else
				{
					return;
				}
			}

			PSMolContainer[] mols = m_DrawWrapper.particleSystem.Members;

			// now do the chains
			for( int i = 0; i < mols.Length; i++ )
			{
				if( mols[i] is PolyPeptide )
				{
					m_DrawWrapper.AddSelection( mols[i], 0, mols[i].Count );
					Selection chain = m_DrawWrapper.getSelection( m_DrawWrapper.SelectionCount -1 );
					chain.ColourMode = SelectionColourMode.Gradient;
					chain.Colour1.SetToIntID( m_ChainColourCounter++ );
					chain.Colour2.SetToIntID( m_ChainColourCounter++ );
					if( m_ChainColourCounter >= 99 )
					{
						m_ChainColourCounter = 0;
					}
				}
			}

			m_ChainColourCounter = 0;

			// end edit
			m_DrawWrapper.EndSelectionEdit();
		}

		private void menu_InvertSelection_Click(object sender, System.EventArgs e)
		{
			m_DrawWrapper.BeginSelectionEdit();
			m_DrawWrapper.CurrentSelection.Inverted = !m_DrawWrapper.CurrentSelection.Inverted;
			m_DrawWrapper.EndSelectionEdit();	
		}

		private void menu_CopyAndInvertCurrentSelection_Click(object sender, System.EventArgs e)
		{
			m_DrawWrapper.BeginSelectionEdit();
			m_DrawWrapper.CopyCurrentSelection();
			m_DrawWrapper.CurrentSelection.Inverted = !m_DrawWrapper.CurrentSelection.Inverted;
			m_DrawWrapper.EndSelectionEdit();
		}

		private void menu_GradandnotSolv_Click(object sender, System.EventArgs e)
		{
			menu_Selections_Common_GradientChains_Click(null,null);
			menu_Selections_Common_NoSolvent_Click(null,null);
		}

		private void menu_ToggleEnabled_Click(object sender, System.EventArgs e)
		{
			m_DrawWrapper.BeginSelectionEdit();
			m_DrawWrapper.CurrentSelectionEnabledState = !m_DrawWrapper.CurrentSelectionEnabledState;
			m_DrawWrapper.EndSelectionEdit();			
		}

		private void menu_Selections_BackboneToAll_Click(object sender, System.EventArgs e)
		{
			m_DrawWrapper.BeginSelectionEdit();

			for( int i = 0; i < m_DrawWrapper.SelectionCount; i++ )
			{
				Selection sel = m_DrawWrapper.getSelection( i );
				sel.DisplayMode = AtomDisplayMode.Backbone;
			}
		
			m_DrawWrapper.EndSelectionEdit();	
		}

		private void menu_Selections_SaveDefFile_Click(object sender, System.EventArgs e)
		{
			if( DialogResult.OK == saveFileDialog.ShowDialog(this) )
			{
				SelectionDefFile.SaveNew( saveFileDialog.FileName, m_DrawWrapper );
			}
		}

		private void menu_Selections_LoadDefFile_Click(object sender, System.EventArgs e)
		{
			if( DialogResult.OK == openFileDialog.ShowDialog(this) )
			{
				SelectionDefFile.LoadTo( openFileDialog.FileName, m_DrawWrapper );
			}
		}
	
		#region Centering Control

		private void SetupCentering()
		{
			string[] modes = Enum.GetNames( typeof( FocusingMode ) );
			for( int i = 0; i < modes.Length; i++ )
			{
				centering_ModeSelect.Items.Add( modes[i] );
			}
			centering_ModeSelect.SelectedIndex = 0; // default
		}

		
		private void LoadDrawExtras()
		{
			m_SetupOccuring = true;
				check_ExtraLines.Enabled = m_DrawWrapper.HasExtendedVectors;
				check_ForceVector.Enabled = m_DrawWrapper.HasForceVectors;
				check_ExtraLines.Checked = m_DrawWrapper.EnableExtLines;
				check_HB.Checked = m_DrawWrapper.EnableHBonding;
				check_ForceVector.Checked = m_DrawWrapper.EnableForceVectors;
			m_SetupOccuring = false;
		}

		private void LoadPSCentering()
		{
			// this function is only called if a PS is present
			ParticleSystem ps = m_DrawWrapper.particleSystem;

			if( ps == null )
			{
				return;
			}

			m_SetupOccuring = true;
			
			centering_AutoRecenter.Checked = m_FocDef.AutoUpdateOnPosChange;
			centering_ModeSelect.SelectedIndex = (int)m_FocDef.Mode;

			switch( m_FocDef.Mode )
			{
				case FocusingMode.GeometricFocus:
					// disable the parameter boxes
					centering_ResidueID.Enabled = false;
					centering_CenteringChainID.Enabled = false;
					break;
				case FocusingMode.FocusByResidue:
					centering_ResidueID.Enabled = true;
					centering_CenteringChainID.Enabled = true;
					centering_CenteringChainID.SelectedItem = m_FocDef.FocusMember;
					centering_ResidueID.Maximum = m_FocDef.FocusMember.Count - 1;
					centering_ResidueID.Value = m_FocDef.ResidueIndex;
					break;
				default:
					throw new Exception("Code not implemented ...");
			}

			m_SetupOccuring = false;
		}

		private void centering_CenterPS_Click(object sender, System.EventArgs e)
		{
            m_FocDef.CallRecenter();
		}

		private void centering_AutoRecenter_CheckedChanged(object sender, System.EventArgs e)
		{
			if( !m_SetupOccuring )
			{
				m_FocDef.AutoUpdateOnPosChange = centering_AutoRecenter.Checked;		
			}
		}

		private void centering_ModeSelect_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if( !m_SetupOccuring )
			{
				m_FocDef.Mode = (FocusingMode) centering_ModeSelect.SelectedIndex;
			}
		}

		private void centering_CenteringChainID_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if( !m_SetupOccuring )
			{
				m_FocDef.FocusMember = (PSMolContainer)centering_CenteringChainID.SelectedItem;
			}
		}

		private void centering_ResidueID_ValueChanged(object sender, System.EventArgs e)
		{
			if( !m_SetupOccuring )
			{
				m_FocDef.ResidueIndex = (int)centering_ResidueID.Value;	
			}
		}

		#endregion

		private void check_HB_CheckedChanged(object sender, System.EventArgs e)
		{
			if( !m_SetupOccuring )
			{
				m_DrawWrapper.EnableHBonding = check_HB.Checked;
			}
		}

		private void check_ForceVector_CheckedChanged(object sender, System.EventArgs e)
		{
			if( !m_SetupOccuring )
			{
				m_DrawWrapper.EnableForceVectors = check_ForceVector.Checked;		
			}
		}

		private void check_ExtraLines_CheckedChanged(object sender, System.EventArgs e)
		{
			if( !m_SetupOccuring )
			{
				m_DrawWrapper.EnableExtLines = check_ExtraLines.Checked;		
			}
		}

        private void menu_Selections_SaveDefFileDAVEDef_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Are you sure you want to (re)set a DAVE-wide visualisation defaults?", "Set DAVE DSD Default?",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
            {
                SelectionDefFile.SaveNew(UoB.Core.CoreIni.Instance.DefaultSharedPath + "default.dsd", m_DrawWrapper);
            }
        }

        private void menu_Selections_SaveDefFileFileDef_Click(object sender, EventArgs e)
        {
            string saveName = m_FileInfo.FullName + ".dsd";
            if( File.Exists(saveName) )
            {
                if (DialogResult.No == MessageBox.Show("A default file already exists. Overwrite?", "Set File DSD Default?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                {
                    return;
                }
            }
            SelectionDefFile.SaveNew(saveName, m_DrawWrapper);
        }
	}
}
