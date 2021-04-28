using System;
using System.Text;
using System.IO;
using System.Collections;

using UoB.Core;
using UoB.Methodology.TaskManagement;
using UoB.Core.FileIO.DSSP;

namespace UoB.Methodology.DSSPAnalysis
{
	/// <summary>
	/// Summary description for DSSPDirectory.
	/// Derives from TaskDirectoryInteration to allow a standard folder framework
	/// Is designed to look sequentially through all the DSSP files in the DSSP folder ...
	/// </summary>
	public abstract class DSSPTaskDirectory : TaskDirectoryInteration
	{
		// random number generator used in some derived classes
		protected UoB.Core.FastRandom m_Random = new UoB.Core.FastRandom();

		// names
		private string m_DBName = null;
		protected string m_GraphTitle = null; // holds the title as it is constructed
		protected string m_OutputFileStem = null; // used for file output name construction

		// file and directory assignment
		private int m_MaxFileImport = int.MaxValue; // effectively no maximum unless set after initialisation - used primarily in debuging ...
		private DirectoryInfo m_DSSPDir; // folder in addition to the TaskDirectoryInteration implementation
		private FileInfo[] m_FI; // to hold the DSSP file names
		private int m_ParsingFileIndex = 0; // incremented to progress through the DSSP file []
		private DSSPFile m_CurrentFile; // a single class for all loaded files, Reinitialise() is called to assign a new file ...

		public DSSPTaskDirectory( string DBName, DirectoryInfo di, bool OriginInteractionRequired ) : base( di, OriginInteractionRequired )
		{
			// name assignment
			m_DBName = DBName; // used in some output, it is impornant to state the DB name used ...

			// file assignment
			m_DSSPDir = new DirectoryInfo( di.FullName + @"\DSSP\"  );
			if( !m_DSSPDir.Exists ) throw new ArgumentException("The DSSP subdir doesnt exist!");     
			m_FI = m_DSSPDir.GetFiles("*.dssp");
			m_CurrentFile = new DSSPFile(); 
			m_CurrentFile.ImportDSSPFileIsJonFormat = true; // (should be default, but do it anyway) true == jon-edit of the DSSP format to include temp. factor and Omega angle ....
			ParsingFileIndex = 0; // initialise m_CurrentFile to the 1st file in this call
		}

		#region Public Accessors
		public string ExtractCurrentNameFromFilename()
		{
			string name = m_FI[m_ParsingFileIndex].Name;
			if( name.Length < 10 )
			{
				throw new Exception("Filename is too short to be valid" );
			}
			// assert that the filename is correct
			if( 0 != String.Compare( name, name.Length - 9, ".PDB.DSSP", 0, 9, true ) )
			{
				throw new Exception( "File name is not a defined .PDB.DSSP extension" );
			}
			return name.Substring(0,name.Length-9);
		}

		public int MaxFileImport
		{
			get
			{
				return m_MaxFileImport;
			}
			set
			{
				if( value < 1 )
				{
					throw new ArgumentException("Max file import cannot be less than 1 file!");
				}
				m_MaxFileImport = value;
			}
		}

		public DirectoryInfo DSSPDirectory
		{
			get
			{
				return m_DSSPDir;
			}
			set
			{
				// set the DI
				if( !value.Exists )
				{
					throw new Exception("Directory does not exist");
				}
				m_DSSPDir = value;

				// get the file list
				m_FI = m_DSSPDir.GetFiles("*.dssp");
				if( m_FI.Length == 0 )
				{
					throw new Exception("No dssp files");
				}
			}
		}

		protected FileInfo[] CurrentFileInfos
		{
			get
			{
				return m_FI;
			}
		}

		public string DBName
		{
			get
			{
				return m_DBName;
			}
		}

		protected DSSPFile CurrentFile
		{
			get
			{
				return m_CurrentFile;
			}
		}

		public int FileCount
		{
			get
			{
				return m_FI.Length;
			}
		}

		public int ParsingFileIndex
		{
			get
			{
				return m_ParsingFileIndex;
			}
			set
			{
				if( value < 0 || value >= m_FI.Length )
				{
					throw new ArgumentException("Index is outside the bounds of the available file list");
				}
				m_ParsingFileIndex = value;
				m_CurrentFile.Reinitialise( m_FI[ m_ParsingFileIndex ].FullName );
			}
		}
		#endregion
	}
}
