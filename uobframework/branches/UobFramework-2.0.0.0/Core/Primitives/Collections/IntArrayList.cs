using System;
using System.Collections;

namespace UoB.Core.Primitives.Collections
{
	/// <summary>
	/// Summary description for intList.
	/// </summary>
	public class IntArrayList : IEnumerable
	{
		private ArrayList m_Ints;

		public IntArrayList()
		{
			m_Ints = new ArrayList();
		}

		public void addInt( int theInt )
		{
			m_Ints.Add( theInt );
		}

		public int Count
		{
			get
			{
				return m_Ints.Count;
			}
		}

		public bool Contains( int f )
		{
			return m_Ints.Contains( f );
		}

		public void Clear()
		{
			m_Ints.Clear();
		}

		public int this[int index] 
		{
			get 
			{
				return (int) m_Ints[index];
			}
			set 
			{
				m_Ints[index] = value;
			}
		}		
		
		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new intListEnumerator(this);
		}

		private class intListEnumerator : IEnumerator
		{
			private int position = -1;
			private IntArrayList ownerList;

			public intListEnumerator(IntArrayList parent)
			{
				ownerList = parent;
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
