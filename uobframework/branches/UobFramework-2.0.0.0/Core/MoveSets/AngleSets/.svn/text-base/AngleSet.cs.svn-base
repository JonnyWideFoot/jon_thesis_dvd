using System;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

using UoB.Core.Structure;
using UoB.Core;

namespace UoB.Core.MoveSets.AngleSets
{
	/// <summary>
	/// Summary description for AngleSet.
	/// </summary>
	public class AngleSet
	{
		private string m_Name = null;
		private ResidueAngleSet[] m_ResidueAngles = null;
		private Regex m_Regex = UoB.Core.Tools.CommonTools.WhiteSpaceRegex;

		public AngleSet( string fileName )
		{
			Init( fileName );
		}

		public AngleSet( SpecifiedAngleSet setName )
		{
			string fileName = CoreIni.Instance.DefaultSharedPath;

			// specify the required filename by the enumeration
			switch( setName )
			{
				case SpecifiedAngleSet.Default:
					fileName += "Default.angleset";
					break;
				case SpecifiedAngleSet.OriginalRAFT:
					fileName += "OriginalRAFT.angleset";
					break;
				default:
                    throw new NoImplementationException("SpecifiedAngleSet found with no implemenation!");
			}

			// Load the file ...
			if( !File.Exists(fileName) )
			{
				throw new Exception("Could not find the required default angleset file!");
			}
			Init( fileName );
		}

		public AngleSet() // load the defualt angle set from the shared directiry
		{
			Init( CoreIni.Instance.DefaultSharedPath + "Default.angleset" );
		}

		private void Init( string fileName )
		{
			if( !File.Exists( fileName ) )
			{
				throw new IOException("Cannot find the specified angle set definition file");
			}

			StreamReader reA = new StreamReader( fileName );
			AssertHeader( reA );

			for( int i = 0; i < m_ResidueAngles.Length; i++ )
			{
				ReadAngleSetBlock( i, reA );
			}

			reA.Close();		
		}

		/// <summary>
		/// Returns the number of residues in the angleset
		/// e.g.
		///%ANGLE_SET
		///%VERSION 1.0  
		///%DESCRIPTOR Original RAFT angleset
		///%RESIDUE_COUNT 20
		/// </summary>
		/// <param name="re"></param>
		/// <returns></returns>
		private void AssertHeader( StreamReader re )
		{
			string line = re.ReadLine();
			if( 0 != String.Compare(line,0,"%ANGLE_SET",0,10,true) )
			{
				throw new Exception("Angleset header line 1 is not %ANGLE_SET");
			}
			line = re.ReadLine();
			if( 0 != String.Compare(line,0,"%VERSION 1.0",0,12,true) )
			{
				throw new Exception("Angleset header line 2 is not %VERSION 1.0");
			}
			line = re.ReadLine();
			if( 0 != String.Compare(line,0,"%DESCRIPTOR ",0,12,true) )
			{
				throw new Exception("Angleset header line 3 is not %DESCRIPTOR ");
			}
			m_Name = line.Substring(12,line.Length-12);
			line = re.ReadLine();
			if( 0 != String.Compare(line,0,"%RESIDUE_COUNT ",0,15,true) )
			{
				throw new Exception("Angleset header line 4 is not %RESIDUE_COUNT ");
			}
			try
			{
				int resCount = int.Parse( line.Substring(15,line.Length-15) );
				m_ResidueAngles = new ResidueAngleSet[ resCount ];
			}
			catch
			{
				throw new Exception("Angleset header line 4s %RESIDUE_COUNT is not an integer");
			}
		}

