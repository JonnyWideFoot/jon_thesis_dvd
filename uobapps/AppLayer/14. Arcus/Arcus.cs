using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

using UoB.Core.FileIO.DSSP;
using UoB.Methodology.DSSPAnalysis;
using UoB.Core.FileIO.PDB;
using UoB.Core.FileIO.Tra;
using UoB.Compression;
using UoB.AppLayer.Common;
using UoB.Core.Structure;
using UoB.Core.Primitives;
using UoB.Core.Primitives.Matrix;

namespace UoB.AppLayer.Arcus
{
    enum Validity
    {
        Valid,
        Invalid
    }

    class ArcusFunctions
    {
        private ArcusFunctions()
        {
        }

        public static Validity IsValidReturn(string resultPath, string currentJobStem)
        {
            if (File.Exists(resultPath + currentJobStem))
            {
                return Validity.Valid;
            }
            return Validity.Invalid;
        }
    }

    class ArcusGenerator : DSSPLoopRuns
    {
        public ArcusGenerator(AppLayerBase parent)
            : base(parent, false, ProcessPriorityClass.Idle)
        {
            StartLoopLength = 8;
            EndLoopLength = 8;
        }

        private void headerToList(StreamWriter rw)
        {
            rw.WriteLine("universe = vanilla");
            rw.WriteLine("should_transfer_files = YES");
            rw.WriteLine("WhenToTransferOutput = ON_EXIT");

            int kflops = 60000;
            rw.WriteLine("requirements = (Machine != \"bohr.phy.bris.ac.uk\") && ({0}(KFlops > {1}) &&((OpSys == \"WINNT50\") || (OpSys == \"WINNT51\")) && ( (Arch == \"x86_64\") || (Arch == \"X86_64\") ||(Arch == \"INTEL\") ) )",
                CommonFunctions.ExcludeMachineString(), kflops);

            rw.WriteLine("Log = submitlog.out");

            rw.WriteLine();
        }

        private void setExec(StreamWriter rw, string execName)
        {
            rw.WriteLine("Executable = ../exec/{0}", execName);
        }

        private void printToList(string jobStem, string fulljobstem, StreamWriter rw, SegmentDef def, int modelCount, int execID)
        {
            rw.Write("transfer_input_files=");

            rw.Write("../exec/scl-B30-occ1.0-rmsd1.0-prop20.0.pdb, ");
            rw.Write("../exec/scl-B30-occ1.0-rmsd1.0-prop20.0.dat, ");
            rw.Write("../exec/big.loop.angleset, ");
            rw.Write("../exec/default.class, ");
            rw.Write("../exec/default.alias, ");
            rw.Write("../exec/amber03aa.ff, ");
            rw.Write("../exec/segdist.dat, ");
            rw.WriteLine("../pdb/{0}.min.pdb", jobStem);
            rw.WriteLine();

            rw.Write("arguments = ");
            rw.WriteLine(String.Format("{0}.min.pdb {1} {2} {3} {4}", jobStem, def.FirstDSSPIndex-1, def.Length, modelCount, execID));

            rw.Write("Output = ");
            rw.Write(fulljobstem);
            rw.WriteLine(".out");

            rw.Write("Error = ");
            rw.Write(fulljobstem);
            rw.WriteLine(".err");

            rw.WriteLine("Queue");

            rw.WriteLine();

            rw.Flush();
        }

