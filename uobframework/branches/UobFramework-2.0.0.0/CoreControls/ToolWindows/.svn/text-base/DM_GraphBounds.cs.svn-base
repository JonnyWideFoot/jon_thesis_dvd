using System;

using UoB.Core;
using UoB.Core.Tools;
using UoB.Core.Data;
using UoB.Core.Data.Graphing;

namespace UoB.CoreControls.ToolWindows
{
	/// <summary>
	/// Summary description for DM_GraphBounds.
	/// </summary>
	public class DM_GraphBounds
	{
		private Graph m_Graph;
		private bool m_DataBorder = true;
		private float m_BorderFraction = 0.05f;
		private float m_XMin;
		private float m_XMax;
		private float m_YMin;
		private float m_YMax;
		private float m_XMin_Default;
		private float m_XMax_Default;
		private float m_YMin_Default;
		private float m_YMax_Default;

		public DM_GraphBounds()
		{
			graph = null; // initialise all to null values
		}

		public Graph graph
		{
			get
			{
				return m_Graph;
			}
			set
			{
				m_Graph = value;
				if( m_Graph != null )
				{
					GetDefaultsFromData();
				}
				else
				{
					m_XMin_Default = 0.0f;
					m_XMax_Default = 0.0f;
					m_YMin_Default = 0.0f;
					m_YMax_Default = 0.0f;
				}
				SetValuesToDefaults();
			}
		}

		private void GetDefaultsFromData()
		{
			float dataV = 0.0f;

			// First: XAxis
			m_XMin_Default = float.MaxValue;
			m_XMax_Default = float.MinValue;
			for( int i = 0; i < m_Graph.XData.Length; i++ )
			{
				dataV = m_Graph.XData[i];				
				if( m_XMax_Default < dataV ) { m_XMax_Default = dataV; }
				if( m_XMin_Default > dataV ) { m_XMin_Default = dataV; }
			}

			// Then: YAxis
			m_YMin_Default = float.MaxValue;
			m_YMax_Default = float.MinValue;
			for( int i = 0; i < m_Graph.YData.Length; i++ )
			{
				dataV = m_Graph.YData[i];				
				if( m_YMax_Default < dataV ) { m_YMax_Default = dataV; }
				if( m_YMin_Default > dataV ) { m_YMin_Default = dataV; }
			}
		}

		public void SetValuesToDefaults()
		{
			m_XMin = m_XMin_Default;
			m_XMax = m_XMax_Default;
			m_YMin = m_YMin_Default;
			m_YMax = m_YMax_Default;
		}

		public bool DataBorder
		{
			get
			{
				return m_DataBorder;
			}
			set
			{
				m_DataBorder = value;
			}
		}

		public void SetDataBounds( float xMin, float xMax, float yMin, float yMax )
		{
			m_XMin = xMin;
			m_XMax = xMax;
			m_YMin = yMin;
			m_YMax = yMax;
		}

		public float BorderFraction
		{
			get
			{
				return m_BorderFraction;
			}
			set
			{
				if( value >= 0.0f && value <= 1.00 )
				{
					m_BorderFraction = value;
				}
			}
		}

		public float XMin_Data
		{
			get
			{
				return m_XMin;
			}
		}

		public float XMax_Data
		{
			get
			{
				return m_XMax;
			}
		}

		public float YMin_Data
		{
			get
			{
				return m_YMin;
			}
		}

		public float YMax_Data
		{
			get
			{
				return m_YMax;
			}
		}

		public float XMin_Plot
		{
			get
			{
				if( this.m_DataBorder )
				{
					float minMaxSeparation = m_XMax - m_XMin; // get the difference
					minMaxSeparation *= m_BorderFraction;
					return m_XMin - minMaxSeparation;	
				}
				else
				{
					return m_XMin;
				}
			}
		}

		public float XMax_Plot
		{
			get
			{
				if( this.m_DataBorder )
				{
					float minMaxSeparation = m_XMax - m_XMin; // get the difference
					minMaxSeparation *= m_BorderFraction;
					return m_XMax + minMaxSeparation;	
				}
				else
				{
					return m_XMax;
				}
			}
		}

		public float YMin_Plot
		{
			get
			{
				if( this.m_DataBorder )
				{
					float minMaxSeparation = m_YMax - m_YMin; // get the difference
					minMaxSeparation *= m_BorderFraction;
					return m_YMin - minMaxSeparation;	
				}
				else
				{
					return m_YMin;
				}
			}
		}

		public float YMax_Plot
		{
			get
			{
				if( this.m_DataBorder )
				{
					float minMaxSeparation = m_YMax - m_YMin; // get the difference
					minMaxSeparation *= m_BorderFraction;
					return m_YMax + minMaxSeparation;	
				}
				else
				{
					return m_YMax;
				}
			}
		}
	}
}
