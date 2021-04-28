using System;
using System.Diagnostics;
using System.Collections;

using UoB.Core.Structure;

namespace UoB.Core.FileIO.PDB
{
	/// <summary>
	/// Used to specify aditional bonding that is not defined by the forcefield
	/// Used in the PDB class to implement the connect statements
	/// </summary>
	public sealed class EntendedBondList : BondList
	{
		public ParticleSystem m_PS;
		private ArrayList m_Definitions = new ArrayList();

		public EntendedBondList( ParticleSystem ps )
		{
			m_PS = ps;
		}

		private void SetupBonds()
		{
			// this should only be called following PDB file creation
			// we assume here that the ps has had .EndEditing() called 

			// NOTE : NMR Structures have connect statements for each model present
			// As DAVE uses each subsequent model as a new position set, these extended bonds will be ignored
			// only connect statements refering to teh 1st model will be used

			int[] theInts;
			for( int k = 0; k < m_Definitions.Count; k++ )
			{
				theInts = (int[]) m_Definitions[k];
							
				// Index, Covalent, Covalent, Covalent, Covalent, Hydrogen, Hydrogen, SaltBridge, Hydrogen, Hydrogen, SaltBridge, 
				// Find Atom I if we have it
				if( theInts[0] == -1 ) continue;
				Atom atomI = null;
				for( int j = 0; j < m_PS.Count; j++ )
				{
					if( m_PS[j].AtomNumber == theInts[0] )
					{
						atomI = m_PS[j];
						break;
					}
				}
				if( atomI == null )
				{
					// this will be for the aditional Models, we assume ....
					continue;
				}

				// Now bond it to some shite
				for ( int i = 1; i < theInts.Length; i++ )
				{
					if( theInts[i] == -1 ) continue; // there is nothing at this position

					for( int j = 0; j < m_PS.Count; j++ )
					{
						if( m_PS[j].AtomNumber == theInts[i] )
						{
							Atom atomJ = m_PS[j];

							if( atomJ == null )
							{
								// should never get here ....
								break;
							}

							if( i == 1 || i == 2 || i == 3 || i == 4 )
							{
								if( atomI.atomPrimitive.Element == 'S' &&
									atomJ.atomPrimitive.Element == 'S' )
								{
									m_Bonds.Add( new Bond( atomI, atomJ, 1, BondType.DiSulphide ) );
								}
								else
								{
									m_Bonds.Add( new Bond( atomI, atomJ, 1, BondType.Covalent ) );
								}
							}
							else if( i == 5 || i == 6 || i == 8 || i == 9 )
							{
								m_Bonds.Add( new Bond( atomI, atomJ, 1, BondType.HBond ) );
							}
							else if( i == 7 || i == 10 )
							{
								m_Bonds.Add( new Bond( atomI, atomJ, 1, BondType.Ionic ) );
							}
							else
							{
								throw new Exception( "How the hell have i got here ? : EntendedBondList.cs" );
							}
						}
					}
				}
			}
			m_Definitions = null;
		}

		public void addBond( string PDBLine )
		{
			m_Definitions.Add( MakeBondIntArray( PDBLine ) );
		}

		private int[] MakeBondIntArray(string inputLine)
		{
			int start = 6;
			int length = 5;
			int[] atomNums = new int[11];
			for ( int i = 0; i < atomNums.Length; i++ )
			{
				try
				{
					atomNums[i] = int.Parse(inputLine.Substring( start, length ) );
				}
				catch
				{
					atomNums[i] = -1;
				}
				start += length;
			}
			return atomNums;
		}


		public void ApplyBonding()
		{
			if( m_Definitions != null ) // not null after initialisation
			{
				SetupBonds();
			}

			if( m_PS != null )
			{
				Trace.WriteLine("Processing extended bond list...");
				Trace.Indent();
				int i = 0;
				while( i < m_Bonds.Count )
				{
					Bond b = (Bond) m_Bonds[i];
					if( m_PS.Contains( b.farAtom ) && m_PS.Contains( b.sourceAtom ) )
					{
						if( !b.farAtom.bondedTo( b.sourceAtom ) )
						{
							b.farAtom.bondTo( b.sourceAtom, b.bondType );
						}
						if( !b.sourceAtom.bondedTo( b.sourceAtom ) )
						{
							b.farAtom.bondTo( b.farAtom, b.bondType );
						}
						i++;
					}
					else
					{
						m_Bonds.RemoveAt(i);
					}		
				}
				Trace.Unindent();
				Trace.WriteLine("Extended BondList Processed");
			}
		}
	}
}
