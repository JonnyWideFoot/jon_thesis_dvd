using System;
using System.IO;
using System.Collections;
using System.Text;

using UoB.Core.MoveSets.AngleSets;
using UoB.Methodology.TaskManagement;

namespace UoB.Methodology.DSSPAnalysis.Ramachandran
{
	/// <summary>
	/// Summary description for ResidueCountTable.
	/// </summary>
	public sealed class ResidueCountTable : OutputTable
	{
		private int m_AllCountAll;
		private int m_AllCountLoop;

		private readonly string[] m_CountTitles = new string[] { "Residue Type", "All Structure", "Percentage", "Loop", "Percentage", "Expected In Loops", "% Change from Expected" };

		public ResidueCountTable( DelimiterMode mode, int allCountAll, int allCountLoop ) : base( mode )
		{
			m_AllCountAll = allCountAll;
			m_AllCountLoop = allCountLoop;
			InitialiseTitle();
			InitialiseAllResidueLine();
		}
        
		private void InitialiseTitle()
		{
			// do the title line ... 
			m_SBuild.Remove( 0, m_SBuild.Length );
			m_SBuild.Append( m_InitLine );
			for( int i = 0; i < m_CountTitles.Length -1; i++ )
			{
				m_SBuild.Append( m_CountTitles[i] );
				m_SBuild.Append( m_Delimiter );
			}
			m_SBuild.Append( m_CountTitles[m_CountTitles.Length-1] );
			m_SBuild.Append( m_EndLine );
			m_RecordLineStrings.Add( m_SBuild.ToString() );
		}

		private void InitialiseAllResidueLine()
		{
			// do the mostly blank all line
			m_SBuild.Remove( 0, m_SBuild.Length );
			m_SBuild.Append( m_InitLine );

			// name
			m_SBuild.Append( "All Types" );
			m_SBuild.Append( m_Delimiter );

			// all count
			string allCountString = m_AllCountAll.ToString("N");
			m_SBuild.Append( allCountString, 0, allCountString.Length-3 ); // "take the .00 off the end"
			m_SBuild.Append( m_Delimiter );

			m_SBuild.Append( '-' );
			m_SBuild.Append( m_Delimiter );

			// loop count
			string loopCountString = m_AllCountLoop.ToString("N");
			m_SBuild.Append( loopCountString, 0, loopCountString.Length-3 ); // "take the .00 off the end"
			m_SBuild.Append( m_Delimiter );

			m_SBuild.Append( '-' );
			m_SBuild.Append( m_Delimiter );

			m_SBuild.Append( '-' );
			m_SBuild.Append( m_Delimiter );
			
			m_SBuild.Append( '-' );

			m_SBuild.Append( m_EndLine );
			m_RecordLineStrings.Add( m_SBuild.ToString() );

		}

		public void AddCountRecord( string name, int allCount, int loopCount )
		{
			// do the title line ... 
			m_SBuild.Remove( 0, m_SBuild.Length );
			m_SBuild.Append( m_InitLine );

			// name
			m_SBuild.Append( name );
			m_SBuild.Append( m_Delimiter );
			// all count
			string allCountS = allCount.ToString("N");
			m_SBuild.Append( allCountS, 0, allCountS.Length-3 );  // "take the .00 off the end"
			m_SBuild.Append( m_Delimiter );
			// all percentage
			float fractAll = (float)allCount / (float)m_AllCountAll;
			m_SBuild.Append( fractAll.ToString("P") );
			m_SBuild.Append( m_Delimiter );
			// loop count
			string loopCountS = loopCount.ToString("N");
			m_SBuild.Append( loopCountS, 0, loopCountS.Length-3 );  // "take the .00 off the end"
			m_SBuild.Append( m_Delimiter );
			// loop percentage
			float fractLoop = (float)loopCount / (float)m_AllCountLoop;
			m_SBuild.Append( fractLoop.ToString("P") );
			m_SBuild.Append( m_Delimiter );
			// Expected loop count
			int expected = (int)(((float)m_AllCountLoop / (float)m_AllCountAll) * (float)allCount);
			string allExpectedS = expected.ToString("N");
			m_SBuild.Append( allExpectedS, 0, allExpectedS.Length-3 );  // "take the .00 off the end"
			m_SBuild.Append( m_Delimiter );
			// percentage change
			float change = ((float)loopCount - (float)expected) / (float)expected;
			m_SBuild.Append( change.ToString("P") );

			m_SBuild.Append( m_EndLine );
			m_RecordLineStrings.Add( m_SBuild.ToString() );
		}

		public int CountLines
		{
			get
			{
				return m_RecordLineStrings.Count;
			}
		}

		public string GetCountRecord( int index )
		{
			return (string) m_RecordLineStrings[index];
		}
	}
}
