#define onlyEandHAreNonContinuous // controls the loop definition of secondary structure below

using System;
using System.Collections;
using System.IO;

namespace UoB.Core.FileIO.DSSP
{
	// ##############################################################
	// look at the top line of this file !!!
	// ##############################################################

	/// <summary>
	/// Summary description for FileDef.
	/// </summary>
	public class DSSPFile : BaseFileType_Reinitialisable
	{
		// the maximum SASA for a given residue on its own in DSSP
		// -999 for n/a
		protected readonly static double[] m_StoredResidueSASAs = new double[] { -999, 214,240,306,298,321,188,291,271,309,280,303,273,242,297,356,227,253,257,358,335, -999, -999 };

		protected ArrayList m_Residues;
		protected ArrayList m_LoopDefs;
		protected ArrayList m_SecondaryDefs;

		private bool m_DSSPFileIsJonFormat = true; // default to true 
		// DSSPFile_JonFormat : I edited the DSSP output to include the 
		// Omega angle Occupancy and Temperature factor

		public DSSPFile() : base()
		{
			m_Residues = new ArrayList();
			m_LoopDefs = new ArrayList();
			m_SecondaryDefs = new ArrayList();
		}

		public bool ImportDSSPFileIsJonFormat
		{
			get
			{
				return m_DSSPFileIsJonFormat;
			}
			set
			{
				m_DSSPFileIsJonFormat = value;
			}
		}

		public static float NullAngleValue
		{
			get
			{
				return 360.0f;
			}
		}

		public ResidueDef[] GetResidues()
		{
			return (ResidueDef[]) m_Residues.ToArray( typeof(ResidueDef) );
		}

		public int ResidueCount
		{
			get
			{
				return m_Residues.Count;
			}
		}

		#region Overridden Functions
		public override void Save(string fileName)
		{
			// do nothing, the m_CanSave is false
		}

		public override void Save(SaveParams saveParams)
		{
			// do nothing, the m_CanSave is false
		}

		protected override void ExtractExtendedInfo()
		{
		}

		public override void ClearFile()
		{
			m_LoopDefs.Clear();
			m_Residues.Clear();
			m_SecondaryDefs.Clear();
			base.ClearFile();
		}

		protected override void Initialise(string fileName)
		{
			base.Initialise (fileName);
			//try
			//{
				if( IsValidFile )
				{
					// fill from the file
					StreamReader re = new StreamReader( m_FileInfo.FullName );

					string line = null;
					for( int i = 0; i < 24; i++ )
					{
						line = re.ReadLine(); // read in blanking lines and discard
					}
					// make some assertions on the file contents based on this last line ....
					if( line == null )
					{
						throw new Exception("File doesnt contain the correct number of lines");
					}
					if( line.Length < 40 )
					{
						throw new Exception( "Header line is not the required size" );
					}
					if( 0 != String.Compare( line, 0, "  #  RESIDUE AA STRUCTURE BP1 BP2  ACC  ", 0, 40, false ) )
					{
						throw new Exception( "The title line of the DSSP file did not contain the correct string");
					}
					// we should now be in the correct body of the file, lets get the definitions
			
					while ( null != (line = re.ReadLine()) )
					{
						AddParse( line );
					}

					// all done with the file, the m_Residues collection is now filled
					re.Close();

					CalculateSegments(); // fill the m_LoopDefs and m_SecondaryDefs collections from the m_Residues information
				}
			//}
			//catch( Exception ex )
			//{
			//	ClearFile();
            //    throw ex; // rethrow
			//}
		}

		#endregion
		
