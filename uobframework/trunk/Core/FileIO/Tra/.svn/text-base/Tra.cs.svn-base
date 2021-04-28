using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Diagnostics;

using UoB.Core.Sequence;
using UoB.Core.Primitives;
using UoB.Core.Primitives.Collections;
using UoB.Core.Data;
using UoB.Core.ForceField;
using UoB.Core.ForceField.Definitions;
using UoB.Core.Structure;
using UoB.Core.FileIO;

namespace UoB.Core.FileIO.Tra
{
	/// <summary>
	/// Summary description for Tra.
	/// </summary>
	public sealed class Tra : BaseFileType_Structural, IDataProvider
	{
		// the bitFlags for the different tra contents types
		private const int TRAJECTORY_TYPE_NONE        =  0x0000;
		private const int TRAJECTORY_TYPE_ATOMPOS     =  0x0001;   // atom positions
		private const int TRAJECTORY_TYPE_PHIPSI      =  0x0002;   // phi/psis
		private const int TRAJECTORY_TYPE_ROTAMERS    =  0x0004;   // aa rotamers
		private const int TRAJECTORY_TYPE_ENERGY      =  0x0008;   // total energy
		private const int TRAJECTORY_TYPE_FORCEVECTOR =  0x0010;   // 10 is the next value in HEX ... (not 16)

		// assigned temporarily, but used by multiple functions
		private FileStream m_fs;
		private BinaryReader m_BinaryReader;

		private PositionStore m_ForceVectors = null;
		private Line3DStore m_ExtdVectors = null;
		private StringStore m_ExtdComments = null;
		private TraHeader m_Header;
		private DataManager m_DataManager;
		private TraPreview m_TraPreview;
		private ArrayList m_TEEntries;

		public Tra( string fullFileName ) : base( fullFileName, true )
		{
			m_DataManager = new DataManager( m_PS.Name, m_Positions );
			m_TEEntries = new ArrayList();
			m_CanSave = true;
		}

		#region overriddenfunctions
		public static void SaveNew( ParticleSystem ps, char chainID, TraSaveInfo saveInfo )
		{
			FileStream fs = new FileStream( saveInfo.FileName, FileMode.Create, FileAccess.Write );
			BinaryWriter rw = new BinaryWriter(fs);	
			PSMolContainer psm = ps.MemberWithID( chainID );

			if( psm == null )
			{
				throw new TraException("There is no polymer with that chainID");				
			}

			if( !( psm is PolyPeptide ) )
			{
				throw new TraException("The chainID stated did not refer to a polypeptide");
			}

			if( psm.Count == 0 )
			{
				throw new TraException("The polymer contains no molecules");
			}

			TraContents contentTypes = saveInfo.contentTypes;

			bool traEntriesIsSet = ( ( contentTypes & TraContents.EnergyInfo ) == TraContents.EnergyInfo );
			bool energiesIsSet = ( ( contentTypes & TraContents.EnergyInfo ) == TraContents.EnergyInfo );
			
			if ( energiesIsSet && !traEntriesIsSet )
			{
				throw new TraException("Parameter Exception : Energies can only be written if TraEntries are to be outputted");
			}

			int[] torsionDefs = new int[0];
			int paddingBytes = 0;

			if( ( contentTypes & TraContents.Impropers ) == TraContents.Impropers )
			{
				torsionDefs = GetImpropers( rw, psm );
				paddingBytes += 12 + ( 4 * torsionDefs.Length ); // 8 bytes for IMPROPER, 4 bytes per int and the torsion count which is a single int
			}
			if( ( contentTypes & TraContents.REBUILDP ) == TraContents.REBUILDP )
			{
				paddingBytes += 16400; // "REBUILDP" + 4 + 4 + 16384
			}

			WriteHeader( rw, contentTypes, saveInfo, psm, paddingBytes, false );
			WriteSystemDefinition( rw, psm ); // compulsory

			if( ( contentTypes & TraContents.Impropers ) == TraContents.Impropers )
			{
				int torsionCount = torsionDefs.Length / 4;
				rw.Write( "IMPROPER".ToCharArray() );
				rw.Write( torsionCount );
				for ( int i = 0; i < torsionDefs.Length; i++ )
				{
					rw.Write( torsionDefs[i] );
				}
			}

			if( ( contentTypes & TraContents.REBUILDP ) == TraContents.REBUILDP )
			{
				rw.Write( "REBUILDP".PadRight(16400,'\0').ToCharArray() ); 
				// padding for PD's dynamics.exe interaction, used in builder operations, but DAVE does that ;-)
			}

			rw.Write( "TRASTART".ToCharArray() );
			rw.Write( "TRAE".ToCharArray() );
			for( int i = 0; i < ps.Count; i++ )
			{
				rw.Write( ps[i].xFloat );
				rw.Write( ps[i].yFloat );
				rw.Write( ps[i].zFloat );
			}

			rw.Close();
		}

		public override void Save( string fileName )
		{
			TraSaveInfo saveInfo = new TraSaveInfo( fileName );
			Save( saveInfo );            
		}

