using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using UoB.Core;
using UoB.Core.FileIO.Tra;

namespace UoB.CoreControls.TraInterface
{
	/// <summary>
	/// Summary description for TrajReadin.
	/// </summary>
	public class TrajReadin : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_Ignore;
		private System.Windows.Forms.Label label_Explanation;
		private System.Windows.Forms.TextBox textBox_TraEnd;
		private System.Windows.Forms.TextBox textBox_TraStart;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox_TraReadGap;
		private System.Windows.Forms.Button button_Accept;
		private System.Windows.Forms.GroupBox groupBox_RecomendedReadin;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label_FileSize;
		private System.Windows.Forms.Label label_NumberOfEntries;
		private System.Windows.Forms.Label label_BlockSize;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private TraPreview m_TraPreview;
		private System.Windows.Forms.Button button_Cancel;
		private Tra m_Tra;

		public TrajReadin( Tra trajectory, Form parent )
		{
			m_Tra = trajectory;
			m_TraPreview = m_Tra.TraPreview;

			int traFileSizeCutoff = 20000000; // default value
			// lets see if we have a redefinition in the program params ...
			string traKey = "TraFileSizeCutoff";
			if ( CoreIni.Instance.ContainsKey( traKey ) )
			{
				try
				{
					traFileSizeCutoff = int.Parse( CoreIni.Instance.ValueOf( traKey ) );
				}
				catch
				{
					// its fucked, so reset to the default;
					CoreIni.Instance.AddDefinition( traKey, traFileSizeCutoff.ToString() );
				}
			}
			else
			{
				// if it isnt there, add it ...
				CoreIni.Instance.AddDefinition( traKey, traFileSizeCutoff.ToString() );
			}

			if ( m_TraPreview.fileSize < traFileSizeCutoff )
			{
				m_Tra.LoadTrajectory(); // file is small enough to just load ...
				return; // return to the main program
			}

			float numEntries = (float)m_TraPreview.numberOfEntries;
			float traSizeCutoff = (float)traFileSizeCutoff;
			float teStartPoint = (float)m_TraPreview.TEStartPoint;
			float amountOfCuttoffForTEEntries = traSizeCutoff - teStartPoint;
			float TEBlockSize = (float)m_TraPreview.TEBlockSize;

			float skipLength = numEntries  / ( amountOfCuttoffForTEEntries / TEBlockSize );

			m_TraPreview.skipLength = (int) skipLength;

			m_TraPreview.skipLength++;
			if ( m_TraPreview.skipLength < 1 ) 
			{
				m_TraPreview.skipLength = 1;
			}	 

			InitializeComponent();

			KeyDown += new KeyEventHandler(TrajReadin_KeyDown);

			label_BlockSize.Text += m_TraPreview.TEBlockSize.ToString();
			label_FileSize.Text += m_TraPreview.fileSize.ToString();
			label_NumberOfEntries.Text += m_TraPreview.numberOfEntries.ToString();

			textBox_TraEnd.Text = m_TraPreview.endPoint.ToString();
			textBox_TraStart.Text = m_TraPreview.startPoint.ToString();
			textBox_TraReadGap.Text = m_TraPreview.skipLength.ToString();

			ShowDialog( parent );
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
			this.button_Ignore = new System.Windows.Forms.Button();
			this.label_Explanation = new System.Windows.Forms.Label();
			this.textBox_TraEnd = new System.Windows.Forms.TextBox();
			this.textBox_TraStart = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textBox_TraReadGap = new System.Windows.Forms.TextBox();
			this.button_Accept = new System.Windows.Forms.Button();
			this.groupBox_RecomendedReadin = new System.Windows.Forms.GroupBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label_NumberOfEntries = new System.Windows.Forms.Label();
			this.label_BlockSize = new System.Windows.Forms.Label();
			this.label_FileSize = new System.Windows.Forms.Label();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.groupBox_RecomendedReadin.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Ignore
			// 
			this.button_Ignore.ForeColor = System.Drawing.Color.Red;
			this.button_Ignore.Location = new System.Drawing.Point(344, 8);
			this.button_Ignore.Name = "button_Ignore";
			this.button_Ignore.Size = new System.Drawing.Size(96, 23);
			this.button_Ignore.TabIndex = 0;
			this.button_Ignore.Text = "Ignore Uberness";
			this.button_Ignore.Click += new System.EventHandler(this.button_Ignore_Click);
			// 
			// label_Explanation
			// 
			this.label_Explanation.BackColor = System.Drawing.SystemColors.Info;
			this.label_Explanation.Location = new System.Drawing.Point(8, 8);
			this.label_Explanation.Name = "label_Explanation";
			this.label_Explanation.Size = new System.Drawing.Size(328, 56);
			this.label_Explanation.TabIndex = 1;
			this.label_Explanation.Text = "The ParticleSystem for this .tra file has been loaded, however the TRA file conta" +
				"ins a large volume of data. In order to minimise memory expenditure you may want" +
				" to consider importing a more limited selection of trajectory entries...";
			// 
			// textBox_TraEnd
			// 
			this.textBox_TraEnd.Location = new System.Drawing.Point(136, 32);
			this.textBox_TraEnd.Name = "textBox_TraEnd";
			this.textBox_TraEnd.TabIndex = 3;
			this.textBox_TraEnd.Text = "";
			// 
			// textBox_TraStart
			// 
			this.textBox_TraStart.Location = new System.Drawing.Point(8, 32);
			this.textBox_TraStart.Name = "textBox_TraStart";
			this.textBox_TraStart.Size = new System.Drawing.Size(88, 20);
			this.textBox_TraStart.TabIndex = 2;
			this.textBox_TraStart.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(208, 23);
			this.label1.TabIndex = 4;
			this.label1.Text = "Start Tra Entry          -->     End Tra Entry";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(144, 23);
			this.label2.TabIndex = 5;
			this.label2.Text = "Reading in every Xth entry";
			// 
			// textBox_TraReadGap
			// 
			this.textBox_TraReadGap.Location = new System.Drawing.Point(8, 72);
			this.textBox_TraReadGap.Name = "textBox_TraReadGap";
			this.textBox_TraReadGap.Size = new System.Drawing.Size(88, 20);
			this.textBox_TraReadGap.TabIndex = 6;
			this.textBox_TraReadGap.Text = "";
			// 
			// button_Accept
			// 
			this.button_Accept.Location = new System.Drawing.Point(448, 8);
			this.button_Accept.Name = "button_Accept";
			this.button_Accept.Size = new System.Drawing.Size(88, 23);
			this.button_Accept.TabIndex = 7;
			this.button_Accept.Text = "Accept";
			this.button_Accept.Click += new System.EventHandler(this.button_Accept_Click);
			// 
			// groupBox_RecomendedReadin
			// 
			this.groupBox_RecomendedReadin.Controls.Add(this.textBox_TraEnd);
			this.groupBox_RecomendedReadin.Controls.Add(this.textBox_TraStart);
			this.groupBox_RecomendedReadin.Controls.Add(this.textBox_TraReadGap);
			this.groupBox_RecomendedReadin.Controls.Add(this.label1);
			this.groupBox_RecomendedReadin.Controls.Add(this.label2);
			this.groupBox_RecomendedReadin.Location = new System.Drawing.Point(288, 72);
			this.groupBox_RecomendedReadin.Name = "groupBox_RecomendedReadin";
			this.groupBox_RecomendedReadin.Size = new System.Drawing.Size(248, 104);
			this.groupBox_RecomendedReadin.TabIndex = 8;
			this.groupBox_RecomendedReadin.TabStop = false;
			this.groupBox_RecomendedReadin.Text = "Recomended Readin Params";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label_NumberOfEntries);
			this.groupBox1.Controls.Add(this.label_BlockSize);
			this.groupBox1.Controls.Add(this.label_FileSize);
			this.groupBox1.Location = new System.Drawing.Point(8, 72);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(272, 104);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "File Preview Information";
			// 
			// label_NumberOfEntries
			// 
			this.label_NumberOfEntries.Location = new System.Drawing.Point(16, 72);
			this.label_NumberOfEntries.Name = "label_NumberOfEntries";
			this.label_NumberOfEntries.Size = new System.Drawing.Size(248, 16);
			this.label_NumberOfEntries.TabIndex = 1;
			this.label_NumberOfEntries.Text = "Number of Entries : ";
			// 
			// label_BlockSize
			// 
			this.label_BlockSize.Location = new System.Drawing.Point(16, 48);
			this.label_BlockSize.Name = "label_BlockSize";
			this.label_BlockSize.Size = new System.Drawing.Size(248, 16);
			this.label_BlockSize.TabIndex = 2;
			this.label_BlockSize.Text = "Block Size (bytes) : ";
			// 
			// label_FileSize
			// 
			this.label_FileSize.Location = new System.Drawing.Point(16, 24);
			this.label_FileSize.Name = "label_FileSize";
			this.label_FileSize.Size = new System.Drawing.Size(248, 16);
			this.label_FileSize.TabIndex = 0;
			this.label_FileSize.Text = "File Size : ";
			// 
			// button_Cancel
			// 
			this.button_Cancel.Location = new System.Drawing.Point(448, 40);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(88, 23);
			this.button_Cancel.TabIndex = 10;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// TrajReadin
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(544, 181);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBox_RecomendedReadin);
			this.Controls.Add(this.button_Accept);
			this.Controls.Add(this.label_Explanation);
			this.Controls.Add(this.button_Ignore);
			this.Name = "TrajReadin";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Trajectory Readin Parameters";
			this.Load += new System.EventHandler(this.TrajReadin_Load);
			this.groupBox_RecomendedReadin.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void button_Ignore_Click(object sender, System.EventArgs e)
		{
			m_TraPreview.endPoint = m_TraPreview.numberOfEntries;
			m_TraPreview.startPoint = 0;
			m_TraPreview.skipLength = 1;
			m_Tra.LoadTrajectory();
			Close();
		}

		private void button_Accept_Click(object sender, System.EventArgs e)
		{
			try
			{
				m_TraPreview.endPoint = int.Parse(this.textBox_TraEnd.Text);
				m_TraPreview.startPoint = int.Parse(this.textBox_TraStart.Text);
				m_TraPreview.skipLength = int.Parse(this.textBox_TraReadGap.Text);
				if ( 
					m_TraPreview.startPoint < 0 
					|| 
					m_TraPreview.endPoint > m_TraPreview.numberOfEntries
					||
					m_TraPreview.skipLength < 0
					)
				{
					MessageBox.Show("Numbers in the boxes are not all valid");
				}
				else
				{
					m_Tra.LoadTrajectory();
					Close();
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show("Error occured in LoadTrajectory() : " + ex.Message );
			}
		}

		private void TrajReadin_KeyDown(object sender, KeyEventArgs e)
		{
			switch(	e.KeyCode )
			{
				case Keys.Return:
					button_Accept_Click(null, null);
					break;
				default:
					break;
			}
		}

		private void TrajReadin_Load(object sender, System.EventArgs e)
		{
		
		}

		private void button_Cancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}
	}
}
