using System;
using System.Collections;
using System.Diagnostics;

using UoB.Core.Primitives;
using UoB.Core.FileIO.PDB;
using UoB.Core.Structure;
using UoB.Core.ForceField.Definitions;

namespace UoB.Core.Structure.Primitives
{
	/// <summary>
	/// Summary description for MoleculePrimitiveBase.
	/// </summary>
	public class MoleculePrimitive : MoleculePrimitiveBase
	{
		protected string m_Geometry;
		protected ArrayList m_Rotamers;
		protected ArrayList m_AtomPrimitives;
		protected ArrayList m_Torsions;

		public MoleculePrimitive( string moleculeName, char moleculeSingleLetterID ) : base(moleculeName)
		{
			m_MoleculeName = moleculeName;
			m_MoleculeSingleLetterID = moleculeSingleLetterID;

			m_IsSolvent = false;
			m_AtomPrimitives = new ArrayList();
			m_Torsions = new ArrayList();
			m_Rotamers = new ArrayList(10);
		}

		internal void SetIsSolvent( bool isSolvent )
		{
			m_IsSolvent = isSolvent;
		}

		internal void SetGeometry( string geo )
		{
			m_Geometry = geo;
		}

		public string Geometry
		{
			get
			{
				return m_Geometry;
			}
		}
		public bool hasRotamers
		{
			get
			{
				return m_Rotamers != null;
			}
		}

		public int RotamerCount
		{
			get
			{
				return m_Rotamers.Count;
			}
		}

		public bool IsValidAtomID( string type )
		{
			// required to assign charges to atom definitions of the Molecule blocks in the file
			for(int i = 0; i < m_AtomPrimitives.Count; i++ )
			{
				if( ((AtomPrimitive) m_AtomPrimitives[i]).AltName == type )
				{
					return true;
				}
			}
			return false;
		}

		public void AddRotamerDefinition( PDBAtomList rotamer )
		{
			if ( ValidateRotamer( rotamer ) )
			{
				m_Rotamers.Add( rotamer );
			}
		}

		private bool ValidateRotamer( PDBAtomList rotamer )
		{
			bool allOK = true;
			int neighbourResidueAnchors = 0;
			// positions in the neighbouring residues used in builder processes
			// e.g. the backbone H
			// this requires the C in the previous residue for allignment
			// therefore we need to define a relative position for that atom in the rotamer

			for( int i = 0; i < rotamer.Count; i++ )
			{
				bool isThere = false;
				string checkName = rotamer[i].atomName;

				if( checkName[0] == '+' || checkName[0] == '-' )
				{
					checkName  = checkName.Substring(1,3) + " ";
					neighbourResidueAnchors++; // increment the anchor positions
				}

				for( int j = 0; j < m_AtomPrimitives.Count; j++ )
				{
					if( ((AtomPrimitive) m_AtomPrimitives[j]).AltName == checkName )
					{
						isThere = true;
						break;
					}
				}
				if ( !isThere )
				{
					allOK = false;
				}
			}

			if( rotamer.Count != (m_AtomPrimitives.Count + neighbourResidueAnchors) )
			{
				// a rotamer atom must be defined for ever atom primitive
				allOK = false;
			}

			if ( !allOK )
			{
				Trace.WriteLine("Rotamer Addition Error in MoleculePrimitive : A rotamer has failed validation");
			}

			return allOK;
		}

		public PDBAtomList GetRotamer( int ID )
		{
			if( 0 <= ID && ID < m_Rotamers.Count )
			{
				return (PDBAtomList) m_Rotamers[ID];
			}
			else
			{
				return null;
			}
		}

		public override bool ContainsAtomWithAltID( string AtomID )
		{
			for(int i = 0; i < m_AtomPrimitives.Count; i++ )
			{
				if( ((AtomPrimitive) m_AtomPrimitives[i]).AltName == AtomID )
				{
					return true;
				}
			}
			return false;
		}

		public override bool ContainsAtomWithPDBID( string AtomID )
		{
			for(int i = 0; i < m_AtomPrimitives.Count; i++ )
			{
				if( ((AtomPrimitive) m_AtomPrimitives[i]).PDBIdentifier == AtomID )
				{
					return true;
				}
			}
			return false;
		}

		public override AtomPrimitiveBase GetAtomPrimitiveFromAltID( string AtomID )
		{
			// required to assign charges to atom definitions of the Molecule blocks in the file
			for(int i = 0; i < m_AtomPrimitives.Count; i++ )
			{
				if( ((AtomPrimitiveBase) m_AtomPrimitives[i]).AltName == AtomID )
				{
					return (AtomPrimitiveBase)m_AtomPrimitives[i];
				}
			}
			return new AtomPrimitiveBase( AtomID );
		}

		public override AtomPrimitiveBase GetAtomPrimitiveFromPDBID( string atomPDBName )
		{
			for(int i = 0; i < m_AtomPrimitives.Count; i++ )
			{
				if( ((AtomPrimitiveBase) m_AtomPrimitives[i]).PDBIdentifier == atomPDBName )
				{
					return (AtomPrimitiveBase)m_AtomPrimitives[i];
				}
			}
			return new AtomPrimitiveBase( atomPDBName );
		}

		public void AddPrimitive( AtomPrimitive primitive )
		{
			if ( ContainsAtomWithPDBID( primitive.PDBIdentifier ) )
			{
				Trace.WriteLine("PARSE ERROR : Mol definition contains an ATOM type duplication. It will be ignored");
			}
			else
			{
				m_AtomPrimitives.Add( primitive );
			}
		}

		public override Torsion[] Torsions
		{
			get
			{
				return (Torsion[]) m_Torsions.ToArray( typeof( Torsion ) );
			}
		}

		public override int TorsionDefinitionCount
		{
			get
			{
				return m_Torsions.Count;
			}
		}

		public void AddTorsion( Torsion tor )
		{
			m_Torsions.Add( tor );
		}

		public override int AtomPrimitiveCount
		{
			get
			{
				return m_AtomPrimitives.Count;
			}
		}

		public AtomPrimitive this[ int index ] 
		{
			get 
			{
				return m_AtomPrimitives[ index ] as AtomPrimitive;
			}
		}
	
		#region IComparer Members

		public override int Compare(object x, object y)
		{
			// Used to sort the atoms in a molecule by its molecule primitive
			// Less than zero -> x is less than y
			// Zero -> x equals y
			// Greater than zero -> x is greater than y

			string atomXID = ((Atom) x).ALTType;
			string atomYID = ((Atom) y).ALTType;
			if ( atomXID == atomYID ) return 0;
			string ID = "";

			for(int i = 0; i < m_AtomPrimitives.Count; i++ )
			{
				ID = ((AtomPrimitive) m_AtomPrimitives[i]).AltName;
				if ( ID == atomXID ) return -1;
				if ( ID == atomYID ) return 1;
			}
			return -1; // put all the shit at the end ...
		}

		#endregion


	}
}
