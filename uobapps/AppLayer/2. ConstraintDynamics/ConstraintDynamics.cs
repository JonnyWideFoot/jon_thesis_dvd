using System;
using System.IO;
using System.Collections;

using UoB.Research;
using UoB.Research.Data;
using UoB.Research.FileIO.Tra;
using UoB.Research.FileIO.PDB;
using UoB.Research.FileIO.FormattedInput;
using UoB.Research.Modelling.Structure;
using UoB.Research.Modelling.ConstraintDynamics;
using UoB.Research.Modelling.ForceField;

namespace ConstraintDynamics
{
	class ConstraintDynamics : TaskDirectoryInteration
	{
		private DirectoryInfo m_TemplateDir; 	

		[STAThread]
		static void Main(string[] args)
		{
			// Directory tree to use
					
			// run type 1, legacy
//			string path = @"G:\08b - Constraint Dynamics\2005_07_02 - Minim 2";
//			float[] epsilons = new float[] { 0.1f,	0.2f,	0.3f,	0.5f,	0.6f,	0.7f,	0.80f,	1.0f,	5.0f,	10.0f, 20.0f, 30.0f, 50.0f };
//			float[] wellWidths = new float[] { 0.0f, 0.5f,	1.0f,	1.5f,	3.0f };
						
			string path = @"G:\08b - Constraint Dynamics\2005_07_04 - Dyn 4";
			float[] epsilons = new float[] { 0.15f, 0.25f, 0.35f, 0.55f, 0.65f, 0.75f, 0.85f, 1.25f, 1.5f, 5.5f, 7.5f, 10.5f, 12.5f, 18.5f, 22.5f, 35.5f, 45.0f };
			float[] wellWidths = new float[] { 0.0f, 0.25f, 0.5f, 0.75f, 1.0f, 1.5f, 2.0f, 2.5f, 3.0f };

			// Initialise class
			DirectoryInfo di = new DirectoryInfo(path);
			ConstraintDynamics cd = new ConstraintDynamics(di);



			// Do what we have come here to do .....
			//7cd.GenerateFiles( epsilons, wellWidths );
			cd.endCRMSAnalysis( epsilons, wellWidths );

			// done!
		}

		public ConstraintDynamics( DirectoryInfo di ) : base( di )
		{
			FFManager ff = FFManager.Instance;
			// important prequel on first initialisation
			ff.FinaliseStage2();

			m_TemplateDir = new DirectoryInfo( di.FullName + "\\_Template\\"  );
		}

		private void PDBGenerationFortempateTra()
		{
			//			PDB filea = new PDB( root + "ext-aa.pdb", true );
			//			filea.particleSystem.PhysicallyReCenterSystem();
			//			PDB.SaveNew( root + "ext-aa-centre.pdb", filea.particleSystem );

			//			PDB file = new PDB( root + "__brought together.pdb", true );
			//			//file.particleSystem.PhysicallyReCenterSystem();
			//			//PDB.SaveNew( root + "dock-cys-daverebuild-centre-v2.pdb", file.particleSystem );
			//
			//			ParticleSystem ps = file.particleSystem;
			//			PSMolContainer mol = ps.MemberAt(0);
			//			for( int i = 0; i < mol.Count; i++ )
			//			{
			//				mol[i].ResidueNumber -= 2; // counting problem for PD
			//			}
		}

		private void endCRMSAnalysis( float[] epsilons, float[] wellWidths )
		{
			StreamWriter rw = new StreamWriter( reportDirectory.FullName + "endCRMSAnalysis.csv" );

			rw.WriteLine( "Analysis for : " + reportDirectory.FullName );
			rw.WriteLine();

			ResultGrid rg = new ResultGrid( epsilons, wellWidths );
						
			foreach( FileInfo fi in resultDirectory.GetFiles("*.tra") )
			{
				string[] names;
				float[] floats;
				ParseFilenameForParams( fi.Name, 5, 2, out names, out floats );

				Tra file = new Tra(fi.FullName);
				file.TraPreview.SetToImport_FirstAndLastOnly();
				file.LoadTrajectory();

				DataListing[] data = file.DataStore.DataListings;
				for( int i = 0; i < data.Length; i++ )
				{
					if( data[i].Name == "cRMS" )
					{
						float dataValue = data[i].Data[data[i].Data.Length-1];
						rw.Write( floats[0] );
						rw.Write(',');
						rw.Write( floats[1] );
						rw.Write(',');
						rw.WriteLine( dataValue );
						rg.SetCellValue( floats, dataValue );
						break;
					}
				}
			}

			rw.WriteLine(); // blanking line

			float[] nullMarkers = new float[] { -1.0f, -1.0f };
			rg.Print2DCSV( rw, nullMarkers, 0, 1 );

			rw.Close();

			return;
		}

