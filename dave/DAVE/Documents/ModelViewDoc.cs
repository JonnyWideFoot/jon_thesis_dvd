using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using UoB.Core.Structure;
using UoB.CoreControls.OpenGLView;
using UoB.CoreControls.PS_Render;
using UoB.CoreControls.Documents;
using UoB.CoreControls.ToolWindows;
using UoB.CoreControls.TraInterface;
using UoB.Core;
using UoB.Core.FileIO.PDB;

namespace UoB.DAVE.Documents
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public sealed class ModelViewDoc : Document
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panel_Viewer;

		private GLView m_Viewer;
		private ParticleSystemDrawWrapper m_DrawWrapper;
		private UoB.CoreControls.ToolWindows.Tra_Scroll_Extended tra_Scroll_Extended;
		private PS_PositionStore m_Models;
		private UpdateEvent m_Event;

		public ModelViewDoc(PS_PositionStore pos)
		{	
			InitializeComponent();

			m_CanSave = true;

			m_Models = pos;
			m_Viewer = new GLView();
			m_DrawWrapper = new ParticleSystemDrawWrapper( pos.particleSystem, m_Viewer );

			m_Members.Add( pos );
			m_Members.Add( pos.particleSystem );
			m_Members.Add( m_DrawWrapper ); // needed by monitoring tool windows that check which members are present for interaction ...
			
			m_Viewer.Parent = this.panel_Viewer;
			m_Viewer.Dock = DockStyle.Fill;
			tra_Scroll_Extended.AttachTo( m_Models );

			m_Event = new UpdateEvent(ps_PositionsUpdate);
			m_Models.particleSystem.ContentUpdate += m_Event;

			Size = new Size(600,480);
			Text = m_Models.particleSystem.Name;
		}

		public override void Save()
		{
			if(saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				PDB.SaveNew( saveFileDialog.FileName, m_Models.particleSystem );
			}
		}

		private void ps_PositionsUpdate()
		{
			Application.DoEvents();
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
			m_Models.particleSystem.ContentUpdate -= m_Event;
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
			this.SuspendLayout();
			// 
			// tra_Scroll_Extended
			// 
			this.tra_Scroll_Extended.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tra_Scroll_Extended.Location = new System.Drawing.Point(0, 181);
			this.tra_Scroll_Extended.Name = "tra_Scroll_Extended";
			this.tra_Scroll_Extended.Size = new System.Drawing.Size(480, 64);
			this.tra_Scroll_Extended.TabIndex = 4;
			// 
			// panel_Viewer
			// 
			this.panel_Viewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Viewer.Location = new System.Drawing.Point(0, 0);
			this.panel_Viewer.Name = "panel_Viewer";
			this.panel_Viewer.Size = new System.Drawing.Size(480, 181);
			this.panel_Viewer.TabIndex = 2;
			// 
			// ModelViewDoc
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(480, 245);
			this.Controls.Add(this.panel_Viewer);
			this.Controls.Add(this.tra_Scroll_Extended);
			this.Name = "ModelViewDoc";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
