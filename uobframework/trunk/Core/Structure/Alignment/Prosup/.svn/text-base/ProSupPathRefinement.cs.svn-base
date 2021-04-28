using System;

using UoB.Core.Primitives;
using UoB.Core.Structure;

namespace UoB.Core.Structure.Alignment.Prosup
{
	/// <summary>
	/// Summary description for ProSupPathRefinement.
	/// </summary>
	public sealed class ProSupPathRefinement : BestPath
	{
		private Position[] m_Mol1;
		private Position[] m_Mol2;
		private float m_RefineCSquared;

		// This class is used to refine the current alignment.
		// The alignment may not be biologially sensible as currently we have used purely geometric
		// information and have not utilied the restraints of the properties of a sequence
		// i.e. in homologous proteins, there are rarely gross sequence movements in relation to each other
		// the start of teh 1st protein will correspond to the start of the second protein and the two,
		// will follow the same path atfer accounting for insertions and deletions that are defined
		// by the relative positioning of the strands. Homologous regions are therefore defined both by
		// being proximal in position and by allowing the smallest number of gaps in the alignment.
		// Creation of gaps is disfavored using a gap penalty, however if introduction of a gap in the
		// alignment allows there to be better overall alignment of the proteins in sequence space,
		// this is then allowed.

		public ProSupPathRefinement( Position[] mol1, Position[] mol2, float refineCSquared, int gapPenalty )
			: base(gapPenalty,mol1.Length,mol2.Length)
		{
			m_Mol1 = mol1;
			m_Mol2 = mol2;
			m_RefineCSquared = refineCSquared;
		}

		public override void FillScoreMatrix()
		{
			// make the scorematrix
			for( int i = 0; i < m_XDimension; i++ )
			{
				for( int j = 0; j < m_YDimension; j++ )
				{
					double distanceSquared = Position.distanceSquaredBetween( m_Mol1[i], m_Mol2[j] );
					if( m_RefineCSquared >= distanceSquared )
					{
						m_ScoreMatrix[i,j] = (float)( m_RefineCSquared - distanceSquared );
					}
					else
					{
						m_ScoreMatrix[i,j] = 0.0f;
					}
				}
			}
			//UoB.Core.Tools.DebugTools.CSVReport( scoreMatrix );
		}

		internal void Execute( Model m )
		{
			//FillScoreMatrix(); getbestpath() does this
			GetBestPath();
			FillEquivList( m );
		}

		private void FillEquivList( Model m )
		{
			// now we need to fill the m.Equivalencies from the best path through the summed m_ScoreMatrix

			// init locals
			int iPointer = m_BestPathStartCellIDX; // these were recorded in the getBestPath() call...
			int jPointer = m_BestPathStartCellIDY;
			bool equiv = true;

			for( int i = 0; i < m.Equivalencies.Length; i++ )
			{
				m.Equivalencies[i] = -1; // initialise to no equivs.
			}

			while( iPointer < ( m_XDimension - 1 ) && jPointer < ( m_YDimension - 1 ) ) // i.e. until we fall off the end of the table
			{
				// lets move through the path array to set m_Equivalencies

				// mark the equiv
				if( equiv )
				{
					m.Equivalencies[ iPointer ] = jPointer;
				}
					
				// get the next cell IDs
				// tempBufferIPointer :- needed as the jPointer-Lookup needs the older iPointer
				int tempBufferIPointer = m_PathStoreMatrix[ iPointer, jPointer, 0 ];
				jPointer = m_PathStoreMatrix[ iPointer, jPointer, 1 ];
				iPointer = tempBufferIPointer; 

				// first get the cell ID of the next cell
					
				if( iPointer < 0 ) // the cells score was 0, and so the root id is marked as -ve, so the equiv is invalid
				{
					iPointer = -iPointer;
					equiv = false;                                      
				}
				else
				{
					equiv = true;
				}
			}
			if( equiv ) // need to set the final one
			{
				m.Equivalencies[ iPointer ] = jPointer;
			}
			// all equivs are now set, we are done
		}
	}
}
