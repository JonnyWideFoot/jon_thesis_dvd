using System;
using System.IO;
using System.Collections;

using UoB.Core.MoveSets.AngleSets;

namespace UoB.Methodology.DSSPAnalysis.AngleFitting
{
	/// <summary>
	/// Summary description for AngleFittingSearch.
	/// </summary>
	public class AngleFittingEngine_Search : AngleFittingEngine_Base
	{
		// Member system purturbation parameters
		private const int m_NumberGlobalPerturb = 30; // the number you can have without an improvement
		private const int m_NumberMiniPerturb = 3;
		private const int m_NmberChangeMoveSize = 3;

		// Angle step parameters, change during simulaton as required
		private double m_StepSize = 0.0;
		private double m_DoubleStepSize = 0.0;
		
		// Simulation members
		private char m_TargetStructureGroup;
		private bool m_AttemptResume;
		private int m_SimMode;
		private DSSPReportingOn m_DSSPReportingOnFlag = DSSPReportingOn.All; // required for the validation information in the file output ...
		private double m_SimulationResultScore = Double.MaxValue;

		/// <summary>
		/// Angle fitting class
		/// Method: Monte carlo perturbation and minimisation of the angle set
		/// </summary>
		/// <param name="angleCount">
		/// How many angles to fit
		/// </param>
		/// <param name="resID">
		/// Characher of the required residue for angle fitting
		/// </param>
		/// <param name="searchMode">
		/// searchMode 0 = angles can explore all space
		/// searchMode 1 = 1st 6 angles are locked into their conformation block
		/// searchMode 2 = my sigmoidal scoreing system all space
		/// </param>
		public AngleFittingEngine_Search( string DSSPDatabaseName, DirectoryInfo di, int angleCount, char resID, int searchMode, char targetStructureGroup, bool attemptResume )
			: base( DSSPDatabaseName, di, angleCount, resID )
		{
			m_SimMode = searchMode;
			if( targetStructureGroup != 'B' && 
				targetStructureGroup != 'A' && 
				targetStructureGroup != 'L' )
			{
				throw new Exception("TargetStructureGroup is invalid! .. Can have 'A', 'L', or 'B' (All, Loop, Both)");
			}
			m_TargetStructureGroup = targetStructureGroup;
			m_AttemptResume = attemptResume;
		}

		private SearchFittingResumeData m_ResumeData = null;
		public override void GoAngleFitting()
		{	
			Console.WriteLine( "---------------------------------" );
			Console.WriteLine( "ANGLE FITTING SEARCH ENGINE v0.08" );
			Console.WriteLine( "---------------------------------" );

			Console.WriteLine( "Beginnning fitting procedure using params ..." );
			Console.Write("\tAttemptResume: ");
			Console.WriteLine( m_AttemptResume.ToString() );
			Console.Write("\tAngleCount:");
			Console.WriteLine( m_AngleCount.ToString() );
			Console.Write("\tMolID: ");
			Console.WriteLine( m_CurrentMolID.ToString() );
			Console.Write("\tSimMode: ");
			Console.WriteLine( m_SimMode.ToString() );
			Console.Write("\tTargetStructesToFit: ");
			Console.Write( m_TargetStructureGroup.ToString() );
			Console.WriteLine(" ... (B=Both,A=AllRes,L=Loop).");

			string outFullName = reportDirectory.FullName + GetOutputFilename();
			Console.Write("Writing to file : ");
			Console.WriteLine(outFullName);
			Console.WriteLine();

			m_ResumeData = new SearchFittingResumeData( outFullName );
					
			if( m_AttemptResume && File.Exists(outFullName) )
			{		
				m_ResumeData.ObtainResumeData( m_AngleCount, m_CurrentMolID );
				Console.WriteLine();
			}
			// Function parameter Append = m_AttemptResume; We only want to append if we are resuming....
			m_RepWriter = new StreamWriter( outFullName, m_AttemptResume );

			m_Time = DateTime.Now;
			if( m_ResumeData.HasCompleteAllResFitData )
			{
				Console.WriteLine("Run already complete : Not Running AllRes mode.");
			}
			else if( m_TargetStructureGroup == 'B' || m_TargetStructureGroup == 'A' )
			{
				m_DSSPReportingOnFlag = DSSPReportingOn.All;
				Console.Write("Obtaining \"All\" Angle Data...");
				m_CountTo = ObtainPhiPsiData( m_DSSPReportingOnFlag, DSSPIncludedRegions.OnlyDefinitelyGood, m_SingleResType );
				Console.WriteLine(" Done!");
				Simulate(); // will result in bestPhis and bestPsis being fitted with the best angle sets
			
				m_RepWriter.WriteLine( "Time taken : " + "ALL" + " " + GetOutputFilename() + " : " + ( ( DateTime.Now - m_Time )).ToString() );
				m_RepWriter.WriteLine();
			}
			else
			{
				Console.WriteLine("User flag disable : Not Running AllRes mode.");
			}
			

			m_Time = DateTime.Now;
			if( m_ResumeData.HasCompleteLoopFitData )
			{
				Console.WriteLine("Run already complete : Not Running Loop mode.");
			}
			else if( m_TargetStructureGroup == 'B' || m_TargetStructureGroup == 'L' )
			{
				m_DSSPReportingOnFlag = DSSPReportingOn.LoopsOnly;
				Console.Write("Obtaining \"Loop\" Angle Data...");
				m_CountTo = ObtainPhiPsiData( m_DSSPReportingOnFlag, DSSPIncludedRegions.OnlyDefinitelyGood, m_SingleResType );
				Console.WriteLine(" Done!");
				Simulate(); // will result in bestPhis and bestPsis being fitted with the best angle sets
			
				m_RepWriter.WriteLine( "Time taken : " + "LOOP" + " " + GetOutputFilename() + " : " + ( ( DateTime.Now - m_Time )).ToString() );
				m_RepWriter.WriteLine();
			}
			else
			{
				Console.WriteLine( "User flag disable : Not Running Loop mode." );
			}

			m_RepWriter.Close();			
		}

