using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using UoB.Core.FileIO.DSSP;
using UoB.Methodology.DSSPAnalysis;

namespace UoB.AppLayer.LoopBuilderStage1
{
	/// <summary>
	/// DSSPLoopDefFileGeneration: A simple class designed to produce a loop list for the testing process 
    /// of the loop building layer currently being implemented in PD. The list is actually a .bat file that will
    /// call c++test.exe with the correct parameters to assess a given loop with a given angleset.
	/// </summary>
	public class DSSPLoopDefFileGeneration : DSSPTaskDirectory
	{
		private string m_ExecFilename = "c++test.exe";
		private string m_AppLanuchPath;
		private string m_CurrentTraFilePath;
		private long m_TotalLoopCount;

        //static DirectoryInfo Test_Dir = new DirectoryInfo(@"G:\Loop_Join_Dist_Test\_8Mer\");
        //static DirectoryInfo Test_Dir = new DirectoryInfo(@"G:\Loop_Join_Dist_Test\_8Mer - UpAngSet\");
        static DirectoryInfo Test_Dir = new DirectoryInfo(@"y:\Loop_TM_Test\_8Mer");

		public DSSPLoopDefFileGeneration( string DBName, DirectoryInfo di )
			: base( DBName, di, false )
		{
			m_AppLanuchPath = 
				//"../_Exec"
				//+ Path.AltDirectorySeparatorChar
				//+ 
				m_ExecFilename;
		}

