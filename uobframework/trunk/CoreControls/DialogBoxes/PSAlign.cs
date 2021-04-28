using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

using UoB.Core.Structure.Alignment;
using UoB.Core.Structure;
using UoB.Core.FileIO.PDB;
using UoB.Core.FileIO.Tra;
using UoB.Core.Tools;
using UoB.CoreControls.Documents;
using UoB.CoreControls.ToolWindows;
using UoB.CoreControls.PS_Render;
using UoB.CoreControls.OpenGLView;
using UoB.Core.FileIO;

namespace UoB.CoreControls.DialogBoxes
{
	/// <summary>
	/// Summary description for PSAlign.
	/// </summary>
	public class PSAlign : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button button_LoadPS1;
		private System.Windows.Forms.Button button_LoadPS2;
		private System.Windows.Forms.ListBox list_PS1Chains;
		private System.Windows.Forms.ListBox list_PS2Chains;
		private System.Windows.Forms.Button button_Align;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Label label_Method;
		private System.Windows.Forms.ListBox listBox_Documents;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_LoadNew;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.ComboBox combo_Method;
		private System.Windows.Forms.Label label_PS1Name;
		private System.Windows.Forms.Label label_PS2Name;

        private string m_FileName1 = "";
		private string m_FileName2 = "";
		private ParticleSystem m_PS1;
		private ParticleSystem m_PS2;
		private Tra_Scroll m_Scroll;
		private ArrayList m_Wraps = new ArrayList();
		private Wrap m_CurrentWrap = null;
		private ParticleSystemDrawWrapper m_View;
		private PSAlignManager m_Aligner;

		public PSAlign( ArrayList AllDocuments )
		{
			commnonInit();

			for( int i = 0; i < AllDocuments.Count; i++ )
			{
				Document d = AllDocuments[i] as Document;
				if( d != null  )
				{
					ScanMembers( (Document)AllDocuments[i] );              
				}
			}	
			for( int i = 0; i < m_Wraps.Count; i++ )
			{
				listBox_Documents.Items.Add( m_Wraps[i] );
			}
			listBox_Documents.DisplayMember = "Name";
			listBox_Documents.ValueMember = "Name";

		}

		public PSAlign()
		{
			commnonInit();
		}

		private void commnonInit()
		{
			InitializeComponent();

			// populate the methods box ...
			string[] methods = Enum.GetNames( typeof(AlignmentMethod) );
			for( int i = 0; i < methods.Length; i++ )
			{
				combo_Method.Items.Add( methods[i] );
			}
			combo_Method.SelectedIndex = 0;

			m_Scroll = new Tra_Scroll();
			m_Scroll.Parent = panel1;
			m_Scroll.Dock = DockStyle.Fill;

			GLView view = new GLView();
			m_View = new ParticleSystemDrawWrapper( null, view );
			view.Parent = panel2;
			view.Dock = DockStyle.Fill;
		}

		private class Wrap
		{
			private object m_Object;
			private string m_Name;
			private bool m_MultiModel;
			private string m_Filename;
			public Wrap( string name, object o, bool multi, string filename )
			{
				m_Filename = filename;
				m_Object = o;
				m_Name = name;
				m_MultiModel = multi;
			}
			public string Name
			{
				get
				{
					return m_Name;
				}
			}
			public string FileName
			{
				get
				{
					return m_Filename;
				}
			}
			public object O
			{
				get
				{
					return m_Object;
				}
			}
			public bool MultiModel
			{
				get
				{
					return m_MultiModel;
				}
			}
		}

