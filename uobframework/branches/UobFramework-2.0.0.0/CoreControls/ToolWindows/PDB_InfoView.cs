using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using UoB.Core.FileIO.PDB;
using System.Text;

namespace UoB.CoreControls.ToolWindows
{
	/// <summary>
	/// Summary description for PDB_InfoView.
	/// </summary>
	public class PDB_InfoView : System.Windows.Forms.UserControl, ITool
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox text_InfoPane;

		private PDBInfo m_Info;

		public PDB_InfoView()
		{
			InitializeComponent();
			m_Info = null;
			getDetails();
		}
		public PDB_InfoView(PDBInfo pInfo)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			m_Info = pInfo;
			getDetails();
		}

		private void getDetails()
		{
			if( m_Info != null )
			{
				text_InfoPane.Text = m_Info.ToString();
			}
			else
			{
				text_InfoPane.Text = "No System Present ...";
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
			this.text_InfoPane = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// text_InfoPane
			// 
			this.text_InfoPane.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.text_InfoPane.Dock = System.Windows.Forms.DockStyle.Fill;
			this.text_InfoPane.Location = new System.Drawing.Point(0, 0);
			this.text_InfoPane.Multiline = true;
			this.text_InfoPane.Name = "text_InfoPane";
			this.text_InfoPane.Size = new System.Drawing.Size(208, 264);
			this.text_InfoPane.TabIndex = 0;
			this.text_InfoPane.Text = "";
			// 
			// PDB_InfoView
			// 
			this.Controls.Add(this.text_InfoPane);
			this.Name = "PDB_InfoView";
			this.Size = new System.Drawing.Size(208, 264);
			this.ResumeLayout(false);

		}
		#endregion

		#region ITool Members


		public void AttactToPDBInfo( PDBInfo info )
		{
			m_Info = info;
			getDetails();
		}


		public void AttachToDocument(UoB.CoreControls.Documents.Document doc)
		{
			m_Info = null;
			if ( doc != null )
			{
				for( int i = 0; i < doc.MemberCount; i++ )
				{
					if ( doc[i].GetType() == typeof(PDB) )
					{
						PDB pdb = (PDB) doc[i];
						m_Info = pdb.ExtendedInformation;
						break; // only one drawWrapper should ever be present
					}
				}
			}
			getDetails();			
		}

		#endregion
	}
}
