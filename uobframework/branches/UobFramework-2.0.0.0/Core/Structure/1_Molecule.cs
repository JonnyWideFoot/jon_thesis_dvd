using System;
using System.Collections;
using UoB.Core.ForceField;
using UoB.Core.Primitives;
using UoB.Core.Structure.Primitives;
using UoB.Core.FileIO.PDB;

namespace UoB.Core.Structure
{
	/// <summary>
	/// Summary description for Molecule.
	/// </summary>
	/// 

	public class Molecule : AtomList, ICloneable
	{
		protected int m_ArrayIndex = -1;
		protected char m_Prefix = '\0'; // used to indicate N and C terminal residues
		protected string m_Name;
		protected int m_ResidueNumber;
		protected char m_InsertionCode;
		protected MoleculePrimitiveBase m_MoleculePrimitive;
        protected MoleculeList m_Parent;
		protected FFManager m_FFParams = FFManager.Instance;

		public Molecule( string name, int residueNumber, char inserstionCode )
		{
			m_Parent = null;
			m_Name = name;
			m_ResidueNumber = residueNumber;
			m_InsertionCode = inserstionCode;
		}

		public void ResetName( string name, char prefix, bool assignAtomPrimitives )
		{
			m_Name = name;
			setMolPrimitive( prefix, assignAtomPrimitives );
		}

		public void setMolPrimitive( char prefix, bool assignAtomPrimitives )
		{
			m_Prefix = prefix; 

			if( prefix != ' ' ) // blanks are not stored as " bla", but as "bla"
			{
				m_MoleculePrimitive = m_FFParams.GetMolPrimitive( prefix + m_Name );
			}
			else
			{
				m_MoleculePrimitive = m_FFParams.GetMolPrimitive( m_Name );
			}

			if( assignAtomPrimitives )
			{
				setAtomPrimitives();
			}
		}

		public void setMolPrimitive( bool assignAtomPrimitives )
		{
			m_Prefix = '\0';
			m_MoleculePrimitive = m_FFParams.GetMolPrimitive( m_Name );
			if( assignAtomPrimitives )
			{
				setAtomPrimitives();
			}
		}

		public void setMolPrimitive( MoleculePrimitiveBase molPrim, bool assignAtomPrimitives )
		{
			m_MoleculePrimitive = molPrim;
			if( assignAtomPrimitives )
			{
				setAtomPrimitives();
			}
		}

		public virtual void setAtomPrimitives()
		{
			if( m_MoleculePrimitive != null )
			{
				for( int i = 0; i < m_Atoms.Count; i++ )
				{
					Atom a = this[i];
					a.atomPrimitive = m_MoleculePrimitive.GetAtomPrimitiveFromPDBID( a.PDBType );
				}
			}
			else
			{
				for( int i = 0; i < m_Atoms.Count; i++ )
				{
					Atom a = this[i];
					a.atomPrimitive = new AtomPrimitiveBase( a.PDBType );
				}
			}
		}

		
		public override void addAtom(Atom theAtom)
		{
			base.addAtom (theAtom);
			theAtom.parentMolecule = this;
		}

		public override void RemoveAtom(Atom theAtom)
		{
			base.RemoveAtom (theAtom);
			theAtom.parentMolecule = null;
		}

		public override void addAtomArray(Atom[] theAtoms)
		{
			for( int i = 0; i < theAtoms.Length; i++ )
			{
				m_Atoms.Add( theAtoms[i] );
				theAtoms[i].parentMolecule = this;
			}
		}

		public override void addAtomList(AtomList theAtoms)
		{
			for( int i = 0; i < theAtoms.Count; i++ )
			{
				m_Atoms.Add( theAtoms[i] );
				theAtoms[i].parentMolecule = this;
			}
		}

		public void PerformProximityBonding()
		{
			PerformProximityBonding(this);
		}

		public static void PerformProximityBonding( AtomList theAtoms )
		{
			for ( int i = 0; i < theAtoms.Count; i++ )
			{
				for ( int j = i; j < theAtoms.Count; j++ )
				{
					if ( !theAtoms[i].bondedTo( (Atom)theAtoms[j]) && !theAtoms[j].bondedTo( (Atom)theAtoms[i]) )
					{
						if ( theAtoms[i].atomPrimitive.Element == 'S' || theAtoms[j].atomPrimitive.Element == 'S' )
						{
							// S-S bonds can be longer
							if ( theAtoms[i].distanceSquaredTo( (Atom)theAtoms[j] ) < 4.0f ) 
							{
								theAtoms[i].bondTo( (Atom)theAtoms[j] );
								theAtoms[j].bondTo( (Atom)theAtoms[i] );
							}
						}
						else
						{
							if ( theAtoms[i].distanceSquaredTo( (Atom)theAtoms[j] ) < 2.56f )
							{
								theAtoms[i].bondTo( (Atom)theAtoms[j] );
								theAtoms[j].bondTo( (Atom)theAtoms[i] );
							}
						}
					}
				}
			}
		}

		public Atom AtomOfType( string PDBID )
		{
			for( int i = 0; i < m_Atoms.Count; i++ )
			{
				if( ((Atom)m_Atoms[i]).PDBType == PDBID )
				{
                    return (Atom) m_Atoms[i];                    
				}
			}
			return null;
		}

		public void SortAtomList()
		{
			// we want the atoms in the same order as in the rotamer
			m_Atoms.Sort( m_MoleculePrimitive );
		}

		public MoleculeList Parent
		{
			get
			{
				return m_Parent;
			}
			set 
			{
				m_Parent = value;
			}
		}

		public int ArrayIndex
		{
			get
			{
				return m_ArrayIndex;
			}
			set
			{
				m_ArrayIndex = value;
			}
		}

		public MoleculePrimitiveBase moleculePrimitive
		{
			get
			{
				return m_MoleculePrimitive;
			}
		}
		
		public bool IsSolvent
		{
			get
			{
				return m_MoleculePrimitive.IsSolvent;
			}
		}

		public int ResidueNumber
		{
			get
			{
				return m_ResidueNumber;
			}
			set
			{
				m_ResidueNumber = value;
			}
		}

		public char InsertionCode
		{
			get
			{
				return m_InsertionCode;
			}
			set
			{
				m_InsertionCode = value;
			}
		}

		public override string ToString()
		{
			return m_MoleculePrimitive.MolName + " " + m_ResidueNumber.ToString();
		}

		public string ContentString
		{
			get
			{
				string s = "";
				for ( int i = 0; i < m_Atoms.Count; i++ )
				{
					s += ((Atom)m_Atoms[i]).atomPrimitive.PDBIdentifier;
					s += ",";
				}
				return s;
			}
		}

		public Char parentChainID
		{
			get
			{
				try
				{
					return ((PSMolContainer)m_Parent).ChainID;
				}
				catch( System.InvalidCastException )
				{
					return '\0';
				}
			}
		}

		public string Name
		{
			get
			{
				return m_MoleculePrimitive.MolName;
			}
		}

		#region ICloneable Members

		public virtual object Clone()
		{
			Molecule m = new Molecule( m_Name, m_ResidueNumber, m_InsertionCode );
			for( int i = 0; i < Count; i++ )
			{
				Atom a = (Atom)((Atom)m_Atoms[i]).Clone();
				m.addAtom( a ); // sets the parented state of the atom too
			}
			m.setMolPrimitive( moleculePrimitive, true );
			return m;
		}

		#endregion
	}

}