		public override void Save( SaveParams saveParams )
		{
			TraSaveInfo saveInfo;

			if( saveParams is TraSaveInfo )
			{
				saveInfo = (TraSaveInfo) saveParams;
			}
			else
			{
				saveInfo = new TraSaveInfo( saveParams.FileName );
				DateTime date = DateTime.Now;
				saveInfo.Descriptor = "Dave tra generation from : " + m_InternalName + "\r\nOccured on : " + date.ToShortDateString() + " at " + date.ToShortTimeString();
				saveInfo.endIndex = PositionDefinitions.Count; // this needs to be set, start and stepping default to 0 and 1 respectively
				saveInfo.setPropertyBlock( m_Header ); // sets the titles for the custom properties
			}

			FileStream fs = new FileStream( saveParams.FileName, FileMode.Create, FileAccess.Write );
			BinaryWriter rw = new BinaryWriter(fs);	

			PSMolContainer psm = m_PS.Members[0];

			if( psm == null )
			{
				throw new TraException("There is no polymer with that chainID");				
			}

			if( !( psm is PolyPeptide ) )
			{
				throw new TraException("The chainID stated did not refer to a polypeptide");
			}

			if( psm.Count == 0 )
			{
				throw new TraException("The polymer contains no molecules");
			}

			
			bool traEntriesIsSet = ( ( saveInfo.contentTypes & TraContents.EnergyInfo ) == TraContents.EnergyInfo );
			bool energiesIsSet = ( ( saveInfo.contentTypes & TraContents.EnergyInfo ) == TraContents.EnergyInfo );
			
			if ( energiesIsSet && !traEntriesIsSet )
			{
				throw new TraException("Parameter Exception : Energies can only be written if TraEntries are to be outputted");
			}

			saveInfo.setPropertyBlock( m_Header ); // sets the titles for the custom properties

			int[] torsionDefs = new int[0];
			int paddingBytes = 0;

			if( ( saveInfo.contentTypes & TraContents.Impropers ) == TraContents.Impropers )
			{
				torsionDefs = GetImpropers( rw, psm );
				paddingBytes += 4 + ( 4 * torsionDefs.Length ); // 4 bytes per int
			}
			if( ( saveInfo.contentTypes & TraContents.REBUILDP ) == TraContents.REBUILDP )
			{
				paddingBytes += 16384;
			}

			WriteHeader( rw, saveInfo.contentTypes, saveInfo, psm, paddingBytes, energiesIsSet );

			int preSavePosaition = PositionDefinitions.Position;
			PositionDefinitions.Position = 0; // we want to save the target structure in the write operation

			WriteSystemDefinition( rw, psm ); // compulsory

			if( ( saveInfo.contentTypes & TraContents.Impropers ) == TraContents.Impropers )
			{
				int torsionCount = torsionDefs.Length / 4;
				rw.Write( torsionCount );
				for ( int i = 0; i < torsionDefs.Length; i++ )
				{
					rw.Write( torsionDefs[i] );
				}
			}

			if( ( saveInfo.contentTypes & TraContents.REBUILDP ) == TraContents.REBUILDP )
			{
				rw.Write( "".PadRight(16384,'\0').ToCharArray() ); 
				// padding for pd.exe interaction, used in builder operations, but dave does that too ;-)
			}

			PositionDefinitions.Position = preSavePosaition; // restore the position

			rw.Write( "TRASTART".ToCharArray() );

			if( !traEntriesIsSet )
			{
				// the first entry must still be outputted
				// this will be set to the current one in the particle system
				rw.Write( "TRAE".ToCharArray() );
				for( int i = 0; i < m_PS.Count; i++ )
				{
					rw.Write( m_PS[i].xFloat );
					rw.Write( m_PS[i].yFloat );
					rw.Write( m_PS[i].zFloat );
				}
			}
			else
			{
				// we now need to write the selection of tra entries
				PS_PositionStore t = PositionDefinitions;
				DataListing[] dl = m_DataManager.DataListings;

				if(energiesIsSet)
				{
					// check to see that the data is valid
					for( int i = 0; i < dl.Length; i++ )
					{
						if( t.Count != dl[i].Data.Length + 1 ) // +1 as there is no energy information for the first conformation, i.e. the target structure
						{
							throw new TraException("A datalisting within the datamanager assiciated with the PS_PositionStore does not contain the correct number of entries compared to the PS_PositionStore length");
						}
					}
				}

				if( saveInfo.startIndex == 0 )
				{
					saveInfo.startIndex++; // the 0th entry is the target structure and has already been written and has no associated energy
				}

				for( int i = saveInfo.startIndex; i <= saveInfo.endIndex; i += saveInfo.indexStepping )
				{
					rw.Write( "TRAE".ToCharArray() );

					for( int j = 0; j < t[i].Length; j++ )
					{
						rw.Write( t[i][j].xFloat );
						rw.Write( t[i][j].yFloat );
						rw.Write( t[i][j].zFloat );
					}

					if(energiesIsSet)
					{
						for( int j = 0; j < dl.Length; j++ )
						{
							rw.Write( dl[j].Data[i-1] ); // output the data values for each property
						}
						for( int j = dl.Length; j < 64; j++ )
						{
							rw.Write( 0.0f ); // fill in the blanks
						}
					}						
				}
			}					

			rw.Close();			
		}

		protected override void ExtractExtendedInfo()
		{
			m_fs = new FileStream( m_FileInfo.FullName, FileMode.Open, FileAccess.Read );
			m_BinaryReader = new BinaryReader(m_fs);
			m_Header = new TraHeader( m_BinaryReader );
			// Header read
		}

