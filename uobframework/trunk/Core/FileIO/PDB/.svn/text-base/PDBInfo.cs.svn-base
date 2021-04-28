using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

using UoB.Core.Tools;
using UoB.Core.Structure;
using UoB.Core.Structure.Primitives;
using UoB.Core.FileIO;

namespace UoB.Core.FileIO.PDB
{
    // The header of an official PDB file conatins the PDB_id of the PDB database. Did the file we just read in
    // have one of these and did it match the filename of the file we just improted. It can be handy to confirm this.
    public enum HeaderStatus
    {
        Undefined,
        NotFilenameMatch,
        Error,
        Valid
    }

    // Enumeration of different types of "SeqAdv" lines
    public enum SeqAdvType
    {        
        // Glycosylation site
        ModifiedResidue, // Post-translational modification
        // Designed chemical modification
        // Phosphorylation site
        // Blocked N-terminus
        // Aminated C-terminus
        // D-configuration
        // Reduced peptide bond
        Unknown
    }

    public class SeqAdv
    {
        // COLUMNS        DATA TYPE       FIELD          DEFINITION
        // ----------------------------------------------------------------------------------
        //  1 -  6        Record name     "SEQADV"
        //  8 - 11        IDcode          idCode         ID code of this entry.
        // 13 - 15        Residue name    resName        Name of the PDB residue in conflict.
        // 17             Character       chainID        PDB chain identifier.
        // 19 - 22        Integer         seqNum         PDB sequence number.
        // 23             AChar           iCode          PDB insertion code.
        // 25 - 28        LString         database       Sequence database name.
        // 30 - 38        LString         dbIdCode       Sequence database accession number.
        // 40 - 42        Residue name    dbRes          Sequence database residue name.
        // 44 - 48        Integer         dbSeq          Sequence database sequence number.
        // 50 - 70        LString         conflict       Conflict comment.

        public string originalLine;

        public string resName;
        public char chainID;
        public int resNum;
        public char iCode;
        public string seqDBName;
        public string seqAccession;
        public string seqDBResName;
        public int seqDBNum;
        public string comment;
        public SeqAdvType type;

        public bool ParseLine(string line)
        {
            originalLine = line;

            // Make life easier and faster by caching the line in a mutable stringbuilder
            StringBuilder sb = new StringBuilder(line);
            if (sb.Length < 80) sb.Append(' ', 80 - sb.Length);

            // Assign our properties
            resName = sb.ToString(12, 3);
            chainID = sb[16];
            try { resNum = int.Parse(sb.ToString(19, 4)); }
            catch { return false; }
            iCode = sb[22];
            seqDBResName = sb.ToString(24, 4);
            seqAccession = sb.ToString(29, 9);
            seqDBName = sb.ToString(39, 3);
            try { seqDBNum = int.Parse(sb.ToString(43, 4)); }
            catch { seqDBNum = -1; }
            comment = sb.ToString(49, 21);
            comment = comment.Trim();

            // Try to parse a 'type'
            ParseType();

            return true;
        }

        protected void ParseType()
        {
            if (0 == String.Compare("MODIFIED RESIDUE", 0, comment, 0, 16, true))
            {
                type = SeqAdvType.ModifiedResidue;
            }
            else
            {
                type = SeqAdvType.Unknown;
            }
        }
    }

    // The "MODRES" pdb file line type
    public class ModResLine
    {
        // COLUMNS        DATA TYPE       FIELD          DEFINITION
        // --------------------------------------------------------------------------------
        //  1 -  6        Record name     "MODRES"
        //  8 - 11        IDcode          idCode         ID code of this entry.
        // 13 - 15        Residue name    resName        Residue name used in this entry.
        // 17             Character       chainID        Chain identifier.
        // 19 - 22        Integer         seqNum         Sequence number.
        // 23             AChar           iCode          Insertion code.
        // 25 - 27        Residue name    stdRes         Standard residue name.
        // 30 - 70        String          comment        Description of the residue
        //                                               modification.

        public string originalLine;

