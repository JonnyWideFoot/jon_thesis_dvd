using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using System.Net.Sockets ;
using System.Net ;
using System.Threading ;
using System.Text;
using System.IO;

using UoB.Comms;

namespace UoB.Comms.ClientSide
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class ConnectionProperties : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox box_IP;
		private System.Windows.Forms.TextBox box_Port;
		private System.Windows.Forms.Button button_Connect;
		private System.Windows.Forms.Button button_DisConnect;
		private System.Windows.Forms.StatusBar sbar;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        private ClientComms m_Client;

		public ConnectionProperties(ClientComms client)
		{
			InitializeComponent();
			m_Client = client;
			button_Connect.Enabled = true;
			button_DisConnect.Enabled = false;
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
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.box_IP = new System.Windows.Forms.TextBox();
			this.box_Port = new System.Windows.Forms.TextBox();
			this.button_Connect = new System.Windows.Forms.Button();
			this.button_DisConnect = new System.Windows.Forms.Button();
			this.sbar = new System.Windows.Forms.StatusBar();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 24);
			this.label2.TabIndex = 15;
			this.label2.Text = "Port";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 13;
			this.label1.Text = "IP Address";
			// 
			// box_IP
			// 
			this.box_IP.Location = new System.Drawing.Point(96, 8);
			this.box_IP.Name = "box_IP";
			this.box_IP.TabIndex = 19;
			this.box_IP.Text = "137.222.50.39";
			// 
			// box_Port
			// 
			this.box_Port.Location = new System.Drawing.Point(96, 40);
			this.box_Port.Name = "box_Port";
			this.box_Port.TabIndex = 20;
			this.box_Port.Text = "8002";
			// 
			// button_Connect
			// 
			this.button_Connect.Location = new System.Drawing.Point(208, 8);
			this.button_Connect.Name = "button_Connect";
			this.button_Connect.TabIndex = 23;
			this.button_Connect.Text = "Connect";
			this.button_Connect.Click += new System.EventHandler(this.button_Connect_Click);
			// 
			// button_DisConnect
			// 
			this.button_DisConnect.Location = new System.Drawing.Point(208, 40);
			this.button_DisConnect.Name = "button_DisConnect";
			this.button_DisConnect.TabIndex = 24;
			this.button_DisConnect.Text = "DisConnect";
			this.button_DisConnect.Click += new System.EventHandler(this.button_DisConnect_Click);
			// 
			// sbar
			// 
			this.sbar.Location = new System.Drawing.Point(0, 73);
			this.sbar.Name = "sbar";
			this.sbar.Size = new System.Drawing.Size(296, 20);
			this.sbar.TabIndex = 1;
			this.sbar.Text = "Click Connect";
			// 
			// ConnectionProperties
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(296, 93);
			this.Controls.Add(this.box_Port);
			this.Controls.Add(this.box_IP);
			this.Controls.Add(this.button_DisConnect);
			this.Controls.Add(this.button_Connect);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.sbar);
			this.Name = "ConnectionProperties";
			this.Text = "TcpClient";
			this.ResumeLayout(false);

		}
		#endregion


		private void button_DisConnect_Click(object sender, System.EventArgs e)
		{
			sbar.Text="Click Connect...";
			button_Connect.Enabled = true;
			button_DisConnect.Enabled = false;
			m_Client.DisConnect();
		}

		private void button_Connect_Click(object sender, System.EventArgs e)
		{
			sbar.Text="Connected";
			button_Connect.Enabled = false;
			button_DisConnect.Enabled = true;
			m_Client.Connect(box_IP.Text, int.Parse(box_Port.Text) );
		}

	}
}
