using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace UoB.DAVE.Dialog
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class aboutBox : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.PictureBox pictureBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public aboutBox()
		{
			InitializeComponent();
			textBox.Text = "\r\n\r\nDAVE Version " + UoB.Research.UoBInit.Instance.Version + @" 
<-- Dynamic Atomic Visualisation Environment -->
CopyRight 2004 Jon Rea
Contact - Jon.Rea@bris.ac.uk";
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(aboutBox));
			this.button_OK = new System.Windows.Forms.Button();
			this.textBox = new System.Windows.Forms.TextBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// button_OK
			// 
			this.button_OK.Cursor = System.Windows.Forms.Cursors.Default;
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_OK.Location = new System.Drawing.Point(80, 88);
			this.button_OK.Name = "button_OK";
			this.button_OK.TabIndex = 0;
			this.button_OK.Text = "OK";
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// textBox
			// 
			this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox.Location = new System.Drawing.Point(0, 0);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(248, 125);
			this.textBox.TabIndex = 1;
			this.textBox.Text = "";
			this.textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.Color.White;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(24, 24);
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			// 
			// aboutBox
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(248, 125);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.button_OK);
			this.Controls.Add(this.textBox);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "aboutBox";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "About DAVE";
			this.ResumeLayout(false);

		}
		#endregion

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			Close();
		}
	}
}
