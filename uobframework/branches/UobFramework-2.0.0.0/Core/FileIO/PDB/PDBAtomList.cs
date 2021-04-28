using System;
using System.Collections;

using UoB.Core.ForceField;

namespace UoB.Core.FileIO.PDB
{
	/// <summary>
	/// Summary description for PDBAtomList.
	/// </summary>
	public class PDBAtomList : IEnumerable
	{
		protected ArrayList m_PDBAtoms;
		protected bool m_AllowClear = true;

		public PDBAtomList()
		{
			m_PDBAtoms = new ArrayList();
		}

		public virtual void addPDBAtom( PDBAtom thePDBAtom )
		{
			m_PDBAtoms.Add( thePDBAtom );
		}

		public virtual void addPDBAtomList( PDBAtomList thePDBAtoms )
		{
			for( int i = 0; i < thePDBAtoms.Count; i++ )
			{
				m_PDBAtoms.Add( thePDBAtoms[i] );
			}
		}

		public int Count
		{
			get
			{
				return m_PDBAtoms.Count;
			}
		}

		public PDBAtomList CloneList( string name, ArrayList notIncludeList )
		{
			if( name.Length != 3 ) throw new Exception("PDBAtomList : the replacement for the name should be of length 3" );

			PDBAtomList pdbAtomList = new PDBAtomList();

			for ( int i = 0; i < m_PDBAtoms.Count; i++ )
			{
				PDBAtom a = (PDBAtom) m_PDBAtoms[i];
				PDBAtom clone = (PDBAtom) a.Clone();
				pdbAtomList.addPDBAtom( clone );
			}

			for( int i = 0; i < notIncludeList.Count; i++ )
			{
				bool IsRemoved = false;
				for( int j = pdbAtomList.Count -1; j >= 0; j-- ) 
					// count backwards so that we can remove at a given index and still remove the correct entry
				{
					string nonInclude = (string) (notIncludeList[i]);
					if( nonInclude.Length != 4 )
					{
						throw new Exception( "PDBAtomList : the given excluded atom name was not of length 4" );
					}
					if( pdbAtomList[j].atomName == nonInclude )
					{
						pdbAtomList.m_PDBAtoms.RemoveAt( j );
						IsRemoved = true;
						break;
					}
				}
				if ( !IsRemoved )
				{
					throw new Exception("PDBAtomList : asked to remove an non-existant atom name");
				}
			}

			return pdbAtomList;
		}

		public bool Contains( PDBAtom a )
		{
			return m_PDBAtoms.Contains( a );
		}

		public void Clear()
		{
			if ( m_AllowClear )
			{
				m_PDBAtoms.Clear();
			}
		}

		public int IndexOf( PDBAtom a )
		{
			for ( int i = 0; i < m_PDBAtoms.Count; i++ )
			{
				if( m_PDBAtoms[i] == a )
				{
					return i;
				}
			}
			return -1;
		}

		public PDBAtom this[int index] 
		{
			get 
			{
				return (PDBAtom) m_PDBAtoms[index];
			}
			set 
			{
				m_PDBAtoms[index] = value;
			}
		}		
		
		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new AtomListEnumerator(this);
		}

		private class AtomListEnumerator : IEnumerator
		{
			private int position = -1;
			private PDBAtomList ownerA;

			public AtomListEnumerator(PDBAtomList theA)
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
