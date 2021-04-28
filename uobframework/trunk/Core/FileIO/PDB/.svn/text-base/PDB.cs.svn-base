using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;

using UoB.Core.Structure;
using UoB.Core.Tools;
using UoB.Core.Data;
using UoB.Core.Primitives;
using UoB.Core.FileIO;
using UoB.Core.FileIO.FormattedInput;
using UoB.Core.Structure.Primitives;

namespace UoB.Core.FileIO.PDB
{
	/// <summary>
	/// Summary description for PDB.
	/// </summary>
	public sealed class PDB : BaseFileType_Structural
	{
		// note that for the code below to work, the 3 letter and 1 letter orders must be the same !
		public static readonly char[] singleLetterAAs = new char[] { 'a', 't', 'g', 'c', 'A', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'Y' };
		public static readonly string[] threeLetterAminoAcids = new string[] { " A ", " T ", " G ", " C ", "ALA", "CYS", "ASP", "GLU", "PHE", "GLY", "HIS", "ILE", "LYS", "LEU", "MET", "ASN", "PRO", "GLN", "ARG", "SER", "THR", "VAL", "TRP", "TYR" };
		//private readonly string[] warningThreeLetterAminoAcidCodes = new string[] { "seleno cysteine and co" };


		/// <summary>
		/// NOTE: a, t, c, g are nucleic acids, the capital letters are the amino acids
		/// </summary>
		/// <param name="singleLetter"></param>
		/// <returns></returns>
		public static string PDBSingleLetterToThreeLetter( char singleLetter )
		{
			for( int i = 0; i < singleLetterAAs.Length; i++ )
			{
				if( singleLetterAAs[i] == singleLetter )
				{
					return threeLetterAminoAcids[i];
				}
			}
			return null;
		}

		public static char PDBThreeLetterToSingleLetter( string threeLetter )
		{
			if( threeLetter.Length != 3 )
			{
				throw new Exception("The given sequence was the wrong length");
			}
			if( 0 == string.Compare( threeLetter, 0, "CSE", 0, 3, true ) )
			{
				threeLetter = "CYS";
			}
			else if( 0 == string.Compare( threeLetter, 0, "MSE", 0, 3, true ) )
			{
				threeLetter = "MET";
			}
			for( int i = 0; i < threeLetterAminoAcids.Length; i++ )
			{
				if( 0 == string.Compare( threeLetterAminoAcids[i], 0, threeLetter, 0, 3, true ) )
				{
					return singleLetterAAs[i];
				}
			}
            return '?';
		}

		public static void AttemptCorrentIDFormat( ref string donor )
		{
			// static function to cull the string and pad in the corrent locations to the corrent length
			donor = donor.Trim(); // trim whitespace
			if( donor.Length == 4 )
			{
				// its already ok
				return;
			}
			if( !char.IsNumber(donor[0]) )
			{
				// then the 1st character must be a space
                donor = " " + donor;
			}
			if( donor.Length == 4 )
			{
				// its already ok
				return;
			}
			donor = donor.PadRight(4,' ');
			if( donor.Length > 4 )
			{
				donor = donor.Substring(0,4);			
			}
			return;
		}

		private EntendedBondList m_ConnectStatements;
		private int m_AtomCreationCounter = 0; // used to give each atom a unique index
		private PDBAtomList m_AltLocStore = new PDBAtomList(); // temp holder is later reset to null

		public PDB( string fullFilePath, bool extractPS ) : base( fullFilePath, extractPS )
		{
			m_CanSave = true;
			m_IsTextBased = true; // flag that this file can be viewed in a text editor
		}

        public override bool Silent
        {
            get
            {
                return m_Silent;
            }
            set
            {
                m_Silent = value;

                // Inform the children
                m_ConnectStatements.Silent = value;
            }
        }

		public new PDBInfo ExtendedInformation
		{
			get
			{
				return (PDBInfo) m_ExtendedInfo;
			}
		}

		public new PDBSequence SequenceInfo
		{
			get
			{
				return (PDBSequence) m_Sequence;
			}
		}


		#region OverridenBaseFunctions
		protected override void ExtractExtendedInfo()
		{
			// initialise the sequence first, ExtInfo will extract the seqres statements during parsing
			m_Sequence = new PDBSequence(); // this is a more complex PDB seqence class that has both PS derived seqences and the seqres statements
			m_ExtendedInfo = new PDBInfo( m_FileInfo.FullName, m_InternalName, (PDBSequence) m_Sequence );
		}
			
