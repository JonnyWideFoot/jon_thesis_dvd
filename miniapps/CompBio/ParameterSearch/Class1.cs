using System;
using System.Collections;
using System.IO;
using System.Timers;

namespace ParamSearch
{

	class ParamOptimiser
	{

		private struct ConstType
		{
			public double Buffer;
			public double Const;
			public string name;
			public bool m_Fixed;
			public bool m_InUse;
			public ConstType(string theName, double initVal, bool fixedConst)
			{
				Buffer = initVal;
				Const = initVal;
				name = theName;
				m_Fixed = fixedConst;
				m_InUse = true;
			}
		}

		private struct MolType
		{
			public double DGtr;
			public double[] area;
			public string typeName;
            
			public MolType(string name, double DGtransfer, double[] theAreas )
			{
				typeName = name;
				DGtr = DGtransfer;
                area = theAreas;                          
			}

		}
   
		private ConstType[] m_Constants;
		private MolType[]  m_MolTypes;

		private Random m_Random = new Random();
		private double[] m_differenceSq;
		private double m_CurrentScore;
		private double m_BestScore = 1000000000;
		private double m_LastScore = 1000000000; // big number
		private string m_TypeArg = "";

		private string m_FilePath = @"x:\";
		private int m_PerturbCount = 0;
		private int m_TimerTickCount = 0;

		public ParamOptimiser(string molCategory)
		{
			m_TypeArg = molCategory;
			loadConfigFiles();
		}

		public ParamOptimiser()
		{
			loadConfigFiles();
		}