        private void printToListS3(string jobStem, string fulljobstem, string scwrl, StreamWriter rw, SegmentDef def, float dielec, int execID)
        {
            rw.Write("transfer_input_files=");

            string str1 = string.Format("{0}",def.FirstDSSPIndex - 1);
            string str2 = string.Format("{0}", def.Length);
            string uberjobStem = fulljobstem + "_1000_" + str1 + '_' + str2;

            rw.Write("../exec/{0}.pdb, ", scwrl);
            rw.Write("../exec/{0}.dat, ", scwrl);
            rw.Write("../exec/big.loop.angleset, ");
            rw.Write("../exec/default.class, ");
            rw.Write("../exec/default.alias, ");
            rw.Write("../exec/amber03aa.ff, ");
            rw.Write("../exec/segdist.dat, ");
            rw.WriteLine("/home/jr0407/tra/{0}.tra.tra", uberjobStem);
            rw.WriteLine();

            rw.Write("arguments = ");
            rw.WriteLine(String.Format("{0}.tra.tra {1} {2} {3:0.0} {4}", uberjobStem, def.FirstDSSPIndex - 1, def.Length, dielec, execID));

            rw.Write("Output = ");
            rw.Write(fulljobstem);
            rw.WriteLine(".out");

            rw.Write("Error = ");
            rw.Write(fulljobstem);
            rw.WriteLine(".err");

            rw.WriteLine("Queue");

            rw.WriteLine();

            rw.Flush();
        }

        bool hasFile(List<string> filenames, string file, SegmentDef loop )
        {
            int start = loop.FirstDSSPIndex-1;
            file += ( "_1000_" + start.ToString() + '_' + loop.Length + ".tra.tra" );

            for (int i = 0; i < filenames.Count; i++)
            {
                if (0 == String.Compare(filenames[i], file))
                {
                    return true;
                }
            }
            return false;
        }

