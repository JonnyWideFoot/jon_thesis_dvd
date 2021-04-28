using System;
using System.Text;
using System.Diagnostics;

using UoB.Core.Primitives;

namespace UoB.Core.FileIO.PDB
{
	/// <summary>
	/// Summary description for PDBAtom.
	/// </summary>
	public class PDBAtom : ICloneable
	{
		public static readonly string PDBID_BackBoneN =   " N  ";
		public static readonly string PDBID_BackBoneC =   " C  ";
		public static readonly string PDBID_BackBoneCA =  " CA ";
		public static readonly string PDBID_BackBoneCB =  " CB ";
		public static readonly string PDBID_BackBoneO =   " O  ";
		public static readonly string PDBID_BackBoneH   = " H  ";
		public static readonly string PDBID_BackBone1HA = " HA ";
		public static readonly string PDBID_BackBone2HA = "2HA ";

		private static object padLock = new object(); // used to lock the statics while in use
		private static char[] lineTypeAr = new char[6]; // 1-6
		private static char[] atomNumberAr = new char[5]; // 7-11
		private static char[] atomNameAr = new char[4]; // 13-16
		private static char[] altLocIndicatorAr = new char[1]; // 17
		private static char[] residueNameAr = new char[3]; // 18-20
		private static char[] chainIDAr = new char[1]; // 22
		private static char[] residueNumberAr = new char[4]; // 23-26
		private static char[] insertionCodeAr = new char[1]; // 27
		private static char[] xAr = new char[8]; // 31-38
		private static char[] yAr = new char[8]; // 39-46
		private static char[] zAr = new char[8]; // 47-54
		private static char[] occupancyAr = new char[6]; // 55-60
		private static char[] tempFactorAr = new char[6]; // 61-66
		private static char[] segIDAr = new char[4]; // 73-76
		private static char[] elementSymbolAr = new char[2]; // 77-78
		private static char[] chargeAr = new char[2]; //79-80

		// internal information
		public string lineType = "";
		public int atomNumber = -1;
		public string atomName = "";
		public char altLocIndicator = ' ';
		public string residueName = "";
		public char chainID = ' ';
		public int residueNumber = -1;
		public char insertionCode = ' ';
		public Position position = new Position();
		public float occupancy = 0.0f;
		public float tempFactor = 0.0f;
		public bool isSolvent = false;

		public PDBAtom()
		{
		}

		public PDBAtom( string line )
		{
			setFrom(line);
		}
		
		public void setFrom( string line )
		{
			//Make a char array that can be disected to form the PDBline elements
			if (line.Length < 80)
			{
				line = line.PadRight(80);
			}

			char[] theLine = line.ToCharArray(0,80);

			lock (padLock)
			{
				Array.Copy(theLine,0,lineTypeAr,0,6);
				Array.Copy(theLine,6,atomNumberAr,0,5);
				Array.Copy(theLine,12,atomNameAr,0,4);
				Array.Copy(theLine,16,altLocIndicatorAr,0,1);
				Array.Copy(theLine,17,residueNameAr,0,3);
				Array.Copy(theLine,21,chainIDAr,0,1);
				Array.Copy(theLine,22,residueNumberAr,0,4);
				Array.Copy(theLine,26,insertionCodeAr,0,1);
				Array.Copy(theLine,30,xAr,0,8);
				Array.Copy(theLine,38,yAr,0,8);
				Array.Copy(theLine,46,zAr,0,8);
				Array.Copy(theLine,54,occupancyAr,0,6);
				Array.Copy(theLine,60,tempFactorAr,0,6);
				Array.Copy(theLine,74,segIDAr,0,4);
				Array.Copy(theLine,76,elementSymbolAr,0,2);
				Array.Copy(theLine,78,chargeAr,0,2);

				lineType = new string(lineTypeAr).ToUpper();
				atomNumber = Int32.Parse(new string(atomNumberAr));
				atomName = new string(atomNameAr).ToUpper();
				altLocIndicator = altLocIndicatorAr[0];
				residueName = new string(residueNameAr).ToUpper();
				chainID = chainIDAr[0];
				residueNumber = 0;
				insertionCode = insertionCodeAr[0];

				// fix the crystalographers special residues to what they should be
				if( residueName == "MSE" ) 
				{
					residueName = "MET";
					if( atomName == "SE  " )
					{
						atomName = " SD ";
					}
				}
				else if( residueName == "CSE" ) 
				{
					residueName = "CYS";
					if( atomName == "SE  " )
					{
						atomName = " SG ";
					}
				}

				try { residueNumber = Int32.Parse(new string(residueNumberAr)); } 
				catch {}

				try { position.x = double.Parse(new string(xAr)); }
                catch { position.x = Double.MaxValue;  Debug.WriteLine("Exception thrown in PDBAtom: X"); }
				try { position.y = double.Parse(new string(yAr)); }
                catch { position.y = Double.MaxValue; Debug.WriteLine("Exception thrown in PDBAtom: Y"); }
				try { position.z = double.Parse(new string(zAr)); }
                catch { position.z = Double.MaxValue; Debug.WriteLine("Exception thrown in PDBAtom: Z"); }

				string occString = new string(occupancyAr).Trim();
				if( occString.Length != 0 )
				{
					try { occupancy = float.Parse(occString); } 
					catch { Debug.WriteLine("Exception thrown in PDBAtom: Occupancy" ); }
				}
				else
				{
					occupancy = -99.0f;
				}

				string tempfString = new string(tempFactorAr).Trim();
				if( tempfString.Length != 0 )
				{
					try { tempFactor = float.Parse(tempfString); } 
					catch { Debug.WriteLine("Exception thrown in PDBAtom: Temp Factor" ); }
				}
				else
				{
					tempFactor = -99.0f;
				}

				//string segID = new string(segIDAr);
				//string elementSymbol = new string(elementSymbolAr);
				//string charge = new string(chargeAr);
			}
		}
		#region ICloneable Members

		public object Clone()
		{
			PDBAtom a = (PDBAtom) MemberwiseClone();
			a.position = new Position( this.position ); 
			// otherwise the pointer will refer to the same atom ( you fool! ;-) )
			return a;
		}

		#endregion

		private static StringBuilder s = new StringBuilder(80);
		public override string ToString()
		{
			// typical line
			//ATOM   1076  N   LEU A 316      59.941  10.539  13.317  1.00 57.04           N  
			s.Append( lineType );
			s.Append( atomNumber.ToString().PadLeft(5,' ') );
			s.Append(" ");
			s.Append( atomName );
			s.Append(" ");
			s.Append( residueName );
			s.Append( " " );
			s.Append( chainID );
			s.Append( residueNumber.ToString().PadLeft(4,' ') );
			s.Append( insertionCode );
			s.Append( "   " );
			s.Append( position.xFloat.ToString("0.000").PadLeft(8,' ') );
			s.Append( position.yFloat.ToString("0.000").PadLeft(8,' ') );
			s.Append( position.zFloat.ToString("0.000").PadLeft(8,' ') );
			s.Append( occupancy.ToString("0.00").PadLeft(6,' ') ); // uncomment when we actually set them
			s.Append( tempFactor.ToString("0.00").PadLeft(6,' ') );

			// blank to end
			for( int i = s.Length; i < 80; i++ )
			{
				s.Append(' ');
			}
			string returnString = s.ToString();
			s.Remove(0, s.Length);
			return returnString;			
		}
	}
}
