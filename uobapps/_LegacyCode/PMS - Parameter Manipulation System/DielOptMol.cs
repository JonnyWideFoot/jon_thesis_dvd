using System;

namespace UoB.PMS
{
	/// <summary>
	/// Summary description for DielOptMol.
	/// </summary>
	public class DielOptMol
	{
		public static float[] dielectricValues;

		private float[] m_solvationEValues;
		public float[] solvationEValues
		{
			get
			{
				return m_solvationEValues;
			}
			set
			{
				if ( dielectricValues.Length != value.Length )
				{
					throw new Exception( "m_solvationEValues.Length != value.Length" );
				}
				m_solvationEValues = value;
			}
		}
		public string Name;
		public float DGtr;
		public float[] AtomTypeSASAs;

		public DielOptMol()
		{
		}
	}
}
