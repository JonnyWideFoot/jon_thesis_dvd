using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

using UoB.Core.FileIO.DSSP;
using UoB.Methodology.DSSPAnalysis;
using UoB.AppLayer.Common;

namespace UoB.AppLayer.Plop
{
    enum Validity
    {
        Valid,
        MethodFail,
        Invalid
    }

    class PlopFunctions
    {
        private PlopFunctions()
        {
        }

        public static bool warnOnError = true;

        private static Regex m_RegexFilesize = new Regex("File size limit exceeded", RegexOptions.Compiled);

        public static Validity IsValidReturn(string dirStem, string jobStem, int startResIndex, int lastResIndex)
        {
            Validity isValid = Validity.Invalid;
            string error = "";
            string stem = dirStem + jobStem;
            bool found = false;
            string line = null;

            FileInfo dieFile = new FileInfo(stem + ".die");
            if (dieFile.Exists)
            {
                error = "Found DIE: " + stem;
                isValid = Validity.Invalid;
                goto END;
            }

            if (!Directory.Exists(dirStem))
            {
                Directory.CreateDirectory(dirStem);
            }

            string[] files = Directory.GetFiles(dirStem, jobStem + "*");
            if (files.Length == 0)
            {
                error = "No Output at all!";
                isValid = Validity.Invalid;
                goto END;
            }

            FileInfo outFile = new FileInfo(stem + ".out");

            FileInfo errFile = new FileInfo(stem + ".err");
            if (!errFile.Exists)
            {
                error = "weird - no err file!";
                isValid = Validity.Invalid;
                goto END;
            }
            if (errFile.Length > 0)
            {
                // /condor_home/execute/dir_15932/condor_exec.exe: line 6: 15935 File size limit exceeded(core dumped) /shared_mount/plop/plop

                StreamReader reErr = new StreamReader(errFile.FullName);
                string lineErr = reErr.ReadLine();
                if (m_RegexFilesize.IsMatch(lineErr))
                {
                    isValid = Validity.MethodFail;
                    error = "Bizaro Mahoosive 2gb out file - method fail";
                }
                else if (Regex.IsMatch(lineErr, "SIGSEGV"))
                {
                    isValid = Validity.MethodFail;
                    error = "Segfault";
                }
                else if (lineErr.Length > 25 && 0 == String.Compare(lineErr, lineErr.Length - 25, "No such file or directory", 0, 25, false))
                {
                    isValid = Validity.Invalid;
                    error = "Cant find plop: ";
                    if (outFile.Exists)
                    {
                        StreamReader reOut = new StreamReader(outFile.FullName);
                        error += reOut.ReadLine();
                        reOut.Close();
                    }
                    else
                    {
                        error += "No Out file";
                    }
                    error += " " + stem;
                }
                else
                {
                    isValid = Validity.Invalid;
                    error = "Found defined ERR: " + stem + "\n";
                    error += "ERR FILE: -->\n";
                    while (null != (lineErr = reErr.ReadLine()))
                    {
                        error += "\n" + lineErr;
                    }
                    error += "\n";
                    reErr.Close();

                    if (outFile.Exists)
                    {
                        error += "OUT FILE: -->\n";
                        StreamReader reOut = new StreamReader(outFile.FullName);
                        error += reOut.ReadLine();
                        error += "\n";
                        reOut.Close();
                    }
                    else
                    {
                        error += "OUT FILE: --> NULL !!!!!";
                    }
                }

                goto END;
            }

            FileInfo conFile = new FileInfo(stem + ".con");
            FileInfo pathFile = new FileInfo(stem + ".path");
            if (!conFile.Exists)
            {
                error = "ODD: no con file!";
                isValid = Validity.Invalid;
                goto END;
            }
            if (!pathFile.Exists)
            {
                error = "ODD: no path file!";
                isValid = Validity.Invalid;
                goto END;
            }

            FileInfo pdbFile = new FileInfo(stem + ".pdb");
            FileInfo rmsdFile = new FileInfo(stem + ".rmsd");

            if (!pdbFile.Exists)
            {
                error = "No PDB file" + stem;
                isValid = Validity.Invalid;
                goto END;
            }
            else if (pdbFile.Length == 0)
            {
                error = "Found empty PDB: " + stem;
                isValid = Validity.Invalid;
                goto END;
            }

            string methFailName = outFile.DirectoryName + 
                Path.DirectorySeparatorChar + 
                Path.GetFileNameWithoutExtension(outFile.FullName) + ".meth_fail";
            if (!outFile.Exists)
            {
                error = "No OUT file: " + stem;
                isValid = Validity.Invalid;
                goto END;
            }
            else if (File.Exists(methFailName))
            {
                error = "UBER OUT file: " + stem;
                isValid = Validity.MethodFail;
                goto END;
            }
            else if (outFile.Length > (1024 * 1024 * 10))
            {
                // Uber-out!!
                // This is due to a minimisation explosion I think
                StreamReader rePBD = new StreamReader(pdbFile.FullName);
                found = false;
                while (null != (line = rePBD.ReadLine()))
                {
                    if (line.Contains("*"))
                    {
                        found = true;
                        break;
                    }
                }
                rePBD.Close();

                // "*** something wrong in integration! ***"
                // " WARNING:  atom out of cell bounds:"

                if (!found)
                {
                    StreamReader reOutA = new StreamReader(outFile.FullName);
                    found = false; // check the out file is ok
                    while (null != (line = reOutA.ReadLine()))
                    {
                        if (0 == String.Compare(line, 0, "     *** something wrong in integration! ***", 0, 44, false))
                        {
                            found = true;
                            break;
                        }
                        else if (0 == String.Compare(line, 0, " WARNING:  atom out of cell bounds:", 0, 35, false))
                        {
                            found = true;
                            break;
                        }
                    }
                    reOutA.Close();
                }

                if (!found) throw new Exception("Assumption failed"); // not a minim explosion    

                // Optional disk cleanup
                try
                {
                    outFile.Delete();
                }
                catch
                {
                }

                StreamWriter rwMethFail = new StreamWriter(methFailName);
                rwMethFail.Close();

                error = "UBER OUT file: " + stem;
                isValid = Validity.MethodFail;
                goto END;
            }

            if (!rmsdFile.Exists)
            {
                error = "No RMSD file" + stem;
                isValid = Validity.Invalid;
                goto END;
            }
            else if (rmsdFile.Length == 0)
            {
                error = "Found empty RMSD: " + stem;
                StreamReader re = new StreamReader(outFile.FullName);
                bool foundBottom = false; // sampling failure resulting in empty RMSD file
                bool foundEnd = false; // check the out file is ok
                while (null != (line = re.ReadLine()))
                {
                    if (0 == String.Compare(line.Trim(), "WARNING:  sampres/ofac both bottomed out!"))
                    {
                        foundBottom = true;
                    }
                    else if (0 == String.Compare(line, 0, " TOTAL TIME ELAPSED", 0, 19, false))
                    {
                        foundEnd = true;
                        break;
                    }
                }
                re.Close();
                if (foundBottom && foundEnd)
                    isValid = Validity.MethodFail;
                goto END;
            }


            StreamReader reOutB = new StreamReader(outFile.FullName);
            found = false; // check the out file is ok
            while (null != (line = reOutB.ReadLine()))
            {
                if (0 == String.Compare(line, 0, " TOTAL TIME ELAPSED", 0, 19, false))
                {
                    found = true;
                    break;
                }
            }
            reOutB.Close();
            if (found)
            {
                error = "EXCELLENT :-D: " + stem;
                isValid = Validity.Valid;
            }
            else
            {
                error = "Not complete out file!" + stem;
                isValid = Validity.Invalid;
            }

        END:

            if (isValid != Validity.Valid)
            {
                if (warnOnError)
                {
                    Trace.WriteLine("");
                    Trace.WriteLine(error);
                }
                return isValid;
            }
            else
            {
                return isValid;
            }
        }
    }

