using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using UoB.Core;
using UoB.Core.Primitives;

namespace UoB.CoreControls.Controls
{
	/// <summary>
	/// Summary description for NumericRange.
	/// </summary>
	public class RangeView : System.Windows.Forms.UserControl
	{
		private System.ComponentModel.Container components = null;
		protected System.Windows.Forms.Label label1;
		protected System.Windows.Forms.Label label2;
		private UoB.CoreControls.Controls.GraphicRangeSelection graphicRangeSelection;
		private UoB.CoreControls.Controls.NumericRange numericRange;

		protected IntRange_EventFire m_Range = new IntRange_EventFire();

		public event RangeChange RangeUpdated;

		public RangeView()
		{
			InitializeComponent();
			m_Range = new IntRange_EventFire();
			numericRange.Range = m_Range;
			graphicRangeSelection.Range = m_Range;
			m_Range.RangeUpdated += new RangeChange( onInternalRangeChange );
		}

		private void onInternalRangeChange( object sender, IntRange_EventFire range )
		{
			SetLabels();
			// if the sender is the internal range then the call that this control has updated should not propogate
			if( RangeUpdated != null && sender != m_Range )
			{
				RangeUpdated( this, m_Range );
			}
		}

		protected virtual void SetLabels()
		{
			label1.Text = "Start  : " + m_Range.RangeStart.ToString();
			label2.Text = "length : " + m_Range.RangeLength.ToString();
		}

		public void SetRange( int rangeStart, int rangeLength, int globalStart, int globalLength )
		{
			m_Range.SetRange( this, rangeStart, rangeLength, globalStart, globalLength );
		}

		public void SetRange( int rangeStart, int rangeLength )
		{
			m_Range.SetRange( this, rangeStart, rangeLength );
		}

		public IntRange_EventFire Range
		{
			get
			{
				return m_Range;
			}
		}

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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.graphicRangeSelection = new UoB.CoreControls.Controls.GraphicRangeSelection();
			this.numericRange = new UoB.CoreControls.Controls.NumericRange();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(152, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = "label1";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(0, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(152, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "label2";
			// 
			// graphicRangeSelection
			// 
			this.graphicRangeSelection.AllowUserBarSelect = true;
			this.graphicRangeSelection.BackColor = System.Drawing.Color.Red;
			this.graphicRangeSelection.Enabled = false;
			this.graphicRangeSelection.Location = new System.Drawing.Point(4, 4);
			this.graphicRangeSelection.Name = "graphicRangeSelection";
			this.graphicRangeSelection.Range = null;
			this.graphicRangeSelection.Size = new System.Drawing.Size(144, 12);
			this.graphicRangeSelection.TabIndex = 5;
			// 
			// numericRange
			// 
			this.numericRange.Location = new System.Drawing.Point(2, 18);
			this.numericRange.Name = "numericRange";
			this.numericRange.Range = null;
			this.numericRange.Size = new System.Drawing.Size(150, 24);
			this.numericRange.TabIndex = 6;
			// 
			// RangeView
			// 
			this.Controls.Add(this.numericRange);
			this.Controls.Add(this.graphicRangeSelection);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "RangeView";
			this.Size = new System.Drawing.Size(152, 72);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
