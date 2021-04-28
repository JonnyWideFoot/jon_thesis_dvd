using System;
using System.Collections;

namespace UoB.Core.ForceField.Definitions
{
	/// <summary>
	/// Summary description for AngleDefListList.
	/// </summary>
	public class AngleDefList: IEnumerable
	{
		private ArrayList m_AngleDefLists;

		public AngleDefList()
		{
			m_AngleDefLists = new ArrayList();
		}

		public void addAngleDef( AngleDefinition theAngleDef )
		{
			m_AngleDefLists.Add( theAngleDef );
		}

		public int Count
		{
			get
			{
				return m_AngleDefLists.Count;
			}
		}

		public bool Contains( AngleDefList a )
		{
			return m_AngleDefLists.Contains( a );
		}

		public void Clear()
		{
			m_AngleDefLists.Clear();
		}

		public AngleDefList this[int index] 
		{
			get 
			{
				return (AngleDefList) m_AngleDefLists[index];
			}
			set 
			{
				m_AngleDefLists[index] = value;
			}
		}		
		
		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new AngleDefListListEnumerator(this);
		}

		private class AngleDefListListEnumerator : IEnumerator
		{
			private int position = -1;
			private AngleDefList ownerList;

			public AngleDefListListEnumerator(AngleDefList theA)
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
