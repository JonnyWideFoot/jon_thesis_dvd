using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using UoB.Core.FileIO.DSSP;
using UoB.Methodology.DSSPAnalysis;
using UoB.Compression;
using UoB.AppLayer.Common;

namespace UoB.AppLayer.Rapper
{
    enum Validity
    {
        Valid,
        Fail,
        MethodFail
    }

    class RapperFunctions
    {
        private RapperFunctions()
        {
        }

        // string name, string pathRoot, string jobStem, char chainID, int startResIndex, int lastResIndex, int loopLength
        public static Validity IsValidReturn(CompressedFileManager _Comp, string resultPath, string jobStem)
        {
            // For output from petra to be valid, both output PDB files must be present AND there must be a tar.bz2 file

            StreamReader re = null;

            

            FileInfo outFile;
            FileInfo outZippedFile = new FileInfo(resultPath + jobStem + ".out.bz2");
            bool compression = outZippedFile.Exists;
            if (compression)
            {
                _Comp.Uncompress(outZippedFile.FullName);
                outFile = new FileInfo(_Comp.OutPath(outZippedFile.FullName).FullName + Path.DirectorySeparatorChar + jobStem + ".out");
            }
            else
            {
                outFile = new FileInfo(resultPath + jobStem + ".out");
            }

            FileInfo errFile = new FileInfo(resultPath + jobStem + ".err");
            FileInfo tarFile = new FileInfo(resultPath + jobStem + ".tar.bz2");

            if (!errFile.Exists || !outFile.Exists || !tarFile.Exists)
            {
                return Validity.Fail;
            }

            if (outFile.Length == 0 || tarFile.Length == 0)
            {
                return Validity.Fail;
            }

            string line;
            if (errFile.Length > 0)
            {
                bool methodFail = false;
                re = new StreamReader(errFile.FullName);
                while (true)
                {
                    if (null == (line = re.ReadLine())) break;
                    if (null == (line = re.ReadLine())) break;
                    if (0 != String.Compare("----------", 0, line, 0, 10)) break;
                    if (null == (line = re.ReadLine())) break;
                    if (0 != String.Compare("ERROR [General Error]:", 0, line, 0, 22)) break;
                    if (null == (line = re.ReadLine())) break;
                    if (null == (line = re.ReadLine())) break;
                    if (
                        0 != String.Compare("  No models could be generated under the provided restraints.", 0, line, 0, 61) &&
                        0 != String.Compare("  The N anchor residue has no valid children, aborting search.", 0, line, 0, 62)
                        ) break;
                    methodFail = true;
                }
                re.Close();
                if (methodFail)
                {
                    return Validity.MethodFail;
                }
                else
                {
                    return Validity.Fail;
                }
            }

            bool valid = false;
            re = new StreamReader(outFile.FullName);
            while (null != (line = re.ReadLine()))
            {
                if (0 == String.Compare("Overall RAPPER runtime:", 0, line, 0, 23))
                {
                    valid = true;
                    break;
                }
            }
            re.Close();

            if (compression)
            {
                _Comp.CleanUp(outZippedFile.FullName);
            }

            if (!valid) return Validity.Fail;
            return Validity.Valid;
        }
    }

    class RapperInputGenerator : DSSPLoopRunGen
    {
        public RapperInputGenerator(AppLayerBase parent)
            : base(parent)
        {
        }

        private void GenerateCondor_Start(StreamWriter rw)
        {
            rw.WriteLine("universe = vanilla");
            rw.WriteLine("should_transfer_files = YES");
            rw.WriteLine("WhenToTransferOutput = ON_EXIT");

            rw.WriteLine("requirements = ((KFlops > 700000) && (OpSys == \"LINUX\") && ( (Arch == \"x86_64\") || (Arch == \"X86_64\") ||(Arch == \"INTEL\") ) )");

            rw.WriteLine("Executable = launch.sh");
            rw.WriteLine("Log = submitlog.out");

            rw.WriteLine();
        }

        private void GenerateCondor(StreamWriter rw, string pdbName, string jobStem, int startResIndex, int lastResIndex)
        {
            rw.Write("transfer_input_files=../pdb/");
            rw.Write(pdbName);
            rw.WriteLine(".min.pdb");

            rw.Write("arguments = ");
            rw.Write(pdbName);
            rw.Write(" ");
            rw.Write(startResIndex);
            rw.Write(" ");
            rw.WriteLine(lastResIndex);

            rw.Write("Output = ");
            rw.Write(jobStem);
            rw.WriteLine(".out");

            rw.Write("Error = ");
            rw.Write(jobStem);
            rw.WriteLine(".err");

            rw.WriteLine("Queue");

            rw.WriteLine();

            rw.Flush();
        }

