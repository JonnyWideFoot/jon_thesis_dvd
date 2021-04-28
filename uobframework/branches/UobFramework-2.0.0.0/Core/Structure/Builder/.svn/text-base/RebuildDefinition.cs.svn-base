using System;
using System.Diagnostics;

using UoB.Core.Primitives.Collections;
using UoB.Core.Structure;
using UoB.Core.Structure.Primitives;
using UoB.Core.Primitives;
using UoB.Core.Primitives.Matrix;
using UoB.Core.FileIO.PDB;

namespace UoB.Core.Structure.Builder
{
	/// <summary>
	/// Summary description for RebuildDefinition.
	/// </summary>
	public sealed class RebuildDefinition
	{
		// main PS members
		private Molecule m_Molecule = null;
		private MoleculePrimitive m_MolPrim = null;
		private AtomPrimitive m_AtomPrimitive = null; // the AP of the atom to be rebuilt
		private PDBAtomList m_Rotamer = null;
		private static MatrixRotation m_RotMat = new MatrixRotation();
		private static Object m_RotMatLock = new Object(); // thread safing for the roation matrix

		// rebuild positions
		private AssociatedObjectList m_Level1List = new AssociatedObjectList();
		private AssociatedObjectList m_Level2List = new AssociatedObjectList();
		private AssociatedObjectList m_Level3List = new AssociatedObjectList();
		private Position m_AtomPosInRotamer = new Position();
		private Position m_AtomPos1InRotamer = new Position();
		private Position m_AtomPos2InRotamer = new Position();
		private Position m_AtomPos3InRotamer = new Position();
		private Atom m_Atom1 = null; // the anchor atoms in relation to the restore atom must be bond rotation independent
		private Atom m_Atom2 = null;
		private Atom m_Atom3 = null;

		// Positioning Vectors
		private Position m_ReturnTranslation = new Position();
		private Position m_RotamerTranslation = new Position();

		private Vector m_MolVectorTo2 = new Vector();
		private Vector m_RotVectorTo2 = new Vector();
		private Vector m_PlaneNormalTo2 = new Vector();
		private double m_DotProductTo2 = 0.0;
		private double m_AngleTo2 = 0.0;

		private Vector m_MolVectorTo3 = new Vector();
		private Vector m_RotVectorTo3 = new Vector();

		private Vector m_NormalToMolPlane = new Vector();
		private Vector m_NormalToRotPlane = new Vector();
		private double m_PlaneDotProduct = 0.0;
		private double m_AngleBetweenPlaneNormals = 0.0;

		private Vector m_RotPlaneCopy = new Vector();

		public RebuildDefinition( Molecule m )
		{
			m_Molecule = m;
		}

		public RebuildDefinition()
		{
		}

		public Molecule molecule
		{
			get
			{
				return m_Molecule;
			}
			set
			{
				m_Molecule = value;
				m_MolPrim = null;
				m_Rotamer = null;
				if( m_Molecule != null )
				{
					if( m_Molecule.moleculePrimitive is MoleculePrimitive )
					{
						m_MolPrim = (MoleculePrimitive) m_Molecule.moleculePrimitive;
						m_Rotamer = m_MolPrim.GetRotamer(0); // get the 1st rotamer defined, all AA's have at least one
					}
				}				
			}
		}

		public bool RebuildAtom( AtomPrimitive ap )
		{
			if( m_Molecule == null || m_MolPrim == null || m_Rotamer == null )
			{
				throw new Exception("REBUILD CRITICAL FAIL : Some molecule information is null");
			}

			m_AtomPrimitive = ap;

			if( !GetAllPositions() )
			{
				return false; // we dont have the required atoms present
			}

			doRebuild(); // cool, all positions present, lets position our new atom and add it to the molecule ...	
		
			return true;
		}

//		private void DebugReportAll()
//		{
//			Position.DebugReport( m_AtomPosInRotamer, 'C' );
//			Position.DebugReport( m_AtomPos1InRotamer, 'C' );
//			Position.DebugReport( m_AtomPos2InRotamer, 'C' );
//			Position.DebugReport( m_AtomPos3InRotamer, 'C' );
//			Position.DebugReport( m_Atom1, 'S' );
//			Position.DebugReport( m_Atom2, 'S' );
//			Position.DebugReport( m_Atom3, 'S' );
//		}
//
//		private void DebugReportAll2()
//		{
//			Position.DebugReport( m_MolVectorTo2, 'N' );
//			Position.DebugReport( m_RotVectorTo2, 'Q' );
//			Position.DebugReport( m_PlaneNormalTo2, 'O' );
//		}
//
//		private void DebugReportAll3()
//		{
//			Position.DebugReport( new Position(), 'Q' );
//			Position.DebugReport( m_MolVectorTo2, 'C' );
//			Position.DebugReport( m_MolVectorTo3, 'C' );
//			Position.DebugReport( m_NormalToMolPlane, 'O' );
//			Position.DebugReport( m_RotVectorTo2, 'S' );
//			Position.DebugReport( m_RotVectorTo3, 'S' );
//			Position.DebugReport( m_NormalToRotPlane, 'N' );
//		}

