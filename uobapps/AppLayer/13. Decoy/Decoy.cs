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

namespace UoB.AppLayer.Decoy
{
    enum Validity
    {
        Valid,
        Invalid
    }

    class NativePertFunctions
    {
        private NativePertFunctions()
        {
        }

        public static Validity IsValidReturn(string resultPath, string currentJobStem)
        {
            string outFile = resultPath + currentJobStem + ".out";
            if( !File.Exists( resultPath + currentJobStem + ".tra" ) )return Validity.Invalid;
            if (!File.Exists(outFile)) return Validity.Invalid;
            if (!File.Exists(resultPath + currentJobStem + ".err")) return Validity.Invalid;

            string line;
            StreamReader re = new StreamReader( outFile);
            while (null != (line = re.ReadLine()))
            {
                if( 0 == string.Compare(line,0,"ff.gbsa: Using GB_Still parameters",0,34,false))
                {
                    int i = 0;
                    while (null != (line = re.ReadLine()))
                    {
                        if( 0 == string.Compare(line,0,"InValid",0,7,false) ||
                            0 == string.Compare(line,0,"*Valid*",0,7,false) )
                        {
                            i++;
                        }
                    }
                    if (i == 500)
                    {
                        return Validity.Valid;
                    }
                    else
                    {
                        return Validity.Invalid;
                    }
                }
            }
            return Validity.Invalid;            
        }
    }

    class DecoyGenerator : DSSPLoopRuns
    {
        public DecoyGenerator(AppLayerBase parent)
            : base(parent, false, ProcessPriorityClass.Idle )
        {
            StartLoopLength = 8;
            EndLoopLength = 8;

            m_Cloop = parent.DriveRoot("cloop") + "cloop\\Result_c30b2\\";
            m_Petra = parent.DriveRoot("petra") + "petra\\result\\";
            m_PreArcus = parent.DriveRoot("prearcus") + "prearcus\\";
            m_Rapper = parent.DriveRoot("rapper") + "rapper\\result\\";
            m_Plop = parent.DriveRoot("plop") + "plop\\result\\";
            m_Modeller = parent.DriveRoot("modeller") + "modeller\\";
            m_NatPert = parent.DriveRoot("Decoy") + "decoy\\nat_pert\\";
        }

        private string m_Cloop;
        private string m_Petra;
        private string m_PreArcus;
        private string m_Rapper;
        private string m_Plop;
        private string m_Modeller;
        private string m_NatPert;

        #region Native Perturbation

        private void GenerateCondor_Start(StreamWriter rw)
        {
            rw.WriteLine("universe = vanilla");
            rw.WriteLine("should_transfer_files = YES");
            rw.WriteLine("WhenToTransferOutput = ON_EXIT");

            rw.WriteLine("requirements = ({0} (KFlops > {1}) && ((OpSys == \"WINNT50\") || (OpSys == \"WINNT51\")) && ( (Arch == \"x86_64\") || (Arch == \"X86_64\") ||(Arch == \"INTEL\")))"
                ,CommonFunctions.ExcludeMachineString(), 700000 );

            rw.WriteLine("Executable = ../exec/nativepert.exe");
            rw.WriteLine("Log = submitlog.out");

            rw.WriteLine();
        }