        public void CreateLaunchWrapper(string execDir)
        {
            StreamWriter launchJob = new StreamWriter(execDir + "launch.sh");

            launchJob.WriteLine("#!/bin/sh");
            launchJob.WriteLine();
            launchJob.WriteLine("echo Running on host: $HOSTNAME");
            launchJob.WriteLine("echo PATH is: $PATH");
            launchJob.WriteLine("echo Using PDBStem: $1");
            launchJob.WriteLine("echo Using StartID: $2");
            launchJob.WriteLine("echo Using EndID: $3");
            launchJob.WriteLine();
            launchJob.WriteLine("len=$(($3-$2+1))");
            launchJob.WriteLine("echo Len was computed as: $len");
            launchJob.WriteLine();
            launchJob.WriteLine("MODEL_NUM=1000");
            launchJob.WriteLine();
            launchJob.WriteLine("# Launch rapper");
            launchJob.WriteLine("/shared_mount/rapper/distrib/rapper_Linux_i386 /shared_mount/rapper/distrib/params_Linux_i386.xml model-loops-benchmark --rapper-dir /shared_mount/rapper --models $MODEL_NUM --pdb $1.min.pdb --start $2 --stop $3 --sidechain-mode smart");
            launchJob.WriteLine();
            launchJob.WriteLine("# all useful output is now sitting in ./TESTRUNS");
            launchJob.WriteLine("# Clean");
            launchJob.WriteLine("rm -f *.pdb");
            launchJob.WriteLine("rm -f ./TESTRUNS/run-parameters.xml");
            launchJob.WriteLine();
            launchJob.WriteLine("# Compress");
            launchJob.WriteLine("cd TESTRUNS");
            launchJob.WriteLine("tar -cvjf $1_$2_$len.tar.bz2 *");
            launchJob.WriteLine("mv $1_$2_$len.tar.bz2 ../");
            launchJob.WriteLine("cd ..");
            launchJob.WriteLine("rm -rf TESTRUNS");
            launchJob.WriteLine();

            launchJob.Close();
        }

        public void CreateRapperJobs()
        {
            for (int loopLength = 6; loopLength <= 11; loopLength++)
            {
                string pathname = scriptGenerationDirectory.FullName + loopLength.ToString() + Path.DirectorySeparatorChar;
                string resultPath = resultDirectory.FullName + Path.DirectorySeparatorChar + loopLength.ToString() + Path.DirectorySeparatorChar;

                Directory.CreateDirectory(pathname);

                StreamWriter condorJob = new StreamWriter(pathname + "condor.job");

                GenerateCondor_Start(condorJob);

                CreateLaunchWrapper(pathname);

                ParsingFileIndex = 0; // reset IMPORTANT
                while (true)
                {
                    SegmentDef[] loops = CurrentFile.GetLoops(loopLength,false, true);
                    string currentName = CurrentFile.InternalName.Substring(0, 5);
                    Trace.Write(currentName + ": ");

                    for (int i = 0; i < loops.Length; i++)
                    {                      
                        // Rapper SHOULDNT use the FirstResidueIndex as it even though it uses PDB not TRA
                        // The PDB files are sourced from the minimised tra files and therefore are 
                        // indexed from 0
                        int startIndex = loops[i].FirstDSSPIndex - 1;
                        int length = loops[i].Length;
                        int lastIndex = loops[i].LastDSSPIndex - 1;

                        string currentJobStem = currentName + '_' + startIndex.ToString();
                        //char currnetInsertionCode = loops[i].FirstResidueInsertionCode;
                        //if (currnetInsertionCode != ' ')
                        // {
                        //     currentJobStem += currnetInsertionCode;
                        // }
                        currentJobStem += '_';
                        currentJobStem += length;
                        char currentChainID = currentName[4];

                        if (Validity.Fail == RapperFunctions.IsValidReturn(m_Comp, resultPath, currentJobStem))
                        {
                            GenerateCondor(condorJob, currentName, currentJobStem, startIndex, lastIndex);
                            Trace.Write('x');
                        }
                        else
                        {
                            Trace.Write('.');
                        }
                    }

                    Trace.WriteLine("");

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
                condorJob.Close();
            }
        }
    }
}