		protected override void ExtractParticleSystem()
		{
			StreamReader re;
			try 
			{
				re = File.OpenText( m_FileInfo.FullName );
			}
			catch(Exception e)
			{
				if( !m_Silent ) Trace.WriteLine(@"File Not Available \ Cannot Open : " + m_FileInfo.FullName  + " Because : " + e.ToString());
				return;
			}

			if( !m_Silent ) Trace.WriteLine("");
			if( !m_Silent ) Trace.WriteLine("Imporing PDB Atom Definitions for : " + m_InternalName );

			int UnProcessedLines = 0;
			bool firstModelDone = false;
            bool secondModelDone = false;
            List<PDBAtom> secondModelAtoms = new List<PDBAtom>();
			string linePassBackBuffer = null; // used so that if a block finds a line that is not relevent to it, it can be passed back

			m_PS = new ParticleSystem( m_InternalName );
			m_ConnectStatements = new EntendedBondList( m_PS );
            m_ConnectStatements.Silent = m_Silent;
			// these will be added to the PS if relevent molecules are found
			Solvent solvent = new Solvent();
			HetMolecules hetMolecules = new HetMolecules();
            m_PS.BeginEditing(); // begin the edir mode so that we can begin to add members		

			// as the m_Atoms array of the PS is not filled until .EndEditing() is called,
			// new positions cannot be added to the PS_PositionHolder until the end, we will therefore hold them here
			ArrayList TempModelPositionHolder = new ArrayList(2000); // holds the positions temporarily per model
            List<Position[]> tempModelContainer = new List<Position[]>(20);        // holds the models

			string lineType = null;
			string inputLine = null;
			while (true)
			{
				if( null == ( inputLine = getLine( re, ref linePassBackBuffer ) ) )
				{
					break;
				}
				lineType = inputLine.Substring(0,6).ToUpper();

				if( lineType == "HETATM" )
				{
					string checkFromSelenoMolecules = inputLine.Substring(17,3);
					if( checkFromSelenoMolecules == "CSE" || checkFromSelenoMolecules == "MSE" )
					{
						lineType = "ATOM  "; // these need to be passed as atoms to be sent to the polypeptide loop
					}
				}

				switch ( lineType )
				{
					case "ATOM  " :
						// In this program we have the policy that full atom objects are only created for the first model
						// contained within the PDB file. Any subsequent models are stored as positions only, and these
						// are applied to the contained atoms when required.
						// this has the one disadvantage of problems when the PDB file is erroraneous and has a 
						// different number of positions in subsequent models compared to the 1st. These are ignored
						// by the program as the arrays are required to be of the same length.
						if ( !firstModelDone ) // set to true when an ENDMDL block is reached
						{
							// spawn the function to read a block and return once an unknown line is found of the poly peptide definition ends
							m_PS.AddMolContainer( ReadPolypeptideBlock( re, ref inputLine, ref linePassBackBuffer ) );
						}
						else
						{
							// Original comment
							// only needs to be done for the subsequent models as the PS_PositionHolder automatically includes the initial particle coordinates
							TempModelPositionHolder.Add( getPositionOnly( inputLine ) );
                            if (!secondModelDone)
                            {
                                secondModelAtoms.Add( new PDBAtom( inputLine ) );
                            }
						}
						break;
					case "HETATM" :
						if ( !firstModelDone )
						{
							Molecule m = ReadMoleculeBlock( re, ref inputLine, ref linePassBackBuffer );
							if ( m.moleculePrimitive.IsSolvent )
							{
								solvent.addMolecule( m );
							}
							else
							{
								hetMolecules.addMolecule( m );
							}
						}
						else
						{
							// only needs to be done for the subsequent models as the PS_PositionHolder automatically includes the initial particle coordinates
							TempModelPositionHolder.Add( getPositionOnly( inputLine ) );
						}
						break;
					case "MODEL ":
						// do nothing
						break;
					case "CONECT":
						//m_ConnectStatements.addBond( inputLine );
						break;
					case "ENDMDL":
						if( !firstModelDone )
                        {
                            firstModelDone = true;
                        }
                        else
                        {
                            secondModelDone = true;
                        }
						if ( TempModelPositionHolder.Count > 0 ) // after one has been created above in the ATOM and HETATM statements
						{
							// read above as to why we need a temp holder
							tempModelContainer.Add( (Position[]) TempModelPositionHolder.ToArray( typeof(Position) ) );
						}
						TempModelPositionHolder.Clear();
						break;
					default:
						UnProcessedLines++;
						break;
				}
			}

			re.Close();

			if( solvent.Count > 0 )
			{
				m_PS.AddMolContainer( solvent );
			}
			if( hetMolecules.Count > 0 )
			{
				m_PS.AddMolContainer( hetMolecules );
			}

			m_PS.EndEditing( true, false );

			m_ConnectStatements.ApplyBonding(); // done following endEditing() as they are in addition to FF bonding
			// bonding and atom allocation needs to be performed prior to model allocation

			m_Sequence.particleSystem = m_PS;
			m_Positions = new PS_PositionStore( m_PS ); // now lets get any models and apply to the posStore

			// these must be added following the call to EndEditing above ...
			if( tempModelContainer.Count > 0 && m_AltLocStore.Count > 0 ) // tricky !
			{
				throw new Exception("PLEASE PLEASE SEND JON THE PDB FILE YOU JUST OPENED. It Contains models which have altlocs within then. This code is untested as i dont have a file");

				//				// make a "model" per altLoc letter group per model
				//				Hashtable models = new Hashtable();
				//
				//				int altLocsPerModel = altLocPositionStore.Count / tempModelContainer.Count;
				//
				//				if( (altLocPositionStore.Count % tempModelContainer.Count) != 0 )
				//				{
				//                    throw new Exception("Well rodger me sideways, we apear to have an error");
				//				}
				//
				//				// loop for 1st model ONLY
				//				// first make the required number of position arrays to account for all of model1's altloc IDs
				//				for( int i = 0; i < altLocsPerModel; i++ )
				//				{
				//					PDBAtom a = (PDBAtom) altLocPositionStore[i];
				//					char id = a.altLocIndicator;
				//					if( !models.ContainsKey( id ) )
				//					{
				//						int count = m_PS.Count;
				//						Position[] p = new Position[ count ];
				//						for( int q = 0; q < count; q++ )
				//						{
				//							p[q] = m_PS[q].ClonePosition; // initiialise to the first altloc location letter in the model
				//						}
				//						models[id] = p;
				//					}
				//					Position[] model = (Position[]) models[id];
				//					Position pAlt = ((PDBAtom)altLocPositionStore[i]).position;
				//					model[ (int)altLocIDStore[i] ].setTo( pAlt.x, pAlt.y, pAlt.z );                                        
				//				}
				//				foreach( Position[] pMod in models )
				//				{
				//					m_Positions.addPositionArray( pMod, false );
				//				}
				//
				//                // thats done, but now we need to sort out the following full models
				//				for( int i = 0; i < tempModelContainer.Count; i++ )
				//				{
				//					Position[] initialPos = (Position[]) tempModelContainer[i];
				//					// the psotion array to make into multiple models
				//
				//					foreach( string key in models.Keys )
				//					{
				//						ArrayList Model = new ArrayList( initialPos.Length );
				//						for( int g = 0; g < initialPos.Length; g++ )
				//						{
				//							Model.Add( initialPos[g] ); // copy references to them all
				//						}
				//
				//						models[key] = Model;
				//					}
				//
				//					// right then .. for this large model with altlocs in, we want to make more non-altloc models
				//
				//					if( altLocPositionStore.Count != altLocIDStore.Count )
				//					{
				//						throw new Exception("Arrrg, me harties .. we hit rock .. splice the main-brace");
				//					}
				//
				//					for( int j = 0; j < altLocPositionStore.Count; j++ ) // use these for names
				//					{
				//						PDBAtom a = (PDBAtom) altLocPositionStore[j];
				//						char id = a.altLocIndicator;
				//						ArrayList Model = (ArrayList) models[id];
				//
				//						int indexToMess = (int)altLocIDStore[j];
				//						// now find what Model index is "id"
				//						// and set
				//
				//						Position p1 = (Position) Model[ indexToMess ];
				//						int donorIndex = 0; //theindexweneedtofindwhichisthemodelindexofid;
				//						Position p2 = (Position) Model[ indexToMess + donorIndex ];
				//						p1.setTo( p2 );
				//						Model.RemoveAt( donorIndex ); // this is fine as we want to redce the array size and the following donors are currently at +1 +2 +3 from the index in altLocPositionStore				
				//					}
				//
				//					foreach( ArrayList a in models )
				//					{
				//                        m_Positions.addPositionArray( (Position[]) a.ToArray( typeof(Position) ), false );
				//					}  
				//				}				              
			}
			else if( tempModelContainer.Count > 0 )
			{
                SortOutContainer(m_PS, secondModelAtoms, tempModelContainer );
			}
			else if( m_AltLocStore.Count > 0 ) // if there are no models, we will make models for the altlocs
			{
				// make a "model" per altLoc letter group
				Hashtable models = new Hashtable();

				// first make the required number of position arrays
				for( int i = 0; i < m_AltLocStore.Count; i++ )
				{
					PDBAtom a = (PDBAtom) m_AltLocStore[i];
					char id = a.altLocIndicator;
					if( !models.ContainsKey( id ) )
					{
						int count = m_PS.Count;
						Position[] p = new Position[ count ];
						for( int qq = 0; qq < count; qq++ )
						{
							p[qq] = m_PS[qq].ClonePosition;
						}
						models[id] = p;
					}
					Position[] model = (Position[]) models[id];
					PDBAtom pAlt = (PDBAtom)m_AltLocStore[i];
					
					// find the atom that we need to fiddle with
					bool wasSet = false;
					for( int j = 0; j < m_PS.Count; j++ )
					{
						Atom ato = m_PS[j];
						if( ato.parentMolecule.ResidueNumber == pAlt.residueNumber &&
							// now compare the last 3 digits of the molname, to take into account that it can now have an N or C prefix
							0 == String.Compare( ato.parentMolecule.Name, ato.parentMolecule.Name.Length - 3, pAlt.residueName, 0, 3, true ) &&
							ato.parentMolecule.parentChainID == pAlt.chainID &&
							ato.parentMolecule.InsertionCode == pAlt.insertionCode &&
							ato.PDBType == pAlt.atomName )
						{
							wasSet = true;
							model[j].setTo( pAlt.position );
							break;
						}
					}
					if( !wasSet ) // then moan lots
					{
						if( !m_Silent ) Trace.WriteLine("AltLoc ignored, the host atom could not be found...");
					}
				}

				foreach( object pModkey in models.Keys )
				{
					m_Positions.addPositionArray( (Position[]) models[pModkey], false );
				}

				models.Clear();
			}

			m_AltLocStore = null; // done with it

			Trace.Indent();
			if( !m_Silent ) Trace.WriteLine( "" );
			if( !m_Silent ) Trace.WriteLine( "Import Summary :" );
			if( !m_Silent ) Trace.WriteLine( m_PS.Count.ToString() + " Atoms have been imported" );
			if( !m_Silent ) Trace.WriteLine( "" );
			Trace.Unindent();

			if( !m_Silent ) Trace.WriteLine("Initialisation Complete for : " + m_PS.Name );
		}

