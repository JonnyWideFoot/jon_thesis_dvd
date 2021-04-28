using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using UoB.Compression;

using UoB.Core.FileIO.DSSP;
using UoB.Methodology.DSSPAnalysis;
using UoB.AppLayer.Common;

namespace UoB.AppLayer.Modeller
{
    public enum OptimisationSchedule
    {
        None,
        very_fast,
        fast,
        slow,
        very_slow
    }

    enum ModVersion
    {
        v8v2,
        v9v1
    }

    enum TargetPlatform
    {
        Grendel,
        RHEL4,
        WinXP
    }

    public enum Validity
    {
        Valid,
        Invalid,
        MethodFail
    }

    public class ModellerFunctions
    {
        private ModellerFunctions()
        {
        }

        public static readonly string PDB_STEM = ".min.pdb";

        public static Validity IsValidReturn(string resultStem, string jobStem, OptimisationSchedule optSched)
        {
            FileInfo delErrFile = new FileInfo(resultStem + jobStem + ".err");
            if (delErrFile.Exists && delErrFile.Length == 0) delErrFile.Delete();

            string archiveFileName = resultStem + jobStem + ".tar.bz2";

            FileInfo file = new FileInfo(archiveFileName);
            // Some seem oddly incomplete
            if (!file.Exists || file.Length < 1500000) // 1.5mb
            {
                return Validity.Invalid;
            }

            // Test the .py file for the correct optSched
            string pyName = resultStem + jobStem + ".py";
            if( !File.Exists(pyName))
            {
                Trace.WriteLine("WARNING: Cannot test .py file for corresponding optSched");
            }
            else
            {
                StreamReader re = new StreamReader(pyName);
                string line = null;
                bool weird = false;
                for (int i = 0; i < 19; i++ )
                {
                    line = re.ReadLine();
                    if (line == null)
                    {
                        //throw new NullReferenceException();
                        weird = true;
                        break;
                    }
                }
                if (weird)
                {                   
                    re.BaseStream.Position = 0;
                    re.DiscardBufferedData();
                    line = re.ReadLine();
                    for(int i = 0; i < line.Length; i++ )
                    {
                        if( line[i] != '\0' ) throw new Exception();                        
                    }
                    Trace.WriteLine("WARNING: .py file odity!!");
                }
                else
                {
                    string comp = optSched.ToString();
                    if (0 != String.Compare(line, line.Length - comp.Length, comp, 0, comp.Length, true))
                    {
                        throw new Exception();
                    }
                }
                re.Close();
            }

            return Validity.Valid;
        }

        public static string AssertArchive(CompressedFileManager m_Comp, string archive, string stem, int m_ModelCount, bool uncompress, bool clean)
        {
            if( uncompress ) m_Comp.Uncompress(archive);
            string archivePath = m_Comp.OutPath(archive).FullName + Path.DirectorySeparatorChar;

            // These files are all not needed, although they signify that the run went to plan.
            string pyFile = archivePath + stem + ".py";
            if (!File.Exists(pyFile))
            {
                Trace.WriteLine("Warning, could not findin archive: " + pyFile);
            }
            string ilFile = archivePath + stem + ".IL00000001.pdb";
            if (!File.Exists(ilFile))
            {
                throw new FileNotFoundException();
            }

            for (int i = 1; i <= m_ModelCount; i++)
            {
                string blFile = String.Concat(archivePath, stem, ".BL", i.ToString().PadLeft(4, '0'), "0001.pdb");
                if (!File.Exists(blFile))
                {
                    throw new FileNotFoundException(blFile);
                }
                string dlFile = String.Concat(archivePath, stem, ".DL", i.ToString().PadLeft(4, '0'), "0001");
                if (!File.Exists(dlFile))
                {
                    throw new FileNotFoundException(dlFile);
                }
            }

            FileInfo logFile = new FileInfo(archivePath + stem + ".log");
            if (!logFile.Exists)
            {
                throw new FileNotFoundException("Cannot find logFile");
            }

            if (clean) m_Comp.CleanUp(archive);

            return archivePath;
        }
    }

