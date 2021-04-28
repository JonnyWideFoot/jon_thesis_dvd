using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Text;

using UoB.Core;
using UoB.Core.Primitives;
using UoB.Core.Structure.Alignment;
using UoB.Core.Structure;
using UoB.CoreControls.PS_Render;

namespace UoB.CoreControls.ToolWindows
{
	/// <summary>
	/// Summary description for PDB_InfoView.
	/// </summary>
	public class Model_EquivView : System.Windows.Forms.UserControl, ITool
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.TextBox text_InfoPane;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox check_Colouration;
		private System.Windows.Forms.Button button_ToggleBB;

		private PS_PositionStore m_MolStore;
		private ModelList m_Models;
		private AlignTextViewer m_ViewGenerator;
		private ParticleSystemDrawWrapper m_Draw;
		private UpdateEvent m_PositionStoreIndexChange;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Button button_Lock;
		private int m_Position = 0;
		private bool m_LockHeld = false;
		//private ParticleSystem m_PerviouslyAssociatedPS = null ;

		public Model_EquivView()
		{
			m_ViewGenerator = new AlignTextViewer( null );
			init();
			getDetails();
		}

		public Model_EquivView( ModelList models, ParticleSystemDrawWrapper draw, PS_PositionStore molStore )
		{
			init();
			AttachToDrawWrap( draw );
			AttachTo( models, molStore );
		}

		private void init()
		{
			InitializeComponent();
			Text = "Alignment Viewer";
			m_PositionStoreIndexChange = new UpdateEvent( indexUpdateEvent );
		}

		private void indexUpdateEvent()
		{
			// fired when the PosStore index is changed - i.e. we are looking at a new model
			if( m_MolStore == null || m_Models == null )
			{
				label1.Text = "No models present";
				m_Position = -1;
				Enabled = false;
			}
			else
			{
				Enabled = true; // whole control status

				m_Position = m_MolStore.Position;
				int PositionArrayIndex = m_Position - 1;
				if( m_Position == 0 )
				{
					label1.Text = "Pre-Alignment Positions";
					if( m_LockHeld && m_Draw != null && check_Colouration.Checked )
					{
						m_Draw.BeginSelectionEdit();
						m_Draw.Selections[1].IsActive = false; // there are no equivs for the initial positions
						m_Draw.EndSelectionEdit();
					}
				}
				else
				{
					label1.Text = "Model : " + PositionArrayIndex.ToString();
					// set colours
					if( m_LockHeld && m_Draw != null && check_Colouration.Checked )
					{
						Model m = m_Models[ PositionArrayIndex ];
						m_Draw.BeginSelectionEdit();
						Selection_CAlphaEquiv s = (Selection_CAlphaEquiv)m_Draw.Selections[1];
						s.IsActive = true;
						s.Reset( m_Models.Mol1, m_Models.Mol2, m.Equivalencies ); 
						m_Draw.EndSelectionEdit();
					}
				}
			}

		}

		private void getDetails()
		{
			text_InfoPane.Text = m_ViewGenerator.ReportString;
			indexUpdateEvent();
		}

		public ModelList Models
		{
			get
			{
				return m_Models;
			}
			set
			{
				m_Models = value;
			}
		}

		public ParticleSystemDrawWrapper DrawWrapper
		{
			get
			{
				return m_Draw;
			}
			set
			{
				bool lockWasHeld = m_LockHeld;
				if( lockWasHeld )
				{
					LockOperation(false);
				}
				m_Draw = value;
				if( lockWasHeld )
				{
					LockOperation(true);
				}
			}
		}

