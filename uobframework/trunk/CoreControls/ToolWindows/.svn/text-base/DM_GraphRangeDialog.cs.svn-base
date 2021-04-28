using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using UoB.Core;

namespace UoB.CoreControls.ToolWindows
{
	/// <summary>
	/// Summary description for DM_GraphRangeDialog.
	/// </summary>
	public class DM_GraphRangeDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_RestoreDefault;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.TextBox text_XMin;
		private System.Windows.Forms.TextBox text_XMax;
		private System.Windows.Forms.TextBox text_YMax;
		private System.Windows.Forms.TextBox text_YMin;
		private System.Windows.Forms.Button button_XMin;
		private System.Windows.Forms.Button button_XMax;
		private System.Windows.Forms.Button button_YMin;
		private System.Windows.Forms.Button button_YMax;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.CheckBox check_DataBorder;
		private System.Windows.Forms.Button button_Apply;

		private DM_GraphBounds m_Bounds;
		UpdateEvent m_UpdateCallBack;

		public DM_GraphRangeDialog( DM_GraphBounds bounds, UpdateEvent updateCallBack )
		{
			InitializeComponent();

            m_Bounds = bounds;
			if( m_Bounds == null )
			{
				throw new ArgumentNullException("The argument bounds was null");
			}

			m_UpdateCallBack = updateCallBack;
			if( m_UpdateCallBack == null )
			{
				throw new ArgumentNullException("The argument updateCallBack was null");
			}

			SetBoxes();
		}

		private void SetBoxes()
		{
			SetBoxXMin();
			SetBoxXMax();
			SetBoxYMin();
			SetBoxYMax();
		}

		private void SetBoxXMin()
		{
			text_XMin.Text = m_Bounds.XMin_Data.ToString();
		}

		private void SetBoxXMax()
		{
			text_XMax.Text = m_Bounds.XMax_Data.ToString();
		}

		private void SetBoxYMin()
		{
			text_YMin.Text = m_Bounds.YMin_Data.ToString();
		}

		private void SetBoxYMax()
		{
			text_YMax.Text = m_Bounds.YMax_Data.ToString();
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
			this.button_RestoreDefault = new System.Windows.Forms.Button();
			this.button_OK = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.text_XMin = new System.Windows.Forms.TextBox();
			this.text_XMax = new System.Windows.Forms.TextBox();
			this.text_YMax = new System.Windows.Forms.TextBox();
			this.text_YMin = new System.Windows.Forms.TextBox();
			this.button_XMin = new System.Windows.Forms.Button();
			this.button_XMax = new System.Windows.Forms.Button();
			this.button_YMin = new System.Windows.Forms.Button();
			this.button_YMax = new System.Windows.Forms.Button();
			this.check_DataBorder = new System.Windows.Forms.CheckBox();
			this.button_Apply = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button_RestoreDefault
			// 
			this.button_RestoreDefault.Location = new System.Drawing.Point(96, 136);
			this.button_RestoreDefault.Name = "button_RestoreDefault";
			this.button_RestoreDefault.Size = new System.Drawing.Size(80, 23);
			this.button_RestoreDefault.TabIndex = 0;
			this.button_RestoreDefault.Text = "Set To &Auto";
			this.button_RestoreDefault.Click += new System.EventHandler(this.button_RestoreDefault_Click);
			// 
			// button_OK
			// 
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.Location = new System.Drawing.Point(8, 168);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(80, 23);
			this.button_OK.TabIndex = 1;
			this.button_OK.Text = "&OK";
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// button_Cancel
			// 
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(96, 168);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(80, 23);
			this.button_Cancel.TabIndex = 2;
			this.button_Cancel.Text = "&Cancel";
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// text_XMin
			// 
			this.text_XMin.Location = new System.Drawing.Point(8, 32);
			this.text_XMin.Name = "text_XMin";
			this.text_XMin.Size = new System.Drawing.Size(80, 20);
			this.text_XMin.TabIndex = 3;
			this.text_XMin.Text = "";
			// 
			// text_XMax
			// 
			this.text_XMax.Location = new System.Drawing.Point(8, 56);
			this.text_XMax.Name = "text_XMax";
			this.text_XMax.Size = new System.Drawing.Size(80, 20);
			this.text_XMax.TabIndex = 4;
			this.text_XMax.Text = "";
			// 
			// text_YMax
			// 
			this.text_YMax.Location = new System.Drawing.Point(8, 104);
			this.text_YMax.Name = "text_YMax";
			this.text_YMax.Size = new System.Drawing.Size(80, 20);
			this.text_YMax.TabIndex = 6;
			this.text_YMax.Text = "";
			// 
			// text_YMin
			// 
			this.text_YMin.Location = new System.Drawing.Point(8, 80);
			this.text_YMin.Name = "text_YMin";
			this.text_YMin.Size = new System.Drawing.Size(80, 20);
			this.text_YMin.TabIndex = 5;
			this.text_YMin.Text = "";
			// 
			// button_XMin
			// 
			this.button_XMin.Location = new System.Drawing.Point(96, 32);
			this.button_XMin.Name = "button_XMin";
			this.button_XMin.Size = new System.Drawing.Size(80, 23);
			this.button_XMin.TabIndex = 7;
			this.button_XMin.Text = "Reset XMin";
			this.button_XMin.Click += new System.EventHandler(this.button_XMin_Click);
			// 
			// button_XMax
			// 
			this.button_XMax.Location = new System.Drawing.Point(96, 56);
			this.button_XMax.Name = "button_XMax";
			this.button_XMax.Size = new System.Drawing.Size(80, 23);
			this.button_XMax.TabIndex = 8;
			this.button_XMax.Text = "Reset XMax";
			this.button_XMax.Click += new System.EventHandler(this.button_XMax_Click);
			// 
			// button_YMin
			// 
			this.button_YMin.Location = new System.Drawing.Point(96, 80);
			this.button_YMin.Name = "button_YMin";
			this.button_YMin.Size = new System.Drawing.Size(80, 23);
			this.button_YMin.TabIndex = 9;
			this.button_YMin.Text = "Reset YMin";
			this.button_YMin.Click += new System.EventHandler(this.button_YMin_Click);
			// 
			// button_YMax
			// 
			this.button_YMax.Location = new System.Drawing.Point(96, 104);
			this.button_YMax.Name = "button_YMax";
			this.button_YMax.Size = new System.Drawing.Size(80, 23);
			this.button_YMax.TabIndex = 10;
			this.button_YMax.Text = "Reset YMax";
			this.button_YMax.Click += new System.EventHandler(this.button_YMax_Click);
			// 
			// check_DataBorder
			// 
			this.check_DataBorder.Checked = true;
			this.check_DataBorder.CheckState = System.Windows.Forms.CheckState.Checked;
			this.check_DataBorder.Location = new System.Drawing.Point(8, 8);
			this.check_DataBorder.Name = "check_DataBorder";
			this.check_DataBorder.Size = new System.Drawing.Size(168, 24);
			this.check_DataBorder.TabIndex = 11;
			this.check_DataBorder.Text = "Data Border";
			this.check_DataBorder.CheckedChanged += new System.EventHandler(this.check_DataBorder_CheckedChanged);
			// 
			// button_Apply
			// 
			this.button_Apply.Location = new System.Drawing.Point(8, 136);
			this.button_Apply.Name = "button_Apply";
			this.button_Apply.Size = new System.Drawing.Size(80, 23);
			this.button_Apply.TabIndex = 12;
			this.button_Apply.Text = "&Apply";
			this.button_Apply.Click += new System.EventHandler(this.button_Apply_Click);
			// 
			// DM_GraphRangeDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(184, 197);
			this.Controls.Add(this.button_Apply);
			this.Controls.Add(this.button_RestoreDefault);
			this.Controls.Add(this.check_DataBorder);
			this.Controls.Add(this.button_YMax);
			this.Controls.Add(this.button_YMin);
			this.Controls.Add(this.button_XMax);
			this.Controls.Add(this.button_XMin);
			this.Controls.Add(this.text_YMax);
			this.Controls.Add(this.text_YMin);
			this.Controls.Add(this.text_XMax);
			this.Controls.Add(this.text_XMin);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "DM_GraphRangeDialog";
			this.Text = "Set Graph Range...";
			this.ResumeLayout(false);

		}
		#endregion