		public override string GetOutputFilename()
		{
			string molID = char.ToUpper(m_CurrentMolID).ToString();
			if( char.IsUpper(m_CurrentMolID) )
			{
				molID += 'T';
			}
			else
			{
				molID += 'C';
			}

			string stem = m_AngleCount.ToString() + "_" + molID + "_" + m_SimMode.ToString() + "_";
			if( MaxFileImport == int.MaxValue )
			{
				// no file limit ...
                return stem + "-1.csv";

			}
			else
			{
				return stem + MaxFileImport.ToString() + ".csv";
			}
		}


		private void Simulate()
		{
			switch( m_SimMode )
			{
				case 0:
					Simulate_Mode0();
					break;
				case 1:
					if( m_AngleCount < 6 )
					{
						throw new Exception("Explicit restricted angle binning requires at least 6 angles");
					}
					Simulate_Mode1();
					break;
				case 2:
					Simulate_Mode2();
					break;
				default:
					throw new Exception("unknown simmode given");
			}
		}

		private void Simulate_Mode0()
		{
			double bestLowInSizeChangeLoop;
			double bestLowInMiniLoop;
			double bestLowInGlobalLoop;

			double currentScore;
				
			int numberWithoutChangeMoveSizeHelp; //  while() loop counters for loop out-break below ...
			int numberWithoutMiniPerturbHelp;
			int numberWithoutGlobalPerturbHelp;			

			Init(); // initial angle values

			// set flags
			double initialRunStartScore = SystemScore(); // then initialise a starting score

			m_SimulationResultScore = initialRunStartScore;
			bestLowInGlobalLoop = initialRunStartScore;
			currentScore = initialRunStartScore;

			Console.WriteLine();
			Console.WriteLine("Initialisation has been performed:");
			Console.WriteLine("Starting angleset with score: {0}", m_SimulationResultScore );
			for( int i = 0; i < m_AngleCount; i++ )
			{
				Console.WriteLine("{0}. Phi:{1} Psi:{2}",i,assessPhis[i],assessPsis[i]);
			}
			Console.WriteLine();

			Console.WriteLine("Beginning the fitting process...");

			numberWithoutGlobalPerturbHelp = 0;
			while( numberWithoutGlobalPerturbHelp < m_NumberGlobalPerturb )
			{
				GlobalPerturbSystem(); // move all angles somewhere random

				// set flags
				bestLowInMiniLoop = double.MaxValue;
				numberWithoutMiniPerturbHelp = 0;
				while( numberWithoutMiniPerturbHelp < m_NumberMiniPerturb ) // make small local (+- 4 degrees in phis and psis) perturbations based on the mutation rate
				{
					MiniPerturbSystem();

					// set flags
					bestLowInSizeChangeLoop = double.MaxValue; // tells us when we have hit the bottom
					numberWithoutChangeMoveSizeHelp = 0;
					while( numberWithoutChangeMoveSizeHelp < m_NmberChangeMoveSize ) //allow 3 moves before we call the branch dead
					{
						// reset the current score
						currentScore = SystemScore();
						if ( currentScore < bestLowInSizeChangeLoop )
						{
							// set flags
							bestLowInSizeChangeLoop = currentScore;
							numberWithoutChangeMoveSizeHelp = 0; // it has helped for this one and therefore should be reset ...
						}
						else
						{
							numberWithoutChangeMoveSizeHelp++; // no help this time ...
						}
						// perform angle movement
						ChangeStep();
						CalculateMoves();
						UpdateWorkingSet();			
					}
				
					if( bestLowInSizeChangeLoop < bestLowInMiniLoop )
					{
						bestLowInMiniLoop = bestLowInSizeChangeLoop;
						numberWithoutMiniPerturbHelp = 0;

						// test for system best here, this folows the descent to the local minimum
						if( m_SimulationResultScore > bestLowInSizeChangeLoop )
						{
							m_SimulationResultScore = bestLowInSizeChangeLoop;
							PrintCurrentState(); // record the good result to a file every time 
						}
					}
					else
					{
						numberWithoutMiniPerturbHelp++;
					}
				}
				
				if( bestLowInMiniLoop < bestLowInGlobalLoop )
				{
					bestLowInGlobalLoop = bestLowInMiniLoop;
					numberWithoutGlobalPerturbHelp = 0;
					ConsolePrint_Help();
				}
				else
				{
					numberWithoutGlobalPerturbHelp++;
					ConsolePrint_NoHelp( numberWithoutGlobalPerturbHelp );
				}
			}
		}