		protected override void ExtractParticleSystem()
		{
			byte[] check1 = new byte[8];
			for ( int i = 0; i < 8; i++ ) // read 8 bytes
			{
				check1[i] = m_BinaryReader.ReadByte();
			}
			string check1str = Encoding.ASCII.GetString(check1);
			if ( check1str != "SYSTDEFI")
			{
				throw new Exception("Tra File : \"SYSTDEFI\" check not passed");
			}

            if (!this.m_Silent)
            {
                Trace.WriteLine("Begining to process Protein Structures...");
                Trace.Indent();
            }

			// previous atom status holders
			string AAID = "";
			Atom currentAtom = null;
			AminoAcid currentAminoAcid = null;
			TraParticleDef particleDef = new TraParticleDef();

			AssociatedObjectList m_BondingDefs = new AssociatedObjectList();
			
			m_PS = new ParticleSystem( m_FileInfo.Name );
			m_PS.BeginEditing();
			
			if( m_Header.atoms > 0 )
			{
				PolyPeptide pp = new PolyPeptide('A');
				m_PS.AddMolContainer(pp);
				for ( int i = 0; i < m_Header.atoms; i++ )
				{
					particleDef.SetFromReader( m_BinaryReader );
					m_BondingDefs.Add( particleDef.n_cov12atoms, particleDef.cov12atom );
					if (AAID != particleDef.parentnumber.ToString())
					{
						if ( currentAminoAcid != null )
						{
							pp.addMolecule(currentAminoAcid);
						}
						AAID = particleDef.parentnumber.ToString();
						cullResString( ref particleDef.parentname );
						currentAminoAcid = new AminoAcid( particleDef.parentname, particleDef.parentnumber, ' ' );
					}

					currentAtom = TRADefinitionToAtom( i, particleDef, currentAminoAcid );
					currentAminoAcid.addAtom(currentAtom);
				}
				pp.addMolecule(currentAminoAcid); // add the last one

				// apply the molecule primitives
				//pp[0].setMolPrimitive( 'N', true ); 
				for( int i = 0; i < pp.Count; i++ )
				{
					pp[i].setMolPrimitive( true );
				}
				//pp[ pp.Count - 1 ].setMolPrimitive( 'C', true );
			}

            m_PS.EndEditing(false,true);
			m_Positions = new PS_PositionStore( m_PS ); // initialise this, even if we dont add to it later, it will contain the initial positions

			// need to use the Tra files bonding definitions
			for ( int i = 0; i < m_BondingDefs.Count; i++ )
			{
				int   n_cov12atoms = (int)   m_BondingDefs.Get1(i);
				int[] cov12atom    = (int[]) m_BondingDefs.Get2(i);

				if ( n_cov12atoms > 6 ) throw new TraException("Too many bonds defined for a single atom ...");
				Atom atom1 = m_PS[i];

				for ( int j = 0; j < n_cov12atoms; j++ )
				{
					if ( cov12atom[j] >= m_PS.Count || cov12atom[j] < 0)
					{
						Trace.WriteLine(j.ToString() + " : Error, bonding instruction given for a atom number ouside the bounds of the system");
						continue;
					}
					Atom atom2 = m_PS[ cov12atom[j] ];
					if(atom1 != null && atom2 != null)
					{
						atom1.bondTo(atom2);
						//atom2.bondTo(atom1); ... no, all bonds are listed for all atoms
					}
					else
					{
						Trace.WriteLine("Bonding directive given for a null atom position in the array...");
					}
				}
			}

			m_Sequence = new PSSequence(m_PS);

            if (!this.m_Silent)
            {
                Trace.Unindent();
                Trace.WriteLine("Protein Structures Processed : " + m_PS.Count.ToString() + " atoms incorperated");
            }

			// Finshed geting system defintion

			// We must now proceed through the file until the start of the TE entries, defined by the header
			
			// record this
			int readSoFar = TraHeader.HEADER_BYTESIZE 
				+ TraParticleDef.SYSDEF_FLAGSTRING_BYTESIZE 
				+ ( TraParticleDef.SYSDEF_BYTESIZE * m_Header.atoms );

             
			// at this location we will now change the code to parse for blocks that match the known method strings
			// these are currently :
			// 0) Explicit REBUILDP and IMPROPER blocks could be present if the tra file is PD derived
			// 1) "EXTDVECT" : Extended vectors to display. these are intended to show method specific lines e.g. rotational
			// moments or the location of focring potentials
			// 2) ....
	
			int stillToRead = m_Header.trajectorystart - readSoFar;
			if( stillToRead >= 8 ) // an extended block of some description is present
			{
				// now perform a sanity check, read to the TRASTART and check its there

				while ( m_Header.trajectorystart > readSoFar )
				{
					// read a header block ...
					string checkStringExtdBlocks = Encoding.ASCII.GetString(m_BinaryReader.ReadBytes(8));
					readSoFar += 8;

					// decide what to do with it ...
					// the custom block needs to be parsed ...
					if( 0 == String.CompareOrdinal( checkStringExtdBlocks, 0, "EXTDVECT", 0, 8 ) )
					{
						// flags that we are to expect extended vectors in the file below.
						// an integer follows the flag to signify how many we will have ...
						m_Header.extnesion_EXTDVECT = m_BinaryReader.ReadInt32();
						readSoFar += 4; // 4 bytes in an integer
					}
					else if( 0 == String.CompareOrdinal( checkStringExtdBlocks, 0, "EXTDCMNT", 0, 8 ) )
					{
						// flags that we are to expect extended comments in the file below.
						// an integer follows the flag to signify their max length ...
						m_Header.extension_EXTDCMNT = m_BinaryReader.ReadInt32();
						readSoFar += 4; // 4 bytes in an integer
					}
					else if( 0 == String.CompareOrdinal( checkStringExtdBlocks, 0, "REBUILDP", 0, 8 ) )
					{
						// "REBUILDP" then 4 then 4 then 16384 
						// Why? ---> only mike knows thw wonders of the REBUILDP block 
						// lets ignore it shall we? ;-)
						m_BinaryReader.BaseStream.Position += 16392;
						readSoFar += 16392;
					}
					else if( 0 == String.CompareOrdinal( checkStringExtdBlocks, 0, "IMPROPER", 0, 8 ) )
					{
						stillToRead = m_BinaryReader.ReadInt32(); // the number of Imporper structs to expect following the int
						readSoFar += 4; // the above int read
						stillToRead *= 16; // 4ints * 4 bytes
						m_BinaryReader.BaseStream.Position += stillToRead;
						readSoFar += stillToRead;
					}
					else
					{
						// here we have a problem ...
						// we have no idea how long this unknown block should be, and therefore we must display 
						// a warning and abort any other extended blocks - the only safe way...
						Trace.WriteLine("WARNING: Unknown extended block encountered. All further blocks will be ignored!");
						stillToRead = m_Header.trajectorystart - readSoFar;
						m_BinaryReader.BaseStream.Position += stillToRead;
						readSoFar += stillToRead;
					}
				}
			}
			else if( stillToRead > 0 ) // between 1 and 7 inclusive bytes! - oddness
			{
				m_BinaryReader.BaseStream.Position += stillToRead;
				readSoFar += stillToRead;
				Trace.WriteLine("WARNING: There is a small number of residual bytes during a tra read between \"TRASTART\" and m_Header.trajectorystart. Slightly dodgy?" );
			}
            else if (stillToRead == 0)
            {
                // all fine!! :-D
            }
            else
            {
                // How did that happen? Should be error checked by the header ?
                throw new Exception("CODE ERROR : Not enough bytes remaining following Tra header reading");
            }

			// Check to see if we have got there ...
			string check2str = Encoding.ASCII.GetString(m_BinaryReader.ReadBytes(8));
			readSoFar += 8;
			if ( 0 != String.CompareOrdinal( check2str, 0, "TRASTART", 0, 8 ))
			{
				throw new TraException("Tra File : \"TRASTART\" check not passed");
			}

			// Didnt really need the last bit, but it is a good check to make ....
			// now we know that the tra start position is defined ok

			// Lets check file size constraints for loading

			m_TraPreview = new TraPreview();
			SetTraPreviewToInitState( m_fs.Length );

			m_BinaryReader.Close(); // we dont need to read any more until the user / program decides to read in a selection of the tra entries
			m_BinaryReader = null;
		}

