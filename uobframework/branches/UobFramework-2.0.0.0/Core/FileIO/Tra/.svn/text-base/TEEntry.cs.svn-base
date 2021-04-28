using System;

using UoB.Core.Primitives;

namespace UoB.Core.FileIO.Tra
{
	/// <summary>
	/// Summary description for Structures.
	/// </summary>
	/// 

	class TEEntry
	{
		private Position[] m_AtomPositions;
		private Position[] m_ForceVectors;
		private float[] m_Phis;
		private float[] m_Psis;
		private int[] m_Rotamers;
		private float[] m_Energies;

		public TEEntry ( Position[] allPositions, Position[] forceVectors, float[] phis, float[] psis, int[] rotamers, float[] te )
		{
			m_AtomPositions = allPositions; 
			m_ForceVectors = forceVectors;
			m_Phis = phis;
			m_Psis = psis;
            m_Rotamers = rotamers;
			m_Energies = te;
		}

		public Position[] Positions
		{
			get
			{
				return m_AtomPositions;
			}
		}

		public Position[] ForceVectors
		{
			get
			{
				return m_ForceVectors;
			}
		}

		public float[] Energies
		{
			get
			{
				return m_Energies;
			}
		}

		public float[] Phis
		{
			get
			{
				return m_Phis;
			}
		}

		public float[] Psis
		{
			get
			{
				return m_Psis;
			}
		}

		public int[] Rotamers
		{
			get
			{
				return m_Rotamers;
			}
		}
	}
}
