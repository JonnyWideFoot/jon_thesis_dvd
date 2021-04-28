using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using UoB.Core.Structure;
using UoB.CoreControls.OpenGLView;
using UoB.CoreControls.PS_Render;
using UoB.CoreControls.Documents;
using UoB.Core;
using UoB.Core.FileIO.PDB;

namespace UoB.DAVE.Documents
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class PSViewDoc : Document
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private GLView m_Viewer;
		private ParticleSystem m_ParticleSystem;
		private ParticleSystemDrawWrapper m_DrawWrapper;

		public PSViewDoc()
		{
			InitializeComponent();
			CommonInit( null );
		}

		public PSViewDoc( ParticleSystem ps )
		{			
			InitializeComponent();
			CommonInit( ps );
		}

		private void CommonInit( ParticleSystem sentPS )
		{
			Size = new Size(600,480);
			m_CanSave = true;

			m_Viewer = new GLView();
			m_Viewer.Parent = this;
			m_Viewer.Dock = DockStyle.Fill;
			m_DrawWrapper = new ParticleSystemDrawWrapper( null, m_Viewer );
			
			AddMember( m_DrawWrapper ); // needed by monitoring tool windows that check which members are present for interaction ...

			particleSystem = sentPS;			
		}

        public ParticleSystemDrawWrapper DrawWrapper
        {
            get
            {
                return m_DrawWrapper;
            }
        }

		public ParticleSystem particleSystem
		{
			get
			{
				return m_ParticleSystem;
			}
			set
			{
				if( m_ParticleSystem != null )
				{
					RemoveMember(m_ParticleSystem);
				}
				m_ParticleSystem = value;
				m_DrawWrapper.particleSystem = m_ParticleSystem;
				if( m_ParticleSystem != null )
				{
					AddMember(m_ParticleSystem);
					Text = m_ParticleSystem.Name;
				}
				else
				{
					Text = "No ParticleSystem";
				}
			}
		}

		public override void Save()
		{
			if(saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				PDB.SaveNew( saveFileDialog.FileName, m_ParticleSystem );
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
			// 
			// PSViewDoc
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(528, 341);
			this.Name = "PSViewDoc";
			this.Text = "Form1";

		}
		#endregion
	}
}
