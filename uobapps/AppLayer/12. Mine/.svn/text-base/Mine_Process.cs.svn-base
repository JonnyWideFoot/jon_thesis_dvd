using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

using UoB.Core.FileIO.DSSP;
using UoB.Core.Data;
using UoB.Methodology.DSSPAnalysis;
using UoB.Core.FileIO.PDB;
using UoB.Core.FileIO.Tra;
using UoB.Core.Structure;
using UoB.Core.Primitives;
using UoB.AppLayer.Common;
using UoB.Core.Tools;

namespace UoB.AppLayer.PreArcus
{
    class PreArcusProcess : DSSPLoopAnalysis
    {
        public PreArcusProcess(AppLayerBase parent)
            : base(parent)
        {
            EndLoopLength = 9; // any more than this was undeesible
            m_MethodSuppliesTemplate = true; // from the Tra file ...
        }

        public bool DoCluster = false;
        public double ClusterCutoff = 0.5; // Cluster RMS cutoff for same cluster inclusion
        public bool DumpDecoys = false;
        public double DumpPoint = 0.0; // Dump resultant energies below X kCal/Mol
        public string resDir = "stage2_aa_data";

        public void completeAttemptedMoves()
        {
            string resPath = baseDirectory.FullName + Path.DirectorySeparatorChar + resDir + Path.DirectorySeparatorChar;
            for (int j = 7; j <= 7; j++)
            {
                string resPathL = resPath + j.ToString() + Path.DirectorySeparatorChar;
                string resPathBin = resPathL + "bin" + Path.DirectorySeparatorChar;

                DirectoryInfo di = new DirectoryInfo(resPathBin);
                FileInfo[] files = di.GetFiles("*.attempted");

                for (int i = 0; i < files.Length; i++)
                {
                    string stem = Path.GetFileNameWithoutExtension(files[i].Name);
                    try
                    {
                        File.Move(di.Parent.FullName + Path.DirectorySeparatorChar + stem,
                            di.FullName + Path.DirectorySeparatorChar + stem);

                        files[i].Delete();
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        public void assertFiles()
        {
            m_CheckFiles = new Queue<FileInfo>();

            string resPath = baseDirectory.FullName + Path.DirectorySeparatorChar + resDir + Path.DirectorySeparatorChar;
            for( int j = StartLoopLength; j <= EndLoopLength; j++ )
            {
                string resPathL = resPath + j.ToString() + Path.DirectorySeparatorChar;
                DirectoryInfo di = new DirectoryInfo(resPathL);
                FileInfo[] traFiles = di.GetFiles("*.tra.bz2");
                for (int i = 0; i < traFiles.Length; i++)
                {
                    m_CheckFiles.Enqueue(traFiles[i]);
                }
            }

            bool multiThread = false;

            if (multiThread)
            {
                for (int i = 0; i < 8; i++)
                {
                    ThreadStart ts = new ThreadStart(FileProc);
                    Thread t = new Thread(ts);
                    t.Start();
                }
            }
            else
            {
                FileProc();
            }
        }

        private Queue<FileInfo> m_CheckFiles;
        private Object m_Lock = new Object();
        private void FileProc()
        {
            while (true)
            {
                FileInfo doFile = null;
                lock (m_Lock)
                {
                    if (m_CheckFiles.Count == 0)
                    {
                        break;
                    }
                    doFile = m_CheckFiles.Dequeue();
                }

                DirectoryInfo diBinPath = Directory.CreateDirectory(doFile.Directory.FullName + Path.DirectorySeparatorChar
                   + "bin" + Path.DirectorySeparatorChar);

                bool allOk = true;

                // Allow compression to run for 10 seconds, some bad files hang the uncompressor
                if (null == m_Comp.Uncompress(doFile.FullName, 10 * 10000000 ))
                {
                    allOk = false;
                }
                else
                {
                    DirectoryInfo outPath = m_Comp.OutPath(doFile.FullName);
                    string name = Path.GetFileNameWithoutExtension(doFile.Name);
                    string outName = outPath + name;

                    try
                    {
                        // Appempt load
                        Tra file = new Tra(outName);
                        file.TraPreview.startPoint = 0;
                        file.TraPreview.skipLength = 1; // load every entry, no matter how big...
                        file.LoadTrajectory(); // load the beast into memory.
                        file = null;
                    }
                    catch
                    {
                        allOk = false;
                    }

                    m_Comp.CleanUp(doFile.FullName);

                    if (allOk)
                    {
                        string stem = Path.GetFileNameWithoutExtension(
                             Path.GetFileNameWithoutExtension(doFile.Name));
                        string outFileArchStem = doFile.Directory.FullName + Path.DirectorySeparatorChar + stem;
                        string outFileArch = outFileArchStem + ".out.bz2";

                        if (!File.Exists(outFileArch))
                        {
                            allOk = false;
                        }
                        else
                        {
                            m_Comp.Uncompress(outFileArch);
                            StreamReader re = new StreamReader(outPath + stem + ".out");

                            string line;
                            bool ok = false;

                            while (null != (line = re.ReadLine()))
                            {
                                if (0 == String.CompareOrdinal(line.Trim(),
                                    "Execution Result --> No errors were detected during execution. Loop generation successful!"))
                                {
                                    ok = true;
                                }
                            }

                            if (!ok)
                            {
                                allOk = false;
                            }

                            re.Close();

                            m_Comp.CleanUp(outFileArch);
                        }
                    }
                }

                if (!allOk)
                {
                    try
                    {
                        doFile.MoveTo(diBinPath.FullName + doFile.Name);
                    }
                    catch
                    {
                        File.Create(diBinPath.FullName + doFile.Name + ".attempted");
                    }


                    string theOutFile = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(doFile.Name)) + ".out.bz2";
                    string tfile = doFile.Directory.FullName + Path.DirectorySeparatorChar + theOutFile;
                    if (File.Exists(tfile))
                    {
                        string x = diBinPath.FullName + Path.DirectorySeparatorChar + theOutFile;
                        try
                        {
                            File.Move(tfile, x);
                        }
                        catch
                        {
                            File.Create(x + ".attempted");
                        }
                    }
                    Console.Write('X');
                }
                else
                {
                    Console.Write('.');
                }
            }
        }

        protected override LoopStatAnalyse DoStructuralAnalysis(string currentName, string pdbID, PDB libPDB, SegmentDef loop, ref bool methodFailure)
        {
            ParticleSystem libPS = null;
            string resPath = baseDirectory.FullName + Path.DirectorySeparatorChar + resDir + Path.DirectorySeparatorChar + loop.Length + Path.DirectorySeparatorChar;
            string stem = String.Format("{0}_{1}_{2}", pdbID, loop.FirstDSSPIndex - 1, loop.Length);
            const int TRA_OFFSET = 5;
            const int EPOT_DATA_INDEX = 5;

            DirectoryInfo di = new DirectoryInfo(resPath);
            FileInfo[] traFiles = di.GetFiles("out_r*" + stem + ".tra.bz2");

            if (traFiles.Length == 0)
            {
                string error = "Stage1 didn't produce us any pickings!: " + stem;
                Trace.WriteLine(error);
                methodFailure = true;
                return null;
            }

            NameValidation(traFiles, stem, loop.Length);

            LoopSet loops = new LoopSet();
            FullSimFileInfo info = new FullSimFileInfo(traFiles[0].Name);
            info.PrintScreen();
            loops.CopyFrom(info.scope);

            for (int i = 0; i < traFiles.Length; i++)
            {
                m_Comp.Uncompress(traFiles[i].FullName);
                DirectoryInfo outPath = m_Comp.OutPath(traFiles[i].FullName);
                string name = Path.GetFileNameWithoutExtension(traFiles[i].Name);
                string outName = outPath + name;

                Tra file = new Tra(outName);

                // Now, scan though all structures and extract positional information
                if (file.TraPreview.numberOfEntries + 1 == TRA_OFFSET)
                {
                    // There are no valid entries in this file!
                    m_Comp.CleanUp(traFiles[i].FullName);
                    continue;
                }

                file.TraPreview.startPoint = 0;
                file.TraPreview.skipLength = 1; // load every entry, no matter how big...
                file.LoadTrajectory(); // load the beast into memory.

                if (libPS == null && file.PositionDefinitions.Count > 1)
                {
                    // Load the minimised native structure for RMS calcs
                    file.PositionDefinitions.Position = 1;

                    // **Clone** the system with its initial positions, which is the minimised native structure
                    libPS = file.particleSystem.Clone() as ParticleSystem;
                }

                info = new FullSimFileInfo(traFiles[i].Name);
                info.PrintScreen();

                List<LoopEntry> loopStore = loops.loopStore;

                DataListing dl = file.DataStore.GetDataListing(EPOT_DATA_INDEX);
                for (int j = TRA_OFFSET; j < file.PositionDefinitions.Count; j++)
                {
                    // Make a new loop info instance
                    LoopEntry store = new LoopEntry();
                    loopStore.Add(store);  // add to global container

                    // General Details
                    store.fromTra = traFiles[i].FullName;
                    store.energy = dl.Data[j - 1];
                    store.traIndex = j;

                    // Store the coordinates
                    file.PositionDefinitions.Position = j;
                    ParticleSystem ps = file.particleSystem;
                    PSMolContainer poly = ps.MemberAt(0);
                    for (int m = 0; m < loops.loopLength; m++)
                    {
                        Molecule mol = poly[m + loops.startResIndex];
                        if (Char.ToLower(mol.moleculePrimitive.SingleLetterID) !=
                            Char.ToLower(loop.Sequence[m]))
                        {
                            if (0 == String.Compare(mol.Name_NoPrefix, "HIP") && loop.Sequence[m] == 'H')
                            {
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                        for (int k = 0; k < mol.Count; k++)
                        {
                            PDBAtom atom = new PDBAtom(PDB.MakePDBStringFromAtom(mol[k]));
                            store.loopAtoms.Add(atom);
                        }
                    }
                }

                m_Comp.CleanUp(traFiles[i].FullName);
            }

            if (libPS == null)
            {
                methodFailure = true;
                Trace.WriteLine("**Warning**, method failure due to empty tra files! (" + stem + ")");
                return null;
            }

            ParticleSystem loopPS = null;
            double ene = Double.MaxValue;

            // Identify the best loop then setup the returning 'LoopStatAnalyse' container
            if (DoCluster)
            {
                //int bestE = loops.ClusterLoops(ClusterCutoff);
                throw new NotImplementedException();
            }
            else
            {
                int bestE = loops.FindSingleLowestEnergy(ref ene);
                LoopEntry le = loops.loopStore[bestE];

                string getTra = le.fromTra;
                FileInfo getTraF = new FileInfo(getTra);
                bool comp = false;

                if (getTraF.Extension == ".bz2")
                {
                    comp = true;
                    m_Comp.Uncompress(getTra);
                    getTra = m_Comp.OutPath(getTra) + Path.GetFileNameWithoutExtension(getTraF.Name);
                }

                Tra tra = new Tra(getTra);
                tra.TraPreview.startPoint = le.traIndex - 1;
                tra.TraPreview.skipLength = 1; // load every entry, no matter how big...
                tra.TraPreview.endPoint = le.traIndex;
                tra.LoadTrajectory(); // Load just the minima
                tra.PositionDefinitions.Position = 1; // the minima
                loopPS = tra.particleSystem;

                if (comp) m_Comp.CleanUp(le.fromTra);
            }

            LoopStatAnalyse stat = new LoopStatAnalyse(libPS, loop);
            stat.MissingHack = false;
            stat.AnalyseFragment(loopPS, -1, ene); // Perform Analysis, -1 means "whole protein"

            return stat;
        }

        private void NameValidation(FileInfo[] traFiles, string stem, int loopLength)
        {
            List<bool> found = null;
            int paraTotalFlag = -1;
            for (int i = 0; i < traFiles.Length; i++)
            {
                String[] fileNameParts = traFiles[i].Name.Split('_');

                if (fileNameParts[0] != "out") throw new Exception();
                if (fileNameParts.Length == 6)
                {
                    if (fileNameParts[5] != loopLength.ToString() + ".tra.bz2") throw new Exception();
                }
                else if (fileNameParts.Length == 7)
                {
                    if (fileNameParts[6] != loopLength.ToString() + ".tra.bz2") throw new Exception();
                }
                else
                {
                    throw new Exception();
                }

                if (fileNameParts[1][0] != 'r') throw new Exception();
                if (int.Parse(fileNameParts[1].Substring(1, fileNameParts[1].Length - 1)) != 110) throw new Exception();
                if (fileNameParts[2][0] != 'n') throw new Exception();
                string paraString = fileNameParts[2].Substring(1, fileNameParts[2].Length - 1);
                string[] paraParts = paraString.Split('-');
                if (paraParts.Length != 2) throw new Exception();
                int paraNode = int.Parse(paraParts[0]);
                int paraTotal = int.Parse(paraParts[1]);
                if (i == 0)
                {
                    paraTotalFlag = paraTotal;
                    found = new List<bool>(paraTotalFlag);
                    for (int kk = 0; kk < paraTotalFlag; kk++) found.Add(false);
                    found[paraNode] = true;
                }
                else
                {
                    if (paraTotal != paraTotalFlag) throw new Exception();
                    if (found[paraNode] == true) throw new Exception();
                    found[paraNode] = true;
                }
            }

            for (int i = 0; i < found.Count; i++)
            {
                if (found[i] != true)
                {
                    Trace.Write("Warning - not all tra files are present for: " + stem);
                    break;
                }
            }
        }
    }
}
