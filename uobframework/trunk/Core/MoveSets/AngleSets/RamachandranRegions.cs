using System;
using UoB.Core.Sequence;

namespace UoB.Core.MoveSets.AngleSets
{
	/// <summary>
	/// Summary description for RaamchandranRegions.
	/// </summary>
	public class RamachandranRegions
	{
		private RamachandranRegions()
		{
		}

		public static RamachandranBound GetBoundSymbol( char boundType )
		{
			switch( boundType )
			{
				// all residues
				case 'A':
					return RamachandranBound.Alpha;
				case 'B':
					return RamachandranBound.Beta;
				case 'L':
					return RamachandranBound.ReverseAlpha;

				// gly only
				case 'W':
					return RamachandranBound.GlyWest;
				case 'E':
					return RamachandranBound.GlyEast;
				case 'D':
					return RamachandranBound.GlyDispersed;

				// pro only
				case 'S':
					return RamachandranBound.ProCisAlpha;
				case 'N':
					return RamachandranBound.ProCisBeta;
				case 'M':
					return RamachandranBound.ProTransMiddle;

				// unknown / undefined
				case '_':
					return RamachandranBound.OutsideBounds;
				default:
					throw new ArgumentException("Invalid character given. No corresponding ramachandran bound type can be ascribed.");
			}
		}

		public static char GetBoundSymbol( RamachandranBound boundType )
		{
			switch( boundType )
			{
				// all residues
				case RamachandranBound.Alpha:
					return 'A';
				case RamachandranBound.Beta:
					return 'B';
				case RamachandranBound.ReverseAlpha:
					return 'L';
					
				// gly only
				case RamachandranBound.GlyWest:
					return 'W';
				case RamachandranBound.GlyEast:
					return 'E';
				case RamachandranBound.GlyDispersed:
					return 'D';
					
				// pro only
				case RamachandranBound.ProCisAlpha:
					return 'S';
				case RamachandranBound.ProCisBeta:
					return 'N';
				case RamachandranBound.ProTransMiddle:
					return 'M';
					
				// unknown / undefined
				case RamachandranBound.OutsideBounds:
					return '_';
				default:
					throw new NotImplementedException("CODE ERROR: in RamachandranRegions.GetBoundSymbol()");
			}
		}

