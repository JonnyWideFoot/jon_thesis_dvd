using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using UoB.Core.FileIO.Tra;

namespace UoB.CoreControls.ToolWindows
{
	/// <summary>
	/// Summary description for Tra_Scroll_Extended.
	/// </summary>
	public class Tra_Scroll_Extended : Tra_Scroll
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Splitter splitter1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Tra_Scroll_Extended( Tra tra ) : base( tra )
		{
			InitializeComponent();
		}

		public Tra_Scroll_Extended() : base()
		{
			InitializeComponent();
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_Timer)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
			this.SuspendLayout();
			// 
			// check_Reset
			// 
			this.check_Reset.Location = new System.Drawing.Point(88, 8);
			this.check_Reset.Name = "check_Reset";
			this.toolTip.SetToolTip(this.check_Reset, "Reset When Reach End Of Tra-Entries");
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(72, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 24);
			this.label1.Text = "Speed";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip.SetToolTip(this.label1, "Desired Speed (FPS)");
			// 
			// button_PlayPause
			// 
			this.button_PlayPause.Location = new System.Drawing.Point(29, 8);
			this.button_PlayPause.Name = "button_PlayPause";
			// 
			// button_Prev
			// 
			this.button_Prev.Location = new System.Drawing.Point(2, 8);
			this.button_Prev.Name = "button_Prev";
			// 
			// button_Next
			// 
			this.button_Next.Location = new System.Drawing.Point(56, 8);
			this.button_Next.Name = "button_Next";
			// 
			// numericUpDown
			// 
			this.numericUpDown.Location = new System.Drawing.Point(8, 34);
			this.numericUpDown.Name = "numericUpDown";
			// 
			// m_Timer
			// 
			this.m_Timer.Enabled = false;
			// 
			// trackBar
			// 
			this.trackBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.trackBar.Location = new System.Drawing.Point(139, 0);
			this.trackBar.Name = "trackBar";
			this.trackBar.Size = new System.Drawing.Size(501, 64);
			// 
			// label_Position
			// 
			this.label_Position.Location = new System.Drawing.Point(144, 38);
			this.label_Position.Name = "label_Position";
			this.label_Position.Size = new System.Drawing.Size(480, 16);
			this.label_Position.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(136, 64);
			this.panel1.TabIndex = 10;
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(136, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 64);
			this.splitter1.TabIndex = 11;
			this.splitter1.TabStop = false;
			// 
			// Tra_Scroll_Extended
			// 
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel1);
			this.Name = "Tra_Scroll_Extended";
			this.Size = new System.Drawing.Size(640, 64);
			this.Controls.SetChildIndex(this.panel1, 0);
			this.Controls.SetChildIndex(this.splitter1, 0);
			this.Controls.SetChildIndex(this.trackBar, 0);
			this.Controls.SetChildIndex(this.button_Next, 0);
			this.Controls.SetChildIndex(this.button_PlayPause, 0);
			this.Controls.SetChildIndex(this.button_Prev, 0);
			this.Controls.SetChildIndex(this.check_Reset, 0);
			this.Controls.SetChildIndex(this.label1, 0);
			this.Controls.SetChildIndex(this.numericUpDown, 0);
			this.Controls.SetChildIndex(this.label_Position, 0);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_Timer)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
	}
}
