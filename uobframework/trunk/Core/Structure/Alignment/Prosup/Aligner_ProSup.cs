using System;
using System.Text;
using System.Diagnostics;
using System.Collections;

using UoB.Core.Primitives;
using UoB.Core.Primitives.Matrix;
using UoB.Core.Structure;
using UoB.Core.FileIO.PDB;

namespace UoB.Core.Structure.Alignment.Prosup
{
	/// <summary>
	/// Implementation of the ProSup Alignment Algorithm
	/// </summary> 
	public sealed class Aligner_ProSup : Aligner
	{
		// Prosup method specific members
		private ProsupOptions m_ProSupOptions;

		// best path 
		private ProSupPathRefinement m_PathRefinement = null;

		// initial seed validation arrays
		private MatrixRotation initRotMat = new MatrixRotation();
		private Position[] workingPosXAlpha; // temporary position holders
		private Position[] workingPosYAlpha;
		private Position workingPosXBeta;  // temporary position holder for beta carbon positions
		private Position workingPosYBeta;
		private int[] initInts; // holds 0,1,2....n-1,n

		// used in iterative Realocation as a temporary buffer
		private int[] m_PrevEquivalencies;

		public Aligner_ProSup( ProsupOptions options, int expectedResidueLoading ) : base( expectedResidueLoading )
		{
			// the temporary holders must be initialised
			m_ProSupOptions = options;

			// initialisation of holders has to be performed
			// first for the seed initialisers ..
			workingPosXAlpha = new Position[ m_ProSupOptions.Seed_InitialLength ]; // we want the CAlpha position
			workingPosYAlpha = new Position[ m_ProSupOptions.Seed_InitialLength ]; 
			workingPosXBeta  = new Position(); // the C-Beta position is later converted into the vector from CAlpha to CBeta
			workingPosYBeta  = new Position();

			initInts = new int[ m_ProSupOptions.Seed_InitialLength ];
			// init the integer array for the seeds and,
			// initialise the temporary holders with (0,0,0), we will set them later when required
			for( int i = 0; i < m_ProSupOptions.Seed_InitialLength; i++ )
			{
				initInts[i] = i;
				workingPosXAlpha[i] = new Position();
				workingPosYAlpha[i] = new Position();
			}

			// then iteration stuff
			m_PrevEquivalencies = new int[ expectedResidueLoading ];

			if( m_ProSupOptions.Refine_PerformPathRefignment )
			{
				m_PathRefinement = new ProSupPathRefinement( workingPosXAlpha, workingPosYAlpha, m_ProSupOptions.Refine_CSquared, m_ProSupOptions.Refine_GapPenalty );
			}
		}

		public override void ReassignSystem(ParticleSystem sourcePS, AlignSourceDefinition def1, AlignSourceDefinition def2 )
		{
			base.ReassignSystem( sourcePS,def1,def2);
			if( m_PrevEquivalencies.Length < AtomXCount )
			{
				m_PrevEquivalencies = new int[ AtomXCount ];
			}
		}

		public override Options OptionSettings
		{
			get
			{
				return m_ProSupOptions;
			}
		}

		public override void Align()
		{
			DateTime dStart = DateTime.Now;

			m_SysDef.Models.Clear();
			SeedGeneration();

			// now that we have all the seeds, we have to find out which atoms are within c of their 
			// positions in the rotation defined by the fragment
			// iterative cycles of atom addition and re-rotation then occur, and the resulting atom groupings
			// sorted by the number of aligned atoms

			int countTo = m_SysDef.Models.ModelCount; // needed as we are calling .RemoveAt()
			int  i = 0;
			while( i < countTo )
			{
				if( !IterativeEquivalencyRealocate( (SeedDefinition) m_SysDef.Models[i]) ) // returns false on fail
				{
					m_SysDef.Models.RemoveAt( i ); // the model is invalid
					countTo--;
				}
				else
				{
					i++; // model was fine and Equiv list was reallocated to the best result
				}
			}
			m_SysDef.Models.EradicateIdentical( m_ProSupOptions.Seed_InitialLength ); // some models may now be degenerate
			
			if( m_ProSupOptions.Refine_PerformPathRefignment )
			{
				m_SysDef.Models.ReduceModelCount( m_ProSupOptions.Refine_MaxModels ); // reduce the number of models passed for refinement
				SetupAndRefineModels();
				m_SysDef.Models.EradicateIdentical( m_ProSupOptions.Seed_InitialLength );
			}

			m_SysDef.Models.ReduceModelCount( 10 ); // max of 10 final models

			// sets up the position array for model viewing later
			SetupPositionStore();

			// final reporting to the user
			DateTime dEnd = DateTime.Now;
			TimeSpan duration = dEnd - dStart;

			string report = "Total Prosup alignment took : " + duration.ToString();

			#if DEBUG
				Debug.WriteLine(report);
			#endif

			m_AlignReport.Append( report );

			m_AlignmentHasRun = true;
			// our work here is done

			base.Align(); // writes the molReport to the AlignSystemDefiniton
		}

