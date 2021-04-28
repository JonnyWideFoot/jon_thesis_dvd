using System;
using System.Collections;
using System.Diagnostics;

using UoB.Core.Primitives;

namespace UoB.Core.Structure
{
	/// <summary>
	/// Summary description for AtomList.
	/// </summary>
	public class AtomList : IEnumerable
	{
		protected ArrayList m_Atoms;
		protected bool m_AllowClear = true;
		protected int[] m_AtomIndices = new int[0];
		protected Position m_GeometricCenter = new Position(); // the geometric centre

		public AtomList( int loadFactor )
		{
			m_Atoms = new ArrayList( loadFactor );
		}

		public virtual void SetPositions( Position[] positions )
		{
			if( m_Atoms.Count != this.Count )
			{
				Trace.WriteLine("Positions-update ignored as the update was of a different length");
				return;
			}
			for ( int i = 0; i < positions.Length; i++ )
			{
				Atom a = (Atom) m_Atoms[i];
				Position p = positions[i];
				a.x = p.x;
				a.y = p.y;
				a.z = p.z;
			}
		}

        private bool AssertRange( double d )
        {
            if (d == Double.MaxValue) return false;
            if (d == Double.MinValue) return false;
            if (d == Double.NaN) return false;
            if (d == Double.NegativeInfinity) return false;
            if (d == Double.PositiveInfinity) return false;
            return true;
        }

        private bool AssertPosition( Position p )
        {
            if (!AssertRange(p.x)) return false;
            if (!AssertRange(p.y)) return false;
            if (!AssertRange(p.z)) return false;
            return true;
        }

        public bool AssertPositions()
        {
            for (int i = 0; i < m_Atoms.Count; i++)
            {
                Atom a = m_Atoms[i] as Atom;
                if (!AssertPosition(a)) return false; 
            }
            return true;
        }

        public AtomList()
		{
			m_Atoms = new ArrayList( 15 ); // arbitary round number per molecule
		}

		public void Translate( Position p )
		{
			foreach ( Atom a in m_Atoms )
			{
				a.x += p.x;
				a.y += p.y;
				a.z += p.z;
			}
		}

		public virtual void MinusAll( Position p)
		{
			for( int i = 0; i < Count; i++ )
			{
				this[i].Minus( p );
			}
		}

		public Position GetGeometricCenter( bool recalculate )
		{
			if( recalculate )
			{
				int number = m_Atoms.Count;
				double sumX = 0.0;
				double sumY = 0.0;
				double sumZ = 0.0;

				for( int i = 0; i < Count; i++ )
				{
					Atom a = (Atom)m_Atoms[i];
					sumX += a.x;
					sumY += a.y;
					sumZ += a.z;
				}

				m_GeometricCenter.x = (sumX / number);
				m_GeometricCenter.y = (sumY / number); 
				m_GeometricCenter.z = (sumZ / number);
			}
			
			return m_GeometricCenter;
		}

		public int[] AtomIndexes
		{
			get
			{
				if ( m_AtomIndices.Length != m_Atoms.Count )
				{
					m_AtomIndices = new int[ m_Atoms.Count ];
					for( int i = 0; i < m_Atoms.Count; i++ )
					{
						m_AtomIndices[i] = ((Atom)m_Atoms[i]).ArrayIndex;
					}
				}
				return m_AtomIndices;
			}
		}

		public virtual void addAtom( Atom theAtom )
		{
			m_Atoms.Add( theAtom );
		}

		public virtual void RemoveAtom( Atom theAtom )
		{
			m_Atoms.Remove( theAtom );
		}

        public virtual void RemoveAtomAt(int index)
        {
            m_Atoms.RemoveAt(index);
        }

        public virtual void DeleteHydrogens()
        {
            int i = m_Atoms.Count - 1;
            while( i >= 0 )
            {
                Atom a = m_Atoms[i] as Atom;
                if (a.atomPrimitive.Element == 'H')
                {
                    m_Atoms.RemoveAt(i);
                }
                i--;
            }
        }

		public virtual void addAtomList( AtomList theAtoms )
		{
			for( int i = 0; i < theAtoms.Count; i++ )
			{
				m_Atoms.Add( theAtoms[i] );
			}
		}

		public virtual void addAtomArray( Atom[] theAtoms )
		{
			for( int i = 0; i < theAtoms.Length; i++ )
			{
				m_Atoms.Add( theAtoms[i] );
			}
		}

		public int Count
		{
			get
			{
				return m_Atoms.Count;
			}
		}

		public bool Contains( Atom a )
		{
			return m_Atoms.Contains( a );
		}

		public void Clear()
		{
			if ( m_AllowClear )
			{
				m_Atoms.Clear();
			}
		}

		public int IndexOf( Atom a )
		{
			for ( int i = 0; i < m_Atoms.Count; i++ )
			{
				if( m_Atoms[i] == a )
				{
					return i;
				}
			}
			return -1;
		}

		public Atom this[int index] 
		{
			get 
			{
				return (Atom) m_Atoms[index];
			}
			set 
			{
				m_Atoms[index] = value;
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
			private AtomList ownerA;

			public AtomListEnumerator(AtomList theA)
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
