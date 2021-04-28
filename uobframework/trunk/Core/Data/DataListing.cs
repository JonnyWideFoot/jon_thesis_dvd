using System;

namespace UoB.Core.Data
{
	/// <summary>
	/// Summary description for DataObject.
	/// </summary>
	public class DataListing
	{
		private string m_Name;
		private float[] m_Data;

		public DataListing(string name, float[] data)
		{
			m_Name = name;
			m_Data = data;
		}

		public float[] Data
		{
			get
			{
				return m_Data;
			}
		}
		public string Name
		{
			get
			{
				return m_Name;
			}
		}
	}
}