		private void doRebuild()
		{
			// first we need to translate both to a rotation centre
			// seeing as we dont know the atom position (obviously) this cant be the centre
			// so we will use atom 1

			m_ReturnTranslation.setTo( m_Atom1 );
			m_RotamerTranslation.setTo( m_AtomPos1InRotamer );

			// we now need to translate the cAlp of both the rotamer and the AA to the origin

			m_AtomPosInRotamer .Minus( m_RotamerTranslation );
			m_AtomPos1InRotamer.Minus( m_RotamerTranslation );
			m_AtomPos2InRotamer.Minus( m_RotamerTranslation );
			m_AtomPos3InRotamer.Minus( m_RotamerTranslation );

			m_Atom1.Minus( m_ReturnTranslation );
			m_Atom2.Minus( m_ReturnTranslation );
			m_Atom3.Minus( m_ReturnTranslation );

			// all our positions now centre around m_Atom1 at (0,0,0)

			// we now want to rotate the atoms so that the normal Positions for m_Atom1 -> atom 2 are aligned

			// no need to minus the m_Atom1 as it is at 0,0,0
			// unit vectors are required ...
			m_MolVectorTo2.SetToUnitVectorOf( m_Atom2 );
			m_RotVectorTo2.SetToUnitVectorOf( m_AtomPos2InRotamer );

			m_DotProductTo2 = Vector.dotProduct( m_MolVectorTo2, m_RotVectorTo2 );
			m_PlaneNormalTo2.SetToCrossProduct( m_RotVectorTo2, m_MolVectorTo2 );
			m_AngleTo2 = Math.Acos( m_DotProductTo2 );

			lock ( m_RotMatLock )
			{
				m_RotMat.setToAxisRot( m_PlaneNormalTo2, m_AngleTo2 );
				// no point in doing m_Atom1 as its on the origin !
				m_RotMat.transform( m_AtomPosInRotamer );
				m_RotMat.transform( m_AtomPos2InRotamer );
				m_RotMat.transform( m_AtomPos3InRotamer );
			}           

			m_MolVectorTo3.SetToUnitVectorOf( m_Atom3 );
			m_NormalToMolPlane.SetToCrossProduct( m_MolVectorTo2, m_MolVectorTo3 );
			m_NormalToMolPlane.MakeUnitVector();

			m_RotVectorTo2.SetToUnitVectorOf( m_AtomPos2InRotamer ); // this has changed from before (you stupid fool ;-) )
			m_RotVectorTo3.SetToUnitVectorOf( m_AtomPos3InRotamer );
			m_NormalToRotPlane.SetToCrossProduct( m_RotVectorTo2, m_RotVectorTo3 ); // can use the normalised mol Position here, its the same
			m_NormalToRotPlane.MakeUnitVector();

			m_PlaneDotProduct = Vector.dotProduct( m_NormalToMolPlane, m_NormalToRotPlane );
			m_AngleBetweenPlaneNormals = Math.Acos( m_PlaneDotProduct );

			//DebugReportAll3();

			// mini-hack to find which way to rotate
			lock ( m_RotMatLock )
			{
				m_RotPlaneCopy.setTo( m_NormalToRotPlane );
				m_RotMat.setToAxisRot( m_MolVectorTo2, m_AngleBetweenPlaneNormals );
				m_RotMat.transform( m_RotPlaneCopy );
				//Position.DebugReport( m_RotPlaneCopy, 'Q' );
				double ang1 = Position.AngleBetweenUnitVectors( m_RotPlaneCopy, m_NormalToMolPlane );

				m_RotPlaneCopy.setTo( m_NormalToRotPlane );
				m_RotMat.setToAxisRot( m_MolVectorTo2, -m_AngleBetweenPlaneNormals );
				m_RotMat.transform( m_RotPlaneCopy );	
				//Position.DebugReport( m_RotPlaneCopy, 'Q' );
				double ang2 = Position.AngleBetweenUnitVectors( m_RotPlaneCopy, m_NormalToMolPlane );

				if ( ang1 > ang2 )
				{
					m_AngleBetweenPlaneNormals = -m_AngleBetweenPlaneNormals;
				}

				// rotate around the previously aligned m_Atom1 to m_Atom2 Position
				// this is correct as the plane normals are 90 degrees offset from the line of the plane,
				// which is formed by the m_Atom1 to atom 2 Position, the angle is therefore the same

				m_RotMat.setToAxisRot( m_MolVectorTo2, m_AngleBetweenPlaneNormals );
				m_RotMat.transform( m_AtomPos3InRotamer );
				m_RotMat.transform( m_AtomPosInRotamer );
				m_RotMat.transform( m_AtomPos2InRotamer );
			}

			Atom a = new Atom(
				m_AtomPrimitive.PDBIdentifier,				
				-1,
				-1,
				m_Molecule,
				m_AtomPosInRotamer.x,
				m_AtomPosInRotamer.y,
				m_AtomPosInRotamer.z
				);
			a.atomPrimitive = m_AtomPrimitive;

			m_Molecule.addAtom( a );
		
			// put 'em all back
			a.Add(		   m_ReturnTranslation );
			m_Atom1.Add( m_ReturnTranslation );
			m_Atom2.Add( m_ReturnTranslation );
			m_Atom3.Add( m_ReturnTranslation );			
		}       


