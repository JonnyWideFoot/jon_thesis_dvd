using System;

namespace UoB.Core.FileIO.DSSP
{
	/// <summary>
	/// Summary description for ResidueDef.
	/// </summary>
	public class ResidueDef
	{
		public int FileIndex;
		public int ResidueNumber;
		public char InsertionCode;
		public char AminoAcidID;
		public char SecondaryType;
		public double Phi; // will be compared to doubles later, need double precision
		public double Psi; // will be compared to doubles later, need double precision
		public double Omega; // I edited DSSP to output omega as a 3rd column in the same region as Phi and Psi
		public float tempFactor; // ditto Omega
		public float occupancy; // ditto Omega
		public float SASA;

		public bool IsCisResidue
		{
			get
			{
				if( Omega > -90.0f && Omega < 90.0f )
				{
					return true;
				}

				// Shit, there are some !
//				if( AminoAcidID != 'P' )
//				{
//					if( Omega < -120.0f && Omega > -145.0f )
//					{
//						int a = 0;
//					}
//				}

				// Omega == -999.0f // null / uncalculated
				// Omega == 360.0f // N-terminal / chain break
				// Omega ~= 180.0f // trans peptide ...

				return false;				
			}
		}

		private bool IsInRange( double angle, double Start, double End )
		{
			return angle <= End && angle >= Start;
		}

		private bool IsInRange( double angle, double Start, double End, double Start2, double End2 )
		{
			return (angle <= End && angle >= Start)
				|| (angle <= End2 && angle >= Start2);
		}

		public bool PhiAndPsiNotNull
		{
			get
			{
				return( Phi != DSSPFile.NullAngleValue && Psi != DSSPFile.NullAngleValue );
			}
		}

		public bool IsInAllowedRegion
		{
			get
			{
				if( AminoAcidID == 'P' || AminoAcidID == 'p' )
				{
					// either cis or trans proline
					//phi -175 to -5
					//psi any
					if( IsInRange( Phi, -175, -5 ) )
					{
						return false;
					}
					else
					{
						return true;
					}
				}
				else if( Char.IsLower( AminoAcidID ) )
				{
					return true; // all non-pro cis residues are diallowed
				}
				else if( AminoAcidID == 'G' )
				{
					if( 
						// angle 1 : reverse helical as used in RAFT
						//phi 0 to 180
						//psi -100 to 100
						IsInRange( Phi, 0, 180 ) &&
						IsInRange( Psi, -100, 100 ) )
					{
						return false;
					}
					else if( 
						// alpha helical as used in RAFT
						//phi -180 to 0
						//psi -100 to 100
						IsInRange( Phi, -180, 0 ) &&
						IsInRange( Psi, -100, 100 ) )
					{
						return false;
					}
					else if( 
						// phi any angle
						// psi = 180 region as used in RAFT :
						// psi -180 to -100 and 100 to 180
						IsInRange( Psi, -180, -100, 100, 180 )  )
					{
						return false;
					}
					else
					{
						return true; // disallowed
					}
				}
				else
				{
					if( 
						// standard beta region as used in RAFT
						//phi -180 to -28
						//psi -180 to -155 and 53 to 180
						IsInRange( Phi, -180, -28 ) &&
						IsInRange( Psi, 53, 180, -180, -155 ) )
					{
						return false;
					}
					else if( 
						// standard alpha region as used in RAFT
						//phi -145 to -15
						//psi -90 to 40
						IsInRange( Phi, -145, -15 ) &&
						IsInRange( Psi, -90, 40 ) )
					{
						return false;
					}
					else if( 
						// standard lefthelical region as used in RAFT
						//phi 20 to 105
						//psi -25 to 90
						IsInRange( Phi, 20, 105 ) &&
						IsInRange( Psi, -25, 90 ) )
					{
						return false;
					}
					else 
					{
						return true;
					}
				}
			}
		}

		public static string OutputTitle 
		{
			get
			{
				return "FileIndex\tResidueNumber\tInsertionCode\tAminoAcidID\tSecondaryType\tPhi\tPsi\tOmega\tSASA";
			}
		}

		public override string ToString()
		{
			return String.Format( "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}", 
				new Object[] {FileIndex,ResidueNumber,InsertionCode,AminoAcidID,SecondaryType,Phi,Psi,Omega,SASA} );
		}

	}
}