        public void ResultsLoopCalibrationTest_Stage2()
        {
            FileInfo[] files_csv = Test_Dir.GetFiles("out_*.csv");

            List<double> strengths = new List<double>();

            List<double> percJoined = new List<double>();
            List<int>    percJoinedCount = new List<int>();

            List<int>    dataAddedCount = new List<int>();
            List<double> percStalled = new List<double>();
            List<double> percUber = new List<double>();

            List<int>    averageEndEneCount = new List<int>();
            List<double> averageEndEne_Tot = new List<double>(); // of those that completed
            List<double> averageEndEne_VDW = new List<double>(); // of those that completed
            List<double> averageEndEne_ReJ = new List<double>(); // of those that completed

            for( double i = 100.0f; i < 25000.0f; i += 1000.0f )
            {
                strengths.Add(i);

                percJoined.Add(0.0f);
                percJoinedCount.Add(0);

                dataAddedCount.Add(0);
                percStalled.Add(0.0f);
                percUber.Add(0.0f);

                averageEndEneCount.Add(0);
                averageEndEne_Tot.Add(0.0f);
                averageEndEne_VDW.Add(0.0f);
                averageEndEne_ReJ.Add(0.0f);
            }

            for (int i = 0; i < files_csv.Length; i++)
            {
                // --- Stage1 ------------------------------------------------------------------
                // Find filenames, verify they exist, find which strength we are refering to...
                // -----------------------------------------------------------------------------

                if (i != 0) Trace.Write(i.ToString("\b\b\b\b\b\b\b\b\b\b\b"));
                Trace.Write(i.ToString().PadLeft(4));
                Trace.Write(" / ");
                Trace.Write(files_csv.Length.ToString());

                int totalMinimisations = 0, eneMinimisations = 0, numStalled = 0, numUberHigh = 0;

                string outFileName = String.Concat(files_csv[i].Directory.FullName, Path.DirectorySeparatorChar,
                    Path.GetFileNameWithoutExtension(files_csv[i].Name), ".out");
                if (!File.Exists(outFileName))
                {
                    throw new Exception(); // incomplete file lists
                }

                string[] nameParts = files_csv[i].Name.Split('_');
                string stregnthS;
                if (nameParts[1].Length == 4) // grrrr '_' in the filename
                {
                    stregnthS = nameParts[5].Substring(0, nameParts[5].Length - 4);
                }
                else
                {
                    stregnthS = nameParts[4].Substring(0, nameParts[4].Length - 4);
                }
                double strength = double.Parse(stregnthS);
                int strengthIndex = (int)((strength - 100.0f) / 1000.0f); // in the List<double> above



                // --- Stage2 ------------------------------------------------------------------
                // Retrieve info from the CSV file.
                // -----------------------------------------------------------------------------

                StreamReader re = new StreamReader(files_csv[i].FullName);
                string line = re.ReadLine(); // dont need data
                line = re.ReadLine(); // dont need data
                line = re.ReadLine(); // dont need data
                line = re.ReadLine(); // need this ... parse it....
                string[] lineParts = line.Split(',');
                if (0 != String.Compare(lineParts[0], "Joined Perc:")) throw new Exception();
                percJoined[strengthIndex] += double.Parse(lineParts[1]);
                percJoinedCount[strengthIndex]++;
                re.Close();



                // --- Stage3A ------------------------------------------------------------------
                // Verify that our out file is valid
                // -----------------------------------------------------------------------------

                re = new StreamReader(outFileName);

                // get to the start of the minimisation block
                bool foundPlace = false;
                while (null != (line = re.ReadLine()))
                {
                    if (0 == String.Compare(line, "ConformerBuilder Initialisation completed successfully!"))
                    {
                        foundPlace = true;
                        break;
                    }
                }
                if (!foundPlace)
                    throw new Exception("Invalid out file!");
                re.ReadLine(); // blanking line ...


                // --- Stage3B ------------------------------------------------------------------
                // Retrieve info from the OUT file.
                // -----------------------------------------------------------------------------

                int count;
                double totEne, distEne,vdwEne;
                bool normalExit = false;
                double avgFinalEne = 0.0f;
                string peekLine;
                int peekCount = -1;
                peekLine = re.ReadLine();
                while (true)
                {
                    line = peekLine;
                    peekLine = re.ReadLine();
                    if (peekLine == null) throw new Exception();
                    if (0 == String.Compare(peekLine, "Neighbor list overflow - increasing neighborlist"))
                    {
                        peekLine = re.ReadLine();
                        if (peekLine == null) throw new Exception();
                    }

                    if (peekLine.Length == 0 && 0 == String.Compare("Analysis Complete in", 0, line, 0, 20))
                    {
                        normalExit = true;
                        break; // There are no minimisations at all!!
                    }
                    else if (0 == String.Compare("Analysis Complete in", 0, peekLine, 0, 20))
                    {
                        normalExit = true;
                        peekCount = -1; // null
                    }
                    else
                    {
                        peekCount = int.Parse(peekLine.Substring(0, 8)); // we now "know" this to be ok
                    }

                    lineParts = Core.Tools.CommonTools.WhiteSpaceRegex.Split(line);
                    count = int.Parse(lineParts[1]);
                    totEne = double.Parse(lineParts[6]);
                    distEne = double.Parse(lineParts[12]);
                    vdwEne = double.Parse(lineParts[8]);

                    if (peekCount == 0 || normalExit == true) // either the next line is the start of a new minimisation, or this is the final minimisation!
                    {
                        if (totEne > 150000)
                        {
                            numUberHigh++;
                        }
                        // either we are at the end and it is 79 which means success or it has stalled!
                        else if (distEne > 1500.0f) // the test for "stalled", i.e. incomplete and bad join energy
                        {
                            numStalled++;
                        }
                        else
                        {
                            avgFinalEne += totEne;
                            eneMinimisations++;
                        }
                        totalMinimisations++;
                    }

                    if (normalExit)
                    {
                        break; // we need to break the loop "Analysis Complete in" was found
                    }
                }

                if (!normalExit) throw new Exception("Invalid out file!");

                if (totalMinimisations > 0)
                {
                    // calc stats
                    dataAddedCount[strengthIndex]++;
                    if (eneMinimisations > 0)
                        averageEndEne_Tot[strengthIndex] += avgFinalEne / (double)eneMinimisations;
                    percUber[strengthIndex] += 100.0f * ((double)numUberHigh / (double)totalMinimisations);
                    percStalled[strengthIndex] += 100.0f * ((double)numStalled / (double)totalMinimisations);
                }

                // --- Stage3C ------------------------------------------------------------------
                // Verify that our out file is valid
                // -----------------------------------------------------------------------------

                bool jobDoneOK = false;
                while (null != (line = re.ReadLine()))
                {
                    if (0 == String.Compare(line, "Execution Result --> No errors were detected during execution. Loop generation successful!"))
                    {
                        jobDoneOK = true;
                        break;
                    }
                }
                if (!jobDoneOK)
                    throw new Exception("Invalid out file!");

                re.Close();
            }


            // --- Stage4 ------------------------------------------------------------------
            // Print our obtained data!
            // -----------------------------------------------------------------------------


            StreamWriter rw = new StreamWriter(Test_Dir.FullName + Path.DirectorySeparatorChar + "_TestResult.csv");
            rw.WriteLine("Strength,%Joined,%Stalled,%UberEnergy,AveEndEne.Tot,AveEndEne.Tot,AveEndEne.Tot");
            for(int i = 0; i < strengths.Count; i++)
            {
                double div = (double)percJoinedCount[i];
                percJoined[i] /= div;

                div = (double)dataAddedCount[i];
                percStalled[i] /= div;
                percUber[i] /= div;

                div = (double)averageEndEneCount[i];
                averageEndEne_Tot[i] /= div;
                averageEndEne_VDW[i] /= div;
                averageEndEne_ReJ[i] /= div;

                rw.Write(strengths[i]);
                rw.Write(',');
                rw.Write(percJoined[i]);
                rw.Write(',');
                rw.Write(percStalled[i]);
                rw.Write(',');
                rw.Write(percUber[i]);
                rw.Write(',');
                rw.Write(averageEndEne_Tot[i]);
                rw.Write(',');
                rw.Write(averageEndEne_VDW[i]);
                rw.Write(',');
                rw.WriteLine(averageEndEne_ReJ[i]);
            }
            rw.Close();
        }

