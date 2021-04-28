using System;
using System.IO;
using System.Collections;
using System.Diagnostics;

using UoB.Core.FileIO.PDB;
using UoB.Core.FileIO.DSSP;
using UoB.Core.Structure;
using UoB.Core.Structure.Builder;
using UoB.Core.ForceField;

using UoB.Methodology.OriginInteraction;
using UoB.Methodology.DSSPAnalysis;

using UoB.AppLayer;
using UoB.AppLayer.Common;

namespace UoB.AppLayer
{
    class PDBDBStatgen : DSSPTaskDirectory
    {
        public PDBDBStatgen(AppLayerBase parent)
            : base("",parent.TaskDir, false)
        {
        }

        private double Range(double ang)
        {
            if (ang < -90.0)
            {
                ang += 360.0; // wrap it :-D
            }
            return ang;
        }

        public void Go()
        {
            StreamWriter omg_pro = new StreamWriter(reportDirectory.FullName + Path.DirectorySeparatorChar + "omg_pro.csv");
            StreamWriter omg_other = new StreamWriter(reportDirectory.FullName + Path.DirectorySeparatorChar + "omg_other.csv");
            ParsingFileIndex = 0; // reset IMPORTANT
            while (true)
            {
                string currentName = CurrentFile.InternalName.Substring(0, 5);
                Trace.WriteLine(currentName);

                ResidueDef[] res = CurrentFile.GetResidues();
                for (int i = 0; i < res.Length; i++)
                {
                    if (Char.ToUpper(res[i].AminoAcidID) == 'P')
                    {
                        double omg = res[i].Omega;
                        if (Math.Ceiling(omg) != 360.0 && Math.Ceiling(omg) != -999.0)
                        {
                            omg = Range(omg);
                            omg_pro.WriteLine(omg);
                        }
                    }
                    else
                    {
                        double omg = res[i].Omega;
                        if (Math.Ceiling(omg) != 360.0 && Math.Ceiling(omg) != -999.0)
                        {
                            omg = Range(omg);
                            omg_other.WriteLine(omg);
                        }
                    }
                }

                // increment conidtion
                if (ParsingFileIndex < FileCount - 1)
                {
                    ParsingFileIndex++;
                }
                else
                {
                    break;
                }
            }
            omg_pro.Close();
            omg_other.Close();

            //NOTE in DSSPFile.cs:
            //Function:
            //private bool LoopPassesFilter(SegmentDef segment)
            //You **MUST** commment out the following block...
            //if (segment[i].IsCisResidue && segment[i].AminoAcidID != 'P')
            //otherwise your cis other count will be 0

            StreamWriter collate = new StreamWriter(reportDirectory.FullName + Path.DirectorySeparatorChar + "propcis.csv");
            StreamWriter omg_Loop = new StreamWriter(reportDirectory.FullName + Path.DirectorySeparatorChar + "omg_pro_loop.csv");

            collate.WriteLine("length, total #, % loops with cis pro, % loops with cis other");

            // 8 because NativePert can only produce loops of length 8
            for (int loopLength = 1; loopLength <= 100; loopLength++)
            {
                int totalLoop = 0;
                int proCisLoop = 0;
                int otherCisLoop = 0;               

                ParsingFileIndex = 0; // reset IMPORTANT
                while (true)
                {
                    string currentName = CurrentFile.InternalName.Substring(0, 5);
                    Trace.WriteLine(currentName);

                    SegmentDef[] loops = CurrentFile.GetLoops(loopLength, false, true);
                    for (int i = 0; i < loops.Length; i++)
                    {
                        totalLoop++;
                        bool cisPro = false;
                        bool cisOther = false;
                        for (int j = 0; j < loops[i].Length; j++)
                        {
                            if (Char.ToUpper(loops[i].Sequence[j]) == 'P')
                            {
                                double omg = loops[i].GetOmega(j);
                                if (Math.Ceiling(omg) != 360.0 && Math.Ceiling(omg) != -999.0)
                                {
                                    omg = Range(omg);
                                    omg_Loop.WriteLine(omg);
                                }
                            }
                            if (loops[i].GetIsCisResidue(j))
                            {
                                if (Char.ToUpper(loops[i].Sequence[j]) == 'P')
                                {
                                    cisPro = true;
                                }
                                else
                                {
                                    cisOther = true;
                                }
                            }
                        }
                        if (cisOther) otherCisLoop++;
                        if (cisPro) proCisLoop++;
                    }

                    // increment conidtion
                    if (ParsingFileIndex < FileCount - 1)
                    {
                        ParsingFileIndex++;
                    }
                    else
                    {
                        break;
                    }
                }

                collate.WriteLine("{0},{1},{2},{3}", loopLength, totalLoop, perc(proCisLoop,totalLoop), perc(otherCisLoop,totalLoop));
            }

            collate.Close();
            omg_Loop.Close();
        }

