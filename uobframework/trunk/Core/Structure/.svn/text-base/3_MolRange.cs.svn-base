using System;

namespace UoB.Core.Structure
{
	/// <summary>
	/// Summary description for MolRange.
	/// </summary>
	public class MolRange
	{
		protected PSMolContainer m_Polymer;
		protected int  m_StartID;
		protected char m_InsetionCode;
		protected int  m_Length;

		public MolRange( PSMolContainer mol )
		{
			if( mol == null )
			{
				throw new Exception("Molecule cannot be null.");
			}
			m_Polymer = mol;
			m_StartID = mol[0].ResidueNumber;
			m_InsetionCode = ' ';
			m_Length = mol.Count;
		}
		
		public MolRange( PSMolContainer mol, int startID, char insertionCode, int length )
		{
			if( mol == null )
			{
				throw new Exception("Molecule cannot be null.");
			}
			if( m_StartID < 0 )
			{
				throw new Exception("StartID was less than zero");
			}
			m_Polymer = mol;
			m_StartID = startID;
			m_InsetionCode = insertionCode;
			m_Length = length;
		}

		public PSMolContainer CloneFromRange()
		{
			if( m_StartID != -1 && m_Length != -1 )
			{
				// "Activator" used so that we can also initialise polypeptides and other higher order PSMolContainers
				Molecule m = m_Polymer.GetMolecule( m_StartID, m_InsetionCode );
				if( m == null )
				{
					throw new Exception( "Current range is invalid, the molecule does not exist in the host polyPeptide");
				}
				
				PSMolContainer p = (PSMolContainer) Activator.CreateInstance( m_Polymer.GetType(), new object[] { ChainID } );
				for( int i = 0; i < m_Length; i++ )
				{
					p.addMolecule( (Molecule) m_Polymer[m.ArrayIndex + i].Clone() );
				}
				return p;
			}
			else
			{
				return (PSMolContainer)m_Polymer.Clone();
			}
		}

		public PSMolContainer Polymer
		{
			get
			{
				return m_Polymer;
			}
		}

		public char ChainID
		{
			get
			{
				return m_Polymer.ChainID;
			}
		}

		public int StartID
		{
			get
			{
				return m_StartID;
			}
			set
			{
				m_StartID = value;
			}
		}

		public int Length
		{
			get
			{
				return m_Length;
			}
			set
			{
				m_Length = value;
			}
		}

	}
}