		private void ScanMembers( Document d )
		{
			BaseFileType_Structural file = null;
			PS_PositionStore pos = null;

			// scan for a filenamed structural file type
			for( int i = 0; i < d.MemberCount; i++ )
			{
				BaseFileType_Structural docMemStruct = d[i] as BaseFileType_Structural;
				if( docMemStruct != null )
				{
					file = docMemStruct;
					break;
				}
			}
			for( int i = 0; i < d.MemberCount; i++ )
			{
				PS_PositionStore docMemPos = d[i] as PS_PositionStore;
				if( docMemPos != null )
				{
					pos = (PS_PositionStore) d[i];
					break;
				}
			}

			if( file != null )
			{
				if( pos != null )
				{
					m_Wraps.Add( new Wrap( file.InternalName, pos, true, file.FullFileName ) );
					return;
				}
				else
				{
					m_Wraps.Add( new Wrap( file.InternalName, file.particleSystem, false, file.FullFileName ) );
					return;
				}
			}
			else
			{
				// the non-filenamed members
				if( pos != null )
				{
					m_Wraps.Add( new Wrap( pos.particleSystem.Name, pos, true, "UNKNOWN" ) );
					return;
				}
				else
				{
					for( int i = 0; i < d.MemberCount; i++ )
					{
						ParticleSystem ps = d[i] as ParticleSystem;
						if( ps != null )
						{
							m_Wraps.Add( new Wrap( ps.Name, ps, false, "UNKNOWN" ) );
							return;
						}
					}	
					// nothing relevent found ... at all!
					return;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.button_LoadPS1 = new System.Windows.Forms.Button();
			this.button_LoadPS2 = new System.Windows.Forms.Button();
			this.list_PS1Chains = new System.Windows.Forms.ListBox();
			this.list_PS2Chains = new System.Windows.Forms.ListBox();
			this.button_Align = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.label_Method = new System.Windows.Forms.Label();
			this.listBox_Documents = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.button_LoadNew = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label3 = new System.Windows.Forms.Label();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.combo_Method = new System.Windows.Forms.ComboBox();
			this.label_PS1Name = new System.Windows.Forms.Label();
			this.label_PS2Name = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button_LoadPS1
			// 
			this.button_LoadPS1.Location = new System.Drawing.Point(192, 56);
			this.button_LoadPS1.Name = "button_LoadPS1";
			this.button_LoadPS1.Size = new System.Drawing.Size(88, 23);
			this.button_LoadPS1.TabIndex = 0;
			this.button_LoadPS1.Text = "Set As PS1";
			this.button_LoadPS1.Click += new System.EventHandler(this.button_LoadPS1_Click);
			// 
			// button_LoadPS2
			// 
			this.button_LoadPS2.Location = new System.Drawing.Point(192, 88);
			this.button_LoadPS2.Name = "button_LoadPS2";
			this.button_LoadPS2.Size = new System.Drawing.Size(88, 23);
			this.button_LoadPS2.TabIndex = 1;
			this.button_LoadPS2.Text = "Set As PS2";
			this.button_LoadPS2.Click += new System.EventHandler(this.button_LoadPS2_Click);
			// 
			// list_PS1Chains
			// 
			this.list_PS1Chains.Location = new System.Drawing.Point(8, 176);
			this.list_PS1Chains.Name = "list_PS1Chains";
			this.list_PS1Chains.Size = new System.Drawing.Size(128, 82);
			this.list_PS1Chains.TabIndex = 2;
			// 
			// list_PS2Chains
			// 
			this.list_PS2Chains.Location = new System.Drawing.Point(144, 176);
			this.list_PS2Chains.Name = "list_PS2Chains";
			this.list_PS2Chains.Size = new System.Drawing.Size(128, 82);
			this.list_PS2Chains.TabIndex = 3;
			// 
			// button_Align
			// 
			this.button_Align.Location = new System.Drawing.Point(152, 296);
			this.button_Align.Name = "button_Align";
			this.button_Align.Size = new System.Drawing.Size(120, 23);
			this.button_Align.TabIndex = 4;
			this.button_Align.Text = "Align Selected";
			this.button_Align.Click += new System.EventHandler(this.button_Align_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.DefaultExt = "PDB";
			this.openFileDialog.Filter = "All Readable Files (*.PDB,*.TRA,*.Align)|*.PDB;*.TRA;*.Align|PDB Files (*.PDB)|*." +
				"PDB|TRA Files (*.TRA)|*.TRA|Align Definition Files (*.Align)|*.Align";
			this.openFileDialog.Title = "Get a PDB File";
			// 
			// label_Method
			// 
			this.label_Method.Location = new System.Drawing.Point(8, 264);
			this.label_Method.Name = "label_Method";
			this.label_Method.Size = new System.Drawing.Size(144, 24);
			this.label_Method.TabIndex = 5;
			this.label_Method.Text = "Choose Alignment Method :";
			this.label_Method.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// listBox_Documents
			// 
			this.listBox_Documents.DisplayMember = "Name";
			this.listBox_Documents.Location = new System.Drawing.Point(8, 24);
			this.listBox_Documents.Name = "listBox_Documents";
			this.listBox_Documents.Size = new System.Drawing.Size(176, 95);
			this.listBox_Documents.TabIndex = 7;
			this.listBox_Documents.ValueMember = "Name";
			this.listBox_Documents.SelectedIndexChanged += new System.EventHandler(this.listBox_Documents_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 128);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 24);
			this.label1.TabIndex = 8;
			this.label1.Text = "System 1 From :";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(144, 128);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(128, 24);
			this.label2.TabIndex = 9;
			this.label2.Text = "System 2 From :";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// button_LoadNew
			// 
			this.button_LoadNew.Location = new System.Drawing.Point(192, 24);
			this.button_LoadNew.Name = "button_LoadNew";
			this.button_LoadNew.Size = new System.Drawing.Size(88, 23);
			this.button_LoadNew.TabIndex = 10;
			this.button_LoadNew.Text = "<- Load";
			this.button_LoadNew.Click += new System.EventHandler(this.button_LoadNew_Click);
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Location = new System.Drawing.Point(288, 24);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(160, 144);
			this.panel1.TabIndex = 11;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(136, 23);
			this.label3.TabIndex = 12;
			this.label3.Text = " All Available Systems";
			// 
			// button_Cancel
			// 
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(16, 296);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(120, 23);
			this.button_Cancel.TabIndex = 13;
			this.button_Cancel.Text = "Cancel";
			// 
			// panel2
			// 
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Location = new System.Drawing.Point(288, 176);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(160, 144);
			this.panel2.TabIndex = 14;
			// 
			// combo_Method
			// 
			this.combo_Method.Location = new System.Drawing.Point(144, 264);
			this.combo_Method.Name = "combo_Method";
			this.combo_Method.Size = new System.Drawing.Size(128, 21);
			this.combo_Method.TabIndex = 15;
			// 
			// label_PS1Name
			// 
			this.label_PS1Name.Location = new System.Drawing.Point(8, 152);
			this.label_PS1Name.Name = "label_PS1Name";
			this.label_PS1Name.Size = new System.Drawing.Size(128, 24);
			this.label_PS1Name.TabIndex = 16;
			this.label_PS1Name.Text = "null";
			this.label_PS1Name.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label_PS2Name
			// 
			this.label_PS2Name.Location = new System.Drawing.Point(144, 152);
			this.label_PS2Name.Name = "label_PS2Name";
			this.label_PS2Name.Size = new System.Drawing.Size(128, 24);
			this.label_PS2Name.TabIndex = 17;
			this.label_PS2Name.Text = "null";
			this.label_PS2Name.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// PSAlign
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(458, 327);
			this.Controls.Add(this.label_PS2Name);
			this.Controls.Add(this.label_PS1Name);
			this.Controls.Add(this.combo_Method);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.button_LoadNew);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listBox_Documents);
			this.Controls.Add(this.label_Method);
			this.Controls.Add(this.button_Align);
			this.Controls.Add(this.list_PS2Chains);
			this.Controls.Add(this.list_PS1Chains);
			this.Controls.Add(this.button_LoadPS2);
			this.Controls.Add(this.button_LoadPS1);
			this.Controls.Add(this.label3);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PSAlign";
			this.ShowInTaskbar = false;
			this.Text = "PSAlign";
			this.ResumeLayout(false);

		}
		#endregion

