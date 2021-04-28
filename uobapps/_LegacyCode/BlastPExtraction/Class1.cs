using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

using UoB.Research.FileIO.FormattedInput;
using UoB.Research.Modelling.Alignment.Tools;
using UoB.Research.Modelling.Structure;
using UoB.Research.FileIO.PDB;
using UoB.Research.Modelling.Alignment;
using UoB.Research.Modelling.Detection;
using UoB.Research.Primitives;
using UoB.Research.Dendrogram;
using UoB.Research.Modelling.Builder;

namespace ConsoleTester
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/// 

		private Class1()
		{
			
			BlastPFile fff = new BlastPFile( @"C:\_Gen Ig Database 04.10.04\_Sequence analysis\Seqres derived sequences\all_02.11.04.nofilter.blastp" );
            
			StreamWriter rw = new StreamWriter( @"c:\out.bob" );

			string path = @"C:\_Gen Ig Database 04.10.04\newOut\";
			//string id1 = "1b88A";
			//string id2 = "1c12A";
			//string id1 = "1b88A";
			//string id2 = "1cf8l";
			string id1 = "1hzhk";
			string id2 = "1cicb";

			guessID( path, ref id1 );
			guessID( path, ref id2 );


			rw.Write( fff.getAlignmentFor( id1, id2 ) );
			rw.Write( fff.getAlignmentFor( id2, id1 ) );

			rw.Close();

			string fileWrite = @"c:\bob.align";
			//AlignFile.WriteFile( fileWrite, path, id1 + ".pdb", id2 + ".pdb" );

			Process p = new Process();
			p.StartInfo.FileName = fileWrite;
			p.Start();

			return;

		}

		private bool guessID( string path, ref string name )
		{
			DirectoryInfo di = new DirectoryInfo( path );
			FileInfo[] files = di.GetFiles( "*.pdb" );
			ArrayList matches = new ArrayList();
			for( int i = 0; i < files.Length; i++ )
			{
				string pdbID = files[i].Name.Substring(0,4).ToUpper();
				string queryID = name.Substring(0,4).ToUpper();
				if( pdbID == queryID )
				{
					// if length is 5, then we test by chainID too
					if( name.Length == 5 )
					{
						if( char.ToUpper(files[i].Name[5]) == char.ToUpper(name[4]) )
						{
							matches.Add( files[i] );
						}
					}
					else
					{
						matches.Add( files[i] );
					}
				}																   
			}
			if( matches.Count < 0 )
			{
				Console.WriteLine( "Fail in finding any vaid matches for : " + name );
				name = "";
				return false;
			}
			else if( matches.Count == 1 )
			{
				Console.WriteLine( "one match found for ID : " + name );
				name = ((FileInfo)matches[0]).Name;
				name = name.Substring(0,name.Length-4);
				Console.WriteLine( "Matched : " + name );
				return true;
			}
			else
			{
				while( true )
				{
					Console.WriteLine("Found Entries ...");
					for( int j = 0; j < matches.Count; j++ )
					{
						FileInfo fi = (FileInfo) matches[j];
						Console.WriteLine( j.ToString() + ") " + fi.Name );
					}
					Console.WriteLine("Select an entry number : ");
					string s = Console.ReadLine();
					int a;
					try
					{
						a = int.Parse( s );
					}
					catch
					{
						Console.WriteLine("Not a valid integer...");
						continue;
					}
					if( a >= 0 && a < matches.Count )
					{
						name = ((FileInfo)matches[a]).Name;
						name = name.Substring(0,name.Length-4);
						Console.WriteLine( "Matched : " + name );
						return true;
					}
					else
					{
						Console.WriteLine("Number not in the range given ...");
						continue;
					}
				}				
			}
		}

		[STAThread]
		static void Main(string[] args)
		{
			Class1 a = new Class1();
		}
	}
}
