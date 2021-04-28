using System;
using System.Text;
using System.Diagnostics;
using System.Collections;

using UoB.Core.Primitives;
using UoB.Core.Primitives.Matrix;
using UoB.Core.Structure;
using UoB.Core.FileIO.PDB;

namespace UoB.Core.Structure.Alignment
{
	/// <summary>
	/// Implementation of the ProSup Alignment Algorithm
	/// </summary> 
	internal sealed class Aligner_Geometric : Aligner
	{
		private int m_EquivCount = -1;

		public Aligner_Geometric( int expectedResidueLoading ) : base( expectedResidueLoading )
		{
		}

		public override void ReassignSystem(ParticleSystem sourcePS, AlignSourceDefinition def1, AlignSourceDefinition def2)
		{
			base.ReassignSystem(sourcePS, def1, def2);
			m_EquivCount = def1.Length;			
		}

		public override void Align()
		{
			DateTime dStart = DateTime.Now;

			m_SysDef.Models.Clear();
	
			Model m = new Model( m_EquivCount );
			for( int i = 0; i < m_EquivCount; i++ )
			{
				m.Equivalencies[i] = i;
			}
			m_SysDef.Models.AddModel( m );

			// all the work is done by the next function, all that is needed is the 
			// equiv list ....

			// sets up the position array for model viewing later
			SetupPositionStore();

			// final reporting to the user
			DateTime dEnd = DateTime.Now;
			TimeSpan duration = dEnd - dStart;

			string report = "Total geometric alignment took : " + duration.ToString();

			#if DEBUG
				Debug.WriteLine(report);
			#endif

			m_AlignReport.Append( report );

			m_AlignmentHasRun = true;
			// our work here is done

			base.Align(); // writes the molReport to the AlignSystemDefiniton
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