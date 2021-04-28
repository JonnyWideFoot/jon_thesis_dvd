using System;
using System.Collections;

namespace UoB.Core.Primitives.Collections
{
	/// <summary>
	/// Summary description for floatList.
	/// </summary>
	public class FloatArrayList : IEnumerable
	{
		private ArrayList m_Floats;

		public FloatArrayList()
		{
			m_Floats = new ArrayList();
		}

		public FloatArrayList( int expectedLoading )
		{
			m_Floats = new ArrayList(expectedLoading);
		}

		public void AddFloat( float theFloat )
		{
			m_Floats.Add( theFloat );
		}

		public int Count
		{
			get
			{
				return m_Floats.Count;
			}
		}

		public bool Contains( float f )
		{
			return m_Floats.Contains( f );
		}

		public void Clear()
		{
			m_Floats.Clear();
		}

		public float this[int index] 
		{
			get 
			{
				return (float) m_Floats[index];
			}
			set 
			{
				m_Floats[index] = value;
			}
		}		
		
		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new floatListEnumerator(this);
		}

		private class floatListEnumerator : IEnumerator
		{
			private int position = -1;
			private FloatArrayList ownerList;

			public floatListEnumerator(FloatArrayList parent)
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
