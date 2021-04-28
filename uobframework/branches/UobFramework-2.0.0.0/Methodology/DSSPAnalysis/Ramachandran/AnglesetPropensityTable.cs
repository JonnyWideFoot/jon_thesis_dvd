using System;
using System.IO;
using System.Collections;
using System.Text;

using UoB.Core.MoveSets.AngleSets;
using UoB.Methodology.TaskManagement;

namespace UoB.Methodology.DSSPAnalysis.Ramachandran
{
	/// <summary>
	/// Summary description for PropensityTable.
	/// </summary>
	public class AnglesetPropensityTable : OutputTable
	{
		private AngleSet m_AngleSet = null;
		private bool m_AllInited = false;
		private bool m_LoopInited = false;
		private int m_AngleCount = -1;
		private char m_MolID = '\0';
		private float[,] m_AnglePropensities = null;

		private readonly string[] m_CountTitles = new string[] { "Angle#","Original RAFT Propensity","All Strcuture Propensity","Loop Propensity" };

		/// <summary>
		/// Functions should be called as follows:
		/// for( bla bla bla )
		/// {
		/// PropensityReset();
		/// PropensityObtainAllResidues();
		/// PropensityObtainLoopResidues();
		/// BuildCurrentPropensityLine();
		/// }
		/// PrintTable();
		/// 
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="angleSet"></param>
		public AnglesetPropensityTable( DelimiterMode mode, AngleSet angleSet ) : base( mode )
		{
			m_AngleSet = angleSet;
			InitialiseTitle();
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

		public void PropensityReset( char molID )
		{
			m_RecordLineStrings.RemoveRange(1,m_RecordLineStrings.Count-1); 
			// delete all the strings except the title

			m_AllInited = false; // reset the initialisation flags
			m_LoopInited = false;

			// fill and validate member variables
			m_MolID = molID;
			m_AngleCount = m_AngleSet.GetAngleCount( molID );
			if( m_AngleCount == -1 ) throw new Exception("The given molID is not valid");
			m_AnglePropensities = new float[ m_AngleCount, 3 ];
			// array is "3" wide because
			// 0 = origainal
			// 1 = new All-Residue
			// 2 = new All-Loop

			// get the old raft propensities
			float[] RAFTPropensities = m_AngleSet.GetPropensities( molID );
			for( int i = 0; i < m_AngleCount; i++ )
			{
				m_AnglePropensities[i,0] = RAFTPropensities[i];
			}
		}

		public void PropensityObtainAllResidues( double[] phiData, double[] psiData )
		{
			if( m_AllInited )
			{
				throw new Exception("All is already initiated");
			}

			// need to fill m_NewAllAnglePropensities
			DoPropensity( 1, phiData, psiData );

			m_AllInited = true;
		}

		public void PropensityObtainLoopResidues( double[] phiData, double[] psiData )
		{
			if( m_LoopInited )
			{
				throw new Exception("Loop is already initiated");
			}

			// need to fill m_LoopAnglePropensities
			DoPropensity( 2, phiData, psiData );

			m_LoopInited = true;
		}

		private void DoPropensity( int yID, double[] phiData, double[] psiData )
		{
			for( int i = 0; i < phiData.Length; i++ )
			{
				m_AnglePropensities[ m_AngleSet.ClosestIDTo( m_MolID, phiData[i], psiData[i] ) - 1, yID ] += 1.0f;
			}

			// make grid into fractions of the total
			for( int i = 0; i < m_AngleCount; i++ )
			{
				m_AnglePropensities[i,yID] = m_AnglePropensities[i,yID] / (float)phiData.Length;
			}
		}

		public void BuildCurrentPropensityLine()
		{
			if( !m_AllInited )
			{
				throw new Exception("AllResidues havent been initiated");
			}
			if( !m_LoopInited )
			{
				throw new Exception("LoopResidues havent been initiated");
			}

			for( int i = 0; i < m_AngleCount; i++ )
			{
				m_SBuild.Append( m_InitLine );
				m_SBuild.Append( ((int)(i+1)).ToString() );
				m_SBuild.Append( m_Delimiter );
				m_SBuild.Append( m_AnglePropensities[i,0].ToString("0.00") ); // raft
				m_SBuild.Append( m_Delimiter );
				m_SBuild.Append( m_AnglePropensities[i,1].ToString("0.00") ); // all
				m_SBuild.Append( m_Delimiter );
				m_SBuild.Append( m_AnglePropensities[i,2].ToString("0.00") ); // loop
				m_SBuild.Append( m_EndLine );
				m_SBuild.Append( "\r\n" );
			}

			m_RecordLineStrings.Add( m_SBuild.ToString() );
		}
	}
}