        public void ResultsLoopCalibrationTestC()
        {
            FileInfo[] files_csv = Test_Dir.GetFiles("out_*.csv");

            string line = null;

            StringBuilder[] printLines = new StringBuilder[50];
            for (int j = 0; j < printLines.Length; j++)
            {
                printLines[j] = new StringBuilder();
            }
    
            for (int i = 0; i < files_csv.Length; i++)
            {
                StreamReader re = new StreamReader(files_csv[i].FullName);
                line = re.ReadLine(); // blanking line

                int q = 0;
                while (null != (line = re.ReadLine()) )
                {
                    string[] lineParts = line.Split(',');

                    if( i != 0 ) printLines[q].Append(',');
                    printLines[q].Append(lineParts[3]);
                    printLines[q].Append(',');
                    printLines[q].Append(lineParts[7]);

                    q++;
                }

                re.Close();
            }

            StreamWriter resfile = new StreamWriter(Test_Dir.FullName + Path.DirectorySeparatorChar + "_TestC_Result.csv");
            for( int j = 0; j < printLines.Length; j++ )
            {
                resfile.WriteLine(printLines[j].ToString());
            }
            resfile.Close();
        }

        public void ResultsLoopCalibrationTestB()
        {
            // find the loop join dist required : "separation" : for at least one native included

            //DirectoryInfo di = new DirectoryInfo(@"G:\Loop_Join_Dist_Test\_8Mer - UpAngSet\");
            FileInfo[] files_csv = Test_Dir.GetFiles("out_*.csv");

            UoB.Core.Data.DataStore_WithColStats_GeneralUse ds = new UoB.Core.Data.DataStore_WithColStats_GeneralUse(
                "LoopCalibrationTest", UoB.Core.Data.StatModes.None);
            ds.BeginEditing();

            ds.MainTable.Columns.Add("Anchor-Separation", typeof(float));
            ds.MainTable.Columns.Add("Propensity Sort List %", typeof(float));
            ds.MainTable.Columns.Add("Separation Cutoff Required", typeof(float));
            ds.MainTable.Columns.Add("ExhCntAtSepReq", typeof(ulong));
            ds.MainTable.Columns.Add("ExhPercAtSepReq", typeof(float));

            for (int i = 0; i < files_csv.Length; i++)
            {
                System.Data.DataRow dr = ds.MainTable.NewRow();

                StreamReader re = new StreamReader(files_csv[i].FullName);
                string line = re.ReadLine();
                string[] lineParts = line.Split(',');

                dr[1] = float.Parse(lineParts[0]);
                dr[2] = float.Parse(lineParts[1]);
                while (true)
                {
                    line = re.ReadLine();
                    lineParts = line.Split(',');
                    int num = int.Parse(lineParts[1]);
                    if (num > 0)
                    {
                        dr[3] = float.Parse(lineParts[0]);
                        dr[4] = ulong.Parse(lineParts[5]);
                        dr[5] = float.Parse(lineParts[7]);
                        break;
                    }
                }

                re.Close();
                ds.MainTable.Rows.Add(dr);
            }

            ds.EndEditing();
            ds.SaveMainTo(
                string.Concat(Test_Dir.FullName, Path.DirectorySeparatorChar, "_TestB_Result.csv")
                , UoB.Core.Data.DataOutputType.CSV);
        }

