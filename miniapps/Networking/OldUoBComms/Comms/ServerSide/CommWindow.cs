using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using UoB;
using UoB.Research;

namespace UoB.Comms.ServerSide
{
	/// <summary>
	/// Summary description for CommWindow.
	/// </summary>
	public class CommWindow : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox box_Report;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListView clientList;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private ServerComms m_Server;

		public CommWindow( ServerComms server )
		{
			Clientupdate = new UpdateEvent( server_ClientListUpdate );
			m_Server = server;
			InitializeComponent();
			CreateHandle();
		}


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
			this.box_Report = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.clientList = new System.Windows.Forms.ListView();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// box_Report
			// 
			this.box_Report.Location = new System.Drawing.Point(8, 176);
			this.box_Report.Multiline = true;
			this.box_Report.Name = "box_Report";
			this.box_Report.Size = new System.Drawing.Size(552, 120);
			this.box_Report.TabIndex = 9;
			this.box_Report.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 160);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(168, 23);
			this.label2.TabIndex = 10;
			this.label2.Text = "Communiction Summary";
			// 
			// clientList
			// 
			this.clientList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						 this.columnHeader4,
																						 this.columnHeader2,
																						 this.columnHeader1,
																						 this.columnHeader3,
																						 this.columnHeader5,
																						 this.columnHeader6});
			this.clientList.Location = new System.Drawing.Point(8, 8);
			this.clientList.Name = "clientList";
			this.clientList.Size = new System.Drawing.Size(552, 144);
			this.clientList.TabIndex = 8;
			this.clientList.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Status";
			this.columnHeader4.Width = 46;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "UserName";
			this.columnHeader2.Width = 72;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "UserInfo";
			this.columnHeader1.Width = 133;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Login Time";
			this.columnHeader3.Width = 135;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Alive Count";
			this.columnHeader5.Width = 68;
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "TotalMsgCount";
			this.columnHeader6.Width = 92;
			// 
			// CommWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(568, 309);
			this.Controls.Add(this.box_Report);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.clientList);
			this.Name = "CommWindow";
			this.Text = "CommWindow";
			this.ResumeLayout(false);

		}
		#endregion

		private UpdateEvent Clientupdate;

		public void UpdateClientList()
		{
			Invoke(Clientupdate, null);
		}

		private void server_ClientListUpdate()
		{
			clientList.Items.Clear();

			foreach ( ConnectionManager cm in m_Server.Connections )
			{
					ClientInfo ci = cm.ClientInfo;
					ListViewItem i = new ListViewItem();
					i.Text = "";
					i.SubItems.Add( ci.Username );
					i.SubItems.Add( ci.UserInfoString );
					i.SubItems.Add( ci.timeString );
					i.SubItems.Add( ci.keepAliveMsgCount.ToString() );
					i.SubItems.Add( ci.totalMsgCount.ToString() );
					clientList.Items.Add(i);
			}
		}

		private void m_Server_m_ConnectionReceived(ConnectionManager cm)
		{
			cm.ClientInfoUpdate += new UpdateEvent(server_ClientListUpdate);
		}
	}
}
