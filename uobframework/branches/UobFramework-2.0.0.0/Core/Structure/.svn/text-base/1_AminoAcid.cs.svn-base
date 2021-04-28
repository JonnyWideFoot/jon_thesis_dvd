using System;
using UoB.Core.FileIO.PDB;

using UoB.Core.ForceField;

namespace UoB.Core.Structure
{
	/// <summary>
	/// Summary description for AminoAcid.
	/// </summary>
	public class AminoAcid : Molecule, ICloneable
	{
		private Atom m_CAlpAtom;
		private Atom m_NTerminalAtom;
		private Atom m_CTerminalAtom;

		public AminoAcid( string name, int residueNumber, char inserstionCode ) : base( name, residueNumber, inserstionCode )
		{
		}

		public override void setAtomPrimitives()
		{
			base.setAtomPrimitives();
			for( int i = 0; i < m_Atoms.Count; i++ )
			{
				if ( this[i].PDBType == PDBAtom.PDBID_BackBoneN  )
				{
					m_NTerminalAtom = this[i];
				}
				else if ( this[i].PDBType == PDBAtom.PDBID_BackBoneC ) 
				{
					m_CTerminalAtom = this[i];
				}
				else if ( this[i].PDBType == PDBAtom.PDBID_BackBoneCA ) 
				{
					m_CAlpAtom = this[i];
				}
			}
		}

		public Atom CAlphaAtom
		{
			get
			{
				return m_CAlpAtom;
			}
		}
	
		public Atom NTerminalAtom
		{
			get
			{
				return m_NTerminalAtom;
			}
		}

		public Atom CTerminalAtom
		{
			get
			{
				return m_CTerminalAtom;
			}
		}

		public float phiAngle
		{
			get
			{
				if ( m_Parent == null )
				{
					return float.NegativeInfinity;
				}

				AminoAcid prevAA = (AminoAcid) m_Parent.previousMolecule(this);

				if ( prevAA == null ||
					prevAA.CTerminalAtom == null ||
					NTerminalAtom == null ||
					CAlphaAtom == null ||
					CTerminalAtom == null
					)
				{
					return float.NegativeInfinity;
				}

				return Atom.MeasureTorsion( prevAA.CTerminalAtom, NTerminalAtom, CAlphaAtom, CTerminalAtom);
			}
		}

		public float psiAngle
		{
			get
			{
				if ( m_Parent == null )
				{
					return float.NegativeInfinity;
				}

				AminoAcid nextAA = (AminoAcid) m_Parent.nextMolecule(this);

				if ( nextAA == null || 
					nextAA.NTerminalAtom == null ||
					NTerminalAtom == null ||
					CAlphaAtom == null ||
					CTerminalAtom == null
					)
				{
					return float.NegativeInfinity;
				}

				return Atom.MeasureTorsion( NTerminalAtom, CAlphaAtom, CTerminalAtom, nextAA.NTerminalAtom);
			}
		}

		#region ICloneable Members

		public override object Clone()
		{
			AminoAcid aa = new AminoAcid( m_Name, m_ResidueNumber, m_InsertionCode );
			for( int i = 0; i < Count; i++ )
			{
				Atom a = (Atom)((Atom)m_Atoms[i]).Clone();
				aa.addAtom( a ); // sets the a.Parent
			}
			aa.setMolPrimitive( moleculePrimitive, true );
			return aa;
		}

		#endregion
	}
}
