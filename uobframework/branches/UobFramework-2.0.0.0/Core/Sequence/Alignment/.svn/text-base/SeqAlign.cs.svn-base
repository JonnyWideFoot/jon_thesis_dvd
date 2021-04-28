using System;
using System.Text;

using UoB.Core.Primitives;

namespace UoB.Core.Sequence.Alignment
{
	/// <summary>
	/// Summary description for SeqAlign.
	/// </summary>
	public sealed class SeqAlign : BestPath
	{
		private string m_Seq1;
		private string m_Seq2;
		private StringBuilder m_Seq1Build = new StringBuilder();
		private StringBuilder m_Seq2Build = new StringBuilder();
		private int[] m_Equiv;

		public SeqAlign( string seq1, string seq2 ) : base( 1, seq1.Length, seq2.Length ) 
		{
			m_Seq1 = seq1;
			m_Equiv = new int[ seq1.Length ];
			m_Seq2 = seq2;
			//FillScoreMatrix(); // get best path does this
			GetBestPath();
			FillEquivList();
		}

		private int nextEquivFromPoint( int lookFrom )
		{
			for( int i = lookFrom; i < m_Equiv.Length; i++ )
			{
				if( m_Equiv[i] != -1 )
				{
					return i;
				}
			}
			return -1;
		}

		private void FillEquivList()
		{
			// now we need to fill the m_Equiv from the best path through the summed m_ScoreMatrix

			// init locals
			int iPointer = m_BestPathStartCellIDX; // these were recorded in the getBestPath() in the base class call...
			int jPointer = m_BestPathStartCellIDY;

			for( int i = 0; i < m_Equiv.Length; i++ )
			{
				m_Equiv[i] = -1; // initialise to no equivs.
			}

			while( Math.Abs(iPointer) < ( m_XDimension - 1 ) && jPointer < ( m_YDimension - 1 ) ) // i.e. until we fall off the end of the table
			{
				// lets move through the path array to set m_Equivalencies

				if( iPointer < 0 ) 
					// the cells score was 0, and so the root-index is marked as -ve, and the equiv is invalid
				{          
                    iPointer = -iPointer;   
				}
				else
				{
					// mark the equiv
					m_Equiv[ iPointer ] = jPointer;  
				}	
					
				// get the next cell IDs
				// tempBufferIPointer :- needed as the jPointer-Lookup needs the older iPointer
				int tempBufferIPointer = m_PathStoreMatrix[ iPointer, jPointer, 0 ];
				jPointer = m_PathStoreMatrix[ iPointer, jPointer, 1 ];
				iPointer = tempBufferIPointer; 
					
			}
			if( iPointer < 0 ) 
				// the cells score was 0, and so the root-index is marked as -ve, and the equiv is invalid
			{          
                //iPointer = -iPointer;   
			}
			else
			{
				// mark the equiv
				m_Equiv[ iPointer ] = jPointer;  
			}	
			// all equivs are now set, we are done
		}

		public override void FillScoreMatrix()
		{
			// make the scorematrix
			for( int i = 0; i < m_XDimension; i++ )
			{
				for( int j = 0; j < m_YDimension; j++ )
				{
					if( m_Seq1[i] == m_Seq2[j] )
					{
						m_ScoreMatrix[i,j] = 2.0f;
					}
					else
					{
						m_ScoreMatrix[i,j] = 0.0f;
					}
				}
			}
			//UoB.Core.Tools.DebugTools.CSVReport( m_ScoreMatrix );
		}


		public int[] equivs
		{
			get
			{
				return m_Equiv;
			}
		}

		public void getGappedSequences( out string seq1, out string seq2 )
		{
			makeGappedStrings();
			seq1 = m_Seq1Build.ToString();
			seq2 = m_Seq2Build.ToString();
			return;
		}

		public override string ToString()
		{
			makeGappedStrings();
			return m_Seq1Build.ToString() + "\r\n" + m_Seq2Build.ToString();;
		}

