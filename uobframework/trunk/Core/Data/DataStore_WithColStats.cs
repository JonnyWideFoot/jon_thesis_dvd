using System;
using System.Data;

namespace UoB.Core.Data
{
	/// <summary>
	/// Summary description for DataStore_WithStats.
	/// </summary>
	public abstract class DataStore_WithColStats : DataStore
	{
		protected DataTable m_StatsTable;
		private StatModes m_Modes;
		protected DataColumn m_StatTitleColumn;

		// store the datarows for fast access in this abstraction level only
		private DataRow m_DR_n = null;
		private DataRow m_DR_Average = null;
		private DataRow m_DR_SD = null;
		private DataRow m_DR_Max = null;
		private DataRow m_DR_Min = null;
		private DataRow m_DR_RMSD = null;

		public DataStore_WithColStats( string name, StatModes mode ) : base( name )
		{
			m_Modes = mode;
			m_StatsTable = new DataTable( "StatsTable" );
			m_DataSet.Tables.Add( m_StatsTable );
		}

		public DataTable StatsTable
		{
			get
			{
				return m_StatsTable;
			}
		}

		public void SaveStatsTo( string fileName, DataOutputType outFileType )
		{
			SaveTo( fileName, outFileType, m_StatsTable );
		}

		public StatModes statModes 
		{
			get
			{
				return m_Modes;
			}
		}


		protected override void PrimaryInit()
		{
			// set to an initialised blank state
			ReinitStatsTable(); // based on the StatModes given in the constructor ...
			base.PrimaryInit();			
		}

		public override void EndEditing()
		{
			base.EndEditing();
			RegenStatsTable();
		}

		private void ReinitStatsTable()
		{
			m_StatsTable.Clear();
			m_StatsTable.Columns.Clear();
			m_StatTitleColumn = new DataColumn( m_TitleColumnString, typeof( string ) );
			m_StatsTable.Columns.Add( m_StatTitleColumn );
			InitialiseStatRowTitles();
		}

		private void InitialiseStatRowTitles()
		{
			if( StatModes.n == (m_Modes & StatModes.n) )
			{
				m_DR_n = m_StatsTable.NewRow();
				m_StatsTable.Rows.Add( m_DR_n );
				m_DR_n[m_StatTitleColumn] = "Data Count";
			}
			if( StatModes.StandardDeviation == (m_Modes & StatModes.StandardDeviation) )
			{
				m_DR_SD = m_StatsTable.NewRow();
				m_StatsTable.Rows.Add( m_DR_SD );
				m_DR_SD[m_StatTitleColumn] = "Standard Deviation";
			}
			if( StatModes.RMSD == (m_Modes & StatModes.RMSD) )
			{
				m_DR_RMSD = m_StatsTable.NewRow();
				m_StatsTable.Rows.Add( m_DR_RMSD );
				m_DR_RMSD[m_StatTitleColumn] = "RMSD";
			}
			if( StatModes.Average == (m_Modes & StatModes.Average) )
			{
				m_DR_Average = m_StatsTable.NewRow();
				m_StatsTable.Rows.Add( m_DR_Average );
				m_DR_Average[m_StatTitleColumn] = "Average";
			}
			if( StatModes.MaxMin == (m_Modes & StatModes.MaxMin) )
			{
				m_DR_Min = m_StatsTable.NewRow();
				m_StatsTable.Rows.Add( m_DR_Min );
				m_DR_Min[m_StatTitleColumn] = "Min";

				m_DR_Max = m_StatsTable.NewRow();
				m_StatsTable.Rows.Add( m_DR_Max );
				m_DR_Max[m_StatTitleColumn] = "Max";
			}
		}

		private void RegenStatsTable()
		{
			ReinitStatsTable();

            int wholeTableRowCount = m_MainTable.Rows.Count;

			// column 0 is the row headers
			for( int i = 1; i < m_MainTable.Columns.Count; i++ )
			{      
				DataColumn mainSourceColumn = m_MainTable.Columns[i];
				DataColumn statColumn = new DataColumn( mainSourceColumn.ColumnName, mainSourceColumn.DataType );
				m_StatsTable.Columns.Add( statColumn );
			}

			// column 0 is the row headers
            float filledRowCount;
			for( int i = 1; i < m_MainTable.Columns.Count; i++ )
			{
                filledRowCount = 0;
                for (int j = 0; j < wholeTableRowCount; j++)
                {
                    if (m_MainTable.Rows[j][i].GetType() != typeof(DBNull) )
                    {
                        filledRowCount += 1.0f;
                    }
                }

                if (StatModes.n == (m_Modes & StatModes.n))
                {
                    m_DR_n[i] = filledRowCount;
                }

				if( StatModes.StandardDeviation == (m_Modes & StatModes.StandardDeviation) )
				{					
					// Standard Deviation
					double Sum = 0.0;
					double SumOfSqrs = 0.0; 
					float getValue = 0.0f;

                    for (int j = 0; j < wholeTableRowCount; j++)
					{
                        if (m_MainTable.Rows[j][i].GetType() != typeof(DBNull))
                        {
                            getValue = (float) m_MainTable.Rows[j][i];
                            Sum += getValue;
                            SumOfSqrs += (getValue * getValue);
                        }
					}

                    double topSum = (filledRowCount * SumOfSqrs) - (Sum * Sum);
                    m_DR_SD[i] = (float)Math.Sqrt(topSum / (filledRowCount * (filledRowCount - 1))); 
				}

				if( StatModes.RMSD == (m_Modes & StatModes.RMSD) )
				{
					double rmsd = 0.0f;
					float dataValue;
                    for (int j = 0; j < wholeTableRowCount; j++)
					{
                        if (m_MainTable.Rows[j][i].GetType() != typeof(DBNull))
                        {
                            dataValue = (float)m_MainTable.Rows[j][i];
                            rmsd += (dataValue * dataValue);
                        }
					}
                    m_DR_RMSD[i] = (float)Math.Sqrt(rmsd / filledRowCount);
				}

				if( StatModes.Average == (m_Modes & StatModes.Average) )
				{
					float average = 0.0f;
                    for (int j = 0; j < wholeTableRowCount; j++)
					{
                        if (m_MainTable.Rows[j][i].GetType() != typeof(DBNull))
                        {
                            DataRow r = m_MainTable.Rows[j];
                            average += (float)r[i];
                        }
					}

                    m_DR_Average[i] = average / filledRowCount;
				}

				if( StatModes.MaxMin == (m_Modes & StatModes.MaxMin) )
				{
					float max = float.MinValue;
					float min = float.MaxValue;
					float dataV;

                    for (int j = 0; j < wholeTableRowCount; j++)
					{
                        if (m_MainTable.Rows[j][i].GetType() != typeof(DBNull))
                        {
                            dataV = (float)m_MainTable.Rows[j][i];
                            if (max < dataV) max = dataV;
                            if (min > dataV) min = dataV;
                        }
					}

					m_DR_Max[i] = max;
					m_DR_Min[i] = min;
				}
			}
		}
	}
}