		private bool equivMolCat(string mol, string lineID)
		{
			try
			{
				if ( mol.ToUpper() == lineID.Split( new char[] {':',':'} )[0].ToUpper() )
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch
			{
				return false;
			}
		}

		private void loadConfigFiles()
		{
			ArrayList ar = new ArrayList();

			StreamReader re = File.OpenText(m_FilePath + "DAVEPseudoPDBSASAAreas.csv");

			string[] firstLine = re.ReadLine().Split(','); // titles and const names
			string[] secondLine = re.ReadLine().Split(','); // initial const values
			string[] thirdLine = re.ReadLine().Split(','); // fix const value ?
			int constColumnOffset = 4; // before "totals " column begins i.e. totals is column 5
			m_Constants = new ConstType[firstLine.Length - constColumnOffset];
			for ( int i = 0; i < m_Constants.Length; i++ )
			{
				bool fixedConst = false;
				if ( thirdLine[i+constColumnOffset] != "")
				{
					fixedConst = true;
				}


				m_Constants[i] = new ConstType(
					firstLine[i+constColumnOffset], 
					double.Parse(secondLine[i+constColumnOffset]),
					fixedConst
					);
			}
			
			string inputLine = null;
			while ((inputLine = re.ReadLine()) != null)
			{
				string[] theLineparts = inputLine.Split(',');

				string tempCheck = inputLine.TrimEnd(',');
				string[] tempChecks = tempCheck.Split(',');
				if ( tempChecks.Length < 5 ) 
				{
					log("Error In Line : " + inputLine);
					continue;
				}
                
				// 3 is the column containing the ignore directive
				if ( theLineparts[3] != "" ) continue;
				if ( m_TypeArg != "" )
				{
					if ( !equivMolCat(m_TypeArg, theLineparts[0]) ) // returns true if they are equiv hence !
					{
						continue;
					}
				}

				MolType thetype = new MolType();
				thetype.typeName = theLineparts[1];
				thetype.DGtr = double.Parse(theLineparts[2]);
				thetype.area = stringToFloatArray(theLineparts, 4);

				ar.Add(thetype);
			}

			m_MolTypes = new MolType[ar.Count];
			ar.CopyTo(m_MolTypes);

			m_differenceSq = new double[m_MolTypes.Length];

            re.Close();


			for ( int i = 0; i < m_Constants.Length; i++ )
			{

				ConstType ct = m_Constants[i];
				bool isUsed = false;
                
				foreach ( MolType mt in m_MolTypes )
				{
					if ( mt.area[i] > 0 )
					{
						isUsed = true;
						break;
					}
				}

				if ( isUsed == false )
				{
					m_Constants[i].m_Fixed = true;
                    m_Constants[i].Const = 0.0;
					m_Constants[i].Buffer = 0.0;
					m_Constants[i].m_InUse = false;
				}
                
			}  


			// initialise scoremap
			StreamWriter rs = new StreamWriter(@"c:\" + "scoremap_" + m_TypeArg + ".csv", false);
			string temp = "BestScore,lastScore";
			foreach ( ConstType type in m_Constants )
			{
				temp += ("," + type.name);
			}
				
			rs.WriteLine(temp);
			rs.Close();

			return;
		}

		private double[] stringToFloatArray(string[] theSting, int offset)
		{
         	double[] tmpFloats = new double[theSting.Length - offset];  
 			for ( int i = 0; i < tmpFloats.Length; i++ )
			{
				tmpFloats[i] = double.Parse(	theSting[i + offset] );
			}
			return tmpFloats;
		}


		private double calcTR(MolType theType)
		{
			double tot =  0.0f;
			for ( int i = 0; i < theType.area.Length; i++ )
			{
				tot += theType.area[i] * m_Constants[i].Buffer;
			}
			return tot;
		}
		private void changeConsts()
		{
			for ( int i = 0; i < m_Constants.Length; i++ )
			{
				m_Constants[i].Buffer = generateConst( ref m_Constants[i] );
			}
		}

		private void perturbConsts()
		{
			int pertCount = m_Random.Next(1,(m_Constants.Length / 3));	// used to perturn a random number of constants at the same time
			
			for ( int i = 0; i < pertCount; i++ )
			{

				m_PerturbCount++; // global counter

				int constIDtoMod = m_Random.Next(0,m_Constants.Length);

				while( m_Constants[constIDtoMod].m_Fixed == true )
				{
					constIDtoMod = m_Random.Next(0,m_Constants.Length);
				}

				m_Constants[constIDtoMod].Const += getSign() * (0.1 * m_Random.NextDouble());
				m_Constants[constIDtoMod].Buffer = m_Constants[constIDtoMod].Const;
            
				string s = "PERTURB! - Const:" 
					+ m_Constants[constIDtoMod].name
					+ " has been modified to: "
					+ m_Constants[constIDtoMod].Const;
			
				Console.WriteLine(s);
				log(s);
				rescore();

			} 
		}

		private int getSign() // returns -1, 0, or +1
		{
			return m_Random.Next(-1,2); // for some reason 2 means between -1 min and 1 max ???
		}

		private double generateConst(ref ConstType theConst)
		{
			int rand = getSign();
			if ( rand == 0 || theConst.m_Fixed == true) return theConst.Buffer;
			double inc = rand * (double)(0.001 * m_Random.NextDouble());
			return theConst.Buffer + inc; 

		}

		public void keepConsts()
		{
			m_TimerTickCount = 0;
			for ( int i = 0; i < m_Constants.Length; i++ )
			{
				m_Constants[i].Const = m_Constants[i].Buffer;
			}
		}

		public void resetConsts()
		{
			for ( int i = 0; i < m_Constants.Length; i++ )
			{
				m_Constants[i].Buffer = m_Constants[i].Const;
			}
		}

		public void printToScreen()
		{
			Console.WriteLine(m_CurrentScore.ToString() + "\n");
			for ( int i = 0; i < m_Constants.Length; i++ )
			{
				Console.WriteLine(
					"Const: " + m_Constants[i].name + " value is : " + m_Constants[i].Const.ToString() + "\n"
					);
			}
		}

		public void printToScoreMapFile()
		{

			StreamWriter rs = new StreamWriter(@"c:\" + "scoremap_" + m_TypeArg + ".csv", true);
			string temp = this.m_BestScore.ToString();
			temp += ("," + this.m_CurrentScore.ToString());
			foreach ( ConstType type in m_Constants )
			{
				if ( type.m_InUse )
				{
					temp += ("," + type.Const.ToString());
				}
				else 
				{
					temp += ",Not in Use.";
				}
			}
			rs.WriteLine(temp);
			rs.Close();

		}

		public void log(string s)
		{
			StreamWriter rs = new StreamWriter(m_FilePath + "log_" + m_TypeArg + ".csv", true);
			rs.WriteLine(s);
			rs.Close();
		}

		private void printToConstFile()
		{
			StreamWriter rw = new StreamWriter(m_FilePath + "constants_" + m_TypeArg + ".csv", false);
			rw.WriteLine("Best Score was : " + m_BestScore.ToString() + " At perturb Count: " + m_PerturbCount.ToString());
		
			string summedTitles = "";
			string summedConsts = "";

			for ( int i = 0; i < m_Constants.Length; i++ )
			{
				summedTitles += ( m_Constants[i].name + "," );
				if ( m_Constants[i].m_InUse )
				{
					summedConsts += ( m_Constants[i].Const.ToString() + "," );
				}
				else 
				{
					summedConsts += "Not in use.,";
				}
			}

			rw.WriteLine(summedTitles);
			rw.WriteLine(summedConsts);

			rw.WriteLine("");
			rw.WriteLine("Name,DGtr,calcDGtr,diff,diffSq");


			for ( int i = 0; i < m_MolTypes.Length; i++ )
			{
				string name = m_MolTypes[i].typeName;
				double experimentalDGtr = m_MolTypes[i].DGtr;
				double calc = calcTR(m_MolTypes[i]);
				double diff = m_MolTypes[i].DGtr - calc;
				double diffSq = (double) Math.Pow( diff, 2 );
				rw.WriteLine(
					name + 
					"," + 
					experimentalDGtr.ToString() +
					"," +
					calc.ToString() +
					"," +
					diff.ToString() +
					"," +
					diffSq.ToString()
					);
			}

			rw.WriteLine("Overall Score");
			rw.WriteLine(this.m_CurrentScore);

			rw.Close();
		}

		public void rescore()
		{
			m_CurrentScore = m_LastScore = scoreFunc();
			Console.WriteLine("Score Following Rand() : " + m_LastScore.ToString());
		}

		private double scoreFunc()
		{
			for ( int i = 0; i < m_MolTypes.Length; i++ )
			{
				double calc = calcTR(m_MolTypes[i]);
				double diff = m_MolTypes[i].DGtr - calc;
				m_differenceSq[i] = (double) Math.Pow( diff, 2 );
			}
			double tot = 0.0f;
			foreach ( double f in m_differenceSq ) {	tot += f;	}
			return (double) Math.Sqrt(tot);
		}

		public void startOptimise2()
		{
//			Console.WriteLine("Initial Score is: " + scoreFunc().ToString());
//
//			StreamWriter rw = new StreamWriter(@"g:\temp.csv", false);
//
//			int n = 100;
//
//
//			rw.WriteLine("Inc,const[0],score");
//
//			for ( int i = 0; i < 3000; i++)
//			{
//				m_Constants[0].Const = startVal + inc;
//				rw.WriteLine(
//					inc.ToString()
//					+ ","
//					+ m_Constants[0].Const.ToString()
//					+ ","
//					+ scoreFunc().ToString()
//					);
//                inc += 0.000001;             
//			}
//			
//			rw.Close();
//			;

		}

		public void startOptimise()
		{
			Console.WriteLine("Initial Score is: " + scoreFunc().ToString());

			while(true)
			{
				changeConsts();
				m_CurrentScore = scoreFunc();

				if ( m_CurrentScore < m_LastScore )
				{
					m_LastScore = m_CurrentScore;
					keepConsts();
					printToScreen();
					printToScoreMapFile();
				}
				else
				{
					resetConsts();
					m_TimerTickCount++;
				}

				if ( m_CurrentScore < m_BestScore )
				{
					m_BestScore = m_CurrentScore;
					printToConstFile();
				}

//				if ( m_LastScore < 0.01 )
//				{
//					printToConstFile();
//					break;
//				}

				if (m_TimerTickCount > 30000000)
				{
					m_TimerTickCount = 0;
					if ( this.m_TypeArg == "" )
					{
						perturbConsts();
					}
					else
					{
						break;
					}
				}
			}
		}


	}




	class Class1
	{
		[STAThread]
		static void Main(string[] args)
		{
			if ( args.Length > 0 )
			{
				ParamOptimiser opti = new ParamOptimiser(args[0]);
				opti.startOptimise();
			}
			else
			{
				ParamOptimiser opti = new ParamOptimiser();
				opti.startOptimise();
			}

		}
	}

}
