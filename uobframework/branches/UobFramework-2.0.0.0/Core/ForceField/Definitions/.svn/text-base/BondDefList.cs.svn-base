using System;
using System.Collections;

namespace UoB.Core.ForceField.Definitions
{
	/// <summary>
	/// Summary description for BondDefListList.
	/// </summary>
	public class BondDefList: IEnumerable
	{
		private ArrayList m_BondDefLists;

		public BondDefList()
		{
			m_BondDefLists = new ArrayList();
		}

		public void addBondDef( BondDefinition theBondDef )
		{
			m_BondDefLists.Add( theBondDef );
		}

		public int Count
		{
			get
			{
				return m_BondDefLists.Count;
			}
		}

		public bool Contains( BondDefList a )
		{
			return m_BondDefLists.Contains( a );
		}

		public void Clear()
		{
			m_BondDefLists.Clear();
		}

		public BondDefList this[int index] 
		{
			get 
			{
				return (BondDefList) m_BondDefLists[index];
			}
			set 
			{
				m_BondDefLists[index] = value;
			}
		}		
		
		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new BondDefListListEnumerator(this);
		}

		private class BondDefListListEnumerator : IEnumerator
		{
			private int position = -1;
			private BondDefList ownerList;

			public BondDefListListEnumerator(BondDefList theA)
			{
				ownerList = theA;
			}

			// Declare the MoveNext method required by IEnumerator:
			public bool MoveNext()
			{
				if (position < ownerList.Count - 1)
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
					return ownerList[position];
				}
			}
		}

		#endregion
	}
}
