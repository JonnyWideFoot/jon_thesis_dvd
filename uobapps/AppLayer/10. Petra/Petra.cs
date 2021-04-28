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
using UoB.AppLayer.Common;

namespace UoB.AppLayer.Petra
{
    enum Validity
    {
        Valid,
        FailPetra_Sampling,
        FailCoda_Sampling,
        FailFread_SegFault,
        FailRead7_SegFault,
        Invalid
    }

    class PetraFunctions
    {
        private PetraFunctions()
        {
        }

        private static Regex FreadSegFault = new Regex("Segmentation fault      \\(core dumped\\) \\$JBIN/FREAD3", RegexOptions.Compiled);

        public static Validity IsValidReturn( CompressedFileManager m_Comp, string stem, string jobStem, SegmentDef loop)
        {
            // For output from Petra to be valid, both output PDB files must be present AND there must be a tar.bz2 file

            FileInfo coda = new FileInfo(stem + jobStem + ".coda.pdb");
            FileInfo Petra = new FileInfo(stem + jobStem + ".Petra.pdb");
            FileInfo codaMeld = new FileInfo(stem + jobStem + ".coda.meld");
            FileInfo PetraMeld = new FileInfo(stem + jobStem + ".Petra.meld");
            FileInfo outFile = new FileInfo(stem + jobStem + ".out");
            FileInfo errFile = new FileInfo(stem + jobStem + ".err");
            FileInfo bz2 = new FileInfo(stem + jobStem + ".tar.bz2");

            if (!outFile.Exists || !errFile.Exists)
            {
                return Validity.Invalid;
            }

            StreamReader re = new StreamReader(outFile.FullName);
            string line;
            string host;
            while (null != (line = re.ReadLine()))
            {
                if (0 == String.Compare(line, 0, "Host:", 0, 5))
                {
                    host = line;
                }
                if (
                    0 == String.Compare(line, "CODA license check out failed!") ||
                    0 == String.Compare(line, "License check out failed!")
                    )
                {
                    re.Close();
                    return Validity.Invalid;
                }
            }
            re.Close();

            re = new StreamReader(errFile.FullName);
            while (null != (line = re.ReadLine()))
            {
                if (FreadSegFault.IsMatch(line))
                {
                    re.Close();
                    return Validity.FailFread_SegFault;
                }
                else if (0 == string.Compare(line, 0, "*** glibc detected *** double free or corruption", 0, 39, false))
                {
                    re.Close();
                    return Validity.FailFread_SegFault;
                }
                else if (Regex.IsMatch(line, "Segmentation fault      \\(core dumped\\) \\$JBIN/Read7"))
                {
                    re.Close();
                    return Validity.FailRead7_SegFault;
                }
            }
            re.Close();

            if (!codaMeld.Exists || codaMeld.Length == 0 || !PetraMeld.Exists || PetraMeld.Length == 0)
            {
                return Validity.Invalid;
            }

            if (codaMeld.Length < 1024 || PetraMeld.Length < 1024)
            {
                // Small meld files could be a coda method failure...
                bool codaFail = false;
                bool PetraFail = false;

                re = new StreamReader(PetraMeld.FullName);
                line = re.ReadLine();
                re.Close();
                PetraFail = (0 == String.Compare(line, "There are no results to obtain!"));

                re = new StreamReader(codaMeld.FullName);
                line = re.ReadLine();
                re.Close();
                codaFail = (0 == String.Compare(line, "There are no results to obtain!"));

                if (PetraFail && !codaFail)
                {
                    // This should not be possible by definition...
                    throw new Exception();
                }
                else if (PetraFail)
                {
                    Validity val = Validity.FailPetra_Sampling;
                    if (!PetraFunctions.CheckArchive(m_Comp, stem, jobStem, loop.Length, ref val, true))
                    {
                        return Validity.Invalid;
                    }
                    else
                    {
                        return val;
                    }
                }
                else if (codaFail)
                {
                    Validity val = Validity.FailCoda_Sampling;
                    if (!PetraFunctions.CheckArchive(m_Comp, stem, jobStem, loop.Length, ref val, true))
                    {
                        return Validity.Invalid;
                    }
                    else
                    {
                        return val;
                    }
                }
            }

            if (coda.Exists && coda.Length > 0 && Petra.Exists && Petra.Length > 0 && bz2.Exists && bz2.Length > 0)
            {
                return Validity.Valid;
            }
            else
            {
                //DirectoryInfo di = new DirectoryInfo(stem);
                //FileInfo[] files = di.GetFiles(jobStem + ".*");
                //foreach (FileInfo file in files)
                //{
                //    file.Delete();
                //}
                return Validity.Invalid;
            }
        }