    class PlopInputGenerator : DSSPLoopRunGen
    {
        public PlopInputGenerator(AppLayerBase parent)
            : base(parent)
        {
        }

        bool m_ObfuscateLoop = true;
        bool m_AnchorsIncluded = true;

        private void GenerateJob(string name, string pathRoot, string jobStem, char chainID, int startResIndex, int lastResIndex)
        {
            string filename = pathRoot + jobStem + ".con";
            StreamWriter rw = new StreamWriter(filename);

            //rw.WriteLine("data ./data/");
            rw.WriteLine("data /shared_mount/plop/data/");

            rw.Write("job ");
            rw.WriteLine(jobStem);

            if (m_ObfuscateLoop)
            {
                // As we are missing atom data, we now need to read the sequence from SEQRES
                // This also meand that opt(imisation) of imported atom positions must be enabled
                rw.WriteLine("load pdb {0}.db.pdb seqres yes opt yes", jobStem);
            }
            else
            {
                rw.Write("load pdb ");
                rw.Write(name);
                rw.WriteLine(".min.pdb");
            }

            if (m_AnchorsIncluded)
            {
                // below we use +-1 from the loop start and end as unless this is done, plop will not move the atoms giving the 
                // omg and phi of the start residue and the psi of the last residue.
                rw.Write("loop predict ");
                rw.Write(chainID);
                rw.Write(':');
                rw.Write((startResIndex - 1).ToString());
                rw.Write(' ');
                rw.Write(chainID);
                rw.Write(':');
                rw.Write((lastResIndex + 1).ToString());
                rw.WriteLine(" &");
            }
            else if (m_ObfuscateLoop)
            {
                rw.Write("loop predict ");
                rw.Write(chainID);
                rw.Write(':');
                rw.Write((startResIndex - 1).ToString());
                rw.Write('A'); // Start insertion code from obfuscated loop buildup                
                rw.Write(' ');
                rw.Write(chainID);
                rw.Write(':');
                rw.Write(lastResIndex.ToString());
                char endID = (char)(((int)'A') + lastResIndex - startResIndex + 1);
                rw.Write(endID);// End insertion code from obfuscated loop buildup  
                rw.WriteLine(" &");
            }
            else
            {
                rw.Write("loop predict ");
                rw.Write(chainID);
                rw.Write(':');
                rw.Write(startResIndex.ToString());
                rw.Write(' ');
                rw.Write(chainID);
                rw.Write(':');
                rw.Write(lastResIndex.ToString());
                rw.WriteLine(" &");
            }

            rw.Write("   pdbfile ");
            rw.Write(jobStem);
            rw.WriteLine(".pdb &");

            rw.Write("   rmsdfile ");
            rw.Write(jobStem);
            rw.WriteLine(".rmsd");

            rw.Close();

            // an annoying plop requirement to have the path of the control file as the standard input!!
            // We can work round this in condor using a path file and piping the contetns into the STDIN...
            filename = pathRoot + jobStem + ".path";
            rw = new StreamWriter(filename);
            rw.Write(jobStem);
            rw.WriteLine(".con");
            rw.Close();
        }

