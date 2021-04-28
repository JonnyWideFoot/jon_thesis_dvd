using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;

using UoB.CoreControls.ToolWindows;

namespace UoB.CoreControls.Reporting
{
	/// <summary>
	/// Summary description for SimpleReporter.
	/// </summary>
	public class SimpleReporter : System.Windows.Forms.UserControl, ITool
	{
		private System.Windows.Forms.TextBox m_TextBox;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private SimpleReportListener m_Listener;

		public string ReportText
		{
			set
			{
				m_TextBox.Text = value;
				if ( m_TextBox.Text.Length > 30 )
				{
					m_TextBox.Select(m_TextBox.Text.Length-30, 0);
				}
				m_TextBox.ScrollToCaret();
			}
		}
		
		public SimpleReporter()
		{
			InitializeComponent();
			m_TextBox.ScrollBars = ScrollBars.Both;
			m_Listener = new SimpleReportListener( this );
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
			this.m_TextBox = new System.Windows.Forms.TextBox();
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// m_TextBox
			// 
			this.m_TextBox.ContextMenu = this.contextMenu1;
			this.m_TextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_TextBox.Location = new System.Drawing.Point(0, 0);
			this.m_TextBox.Multiline = true;
			this.m_TextBox.Name = "m_TextBox";
			this.m_TextBox.Size = new System.Drawing.Size(256, 273);
			this.m_TextBox.TabIndex = 0;
			this.m_TextBox.Text = "";
			this.m_TextBox.WordWrap = false;
			// 
			// contextMenu1
			// 
			this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "&Clear Listing";
			this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
			// 
			// SimpleReporter
			// 
			this.Controls.Add(this.m_TextBox);
			this.Name = "SimpleReporter";
			this.Size = new System.Drawing.Size(256, 273);
			this.ResumeLayout(false);

		}
		#endregion

		private void menuItem1_Click(object sender, System.EventArgs e)
		{
			m_Listener.ClearText();
		}
		#region ITool Members

		public void AttachToDocument(UoB.CoreControls.Documents.Document doc)
		{
			// Reporter doesnt care, all documents are listend to via the Debug and Trace talk
			// of System.Diagnostics
		}

		#endregion
	}
}
