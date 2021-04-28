using System;
using System.Collections;

using UoB.Core.Structure;

namespace UoB.CoreControls.PS_Render
{
	/// <summary>
	/// Summary description for Selection_CAlphaEquiv.
	/// </summary>
	public class Selection_CAlphaEquiv : Selection
	{
		private PSMolContainer m_Mol1;
		private PSMolContainer m_Mol2;
		private int[] m_Equivs;

		public Selection_CAlphaEquiv( PSMolContainer mol1, PSMolContainer mol2, int[] equiv )
		{
			m_Mol1 = mol1;
			m_Mol2 = mol2;
			m_Equivs = equiv;
			m_AtomIndexes = new ArrayList( equiv.Length * 10 ); // ish
			GetAtomIndexes();
		}

		public void Reset( PSMolContainer mol1, PSMolContainer mol2, int[] equiv )
		{
			m_Mol1 = mol1;
			m_Mol2 = mol2;
			m_Equivs = equiv;
			GetAtomIndexes();
		}

		public override bool Inverted
		{
			get
			{
				return m_Inverted;
			}
			set
			{
				m_Inverted = value;
			}
		}

		public override void autoName()
		{
			m_Name = "Equiv Selection";
		}

		public override string Name
		{
			get
			{
				return m_Name;
			}
			set
			{
				m_Name = value;
			}
		}

		private void GetAtomIndexes()
		{
			m_AtomIndexes.Clear();

			for( int i = 0; i < m_Equivs.Length; i++ )
			{
				if( m_Equivs[i] != -1 )
				{
					int[] ints = m_Mol1[i].AtomIndexes;
					for( int j = 0; j < ints.Length; j++ )
					{
						m_AtomIndexes.Add(ints[j]);
					}

					ints = m_Mol2[m_Equivs[i]].AtomIndexes;
					for( int j = 0; j < ints.Length; j++ )
					{
						m_AtomIndexes.Add(ints[j]);
					}
				}
			}
		}
	}
}