		private void SetTraPreviewToInitState( long fileSize )
		{
			m_TraPreview.TEStartPoint = m_Header.trajectorystart;
			m_TraPreview.fileSize = fileSize;
			m_TraPreview.TEBlockSize = m_Header.blocksize;

			// the -8 is the length of TRASTART - the TE entries follow this ...
			float remainder = ((float)(m_TraPreview.fileSize - m_Header.trajectorystart - 8)) % (float)m_TraPreview.TEBlockSize;
			if ( remainder != 0 )
			{
				Trace.WriteLine("IMPORT WARNING! : The file-size information does not parse properly. Following TE entry processing, there is a remainder of bytes! : \nRemainder = " + remainder.ToString() + " bytes");
			}

			m_TraPreview.numberOfEntries = (int) (((float)(m_TraPreview.fileSize - m_Header.trajectorystart - 8)) / (float)m_TraPreview.TEBlockSize);
			
			m_TraPreview.startPoint = 0;
			m_TraPreview.skipLength = 1;
			m_TraPreview.endPoint = m_TraPreview.numberOfEntries;
		}

		#endregion

		#region helperfunctions

		public static char[,] getCharArray(BinaryReader r, int columns, int rows)
		{
			char[,] returnArray = new char[columns,rows];
			for ( int i = 0; i < columns; i++ )
			{
				for ( int j = 0; j < rows; j++ )
				{
					returnArray[i,j] = r.ReadChar();
				}
			}
			return returnArray;
		}

		public static int[] getIntArray(BinaryReader r, int length)
		{
			int[] returnArray = new int[length];
			for ( int i = 0; i < length; i++ )
			{
				returnArray[i] = r.ReadInt32();
			}
			return returnArray;
		}

		public static float[] getFloatArray(BinaryReader r, int length)
		{
			float[] returnArray = new float[length];
			for ( int i = 0; i < length; i++ )
			{
				returnArray[i] = r.ReadSingle();
			}
			return returnArray;
		}

		public static char[] getCharArray( BinaryReader r, int length )
		{
			char[]returnArray = new char[length];
			bool endReached = false;
			for ( int i = 0; i < length; i++ )
			{
				if ( endReached )
				{
					r.ReadByte();
					returnArray[i] = '\0';
				}
				else
				{
					returnArray[i] = Convert.ToChar( r.ReadByte() );
				}

				if ( returnArray[i] == '\0' )
				{
					endReached = true;
				}
			}
			return returnArray;
		}
        

		private void cullResString( ref string name ) // to remove trailing '/0' and ' ' (mike and allan respectively)
		{
			name = name.TrimEnd('\0');
			name = name.TrimEnd(' ');
		}

		private Atom TRADefinitionToAtom( int number, TraParticleDef currentTSEntry, AminoAcid parent )
		{

			string pdbName = currentTSEntry.pdbname;
			pdbName = pdbName.TrimEnd('\0');
			pdbName = pdbName.PadRight(4,' ');
			if ( !char.IsNumber( pdbName[0] ) )
			{
				// the PDBID doesnt have a number as the first character, therefore they all have to move along one ...
				// i.e. "N   " should be " N  " ... but "1H  " should stay as it is ...
				// then additional check to take "HH12" into account
				if ( !(char.IsLetterOrDigit(pdbName[0]) &&
					char.IsLetterOrDigit(pdbName[1]) &&
					char.IsLetterOrDigit(pdbName[2]) &&
					char.IsLetterOrDigit(pdbName[3]))   )
				{
					pdbName = " " + pdbName;
				}
			}
			if ( pdbName.Length > 4 ) 
			{
				pdbName = pdbName.Substring(0,4);
			}
			else if ( pdbName.Length < 4 ) 
			{
				pdbName = pdbName.PadRight(4,' ');
			}
			currentTSEntry.pdbname = pdbName;

			return new Atom(
				pdbName,
				number,
				number,
				parent,
				currentTSEntry.targetx,
				currentTSEntry.targety,
				currentTSEntry.targetz );
		}