		private void Simulate_Mode1()
		{
			// this function is a DIRECT copy of Simulate_Mode0, except that :
			//
			// SystemScore_Restricted() is called rather than SystemScore() 
			// GlobalPerturbSystem_Restricted() is called rather than GlobalPerturbSystem()
			// CalculateMoves_Restricted() not CalculateMoves();
			// at the start CreateRestrictionBins() is called.
			//
			// The rest is identical

			double bestLowInSizeChangeLoop;
			double bestLowInMiniLoop;
			double bestLowInGlobalLoop;

			double currentScore;
				
			int numberWithoutChangeMoveSizeHelp; //  while() loop counters for loop out-break below ...
			int numberWithoutMiniPerturbHelp;
			int numberWithoutGlobalPerturbHelp;			

			Init(); // initial angle values
			CreateRestrictionBins();
						
			// set flags
			m_SimulationResultScore = SystemScore_Restricted(); // then initialise a starting score
			bestLowInGlobalLoop = m_SimulationResultScore;
			currentScore = m_SimulationResultScore;

			Console.WriteLine();
			Console.WriteLine("Initialisation has been performed:");
			Console.WriteLine("Starting angleset with score: {0}", m_SimulationResultScore );
			for( int i = 0; i < m_AngleCount; i++ )
			{
				Console.WriteLine("{0}. Phi:{1} Psi:{2}",i,assessPhis[i],assessPsis[i]);
			}
			Console.WriteLine();

			Console.WriteLine("Beginning the fitting process...");

			numberWithoutGlobalPerturbHelp = 0;
			while( numberWithoutGlobalPerturbHelp < m_NumberGlobalPerturb )
			{
				GlobalPerturbSystem_Restricted(); // move all angles somewhere random

				// set flags
				bestLowInMiniLoop = double.MaxValue;
				numberWithoutMiniPerturbHelp = 0;
				while( numberWithoutMiniPerturbHelp < m_NumberMiniPerturb ) // make small local (+- 4 degrees in phis and psis) perturbations based on the mutation rate
				{
					MiniPerturbSystem();

					// set flags
					bestLowInSizeChangeLoop = double.MaxValue; // tells us when we have hit the bottom
					numberWithoutChangeMoveSizeHelp = 0;
					while( numberWithoutChangeMoveSizeHelp < m_NmberChangeMoveSize ) //allow 3 moves before we call the branch dead
					{
						// reset the current score
						currentScore = SystemScore_Restricted();
						if ( currentScore < bestLowInSizeChangeLoop )
						{
							// set flags
							bestLowInSizeChangeLoop = currentScore;
							numberWithoutChangeMoveSizeHelp = 0; // it has helped for this one and therefore should be reset ...
						}
						else
						{
							numberWithoutChangeMoveSizeHelp++; // no help this time ...
						}
						// perform angle movement
						ChangeStep();
						CalculateMoves_Restricted();
						UpdateWorkingSet();			
					}
				
					if( bestLowInSizeChangeLoop < bestLowInMiniLoop )
					{
						bestLowInMiniLoop = bestLowInSizeChangeLoop;
						numberWithoutMiniPerturbHelp = 0;

						// test for system best here, this folows the descent to the local minimum
						if( m_SimulationResultScore > bestLowInSizeChangeLoop )
						{
							m_SimulationResultScore = bestLowInSizeChangeLoop;
							PrintCurrentState(); // record the good result
						}
					}
					else
					{
						numberWithoutMiniPerturbHelp++;
					}
				}
				
				if( bestLowInMiniLoop < bestLowInGlobalLoop )
				{
					bestLowInGlobalLoop = bestLowInMiniLoop;
					numberWithoutGlobalPerturbHelp = 0;
					ConsolePrint_Help();
				}
				else
				{
					numberWithoutGlobalPerturbHelp++;
					ConsolePrint_NoHelp( numberWithoutGlobalPerturbHelp );
				}
			}
		}

