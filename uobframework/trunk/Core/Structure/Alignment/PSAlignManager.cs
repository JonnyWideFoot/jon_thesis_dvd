using System;
using System.Diagnostics;
using System.IO;

using UoB.Core.Structure.Alignment;
using UoB.Core.Structure;
using UoB.Core.Primitives;
using UoB.Core.Primitives.Matrix;
using UoB.Core.FileIO.PDB;
using UoB.Core.FileIO.Tra;

using UoB.Core.Structure.Alignment.Prosup;

namespace UoB.Core.Structure.Alignment
{
	/// <summary>
	/// Summary description for PSAligner.
	/// </summary>
	public sealed class PSAlignManager
	{
		private AlignSourceDefinition m_Mol1;
		private AlignSourceDefinition m_Mol2;

		private PSMolContainer m_PS1Clone;
		private PSMolContainer m_PS2Clone;

		private AlignmentMethod m_Method;
		private int m_ExpectedResLoad;
		private Aligner m_Aligner;
		private bool m_SystemAssigned = false;

		public PSAlignManager( AlignmentMethod method, int expectedResidueLoad )
		{
			m_ExpectedResLoad = expectedResidueLoad; // set before method assignment
			AlignerMethod = method;
		}

		public void ResetPSMolContainers( Tra file, int indexA, int indexB )
		{
			ParticleSystem cloneSys = new ParticleSystem( file.InternalName + " Alignment of " + indexA.ToString() + " and " + indexB.ToString() );
			cloneSys.BeginEditing();

			int previousIndex = file.PositionDefinitions.Position;

			file.PositionDefinitions.Position = indexA; // set the positions to the required index for alignment
			m_PS1Clone = (PSMolContainer)file.particleSystem.MemberAt(0).Clone(); // we dont want to change the originals
			file.PositionDefinitions.Position = indexB;
			m_PS2Clone = (PSMolContainer)file.particleSystem.MemberAt(0).Clone(); // so clone them ...

			file.PositionDefinitions.Position = previousIndex;

			m_PS1Clone.ChainID = 'A'; // we dont a want chainID conflict between the 2 members
			m_PS2Clone.ChainID = 'B';

			cloneSys.AddMolContainer( m_PS1Clone );
			cloneSys.AddMolContainer( m_PS2Clone );

			cloneSys.EndEditing( true, true ); // we have our clones system fully defined

			setCentersToOrigin(); 
			// this changes the actual m_ClonePartSys members
			// mol2 will be moved again later, however it makes sense to have mol1's geometric center aligned to the origin
			
			m_Mol1 = new AlignSourceDefinition( file.FullFileName, new MolRange( file.particleSystem.MemberAt(0) ) );
			m_Mol2 = new AlignSourceDefinition( file.FullFileName, new MolRange( file.particleSystem.MemberAt(0) ) );
			m_Aligner.ReassignSystem( cloneSys, m_Mol1, m_Mol2 );

			m_SystemAssigned = true;
		}

		public void ResetPSMolContainers( string name, AlignSourceDefinition mol1, AlignSourceDefinition mol2 )
		{
			m_Mol1 = mol1;
			m_Mol2 = mol2;

			ParticleSystem cloneSys = new ParticleSystem( name );
			cloneSys.BeginEditing();

            m_PS1Clone = mol1.MolRange.CloneFromRange(); // we dont want to change the originals
    		m_PS2Clone = mol2.MolRange.CloneFromRange(); // so clone them ...

			m_PS1Clone.ChainID = 'A'; // we dont a want chainID conflict between the 2 members
			m_PS2Clone.ChainID = 'B';

			cloneSys.AddMolContainer( m_PS1Clone );
			cloneSys.AddMolContainer( m_PS2Clone );

			cloneSys.EndEditing( true, true ); // we have our clones system fully defined

			setCentersToOrigin(); 
			// this changes the actual m_ClonePartSys members
			// mol2 will be moved again later, however it makes sense to have mol1's geometric center aligned to the origin
			
			m_Aligner.ReassignSystem( cloneSys, m_Mol1, m_Mol2 );

			m_SystemAssigned = true;
		}

