using System;
using System.IO;

using UoB.Core.Primitives.Matrix;
using UoB.Core.Primitives;
using UoB.Core.Structure;

namespace UoB.Core.Structure.Alignment
{
	public class Model
	{
		protected MatrixRotation m_RotMatrix; // as below
		protected Position m_TranslationX; // we MUST garuntee that the positions that end up in these containers are the
		protected Position m_TranslationY; // positions that will allow translation of the ORIGINAL positions in m_PartSys
		protected int m_EquivCount; // we dont want to count it when it is asked for, so store it when m_Equivs is changed
		protected int[] m_Equivalencies;
		protected double m_CRMS; // as defined by the pairings in the m_EquivCount

		public Model( int molLength )
		{
			// initialise equiv list to null
			m_Equivalencies = new int[ molLength ];
			for( int i = 0; i < m_Equivalencies.Length; i++ )
			{
				m_Equivalencies[i] = -1;
			}
			m_TranslationX = new Position();
			m_TranslationY = new Position();
			m_RotMatrix = new MatrixRotation();
			m_CRMS = -1.0;
		}

		public Model( StreamReader re )
		{
			SetFromFileRead(re);
		}

		public void SetFromFileRead( StreamReader re )
		{
			//		EQUIVS 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113
			//		EQUIVCOUNT 114
			//		ROTMATRIX 0.461103329240418,-0.706823227431974,0.536455631833648,-0.377322379399791,0.390997622272712,0.839493109787885,-0.803126105768783,-0.589509783243437,-0.0864099166345427
			//		XTRANSLATE -0.35273350726511,-0.0984043660235034,-0.504632557366021
			//		YTRANSLATE 0.258279090113031,0.32965525681275,-0.230296252375934
			//		CRMS 0.333492811401701

			string lineCache;
			string[] lineParts;
			
			lineCache = re.ReadLine().Trim();
			AssertLine( ref lineCache, "EQUIVS ", "EQUIVS definition not present in system definition");
			lineParts = lineCache.Split( new char[] { ',' } );
			m_Equivalencies = new int[ lineParts.Length ];
			for( int i = 0; i < m_Equivalencies.Length; i++ )
			{
				m_Equivalencies[i] = int.Parse( lineParts[i] );
			}

			lineCache = re.ReadLine().Trim();
			AssertLine( ref lineCache, "EQUIVCOUNT ", "EQUIVCOUNT definition not present in system definition");
			m_EquivCount = int.Parse(lineCache);				

			lineCache = re.ReadLine().Trim();
			AssertLine( ref lineCache, "ROTMATRIX ", "ROTMATRIX definition not present in system definition");
			lineParts = lineCache.Split( new char[] { ',' } );
			m_RotMatrix = new MatrixRotation();
			m_RotMatrix.r[0,0] = double.Parse(lineParts[0]);
			m_RotMatrix.r[0,1] = double.Parse(lineParts[1]);
			m_RotMatrix.r[0,2] = double.Parse(lineParts[2]);
			m_RotMatrix.r[1,0] = double.Parse(lineParts[3]);
			m_RotMatrix.r[1,1] = double.Parse(lineParts[4]);
			m_RotMatrix.r[1,2] = double.Parse(lineParts[5]);
			m_RotMatrix.r[2,0] = double.Parse(lineParts[6]);
			m_RotMatrix.r[2,1] = double.Parse(lineParts[7]);
			m_RotMatrix.r[2,2] = double.Parse(lineParts[8]);

			lineCache = re.ReadLine().Trim();
			AssertLine( ref lineCache, "XTRANSLATE ", "XTRANSLATE definition not present in system definition");
			lineParts = lineCache.Split( new char[] { ',' } );
			m_TranslationX = new Position();
			m_TranslationX.x = double.Parse(lineParts[0]);
			m_TranslationX.y = double.Parse(lineParts[1]);
			m_TranslationX.z = double.Parse(lineParts[2]);

			lineCache = re.ReadLine().Trim();
			AssertLine( ref lineCache, "YTRANSLATE ", "YTRANSLATE definition not present in system definition");
			lineParts = lineCache.Split( new char[] { ',' } );
			m_TranslationY = new Position();
			m_TranslationY.x = double.Parse(lineParts[0]);
			m_TranslationY.y = double.Parse(lineParts[1]);
			m_TranslationY.z = double.Parse(lineParts[2]);

			lineCache = re.ReadLine().Trim();
			AssertLine( ref lineCache, "CRMS ", "CRMS definition not present in system definition");
			m_CRMS = double.Parse(lineCache);
		}