        private void GenerateCondor(StreamWriter rw, string pdbName, string jobStem, int startResIndex, int loopLength)
        {
            rw.Write("transfer_input_files=../exec/lib/amber03aa.ff,../exec/lib/dict.pdb,../exec/lib/default.class,../exec/lib/default.alias,../exec/lib/quotes.dat,../pdb_min/");
            rw.Write(pdbName);
            rw.WriteLine(".min.pdb");

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

        public void CreateNativePertJobs()
        {
            // ensure we can save ali files
            Directory.CreateDirectory(baseDirectory.FullName + Path.DirectorySeparatorChar + "ali");

            // 8 because NativePert can only produce loops of length 8
            for (int loopLength = StartLoopLength; loopLength <= EndLoopLength; loopLength++)
            {
                string pathname = scriptGenerationDirectory.FullName + loopLength.ToString() + Path.DirectorySeparatorChar;
                string resultPath = baseDirectory.FullName + "nat_pert" + Path.DirectorySeparatorChar + loopLength.ToString() + Path.DirectorySeparatorChar;

                Directory.CreateDirectory(pathname);

                StreamWriter condorJob = new StreamWriter(pathname + "condor.job");

                GenerateCondor_Start(condorJob);

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

                        Validity valid = NativePertFunctions.IsValidReturn(resultPath, currentJobStem);

                        switch (valid)
                        {
                            case Validity.Invalid:
                                GenerateCondor(condorJob, currentName, currentJobStem, startIndex, loopLength);
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
                condorJob.Close();
            }
        }

        #endregion

        #region Creation

        private void SaveDecoy(ParticleSystem ps, string savePath, string stem, double rms, string source)
        {
            String longSaveStem = String.Format("{0}_{1}_{2:F3}", stem, source, rms);
            PDB.SaveNew(savePath + longSaveStem + ".pdb", ps);
            rw_list.WriteLine("{0},{1:F3},{2}", source, rms, stem);
            rw_list.Flush();
        }

        private double GetRMS(ParticleSystem psNative, ParticleSystem psDecoy, SegmentDef loop)
        {
            PolyPeptide polyNative = psNative.MemberAt(0) as PolyPeptide;
            PolyPeptide polyDecoy = psDecoy.MemberAt(0) as PolyPeptide;
            if (polyDecoy.Count != polyDecoy.Count) throw new Exception();
            double rms = 0.0;
            int total = 0;
            for (int i = loop.FirstDSSPIndex - 1; i <= loop.LastDSSPIndex - 1; i++)
            {
                AminoAcid aaNative = polyNative[i] as AminoAcid;
                AminoAcid aaDecoy = polyDecoy[i] as AminoAcid;
                for (int j = 0; j < aaDecoy.Count; j++)
                {
                    Atom d = aaDecoy[j];
                    //if (d.PDBType == " N  " ||
                    //    d.PDBType == " C  " ||
                    //    d.PDBType == " CA " ||
                    //    d.PDBType == " O  ")
                    //{
                    Atom n = aaNative.AtomOfType(d.PDBType);
                    double dsq = n.distanceSquaredTo(d);
                    double dd = Math.Sqrt(dsq);
                    rms += dsq;
                    total++;
                    //}
                }
            }
            return Math.Sqrt(rms / (double)total);
        }

        private void StandardRename(ParticleSystem ps)
        {
            ps.BeginEditing();
            PolyPeptide poly = ps.MemberAt(0) as PolyPeptide;
            bool atomicContentChanged = false;
            List<Atom> newAtoms = new List<Atom>();

            for (int i = 0; i < poly.Count; i++)
            {
                poly[i].ResidueNumber = i + 1;
                poly[i].InsertionCode = ' ';

                if (0 == String.CompareOrdinal("HSD", poly[i].Name_NoPrefix) ||
                    0 == String.CompareOrdinal("HIP", poly[i].Name_NoPrefix))
                {
                    poly[i].ResetName("HIS", true);
                }

                for (int j = 0; j < poly[i].Count; j++)
                {
                    if (0 == String.CompareOrdinal(poly[i][j].PDBType, " OT1"))
                    {
                        if (!atomicContentChanged)
                        {
                            ps.BeginEditing();
                            atomicContentChanged = true;
                        }
                        Atom rem = poly[i][j];
                        newAtoms.Add(new Atom(" O  ", rem.ArrayIndex, rem.AtomNumber, rem.parentMolecule, rem.x, rem.y, rem.z));
                        poly[i].RemoveAtom(rem);
                        j--;
                    }
                    else if (0 == String.CompareOrdinal(poly[i][j].PDBType, " OT2"))
                    {
                        if (!atomicContentChanged)
                        {
                            ps.BeginEditing();
                            atomicContentChanged = true;
                        }
                        Atom rem = poly[i][j];
                        newAtoms.Add(new Atom(" OXT", rem.ArrayIndex, rem.AtomNumber, rem.parentMolecule, rem.x, rem.y, rem.z));
                        poly[i].RemoveAtom(rem);
                        j--;
                    }
                    else if (0 == String.CompareOrdinal(poly[i][j].parentMolecule.Name_NoPrefix, "ILE") &&
                        0 == String.CompareOrdinal(poly[i][j].PDBType, " CD "))
                    {
                        if (!atomicContentChanged)
                        {
                            ps.BeginEditing();
                            atomicContentChanged = true;
                        }
                        Atom rem = poly[i][j];
                        newAtoms.Add(new Atom(" CD1", rem.ArrayIndex, rem.AtomNumber, rem.parentMolecule, rem.x, rem.y, rem.z));
                        poly[i].RemoveAtom(rem);
                        j--;
                    }
                }
                if (newAtoms.Count > 0)
                {
                    for (int q = 0; q < newAtoms.Count; q++)
                    {
                        poly[i].addAtom(newAtoms[q]);
                    }
                    poly[i].ResetName(poly[i].Name_NoPrefix, true);
                    newAtoms.Clear();
                }
            }
            if (atomicContentChanged) ps.EndEditing(true, true);
        }

        private ParticleSystem Compute(ParticleSystem decoy, ParticleSystem native, SegmentDef loop, int stemLength, string savePath, string stem, string sourceName, ParticleSystem template, bool superimpose)
        {
            PolyPeptide nativePoly = native.MemberAt(0) as PolyPeptide;

            PolyPeptide decoyPoly = decoy.MemberAt(0) as PolyPeptide;
            StandardRename(decoy);
            decoy.DeleteHydrogens();

            if (decoyPoly.Count != nativePoly.Count)
            {
                if (template == null) throw new NullReferenceException();
                PolyPeptide templatePoly = template.MemberAt(0) as PolyPeptide;
                if (templatePoly.Count != nativePoly.Count) throw new Exception();

                // Create the new System
                ParticleSystem newPS = new ParticleSystem("stitched");
                PolyPeptide newPoly = templatePoly.Clone() as PolyPeptide;
                newPS.BeginEditing();
                newPS.AddMolContainer(newPoly);
                newPS.EndEditing(true, true);

                // Reset the positions to those in the model
                for (int i = 0; i < decoyPoly.Count; i++)
                {
                    AminoAcid aa = decoyPoly[i] as AminoAcid;
                    AminoAcid bb = newPoly[loop.FirstDSSPIndex + aa.ResidueNumber - 2 - stemLength];
                    if (0 != String.CompareOrdinal(aa.Name_NoPrefix, bb.Name_NoPrefix))
                    {
                        throw new Exception();
                    }
                    for (int j = 0; j < aa.Count; j++)
                    {
                        Atom aj = aa[j];
                        Atom bj = bb.AtomOfType(aj.PDBType);
                        bj.setTo(aj);
                    }
                }

                decoy = newPS;
            }
            else
            {
                if (template != null)
                {
                    throw new Exception("Model unexpectedly complete");
                }
            }

            if (superimpose)
            {
                Superimpose(native, decoy, loop, stemLength);
            }

            double rms = GetRMS(native, decoy, loop);
            SaveDecoy(decoy, savePath, stem, rms, sourceName);
            return decoy;
        }

        private ParticleSystem Compute(string DecoyPDBPath, ParticleSystem native, SegmentDef loop, int stemLength, string savePath, string stem, string sourceName, ParticleSystem template, bool superimpose, bool missingFilesMatter)
        {
            if (!File.Exists(DecoyPDBPath))
            {
                //Trace.WriteLine("Warning, no file: " + DecoyPDBPath);
                //return null;
                if (missingFilesMatter)
                {
                    throw new IOException("AAAAGGHH:" + DecoyPDBPath);
                }
                else
                {
                    return null;
                }
            }
            PDB decoy = new PDB(DecoyPDBPath, true);
            return Compute(decoy.particleSystem, native, loop, stemLength, savePath, stem, sourceName, template, superimpose);
        }

        private void Superimpose(ParticleSystem ps, ParticleSystem move, SegmentDef loop, int stemLength)
        {
            PolyPeptide poly = ps.MemberAt(0) as PolyPeptide;
            PolyPeptide movePoly = move.MemberAt(0) as PolyPeptide;

            if (poly.Count != movePoly.Count) throw new Exception();

            List<Position> polyPosL = new List<Position>();
            List<Position> movePosL = new List<Position>();

            for (int i = 0; i < loop.FirstDSSPIndex - 1 - stemLength; i++)
            {
                polyPosL.Add(new Position(poly[i].NTerminalAtom));
                polyPosL.Add(new Position(poly[i].CAlphaAtom));
                polyPosL.Add(new Position(poly[i].AtomOfType(" O  ")));
                polyPosL.Add(new Position(poly[i].CTerminalAtom));

                movePosL.Add(new Position(movePoly[i].NTerminalAtom));
                movePosL.Add(new Position(movePoly[i].CAlphaAtom));
                movePosL.Add(new Position(movePoly[i].AtomOfType(" O  ")));
                movePosL.Add(new Position(movePoly[i].CTerminalAtom));
            }

            for (int i = loop.LastDSSPIndex + stemLength; i < poly.Count; i++)
            {
                polyPosL.Add(new Position(poly[i].NTerminalAtom));
                polyPosL.Add(new Position(poly[i].CAlphaAtom));
                polyPosL.Add(new Position(poly[i].AtomOfType(" O  ")));
                polyPosL.Add(new Position(poly[i].CTerminalAtom));

                movePosL.Add(new Position(movePoly[i].NTerminalAtom));
                movePosL.Add(new Position(movePoly[i].CAlphaAtom));
                movePosL.Add(new Position(movePoly[i].AtomOfType(" O  ")));
                movePosL.Add(new Position(movePoly[i].CTerminalAtom));
            }

            int[] equiv = new int[(poly.Count - loop.Length - (2 * stemLength)) * 4];
            for (int i = 0; i < equiv.Length; i++) equiv[i] = i;
            Position[] polyPos = polyPosL.ToArray();
            Position[] movePos = movePosL.ToArray();

            //Position.DebugWriteFile(polyPos, movePos);

            Position cogX = new Position();
            Position cogY = new Position();
            MatrixRotation rotMat = new MatrixRotation();
            rotMat.getTranslation(cogX, cogY, polyPos, movePos);
            rotMat.translate(polyPos, cogX);
            rotMat.translate(movePos, cogY);
            rotMat.setToOptimalRotation(polyPos, movePos, equiv);

            //Position.DebugWriteFile(polyPos, movePos);

            movePoly.TranslateAll(cogY);
            movePoly.TransformAll(rotMat);
            cogX.x = -cogX.x;
            cogX.y = -cogX.y;
            cogX.z = -cogX.z;
            movePoly.TranslateAll(cogX);
        }

        private void Obtain(string savePath, string pdbStem, SegmentDef loop)
        {
            string basePath = baseDirectory.FullName;
            string stem = String.Concat(pdbStem, '_', loop.FirstDSSPIndex, '_', loop.Length);
            string stemOffset = String.Concat(pdbStem, '_', loop.FirstDSSPIndex - 1, '_', loop.Length);

            FileInfo mainArchive = new FileInfo(savePath + stem + ".tar.bz2");
            FileInfo mainList = new FileInfo(savePath + stem + ".list");
            if (mainList.Exists && !mainArchive.Exists)
            {
                mainList.Delete();
            }
            if (mainList.Exists && mainArchive.Exists)
            {
                // Ve hast been here before!
                return;
            }
            else
            {
                // nothing ... we need to run
            }

            savePath += stem + Path.DirectorySeparatorChar;
            Directory.CreateDirectory(savePath);
            rw_list = new StreamWriter(basePath + "decoy" + Path.DirectorySeparatorChar + 
                loop.Length.ToString() + Path.DirectorySeparatorChar + stem + ".list");      

            string nativePath = String.Concat(basePath, "pdb", Path.DirectorySeparatorChar, pdbStem, ".pdb");
            PDB native = new PDB(nativePath, true);
            ParticleSystem nativePS = native.particleSystem;
            StandardRename(nativePS);
            nativePS.DeleteHydrogens();
            SaveDecoy(nativePS, savePath, stem, 0.0, "native");

            DirectoryInfo diOut = null;
            string extractionPath = null;

            // ----------------
            // Minimised Native
            // ----------------

            string minimPath = String.Concat(basePath, "pdb_min", Path.DirectorySeparatorChar, pdbStem, ".min.pdb");
            ParticleSystem minimPS = Compute(minimPath, nativePS, loop, 0, savePath, stem, "minim", null, false, true);
            if (minimPS == null) throw new NullReferenceException();

            // -------------
            //  Native Pert
            // -------------

            int tag = 0; // numeric tag to give unique filenames

            Tra tra = new Tra(m_NatPert + loop.Length.ToString() + Path.DirectorySeparatorChar + stemOffset + ".tra");
            tra.TraPreview.skipLength = 1;
            tra.TraPreview.startPoint = 0;
            tra.LoadTrajectory();

            // 0 is the target structure
            for (int j = 1; j < tra.PositionDefinitions.Count; j++)
            {
                tra.PositionDefinitions.Position = j;
                Compute(tra.particleSystem.Clone() as ParticleSystem, nativePS, loop, -1, savePath, stem, "natpert_" + (tag++).ToString(), null, false);
            }

            // ----------------
            //  Modeller (8v2)
            // ----------------
            string modellerArchivePath = String.Concat(
                m_Modeller, "\\CompleteData_8v2\\ScriptGen_Very_Fast\\", loop.Length.ToString(), Path.DirectorySeparatorChar, stemOffset, ".tar.bz2");
            if (File.Exists(modellerArchivePath))
            {
                m_Comp.Uncompress(modellerArchivePath);
                diOut = m_Comp.OutPath(modellerArchivePath);
                extractionPath = diOut.FullName;
                for (int i = 0; i <= 1000; i++)
                {
                    string iStr = i.ToString().PadLeft(4, '0');
                    string name = String.Concat(extractionPath, stemOffset, ".BL", iStr, "0001", ".pdb");
                    Compute(name, nativePS, loop, -1, savePath, stem, "modeller_" + iStr, null, false, true);
                }
                m_Comp.CleanUp(modellerArchivePath);
            }
            else
            {
                Trace.WriteLine("Modeller achive missing!: " + modellerArchivePath);
            }

            // ------
            //  Plop
            // ------

            string plopPath = String.Concat(m_Plop, loop.Length, Path.DirectorySeparatorChar);

            PDB plop = new PDB(plopPath + stemOffset + ".pdb", true);
            int numModel = plop.PositionDefinitions.Count;
            for (int i = 0; i < numModel; i++)
            {
                plop.PositionDefinitions.Position = i;
                string iStr = i.ToString().PadLeft(3, '0');
                // You have to clone the bastard because the atom count will change due to H deletion, then the Position update will not work above
                Compute(plop.particleSystem.Clone() as ParticleSystem, nativePS, loop, 1, savePath, stem, "plop_" + iStr, minimPS, true);
            }

            // -----------
            //  Pre-Arcus 
            // -----------

            PreArcus(
                String.Concat(m_PreArcus, "stage2_ua_data", Path.DirectorySeparatorChar, loop.Length, Path.DirectorySeparatorChar),
                nativePS, loop, savePath, stem, "arcus_ua");
            PreArcus(
                String.Concat(m_PreArcus, "stage2_aa_data", Path.DirectorySeparatorChar, loop.Length, Path.DirectorySeparatorChar),
                nativePS, loop, savePath, stem, "arcus_aa");

            // ---------------
            //  Rapper Method
            // ---------------

            string rapperRoot = String.Concat(m_Rapper, loop.Length, Path.DirectorySeparatorChar);
            string rapperArchivePath = String.Concat(rapperRoot, stemOffset, ".tar.bz2");
            m_Comp.Uncompress(rapperArchivePath);
            diOut = m_Comp.OutPath(rapperArchivePath);
            extractionPath = diOut.FullName;

            string rapperPDB = extractionPath + "looptest.pdb";
            if (!File.Exists(rapperPDB))
            {
                string errFile = String.Concat(rapperRoot, stemOffset, "err");
                FileInfo errFileX = new FileInfo(errFile);
                if (errFile.Length == 0)
                {
                    throw new Exception();
                }
            }
            else
            {
                PDB rapper = new PDB(extractionPath + "looptest.pdb", true);
                numModel = rapper.PositionDefinitions.Count;
                for (int i = 0; i < numModel; i++)
                {
                    rapper.PositionDefinitions.Position = i;
                    string iStr = i.ToString().PadLeft(3, '0');
                    // You have to clone the bastard because the atom count will change due to H deletion, then the Position update will not work above
                    Compute(rapper.particleSystem.Clone() as ParticleSystem, nativePS, loop, 0, savePath, stem, "rapper_" + iStr, minimPS, true);
                }
            }

            m_Comp.CleanUp(rapperArchivePath);

            // --------------
            //  Petra Method
            // --------------

            string resStem = String.Concat(m_Petra, loop.Length, Path.DirectorySeparatorChar, stem);
            Compute(resStem + ".petra.pdb", nativePS, loop, 0, savePath, stem, "petra", null, false, false);
            Compute(resStem + ".coda.pdb", nativePS, loop, 0, savePath, stem, "coda", null, false, false);

            // --------------
            //  Cloop Method
            // --------------

            string cloopArchivePath = String.Concat(m_Cloop, loop.Length, Path.DirectorySeparatorChar, stem, ".tar.bz2");
            m_Comp.Uncompress(cloopArchivePath);
            diOut = m_Comp.OutPath(cloopArchivePath);
            extractionPath = diOut.FullName;

            // Cloop Build Template
            string cloopTemplate = String.Concat(extractionPath, pdbStem, ".pdb");
            PDB cloopPDBTempl = new PDB(cloopTemplate, true);
            ParticleSystem cloopPSTempl = cloopPDBTempl.particleSystem;
            StandardRename(cloopPSTempl);
            cloopPSTempl.DeleteHydrogens();

            // Test superimposition
            //Superimpose(nativePS,cloopPSTempl, loop);
            //PDB.SaveNew(String.Format("{0}{1}_cloop_sup.pdb", savePath, stem), cloopPSTempl);

            for (int i = 1; i <= 1000; i++)
            {
                string iStr = i.ToString().PadLeft(3, '0');
                string name = String.Concat(extractionPath, "conf", iStr, ".pdb");
                Compute(name, nativePS, loop, 1, savePath, stem, "cloop_" + iStr, cloopPSTempl, true, true);
            }
            m_Comp.CleanUp(cloopArchivePath);

             // ------------
            //  All Done!!
            // ------------

            rw_list.Close();
            m_Comp.CompressDir(savePath, CompressionMode.bzip2, true);
            
        }

        private void PreArcus(string sourcePath, ParticleSystem native, SegmentDef loop, string savePath, string stem, string sourceName)
        {
            int tag = 0; // numeric tag to give unique filenames
            DirectoryInfo di = new DirectoryInfo(sourcePath);
            FileInfo[] files = di.GetFiles(String.Format("out_*{0}.tra.bz2", stem));
            for (int i = 0; i < files.Length; i++)
            {
                m_Comp.Uncompress(files[i].FullName);
                DirectoryInfo outpath = m_Comp.OutPath(files[i].FullName);

                Tra tra = new Tra(outpath + Path.GetFileNameWithoutExtension(files[i].Name));
                if (tra.TraPreview.numberOfEntries == 4) 
                    continue;
                tra.TraPreview.skipLength = 1;
                tra.TraPreview.startPoint = 4;
                tra.LoadTrajectory();

                // 0 is the target structure
                for (int j = 1; j < tra.PositionDefinitions.Count; j++)
                {
                    tra.PositionDefinitions.Position = j;
                    Compute(tra.particleSystem, native, loop, 0, savePath, stem, "prearcus_" + (tag++).ToString(), null, false);
                }

                m_Comp.CleanUp(files[i].FullName);
            }
        }

        
        public void thinDecoys()
        {
            DirectoryInfo diDecoy = new DirectoryInfo(baseDirectory.FullName + "\\decoy\\");
            diDecoy.Create();

            for (int loopLength = StartLoopLength; loopLength <= EndLoopLength; loopLength++)
            {
                string decoyPath = diDecoy.FullName + loopLength.ToString() + Path.DirectorySeparatorChar;
                Directory.CreateDirectory(decoyPath);

                ParsingFileIndex = 0; // reset IMPORTANT
                while (true)
                {
                    string currentName = CurrentFile.InternalName.Substring(0, 5);

                    Trace.Write(currentName + ">: ");

                    SegmentDef[] loops = CurrentFile.GetLoops(loopLength, false, true);
                    for (int i = 0; i < loops.Length; i++)
                    {
                        string stem = String.Concat(CurrentFile.InternalName.Substring(0, 5), '_', loops[i].FirstDSSPIndex, '_', loops[i].Length);
                        thin(decoyPath, stem, loops[i]);
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
            }
        }

        class Job : IComparable<Job>
        {
            public string method;
            public string subJob;
            public double rmsd;
            public string rmsdS;
            public string jobStem;

            public bool rmsdIsReal()
            {
                return !Double.IsInfinity(rmsd) &&
                    !Double.IsNaN(rmsd) &&
                    !Double.IsNegativeInfinity(rmsd) &&
                    !Double.IsPositiveInfinity(rmsd);
            }

            public static bool operator>(Job a, Job b)
            {               
                if( a.rmsd == b.rmsd )                     
                {
                    if( a.method == b.method )
                    {
                        return 0>a.subJob.CompareTo(b.subJob);
                    }
                    return 0>a.method.CompareTo( b.method);
                }
                return a.rmsd > b.rmsd;
            }

            public static bool operator <(Job a, Job b)
            {
                if( a.rmsd == b.rmsd )                     
                {
                    if( a.method == b.method )
                    {
                        return 0 < a.subJob.CompareTo(b.subJob);
                    }
                    return 0 < a.method.CompareTo(b.method);
                }
                return a.rmsd < b.rmsd;
            }

            public static bool operator !=(Job a, Job b)
            {
                if (a.rmsd == b.rmsd)
                {
                    if (a.method == b.method)
                    {
                        if (a.subJob == b.subJob)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            public static bool operator ==( Job a, Job b )
            {
                if (a.rmsd == b.rmsd)
                {
                    if (a.method == b.method)
                    {
                        if (a.subJob == b.subJob)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public int CompareTo(Job b)
            {
                if (this > b)
                {
                    return 1;
                }
                else if( this < b )
                {
                    return -1;
                }
                return 0;
            }

            public override bool Equals(object obj)
            {
                Job j = null;
                if( null != (j as Job)) 
                    return this == j;
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public void Parse(string line)
            {
                string[] parts = line.Split(',');
                if (parts.Length != 3) throw new Exception();
                string[] methParts = parts[0].Split('_');
                subJob = "";
                if (methParts.Length > 2 || methParts.Length == 0 )
                {
                    throw new Exception();
                }
                else if (methParts.Length == 2)
                {
                    subJob = methParts[1];
                }         
                method = methParts[0];
                rmsd = double.Parse(parts[1]);
                rmsdS = parts[1];
                jobStem = parts[2];
            }

            public string ToPDBName()
            {
                string sub = "";
                if (subJob.Length != 0)
                    sub = '_' + subJob;
                return String.Concat(
                   jobStem, '_', method, sub, '_', rmsdS, ".pdb");
            }

            public override string ToString()
            {
                string sub = "";
                if( subJob.Length != 0 )
                    sub = '_' + subJob;
                return String.Concat(
                    method, sub, ',', rmsdS, ',', jobStem);
            }
        }

        Random rand = new Random();
        private void cull(List<Job> jobs, List<Job> usedJobs, int toMax, double cullResln)
        {
            double max = Double.MinValue;
            for (int i = 0; i < jobs.Count; i++)
            {
                if (max < jobs[i].rmsd)
                    max = jobs[i].rmsd;
            }
            int maxBin = (int)(max / cullResln)+1;

            List<Job>[] jobBins = new List<Job>[maxBin];
            for (int i = 0; i < jobBins.Length; i++)
            {
                jobBins[i] = new List<Job>();
                double limMinus = (double)i * cullResln;
                double limPlus = limMinus + cullResln;
                for (int j = 0; j < jobs.Count; j++)
                {
                    if (limMinus <= jobs[j].rmsd &&
                        limPlus > jobs[j].rmsd)
                    {
                        jobBins[i].Add(jobs[j]);
                    }
                }
            }

            int iSeek = 0;
            while( usedJobs.Count < toMax )
            {
                if (jobBins[iSeek].Count > 0)
                {
                    int randIndex = rand.Next(0, jobBins[iSeek].Count);
                    usedJobs.Add(jobBins[iSeek][randIndex]);
                    jobBins[iSeek].RemoveAt(randIndex);
                }
                iSeek++;
                if (iSeek == jobBins.Length) 
                    iSeek = 0;
            }            
        }

        private void thin(string savePath, string pdbStem, SegmentDef loop)
        {
            StreamReader re_list;
            re_list = new StreamReader(savePath + pdbStem + ".list");

            List<Job> normalJobs = new List<Job>();
            List<Job> nativeJobs = new List<Job>();
            List<Job> usedNormalJobs = new List<Job>();
            List<Job> usedNativeJobs = new List<Job>();
            string line;
            while (null != (line = re_list.ReadLine()))
            {
                Job j = new Job();
                j.Parse(line);
                if (!j.rmsdIsReal()) continue;
                if (0 == String.Compare(j.method, "native"))
                {
                    usedNativeJobs.Add(j);
                }
                else if (0 == String.Compare(j.method, "minim"))
                {
                    usedNativeJobs.Add(j);
                }
                else if (0 == String.Compare(j.method, "natpert"))
                {
                    nativeJobs.Add(j);
                }
                else
                {
                    normalJobs.Add(j);
                }
            }

            nativeJobs.Sort();
            normalJobs.Sort();

            cull(nativeJobs, usedNativeJobs, 100, 0.05);
            cull(normalJobs, usedNormalJobs, 900, 0.2);

            usedNativeJobs.Sort();
            usedNormalJobs.Sort();

            re_list.Close();

            StreamWriter a = new StreamWriter(savePath + pdbStem + ".cull.csv");
            StreamWriter b = new StreamWriter(savePath + pdbStem + ".cull.list");
            for (int i = 0; i < usedNativeJobs.Count; i++)
            {
                a.WriteLine(usedNativeJobs[i].ToString());
                b.WriteLine(usedNativeJobs[i].ToPDBName());
            }
            for (int i = 0; i < usedNormalJobs.Count; i++)
            {
                a.WriteLine(usedNormalJobs[i].ToString());
                b.WriteLine(usedNormalJobs[i].ToPDBName());
            }
            a.Close();
            b.Close();
        }

        private StreamWriter rw_list;
        public void CreateDecoys()
        {
            DirectoryInfo diDecoy = new DirectoryInfo(baseDirectory.FullName + "\\decoy\\");
            diDecoy.Create();

            for (int loopLength = StartLoopLength; loopLength <= EndLoopLength; loopLength++)
            {
                string decoyPath = diDecoy.FullName + loopLength.ToString() + Path.DirectorySeparatorChar;
                Directory.CreateDirectory(decoyPath);

                ParsingFileIndex = 0; // reset IMPORTANT
                while (true)
                {
                    string currentName = CurrentFile.InternalName.Substring(0, 5);

                    Trace.Write(currentName + ">: ");

                    SegmentDef[] loops = CurrentFile.GetLoops(loopLength, false, true);
                    for (int i = 0; i < loops.Length; i++)
                    {
                        Obtain(decoyPath, CurrentFile.InternalName.Substring(0, 5), loops[i]);
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
            }
        }

        #endregion

        #region Cluster

        double m_ClusterCutoff = 0.2;

        private float rmsFromName(string filename)
        {
            return 0.0f;
        }

        private void GetClusters(DirectoryInfo decoyPath, DirectoryInfo clusterPath, string pdbID, SegmentDef loop)
        {
            LoopSet ls = new LoopSet();

            string archivePath = String.Concat(decoyPath.FullName, pdbID, ".tar.bz2");
            DirectoryInfo outLocation = m_Comp.OutPath(archivePath);
            m_Comp.Uncompress(archivePath);
            FileInfo[] files = outLocation.GetFiles(pdbID + "*");
            for (int i = 0; i < files.Length; i++)
            {
                LoopEntry le = new LoopEntry();
                StreamReader re = new StreamReader(files[i].FullName);
                string line;
                while (null != (line = re.ReadLine()))
                {
                    if (0 != String.CompareOrdinal("ATOM", 0, line, 0, 4)) continue;
                    PDBAtom atom = new PDBAtom(line);
                    le.loopAtoms.Add(atom);
                }
                re.Close();
                le.energy = rmsFromName(files[i].Name);
                le.fromTra = files[i].FullName;
                le.traIndex = 0;

                ls.loopStore.Add(le);
            }

            ls.ClusterLoops(m_ClusterCutoff);
        }

        public void ClusterDecoys()
        {
            throw new NotImplementedException("This code has not been run or tested in any way.");

            //DirectoryInfo diDecoy = new DirectoryInfo(baseDirectory.FullName + "\\decoy\\");
            //if (!diDecoy.Exists) throw new Exception();
            //DirectoryInfo diCluster = new DirectoryInfo(baseDirectory.FullName + "\\clusters\\");

            //for (int loopLength = StartLoopLength; loopLength <= EndLoopLength; loopLength++)
            //{
            //    string decoyPath = diDecoy.FullName + loopLength.ToString() + Path.DirectorySeparatorChar;
            //    Directory.CreateDirectory(decoyPath);

            //    ParsingFileIndex = 0; // reset IMPORTANT
            //    while (true)
            //    {
            //        string currentName = CurrentFile.InternalName.Substring(0, 5);

            //        Trace.Write(currentName + ">: ");

            //        SegmentDef[] loops = CurrentFile.GetLoops(loopLength, false, true);
            //        for (int i = 0; i < loops.Length; i++)
            //        {
            //            GetClusters(diDecoy, diCluster, CurrentFile.InternalName.Substring(0, 5), loops[i]);
            //            Trace.Write('.');
            //        }

            //        Trace.WriteLine("");

            //        // increment conidtion
            //        if (ParsingFileIndex < FileCount - 1)
            //        {
            //            ParsingFileIndex++;
            //        }
            //        else
            //        {
            //            break;
            //        }
            //    }
            //}
        }

        #endregion

        #region Generate Jobs

        public delegate void TaskDelegate(string path, string stem, SegmentDef def);
        public delegate void PBDDelegate(StreamWriter rwQueue, string writePath, string jobStem, SegmentDef segdef);

        public void Gen(TaskDelegate myDelegate, PBDDelegate pbs)
        {
            DirectoryInfo diDecoy = new DirectoryInfo(baseDirectory.FullName + "\\decoy\\");
            diDecoy.Create();

            for (int loopLength = StartLoopLength; loopLength <= EndLoopLength; loopLength++)
            {
                string decoyPath = diDecoy.FullName + loopLength.ToString() + Path.DirectorySeparatorChar;
                Directory.CreateDirectory(decoyPath);

                string scriptPath = scriptGenerationDirectory.FullName + Path.DirectorySeparatorChar + loopLength + Path.DirectorySeparatorChar;
                Directory.CreateDirectory(scriptPath);
                StreamWriter rwQueue = new StreamWriter(scriptPath + "pbs_submit.sh");

                ParsingFileIndex = 0; // reset IMPORTANT
                while (true)
                {
                    string currentName = CurrentFile.InternalName.Substring(0, 5);

                    Trace.Write(currentName + ">: ");

                    SegmentDef[] loops = CurrentFile.GetLoops(loopLength, false, true);
                    for (int i = 0; i < loops.Length; i++)
                    {
                        string stem = String.Concat(currentName, '_', loops[i].FirstDSSPIndex, '_', loops[i].Length);
                        pbs(rwQueue, scriptPath, stem, loops[i]);
                        myDelegate(decoyPath, stem, loops[i]);
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

                rwQueue.Close();
            }
        }

        public void GenerateRaft()
        {
            Raft app = new Raft();
            Gen(new TaskDelegate(app.Compute),  new PBDDelegate(app.GenPBSQueue));
        }

        public void GenerateDockit()
        {
            Dockit app = new Dockit();
            Gen(new TaskDelegate(app.Compute), new PBDDelegate(app.GenPBSQueue));
        }

        public void GenerateDFire()
        {
            DFire app = new DFire();
            Gen(new TaskDelegate(app.Compute), new PBDDelegate(app.GenPBSQueue));
        }

        #endregion
    }

    #region PBS Generators

    public class Raft : PBSRun
    {
        public override string Name
        {
            get { return "Raft"; }
        }
        public override void CopyFiles(StreamWriter rw)
        {
        }
        public override void ExtractFiles(StreamWriter rw)
        {
        }
        public override void RunApp(StreamWriter rw, SegmentDef segdef, string stem)
        {
        }
        public override void DelFiles(StreamWriter rw)
        {
        }
        public override void Compute(string path, string stem, SegmentDef def)
        {
        }
    }

    public class Dockit : PBSRun
    {
        public override string Name
        {
            get { return "Dockit"; }
        }
        public override void CopyFiles(StreamWriter rw)
        {
        }
        public override void ExtractFiles(StreamWriter rw)
        {
        }
        public override void RunApp(StreamWriter rw, SegmentDef segdef, string stem)
        {
        }
        public override void DelFiles(StreamWriter rw)
        {
        }
        public override void Compute(string path, string stem, SegmentDef def)
        {
        }
    }

    public class DFire : PBSRun
    {
        public override string Name
        {
            get { return "DFire"; }
        }
        public override void CopyFiles(StreamWriter rw)
        {
            rw.WriteLine("cp ../exec/dfire.tgz $WORKDIR");
        }
        public override void ExtractFiles(StreamWriter rw)
        {
            rw.WriteLine("tar xvzf dfire.tgz");
            rw.WriteLine("mkdir out");
        }
        public override void RunApp(StreamWriter rw, SegmentDef loop, string stem )
        {
            rw.WriteLine("./dloop ./{3}/$myline.pdb {0} {1} > ./out/$myline.{2}.out", loop.FirstDSSPIndex, loop.Length, Name, stem); 
        }
        public override void DelFiles(StreamWriter rw)
        {
            // Original archive
            rw.WriteLine("rm dfire.tgz");
            // Contents
            rw.WriteLine("rm dfire.lib");
            rw.WriteLine("rm dloop");
            rw.WriteLine("rm hash.dat");
            rw.WriteLine("rm scwrlbin34.lib.red");
        }
        public override void Compute(string path, string stem, SegmentDef def)
        {
            // nothing to do
        }
    }

    public abstract class PBSRun
    {
        public abstract string Name { get; }
        public abstract void CopyFiles(StreamWriter rw);
        public abstract void ExtractFiles(StreamWriter rw);
        public abstract void RunApp(StreamWriter rw, SegmentDef segdef, string stem );
        public abstract void DelFiles(StreamWriter rw);

        public abstract void Compute(string path, string stem, SegmentDef def);

        private string m_GrendelNodeType = ",nodes=1:fast";
        //private string m_GrendelNodeType = "";
        public void GenPBSQueue(StreamWriter rwQueue, string writePath, string jobStem, SegmentDef segdef )
        {
            rwQueue.WriteLine("qsub -o {0}.out -e {0}.err {0}.sh", jobStem);
            rwQueue.Flush();

            StreamWriter rwJob = new StreamWriter(writePath + jobStem + ".sh");

            rwJob.WriteLine("#!/bin/sh");
            rwJob.WriteLine("#PBS -l walltime=71:59:00{0}", m_GrendelNodeType);
            rwJob.WriteLine("");
            rwJob.WriteLine("#########################");
            rwJob.WriteLine("# Setup");
            rwJob.WriteLine("#########################");
            rwJob.WriteLine("");
            rwJob.WriteLine("echo PBS Launching Script In:");
            rwJob.WriteLine("pwd");
            rwJob.WriteLine("echo Moving To Run Dir");
            rwJob.WriteLine("cd ~/decoy/{0}", segdef.Length);
            rwJob.WriteLine("");
            rwJob.WriteLine("# store the current folder");
            rwJob.WriteLine("JHOME=$PWD");
            rwJob.WriteLine("export JHOME");
            rwJob.WriteLine("echo Submit dir was: $JHOME");
            rwJob.WriteLine("");
            rwJob.WriteLine("# Setup the working environment");
            rwJob.WriteLine("if [ -z \"$JOBNO\" ]");
            rwJob.WriteLine("then");
            rwJob.WriteLine("        WORKDIR=/tmp/jr0407/{0}", jobStem);
            rwJob.WriteLine("else");
            rwJob.WriteLine("        WORKDIR=/tmp/jr0407/{0}_$JOBNO", jobStem);
            rwJob.WriteLine("fi");
            rwJob.WriteLine("export WORKDIR");
            rwJob.WriteLine("echo Using workdir: $WORKDIR");
            rwJob.WriteLine("");
            rwJob.WriteLine("#clean out any existing muck");
            rwJob.WriteLine("rm -rf $WORKDIR");
            rwJob.WriteLine("mkdir -p $WORKDIR");

            // Copy files over
            rwJob.WriteLine("cp ../decoy/{0}.tar.bz2 $WORKDIR", jobStem);
            rwJob.WriteLine("cp ../decoy/{0}.cull.list $WORKDIR", jobStem); 
            CopyFiles(rwJob);

            rwJob.WriteLine("cd $WORKDIR");
            rwJob.WriteLine("");
            rwJob.WriteLine("# store name of node and scratch directory in file rundat");
            rwJob.WriteLine("hostname > {0}.rundat", jobStem);
            rwJob.WriteLine("pwd >> {0}.rundat", jobStem);
            rwJob.WriteLine("");

            // Extract run data
            ExtractFiles(rwJob);
            rwJob.WriteLine("tar xvjf {0}.tar.bz2", jobStem);            

            rwJob.WriteLine("");
            rwJob.WriteLine("echo These files are here!");
            rwJob.WriteLine("ls");
            rwJob.WriteLine("");
            rwJob.WriteLine("#########################");
            rwJob.WriteLine("# Run");
            rwJob.WriteLine("#########################");
            rwJob.WriteLine("");

            // Execution - read the files to process from a file
            rwJob.WriteLine("exec 3< {0}.cull.csv", jobStem);
            rwJob.WriteLine("");
            rwJob.WriteLine("until [ $done ]");
            rwJob.WriteLine("do");
            rwJob.WriteLine("    read <&3 myline");
            rwJob.WriteLine("    res=$?");
            rwJob.WriteLine("");
            RunApp(rwJob, segdef, jobStem); // Method Run
            rwJob.WriteLine("");
            rwJob.WriteLine("    if [ $res != 0 ]; then");
            rwJob.WriteLine("        done=1");
            rwJob.WriteLine("        continue");
            rwJob.WriteLine("    fi");
            rwJob.WriteLine("done");

            rwJob.WriteLine("");
            rwJob.WriteLine("#########################");
            rwJob.WriteLine("# Clean and zip complete");
            rwJob.WriteLine("#########################");
            rwJob.WriteLine("");
            rwJob.WriteLine("echo Run Complete. Files generated:");
            rwJob.WriteLine("ls -oAh");
            rwJob.WriteLine("");
            rwJob.WriteLine("echo Remove the irrelevent items so they are not sent back");

            // Del Shite
            DelFiles(rwJob);

            rwJob.WriteLine("");
            rwJob.WriteLine("echo Compression Commencing...");
            rwJob.WriteLine("tar --remove-files -cvjf {0}.{1}.tar.bz2 *", jobStem, Name);
            rwJob.WriteLine("");
            rwJob.WriteLine("# Return the files to the home node");
            rwJob.WriteLine("mv -u * $JHOME");
            rwJob.WriteLine("rm -rf $WORKDIR");

            rwJob.Close();

            return;
        }
    }

    #endregion
}