    class ModellerInputGenerator : DSSPLoopRunGen
    {
        private OptimisationSchedule m_OptMode = OptimisationSchedule.very_slow;
        private ModVersion m_Version = ModVersion.v8v2;
        private TargetPlatform m_Platform = TargetPlatform.Grendel;

        //private string m_GrendelNodeType = ",nodes=1:fast";
        private string m_GrendelNodeType = ",nodes=1:std:smalltmp";
        //private string m_GrendelNodeType = "";

        //private OptimisationSchedule m_OptMode = OptimisationSchedule.very_fast;
        //private ModVersion m_Version = ModVersion.v8v2;
        //private TargetPlatform m_Platform = TargetPlatform.WinXP;

        public ModellerInputGenerator(AppLayerBase parent)
            : base(parent)
        {
        }

        private void GenerateJob(string name, string pathRoot, string jobStem, char chainID, int startResIndex, int lastResIndex)
        {
            string filename = pathRoot + jobStem + ".py";
            StreamWriter rw = new StreamWriter(filename);

            rw.WriteLine("from modeller.automodel import *");
            rw.WriteLine();
            rw.WriteLine("log.verbose()");
            rw.WriteLine("env = environ()");
            rw.WriteLine();
            rw.WriteLine("env.io.atom_files_directory = './'");
            rw.WriteLine("env.io.output_directory = './'");
            rw.WriteLine();

            switch (m_Version)
            {
                case ModVersion.v8v2:
                    rw.WriteLine("class myloop(loopmodel):");
                    break;
                case ModVersion.v9v1:
                    rw.WriteLine("class myloop(dopehr_loopmodel):"); // The new and sophisticated one
                    break;                    
                default:
                    throw new Exception();
            }           

            rw.WriteLine("    def select_loop_atoms(self):");

            switch (m_Version)
            {
                case ModVersion.v8v2:
                    rw.WriteLine("        self.pick_atoms(selection_segment=('"
                        + startResIndex.ToString()
                        + ":"
                        + 'A' // all our chainIDs were fixed as 'A' by the rebuilder / minimiser
                        + "', '"
                        + lastResIndex.ToString()
                        + ":"
                        + 'A'
                        + "'), selection_status='INITIALIZE')");
                    break;
                case ModVersion.v9v1:
                    rw.WriteLine("        return selection(self.residue_range('"
                        + startResIndex.ToString()
                        + ":"
                        + 'A' // all our chainIDs were fixed as 'A' by the rebuilder / minimiser
                        + "', '"
                        + lastResIndex.ToString()
                        + ":"
                        + 'A'
                        + "'))");    
                    break;
                default:
                    throw new Exception();
            }

            rw.WriteLine();
            rw.WriteLine("m = myloop(env,");
            rw.WriteLine("           inimodel='" + name + ModellerFunctions.PDB_STEM + "',");
            rw.WriteLine("           sequence='" + jobStem + "')");
            rw.WriteLine();

            switch (m_OptMode)
            {
                case OptimisationSchedule.None:
                    rw.WriteLine("m.loop.starting_model = 0");
                    rw.WriteLine("m.loop.ending_model  = 1000");
                    rw.WriteLine("m.loop.md_level = None");                    
                    break;
                case OptimisationSchedule.very_fast:
                    rw.WriteLine("m.loop.starting_model = 0");
                    rw.WriteLine("m.loop.ending_model  = 1000");
                    rw.WriteLine("m.loop.md_level = refine.very_fast");
                    break;
                case OptimisationSchedule.fast:
                    rw.WriteLine("m.loop.starting_model = 0");
                    rw.WriteLine("m.loop.ending_model  = 1000");
                    rw.WriteLine("m.loop.md_level = refine.fast");
                    break;
                case OptimisationSchedule.slow:
                    rw.WriteLine("m.loop.starting_model = 0");
                    rw.WriteLine("m.loop.ending_model  = 1000");
                    rw.WriteLine("m.loop.md_level = refine.slow");
                    break;
                case OptimisationSchedule.very_slow:
                    rw.WriteLine("m.loop.starting_model = 0");
                    rw.WriteLine("m.loop.ending_model  = 1000");
                    rw.WriteLine("m.loop.md_level = refine.very_slow");
                    break;
                default:
                    throw new NotImplementedException();
            }

            rw.WriteLine();
            rw.WriteLine("m.make()");

            rw.Close();
        }

