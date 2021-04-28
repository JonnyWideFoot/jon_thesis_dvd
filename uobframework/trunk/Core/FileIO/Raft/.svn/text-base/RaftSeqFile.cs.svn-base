using System;
using System.IO;

using UoB.Core.Structure;

namespace UoB.Core.FileIO.Raft
{
	/// <summary>
	/// Summary description for RaftSeqFile.
	/// </summary>
	public class RaftSeqFile
	{
		private RaftSeqFile()
		{
		}

		public static void Write( string fileName, PolyPeptide p )
		{
			StreamWriter rw = new StreamWriter( fileName );

			// Remember that the 2 previous lines are required,
			// any other lines starting with # are comments
			// The sequence is defined by an integer in the first 10 characters of the
			// next, non-comment line, and a corresponding string of single AA code 
			// letter without line breaks
			rw.WriteLine( "%SEQUENCE FILE" );
			rw.WriteLine( "%VERSION  1.0" );

			// Now write something like ....
			//"10"
			//"AAAAAAAAAA"
			rw.WriteLine( p.Count.ToString() ); // sequence length
			rw.WriteLine( p.MonomerString );    // the sequence ...

			rw.Close();
		}	
	}
}
