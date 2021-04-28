using System;
using System.Data;

using UoB.Core.Primitives.Collections;

namespace UoB.Core.Data
{
	/// <summary>
	/// Summary description for DataStore_WithStats.
	/// </summary>
	public abstract class DataStore_StatTable : DataStore
	{
		/// <summary>
		/// Used to control which types of statistics are needed by this class
		/// </summary>
		private StatModes m_Modes;

		/// <summary>
		/// To be filled by the AddColumnFunction of derived classes prior to a 
		/// EvaluateInternalDataAndAddColumn() call
		/// </summary>
		protected FloatArrayList m_DataBuffer;

		// store the datarows for fast access in this abstraction level only
		private DataRow m_DR_n = null;
		private DataRow m_DR_Average = null;
		private DataRow m_DR_SD = null;
		private DataRow m_DR_Max = null;
		private DataRow m_DR_Min = null;
		private DataRow m_DR_RMSD = null;

		public DataStore_StatTable( string name, StatModes mode, int expectedEntryLoad ) : base( name )
		{
			m_Modes = mode;
			m_DataBuffer = new FloatArrayList( expectedEntryLoad );
		}

		public DataStore_StatTable( string name, StatModes mode ) : base( name )
		{
			m_Modes = mode;
			m_DataBuffer = new FloatArrayList();
		}

		protected override void Clear()
		{
			base.Clear();

			m_DR_n = null;
			m_DR_Average = null;
			m_DR_SD = null;
			m_DR_Max = null;
			m_DR_Min = null;
			m_DR_RMSD = null;
		}

		protected override void PrimaryInit()
		{
			InitialiseRowTitles(); // based on the StatModes given in the constructor ...
			base.PrimaryInit();
		}

		private void InitialiseRowTitles()
		{
			if( StatModes.n == (m_Modes & StatModes.n) )
			{
				m_DR_n = m_MainTable.NewRow();
				m_MainTable.Rows.Add( m_DR_n );
				m_DR_n[m_TitleColumn] = "Data Count";
			}
			if( StatModes.StandardDeviation == (m_Modes & StatModes.StandardDeviation) )
			{
				m_DR_SD = m_MainTable.NewRow();
				m_MainTable.Rows.Add( m_DR_SD );
				m_DR_SD[m_TitleColumn] = "Standard Deviation";
			}
			if( StatModes.RMSD == (m_Modes & StatModes.RMSD) )
			{
				m_DR_RMSD = m_MainTable.NewRow();
				m_MainTable.Rows.Add( m_DR_RMSD );
				m_DR_RMSD[m_TitleColumn] = "RMSD";
			}
			if( StatModes.Average == (m_Modes & StatModes.Average) )
			{
				m_DR_Average = m_MainTable.NewRow();
				m_MainTable.Rows.Add( m_DR_Average );
				m_DR_Average[m_TitleColumn] = "Average";
			}
			if( StatModes.MaxMin == (m_Modes & StatModes.MaxMin) )
			{
				m_DR_Min = m_MainTable.NewRow();
				m_MainTable.Rows.Add( m_DR_Min );
				m_DR_Min[m_TitleColumn] = "Min";

				m_DR_Max = m_MainTable.NewRow();
				m_MainTable.Rows.Add( m_DR_Max );
				m_DR_Max[m_TitleColumn] = "Max";
			}
		}

		public StatModes statModes 
		{
			get
			{
				return m_Modes;
			}
		}

		protected void EvaluateInternalDataAndAddColumn( string uniqueColumnName )
		{
			DataColumn c = new DataColumn( uniqueColumnName, typeof( float ) );
			m_MainTable.Columns.Add( c );

			//UoB.Core.Data.Tools.DataWriter.writeDataColumn( @"c:\bob.csv", false, "data", m_DataBuffer );

			if( StatModes.StandardDeviation == (m_Modes & StatModes.StandardDeviation) )
			{
				// Standard Deviation
				double Sum = 0.0;
				double SumOfSqrs = 0.0; 
				float getVal;

				for( int j = 0; j < m_DataBuffer.Count; j++ )
				{
					getVal = m_DataBuffer[j];
					Sum += getVal; 
					SumOfSqrs += (getVal * getVal); 							
				}

				double n = (double)m_DataBuffer.Count; 
				double topSum = (n * SumOfSqrs) - (Sum*Sum); 

				m_DR_SD[c] = (float) Math.Sqrt( topSum / (n * (n-1)) ); 
			}

			if( StatModes.n == (m_Modes & StatModes.n) )
			{
				m_DR_n[c] = (float) m_DataBuffer.Count; 
			}

			if( StatModes.RMSD == (m_Modes & StatModes.RMSD) )
			{
				double rmsd = 0.0f;
				for( int j = 0; j < m_DataBuffer.Count; j++ )
				{
					rmsd += ( m_DataBuffer[j] * m_DataBuffer[j] );
				}
				double n = (double)m_DataBuffer.Count;

				m_DR_RMSD[c] = (float) Math.Sqrt( rmsd / n );				
			}

			if( StatModes.Average == (m_Modes & StatModes.Average) )
			{
				float average = 0.0f;
				for( int j = 0; j < m_DataBuffer.Count; j++ )
				{
					average += m_DataBuffer[j];		
				}
				float n = (float)m_DataBuffer.Count;

				m_DR_Average[c] = average / n;
			}

			if( StatModes.MaxMin == (m_Modes & StatModes.MaxMin) )
			{
				float max = float.MinValue;
				float min = float.MaxValue;

				for( int j = 0; j < m_DataBuffer.Count; j++ )
				{
					if( max < m_DataBuffer[j] ) max = m_DataBuffer[j];		
					if( max > m_DataBuffer[j] ) min = m_DataBuffer[j];	
				}

				m_DR_Max[c] = max;
				m_DR_Min[c] = min;
			}
		}
	}
}