		private void Simulate_Mode2()
		{
			// this function is a DIRECT copy of Simulate_Mode0, except that :
			// SystemScore_Sigmoid() not SystemScore()
			// CalculateMoves_Sigmoid() not CalculateMoves()

			double bestLowInSizeChangeLoop;
			double bestLowInMiniLoop;
			double bestLowInGlobalLoop;

			double currentScore;
				
			int numberWithoutChangeMoveSizeHelp; //  while() loop counters for loop out-break below ...
			int numberWithoutMiniPerturbHelp;
			int numberWithoutGlobalPerturbHelp;			

			Init(); // initial angle values
						
			// set flags
			m_SimulationResultScore = SystemScore_Sigmoid(); // then initialise a starting score
			bestLowInGlobalLoop = m_SimulationResultScore;
			currentScore = m_SimulationResultScore;

			Console.WriteLine();
			Console.WriteLine("Initialisation has been performed:");
			Console.WriteLine("Starting angleset with score: {0}", m_SimulationResultScore );
			for( int i = 0; i < m_AngleCount; i++ )
			{
				Console.WriteLine("{0}. Phi:{1} Psi:{2}",i,assessPhis[i],assessPsis[i]);
			}
			Console.WriteLine();

			Console.WriteLine("Beginning the fitting process...");

			numberWithoutGlobalPerturbHelp = 0;
			while( numberWithoutGlobalPerturbHelp < m_NumberGlobalPerturb )
			{
				GlobalPerturbSystem(); // move all angles somewhere random

				// set flags
				bestLowInMiniLoop = double.MaxValue;
				numberWithoutMiniPerturbHelp = 0;
				while( numberWithoutMiniPerturbHelp < m_NumberMiniPerturb ) // make small local (+- 4 degrees in phis and psis) perturbations based on the mutation rate
				{
					MiniPerturbSystem();

					// set flags
					bestLowInSizeChangeLoop = double.MaxValue; // tells us when we have hit the bottom
					numberWithoutChangeMoveSizeHelp = 0;
					while( numberWithoutChangeMoveSizeHelp < m_NmberChangeMoveSize ) //allow 3 moves before we call the branch dead
					{
						// reset the current score
						currentScore = SystemScore_Sigmoid();
						if ( currentScore < bestLowInSizeChangeLoop )
						{
							// set flags
							bestLowInSizeChangeLoop = currentScore;
							numberWithoutChangeMoveSizeHelp = 0; // it has helped for this one and therefore should be reset ...
						}
						else
						{
							numberWithoutChangeMoveSizeHelp++; // no help this time ...
						}
						// perform angle movement
						ChangeStep();
						CalculateMoves_Sigmoid();
						UpdateWorkingSet();			
					}
				
					if( bestLowInSizeChangeLoop < bestLowInMiniLoop )
					{
						bestLowInMiniLoop = bestLowInSizeChangeLoop;
						numberWithoutMiniPerturbHelp = 0;

						// test for system best here, this folows the descent to the local minimum
						if( m_SimulationResultScore > bestLowInSizeChangeLoop )
						{
							m_SimulationResultScore = bestLowInSizeChangeLoop;
							PrintCurrentState(); // record the good result
						}
					}
					else
					{
						numberWithoutMiniPerturbHelp++;
					}
				}
				
				if( bestLowInMiniLoop < bestLowInGlobalLoop )
				{
					bestLowInGlobalLoop = bestLowInMiniLoop;
					numberWithoutGlobalPerturbHelp = 0;	
					ConsolePrint_Help();
				}
				else
				{
					numberWithoutGlobalPerturbHelp++;
					ConsolePrint_NoHelp( numberWithoutGlobalPerturbHelp );
				}
			}
		}


		#region Simulation User Reporting
		private DateTime m_CheckPoint;
		private void ConsolePrint_Help()
		{
			Console.Write( "Global Perturb Helped. SimScore:" );
			Console.Write( m_SimulationResultScore );
			Console.Write( " Descent time: " );			
			Console.WriteLine( DateTime.Now - m_CheckPoint );
			m_CheckPoint = DateTime.Now;
		}

		private void ConsolePrint_NoHelp( int numNotHelped )
		{
			Console.Write( "Global Perturb No Help (" );
			Console.Write( numNotHelped );
			Console.Write( ") Descent time: " );			
			Console.WriteLine( DateTime.Now - m_CheckPoint );
			m_CheckPoint = DateTime.Now;
		}
		#endregion

		#region Perturbation Schemes
		private void MiniPerturbSystem()
		{
			for( int i = 0; i < m_AngleCount; i++ )
			{
				assessPhis[i] += (8 * m_Random.NextDouble() - 4); // values obtained are +- 4.0
				if( assessPhis[i] < -180 )
				{
					assessPhis[i] += 360.0f;
				}
				else if( assessPhis[i] > 180 )
				{
					assessPhis[i] -= 360.0f;
				}
				assessPsis[i] += (8 * m_Random.NextDouble() - 4);
				if( assessPsis[i] < -180 )
				{
					assessPsis[i] += 360.0f;
				}
				else if( assessPsis[i] > 180 )
				{
					assessPsis[i] -= 360.0f;
				}
			}
		}

		private void GlobalPerturbSystem()
		{
			for( int i = 0; i < m_AngleCount; i++ )
			{
				GlobalPerturb( ref assessPhis[i] );
				GlobalPerturb( ref assessPsis[i] );
			}
		}

		private void GlobalRestrictedPerturb( ref double perturb, double Start, double End )
		{
			double range = End - Start;
			perturb = Start + ( range * m_Random.NextDouble() );
		}

		private void GlobalRestrictedPerturb( ref double perturb, double Start, double End, double Start2, double End2 )
		{
			double range = (End - Start) + (End2 - Start2);
			perturb = Start + ( range * m_Random.NextDouble() );
			if( perturb > End ) // we are past the 1st block ....
			{
				perturb += Start2 - End; // add the gap jump back on
			}
		}

		private void GlobalPerturb( ref double ang )
		{
			ang = ( 360.0 * m_Random.NextDouble() ) - 180.0;
		}

