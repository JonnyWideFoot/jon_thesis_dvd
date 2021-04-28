using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace UoB.CoreControls.Documents
{
	/// <summary>
	/// Summary description for Document.
	/// </summary>
	public class Document : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		protected ArrayList m_Members;
		protected System.Windows.Forms.SaveFileDialog saveFileDialog;
		protected bool m_CanSave = false;

		public Document()
		{
			m_Members = new ArrayList(10);
			InitializeComponent();
		}

		public Object this[int index] 
		{
			get 
			{
				return( m_Members[index] );
			}
		}

		public virtual bool canSave
		{
			get
			{
				return m_CanSave;
			}
		}

		public virtual void Save( )
		{
		}

		public void RemoveMember( object member )
		{
			if( m_Members.Contains( member ) )
			{
				m_Members.Remove( member );
			}
			else
			{
				throw new Exception("Remove called for an object that isnt a member!");
			}
		}

		public void AddMember(object member)
		{
			m_Members.Add( member );
		}

		public int MemberCount
		{
			get
			{
				return m_Members.Count;
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
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.RestoreDirectory = true;
			this.saveFileDialog.Title = "Save File";
			// 
			// Document
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(472, 245);
			this.Name = "Document";
			this.Text = "Document";

		}
		#endregion
	}
}
