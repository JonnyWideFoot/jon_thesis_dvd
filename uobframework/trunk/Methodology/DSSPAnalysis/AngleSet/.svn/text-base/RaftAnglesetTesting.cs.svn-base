using System;
using System.IO;

using UoB.Core.FileIO.FormattedInput;
using UoB.Core.MoveSets.AngleSets;
using UoB.Core.FileIO.PDB;
using UoB.Core.Structure;

using UoB.Methodology.TaskManagement;

namespace UoB.Methodology.DSSPAnalysis.AngleSetAnalysis
{
	/// <summary>
	/// Summary description for RaftTestGeneration.
	/// </summary>
	public class RaftAnglesetTesting : DSSPTaskDirectory
	{
		ScriptInitManager m_ScriptManager;
		DirectoryInfo di_PDB = null;
		DirectoryInfo di_PDBLoop = null;
		AngleSet m_Angleset = null;

		/// <summary>
		/// Class used to look at the representation of a given angleset in the RAFT implementation
		/// RAFT is used to measure the cRMS of a given conformational descriptor
		/// </summary>
		/// <param name="di"></param>
		public RaftAnglesetTesting(  string DBName, DirectoryInfo di ) : base(  DBName, di, false )
		{
			throw new Exception( "Recoding needed to integrate the new m_ScriptManager class and to standardise for a given angleset ..." );

			// There are many functions in this class, not sure which ones are legacy code any more
			// all needs to be a bit more standard .....

//			m_ScriptManager = new ScriptInitManager( scriptGenerationDirectory, ScriptMode.ClusterMoveBackAtEnd, "?", "?", ".inp" )
//			m_Angleset = new RaftAngles();
//			di_PDB = new DirectoryInfo( di.FullName + "\\PDB\\" );
//			di_PDBLoop = new DirectoryInfo( di.FullName + "\\PDBLoop\\" );
		}

		public void AnalyseBestCRMS( int loopLength, bool nonGly )
		{
			FileInfo[] files = this.resultDirectory.GetFiles("*.out");

			string[] parseCommands = {
										"readuntilpattern,flroot",
										"deadlines,1",
										"readstring,filestem,<-ignore~10->,<-string~8->",

										"readuntilpattern,%SEQUENCE FILE",
										"deadlines,1",
										"readstring,sequenceLength,<-string~4->",
										"readstring,sequence,<-string~8->",

										"readuntilpattern,GENERATION   1",
										"deadlines,1",
										"readstring,bestconformer,<-ignore~5->,<-string~14->",

										"done"
									 };
			// extract to these
			string[]   names			=	new string[ files.Length ];
			string[]   conformers		=	new string[ files.Length ];
			string[]   nativeConformers =	new string[ files.Length ];
			string[]   sequences		=	new string[ files.Length ];
			float[]    cRMSs			=	new float[ files.Length ];
			float[]    aRMSs			=	new float[ files.Length ];

			string openName = "All8mers-cRMS.csv";
			if( nonGly )
			{
				openName = "All8mers-cRMS-nonGly.csv";
			}

			StreamWriter rw = new StreamWriter( reportDirectory.FullName + Path.DirectorySeparatorChar + openName );
			
			rw.WriteLine("Name,Sequence,NativeConformer,Conformer,BothSame,cRMS,aRMS");

			for( int i = 0; i < files.Length; i++ )
			{
				try
				{
					FileObject fo = FileScanTools.parseFile(files[i].FullName, parseCommands );
					names[i] = ((string[])fo[0].data)[0];
					names[i] = names[i].Substring(0,names[i].Length-1);
					int sequenceLength = int.Parse( ((string[])fo[1].data)[0] );
					sequences[i] = ((string[])fo[2].data)[0];
					if( sequenceLength != 8 || sequenceLength != sequences[i].Length )
					{
						throw new Exception("Sequence length does not match the sequence or the predefined job type");
					}

					bool containsGly = false;
					if( nonGly )
					{ // mark containsGly if glycine prenent
						for( int seq = 0; seq < sequences[i].Length; seq++ )
						{
							if( sequences[i][seq] == 'G' )
							{
								containsGly = true;
								break;
							}
						}
					}
					if( containsGly )
					{
						continue; // dont print in the routine ...
					}

					string bestConf = ((string[])fo[3].data)[0]; // the description of the best conformer in the .out file
					string[] parts = bestConf.Split(' ');
					cRMSs[i] = float.Parse( parts[0] );
					conformers[i] = parts[1];

					string pdbLoadPath = di_PDBLoop.FullName + Path.DirectorySeparatorChar;
					PDB file = new PDB( pdbLoadPath + names[i] + "_.pdb", true );
					PolyPeptide pp = (PolyPeptide) file.particleSystem.MemberAt(0);
					if( pp.MonomerString != sequences[i] ) throw new Exception("something is horribly wrong, sequences dont match");

					nativeConformers[i] = m_Angleset.GetBestConformerString( pp );
					aRMSs[i] = m_Angleset.CalcARMS( pp, sequences[i], conformers[i] );

					rw.Write( names[i] );
					rw.Write( ',' );
					rw.Write( sequences[i] );
					rw.Write( ',' );
					rw.Write( nativeConformers[i] );
					rw.Write( ',' );
					rw.Write( parts[1] );
					rw.Write( ',' );
					rw.Write( (nativeConformers[i] == parts[1]).ToString() );
					rw.Write( ',' );
					rw.Write( parts[0] );
					rw.Write( ',' );
					rw.WriteLine( aRMSs[i].ToString("0.00") );
				}
				catch( Exception ex )
				{
					Console.WriteLine("FAIL : " + files[i].FullName);

					string fileStem = files[i].Name.Substring(0,files[i].Name.Length-4);

					FileInfo[] nobbedFiles = this.resultDirectory.GetFiles( fileStem + '*' );

					for( int q = 0; q < nobbedFiles.Length; q++ )
					{
						nobbedFiles[q].MoveTo( this.baseDirectory.FullName + Path.DirectorySeparatorChar + "\\nobbed\\" + nobbedFiles[q].Name );
					}

					StreamWriter rwNob = new StreamWriter( this.baseDirectory.FullName + Path.DirectorySeparatorChar + "\\nobbed\\" + fileStem + ".nob" );
					rwNob.WriteLine( ex.Message );
					rwNob.WriteLine();
					rwNob.WriteLine( ex.ToString() );
					rwNob.Close();
				}
			}

			rw.Close();
		}