		private void GlobalPerturbSystem_Restricted()
		{
			if( m_CurrentMolID == 'G' )
			{
				// angle 1 : reverse helical as used in RAFT
				// phi 0 to 180
				// psi -100 to 100
				GlobalRestrictedPerturb( ref assessPhis[0], 0, 180 );
				GlobalRestrictedPerturb( ref assessPsis[0], -100, 100 );

				// alpha helical as used in RAFT
				// phi -180 to 0
				// psi -100 to 100
				GlobalRestrictedPerturb( ref assessPhis[1], -180, 0 );
				GlobalRestrictedPerturb( ref assessPsis[1], -100, 100 );

				// phi any angle
				// psi = 180 region as used in RAFT :
				// psi -180 to -100 and 100 to 180
				for( int i = 2; i < 6; i++ )
				{
					GlobalPerturb( ref assessPhis[i] ); // any
					GlobalRestrictedPerturb( ref assessPsis[i], -180, -100, 100, 180 );
				}

				// any new angles given
				for( int i = 6; i < m_AngleCount; i++ )
				{
					GlobalPerturb( ref assessPhis[i] );
					GlobalPerturb( ref assessPsis[i] );
				}
			}
			else if( m_CurrentMolID == 'P' || m_CurrentMolID == 'p' )
			{
				//phi -175 to -5
				//psi any
				for( int i = 0; i < 6; i++ )
				{
					GlobalRestrictedPerturb( ref assessPhis[i], -175, -5 );
					GlobalPerturb( ref assessPsis[i] );
				}
				// any new angles given
				for( int i = 6; i < m_AngleCount; i++ )
				{
					GlobalPerturb( ref assessPhis[i] );
					GlobalPerturb( ref assessPsis[i] );
				}
			}
			else
			{
				// standard beta region as used in RAFT
				//phi -180 to -28
				//psi -180 to -155 and 53 to 180
				for( int i = 0; i < 3; i++ )
				{
					GlobalRestrictedPerturb( ref assessPhis[i], -180, -28 );
					GlobalRestrictedPerturb( ref assessPsis[i], 53, 180, -180, -155 );
				}
				// standard alpha region as used in RAFT
				//phi -145 to -15
				//psi -90 to 40
				for( int i = 3; i < 5; i++ )
				{
					GlobalRestrictedPerturb( ref assessPhis[i], -145, -15 );
					GlobalRestrictedPerturb( ref assessPsis[i], -90, 40 );
				}
				// standard lefthelical region as used in RAFT
				//phi 20 to 105
				//psi -25 to 90
				GlobalRestrictedPerturb( ref assessPhis[5], 20, 105 );
				GlobalRestrictedPerturb( ref assessPsis[5], -25, 90 );
				
				// any new angles given
				for( int i = 6; i < m_AngleCount; i++ )
				{
					GlobalPerturb( ref assessPhis[i] );
					GlobalPerturb( ref assessPsis[i] );
				}
			}
		}

		#endregion
		
		#region Simulation Loop Functions

		private float[] phiData_1; // 3 different regions for angle parametisation
		private float[] psiData_1; // these are the "restrcition bins"
		private float[] phiData_2;
		private float[] psiData_2;
		private float[] phiData_3;
		private float[] psiData_3;
		private float[] phiData_O;
		private float[] psiData_O;

		private void Init()
		{
			m_SimulationResultScore = Double.MaxValue;
			m_CheckPoint = DateTime.Now; // in user repirting, must be assigned here now as an initial value

			if( m_DSSPReportingOnFlag == DSSPReportingOn.All && m_ResumeData.HasAllResFitData )
			{
				for( int i = 0; i < m_AngleCount; i++ )
				{
					assessPhis[i] = m_ResumeData.ResumeAllResPhi[i];
					assessPsis[i] = m_ResumeData.ResumeAllResPsi[i];
				}
			}
			else if( m_DSSPReportingOnFlag == DSSPReportingOn.LoopsOnly && m_ResumeData.HasLoopFitData )
			{
				for( int i = 0; i < m_AngleCount; i++ )
				{
					assessPhis[i] = m_ResumeData.ResumeLoopPhi[i];
					assessPsis[i] = m_ResumeData.ResumeLoopPsi[i];
				}
			}
			else
			{
				for( int i = 0; i < m_AngleCount; i++ )
				{
					assessPhis[i] = i; // start from arbitary separated positions .. i.e. 1,1 2,2 3,3 etc ...
					assessPsis[i] = i;
				}
			}
		}