		private void AnalyseFiles()
		{
			//			int x = 0;
			//			int y = 0;
			//			int z = 0;
			//			for( float epsilonFactor = 0.01f; epsilonFactor < 0.7f; epsilonFactor *= 2.0f )// upto 0.64
			//			{
			//				x++;
			//			}
			//			for( float gammaFactor = 0.1f; gammaFactor < 0.6f; gammaFactor += 0.1f )
			//			{
			//				y++;
			//			}
			//			for( float wellWidthFactor = 0.0f; wellWidthFactor <= 3.0f; wellWidthFactor += 0.5f )
			//			{
			//				z++;
			//			}
			//			int maxX = x;
			//			int maxY = y;
			//			int maxZ = z;
			//
			//			string fileRoot = @"c:\_jonpd\";
			//			float[,,] dataAve = new float[x,y,z];
			//			float[,,] dataSD  = new float[x,y,z];
			//			float[,,] dataDRMS = new float[x,y,z];
			//
			//			x = 0;
			//			for( float epsilonFactor = 0.01f; epsilonFactor < 0.7f; epsilonFactor *= 2.0f )// upto 0.64
			//			{
			//				y = 0;
			//				for( float gammaFactor = 0.1f; gammaFactor < 0.6f; gammaFactor += 0.1f )
			//				{
			//					z = 0;
			//					for( float wellWidthFactor = 0.0f; wellWidthFactor <= 3.0f; wellWidthFactor += 0.5f )
			//					{
			//						string filename = epsilonFactor.ToString("0.00") + "-" + gammaFactor.ToString("0.00") + "-" + wellWidthFactor.ToString("0.00");
			//						Tra file = new Tra( fileRoot + filename + ".tra" );
			//						file.LoadTrajectory();
			//
			//						ConstraintList conList = new ConstraintList(file.particleSystem);
			//						conList.LoadFromFile( fileRoot + filename + ".con", true );
			//
			//						TraMonitorManager m = conList.GetTraMonitor( file );
			//
			//						dataDRMS[x,y,z] = m.GetDataDRMSFromMonitors( -1 );
			//						dataAve[x,y,z] = m.GetDataAverageFromMonitors( -1 );
			//						dataSD[x,y,z] = m.GetDataSDFromMonitors( -1 );
			//						z++;
			//					}
			//					y++;
			//				}
			//				x++;
			//			}
			//
			//			for( int i = 0; i < maxZ; i++ )
			//			{
			//				WriteDataTable( fileRoot + @"Look@\" + "ResultTableAVE" + ".csv", dataAve );
			//				WriteDataTable( fileRoot + @"Look@\" + "ResultTableSDv" + ".csv", dataSD );
			//				WriteDataTable( fileRoot + @"Look@\" + "ResultTableDRMS" + ".csv", dataDRMS );
			//			} 





			//			string root = @"C:\__MainRunFolder\old\Copy of _JonPD\020205\";
			//			//260105-0.tra
			//
			//			for( int i = 0; i < 1; i++ )
			//			{
			//
			//				Tra file = new Tra( root + i.ToString() + ".tra" );
			//				file.TraPreview.skipLength = 1; // gotta catch 'em all !
			//				file.LoadTrajectory(); // get the position definitions
			//
			//				ConstraintList conList = new ConstraintList(file.particleSystem);
			//				conList.LoadFromFile( @"C:\__MainRunFolder\old\Copy of _JonPD\020205\mini4.con", true );
			//
			//				TraMonitorManager m = conList.GetTraMonitor( file );
			//
			//				m.WriteToFile( 0,-1, root + i.ToString(), DataOutputType.XMLExcel );
			//
			//			}






			//			string trapath = @"C:\Copy of _JonPD\240105-0.tra";
			//			string conpath = @"C:\Copy of _JonPD\0.64-0.10-0.00.con";
			//
			//			Tra file = new Tra( trapath );
			//			file.TraPreview.skipLength = 1; // gotta catch 'em all !
			//			file.LoadTrajectory(); // get the position definitions
			//
			//			ConstraintList conList = new ConstraintList(file.particleSystem);
			//			conList.LoadFromFile( conpath, true );
			//
			//			TraMonitorManager m = conList.GetTraMonitor( file );
			//
			//			m.WriteToFile(0,-1,@"c:\bob2.csv");
			//
			//			return;
		}