        public void WriteLoopCalibrationTest_Stage2(int[] loopLengths, bool fileCheck)
        {
            for (int iLoop = 0; iLoop < loopLengths.Length; iLoop++)
            {
                string outFile = scriptGenerationDirectory.FullName + "loop_" + loopLengths[iLoop].ToString() + ".que";
                StreamWriter rw = new StreamWriter(outFile, false);

                // write preamble

                rw.WriteLine("universe = vanilla");
                rw.WriteLine("should_transfer_files = YES");
                rw.WriteLine("WhenToTransferOutput = ON_EXIT_OR_EVICT");

                bool LINUXSUPPORT = true;
                if (LINUXSUPPORT)
                {
                    rw.WriteLine("Requirements = ( ((OpSys == \"WINNT50\") || (OpSys == \"WINNT51\") || (OpSys == \"LINUX\")) && ((Arch == \"INTEL\") || (Arch == \"x86_64\")) && (KFlops > 200000))");
                    rw.WriteLine("Executable = executable = loopbuilder.$$(OpSys).exe");
                }
                else
                {
                    rw.WriteLine("REQUIREMENTS = ((OpSys == \"WINNT50\") || (OpSys == \"WINNT51\"))");
                    rw.WriteLine("Executable = loopbuilder.exe");
                }

                rw.WriteLine("Error = condor_stder.log");
                rw.WriteLine("Output = condor_stdout.log");
                rw.WriteLine("Log = condor_job.log");

                // write jobs ...

                ParsingFileIndex = 0; // reset IMPORTANT

                m_TotalLoopCount = 0;
                int traFilesFound = 0;

                while (true)
                {
                    // determine if the current DSSP file has a valid tra file in the culled set

                    m_CurrentTraFilePath = "../tra/"
                        + Path.GetFileNameWithoutExtension(CurrentFile.fileInfo.Name)
                        + ".tra";

                    if (CurrentFile.ResidueCount <= 200 && File.Exists(scriptGenerationDirectory.FullName + m_CurrentTraFilePath))
                    {
                        Trace.WriteLine(String.Format("Parsing DSSP file for tra file ({0}): {1}", traFilesFound, m_CurrentTraFilePath));
                        traFilesFound++;

                        SegmentDef[] loops = CurrentFile.GetLoops(loopLengths[iLoop], false, true); // get the current loop set

                        for (int j = 0; j < loops.Length; j++)
                        {
                            string file = CurrentFile.fileInfo.Name;
                            file = Path.GetFileNameWithoutExtension(file);
                            file += ".tra";

                            rw.WriteLine();
                            rw.Write("transfer_input_files=amber03ua.ff,dict.pdb,loopcalibration.conf,Optimal_Loop_6-5-8.angleset,../tra/");
                            rw.WriteLine(file);

                            for (float strength = 100.0f; strength < 25000.0f; strength += 1000.0f)
                            {
                                string jobID = String.Concat(file, "_", loops[j].FirstDSSPIndex, "_", loops[j].Length, "_", strength.ToString("0"));

                                string findStem = String.Concat(scriptGenerationDirectory.Parent.FullName, "\\_", loopLengths[iLoop].ToString(), "Mer\\");
                                if (fileCheck)
                                {
                                    string outFilename = String.Concat(findStem, "out_", jobID, ".out");
                                    if (File.Exists(String.Concat(findStem, "out_", jobID, ".tra")) &&
                                        File.Exists(outFilename) &&
                                        File.Exists(String.Concat(findStem, "out_", jobID, ".csv")))
                                    {
                                        string line;
                                        bool valid = false;
                                        StreamReader re_out = new StreamReader(outFilename);
                                        while (null != (line = re_out.ReadLine()))
                                        {
                                            if (0 == String.Compare(line, "Execution Result --> No errors were detected during execution. Loop generation successful!"))
                                            {
                                                valid = true;
                                                break;
                                            }
                                        }
                                        re_out.Close();
                                        if(valid)
                                        {
                                            continue; // exists, but not complete!
                                        }
                                    }
                                }

                                rw.Write("arguments = -infile loopcalibration.conf -inputfile ");
                                rw.Write(file);
                                rw.Write(" -loopff_mag_steric_ljf ");
                                rw.Write(strength.ToString("0"));
                                rw.Write(" -stem out_");
                                rw.Write(jobID);
                                rw.Write(" -loopstartindex ");
                                rw.Write(loops[j].FirstDSSPIndex);
                                rw.Write(" -looplength ");
                                rw.WriteLine(loops[j].Length);

                                rw.WriteLine("Queue");
                            }
                        }
                    }

                    if (ParsingFileIndex < FileCount - 1)
                    {
                        ParsingFileIndex++;
                    }
                    else
                    {
                        break;
                    }
                }

                Trace.WriteLine(String.Format("Total valid tra files found {0}, containing {1} loops", traFilesFound, m_TotalLoopCount));

                rw.Close();
            }
        }