		private void CreateRestrictionBins()
		{
			// this is to restrict the data sets to certain specific regions
			ArrayList h1 = new ArrayList(); // phi region 1
			ArrayList s1 = new ArrayList(); // psi region 1
			ArrayList h2 = new ArrayList(); // phi region 2
			ArrayList s2 = new ArrayList(); // psi region 2
			ArrayList h3 = new ArrayList(); // phi region 3
			ArrayList s3 = new ArrayList(); // psi region 3
			ArrayList hO = new ArrayList(); // phi other
			ArrayList sO = new ArrayList(); // psi other

			// bin all these angles
			if( m_CurrentMolID == 'G' )
			{
				for( int i = 0; i < m_CountTo; i++ )
				{
					// angle 1 : reverse helical as used in RAFT
					// phi 0 to 180
					// psi -100 to 100
					if(RamachandranTools.AngleIsInRange( phiData[i], 0, 180 ) && 
						RamachandranTools.AngleIsInRange( psiData[i], -100, 100 ) )
					{
						h1.Add( phiData[i] );
						s1.Add( psiData[i] );
					}
					else if (
						// alpha helical as used in RAFT
						// phi -180 to 0
						// psi -100 to 100
						RamachandranTools.AngleIsInRange( phiData[i], 0, 180 ) && 
						RamachandranTools.AngleIsInRange( psiData[i], -100, 100 ) )
					{
						h2.Add( phiData[i] );
						s2.Add( psiData[i] );
					}
					else if (
						// phi = 180 region as used in RAFT
						// phi -180 to -100 and 100 to 180
						// psi any
						RamachandranTools.AngleIsInRange( phiData[i], -180, -100, 100, 180 ) )
					{
						h3.Add( phiData[i] );
						s3.Add( psiData[i] );
					}
					else
					{
						hO.Add( phiData[i] );
						sO.Add( psiData[i] );
					}
				}
			}
			else if( m_CurrentMolID == 'P' )
			{
				for( int i = 0; i < m_CountTo; i++ )
				{
					if( 
						//phi any
						//psi -175 to -5
						RamachandranTools.AngleIsInRange( psiData[i], -175, -5 ) )
					{
						h1.Add( phiData[i] );
						s1.Add( psiData[i] );
					}
					else
					{
						hO.Add( phiData[i] );
						sO.Add( psiData[i] );
					}
				}
			}
			else
			{
				for( int i = 0; i < m_CountTo; i++ )
				{
					// standard beta region as used in RAFT
					//phi -180 to -28
					//psi -180 to -155 and 53 to 180
					if( RamachandranTools.AngleIsInRange( phiData[i], -180, -28 ) && 
						RamachandranTools.AngleIsInRange( psiData[i], 53, 180, -180, -155 ) )
					{
						// we are in the beta region
						h1.Add( phiData[i] );
						s1.Add( psiData[i] );
					}
						// standard alpha region as used in RAFT
						//phi -145 to -15
						//psi -90 to 40
					else if( RamachandranTools.AngleIsInRange( phiData[i], -145, -15 ) && 
						RamachandranTools.AngleIsInRange( psiData[i], -90, 40 ) )
					{
						// we are in the alpha region
						h2.Add( phiData[i] );
						s2.Add( psiData[i] );
					}
						// standard lefthelical region as used in RAFT
						//phi 20 to 105
						//psi -25 to 90
					else if( RamachandranTools.AngleIsInRange( phiData[i], 20, 105 ) &&
						RamachandranTools.AngleIsInRange( psiData[i], -25, 90 ) )
					{
						// we are in the lefthelical region
						h3.Add( phiData[i] );
						s3.Add( psiData[i] );
					}
						// we are outside the RAFT regions
					else
					{
						hO.Add( phiData[i] );
						sO.Add( psiData[i] );
					}		
				}
			}

			phiData_1 = (float[])h1.ToArray(typeof(float));
			psiData_1 = (float[])s1.ToArray(typeof(float));
			phiData_2 = (float[])h2.ToArray(typeof(float));
			psiData_2 = (float[])s2.ToArray(typeof(float));
			phiData_3 = (float[])h3.ToArray(typeof(float));
			psiData_3 = (float[])s3.ToArray(typeof(float));
			phiData_O = (float[])hO.ToArray(typeof(float));
			psiData_O = (float[])sO.ToArray(typeof(float));
		}


		private void ChangeStep()
		{
			m_StepSize = m_Random.NextDouble();
			m_DoubleStepSize = m_StepSize * 2;
		}

		private void UpdateWorkingSet()
		{
			for( int i = 0; i < m_AngleCount; i++ )
			{
				assessPhis[i] = bestPhis[i];
				assessPsis[i] = bestPsis[i];
			}
		}

		private void PrintCurrentState()
		{
			m_RepWriter.Write(m_CurrentMolID);
			m_RepWriter.Write(',');
			m_RepWriter.Write(m_DSSPReportingOnFlag);
			m_RepWriter.Write(',');
			for( int i = 0; i < m_AngleCount; i++ )
			{
				m_RepWriter.Write(assessPhis[i]);
				m_RepWriter.Write(',');
				m_RepWriter.Write(assessPsis[i]);
				m_RepWriter.Write(',');
			}
			m_RepWriter.WriteLine( m_SimulationResultScore );
			m_RepWriter.Flush();
		}


		private bool CalculateMoves_Restricted()
		{
			// assessPhis[index]; //these MUST return to the same values on exit
			// assessPsis[index];

			// bestPhis[ index ] // these should contain the desired direction
			// bestPsis[ index ] // these should contain the desired direction

			for( int index = 0; index < m_AngleCount; index++ )
			{
				double storeScore = double.MaxValue;
				double currentBest = SystemScore_Restricted();
				int marker = 0;

				assessPhis[index] = RamachandranTools.AddAngle( assessPhis[index], m_StepSize );// store values
				storeScore = SystemScore_Restricted();
				if( storeScore < currentBest )
				{
					marker = 1;
					currentBest = storeScore;
				}

				assessPhis[index] = RamachandranTools.MinusAngle( assessPhis[index], m_DoubleStepSize ); // store values
				storeScore = SystemScore_Restricted();
				if( storeScore < currentBest )
				{
					marker = 2;
					currentBest = storeScore;
				}

				assessPhis[index] = RamachandranTools.AddAngle( assessPhis[index], m_StepSize ); // store values
				assessPsis[index] = RamachandranTools.AddAngle( assessPsis[index], m_StepSize ); // store values
				storeScore = SystemScore_Restricted();
				if( storeScore < currentBest )
				{
					marker = 3;
					currentBest = storeScore;
				}

				assessPsis[index] = RamachandranTools.MinusAngle( assessPsis[index], m_DoubleStepSize ); // store values
				storeScore = SystemScore_Restricted();
				if( storeScore < currentBest )
				{
					marker = 4;
					currentBest = storeScore;
				}

				// all is now returned to normal
				assessPsis[index] = RamachandranTools.AddAngle( assessPsis[index], m_StepSize ); // store values

				switch( marker ) // store the desired direction
				{
					case 0:	
						bestPhis[index] = assessPhis[index];			// store values
						bestPsis[index] = assessPsis[index];			// store values
						break;
					case 1:
						bestPhis[index] = RamachandranTools.AddAngle( assessPhis[index], m_StepSize ); // store values
						bestPsis[index] = assessPsis[index];			// store values
						break;
					case 2:
						bestPhis[index] = RamachandranTools.MinusAngle( assessPhis[index], m_StepSize ); // store values
						bestPsis[index] = assessPsis[index];			// store values
						break;
					case 3: 
						bestPhis[index] = assessPhis[index];			// store values
						bestPsis[index] = RamachandranTools.AddAngle( assessPsis[index], m_StepSize); // store values
						break;
					default: // must be 4
						bestPhis[index] = assessPhis[index];			// store values
						bestPsis[index] = RamachandranTools.MinusAngle( assessPsis[index], m_StepSize); // store values
						break;
				}	
			}
			return true;	
		}