		private void SetPS( int index )
		{
			if( listBox_Documents.Items.Count > 0 && listBox_Documents.SelectedItem != null )
			{
				Wrap w = (Wrap) listBox_Documents.SelectedItem;

				ParticleSystem ps = null;
				if( w.MultiModel )
				{
					PS_PositionStore models = (PS_PositionStore) w.O;
					ps = (ParticleSystem) models.particleSystem.Clone();
				}
				else
				{
					ParticleSystem psSource = (ParticleSystem) w.O;
					ps = (ParticleSystem) psSource.Clone();
				}

				if( index == 0 )
				{
					m_FileName1 = w.FileName;
					m_PS1 = ps;
					if( !(m_PS1 == null) )
					{
						SetupChainBox( 0 );
						label_PS1Name.Text = m_PS1.Name;
					}
					else
					{
						label_PS1Name.Text = "null";
					}
				}
				else
				{
					m_FileName2 = w.FileName;
					m_PS2 = ps;
					if( !(m_PS2 == null) )
					{
						SetupChainBox( 1 );
						label_PS2Name.Text = m_PS2.Name;
					}
					else
					{
						label_PS2Name.Text = "null";
					}
				}
			}
		}

		private void SetupChainBox( int index )
		{
			ParticleSystem ps;
			ListBox list;
			if( index == 0 )
			{
				ps = m_PS1;
				list = list_PS1Chains;
			}
			else
			{
				ps = m_PS2;
				list = list_PS2Chains;
			}

			PSMolContainer[] mols = ps.Members;
			list.Items.Clear();
			for( int i = 0; i < mols.Length; i++ )
			{
				if( mols[i] is Solvent )
				{
					continue; // cant align solvent
				}
				if( mols[i] is HetMolecules )
				{
					continue; // cant align HetMolecules
				}
				list.Items.Add( mols[i] );
			}
			if( list.Items.Count > 0 )
			{
				list.SelectedIndex = 0;
			}
		}