        protected static bool CheckArchive(CompressedFileManager _Comp, string path, string jobStem, int loopLength, ref Validity valid, bool manageCompression)
        {
            bool error = false;
            string archive = path + jobStem + ".tar.bz2";
            DirectoryInfo di = _Comp.OutPath(archive);
            string longStem = di.FullName + Path.DirectorySeparatorChar + jobStem;

            if( manageCompression ) _Comp.Uncompress(archive);

            FileInfo fread = new FileInfo(longStem + ".fread");
            if (!fread.Exists)
            {
                error = true;
                goto END;
            }
            FileInfo read7 = new FileInfo(longStem + ".read7");
            if (!read7.Exists)
            {
                error = true;
                goto END;
            }

            FileInfo fileInfo1 = new FileInfo(longStem + ".Petra.coda");
            if (!fileInfo1.Exists)
            {
                error = true;
                goto END;
            }

            FileInfo fileInfo2 = new FileInfo(longStem + ".Petra");
            if (!fileInfo2.Exists)
            {
                StreamReader re = new StreamReader(path + jobStem + ".out");
                string line;
                bool found = false;
                while (null != (line = re.ReadLine()))
                {
                    if (0 == String.Compare(line, "No examples within restraints"))
                    {
                        line = re.ReadLine();
                        if (line != null && 0 == String.Compare(line, "Running Coda_DB:"))
                        {
                            valid = Validity.FailPetra_Sampling;
                            found = true;
                            break;
                        }
                    }

                }
                if (!found) error = true;
                re.Close();
                goto END;
            }

            FileInfo coda = new FileInfo(longStem + ".both.coda");
            if (!coda.Exists)
            {
                error = true;
                goto END;
            }

            if (valid == Validity.FailPetra_Sampling)
            {
                StreamReader re = new StreamReader(fileInfo1.FullName);
                string line;
                while (null != (line = re.ReadLine()))
                {
                    if (line[0] != '#')
                    {
                        error = true;
                        re.Close();
                        goto END;
                    }
                }
                re.Close();
            }

            if (valid == Validity.FailPetra_Sampling || valid == Validity.FailCoda_Sampling)
            {
                StreamReader re = new StreamReader(coda.FullName);
                string line;
                while (null != (line = re.ReadLine()))
                {
                    if (line[0] != '#')
                    {
                        error = true;
                        re.Close();
                        goto END;
                    }
                }
                re.Close();
            }

        END:
            if (manageCompression) _Comp.CleanUp(archive);
            return !error;
        }
    }

    class PetraInputGenerator : DSSPLoopRunGen
    {
        public PetraInputGenerator(AppLayerBase parent)
            : base(parent)
        {
        }    

        private void GenerateCondor_Start(StreamWriter rw)
        {
            rw.WriteLine("universe = vanilla");
            rw.WriteLine("should_transfer_files = YES");
            rw.WriteLine("WhenToTransferOutput = ON_EXIT");

            rw.Write("requirements = ((OpSys == \"LINUX\") && ");
            rw.Write("( (Arch == \"x86_64\") || (Arch == \"X86_64\") ||(Arch == \"INTEL\") ) && ");
            rw.WriteLine("((Machine == \"bimble1.clust\") || (Machine == \"bimble2.clust\") || (Machine == \"bimble3.clust\") || (Machine == \"bimble4.clust\") || (Machine == \"bimble5.clust\") || (Machine == \"bimble6.clust\") || (Machine == \"bimble7.clust\") || (Machine == \"bimble8.clust\") || (Machine == \"bimble9.clust\") || (Machine == \"bch-morticia.bch.bris.ac.uk\")) )");

            rw.WriteLine("Executable = launch.sh");
            rw.WriteLine("Log = submitlog.out");

            rw.WriteLine();
        }