        private void SortOutContainer(ParticleSystem ps, List<PDBAtom> secondModelAtom, List<Position[]> tempModelContainer )
        {
            // As the particle system atomsa re now in the order they appear in the forcefield, all the subsequent model atoms must now be reordered... Grr.

            // Ensure that the atomNumber we are using as an index is within the bounds of our array.
            for( int i = 0; i < secondModelAtom.Count; i++ )
            {
                secondModelAtom[i].atomNumber = i;
            }

            if (ps.Count != secondModelAtom.Count)
            {
                throw new Exception("SecondModelAtom index mismatch!");
            }

            for (int k = 0; k < tempModelContainer.Count; k++)
            {
                if (ps.Count != tempModelContainer[k].Length)
                {
                    throw new Exception("Model atom count mismatch!");
                }
            }

            Position swap = new Position();

            for( int i = 0; i < ps.Count; i++ )
            {
                bool flagged = false;
                for( int j = 0; j < secondModelAtom.Count; j++ )
                {
                    if( ps[i].parentMolecule.ResidueNumber == secondModelAtom[j].residueNumber &&
                        ps[i].parentMolecule.InsertionCode == secondModelAtom[j].insertionCode &&
                        0 == String.CompareOrdinal(ps[i].PDBType,secondModelAtom[j].atomName ) && 
                        0 == String.CompareOrdinal(ps[i].parentMolecule.Name_NoPrefix, secondModelAtom[j].residueName) )
                    {
                        flagged = true;    
                        int swapA = secondModelAtom[j].atomNumber;
                        if (i != swapA)
                        {
                            bool innerFlag = false;
                            for (int k = 0; k < secondModelAtom.Count; k++)
                            {
                                if (secondModelAtom[k].atomNumber == i)
                                {
                                    secondModelAtom[k].atomNumber = swapA;
                                    innerFlag = true;
                                    break;
                                }
                            }
                            if (!innerFlag) throw new Exception("Code saaumption failed");
                            // Swap required to maintain atom correspondance
                            for (int k = 0; k < tempModelContainer.Count; k++)
                            {
                                // Perform coordinate swap
                                Position a = tempModelContainer[k][swapA];
                                Position b = tempModelContainer[k][i];
                                swap.setTo(a);
                                a.setTo(b);
                                b.setTo(swap);
                            }
                        }
                        secondModelAtom.RemoveAt(j);
                        break;
                    }
                }
                if( !flagged ) 
                {
                    throw new Exception("Model atoms are invalid, there is a name mismatch!");
                }
            }


            if (secondModelAtom.Count != 0) throw new Exception("Code assumption failure");

            for (int i = 0; i < tempModelContainer.Count; i++)
            {
                m_Positions.addPositionArray(tempModelContainer[i], false);
            }
        }

