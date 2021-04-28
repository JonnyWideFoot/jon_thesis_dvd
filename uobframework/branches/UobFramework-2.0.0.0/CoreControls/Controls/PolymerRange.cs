using System;

using UoB.Core.Primitives;
using UoB.Core.Structure;

namespace UoB.CoreControls.Controls
{
	/// <summary>
	/// Summary description for PolymerRange.
	/// </summary>
	public class PolymerRange : RangeView
	{
		private PSMolContainer m_Polymer;

		public PolymerRange() : base()
		{
			Polymer = null;
		}

		public PolymerRange( PSMolContainer poly ) : base()
		{
			Polymer = poly;
		}

		public PSMolContainer Polymer
		{
			get
			{
				return m_Polymer;
			}
			set
			{
				m_Polymer = value;
				if( m_Polymer != null )
				{
					SetRange( 0, m_Polymer.Count, 0, m_Polymer.Count );
				}
				else
				{
					SetRange(0,1,0,1);
				}
				SetControlState();
			}
		}

		private void SetControlState()
		{
			for( int i = 0; i < Controls.Count; i++ )
			{
				Controls[i].Enabled = ( m_Polymer != null );
			}
		}

		public int StartID
		{
			get
			{
				return m_Range.RangeStart;
			}
		}

		public int EndID
		{
			get
			{
				return m_Range.RangeStart + m_Range.RangeLength -1;
			}
		}

		public int Length 
		{
			get
			{
				return m_Range.RangeLength;
			}
		}

		protected override void SetLabels()
		{
			if( m_Polymer != null )
			{
				label1.Text = "Start : " + m_Polymer[StartID].ToString();
				label2.Text = "End :  " + m_Polymer[EndID].ToString(); 
			}
			else
			{
				label1.Text = "Start : No Molecule";
				label2.Text = "End  : No Molecule";
			}
		}

	}
}