		private bool GetAllPositions()
		{
			string buildName = m_AtomPrimitive.AltName;

			SetPositionFromRotamer( m_AtomPosInRotamer, buildName ); // get our donor position

			FillPartnersForLevel1( buildName );
			if( m_Level1List.Count >= 3 )
			{
				// cool we already have enough part
				m_Atom1 = (Atom) m_Level1List.Get2(0);
				m_Atom2 = (Atom) m_Level1List.Get2(1);
				m_Atom3 = (Atom) m_Level1List.Get2(2);
				SetPositionFromRotamer( m_AtomPos1InRotamer, (string)m_Level1List.Get1(0) );
				SetPositionFromRotamer( m_AtomPos2InRotamer, (string)m_Level1List.Get1(1) );
				SetPositionFromRotamer( m_AtomPos3InRotamer, (string)m_Level1List.Get1(2) );
				return true; // cool we are done...
			}
			else
			{
				// we need to search for other partners
				for( int i = 0; i < m_Level1List.Count; i++ )
				{
					m_Atom1 = (Atom) m_Level1List.Get2(i);
					FillPartnersForLevel2( m_Atom1.ALTType );
					if( m_Level2List.Count >= 2 )
					{
						// we now have enough aditional valid atoms
						m_Atom2 = (Atom) m_Level2List.Get2(0);
						m_Atom3 = (Atom) m_Level2List.Get2(1);
						SetPositionFromRotamer( m_AtomPos1InRotamer, (string)m_Level1List.Get1(i) );
						SetPositionFromRotamer( m_AtomPos2InRotamer, (string)m_Level2List.Get1(0) );
						SetPositionFromRotamer( m_AtomPos3InRotamer, (string)m_Level2List.Get1(1) );
						return true;
					}
				}
				// could not find any level1's with enough vaid bonding partners
//				if( m_AtomPrimitive.BondingPartners.Length == 1 )
//				{
					// these are technically 3rd level rotation independent, and so can use a level-3 atom
					for( int i = 0; i < m_Level1List.Count; i++ )
					{
						m_Atom1 = (Atom) m_Level1List.Get2(i);
						FillPartnersForLevel2( m_Atom1.ALTType );
						for( int j = 0; j < m_Level2List.Count; j++ )
						{
							m_Atom2 = (Atom) m_Level2List.Get2(j);
							FillPartnersForLevel3( m_Atom2.ALTType );
							if( m_Level3List.Count >= 1 )
							{
								// we now have enough valid atoms
								m_Atom3 = (Atom) m_Level3List.Get2(0);
								SetPositionFromRotamer( m_AtomPos1InRotamer, (string)m_Level1List.Get1(i) );
								SetPositionFromRotamer( m_AtomPos2InRotamer, (string)m_Level2List.Get1(j) );
								SetPositionFromRotamer( m_AtomPos3InRotamer, (string)m_Level3List.Get1(0) );
								return true;
							}
						}
					}
					return false; // we couldnt find enough
//				}
//				else
//				{
//					// other atoms with more partners are dependent on the 
//					// relationship of the atom1-atom2-atom3-atom4 torsion, and so cannot be placed in this way
//					return false;
//				}
			}
		}

