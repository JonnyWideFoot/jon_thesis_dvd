using System;
using System.IO;

using UoB.Core.MoveSets.AngleSets;

namespace UoB.Methodology.DSSPAnalysis.AngleFitting
{
	/// <summary>
	/// Summary description for AngleFittingExhaustive.
	/// </summary>
	public sealed class AngleFittingEngine_Exhaustive : AngleFittingEngine_Base
	{
		// count the number of assesments, and report every m_ReportFrequency
		private long m_AssessCount = 0;
		private long m_ReportFrequency = 2000;
		// Grid Searchb Parameters
		private double gridStep = 20.0;
		private double gridPhiMin = -180.0;
		private double gridPsiMin = -180.0;
		private double gridPhiMax = 180.0;
		private double gridPsiMax = 180.0;

		public AngleFittingEngine_Exhaustive( string DSSPDatabaseName, DirectoryInfo di, int angleCount, char resID ) 
			: base( DSSPDatabaseName, di, angleCount, resID )
		{	
			throw new NotImplementedException();
			// BIG WARNING :
			// this class is largely untested as it takes 1*10^646 years to run...
			// hence the sister class that performs the montecarlo search .. use that instead!
		}

		public override void GoAngleFitting()
		{

			m_RepWriter = new StreamWriter( reportDirectory.FullName + GetOutputFilename() );

			m_Time = DateTime.Now;

			m_CountTo = ObtainPhiPsiData( DSSPReportingOn.All, DSSPIncludedRegions.OnlyDefinitelyGood, m_SingleResType );
			IncrementAnglesAndAssess(0); // kick off the recursive function

			m_RepWriter.WriteLine( "Time taken : " + "ALL" + " " + GetOutputFilename() + " : " + ( ( DateTime.Now - m_Time )).ToString() );
			m_RepWriter.WriteLine();
			
			m_Time = DateTime.Now;

			m_CountTo = ObtainPhiPsiData( DSSPReportingOn.LoopsOnly, DSSPIncludedRegions.OnlyDefinitelyGood, m_SingleResType );
			IncrementAnglesAndAssess(0); // kick off the recursive function

			m_RepWriter.WriteLine( "Time taken : " + "LOOP" + " " + GetOutputFilename() + " : " + ( ( DateTime.Now - m_Time )).ToString() );
			m_RepWriter.WriteLine();

			m_RepWriter.Close();
		}

		public override string GetOutputFilename()
		{
			string stem = m_AngleCount.ToString() + "_" + m_CurrentMolID + "Exhaustive_";
			if( MaxFileImport == int.MaxValue )
			{
				// no file limit ...
				return stem + "nolimit.csv";

			}
			else
			{
				return stem + MaxFileImport.ToString() + ".csv";
			}
		}

		/// <summary>
		/// Recursive function to increment the angle set...
		/// </summary>
		/// <param name="angleIndex"></param>
		private void IncrementAnglesAndAssess( int angleIndex )
		{
			if( assessPhis.Length == angleIndex )
			{
				// score when the last angle is reached
				ScoreCurrentPhiPsiSet();
			}
			else
			{
				// increment until we reach the end angle
				for( double anglePhi = gridPhiMin; anglePhi < gridPhiMax; anglePhi += gridStep )
				{
					for( double anglePsi = gridPsiMin; anglePsi < gridPsiMax; anglePsi += gridStep )
					{
						assessPhis[angleIndex] = anglePhi;
						assessPsis[angleIndex] = anglePsi;
						IncrementAnglesAndAssess( angleIndex + 1 );
					}
				}
			}
		}

		private void ScoreCurrentPhiPsiSet()
		{
			m_AssessCount++;

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

			if( score < bestScore )
			{
				// store that angle set
				for( int i = 0; i < m_AngleCount; i++ )
				{
					bestPhis[i] = assessPhis[i];
					bestPsis[i] = assessPsis[i];
				}
				bestScore = score;
				PrintBest( bestScore );
			}   
         
			if( m_AssessCount % m_ReportFrequency == 0 ) 
			{
				m_RepWriter.WriteLine( "Done : " + m_AssessCount.ToString() + " : Current tick time : " + (( DateTime.Now - m_Time )).ToString() );
			}
		}

		private void PrintBest( double bestScore )
		{
			m_RepWriter.Write("Best Conf : ");
			for( int i = 0; i < m_AngleCount; i++ )
			{
				m_RepWriter.Write('(');
				m_RepWriter.Write(bestPhis[i]);
				m_RepWriter.Write(',');
				m_RepWriter.Write(bestPsis[i]);
				m_RepWriter.Write(')');
				m_RepWriter.Write('~');
			}
			m_RepWriter.WriteLine(bestScore);
			m_RepWriter.Flush();
		}
	}
}
