using System;
using System.Text;
using System.Collections;

using UoB.Core.FileIO.PDB;
using UoB.Core.Primitives;
using UoB.Core.Structure;

namespace UoB.Core.Structure.Alignment
{
	/// <summary>
	/// Summary description for Aligner.
	/// </summary>
	internal abstract class Aligner
	{
		// method tools
		protected bool m_AlignmentHasRun = false;
		protected StringBuilder m_AlignReport;
		protected AlignmentSysDef m_SysDef;

		// protected position holders for use in the alignment process
		protected Position[] AtomXPositions; // these will contain ONLY C-Alpha positions
		protected Position[] AtomYPositions;
		protected Position[] AtomXBetaPositions = null; // these will contain ONLY C-Beta positions
		protected Position[] AtomYBetaPositions = null;
		protected int AtomXCount = -1;
		protected int AtomYCount = -1; // initialise to -1 to ensure array creation on constructor call

		public Aligner( int expectedMaxResCount )
		{
			m_AlignReport = new StringBuilder(2000);
			initialise( expectedMaxResCount );
		}

		private Options m_Options = new Options();
		public virtual Options OptionSettings
		{
			get
			{
				return m_Options;
			}
		}

		public virtual void ReassignSystem( ParticleSystem sourcePS, AlignSourceDefinition def1, AlignSourceDefinition def2 )
		{
			if( sourcePS == null )
			{
				throw new Exception("The particel system given on the Aligner reinitialise request was null!");
			}
			m_AlignmentHasRun = false;
			m_SysDef = new AlignmentSysDef( new PS_PositionStore( sourcePS ), 0, 1, def1, def2 );
			m_AlignReport.Remove(0,m_AlignReport.Length);

			AtomXCount = m_SysDef.PS1.Count; // the arrays can be larger than required for speed in allocation
			AtomYCount = m_SysDef.PS2.Count; // we therefore recored what index we are counting upto for both molecules

			if( AtomXPositions.Length < AtomXCount )
			{
				AtomXPositions = new Position[ AtomXCount ];
				AtomXBetaPositions = new Position[ AtomXCount ];
			}
			for( int i = 0; i < AtomXCount; i++ )
			{
				AtomXPositions[i] = new Position();
				AtomXBetaPositions[i] = new Position();
			}
			if( AtomYPositions.Length < AtomYCount )
			{
				AtomYPositions = new Position[ AtomYCount ];
				AtomYBetaPositions = new Position[ AtomYCount ];
			}
			for( int i = 0; i < AtomYCount; i++ )
			{
				AtomYPositions[i] = new Position();
				AtomYBetaPositions[i] = new Position();
			}

			ReGetAlphaPositionHolders();
			ReGetBetaPositionHolders();
		}

		public AlignmentSysDef SystemDefinition
		{
			get
			{
				return m_SysDef;
			}
		}

		public string Report
		{
			get
			{
				return m_AlignReport.ToString();
			}
		}

		public bool AlignmentHasRun
		{
			get
			{
				return m_AlignmentHasRun;
			}
		}

		public virtual void Align() // individual methods are responsible for the alignment process
		{
			m_SysDef.Report = m_AlignReport.ToString();
		}
		// so we mark this abstract. This class is intended as a framework to allow access to 
		// positioning information

		#region helper functions
		private void initialise( int expectedMaxResCount )
		{

			// use a Matrix for the positions of atomX[n] and atomY[n]
			// these must be extracted as the C-Alpha positions
			
			AtomXPositions = new Position[ expectedMaxResCount ];
			AtomXBetaPositions = new Position[ expectedMaxResCount ];
			AtomYPositions = new Position[ expectedMaxResCount ];
			AtomYBetaPositions = new Position[ expectedMaxResCount ];
			AtomXCount = expectedMaxResCount;
			AtomYCount = expectedMaxResCount;

			for( int i = 0; i < AtomXCount; i++ )
			{
				AtomXPositions[i] = new Position();
				AtomXBetaPositions[i] = new Position();
			}
			for( int i = 0; i < AtomYCount; i++ )
			{
				AtomYPositions[i] = new Position();
				AtomYBetaPositions[i] = new Position();
			}
		}

		protected void ReGetAlphaPositionHolders()
		{
			for( int i = 0; i < m_SysDef.PS1.Count; i++ )
			{
				Molecule m = m_SysDef.PS1[i];
				for( int j = 0; j < m.Count; j++ )
				{
					if( m[j].PDBType == PDBAtom.PDBID_BackBoneCA )
					{
						AtomXPositions[i].setTo( m[j] );
						goto NEXTPS1; // found one, we only want one per residue
					}
				}
				// we havent found it
				throw new Exception("A residue was found that did not define a C-Alpha Position... Alignment cannot be perfomed for the system.");
			NEXTPS1:
				continue; // found one
			}
			

			for( int i = 0; i < m_SysDef.PS2.Count; i++ )
			{
				Molecule m = m_SysDef.PS2[i];
				for( int j = 0; j < m.Count; j++ )
				{
					if( m[j].PDBType == PDBAtom.PDBID_BackBoneCA )
					{
						AtomYPositions[i].setTo( m[j] );
						goto NEXTPS2; // found one, we only want one per residue
					}
				}
				// we havent found it
				throw new Exception("A residue was found that did not define a C-Alpha Position... Alignment cannot be perfomed for the system.");
			NEXTPS2:
				continue; // found one
			}
			
		}

		protected void ReGetBetaPositionHolders()
		{
			for( int i = 0; i < m_SysDef.PS1.Count; i++ )
			{
				Molecule m = m_SysDef.PS1[i];
				for( int j = 0; j < m.Count; j++ )
				{
					if( m[j].PDBType == PDBAtom.PDBID_BackBoneCB )
					{
						AtomXBetaPositions[i].setTo( m[j] );
						goto DONESYS1; // found one, we only want one per residue
					}
				}
				// we havent found it
				AtomXBetaPositions[i].x = double.NegativeInfinity; // fast way to mark it as invalid
				DONESYS1:
					continue; // found one
			}
			

			for( int i = 0; i < m_SysDef.PS2.Count; i++ )
			{
				Molecule m = m_SysDef.PS2[i];
				for( int j = 0; j < m.Count; j++ )
				{
					if( m[j].PDBType == PDBAtom.PDBID_BackBoneCB )
					{
						AtomYBetaPositions[i].setTo( m[j] );
						goto DONESYS2; // found one, we only want one per residue
					}
					// we havent found it
					AtomYBetaPositions[i].x = double.NegativeInfinity; // fast way to mark it as invalid
				DONESYS2:
					continue; // found one
				}	
			}
		}
		#endregion
	}
}
