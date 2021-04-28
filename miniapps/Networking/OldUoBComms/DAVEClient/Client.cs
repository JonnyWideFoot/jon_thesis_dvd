using System;
using System.Drawing;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

using UoB.Comms;
using UoB.Comms.ClientSide;
using UoB.Research.FileIO.PDB;
using UoB.Research.Modelling.Structure;

namespace UoB.DAVEClient
{
	/// <summary>
	/// Summary description for ClientMain.
	/// </summary>
	public class ClientMain : System.Windows.Forms.Form
	{

		private System.ComponentModel.Container components = null;


		private System.Windows.Forms.Button button_SendPDB;
		private System.Windows.Forms.Button buton_SendMessage;
		private System.Windows.Forms.TextBox box_MessageText;
		private System.Windows.Forms.TextBox box_UserComment;
		private System.Windows.Forms.TextBox box_UserName;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button_SendUserInfo;
		private System.Windows.Forms.Button button_BeginModulate;

		private ClientComms m_Comms;
		private PDB m_PDB;
		private System.Windows.Forms.TextBox text_Modulate_Count;
		private ParticleSystem m_ParticleSystem;

		public ClientMain()
		{
			InitializeComponent();

			string filePath = @"c:\mu2.pdb";

			m_Comms = new ClientComms();
			m_Comms.ShowSettingsWindow();
			m_PDB = new PDB(filePath, true);
			m_ParticleSystem = m_PDB.particleSystem;
		}

		private void button_SendUserInfo_Click(object sender, System.EventArgs e)
		{
			string send = box_UserName.Text + ":" + box_UserComment.Text;
			CommNote_UserInfo commNote = new CommNote_UserInfo( send );
			m_Comms.SendNote( commNote );
		}

		private void buton_SendMessage_Click(object sender, System.EventArgs e)
		{
			CommNote_Reporter commNote = new CommNote_Reporter( box_MessageText.Text );
			m_Comms.SendNote( commNote );
		}

		private void button_SendPDB_Click(object sender, System.EventArgs e)
		{
			Trace.WriteLine("Sending PDB File Clicked ...");
			m_Comms.SendNote( new CommNote_SendAllAtoms( m_ParticleSystem ) );
		}

		private void button_BeginModulate_Click(object sender, System.EventArgs e)
		{
            System.Timers.Timer bob = new System.Timers.Timer(2000);
			bob.Elapsed += new System.Timers.ElapsedEventHandler(bob_Elapsed);
			bob.Start();
		}

		private void bob_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
            ThreadPool.QueueUserWorkItem( new WaitCallback( ModulateWorker ) );
		}

