using System;
using System.IO;

using UoB.Core.Sequence;
using UoB.Methodology.DSSPAnalysis;

namespace UoB.Methodology.DSSPAnalysis.AngleFitting
{
	/// <summary>
	/// Summary description for AngleFittingBase.
	/// </summary>
	public abstract class AngleFittingEngine_Base : DSSPTaskDirecory_PhiPsiData
	{
		// Result Output
		protected DateTime m_Time = DateTime.MinValue;
		protected StreamWriter m_RepWriter = null;

		// curent angle fitting params
		protected int m_CountTo; // a "speed-up" so ArrayList.Count doesnt have to be called during simulation ... Filled by ObtainPhiPsiData() used throughout
		protected char m_CurrentMolID;
		protected StandardResidues m_SingleResType;
		protected int m_AngleCount;	

		// search result lists
		protected double bestScore = double.MaxValue;
		protected double[] bestPhis = null;
		protected double[] bestPsis = null;
		protected double[] assessPhis = null;
		protected double[] assessPsis = null;

		public AngleFittingEngine_Base( string DSSPDatabaseName, DirectoryInfo di, int angleCount, char resID) 
			: base( DSSPDatabaseName, di, false )
		{
			m_AngleCount = angleCount;
			// NOTE! ... lower case resID signifies that the residue import should be cis, uppercase is trans
			// the StandardResidues enumeration is case sensitive
			m_CurrentMolID = resID;
			m_SingleResType = (StandardResidues) Enum.Parse( typeof(StandardResidues), m_CurrentMolID.ToString(), false );
			m_Time = DateTime.MinValue; // null for the moment
			
			// init arrays to correct size
			assessPhis = new double[m_AngleCount];
			assessPsis = new double[m_AngleCount];
			bestPhis = new double[m_AngleCount];
			bestPsis = new double[m_AngleCount];
		}

		public abstract string GetOutputFilename();
		public abstract void GoAngleFitting();
	}
}
