using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UoB.Core.Primitives;

namespace UoB.Core.Structure
{
	/// <summary>
	/// Summary description for MoleculeList.
	/// </summary>
	public class MoleculeList : IEnumerable, ICloneable
	{
		protected ArrayList m_Molecules;
		protected AtomList m_Atoms; // must be cleared when the parent particle system enters content editing mode and subsequently rebuilt.
		protected bool m_AllowClear = true;

		public MoleculeList()
		{
			m_Molecules = new ArrayList();
			m_Atoms = new AtomList(100);
		}

		public MoleculeList( int load )
		{
			m_Molecules = new ArrayList();
			m_Atoms = new AtomList( load );
		}

        public void TransformAll(UoB.Core.Primitives.Matrix.MatrixRotation rotMat)
		{
			for( int i = 0; i < m_Atoms.Count; i++ )
			{
                rotMat.transform( m_Atoms[i] );
			}
		}

		public void TranslateAll( Position p )
		{
			for( int i = 0; i < m_Atoms.Count; i++ )
			{
				m_Atoms[i].Minus( p );
			}
		}

		public int[] MoleculeIndexes
		{
			get
			{
				int[] indexes = new int[ this.Count ];
				for( int i = 0; i < m_Molecules.Count; i++ )
				{
					indexes[i] = ((Atom)m_Molecules[i]).ArrayIndex;
				}
				return indexes;
			}
		}

		private static StringBuilder sb = new StringBuilder();
		public string MonomerString // single letter monomer codes for each child molecule
		{
			get
			{
				sb.Remove(0,sb.Length);
				for( int i = 0; i < m_Molecules.Count; i++ )
				{
					sb.Append( ((Molecule)m_Molecules[i]).moleculePrimitive.SingleLetterID );
				}
				return sb.ToString();
			}
		}

		public virtual void RemoveMolAt( int index )
		{
			if( index < 0 || index >= m_Molecules.Count )
			{
				throw new Exception("Cannot remove, the index is not within the range of the array!");
			}
			m_Molecules.RemoveAt( index );
		}

        public virtual void RemoveMolAt(int index, int count)
        {
            if (index < 0 ||  index + count >= m_Molecules.Count )
            {
                throw new Exception("Cannot remove, the index is not within the range of the array!");
            }
            m_Molecules.RemoveRange(index, count);
        }

		public virtual void addMolecule( Molecule theMolecule )
		{
			theMolecule.Parent = this;
			theMolecule.ArrayIndex = Count;
			m_Molecules.Add( theMolecule );
		}

		public virtual void addMoleculeList( MoleculeList theMolecules )
		{
			for( int i = 0; i < theMolecules.Count; i++ )
			{
				m_Molecules.Add( theMolecules[i] );
			}
		}

		public Molecule nextMolecule( Molecule currentMolecule )
		{
			if( !m_Molecules.Contains( currentMolecule ) ) throw new ArgumentException("A molecule is assigned to the wrong parent, it has asked for its neighbor in the incorrect array");
			int index = m_Molecules.IndexOf( currentMolecule );
			index = index + 1;
			if ( index < m_Molecules.Count )
			{
				return (Molecule) m_Molecules[ index ];
			}
			else
			{
				return null;
			}
		}

        public void DoGeometrixCenter()
        {
            Position p = GetGeometricCenter();
            TranslateAll(p);
        }

        public void DoGeometrixCenter(List<string> useAtom)
        {
            Position p = GetGeometricCenter(useAtom);
            TranslateAll(p);
        }

        public Position GetGeometricCenter()
        {
            Position geo = new Position();
            for (int i = 0; i < m_Atoms.Count; i++)
            {
                geo += m_Atoms[i];
            }
            geo.Divide((double)m_Atoms.Count);
            return geo;
        }

        public Position GetGeometricCenter(List<string> useAtom)
        {
            Position geo = new Position();
            double count = 0.0;
            for (int i = 0; i < m_Atoms.Count; i++)
            {
                if (useAtom.Contains(m_Atoms[i].PDBType))
                {
                    geo += m_Atoms[i];
                    count += 1.0;
                }
            }
            if (count == 0.0)
            {
                return new Position();
            }
            else
            {
                geo.Divide(count);
                return geo;
            }
        }    

        public Molecule previousMolecule(Molecule currentMolecule)
		{
			if( !m_Molecules.Contains( currentMolecule ) ) throw new ArgumentException("A molecule is assigned to the wrong parent, it has asked for its neighbor in the incorrect array");
			int index = m_Molecules.IndexOf( currentMolecule );
			index = index - 1;
			if ( index >= 0 )
			{
				return (Molecule) m_Molecules[ index ];
			}
			else
			{
				return null;
			}
		}

		public virtual void addMoleculeArray( Molecule[] theMolecules )
		{
			for( int i = 0; i < theMolecules.Length; i++ )
			{
				m_Molecules.Add( theMolecules[i] );
			}
		}

		public override string ToString()
		{
			return "Molecule List";
		}

		public AtomList Atoms
		{
			get
			{
				return m_Atoms;
			}
		}

		public int[] AtomIndexes
		{
			get
			{
				return m_Atoms.AtomIndexes;
			}
		}

		private void RegenerateAtomList()
		{
			m_Atoms.Clear();
			foreach ( Molecule m in m_Molecules )
			{
				m_Atoms.addAtomList( m );
			}
		}

		public int Count
		{
			get
			{
				return m_Molecules.Count;
			}
		}

		public bool Contains( Molecule m )
		{
			return m_Molecules.Contains( m );
		}

		public void Clear()
		{
			if ( m_AllowClear )
			{
				m_Molecules.Clear();
			}
		}

		public int IndexOf( Molecule m )
		{
			for ( int i = 0; i < m_Molecules.Count; i++ )
			{
				if( m_Molecules[i] == m )
				{
					return i;
				}
			}
			return -1;
		}

		public Molecule this[int index] 
		{
			get 
			{
				return (Molecule) m_Molecules[index];
			}
			set 
			{
				m_Molecules[index] = value;
			}
		}		
		
		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new MoleculeListEnumerator( this );
		}

		private class MoleculeListEnumerator : IEnumerator
		{
			private int position = -1;
			private MoleculeList ownerList;

			public MoleculeListEnumerator(MoleculeList theList)
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

		#region ICloneable Members

		public virtual object Clone()
		{
			MoleculeList ml = new MoleculeList();
			for( int i = 0; i < m_Molecules.Count; i++ )
			{
				ml.addMolecule( (Molecule)((Molecule)m_Molecules[i]).Clone() );
			}
			return ml;
		}

		#endregion
	}
}
