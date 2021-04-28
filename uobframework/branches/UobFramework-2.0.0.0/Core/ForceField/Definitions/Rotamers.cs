using System;
using System.IO;
using System.Diagnostics;
using System.Collections;

using UoB.Core;
using UoB.Core.FileIO.PDB;
using UoB.Core.Structure;
using UoB.Core.Structure.Primitives;

namespace UoB.Core.ForceField.Definitions
{
	/// <summary>
	/// Summary description for Rotamers.
	/// </summary>
	public class Rotamers
	{
		private Rotamers( )
		{
		}

		public static void Assign( MoleculePrimitiveList moleculePrimitives )
		{
			CoreIni init = CoreIni.Instance;
			string path = init.DefaultSharedPath + "rotamers.ff";
			if(!File.Exists( path ))
			{
				throw new Exception( "ERROR : Rotamers file cant be found" );
			}

			Trace.WriteLine( "Initialising Rotamer Definitions" );
			Trace.WriteLine("");
			Trace.Indent();
			Trace.WriteLine( "Rotamer file : " + path );

			StreamReader re = new StreamReader( path );

			PDBAtomList atoms = new PDBAtomList();    // all these lists can contain the same PDBatoms
			PDBAtomList atoms_nt = new PDBAtomList(); // they are intended as holders for constant values
			PDBAtomList atoms_ct = new PDBAtomList(); // PDB atoms should nevr be changed
			PDBAtomList globalAtoms = new PDBAtomList();
			PDBAtomList globalAtoms_nt = new PDBAtomList();
			PDBAtomList globalAtoms_ct = new PDBAtomList();

			MoleculePrimitive mp = null;   // normal primitive
			MoleculePrimitive mpct = null; // C-terminal residue
			MoleculePrimitive mpnt = null; // N-terminal residue

			ArrayList globalNulls = new ArrayList(6); 
			ArrayList globalNulls_nt = new ArrayList(6); 
			ArrayList globalNulls_ct = new ArrayList(6); 
													  // per molPrimitive - temparary holder for the global
													  // atom names that must not be used in that rotamer.

			int resNumber = -1;

			bool globalsAreSet = false;

			string inputLine;
			while ( ( inputLine = re.ReadLine() ) != null )
			{
				if( inputLine.Length < 4 ) 
				{
					continue;
				}

				if( inputLine[0] == '#' )
				{
					continue;
				}

				if( inputLine.Substring(0,11) == "GLOBALSTART" )
				{
					while( (inputLine = re.ReadLine()) != null )
					{
						if( inputLine.Length < 9 ) 
						{
							continue;
						} 
						else if( inputLine.Substring(0,4) == "ATOM" )
						{
							PDBAtom a = new PDBAtom( inputLine );
							char polymerPosition = inputLine[12];
							a.atomName = inputLine.Substring(13,4);
							// we are deviating from the PDB format
							// the polymerPosition is given in the column reserved for numbers
							// the name in the file is the forcefield altname, this is longer than the
							// PDB name by 1, meaning that it overflows into the next column
							// but as the PDBatom class indexes by column number, this doesnt matter

							if( polymerPosition == ' ' )
							{
								// signifies that the atom is aplicable to all residues
								// it should come first in the list cos its always the most common type ...
								globalAtoms.addPDBAtom( a );
								globalAtoms_nt.addPDBAtom( a );
								globalAtoms_ct.addPDBAtom( a );
							}
							else if( polymerPosition == '=' )
							{
								// signifies that the atom is aplicable to only intra-strand residues
								globalAtoms.addPDBAtom( a );
							}
							else if( polymerPosition == '+' )
							{
								// signifies that the atom is only aplicable + terminal residues
								globalAtoms_nt.addPDBAtom( a );
							}
							else if( polymerPosition == '-' )
							{
								// signifies that the atom is only aplicable - terminal residues
								globalAtoms_ct.addPDBAtom( a );
							}
							else if( polymerPosition == '#' )
							{
								// signifies that the atom is only aplicable + terminal residues and intrastrand
								globalAtoms.addPDBAtom( a );
								globalAtoms_nt.addPDBAtom( a );
							}
							else if( polymerPosition == '~' )
							{
								// signifies that the atom is only aplicable - terminal residues and intrastrand
								globalAtoms.addPDBAtom( a );
								globalAtoms_ct.addPDBAtom( a );
							}
							else
							{
								// here we will assume that the other atoms are meant to be a ' '
								// if not however, we will report it to the trace, just to let the user know
								globalAtoms.addPDBAtom( a );
								globalAtoms_nt.addPDBAtom( a );
								globalAtoms_ct.addPDBAtom( a );
								Trace.WriteLine("Possible error found in the rotamer file global definitions. An unknown \"polymerPosition\" chacacter was found : " + polymerPosition.ToString() );
							}
						}
						else if( inputLine.Substring(0,9) == "GLOBALEND" )
						{
							// we have found the end of teh global section
							globalsAreSet = true;
							break;
						}
						else
						{
							// unknownline
						}
					}
					continue;
				}


				if( inputLine.Substring(0,4) == "NEXT" )
				{
					// add the last set of found rotamer atom definitions and increment the molecule primitive
					if ( atoms.Count > 0 || atoms_nt.Count > 0 || atoms_ct.Count > 0 ) // if for some odd reason you get 2 next statements in a row, or one at the start of the definitions - you shouldnt, but ....
					{
						if ( mp != null )
						{
							if ( mpct == null || mpnt == null )
							{
								throw new Exception("Rotamer error : hu? why havent all three been assigned to ???");
							}

							string resname = null;
							if( atoms.Count > 0 )
							{
								resname = atoms[0].residueName;
							}
							else if( atoms_nt.Count > 0 )
							{
								resname = atoms_nt[0].residueName;
							}
							else // we have already checked that at least one holds an atom
							{
								resname = atoms_ct[0].residueName;
							}

							PDBAtomList p   =  globalAtoms.CloneList   ( resname, globalNulls ); 
							// use the name from atom0 for the whole global list
							p.addPDBAtomList( atoms );
							mp.AddRotamerDefinition( p ); 

							PDBAtomList p_nt = globalAtoms_nt.CloneList( resname, globalNulls_nt );
							p_nt.addPDBAtomList( atoms_nt );
							mpnt.AddRotamerDefinition( p_nt );

							PDBAtomList p_ct = globalAtoms_ct.CloneList( resname, globalNulls_ct );
							p_ct.addPDBAtomList( atoms_ct );
							mpct.AddRotamerDefinition( p_ct );

							// we dont care that "atoms" is reused for all/any rotamers
							// the lists dont get changed by external processes
							// they act as holders for positions
						}
						atoms.Clear(); // get a new list for the next rotamer definition
						atoms_nt.Clear();
						atoms_ct.Clear();
						globalNulls.Clear();
						globalNulls_nt.Clear();
						globalNulls_ct.Clear();
					}

					resNumber = -1; // reset for the next rotamer set
					mp = null;
				}

				inputLine = inputLine.PadRight(80,' ');

				if( inputLine.Substring(0,6) == "ATOM  " )
				{
					if ( !globalsAreSet )
					{
						throw new Exception("Fatal exception in rotamer file parsing, the global definitions for the rotamers could not be found." );
					}

					if( inputLine.Substring(32,4) == "NULL" )
					{
						// we have a global atom that should not be in the rotamer definition

						char polymerPositionForNull = inputLine[12];
						string nullName = inputLine.Substring(13,4);

						if( polymerPositionForNull == ' ' )
						{
							// signifies that the atom is aplicable to all residues
							globalNulls.Add( nullName );
							globalNulls_nt.Add( nullName );
							globalNulls_ct.Add( nullName );
						}
						else if( polymerPositionForNull == '=' )
						{
							// signifies that the atom is aplicable to only intra-strand residues
							globalNulls.Add( nullName );
						}
						else if( polymerPositionForNull == '+' )
						{
							// signifies that the atom is only aplicable + terminal residues
							globalNulls_nt.Add( nullName );
						}
						else if( polymerPositionForNull == '-' )
						{
							// signifies that the atom is only aplicable - terminal residues
							globalNulls_ct.Add( nullName );
						}
						else if( polymerPositionForNull == '#' )
						{
							// signifies that the atom is only aplicable + terminal residues and intrastrand
							globalNulls.Add( nullName );
							globalNulls_nt.Add( nullName );
						}
						else if( polymerPositionForNull == '~' )
						{
							// signifies that the atom is only aplicable - terminal residues and intrastrand
							globalNulls.Add( nullName );
							globalNulls_ct.Add( nullName );
						}
						else
						{
							// here we will assume that the other atoms are meant to be a ' '
							// if not however, we will report it to the trace, just to let the user know
							globalNulls.Add( nullName );
							globalNulls_nt.Add( nullName );
							globalNulls_ct.Add( nullName );
							Trace.WriteLine("Possible error found in the rotamer file definitions. An unknown \"polymerPositionForNull\" chacacter was found : " + polymerPositionForNull.ToString() );
						}


                        continue; // and the beat goes on ....
					}

					PDBAtom a = new PDBAtom( inputLine );
					char polymerPosition = inputLine[12];
					a.atomName = inputLine.Substring(13,4);

					if ( resNumber == -1 )
					{
						mp = moleculePrimitives.getPrimitive( a.residueName );
						mpnt = moleculePrimitives.getPrimitive( "N" + a.residueName );
						mpct = moleculePrimitives.getPrimitive( "C" + a.residueName );
					}

					string resname = null;

					if ( resNumber != a.residueNumber )
					{
						resNumber = a.residueNumber;

						// add the current rotamer
						if ( atoms.Count > 0 || atoms_nt.Count > 0 || atoms_ct.Count > 0 )
						{
							if ( mp != null )
							{
								if ( mpct == null || mpnt == null )
								{
									throw new Exception("Rotamer error : hu? why havent all three been assigned to ???");
								}

								if( atoms.Count > 0 )
								{
									resname = atoms[0].residueName;
								}
								else if( atoms_nt.Count > 0 )
								{
									resname = atoms_nt[0].residueName;
								}
								else // we have already checked that at least one holds an atom
								{
									resname = atoms_ct[0].residueName;
								}

								PDBAtomList p   =  globalAtoms.CloneList   ( resname, globalNulls ); 
								// use the name from atom0 for the whole global list
								p.addPDBAtomList( atoms );
								mp.AddRotamerDefinition( p ); 

								PDBAtomList p_nt = globalAtoms_nt.CloneList( resname, globalNulls );
								p_nt.addPDBAtomList( atoms_nt );
								mpnt.AddRotamerDefinition( p_nt );

								PDBAtomList p_ct = globalAtoms_ct.CloneList( resname, globalNulls );
								p_ct.addPDBAtomList( atoms_ct );
								mpct.AddRotamerDefinition( p_ct );

								// we dont care that "atoms" is reused for all/any rotamers
								// the lists dont get changed by external processes
								// they act as holders for positions
							}
							atoms.Clear(); // get a new list for the next rotamer definition
							atoms_nt.Clear(); // get a new list for the next rotamer definition + terminus
							atoms_ct.Clear(); // get a new list for the next rotamer definition - terminus
							globalNulls.Clear();
							globalNulls_nt.Clear();
							globalNulls_ct.Clear();
						}
						
						if( atoms.Count > 0 || atoms_nt.Count > 0 || atoms_ct.Count > 0 )
						{
							Trace.WriteLine("The definition for : " + resname + " was ignored. No Molecule-Primitive could be found. Atom definitions have been discarded");
							atoms.Clear(); // the above " atoms = new PDBAtomList(); " was never triggered as mp,mpct,mpnt were null 
							atoms_nt.Clear();
							atoms_ct.Clear();
							globalNulls.Clear();
							globalNulls_nt.Clear();
							globalNulls_ct.Clear();
						}
					}

					// we have found an atom - it must be added to the apropriate list

					if ( polymerPosition == ' ' )
					{
						// signifies that the atom is aplicable to all residues
						atoms.addPDBAtom( a );
						atoms_nt.addPDBAtom( a );
						atoms_ct.addPDBAtom( a );
					}
					else if( polymerPosition == '=' )
					{
						// signifies that the atom is aplicable to only intra-strand residues
						atoms.addPDBAtom( a );
					}
					else if( polymerPosition == '+' )
					{
						// signifies that the atom is only aplicable + terminal residues
						atoms_nt.addPDBAtom( a );
					}
					else if( polymerPosition == '-' )
					{
						// signifies that the atom is only aplicable - terminal residues
						atoms_ct.addPDBAtom( a );
					}
					else if( polymerPosition == '#' )
					{
						// signifies that the atom is only aplicable + terminal residues and intrastrand
						atoms.addPDBAtom( a );
						atoms_nt.addPDBAtom( a );
					}
					else if( polymerPosition == '~' )
					{
						// signifies that the atom is only aplicable - terminal residues and intrastrand
						atoms.addPDBAtom( a );
						atoms_ct.addPDBAtom( a );
					}
					else
					{
						// here we will assume that the other atoms are meant to be a ' '
						// if not however, we will report it to the trace, just to let the user know
						atoms.addPDBAtom( a );
						atoms_nt.addPDBAtom( a );
						atoms_ct.addPDBAtom( a );
						Trace.WriteLine("Possible error found in the rotamer file normal atom definitions. An unknown \"polymerPosition\" chacacter was found : " + polymerPosition.ToString() );
					}



				}
			}
			Trace.Unindent();
			Trace.WriteLine("");
			Trace.WriteLine("Rotamers Initialised");
			Trace.WriteLine("");
		}
	}
}
