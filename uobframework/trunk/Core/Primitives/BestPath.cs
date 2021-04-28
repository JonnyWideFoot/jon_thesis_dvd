using System;

namespace UoB.Core.Primitives
{
	/// <summary>
	/// Summary description for BestPath.
	/// </summary>
	public abstract class BestPath
	{
		protected int m_GapPenalty;
		protected int m_XDimension;
		protected int m_YDimension;
		protected int m_BestPathStartCellIDX;
		protected int m_BestPathStartCellIDY;

		protected float[,] m_ScoreMatrix;
		protected int[,,] m_PathStoreMatrix;
        
		public BestPath( int gapPenalty, int xDimension, int yDimension )
		{
			m_GapPenalty = gapPenalty;
			m_XDimension = xDimension;
			m_YDimension = yDimension;
			m_ScoreMatrix = new float[xDimension,yDimension];
			m_PathStoreMatrix = new int[xDimension-1,yDimension-1,2]; // has to be 1 smaller than the scoreMartix, the 3rd dimension holds the 2 coordinates i and j of the desired next cell in the scoreMatrix
		}

		public abstract void FillScoreMatrix(); // override for in derived classes for custom score behaviour

        private void Print(int[,,] f)
        {
            for (int i = f.GetLowerBound(0); i <= f.GetUpperBound(0); i++)
            {
                for (int j = f.GetLowerBound(1); j <= f.GetUpperBound(1); j++)
                {
                    Console.Write('{');
                    for (int k = f.GetLowerBound(2); k <= f.GetUpperBound(2); k++)
                    {
                        if (k != f.GetUpperBound(2) - 1)
                            Console.Write(',');
                        Console.Write(f[i, j, k]);                        
                    }
                    Console.Write('}');
                    Console.Write(' ');
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private void Print(float[,] f)
        {
            for (int i = f.GetLowerBound(0); i <= f.GetUpperBound(0); i++)
            {
                for (int j = f.GetLowerBound(1); j <= f.GetUpperBound(1); j++)
                {
                    Console.Write(f[i, j]);
                    Console.Write(' ');
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        protected void GetBestPath() // finds the sum route through the matrix and then fills the m_PathStoreMatrix to record the best path
		{
			FillScoreMatrix();

            //Print(m_ScoreMatrix);

			int IPlusOne;
			int JPlusOne;
			int bestI;
			int bestJ;
			float bestValue;

			for( int i = m_XDimension - 2; i >= 0; i-- ) // counting backwards from the far corner of the table and up one diagonally
			{
				for( int j = m_YDimension - 2; j >= 0; j-- )
				{
					IPlusOne = i + 1; // the plus ones are the next cell down diagonally, i.e. the first evaluation for best path ..
					JPlusOne = j + 1;
					bestI = IPlusOne; // we will initially asume that the diagonal is the best initial path, to be edited in a moment if not
					bestJ = JPlusOne;
					bestValue = m_ScoreMatrix[bestI,bestJ]; // so what is the assumptions value

					for( int countI = i + 2; countI < m_XDimension; countI++ )
					{
						float additionScore = m_ScoreMatrix[countI,JPlusOne] - m_GapPenalty; 
						// gap penalty is invoked on anything other than the direct diagonal;
						if( additionScore > bestValue )
						{
							bestI = countI;
							bestJ = JPlusOne;
							bestValue = additionScore;
						}
					}
					for( int countJ = j + 2; countJ < m_YDimension; countJ++ )
					{
						float additionScore = m_ScoreMatrix[IPlusOne,countJ] - m_GapPenalty;
						if( additionScore > bestValue )
						{
							bestI = IPlusOne;
							bestJ = countJ;
							bestValue = additionScore;
						}
					}

					m_PathStoreMatrix[i,j,0] = bestI; // this records an ID for the cell we found for following the path backwards
					m_PathStoreMatrix[i,j,1] = bestJ;

					m_ScoreMatrix[i,j] += bestValue; // the best possible scoring total from the path from the current i,j pairing			
				}
			}

            //Print(m_PathStoreMatrix);
            //Print(m_ScoreMatrix);

			// we now need to find the best starting cell and set m_BestPathStartCellIDX, and m_BestPathStartCellIDY
			float bestScoreMatrixValue = m_ScoreMatrix[0,0]; // initialise to this
			// is it true, probably not...

			for( int i = 1; i < m_XDimension; i++ )
			{
				if( m_ScoreMatrix[i,0] > bestScoreMatrixValue )
				{
					m_BestPathStartCellIDX = i;
					bestScoreMatrixValue = m_ScoreMatrix[i,0];
				}
			}
			for( int j = 1; j < m_YDimension; j++ )
			{
				if( m_ScoreMatrix[0,j] > bestScoreMatrixValue )
				{
					m_BestPathStartCellIDX = 0; // we neeed to be in the 1st row or the 1st column
					m_BestPathStartCellIDY = j;
					bestScoreMatrixValue = m_ScoreMatrix[0,j];
				}
			}
			// the m_PathStoreMatrix has been found and the starting cell for the path has been defined
			// our work here is done
		}
	}
}