		#region Parse Functions
		private void AddParse( string line )
		{ 
			//  #  RESIDUE AA STRUCTURE BP1 BP2  ACC     N-H-->O    O-->H-N    N-H-->O    O-->H-N    TCO  KAPPA ALPHA  PHI   PSI    OMG  Occupancy TempFactor X-CA   Y-CA   Z-CA 
			//    1    3   E              0   0    0      0, 0.0     2,-0.3     0, 0.0     0, 0.0   0.000 360.0 360.0 360.0  11.1 360.0   1.0    0.9   15.4   13.4   -4.1
			//    2    4   S        +     0   0    0      2,-0.0     2,-0.3    26,-0.0    81,-0.2  -0.995 360.0 176.1-146.8 135.6-178.4   1.0   70.9   15.3   17.2   -4.1

			ResidueDef def = new ResidueDef();

			def.FileIndex		=	int.Parse( line.Substring(0,5) );
			def.AminoAcidID	    =	line[13];

			if( def.AminoAcidID != '!' ) // '!' represents a DSSP unknown residue type in the PDB file
			{
				def.ResidueNumber	=	int.Parse(   line.Substring(5,  5) );
				def.InsertionCode   =   line[10];
				def.SecondaryType   =	line[16];
				def.Phi				=	double.Parse( line.Substring(103,6) );
				def.Psi				=	double.Parse( line.Substring(109,6) );
				if( m_DSSPFileIsJonFormat )
				{
					def.Omega		=	double.Parse( line.Substring(115,6) );
					def.occupancy	=	float.Parse( line.Substring(121,6) );
					def.tempFactor	=	float.Parse( line.Substring(128,6) );

					// not in use at the mo ...
					//def.x           =   float.Parse( line.Substring(134,7) );
					//def.y           =   float.Parse( line.Substring(141,7) );
					//def.z           =   float.Parse( line.Substring(148,7) );

					def.SASA        =   -999.0f; 					
					// SASA wouldnt compile once i had changed the code for omega 
					// parsing, we will therefoee just ignore it cos i dont need it...
				}
				else
				{
					def.Omega        =   -999.0f; 
					def.tempFactor   =   -999.0f;
					def.occupancy    =   -999.0f;
					// only relevent for my edited version of DSSP

					// parse this ...
					def.SASA         =   float.Parse( line.Substring(35 ,3) );
				}
				if( def.IsCisResidue ) // based on the omega value just added
				{
					def.AminoAcidID = char.ToLower( def.AminoAcidID );
				}
				else
				{
					def.AminoAcidID = char.ToUpper( def.AminoAcidID );
				}
			}
			else
			{
				// residueBreak, set to "nulls"
				def.ResidueNumber	=	-999;
				def.InsertionCode   =   '\0';
				def.SecondaryType   =	null2SChar;
				def.Phi				=	-999.0f;
				def.Psi				=	-999.0f;
				def.Omega			=   -999.0f;
				def.SASA			=   -999.0f;
				def.tempFactor      =   -999.0f;
				def.occupancy       =   -999.0f;
			}

			m_Residues.Add( def );
		}

		#endregion

		#region loop calculation Functions

		private const char null2SChar = ' '; // character meaning "no secondary structure classification in the DSSP file
		private const int minStretch_H = 5; // minimum length for a helix to be deemed continuous
		private const int minStretch_E = 3; // minimum length for a strand to be deemed continuous
		#if onlyEandHAreNonContinuous // complie time decision flag for my purposes ...
			// no extra variables needed
		#else
			// define a generic length for all other significant types
			private const int minStretch_Other = 5; // minimum length for a strand to be deemed continuous
		#endif
	
