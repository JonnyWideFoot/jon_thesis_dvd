using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace UoB.CoreControls.Controls
{
	/// <summary>
	/// Summary description for NumericRange.
	/// </summary>
	public class NumericRange : BaseRangeControl
	{
		private bool m_Editing = false;
		private System.Windows.Forms.NumericUpDown num_Start;
		private System.Windows.Forms.NumericUpDown num_End;

		public NumericRange()
		{
			InitializeComponent();
		}

		protected override void UpdateDisplayFromRange()
		{
			if( Enabled && m_Range != null )
			{
				m_Editing = true;
				num_Start.Minimum = m_Range.GlobalStart;
				num_Start.Maximum = m_Range.GlobalLength - m_Range.GlobalStart  - 1;
				num_End.Minimum = m_Range.GlobalStart;
				num_End.Maximum = m_Range.GlobalLength - m_Range.GlobalStart - 1;
				num_Start.Value = m_Range.RangeStart;
				num_End.Value = m_Range.RangeStart + m_Range.RangeLength - 1;
				m_Editing = false;
			}
			else
			{
				m_Editing = true;
				num_Start.Minimum = 0;
				num_Start.Maximum = 0;
				num_End.Minimum = 0;
				num_End.Maximum = 0;
				num_Start.Value = 0;
				num_End.Value = 0;
				m_Editing = false;
			}
		}

		private void num_ValueChanged(object sender, System.EventArgs e)
		{
			if( !m_Editing )
			{
				int start = (int)num_Start.Value;
				int end = (int)num_End.Value;
				m_Range.SetRange( this, start, end - start + 1 );
			}		
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.num_Start = new System.Windows.Forms.NumericUpDown();
			this.num_End = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.num_Start)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.num_End)).BeginInit();
			this.SuspendLayout();
			// 
			// num_Start
			// 
			this.num_Start.Location = new System.Drawing.Point(0, 0);
			this.num_Start.Name = "num_Start";
			this.num_Start.Size = new System.Drawing.Size(72, 20);
			this.num_Start.TabIndex = 0;
			this.num_Start.ValueChanged += new System.EventHandler(this.num_ValueChanged);
			// 
			// num_End
			// 
			this.num_End.Location = new System.Drawing.Point(80, 0);
			this.num_End.Name = "num_End";
			this.num_End.Size = new System.Drawing.Size(72, 20);
			this.num_End.TabIndex = 1;
			this.num_End.ValueChanged += new System.EventHandler(this.num_ValueChanged);
			// 
			// NumericRange
			// 
			this.Controls.Add(this.num_End);
			this.Controls.Add(this.num_Start);
			this.Name = "NumericRange";
			this.Size = new System.Drawing.Size(152, 24);
			((System.ComponentModel.ISupportInitialize)(this.num_Start)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.num_End)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
	}
}
