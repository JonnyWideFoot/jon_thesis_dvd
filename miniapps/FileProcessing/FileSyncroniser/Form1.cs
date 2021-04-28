using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;

using System.Text;

namespace FileSyncroniser
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox text_PathSource;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox text_PathDest;
		private System.Windows.Forms.Button button_Go;
		private System.Windows.Forms.TextBox text_Report;
		private System.Windows.Forms.Panel panel1;
        private SplitContainer splitContainer1;
        private Button m_Clear;
        private CheckBox checkBox1;
        private TextBox pathBox;
        private Button Stop;
        private Label ticker;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
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
            this.text_PathSource = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.text_PathDest = new System.Windows.Forms.TextBox();
            this.button_Go = new System.Windows.Forms.Button();
            this.text_Report = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.m_Clear = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pathBox = new System.Windows.Forms.TextBox();
            this.Stop = new System.Windows.Forms.Button();
            this.ticker = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // text_PathSource
            // 
            this.text_PathSource.Location = new System.Drawing.Point(80, 7);
            this.text_PathSource.Name = "text_PathSource";
            this.text_PathSource.Size = new System.Drawing.Size(200, 20);
            this.text_PathSource.TabIndex = 0;
            this.text_PathSource.Text = "Y:\\";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "SourceDir:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "DestDir:";
            // 
            // text_PathDest
            // 
            this.text_PathDest.Location = new System.Drawing.Point(81, 31);
            this.text_PathDest.Name = "text_PathDest";
            this.text_PathDest.Size = new System.Drawing.Size(200, 20);
            this.text_PathDest.TabIndex = 2;
            this.text_PathDest.Text = "Z:\\MaxSync\\";
            // 
            // button_Go
            // 
            this.button_Go.Location = new System.Drawing.Point(288, 6);
            this.button_Go.Name = "button_Go";
            this.button_Go.Size = new System.Drawing.Size(128, 23);
            this.button_Go.TabIndex = 4;
            this.button_Go.Text = "Syncronice to Dest ...";
            this.button_Go.Click += new System.EventHandler(this.button_Go_Click);
            // 
            // text_Report
            // 
            this.text_Report.Dock = System.Windows.Forms.DockStyle.Fill;
            this.text_Report.Location = new System.Drawing.Point(0, 0);
            this.text_Report.Multiline = true;
            this.text_Report.Name = "text_Report";
            this.text_Report.Size = new System.Drawing.Size(520, 308);
            this.text_Report.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ticker);
            this.panel1.Controls.Add(this.Stop);
            this.panel1.Controls.Add(this.pathBox);
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Controls.Add(this.m_Clear);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.text_PathSource);
            this.panel1.Controls.Add(this.button_Go);
            this.panel1.Controls.Add(this.text_PathDest);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(520, 85);
            this.panel1.TabIndex = 6;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(369, 34);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(94, 17);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "Inform Existing";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // m_Clear
            // 
            this.m_Clear.Location = new System.Drawing.Point(288, 32);
            this.m_Clear.Name = "m_Clear";
            this.m_Clear.Size = new System.Drawing.Size(75, 23);
            this.m_Clear.TabIndex = 5;
            this.m_Clear.Text = "Clear Text";
            this.m_Clear.UseVisualStyleBackColor = true;
            this.m_Clear.Click += new System.EventHandler(this.m_Clear_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.text_Report);
            this.splitContainer1.Size = new System.Drawing.Size(520, 397);
            this.splitContainer1.SplitterDistance = 85;
            this.splitContainer1.TabIndex = 7;
            // 
            // pathBox
            // 
            this.pathBox.Location = new System.Drawing.Point(12, 59);
            this.pathBox.Name = "pathBox";
            this.pathBox.Size = new System.Drawing.Size(496, 20);
            this.pathBox.TabIndex = 7;
            // 
            // Stop
            // 
            this.Stop.ForeColor = System.Drawing.Color.Red;
            this.Stop.Location = new System.Drawing.Point(422, 7);
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(41, 23);
            this.Stop.TabIndex = 8;
            this.Stop.Text = "Stop";
            this.Stop.UseVisualStyleBackColor = false;
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // ticker
            // 
            this.ticker.AutoSize = true;
            this.ticker.Font = new System.Drawing.Font("Courier New", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ticker.Location = new System.Drawing.Point(475, 7);
            this.ticker.Name = "ticker";
            this.ticker.Size = new System.Drawing.Size(38, 39);
            this.ticker.TabIndex = 9;
            this.ticker.Text = ".";
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(520, 397);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private int m_MaxLength = 5000;
        FileSyncroniser f = null;

		private void button_Go_Click(object sender, System.EventArgs e)
		{
            if (f != null)
            {
                f.Kill();
                f = null;
            }

            Thread t = new Thread(new ThreadStart(Launch));
            t.Priority = ThreadPriority.BelowNormal;
            t.Start();
		}

        private void Launch()
        {
            f = new FileSyncroniser(text_PathSource.Text, text_PathDest.Text, false, false);
            f.InformExisting = checkBox1.Checked;
            f.FileProcess += new StringEvent(f_FileAdded);
            f.DirProcess += new StringEvent(f_DirAdded);
            f.Tick += new NullEvent(f_Tick);
            f.Execute();
            if (f.WasKilled)
            {
                MessageBox.Show("Directory Sync In-Complete! User Terminated!", "Sync Complete", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show("Directory Sync Complete!", "Sync Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void f_FileAdded(string s)
        {
            // Invoke the string listing on the main thread
            Invoke(new StringEvent(t_fileAdded), s);
        }

        private void f_DirAdded(string s)
        {
            // Invoke the string listing on the main thread
            Invoke(new StringEvent(t_DirAdded), s);
        }

        private void f_Tick()
        {
            Invoke(new NullEvent(t_Tick));
        }

		StringBuilder m_SB = new StringBuilder();
        private void t_fileAdded(string s)
        {
            m_SB.Insert(0, s + "\r\n");
            if (m_SB.Length > m_MaxLength)
            {
                m_SB.Remove((int)m_MaxLength, (int)(m_SB.Length - m_MaxLength));
            }
            text_Report.Text = m_SB.ToString();
            t_Tick();
        }

        private void t_DirAdded(string s)
        {
            pathBox.Text = s;
        }

        private void t_Tick()
        {
            switch (ticker.Text[0])
            {
                case '/':
                    ticker.Text = "-";
                    break;
                case '-':
                    ticker.Text = "\\";
                    break;
                case '\\':
                    ticker.Text = "|";
                    break;
                case '|':
                    ticker.Text = "/";
                    break;
                default:
                    ticker.Text = "/";
                    break;
            }
        }

        private void m_Clear_Click(object sender, EventArgs e)
        {
            m_SB.Remove(0, m_SB.Length);
            text_Report.Text = "";
            pathBox.Text = "";
        }

        private void DoGracefulHalt()
        {
            if (f != null)
            {
                f.Kill();
                f = null;
            }
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            DoGracefulHalt();            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DoGracefulHalt();
        }
	}
}