        public void WriteLoopCalibrationTest(int[] loopLengths, bool fileCheck )
        {
            for (int iLoop = 0; iLoop < loopLengths.Length; iLoop++)
            {
                string outFile = scriptGenerationDirectory.FullName + "loop_" + loopLengths[iLoop].ToString() + ".que";
                StreamWriter rw = new StreamWriter(outFile, false);

                // write preamble
                
                rw.WriteLine("universe = vanilla");
                rw.WriteLine("should_transfer_files = YES");
                rw.WriteLine("WhenToTransferOutput = ON_EXIT");

                bool LINUXSUPPORT = false;
                if (LINUXSUPPORT)
                {
                    rw.WriteLine("Requirements = ( ((OpSys == \"WINNT50\") || (OpSys == \"WINNT51\") || (OpSys == \"LINUX\")) && ((Arch == \"INTEL\") || (Arch == \"x86_64\")) && (KFlops > 200000))");
                    rw.WriteLine("Executable = executable = loopbuilder.$$(OpSys).exe");
                }
                else
                {
                    rw.WriteLine("REQUIREMENTS = ((OpSys == \"WINNT50\") || (OpSys == \"WINNT51\"))");
                    rw.WriteLine("Executable = loopbuilder.exe");
                }

                rw.WriteLine("Error = condor_stder.log");
                rw.WriteLine("Output = condor_stdout.log");
                rw.WriteLine("Log = condor_job.log");
               
                // write jobs ...

                ParsingFileIndex = 0; // reset IMPORTANT

                m_TotalLoopCount = 0;
                int traFilesFound = 0;

                while (true)
                {
                    // determine if the current DSSP file has a valid tra file in the culled set

                    m_CurrentTraFilePath = "../tra/"
                        + Path.GetFileNameWithoutExtension(CurrentFile.fileInfo.Name)
                        + ".tra";

                    if (CurrentFile.ResidueCount <= 200 && File.Exists(scriptGenerationDirectory.FullName + m_CurrentTraFilePath))
                    {
                        Trace.WriteLine(String.Format("Parsing DSSP file for tra file ({0}): {1}", traFilesFound, m_CurrentTraFilePath));
                        traFilesFound++;

                        SegmentDef[] loops = CurrentFile.GetLoops(loopLengths[iLoop], false, true); // get the current loop set

                        for (int j = 0; j < loops.Length; j++)
                        {
                            string file = CurrentFile.fileInfo.Name;
                            file = Path.GetFileNameWithoutExtension(file);
                            file += ".tra";

                            rw.WriteLine();
                            rw.Write("sh launch.sh < amber03ua.ff,dict.pdb,loopcalibration.path >amber03ua.ff,dict.pdb,loopcalibration.out 2>amber03ua.ff,dict.pdb,loopcalibration.errf,Optimal_Loop_6-5-8.angleset,../tra/");
                            rw.WriteLine(file);

                            string jobID = String.Concat(file, "_", loops[j].FirstDSSPIndex, "_", loops[j].Length);

                            string findStem = String.Concat(scriptGenerationDirectory.Parent.FullName, "\\_", loopLengths[iLoop].ToString(), "Mer\\");
                            if (fileCheck)
                            {
                                string outFilename = String.Concat(findStem, "out_", jobID, ".out");
                                if (File.Exists(String.Concat(findStem, "out_", jobID, ".tra")) &&
                                    File.Exists(outFilename) &&
                                    File.Exists(String.Concat(findStem, "out_", jobID, ".csv")))
                                {
                                    string line;
                                    bool valid = false;
                                    StreamReader re_out = new StreamReader(outFilename);
                                    while (null != (line = re_out.ReadLine()))
                                    {
                                        if (0 == String.Compare(line, "Execution Result --> No errors were detected during execution. Loop generation successful!"))
                                        {
                                            valid = true;
                                            break;
                                        }
                                    }
                                    re_out.Close();
                                    if (valid)
                                    {
                                        continue;
                                    }
                                }
                            }

                            rw.Write("arguments = -infile loopcalibration.conf -inputfile ");
                            rw.Write(file);
                            rw.Write(" -stem out_");
                            rw.Write(jobID);
                            rw.Write(" -loopstartindex ");
                            rw.Write(loops[j].FirstDSSPIndex);
                            rw.Write(" -looplength ");
                            rw.WriteLine(loops[j].Length);

                            rw.WriteLine("Queue");
                        }
                    }

                    if (ParsingFileIndex < FileCount - 1)
                    {
                        ParsingFileIndex++;
                    }
                    else
                    {
                        break;
                    }
                }
                    
                Trace.WriteLine(String.Format("Total valid tra files found {0}, containing {1} loops", traFilesFound, m_TotalLoopCount));
            
                rw.Close();
            }         
        }