		private void ReadAngleSetBlock( int index, StreamReader re )
		{
			string line = re.ReadLine();
			if( line == null )
			{
				throw new Exception("Unexpected end of file");
			}
			string[] lineparts = m_Regex.Split( line );
			// A	Ala	6	
			if( lineparts.Length < 3 )
			{
				throw new Exception("Error in anglset line parsing: The residue block header contained the wrong number of items");
			}
			char id = lineparts[0][0];
			string name = lineparts[1];
            int resCount = -1;
			try
			{
				resCount = int.Parse( lineparts[2] );
				if( resCount < 0 )
				{
					throw new Exception();
				}
			}
			catch
			{
				throw new Exception("Error in anglset line parsing: The residue block header contained an invalid anglecount");
			}
			
			double[] phis = new double[ resCount ];
			double[] psis = new double[ resCount ];
			double[] omegas = new double[ resCount ];
			float[] propensities = new float[ resCount ];
			char [] classes = new char[ resCount ];

			//-140	153	180	0.135	B
			for( int i = 0; i < resCount; i++ )
			{
				line = re.ReadLine();
				if( line == null )
				{
					throw new Exception("Unexpected end of file");
				}
				lineparts = m_Regex.Split( line );
				if( lineparts.Length < 5 )
				{
					throw new Exception("Error in anglset line parsing: The residue block header contained the wrong number of items");
				}
				try
				{
					phis[i] = double.Parse( lineparts[0] );
					psis[i] = double.Parse( lineparts[1] );
					omegas[i] = double.Parse( lineparts[2] );
					propensities[i] = float.Parse( lineparts[3] );
					classes[i] = lineparts[4][0];
				}
				catch
				{
					throw new Exception("Invalid angleset line");					
				}
			}

			m_ResidueAngles[index] = new ResidueAngleSet( id, name, phis, psis, omegas, propensities, classes );
		}

		public float CalcARMS( PolyPeptide pp, string sequence, string conformer )
		{
			double totalSqr = 0.0f;

			for( int i = 1; i < sequence.Length - 1; i++ ) // should be +- 1
			{
				ResidueAngleSet angles = m_ResidueAngles[sequence[i]];
				int conformerID = int.Parse( conformer[i].ToString() );

				double anglePhi = angles.getPhi( conformerID );
				double anglePsi = angles.getPsi( conformerID );
				double realPhi = pp[i].phiAngle;
				double realPsi = pp[i].psiAngle;

				double phiDiff = anglePhi - realPhi;
				if( phiDiff < 0.0 )   phiDiff = -phiDiff;
				if( phiDiff > 180.0 ) phiDiff = 360.0 - phiDiff;
				totalSqr += Math.Pow( phiDiff, 2.0 );

				double psiDiff = anglePsi - realPsi;
				if( psiDiff < 0.0 )   psiDiff = -psiDiff;
				if( psiDiff > 180.0 ) psiDiff = 360.0 - psiDiff;
				totalSqr += Math.Pow( psiDiff, 2.0 );
			}

			return (float) Math.Sqrt( totalSqr / (2.0*(double)(sequence.Length-2)));
		}

		public string GetBestConformerString( PolyPeptide poly )
		{
			string conformer = "6";

			int countTo = poly.Count - 1;
			for( int i = 1; i < countTo; i++ )
			{
				int conf = ClosestIDTo( poly[i] );		
				conformer += conf.ToString()[0];
			}

			conformer += '6';

			return conformer;
		}

		public int[] GetBestConformer( PolyPeptide poly )
		{
			int[] conf = new int[ poly.Count ];
			conf[0] = 6;
			conf[ conf.Length - 1 ] = 6;

			for( int i = 1; i < conf.Length - 1; i++ )
			{
				conf[i] = ClosestIDTo( poly[i] );		
			}

			return conf;
		}

		public int ClosestIDTo( AminoAcid aa )
		{
			return ClosestIDTo( aa.moleculePrimitive.SingleLetterID, aa.phiAngle, aa.psiAngle );
		}

		public double ClosestDistanceTo( char molID, double phi, double psi )
		{
			double[] availPhis = GetPhis( molID );
			double[] availPsis = GetPsis( molID );
            return ClosestDistanceTo( availPhis, availPsis, phi, psi );
		}