		public Options CurrentOptionSet
		{
			get
			{
				if( m_Aligner != null )
				{
					return m_Aligner.OptionSettings;
				}
				else
				{
					return null;
				}
			}
		}

		public void PerformAlignment()
		{
			if( !m_SystemAssigned )
			{
				throw new Exception("System has not been assigned prior to the PerformAlignment() call");
			}
			if( !m_Aligner.AlignmentHasRun )
			{
				m_Aligner.Align();
			}
		}

		#region Accessors
		public AlignmentSysDef SystemDefinition
		{
			get
			{
				if( !m_SystemAssigned )
				{
					throw new Exception("System has not been assigned prior to the SystemDefinition Accessor call");
				}
				if( !m_Aligner.AlignmentHasRun )
				{
                    m_Aligner.Align();					
				}
				return m_Aligner.SystemDefinition;
			}
		}
		public AlignmentMethod AlignerMethod
		{
			get
			{
				return m_Method;
			}
			set
			{
				if( m_Aligner == null || m_Method != value )
				{
					m_Method = value;
					m_SystemAssigned = false; // we are about to create a new aligner with an unassigned system
					switch( m_Method )
					{
						case AlignmentMethod.ProSup:
							m_Aligner = new Aligner_ProSup( new ProsupOptions(), m_ExpectedResLoad );
							break;
						case AlignmentMethod.Geometric:
							m_Aligner = new Aligner_Geometric( m_ExpectedResLoad );
							break;
						default:
							throw new ArgumentException("The Method Specified is not supported");
					}
				}
			}
		}
		#endregion


//		public float dRMS( int modelIndex )
//		{
//			// should be called following alignment, but if not ...
//			// aligner will be null if no alignment has been run
//			if( m_Aligner == null || !m_Aligner.AlignmentHasRun )
//			{
//				PerformAlignment(); // call the above function to setup and perform the alignment
//			}
//
//			// a DRMS is a distance RMS (Root Mean Squared Deviation) This is 3D superimposition independent, but is 
//			// alignment dependent - i.e. which residues you have said are equivelent. A distance matrix is made
//			// for both proteins, and then the differene in the distances for each of the equivelent pairings 
//			// is assessed for each protein. These are then squared summed, divided by the number of equivelences,
//			// and square rooted to get the end answer.
//
//			return m_Aligner.dRMS( modelIndex ); // aligner is asked 
//		}

//		public float cRMS( int modelIndex )
//		{
//			// should be called following alignment, but if not ...
//			// aligner will be null if no alignment has been run
//			if( m_Aligner == null || !m_Aligner.AlignmentHasRun )
//			{
//				PerformAlignment(); // call the above function to setup and perform the alignment
//			}
//
//            // a cRMS is a coordinate RMS(Root Mean Squared Deviation) This is both 3D Superimposition Dependent and
//			// alignment dependent. The distance between an atom in peptideX and the equivelent atom in peptideY
//			// is assessed, summed for each pair, divided by the number of equivelences and square rooted.
//
//			return m_Aligner.cRMS( modelIndex );
//		}



		// done public functions
		// now private helper functions
		#region Helper Functions
		private void setCentersToOrigin()
		{
			Position mol1Center = getGeometricCenter(m_PS1Clone);
			Position mol2Center = getGeometricCenter(m_PS2Clone);
			AtomList atoms1 = m_PS1Clone.Atoms;
			AtomList atoms2 = m_PS2Clone.Atoms;
			for( int i = 0; i < atoms1.Count; i++ )
			{
				atoms1[i].Minus( mol1Center );	
			}
			for( int i = 0; i < atoms2.Count; i++ )
			{
				atoms2[i].Minus( mol2Center );
			}
		}

		private Position getGeometricCenter(PSMolContainer mol)
		{
			Position molCenter = new Position();
			AtomList atoms = mol.Atoms;
			int count = atoms.Count;
			for( int i = 0; i < count; i++ )
			{
				molCenter.Add( atoms[i] );
			}
			molCenter.Divide( (double) count );
			return molCenter;
		}
		#endregion
	}
}
