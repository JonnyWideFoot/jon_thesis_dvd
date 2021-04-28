using System;
using System.IO;
using System.Collections;
using System.Text;

namespace OneLineAlnFiles
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{

		public Class1()
		{
			DirectoryInfo current = new DirectoryInfo( Directory.GetCurrentDirectory() );
			FileInfo[] files = current.GetFiles("*.aln");
			ArrayList lines = new ArrayList();

			string line;
			for( int j = 0; j < files.Length; j++ )
			{
				string name = files[j].FullName;

				bool ok = true;

				if( name.Length > 10 )
				{
					if( name.Substring(name.Length-10,10) == ".1line.aln" )
					{
						ok = false; // its already a 1line file
					}
				}

				if( ok )
				{

					StreamReader re = new StreamReader(name);

					re.ReadLine(); // 3 header lines from clustalW
					re.ReadLine();
					re.ReadLine();

					lines.Clear();				

					while( ( line = re.ReadLine() ) != null )						  
					{
						StringBuilder sb = new StringBuilder( line );
						lines.Add( sb );
						if( sb[0] == ' ' ) // the identity line
						{
							re.ReadLine(); // read the next blank line
							break;
						}
					}

					// scan to find the start point
					int startPoint = getStart(0,lines);
					for( int k = 1; k < lines.Count - 1; k++ ) // -1 as the last one is the identity line
					{
						int next = getStart( k, lines );
						if( next != startPoint || next == -1 )
						{
							throw new Exception("Lines are not all valid/equal start points");
						}
					}

					while( true )						  
					{
						for( int k = 0; k < lines.Count; k++ )
						{
							StringBuilder sb = (StringBuilder) lines[k];
							line = re.ReadLine();
							if( line == null )
							{
								if( k == 0 )
								{
									goto ENDFILE; // OK termination
								}
								else
								{
									throw new Exception("file contains too few lines");
								}
							}
							sb.Append( line, startPoint, line.Length - startPoint);
						}
						line = re.ReadLine(); // blank boy...
						if( line == null )
						{
							goto ENDFILE; // OK termination
						}
						if( line.Length != 0 )
						{
							throw new Exception("Non 0 length blank line found");
						}
					}

					ENDFILE:
					re.Close();

					string saveTo = Path.GetFileNameWithoutExtension( name );
					saveTo += ".1line.aln";

					StreamWriter rw = new StreamWriter(saveTo);
					for( int i = 0; i < lines.Count; i++ )
					{
						StringBuilder sb = (StringBuilder) lines[i];
						rw.WriteLine( sb.ToString() );
					}
					rw.Close();
				}
			}
		}

		private int getStart( int index, ArrayList stringBuilders )
		{
			StringBuilder sb = (StringBuilder) stringBuilders[index];
			int firstSpace = -1;
			for( int i = 0; i < sb.Length; i++ )
			{
				if( sb[i] == ' ' )
				{
					firstSpace = i;
				}
			}
			if( firstSpace == -1 ) return -1; // error, no spaces found
			for( int i = firstSpace; i < sb.Length; i++ )
			{
				if( sb[i] != ' ' )
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Class1 c1 = new Class1(); // executed in constructor
		}
	}
}