		private void GenerateFiles( float[] epsilons, float[] wellWidths )
		{	
			string pathStem = resultDirectory.FullName;
			string templatePath = m_TemplateDir.FullName;
			string commonStem = pathStem + @"..\..\_Common\";

			// config details
			string[] confParamsKeys = new string[] { "inputtra", "outfilestem", "confile" };
			string[] confParams = new string[] { "?", "?", "?" }; // to be filled per file
			string[] infiles = new string[] { "IG_Cryst_Oldff.tra" };
			string primaryInfile = infiles[0];
			string[] templateFilenames = new string[] { "dyn.templ" };
			ConstrainingMode[] modes = new ConstrainingMode[] { ConstrainingMode.UberAbsolute };
			
			Tra primaryTra = new Tra(commonStem + primaryInfile);
			// file.LoadTrajectory() ... no need, as we want entry 0
			StreamWriter jobList = new StreamWriter( pathStem + "jobs.bat" );
			ConstraintList conList = new ConstraintList();
			
			for( int ep = 0; ep < epsilons.Length; ep++ ) // 10 confile strengths
			{
				for( int ww = 0; ww < wellWidths.Length; ww++ ) // 10 confile strengths
				{
					for( int jj = 0; jj < modes.Length; jj++ )
					{
						// this represents each con file definition ...
						conList.ResetAll();
						conList.SetGlobalRestraintsAs( primaryTra.particleSystem, modes[jj], true, epsilons[ep], 0.4f, wellWidths[ww] );
					
						string conStem = 
							Enum.GetName( typeof(ConstrainingMode), modes[jj] ) 
							+ "_" + epsilons[ep].ToString("0.00") 
							+ "_" + wellWidths[ww].ToString("0.00");
						string conName = conStem + ".con";

						conList.WriteToFile( pathStem + conName );				

						for( int q = 0; q < infiles.Length; q++ ) // do 1st so only need to load the TRA once
						{
							for( int kk = 0; kk < templateFilenames.Length; kk++ )
							{
								string mainFileStem = Path.GetFileNameWithoutExtension(infiles[q]) + "_" + Path.GetFileNameWithoutExtension(templateFilenames[kk]) + "_" + conStem;

								confParams[0] = @"..\..\_Common/" + infiles[q];
								confParams[1] = mainFileStem;
								confParams[2] = conName;

								InputFile.Create( templatePath + templateFilenames[kk], pathStem + mainFileStem + ".conf", confParamsKeys, confParams );
								jobList.WriteLine( @"start /D.\ /B /BELOWNORMAL ..\..\_PDExec\dynamics5.exe -infile " + mainFileStem + ".conf" );
								jobList.WriteLine( "wscript.exe ..\\..\\_common\\WaitForExe.vbs \"dynamics5.exe\"" );
								jobList.WriteLine();
							}					
						}
					}
				}
			}
							
			jobList.Close();

			return;
		}

