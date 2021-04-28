using System;
using System.Text;
using System.Diagnostics;
using System.Collections;

using UoB.Core.Sequence;
using UoB.Core.Structure;
using UoB.Core.Sequence.Alignment;

namespace UoB.Core.FileIO.PDB
{
	/// <summary>
	/// Summary description for SeqRes.
	/// </summary>
	public sealed class PDBSequence : PSSequence
	{
		private Hashtable m_SeqRes;
		private SeqAlign m_Aligner = null;

		public PDBSequence( ParticleSystem ps ) : base( ps )
		{
			m_SeqRes = new Hashtable();
		}

		public PDBSequence() : base()
		{
			m_SeqRes = new Hashtable();
		}

        private StringBuilder m_SeqresBuild = new StringBuilder();
		private int m_TotalResCount = 0;
		private int m_FoundResCount = 0;

//		public RebuildRegionsDefinition getRebuildDef( char chainID )
//		{
//			m_Aligner = new SeqAlign( (string) m_SequenceBin[ chainID ],(string)m_SeqRes[ chainID ] );	
//		}

		public string getStructuralSeq( char id )
		{
			return (string)m_SequenceBin[id];
		}

		public string getSEQRESSeq( char id )
		{
			string s = (string)m_SeqRes[id];
			if( s != null )
			{
				return s;
			}
			else
			{
				throw new Exception("Chain ID : : + id + " + "is not a valid ID.");
			}
		}

		public string getSEQRESSeq( char id, int startResidueID, int endResidueID )
		{
			string sStruct = (string)m_SequenceBin[id];
			string sSEQRES = (string)m_SeqRes[id];
			if( m_PS == null ) return "A ParticleSystem was not present";
			PSMolContainer mol = m_PS.MemberWithID( id );
			if( mol == null ) return "Molecule was not present";
			if( sStruct == null ) return "MoleculeDerived Sequence is not present";
			if( sSEQRES == null ) return "SEQRES was not present for : " + id;

			// we are OK to have a go

			m_Aligner = new SeqAlign( sStruct, sSEQRES );
			// align them to see where we are.
			string seq1; // structural 
			string seq2; // seqres
			m_Aligner.getGappedSequences( out seq1, out seq2 );

			// first perform debug assertions
			if( seq1.Length != seq2.Length )
			{
				throw new Exception("CODE ERROR - Sequence lengths did not match, they always should...");
			}
			bool ok = true;
			for( int k = 0; k < seq1.Length; k++ )
			{
				if( seq1[k] != '-' )
				{
					if( seq1[k] != seq2[k] )
					{
						ok = false;
						break;
					}
				}
			}
			if( !ok )
			{
				throw new Exception( "Sequence validation following alignment did not succeed, either SEQRES or structural information is flase..." );
			}
			// done debug assertions

			int startMolIndex = -1;
			for( int i = 0; i < mol.Count; i++ )
			{
				if( mol[i].ResidueNumber == startResidueID )
				{
					startMolIndex = i;
					break;
				}
			}
			if( startMolIndex == -1 ) throw new Exception("Start index could not be found");
			int endMolIndex = -1;
			for( int i = startMolIndex; i < mol.Count; i++ )
			{
				if( mol[i].ResidueNumber == endResidueID )
				{
					endMolIndex = i;
					break;
				}
			}
			if( endMolIndex == -1 ) throw new Exception("End index could not be found");

			// we have now defined the residues that we wish to use as the start and end points
			// these relate to the ungapped structural sequence
			// these must now be related to the gapped sequence


			int structStartIndex = -1; // the number of -'s that have been added at the start
			for( int i = 0; i < seq1.Length; i++ )
			{
				if( seq1[i] != '-' )
				{
					structStartIndex = i;
					break;
				}
			}

			if( structStartIndex == -1 ) throw new Exception("The entire sequence was blanking markers");

			// startMolIndex = number of letters to ignore ...
			int counter = structStartIndex; // the first non-dash
			int countTo = structStartIndex + startMolIndex; // plus the offset from the start that has the correct residue ID
			int blanks = 0;
			while( counter < countTo )
			{
				if( seq1[counter] == '-' )
				{
					counter++;
                    countTo++;
					blanks++;
				}
				else
				{
					counter++;
				}
			}

			int startSEQRESIndex = structStartIndex + startMolIndex + blanks;

			counter = startSEQRESIndex; // the first non-dash
			countTo = startSEQRESIndex + (endMolIndex - startMolIndex + 1); // plus the offset from the start that has the correct residue ID
			blanks = 0;
			while( counter < countTo )
			{
				if( seq1[counter] == '-' )
				{
					counter++;
					countTo++;
					blanks++;
				}
				else
				{
					counter++;
				}
			}
			int SEQRESLength = (endMolIndex - startMolIndex + 1) + blanks;		

			// we now know the ID of the first defined 
			return sSEQRES.Substring( startSEQRESIndex, SEQRESLength );
		}