		/// <summary>
		/// Parses the current line of the file for the required residue information
		/// </summary>
		/// <param name="line"></param>
		private void CalculateSegments()
		{
			// DSSP Residue Secondary Structure Codes
			// H = alpha helix 
			// B = residue in isolated beta-bridge 
			// E = extended strand, participates in beta ladder 
			// G = 3-helix (3/10 helix) 
			// I = 5 helix (pi helix) 
			// T = hydrogen bonded turn 
			// S = bend 

			ResidueDef[] resDefs = (ResidueDef[])m_Residues.ToArray( typeof(ResidueDef) );	
			char[] types = new char[ resDefs.Length ];
			for( int i = 0; i < resDefs.Length; i++ )
			{
                types[i] = resDefs[i].SecondaryType;
			}

			// initialise the 1st loop
			SegmentDef loop = new SegmentDef(m_LoopDefs.Count);
			m_LoopDefs.Add( loop );

			int nextIndex = 0;
			int currentIndex = 0; // indexer
			while( currentIndex < types.Length )
			{
				if( isContinuous( types, currentIndex, ref nextIndex ) )
				{
					// if continuous then add as a secondary strcuture
					SegmentDef secStruct = new SegmentDef( m_SecondaryDefs.Count );
					for( int k = currentIndex; k < nextIndex; k++ )
					{
						ResidueDef secRes = resDefs[k];
						secStruct.AddResidue(secRes);
					}
					m_SecondaryDefs.Add( secStruct );

					// now define the next loop structure ...
					loop = new SegmentDef(m_LoopDefs.Count);
					m_LoopDefs.Add( loop );
				
					// reset counting for after the block
					currentIndex = nextIndex;
				}
				else
				{
					ResidueDef def = resDefs[currentIndex];
					loop.AddResidue(def); // our loop just got one bigger ...
					currentIndex++; // next cycle we look at the next residue along
				}
			}
		}


		private bool isContinuous( char[] types, int currentIndex, ref int nextIndex )
		{

			int minStretch = int.MaxValue; // null
			// minStretch is type dependent, set it below ...
			if( types[currentIndex] == 'E' )
			{
				minStretch = minStretch_E;
			}
			else if( types[currentIndex] == 'H' )
			{
				minStretch = minStretch_H;
			}
			else
			{
				#if onlyEandHAreNonContinuous
					return false; // only strands and helices can be continuous secondary structure with this define ...
				#else
				minStretch = minStretch_Other;
				#endif
			}

			if( currentIndex + minStretch >= types.Length ) // array bounds
			{
				nextIndex = currentIndex + 1;
				return false;
			}
			else // perform the assesment, the there a stretch of a minimum of 'minStretch' length
			{
				for( int i = 1; i < minStretch; i++ )
				{
					if( types[currentIndex+i] == null2SChar || types[currentIndex] != types[currentIndex+i] )
					{
						nextIndex = currentIndex + 1;
						return false;
					}
				}	
				// is continuous ... find out how far ...
				nextIndex = currentIndex + minStretch;
				while( nextIndex < types.Length )
				{
					if( types[nextIndex] != types[currentIndex] )
					{
						break; // stretch ended
					}
					else
					{
						nextIndex++;
					}
				}
				return true;
			}
		}


		public SegmentDef[] GetLoops( int length, bool includeTermini )
		{
			ArrayList ar = new ArrayList();
			int startAt = 0;
			int endAt = m_LoopDefs.Count;
			if( !includeTermini ) 
			{
				startAt++;
				endAt--;
			}
			for( int i = startAt; i < endAt; i++ )
			{
				SegmentDef ld = (SegmentDef) m_LoopDefs[i];
				if( length == ld.Length )
				{					
					ar.Add( ld );
				}
			}
			return (SegmentDef[]) ar.ToArray(typeof(SegmentDef));
		}

		public SegmentDef[] GetLoops( bool includeTermini )
		{
			ArrayList ar = new ArrayList();
			int startAt = 0;
			int endAt = m_LoopDefs.Count;
			if( !includeTermini ) 
			{
				startAt++;
				endAt--;
			}
			for( int i = startAt; i < endAt; i++ )
			{
				ar.Add( m_LoopDefs[i] );
			}	
			return (SegmentDef[]) ar.ToArray(typeof(SegmentDef));
		}

		public SegmentDef[] GetSecondaryStructures()
		{
			return (SegmentDef[]) m_SecondaryDefs.ToArray(typeof(SegmentDef));
		}

		// Note that the N and C termini are always there at positions 0 and n-1 even if the lengths are 0 ...
		public SegmentDef[] GetLoops()
		{
			return (SegmentDef[]) m_LoopDefs.ToArray(typeof(SegmentDef));
		}
		#endregion
	}
}