		public void ExhaustiveCRMSTest_StructuralTypeBin( int mer )
		{
			// for this one we use the previously generated Loop definitions and not the DSSP files
			string rootPath = di_PDB.Parent.FullName;
			DirectoryInfo diPDBLoop = new DirectoryInfo( baseDirectory.FullName + "\\PDBLoop\\" );
			FileInfo[] files = diPDBLoop.GetFiles("*.pdb");
			for( int i = 0; i < files.Length; i++ )
			{
				string fileName = files[i].Name;
				PDB.PDB loop = new PDB.PDB( files[i].FullName, true );
				// first find the closest raft angle
				RaftInputGeneration.WriteRaftExhaustiveBinRestrictionCRMSTest( 						
						rootPath, 
						fileName.Substring(0, fileName.Length - 4 ),
						(PolyPeptide)loop.particleSystem.MemberAt(0),
						mer
					);
			}

			RaftInputGeneration.WriteStartScript( rootPath, ScriptMode.StandAlone );
		}
		
		public void ExhaustiveCRMSTest( int mer )
		{
			ParsingFileIndex = 0; // reset IMPORTANT

			string rootPath = di_PDB.Parent.FullName;

			while( true )
			{
				// per-file task
				LoopDef[] loops = CurrentFile.GetLoops( 8 );
				string name = Path.GetFileNameWithoutExtension( CurrentFile.FileName );
				for( int i = 0; i < loops.Length; i++ )
				{
					RaftInputGeneration.WriteRaftExhaustiveCRMSTest( 
						rootPath, 
						name.Substring(0, name.Length - 4 ) + "_" + i.ToString(), 
                        name,
						loops[i].Sequence,
						loops[i].FirstResidueIndex, 
						loops[i].FirstResidueInsertionCode,
						loops[i].Length  
						);
				}
				// end task

				// increment conidtion
				if( ParsingFileIndex < FileCount - 1 )
				{
					ParsingFileIndex++;
				}
				else
				{
					break;
				}
			}

			RaftInputGeneration.WriteStartScript( rootPath, ScriptMode.ClusterMoveBackPerJob );
		}
	}
}
