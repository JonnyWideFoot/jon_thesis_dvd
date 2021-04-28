using System;
using System.Collections;

using UoB.Core.Structure;

namespace UoB.Core.Structure.Alignment
{
	/// <summary>
	/// Summary description for ModelList.
	/// </summary>
	public class ModelList
	{
		protected ArrayList m_Models;
		protected PSMolContainer m_Mol1;
		protected PSMolContainer m_Mol2;

		public ModelList( PSMolContainer mol1, PSMolContainer mol2 )
		{
			m_Mol1 = mol1;
			m_Mol2 = mol2;
			m_Models = new ArrayList();
		}

		public PSMolContainer Mol1
		{
			get
			{
				return m_Mol1;
			}
		}

		public PSMolContainer Mol2
		{
			get
			{
				return m_Mol2;
			}
		}

		public void AddModel( Model m )
		{
			if( m.Equivalencies.Length != m_Mol1.Count )
			{
				throw new Exception("Model seems to be corrupt, the Equiv length is not the same as internal molecule 1's length");
			}
			m_Models.Add( m );
		}

		public void Clear()
		{
			m_Models.Clear();
		}

		public void Sort()
		{
			m_Models.Sort( m_Comparer );
		}

		public void RemoveAt( int index )
		{
			m_Models.RemoveAt( index );
		}

		public void RemoveRange( int index, int count )
		{
			m_Models.RemoveRange( index, count );
		}

		public int ModelCount
		{
			get
			{
				return m_Models.Count;
			}
		}

		public Model this[ int i ]
		{
			get
			{
				try
				{
					return (Model) m_Models[i];
				}
				catch
				{
					return null;
				}
			}
		}

		public void ReduceModelCount( int modelCount )
		{
			if( m_Models.Count > modelCount )
			{
				// remove the dead wood
				m_Models.RemoveRange( modelCount, m_Models.Count - modelCount );
			}
		}

		// methods to work on the array
		public void EradicateIdentical( int minimumMatches )
		{
			// first sort the list, making erradication more efficient
			m_Models.Sort( m_Comparer );

			int countTo = m_Models.Count;
			int i = 0;
			while( i < countTo )
			{
				Model f = (Model) m_Models[i];
				if( f.numberEquivalencies < minimumMatches ) // we dont want seeds that are only the original 'n' aligned residues
				{
					m_Models.RemoveAt(i);
					countTo--;
				}
				else
				{
					for( int j = i + 1; j < countTo; /* no increment here, done below as we are removing items from the array */ )
					{
						Model f2 = (Model) m_Models[j];
						if( EquivListsAreIdentical( f.Equivalencies, f2.Equivalencies ) )
						{
							m_Models.RemoveAt(j);
							countTo--;
						}
						else
						{
							j++; // only need to increment if we havent deleted one
						}
					}
					i++;
				}
			}
		}

		private bool EquivListsAreIdentical( int[] a, int[] b )
		{
			for( int i = 0; i < a.Length; i++ )
			{
				if( a[i] != b[i] )
				{
					return false;
				}
			}
			return true;
		}

		private static ModelComparer m_Comparer = new ModelComparer();
		private class ModelComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				try
				{
					Model fX = (Model)x;
					Model fY = (Model)y;
					int alignedCountX = fX.numberEquivalencies;
					int alignedCountY = fY.numberEquivalencies;
					if( alignedCountX > alignedCountY ) // if fX is BETTER
					{
						return -1;
					}
					else if ( alignedCountX < alignedCountY )
					{
						return 1;
					}
					else if ( fX.CRMS < fY.CRMS ) // if fX is BETTER i.e. low dRMS
					{
						return -1;
					}
					else if ( fX.CRMS > fY.CRMS )
					{
						return 1;
					}
					else
					{
						return 0;
					}
				}
				catch
				{
					return 0;
				}
			}
		}



	}
}