		private void GenerateFilesLegacyCode()
		{
			//			PDB file = new PDB( root + "__brought togetherA.pdb", true );
			//			ParticleSystem ps = file.particleSystem;
			//			PSMolContainer mol = ps.MemberAt(0);
			//
			//			for( int i = 0; i < mol.Count; i++ )
			//			{
			//				mol[i].ResidueNumber -= 2; // counting problem for PD
			//			}
			//			ConstraintList con = new ConstraintList();
			//			con.SetGlobalRestraintsAs( file.particleSystem, ConstrainingMode.BackbonePlusBetaCarbon, false, 50.0f, 1.0f, 0.0f ); 
			//			con.WriteToFile(root + "reletive-sep-20.0-A.con");
			//
			//			file = new PDB( root + "__brought togetherB.pdb", true );
			//			ps = file.particleSystem;
			//			mol = ps.MemberAt(0);
			//
			//			for( int i = 0; i < mol.Count; i++ )
			//			{
			//				mol[i].ResidueNumber -= 2; // counting problem for PD
			//			}
			//			con = new ConstraintList();
			//			con.SetGlobalRestraintsAs( file.particleSystem, ConstrainingMode.BackbonePlusBetaCarbon, false, 50.0f, 1.0f, 0.0f ); 
			//			con.WriteToFile(root + "reletive-sep-20.0-B.con");
			//			
			//			return;

			//			//PDB bob = new PDB(@"C:\_Dyn\_Common\IG_Extd_Oldff.pdb",true);
			//			//bob.particleSystem.PhysicallyReCenterSystem();
			//			//PDB.SaveNew(@"C:\_Dyn\_Common\IG_CenterExtd_Oldff.pdb",bob.particleSystem);
			//			
			//
			//			// Paths
			//			string pathStem = @"C:\_Dyn\14.04.05\";
			//			string commonStem = @"C:\_Dyn\_Common\";
			//			string templatePath = pathStem + "_Template\\";
			//
			//			// config details
			//			string[] confParamsKeys = new string[] { "inputtrastem", "outfilestem", "confile" };
			//			string[] confParams = new string[] { "?", "?", "?" }; // to be filled per file
			//			string[] infiles = new string[] { "IG_CenterExtd_Oldff.tra", "IG_Extd_Oldff.tra", "IG_Cryst_Oldff.tra" };
			//			string primaryInfile = "aliDyn.tra"; //infiles[0]; // they are both the same in this special case where one is the crystal
			//			string[] templateFilenames = new string[] { "newMDCG.templ", "noMD.templ" };
			//			ConstrainingMode[] modes = new ConstrainingMode[] { ConstrainingMode.UberAbsolute };
			//			
			//			Tra primaryTra = new Tra(commonStem + primaryInfile);
			//			// file.LoadTrajectory() ... no need, as we want entry 0 from these
			//			StreamWriter jobList = new StreamWriter( pathStem + "jobs.bat" );
			//			ConstraintList conList = new ConstraintList();
			//			float gammaInitial = 3.0f;
			//			float gammaIncrement = 1.0f;
			//			float gamma = gammaInitial;		
			//
			//			for( int i = 0; i < 7; i++ ) // 10 confile strengths
			//			{
			//				for( int jj = 0; jj < modes.Length; jj++ )
			//				{
			//					// this represents each con file definition ...
			//					conList.ResetAll();
			//					conList.SetGlobalRestraintsAs( primaryTra.particleSystem, modes[jj], true, gamma, 0.4f, 1.0f );
			//					string conStem = Enum.GetName( typeof(ConstrainingMode), modes[jj] ) + "_" + gamma.ToString("0.00");
			//					string conName = conStem + ".con";
			//					conList.WriteToFile( pathStem + conName );				
			//
			//					for( int q = 0; q < infiles.Length; q++ ) // do 1st so only need to load the TRA once
			//					{
			//						for( int kk = 0; kk < templateFilenames.Length; kk++ )
			//						{
			//							string mainFileStem = Path.GetFileNameWithoutExtension(infiles[q]) + "_" + Path.GetFileNameWithoutExtension(templateFilenames[kk]) + "_" + conStem;
			//
			//							confParams[0] = "../_Common/" + infiles[q];
			//							confParams[1] = mainFileStem;
			//							confParams[2] = conName;
			//
			//							InputFile.Create( templatePath + templateFilenames[kk], pathStem + mainFileStem + ".conf", confParamsKeys, confParams );
			//							jobList.WriteLine( @"start /D.\ /B /BELOWNORMAL ..\_PDExec\dynamics5.exe -infile " + mainFileStem + ".conf" );
			//							jobList.WriteLine( "wscript.exe ..\\_common\\WaitForExe.vbs \"dynamics5.exe\"" );
			//							jobList.WriteLine();
			//						}					
			//					}
			//				}
			//				gamma += gammaIncrement;
			//			}
			//							
			//			jobList.Close();
			//
			//			//InputFile.RandomiseInputFileLineOrder( pathStem + "jobs.bat", pathStem + "jobs_RandomOrder.bat" );
			//
			//			return;

		}

	}
}
