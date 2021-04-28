using System;
using System.Collections;

using UoB.Core.Primitives;
using UoB.Core.Structure;

namespace UoB.CoreControls.PS_Render
{
	/// <summary>
	/// Summary description for Selection.
	/// </summary>
	public class AminoAcidSelection : Selection
	{
		private int m_Start;
		private int m_Length;
		private PSMolContainer m_Mol;

		public AminoAcidSelection( PSMolContainer mol, int startMolIndex, int length )
		{
			m_AtomIndexes = new ArrayList( length * 10 ); // ish
			Reset( mol, startMolIndex, length );
		}

		public void Reset( PSMolContainer mol, int startMolIndex, int length )
		{
			m_Mol = mol;
			m_Start = startMolIndex;
			m_Length = length;
			GetAtomIndexes();
			autoName();
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
				autoName();
				GetAtomIndexes();
			}
		}

		private void GetAtomIndexes()
		{
			m_AtomIndexes.Clear();

			if( m_Mol != null )
			{
				if( m_Inverted )
				{
					for( int i = 0; i <= m_Start; i++ )
					{
						int[] ints = m_Mol[i].AtomIndexes;
						for( int j = 0; j < ints.Length; j++ )
						{
							m_AtomIndexes.Add(ints[j]);
						}
					}
					for( int i = End + 1; i < m_Mol.Count; i++ )
					{
						int[] ints = m_Mol[i].AtomIndexes;
						for( int j = 0; j < ints.Length; j++ )
						{
							m_AtomIndexes.Add(ints[j]);
						}
					}
				}
				else
				{
					for( int i = m_Start; i <= End; i++ )
					{
						int[] ints = m_Mol[i].AtomIndexes;
						for( int j = 0; j < ints.Length; j++ )
						{
							m_AtomIndexes.Add(ints[j]);
						}
					}
				}
			}
		}

		public PSMolContainer Molecule
		{
			get
			{
				return m_Mol;
			}
			set
			{
				m_Mol = value;
				m_Start = 0;
				m_Length = m_Mol.Count;
				autoName();
				GetAtomIndexes();
			}
		}

		public override string Name
		{
			get
			{
				return m_Name;
			}
			set
			{
				if( value != "" )
				{
					m_Name = value;
				}
				else
				{
					autoName();
				}
			}
		}

		public override void autoName()
		{
			// Autoname
			if( m_Mol is Solvent )
			{
				m_Name = m_Append + "Explicit Solvent";
			}
			else if( m_Mol is HetMolecules )
			{
				m_Name = m_Append + "HetMolecules";
			}
			else
			{
				m_Name = m_Append + "Chain " + m_Mol.ChainID + ", From " + m_Start.ToString() + " To " + (End).ToString();
			}
		}

		public void setBounds( int start, int length )
		{
			m_Start = start;
			m_Length = length;
			autoName();
			GetAtomIndexes();
		}

		public int Start
		{
			get
			{
				return m_Start;
			}
		}

		public int Length
		{
			get
			{
				return m_Length;
			}
		}

		public int End
		{
			get
			{
				return m_Length + m_Start - 1;
			}
		}
	}
}