        public void WriteSimpleDefFile(int[] loopLengths)
        {
            string outFile = scriptGenerationDirectory.FullName + "loopDef.csv";
            StreamWriter rw = new StreamWriter(outFile, false);

            for (int iLoop = 0; iLoop < loopLengths.Length; iLoop++)
            {
                ParsingFileIndex = 0; // reset IMPORTANT

                m_TotalLoopCount = 0;
                int traFilesFound = 0;

                while (true)
                {
                    // determine if the current DSSP file has a valid tra file in the culled set

                    m_CurrentTraFilePath = "../tra/"
                        + Path.GetFileNameWithoutExtension(CurrentFile.fileInfo.Name)
                        + ".tra";

                    if (File.Exists(scriptGenerationDirectory.FullName + m_CurrentTraFilePath))
                    {
                        Trace.WriteLine(String.Format("Parsing DSSP file for tra file ({0}): {1}", traFilesFound, m_CurrentTraFilePath));
                        traFilesFound++;

                        SegmentDef[] loops = CurrentFile.GetLoops(loopLengths[iLoop], false, true); // get the current loop set

                        for (int j = 0; j < loops.Length; j++)
                        {
                            SimplePrintSegment(rw, loops[j]);
                        }
                    }

                    if (ParsingFileIndex < FileCount - 1)
                    {
                        ParsingFileIndex++;
                    }
                    else
                    {
                        break;
                    }
                }

                Trace.WriteLine(String.Format("Total valid tra files found {0}, containing {1} loops", traFilesFound, m_TotalLoopCount));
            }

            rw.Close();
        }

