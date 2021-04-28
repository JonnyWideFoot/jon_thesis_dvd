using System;

namespace UoB.Core.Structure.Builder
{
	/// <summary>
	/// Summary description for Selection.
	/// </summary>
	public class Selection
	{
		public char ChainID;
		public int templateStart;
		public int templateEnd;
		public int threadStart;
		public int threadEnd;

		public Selection( char chainID, int tempStart, int tempEnd, int thrStart, int thrEnd )
		{
			ChainID = chainID;
			templateStart = tempStart;
			templateEnd = tempEnd;
			threadStart = thrStart;
			threadEnd = thrEnd;
		}

		public int templateLength
		{
			get
			{
				return templateEnd - templateStart + 1;
			}
		}

		public int threadLength
		{
			get
			{
				return threadEnd - threadStart + 1;
			}
		}

		public override string ToString()
		{
			return ChainID.ToString() + ":" + templateStart.ToString() + "-" + templateEnd.ToString() + ":" + 
				threadStart.ToString() + "-" + threadEnd.ToString();
		}

	}
}