		public PS_PositionStore MolStore
		{
			get
			{
				return m_MolStore;
			}
			set
			{
				if( m_MolStore != null )
				{
					m_MolStore.IndexChanged -= m_PositionStoreIndexChange;
				}
				m_MolStore = value;
				if( m_MolStore != null )
				{
					m_MolStore.IndexChanged += m_PositionStoreIndexChange;
				}
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


		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.text_InfoPane = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.button_ToggleBB = new System.Windows.Forms.Button();
			this.check_Colouration = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.button_Lock = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// text_InfoPane
			// 
			this.text_InfoPane.BackColor = System.Drawing.Color.White;
			this.text_InfoPane.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.text_InfoPane.Dock = System.Windows.Forms.DockStyle.Fill;
			this.text_InfoPane.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.text_InfoPane.Location = new System.Drawing.Point(0, 0);
			this.text_InfoPane.Multiline = true;
			this.text_InfoPane.Name = "text_InfoPane";
			this.text_InfoPane.ReadOnly = true;
			this.text_InfoPane.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.text_InfoPane.Size = new System.Drawing.Size(744, 384);
			this.text_InfoPane.TabIndex = 0;
			this.text_InfoPane.Text = "";
			this.text_InfoPane.WordWrap = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.button_Lock);
			this.panel1.Controls.Add(this.button_ToggleBB);
			this.panel1.Controls.Add(this.check_Colouration);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(744, 56);
			this.panel1.TabIndex = 1;
			// 
			// button_ToggleBB
			// 
			this.button_ToggleBB.Enabled = false;
			this.button_ToggleBB.Location = new System.Drawing.Point(80, 24);
			this.button_ToggleBB.Name = "button_ToggleBB";
			this.button_ToggleBB.Size = new System.Drawing.Size(104, 23);
			this.button_ToggleBB.TabIndex = 2;
			this.button_ToggleBB.Text = "Toggle Backbone";
			this.button_ToggleBB.Click += new System.EventHandler(this.button_ToggleBB_Click);
			// 
			// check_Colouration
			// 
			this.check_Colouration.Enabled = false;
			this.check_Colouration.Location = new System.Drawing.Point(192, 24);
			this.check_Colouration.Name = "check_Colouration";
			this.check_Colouration.Size = new System.Drawing.Size(176, 24);
			this.check_Colouration.TabIndex = 1;
			this.check_Colouration.Text = "Perform colouration on update";
			this.check_Colouration.CheckedChanged += new System.EventHandler(this.check_Colouration_CheckedChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(144, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "label1";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.text_InfoPane);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 56);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(744, 384);
			this.panel2.TabIndex = 2;
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(0, 56);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(744, 3);
			this.splitter1.TabIndex = 3;
			this.splitter1.TabStop = false;
			this.splitter1.Visible = false;
			// 
			// button_Lock
			// 
			this.button_Lock.Location = new System.Drawing.Point(8, 24);
			this.button_Lock.Name = "button_Lock";
			this.button_Lock.Size = new System.Drawing.Size(64, 23);
			this.button_Lock.TabIndex = 3;
			this.button_Lock.Text = "Lock Off";
			this.toolTip.SetToolTip(this.button_Lock, "Establish DrawWrapper Lock For ColourSelection Control");
			this.button_Lock.Click += new System.EventHandler(this.button_Lock_Click);
			// 
			// Model_EquivView
			// 
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "Model_EquivView";
			this.Size = new System.Drawing.Size(744, 440);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region ITool Members

		public void AttachTo( ModelList models )
		{
			AttachTo( models, null );
		}

		public void AttachToDrawWrap( ParticleSystemDrawWrapper draw )
		{
			if( check_Colouration.Checked )
			{
				check_Colouration.Checked = false;
				LockOperation( false );
			}
			DrawWrapper = draw;
		}

		public void AttachTo( ModelList models, PS_PositionStore molStore )
		{
			MolStore = molStore;
			Models = models;
			m_ViewGenerator.Models = m_Models; // even if its null
			if( m_LockHeld )
			{
				SetupSelectionsForNewPS(); // ensure that our locked drawwrapper contains a valid selection at position 1
			}
			getDetails();
		}

		public void AttachToDocument(UoB.CoreControls.Documents.Document doc)
		{
			if( check_Colouration.Checked )
			{
				check_Colouration.Checked = false;
				LockOperation( false );
			}

			m_Models = null;
			m_Draw = null;
			m_MolStore = null;

			if ( doc != null )
			{
				for( int i = 0; i < doc.MemberCount; i++ )
				{
					if ( doc[i].GetType() == typeof(ModelList) )
					{
						Models = (ModelList) doc[i];
					}
					else if( doc[i].GetType() == typeof(ParticleSystemDrawWrapper) )
					{
						DrawWrapper = (ParticleSystemDrawWrapper) doc[i];
					}
					else if( doc[i].GetType() == typeof(PS_PositionStore) )
					{
						MolStore = (PS_PositionStore) doc[i];			
					}
				}
				if( Models == null || DrawWrapper == null || MolStore == null )
				{
					// cant do selection edit, the required components are not defined
					check_Colouration.Checked = false;
				}
			}

			m_ViewGenerator.Models = m_Models; // even if its null
			getDetails(); // generate a report in the ViewGenerator and display in this control
		}

		#endregion

		private void check_Colouration_CheckedChanged(object sender, System.EventArgs e)
		{
			ChainColouration( check_Colouration.Checked );
		}

		private void button_ToggleBB_Click(object sender, System.EventArgs e)
		{
			BackBoneToggle();
		}
		
		private void button_Lock_Click(object sender, System.EventArgs e)
		{
			LockOperation( !m_LockHeld );            		
		}

		#region PSDRAW control

		private void SetupSelectionsForNewPS()
		{
			if( m_LockHeld )
			{
				m_Draw.BeginSelectionEdit();
				m_Draw.ClearSlections();
				if( m_Position > 0 )
				{
					m_Draw.AddSelection( m_Models, m_Position - 1 );
				}
				else
				{
					m_Draw.AddSelection( m_Models, 0 ); // wrong model, but we need to init to something
				}
				m_Draw.EndSelectionEdit();	
				if( check_Colouration.Checked )
				{
					ChainColouration( true );
				}
			}
		}

		private void LockOperation( bool turnOn )
		{
			if( turnOn )
			{
				try
				{
					m_Draw.SetSelectionLock( this, true );
				}
				catch
				{
					MessageBox.Show("Lock Operation Could not be perfomed");
					return;
				}
				SetInternalLockStatus( true );
                SetupSelectionsForNewPS();				
			}
			else
			{
				SetInternalLockStatus( false );
				if( m_LockHeld ) // should always be the case if we have got here
				{
					m_Draw.BeginSelectionEdit();
					m_Draw.ClearSlections();
					m_Draw.SetSelectionLock( this, false );
					m_Draw.EndSelectionEdit();					
				}
			}
		}

		private void SetInternalLockStatus( bool on )
		{
			if( on )
			{
				button_Lock.Text = "Lock On";
			}
			else
			{
				button_Lock.Text = "Lock Off";
			}
			m_LockHeld = on;
			button_ToggleBB.Enabled = on;
			check_Colouration.Enabled = on;
		}

		private void BackBoneToggle()
		{
			if( m_Draw != null )
			{
				m_Draw.BeginSelectionEdit();
				Selection s1 = m_Draw.Selections[0];
				Selection s2 = m_Draw.Selections[1];
				if( s1.DisplayMode != AtomDisplayMode.Backbone )
				{
					s1.DisplayMode = AtomDisplayMode.Backbone;
					s2.DisplayMode = AtomDisplayMode.Backbone;
				}
				else
				{
					s1.DisplayMode = AtomDisplayMode.HeavyAtom;
					s2.DisplayMode = AtomDisplayMode.HeavyAtom;
				}
				m_Draw.EndSelectionEdit();
			}
		}

		private void ChainColouration( bool on )
		{
			if( m_Draw != null )
			{
				if( on )
				{
					m_Draw.BeginSelectionEdit();
					Selection s = m_Draw.Selections[1]; // CAlphaEquiv Sel
					s.Colour1 = Colour.FromIntID( 0 ); // red
					s.Colour2 = Colour.FromIntID( 2 ); // yellow
					s.ColourMode = SelectionColourMode.Gradient;
					m_Draw.EndSelectionEdit();
				}
				else
				{
					m_Draw.BeginSelectionEdit();
					Selection s = m_Draw.Selections[1]; // CAlphaEquiv Sel
					s.ColourMode = SelectionColourMode.ForceFieldDefault;
					m_Draw.EndSelectionEdit();
				}
			}
			else
			{
				check_Colouration.Checked = false;
			}
		}
		#endregion
	}
}