		public void WriteDefinition( StreamWriter rw )
		{
			rw.Write("\t\tEQUIVS ");
			for( int i = 0; i < m_Equivalencies.Length; i++ )
			{
				rw.Write( m_Equivalencies[i] );
				if( i != (m_Equivalencies.Length - 1) )
				{
					rw.Write(',');
				}
			}
			rw.WriteLine();

			rw.Write("\t\tEQUIVCOUNT ");
			rw.Write( m_EquivCount.ToString() );
			rw.WriteLine();

			rw.Write("\t\tROTMATRIX ");

			rw.Write( m_RotMatrix.r[0,0] );
			rw.Write(',');
			rw.Write( m_RotMatrix.r[0,1] );
			rw.Write(',');
			rw.Write( m_RotMatrix.r[0,2] );
			rw.Write(',');
			rw.Write( m_RotMatrix.r[1,0] );
			rw.Write(',');
			rw.Write( m_RotMatrix.r[1,1] );
			rw.Write(',');
			rw.Write( m_RotMatrix.r[1,2] );
			rw.Write(',');
			rw.Write( m_RotMatrix.r[2,0] );
			rw.Write(',');
			rw.Write( m_RotMatrix.r[2,1] );
			rw.Write(',');
			rw.Write( m_RotMatrix.r[2,2] );
			rw.WriteLine();

			rw.Write("\t\tXTRANSLATE ");
			rw.Write( m_TranslationX.x + "," + m_TranslationX.y + "," + m_TranslationX.z );
			rw.WriteLine();

			rw.Write("\t\tYTRANSLATE ");
			rw.Write( m_TranslationY.x + "," + m_TranslationY.y + "," + m_TranslationY.z );
			rw.WriteLine();

			rw.Write("\t\tCRMS ");
			rw.Write( m_CRMS );
			rw.WriteLine();
		}

		// these are now set externally by the alignment process
//		internal void setInternalsFollowingAlignment( Position donorXTranslation, Position donorYTranslation, MatrixRotation donorRotMat ) // assumes that the structures ARE already overlayed
//		{
//			m_TranslationX.setTo( donorXTranslation );
//			m_TranslationY.setTo( donorYTranslation );
//			m_RotMatrix.setTo( donorRotMat );
//		}

		internal void setEquivCountFollowingAlignment()
		{
			m_EquivCount = 0;
			for( int i = 0; i < m_Equivalencies.Length; i++ )
			{
				if( m_Equivalencies[i] != -1 )
				{
					m_EquivCount++;
				}
			}
		}

		internal void setCRMSFollowingAlignment( Position[] p1, Position[] p2 )
		{
			m_CRMS = 0.0f;
			for( int i = 0; i < m_Equivalencies.Length; i++ )
			{
				if( m_Equivalencies[i] == -1 ) continue; // the pair are not equivs, therefore not a valid distance comparison
				m_CRMS += Math.Pow( ( p1[i].x - p2[ m_Equivalencies[i] ].x ), 2 );	
				m_CRMS += Math.Pow( ( p1[i].y - p2[ m_Equivalencies[i] ].y ), 2 );	
				m_CRMS += Math.Pow( ( p1[i].z - p2[ m_Equivalencies[i] ].z ), 2 );	
			}
			m_CRMS = Math.Sqrt( ( m_CRMS / ( m_Equivalencies.Length * 3 ) ) );

		}

		public double CRMS
		{
			get
			{
				return m_CRMS;
			}
		}


		public MatrixRotation RotMatrix
		{
			get
			{
				return m_RotMatrix;
			}
		}

		public Position TranslationX
		{
			get
			{
				return m_TranslationX;
			}
		}

		public Position TranslationY
		{
			get
			{
				return m_TranslationY;
			}
		}

		public int[] Equivalencies
		{
			get
			{
				return m_Equivalencies;
			}
		}

		public int numberEquivalencies
		{
			get
			{

				return m_EquivCount;
			}
		}

		public bool removeEquivListIslands( int islandSize, int maxRequired )
		{
			int totalNonMinus1 = 0;
			int numSinceLastMinus1 = 0;

			for( int i = 0; i < m_Equivalencies.Length; i++ )
			{
				if( m_Equivalencies[i] == -1 )
				{
					if( numSinceLastMinus1 > 0 )
					{
						if( numSinceLastMinus1 < islandSize )
						{
							int j = i - 1;
							int count = 0;
							while( count++ < numSinceLastMinus1 )
							{
								m_Equivalencies[j] = -1;
								totalNonMinus1--;
								j--;
							}
						}
						numSinceLastMinus1 = 0;
					}
				}
				else
				{
					totalNonMinus1++;
					numSinceLastMinus1++;
				}
			}
			// one last time, assuming length + 1 is a -1 to do the last "islandSize" items
			if( numSinceLastMinus1 > 0 )
			{
				if( numSinceLastMinus1 < islandSize )
				{
					int j = m_Equivalencies.Length - 1;
					int count = 0;
					while( count++ < numSinceLastMinus1 )
					{
						m_Equivalencies[j] = -1;
						totalNonMinus1--;
						j--;
					}
				}
				numSinceLastMinus1 = 0;
			}

			if( totalNonMinus1 < maxRequired )
			{
				return false;
			}
			else			
			{
				return true;
			}
		}

		#region Helper Functions
		private void AssertLine( ref string line, string checkStart, string exceptionOnFalse )
		{
			string lineType = line.Trim().Substring(0,checkStart.Length);
			if( lineType != checkStart )
			{
				throw new Exception( exceptionOnFalse );
			}
			line = line.Substring(checkStart.Length,line.Length-checkStart.Length);
		}

		private bool AssertLine( StreamReader re, string check )
		{
			return check == re.ReadLine().Trim().Substring(0,check.Length);
		}

		private void AssertLine( StreamReader re, string check, string exceptionOnFalse )
		{
			if( check == re.ReadLine().Substring(0,check.Length) )
			{
				return;
			}
			else
			{
				throw new Exception(exceptionOnFalse);
			}
		}
		#endregion

	}
}
