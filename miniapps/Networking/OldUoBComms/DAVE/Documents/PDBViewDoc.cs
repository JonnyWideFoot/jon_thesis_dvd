using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using UoB.Research.FileIO.PDB;

namespace UoB.DAVE.Documents
{
	/// <summary>
	/// Summary description for PDBViewDoc.
	/// </summary>
	public sealed class PDBViewDoc : PSViewDoc
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private UoB.Generic.ToolWindows.Model_Scroll m_ModelScroll;
		private PDB m_PDB;

		public PDBViewDoc(PDB thePDB) : base (thePDB.particleSystem)
		{
			m_PDB = thePDB;
			InitializeComponent();
			m_ModelScroll.AttachTo( thePDB.PositionDefinitions );  
			if ( thePDB.PositionDefinitions == null || thePDB.PositionDefinitions.Count == 1 )
			{
				m_ModelScroll.Visible = false;
			}
			Size = new Size(600,480);
			Text = thePDB.particleSystem.Name;

			AddMember( m_PDB );
			AddMember( m_PDB.SequenceInfo );
		}

		public PDB PDBFile
		{
			get
			{
				return m_PDB;
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
			this.m_ModelScroll = new UoB.Generic.ToolWindows.Model_Scroll();
			this.SuspendLayout();
			// 
			// m_ModelScroll
			// 
			this.m_ModelScroll.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.m_ModelScroll.Location = new System.Drawing.Point(0, 429);
			this.m_ModelScroll.Name = "m_ModelScroll";
			this.m_ModelScroll.Size = new System.Drawing.Size(664, 48);
			this.m_ModelScroll.TabIndex = 0;
			// 
			// PDBViewDoc
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(664, 477);
			this.Controls.Add(this.m_ModelScroll);
			this.Name = "PDBViewDoc";
			this.Text = "PDBViewDoc";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