		private void makeGappedStrings()
		{
			// initilaise the arrays
			m_Seq1Build.Remove(0,m_Seq1Build.Length);
			m_Seq2Build.Remove(0,m_Seq2Build.Length);

			// setup the starts to be in-line
			int firstIDFoundAt = nextEquivFromPoint(0);
			if( firstIDFoundAt == -1 )
			{
				m_Seq1Build.Append( "FAIL : The equivalency list is empty, no alignment could be shown!" );
				return;
			}

			// the first pair deemed to be structurally equivelent
			int mol1Index = firstIDFoundAt;
			int mol2Index = m_Equiv[mol1Index];

			m_Seq1Build.Append( m_Seq1[mol1Index] );
			m_Seq2Build.Append( m_Seq2[mol2Index] );

			mol1Index--; // set the cursor to one backwards
			mol2Index--;
			// now backtrack to fill the start
			while( mol1Index >= 0 || mol2Index >= 0 )
			{
				if(  mol1Index >= 0 )
				{
					m_Seq1Build.Insert(0, m_Seq1[mol1Index] );
				}
				else
				{
					m_Seq1Build.Insert(0,'-');
				}
				if(  mol2Index >= 0 )
				{
					m_Seq2Build.Insert(0, m_Seq2[mol2Index] );
				}
				else
				{
					m_Seq2Build.Insert(0,'-');
				}

				mol1Index--; // work our way back to the start of the sequences
				mol2Index--;
			}

			// now fill to the end
			mol1Index = firstIDFoundAt;
			mol2Index = m_Equiv[ mol1Index ];	
			int mol1NextIndex;
			int mol2NextIndex;

			while( (mol1NextIndex = nextEquivFromPoint( mol1Index + 1 ) ) != -1 ) // -1 is returned once the equivs have ended
			{
				// get mol2's index
				mol2NextIndex = m_Equiv[ mol1NextIndex ];

				// somehow fill between the equivs if there are residues in between
				mol2Index = m_Equiv[ mol1Index ]; // the next residues

                // POTENTIAL ERROR: Found this in later debug, whats is for ???
				// mol1Index = mol1Index;           // the next residues

				while( true )
				{			
					mol2Index++;
					mol1Index++;
					if( mol1Index < mol1NextIndex && mol2Index < mol2NextIndex )
					{
						m_Seq1Build.Append( m_Seq1[mol1Index] );
						m_Seq2Build.Append( m_Seq2[mol2Index] );
					}
					else if( mol1Index < mol1NextIndex )
					{
						m_Seq1Build.Append( m_Seq1[mol1Index] );
						m_Seq2Build.Append('-');
					}
					else if( mol2Index < mol2NextIndex )
					{
						m_Seq1Build.Append('-');
						m_Seq2Build.Append( m_Seq2[mol2Index] );
					}
					else
					{
						break; // we are done here, exit condition						
					}
				}

				// now add the equiv add an equiv
				m_Seq1Build.Append( m_Seq1[mol1NextIndex] );
				m_Seq2Build.Append( m_Seq2[mol2NextIndex] );

				mol1Index = mol1NextIndex;
				mol2Index = m_Equiv[ mol1Index ];
			}

			// now fill to the end
			while( mol1Index < m_Seq1.Length || mol2Index < m_Seq2.Length )
			{
				mol1Index++; // work our way back to the start of the sequences
				mol2Index++;
				if( mol1Index < m_Seq1.Length && mol2Index < m_Seq2.Length )
				{
					m_Seq1Build.Append( m_Seq1[mol1Index] );
					m_Seq2Build.Append( m_Seq2[mol2Index] );
				}
				else if( mol1Index < m_Seq1.Length )
				{
					m_Seq1Build.Append( m_Seq1[mol1Index] );
					m_Seq2Build.Append('-');
				}
				else if( mol2Index < m_Seq2.Length )
				{
					m_Seq1Build.Append('-');
					m_Seq2Build.Append( m_Seq2[mol2Index] );
				}
			}
		}
	}
}