		public void WriteLoopDefFiles( int clusterNodeCount, int[] loopLengths, int[]normCounts, int[]glyCounts )
		{
			int nodeSend = 0; // index of the clusterNodeLaunch[] stream to rwite to

			StreamWriter[] clusterNodeLaunch = new StreamWriter[ clusterNodeCount ];
			for( int i = 0; i < clusterNodeCount; i++ )
			{
				string outFile = scriptGenerationDirectory.FullName + "box" + (i+1).ToString() + ".bat";
				clusterNodeLaunch[i] = new StreamWriter( outFile, false );
			}

			for( int iLoop = 0; iLoop < loopLengths.Length; iLoop++ )
			{
				ParsingFileIndex = 0; // reset IMPORTANT

				m_TotalLoopCount = 0;
				int traFilesFound = 0;

				while( true )
				{
					// determine if the current DSSP file has a valid tra file in the culled set
            
					m_CurrentTraFilePath = "../tra/" 
						+ Path.GetFileNameWithoutExtension( CurrentFile.fileInfo.Name )
						+ ".tra";

					if( File.Exists( scriptGenerationDirectory.FullName + m_CurrentTraFilePath ) )
					{
						Trace.WriteLine(String.Format("Parsing DSSP file for tra file ({0}): {1}", traFilesFound, m_CurrentTraFilePath ));
						traFilesFound++;

						SegmentDef[] loops = CurrentFile.GetLoops( loopLengths[iLoop], false, true ); // get the current loop set

                        for (int j = 0; j < loops.Length; j++)
                        {
                            int writeIndex = nodeSend % clusterNodeCount;
                            for (int iNorm = 0; iNorm < normCounts.Length; iNorm++)
                            {
                                for (int iGly = 0; iGly < glyCounts.Length; iGly++)
                                {
                                    PrintSegment(clusterNodeLaunch[writeIndex], loops[j], normCounts[iNorm], glyCounts[iGly]);
                                }
                            }
                            nodeSend++;
                        }						
					}
				
					if( ParsingFileIndex < FileCount - 1 )
					{
						ParsingFileIndex++;
					}
					else
					{
						break;
					}
				}

				Trace.WriteLine(String.Format("Total valid tra files found {0}, containing {1} loops", traFilesFound, m_TotalLoopCount ));
			}

			for( int i = 0; i < clusterNodeCount; i++ )
			{
				clusterNodeLaunch[i].Close();
			}
		}

        private void CondorStage1_Header(StreamWriter rw)
        {
            rw.WriteLine("universe = vanilla");
            rw.WriteLine("should_transfer_files = YES");
            rw.WriteLine("WhenToTransferOutput = ON_EXIT");
            rw.WriteLine();
            rw.WriteLine("executable = loopbuilder.$$(OpSys).$$(Arch).exe");
            rw.WriteLine("requirements = ((OpSys == \"WINNT51\" || OpSys == \"LINUX\" ) && (KFlops > 200000) && (Arch == \"x86_64\" || Arch == \"X86_64\" || Arch == \"INTEL\"))");
            rw.WriteLine();
            rw.WriteLine("Error = logerr");
            rw.WriteLine("Output = logout");
            rw.WriteLine("Log = loglog");
            rw.WriteLine();
        }

        private void CondorStage1_Transfer(StreamWriter rw, string traStem, string aa_ua )
        {
            rw.WriteLine();
            rw.WriteLine("transfer_input_files=amber03{0}.ff,dictallHnew.pdb,loop_s1.conf,Optimal_Loop_6-5-8.angleset,../stage2_{0}/tra_store/{1}_Min.tra", aa_ua, traStem);
        }

        private void CondorStage1_Line(StreamWriter rw, SegmentDef seg, string traStem )
        {
            rw.Write("arguments = -infile loop_s1.conf -inputfile ");
            rw.Write(traStem);
            rw.Write("_Min.tra -stem ");
            rw.Write(traStem.Substring(0,traStem.Length-4));
            rw.Write('_');
            rw.Write(seg.FirstDSSPIndex-1);
            rw.Write('_');
            rw.Write(seg.Length);
            rw.Write(" -loopstartindex ");
            rw.Write(seg.FirstDSSPIndex - 1);
            rw.Write(" -looplength ");
            rw.WriteLine(seg.Length);

            rw.WriteLine("queue");
        }

        private bool isValid( string resPath, string stem )
        {
            return (File.Exists(resPath + stem + ".loop") &&
                 File.Exists(resPath + stem + ".out") &&
                File.Exists(resPath + stem + ".tra"));
        }

        private bool m_UnitedAtom = true;