        public string resName;
        public char chainID;
        public int resNum;
        public char iCode;
        public string stdResName;
        public string chemicalName;

        public bool ParseLine( string line )
        {
            originalLine = line;

            // Make life easier and faster by caching the line in a mutable stringbuilder
            StringBuilder sb = new StringBuilder(line);
            if( sb.Length < 80 ) sb.Append(' ', 80 - sb.Length);

            // Assign our properties
            resName = sb.ToString(12, 3);
            chainID = sb[16];
            try { resNum = int.Parse(sb.ToString(18, 4)); }
            catch { return false; }
            iCode = sb[22];
            stdResName = sb.ToString(24, 3);
            if (0 == String.CompareOrdinal(stdResName, "   "))
                return false; // The bitch is blank - it tells us nothing!
            chemicalName = sb.ToString(29, 46);
            chemicalName = chemicalName.Trim();

            //SupplyLibrary("c:\\Modres_cache.txt", resName, stdResName, chemicalName);

            return true;
        }

        // Can be called above if we want to make a MODRES library for PD...
        private void SupplyLibrary(string filename,string alias, string libname, string chemicalName)
        {
            // Assert that it isnt == to itself!
            alias = alias.ToUpper();
            libname = libname.ToUpper();
            if (alias.CompareTo(libname) == 0) return;

            // Import our current library
            List<string> aliases = new List<string>();
            List<string> names = new List<string>();
            List<string> chems = new List<string>();

            StringBuilder cat = new StringBuilder();
            if (File.Exists(filename) )
            {
                StreamReader re = new StreamReader(filename);
                string line;
                while (null != (line = re.ReadLine()))
                {
                    string[] lineparts = CommonTools.WhiteSpaceRegex.Split(line);
                    if( lineparts.Length <= 3 ) continue;
                    if (lineparts[0].CompareTo("ALIAS") != 0) continue;
                    aliases.Add(lineparts[1]);
                    names.Add(lineparts[2]);
                    cat.Remove(0, cat.Length);
                    for (int q = 3; q < lineparts.Length; q++)
                    {
                        if( q != 3 ) cat.Append(' ');
                        cat.Append(lineparts[q]);
                    }
                    if( cat[0] == '#' ) 
                        cat.Remove(0,1);
                    chems.Add(cat.ToString().Trim());
                }
                re.Close();
            }        

            aliases.Add(alias);
            names.Add(libname);
            chems.Add(chemicalName);

            // Remove identicals
            for (int i = aliases.Count - 1; i >= 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    if ((aliases[i].CompareTo(aliases[j]) == 0) &&
                        (names[i].CompareTo(names[j]) == 0))
                    {
                        int rem; // ensure we keep names (by '>' I mean non-zero length chemical names)
                        if (chems[i].Length > chems[j].Length)
                        {
                            rem = j;
                        }
                        else
                        {
                            rem = i;
                        }
                        aliases.RemoveAt(rem);
                        names.RemoveAt(rem);
                        chems.RemoveAt(rem);
                    }
                }
            }

            StreamWriter rw = new StreamWriter("c:\\Modres_cache.txt");
            for (int i = 0; i < aliases.Count; i++)
            {
                rw.Write("ALIAS ");
                rw.Write(aliases[i]);
                rw.Write(' '); 
                rw.Write(names[i]);
                rw.Write(" # ");
                rw.WriteLine(chems[i]);
            }
            rw.Close();
        }