		/// <summary>
		/// Hard coding the "named" regions of the Ramachandran plot
		/// 
		/// This maybe should be made more generalised by inclusion in the angleset files, or a separate
		/// "RamaRegion" file. But for now and for simplicity it will be hard-coded and residue specific
		/// i.e. Pro and Gly are the only special cases.
		/// 
		/// Another issue is that the regions are defined by rectangles on the plot. Maybe a bool grid 
		/// would be better, but this would need rather a lot more code ..
		/// 
		/// 2 Big warnings/issues about this function call ...
		/// NOTE 1: these regions should all be manually rechecked at a later data if precise regions are required: they are currently all either RAFT or "ruler on paper" defined.
		/// NOTE 3: ResID is a flagged enumeration. Ramachandran regions are only valid for either Gly or Pro-cis or Pro-trans on their own, OR any number of other 18 "standard" resifdues. 
		/// If there is a mix, then the regions are NOT valid. (Including a mix of Cis and Trans pro).
		/// </summary>
		/// <param name="resID">The single letter residue ID to which the Phi and Psi belond : NOTE : the case here IS sensitive, lower indicates a Cis residue</param>
		/// <param name="phi">The Phi bacbone torsion angle of the residue on the Ramachandran plot</param>
		/// <param name="psi">The Psi bacbone torsion angle of the residue on the Ramachandran plot</param>
		/// <returns></returns>
		public static RamachandranBound GetAngleClass( StandardResidues resID, double phi, double psi )
		{
			long ID = (long) resID;
			long testID = 1;
			if( resID == StandardResidues.None )
			{
				throw new ArgumentException("StandardResidues.None was specified. No Region can be defined");
			}
			bool isSingleFlag = false;
			for( int i = 1; i <= 64; i++ )
			{
				if( ID == testID )
				{
					isSingleFlag = true;
					break;
				}
				testID *= 2;
			}

			if( !isSingleFlag )
			{
				int numDefined = 0;
				if( ( resID & StandardResidues.G ) == StandardResidues.G )
				{
					numDefined++;
				}
				if( ( resID & StandardResidues.p ) == StandardResidues.p )
				{
					numDefined++;
				}
				if( ( resID & StandardResidues.P ) == StandardResidues.P )
				{
					numDefined++;
				}
				// if more than one flag is set AND any 1 or more of the above residues are set, then the ramachandran regions become undefinable
				// if more than one is set within the other 18 then we are still ok because they are the same for all regions.
				if( numDefined > 0 )
				{
					throw new ArgumentException("Multiple flags for \"StandardResidues resID\" were specified. No Region can be defined for multiple sections including more than one of: Gly, Trans-Pro, Cis-Pro, and the other 18 residues!");
				}
			}

			if( resID == StandardResidues.G ) // gly
			{
				// 5 posibilities exist for Gly

				// MODIFICATION for Gly of standard alpha region as used in RAFT
				//phi -145 to -15
				//psi -90 to 40
				if( IsInRangeBin( phi, -120, -30 ) && 
					IsInRangeBin( psi, -60, 30 ) )
				{
					return RamachandranBound.Alpha;
				}

				// MODIFICATION for Gly of standard lefthelical region as used in RAFT
				//phi 20 to 105
				//psi -25 to 90
				if( IsInRangeBin( phi, 20, 105 ) && 
					IsInRangeBin( psi, -25, 90 ) )
				{
					return RamachandranBound.ReverseAlpha;
				}

				// "WEST" Region of glycines Psi=180 ramachandran bound
				// phi 40 to 120
				// psi -180 to -110 and 120 to 180
				if( ( IsInRangeBin( psi, -180, -110 ) ||  IsInRangeBin( psi, 120, 180 ) )
					&& IsInRangeBin( phi, 40, 120 ) )
				{
					return RamachandranBound.GlyWest;
				}

				// "EAST" Region of glycines Psi=180 ramachandran bound
				// phi -120 to -50
				// psi -180 to -120 and 120 to 180
				if( ( IsInRangeBin( psi, -180, -120 ) ||  IsInRangeBin( psi, 120, 180 ) )
					&& IsInRangeBin( phi, -120, -50 ) )
				{
					return RamachandranBound.GlyEast;
				}

				// "Dispersed" Region of glycines Psi=180 ramachandran bound
				// phi 140 to 180 and -180 to -140
				// psi -180 to -120 and 110 to 180
				if( ( IsInRangeBin( psi, 140, 180 ) ||  IsInRangeBin( psi, -180, -140 ) )
					&& ( IsInRangeBin( phi, -180, -120 ) ||  IsInRangeBin( phi, 110, 180 ) ) 
					)
				{
					return RamachandranBound.GlyDispersed;
				}				
			}
			else if( resID == StandardResidues.P ) // trans Pro
			{
				// Trans Proline Alpha Region
				// phi -110 to -40
				// psi -60 to 20
				if(IsInRangeBin( phi, -110, -40 ) && IsInRangeBin( psi, -60, 20 ) )
				{
					return RamachandranBound.Alpha;
				}

				// Trans Proline Beta Region
				// phi -120 to -40
				// psi 110 to 180 and -180 to -160
				if( IsInRangeBin( phi, -120, -40 ) && 
					( IsInRangeBin( psi, 110, 180 ) || IsInRangeBin( psi, -180, -160 ) ) 
					)
				{
					return RamachandranBound.Beta;
				}

				// Trans Proline small middle region
				// phi -90 to -60
				// psi 40 to 90
				if( IsInRangeBin( phi, -90, -60 ) && 
					IsInRangeBin( psi, 40, 90 ) 
					)
				{
					return RamachandranBound.ProTransMiddle;
				}
			}
			else if( resID == StandardResidues.p ) // cis pro
			{

				// Cis Proline Alpha Region
				// phi -110 to -40
				// psi -60 to 20
				if( IsInRangeBin( phi, -110, -40 ) && IsInRangeBin( psi, -60, 20 ) )
				{
					return RamachandranBound.ProCisAlpha;
				}

				// Cis Proline Beta Region - aka "South"
				// phi
				// psi 
				if( IsInRangeBin( phi, -120, -40 ) && 
					( IsInRangeBin( psi, 110, 180 ) || IsInRangeBin( psi, -180, -160 ) ) 
					)
				{
					return RamachandranBound.ProCisBeta;
				}
			}
			else // all other residues
			{
				// standard beta region as used in RAFT
				//phi -180 to -28
				//psi -180 to -155 and 53 to 180
				if( IsInRangeBin( phi, -180, -28 ) && 
					( IsInRangeBin( psi, 53, 180 ) || IsInRangeBin( psi, -180, -155 ) ) 
					)
				{
					return RamachandranBound.Beta;
				}

				// very minor extension of the beta region at the top right of the ramachandran plot
				// quoted as a "real" region in the "dissalowed regions of the ramachandran plot paper.
				//phi 170 to 180 - very thin
				//psi 120 to 180 - not the entire height of the normal beta region.
				if( IsInRangeBin( phi, 170, 180 ) && IsInRangeBin( psi, 120, 180 ) ) 
				{
					return RamachandranBound.Beta;
				}

				// standard alpha region as used in RAFT
				//phi -145 to -15
				//psi -90 to 40
				if( IsInRangeBin( phi, -145, -15 ) && 
					IsInRangeBin( psi, -90, 40 ) )
				{
					return RamachandranBound.Alpha;
				}

				// standard lefthelical region as used in RAFT
				//phi 20 to 105
				//psi -25 to 90
				if( IsInRangeBin( phi, 20, 105 ) && 
					IsInRangeBin( psi, -25, 90 ) )
				{
					return RamachandranBound.ReverseAlpha;
				}
			}

			// default to this if no other regions can be found.
			return RamachandranBound.OutsideBounds;
		}

		private static bool IsInRangeBin( double theValue, double min, double max )
		{
#if DEBUG   // debug assertion: this is only ever called by "hard-coded" function calls, 
			// and therefore if the following is the case, it can only be a code error, 
			// therefore only check it at debug time.
			if( min > max ) throw new ArgumentException("CODE ERROR: The min argument was larger than the max!");
#endif
			return theValue >= min 
				&& theValue <= max;
		}
	}
}