        private void GenerateCondor_Start(StreamWriter rw)
        {
            if (m_Platform == TargetPlatform.Grendel)
            {
                // Nothing required
                return;
            }

            rw.WriteLine("universe = vanilla");
            rw.WriteLine("should_transfer_files = YES");
            rw.WriteLine("WhenToTransferOutput = ON_EXIT");

            switch( m_Platform )
            {
                case TargetPlatform.RHEL4:
                    rw.WriteLine("requirements = ({0}(OpSys == \"LINUX\") && ( (Arch == \"x86_64\") || (Arch == \"X86_64\") ||(Arch == \"INTEL\") ) )", CommonFunctions.ExcludeMachineString());
                    rw.WriteLine("Executable = launch.sh");
                    break;
                case TargetPlatform.WinXP:
                    float kflops = 150000;
                    if (this.m_OptMode == OptimisationSchedule.very_slow)
                    {
                        kflops = 730000;
                    }
                    rw.WriteLine("requirements = ({0}(KFlops > {1}) &&((OpSys == \"WINNT50\") || (OpSys == \"WINNT51\")) && ( (Arch == \"x86_64\") || (Arch == \"X86_64\") ||(Arch == \"INTEL\") ) )", CommonFunctions.ExcludeMachineString(), kflops);
                    switch (m_Version)
                    {
                        case ModVersion.v8v2:
                            rw.WriteLine("Executable = launch.bat");
                            break;
                        case ModVersion.v9v1:
                            rw.WriteLine("Executable = ../../exec_9v1/launch.bat");
                            break;
                        default:
                            throw new Exception();
                    }
                    break;
                default:
                    throw new Exception();
            }

            rw.WriteLine("Log = submitlog.out");

            rw.WriteLine();
        }

