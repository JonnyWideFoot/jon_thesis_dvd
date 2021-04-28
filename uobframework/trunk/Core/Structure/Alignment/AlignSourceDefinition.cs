using System;
using System.IO;
using System.Text;

using UoB.Core.FileIO.PDB;
using UoB.Core.Structure;

namespace UoB.Core.Structure.Alignment
{
	/// <summary>
	/// Summary description for AlignSourceDefinition.
	/// </summary>
	public class AlignSourceDefinition
	{
		private string m_FileName;
		private char m_ChainID;
		private int m_StartResID;
		private int m_Length;
		private MolRange m_MolRange;

        public AlignSourceDefinition(ParticleSystem ps, int molIndex)
        {
            m_FileName = "";
            PSMolContainer mol = ps.MemberAt(molIndex);
            if (mol == null) throw new Exception();
            m_ChainID = mol.ChainID;
            m_StartResID = mol[0].ResidueNumber;
            m_Length = mol.Count;

            m_MolRange = new MolRange(ps.MemberAt(molIndex));
        }

        public AlignSourceDefinition(string filename, ParticleSystem ps, int molIndex)
        {
            m_FileName = filename;
            PSMolContainer mol = ps.MemberAt(molIndex);
            if( mol == null ) throw new Exception();
            m_ChainID = mol.ChainID;
            m_StartResID = mol[0].ResidueNumber;
            m_Length = mol.Count;

            m_MolRange = new MolRange(ps.MemberAt(molIndex));
        }

		public AlignSourceDefinition( string filename, char chainID, int startResID, char insertionCode, int length )
		{
			m_FileName = filename;
			m_ChainID = chainID;
			m_StartResID = startResID;
			m_Length = length; 

			FileInfo fi = new FileInfo( m_FileName );
			if( !fi.Exists )
			{
				throw new Exception("File could not be found at : " + m_FileName );
			}
			PDB pdbFile = new PDB( m_FileName, true );
			m_MolRange = new MolRange(pdbFile.particleSystem.MemberWithID(m_ChainID),m_StartResID, ' ', m_Length );

			// Jon edit flag
			// see public AlignSourceDefinition( StreamReader re )
			// m_MolRange = new MolRange(pdbFile.particleSystem.MemberWithID(m_ChainID),' ',m_StartResID, m_EndResID );
			// found that MolRange needs the insertion code to properly represent a range, and that length was a better representation than endID, this is now changed in the base class, and therefore this class needs to be changed too
			throw new Exception("");

			// also there is no point in having an internal molrange AND the other chars and ints
			// decide which one is best to keep and stick to it
		}

		public AlignSourceDefinition( string sourceFileName, MolRange molRange )
		{
			FileInfo fi = new FileInfo( sourceFileName );
			if( !fi.Exists )
			{
				throw new Exception("File could not be found at : " + m_FileName );
			}
			m_FileName = sourceFileName;
			m_MolRange = molRange;
			if( m_MolRange == null )
			{
				throw new Exception("The given MolRange was null!");
			}
			m_StartResID = m_MolRange.StartID;
			m_Length = m_MolRange.Length;
			m_ChainID = m_MolRange.ChainID;


			// Jon edit flag
			// see public AlignSourceDefinition( StreamReader re )
			// m_MolRange = new MolRange(pdbFile.particleSystem.MemberWithID(m_ChainID),' ',m_StartResID, m_EndResID );
			// found that MolRange needs the insertion code to properly represent a range, and that length was a better representation than endID, this is now changed in the base class, and therefore this class needs to be changed too
			throw new Exception("");

			// also there is no point in having an internal molrange AND the other chars and ints
			// decide which one is best to keep and stick to it
		}

		public AlignSourceDefinition( StreamReader re )
		{
			string lineCache;

			string fileName = re.ReadLine().Trim();
			AssertLine( ref fileName, "FileName ","Filename definition not found where expected.");
			FileInfo fileInfo = new FileInfo( fileName );
			if( !fileInfo.Exists )
			{
				throw new Exception("Filename is not present at that location : " +  fileName );
			}
			if( fileInfo.Extension.ToUpper() != ".PDB" )
			{
				throw new Exception("Only filenames with a PDB extension are currently supported : " + m_FileName );
			}

			m_FileName = fileInfo.FullName;
			lineCache = re.ReadLine();
			AssertLine( ref lineCache, "ChainID ", "ChainID definition not found where expected.");
			m_ChainID = lineCache[0];
			lineCache = re.ReadLine();
			AssertLine( ref lineCache, "StartID ", "StartID definition not found where expected.");
			m_StartResID = int.Parse( lineCache );
			lineCache = re.ReadLine();
			AssertLine( ref lineCache, "Length ", "Length definition not found where expected.");
			m_Length = int.Parse( lineCache );	
	
			PDB pdbFile = new PDB( m_FileName, true );

			m_MolRange = new MolRange(pdbFile.particleSystem.MemberWithID(m_ChainID),m_StartResID,'~', m_Length );
		}

		public MolRange MolRange
		{
			get
			{
                return m_MolRange;
			}
		}

		public string FileName 
		{
			get
			{
				return m_FileName;
			}
		}

		public char ChainID
		{
			get
			{
				return m_ChainID;
			}
		}

		public int Length
		{
			get
			{
				return m_Length;
			}
		}

		public int StartResidueID
		{
			get
			{
				return m_StartResID;
			}
		}

		private static StringBuilder m_ReturnStringBuild = new StringBuilder();
		private object StringBuilderLock = new object();
		public override string ToString()
		{
			lock( StringBuilderLock )
			{
				m_ReturnStringBuild.Remove(0,m_ReturnStringBuild.Length);
				m_ReturnStringBuild.Append("\t\tFileName ");
				m_ReturnStringBuild.Append( m_FileName );
				m_ReturnStringBuild.Append("\r\n\t\tChainID ");
				m_ReturnStringBuild.Append( m_ChainID );
				m_ReturnStringBuild.Append("\r\n\t\tStartID ");
				m_ReturnStringBuild.Append( m_StartResID );
				m_ReturnStringBuild.Append("\r\n\t\tLength ");
				m_ReturnStringBuild.Append( m_Length );
				m_ReturnStringBuild.Append("\r\n");
				return m_ReturnStringBuild.ToString();
			}
		}

		#region helper functions
		
		private void AssertLine( ref string line, string checkStart, string exceptionOnFalse )
		{
			line = line.Trim();
			string lineType = line.Substring(0,checkStart.Length);
			if( lineType != checkStart )
			{
				throw new Exception( exceptionOnFalse );
			}
			line = line.Substring(checkStart.Length,line.Length-checkStart.Length);
		}

		private bool AssertLine( StreamReader re, string check )
		{
			return check == re.ReadLine().Trim().Substring(0,check.Length);
		}

		private void AssertLine( StreamReader re, string check, string exceptionOnFalse )
		{
			if( check == re.ReadLine().Trim().Substring(0,check.Length) )
			{
				return;
			}
			else
			{
				throw new Exception(exceptionOnFalse);
			}
		}

		#endregion

	}
}
