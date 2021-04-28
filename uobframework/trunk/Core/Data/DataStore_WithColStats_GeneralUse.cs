using System;
using System.Data;
using System.IO;

namespace UoB.Core.Data
{
	/// <summary>
	/// Summary description for DataStore_WithColStats_GeneralUse.
	/// </summary>
	public class DataStore_WithColStats_GeneralUse : DataStore_WithColStats
	{
		public DataStore_WithColStats_GeneralUse( string name, StatModes mode ) 
			: base( name, mode )
		{
		}

		public void AddColumn( string colName, float[] data )
		{
			DataColumn c = new DataColumn( colName, typeof(float) );
			m_MainTable.Columns.Add( c );
			for( int j = 0; j < data.Length; j++ )
			{
				DataRow r;
				if( j >= m_MainTable.Rows.Count )
				{
					r = m_MainTable.NewRow();
					m_MainTable.Rows.Add(r);
				}
				else
				{
					r = m_MainTable.Rows[j];
				}
				r[c] = data[j];
			}
		}

		public void AddColumn( StreamReader floatTextStream, string columnName, StreamImportMode mode )
		{
			switch( mode )
			{
				case StreamImportMode.SingleFloatTextLines:
					try
					{
						DataColumn c = new DataColumn( columnName, typeof(float) );
						m_MainTable.Columns.Add( c );
						string line;
						int j = 0;
						while( null != ( line = floatTextStream.ReadLine() ) )
						{
							if( line.Length == 0 ) continue;
							DataRow r;
							if( j >= m_MainTable.Rows.Count )
							{
								r = m_MainTable.NewRow();
								m_MainTable.Rows.Add(r);
							}
							else
							{
								r = m_MainTable.Rows[j];
							}
							j++;
							r[c] = float.Parse( line );
						}
					}
					catch( Exception ex )
					{
						throw new Exception("File data parsing failed", ex );
					}
					break;
				default:
					throw new NotImplementedException("StreamImportMode is not implemented");
			}
		}
	}
}