        private void GenerateCondor(StreamWriter rw, string pdbName, string jobStem, int loopLength)
        {
            if (m_Platform == TargetPlatform.Grendel)
            {
                rw.WriteLine("qsub -o {0}.out -e {0}.err {0}.sh", jobStem);

                StreamWriter rwJob = new StreamWriter( scriptGenerationDirectory.FullName + Path.DirectorySeparatorChar + loopLength.ToString() + Path.DirectorySeparatorChar + jobStem + ".sh");

                rwJob.WriteLine("#!/bin/sh");
                rwJob.WriteLine("#PBS -l walltime={0}:00:00{1}",loopLength>=10?144:72,m_GrendelNodeType);
                rwJob.WriteLine("");
                rwJob.WriteLine("#########################");
                rwJob.WriteLine("# Setup");
                rwJob.WriteLine("#########################");
                rwJob.WriteLine("");
                rwJob.WriteLine("echo PBS Launching Script In:");
                rwJob.WriteLine("pwd");
                rwJob.WriteLine("echo Moving To Run Dir");
                rwJob.WriteLine("cd ~/{0}",loopLength);
                rwJob.WriteLine("");
                rwJob.WriteLine("# store the current folder");
                rwJob.WriteLine("JHOME=$PWD");
                rwJob.WriteLine("export JHOME");
                rwJob.WriteLine("echo Submit dir was: $JHOME");
                rwJob.WriteLine("");
                rwJob.WriteLine("# Setup the working environment");
                rwJob.WriteLine("if [ -z \"$JOBNO\" ]");
                rwJob.WriteLine("then");
                rwJob.WriteLine("        WORKDIR=/tmp/jr0407/{0}",jobStem);
                rwJob.WriteLine("else");
                rwJob.WriteLine("        WORKDIR=/tmp/jr0407/{0}_$JOBNO",jobStem);
                rwJob.WriteLine("fi");
                rwJob.WriteLine("export WORKDIR");
                rwJob.WriteLine("echo Using workdir: $WORKDIR");
                rwJob.WriteLine("");
                rwJob.WriteLine("#clean out any existing muck");
                rwJob.WriteLine("rm -rf $WORKDIR");
                rwJob.WriteLine("mkdir -p $WORKDIR");
                rwJob.WriteLine("cp ../pdb/{0}.min.pdb $WORKDIR",pdbName);
                rwJob.WriteLine("cp ../exec/Modeller8v2.tar.gz $WORKDIR");
                rwJob.WriteLine("cp {0}.py $WORKDIR",jobStem);
                rwJob.WriteLine("cd $WORKDIR");
                rwJob.WriteLine("");
                rwJob.WriteLine("MODINSTALL8v2=./Modeller8v2");
                rwJob.WriteLine("export MODINSTALL8v2");
                rwJob.WriteLine("KEY_MODELLER8v2=MODELIRANJE");
                rwJob.WriteLine("export KEY_MODELLER8v2");
                rwJob.WriteLine("# Provide do-nothing Python libraries if standard copies aren't on this system");
                rwJob.WriteLine("if test -z \"${PYTHONHOME}\"; then");
                rwJob.WriteLine("  if test ! -d /usr/lib/python2.3 \\");
                rwJob.WriteLine("       -a ! -d /System/Library/Frameworks/Python.framework/Versions/2.3/lib \\");
                rwJob.WriteLine("       -a ! -d /usr/local/lib/python2.3; then");
                rwJob.WriteLine("    PYTHONHOME=${MODINSTALL8v2}/bin/");
                rwJob.WriteLine("    export PYTHONHOME");
                rwJob.WriteLine("  fi");
                rwJob.WriteLine("fi");
                rwJob.WriteLine("");
                rwJob.WriteLine("# to avoid running out of stack space");
                rwJob.WriteLine("ulimit -S -s unlimited");
                rwJob.WriteLine("");
                rwJob.WriteLine("# store name of node and scratch directory in file rundat");
                rwJob.WriteLine("hostname > {0}.rundat", jobStem);
                rwJob.WriteLine("pwd >> {0}.rundat", jobStem);
                rwJob.WriteLine("");
                rwJob.WriteLine("tar xvzf Modeller8v2.tar.gz");
                rwJob.WriteLine("");
                rwJob.WriteLine("echo These files are here!");
                rwJob.WriteLine("ls");
                rwJob.WriteLine("");
                rwJob.WriteLine("#########################");
                rwJob.WriteLine("# Run");
                rwJob.WriteLine("#########################");
                rwJob.WriteLine("");
                rwJob.WriteLine("chmod 700 ./Modeller8v2/bin/mod8v2_i386-intel8");
                rwJob.WriteLine("./Modeller8v2/bin/mod8v2_i386-intel8 {0}.py",jobStem);
                rwJob.WriteLine("");
                rwJob.WriteLine("#########################");
                rwJob.WriteLine("# Clean and zip complete");
                rwJob.WriteLine("#########################");
                rwJob.WriteLine("");
                rwJob.WriteLine("echo Run Complete. Files generated:");
                rwJob.WriteLine("ls -oAh");
                rwJob.WriteLine("");
                rwJob.WriteLine("echo Remove the modeller items so they are not sent back");
                rwJob.WriteLine("rm -rf Modeller8v2");
                rwJob.WriteLine("rm -f Modeller8v2.tar.gz");
                rwJob.WriteLine("");
                rwJob.WriteLine("echo Compression Commencing...");
                rwJob.WriteLine("tar --remove-files -cvjf {0}.tar.bz2 *", jobStem);
                rwJob.WriteLine("");
                rwJob.WriteLine("# Return the files to the home node");
                rwJob.WriteLine("mv -u * $JHOME");
                rwJob.WriteLine("rm -rf $WORKDIR");

                rwJob.Close();

                return;
            }

            string shipZip = "";

            if (TargetPlatform.WinXP == m_Platform)
            {
                switch (m_Version)
                {
                    case ModVersion.v8v2:
                        shipZip = "../../exec/7za.exe,../../exec/Modeller8v2.zip,";
                        break;
                    case ModVersion.v9v1:
                        shipZip = "../../exec_9v1/7za.exe,../../exec_9v1/Modeller9v1.zip,";
                        break;
                    default:
                        throw new Exception();
                }
            }

            rw.Write("transfer_input_files=");
            rw.Write(shipZip);
            rw.Write(jobStem);
            rw.Write(".py,../../pdb/");
            rw.Write(pdbName);
            rw.WriteLine(ModellerFunctions.PDB_STEM);

            rw.Write("arguments = ");
            rw.Write(jobStem);

            switch (m_Platform)
            {
                case TargetPlatform.RHEL4:
                    rw.WriteLine(".py");
                    break;
                case TargetPlatform.WinXP:
                    rw.WriteLine();
                    break;
                default:
                    throw new Exception();
            }            

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
            StreamWriter launchJob = null;

            switch (m_Platform)
            {
                case TargetPlatform.Grendel:
                    return;
                case TargetPlatform.RHEL4:
                    if (m_Version != ModVersion.v8v2) throw new Exception();

                    launchJob = new StreamWriter(execDir + "launch.sh");
                    launchJob.WriteLine("#!/bin/sh");
                    //launchJob.WriteLine("export PATH=$PATH:/usr/X11R6/bin");
                    launchJob.WriteLine("echo Running on host: $HOSTNAME");
                    launchJob.WriteLine("echo PATH is: $PATH");
                    launchJob.WriteLine("echo Argument: '$1'");
                    launchJob.WriteLine("echo");
                    launchJob.WriteLine();

                    launchJob.WriteLine("# Launch modeller");
                    launchJob.WriteLine("mod8v2 $1");
                    launchJob.WriteLine();

                    launchJob.WriteLine("# Clean and compress");
                    launchJob.WriteLine("if [ \"$1\" = \"\" ]; then");
                    launchJob.WriteLine("echo No delete argument");
                    launchJob.WriteLine("else");
                    launchJob.WriteLine("stem=${1%.py}");
                    launchJob.WriteLine("ext=\"tar.bz2\"");
                    launchJob.WriteLine("tar -cvjf arch_$stem.$ext $stem*"); // compress the PDB output files
                    //#rm `ls | grep -v \"my_no_delete_pattern\"`
                    launchJob.WriteLine("rm -rf $stem*"); // remove the compressed files
                    launchJob.WriteLine("mv arch_$stem.$ext $stem.$ext"); // rename the archive - we had to name it wrongly to avoid its deletion!
                    launchJob.WriteLine("fi");

                    launchJob.Close();

                    break;
                case TargetPlatform.WinXP:

                    string modString = "";

                    switch (m_Version)
                    {
                        case ModVersion.v8v2:
                            modString = "8v2";                         
                            break;
                        case ModVersion.v9v1:
                            modString = "9v1";                                 
                            break;
                        default:
                            throw new Exception();
                    }

                    launchJob = new StreamWriter(execDir + "launch.bat");

                    launchJob.WriteLine("@echo off");
                    launchJob.WriteLine();
                    launchJob.WriteLine("echo Argument: '%1'");
                    launchJob.WriteLine("echo Executing on:");
                    launchJob.WriteLine("hostname");
                    launchJob.WriteLine();
                    launchJob.WriteLine("rem Extract Modeller to the local dir");
                    launchJob.WriteLine("7za x Modeller" + modString + ".zip");
                    launchJob.WriteLine();
                    launchJob.WriteLine("set MODINSTALL" + modString + "=.\\Modeller" + modString);
                    launchJob.WriteLine("set PYTHONPATH=.\\Modeller" + modString + "\\modlib");
                    launchJob.WriteLine("set KEY_MODELLER" + modString + "=MODELIRANJE");
                    launchJob.WriteLine("set LIB_ASGL=.\\Modeller" + modString + "\\asgl");
                    launchJob.WriteLine("set BIN_ASGL=.\\Modeller" + modString + "\\bin");
                    launchJob.WriteLine("set PATH=%MODINSTALL" + modString + "%\\bin;%PATH%");
                    launchJob.WriteLine();
                    launchJob.WriteLine(".\\Modeller" + modString + "\\bin\\mod" + modString + ".exe %1.py");
                    launchJob.WriteLine();
                    launchJob.WriteLine("rem Remove the modeller run folder so its not sent back");
                    launchJob.WriteLine("rmdir /S /Q Modeller" + modString + "");
                    launchJob.WriteLine();
                    launchJob.WriteLine("7za a -ttar -x!%1.out -x!%1.err arch_%1.tar %1*");
                    launchJob.WriteLine("del /F /Q %1*");
                    launchJob.WriteLine("move arch_%1.tar %1.tar");
                    launchJob.WriteLine("7za a -tbzip2 %1.tar.bz2 %1.tar");
                    launchJob.WriteLine("del /F /Q %1.tar");
                    launchJob.WriteLine();
                    launchJob.WriteLine("rem Clean up what we sent too");
                    launchJob.WriteLine("del /F /Q Modeller" + modString + ".zip");
                    launchJob.WriteLine("del /F /Q 7za.exe");
                    launchJob.WriteLine("del /F /Q *.pdb");
                    launchJob.WriteLine("del /F /Q launch.bat");

                    launchJob.Close();

                    break;
                default:
                    throw new Exception();
            }             
        }