		private void button_RestoreDefault_Click(object sender, System.EventArgs e)
		{
			m_Bounds.SetValuesToDefaults();
			m_UpdateCallBack();
		}

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			if( ApplyChanges() )
			{
				Close();	
			}
		}

		private void button_Apply_Click(object sender, System.EventArgs e)
		{
			if( ApplyChanges() )
			{
				m_UpdateCallBack();			
			}
		}

		private bool ApplyChanges()
		{
			float xMin = 0.0f;
			float xMax = 0.0f;
			float yMin = 0.0f;
			float yMax = 0.0f;
			
			try
			{
				xMin = float.Parse( text_XMin.Text );
			}
			catch
			{
				MessageBox.Show(this,
					"xMin is not a valid number",
					"Invalid number",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error );
				return false;
			}

			try
			{
				xMax = float.Parse( text_XMax.Text );
			}
			catch
			{
				MessageBox.Show(this,
					"xMax is not a valid number",
					"Invalid number",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error );
				return false;
			}

			try
			{
				yMin = float.Parse( text_YMin.Text );
			}
			catch
			{
				MessageBox.Show(this,
					"yMin is not a valid number",
					"Invalid number",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error );
				return false;
			}

			try
			{
				yMax = float.Parse( text_YMax.Text );
			}
			catch
			{
				MessageBox.Show(this,
					"yMax is not a valid number",
					"Invalid number",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error );
				return false;
			}	
	
			m_Bounds.DataBorder = check_DataBorder.Checked;
			m_Bounds.SetDataBounds( xMin, xMax, yMin, yMax );
			return true;
		}

		private void button_Cancel_Click(object sender, System.EventArgs e)
		{
			Close();		
		}

		private void button_XMin_Click(object sender, System.EventArgs e)
		{
			SetBoxXMin();
		}

		private void button_XMax_Click(object sender, System.EventArgs e)
		{
			SetBoxXMax();
		}

		private void button_YMin_Click(object sender, System.EventArgs e)
		{
			SetBoxYMin();
		}

		private void button_YMax_Click(object sender, System.EventArgs e)
		{
			SetBoxYMax();
		}

		private void check_DataBorder_CheckedChanged(object sender, System.EventArgs e)
		{
			m_Bounds.DataBorder = check_DataBorder.Checked;
			m_UpdateCallBack();
		}
	}
}