		private void SetupAndRefineModels()
		{
			for( int k = 0; k < m_SysDef.Models.ModelCount; k++ )
			{
				Model m = m_SysDef.Models[k];
				int count = 0;
				bool changes = true;
				int numChanges = 0;
				int changesAre1Count = 0; // sometimes there is a pivotPoint where a single change flicks on and off, we want to catch this and quite the loop anyway

				while(changes)
				{
					count++;
					// this initially uses the old equivelences, these will be refined below
					m.RotMatrix.setToOptimalRotation( AtomXPositions, AtomYPositions, m.Equivalencies );
					// this is the final step of the process and involves using this dynamic programming algorithm
					m_PathRefinement.Execute( m );

					changes = false;

					numChanges = 0;
					for( int i = 0; i < m.Equivalencies.Length; i++ )
					{
						if( m.Equivalencies[i] != m_PrevEquivalencies[i] )
						{
							changes = true;
							numChanges++;
							break;
						}                            
					}

					if( !changes ) 
					{
						break; // the exit condition
					}

					if( count > 10 ) // prob an infinate loop
					{
						if( numChanges == 1 )
						{
							changesAre1Count++;
							if( changesAre1Count >= 8 )
							{
								break; // weird loop
							}
						}
						else
						{
							changesAre1Count = 0;
						}
						if( count > 100 ) // wont converge
						{
							break;
						}
					}

					for( int i = 0; i <  m.Equivalencies.Length; i++ )
					{
						m_PrevEquivalencies[i] = m.Equivalencies[i];
					}
				}

				m.setCRMSFollowingAlignment( AtomXPositions, AtomYPositions );// get this new value
			}
		}