        public bool Matches(Molecule mol)
        {
            return ((mol.parentChainID == chainID) &&
                (mol.InsertionCode == iCode) &&
                (mol.ResidueNumber == resNum));

        }
    }

    public class ModResCollection
    {
        public ModResCollection()
        {
            m_Lines = new List<ModResLine>();
        }

        public void ParseLine(string line)
        {
            ModResLine modres = new ModResLine();
            if (modres.ParseLine(line))
            {
                m_Lines.Add(modres);
            }
        }

        public void AttemptRename( Molecule mol )
        {
            if (m_Lines.Count == 0) return;

            string originalName = mol.Name_NoPrefix;

            int matchAt = -1;

            for( int i = 0; i < m_Lines.Count; i++ )
            {
                if (m_Lines[i].Matches(mol))
                {
                    mol.ResetName(m_Lines[i].stdResName, false); // try to find a primitive now...
                    if (null != (mol.moleculePrimitive as MoleculePrimitive))
                    {
                        if (matchAt != -1)
                        {
                            // Duplicate match - no good!! 
                            // This is probably a multi-residue alias!!
                            // It could also be an error (i have seen at least one)
                            matchAt = -1; // flag as crap.
                            break;
                        }
                        matchAt = i;
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            if (matchAt != -1) // if we found one, have a go...
            {
                mol.ResetName(m_Lines[matchAt].stdResName, true); // this time 'true' to set the primitives
                char iCode = m_Lines[matchAt].iCode;
                if (iCode == ' ') iCode = '_';
                Trace.WriteLine(@"Successful Name-Munge from MODRES data: Converted Chain:'" +
                    m_Lines[matchAt].chainID + "'s '" + originalName + ':' + m_Lines[matchAt].resNum + ':'
                    + iCode + "' to a '" + m_Lines[matchAt].stdResName + "'");
                return; // Cool, we now have a valid molecule in the eyes of our forcefield!
            }
            else
            {
                // We haven't improved the situation, revert!
                mol.ResetName(originalName, true);
            }
        }

        private List<ModResLine> m_Lines;
    }

	/// <summary>
	/// Summary description for PDBInfo.
	/// </summary>
	public sealed class PDBInfo : ExtendedInfo
	{
		// Extend for extra Information later
		private string m_Name;
		private const float nullResolution = 9999.9f;
		private float m_Resolution = nullResolution;
		private int m_NumberOfPeptides = 0;         // the number actually defined by atom records
		private int m_NumberOfDisulphides = -1;		// "SSBOND" statements
		private int m_HetCount = 0;					// number of hetmolecules defined by a "HET   " line
		private int m_AtomCount = 0;				// "ATOM  "
		private int m_HetatomCount = 0;				// "HETATM", but not when HOH is the molID
		private int m_ExplicitWaterCount = 0;		// "HETATM" when HOH
		private int m_ModelCount = 0;
		private PDBExpRslnMethod m_ResolutionMethod = PDBExpRslnMethod.Undefined;
        private ModResCollection m_ModRes = new ModResCollection();
		private StringBuilder m_TitleBlock = new StringBuilder(); // store all important header info
		private StringBuilder m_StringBuilder = new StringBuilder();
        private string m_HeaderID = "";
        private HeaderStatus m_HeaderStatus = HeaderStatus.Undefined;

		public override string ToString()
		{
			m_StringBuilder.Remove(0,m_StringBuilder.Length);

			m_StringBuilder.Append( "PDB File : " );
			m_StringBuilder.Append(	m_Name );
			m_StringBuilder.Append( "\r\n\r\n" );
			
			m_StringBuilder.Append( "Atom Count : " );
			m_StringBuilder.Append(	m_AtomCount );
			m_StringBuilder.Append( "\r\n" );

			m_StringBuilder.Append( "Amino Acids : (CODE NOT IMPLENENTED)" );
			//m_StringBuilder.Append(	m_Info.AADefString );
			m_StringBuilder.Append( "\r\n" );

			m_StringBuilder.Append( "PolyPeptide Count : " );
			m_StringBuilder.Append(	StringPolyPeptide );
			m_StringBuilder.Append( "\r\n" );

			return m_StringBuilder.ToString();
		}

		public void WritePDBResolution( StreamWriter rw )
		{
			//REMARK   2 RESOLUTION. 1.6  ANGSTROMS.
			if( m_Resolution != nullResolution )
			{
				rw.WriteLine("REMARK   2 RESOLUTION. {0,-4:N} ANGSTROMS.", Resolution );
			}
		}

		public void WritePDBExptlReslnMethod( StreamWriter rw )
		{
			switch( m_ResolutionMethod )
			{
				case PDBExpRslnMethod.Crystalographic:
					rw.WriteLine( "EXPDTA    X-RAY DIFFRACTION" );
					break;
				case PDBExpRslnMethod.ElectronMicroscopy:
					rw.WriteLine( "EXPDTA    ELECTRON MICROSCOPY" );
					break;
				case PDBExpRslnMethod.FiberDiffraction:
					rw.WriteLine( "EXPDTA    FIBER DIFFRACTION" );
					break;
				case PDBExpRslnMethod.InfraredMicroscopy:
					rw.WriteLine( "EXPDTA    INFRARED SPECTROSCOPY" );
					break;
				case PDBExpRslnMethod.NMR:
					rw.WriteLine( "EXPDTA    NMR" );
					break;
				case PDBExpRslnMethod.NMRAveraged:
					rw.WriteLine( "EXPDTA    NMR, AVERAGE STRUCTURE" );
					break;
				case PDBExpRslnMethod.NMRMinimized:
					rw.WriteLine( "EXPDTA    NMR, MINIMIZED STRUCTURE" );
					break;
				case PDBExpRslnMethod.NMRRegularizedMean:
					rw.WriteLine( "EXPDTA    NMR, REGULARIZED MEAN STRUCTURE" );
					break;
				case PDBExpRslnMethod.NMRRepresentative:
					rw.WriteLine( "EXPDTA    NMR, REPRESENTATIVE STRUCTURE" );
					break;
				case PDBExpRslnMethod.NMRRestrainedRegularizedMean:
					rw.WriteLine( "EXPDTA    NMR, RESTRAINED REGULARIZED MEAN STRUCTURE" );
					break;
				case PDBExpRslnMethod.NMRTheoretical:
					rw.WriteLine( "EXPDTA    NMR; THEORETICAL MODEL" );
					break;
				case PDBExpRslnMethod.SingleCrystalElectronMicroscopy:
					rw.WriteLine( "EXPDTA    SINGLE-CRYSTAL ELECTRON DIFFRACTION" );
					break;
				case PDBExpRslnMethod.DAVEUnknown:
					break;
				case PDBExpRslnMethod.Undefined:
					rw.WriteLine( "EXPDTA    Undefined: No \"EXPDTA\" Tag In Original PDB" );
					break;
				default:
					break;
			}
		}

		public PDBInfo( string fileName, string name, PDBSequence seq )
		{
			m_Name = name;

			char prevPPID = '\0';
			ArrayList AANumberList = new ArrayList(); // used to hold the number of AAs in each PP

			int modelStatementCount = 0;
			int endModelStatementCount = 0;

			StreamReader re = null;

			try 
			{
				re = File.OpenText(fileName);
			}
			catch(Exception e)
			{
				System.Diagnostics.Trace.WriteLine(@"File Not Available \ Cannot Open : " + fileName  + " Because : " + e.ToString());
				return;
			}

			try
			{
				string inputLine = null;
				while ((inputLine = re.ReadLine()) != null)
				{
					///<summary>
					///Now we disect the lines to find their type and extract all information
					///</summary>
					///

					if (inputLine.Length < 80)
					{
						inputLine = inputLine.PadRight(80);
					}

					string lineType = inputLine.Substring(0,6).ToUpper();

					switch ( lineType )
					{
						case "ATOM  ":
							// we dont explicitly count polypeptides, this done via the seqres statements - dodgyness !
							m_AtomCount++;
							HETATMTOATOMJUMP:
								// record chainID count, HETATMS in the form of selenocysteine are still valid in this context
								if ( prevPPID != inputLine[21] )
								{ 
									prevPPID = inputLine[21];
									m_NumberOfPeptides++;
								}
							break;
						case "HETATM":
							string molID = inputLine.Substring(17,3); // test the molType
							if ( molID == "HOH" )
							{
								if( inputLine.Substring(13,1)[0] == 'O' ) // for counting water, only count O, and not the hydrogens
								{
									m_ExplicitWaterCount++;
								}
							}
							else
							{
								m_HetatomCount++;
								if ( molID == "CSE" )
								{
									goto HETATMTOATOMJUMP; // record polyP count, Selenocysteine is a hetatm
								}
							}
							break;
						case "HET   ":
							m_HetCount++;                                                        
							break;
						case "REMARK": // at the moment just using this to catch the resolution
							try 
							{
                                if ((0 == String.Compare(inputLine,9,"2 RESOLUTION",0,12)))
								{
                                    string resolution;
                                    try
                                    {
                                        resolution = inputLine.Substring(22, 5);
                                        m_Resolution = float.Parse(resolution);
                                    }
                                    catch
                                    {
                                        // slightly dodgy hack here
                                        //there are REMARK 2 lines that state other irrelevant stuff and blank lines
                                    }
								}
								else
								{
									break;
								}
							}
							catch(Exception e)
							{
								System.Diagnostics.Debug.WriteLine(e.ToString());
							}
							break;
						case "SEQRES":
							seq.ParseInputLine( inputLine );
							break;
						case "MODEL ":
							modelStatementCount++;
							break;
						case "ENDMDL":
							endModelStatementCount++;
							break;
						case "SSBOND":
							if ( m_NumberOfDisulphides == -1 )
							{ 
								m_NumberOfDisulphides = 0;
							}
							m_NumberOfDisulphides++;
							break;
							// now all the header stuff
						case "HEADER":
							m_TitleBlock.Append( inputLine.ToCharArray() );
							m_TitleBlock.Append( '\r' );
							m_TitleBlock.Append( '\n' );
                            if (m_HeaderStatus != HeaderStatus.Undefined)
                            {
                                m_HeaderStatus = HeaderStatus.Error; // Multiple header definitions!!!
                            }
                            else
                            {
                                //HEADER    COMPLEX (TRANSDUCER/TRANSDUCTION)       05-DEC-97   1A0R 
                                FileInfo fi = new FileInfo(fileName);
                                m_HeaderID = inputLine.Substring(62, 4);
                                if(0 == m_HeaderID.CompareTo( Path.GetFileNameWithoutExtension(fi.Name) ))
                                {
                                    m_HeaderStatus = HeaderStatus.Valid;
                                }
                                else
                                {
                                    m_HeaderStatus = HeaderStatus.NotFilenameMatch;
                                }
                            }                    
							break;
						case "TITLE ":
							m_TitleBlock.Append( inputLine.ToCharArray() );
							m_TitleBlock.Append( '\r' );
							m_TitleBlock.Append( '\n' );
							break;
						case "COMPND":
							m_TitleBlock.Append( inputLine.ToCharArray() );
							m_TitleBlock.Append( '\r' );
							m_TitleBlock.Append( '\n' );
							break;
						case "SOURCE":
							m_TitleBlock.Append( inputLine.ToCharArray() );
							m_TitleBlock.Append( '\r' );
							m_TitleBlock.Append( '\n' );
							break;
						case "KEYWDS":
							m_TitleBlock.Append( inputLine.ToCharArray() );
							m_TitleBlock.Append( '\r' );
							m_TitleBlock.Append( '\n' );
							break;
                        case "MODRES":
                            try
                            {
                                m_ModRes.ParseLine(inputLine);
                            }
                            catch (Exception e)
                            {
                                System.Diagnostics.Debug.WriteLine(e.ToString());
                            }
                            break;
						case "EXPDTA":
							// extract what it is
							//EXPDTA    INFRARED SPECTROSCOPY
							//EXPDTA    SINGLE-CRYSTAL ELECTRON DIFFRACTION
							//EXPDTA    ELECTRON MICROSCOPY
							//EXPDTA    FIBER DIFFRACTION
							//EXPDTA     X-RAY DIFFRACTION    - oh my god how stupid are these people ... 
							//EXPDTA    X-RAY DIFFRACTION
							//EXPDTA    NMR
							//EXPDTA    NMR; THEORETICAL MODEL                                                
							//EXPDTA    NMR, 30 MODELS
							//EXPDTA    NMR, REPRESENTATIVE STRUCTURE                                         
							//EXPDTA    NMR, RESTRAINED REGULARIZED MEAN STRUCTURE
							//EXPDTA    NMR, REGULARIZED MEAN STRUCTURE
							//EXPDTA    NMR, MINIMIZED STRUCTURE
							//EXPDTA    NMR, MINIMIZED AVERAGE STRUCTURE 
							//EXPDTA    NMR, AVERAGE STRUCTURE
							//{
								//EXPDTA   NMR, 26 STRUCTURES         - oh my god how stupid are these people ...                                             
								//EXPDTA    NMR, 25 STRUCTURE
								//EXPDTA    NMR, 20 STRUCTURES
								//EXPDTA    NMR, 15 STRUCTURES
								//EXPDTA    NMR, 10 STRUCTURES
								//EXPDTA    NMR, 5 STRUCTURES
								//EXPDTA    NMR, 1 STRUCTURE
							//}
							//EXPDTA    SYNCHROTRON X-RAY DIFFRACTION

							m_ResolutionMethod = PDBExpRslnMethod.Undefined;

							if( String.Compare( inputLine,10,"X-RAY DIFFRACTION",0,17,true ) == 0 )
							{
								m_ResolutionMethod = PDBExpRslnMethod.Crystalographic;
							}
							else if( String.Compare( inputLine,11,"X-RAY DIFFRACTION",0,17,true ) == 0 ) // grrrrr
							{
								m_ResolutionMethod = PDBExpRslnMethod.Crystalographic; 
							}
							else if( String.Compare( inputLine,10,"SYNCHROTRON X-RAY DIFFRACTION",0,29,true ) == 0 )
							{
								m_ResolutionMethod = PDBExpRslnMethod.Crystalographic;
							}
							else if( String.Compare( inputLine,10,"FIBER DIFFRACTION",0,17,true ) == 0 )
							{
								m_ResolutionMethod = PDBExpRslnMethod.FiberDiffraction;
							}
							else if( String.Compare( inputLine,10,"ELECTRON MICROSCOPY",0,19,true ) == 0 )
							{
								m_ResolutionMethod = PDBExpRslnMethod.ElectronMicroscopy;
							}
							else if( String.Compare( inputLine,10,"INFRARED SPECTROSCOPY",0,21,true ) == 0 )
							{
								m_ResolutionMethod = PDBExpRslnMethod.InfraredMicroscopy;
							}
							else if( String.Compare( inputLine,10,"SINGLE-CRYSTAL ELECTRON DIFFRACTION",0,35,true ) == 0 )
							{
								m_ResolutionMethod = PDBExpRslnMethod.SingleCrystalElectronMicroscopy;
							}
							else if ( String.Compare( inputLine,10,"NMR",0,3,true ) == 0 
								||    String.Compare( inputLine, 9,"NMR",0,3,true ) == 0 // stupid arse format!
								)
							{
								if( inputLine[13] == ' ' )
								{
									m_ResolutionMethod = PDBExpRslnMethod.NMR;
								}
								else if( String.Compare( inputLine,15,"MINIMIZED AVERAGE STRUCTURE",0,27,true ) == 0 ) 
								{
									m_ResolutionMethod = PDBExpRslnMethod.NMRAveraged;
								}
								else if( String.Compare( inputLine,15,"REGULARIZED MEAN STRUCTURE",0,26,true ) == 0 ) 
								{
									m_ResolutionMethod = PDBExpRslnMethod.NMRRegularizedMean;
								}
								else if( String.Compare( inputLine,15,"THEORETICAL MODEL",0,17,true ) == 0 ) 
								{
									m_ResolutionMethod = PDBExpRslnMethod.NMRTheoretical;
								}
								else if( String.Compare( inputLine,15,"REPRESENTATIVE STRUCTURE",0,24,true ) == 0 ) 
								{
									m_ResolutionMethod = PDBExpRslnMethod.NMRRepresentative;
								}
								else if( String.Compare( inputLine,15,"RESTRAINED REGULARIZED MEAN STRUCTURE",0,37,true ) == 0 ) 
								{
									m_ResolutionMethod = PDBExpRslnMethod.NMRRestrainedRegularizedMean;
								}
								else if( String.Compare( inputLine,15,"MINIMIZED STRUCTURE",0,19,true ) == 0 )
								{
									m_ResolutionMethod = PDBExpRslnMethod.NMRMinimized;
								}
								else if( String.Compare( inputLine,15,"AVERAGE STRUCTURE",0,17,true ) == 0 )
								{
									m_ResolutionMethod = PDBExpRslnMethod.NMRAveraged;
								}
								else if( String.Compare( inputLine,17,"STRUCTURE",0,9,true ) == 0 )
								{
									m_ResolutionMethod = PDBExpRslnMethod.NMR;
								}
								else if( String.Compare( inputLine,18,"STRUCTURE",0,9,true ) == 0 )
								{
									m_ResolutionMethod = PDBExpRslnMethod.NMR;
								}
								else if( String.Compare( inputLine,18,"MODELS",0,6,true ) == 0 )
								{
									m_ResolutionMethod = PDBExpRslnMethod.NMR;
								}
								else
								{
									//Debug.WriteLine("A DAVE Unknown NMR reference was found in the PDB file, please tell jon ...");
									m_ResolutionMethod = PDBExpRslnMethod.NMR;
								}
							}
							else
							{
                                //Debug.WriteLine("A DAVE Unknown EXPDTA reference was found in the PDB file, please tell jon ...");
								m_ResolutionMethod = PDBExpRslnMethod.DAVEUnknown;
							}
                            
							m_TitleBlock.Append( inputLine.ToCharArray() );
							m_TitleBlock.Append( '\r' );
							m_TitleBlock.Append( '\n' );
							break;
						case "AUTHOR":
							m_TitleBlock.Append( inputLine.ToCharArray() );
							m_TitleBlock.Append( '\r' );
							m_TitleBlock.Append( '\n' );
							break;
						case "REVDAT":
							m_TitleBlock.Append( inputLine.ToCharArray() );
							m_TitleBlock.Append( '\r' );
							m_TitleBlock.Append( '\n' );
							break;
						case "JRNL  ":
							m_TitleBlock.Append( inputLine.ToCharArray() );
							m_TitleBlock.Append( '\r' );
							m_TitleBlock.Append( '\n' );
							break;
							// end header stuff
						default :
							break;

					}
				}

				if ( modelStatementCount != endModelStatementCount )
				{
					Trace.WriteLine("PDBFile Error : the number of Model Statements and EndModel Statements do not match !");
				}

				m_ModelCount = modelStatementCount;
			}
			catch(Exception e)
			{
				System.Diagnostics.Trace.WriteLine(@"Failure whilst parsing the extended PDB information. Because : " + e.ToString());
				return;
			}
			finally
			{
				re.Close();
			}
		}

		public int NumberOfPolyPeptides
		{
			get
			{
				return m_NumberOfPeptides;
			}
		}

		public string StringDisulphides
		{
			get
			{
				if ( m_NumberOfDisulphides > 0 )
					return m_NumberOfDisulphides.ToString();
				else
				{
					return "0";
				}
			}
		}

		public float Resolution 
		{
			get
			{
				return m_Resolution;
			}
		}

		public string StringResolution
		{
			get
			{
				if ( m_Resolution != 9999.9f )
				{
					return m_Resolution.ToString();
				}
				else
				{
					return "Not Defined";
				}
			}
		}

		public string StringPolyPeptide
		{
			get
			{
				return m_NumberOfPeptides.ToString();
			}
		}
		public string HeaderInfo
		{
			get
			{
				return m_TitleBlock.ToString();
			}
		}
        public string HeaderID
        {
            get
            {
                return m_HeaderID;
            }
        }
        public HeaderStatus HeaderStatus
        {
            get
            {
                return m_HeaderStatus;
            }
        }
		public PDBExpRslnMethod ExptlReslnMethod
		{
			get
			{
				return m_ResolutionMethod;
			}
		}

        public ModResCollection ModRes
        {
            get
            {
                return m_ModRes;
            }
        }
	}
}