        private void GenerateCondor_Start(StreamWriter rw)
        {
            rw.WriteLine("universe = vanilla");
            rw.WriteLine("should_transfer_files = YES");
            rw.WriteLine("WhenToTransferOutput = ON_EXIT");

            rw.WriteLine("requirements = ((TotalCpus == 1) && (TotalMemory >= 490) && (OpSys == \"LINUX\") && ( (Arch == \"x86_64\") || (Arch == \"X86_64\") ||(Arch == \"INTEL\") ) )");

            rw.WriteLine("Executable = launch.sh");
            rw.WriteLine("Log = submitlog.out");

            rw.WriteLine();
        }

        private void GenerateCondor(StreamWriter rw, string pdbName, string jobStem)
        {
            rw.Write("transfer_input_files=");
            rw.Write(jobStem);
            rw.Write(".con,");

            //  Old versions that used to 'send' plop over condor
            //rw.WriteLine(".pdb,plop,libguide.so,libsvml.so,libimf.so,data/contact.dat,data/newsgb.pair,data/params.nb,data/res_table.dat,data/sgb.sncorrfnprm.noself,data/loop.dist,data/params.cov,data/params.penalty,data/similar.dat,data/temp_mine/ala,data/temp_mine/arg,data/temp_mine/asn,data/temp_mine/asp,data/temp_mine/cys,data/temp_mine/gln,data/temp_mine/glu,data/temp_mine/glub,data/temp_mine/gly,data/temp_mine/his,data/temp_mine/ile,data/temp_mine/leu,data/temp_mine/lys,data/temp_mine/lyse,data/temp_mine/met,data/temp_mine/phe,data/temp_mine/pro,data/temp_mine/ser,data/temp_mine/thr,data/temp_mine/trp,data/temp_mine/tyr,data/temp_mine/val,data/rotamers/ARG.side,data/rotamers/ASP.side,data/rotamers/GEN.back,data/rotamers/GLU.side,data/rotamers/GLY.back,data/rotamers/ILE.side,data/rotamers/LYS.side,data/rotamers/PHE.side,data/rotamers/PRO.side,data/rotamers/THR.side,data/rotamers/TYR.side,data/rotamers/ASN.side,data/rotamers/CYS.side,data/rotamers/GLN.side,data/rotamers/HIS.side,data/rotamers/LEU.side,data/rotamers/MET.side,data/rotamers/PRO.back,data/rotamers/SER.side,data/rotamers/TRP.side,data/rotamers/VAL.side,data/temp_mine/argb,data/temp_mine/asnb,data/temp_mine/glnb,data/temp_mine/hidb,data/temp_mine/hisb,data/temp_mine/leub,data/temp_mine/lysb,data/temp_mine/metb,data/temp_mine/pheb,data/temp_mine/prob,data/temp_mine/thrb,data/temp_mine/trpb,data/temp_mine/tyob,data/temp_mine/valb,data/temp_mine/alae,data/temp_mine/alab");
            //rw.WriteLine(".min.pdb,plop,libguide.so,libsvml.so,libimf.so,data/contact.dat,data/newsgb.pair,data/params.nb,data/res_table.dat,data/sgb.sncorrfnprm.noself,data/loop.dist,data/params.cov,data/params.penalty,data/similar.dat,data/rotamers/GEN.back,data/rotamers/CYS.side,data/rotamers/ASP.side,data/rotamers/GLU.side,data/rotamers/PHE.side,data/rotamers/GLY.back,data/rotamers/HIS.side,data/rotamers/ILE.side,data/rotamers/LYS.side,data/rotamers/LEU.side,data/rotamers/MET.side,data/rotamers/ASN.side,data/rotamers/PRO.side,data/rotamers/PRO.back,data/rotamers/GLN.side,data/rotamers/ARG.side,data/rotamers/SER.side,data/rotamers/THR.side,data/rotamers/VAL.side,data/rotamers/TRP.side,data/rotamers/TYR.side,data/temp_mine/ala,data/temp_mine/argb,data/temp_mine/asne,data/temp_mine/aspz,data/temp_mine/gln,data/temp_mine/glub,data/temp_mine/glye,data/temp_mine/hie,data/temp_mine/hipe,data/temp_mine/ile,data/temp_mine/leub,data/temp_mine/lyse,data/temp_mine/metz,data/temp_mine/pro,data/temp_mine/sere,data/temp_mine/thrz,data/temp_mine/tyr,data/temp_mine/valb,data/temp_mine/alab,data/temp_mine/arge,data/temp_mine/asnz,data/temp_mine/cys,data/temp_mine/glnb,data/temp_mine/glue,data/temp_mine/glyz,data/temp_mine/hieb,data/temp_mine/his,data/temp_mine/ileb,data/temp_mine/leue,data/temp_mine/lysz,data/temp_mine/phe,data/temp_mine/prob,data/temp_mine/serz,data/temp_mine/trp,data/temp_mine/tyrb,data/temp_mine/vale,data/temp_mine/alae,data/temp_mine/argz,data/temp_mine/asp,data/temp_mine/cysb,data/temp_mine/glne,data/temp_mine/gluz,data/temp_mine/hid,data/temp_mine/hiee,data/temp_mine/hisb,data/temp_mine/ilee,data/temp_mine/leuz,data/temp_mine/met,data/temp_mine/pheb,data/temp_mine/proe,data/temp_mine/thr,data/temp_mine/trpb,data/temp_mine/tyre,data/temp_mine/valz,data/temp_mine/alaz,data/temp_mine/asn,data/temp_mine/aspb,data/temp_mine/cyse,data/temp_mine/glnz,data/temp_mine/gly,data/temp_mine/hidb,data/temp_mine/hip,data/temp_mine/hise,data/temp_mine/ilez,data/temp_mine/lys,data/temp_mine/metb,data/temp_mine/phee,data/temp_mine/ser,data/temp_mine/thrb,data/temp_mine/trpe,data/temp_mine/tyrz,data/temp_mine/arg,data/temp_mine/asnb,data/temp_mine/aspe,data/temp_mine/cysz,data/temp_mine/glu,data/temp_mine/glyb,data/temp_mine/hide,data/temp_mine/hipb,data/temp_mine/hisz,data/temp_mine/leu,data/temp_mine/lysb,data/temp_mine/mete,data/temp_mine/phez,data/temp_mine/serb,data/temp_mine/thre,data/temp_mine/trpz,data/temp_mine/val");

            if (m_ObfuscateLoop)
            {
                rw.Write("../pdb/");
                rw.Write(jobStem);
                rw.WriteLine(".db.pdb");
            }
            else
            {
                rw.Write("../pdb_source/");
                rw.Write(pdbName);
                rw.WriteLine(".min.pdb");
            }

            rw.Write("Input = ");
            rw.Write(jobStem);
            rw.WriteLine(".path");

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
            launchJob.WriteLine("export PATH=$PATH:/usr/X11R6/bin");
            launchJob.WriteLine("echo Running on host: $HOSTNAME");
            launchJob.WriteLine("echo PATH is: $PATH");
            launchJob.WriteLine("echo");
            //launchJob.WriteLine("/lib/ld-linux.so.2 --library-path ./ ./plop");
            launchJob.WriteLine("/shared_mount/plop/plop");
            launchJob.Close();
        }