        public void printListS3(int loopLength)
        {
            StreamReader re = new StreamReader(resultDirectory.FullName + "loopdone.list");
            List<string> donefiles = new List<string>();
            string line;
            while( ( line = re.ReadLine() ) != null )
            {
                donefiles.Add(line);
            }
            re.Close();

            string pathname = scriptGenerationDirectory.FullName + loopLength.ToString() + Path.DirectorySeparatorChar;
            Directory.CreateDirectory(pathname);

            StreamWriter jobList = new StreamWriter(pathname + "job.que");
            headerToList(jobList);

            string[] execNames = { "", "", "", "", "scratch_s3.exe" };
            float[] doDielec = { 1.0f };
            bool del = false;

            for (int r = 0; r < execNames.Length; r++)
            {
                if (execNames[r].Length == 0)
                    continue;

                setExec(jobList, execNames[r]);

                for (int q = 0; q < doDielec.Length; q++)
                {
                    List<SegmentDef> storeLoops = new List<SegmentDef>();
                    List<string> storeLoopSources = new List<string>();
                    List<string> storeLoopStems = new List<string>();

                    ParsingFileIndex = 0; // reset IMPORTANT
                    while (true)
                    {
                        string currentName = CurrentFile.InternalName.Substring(0, 5);

                        Trace.Write(currentName + ">: ");

                        SegmentDef[] loops = CurrentFile.GetLoops(loopLength, false, true);
                        for (int i = 0; i < loops.Length; i++)
                        {
                            // NativePert SHOULDNT use the FirstResidueIndex as it even though it uses PDB not TRA
                            // The PDB files are sourced from the minimised tra files and therefore are 
                            // indexed from 0
                            int startIndex = loops[i].FirstDSSPIndex - 1;
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
                            currentJobStem = r.ToString() + '_' + currentJobStem;

                            Validity valid = ArcusFunctions.IsValidReturn(currentJobStem, currentJobStem);

                            switch (valid)
                            {
                                case Validity.Invalid:
                                    storeLoops.Add(loops[i]);
                                    storeLoopSources.Add(currentName);
                                    storeLoopStems.Add(currentJobStem);
                                    break;
                                case Validity.Valid:
                                    break;
                                default:
                                    throw new Exception();
                            }

                            Trace.Write('.');
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

                    Random rand = new Random(0); // a defined seed - reproducible!

                    while (del && storeLoops.Count > 100)
                    {
                        int at = rand.Next(0, storeLoops.Count - 1);
                        storeLoops.RemoveAt(at);
                        storeLoopSources.RemoveAt(at);
                        storeLoopStems.RemoveAt(at);
                    }

                    for (int i = 0; i < storeLoops.Count; i++)
                    {

                        if (hasFile(donefiles, storeLoopStems[i], storeLoops[i]))
                        {
                            printToListS3(storeLoopSources[i], storeLoopStems[i], "scl-B30-occ1.0-rmsd0.5-prop20.0", jobList, storeLoops[i], doDielec[q], r);
                        }
                        else
                        {
                            //file.MoveTo(baseDirectory.FullName + "ScriptGen" + name + "//keep//" + storeLoopStems[i] + '_' + doDielec[q].ToString("0.00") + ".csv");
                        }
                    }
                }
            }

            jobList.Close();
        }

        public void printList(int loopLength)
        {
            string pathname = scriptGenerationDirectory.FullName + loopLength.ToString() + Path.DirectorySeparatorChar;
            Directory.CreateDirectory(pathname);

            StreamWriter jobList = new StreamWriter(pathname + "job.que");
            headerToList(jobList);

            //string[] execNames = { "scratch.exe" };
            //int[] doCounts = { 1, 5, 10, 25, 50, 75, 100, 250, 500, 1000, 1500, 2000 };
            //bool del = true;
            //string name = "_NumMod";

            //string[] execNames = { "", "scratch_nodist.exe", "scratch_noseg.exe", "scratch_nosegordist.exe" };
            //int[] doCounts = { 500 };
            //bool del = true;
            //string name = "_filt";

            string[] execNames = { "", "", "", "", "scratch_s2.exe" };
            int[] doCounts = { 1000 };
            bool del = false;
            string name = "_s2";

            for (int r = 0; r < execNames.Length; r++)
            {
                if (execNames[r].Length == 0) 
                    continue;

                setExec(jobList, execNames[r]);

                for (int q = 0; q < doCounts.Length; q++)
                {
                    List<SegmentDef> storeLoops = new List<SegmentDef>();
                    List<string> storeLoopSources = new List<string>();
                    List<string> storeLoopStems = new List<string>();

                    ParsingFileIndex = 0; // reset IMPORTANT
                    while (true)
                    {
                        string currentName = CurrentFile.InternalName.Substring(0, 5);

                        Trace.Write(currentName + ">: ");

                        SegmentDef[] loops = CurrentFile.GetLoops(loopLength, false, true);
                        for (int i = 0; i < loops.Length; i++)
                        {
                            // NativePert SHOULDNT use the FirstResidueIndex as it even though it uses PDB not TRA
                            // The PDB files are sourced from the minimised tra files and therefore are 
                            // indexed from 0
                            int startIndex = loops[i].FirstDSSPIndex - 1;
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
                            currentJobStem = r.ToString() + '_' + currentJobStem;

                            Validity valid = ArcusFunctions.IsValidReturn(currentJobStem, currentJobStem);

                            switch (valid)
                            {
                                case Validity.Invalid:
                                    storeLoops.Add(loops[i]);
                                    storeLoopSources.Add(currentName);
                                    storeLoopStems.Add(currentJobStem);
                                    break;
                                case Validity.Valid:
                                    break;
                                default:
                                    throw new Exception();
                            }

                            Trace.Write('.');
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

                    Random rand = new Random(0); // a defined seed - reproducible!

                    while (del && storeLoops.Count > 100)
                    {
                        int at = rand.Next(0, storeLoops.Count - 1);
                        storeLoops.RemoveAt(at);
                        storeLoopSources.RemoveAt(at);
                        storeLoopStems.RemoveAt(at);
                    }

                    for (int i = 0; i < storeLoops.Count; i++)
                    {
                        FileInfo file = new FileInfo(baseDirectory.FullName + "ScriptGen" + name + "//" + storeLoopStems[i] + '_' + doCounts[q].ToString() + ".csv");
                        if (!file.Exists || file.Length == 0)
                        {
                            printToList(storeLoopSources[i], storeLoopStems[i], jobList, storeLoops[i], doCounts[q], r);
                        }
                        else
                        {
                            //file.MoveTo(baseDirectory.FullName + "ScriptGen" + name + "//keep//" + storeLoopStems[i] + '_' + doCounts[q].ToString() + ".csv");
                        }
                    }
                }
            }

            jobList.Close();
        }
    }
}
