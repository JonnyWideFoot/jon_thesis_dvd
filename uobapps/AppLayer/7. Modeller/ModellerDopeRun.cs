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
    class ModellerDopeRun : DSSPLoopRunGen
    {
        public ModellerDopeRun(AppLayerBase parent)
            : base(parent)
        {
        }

        private OptimisationSchedule m_OptMode = OptimisationSchedule.very_fast;
        private ModVersion m_Version = ModVersion.v8v2;
        private TargetPlatform m_Platform = TargetPlatform.WinXP;

        public static void WriteDopeGenerator(string filename)
        {
            StreamWriter rw = new StreamWriter(filename);
            rw.WriteLine("from sys import *");
            rw.WriteLine("from array import array");
            rw.WriteLine("from modeller import *");
            rw.WriteLine("from modeller.scripts import *");
            rw.WriteLine("");
            rw.WriteLine("print \"Application Argument Info:\"");
            rw.WriteLine("print \"Count: \" + str(len(argv))");
            rw.WriteLine("print argv");
            rw.WriteLine("if len(argv) < 2 or len(argv[1]) == 0:");
            rw.WriteLine("	print \"No Stem Argument Supplied!\"");
            rw.WriteLine("	exit(-1)");
            rw.WriteLine("	");
            rw.WriteLine("modelCount = 1000");
            rw.WriteLine("stem = argv[1]");
            rw.WriteLine("");
            rw.WriteLine("env = environ()");
            rw.WriteLine("env.libs.topology.read(file='$(LIB)/top_heav.lib')");
            rw.WriteLine("env.libs.parameters.read(file='$(LIB)/par.lib')");
            rw.WriteLine("");
            rw.WriteLine("data = array('f')");
            rw.WriteLine("");
            rw.WriteLine("for i in range(modelCount):");
            rw.WriteLine("");
            rw.WriteLine("	fileName = stem + \"\\\\\" + stem + \".BL\"   ");
            rw.WriteLine("	num  = i+1 ");
            rw.WriteLine("	if num < 1000:");
            rw.WriteLine("		fileName += '0'");
            rw.WriteLine("	if num < 100:");
            rw.WriteLine("		fileName += '0'");
            rw.WriteLine("	if num < 10:");
            rw.WriteLine("		fileName += '0'    ");
            rw.WriteLine("");
            rw.WriteLine("	fileName += str(num) + \"0001.pdb\"");
            rw.WriteLine("	");
            rw.WriteLine("	print \"Loading: \" + fileName");
            rw.WriteLine("");
            rw.WriteLine("	mdl = model(env)");
            rw.WriteLine("	mdl.read(file=fileName)");
            rw.WriteLine("	aln = alignment(env)");
            rw.WriteLine("	code = stem");
            rw.WriteLine("	");
            rw.WriteLine("	# generate topology");
            rw.WriteLine("	aln.append_model(mdl, atom_files=fileName, align_codes=code)");
            rw.WriteLine("	aln.append_model(mdl, atom_files=fileName, align_codes=code+'-ini')");
            rw.WriteLine("	mdl.generate_topology(aln, sequence=code+'-ini')");
            rw.WriteLine("	mdl.transfer_xyz(aln)");
            rw.WriteLine("	");
            rw.WriteLine("	dopeScore = mdl.assess_dope()");
            rw.WriteLine("");
            rw.WriteLine("	data.append( dopeScore )");
            rw.WriteLine("	    ");
            rw.WriteLine("filename = stem + \".dope\"  	");
            rw.WriteLine("FILE = open(filename,\"w\")     ");
            rw.WriteLine("for i in range(0, modelCount):");
            rw.WriteLine("    FILE.writelines( str(i+1) + ' ' + str(data[i]) + '\\n')");
            rw.WriteLine("FILE.close()");
            rw.Close();
        }

        private void GenerateCondor_Start(StreamWriter rw)
        {
            rw.WriteLine("universe = vanilla");
            rw.WriteLine("should_transfer_files = YES");
            rw.WriteLine("WhenToTransferOutput = ON_EXIT");

            switch (m_Platform)
            {
                case TargetPlatform.RHEL4:
                    throw new NotImplementedException();
                case TargetPlatform.WinXP:
                    float kflops = 350000;
                    rw.WriteLine("requirements = ({0}(KFlops > {1}) &&((OpSys == \"WINNT50\") || (OpSys == \"WINNT51\")) && ( (Arch == \"x86_64\") || (Arch == \"X86_64\") ||(Arch == \"INTEL\") ) )", CommonFunctions.ExcludeMachineString(), kflops);
                    switch (m_Version)
                    {
                        case ModVersion.v8v2:
                            rw.WriteLine("Executable = launch_dope.bat");
                            break;
                        case ModVersion.v9v1:
                            throw new NotImplementedException();
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

        private void GenerateCondor(StreamWriter rw, string pdbName, string jobStem)
        {
            string shipZip = "";

            if (TargetPlatform.WinXP == m_Platform)
            {
                switch (m_Version)
                {
                    case ModVersion.v8v2:
                        shipZip = "../../exec/7za.exe,../../exec/Modeller8v2.zip,";
                        break;
                    case ModVersion.v9v1:
                        throw new NotImplementedException();
                    default:
                        throw new Exception();
                }
            }

            rw.Write("transfer_input_files=");
            rw.Write(shipZip);
            rw.Write("dope.py,");
            rw.Write(jobStem);
            rw.WriteLine(".tar.bz2");

            rw.Write("arguments = ");
            rw.WriteLine(jobStem);

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
                case TargetPlatform.RHEL4:
                    throw new NotImplementedException();
                case TargetPlatform.WinXP:

                    string modString = "";

                    switch (m_Version)
                    {
                        case ModVersion.v8v2:
                            modString = "8v2";
                            break;
                        case ModVersion.v9v1:
                            throw new NotImplementedException();
                        default:
                            throw new Exception();
                    }

                    launchJob = new StreamWriter(execDir + "launch_dope.bat");

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
                    launchJob.WriteLine("7za e %1.tar.bz2");
                    launchJob.WriteLine("7za e -o%1 %1.tar");
                    launchJob.WriteLine();
                    launchJob.WriteLine(".\\Modeller" + modString + "\\bin\\mod" + modString + ".exe dope.py %1");
                    launchJob.WriteLine();
                    launchJob.WriteLine("rem Remove the modeller run folders so they are not sent back");
                    launchJob.WriteLine("rmdir /S /Q Modeller" + modString + "");
                    launchJob.WriteLine("rmdir /S /Q %1");
                    launchJob.WriteLine();
                    launchJob.WriteLine("move dope.log %1.log");
                    launchJob.WriteLine();
                    launchJob.WriteLine("rem Clean up what we sent too");
                    launchJob.WriteLine("del /F /Q %1.tar");
                    launchJob.WriteLine("del /F /Q %1.tar.bz2");
                    launchJob.WriteLine("del /F /Q Modeller" + modString + ".zip");
                    launchJob.WriteLine("del /F /Q 7za.exe");
                    launchJob.WriteLine("del /F /Q dope.py");

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

                StreamWriter condorJob = new StreamWriter(pathname + "condor.job");

                GenerateCondor_Start(condorJob);
                CreateLaunchWrapper(pathname);
                WriteDopeGenerator(pathname + "dope.py");

                ParsingFileIndex = 0; // reset IMPORTANT
                while (true)
                {
                    Trace.Write("Doing " + CurrentFile.fileInfo.Name + ".. ");

                    SegmentDef[] loops = CurrentFile.GetLoops(loopLength, false, true);
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

                        Validity valid = ModellerFunctions.IsValidReturn(resultStem, currentJobStem, m_OptMode);
                        if (valid == Validity.Invalid)
                        {
                            continue;
                            //throw new Exception("We need valid modeller output here!");
                        }
                        else if( valid == Validity.MethodFail )
                        {
                            continue;
                        }
                        else if( !File.Exists( resultStem + currentJobStem + ".dope") )
                        {
                            //GenerateJob(currentName, pathname, currentJobStem, currentChainID, startIndex, lastIndex);
                            GenerateCondor(condorJob, currentName, currentJobStem);
                            Trace.Write('x');
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