		#endregion

		#region IDataProvider Members

		public DataManager DataStore
		{
			get
			{
				return m_DataManager;
			}
		}

		#endregion

		#region externalacessors

		public int Length
		{
			get
			{
				return m_Positions.Count;
			}
		}
		public TraPreview TraPreview
		{
			get
			{
				return m_TraPreview;
			}
		}
		public string Descriptor
		{
			get
			{
				return m_Header.descriptor;
			}
		}

		public string AssociatedText
		{
			get
			{
				return m_Header.text;
			}
		}

		public TraHeader Header 
		{
			get
			{
				return m_Header;
			}
		}

		public PositionStore ForceVectors
		{
			get
			{
				return m_ForceVectors;
			}
		}
		public Line3DStore ExtendedVectors
		{
			get
			{
				return m_ExtdVectors;
			}
		}
		public StringStore ExtendedComments
		{
			get
			{
				return m_ExtdComments;
			}
		}
		
		#endregion

		#region file writing helper functions
		private static void WriteHeader( BinaryWriter rw, TraContents contentTypes, TraSaveInfo saveInfo, PSMolContainer psm, int paddingbytesBeforeTraStart, bool EnergiesWillBwWritten )
		{
			
			//		public int      version;                        // version - currently 2
			//		public int      type;                           // type descriptors
			//		public int      residues;                       // nr of residues
			//		public int      atoms;                          // nr of atoms
			//		public int      blocksize;                      // blocksize of PS_PositionStore entry
			//		public int      trajectorystart;                // byte position of the start of first            PS_PositionStore entry
			//		public int      dateday,datemonth,dateyear;     // date
			//		public char[]   ID0;                            // Length 32 - custom ID strings
			//		public char[]   ID1;
			//		public char[]   ID2;
			//		public char[]   ID3;
			//		public char[,]  customAtomProperty;             // Length 32 * 16   - names of custom Properties
			//		public char[,]  customEnergyEntry;              // Length 64 * 16   - names of custom energies
			//		public char[]   descriptor;                     // Length 1024      - custom 1K descriptor ASCII area
			//		public char[]   text;                           // Length 1024 * 15 - custom 15K descriptor ASCII area

			rw.Write( 2 ); // Tra File Protocol Version
			int type = 0;
			type = type | TRAJECTORY_TYPE_ATOMPOS; // this is mandaroty
			if ( EnergiesWillBwWritten )
			{
				type = type | TRAJECTORY_TYPE_ENERGY;
			}
			rw.Write( type );
			rw.Write( psm.Count );
			int atomCount = psm.Atoms.Count;
			rw.Write( atomCount );

			// + 4 = "TRAE"
			// 12 = 4 bytes * 3 floats for vector
			int blockSize = 4 + (psm.Atoms.Count * 12);
			if ( EnergiesWillBwWritten ) blockSize += 256; // 64 energy floats * 4 bytes

			rw.Write( blockSize );

			int traStartPos = TraHeader.HEADER_BYTESIZE 
				+ TraParticleDef.SYSDEF_FLAGSTRING_BYTESIZE 
				+ ( TraParticleDef.SYSDEF_BYTESIZE * atomCount ) 
				+ paddingbytesBeforeTraStart;

			rw.Write( traStartPos ); // the size of the header structure in this case as we have no padding
			DateTime date = DateTime.Now;
			rw.Write( date.Day );
			rw.Write( date.Month );
			rw.Write( date.Year );
			rw.Write( saveInfo.ID0.ToCharArray() ); // ID0
			rw.Write( saveInfo.ID1.ToCharArray() ); // ID1
			rw.Write( saveInfo.ID2.ToCharArray() ); // ID2
			rw.Write( saveInfo.ID3.ToCharArray() ); // ID3
			rw.Write( saveInfo.CustomPropertyBlock.ToCharArray() ); // the custom properties
			rw.Write( saveInfo.Descriptor.ToCharArray() );
			rw.Write( saveInfo.Text.ToCharArray() );
			// header done
		}

		private static char[] getFormattedName( string name )
		{
			char[] returnArray = new char[8];
			for( int i = 0; i < 8; i++ )
			{
				if( i < name.Length && name[i] != ' ' )
				{
					returnArray[i] = name[i];
				}
				else
				{
					returnArray[i] = '\0';
				}
			}
			return returnArray;
		}

