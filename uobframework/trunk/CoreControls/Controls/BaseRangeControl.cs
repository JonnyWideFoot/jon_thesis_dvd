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
	/// Summary description for RangeControl.
	/// </summary>
	public class BaseRangeControl : System.Windows.Forms.UserControl
	{
		protected System.ComponentModel.Container components = null;

		protected RangeChange m_RangeUpdated;
		protected IntRange_EventFire m_Range = null;

		public BaseRangeControl()
		{
			InitializeComponent();			
			m_RangeUpdated = new RangeChange( UpdateControl );
		}

		public IntRange_EventFire Range
		{
			get
			{
				return m_Range;
			}
			set
			{
				if( m_Range != null )
				{
					// unsubscribe
					m_Range.RangeUpdated -= m_RangeUpdated;
				}
				m_Range = null;
				if( value != null )
				{
					m_Range = value;
					m_Range.RangeUpdated += m_RangeUpdated;
					Enabled = true;
				}
				else
				{
					Enabled = false;
				}
				UpdateDisplayFromRange();
			}
		}

		protected virtual void UpdateDisplayFromRange()
		{
		}

		protected void UpdateControl( object Sender, IntRange_EventFire range )
		{
			if( Sender != this )
			{
				UpdateDisplayFromRange();
			}
			else
			{
				Invalidate();
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
			// 
			// RangeControl
			// 
			this.Name = "RangeControl";
			this.Size = new System.Drawing.Size(56, 56);

		}
		#endregion
	}
}