		private bool CalculateMoves_Sigmoid()
		{
			// assessPhis[index]; //these MUST return to the same values on exit
			// assessPsis[index];

			// bestPhis[ index ] // these should contain the desired direction
			// bestPsis[ index ] // these should contain the desired direction

			for( int index = 0; index < m_AngleCount; index++ )
			{
				double storeScore = double.MaxValue;
				double currentBest = SystemScore_Sigmoid();
				int marker = 0;

				assessPhis[index] = RamachandranTools.AddAngle( assessPhis[index], m_StepSize );// store values
				storeScore = SystemScore_Sigmoid();
				if( storeScore < currentBest )
				{
					marker = 1;
					currentBest = storeScore;
				}

				assessPhis[index] = RamachandranTools.MinusAngle( assessPhis[index], m_DoubleStepSize ); // store values
				storeScore = SystemScore_Sigmoid();
				if( storeScore < currentBest )
				{
					marker = 2;
					currentBest = storeScore;
				}

				assessPhis[index] = RamachandranTools.AddAngle( assessPhis[index], m_StepSize ); // store values
				assessPsis[index] = RamachandranTools.AddAngle( assessPsis[index], m_StepSize ); // store values
				storeScore = SystemScore_Sigmoid();
				if( storeScore < currentBest )
				{
					marker = 3;
					currentBest = storeScore;
				}

				assessPsis[index] = RamachandranTools.MinusAngle( assessPsis[index], m_DoubleStepSize ); // store values
				storeScore = SystemScore_Sigmoid();
				if( storeScore < currentBest )
				{
					marker = 4;
					currentBest = storeScore;
				}

				// all is now returned to normal
				assessPsis[index] = RamachandranTools.AddAngle( assessPsis[index], m_StepSize ); // store values

				switch( marker ) // store the desired direction
				{
					case 0:	
						bestPhis[index] = assessPhis[index];			// store values
						bestPsis[index] = assessPsis[index];			// store values
						break;
					case 1:
						bestPhis[index] = RamachandranTools.AddAngle( assessPhis[index], m_StepSize ); // store values
						bestPsis[index] = assessPsis[index];			// store values
						break;
					case 2:
						bestPhis[index] = RamachandranTools.MinusAngle( assessPhis[index], m_StepSize ); // store values
						bestPsis[index] = assessPsis[index];			// store values
						break;
					case 3: 
						bestPhis[index] = assessPhis[index];			// store values
						bestPsis[index] = RamachandranTools.AddAngle( assessPsis[index], m_StepSize); // store values
						break;
					default: // must be 4
						bestPhis[index] = assessPhis[index];			// store values
						bestPsis[index] = RamachandranTools.MinusAngle( assessPsis[index], m_StepSize); // store values
						break;
				}	
			}
			return true;
		}

		private bool CalculateMoves()
		{
			// assessPhis[index]; //these MUST return to the same values on exit
			// assessPsis[index];

			// bestPhis[ index ] // these should contain the desired direction
			// bestPsis[ index ] // these should contain the desired direction

			for( int index = 0; index < m_AngleCount; index++ )
			{
				double storeScore = double.MaxValue;
				double currentBest = SystemScore();
				int marker = 0;

				assessPhis[index] = RamachandranTools.AddAngle( assessPhis[index], m_StepSize );// store values
				storeScore = SystemScore();
				if( storeScore < currentBest )
				{
					marker = 1;
					currentBest = storeScore;
				}

				assessPhis[index] = RamachandranTools.MinusAngle( assessPhis[index], m_DoubleStepSize ); // store values
				storeScore = SystemScore();
				if( storeScore < currentBest )
				{
					marker = 2;
					currentBest = storeScore;
				}

				assessPhis[index] = RamachandranTools.AddAngle( assessPhis[index], m_StepSize ); // store values
				assessPsis[index] = RamachandranTools.AddAngle( assessPsis[index], m_StepSize ); // store values
				storeScore = SystemScore();
				if( storeScore < currentBest )
				{
					marker = 3;
					currentBest = storeScore;
				}

				assessPsis[index] = RamachandranTools.MinusAngle( assessPsis[index], m_DoubleStepSize ); // store values
				storeScore = SystemScore();
				if( storeScore < currentBest )
				{
					marker = 4;
					currentBest = storeScore;
				}

				// all is now returned to normal
				assessPsis[index] = RamachandranTools.AddAngle( assessPsis[index], m_StepSize ); // store values

				switch( marker ) // store the desired direction
				{
					case 0:	
						bestPhis[index] = assessPhis[index];			// store values
						bestPsis[index] = assessPsis[index];			// store values
						break;
					case 1:
						bestPhis[index] = RamachandranTools.AddAngle( assessPhis[index], m_StepSize ); // store values
						bestPsis[index] = assessPsis[index];			// store values
						break;
					case 2:
						bestPhis[index] = RamachandranTools.MinusAngle( assessPhis[index], m_StepSize ); // store values
						bestPsis[index] = assessPsis[index];			// store values
						break;
					case 3: 
						bestPhis[index] = assessPhis[index];			// store values
						bestPsis[index] = RamachandranTools.AddAngle( assessPsis[index], m_StepSize); // store values
						break;
					default: // must be 4
						bestPhis[index] = assessPhis[index];			// store values
						bestPsis[index] = RamachandranTools.MinusAngle( assessPsis[index], m_StepSize); // store values
						break;
				}	
			}
			return true;		
		}