		public static double ClosestDistanceTo( double[] availPhis, double[] availPsis, double phi, double psi )
		{
			double bestDistance = double.MaxValue;
			for( int i = 0; i < availPhis.Length; i++ )
			{
				double phiDiff = phi - availPhis[i];
				if( phiDiff < 0 ) phiDiff = -phiDiff;
				if( phiDiff > 180 ) phiDiff = 360.0 - phiDiff;
				double psiDiff = psi - availPsis[i];
				if( psiDiff < 0 ) psiDiff = -psiDiff;
				if( psiDiff > 180 ) psiDiff = 360.0 - psiDiff;

				double distance = Math.Sqrt( 
					Math.Pow( phiDiff, 2 ) + 
					Math.Pow( psiDiff, 2 ) 
					);
				if( distance < bestDistance )
				{
					bestDistance = distance;
				}
			}
			return bestDistance;
		}

		public int ClosestIDTo( char molID, double phi, double psi )
		{
            return ClosestIDTo( GetPhis( molID ), GetPsis( molID ), phi, psi );
		}

		public static int ClosestIDTo( double[] availPhis, double[] availPsis, double phiPoint, double psiPoint )		
		{
			double bestDistance = double.MaxValue;
			int bestID = -1;
			for( int i = 0; i < availPhis.Length; i++ )
			{
				double phiDiff = phiPoint - availPhis[i];
				if( phiDiff < 0 ) phiDiff = -phiDiff;
				if( phiDiff > 180 ) phiDiff = 360.0 - phiDiff;
				double psiDiff = psiPoint - availPsis[i];
				if( psiDiff < 0 ) psiDiff = -psiDiff;
				if( psiDiff > 180 ) psiDiff = 360.0 - psiDiff;

				double distance = Math.Sqrt( 
					Math.Pow( phiDiff, 2 ) + 
					Math.Pow( psiDiff, 2 ) 
					);
				if( distance < bestDistance )
				{
					bestDistance = distance;
					bestID = i;
				}
			}
			return bestID;
		}

		public int[] ConformersInSameBinAs_RAFTEXPLICIT( char molID, int angleID )
		{
			// DONT use me, not generic for all angle sets

			if( molID == 'G' )
			{
				switch( angleID )
				{
					case 1:
						return new int[] { 1 };
					case 2:
						return new int[] { 2 };
					case 3:
						return new int[] { 3 };
					case 4:
						return new int[] { 4 };
					case 5:
						return new int[] { 5, 6 };
					case 6:
						return new int[] { 5, 6 };
					default:
						throw new Exception("The ID givem is out of the angleID range");
				}
			}
			else if( molID == 'P' )
			{
				switch( angleID )
				{
					case 1:
						return new int[] { 1, 4 };
					case 2:
						return new int[] { 2, 3, 5, 6 };
					case 3:
						return new int[] { 2, 3, 5, 6 };
					case 4:
						return new int[] { 1, 4 };
					case 5:
						return new int[] { 2, 3, 5, 6 };
					case 6:
						return new int[] { 2, 3, 5, 6 };
					default:
						throw new Exception("The ID givem is out of the angleID range");
				}
			}
			else
			{
				switch( angleID )
				{
					case 1:
						return new int[] { 1, 2 ,3 };
					case 2:
						return new int[] { 1, 2 ,3 };
					case 3:
						return new int[] { 1, 2 ,3 };
					case 4:
						return new int[] { 4, 5 };
					case 5:
						return new int[] { 4, 5 };
					case 6:
						return new int[] { 6 };
					default:
						throw new Exception("The ID givem is out of the angleID range");
				}
			}
		}

		/// <summary>
		/// Returns the number of angles in a given class for a given residue.
		/// e.g. Ala in the Original raft angle set has three angles in the beta class
		/// </summary>
		/// <param name="molID"></param>
		/// <param name="angleID"></param>
		/// <returns></returns>
		public int TotalNumberOfAnglesInClassOf( char molID, int angleID )
		{
			ResidueAngleSet resSet = this[ molID ];
			char angleClass = resSet.getAngleClass( angleID );
			int count = 0;
			if( resSet == null )
			{
				throw new Exception("MolID not found");
			}
			// including itself
			for( int i = 0; i < m_ResidueAngles.Length; i++ )
			{
				if( angleClass == m_ResidueAngles[i].getAngleClass( angleID ) )
				{
					count++;
				}
			}
			return count;
		}

