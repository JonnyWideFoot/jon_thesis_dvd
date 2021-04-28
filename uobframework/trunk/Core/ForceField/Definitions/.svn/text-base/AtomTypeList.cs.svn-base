using System;
using System.Collections;

namespace UoB.Core.ForceField.Definitions
{
	/// <summary>
	/// Summary description for AtomTypeList.
	/// </summary>
	public class AtomTypeList: IEnumerable
	{
		private ArrayList m_AtomTypeLists;

		public AtomTypeList()
		{
			m_AtomTypeLists = new ArrayList();
		}

		public void addAtomType( AtomType theAtomTypeDef )
		{
			m_AtomTypeLists.Add( theAtomTypeDef );
		}

		public int Count
		{
			get
			{
				return m_AtomTypeLists.Count;
			}
		}

		public AtomType GetTypeFromFFID( ref string TypeID )
		{
			for ( int i = 0; i < m_AtomTypeLists.Count; i++ )
			{
				if( this[i].TypeID == TypeID ) return this[i];
			}
			return AtomType.NullType;
		}

		public bool ContainsID( string TypeID )
		{
			for ( int i = 0; i < m_AtomTypeLists.Count; i++ )
			{
				if( this[i].TypeID == TypeID ) return true;
			}
			return false;
		}

		public bool Contains( AtomTypeList a )
		{
			return m_AtomTypeLists.Contains( a );
		}

		public AtomType this[int index] 
		{
			get 
			{
				return (AtomType) m_AtomTypeLists[index];
			}
			set 
			{
				m_AtomTypeLists[index] = value;
			}
		}		
		
		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new AtomTypeListEnumerator(this);
		}

		private class AtomTypeListEnumerator : IEnumerator
		{
			private int position = -1;
			private AtomTypeList ownerList;

			public AtomTypeListEnumerator(AtomTypeList theList)
			{
				ownerList = theList;
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
