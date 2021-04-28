using System;
using System.IO;

using UoB.Core.Structure;

namespace UoB.Core.FileIO.PDB
{
	/// <summary>
	/// Summary description for CATrace.
	/// </summary>
	public class CATrace
	{
		private CATrace()
		{
		}

		public static void WriteCATraces( string directoryString )
		{
			DirectoryInfo directory = new DirectoryInfo(directoryString);
			WriteCATraces( directory );
		}

		public static void WriteCATraces( DirectoryInfo directory )
		{
			foreach( FileInfo fi in directory.GetFiles("*.pdb") )
			{
				WriteCATrace( fi.FullName, directory.FullName + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension( fi.FullName ) + ".CA.PDB" );
			}
		}

		public static void WriteCATrace( ParticleSystem ps, string outputPDBFileName )
		{
			StreamWriter rw = new StreamWriter( outputPDBFileName, false );
			for( int i = 0; i < ps.Count; i++ )
			{
				if( ps[i].PDBType == PDBAtom.PDBID_BackBoneCA )
				{
					rw.WriteLine( PDB.MakePDBStringFromAtom( ps[i] ) );
				}
			}
			rw.Close();
		}

		public static void WriteCATrace( string inputPDBFileName, string outputPDBFileName )
		{
			StreamReader re = new StreamReader(inputPDBFileName);
			StreamWriter rw = new StreamWriter(outputPDBFileName);
			string line;
			while( ( line = re.ReadLine() ) != null )
			{
				if(    0 == String.Compare( line,  0, "ATOM  ", 0, 6, true ) 
					&& 0 == String.Compare( line, 12, " CA "  , 0, 4, true ) 
					)
				{
					rw.WriteLine( line );
				}
			}
			re.Close();
			rw.Close();
		}
	}
}
