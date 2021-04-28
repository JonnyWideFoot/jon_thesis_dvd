using System;
using System.Text;
using System.Collections;
using System.IO;

namespace UoB.Core.FileIO.DSSP
{
	/// <summary>
	/// Summary description for SegmentDef.
	/// </summary>
	public class SegmentDef // can be a length of either loop or secondary structure
	{
		private int m_ExtArrayIndex = -1;
		private StringBuilder m_Sequence = new StringBuilder();
		private ArrayList m_ResDefs = new ArrayList();
		private bool m_IsIncompleteInPDB = false;

		public SegmentDef( int ExternalArrayIndex )
		{
			m_ExtArrayIndex = ExternalArrayIndex;
		}

		public ResidueDef this[ int segmentIndex ]
		{
			get
			{
				return (ResidueDef) m_ResDefs[ segmentIndex ];
			}
		}

		public void AddResidue( ResidueDef def )
		{
			m_Sequence.Append( def.AminoAcidID );
			if( def.AminoAcidID == '!' )
			{
				m_IsIncompleteInPDB = true;
			}
			m_ResDefs.Add( def );
		}

		public double GetPhi( int index )
		{
			ResidueDef def = (ResidueDef) m_ResDefs[index];
			return def.Phi;
		}

		public double GetPsi( int index )
		{
			ResidueDef def = (ResidueDef) m_ResDefs[index];
			return def.Psi;
		}

		public bool GetIsCisResidue( int index )
		{
			ResidueDef def = (ResidueDef) m_ResDefs[index];
			return def.IsCisResidue;
		}

		public double GetOmega( int index )
		{
			ResidueDef def = (ResidueDef) m_ResDefs[index];
			return def.Omega;
		}

		public int FirstResidueIndex
		{
			get
			{
				return ((ResidueDef) m_ResDefs[0]).ResidueNumber;
			}
		}

		public char FirstResidueInsertionCode
		{
			get
			{
				return ((ResidueDef) m_ResDefs[0]).InsertionCode;
			}
		}

		public int FirstDSSPIndex
		{
			get
			{
				return ((ResidueDef) m_ResDefs[0]).FileIndex;
			}
		}

		public int LastResidueIndex
		{
			get
			{
				return ((ResidueDef) m_ResDefs[m_ResDefs.Count-1]).ResidueNumber;
			}
		}

		public char LastResidueInsertionCode
		{
			get
			{
				return ((ResidueDef) m_ResDefs[m_ResDefs.Count-1]).InsertionCode;
			}
		}

		public int LastDSSPIndex
		{
			get
			{
				return ((ResidueDef) m_ResDefs[m_ResDefs.Count-1]).FileIndex;
			}
		}

		public bool IsIncompleteInPDB
		{
			get
			{
				return m_IsIncompleteInPDB;
			}
		}

		public string Sequence
		{
			get
			{
				return m_Sequence.ToString();
			}
		}

		public int Length
		{
			get
			{
				if( m_IsIncompleteInPDB )
				{
					return -1;
				}
				else
				{
					return m_Sequence.Length;
				}
			}
		}

		public override string ToString()
		{
			 return String.Format( "{0}\t{1}\t{2}\t{3}", 
				FirstDSSPIndex, FirstResidueIndex, Length, Sequence );		
		}

		public static string OutputTitle
		{
			get
			{
				return( "FirstDSSPIndex\tFirstResidueIndex\tLength\tSequence" );
			}
		}       
	}
}