		private void SeedGeneration()
		{
			// To find an initial set of equivalent atoms, all possible fragments i and j
			// of length n of the two structures are superimposed. The pair (i,j)
			// defines a seed of n atom pairs if its rms error rij < c.
			// defaults for n and c are 5 and 3 respectively
			// n has to be small if one is interested in weak similarities

			int seedLength = m_ProSupOptions.Seed_InitialLength;
			int totalCount = m_SysDef.PS1.Count;
			float CRMSCutoffSquared = m_ProSupOptions.Seed_CutoffCRMSSquared;
			Position translationX = new Position(); // used below
			Position translationY = new Position();

			//Position.DebugWriteFile( AtomXPositions, AtomYPositions );

			for( int i = 0; i < AtomXCount - seedLength + 1; i++ )
			{
				for( int j = 0; j < AtomYCount - seedLength + 1; j++ )
				{

					// set the equiv positions, test CRMS is in limit, test that the beta-carbon vectors are ok
					// if all checks are passed at this stage then the seed is added as a potential model

					for( int k = 0; k < seedLength; k++ )
					{
						workingPosXAlpha[k].setTo( AtomXPositions[ k + i ] );
						workingPosYAlpha[k].setTo( AtomYPositions[ k + j ] );
					}

					// does not perform any transforms, but does retrieve optimal translations
					// for superimpositions of the geometric centers of the current equivalencies on (0,0,0)
					// the contents of the two position objects is overwritten with the new info
					initRotMat.getTranslation( translationX, translationY, workingPosXAlpha, workingPosYAlpha ); 
					
					// perform the required translations, both sets of CAlphas now have their geometric centers on (0,0,0)
					initRotMat.translate( workingPosXAlpha, translationX );
					initRotMat.translate( workingPosYAlpha, translationY );

					// get the rotation for superimposition of the CAlpha positons
					initRotMat.setToOptimalRotation( workingPosXAlpha, workingPosYAlpha, initInts );

					// transform - perform the rotation on all AtomY's
					initRotMat.transform( workingPosYAlpha );

					// initial check, have we aligned the fragment backwards ??
					if( 25.0f < Position.distanceSquaredBetween( workingPosXAlpha[0], workingPosYAlpha[0] ) )
					{
						goto FAIL; // the fragment is backwards and therefore shite
					}

					// are they all within cutoff ?
					double total = 0.0;
					for( int q = 0; q < seedLength; q++ )
					{
						total += Position.distanceSquaredBetween( workingPosXAlpha[q], workingPosYAlpha[q] );
					}
					total /= seedLength; // SPEEDUP : no square-root required, we are only using a "<" below
				
					// if the total CRMS is in the valid range, we could have a valid seed
					if( total < CRMSCutoffSquared )
					{
						// first check passed
						if( m_ProSupOptions.BetaFilter_C )
						{
							// then do some checking, otherwise we have alread passed
							// for each beta position, obtain the position, perform the same transforms, and then assess
							for( int k = 0; k < seedLength; k++ )
							{
								Position BetaX = AtomXBetaPositions[ k + i ]; // globals, musn't change relative to each other
								Position BetaY = AtomYBetaPositions[ k + j ];
								if( BetaX.x == double.NegativeInfinity || BetaY.x == double.NegativeInfinity ) continue; 
								// no betaCarbon is available, as default - consider valid by default - i.e. Gly
								workingPosXBeta.SetToAMinusB( BetaX, translationX );
								workingPosYBeta.SetToAMinusB( BetaY, translationY );
								initRotMat.transform( workingPosYBeta );

								// the buffered beta carbon and alpha carbon positions are now ok relative to each other
								// we now need to transform the position so that we compare the relative position of the
								// beta carbons when their C-Alpha atoms are in the same location

								workingPosYBeta.Minus( workingPosYAlpha[k] );
								workingPosYBeta.Add  ( workingPosXAlpha[k] );

								double distanceSquared = Position.distanceSquaredBetween( workingPosXBeta, workingPosYBeta );
								if( distanceSquared > m_ProSupOptions.BetaFilter_CutoffDistanceSquared )
								{
									goto FAIL; // the dreaded goto to avoid a bool assignment
								}
							}
						}

						// the seed passed all tests, make a model definition
						SeedDefinition model = new SeedDefinition( i, j, seedLength, totalCount );
						m_SysDef.Models.AddModel( model );
					}
					else
					{
						goto FAIL; // if not under cutoff ...
					}
				FAIL: // the betaCarbon test was failed
				continue;
				}
			}
		}

		private bool IterativeEquivalencyRealocate( SeedDefinition m )
		{
			bool changes;
			int count = 0;
            
			// cache once first
			for( int i = 0; i < AtomXCount; i++ )
			{
				m_PrevEquivalencies[i] = m.Equivalencies[i]; // m.Equivs was initialied to -1, with the seed equivs set
			}

			while(true)
			{
				m.RotMatrix.getTranslation( m.TranslationX, m.TranslationY, AtomXPositions, AtomYPositions, m.Equivalencies );
				m.RotMatrix.translate( AtomXPositions, m.TranslationX );
				m.RotMatrix.translate( AtomYPositions, m.TranslationY );
				m.RotMatrix.setToOptimalRotation( AtomXPositions, AtomYPositions, m.Equivalencies );
				m.RotMatrix.transform( AtomYPositions );

				RealocateAtoms( m ); // which ones are within the scope

				if( !m.removeEquivListIslands( m_ProSupOptions.Iterate_MinStretchLength, m_ProSupOptions.Iterate_MinTotalEquivsForValidity ) )
				{
                    return false; // returns false if doing so results in too few equivelencies for continuation
				}

				changes = false;
				for( int i = 0; i < AtomXCount; i++ )
				{
					if( m.Equivalencies[i] != m_PrevEquivalencies[i] )
					{
						changes = true;
						break;
					}                            
				}
				
				if( !changes ) 
				{
					break; // the exit condition
				}

				if( count > 35 ) // wont converge
				{
					break;
				}

				// set the old cache for equivs
				for( int i = 0; i <  AtomXCount; i++ )
				{
					m_PrevEquivalencies[i] = m.Equivalencies[i];
				}

				count++;
			}

			m.setEquivCountFollowingAlignment();
			m.setCRMSFollowingAlignment( AtomXPositions, AtomYPositions );
			return true;
		}

