using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

using UoB.Core.FileIO.DSSP;
using UoB.Methodology.DSSPAnalysis;
using UoB.Core.FileIO.PDB;
using UoB.Compression;
using UoB.Core.Structure;
using UoB.Core.Primitives;
using UoB.Core.Structure.Primitives;
using UoB.AppLayer.Common;

namespace UoB.AppLayer.Cloop
{
    enum Validity
    {
        Valid,
        Fail,
        Invalid
    }

    enum Target
    {
        Grendel,
        Dirac
    }

    class CloopFunctions
    {
        private CloopFunctions()
        {
        }

        public static Validity IsValidReturn(string resultPath, string jobStem)
        {
            FileInfo outFile = new FileInfo(resultPath + jobStem + ".out.bz2");
            FileInfo tarFile = new FileInfo(resultPath + jobStem + ".tar.bz2");
            if (!outFile.Exists)
            {
                outFile = new FileInfo(resultPath + jobStem + ".out");
            }
            if (outFile.Exists && tarFile.Exists && outFile.Length > 0 && tarFile.Length > 0 )
            {                
                return Validity.Valid;
            }
            else
            {
                return Validity.Invalid;
            }
        }

        public static bool CheckArchive(CompressedFileManager _Comp, string resultPath, string jobStem, int loopLength, bool manageCompression)
        {
            bool error = false;

            string archive = resultPath + jobStem + ".tar.bz2";
            string outArchive = resultPath + jobStem + ".out.bz2";
            string longStem = resultPath + jobStem + Path.DirectorySeparatorChar + jobStem;

            if (manageCompression)
            {
                _Comp.Uncompress(archive);
                //_Comp.Uncompress(outArchive); - not needed for normal analysis - we already know the 'stem.out.bz2' exists
            }

            DirectoryInfo outPath = _Comp.OutPath(archive);

            FileInfo eneFile = new FileInfo(String.Concat(outPath.FullName, jobStem.Substring(0, 5), ".ene"));
            if (!eneFile.Exists)
            {
                error = true;
                goto END;
            }

            for (int i = 1; i <= 1000; i++)
            {
                if (!File.Exists(String.Format("{0}conf{1}.pdb", outPath.FullName, i.ToString().PadLeft(3, '0'))))
                {
                    error = true;
                    goto END;
                }
            }

            END:
            if (manageCompression) _Comp.CleanUp(archive);
            return !error;
        }
    }

    class CloopInputGenerator : DSSPLoopRunGen
    {
        public CloopInputGenerator(AppLayerBase parent)
            : base(parent)
        {
        }

        int m_LoopStartLen = 6;
        int m_LoopEndLen = 11;

        // Dirac
        Target m_Target = Target.Dirac;
        string m_CharmVString = "c30b2";
        //string m_CharmmPath = @"/home2/chmwvdk/charmm/c29b2";
        string m_CharmmPath = @"/home/ccibs/charm/c30b2"; // I have run this on Dirac for 6mers only
        //string m_CharmmPath = @"/home/chfc/PROGRAMS/charmm30/c30b2";
        //string m_CharmmPath = @"/home/chfc/PROGRAMS/charmm29/c29b2/";

        // Grendel
        //Target m_Target = Target.Grendel;
        //string m_CharmmPath = @"/home3/chmwvdk/charmm/c29b2";

        private void GenerateSubmit(StreamWriter rw, string pdbName, string jobStem, int startResIndex, int loopLength)
        {
            rw.WriteLine( "qsub -o {0}.out -e {0}.err {0}.sh ", jobStem);
        }