		private double SystemScore()
		{
			double score = 0.0;

			for( int j = 0; j < m_CountTo; j++ )
			{
				double bestDistance = double.MaxValue;
				for( int i = 0; i < m_AngleCount; i++ )
				{
					double distance = RamachandranTools.SquareDistanceBetween(assessPhis[i],assessPsis[i],phiData[j],psiData[j]);
					if( distance < bestDistance )
					{
						bestDistance = distance;
					}
				}
				score += bestDistance;
			}

			return score;
		}
		private double SystemScore_Restricted()
		{
			double score = 0.0;

			// beta region
			for( int j = 0; j < phiData_1.Length; j++ )
			{
				double bestDistance = double.MaxValue;
				for( int i = 0; i < 3; i++ )
				{
					double distance = RamachandranTools.SquareDistanceBetween(assessPhis[i],assessPsis[i],phiData_1[j],psiData_1[j]);
					if( distance < bestDistance )
					{
						bestDistance = distance;
					}
				}
				// any aditional angles can compete with the pre-defined angles in the region
				for( int i = 6; i < m_AngleCount; i++ )
				{
					double distance = RamachandranTools.SquareDistanceBetween(assessPhis[i],assessPsis[i],phiData_1[j],psiData_1[j]);
					if( distance < bestDistance )
					{
						bestDistance = distance;
					}
				}
				score += bestDistance;
			}

			// alpha region
			for( int j = 0; j < phiData_2.Length; j++ )
			{
				double bestDistance = double.MaxValue;
				for( int i = 3; i < 5; i++ )
				{
					double distance = RamachandranTools.SquareDistanceBetween(assessPhis[i],assessPsis[i],phiData_2[j],psiData_2[j]);
					if( distance < bestDistance )
					{
						bestDistance = distance;
					}
				}
				// any aditional angles can compete with the pre-defined angles in the region
				for( int i = 6; i < m_AngleCount; i++ )
				{
					double distance = RamachandranTools.SquareDistanceBetween(assessPhis[i],assessPsis[i],phiData_2[j],psiData_2[j]);
					if( distance < bestDistance )
					{
						bestDistance = distance;
					}
				}
				score += bestDistance;
			}

			// reverse region
			for( int j = 0; j < phiData_3.Length; j++ )
			{
				double bestDistance = RamachandranTools.SquareDistanceBetween(assessPhis[5],assessPsis[5],phiData_3[j],psiData_3[j]);
				// any aditional angles can compete with the pre-defined angles in the region
				for( int i = 6; i < m_AngleCount; i++ )
				{
					double distance = RamachandranTools.SquareDistanceBetween(assessPhis[i],assessPsis[i],phiData_3[j],psiData_3[j]);
					if( distance < bestDistance )
					{
						bestDistance = distance;
					}
				}
				score += bestDistance;
			}

			// other
			if( m_AngleCount > 6 )
			{
				for( int j = 0; j < phiData_O.Length; j++ )
				{
					double bestDistance = double.MaxValue;
					for( int i = 6; i < m_AngleCount; i++ )
					{
						double distance = RamachandranTools.SquareDistanceBetween(assessPhis[i],assessPsis[i],phiData_O[j],psiData_O[j]);
						if( distance < bestDistance )
						{
							bestDistance = distance;
						}					
					}
					score += bestDistance;
				}
			}

			return score;
		}

		private double SystemScore_Sigmoid()
		{
			double score = 0.0;

			for( int j = 0; j < m_CountTo; j++ )
			{
				double bestDistance = double.MaxValue;
				for( int i = 0; i < m_AngleCount; i++ )
				{
					double distance = SigmoidScore( RamachandranTools.DistanceBetween(assessPhis[i],assessPsis[i],phiData[j],psiData[j]) );
					if( distance < bestDistance )
					{
						bestDistance = distance;
					}
				}
				score += bestDistance; // in this case "best distance" is a negative score
			}

			return score;
		}

		private double SigmoidScore( double distance )
		{
			//	B2	1
			//	B3	1
			//	B4	3
			//	B5	-0.08
			// =(E$2/(EXP(($D8*E$5)+E$4)+E$3))-1
			return (1.0 / ( Math.Exp( (distance * -0.08) + 3.0 ) + 1.0 ) ) - 1.0;
		}


		#endregion
	}
}