		public override void Save( SaveParams saveParams )
		{
            SaveNew( saveParams, m_PS );
		}

		public override void Save( string fileName )
		{
			SaveNew( fileName, m_PS );
		}

		public void Save( string fileName, bool outputSEQRES )
		{
			SaveNew( fileName, m_PS, outputSEQRES );
		}

		public static void SaveNew( SaveParams saveParams, ParticleSystem ps )
		{
			SaveNew( saveParams.FileName, ps, saveParams.SaveSequence );
		}

        public static void WriteSSBONDs(StreamWriter rw, ParticleSystem ps)
        {
            int startIndex = 1;
            for (int i = 0; i < ps.MemberCount; i++)
            {
                WriteSSBONDs(rw, ps, i, ref startIndex);
            }
        }

        public static void WriteSSBONDs(StreamWriter rw, ParticleSystem ps, int molIndex, ref int startIndex )
        {
            PSMolContainer mol = ps.MemberAt(molIndex);
            if (mol == null) throw new ArgumentOutOfRangeException();

            for (int i = 0; i < mol.Count; i++)
            {
                if( (0 == String.Compare(mol[i].Name_NoPrefix, "CYX", true)) ||
                    (0 == String.Compare(mol[i].Name_NoPrefix, "CYS", true)) )
                {
                    for (int j = i + 1; j < mol.Count; j++)
                    {
                        if ( (0 == String.Compare(mol[j].Name_NoPrefix, "CYX", true)) ||
                             (0 == String.Compare(mol[j].Name_NoPrefix, "CYS", true)) )
                        {
                            Atom a = mol[i].AtomOfType(" SG ");
                            Atom b = mol[j].AtomOfType(" SG ");
                            if (a != null && b != null)
                            {
                                if (8.0 > a.distanceSquaredTo(b))
                                {                                    
                                    rw.Write("SSBOND"); // 1 -> 6
                                    rw.Write(' '); // 7
                                    rw.Write("{0,3:G}", startIndex); // 8 -> 10
                                    rw.Write(' '); // 11

                                    rw.Write("CYS"); // 12 -> 14
                                    rw.Write(' '); // 15
                                    rw.Write( mol.ChainID ); // 16
                                    rw.Write(' '); // 17
                                    rw.Write("{0,4:G}", mol[i].ResidueNumber);// 18 -> 21
                                    rw.Write(mol[i].InsertionCode); // 22

                                    rw.Write("   ");// 23 -> 25

                                    rw.Write("CYS"); // 26 -> 28
                                    rw.Write(' '); // 29
                                    rw.Write(mol.ChainID); // 30
                                    rw.Write(' '); // 31
                                    rw.Write("{0,4:G}", mol[j].ResidueNumber);// 32 -> 35
                                    rw.Write(mol[j].InsertionCode); // 36

                                    rw.WriteLine(new String(' ', 44)); // 37 -> 80

                                    startIndex++;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void SaveSEQRESTo(StreamWriter rw, ParticleSystem ps)
		{
			//SEQRES   1 E  181  MET ASN GLN LYS ALA VAL ILE LEU ASP GLU GLN ALA ILE          
			//SEQRES   2 E  181  ARG ARG ALA LEU THR ARG ILE ALA HIS GLU MET ILE GLU          
			//SEQRES   3 E  181  ARG ASN LYS GLY MET ASN ASN CYS ILE LEU VAL GLY ILE    
			//SEQRES   1 H  379  ILE LYS SER ALA LEU LEU VAL LEU GLU ASP GLY THR GLN          
			//SEQRES   2 H  379  PHE HIS GLY ARG ALA ILE GLY ALA THR GLY SER ALA VAL          
			//SEQRES   3 H  379  GLY GLU VAL VAL PHE ASN THR SER MET THR GLY TYR GLN

			for( int i = 0; i < ps.MemberCount; i++ )
			{
				PolyPeptide pp = ps.MemberAt(i) as PolyPeptide;
				int lineNumber = 0;
				if( pp != null )
				{
					int count = 0;
					do
					{
                        lineNumber++;

						s.Remove( 0, s.Length );

						s.Append( "SEQRES" );
						s.Append( lineNumber.ToString().PadLeft(4,' ') );
						s.Append( ' ' );
						s.Append( pp.ChainID );
						s.Append( pp.Count.ToString().PadLeft( 5, ' ' ) );
						s.Append( ' ' );
						s.Append( ' ' );

						for( int j = 0; j < 13; j++ )
						{
							if( count < pp.Count )
							{
								string name = pp[count].Name;
								s.Append( name, name.Length - 3, 3 );
								s.Append( ' ' );
								count++;
							}
							else
							{
								break;
							}
						}

						s.Append( ' ', 80 - s.Length );
                    	rw.WriteLine( s.ToString() );
					}
					while( count < pp.Count);
				}
			}
		}

		public static void SaveAtomsTo( StreamWriter rw, ParticleSystem ps )
		{
			for( int i = 0; i < ps.Count; i++ )
			{
				rw.WriteLine( MakePDBStringFromAtom(ps[i]) );
			}
		}

		public static void SaveNew( string fileName, ParticleSystem ps )
		{
			SaveNew( fileName, ps, false, null, false );
		}

		public static void SaveNew( string fileName, ParticleSystem ps, bool outputSEQRES )
		{
			SaveNew( fileName, ps, outputSEQRES, null, false );
		}

		public static void SaveNew( string fileName, ParticleSystem ps, bool outputSEQRES, PDBInfo extInfo, bool verbose )
		{
            ParticleSystem seqres = null;
            if( outputSEQRES ) seqres = ps;
            SaveNew(fileName, ps, seqres, extInfo, verbose);
        }

        public static void SaveNew(string fileName, ParticleSystem ps, ParticleSystem seqres, PDBInfo extInfo, bool verbose)
        {
            if (verbose)
            {
                Trace.WriteLine("PDB save routine called for : " + ps.Name);
                Trace.WriteLine("Saving to filename : " + ps.Name);
                Trace.Indent();
            }

			StreamWriter rw = new StreamWriter( fileName );

            if (verbose) Trace.WriteLine("Writing file header");
			rw.WriteLine( "REMARK DAVE Generated PDB-Sytle OutPut File" );
			rw.Write( "REMARK Written using Core.dll build: " );
			rw.WriteLine( CoreIni.Instance.Version.ToString() );
			DateTime dt = DateTime.Now;
			rw.WriteLine( "REMARK Date Written : " + dt.ToLongDateString() );
			rw.WriteLine( "REMARK Time Written : " + dt.ToLongTimeString() );

			if( extInfo != null )
			{
				extInfo.WritePDBResolution (rw);
				extInfo.WritePDBExptlReslnMethod(rw);
			}

            if (seqres != null)
			{
                SaveSEQRESTo(rw, seqres);
			}

            WriteSSBONDs(rw, ps);

            if (verbose) Trace.WriteLine("Writing atom lines : " + ps.Count + " atoms are present");
			SaveAtomsTo( rw, ps );

            string end = "END";
            end.PadRight(80,' ');
			rw.WriteLine(end);			

			rw.Close();

            if (verbose)
            {
                Trace.Unindent();
                Trace.WriteLine("File write complete");
                Trace.WriteLine("");
            }
        }

		private static StringBuilder s = new StringBuilder(80);
		public static string MakePDBStringFromAtom(Atom a)
		{
			s.Remove(0, s.Length);

			// typical line
			//ATOM   1076  N   LEU A 316      59.941  10.539  13.317  1.00 57.04           N  
			s.Append( "ATOM  " );
			s.Append( a.AtomNumber.ToString().PadLeft(5,' ') );
			s.Append(" ");
			s.Append( a.atomPrimitive.PDBIdentifier );
			s.Append(" ");
			string moleculeName = a.parentMolecule.moleculePrimitive.MolName;
			if ( moleculeName.Length >= 4 )
			{
				moleculeName = moleculeName.Substring(1,3);
			}
            if (0 == String.Compare(moleculeName, "CYX", true))
            {
                // Read **HACK**
                moleculeName = "CYS";
            }
			s.Append( moleculeName );
			s.Append( " " );
			s.Append( a.parentMolecule.parentChainID );
			s.Append( a.parentMolecule.ResidueNumber.ToString().PadLeft(4,' ') );
			s.Append( a.parentMolecule.InsertionCode );
			s.Append( "   " );
			s.Append( a.xFloat.ToString("0.000").PadLeft(8,' ') );
			s.Append( a.yFloat.ToString("0.000").PadLeft(8,' ') );
			s.Append( a.zFloat.ToString("0.000").PadLeft(8,' ') );
			s.Append( a.Occupancy.ToString("0.00").PadLeft(6,' ') ); // uncomment when we actually set them
			s.Append( a.TempFactor.ToString("0.00").PadLeft(6,' ') );
			//s.Append( (0.0f).ToString("0.00").PadLeft(6,' ') );
			//s.Append( (0.0f).ToString("0.00").PadLeft(6,' ') );
			s.Append( ' ', 11 );
			s.Append( a.atomPrimitive.Element );
				
			s.Append(' ',80-s.Length);
			return s.ToString();
		}
		#endregion

		#region ExtractionHelperFunctions

		private Position getPositionOnly( string PDBLine )
		{
			//ATOM    240  OE2AGLU A 217       8.998   3.088  15.687  0.50 45.76           O  
			Position p = new Position();
			try
			{
				p.x = double.Parse( PDBLine.Substring( 30, 8 ) );
			}
			catch
			{
			}
			try
			{
				p.y = double.Parse( PDBLine.Substring( 38, 8 ) );
			}
			catch
			{
			}
			try
			{
				p.z = double.Parse( PDBLine.Substring( 46, 8 ) );
			}
			catch
			{
			}
			return p;
		}

		private string getLine( StreamReader re, ref string linePassBackBuffer )
		{
			string returnLine;
			if ( linePassBackBuffer != null )
			{
				returnLine = linePassBackBuffer;
				linePassBackBuffer = null;
			}
			else
			{
				returnLine = re.ReadLine();
			}
			if( returnLine == null )
			{
				return null;
			}
			else
			{
				if (returnLine.Length < 80)
				{
					returnLine = returnLine.PadRight(80);
				}
				return returnLine;
			}
		}

		private bool shouldContinue( string wholeLine )
		{
			// dodgy terms that are sometimes found with atom records that bugger up my prog! - ass-holes ...
			// IDs that are sometimes found, can be skipped
			return ( 
				0 == String.Compare("ANISOU", 0, wholeLine, 0, 6, true ) ||
				0 == String.Compare("SIGUIJ", 0, wholeLine, 0, 6, true ) ||
				0 == String.Compare("SIGATM", 0, wholeLine, 0, 6, true ) 
				);
		}

		public bool isPolypeptideAtomLineType( string wholeLine )
		{
			// selenocysteine is annoyingly a hetatm, so we will search for HETATM too
			return ( 
					0 != String.Compare( "H0H", 0, wholeLine, 17, 3, true ) &&
                    0 != String.Compare( "SOL", 0, wholeLine, 17, 3, true) && 
				(
					0 == String.Compare("ATOM  ", 0, wholeLine, 0, 6, true ) ||
					0 == String.Compare("HETATM", 0, wholeLine, 0, 6, true ) 
				)
				);
		}

		// ALTLOCS : the reason the PDB format is a total arse :
		// How the hell do you parse below ?

		// in this case we need model ' ', A and B
		// in the next case we want models A and B only, but sharing atoms from ' '
		//ATOM    427  N   LYS A  52     -11.825  21.706   8.862  1.00 13.44           N  
		//ATOM    428  CA  LYS A  52     -11.197  22.520   7.846  1.00 13.28           C  
		//ATOM    429  C   LYS A  52      -9.780  22.081   7.531  1.00 12.02           C  
		//ATOM    430  O   LYS A  52      -8.954  21.862   8.418  1.00 13.38           O  
		//ATOM    431  CB  LYS A  52     -11.132  23.979   8.343  0.20 14.22           C  
		//ATOM    432  CG  LYS A  52     -12.503  24.602   8.566  0.20 15.79           C  
		//ATOM    433  CD  LYS A  52     -13.149  24.945   7.234  0.20 20.44           C  
		//ATOM    434  CE  LYS A  52     -14.560  25.476   7.385  0.20 24.70           C  
		//ATOM    435  NZ  LYS A  52     -15.533  24.425   7.777  0.20 30.29           N  
		//ATOM    436  CB ALYS A  52     -11.262  23.997   8.261  0.50 16.68           C  
		//ATOM    437  CG ALYS A  52     -12.707  24.471   8.486  0.50 20.24           C  
		//ATOM    438  CD ALYS A  52     -13.460  24.559   7.183  0.50 22.29           C  
		//ATOM    439  CE ALYS A  52     -14.908  24.949   7.326  0.50 22.84           C  
		//ATOM    440  NZ ALYS A  52     -15.191  26.236   7.976  0.50 20.89           N  
		//ATOM    441  CB BLYS A  52     -11.085  23.956   8.330  0.30 12.97           C  
		//ATOM    442  CG BLYS A  52     -12.495  24.658   8.513  0.30 15.29           C  
		//ATOM    443  CD BLYS A  52     -13.130  24.972   7.260  0.30 18.51           C  
		//ATOM    444  CE BLYS A  52     -14.659  25.457   7.731  0.30 22.49           C  
		//ATOM    445  NZ BLYS A  52     -15.127  26.155   6.119  0.30 19.09           N  
		//
		//ATOM    446  C   VAL A  53      -7.161  22.952   6.193  1.00 11.24           C  
		//ATOM    447  O   VAL A  53      -7.582  24.111   6.150  1.00 12.99           O  
		//ATOM    448  N  AVAL A  53      -9.452  22.058   6.248  0.60 11.31           N  
		//ATOM    449  CA AVAL A  53      -8.089  21.814   5.794  0.60 12.24           C  
		//ATOM    450  CB AVAL A  53      -8.086  21.638   4.260  0.60 14.27           C  
		//ATOM    451  CG1AVAL A  53      -6.701  21.625   3.685  0.60 13.50           C  
		//ATOM    452  CG2AVAL A  53      -8.856  20.365   3.879  0.60 15.98           C  
		//ATOM    453  N  BVAL A  53      -9.435  22.023   6.239  0.40 13.10           N  
		//ATOM    454  CA BVAL A  53      -8.050  21.743   5.872  0.40 12.48           C  
		//ATOM    455  CB BVAL A  53      -7.829  21.284   4.426  0.40 13.73           C  
		//ATOM    456  CG1BVAL A  53      -8.565  19.966   4.165  0.40 17.06           C  
		//ATOM    457  CG2BVAL A  53      -8.251  22.325   3.399  0.40 12.37           C  

		private PolyPeptide ReadPolypeptideBlock( StreamReader re, ref string caughtLine, ref string linePassBackBuffer )
		{
			// NOTE : all changes in this block should be considered for use in ReadMoleculeBlock() ...

			char firstAtomChainID = caughtLine[21]; // records the first chain ID to decide when to exit the function
			PolyPeptide p = new PolyPeptide( firstAtomChainID );

			PDBAtom cacheAtom = new PDBAtom(); // temp holder
			string inputLine;
			linePassBackBuffer = caughtLine; // just for the 1st pass
			while(true) // while we are in the PP definitions
			{
				if( null == ( inputLine = getLine( re, ref linePassBackBuffer ) ) )
				{
					break;
				}

				// CONTINUE CONDITIONS
				if( shouldContinue( inputLine ) ) continue; 

				// RETURN CONDITIONS
				char nextAtomChainID = inputLine[21];

				// we will either compare ATOM for most aa's or HETATM for selenocysteine
				if ( !isPolypeptideAtomLineType(inputLine) || (firstAtomChainID != nextAtomChainID) ) // the chainIDs dont match, pass back to start a new polypeptide
				{
					linePassBackBuffer = inputLine; // the polypeptide has ended, the next line is an unknown, pass it back
					break;
				}

				cacheAtom.setFrom( inputLine ); // we got an atom

				AminoAcid aa = new AminoAcid( cacheAtom.residueName, cacheAtom.residueNumber, cacheAtom.insertionCode );
				aa.addAtom( PDBAtomToAtom( cacheAtom, aa ) );
				p.addMolecule(aa);

				char firstAltLocInResidueOtherThanSpace = '\0'; // '/0' == null
				// ****-holes can't even keep the first altloc character standard for PDB files
				// I have found everything from A to D to E, different for different AAs in the PP ... grrrr
				// this is set when the first altloc is found in a given molecule and used thereafter
				if( cacheAtom.altLocIndicator != ' ' ) // first atom in this molecule is an altloc candidate
				{
					firstAltLocInResidueOtherThanSpace = cacheAtom.altLocIndicator;
				}

				string firstAtomResidueName = cacheAtom.residueName;
				int  firstAtomResidueNumber = cacheAtom.residueNumber;
				char firstAtomInsertionCode = cacheAtom.insertionCode;
			
				while( true ) // while we are in the AA
				{
					inputLine = re.ReadLine();
					if ( inputLine == null ) break;
					inputLine = inputLine.PadRight(80,' ');

					if( shouldContinue( inputLine ) ) continue; // test for skipable lineID's

					if ( !isPolypeptideAtomLineType( inputLine ) )
					{
						linePassBackBuffer = inputLine;
						break;
					}

					cacheAtom.setFrom( inputLine );

					if ( firstAtomResidueName   == cacheAtom.residueName   &&
						firstAtomResidueNumber == cacheAtom.residueNumber &&
						firstAtomInsertionCode == cacheAtom.insertionCode ) // if more than one Amino acid has the same residue number
					{
						if ( cacheAtom.altLocIndicator != ' ' )
						{
							// if this is not known, then set it ...
							if( firstAltLocInResidueOtherThanSpace == '\0' )
							{
								firstAltLocInResidueOtherThanSpace = cacheAtom.altLocIndicator;
							}
							if( firstAltLocInResidueOtherThanSpace == cacheAtom.altLocIndicator )
							{
								// "dont know list"

								// there are 3 possible states :
								// a) all absent : then all should be added to the amino acid as the 1st model
								// b) all present: then they should all be in a an altloc model
								// c) dirty mix  : if the atom is absent then add it as the default position. 
								//    Otherwise, add it as an altloc which will override the current default 
								//    in the model that it is added to ...

								bool hasBeenAddedIn1stModel = false;
								for( int qq = 0; qq < aa.Count; qq++ )
								{
									Atom atom = aa[qq];
									if( 0 == String.CompareOrdinal( atom.PDBType, 0, cacheAtom.atomName, 0, 4 ) )
									{
										m_AltLocStore.addPDBAtom( (PDBAtom) cacheAtom.Clone());
										hasBeenAddedIn1stModel = true;
										break;
									}
								}
								if( hasBeenAddedIn1stModel )
								{
									continue;
								}
								else
								{
									// it is not currently in the AminoAcid, so add it as the default
									// fall through to add the atom
								}
							}
							else
							{
								// add to "defo an altloc list"
								m_AltLocStore.addPDBAtom( (PDBAtom) cacheAtom.Clone());
								continue;
							}
						}
						// add the atom only if defo not an altloc
						aa.addAtom( PDBAtomToAtom( cacheAtom, aa ) );
					}
					else
					{
						linePassBackBuffer = inputLine; // the atom is part of the next model
						break;
					}
				}		
			}
			// end adding residues to polypeptides


			// the first and last AA's have to have their molPrimitives redefined to allow for the N and C termini atoms
			
            ModResCollection ModRes = ExtendedInformation.ModRes;
            int cMin1 = p.Count - 1;

            // N-terminus
            p[0].setMolPrimitive( 'N', true );
            
            // In between
            for (int i = 1; i < cMin1; i++)
			{
				p[i].setMolPrimitive(true);
			}

            // C-terminus            
            p[cMin1].setMolPrimitive('C', true);

            for (int i = 0; i <= cMin1; i++)
            {
                // If it is null, we have no ff definiton for this name, is it a modified residue?
                if (null == (p[i].moleculePrimitive as MoleculePrimitive))
                {
                    ModRes.AttemptRename(p[i]);
                }
            }

			return p;
		}

		private Molecule ReadMoleculeBlock( StreamReader re, ref string caughtLine, ref string linePassBackBuffer )
		{
			PDBAtom cacheAtom = new PDBAtom( caughtLine );
			Molecule m = new Molecule(cacheAtom.residueName, cacheAtom.residueNumber, cacheAtom.insertionCode );
			m.addAtom( PDBAtomToAtom( cacheAtom, m ) );

			char firstAltLocInResidueOtherThanSpace = '\0'; // ****-holes can't even keep the first altloc character standard for PDB files
			// I have found everything from A to D to E, different for different AAs in the PP ... grrrr
			// this is set when the first altloc is found in a given molecule and used thereafter
			if( cacheAtom.altLocIndicator != ' ' ) // first atom in this molecule is an altloc candidate
			{
				firstAltLocInResidueOtherThanSpace = cacheAtom.altLocIndicator;
			}

			char firstAtomChainID = cacheAtom.chainID; // records the first chain ID to decide when to exit the function
			string firstAtomResidueName = cacheAtom.residueName;
			int  firstAtomResidueNumber = cacheAtom.residueNumber;
			char firstAtomInsertionCode = cacheAtom.insertionCode;

			string inputLine = null;
			while( true )
			{
				if( null == ( inputLine = getLine( re, ref linePassBackBuffer ) ) )
				{
					break;
				}

				// CONTINUE CONDITIONS due to odd additional lines in this region
				if( shouldContinue( inputLine ) ) continue; 

				// if no longer a HETATM, break
				if( 0 != String.Compare("HETATM", 0, inputLine, 0, 6, true ) )
				{
					linePassBackBuffer = inputLine;
					break;
				}

				cacheAtom.setFrom( inputLine ); // we got an atom

				if ( firstAtomResidueName   == cacheAtom.residueName   &&
				 	 firstAtomResidueNumber == cacheAtom.residueNumber &&
					 firstAtomInsertionCode == cacheAtom.insertionCode )
				{
					// its still in the same molecule group, is it an altloc ?
					if ( cacheAtom.altLocIndicator != ' ' )
					{
						// if this is not known, then set it ...
						if( firstAltLocInResidueOtherThanSpace == '\0' )
						{
							firstAltLocInResidueOtherThanSpace = cacheAtom.altLocIndicator;
						}
						if( firstAltLocInResidueOtherThanSpace == cacheAtom.altLocIndicator )
						{
							// "dont know list"

							// there are 3 possible states :
							// a) all absent : then all should be added to the amino acid as the 1st model
							// b) all present: then they should all be in a an altloc model
							// c) dirty mix  : if the atom is absent then add it as the default position. 
							//    Otherwise, add it as an altloc which will override the current default 
							//    in the model that it is added to ...

							bool hasBeenAddedIn1stModel = false;
							for( int qq = 0; qq < m.Count; qq++ )
							{
								Atom atom = m[qq];
								if( 0 == String.CompareOrdinal( atom.PDBType, 0, cacheAtom.atomName, 0, 4 ) )
								{
									// it is an altloc ....
									PDBAtom altLoc = (PDBAtom)cacheAtom.Clone();
									altLoc.chainID = ' '; // HACK : this molecule will be added to the HetMolecule group
									// this group is forced to have a ' ' name as it acts as a bin for all hetmolecules
									// this muct therefore be forced here .... 
									m_AltLocStore.addPDBAtom( altLoc );
									hasBeenAddedIn1stModel = true;
									break;
								}
							}
							if( hasBeenAddedIn1stModel )
							{
								continue;
							}
							else
							{
								// it is not currently in the Molecule, so add it as the default
								// fall through to add the atom
							}
						}
						else
						{
							// it is an altloc ....
							PDBAtom altLoc = (PDBAtom)cacheAtom.Clone();
							altLoc.chainID = ' '; // HACK : this molecule will be added to the HetMolecule group
							// this group is forced to have a ' ' name as it acts as a bin for all hetmolecules
							// this muct therefore be forced here .... 
							m_AltLocStore.addPDBAtom( altLoc );
							continue;
						}
					}
					// add the atom only if defo not an altloc
					m.addAtom( PDBAtomToAtom( cacheAtom, m ) );
				}
				else
				{
					linePassBackBuffer = inputLine;
					break;
				}
			}

            m.setMolPrimitive( true );
			return m;
		}

		#endregion
        
		#region SaveHelperFunctions		
		private Atom PDBAtomToAtom(PDBAtom a, Molecule parent)
		{
			return new Atom(
				a.atomName,
				m_AtomCreationCounter++, // increment following assignment
				a.atomNumber,
				parent,
				a.position.x,
				a.position.y,
				a.position.z, 
				a.occupancy, 
				a.tempFactor );
		}

		#endregion

	}
}