		#region HelperFunctions

		private void FillPartnersForLevel1( string altAtomName )
		{
			m_Level1List.Clear();
			AtomPrimitive sourceAtomP = AtomPrimitiveWithName( altAtomName );
			if( sourceAtomP == null ) return;
			string[] partners = sourceAtomP.BondingPartners;
			for( int i = 0; i < partners.Length; i++ )
			{
				Atom partner = AtomWithName( partners[i] );
				if( partner != null )
				{
					m_Level1List.Add( partners[i], partner );
				}
			}
		}

		private void FillPartnersForLevel2( string altAtomName )
		{
			m_Level2List.Clear();
			AtomPrimitive sourceAtomP = AtomPrimitiveWithName( altAtomName );
			if( sourceAtomP == null ) return;
			string[] partners = sourceAtomP.BondingPartners;
			for( int i = 0; i < partners.Length; i++ )
			{
				Atom partner = AtomWithName( partners[i] );
				if( partner != null && !m_Level1List.Contains2( partner ) )
				{
					m_Level2List.Add( partners[i], partner );
				}
			}
		}

		private void FillPartnersForLevel3( string altAtomName )
		{
			m_Level3List.Clear();
			AtomPrimitive sourceAtomP = AtomPrimitiveWithName( altAtomName );
			if( sourceAtomP == null ) return;
			string[] partners = sourceAtomP.BondingPartners;
			for( int i = 0; i < partners.Length; i++ )
			{
				Atom partner = AtomWithName( partners[i] );
				if( partner != null && !m_Level1List.Contains2( partner ) && !m_Level2List.Contains2( partner ) )
				{
					m_Level3List.Add( partners[i], partner );
				}
			}
		}

		private AtomPrimitive AtomPrimitiveWithName( string searchString )
		{
			for ( int k = 0; k < m_MolPrim.AtomPrimitiveCount; k++ )
			{
				if( m_MolPrim[k].AltName == searchString )
				{
					return m_MolPrim[k];
				}
			}
			return null;
		}

		private Atom AtomWithName( string searchString )
		{
			Molecule molToSearch = m_Molecule;
			if( searchString[0] == '-' )
			{
				// the atom is in the previous residue
				searchString = searchString.Substring(1,3) + " ";
				molToSearch = molToSearch.Parent.previousMolecule( molToSearch );
			}
			else if ( searchString[0] == '+' )
			{
				// the atom is in the next residue
				searchString = searchString.Substring(1,3) + " ";
				molToSearch = molToSearch.Parent.nextMolecule( molToSearch );			
			}

			if( molToSearch != null )
			{
				for ( int k = 0; k < molToSearch.Count; k++ )
				{
					if( molToSearch[k].atomPrimitive.AltName == searchString )
					{
						return molToSearch[k];
					}
				}
			}
			return null;
		}

		private bool SetPositionFromRotamer( Position target, string name )
		{
			for( int i = 0; i < m_Rotamer.Count; i++ )
			{
				if( m_Rotamer[i].atomName == name )
				{
					target.setTo( m_Rotamer[i].position );
					return true;
				}
			}
			//throw new BuilderException("Requested PDBID was not found in the given rotamer definition");
			//we have a slight problem with HT1's not being defined by the rotamer library
			return false;
		}

		#endregion
	}
}
