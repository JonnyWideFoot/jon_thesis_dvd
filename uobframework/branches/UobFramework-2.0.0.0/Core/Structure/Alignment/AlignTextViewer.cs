using System;
using System.Text;
using System.IO;

using UoB.Core.Structure;

namespace UoB.Core.Structure.Alignment
{
	public class AlignTextViewer
	{
		protected ModelList m_Models;
		protected StringBuilder m_StringBuilder;

		public AlignTextViewer( ModelList models )
		{
			m_Models = models;
			m_StringBuilder = new StringBuilder( 6000 );
		}

		public ModelList Models
		{
			get
			{
				return m_Models;
			}
			set
			{
				m_Models = value;
			}
		}

		public void WriteFile( string fileName )
		{
			StreamWriter re = new StreamWriter( fileName );
			ReportAll();
			re.Write( m_StringBuilder.ToString() );
			re.Close();
		}

		public string ReportString
		{
			get
			{
				ReportAll();
				return m_StringBuilder.ToString();
			}
		}

		private void ReportAll()
		{
			m_StringBuilder.Remove(0,m_StringBuilder.Length);
			bool fail = false;
			if( m_Models == null )
			{
				m_StringBuilder.Append( "No Model List is Present\r\n" );
				fail = true;
			}
			else
			{
				if( m_Models.Mol1 == null )
				{
					m_StringBuilder.Append( "Molecule 1 is null\r\n" );
					fail = true;
				}
				if( m_Models.Mol2 == null )
				{
					m_StringBuilder.Append( "Molecule 2 is null\r\n" );
					fail = true;
				}
			}
			if( fail ) return; // we cant report, ro return ...

			for( int i = 0; i < m_Models.ModelCount; i++ )
			{
                Report( i );
			}
   		}

		StringBuilder sM1 = new StringBuilder();
		StringBuilder sStructlyEquiv = new StringBuilder();
		StringBuilder sSequenceEquiv = new StringBuilder();
		StringBuilder sM2 = new StringBuilder();