        private double perc(int amount, int tot)
        {
            if (amount <= 0) return 0.0;
            return 100.0 * ((double)amount / (double)tot);
        }
    }

    
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class CommonTaskInvoke : AppLayerBase
	{
		public CommonTaskInvoke()
		{
		}

		public override bool RequiresForcefield
		{
			get
			{
				return true;
			}
		}

        public override string MethodPrintName
        {
            get { return "Common"; }
        }

        public override void MainStem(string[] args)
        {
            //RecursiveExportOriginToEPS( new DirectoryInfo(@"F:\_thesis\LoopModelReports\"));


            PDBDBStatgen gen = new PDBDBStatgen(this);
            gen.Go();
        }

        private void RecursiveExportOriginToEPS(DirectoryInfo di)
        {
            DirectoryInfo[] dis = di.GetDirectories();
            for (int i = 0; i < dis.Length; i++)
            {
                RecursiveExportOriginToEPS(dis[i]);
            }

            FileInfo[] files = di.GetFiles("*.opj");
            for (int i = 0; i < files.Length; i++)
            {
                ExportOrginToEPS(files[i]);
            }
        }

        private OriginInterface m_Origin = null;

        private void ExportOrginToEPS(FileInfo file)
        {
            if (m_Origin == null) m_Origin = new OriginInterface(true);
            m_Origin.LoadTemplateFile(file.FullName);
            string name = file.Directory.FullName + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(file.Name) + ".eps";
            m_Origin.SaveEPSPicture(name, "hist");
            return;
        }

        public void OldFuncs()
        {
			string path = @"C:\_CurrentWork\09d - My Loop DataBase\PDBSelectRevamp2\";
			string chainList = "_THE CHAIN LIST.txt";

			//MakeDownloadListsFromChainList( path, chainList );
			//RenameGetrightFiles( @"c:\downloads\" );
			//CheckHeaders( path + "PDB\\" );
			//CheckFooters( path + "PDB\\" );
			//CheckAllChainsPresent( path + "PDB\\", path + chainList );	
			//Filtering( path + "PDB\\" );
			CheckAndRebuild( path, path + chainList );

			//RebuildAllPDBFilesInADirectory_IF_NO_DODGY_SIDECHAINS_AND_NO_MISSING_RES_ATOMS();
		}

		private static void CheckAndRebuild( string path, string chainList )
		{
			string pdbPath = path + "Accept1.8\\";
			//string pdbPath = path + "Accept1.xxx\\";
			string ProcessedPath = path + @"ProcessedChains_1.8\";
			string ProcessedPath_Rebuild = path + @"ProcessedChains_Rebuild_1.8\";

			Directory.CreateDirectory( ProcessedPath );
			Directory.CreateDirectory( ProcessedPath_Rebuild  );

			ArrayList PDBIDAr = new ArrayList();
			ArrayList chainIDAr = new ArrayList();
			StreamReader re = new StreamReader(chainList);
			string line;
			while( null != ( line = re.ReadLine() ) )
			{
				string name = line.Substring( line.Length - 5, 4 );
				PDBIDAr.Add( name );
				char chainID = line[ line.Length - 1 ];
				if( chainID == '_' )
				{
					chainID = ' ';
				}
				chainIDAr.Add( chainID );
			}
			re.Close();

			DirectoryInfo di = new DirectoryInfo( pdbPath );
			FileInfo[] files = di.GetFiles("*.pdb");

			for( int i = 0; i < files.Length; i++ )
			{
				PDB file = new PDB(  files[i].FullName, true );
				string pdbID = Path.GetFileNameWithoutExtension(files[i].Name);

				int countTo = PDBIDAr.Count;
				for( int j = 0; j < countTo; j++ )
				{
					if( pdbID == (string) PDBIDAr[j] )
					{
						char chainID = (char) chainIDAr[j];
						PSMolContainer member = file.particleSystem.MemberWithID( chainID ) as PSMolContainer;
						if( member == null )
						{
							throw new Exception("VERY ODD ERROR, the membes is null? How?");
						}
						if( chainID == ' ' )
						{
							chainID = '_';
						}
						string name = pdbID + chainID;

						// we found the chain

						ParticleSystem psClone = new ParticleSystem( name );
						psClone.BeginEditing();
						psClone.AddMolContainer( (PSMolContainer) member.Clone() );
						psClone.EndEditing( true, true );
						PDB.SaveNew( ProcessedPath + name + ".pdb", psClone, true, file.ExtendedInformation, false );

						PS_Builder b = new PS_Builder( psClone );
						if( b.RebuildTemplate( RebuildMode.AllAtoms, false, false, true, true, true ) )
						{
							PDB.SaveNew( ProcessedPath_Rebuild + name + ".pdb", b.TemplateSystem, true, file.ExtendedInformation, false );
						}

						PDBIDAr.RemoveAt( j );
						chainIDAr.RemoveAt( j );
						j--;
						countTo--;
					}
				}
			}
		}

		private static void Filtering( string path )
		{
			string acceptPath = path + @"_Accept\";
			string unknownPath = path + @"_Unknown\";

			DirectoryInfo di = new DirectoryInfo( path );
			FileInfo[] files = di.GetFiles("*.pdb");

			for( int i = 0; i < files.Length; i++ )
			{
				PDB file = new PDB( files[i].FullName, false );
				if( file.ExtendedInformation.ExptlReslnMethod == PDBExpRslnMethod.Crystalographic 
					|| file.ExtendedInformation.ExptlReslnMethod == PDBExpRslnMethod.DAVEUnknown
					|| file.ExtendedInformation.ExptlReslnMethod == PDBExpRslnMethod.Undefined
					)
				{
					
					// commented as they were a check that didnt flag
					//					if( file.ExtendedInformation.Resolution == 999.9f || 
					//						file.ExtendedInformation.Resolution == 0.0f || 
					//						file.ExtendedInformation.Resolution == -1.0f )
					//					{
					//						File.Copy( files[i].FullName, unknownPath + files[i].Name );
					//					}
					//					else 
					if( file.ExtendedInformation.Resolution <= 1.8f )
					{
						File.Copy( files[i].FullName, acceptPath + files[i].Name );
					}
					else
					{
						continue;
					}
				}
			}

			return;
		}

		private static void Filtering()
		{
			string path = @"C:\Main Backup 21.03.05\_My Calculations\09d - My Loop DataBase\PDBSelect2004\";
			string rejectPath = path + @"_Reject\";
			string acceptPath = path + @"_Accept\";
			string unknownPath = path + @"_Unknown\";

			DirectoryInfo di = new DirectoryInfo( path + "Download" );
			FileInfo[] files = di.GetFiles("*.pdb");

			StreamWriter rw = new StreamWriter( path + "DownloadFileReject-Stage1.log", false );
			for( int i = 0; i < files.Length; i++ )
			{
				PDB file = new PDB( files[i].FullName, false );
				if( file.ExtendedInformation.ExptlReslnMethod == PDBExpRslnMethod.Undefined )
				{
					Trace.WriteLine("UNDEFINED");
					rw.WriteLine( files[i].Name + ",is unknown exp method" );
					File.Move( files[i].FullName, unknownPath + files[i].Name );
				}
				else if( file.ExtendedInformation.ExptlReslnMethod != PDBExpRslnMethod.Crystalographic )
				{
					Trace.WriteLine("REJECT");
					rw.WriteLine( files[i].Name + ",isnt Crystal strcuture" );
					File.Move( files[i].FullName, rejectPath + Path.GetFileNameWithoutExtension(files[i].Name) + ".NonCrystal.pdb" );
				}
				else if( file.ExtendedInformation.Resolution > 2.2f )
				{
					Trace.WriteLine("REJECT");
					rw.WriteLine( files[i].Name + ",isnt better than 2.2A resnl" );
					File.Move( files[i].FullName, rejectPath + Path.GetFileNameWithoutExtension(files[i].Name) + "." + file.ExtendedInformation.Resolution.ToString() + ".pdb" );
				}
				else
				{
					Trace.WriteLine("ACCEPT");
					File.Move( files[i].FullName, acceptPath + files[i].Name );
					rw.WriteLine( files[i].Name + ",Ok round 1" );
				}
			}
			rw.Close();

			return;
		}

		private static void CheckAllChainsPresent( string pdbPath, string chainList )
		{
			ArrayList PDBIDAr = new ArrayList();
			ArrayList chainIDAr = new ArrayList();
			StreamReader re = new StreamReader(chainList);
			string line;
			while( null != ( line = re.ReadLine() ) )
			{
				string name = line.Substring( line.Length - 5, 4 );
				PDBIDAr.Add( name );
				char chainID = line[ line.Length - 1 ];
				if( chainID == '_' )
				{
					chainID = ' ';
				}
				chainIDAr.Add( chainID );
			}
			re.Close();

			DirectoryInfo di = new DirectoryInfo( pdbPath );
			FileInfo[] files = di.GetFiles("*.pdb");

			for( int i = 0; i < files.Length; i++ )
			{
				try
				{
					PDB file = new PDB(  files[i].FullName, true );
					bool found = false;
					int countTo = PDBIDAr.Count;
					for( int j = 0; j < countTo; j++ )
					{
						if( Path.GetFileNameWithoutExtension(files[i].Name) == (string) PDBIDAr[j] )
						{
							if( file.particleSystem.MemberWithID( (char) chainIDAr[j] ) == null )
							{
								throw new Exception("");
							}
							found = true;
							PDBIDAr.RemoveAt( j );
							chainIDAr.RemoveAt( j );
							j--;
							countTo--;
						}
					}
					if( !found )
					{
						File.Move( pdbPath + files[i].Name, pdbPath + "\\bin\\" + files[i].Name );
					}
				}
				catch
				{
					File.Move( pdbPath + files[i].Name, pdbPath + "\\exp\\" + files[i].Name );
				}
			}

			if( PDBIDAr.Count > 0 )
			{
				throw new Exception("");
			}
		}

		private static void CheckFooters( string stem )
		{
			DirectoryInfo di = new DirectoryInfo( stem );
			FileInfo[] files = di.GetFiles("*.pdb");

			StreamWriter rw = new StreamWriter( stem + "_checkIsCorrectFile.csv", false );

			for( int i = 0; i < files.Length; i++ )
			{
				StreamReader re = new StreamReader( files[i].FullName );
				string line2 = null;
				string line1 = null;
				while( true )
				{
					string line = re.ReadLine();
					if( line == null ) break;
					line2 = line1;
					line1 = line;			
				}
				re.Close();

   
				if(  line1.Substring(0,3) != "END" )
				{
					rw.WriteLine( "Fail," + files[i].Name + ",the FOOTER isn't there!" );
					File.Move( stem + files[i].Name, stem + "\\bin\\" + files[i].Name );
				}

				rw.Flush();
			}

			rw.Close();
		}

		private static void CheckHeaders( string stem )
		{
			DirectoryInfo di = new DirectoryInfo( stem );
			FileInfo[] files = di.GetFiles("*.pdb");

			StreamWriter rw = new StreamWriter( stem + "_checkIsCorrectFile.csv", false );

			for( int i = 0; i < files.Length; i++ )
			{
				StreamReader re = new StreamReader( files[i].FullName );
				//HEADER    COMPLEX (TRANSDUCER/TRANSDUCTION)       05-DEC-97   1A0R 
				string line = re.ReadLine();
				re.Close();

				string name = Path.GetFileNameWithoutExtension( files[i].FullName );
				string checkID = line.Substring(62,4);
    
				if(  line.Substring(0,6) != "HEADER" )
				{
					rw.WriteLine( "Fail," + name + ",the HEADER isn't there!" );
					File.Move( stem + files[i].Name, stem + "\\bin\\" + files[i].Name );
				}
				else if( checkID != name )
				{
					rw.WriteLine( "Fail," + name + ",PDBIDs dont match" );
					File.Move( stem + files[i].Name, stem + "\\bin\\" + files[i].Name );
				}
				else
				{
					rw.WriteLine( "Good," + name );
				}

				rw.Flush();
			}

			rw.Close();
		}

		private static void RenameGetrightFiles( string path )
		{
			DirectoryInfo di = new DirectoryInfo( path );
			FileInfo[] files = di.GetFiles( "*.getright" );
			for( int i = 0; i < files.Length; i++ )
			{
				files[i].MoveTo( Path.GetFileNameWithoutExtension( files[i].FullName ) );
			}
		}

		private static void MakeDownloadListsFromChainList( string path, string chainList )
		{
			string u1 = @"http://www.rcsb.org/pdb/cgi/export.cgi/";
			string u2 = @".pdb?format=PDB&pdbId=";
			string u3 = @"&compression=None";

			int listNum = 0;
			StreamReader re = new StreamReader( path + chainList );
			
			string line;
			StreamWriter rw = null;
			while( true )
			{
				rw = new StreamWriter( path + "dl" + listNum.ToString() + ".lst" );
				int count = 0;
				listNum++;
				while( count < 500 )
				{
					if( null != (line = re.ReadLine() ) )
					{
						string name = line.Substring( line.Length - 5, 4 );
						rw.Write( u1 );
						rw.Write( name );
						rw.Write( u2 );
						rw.Write( name);
						rw.WriteLine( u3 );
						count++;
					}
					else
					{
						goto END;
					}
				}
				rw.Close();
				rw = null;
			}           
			END:
				if( rw != null )
				{
					rw.Close();
				}
			re.Close();
		}

		private static void RebuildAllPDBFilesInADirectory_IF_NO_DODGY_SIDECHAINS_AND_NO_MISSING_RES_ATOMS()
		{
			DirectoryInfo diIn = new DirectoryInfo( @"C:\Main Backup 21.03.05\_My Calculations\09d - My Loop DataBase\PDBSelect2004\4. 1.9 or less\2. ProcessedStructures" );
			DirectoryInfo diOut = new DirectoryInfo( @"C:\Main Backup 21.03.05\_My Calculations\09d - My Loop DataBase\PDBSelect2004\4. 1.9 or less\2a. ProcessedStructures with Hydrogens" );
			
			FileInfo[] fileNames = diIn.GetFiles( "*.pdb" );

			StreamWriter rw = new StreamWriter( diOut.FullName + Path.DirectorySeparatorChar + "output.log" );

			for( int i = 0; i < fileNames.Length; i++ )
			{
				string name = fileNames[i].Name;

				if( File.Exists( diOut.FullName + Path.DirectorySeparatorChar + name ) )
				{
					rw.WriteLine("Already Exists : " + name);
					continue; // file already exists
				}

				PDB file = new PDB( diIn.FullName + Path.DirectorySeparatorChar + name, true );
				PS_Builder build = new PS_Builder( file.particleSystem );
				if( build.RebuildTemplate( RebuildMode.AllAtoms, false, false, true, true, true ) )
				{
					PDB.SaveNew( diOut.FullName + Path.DirectorySeparatorChar + name, file.particleSystem );
				}
				else
				{
					rw.WriteLine("Fail on : " + name);
					rw.Flush();
				}
			}

			rw.Close();
		}
	}
}
