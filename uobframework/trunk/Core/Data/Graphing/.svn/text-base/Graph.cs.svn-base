using System;

namespace UoB.Core.Data.Graphing
{
	/// <summary>
	/// Summary description for Graph.
	/// </summary>
	public class Graph
	{
		private DataListing m_XData;
		private DataListing m_YData;
		public string GraphTitle;

		public float[] XData
		{
			get
			{
				return m_XData.Data;
			}
		}

		public float[] YData
		{
			get
			{
				return m_YData.Data;
			}
		}

		public Graph(string title, DataListing xData, DataListing yData)
		{
			GraphTitle = title;
			m_XData = xData;
			m_YData = yData;
		}

		public string xAxisLabel
		{
			get
			{
				return m_XData.Name;
			}
		}
	
		public string yAxisLabel
		{
			get
			{
				return m_YData.Name;
			}
		}

	}
}
