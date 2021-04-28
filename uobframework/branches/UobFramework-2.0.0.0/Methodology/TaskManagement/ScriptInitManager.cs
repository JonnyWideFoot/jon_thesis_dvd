using System;
using System.IO;

namespace UoB.Methodology.TaskManagement
{
	/// <summary>
	/// Summary description for ScriptInitManager.
	/// </summary>
	public class ScriptInitManager
	{
		private string m_NodeList = "t2 t3 t4 t32 t18 t19 t20 t21 t22 t23 t25 t26 t27 t28 t29 t30 t31"; // t24 is dodgy
		private string m_SubmittingTo = "/home/jr0407/";
		private string m_SubmittingFrom = "/cyan/jr0407/scriptgen/";
		private string m_ReturnTo = "/cyan/jr0407/output/";
		
		private ScriptMode m_Mode;
        private DirectoryInfo m_OutputDirectory;
		private string m_InvokePreString;
		private string m_InvokePostString;
		private string m_InputExtension;

		public ScriptInitManager( 
			DirectoryInfo outputDirectory,
			ScriptMode mode, 
			string invokePreString, 
			string invokePoststring,
			string inputExtension )
		{
			m_Mode = mode;
			m_OutputDirectory = outputDirectory; 
			m_InvokePreString = invokePreString;
			m_InvokePostString = invokePoststring;
			m_InputExtension = inputExtension;
		}

		#region Startup Scripting
		private void SSHAndSetupNodeProcessor( StreamWriter rw, int proc )
		{
			rw.WriteLine( "\techo Running launchNode.csh on $t processor " + proc.ToString() );
			rw.Write( "\tssh $t \"" );

			rw.Write( "cd " );
			rw.Write( m_SubmittingFrom );
			rw.Write( "; " );

			rw.Write( "csh launchNode.csh $t proc" );
			rw.Write( proc.ToString() );
			rw.Write( "; " );

			rw.WriteLine( "&\" & " );
			rw.WriteLine( "\tsleep 10 ");
		}