		public bool hasSEQRESParseError
		{
			get
			{
				return m_SEQRESError;
			}
		}

		private bool m_SEQRESError = false;
		public void ParseInputLine(string input)
		{
			if( input.Substring(0,6) != "SEQRES" )
			{
				Trace.WriteLine("Invalid line sent to the PDBSequence class, non-SEQRES, it has been ignored.");
				return;
			}

			// int serNum = int.Parse( input.Substring(8,2) ); // dont care
			char chainID = input[11];
			m_TotalResCount = int.Parse( input.Substring(13,4) );
			
            int pos = 19;
			int lineSlot = 0;
			while( m_FoundResCount < m_TotalResCount )
			{
				pos = 19 + (4*lineSlot); 
				if( input[pos] == ' ' && input[pos+1] == ' ' && input[pos+2] == ' ') // 0+1+2 as DNA is listed as " A " "A  " "  A" etc....
				{
					m_SEQRESError = true;
					Trace.WriteLine("Error is seqres parsing, the AA count is incorrect");
					return;
				}
				m_SeqresBuild.Append( PDB.PDBThreeLetterToSingleLetter( input.Substring(pos,3) ) );	
				m_FoundResCount++;
				lineSlot = m_FoundResCount % 13; // 13 are listed max per line
				if( m_FoundResCount % 13 == 0 ) break; // end of this line
			}

			if( m_FoundResCount == m_TotalResCount ) // should never be greater than, if it is, we will have thrown an exception above
			{
				m_FoundResCount = 0;
				m_SeqRes.Add( chainID, m_SeqresBuild.ToString() );
				m_SeqresBuild.Remove( 0, m_SeqresBuild.Length );
			}
		}

		public override string ToString()
		{
			m_TempBuilder.Remove(0,m_TempBuilder.Length); // clear to initialise

			m_TempBuilder.Append( "Sequence information extracted from the structural information present in the file.\r\n\r\n" );

			IDictionaryEnumerator myEnumerator = m_SequenceBin.GetEnumerator();
			while ( myEnumerator.MoveNext() )
			{
                m_TempBuilder.Append( m_PS.Name );
				m_TempBuilder.Append( " chainID : " );
				char chainID = (char) myEnumerator.Key;
				if( chainID == '\0' )
				{
					chainID = ' ';
				}
				m_TempBuilder.Append( chainID );
				m_TempBuilder.Append( "\r\n" );
				m_TempBuilder.Append( (string) myEnumerator.Value );
				m_TempBuilder.Append( "\r\n" );
			}

			m_TempBuilder.Append( "\r\n\r\nSequence information extracted from the SEQRES statements in the PDB file.\r\n\r\n" );

			myEnumerator = m_SeqRes.GetEnumerator();
			while ( myEnumerator.MoveNext() )
			{
				m_TempBuilder.Append( m_PS.Name );
				m_TempBuilder.Append( " chainID : " );
				char chainID = (char) myEnumerator.Key;
				if( chainID == '\0' )
				{
					chainID = ' ';
				}
				m_TempBuilder.Append( chainID );
				m_TempBuilder.Append( "\r\n" );
				m_TempBuilder.Append( (string) myEnumerator.Value );
				m_TempBuilder.Append( "\r\n" );
			}

			m_TempBuilder.Append("\r\n\r\nAttempting to overlay the structural and SEQRES information...\r\n");
			myEnumerator.Reset();
			while ( myEnumerator.MoveNext() )
			{
				if( m_SequenceBin.ContainsKey( myEnumerator.Key ) )
				{
					m_Aligner = new SeqAlign( (string) m_SequenceBin[ (char) myEnumerator.Key ],(string)myEnumerator.Value );
					m_TempBuilder.Append( "Structure Derived Sequence, chain : " + (char) myEnumerator.Key + "\r\n" );
					m_TempBuilder.Append( m_Aligner.ToString() );
					m_TempBuilder.Append( "\r\nSEQRES    Derived Sequence, chain : " + (char) myEnumerator.Key + "\r\n" );
					m_TempBuilder.Append("\r\n");
				}
			}

			return m_TempBuilder.ToString();            			
		}

	}
}