		private void RealocateAtoms( SeedDefinition m )
		{
			// now what atoms can we add to the seed ?
			// C-terminus first
			int n = m_ProSupOptions.Seed_InitialLength;
			int countJFrom = m.Mol2StartIndex + n;
			bool wasSet;
			double cSquared = m_ProSupOptions.Seed_CutoffCRMSSquared; // use this as the cutoff

			for( int i = m.Mol1StartIndex + n; i < AtomXCount; i++ ) 
			{
				// from the C-terminus of the fragment to the end of the structure
				// look for potential partners, taking possible gaps into account
				wasSet = false;
				for( int j = countJFrom; j < AtomYCount; j++ ) 
				{
					double distance = Position.distanceSquaredBetween( AtomXPositions[i], AtomYPositions[j] );
					if( distance < cSquared )
					{
						m.Equivalencies[i] = j;
						countJFrom = j + 1;
						wasSet = true;
						break;
					}
				}
				if( !wasSet )
				{
					m.Equivalencies[i] = -1;
				}
			}

			// now N-terminus
			countJFrom = m.Mol2StartIndex - 1;
			for( int i = m.Mol1StartIndex-1; i >= 0; i-- ) 
			{
				// from the C-terminus of the fragment to the end of the structure
				// look for potential partners, taking possible gaps into account

				wasSet = false;
				for( int j = countJFrom; j >= 0; j-- ) 
				{
					if( Position.distanceSquaredBetween( AtomXPositions[i], AtomYPositions[j] ) < cSquared )
					{
						m.Equivalencies[i] = j;
						countJFrom = j - 1;
						wasSet = true;
						break;
					}
				}
				if( !wasSet )
				{
					m.Equivalencies[i] = -1;
				}
			}  
		}

		private void SetupPositionStore()
		{
			for( int i = 0; i < m_SysDef.Models.ModelCount; i++ )
			{
				Position[] pos = new Position[ m_SysDef.particleSystem.Count ];
				for( int j = 0; j < pos.Length; j++ )
				{
					pos[j] = m_SysDef.particleSystem[j].ClonePosition;
				}

				Model m = m_SysDef.Models[i];

				// as we have shafted the relationship between the AtomCAlpha holers and m_SysDef.PS1/m_SysDef.PS2
				// we will have to re-get these - its also good practice as during rotations, some precision is lost
				ReGetAlphaPositionHolders();

				// get new translations
				m.RotMatrix.getTranslation( m.TranslationX, m.TranslationY, AtomXPositions, AtomYPositions, m.Equivalencies );
				m.RotMatrix.translate( AtomXPositions, m.TranslationX );
				m.RotMatrix.translate( AtomYPositions, m.TranslationY );

				// as were using a single pos holder, we cant use the general .Translate function of the MatrixRotation
				// for the m_SysDef.particleSystem but its easy code so copied below with small changes
				
				Position center = new Position(); // calc the center of the XAtomSystem
				for( int j = 0; j < m_SysDef.PS1.Atoms.Count; j++ )
				{
					pos[j].Minus( m.TranslationX );
					center.Add( pos[j] );
				}
				center.Divide( m_SysDef.PS1.Atoms.Count );
				for( int j = m_SysDef.PS1.Atoms.Count; j < m_SysDef.PS1.Atoms.Count + m_SysDef.PS2.Atoms.Count; j++ )
				{
					pos[j].Minus( m.TranslationY );
				}

				m.RotMatrix.setToOptimalRotation( AtomXPositions, AtomYPositions, m.Equivalencies );
				m.RotMatrix.transform( pos, m_SysDef.PS1.Atoms.Count, m_SysDef.PS2.Atoms.Count ); // only transform the y coordinates
             
				// finally apply the center of the AtomX's so that all models will be centered around 0,0,0 when viewed
				for( int j = 0; j < pos.Length; j++ )
				{
					pos[j].Minus( center );
				}													   

				m_SysDef.ModelStore.addPositionArray( pos, false );
			}

			if( m_SysDef.Models.ModelCount > 0 ) // potentially there may be no valid alignments
			{
				m_SysDef.ModelStore.Position = 1; // pos 0 is unaligned, 1 is the best model, 2,3,4.. are other models
			}

			m_SysDef.particleSystem.PhysicallyReCenterSystem();
		}
	}
}