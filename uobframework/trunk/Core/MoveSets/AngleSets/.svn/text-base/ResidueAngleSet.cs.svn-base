using System;

namespace UoB.Core.MoveSets.AngleSets
{
	/// <summary>
	/// Summary description for RaftAngleSet.
	/// </summary>
	public class ResidueAngleSet
	{
		private char m_ID;
		private string m_Name = null;
		private double[] m_PhiAngles = null;
		private double[] m_PsiAngles = null;
		private double[] m_Omegas = null;
		private float[] m_Propensities = null;
		private char[] m_AngleClasses = null;

		public ResidueAngleSet( char id, string name, double[] phiAngles, double[] psiAngles, double[] omegas, float[] propensities, char[] angleClasses )
		{
			m_ID = id;
			m_Name = name;
			m_PhiAngles = phiAngles;
			m_PsiAngles = psiAngles;
			m_Omegas = omegas;
			m_Propensities = propensities;
			m_AngleClasses = angleClasses;

			if(    m_PhiAngles.Length != m_PsiAngles.Length 
				|| m_PhiAngles.Length != m_Propensities.Length 
				|| m_PhiAngles.Length != m_Omegas.Length 
				|| m_PhiAngles.Length != m_AngleClasses.Length 
				)
			{
				throw new Exception("Array length mismatch");
			}
		}

		public int AngleCount
		{
			get
			{
				return m_PhiAngles.Length;
			}
		}

		public char ID
		{
			get
			{
                return m_ID;
			}
		}

		public string Name
		{
			get
			{
				return m_Name;
			}
		}

		public char getAngleClass( int angleID )
		{
			if( (angleID < 0) || ((angleID) >= AngleCount ) ) throw new Exception("Invalid raft angle ID given");
			return m_AngleClasses[ angleID ];		
		}

        public bool getIsCis(int angleID)
        {
            if ((angleID < 0) || ((angleID) >= AngleCount)) throw new Exception("Invalid raft angle ID given");
            return m_Omegas[angleID] == 0.0;
        }

		public double getOmega( int angleID )
		{
			if( (angleID < 0) || ((angleID) >= AngleCount ) ) throw new Exception("Invalid raft angle ID given");
			return m_Omegas[ angleID ];		
		}

		public float getPropensity( int angleID )
		{
			if( (angleID < 0) || ((angleID) >= AngleCount ) ) throw new Exception("Invalid raft angle ID given");
			return m_Propensities[ angleID ];		
		}

		public double getPhi( int angleID )
		{
			if( (angleID < 0) || ((angleID) >= AngleCount ) ) throw new Exception("Invalid raft angle ID given");
			return m_PhiAngles[ angleID ];		
		}

		public double getPsi( int angleID )
		{
			if( (angleID < 0) || ((angleID) >= AngleCount ) ) throw new Exception("Invalid raft angle ID given");
			return m_PsiAngles[ angleID ];		
		}
	}
}
