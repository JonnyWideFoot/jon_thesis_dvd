using System;
using System.Collections;

namespace UoB.Core.ForceField
{
	/// <summary>
	/// Summary description for RotamerList.
	/// </summary>
	public class RotamerList : IEnumerable
	{
		protected ArrayList m_Rotamers;
		protected bool m_AllowClear = true;

		public RotamerList()
		{
			m_Rotamers = new ArrayList();
		}

		public virtual void addRotamer( Rotamer theRotamer )
		{
			m_Rotamers.Add( theRotamer );
		}

		public virtual void addRotamerList( RotamerList theRotamers )
		{
			for( int i = 0; i < theRotamers.Count; i++ )
			{
				m_Rotamers.Add( theRotamers[i] );
			}
		}

		public virtual void addRotamerArray( Rotamer[] theRotamers )
		{
			for( int i = 0; i < theRotamers.Length; i++ )
			{
				m_Rotamers.Add( theRotamers[i] );
			}
		}

		public int Count
		{
			get
			{
				return m_Rotamers.Count;
			}
		}

		public bool Contains( Rotamer a )
		{
			return m_Rotamers.Contains( a );
		}

		public void Clear()
		{
			if ( m_AllowClear )
			{
				m_Rotamers.Clear();
			}
		}

		public int IndexOf( Rotamer a )
		{
			for ( int i = 0; i < m_Rotamers.Count; i++ )
			{
				if( m_Rotamers[i] == a )
				{
					return i;
				}
			}
			return -1;
		}

		public Rotamer this[int index] 
		{
			get 
			{
				return (Rotamer) m_Rotamers[index];
			}
			set 
			{
				m_Rotamers[index] = value;
			}
		}		
		
		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new RotamerListEnumerator(this);
		}

		private class RotamerListEnumerator : IEnumerator
		{
			private int position = -1;
			private RotamerList ownerA;

			public RotamerListEnumerator(RotamerList theA)
			{
				ownerA = theA;
			}

			// Declare the MoveNext method required by IEnumerator:
			public bool MoveNext()
			{
				if (position < ownerA.Count - 1)
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
					return ownerA[position];
				}
			}
		}

		#endregion
	}
}
