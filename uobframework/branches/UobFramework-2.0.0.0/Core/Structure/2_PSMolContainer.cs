using System;
using System.Collections;
using UoB.Core.Primitives;

namespace UoB.Core.Structure
{
	/// <summary>
	/// Summary description for Solvent.
	/// </summary>
	public class PSMolContainer : MoleculeList
	{
		protected int m_ArrayIndex = -1;
		protected Char m_ChainID;
		protected ParticleSystem m_Parent;

		public PSMolContainer( char chainID )
		{
			m_AllowClear = false;
			m_ChainID = chainID;
			m_Parent = null;
		}

		public int ArrayIndex
		{
			get
			{
				return m_ArrayIndex;
			}
			set
			{
				m_ArrayIndex = value;
			}
		}

		public ParticleSystem Parent
		{
			get
			{
				return m_Parent;
			}
			set
			{
				m_Parent = value;
			}
		}

		public char ChainID
		{
			get
			{
				return m_ChainID;
			}
			set
			{
				m_ChainID = value;
			}
		}

		public Molecule GetMolecule( int residueID, char insertionCode )
		{
			for( int i = 0; i < this.Count; i++ )
			{
				if( this[i].ResidueNumber == residueID 
					&& this[i].InsertionCode == insertionCode )
				{
					return this[i];
				}
			}
			return null;
		}

		public override string ToString()
		{
			return "PS Molecule Container";
		}

		#region ICloneable Members

		public override object Clone()
		{
			PSMolContainer hm = new PSMolContainer( m_ChainID );
			for ( int i = 0; i < m_Molecules.Count; i++ )
			{
				Molecule m = (Molecule) m_Molecules[i];
				hm.addMolecule( (Molecule) m.Clone() ); // adding the cloned molecule assigs the parent parameter of the molecule
			}
			return hm;
		}

		#endregion
	}
}
