using System;
using System.IO;

namespace UoB.Core.Data.IO
{
	/// <summary>
	/// Summary description for ResultGrid.
	/// </summary>
	public sealed class ResultGrid
	{
		private Array m_Array = null;
		private int[] m_Lengths = null;
		private float[][] m_DimensionValues = null;

		public ResultGrid( params float[][] dimensionValues )
		{
			m_DimensionValues = dimensionValues;
			m_Lengths = new int[ dimensionValues.GetUpperBound(0) + 1 ];
			for( int i = 0; i < m_Lengths.Length; i++ )
			{
				m_Lengths[i] = dimensionValues[i].GetUpperBound(0) + 1;
			}
			m_Array = Array.CreateInstance( typeof( float ), m_Lengths );
		}

		private int[] GetIndexers( float[] dimensionValues )
		{
			if( m_Lengths.Length != dimensionValues.Length )
			{
				throw new Exception("The wrong number of dimensions was given");
			}
			int[] indexers = new int[ m_Lengths.Length ];
			for( int i = 0; i < m_Lengths.Length; i++ )
			{
				indexers[i] = GetIndexer( i, dimensionValues );
			}
			return indexers;
		}

		private int GetIndexer( int index, float[] dimensionValues )
		{
			int countToBound = m_DimensionValues[index].GetUpperBound(0);
			for( int j = 0; j <= countToBound; j++ )
			{
				if( dimensionValues[index] == m_DimensionValues[index][j] )
				{
					return j;
				}
			}
			throw new Exception("Cannot find the value for dimension indexer : " + index.ToString() );		 
		}

		public void SetCellValue( float[] dimensionValues, float cellValue )
		{
			int[] indexers = GetIndexers( dimensionValues );
			m_Array.SetValue( cellValue,indexers );
		}

		public void Print2DCSV( string fileName, float[] fixDimValues, int dimA, int dimB )
		{
			StreamWriter rw = new StreamWriter( fileName );
				Print2DCSV( rw, fixDimValues, dimA, dimB );
			rw.Close();
		}

		public void Print2DCSV( StreamWriter rw, float[] fixDimValues, int dimA, int dimB )
		{
			// fixDimValues == the other dimesnions that we are not printing
			if( fixDimValues.Length != m_Lengths.Length )
			{
				throw new Exception("Dim fixed values is not the correct length");
			}
			int[] indexers = new int[ m_Lengths.Length ];

			// fill the standard indexers
			for( int i = 0; i < m_Lengths.Length; i++ )
			{
				if( dimA == i ||
					dimB == i )
				{
					indexers[i] = -1; // null
					continue;
				}
				else
				{
					indexers[i] = GetIndexer( i, fixDimValues );
				}
			} 

			// now increment through the other values and print our table
			int dimABound = m_Array.GetUpperBound( dimA );
			int dimBBound = m_Array.GetUpperBound( dimB );

			float[] dimAIncrements = m_DimensionValues[ dimA ];
			float[] dimBIncrements = m_DimensionValues[ dimB ];

			rw.Write("-,");
			for( int i = 0; i < dimBIncrements.Length; i++ )
			{
				rw.Write( dimBIncrements[i] );
				if( i != dimBIncrements.Length - 1)
				{
					rw.Write(',');
				}
				else
				{
					rw.WriteLine();
				}
			}
			
			for( int i = 0; i <= dimABound; i++ )
			{
				rw.Write( dimAIncrements[i] );
				rw.Write(',');
				for( int j = 0; j <= dimBBound; j++ )
				{
					indexers[ dimA ] = i;
					indexers[ dimB ] = j;
					rw.Write( m_Array.GetValue( indexers ) );

					if( j != dimBIncrements.Length - 1)
					{
						rw.Write(',');
					}				
					else
					{
						rw.WriteLine();
					}
				}
			}
		}
	}
}