		private void button_LoadPS1_Click(object sender, System.EventArgs e)
		{
			SetPS(0);
		}

		private void button_LoadPS2_Click(object sender, System.EventArgs e)
		{
			SetPS(1);		
		}

		private void button_Align_Click(object sender, System.EventArgs e)
		{
			if( list_PS1Chains.SelectedItem == null ||
				list_PS2Chains.SelectedItem == null )
			{
				MessageBox.Show("The chains can't be null, please select one in each box");
				return;
			}

			AlignmentMethod method = (AlignmentMethod) Enum.Parse( typeof(AlignmentMethod), (string) combo_Method.SelectedItem, true );
			
			AlignSourceDefinition def1 = new AlignSourceDefinition( m_FileName1, new MolRange( (PSMolContainer) list_PS1Chains.SelectedItem ) );
			AlignSourceDefinition def2 = new AlignSourceDefinition( m_FileName2, new MolRange( (PSMolContainer) list_PS2Chains.SelectedItem ) );
			
			m_Aligner = new PSAlignManager(method,130);
			m_Aligner.ResetPSMolContainers( "Alignment of " + m_PS1.Name + " and " + m_PS2.Name, def1, def2 );

			m_Aligner.PerformAlignment();
			DialogResult = DialogResult.OK;
			Close();
		}

		public AlignmentSysDef SystemDefinition
		{
			get
			{
				if( m_Aligner != null )
				{
					return m_Aligner.SystemDefinition;
				}
				else
				{
					return null;
				}
			}
		}

		private void listBox_Documents_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			m_CurrentWrap = (Wrap)listBox_Documents.SelectedItem;
			if( m_CurrentWrap.MultiModel )
			{
				PS_PositionStore pos = (PS_PositionStore)m_CurrentWrap.O;
				m_Scroll.AttachTo( pos );
				m_View.particleSystem = pos.particleSystem;
			}
			else
			{
				m_Scroll.AttachTo( null );
				m_View.particleSystem = (ParticleSystem) m_CurrentWrap.O;
			}
		}

		private void button_LoadNew_Click(object sender, System.EventArgs e)
		{
			if(openFileDialog.ShowDialog() == DialogResult.OK) 
			{
				string fileName = openFileDialog.FileName;
				FileInfo fi = new FileInfo( fileName );
				Wrap w;
				switch( fi.Extension.ToUpper() )
				{
					case ".PDB":
						PDB pdb = new PDB(fileName,true);
						w = new Wrap( "Internal : " + pdb.InternalName, pdb.particleSystem, false, pdb.FullFileName );
						m_Wraps.Add( w );
						listBox_Documents.Items.Add( w );
						listBox_Documents.SelectedIndex = listBox_Documents.Items.Count - 1;
						break;
					case ".TRA":
						Tra t = new Tra( fileName );
						t.LoadTrajectory();
						w = new Wrap( "Internal, MultipleModels : " + t.InternalName, t.PositionDefinitions, true, t.FullFileName );
						m_Wraps.Add( w );
						listBox_Documents.Items.Add( w );
						listBox_Documents.SelectedIndex = listBox_Documents.Items.Count - 1;
						break;
					case ".ALIGN":
						// defines all we need to know about settings
						try
						{
							AlignFile ali = new AlignFile( fileName );
							m_Aligner = ali.AliManager;
							DialogResult = DialogResult.OK;
							Close();
						}
						catch( Exception ex )
						{
							MessageBox.Show(this,"Error during align file parseing : " + ex.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
						}
						break;
					default:
						MessageBox.Show(this,"Unsupported file Extenstion","Error",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
						break;
				}
			}		
		}		
	}
}