		private string makeEquivString( Model m, PSMolContainer mol1, PSMolContainer mol2 )
		{
			// initilaise the arrays
			int[] equivs = m.Equivalencies;
			sM1.Remove(0,sM1.Length);
			sM2.Remove(0,sM2.Length);
			sStructlyEquiv.Remove( 0, sStructlyEquiv.Length );
			sSequenceEquiv.Remove( 0, sSequenceEquiv.Length );

			// setup the starts to be in-line
			int firstIDFoundAt = nextEquivFromPoint(m,0);
			if( firstIDFoundAt == -1 )
			{
				return "The equiv list is empty!";
			}

			// the first pair deemed to be structurally equivelent
			int mol1Index = firstIDFoundAt;
			int mol2Index = equivs[mol1Index];

			sM1.Append( mol1[mol1Index].moleculePrimitive.SingleLetterID );
			sM2.Append( mol2[mol2Index].moleculePrimitive.SingleLetterID );
			sStructlyEquiv.Append('*'); // mark them equivs

			mol1Index--; // set the cursor to one backwards
			mol2Index--;
			// now backtrack to fill the start
			while( mol1Index >= 0 || mol2Index >= 0 )
			{
				if(  mol1Index >= 0 )
				{
					sM1.Insert(0, mol1[mol1Index].moleculePrimitive.SingleLetterID );
				}
				else
				{
					sM1.Insert(0,'-');
				}
				if(  mol2Index >= 0 )
				{
					sM2.Insert(0, mol2[mol2Index].moleculePrimitive.SingleLetterID );
				}
				else
				{
					sM2.Insert(0,'-');
				}
				sStructlyEquiv.Insert(0,' ');

				mol1Index--; // work our way back to the start of the sequences
				mol2Index--;
			}

			// now fill to the end
			mol1Index = firstIDFoundAt;
			mol2Index = equivs[ mol1Index ];	
			int mol1NextIndex;
			int mol2NextIndex;

			while( (mol1NextIndex = nextEquivFromPoint( m, mol1Index + 1 ) ) != -1 ) // -1 is returned once the equivs have ended
			{
				// get mol2's index
				mol2NextIndex = equivs[ mol1NextIndex ];

				// somehow fill between the equivs if there are residues in between
				mol2Index = equivs[ mol1Index ]; // the next residues

                // POTENTIAL ERROR: Found this in later debug, whats is for ???
				//mol1Index = mol1Index;           // the next residues

				while( true )
				{			
					mol2Index++;
					mol1Index++;
					if( mol1Index < mol1NextIndex && mol2Index < mol2NextIndex )
					{
						sM1.Append( mol1[mol1Index].moleculePrimitive.SingleLetterID );
						sM2.Append( mol2[mol2Index].moleculePrimitive.SingleLetterID );
					}
					else if( mol1Index < mol1NextIndex )
					{
						sM1.Append( mol1[mol1Index].moleculePrimitive.SingleLetterID );
						sM2.Append('-');
					}
					else if( mol2Index < mol2NextIndex )
					{
						sM1.Append('-');
						sM2.Append( mol2[mol2Index].moleculePrimitive.SingleLetterID );
					}
					else
					{
						break; // we are done here, exit condition						
					}

					sStructlyEquiv.Append(' '); // mark them non-equivs
				}

				// now add the equiv add an equiv
				sM1.Append( mol1[mol1NextIndex].moleculePrimitive.SingleLetterID );
				sM2.Append( mol2[mol2NextIndex].moleculePrimitive.SingleLetterID );
				sStructlyEquiv.Append('*'); // mark them equivs

				mol1Index = mol1NextIndex;
				mol2Index = equivs[ mol1Index ];
			}

			// now fill to the end
			while( mol1Index < mol1.Count || mol2Index < mol2.Count )
			{
				mol1Index++; // work our way back to the start of the sequences
				mol2Index++;
				if( mol1Index < mol1.Count && mol2Index < mol2.Count )
				{
					sM1.Append( mol1[mol1Index].moleculePrimitive.SingleLetterID );
					sM2.Append( mol2[mol2Index].moleculePrimitive.SingleLetterID );
				}
				else if( mol1Index < mol1.Count )
				{
					sM1.Append( mol1[mol1Index].moleculePrimitive.SingleLetterID );
					sM2.Append('-');
				}
				else if( mol2Index < mol2.Count )
				{
					sM1.Append('-');
					sM2.Append( mol2[mol2Index].moleculePrimitive.SingleLetterID );
				}
				sStructlyEquiv.Append(' ');
			}

			// fill the sequence equivs

			for( int i = 0; i < sM1.Length; i++ )
			{
				if( sM1[i] == sM2[i] && sM1[i] != ' ' && sM1[i] != '-' && sM1[i] != ' ' && sM1[i] != '-'  )
				{
					// i.e. its not an insertion or blanks at the start/end
					sSequenceEquiv.Append( '|' );
				}
				else
				{
					sSequenceEquiv.Append( ' ' );
				}
			}

			// use sM1 to build the return string
			sM1.Append("\r\n");
			sM1.Append( sStructlyEquiv );
			sM1.Append("\r\n");
			sM1.Append( sSequenceEquiv );
			sM1.Append("\r\n");
			sM1.Append( sM2 );

			return sM1.ToString();
		}

		private int nextEquivFromPoint( Model m, int lookFrom )
		{
			for( int i = lookFrom; i < m.Equivalencies.Length; i++ )
			{
				if( m.Equivalencies[i] != -1 )
				{
					return i;
				}
			}
			return -1;
		}

		private void Report( int index )
		{
			if( index >= 0 && index < m_Models.ModelCount ) // always should be, we are calling it internally to this class
			{
                Model m = m_Models[index];					
				m_StringBuilder.Append("Model : " + index.ToString() + ". Equivelencies : " + m.numberEquivalencies + ". cRMS : " + m.CRMS + "\r\n" );
				m_StringBuilder.Append( makeEquivString( m, m_Models.Mol1, m_Models.Mol2) );
				m_StringBuilder.Append("\r\n\r\n\r\n");
			}
			else
			{
				m_StringBuilder.Append( "No Model-Definition at given index : " + index.ToString() + "\r\n");
			}
		}
	}
}
