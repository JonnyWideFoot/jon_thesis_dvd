using System;

namespace UoB.Core.Structure.Alignment.Prosup
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	internal sealed class SeedDefinition : Model
	{
		private int m_Mol1StartIndex;
		private int m_Mol2StartIndex;

		public SeedDefinition( int mol1Start, int mol2Start, int seedLength, int totalLength ) 
			: base( totalLength )
		{
			m_Mol1StartIndex = mol1Start;
			m_Mol2StartIndex = mol2Start;

			int indexX;
			int indexY;
			for( int i = 0; i < seedLength; i++ )
			{
				indexX = m_Mol1StartIndex + i;
				indexY = m_Mol2StartIndex + i;
				m_Equivalencies[ indexX ] = indexY;           
			}
		} 

		public int Mol1StartIndex
		{
			get
			{
				return m_Mol1StartIndex;
			}
		}

		public int Mol2StartIndex
		{
			get
			{
				return m_Mol2StartIndex;
			}
		}
	}
}
