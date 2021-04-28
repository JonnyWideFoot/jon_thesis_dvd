using System;
using System.Collections;

namespace UoB.Core.Structure
{
	/// <summary>
	/// Summary description for PolyPeptide.
	/// </summary>
	public class PolyPeptide : PSMolContainer
	{
		public PolyPeptide ( char chainID ) : base ( chainID )
		{
        }

		public new AminoAcid GetMolecule( int residueID, char insertionCode )
		{
            return (AminoAcid) base.GetMolecule( residueID, insertionCode );		
		}

		public new AminoAcid this[ int index ]
		{
			get
			{
				return (AminoAcid) base[ index ];
			}
		}

		public override string ToString()
		{
			return "PolyPeptide : " + m_ChainID.ToString();
		}

		public override object Clone()
		{
			PolyPeptide p = new PolyPeptide( m_ChainID );
			for ( int i = 0; i < m_Molecules.Count; i++ )
			{
				AminoAcid aa = (AminoAcid) m_Molecules[i];
				p.addMolecule( (AminoAcid) aa.Clone() ); // addition assigs the parent variable of the molecule
			}
			return p;
		}

        public override object Clone(int start, int length)
        {
            PolyPeptide hm = new PolyPeptide(m_ChainID);
            for (int i = start; i < start + length; i++)
            {
                AminoAcid m = (AminoAcid)m_Molecules[i];
                hm.addMolecule((AminoAcid)m.Clone()); // adding the cloned molecule assigs the parent parameter of the molecule
            }
            return hm;
        }
	}
}
