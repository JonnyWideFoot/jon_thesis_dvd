using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

using UoB.Research.Primitives;
using UoB.Research.Primitives.Matrix;
using UoB.Research.Modelling.Alignment;
using UoB.Research.Modelling.Structure;
using UoB.Research.FileIO.PDB;
using UoB.Generic.OpenGLView;
using UoB.Generic.PS_Render;
using UoB.Generic.ToolWindows;
using UoB.Research.Modelling.ForceField;

namespace AlignmentViewer
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Aligner : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Panel panel_View;
		private System.Windows.Forms.Panel panel2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox text_FolderScanPath;
		private System.Windows.Forms.Button button_BeginFolderScan;
		private System.Windows.Forms.Button button_Prev;
		private System.Windows.Forms.Button button_Next;
		private System.Windows.Forms.Label label_PosIndex;
		private System.Windows.Forms.Button button_Increment;
		private System.Windows.Forms.Button button_Stop;
		private System.Windows.Forms.Button button_Bin;
		private System.Windows.Forms.TextBox text_PDBPath;

		private ParticleSystemDrawWrapper m_Wrap;
		private string m_AlignFileName = null;
		private AlignFile m_AlignFile;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Button button_TogRib;
		private System.Windows.Forms.Label label_FileName;
		Form2 m_EquivForm = new Form2();

		public Aligner()
		{
			InitializeComponent();
			GLView view = new GLView();
			m_Wrap = new ParticleSystemDrawWrapper(null,view);
			view.Parent = panel_View;
			view.Dock = DockStyle.Fill;
			Show();
			SetDesktopLocation( 50,100 );
			m_EquivForm.m_ModelEquivView.AttachToDrawWrap( m_Wrap );
			AddOwnedForm( m_EquivForm );
			m_EquivForm.Show();	
			m_EquivForm.Width = 1000;
			m_EquivForm.ControlBox = false;
			m_EquivForm.SetDesktopLocation( DesktopBounds.Left, DesktopBounds.Bottom );
		}
		protected override void OnMove(EventArgs e)
		{
			base.OnMove (e);
			m_EquivForm.SetDesktopLocation( DesktopBounds.Left, DesktopBounds.Bottom );
		}
		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);
			m_EquivForm.SetDesktopLocation( DesktopBounds.Left, DesktopBounds.Bottom );
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
			this.button1 = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.panel_View = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.text_PDBPath = new System.Windows.Forms.TextBox();
			this.button_Bin = new System.Windows.Forms.Button();
			this.button_Stop = new System.Windows.Forms.Button();
			this.button_Increment = new System.Windows.Forms.Button();
			this.label_PosIndex = new System.Windows.Forms.Label();
			this.button_Next = new System.Windows.Forms.Button();
			this.button_Prev = new System.Windows.Forms.Button();
			this.button_BeginFolderScan = new System.Windows.Forms.Button();
			this.text_FolderScanPath = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.button_TogRib = new System.Windows.Forms.Button();
			this.label_FileName = new System.Windows.Forms.Label();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(8, 8);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(48, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Load";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.Filter = "Align Files|*.align";
			this.openFileDialog1.InitialDirectory = "c:\\___UberAlign\\";
			// 
			// panel_View
			// 
			this.panel_View.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_View.Location = new System.Drawing.Point(0, 0);
			this.panel_View.Name = "panel_View";
			this.panel_View.Size = new System.Drawing.Size(920, 525);
			this.panel_View.TabIndex = 1;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.label_FileName);
			this.panel2.Controls.Add(this.button_TogRib);
			this.panel2.Controls.Add(this.text_PDBPath);
			this.panel2.Controls.Add(this.button_Bin);
			this.panel2.Controls.Add(this.button_Stop);
			this.panel2.Controls.Add(this.button_Increment);
			this.panel2.Controls.Add(this.label_PosIndex);
			this.panel2.Controls.Add(this.button_Next);
			this.panel2.Controls.Add(this.button_Prev);
			this.panel2.Controls.Add(this.button_BeginFolderScan);
			this.panel2.Controls.Add(this.text_FolderScanPath);
			this.panel2.Controls.Add(this.label1);
			this.panel2.Controls.Add(this.button1);
			this.panel2.Controls.Add(this.numericUpDown1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(240, 525);
			this.panel2.TabIndex = 3;
			// 
			// text_PDBPath
			// 
			this.text_PDBPath.Location = new System.Drawing.Point(8, 32);
			this.text_PDBPath.Name = "text_PDBPath";
			this.text_PDBPath.Size = new System.Drawing.Size(216, 20);
			this.text_PDBPath.TabIndex = 10;
			this.text_PDBPath.Text = "G:\\___UberAlign\\seed1\\";
			// 
			// button_Bin
			// 
			this.button_Bin.Location = new System.Drawing.Point(104, 256);
			this.button_Bin.Name = "button_Bin";
			this.button_Bin.TabIndex = 9;
			this.button_Bin.Text = "Bin";
			this.button_Bin.Click += new System.EventHandler(this.button_Bin_Click);
			// 
			// button_Stop
			// 
			this.button_Stop.Location = new System.Drawing.Point(8, 296);
			this.button_Stop.Name = "button_Stop";
			this.button_Stop.TabIndex = 8;
			this.button_Stop.Text = "Stop";
			this.button_Stop.Click += new System.EventHandler(this.button_Stop_Click);
			// 
			// button_Increment
			// 
			this.button_Increment.Location = new System.Drawing.Point(8, 272);
			this.button_Increment.Name = "button_Increment";
			this.button_Increment.TabIndex = 7;
			this.button_Increment.Text = "Increment";
			this.button_Increment.Click += new System.EventHandler(this.button_Increment_Click);
			// 
			// label_PosIndex
			// 
			this.label_PosIndex.Location = new System.Drawing.Point(96, 112);
			this.label_PosIndex.Name = "label_PosIndex";
			this.label_PosIndex.Size = new System.Drawing.Size(104, 16);
			this.label_PosIndex.TabIndex = 6;
			this.label_PosIndex.Text = "Position : 0";
			// 
			// button_Next
			// 
			this.button_Next.Location = new System.Drawing.Point(48, 240);
			this.button_Next.Name = "button_Next";
			this.button_Next.Size = new System.Drawing.Size(32, 23);
			this.button_Next.TabIndex = 5;
			this.button_Next.Text = "->";
			this.button_Next.Click += new System.EventHandler(this.button_Next_Click);
			// 
			// button_Prev
			// 
			this.button_Prev.Location = new System.Drawing.Point(8, 240);
			this.button_Prev.Name = "button_Prev";
			this.button_Prev.Size = new System.Drawing.Size(32, 23);
			this.button_Prev.TabIndex = 4;
			this.button_Prev.Text = "<-";
			this.button_Prev.Click += new System.EventHandler(this.button_Prev_Click);
			// 
			// button_BeginFolderScan
			// 
			this.button_BeginFolderScan.Location = new System.Drawing.Point(8, 216);
			this.button_BeginFolderScan.Name = "button_BeginFolderScan";
			this.button_BeginFolderScan.Size = new System.Drawing.Size(72, 23);
			this.button_BeginFolderScan.TabIndex = 3;
			this.button_BeginFolderScan.Text = "Init";
			this.button_BeginFolderScan.Click += new System.EventHandler(this.button_BeginFolderScan_Click);
			// 
			// text_FolderScanPath
			// 
			this.text_FolderScanPath.Location = new System.Drawing.Point(8, 128);
			this.text_FolderScanPath.Name = "text_FolderScanPath";
			this.text_FolderScanPath.Size = new System.Drawing.Size(216, 20);
			this.text_FolderScanPath.TabIndex = 2;
			this.text_FolderScanPath.Text = "C:\\_Gen Ig Database 04.10.04\\newOut\\";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 112);
			this.label1.Name = "label1";
			this.label1.TabIndex = 1;
			this.label1.Text = "Folder Scan :";
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(8, 56);
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.TabIndex = 0;
			this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(240, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 525);
			this.splitter1.TabIndex = 4;
			this.splitter1.TabStop = false;
			// 
			// button_TogRib
			// 
			this.button_TogRib.Location = new System.Drawing.Point(104, 216);
			this.button_TogRib.Name = "button_TogRib";
			this.button_TogRib.Size = new System.Drawing.Size(104, 23);
			this.button_TogRib.TabIndex = 11;
			this.button_TogRib.Text = "Toggle Ribbon";
			this.button_TogRib.Click += new System.EventHandler(this.button_TogRib_Click);
			// 
			// label_FileName
			// 
			this.label_FileName.Location = new System.Drawing.Point(8, 152);
			this.label_FileName.Name = "label_FileName";
			this.label_FileName.Size = new System.Drawing.Size(224, 64);
			this.label_FileName.TabIndex = 12;
			this.label_FileName.Text = "label2";
			// 
			// Aligner
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(920, 525);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel_View);
			this.Name = "Aligner";
			this.Text = "Form1";
			this.panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			FFManager ffMan = FFManager.Instance;
			ffMan.FinaliseStage2();
			Application.Run(new Aligner());
		}

		#region Align file interaction
		private void button1_Click(object sender, System.EventArgs e)
		{
			if ( openFileDialog1.ShowDialog() == DialogResult.OK )
			{
				m_AlignFileName = openFileDialog1.FileName;
                m_AlignFile = new AlignFile( m_AlignFileName );
				numericUpDown1.Minimum = 0;
				numericUpDown1.Value = 0;
				numericUpDown1.Maximum = m_AlignFile.SysDefCount - 1;
				show( 0 );				
			}
		}

		private void show( int index )
		{
			AlignmentSysDef def = m_AlignFile[index];
			m_Wrap.particleSystem = def.particleSystem;
			def.ModelStore.Position = 1; // the first valid model
			m_EquivForm.m_ModelEquivView.AttachTo( def.Models, def.ModelStore );
			m_EquivForm.m_ModelScroll.AttachTo( def.ModelStore );
		}

		private void numericUpDown1_ValueChanged(object sender, System.EventArgs e)
		{
			show( (int)numericUpDown1.Value );
		}
		
		#endregion

		#region PDB files in folder view functions
		private FileInfo[] filenames;
		private int position = 0;
		private void button_BeginFolderScan_Click(object sender, System.EventArgs e)
		{
			m_Timer.Interval = 500;
			m_Timer.Tick += new EventHandler(m_Timer_Tick);
			position = 0;
			DirectoryInfo di = new DirectoryInfo( text_FolderScanPath.Text );
			filenames = di.GetFiles( "*.pdb" );
			ShowAtPos( 0 );
		}

		private void ShowAtPos( int index )
		{
			PDB file = new PDB( filenames[index].FullName, true );
			m_Wrap.particleSystem = file.particleSystem;
			label_PosIndex.Text = "Position : " + position.ToString();
			label_FileName.Text = file.FullFilePath;
		}

		private void button_Next_Click(object sender, System.EventArgs e)
		{
			position++;
			if( position == filenames.Length )
			{
				position = 0;
			}
			ShowAtPos( position );
		}

		private void button_Prev_Click(object sender, System.EventArgs e)
		{
			position--;
			if( position == -1 )
			{
				position = filenames.Length -1;
			}
			ShowAtPos( position );
		}

		private Timer m_Timer = new Timer();
		private void button_Increment_Click(object sender, System.EventArgs e)
		{
            m_Timer.Start();		
		}

		private void button_Stop_Click(object sender, System.EventArgs e)
		{
            m_Timer.Stop();		
		}

		private string binPath = @"C:\_Gen Ig Database 29.07.04\shitBin\";
		private void button_Bin_Click(object sender, System.EventArgs e)
		{
            FileInfo fi = filenames[position];
			fi.MoveTo(binPath+fi.Name);
		}

		private void m_Timer_Tick(object sender, EventArgs e)
		{
			button_Next_Click(null,null);
		}
		#endregion

		private bool m_WrapRibOn = false;
		private void button_TogRib_Click(object sender, System.EventArgs e)
		{
			if( m_WrapRibOn )
			{
				m_Wrap.BeginSelectionEdit();
				m_Wrap.Selections[0].DrawStyle = AtomDrawStyle.Lines;
				m_Wrap.EndSelectionEdit();
			}
			else
			{
				m_Wrap.BeginSelectionEdit();
				m_Wrap.Selections[0].DrawStyle = AtomDrawStyle.Ribbon;
				m_Wrap.EndSelectionEdit();
			}
			m_WrapRibOn = !m_WrapRibOn;
		}
	}
}
