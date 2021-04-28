using System;
using System.IO;
using System.Collections;

using UoB.Research.Modelling.Structure;
using UoB.Research.FileIO.PDB;
using UoB.Research.Modelling.Alignment;
using UoB.Research.Modelling.Detection;
using UoB.Research.Primitives;

namespace UoB.Aligner
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{

		private string path;
		private string pdbPath;

		private String[] fileNamesFromListFile( string fileName, string fileRoot )
		{
			StreamReader re = new StreamReader( fileName );
			ArrayList fileNames = new ArrayList();
			string line;
			while( null != ( line = re.ReadLine() ) )
			{
				fileNames.Add( fileRoot + line + ".pdb" );
			}
			re.Close();
			return (string[]) fileNames.ToArray( typeof( string ) );
		}

		public Class1( int seed )
		{
			path = @"g:\___UberAlign\";
			//pdbPath = path + @"PDB\";
			pdbPath = @"g:\_Gen Ig Database 04.10.04\newOut\";
			string jobName = @"seed1\";
			string outDir = path + jobName;
			if( !Directory.Exists( outDir ) )
			{
				Directory.CreateDirectory( outDir );
			}

			// subFam# 02.11.04.list is the string format to retrieve
			
			string[] listFiles = Directory.GetFiles(path,"*.list");
			object[] fileLists = new object[ listFiles.Length ];
			for( int i = 0; i < fileLists.Length; i++ )
			{
				fileLists[i] = fileNamesFromListFile( listFiles[i], pdbPath );
			}

			string[] fileNames = (string[]) fileLists[seed];

			string saveTo = outDir + "subfam" + seed.ToString() + ".align";

			if( File.Exists( saveTo ) )
			{
				Console.WriteLine("File present, so overwriting ...");
				AlignFile.ClearReportFile( saveTo );
			}		

			int total = 0;
//			for( int i = 0; i < fileNames.Length; i++ )
//			{
//				for( int j = i + 1; j < fileNames.Length; j++ )
//				{
//					total++;
//				}
//			}
			for( int k = 2; k < fileLists.Length; k++ )
			{
				string[] subFamFilenames = (string[]) fileLists[k];
				if( seed != k )
				{
					for( int i = 0; i < fileNames.Length; i++ )
					{
						for( int j = 0; j < subFamFilenames.Length; j++ )
						{
							total++;
						}
					}
				}
			}

			int done = 0;
			PSAlignManager alig = new PSAlignManager( AlignmentMethod.ProSup, 130 );

//			for( int i = 0; i < fileNames.Length; i++ )
//			{
//				// for each PDB file ..
//				PDB pdb1 = new PDB( fileNames[i], true );
//				MolRange molRange1 = new MolRange(pdb1.particleSystem.MemberAt(0));
//				AlignSourceDefinition sourceDef1 = new AlignSourceDefinition( pdb1.FullFilePath, molRange1 );
//				// against all in the same sub-family
//				for( int j = i + 1; j < fileNames.Length; j++ )
//				{
//					PDB pdb2 = new PDB( fileNames[j], true );
//					MolRange molRange2 = new MolRange(pdb2.particleSystem.MemberAt(0));
//					AlignSourceDefinition sourceDef2 = new AlignSourceDefinition( pdb2.FullFilePath, molRange2 );
//
//					string name = pdb1.Name + " vs " + pdb2.Name;
//					alig.ResetPSMolContainers( name, sourceDef1, sourceDef2 );
//					alig.PerformAlignment();
//					AlignFile.SaveReport( saveTo, alig.SystemDefinition, true, AlignSaveParams.AlignReport | AlignSaveParams.PSInfoBlock | AlignSaveParams.ModelsDefined, alig.CurrentOptionSet );
//					Console.WriteLine( done.ToString().PadLeft(5,' ') + @"/" + total.ToString().PadLeft(5,' ') + " Done. For : " + fileNames[i] + " vs " + fileNames[j] );
//					done++;
//				}
//			}

			// then against all other members of all other sub-families
			for( int k = 2; k < fileLists.Length; k++ )
			{
				if( seed != k )
				{
					saveTo = outDir + @"\sufam" + seed.ToString() + "vs" + "subfam" + k.ToString() + ".align";
					int alreadyDone = 0;
					if( File.Exists( saveTo ) )
					{
						Console.WriteLine("File present, so scanning for count..");
						alreadyDone = getDoneCount( saveTo );
					}

					string[] subFamFilenames = (string[]) fileLists[k];

					for( int i = 0; i < fileNames.Length; i++ )
					{
						PDB pdb1 = null;
						MolRange molRange1 = null;
						AlignSourceDefinition sourceDef1 = null;
						for( int j = 0; j < subFamFilenames.Length; j++ )
						{
							if( j == 0 )
							{
								// init PS1
								pdb1 = new PDB( fileNames[i], true );
								molRange1 = new MolRange(pdb1.particleSystem.MemberAt(0));
								sourceDef1 = new AlignSourceDefinition( pdb1.FullFilePath, molRange1 );
							}
							if( done < alreadyDone )
							{
								Console.WriteLine( done.ToString() + " already present, skip.." );
								done++;
								continue;
							}
							PDB pdb2 = new PDB( subFamFilenames[j], true );
							MolRange molRange2 = new MolRange(pdb2.particleSystem.MemberAt(0));
							AlignSourceDefinition sourceDef2 = new AlignSourceDefinition( pdb2.FullFilePath, molRange2 );

							string name = pdb1.Name + " vs " + pdb2.Name;
							alig.ResetPSMolContainers( name, sourceDef1, sourceDef2 );
							try
							{
								alig.PerformAlignment();
								AlignFile.SaveReport( saveTo, alig.SystemDefinition, true, AlignSaveParams.AlignReport | AlignSaveParams.PSInfoBlock | AlignSaveParams.ModelsDefined, alig.CurrentOptionSet );
								Console.WriteLine( done.ToString().PadLeft(5,' ') + @"/" + total.ToString().PadLeft(5,' ') + " Done. For : " + fileNames[i] + " vs " + subFamFilenames[j] );
								done++;
							}
							catch
							{
								Console.WriteLine("Ciritcial Alignment Failure");
								StreamWriter rw = new StreamWriter( outDir + "failList.txt", true );
								rw.WriteLine(done.ToString().PadLeft(5,' ') + @"/" + total.ToString().PadLeft(5,' ') + " Failed. For : " + fileNames[i] + " vs " + subFamFilenames[j] );
								rw.Close();
								done++;
							}							
						}
					}
				}
			}            
		}

		private int getDoneCount( string fileName )
		{
			int foundStart = 0;
			int foundEnd = 0;
			StreamReader re = new StreamReader(fileName);
			string line;
			while( null != ( line = re.ReadLine() ) )
			{
				if( line.Length < 14 ) continue;
				if( "ENDALIGNREPORT" == line.Substring(0,14) )
				{
					foundEnd++;
				}
				else if( line.Length >= 16 && "BEGINALIGNREPORT" == line.Substring(0,16) )
				{
					foundStart++;
				}
			}
			if( foundEnd != foundStart )
			{
				throw new Exception();
			}
			return foundStart;
		}


		[STAThread]
		static void Main(string[] args)
		{
			int seed;
			try
			{
				seed = int.Parse( args[0] );
			}
			catch
			{
				Console.WriteLine("Give me a seedID bitch!");
				return;
			}
			Class1 c = new Class1( seed );
		}
	}
}