		private static void WriteSystemDefinition( BinaryWriter rw, PSMolContainer psm )
		{
			
			rw.Write("SYSTDEFI".ToCharArray());

			int atomNumberOffset = psm.Atoms[0].ArrayIndex; // see below, we will renumber the atoms, but then the bonding partner ID will need this

			for( int i = 0; i < psm.Atoms.Count; i++ )
			{
				Atom a = psm.Atoms[i];

				//		public int     atomnumber;                    // atom number
				//		public char[]  pdbname;                       // Length 8 - pdb name
				//		public char[]  primitivetype;                 // Length 8 - primitive forcefield type
				//		public char[]  altname;                       // Length 8 - alternative name
				//
				//		public int     parentnumber;                  // parent (i.e. residue) number
				//		public char[]  parentname;                    // Length 8 - parentname
				//
				//		public float   targetx,targety,targetz;       // Xray strucuture coordinate if known
				//		public bool    targetknown;                   // 0 if unknown , 1 if known
				//
				//		public int[]   cov12atom;                     // Length 6 - atom indices with bondorder 1
				//		public int    n_cov12atoms;
				//		public float   charge;
				//		public float   radius;
				//		public float[] customProperty;                // Length 32

				rw.Write( i ); // all atoms need a unique atom index, may as well use "i" for this ...
				string PDBName = a.PDBType;
				// make sure that there is no space at the start
				if( PDBName[0] == ' ' )
				{
					PDBName = PDBName.Substring(1,3) + ' ';
				}
				rw.Write( getFormattedName( PDBName ) );
				rw.Write( getFormattedName( a.atomPrimitive.ForceFieldID ) );
				rw.Write( getFormattedName( a.ALTType ) );
				rw.Write( a.parentMolecule.ResidueNumber );
				rw.Write( getFormattedName( a.parentMolecule.Name ) );
				rw.Write( a.xFloat );
				rw.Write( a.yFloat );
				rw.Write( a.zFloat );
				rw.Write( 0 ); // target not known ???? do i care ?
				if( a.bondCount > 6 )
				{
					throw new TraException("The number of bonds exceeds the tra specification in : " + a.ToString() );
				}
				for( int j = 0; j < a.bondCount; j++ )
				{
					rw.Write( a.BondedAtoms[j].ArrayIndex - atomNumberOffset );
				}
				for( int j = a.bondCount; j < 6; j++ )
				{
					rw.Write( -1 ); // fill in the remaining spaces upto the max of 6
				}
				rw.Write( a.bondCount );
				rw.Write( a.atomPrimitive.DefaultCharge );
				rw.Write( a.atomPrimitive.Radius );
				for( int j = 0; j < 32; j++ )
				{
					rw.Write(0.0f); // no custom properties
				}           
			}

			// done writing SYSDEFI
		}

		private static int[] GetImpropers( BinaryWriter rw, PSMolContainer psm )
		{
			ArrayList integers = new ArrayList();

			for( int i = 0; i < psm.Count; i++ )
			{
				Torsion[] torsions = psm[i].moleculePrimitive.Torsions;

				for ( int j = 0; j < torsions.Length; j++ )
				{
					int atom1 = -1;
					int atom2 = -1;
					int atom3 = -1;
					int atom4 = -1;

					// check each definition in turn to see if we have all the relevent atoms present for the torsion
					string checkType = torsions[j].Type_1;
					Molecule molToCheck = null;

					if( checkType[0] == '-' )
					{
						checkType = checkType.Substring(1,3) + " ";
						molToCheck = psm[i].Parent.nextMolecule(psm[i]);
					}
					else if( checkType[0] == '+' )
					{
						checkType = checkType.Substring(1,3) + " ";
						molToCheck = psm[i].Parent.previousMolecule(psm[i]);
					}
					else
					{
						molToCheck = psm[i];
					}
					if( molToCheck == null ) continue;
					for( int k = 0; k < molToCheck.Count; k++ )
					{															
						if( molToCheck[k].ALTType == checkType )
						{
							atom1 = molToCheck[k].ArrayIndex;
							break;
						}
					}




					checkType = torsions[j].Type_2;
					molToCheck = null;

					if( checkType[0] == '-' )
					{
						checkType = checkType.Substring(1,3) + " ";
						molToCheck = psm[i].Parent.nextMolecule(psm[i]);
					}
					else if( checkType[0] == '+' )
					{
						checkType = checkType.Substring(1,3) + " ";
						molToCheck = psm[i].Parent.previousMolecule(psm[i]);
					}
					else
					{
						molToCheck = psm[i];
					}
					if( molToCheck == null ) continue;
					for( int k = 0; k < molToCheck.Count; k++ )
					{
						if( molToCheck[k].ALTType == checkType )
						{
							atom2 = molToCheck[k].ArrayIndex;
						}
					}




					checkType = torsions[j].Type_3;
					molToCheck = null;

					if( checkType[0] == '-' )
					{
						checkType = checkType.Substring(1,3) + " ";
						molToCheck = psm[i].Parent.nextMolecule(psm[i]);
					}
					else if( checkType[0] == '+' )
					{
						checkType = checkType.Substring(1,3) + " ";
						molToCheck = psm[i].Parent.previousMolecule(psm[i]);
					}
					else
					{
						molToCheck = psm[i];
					}
					if( molToCheck == null ) continue;
					for( int k = 0; k < molToCheck.Count; k++ )
					{
						if( molToCheck[k].ALTType == checkType )
						{
							atom3 = molToCheck[k].ArrayIndex;
						}
					}


					checkType = torsions[j].Type_4;
					molToCheck = null;

					if( checkType[0] == '-' )
					{
						checkType = checkType.Substring(1,3) + " ";
						molToCheck = psm[i].Parent.nextMolecule(psm[i]);
					}
					else if( checkType[0] == '+' )
					{
						checkType = checkType.Substring(1,3) + " ";
						molToCheck = psm[i].Parent.previousMolecule(psm[i]);
					}
					else
					{
						molToCheck = psm[i];
					}
					if( molToCheck == null ) continue; 
					for( int k = 0; k < molToCheck.Count; k++ )
					{
						if( molToCheck[k].ALTType == checkType )
						{
							atom4 = molToCheck[k].ArrayIndex;
						}
					}
				
					if ( atom1 != -1 &&
						atom2 != -1 &&
						atom3 != -1 &&
						atom4 != -1 )
					{
						integers.Add( atom1 );
						integers.Add( atom2 );
						integers.Add( atom3 );
						integers.Add( atom4 );
					}
				}
			}
			

			return (int[]) integers.ToArray( typeof(int) );
		}
		#endregion

		#region additional extraction functions

