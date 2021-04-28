using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

using UoB.Methodology.PhiPsiAnalysis;

namespace Origin
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button button_Clear;
		private System.Windows.Forms.Button button_ProcDI;
		private System.Windows.Forms.Button button_SetFromTo;
		private System.Windows.Forms.TextBox text_FromX;
		private System.Windows.Forms.TextBox text_ToX;
		private System.Windows.Forms.TextBox text_ToY;
		private System.Windows.Forms.TextBox text_FromY;

		private PhiPsiData m_Origin;

		public Form1()
		{
			InitializeComponent();
	
			//string dbName = "IGDataBase";
			//string dirPath = @"C:\Main Backup 21.03.05\_My Calculations\09d - My Loop DataBase\IGDomains\";
			
			string dbName = "PDBSelect2004-1.9A";
			string dirPath = @"C:\Main Backup 21.03.05\_My Calculations\09d - My Loop DataBase\PDBSelect2004\4. 1.9 or less\";

			DirectoryInfo di = new DirectoryInfo(dirPath);

			if( !di.Exists )
			{
				throw new Exception();
			}
			
			m_Origin = new PhiPsiData( dbName, di );

			
			//m_Origin.GoHTMLAngleFittingReport();
			m_Origin.DeviantAngleStats();

			//m_Origin.CallStatFunction();
			//m_Origin.LoopLengthCounting();
			//m_Origin.CountDisallowed();
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
			this.button_Clear = new System.Windows.Forms.Button();
			this.button_ProcDI = new System.Windows.Forms.Button();
			this.text_FromX = new System.Windows.Forms.TextBox();
			this.text_ToX = new System.Windows.Forms.TextBox();
			this.button_SetFromTo = new System.Windows.Forms.Button();
			this.text_ToY = new System.Windows.Forms.TextBox();
			this.text_FromY = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// button_Clear
			// 
			this.button_Clear.Location = new System.Drawing.Point(8, 8);
			this.button_Clear.Name = "button_Clear";
			this.button_Clear.Size = new System.Drawing.Size(88, 23);
			this.button_Clear.TabIndex = 0;
			this.button_Clear.Text = "Clear Current";
			this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
			// 
			// button_ProcDI
			// 
			this.button_ProcDI.Location = new System.Drawing.Point(8, 40);
			this.button_ProcDI.Name = "button_ProcDI";
			this.button_ProcDI.Size = new System.Drawing.Size(88, 23);
			this.button_ProcDI.TabIndex = 2;
			this.button_ProcDI.Text = "Go Report";
			this.button_ProcDI.Click += new System.EventHandler(this.button_ProcDI_Click);
			// 
			// text_FromX
			// 
			this.text_FromX.Location = new System.Drawing.Point(104, 8);
			this.text_FromX.Name = "text_FromX";
			this.text_FromX.Size = new System.Drawing.Size(48, 20);
			this.text_FromX.TabIndex = 4;
			this.text_FromX.Text = "-180";
			// 
			// text_ToX
			// 
			this.text_ToX.Location = new System.Drawing.Point(160, 8);
			this.text_ToX.Name = "text_ToX";
			this.text_ToX.Size = new System.Drawing.Size(48, 20);
			this.text_ToX.TabIndex = 5;
			this.text_ToX.Text = "180";
			// 
			// button_SetFromTo
			// 
			this.button_SetFromTo.Location = new System.Drawing.Point(120, 56);
			this.button_SetFromTo.Name = "button_SetFromTo";
			this.button_SetFromTo.Size = new System.Drawing.Size(72, 23);
			this.button_SetFromTo.TabIndex = 6;
			this.button_SetFromTo.Text = "SetFromTo";
			this.button_SetFromTo.Click += new System.EventHandler(this.button_SetFromTo_Click);
			// 
			// text_ToY
			// 
			this.text_ToY.Location = new System.Drawing.Point(160, 32);
			this.text_ToY.Name = "text_ToY";
			this.text_ToY.Size = new System.Drawing.Size(48, 20);
			this.text_ToY.TabIndex = 8;
			this.text_ToY.Text = "180";
			// 
			// text_FromY
			// 
			this.text_FromY.Location = new System.Drawing.Point(104, 32);
			this.text_FromY.Name = "text_FromY";
			this.text_FromY.Size = new System.Drawing.Size(48, 20);
			this.text_FromY.TabIndex = 7;
			this.text_FromY.Text = "-180";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(216, 85);
			this.Controls.Add(this.text_ToY);
			this.Controls.Add(this.text_FromY);
			this.Controls.Add(this.button_SetFromTo);
			this.Controls.Add(this.text_ToX);
			this.Controls.Add(this.text_FromX);
			this.Controls.Add(this.button_ProcDI);
			this.Controls.Add(this.button_Clear);
			this.Name = "Form1";
			this.Text = "Form1";
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			System.Windows.Forms.Application.Run(new Form1());
		}

		private void ClearCurrentData()
		{
			MessageBox.Show("Deactivated");
			//m_Origin.ClearCurrent();
		}

		private void button_Clear_Click(object sender, System.EventArgs e)
		{
			ClearCurrentData();		
		}

		private void button_ProcDI_Click(object sender, System.EventArgs e)
		{
			for( int i = 0; i < this.Controls.Count; i++ )
			{
				this.Controls[i].Enabled = false;
			}

			//m_Origin.GoHTMLRamachandranReport();
			//m_Origin.GoPropensityReport();

			for( int i = 0; i < this.Controls.Count; i++ )
			{
				this.Controls[i].Enabled = true;
			}
		}

		private void button_SetFromTo_Click(object sender, System.EventArgs e)
		{
			MessageBox.Show("Deactivated");
//			try
//			{
//				m_Origin.SetBounds( 
//					float.Parse( text_FromX.Text ),
//					float.Parse( text_ToX.Text ),
//					float.Parse( text_FromY.Text ),
//					float.Parse( text_ToY.Text )
//					);
//			}
//			catch( Exception ex )
//			{
//				MessageBox.Show(ex.Message);
//			}
		}
	}
}
