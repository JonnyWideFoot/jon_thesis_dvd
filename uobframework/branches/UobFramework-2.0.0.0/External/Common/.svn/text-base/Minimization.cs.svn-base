using System;
using System.IO;
using System.Diagnostics;

using UoB.Core;
using UoB.Core.Structure;
using UoB.Core.FileIO.FormattedInput;
using UoB.Core.FileIO.Tra;
using UoB.Core.FileIO.PDB;
using UoB.Core.Structure.Builder;

namespace UoB.External.Common
{
	/// <summary>
	/// Summary description for Minimization.
	/// </summary>
	public sealed class Minimization : OutSource
	{
		private ParticleSystem m_ParticleSystem;
		private CoreIni m_CoreIni = CoreIni.Instance;
		private int m_Length = 1000000;
		private const int m_DumpStepping = 400;
		private char m_ChainID;
		private TraProgressWatcher m_Progress = null;
		private string m_MinimOutputTra = null;

		private string inputFileStem = null;
		private string outputFileStem = null;
		private string confName = null;
		private string filePath = null;
		private const string nameStem = "Minim";

		public Minimization( ParticleSystem ps, char chainID )
		{
			m_ParticleSystem = ps;
			m_ChainID = chainID;
			//filePath = m_UoBInit.DefaultSharedPath + "interop/dynamics/"; 
			filePath = m_CoreIni.DefaultSharedPath + "interop/pdinterop/"; // the older version

			if( m_ParticleSystem.MemberWithID( m_ChainID ) == null )
			{
				throw new InteropException("The chainID specified for the ParticleSystem refers to a molecule not present in the system");
			}

			m_Process = new Process();
		}

		public int Length
		{
			get
			{
				return m_Length;
			}
			set
			{
				if( value > 0 )
				{
					m_Length = value;
				}
			}
		}

		private void proc_Exited(object sender, EventArgs e)
		{
			Process proc = (Process) sender;
			Trace.WriteLine("PD has completed, ExitCode : " + proc.ExitCode.ToString() );
			CallFileDone(  m_MinimOutputTra );
			Trace.WriteLine( "Process has ended");
		}

		public override void Kill()
		{
			base.Kill();
			CleanUpFiles();
		}

		private void CleanUpFiles()
		{
			// try to delete all the files, boo-hoo if we cant ... lots of empty catch statements
			try
			{
				File.Delete( filePath + confName );
			}
			catch
			{
			}
			try
			{
				File.Delete( filePath + inputFileStem + ".tra" );
			}
			catch
			{
			}
			try
			{
				DirectoryInfo di = new DirectoryInfo( filePath );
				FileInfo[] fi = di.GetFiles();
				for( int i = 0; i < fi.Length; i++ )
				{
					if( fi[i].Name == outputFileStem )
					{
						try
						{
							fi[i].Delete();
						}
						catch
						{
						}
					}
				}
			}
			catch
			{
			}
		}

		public override float ProgressPercentage
		{
			get
			{
				if( m_Progress == null )
				{
					int numberOfEntries = m_Length / m_DumpStepping;
					m_Progress = new TraProgressWatcher( m_MinimOutputTra, numberOfEntries );
				}
				return m_Progress.Progress;
			}
		}

		public override void Start()
		{
			int counter = 0;
			while(true)	// first we need to decide on a filename and a filestem for the output
			{
				inputFileStem = nameStem + "Input_" + counter.ToString("000");
				confName = nameStem + "Input_" + counter.ToString("000") + ".conf";
				outputFileStem = nameStem + "Output_" + counter.ToString("000");
				counter++;
				if( File.Exists( filePath + inputFileStem + ".tra") ) continue;
				if( File.Exists( filePath + confName ) ) continue;
				if( File.Exists( filePath + outputFileStem  + ".tra" ) ) continue;
                break; // we now have a non-taken name
			}

			// Generate the required input file for the minimisation
			//<%InputFile%>
			//<%OutputsStem%>
			//<%StepNumber%>

			Trace.WriteLine("Building the required input files for PD interop");

			InputFile.Create( filePath + "template/" + "minimize.tconf", filePath + inputFileStem + ".conf",
				new string[] { "InputFile", "OutputsStem", "StepNumber", "DumpStep" },
				new string[] { inputFileStem + ".tra", outputFileStem, m_Length.ToString(), m_DumpStepping.ToString() } );

			m_MinimOutputTra = filePath + outputFileStem + ".tra";

			DateTime date = DateTime.Now; // set the reported time at this point
			try
			{
				ParticleSystem pClone = (ParticleSystem) m_ParticleSystem.Clone();
				PS_Builder builder = new PS_Builder( pClone );
				builder.RebuildTemplate( RebuildMode.AllAtoms, false, false, false, true, true );
				builder.TemplateSystem.SortMemberAtoms(); // this also reindexes

				try
				{
					TraContents contents = TraContents.Positions;
					contents = contents | TraContents.Impropers;
					//contents = contents | TraContents.REBUILDP; // aparently we dont actually need this

					TraSaveInfo info = new TraSaveInfo(filePath + inputFileStem + ".tra", contents );
					info.Descriptor = "Minimisation Job : " + m_ParticleSystem.Name + "\r\nInvoked on : " + date.ToShortDateString() + " at " + date.ToShortTimeString();

					Tra.SaveNew( builder.TemplateSystem, m_ChainID, info );
				}
				catch( TraException ex )
				{
					throw new InteropException( "There was an error writing the job file\r\nBecause :" + ex.ToString() );
				}

			}
			catch( BuilderException ex )
			{
				throw new InteropException( "There was an error whilst creating the particle system\r\nBecause :" + ex.ToString() );
			}

			// cool, we have our tra file ...

			Trace.WriteLine("Building complete, preparing to launch PD.");

			string appFileName = filePath + "dynamics.exe";
			string args = " -infile " + confName; // confName contains ".conf"

			m_Process.EnableRaisingEvents = true;
			m_Process.StartInfo = new ProcessStartInfo( appFileName, args );
			
			#if DEBUG
				// If im in debug mode i want to see the window and output its contents to a file
				m_Process.StartInfo.Arguments += "> DebugOutput.log";
			#else
				m_Process.StartInfo.CreateNoWindow = true; // we dont need to show the user Dynamics.exe screen output
			#endif
			
			m_Process.Exited += new EventHandler( proc_Exited );
			Directory.SetCurrentDirectory( filePath );
			m_Process.Start();
		}

	}
}