        public void CreatePlopJobs()
        {
            for (int loopLength = 6; loopLength <= 11; loopLength++)
            {
                string pathname = scriptGenerationDirectory.FullName + loopLength.ToString() + Path.DirectorySeparatorChar;
                string resultStem = resultDirectory.FullName + loopLength.ToString() + Path.DirectorySeparatorChar;

                Directory.CreateDirectory(pathname);

                StreamWriter condorJob = new StreamWriter(pathname + "condor.job");

                GenerateCondor_Start(condorJob);

                CreateLaunchWrapper(pathname);

                ParsingFileIndex = 0; // reset IMPORTANT
                while (true)
                {
                    SegmentDef[] loops = CurrentFile.GetLoops(loopLength, false, true);

                    string currentName = CurrentFile.InternalName.Substring(0, 5);
                    Trace.Write(currentName);
                    Trace.Write("> ");

                    for (int i = 0; i < loops.Length; i++)
                    {
                        // PLOP SHOULDNT use the FirstResidueIndex as it even though it uses PDB not TRA
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

                        if (Validity.Invalid == PlopFunctions.IsValidReturn(resultStem, currentJobStem, startIndex, lastIndex))
                        {
                            GenerateJob(currentName, pathname, currentJobStem, currentChainID, startIndex, lastIndex);
                            GenerateCondor(condorJob, currentName, currentJobStem);
                            if (m_ObfuscateLoop) CreatePDBFile(currentName, currentJobStem, Obfuscation.Delete, startIndex, loopLength, ".db.pdb", false, false);
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
                condorJob.Close();
            }
        }
    }
}