		private int m_CacheTraIndex = -1;
		public void SetForReInitialise() // used to clear the tra prior to a reload of more data
		{
			// need to find the file size first and set this in the header block
			m_CacheTraIndex = m_Positions.Position;
			m_TEEntries.Clear(); // these hold the definitions in each tra position, this should be cleared
			m_DataManager.Clear();
			m_Positions.Clear();
			if( m_ForceVectors != null )
			{
				m_ForceVectors.Clear();
			}
			if( m_ExtdVectors != null )
			{
				m_ExtdVectors.Clear();
			}
			if( m_ExtdComments != null )
			{
				m_ExtdComments.Clear();
			}
			m_FileInfo.Refresh(); // gets the true filesize again - needed !
			SetTraPreviewToInitState(m_FileInfo.Length);
		}
		
		public void LoadTrajectory()
		{
			// What is the size per block ? how many padding bytes have therefore been assigned by the Tra file Creator ?
			// these padding bytes will hold program specific data and are not read by the DAVE framework

			int sizeInUse = 4; // TRAE check is 4 bytes long ...

			int atomposDefined = TRAJECTORY_TYPE_ATOMPOS & m_Header.type;
			if ( atomposDefined > 0)
			{
				int posSize = 12 * m_Header.atoms; //12 bytes per position definition
				sizeInUse += posSize;
			}
			int phipsiDefined = TRAJECTORY_TYPE_PHIPSI & m_Header.type;
			if ( phipsiDefined > 0)
			{
				int phiPsiSize = 8 * m_Header.residues; // 8 bytes per pair - i.e. 2 floats
				sizeInUse += phiPsiSize;
			}
			int rotamersDefined = TRAJECTORY_TYPE_ROTAMERS & m_Header.type;
			if ( rotamersDefined > 0)
			{
				int rotamerSize = 4 * m_Header.residues;  // 4 bytes per rotamer
				sizeInUse += rotamerSize;
			}
			int energiesDefined = TRAJECTORY_TYPE_ENERGY & m_Header.type;
			if ( energiesDefined > 0)
			{
				int energySize = 4 * 64; // 64 floats are defined per energy structure.
				sizeInUse += energySize;
			}
			int forcevectorDefined = TRAJECTORY_TYPE_FORCEVECTOR & m_Header.type;
			if ( forcevectorDefined > 0)
			{
				if( m_ForceVectors == null ) // will be when first loaded
				{
					m_ForceVectors = new PositionStore( m_Header.atoms ); // must be defined once the array-width is known
					// the position of this must be linked to the position of the PS_PositionStore for atoms
					m_ForceVectors.linkIndex( m_Positions );

					// add the first array as the positions of the atoms
					Position[] nullForcePos = new Position[ m_Header.atoms ];
					for( int q = 0; q < nullForcePos.Length; q++ )
					{
						nullForcePos[q] = new Position( m_PS[q] );
					}

					m_ForceVectors.AddPositionArray( nullForcePos );
				}
				else
				{
                    m_ForceVectors.Clear();
				}

				int posSize = 12 * m_Header.atoms; //12 bytes per position definition
				sizeInUse += posSize;
			}
			if( m_Header.extnesion_EXTDVECT > 0 )
			{
				if( m_ExtdVectors == null ) // will be when first load ...
				{
					m_ExtdVectors = new Line3DStore( m_Header.extnesion_EXTDVECT );
					m_ExtdVectors.linkIndex( m_Positions );

					// add the first array as null vectors
					Line3D[] nullVects = new Line3D[ m_Header.extnesion_EXTDVECT ];
					float[] nullDef = new float[] { 0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f,0.0f };
					for( int q = 0; q < nullVects.Length; q++ )
					{
						nullVects[q] = new Line3D( nullDef );
					}

					m_ExtdVectors.addLine3DArray( nullVects );
				}
				else
				{
					m_ExtdVectors.Clear();
				}
				sizeInUse += ( 28 * m_Header.extnesion_EXTDVECT ); // (4byts * 6floats) + (1 * 4byteInt) = 28 bytes per vector
			}
			if( m_Header.extension_EXTDCMNT > 0 )
			{
				if( m_ExtdComments == null ) // will be when first load ...
				{
					m_ExtdComments = new StringStore( m_Header.extension_EXTDCMNT );
					m_ExtdComments.linkIndex( m_Positions ); // when the index of the positions changes, so does the current index of this store...
					m_ExtdComments.AddString( "No string is givem for the template entry..." );
				}
				else
				{
					m_ExtdComments.Clear();
				}
				sizeInUse += ( m_Header.extension_EXTDCMNT ); // 1 char == 1 byte
			}

			m_fs = new FileStream( m_FileInfo.FullName, FileMode.Open, FileAccess.Read );
			m_BinaryReader = new BinaryReader(m_fs);

			m_BinaryReader.BaseStream.Position += m_Header.trajectorystart; // read past the header to the start of the position definitions

			// Check to see if we have got there ...
			byte[] check2 = new byte[8];
			for ( int i = 0; i < 8; i++ ) // read 8 bytes
			{
				check2[i] = m_BinaryReader.ReadByte();
			}		
	
			string check2str = Encoding.ASCII.GetString(check2);
			if ( check2str != "TRASTART")
			{
				throw new Exception("Tra File : \"TRASTART\" check not passed");
				// this would be very odd as this should have already been passed above ... ????!!?!?
			}

			// Cool !			
			// lets get some trajectory entries !!!

			if ( m_TraPreview.skipLength < 1 )
			{
				m_TraPreview.skipLength = 1; // anyting of 0 or less would rather fuck us up below ...
			}

			m_BinaryReader.BaseStream.Position += (m_TraPreview.startPoint * m_Header.blocksize);

			Position[] atomPositions = new Position[0];
			Position[] forceVectors = new Position[0];
			Line3D[]   extdVectors = new Line3D[0];
			float[] phis = new float[0];
			float[] psis = new float[0];
			int[] rotamers = new int[0];
			float[] energies = new float[0];

			float skipper;
			for ( int j = m_TraPreview.startPoint; j < m_TraPreview.endPoint; j++ )
			{
				skipper = ((float)(j-m_TraPreview.startPoint) % (float)m_TraPreview.skipLength); 
				// twas a bug!, didnt read in the correct skipped entry
				//float skipperold = ((float)(j) % (float)m_TraPreview.skipLength); 
				if( 0.0f != skipper )
				{
					m_BinaryReader.BaseStream.Position += m_Header.blocksize;
					// skip blocks that we dont want as defied in the m_TraPreview options
				}
				else
				{

					byte[] check3 = new byte[4];
					for ( int i = 0; i < 4; i++ )
					{
						check3[i] = m_BinaryReader.ReadByte();
					}
					string check3str = Encoding.ASCII.GetString(check3);
					if ( check3str != "TRAE")
					{
						throw new Exception("Tra File : \"TRAE\" check not passed");
					}

					if ( atomposDefined > 0 ) // setup the pos holder here, this will always be defined, it is compulsory
					{
						atomPositions = new Position[ m_Header.atoms ];
						for ( int i = 0; i < m_Header.atoms; i++ )
						{
							atomPositions[i] = new Position( m_BinaryReader.ReadSingle(), m_BinaryReader.ReadSingle(), m_BinaryReader.ReadSingle() );
						}
					}

					if ( phipsiDefined > 0 )
					{
						phis = new float[ m_Header.residues ];
						psis = new float[ m_Header.residues ];

						for ( int i = 0; i < m_Header.residues; i++ )
						{
							phis[i] = m_BinaryReader.ReadSingle();
							psis[i] = m_BinaryReader.ReadSingle();
						}
					}

					if ( rotamersDefined > 0 )
					{
						rotamers = new int[ m_Header.residues ];
					}

					if ( energiesDefined > 0 )
					{
						energies = Tra.getFloatArray(m_BinaryReader,64);
					} 
            		
					if ( forcevectorDefined > 0 ) // these are in the file after the definition of other things
					{
						forceVectors = new Position[ m_Header.atoms ];
						for ( int i = 0; i < m_Header.atoms; i++ )
						{
							forceVectors[i] = new Position( m_BinaryReader.ReadSingle(), m_BinaryReader.ReadSingle(), m_BinaryReader.ReadSingle() );
						}
					}

					m_TEEntries.Add( new TEEntry( atomPositions, forceVectors, phis, psis, rotamers, energies ) );

					if( m_ExtdVectors != null )
					{
						Line3D[] lines = new Line3D[m_Header.extnesion_EXTDVECT];						
						float[] init = new float[6];
						int colourCode;
						for( int i = 0; i < lines.Length; i++ )
						{
							for( int k = 0; k < 6; k++ )
							{
								init[k] = m_BinaryReader.ReadSingle();
							}
							colourCode = m_BinaryReader.ReadInt32();
							lines[i] = new Line3D( init, colourCode );                            						
						}
						m_ExtdVectors.addLine3DArray(lines);
					}

					if( m_ExtdComments != null )
					{
						byte[] encodedString = m_BinaryReader.ReadBytes( m_Header.extension_EXTDCMNT );		
						m_ExtdComments.AddString( Encoding.ASCII.GetString(encodedString) );
					}

					if ( m_Header.blocksize > sizeInUse )
					{
						m_BinaryReader.BaseStream.Position += (m_Header.blocksize-sizeInUse);
					}
				}
			}

			m_BinaryReader.Close();

			GenerateEnergies();
            GenerateTrajectory(); // needs to be called in this order... for the call (m_Positions.Position = m_CacheTraIndex;)
		}

