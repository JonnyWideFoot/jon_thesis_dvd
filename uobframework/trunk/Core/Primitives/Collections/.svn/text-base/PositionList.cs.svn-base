using System;
using System.Collections;

namespace UoB.Core.Primitives.Collections
{
	/// <summary>
	/// Summary description for PositionList.
	/// </summary>
	public class PositionList : IEnumerable
	{
		private ArrayList m_Positions;

		public PositionList()
		{
			m_Positions = new ArrayList();
		}

		public PositionList( int load )
		{
			m_Positions = new ArrayList(load);
		}

		public void addPosition( Position Position )
		{
			m_Positions.Add( Position );
		}

		public void AddArray( ref Position[] Positions )
		{
			for ( int i = 0; i < Positions.Length; i++ )
			{
				m_Positions.Add( Positions[i] );
			}
		}

		public int Count
		{
			get
			{
				return m_Positions.Count;
			}
		}

		public void Clear()
		{
			m_Positions.Clear();
		}

		public Position this[int index] 
		{
			get 
			{
				return (Position) m_Positions[index];
			}
			set 
			{
				m_Positions[index] = value;
			}
		}		
		
		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new PositionListEnumerator(this);
		}

		private class PositionListEnumerator : IEnumerator
		{
			private int position = -1;
			private PositionList ownerVL;

			public PositionListEnumerator(PositionList theVL)
			{
				ownerVL = theVL;
			}

			// Declare the MoveNext method required by IEnumerator:
			public bool MoveNext()
			{
				if (position < ownerVL.Count - 1)
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
					return ownerVL[position];
				}
			}
		}

		#endregion
	}
}
