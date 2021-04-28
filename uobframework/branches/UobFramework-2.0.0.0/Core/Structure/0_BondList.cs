using System;
using System.Collections;

namespace UoB.Core.Structure
{
	/// <summary>
	/// Summary description for BondList.
	/// </summary>
	public class BondList : IEnumerable
	{
		protected ArrayList m_Bonds;

		public BondList()
		{
			m_Bonds = new ArrayList();
		}

		public BondList( int creationWeight )
		{
			m_Bonds = new ArrayList(creationWeight);
		}

		public void addBond( Bond theBond )
		{
			m_Bonds.Add( theBond );
		}

		public int Count
		{
			get
			{
				return m_Bonds.Count;
			}
		}

		public void Clear()
		{
			m_Bonds.Clear();
		}

		public Bond this[int index] 
		{
			get 
			{
				return (Bond) m_Bonds[index];
			}
			set 
			{
				m_Bonds[index] = value;
			}
		}		
		
		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new BondListEnumerator(this);
		}

		private class BondListEnumerator : IEnumerator
		{
			private int position = -1;
			private BondList ownerB;

			public BondListEnumerator(BondList theA)
			{
				ownerB = theA;
			}

			// Declare the MoveNext method required by IEnumerator:
			public bool MoveNext()
			{
				if (position < ownerB.Count - 1)
				{
					position++;
					return true;
				}
				else
				{
					return false;
				}
			}

			// Declare the Reset method required by IEnumerator:
			public void Reset()
			{
				position = -1;
			}

			// Declare the Current property required by IEnumerator:
			public object Current
			{
				get
				{
					return ownerB[position];
				}
			}
		}

		#endregion
	}
}