        private void GenerateCondor(StreamWriter rw, string pdbName, string jobStem, int startResIndex, int loopLength)
        {
            rw.Write("transfer_input_files=../pdb/");
            rw.Write(pdbName);
            rw.Write(".min.pdb, ../ali/");
            rw.Write(pdbName);
            rw.WriteLine(".ali");

            rw.Write("arguments = ");
            rw.Write(pdbName);
            rw.Write(" ");
            rw.Write(startResIndex);
            rw.Write(" ");
            rw.WriteLine(loopLength);

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

        public void CreateLaunchWPetra(string execDir)
        {
            //string rootPath = "" + rootPath + "";
            string rootPath = "/local";

            StreamWriter j = new StreamWriter(execDir + "launch.sh");

            j.WriteLine("#!/bin/sh");
            j.WriteLine();
            j.WriteLine("# Exit if things go pear shaped");
            j.WriteLine("#set -e");
            j.WriteLine();
            j.WriteLine("if [ \"$1\" == \"\" ] ");
            j.WriteLine("then ");
            j.WriteLine("echo Arg 1 is not set");
            j.WriteLine("exit 1");
            j.WriteLine("fi");
            j.WriteLine();
            j.WriteLine("if [ \"$2\" == \"\" ] ");
            j.WriteLine("then ");
            j.WriteLine("echo Arg 2 is not set");
            j.WriteLine("exit 1");
            j.WriteLine("fi");
            j.WriteLine();
            j.WriteLine("if [ \"$3\" == \"\" ] ");
            j.WriteLine("then ");
            j.WriteLine("echo Arg 3 is not set");
            j.WriteLine("exit 1");
            j.WriteLine("fi");
            j.WriteLine();
            j.WriteLine("echo Args Look Good:");
            j.WriteLine("echo Host: $HOSTNAME");
            j.WriteLine("echo Stem: $1");
            j.WriteLine("echo StartRes: $2");
            j.WriteLine("echo Length: $3");
            j.WriteLine("stem=$1_$2_$3");
            j.WriteLine("echo Stem: $stem");
            j.WriteLine();
            j.WriteLine("###########################");
            j.WriteLine("# Config petra environment");
            j.WriteLine("###########################");
            j.WriteLine("LD_LIBRARY_PATH=" + rootPath + "/petra/bin");
            j.WriteLine("export LD_LIBRARY_PATH");
            j.WriteLine("TA_ORCHESTRAR_BIN=" + rootPath + "/petra/bin");
            j.WriteLine("export TA_ORCHESTRAR_BIN");
            j.WriteLine("TA_ORCHESTRAR_DB=" + rootPath + "/petra/lib");
            j.WriteLine("export TA_ORCHESTRAR_DB");
            j.WriteLine("ANDANTE_DATA=$TA_ORCHESTRAR_DB/Andante");
            j.WriteLine("export ANDANTE_DATA");
            j.WriteLine("CODA=$TA_ORCHESTRAR_DB/Coda");
            j.WriteLine("export CODA");
            j.WriteLine("JBIN=$TA_ORCHESTRAR_BIN");
            j.WriteLine("export JBIN");
            //j.WriteLine("TA_LICENSE_FILE=./license_file");
            j.WriteLine("TA_LICENSE_FILE=$TA_ORCHESTRAR_BIN/license_file");
            j.WriteLine("export TA_LICENSE_FILE");
            j.WriteLine("#printenv");
            j.WriteLine();
            j.WriteLine("#########################");
            j.WriteLine("# Run petra (AKA minus2)");
            j.WriteLine("#########################");
            j.WriteLine();
            j.WriteLine("# Sequence Messing");
            j.WriteLine("echo Running 'Joy':");
            j.WriteLine("$JBIN/joy $1.ali");
            j.WriteLine();
            j.WriteLine("# Loop Selection");
            j.WriteLine("echo Running 'Fread':");
            j.WriteLine("$JBIN/FREAD3 -pdb $1.min.pdb -seq $1.ali -startres $2 -len $3 -o $stem.fread");
            j.WriteLine("echo Running 'Read7':");
            j.WriteLine("$JBIN/Read7 -pdb $1.min.pdb -seq $1.ali -startres $2 -len $3 -o $stem.read7");
            j.WriteLine("echo Running 'Minus':");
            j.WriteLine("$JBIN/MINUS2 -pdb $1.min.pdb -seq $1.ali -startres $2 -len $3 -o $stem.petra -m $stem.read7");
            j.WriteLine();
            j.WriteLine("# Stitch loop allowing DB information");
            j.WriteLine("echo Running 'Coda_DB':");
            j.WriteLine("$JBIN/CODA3 -ff $stem.fread -mf $stem.petra -o $stem.both.coda -len $3");
            j.WriteLine("echo Running 'PyConcat_DB':");
            j.WriteLine("python $JBIN/concat_coda.py $1 $stem | $JBIN/pretuner > $stem.coda.meld");
            j.WriteLine("echo Running 'Tuner_DB':");
            j.WriteLine("$JBIN/tuner -pdb $stem.coda.meld -out $stem.coda.tuner -meld -dino ");
            j.WriteLine("echo Running 'Andante_DB':");
            j.WriteLine("$JBIN/andante -i $1 -cm $stem.coda.tuner -chi1 -chi12 -minpid 40 -minb 40 -ccc 7.0 -cbb 10.0 -o $stem.coda.pdb");
            j.WriteLine();
            j.WriteLine("# Stitch loop NOT allowing DB information - i.e. petra only");
            j.WriteLine("echo Running 'Coda_AbInitio':");
            j.WriteLine("$JBIN/CODA3 -up -ty 3 -ff $stem.fread -mf $stem.petra -o $stem.petra.coda -len $3");
            j.WriteLine("echo Running 'PyConcat_AbInitio':");
            j.WriteLine("python $JBIN/concat_petra.py $1 $stem | $JBIN/pretuner > $stem.petra.meld"); 
            j.WriteLine("echo Running 'Tuner_AbInitio':");
            j.WriteLine("$JBIN/tuner -pdb $stem.petra.meld -out $stem.petra.tuner -meld -dino ");
            j.WriteLine("echo Running 'Andante_AbInitio':");
            j.WriteLine("$JBIN/andante -i $1 -cm $stem.petra.tuner -chi1 -chi12 -minpid 40 -minb 40 -ccc 7.0 -cbb 10.0 -o $stem.petra.pdb");
            j.WriteLine();
            j.WriteLine("#########################");
            j.WriteLine("# Clean and zip complete");
            j.WriteLine("#########################");
            j.WriteLine();
            j.WriteLine("echo Removing any core dumps so that we dont bother storing them.");
            j.WriteLine("rm -f core.*");
            j.WriteLine();
            j.WriteLine("echo Run Complete. Files generated:");
            j.WriteLine("ls -oAh");
            j.WriteLine();
            j.WriteLine("echo Rename stupid .atm extension");
            j.WriteLine("mv $stem.coda.pdb.atm $stem.coda.pdb");
            j.WriteLine("mv $stem.petra.pdb.atm $stem.petra.pdb");
            j.WriteLine();
            j.WriteLine("echo Compression Commencing...");
            j.WriteLine("# Compress all things that we have made except the condor logs and the two final files, ");
            j.WriteLine("# which are more accessible during analysis if we dont bother compressing them.");
            j.WriteLine("# All other generated data is available, but compressed.");
            j.WriteLine("tar --remove-files --exclude \"$stem.coda.meld\" --exclude \"$stem.petra.meld\" --exclude \"$stem.coda.pdb\" --exclude \"$stem.petra.pdb\" --exclude \"$stem.out\" --exclude \"$stem.err\" -cvjf $stem.tar.bz2 *");
            j.WriteLine("echo Compression Done! Listing remaining files:");
            j.WriteLine("ls -oAh");

            j.Close();
        }

        private void GenSequenceFile(DSSPFile file, string pdbFileStem )
        {
            string pdbPath = baseDirectory.FullName + Path.DirectorySeparatorChar + "pdb" + Path.DirectorySeparatorChar;
            string pdbName = pdbPath + pdbFileStem + ".min.pdb";
            string aliName = 
                baseDirectory.FullName + Path.DirectorySeparatorChar + "ali" + Path.DirectorySeparatorChar +
                pdbFileStem + ".ali";

            if (File.Exists(aliName)) return;
            
            PDB pdbFile = new PDB(pdbName, false);
            string seq = pdbFile.SequenceInfo.getSEQRESSeq('A');

            StreamWriter rw = new StreamWriter(aliName);
            
            rw.Write( ">P1;" );
            rw.Write(pdbFileStem);
            rw.WriteLine(".min");
            rw.WriteLine("structure");
            rw.Write( seq );
            rw.WriteLine('*');

            rw.WriteLine(">P1;target");
            rw.WriteLine("sequence");
            rw.Write(seq);
            rw.WriteLine('*');

            rw.Close();
        }

        public void CreatePetraJobs()
        {
            // ensure we can save ali files
            Directory.CreateDirectory(baseDirectory.FullName + Path.DirectorySeparatorChar + "ali");

            int Total = 0;
            List<Validity> failReasons = new List<Validity>();

            // 8 because Petra can only produce loops of length 8
            for (int loopLength = 6; loopLength <= 8; loopLength++)
            {
                string pathname = scriptGenerationDirectory.FullName + loopLength.ToString() + Path.DirectorySeparatorChar;
                string resultPath = resultDirectory.FullName + loopLength.ToString() + Path.DirectorySeparatorChar;

                Directory.CreateDirectory(pathname);

                StreamWriter condorJob = new StreamWriter(pathname + "condor.job");

                GenerateCondor_Start(condorJob);

                CreateLaunchWPetra(pathname);

                ParsingFileIndex = 0; // reset IMPORTANT
                while (true)
                {
                    string currentName = CurrentFile.InternalName.Substring(0, 5);
                    GenSequenceFile(CurrentFile, currentName);

                    Trace.Write(currentName + ">: ");

                    SegmentDef[] loops = CurrentFile.GetLoops(loopLength, false, true);
                    for (int i = 0; i < loops.Length; i++)
                    {
                        // Petra SHOULDNT use the FirstResidueIndex as it even though it uses PDB not TRA
                        // The PDB files are sourced from the minimised tra files and therefore are 
                        // indexed from 0
                        int startIndex = loops[i].FirstDSSPIndex - 1;
                        int length = loops[i].Length;

                        // Petra (really stupidly) assumes that the residue start index is '1' for some reason, 
                        // therefore we are 1 based, not 0 based. The PDB library has been changed to reflect this.
                        startIndex++;

                        string currentJobStem = currentName + '_' + startIndex.ToString();
                        //char currnetInsertionCode = loops[i].FirstResidueInsertionCode;
                        //if (currnetInsertionCode != ' ')
                        // {
                        //     currentJobStem += currnetInsertionCode;
                        // }
                        currentJobStem += '_';
                        currentJobStem += length;
                        char currentChainID = currentName[4];

                        Validity valid = PetraFunctions.IsValidReturn(m_Comp,resultPath, currentJobStem, loops[i]);

                        switch (valid)
                        {
                            case Validity.Invalid:
                                GenerateCondor(condorJob, currentName, currentJobStem, startIndex, loopLength);
                                break;
                            case Validity.Valid:
                                break;
                            default:
                                failReasons.Add(valid);
                                break;
                        }

                        Total++;

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
                condorJob.Close();
            }

            StreamWriter rw = new StreamWriter( this.resultDirectory.FullName + "failures.csv");
            Array validities = Enum.GetValues(typeof(Validity));
            string[] valName = Enum.GetNames(typeof(Validity));
            foreach( Validity valI in validities)
            {
                int count = 0;
                for( int j = 0; j < failReasons.Count; j++ )
                {
                    if (valI == failReasons[j])
                    {
                        count++;
                    }
                }
                String report = String.Format("{0},{1},{2},{3},{4:p}", valI, failReasons.Count, count, Total, (double)count / (double)failReasons.Count);
                Trace.WriteLine( report);
                rw.WriteLine( report);
            }
            rw.Close();
        }
    }
}
