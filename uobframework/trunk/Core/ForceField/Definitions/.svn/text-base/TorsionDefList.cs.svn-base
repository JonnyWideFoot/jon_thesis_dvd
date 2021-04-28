using System;
using System.Collections;

namespace UoB.Core.ForceField.Definitions
{
	/// <summary>
	/// Summary description for TorsionDefListList.
	/// </summary>
	public class TorsionDefList: IEnumerable
	{
		private ArrayList m_TorsionDefList;

		public TorsionDefList()
		{
			m_TorsionDefList = new ArrayList();
		}

		public void addTorsionDef( TorsionDefinition theTorsionDef )
		{
			m_TorsionDefList.Add( theTorsionDef );
		}

		public int Count
		{
			get
			{
				return m_TorsionDefList.Count;
			}
		}

		public bool Contains( TorsionDefList a )
		{
			return m_TorsionDefList.Contains( a );
		}

		public void Clear()
		{
			m_TorsionDefList.Clear();
		}

		public TorsionDefinition this[int index] 
		{
			get 
			{
				return (TorsionDefinition) m_TorsionDefList[index];
			}
			set 
			{
				m_TorsionDefList[index] = value;
			}
		}		
		
		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new TorsionDefListEnumerator(this);
		}

		private class TorsionDefListEnumerator : IEnumerator
		{
			private int position = -1;
			private TorsionDefList ownerList;

			public TorsionDefListEnumerator(TorsionDefList theA)
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