		public void WriteJobFiles()
		{
			if( m_Mode == ScriptMode.StandAlone )
			{
				//"Untested Code below"
				StreamWriter rw = new StreamWriter( m_OutputDirectory + "backgroundStartRunProgram.csh");
				rw.WriteLine( "csh runprogram.csh &" );
				rw.Close();
			
				rw = new StreamWriter( m_OutputDirectory + "runprogram.csh");

				rw.Write( "foreach file (*." );
				rw.Write( m_InputExtension );
				rw.WriteLine( ")" );
				rw.Write( "\tset newf = `basename $file ." );
				rw.Write( m_InputExtension );
				rw.WriteLine( '`' );
				rw.WriteLine( "\techo starting $file" );
				rw.WriteLine( '\t' );
				rw.Write( m_InvokePreString );
				rw.Write( "$file" );
				rw.Write( m_InvokePostString );
				rw.WriteLine( " 2>& $newf.out" );
				rw.WriteLine( "\techo ending $file" );
				rw.WriteLine( "end" );
			
				rw.Close();
			}
			else
			{
				// Blanking file to mark jobs as submitted to a node ...
				StreamWriter rw = new StreamWriter( m_OutputDirectory + "_blank" );

					rw.WriteLine("a");
					rw.Close();

					// run from /home/jr0407/scriptgen/ on local disk, takes the name as arg1
					rw = new StreamWriter( m_OutputDirectory + "runprogram.csh");
					rw.Write( m_InvokePreString );
					rw.Write( "$1" );
					rw.Write( '.' );
					rw.Write( m_InputExtension );
					rw.Write( m_InvokePostString );
					rw.WriteLine( " 2>& $1.out" );

				rw.Close();

				// run from /cyan/jr0407/scriptgen/ on network disk
				rw = new StreamWriter( m_OutputDirectory + "launchAll.csh" );

					rw.Write( "foreach t ( " );
					rw.Write( m_NodeList );
					rw.WriteLine(" ) ");

					SSHAndSetupNodeProcessor(rw,1);
					//launch 2nd time as we have 2 processors
					SSHAndSetupNodeProcessor(rw,2);

					rw.WriteLine( "end ");

				rw.Close();

				// run from /cyan/jr0407/scriptgen/ on network disk
				rw = new StreamWriter( m_OutputDirectory + "launchNode.csh");

					rw.WriteLine( "echo Entering the launch job loop on $1 ");
					rw.WriteLine();

					rw.WriteLine();

					if( m_Mode == ScriptMode.ClusterMoveBackPerJob )
					{
						// do all job specific the files
						rw.Write( "foreach file ( *." );
						rw.Write( m_InputExtension );
						rw.WriteLine( " ) ");
						rw.WriteLine();
						rw.WriteLine( "\tset name = $file:r ");
						rw.WriteLine( "\tif ( -e $name.flag ) then "); // if the file is flagged as taken, dont do it again ;-)
						rw.WriteLine( "\t\t#do nothing, the file is running or has run ... ");
						rw.WriteLine( "\telse ");
						rw.WriteLine();
						rw.WriteLine( "\t\techo initialising $name on $1 ");
						
						rw.WriteLine( "\t\techo init $name on $1 - begining to copy files" );

						rw.WriteLine( "\t\tcp _blank $name.flag" ); // in scriptgen on cyan mark the file as taken
						rw.Write( "\t\tcp runprogram.csh " );
						rw.Write( m_SubmittingTo );
						rw.WriteLine( "$2/" ); 
						rw.Write( "\t\tcp $name.* " );
						rw.Write( m_SubmittingTo );
						rw.WriteLine( "$2/" ); 

						rw.Write( "\t\tcd " );
						rw.Write( m_SubmittingTo );
						rw.WriteLine( "$2/" ); 
						rw.WriteLine( "\t\tcsh runprogram.csh $name" ); // run with that dir as the working directory, i hope
			    
						rw.WriteLine( "\t\techo done $name on $1 - begining to move files back" );
						rw.Write( "\t\tmv $name* " );
						rw.WriteLine( m_ReturnTo );

						rw.Write( "\t\tcd " );
						rw.WriteLine( m_SubmittingFrom );

						rw.WriteLine();
						rw.WriteLine( "\tendif ");
						rw.WriteLine();
						rw.WriteLine( "end ");
					}
					else if( m_Mode == ScriptMode.ClusterMoveBackAtEnd )
					{
						// do all job specific the files
						rw.WriteLine( "foreach file ( *.inp ) ");
						rw.WriteLine();
						rw.WriteLine( "\tset name = $file:r ");
						rw.WriteLine( "\tif ( -e $name.flag ) then "); // if the file is flagged as taken, dont do it again ;-)
						rw.WriteLine( "\t\t#do nothing, the file is running or has run ... ");
						rw.WriteLine( "\telse ");
						rw.WriteLine();
						rw.WriteLine( "\t\techo initialising $name on $1 ");
						
						rw.WriteLine( "\t\techo init $name on $1 - copy files for current job" );

						rw.WriteLine( "\t\tcp _blank $name.flag" ); // in scriptgen on cyan mark the file as taken
						rw.WriteLine( "\t\tcp runprogram.csh /home/jr0407/$2/" ); 

						rw.WriteLine( "\t\tcp $name.inp /home/jr0407/$2/" ); 
						rw.WriteLine( "\t\tcp $name.seq /home/jr0407/$2/" ); 
						rw.WriteLine( "\t\tcp $name.emc /home/jr0407/$2/" ); 
						rw.WriteLine( "\t\tcp $name.init.cnf /home/jr0407/$2/" ); 
						rw.WriteLine( "\t\tcp ../PDBLoop/$name.pdb /home/jr0407/$2/" ); 

						rw.WriteLine( "\t\tcd /home/jr0407/$2/" ); 
						rw.WriteLine( "\t\tcsh runprogram.csh $name" ); // run with that dir as the working directory, i hope
						rw.WriteLine( "\t\tcd /cyan/jr0407/_autogen/" );

						rw.WriteLine();
						rw.WriteLine( "\tendif ");
						rw.WriteLine();
						rw.WriteLine( "end ");

						rw.Close();


						rw = new StreamWriter( m_OutputDirectory + "bringBack.csh");

						rw.Write( "foreach t ( ");
						rw.Write( m_NodeList );
						rw.WriteLine( " ) ");
						
						rw.Write( "\tssh $t \"" );
					
						rw.Write( "cd /home/jr0407/proc1/; " );
						rw.Write( "mv * /cyan/jr0407/output/; " );

						rw.Write( "cd /home/jr0407/proc2/; " );
						rw.WriteLine( "mv * /cyan/jr0407/output/;\"" );
	 
						rw.WriteLine( "end ");
					}
					else
					{
						throw new Exception("Uncoded script mode");
					}

				rw.Close();
			}
		}

		public static void WriteStartScripts( string dirPath, string[] jobStems )
		{
			if( dirPath[dirPath.Length-1] != '\\' )
			{
				dirPath = dirPath + "\\_autogen\\";
			}
			else 
			{
				dirPath = dirPath + "_autogen\\";
			}

			StreamWriter rw = new StreamWriter( dirPath + "go.csh");
			rw.WriteLine( "dos2unix *" );
			rw.WriteLine( "csh run_raft.csh &" );
			rw.Close();

			rw = new StreamWriter( dirPath + "run_raft.csh" );

			for( int i = 0; i < jobStems.Length; i++ )
			{
				rw.Write( "/mem/jr0407/Raft/Src/raftonf <");
				rw.Write( jobStems[i] );
				rw.Write( ".inp >" );
				rw.Write( jobStems[i] );
				rw.WriteLine(".out");			
			}

			rw.WriteLine( "rm *.tmp"); // clean the directory
			rw.Close();
		}
		#endregion
	}
}
