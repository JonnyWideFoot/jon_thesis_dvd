using System;

namespace UoB.Core.FileIO.Raft
{
	public class EmcFillParams
	{
		private static Random rand = new Random();
		public int randomNumber;

		public EmcFillParams( int randomSeed )
		{
			randomNumber = rand.Next( randomSeed );
		}

		public EmcFillParams()
		{
			randomNumber = rand.Next();
		}

		public int genStart = 3000000; // DEFAULT VALUES
		public int parentPass = 5000;
		public int genCount = 5;
		public int midConfCount = 100;
		public int midCoordCount = 10;
		public int lastConfCount = 1000;
		public int lastCoordCount = 100;
		public float mutationRate = 50.0f;
	}
}