        private void CreateLaunchScript(string path, string name, string jobStem, int startResIndex, int loopLength)
        {
            StreamWriter rw = new StreamWriter(path + jobStem + ".sh");

            rw.WriteLine("#!/bin/sh");
            string nodeType = ",nodes=1:fast";
            if (m_Target == Target.Dirac) nodeType = "";
            rw.WriteLine("#PBS -l walltime={0}:00:00{1}", loopLength - 2, nodeType);            
            rw.WriteLine();            
            rw.WriteLine("echo PBS Launching Script In:");
            rw.WriteLine("pwd");
            rw.WriteLine("echo Moving To Run Dir");
            rw.WriteLine("cd ~/cloop/{0}", loopLength);
            rw.WriteLine();

            // Prevent a race condition between multiple returning jobs by using a lock
            rw.WriteLine("if ( set -o noclobber; echo \"$$\" > \"lockfile\") 2> /dev/null; ");
            rw.WriteLine("then");
            rw.WriteLine("   trap 'rm -f \"lockfile\"; exit $?' INT TERM EXIT");           
            rw.WriteLine(); // critical-section
            rw.WriteLine("   echo \"Performing compression operation...\"");
            rw.WriteLine();
            rw.WriteLine("   bzip2 *.out"); // annoying, but the out files are HUGE (~50mb)! - compress any that are sitting in the folder from other runs
            rw.WriteLine();
            rw.WriteLine("   rm -f \"lockfile\"");
            rw.WriteLine("   trap - INT TERM EXIT");
            rw.WriteLine("else");
            rw.WriteLine("   echo \"Failed to acquire lockfile: Another returning job must be compressing... Ignoring compression command.\" ");
            rw.WriteLine("   echo \"Held by $(cat lockfile)\"");
            rw.WriteLine("fi"); 
            rw.WriteLine();

            rw.WriteLine("# store the current folder");
            rw.WriteLine("JHOME=$PWD");
            rw.WriteLine("export JHOME");
            rw.WriteLine("echo Submit dir was: $JHOME");
            rw.WriteLine();
            rw.WriteLine("# Setup the working environment");
            rw.WriteLine("if [ -z \"$JOBNO\" ]");
            rw.WriteLine("then");
            rw.WriteLine("        WORKDIR=/tmp/jr0407/{0}", jobStem);
            rw.WriteLine("else");
            rw.WriteLine("        WORKDIR=/tmp/jr0407/{0}_$JOBNO", jobStem);
            rw.WriteLine("fi");
            rw.WriteLine("export WORKDIR");
            rw.WriteLine("echo Using workdir: $WORKDIR");
            rw.WriteLine("#clean out any existing muck");
            rw.WriteLine("rm -rf $WORKDIR/*");
            rw.WriteLine("mkdir -p $WORKDIR");
            rw.WriteLine("cp ../pdb/" + jobStem.ToLower() + ".pdb $WORKDIR");
            rw.WriteLine("cp ../exec/cloop.inp $WORKDIR");
            rw.WriteLine("cp ../exec/randconf.str $WORKDIR");
            rw.WriteLine("cd $WORKDIR");
            rw.WriteLine();
            rw.WriteLine("# store name of node and scratch directory in file rundat");
            rw.WriteLine("hostname > {0}.rundat", jobStem);
            rw.WriteLine("pwd >> {0}.rundat", jobStem);
            rw.WriteLine();
            rw.WriteLine("{3}/exec/gnu/charmm pname={0} startindex={1} endindex={2} toppar={3}/toppar/ < cloop.inp",
                name.ToLower(), startResIndex, startResIndex + loopLength - 1, m_CharmmPath);
            rw.WriteLine();
            rw.WriteLine("#########################");
            rw.WriteLine("# Clean and zip complete");
            rw.WriteLine("#########################");
            rw.WriteLine();
            rw.WriteLine("echo Run Complete. Files generated:");
            rw.WriteLine("ls -oAh");
            rw.WriteLine();
            rw.WriteLine("echo Compression Commencing...");
            char compressionFlag = 'j'; // default
            if (m_Target == Target.Dirac) compressionFlag = 'I';
            rw.WriteLine("tar --remove-files -cv{0}f {1}.tar.bz2 *", compressionFlag, jobStem);
            rw.WriteLine();
            rw.WriteLine("# Return the files to the home node");
            rw.WriteLine("mv -u * $JHOME");
            rw.WriteLine("rm -rf $WORKDIR");

            rw.Close();
        }

        public void CreateCloopJobs()
        {
            for (int loopLength = m_LoopStartLen; loopLength <= m_LoopEndLen; loopLength++)
            {
                int Fail = 0;
                int Total = 0;

                string pathname = scriptGenerationDirectory.FullName + loopLength.ToString() + Path.DirectorySeparatorChar;
                string resPath = baseDirectory.FullName + "Result_" + m_CharmVString + Path.DirectorySeparatorChar + loopLength.ToString() + Path.DirectorySeparatorChar;

                Directory.CreateDirectory(pathname);

                StreamWriter SubmitJob = new StreamWriter(pathname + "submit.sh");

                ParsingFileIndex = 0; // reset IMPORTANT
                while (true)
                {
                    string currentName = CurrentFile.InternalName.Substring(0, 5);

                    Trace.Write(currentName + ">: ");

                    SegmentDef[] loops = CurrentFile.GetLoops(loopLength, false, true);                    
                    for (int i = 0; i < loops.Length; i++)
                    {
                        // Cloop SHOULDNT use the FirstResidueIndex as it even though it uses PDB not TRA
                        // The PDB files are sourced from the minimised tra files and therefore are 
                        // indexed from 0
                        int startIndex = loops[i].FirstDSSPIndex - 1;
                        // make residues 1 not 0 based for simplicity for the CLoop job to interpret
                        startIndex++;
                        int length = loops[i].Length;                      

                        string currentJobStem = currentName + '_' + startIndex.ToString();
                        //char currnetInsertionCode = loops[i].FirstResidueInsertionCode;
                        //if (currnetInsertionCode != ' ')
                        // {
                        //     currentJobStem += currnetInsertionCode;
                        // }
                        currentJobStem += '_';
                        currentJobStem += length;
                        char currentChainID = currentName[4];

                        // Now that we have written the file using OUR index system, 

                        Validity valid = CloopFunctions.IsValidReturn(resPath, currentJobStem);

                        if (valid == Validity.Valid)
                        {
                            if (!CloopFunctions.CheckArchive(m_Comp, resPath, currentJobStem, loopLength, true))
                            {
                                valid = Validity.Invalid;
                            }
                        }

                        switch( valid )
                        {
                            case Validity.Invalid:
                                GenerateSubmit(SubmitJob, currentName, currentJobStem, startIndex, loopLength);
                                CreatePDBFile(currentName, currentJobStem, Obfuscation.Randomise, startIndex - 1, loopLength, ".pdb", true, true);
                                CreateLaunchScript(pathname, currentName, currentJobStem, startIndex, loopLength);
                                Trace.Write('X');
                                break;
                            case Validity.Valid:
                                Trace.Write('|');
                                break;
                            case Validity.Fail:
                                Fail++;
                                Trace.Write('?');
                                break;
                            default:
                                throw new Exception();
                        }

                        Total++;
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
                SubmitJob.Close();

                Trace.WriteLine("Method Failure:");
                Trace.WriteLine(String.Format("Fail: {0} of {1} ({2:p})", Fail, Total, (double)Fail / (double)Total));
                continue;
            }            
        }
    }
}
