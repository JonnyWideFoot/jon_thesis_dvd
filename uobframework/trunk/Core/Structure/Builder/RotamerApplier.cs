using System;
using System.Diagnostics;

using UoB.Core.Structure;
using UoB.Core.Structure.Primitives;
using UoB.Core.Primitives;
using UoB.Core.Primitives.Matrix;
using UoB.Core.FileIO.PDB;

namespace UoB.Core.Structure.Builder
{
	/// <summary>
	/// Summary description for RotamerApplier.
	/// </summary>
	public sealed class RotamerApplier
	{
		// main PS members
		private Molecule m_Molecule = null;
		private static MatrixRotation m_RotMat = new MatrixRotation();
		private static Object m_RotMatLock = new Object(); // thread safing for the roation matrix

		public RotamerApplier()
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
			}
		}

		public void ApplyRotamer( int rotamerID )
		{
			if( !(m_Molecule.moleculePrimitive is MoleculePrimitive) )
			{
				throw new BuilderException("MoleculePrimitive is not ForceField defined");
			}

			MoleculePrimitive molPrim = (MoleculePrimitive) m_Molecule.moleculePrimitive;
			if ( rotamerID >= molPrim.RotamerCount )
			{
				throw new BuilderException("The Rotamer ID was invalid, it is bigger than the number of definitions");
			}

			PDBAtomList rotamer = molPrim.GetRotamer(rotamerID); 

			if ( rotamer == null )
			{
				throw new BuilderException("The Rotamer ID was invalid");
			}

			int CIndex = -1;
			int CAIndex = -1;
			int CBIndex = -1;

			Position[] rotamerHolder = new Position[ m_Molecule.Count ]; // we only need to do this because we dont want to change the actual rotamer in memory
			// and because we dont nececarily want all the atoms
			for( int i = 0; i < m_Molecule.Count; i++ )
			{
				bool posIsSet = false;
				for( int j = 0; j < rotamer.Count; j++ )
				{
					if( m_Molecule[i].PDBType == rotamer[j].atomName )
					{
						if ( m_Molecule[i].atomPrimitive.PDBIdentifier == PDBAtom.PDBID_BackBoneC )
						{
							CIndex = i;
						} 
						else
							if ( m_Molecule[i].atomPrimitive.PDBIdentifier == PDBAtom.PDBID_BackBoneCB )
						{
							CBIndex = i;
						} 
						else
							if ( m_Molecule[i].atomPrimitive.PDBIdentifier == PDBAtom.PDBID_BackBoneCA )
						{
							CAIndex = i;
						}

						rotamerHolder[i].x = rotamer[j].position.x;
						rotamerHolder[i].y = rotamer[j].position.y;
						rotamerHolder[i].z = rotamer[j].position.z;
						posIsSet = true;
						break;
					}
				}
				if( !posIsSet ) throw new Exception("Rotamer atom is not present");
			}

			// to return the rotamer with cAlp at (0,0,0) to the current position of the AA
			Position returnTranslation = new Position( m_Molecule[CAIndex].x, m_Molecule[CAIndex].y, m_Molecule[CAIndex].z);
			Position rotamerTranslation = new Position( rotamerHolder[CAIndex].x, rotamerHolder[CAIndex].y, rotamerHolder[CAIndex].z );

			// we now need to translate the cAlp of both the rotamer and the AA to the origin
			for( int i = 0; i < m_Molecule.Count; i++ )
			{
				rotamerHolder[i].x -= rotamerTranslation.x;
				rotamerHolder[i].y -= rotamerTranslation.y;
				rotamerHolder[i].z -= rotamerTranslation.z;
				m_Molecule[i].x -= returnTranslation.x;
				m_Molecule[i].y -= returnTranslation.y;
				m_Molecule[i].z -= returnTranslation.z;
			}

			// Now we want the unit Positions from the cAlpPos to the nPos for both the rotamer and the molecule
			Vector molCAlpCBPosition = new Vector(m_Molecule[CBIndex].x, m_Molecule[CBIndex].y, m_Molecule[CBIndex].z); 
			// no need to minus the cAlp as it is 0,0,0

			Vector rotCAlpCBPosition = new Vector(rotamerHolder[CBIndex].x,rotamerHolder[CBIndex].y,rotamerHolder[CBIndex].z);
			// Vector need to minus the cAlp as it is 0,0,0

			// obtain normalised Positions for the two alignment Positions N->cAlp and C->cAlp

			double molLengthCBcAlp;
			double rotLengthCBcAlp;
			Vector.MakeUnitVector( molCAlpCBPosition, out molLengthCBcAlp );
			Vector.MakeUnitVector( rotCAlpCBPosition, out rotLengthCBcAlp );

			double dotProductCB = Vector.dotProduct( molCAlpCBPosition, rotCAlpCBPosition );
			Vector planeNormalCB = Vector.crossProduct( rotCAlpCBPosition, molCAlpCBPosition);
			double angleCB = Math.Acos( dotProductCB );

			lock ( m_RotMatLock )
			{
				m_RotMat.setToAxisRot( planeNormalCB, angleCB );
				m_RotMat.transform( rotamerHolder );
			}

			// this is fine - we still want the original frame of reference for the rotamer
			Vector molCAlpCPosition = new Vector(
				m_Molecule[CIndex].x - m_Molecule[CAIndex].x,
				m_Molecule[CIndex].y - m_Molecule[CAIndex].y,
				m_Molecule[CIndex].z - m_Molecule[CAIndex].z );

			Vector.MakeUnitVector( molCAlpCPosition );
			Vector normalToMolPlane = Vector.crossProduct( molCAlpCBPosition, molCAlpCPosition );

			Vector rotCAlpCPosition = new Vector(
				rotamerHolder[CIndex].x - rotamerHolder[CAIndex].x,
				rotamerHolder[CIndex].y - rotamerHolder[CAIndex].y,
				rotamerHolder[CIndex].z - rotamerHolder[CAIndex].z );

			Vector.MakeUnitVector( rotCAlpCPosition );
			Vector normalToRotPlane = Vector.crossProduct( molCAlpCBPosition, rotCAlpCPosition ); // can use the normalised mol Position here, its the same
			
			Vector.MakeUnitVector( normalToMolPlane );
			Vector.MakeUnitVector( normalToRotPlane );
			double planeDotProduct = Vector.dotProduct( normalToMolPlane, normalToRotPlane );
			double angleBetweenPlaneNormals = Math.Acos( planeDotProduct );

			// new bit for if normals face in the wrong direction


			Position copy1RotamerPlane = normalToRotPlane.ClonePosition;
			Position copy2RotamerPlane = normalToRotPlane.ClonePosition;

			lock ( m_RotMatLock )
			{
				m_RotMat.setToAxisRot( molCAlpCBPosition, angleBetweenPlaneNormals );
				m_RotMat.transform( copy1RotamerPlane );

				m_RotMat.setToAxisRot( molCAlpCBPosition, -angleBetweenPlaneNormals );
				m_RotMat.transform( copy2RotamerPlane );

				double ang1 = Position.AngleBetween( copy1RotamerPlane, normalToMolPlane );
				double ang2 = Position.AngleBetween( copy2RotamerPlane, normalToMolPlane );

				if ( ang1 > 0.001 )
				{
					angleBetweenPlaneNormals = -angleBetweenPlaneNormals;
				}

			}

			// end new bit


			lock ( m_RotMatLock )
			{
				// rotate around the previously aligned Calp to N Position
				m_RotMat.setToAxisRot( molCAlpCBPosition, angleBetweenPlaneNormals );
				m_RotMat.transform( rotamerHolder );
			}

			// make the final translation
			for( int i = 0; i < m_Molecule.Count; i++ )
			{
				if ( !m_Molecule[i].atomPrimitive.IsBackBone )
				{
					m_Molecule[i].x = rotamerHolder[i].x + returnTranslation.x;
					m_Molecule[i].y = rotamerHolder[i].y + returnTranslation.y;
					m_Molecule[i].z = rotamerHolder[i].z + returnTranslation.z;
				}
				else
				{
					m_Molecule[i].x += returnTranslation.x;
					m_Molecule[i].y += returnTranslation.y;
					m_Molecule[i].z += returnTranslation.z;
				}
			}

			// are we now done ??? who knows ?!!! !
		}
	}
}
