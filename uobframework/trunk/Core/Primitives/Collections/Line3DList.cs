using System;
using System.Collections;

namespace UoB.Core.Primitives.Collections
{
	/// <summary>
	/// Summary description for Line3DList.
	/// </summary>
	public class Line3DList : IEnumerable
	{
		private ArrayList m_Lines;

		public Line3DList()
		{
			m_Lines = new ArrayList();
		}

		public void addLine( Line3D line )
		{
			m_Lines.Add( line );
		}

		public int Count
		{
			get
			{
				return m_Lines.Count;
			}
		}

		public void Clear()
		{
			m_Lines.Clear();
		}

		public Line3D this[int index] 
		{
			get 
			{
				return (Line3D) m_Lines[index];
			}
			set 
			{
				m_Lines[index] = value;
			}
		}		
		
		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new Line3DListEnumerator(this);
		}

		private class Line3DListEnumerator : IEnumerator
		{
			private int position = -1;
			private Line3DList ownerLL;

			public Line3DListEnumerator(Line3DList theLL)
			{
				ownerLL = theLL;
			}

			// Declare the MoveNext method required by IEnumerator:
			public bool MoveNext()
			{
				if (position < ownerLL.Count - 1)
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
					return ownerLL[position];
				}
			}
		}

		#endregion
	}
}
