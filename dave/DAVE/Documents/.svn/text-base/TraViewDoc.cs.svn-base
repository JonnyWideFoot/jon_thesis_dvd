using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using UoB.Core.Structure;
using UoB.CoreControls.OpenGLView;
using UoB.CoreControls.OpenGLView.RenderManagers;
using UoB.CoreControls.PS_Render;
using UoB.CoreControls.Documents;
using UoB.CoreControls.ToolWindows;
using UoB.CoreControls.TraInterface;
using UoB.Core;
using UoB.Core.FileIO.Tra;

namespace UoB.DAVE.Documents
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public sealed class TraViewDoc : Document
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Panel panel_Viewer;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Button button_ReloadTra;

		private UoB.CoreControls.ToolWindows.Tra_Scroll_Extended tra_Scroll_Extended;
		private UpdateEvent m_Event;
		private GLView m_Viewer;
		private ParticleSystemDrawWrapper m_DrawWrapper;

		private Tra m_TraFile;
		
		public TraViewDoc()
		{
			InitializeComponent();
			CommonInit( null );
		}

		public TraViewDoc( Tra theTra )
		{	
			InitializeComponent();
			CommonInit( theTra );
		}

		private void CommonInit( Tra theTra )
		{
			// window properties
			m_CanSave = true; // flag for outside callers to say that this class can save
			Size = new Size(600,480);

			// activate the viewer
			m_Viewer = new GLView();
			m_Viewer.Parent = this.panel_Viewer;
			m_Viewer.Dock = DockStyle.Fill;
			m_DrawWrapper = new ParticleSystemDrawWrapper( null, m_Viewer );
			AddMember( m_DrawWrapper ); // neAddMember(m_PDB.fileInfo);eded by monitoring tool windows that check which members are present for interaction ...
            AddMember(theTra.fileInfo);

			m_Event = new UpdateEvent(ps_PositionsUpdate);

			Trajectory = theTra; // public accessor initialisation - works even if null
		}

		private void ps_PositionsUpdate()
		{
			m_Viewer.Refresh();
		}

		public override void Save()
		{
			TraSaveDialog save = new TraSaveDialog( m_TraFile );
			save.ShowDialog(this);			
		}

		public Tra Trajectory
		{
			get
			{
				return m_TraFile;
			}
			set
			{
				if( m_TraFile != null )
				{
					RemoveMember( m_TraFile );
					RemoveMember( m_TraFile.DataStore );
					RemoveMember( m_TraFile.particleSystem );
					RemoveMember( m_TraFile.SequenceInfo );
					m_TraFile.particleSystem.PositionsUpdate -= m_Event;
				}

				m_TraFile = value;
				
				if( m_TraFile != null )
				{
					Text = m_TraFile.InternalName;

					m_TraFile.particleSystem.PositionsUpdate += m_Event;
					AddMember( m_TraFile );
					AddMember( m_TraFile.DataStore );
					AddMember( m_TraFile.particleSystem );
					AddMember( m_TraFile.PositionDefinitions );
					AddMember( m_TraFile.SequenceInfo );

					m_DrawWrapper.particleSystem = m_TraFile.particleSystem;
					tra_Scroll_Extended.AttachTo( m_TraFile );

					// initialise the draw wrapper to display this extra information
					m_DrawWrapper.SetupForceVectors( m_TraFile.ForceVectors );
					m_DrawWrapper.SetupExtendedVectors( m_TraFile.ExtendedVectors );
				}
				else
				{
					m_DrawWrapper.particleSystem = null;
					tra_Scroll_Extended.AttachTo( null );

					m_DrawWrapper.SetupForceVectors(null);
					m_DrawWrapper.SetupExtendedVectors(null);
				}
			}
		}

        public ParticleSystemDrawWrapper DrawWrapper
        {
            get
            {
                return m_DrawWrapper;
            }
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			Trajectory = null;

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TraViewDoc));
			this.tra_Scroll_Extended = new UoB.CoreControls.ToolWindows.Tra_Scroll_Extended();
			this.panel_Viewer = new System.Windows.Forms.Panel();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.button_ReloadTra = new System.Windows.Forms.Button();
			this.panel_Viewer.SuspendLayout();
			this.SuspendLayout();
			// 
			// tra_Scroll_Extended
			// 
			this.tra_Scroll_Extended.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tra_Scroll_Extended.Location = new System.Drawing.Point(0, 253);
			this.tra_Scroll_Extended.Name = "tra_Scroll_Extended";
			this.tra_Scroll_Extended.Size = new System.Drawing.Size(672, 64);
			this.tra_Scroll_Extended.TabIndex = 4;
			// 
			// panel_Viewer
			// 
			this.panel_Viewer.Controls.Add(this.button_ReloadTra);
			this.panel_Viewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Viewer.Location = new System.Drawing.Point(0, 0);
			this.panel_Viewer.Name = "panel_Viewer";
			this.panel_Viewer.Size = new System.Drawing.Size(672, 253);
			this.panel_Viewer.TabIndex = 2;
			// 
			// button_ReloadTra
			// 
			this.button_ReloadTra.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button_ReloadTra.Image = ((System.Drawing.Image)(resources.GetObject("button_ReloadTra.Image")));
			this.button_ReloadTra.Location = new System.Drawing.Point(0, 0);
			this.button_ReloadTra.Name = "button_ReloadTra";
			this.button_ReloadTra.Size = new System.Drawing.Size(24, 24);
			this.button_ReloadTra.TabIndex = 6;
			this.toolTip.SetToolTip(this.button_ReloadTra, "Click to reload the current tra files positional data");
			this.button_ReloadTra.Click += new System.EventHandler(this.button_ReloadTra_Click);
			// 
			// TraViewDoc
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(672, 317);
			this.Controls.Add(this.panel_Viewer);
			this.Controls.Add(this.tra_Scroll_Extended);
			this.Name = "TraViewDoc";
			this.Text = "Form1";
			this.panel_Viewer.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void button_ReloadTra_Click(object sender, System.EventArgs e)
		{
			if( DialogResult.Yes == MessageBox.Show(this,"Reload the current tra file?","Tra Reload",MessageBoxButtons.YesNo,MessageBoxIcon.Question) )
			{
				m_TraFile.SetForReInitialise();
				TrajReadin tr = new TrajReadin( m_TraFile, (Form)this.Parent.Parent );
				tra_Scroll_Extended.SetupSkippingReport( m_TraFile );
			}
		}
	}
}