        public void CondorStage1_Generate(int[] loopLengths)
        {
            string atomRepType = m_UnitedAtom ? "ua" : "aa";

            string respath = baseDirectory.FullName + "stage1_" + atomRepType + Path.DirectorySeparatorChar;

            string traStem = "../tra_minimised_" + atomRepType + "/";
            string outFile = scriptGenerationDirectory.FullName + "stage1.que";
            StreamWriter rw = new StreamWriter(outFile, false);

            CondorStage1_Header(rw);

            for (int iLoop = 0; iLoop < loopLengths.Length; iLoop++)
            {
                ParsingFileIndex = 0; // reset IMPORTANT

                m_TotalLoopCount = 0;
                int traFilesFound = 0;

                double countValidLoops = 0.0;
                double countTotalLoops = 0.0;

                while (true)
                {
                    // determine if the current DSSP file has a valid tra file in the culled set
                    string traNameStem = Path.GetFileNameWithoutExtension(CurrentFile.fileInfo.Name);
                    m_CurrentTraFilePath = traStem + traNameStem + "_Min.tra";

                    if (File.Exists(scriptGenerationDirectory.FullName + m_CurrentTraFilePath))
                    {
                        Trace.WriteLine(String.Format("Parsing DSSP file for tra file ({0}): {1}", traFilesFound, m_CurrentTraFilePath));
                        traFilesFound++;

                        // Obtain loop lists from the Current DSSP file
                        SegmentDef[] allLoopsIncInvalid = CurrentFile.GetLoops(loopLengths[iLoop], false, false);
                        SegmentDef[] loops = CurrentFile.GetLoops(loopLengths[iLoop], false, true); // get the current loop set

                        // Do some counting for reporting to the user
                        countValidLoops += loops.Length;
                        countTotalLoops += allLoopsIncInvalid.Length;
                        m_TotalLoopCount += loops.Length;

                        bool writtenTransfer = false;
                        for (int j = 0; j < loops.Length; j++)
                        {
                            string stem = String.Format("{0}_{1}_{2}", traNameStem.Substring(0,5), loops[j].FirstDSSPIndex - 1, loops[j].Length);
                            if (!isValid(respath,stem))
                            {
                                if (!writtenTransfer)
                                {
                                    CondorStage1_Transfer(rw, traNameStem, atomRepType); // Only write this if we have valid loops in the current file.
                                    writtenTransfer = true;
                                }
                                CondorStage1_Line(rw, loops[j], traNameStem);
                            }
                        }
                        rw.Flush();
                    }
                    else
                    {
                        throw new Exception("Could not find the Tra file!");
                    }

                    if (ParsingFileIndex < FileCount - 1)
                    {
                        ParsingFileIndex++;
                    }
                    else
                    {
                        break;
                    }
                }

                Trace.WriteLine(String.Format("Total valid tra files found {0}, containing {1} loops ({2:P} valid)", traFilesFound, m_TotalLoopCount, countValidLoops / countTotalLoops));
            }

            rw.Close();
        }

        private void SimplePrintSegment(StreamWriter rw, SegmentDef segment)
        {
            m_TotalLoopCount++;
            rw.WriteLine("{0},{1},{2},{3}", 
                CurrentFile.fileInfo.Name,
                CurrentFile.ResidueCount,
                segment.FirstDSSPIndex - 1, // DSSP uses 1 based indexing, PD uses 0 based ...
                segment.Length
                );
        }

		private void PrintSegment( StreamWriter rw, SegmentDef segment, int norm, int gly )
		{
			// PD argList
			// 0 == anglesetFileName
			// 1 == logFileOutput
			// 2 == inTraFile
			// 3 == outTraFile
			// 4 == loopStartIndex
			// 5 == loopLength

			// all fine .. print it ...
			m_TotalLoopCount++;
            rw.Write( m_AppLanuchPath );
			rw.WriteLine( " ../scriptgen/{1}_{2}.angleset out_{0}_{1}_{2}.log {3} tempOutput.tra {4} {0}", segment.Length, norm, gly, m_CurrentTraFilePath, 
				segment.FirstDSSPIndex - 1 // DSSP uses 1 based indexing, PD uses 0 based ...
				);
		}
	}
}
