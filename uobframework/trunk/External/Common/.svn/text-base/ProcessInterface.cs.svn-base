using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using UoB.Core.Structure;

namespace UoB.External.Common
{
	/// <summary>
	/// Summary description for MinimizationInterface.
	/// </summary>
	public class ProcessInterface : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.ComboBox combo_ProcessPriority;
		private System.Windows.Forms.Button button_Kill;
		private System.Windows.Forms.Button button_Start;
		private System.Windows.Forms.Label label2;

		protected OutSource m_Process;
		private System.Windows.Forms.Label label_Progress;
		protected Timer m_Timer;
		protected bool m_IsRunning = false;

		public ProcessInterface()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			m_Timer = new Timer();
			m_Timer.Tick += new EventHandler(t_Tick);
			m_Timer.Interval = 1000;
           
			progressBar1.Maximum = 1000; // not 100 to increase the resolution of the progress
			progressBar1.Minimum = 0;

			combo_ProcessPriority.SelectedIndex = 0;
		}

		private void t_Tick(object sender, EventArgs e)
		{
			int barValue = (int) ( m_Process.ProgressPercentage * 1000 ); 
			if( barValue > 1000 ) barValue = 1000;
			progressBar1.Value = barValue;
			label_Progress.Text = ((int)( m_Process.ProgressPercentage * 100 )).ToString() + "%"; 
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
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.label_Progress = new System.Windows.Forms.Label();
			this.combo_ProcessPriority = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.button_Kill = new System.Windows.Forms.Button();
			this.button_Start = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// progressBar1
			// 
			this.progressBar1.Enabled = false;
			this.progressBar1.Location = new System.Drawing.Point(8, 8);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(320, 16);
			this.progressBar1.TabIndex = 0;
			// 
			// label_Progress
			// 
			this.label_Progress.Location = new System.Drawing.Point(160, 24);
			this.label_Progress.Name = "label_Progress";
			this.label_Progress.Size = new System.Drawing.Size(168, 23);
			this.label_Progress.TabIndex = 1;
			this.label_Progress.Text = "Progress #";
			this.label_Progress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// combo_ProcessPriority
			// 
			this.combo_ProcessPriority.Enabled = false;
			this.combo_ProcessPriority.Items.AddRange(new object[] {
																	   "Normal",
																	   "BelowNormal",
																	   "Idle"});
			this.combo_ProcessPriority.Location = new System.Drawing.Point(8, 48);
			this.combo_ProcessPriority.Name = "combo_ProcessPriority";
			this.combo_ProcessPriority.Size = new System.Drawing.Size(232, 21);
			this.combo_ProcessPriority.TabIndex = 3;
			this.combo_ProcessPriority.SelectedIndexChanged += new System.EventHandler(this.combo_ProcessPriority_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "Process Priority";
			// 
			// button_Kill
			// 
			this.button_Kill.Enabled = false;
			this.button_Kill.ForeColor = System.Drawing.Color.Red;
			this.button_Kill.Location = new System.Drawing.Point(256, 48);
			this.button_Kill.Name = "button_Kill";
			this.button_Kill.TabIndex = 5;
			this.button_Kill.Text = "KILL";
			this.button_Kill.Click += new System.EventHandler(this.button_Kill_Click);
			// 
			// button_Start
			// 
			this.button_Start.Location = new System.Drawing.Point(256, 80);
			this.button_Start.Name = "button_Start";
			this.button_Start.TabIndex = 2;
			this.button_Start.Text = "Start";
			this.button_Start.Click += new System.EventHandler(this.button_Start_Click);
			// 
			// ProcessInterface
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(336, 117);
			this.Controls.Add(this.button_Kill);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.combo_ProcessPriority);
			this.Controls.Add(this.button_Start);
			this.Controls.Add(this.progressBar1);
			this.Controls.Add(this.label_Progress);
			this.Name = "ProcessInterface";
			this.Text = "ProcessInterface";
			this.Closed += new System.EventHandler(this.ProcessInterface_Closed);
			this.ResumeLayout(false);

		}
		#endregion

		private void combo_ProcessPriority_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if( m_IsRunning )
			{
				m_Process.SetPriority( (ProcessPriorityClass) Enum.Parse( typeof(ProcessPriorityClass), combo_ProcessPriority.SelectedItem.ToString(), true ) );
			}
		}

		protected virtual void button_Start_Click(object sender, System.EventArgs e)
		{
			m_Timer.Start();
			m_IsRunning = true;
		}

		protected virtual void SetEnabledForRunning( bool IsRunning )
		{
			button_Kill.Enabled = IsRunning;
			button_Start.Enabled = !IsRunning;
			progressBar1.Enabled = IsRunning;
			combo_ProcessPriority.Enabled = IsRunning;
		}

		protected virtual void onProcessEnd()
		{
			Close();
		}

		private void button_Kill_Click(object sender, System.EventArgs e)
		{
			Close();
			m_Process.Kill();
			Trace.WriteLine("User has killed the process");
		}

		private void ProcessInterface_Closed(object sender, System.EventArgs e)
		{
			SetEnabledForRunning( false );
			m_IsRunning = false;
			m_Timer.Stop();
			m_Timer.Dispose();
		}
	}
}