		private void GenerateTrajectory()
		{
			for ( int i = 0; i < m_TEEntries.Count; i++ )
			{
				TEEntry te = (TEEntry) m_TEEntries[i];
				m_Positions.addPositionArray( te.Positions, false );
				if( te.ForceVectors.Length > 0 )
				{
					m_ForceVectors.AddPositionArray( te.ForceVectors );
				}
			}
			if( m_CacheTraIndex != -1 && m_CacheTraIndex >= 0 && m_CacheTraIndex < m_Positions.Count )
			{
				m_Positions.Position = m_CacheTraIndex;
			}
			else
			{
				m_Positions.Position = 0;
			}
		}

		private void GenerateEnergies()
		{
			int energiesDefined = TRAJECTORY_TYPE_ENERGY & m_Header.type;
			if ( energiesDefined > 0)
			{
				float[,] columns = new float[64,m_TEEntries.Count];

				for ( int j = 0; j < 64; j++ )
				{
					for ( int i = 0; i < m_TEEntries.Count; i++ )
					{
						TEEntry te = (TEEntry) m_TEEntries[i];
						columns[j,i] = te.Energies[j];
					}
				}
				for ( int k = 0; k < 64; k++ )
				{
					if ( m_Header.customEnergyEntry[k,0] != '\0' )
					{
						char[] chars = new char[16];
						for ( int m = 0; m < chars.Length; m++ )
						{
							chars[m] = m_Header.customEnergyEntry[k,m];
						}
						string title = new string( chars );

						title = title.TrimEnd('\0');
						title = title.TrimEnd(' ');

						float[] data = new float[m_TEEntries.Count];
						for ( int l = 0; l < data.Length; l++ )
						{
							data[l] = columns[k,l];
						}
						m_DataManager.AddDataColumn(title, data);
					}
				}
			}
		}

		#endregion
	}
}
