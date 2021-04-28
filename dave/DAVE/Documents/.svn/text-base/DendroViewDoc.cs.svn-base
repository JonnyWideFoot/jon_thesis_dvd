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
using UoB.Core.FileIO.Dendrogram;
using UoB.CoreControls.Dendrogram;

namespace UoB.DAVE.Documents
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class DendroViewDoc : Document
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private GLView m_Viewer;
		private DendroTree m_Tree;
		private TreeRenderer m_DrawWrapper;

		public DendroViewDoc( DendroTree tree )
		{			
			InitializeComponent();

			m_CanSave = false;
			m_Tree = tree;
			m_Viewer = new GLView();
			m_DrawWrapper = new TreeRenderer( tree, m_Viewer );

			m_Members.Add( tree );

			m_Viewer.Parent = this;
			m_Viewer.Dock = DockStyle.Fill;
			Size = new Size(600,480);
			Text = "Tree Viewer";
		}

		public void Begin()
		{
			m_DrawWrapper.beginSimulation();
		}

		public override void Save()
		{
			// null
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
