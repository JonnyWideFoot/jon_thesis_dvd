using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using UoB.Research.FileIO.PDB;
using UoB.Research;
using UoB.Research.FileIO.Tra;
using UoB.Research.Modelling.Structure;
using UoB.Research.Modelling.Builder;
using UoB.Research.Tools;

namespace UoB.CoSMoS
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class CosmosMain
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/// 

		private static string m_TitleBlock = @"
	_____            _____       ___  ___           _____
	/  ___|         /  ___/     /   |/   |         /  ___/
	| |      _____  | |___     / /|   /| |  _____  | |___ 
	| |     /  _  \ \___  \   / / |__/ | | /  _  \ \___  \
	| |___  | |_| |  ___| |  / /       | | | |_| |  ___| |
	\_____| \_____/ /_____/ /_/        |_| \_____/ /_____/



			Comparative
			o
			Systematic
			Modelling
			o
			System

			Copyright Jon Rea 2004
			University of Bristol


Welcome to ...CoSMoS !
CoSMoS is the command-promt interface to the UoB Comparative Homology Modelling Suite. 
CoSMos is completely written in C# using the .NET framework (http://www.microsoft.com/). 
This program can be run under Mac OS X, Redhat Linux and SuSE Linux using the MONO 
OpenSource .net framework (http://www.go-mono.com/). Enjoy ....
";

		private static string m_HelpBlock = "\n\n-input fileName\n(synonym -i)\npath = fileName to .tra or .pdb file\n\n-output fileName\n(synonym -i)\nfileName = the desired output fileName\ndefault = <systemname>.model.pdb.pdb\n\n-traindex number\n(synonym -t)\nnumber = integer index of the required tra-entry\nDEFAULT : 0 (Target structure definition)\n\n-rebuild \"type\"\n(synonym -r)\nWhere type is : DEFAULT : allatoms, polar, polarandaromatic, heavyonly\n\n-align path\n(synonym -a)\npath = filepath to .ali file\n\n";

		private Tra m_Tra = null;
		private PDB m_PDB = null;
		private const string argError = "Error Status : "; // argError pre-string
		private const string logFileName = "cosmos.log";
		private PS_Builder m_Builder;
		private StreamWriter rw;

		// Params

		private CosmosMain()
		{
			init();

			Report(m_TitleBlock);
		}

		private bool reportWriteHasFailedOnce = false;
		private void Report( string s )
		{
			Console.WriteLine(s);
			if( reportWriteHasFailedOnce == false && rw != null )
			{
				try
				{
                    rw.WriteLine(s);
				}
				catch
				{
					reportWriteHasFailedOnce = true;
				}
			}
		}

		private void PrintHelp()
		{
            Console.WriteLine(m_HelpBlock);			
		}

		private void init()
		{
			try
			{
				rw = new StreamWriter(logFileName, false);
			}
			catch
			{
				Console.WriteLine("LogFile could not be opened - no output will be recorded");
			}
			m_Builder = new PS_Builder();
		}

		public void Run()
		{
			Report("Initiating modelling run ...");
			if( !m_InputIsSet )
			{
				Report( "Error : An input file must be provided" );
				return;
			}
			if( !File.Exists( m_InputFile ) )
			{
				Report( "ERROR : The input filename was invalid" );
				return;
			}

			FileInfo fi = new FileInfo( m_InputFile );
			string extenstion = fi.Extension.ToLower();

			if( extenstion == "tra" )
			{
				Report("Performing preliminary tra file load...");
				m_Tra = new Tra( m_InputFile );
				if( m_TraIndexIsSet ) // if not then no need to load the tra, we are just using the sysdefs
				{
					Report("Loading tra definitions");
					m_Tra.LoadTrajectory();

					if( m_TraIndex < m_Tra.TraPreview.numberOfEntries && m_TraIndex >= 0 )
					{
						Report("Setting Tra Position");
						m_Tra.PositionDefinitions.Position = m_TraIndex;
					}
					else
					{
						Report("Error : The traIndex provided is invalid for this tra file. Index given : " + m_TraIndex.ToString() + " Number of entries in the tra file : " + m_Tra.TraPreview.numberOfEntries.ToString() );
						return;
					}
				}
				if ( m_Tra == null )
				{
					Report("Error : Trajectory is null\nFilename used was : " + m_InputFile);
					return;
				}
						   
				m_Builder.TemplateSystem = m_Tra.particleSystem;				
			}
			else if ( extenstion == "pdb" )
			{
				Report("Opening PDB file ... ");
				m_PDB = new PDB( m_InputFile, true);
                if ( m_PDB == null )
				{
					Report("Error : PDB file is null");
					return;
				}
				m_Builder.TemplateSystem = m_PDB.particleSystem;
			}

			if( m_Builder.TemplateSystem == null )
			{
                Report( "ERROR : The Template of the builder was null\nFilename used was : " + m_InputFile );
				return;
			}

			if( !m_RebuildModeIsSet )
			{
				Report("Using the default all atom mode for template system rebuild");
			}
			Report("Initiating Template Rebuid...");
			m_Builder.RebuildTemplate( m_RebuildMode, false );

			if( m_AliIsSet )
			{
				Report("NO IMPLEMENTATION : Currently ignoring all alignment info");
				return;
			}

			string templateDump = m_Builder.TemplateSystem.Name + ".template.pdb";
			Report("Dumping rebuilt template used in the process...");
			PDB.SaveNew( templateDump, m_Builder.TemplateSystem );

			Report("Entering model build");
			m_Builder.BuildModel( false );

			if( !m_OutputIsSet )
			{
				string filename = m_Builder.TemplateSystem.Name + ".model.pdb";
				Report("No output file was specified, therefore the default will be used : " + filename);
				m_OutputFile = filename;
			}

			Report("Dumping built model...");
			PDB.SaveNew( m_OutputFile, m_Builder.ModelSystem );
						
			rw.Close(); // close the output file when the run is done to release the file handle

			Report("Model Build Sucessful!");
			Report("Thank you for using CoSMoS...");			
		}

		private string m_InputFile = "";
		private bool m_InputIsSet = false;
		public string InputFile
		{
			get
			{
				return m_InputFile;
			}
			set
			{
				m_InputFile = value;
				m_InputIsSet = true;
			}
		}

		private string m_OutputFile = "";
		private bool m_OutputIsSet = false;
		public string OutputFile
		{
			get
			{
				return m_OutputFile;
			}
			set
			{
				m_OutputFile = value;
				m_OutputIsSet = true;
			}
		}

		private string m_AliFile = "";
		private bool m_AliIsSet = false;
		public string AliFile
		{
			get
			{
				return m_AliFile;
			}
			set
			{
				m_AliFile = value;
				m_AliIsSet = true;
			}
		}

		private RebuildMode m_RebuildMode = RebuildMode.AllAtoms;
		private bool m_RebuildModeIsSet = false;
		public RebuildMode rebuildMode
		{
			get
			{
				return m_RebuildMode;
			}
			set
			{
				m_RebuildMode = value;
				m_RebuildModeIsSet = true;
			}
		}

		private int m_TraIndex = -1;
		private bool m_TraIndexIsSet = false;
		public int TraIndex
		{
			get
			{
				return m_TraIndex;
			}
			set
			{
				m_TraIndex = value;
				m_TraIndexIsSet = true;
			}
		}

		[STAThread]
		static void Main(string[] args)
		{
			try
			{
				CosmosMain application = new CosmosMain(); // initialisation will talk program output headers

				// now set its params for the run

				// we need some args
				if( args.Length == 0 )
				{
					application.PrintHelp();
					return;
				}
				if( args.Length % 2 != 0 )
				{
					application.PrintHelp();
					Console.WriteLine( "The number of arguments must be even, therefore there is an error in the input line");
					return;
				}

				// which args have we got, tell cosmos about it ...
				for( int i = 0; i < args.Length; i+=2 ) // +2 : only the even ones, these are the switches
				{
					if( args[i].ToLower() == "-i" || args[i].ToLower() == "-input" )
					{
						application.InputFile = args[i+1];
					}
					else if( args[i].ToLower() == "-a" || args[i].ToLower() == "-align" )
					{
						application.AliFile = args[i+1];
					}
					else if( args[i].ToLower() == "-o" || args[i].ToLower() == "-output" )
					{
						application.OutputFile = args[i+1];
					}
					else if( args[i].ToLower() == "-r" || args[i].ToLower() == "-rebuild" )
					{
						switch( args[i+1].ToLower() )
						{
							case "allatoms":
								application.rebuildMode = RebuildMode.AllAtoms;
								break;
							case "polar":
								application.rebuildMode = RebuildMode.PolarHydrogens;
								break;
							case "polarandaromatic":
								application.rebuildMode = RebuildMode.PolarAndAromatic;
								break;
							case "heavyonly":
								application.rebuildMode = RebuildMode.HeavyAtomsOnly;
								break;
							default:
								Console.WriteLine( "Unknown rebuild type was entered : " + args[i+1] + "\nThe following are allowed : allatoms, polar, polarandaromatic, heavyonly.\nInput is case insensitive" );
								return; // error, return
						}
						 
					}
					else if( args[i].ToLower() == "-t" || args[i].ToLower() == "-traindex" )
					{
						try
						{
							application.TraIndex = int.Parse( args[i+1] );
						}
						catch
						{
							Console.WriteLine("The traIndex supplied was not a valid integer" );
						}
					}
					else
					{
						Console.WriteLine("Input parse error : the following switch is invalid : " + args[i].ToString() );
						return;
					}
				}

				application.Run();
			}
			catch(Exception e)
			{
				Console.WriteLine( e.ToString() );
			}
		}
	}
}