		private static int count = 0;
		private object sendLock = new object();
		private void ModulateWorker(object nullState)
		{
			lock ( sendLock )
			{
				Random rand = new Random();

				bool done = false;
				while(!done)
				{  
					try
					{
						m_ParticleSystem.AcquireWriterLock(1000);
						try
						{
							// It is safe for this thread to read from
							// the shared resource.

							foreach ( Atom A in m_ParticleSystem )
							{
								A.x = rand.NextDouble() * 10;
							}

							done = true;
						}        
						finally
						{
							// Ensure that the lock is released.
							m_ParticleSystem.ReleaseWriterLock();
						}
					}
					catch (ApplicationException)
					{
						// The reader lock request timed out.
					}
				}

				count++;
				text_Modulate_Count.Text = count.ToString();
				m_Comms.SendNote( new CommNote_Reporter("Sending Positions : " + count.ToString() ) );
				m_Comms.SendNote( new CommNote_SendAllPositions( m_ParticleSystem ) );
			}
		}




		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new ClientMain());
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
			this.button_SendPDB = new System.Windows.Forms.Button();
			this.buton_SendMessage = new System.Windows.Forms.Button();
			this.box_MessageText = new System.Windows.Forms.TextBox();
			this.box_UserComment = new System.Windows.Forms.TextBox();
			this.box_UserName = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.button_SendUserInfo = new System.Windows.Forms.Button();
			this.button_BeginModulate = new System.Windows.Forms.Button();
			this.text_Modulate_Count = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// button_SendPDB
			// 
			this.button_SendPDB.Location = new System.Drawing.Point(8, 120);
			this.button_SendPDB.Name = "button_SendPDB";
			this.button_SendPDB.Size = new System.Drawing.Size(88, 23);
			this.button_SendPDB.TabIndex = 34;
			this.button_SendPDB.Text = "Send PDB Def";
			this.button_SendPDB.Click += new System.EventHandler(this.button_SendPDB_Click);
			// 
			// buton_SendMessage
			// 
			this.buton_SendMessage.Location = new System.Drawing.Point(224, 80);
			this.buton_SendMessage.Name = "buton_SendMessage";
			this.buton_SendMessage.TabIndex = 33;
			this.buton_SendMessage.Text = "Message";
			this.buton_SendMessage.Click += new System.EventHandler(this.buton_SendMessage_Click);
			// 
			// box_MessageText
			// 
			this.box_MessageText.Location = new System.Drawing.Point(8, 80);
			this.box_MessageText.Name = "box_MessageText";
			this.box_MessageText.Size = new System.Drawing.Size(208, 20);
			this.box_MessageText.TabIndex = 32;
			this.box_MessageText.Text = "This message has just been sent ....";
			// 
			// box_UserComment
			// 
			this.box_UserComment.Location = new System.Drawing.Point(96, 40);
			this.box_UserComment.Name = "box_UserComment";
			this.box_UserComment.Size = new System.Drawing.Size(136, 20);
			this.box_UserComment.TabIndex = 31;
			this.box_UserComment.Text = "Jons Temp Runner Thingy";
			// 
			// box_UserName
			// 
			this.box_UserName.Location = new System.Drawing.Point(96, 8);
			this.box_UserName.Name = "box_UserName";
			this.box_UserName.TabIndex = 30;
			this.box_UserName.Text = "Jon";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 40);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(80, 16);
			this.label4.TabIndex = 29;
			this.label4.Text = "User Comment";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(72, 16);
			this.label3.TabIndex = 28;
			this.label3.Text = "User Name";
			// 
			// button_SendUserInfo
			// 
			this.button_SendUserInfo.Location = new System.Drawing.Point(248, 40);
			this.button_SendUserInfo.Name = "button_SendUserInfo";
			this.button_SendUserInfo.Size = new System.Drawing.Size(96, 23);
			this.button_SendUserInfo.TabIndex = 35;
			this.button_SendUserInfo.Text = "Send UserInfo";
			this.button_SendUserInfo.Click += new System.EventHandler(this.button_SendUserInfo_Click);
			// 
			// button_BeginModulate
			// 
			this.button_BeginModulate.Location = new System.Drawing.Point(104, 120);
			this.button_BeginModulate.Name = "button_BeginModulate";
			this.button_BeginModulate.Size = new System.Drawing.Size(112, 23);
			this.button_BeginModulate.TabIndex = 36;
			this.button_BeginModulate.Text = "Modulate Positions";
			this.button_BeginModulate.Click += new System.EventHandler(this.button_BeginModulate_Click);
			// 
			// text_Modulate_Count
			// 
			this.text_Modulate_Count.Location = new System.Drawing.Point(248, 120);
			this.text_Modulate_Count.Name = "text_Modulate_Count";
			this.text_Modulate_Count.TabIndex = 37;
			this.text_Modulate_Count.Text = "";
			// 
			// ClientMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(352, 149);
			this.Controls.Add(this.text_Modulate_Count);
			this.Controls.Add(this.button_BeginModulate);
			this.Controls.Add(this.button_SendUserInfo);
			this.Controls.Add(this.button_SendPDB);
			this.Controls.Add(this.buton_SendMessage);
			this.Controls.Add(this.box_MessageText);
			this.Controls.Add(this.box_UserComment);
			this.Controls.Add(this.box_UserName);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Name = "ClientMain";
			this.Text = "ClientMain";
			this.ResumeLayout(false);

		}
		#endregion


	}
}