        public void CreateModellerJobs()
        {
            for (int loopLength = 6; loopLength <= 11; loopLength++)
            {
                string pathname = scriptGenerationDirectory.FullName + loopLength.ToString() + Path.DirectorySeparatorChar;
                string resultStem =
                  this.baseDirectory.FullName +
                  @"CompleteData_8v2\ScriptGen_" + m_OptMode.ToString() + '\\' +
                  loopLength.ToString() +
                  Path.DirectorySeparatorChar;

                Directory.CreateDirectory(pathname);

                StreamWriter condorJob = null;
                if (m_Platform == TargetPlatform.Grendel)
                {
                    string submitName = pathname + "submit.sh";
                    condorJob = new StreamWriter(submitName);
                }
                else
                {
                    string submitName = pathname + "condor.job";
                    condorJob = new StreamWriter(submitName);
                    GenerateCondor_Start(condorJob);
                    CreateLaunchWrapper(pathname);
                }

                ParsingFileIndex = 0; // reset IMPORTANT
                while (true)
                {
                    Trace.Write("Doing " + CurrentFile.fileInfo.Name + ".. ");

                    SegmentDef[] loops = CurrentFile.GetLoops(loopLength,false, true);
                    for (int i = 0; i < loops.Length; i++)
                    {
                        string currentName = CurrentFile.InternalName.Substring(0, 5);

                        // MODELLER SHOULDN'T use the FirstResidueIndex as it even though it uses PDB not TRA
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
                                                
                        Validity isValid = ModellerFunctions.IsValidReturn(resultStem, currentJobStem, m_OptMode);

                        // Check the contents of the archive too? (slow!)
                        bool uberAssert = false;

                        if (isValid == Validity.Valid && uberAssert )
                        {
                            string archive = resultStem + currentJobStem + ".tar.bz2";
                            try
                            {
                                string archivePath = ModellerFunctions.AssertArchive(m_Comp, archive, currentJobStem, 1000, true, true);
                            }
                            catch (FileNotFoundException exF)
                            {
                                Trace.WriteLine("ARCHIVE File-Finding FAIL!!\r\n" + exF.ToString());
                                Trace.WriteLine(exF.Message);
                                isValid = Validity.Invalid;
                            }
                            catch (Exception ex)
                            {
                                Trace.WriteLine("ARCHIVE FAIL!!\r\n" + ex.ToString());
                                isValid = Validity.Invalid;
                            }
                        }

                        // Now deal with invalidated states
                        if (Validity.Invalid == isValid)
                        {
                            GenerateJob(currentName, pathname, currentJobStem, currentChainID, startIndex, lastIndex);
                            GenerateCondor(condorJob, currentName, currentJobStem, loopLength);
                            Trace.Write('x');
                        }
                        else if (Validity.MethodFail == isValid)
                        {
                            Trace.Write('?');
                        }
                        else
                        {
                            Trace.Write('|');
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