		public static int NumberOfAnglesInBinOf_RAFTEXPLICIT( char molID, int angleID )
		{
			// I wouldn't recomend using this function ... ever ...
			angleID++; // (convert from counting here from base 0 to RAFT counting from 1)
			if( molID == 'G' )
			{
				switch( angleID )
				{
					case 1:
						return 1;
					case 2:
						return 1;
					case 3:
						return 1;
					case 4:
						return 1;
					case 5:
						return 2;
					case 6:
						return 2;
					default:
						throw new Exception("The ID given is out of the angleID range");
				}
			}
			else if( molID == 'P' )
			{
				switch( angleID )
				{
					case 1:
						return 2;
					case 2:
						return 4;
					case 3:
						return 4;
					case 4:
						return 2;
					case 5:
						return 4;
					case 6:
						return 4;
					default:
						throw new Exception("The ID givem is out of the angleID range");
				}
			}
			else
			{
				switch( angleID )
				{
					case 1:
						return 3;
					case 2:
						return 3;
					case 3:
						return 3;
					case 4:
						return 2;
					case 5:
						return 2;
					case 6:
						return 1;
					default:
						throw new Exception("The ID given is out of the angleID range");
				}
			}
		}

		public int ResidueAnglesets
		{
			get
			{
				return m_ResidueAngles.Length;
			}
		}

		public int TotalAnglesInAllResidueAnglesets
		{
			get
			{
				int totalCount = 0;
				for( int i = 0; i < m_ResidueAngles.Length; i++ )
				{
					totalCount += m_ResidueAngles[i].AngleCount;
				}
				return totalCount;
			}
		}

		/// <summary>
		/// Returns the angle count for the current residue in the angleset 
		/// </summary>
		/// <param name="id">single letter molecule ID to query the angleset</param>
		/// <returns>Returns the number of angles or -1 if the id was invalid</returns>
		public int GetAngleCount( char id )
		{
			ResidueAngleSet rSet = this[id];
			if( rSet == null )
			{
				return -1;
			}
			else
			{
				return rSet.AngleCount;
			}
		}

		public float[] GetPropensities( char id )
		{
			ResidueAngleSet rSet = this[id];
			if( rSet == null )
			{
				throw new Exception("ID not found");
			}
			else
			{
				float[] propensity = new float[ rSet.AngleCount ];
				for( int i = 0; i < rSet.AngleCount; i++ )
				{
					propensity[i] = rSet.getPropensity(i);
				}
				return propensity;
			}
		}

		public double[] GetPhis( char id )
		{
			ResidueAngleSet rSet = this[id];
			if( rSet == null )
			{
				throw new Exception("ID not found");
			}
			else
			{
				double[] phis = new double[ rSet.AngleCount ];
				for( int i = 0; i < rSet.AngleCount; i++ )
				{
					phis[i] = rSet.getPhi(i);
				}
				return phis;
			}
		}

		public double[] GetPsis( char id )
		{
			ResidueAngleSet rSet = this[id];
			if( rSet == null )
			{
				throw new Exception("ID not found");
			}
			else
			{
				double[] psis = new double[ rSet.AngleCount ];
				for( int i = 0; i < rSet.AngleCount; i++ )
				{
					psis[i] = rSet.getPsi(i);
				}
				return psis;
			}
		}

		public ResidueAngleSet this[ int index ]
		{
			get
			{
				return m_ResidueAngles[index];
			}
		}

		public ResidueAngleSet this[ char type ]
		{
			get
			{
				for( int i = 0; i < m_ResidueAngles.Length; i++ )
				{
					if( type == m_ResidueAngles[i].ID )
					{
						return m_ResidueAngles[i];
					}
				}
				return null;
			}
		}
	}
}
