using System;
using System.IO;
using System.Text;

using UoB.Core.FileIO;

namespace UoB.Core.FileIO.FormattedInput
{
	/// <summary>
	/// Summary description for AlnFile.
	/// </summary>
	public class BlastPFile
	{
		private FileInfo m_File;

		public BlastPFile( string fileName )
		{
			m_File = new FileInfo( fileName );
		}

		public string getAlignmentFor( string id1, string id2 )
		{
			StreamReader re = new StreamReader( m_File.FullName );
			StringBuilder b = new StringBuilder();

			string line;
			bool found = false;
			while( null != ( line = re.ReadLine() ) )
			{
				string get1 = "Query= " + id1;
				if( line.Length < get1.Length ) continue;
				if( line.Substring(0,get1.Length) == get1 )
				{
					found = true;
					break;
					// we found our startPoint
				}
			}
			if( !found )
			{
				return "ID1 could not be found in the given file, aborting...";
			}
			b.Append( line + "\r\n\r\n" );
			found = false;
			// now try to find ID2
			while( null != ( line = re.ReadLine() ) )
			{
				string get1Check = "Query= ";
				if( line.Length < get1Check.Length ) continue;
				if( line.Substring(0,get1Check.Length) == get1Check )
				{
					found = false;
					break;
					// we have reached a new query start, this means the sequences in question were not aligned
				}

				string get2 = '>' + id2;
				if( line.Length < get2.Length ) continue;
				if( line.Substring(0,get2.Length) == get2 )
				{
					found = true;
					break;
					// we found our startPoint
				}
			}
			if( !found )
			{
				return "ID2 could not be found in the given file, aborting...";
			}
			b.Append( line + "\r\n" );
			while( null != ( line = re.ReadLine() ) )
			{
				if( line.Length != 0 )
				{
					if( line[0] == '>' ) break; // the next aligned item that we dont want
				}
				b.Append( line + "\r\n" );
			}
			re.Close();

			return b.ToString();
		}
	}
}
