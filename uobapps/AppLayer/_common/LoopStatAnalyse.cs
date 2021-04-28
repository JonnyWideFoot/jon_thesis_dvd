using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using UoB.Core.Structure;
using UoB.Core.Primitives;
using UoB.Core.Tools;
using UoB.Core.FileIO.DSSP;
using UoB.Core.FileIO.PDB;
using UoB.Methodology.DSSPAnalysis;
using UoB.AppLayer.Common;
using UoB.Compression;
using UoB.Methodology.OriginInteraction;

namespace UoB.AppLayer.Common
{
    class CommonFunctions
    {
        private CommonFunctions() { }

        private static bool DisablePhysEW = true;
        public static string ExcludeMachineString()
        {
            if (DisablePhysEW)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append('(');
                for (int i = 1; i <= 30; i++)
                {
                    if (i != 1) sb.Append(" && ");
                    sb.AppendFormat("(Machine != \"pys-ew{0}.pys.bris.ac.uk\")", i.ToString().PadLeft(2, '0'));
                }
                sb.Append(") && ");
                return sb.ToString();
            }
            return "";
        }
    }

    class DSSPLoopRuns : DSSPTaskDirectory, IDisposable
    {
        protected CompressedFileManager m_Comp = null;
        AppLayerBase m_Parent = null;

        string cachePath = null;

        public DSSPLoopRuns(AppLayerBase parent, bool OriginInteractionRequired )
            : base(parent.DBName, parent.TaskDir, OriginInteractionRequired)
        {
            constructor(parent, OriginInteractionRequired, ProcessPriorityClass.BelowNormal);
        }

        public DSSPLoopRuns(AppLayerBase parent, bool OriginInteractionRequired, ProcessPriorityClass _LaunchPrio)
            : base(parent.DBName, parent.TaskDir, OriginInteractionRequired)
        {
            constructor(parent, OriginInteractionRequired, _LaunchPrio);
        }

        private void constructor(AppLayerBase parent, bool OriginInteractionRequired, ProcessPriorityClass _LaunchPrio )
        {
            m_Parent = parent;
            if (m_Parent == null) throw new NullReferenceException("Parent Cannot Be Null");

            m_Comp = new CompressedFileManager(baseDirectory.FullName + "\\exec\\7za.exe", "bz2", _LaunchPrio);
            if (Directory.Exists(@"b:\\"))
            {
                m_Comp.SetExtractionPath("b:\\"); // use the ramdrive!
            }
            else if (Directory.Exists(@"s:\\"))
            {
                m_Comp.SetExtractionPath("s:\\"); // use dedicated scratch drive!
            }
            else
            {
                string cacheStem = "c:\\AppLayerCache";
                int i = 1;
                for( ; i < 100; i++ )
                {
                    cachePath = cacheStem + i.ToString() + Path.DirectorySeparatorChar;
                    DirectoryInfo di = new DirectoryInfo(cachePath);
                    if (!di.Exists)
                    {
                        try
                        {
                            di.Create();
                            File.Create(cachePath + "lock");
                            break;
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!File.Exists(cachePath + "lock"))
                        {
                            try
                            {
                                File.Create(cachePath + "lock");
                                break;
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
                if (i == 100) throw new Exception("Could not create cache store");
                m_Comp.SetExtractionPath(cachePath);
            }
        }

        public void Dispose()
        {
            if (cachePath != null)
            {
                if (File.Exists(cachePath + "lock"))
                {
                    try
                    {
                        File.Delete(cachePath + "lock");
                    }
                    catch
                    {
                    }
                }
                if (Directory.Exists(cachePath))
                {
                    try
                    {
                        Directory.Delete(cachePath,true);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public string MethodPrintName
        {
            get { return m_Parent.MethodPrintName; }
        }

        private int m_StartLoopLength = 6;

        protected int StartLoopLength
        {
            get { return m_StartLoopLength; }
            set { m_StartLoopLength = value; }
        }
        private int m_EndLoopLength = 11;

        protected int EndLoopLength
        {
            get { return m_EndLoopLength; }
            set { m_EndLoopLength = value; }
        }
    }

    enum Obfuscation
    {
        Delete,
        Randomise,
        None
    }

    class DSSPLoopRunGen : DSSPLoopRuns
    {
        public DSSPLoopRunGen(AppLayerBase parent)
            : base( parent, false )
        {
        }

        private System.Random rand = new Random(12345);

        private void ObfuscateAmino(AminoAcid amino)
        {
            double lim = 6.0; // move atoms upto +- lim Angstrom at random
            for (int i = 0; i < amino.Count; i++)
            {
                amino[i].x += (2.0 * lim * rand.NextDouble()) - lim;
                amino[i].y += (2.0 * lim * rand.NextDouble()) - lim;
                amino[i].z += (2.0 * lim * rand.NextDouble()) - lim;
            }
        }

        private void DeleteLoop(PolyPeptide poly, int startResIndex, int loopLength)
        {
            poly.RemoveMolAt(startResIndex, loopLength);
        }

        private void ObfuscateLoop(PolyPeptide poly, int startRes, int loopLength)
        {
            for (int i = 0; i < loopLength; i++)
            {
                ObfuscateAmino(poly[startRes + i]);
            }
        }

        public void CreatePDBFile(string name, string jobStem, Obfuscation mode, int startResIndex, int loopLength, string ext, bool lowerCase, bool HSDHack)
        {
            string dbSource = baseDirectory.FullName + "pdb_source" + Path.DirectorySeparatorChar;
            string dbDest = baseDirectory.FullName + "pdb" + Path.DirectorySeparatorChar;
            string destFile = dbDest + jobStem + ext;
            if( lowerCase ) destFile = destFile.ToLower();
            if (File.Exists(destFile)) return;
            Directory.CreateDirectory(dbDest);
            PDB file = new PDB(dbSource + name + ".min.pdb", true);
            ParticleSystem ps = file.particleSystem;
            PolyPeptide poly = ps.MemberAt(0) as PolyPeptide;

            if (HSDHack)
            {
                for (int i = 0; i < poly.Count; i++)
                {
                    if (poly[i].Name_NoPrefix == "HIS")
                    {
                        poly[i].ResetName("HSD", false);
                    }
                }
            }

            switch (mode)
            {
                case Obfuscation.Delete:
                    ParticleSystem seqres = ps.Clone() as ParticleSystem;
                    ps.BeginEditing();
                    DeleteLoop(poly, startResIndex, loopLength);
                    ps.EndEditing(true, true);
                    PDB.SaveNew(destFile, ps, seqres, null, false);
                    break;
                case Obfuscation.Randomise:
                    ObfuscateLoop(poly, startResIndex, loopLength);
                    PDB.SaveNew(destFile, ps, true);
                    break;
                case Obfuscation.None:
                    PDB.SaveNew(destFile, ps, true);
                    break;
                default:
                    throw new Exception("Unknown obfuscation mode encountered");
            }         

            return;
        }
    }

    class DSSPLoopAnalysis : DSSPLoopRuns
    {
        public DSSPLoopAnalysis(AppLayerBase parent)
            : base( parent, true )
        {
        }

        public bool MethodSuppliesTemplate
        {
            get
            {
                return m_MethodSuppliesTemplate;
            }
        }

        protected bool m_MethodSuppliesTemplate = false;

        protected virtual LoopStatAnalyse DoStructuralAnalysis(string currentName, string stem, PDB libPDB, SegmentDef loop, ref bool methodFailure)
        {
            throw new NotImplementedException();
        }

        protected virtual void JobBeginHook( int loopLength )
        {
            // Base function does nothing
        }

        protected virtual void JobEndHook(int loopLength)
        {
            // Base function does nothing
        }

        protected virtual bool PreAnalysisHook(string currentName, string stem, PDB libPDB, SegmentDef loop)
        {
            // Base function does nothing
            return true;
        }

        protected virtual bool PostAnalysisHook(string currentName, string stem, PDB libPDB, SegmentDef loop)
        {
            // Base function does nothing
            return true;
        }

        public void ProcessJobs()
        {
            // Start HTML output
            HTMLReportingBegin(String.Concat(MethodPrintName, " Statistical Analysis"));

            StreamWriter methodFail = new StreamWriter(reportDirectory.FullName + MethodPrintName + "_methodFail.csv");
            StreamWriter methodFailList = new StreamWriter(reportDirectory.FullName + MethodPrintName + "_methodFailList.txt");

            StreamWriter rwStats_CA = new StreamWriter(reportDirectory.FullName + MethodPrintName + "_stats_CA.csv");
            StreamWriter rwStats_BB = new StreamWriter(reportDirectory.FullName + MethodPrintName + "_stats_BB.csv");
            StreamWriter rwStats_AA = new StreamWriter(reportDirectory.FullName + MethodPrintName + "_stats_AA.csv");
            StreamWriter rwStats_BBA = new StreamWriter(reportDirectory.FullName + MethodPrintName + "_stats_BBA.csv");

            for (int loopLength = StartLoopLength; loopLength <= EndLoopLength; loopLength++)
            {
                HTMLStartReportBlock(String.Format("{0}-mers", loopLength));

                // Derived class hook call
                JobBeginHook(loopLength);

                StreamWriter rw = new StreamWriter(reportDirectory.FullName + MethodPrintName + "_rep_" + loopLength.ToString() + ".csv");

                string pdbPath = baseDirectory.FullName + "pdb\\";

                // Cartesian
                List<double> CACRMS = new List<double>();
                List<double> BackboneCRMS = new List<double>();
                List<double> AllAtomCRMS = new List<double>();
                List<double> BackboneARMS = new List<double>();

                int methodFailureCount = 0;
                bool methodFailureHit = false;
                int totalLoops = 0;
                ParsingFileIndex = 0; // reset IMPORTANT                
                while (true)
                {
                    string currentName = CurrentFile.InternalName.Substring(0, 5);
                    Trace.Write(currentName);
                    Trace.Write("> ");

                    // Original PDB file
                    PDB libPDB = null;
                    if (!m_MethodSuppliesTemplate)
                    {
                        // Load the PDB from our library
                        libPDB = new PDB(pdbPath + currentName + ".min.pdb", true);
                    }
                    // else the method will use its own PDB template for the creation of the 'LoopStatAnalyse' object below

                    SegmentDef[] loops = CurrentFile.GetLoops(loopLength, false, true);
                    for (int i = 0; i < loops.Length; i++)
                    {
                        string pdbID = CurrentFile.InternalName.Substring(0, 5);

                        if (!PreAnalysisHook(currentName, pdbID, libPDB, loops[i]))
                        {
                            continue;
                        }

                        totalLoops++;
                        methodFailureHit = false;
                        LoopStatAnalyse stat = DoStructuralAnalysis(currentName, pdbID, libPDB, loops[i], ref methodFailureHit);
                        if (methodFailureHit)
                        {
                            methodFailureCount++;
                            methodFailList.WriteLine("{0}_{1}_{2}", pdbID, loops[i].FirstDSSPIndex, loops[i].Length);
                            methodFailList.Flush();
                        }
                        if (stat == null)
                        {
                            Trace.Write('X');
                            continue;
                        }

                        // Report the best one we found
                        string printStem = String.Format("{0}_{1}_{2}", pdbID, loops[i].FirstDSSPIndex, loops[i].Length);
                        stat.Report(rw, printStem, 0);

                        CACRMS.Add(stat.getRMS_CA(0));
                        BackboneCRMS.Add(stat.getRMS_BB(0));
                        AllAtomCRMS.Add(stat.getRMS_AA(0));
                        BackboneARMS.Add(stat.getRMS_BBA(0));

                        if (!PostAnalysisHook(currentName, pdbID, libPDB, loops[i]))
                        {
                            continue;
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
                        double cutoffA = 1.0;
                        double cutoffB = 2.0;
                        double cutoffC = 3.0;
                        double within1 = 0.5;
                        double within2 = 0.75;
                        double within3 = 0.90;

                        DoStatBoxHeader(false, cutoffA, cutoffB, cutoffC, within1 * 100.0, within2 * 100.0, within3 * 100.0);
                        DoStatBoxes(loopLength, "C&alpha;", rwStats_CA, CACRMS, cutoffA, cutoffB, cutoffC, within1, within2, within3);
                        DoStatBoxes(loopLength, "Backbone", rwStats_BB, BackboneCRMS, cutoffA, cutoffB, cutoffC, within1, within2, within3);
                        DoStatBoxes(loopLength, "All Heavy Atom", rwStats_AA, AllAtomCRMS, cutoffA, cutoffB, cutoffC, within1, within2, within3);

                        cutoffA = 10.0;
                        cutoffB = 20.0;
                        cutoffC = 30.0;

                        DoStatBoxHeader(true, cutoffA, cutoffB, cutoffC, within1 * 100.0, within2 * 100.0, within3 * 100.0);
                        DoStatBoxes(loopLength, "Backbone Torsion", rwStats_BBA, BackboneARMS, cutoffA, cutoffB, cutoffC, within1, within2, within3);

                        DoStatBoxFooter();
                        break; // we have done the DSSP set, break the while loop
                    }
                }

                rw.Close();
                methodFail.WriteLine("{0},{1},{2}", loopLength, methodFailureCount, totalLoops);

                // Derived class hook call
                JobEndHook(loopLength);

                DoOrigin(loopLength, CACRMS, BackboneCRMS, AllAtomCRMS, BackboneARMS);

                HTMLEndReportBlock();

                continue;
            }            

            rwStats_AA.Close();
            rwStats_BB.Close();
            rwStats_CA.Close();
            rwStats_BBA.Close();

            methodFail.Close();
            methodFailList.Close();

            HTMLReportingEnd();

            // Kill the origin app
            InteractOrigin.Terminate();

            return;
        }

        private void DoStatBoxHeader( bool arms, double cutoff1, double cutoff2, double cutoff3, double within1, double within2, double within3 )
        {
            if (!arms)
            {
                m_HTMLReporter.WriteLine("<center><table width=800 border=1 bordercolor=black cellpadding=0 cellspacing=0 >");
            }
            m_HTMLReporter.WriteLine("<tr><td>&nbsp;</td><td colspan=5><center>Averages</center></td><td colspan=3><center>% < X RMSD ({0})<center></td><td colspan=3><center>Value Which Y% Are Within</center></td></tr>", arms ? "degrees" : "&Aring;");
            m_HTMLReporter.WriteLine("<tr><td>RMSD Type</td><td>Mean</td><td>Median</td><td>Max</td><td>Min</td><td>StdDev</td><td>{0:f1}{6}</td><td>{1:f1}{6}</td><td>{2:f1}{6}</td><td>{3:f0}%</td><td>{4:f0}%</td><td>{5:f0}%</td></tr>",
                cutoff1, cutoff2, cutoff3, within1, within2, within3, arms ? "&deg;" : "&Aring;");
        }

        private void DoStatBoxFooter()
        {
            m_HTMLReporter.WriteLine("</table></center><br>");
        }

        private void DoStatBoxes(int loopLength, string dataL, StreamWriter rwStats, List<double> data, double xPercCutoff1, double xPercCutoff2, double xPercCutoff3, double within1, double within2, double within3)
        {
            double mean = -1.0, median = -1.0, max = -1.0, min = -1.0, stdDev = -1.0, percLower1 = -1.0, percLower2 = -1.0, percLower3 = -1.0, countWithin1 = -1.0, countWithin2 = -1.0, countWithin3 = -1.0;

            if (data.Count > 0)
            {
                mean = MathsTools.Mean(data);
                median = MathsTools.Median(data);
                max = MathsTools.Max(data);
                min = MathsTools.Min(data);
                stdDev = MathsTools.StdDev(data);
                percLower1 = MathsTools.PercLower(data, xPercCutoff1) * 100.0;
                percLower2 = MathsTools.PercLower(data, xPercCutoff2) * 100.0;
                percLower3 = MathsTools.PercLower(data, xPercCutoff3) * 100.0;
                countWithin1 = MathsTools.ValueAtPercentageCutoff(data, within1, true);
                countWithin2 = MathsTools.ValueAtPercentageCutoff(data, within2, true);
                countWithin3 = MathsTools.ValueAtPercentageCutoff(data, within3, true);
            }

            rwStats.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                loopLength, mean, median, max, min, stdDev, percLower1, percLower2, percLower3, countWithin1, countWithin2, countWithin3
            );

            m_HTMLReporter.WriteLine("<tr><td>{0}</td><td>{1:f3}</td><td>{2:f3}</td><td>{3:f3}</td><td>{4:f3}</td><td>{5:f3}</td><td>{6:f3}</td><td>{7:f3}</td><td>{8:f3}</td><td>{9:f3}</td><td>{10:f3}</td><td>{11:f3}</td></tr>",
                dataL, mean, median, max, min, stdDev, percLower1, percLower2, percLower3, countWithin1, countWithin2, countWithin3);
        }

        private void DoOrigin(int loopLength, List<double> CACRMS, List<double> BackboneCRMS, List<double> AllAtomCRMS, List<double> BackboneARMS )
        {
            string angstrom = new String((char)197,1); // '197' is the ASCII code for the angstrom symbol
            m_HTMLReporter.Write("<center><table><tr><td>");
            DoGraph(loopLength, CACRMS, "ca", "C\\g(a)", 0.5, angstrom); // '\g(a) interprets the 'a' as an 'alpha'
            m_HTMLReporter.Write("</td><td>");
            DoGraph(loopLength, BackboneCRMS, "bb", "Backbone", 0.5, angstrom);
            m_HTMLReporter.Write("</td></tr><tr><td>");
            DoGraph(loopLength, AllAtomCRMS, "aa", "All Heavy Atom", 0.5, angstrom);
            m_HTMLReporter.Write("</td><td>");
            DoGraph(loopLength, BackboneARMS, "bba", "Backbone (\\g(F,Y,W)) Torsional", 5.0, "Degrees"); // F Y and W are the origin codes for greek capitals for Phi Psi and Omega
            m_HTMLReporter.Write("</td></tr></table></center>");
        }

        private void DoGraph(int loopLength, List<double> dataL, string fileRMSName, string RMSSubType, double binSize, string units )
        {
            int thumbHeight = 300;
            int thumbWidth = 400;
            int imgHeight = 960;
            int imgWidth = 1280;
            string imgSaveStem = String.Format("{1}_{0}_{2}", loopLength, fileRMSName, MethodPrintName);
            string imgSavePath = String.Concat(reportDirectory.FullName, Path.DirectorySeparatorChar, imgSaveStem);
            string imgSaveThumbPath = String.Concat(reportDirectory.FullName, Path.DirectorySeparatorChar, "T_", imgSaveStem);
            ImageType imgExt = ImageType.PNG; // PNG files rock dude!

            // Convert the data to an array that origin can use
            double[] data = dataL.ToArray();

            // Final bin limit calc
            double max = MathsTools.Max(dataL);
            double rem = max % binSize;
            max = max + binSize - rem;

            double yInc = 20.0;
            if (dataL.Count < 750) yInc = 15.0;
            if (dataL.Count < 500) yInc = 10.0;

            string RMSType = String.Format("{0} RMSD ({1})", RMSSubType, units);
            string title = String.Format("Histogramatic Distribution of Structural {2} RMSD\nValues Representing the Single Best Energy Model Generated\nfor each {1}-mer Loop Candidate by the Method '{0}'",
                MethodPrintName, loopLength, RMSSubType);
            
            // Tell origin how to make the graph, and then instruct it to save the image
            InteractOrigin.UpdateWorksheet("data", 0, data);            
            //InteractOrigin.ChangeLabel("hist", "title", title); // we now dont have a graph title, print .figure files instead
            InteractOrigin.ChangeXAxisLabel("hist", String.Format("{0}", RMSType));
            InteractOrigin.ChangeYAxisLabel("hist", String.Format("Occurence"));            
            InteractOrigin.SetHistogramBin("hist", "data_a", 0, binSize, max );
            InteractOrigin.SetXAxisIncrement("hist", binSize * 2);
            InteractOrigin.SetYAxisIncrement("hist", yInc);
            InteractOrigin.RescaleAuto("hist");
            InteractOrigin.SaveEPSPicture(imgSavePath + ".eps", "hist");
            InteractOrigin.SavePicture(imgExt, imgSavePath, "hist", imgWidth, imgHeight);
            InteractOrigin.SavePicture(imgExt, imgSaveThumbPath, "hist", thumbWidth, thumbHeight);
            InteractOrigin.Save(imgSavePath + ".opj"); // just in case we want a minor edit later, save the full file
                        
            m_HTMLReporter.WriteLine(String.Format("<a href=\"{2}\" border=0><img src=\"T_{2}\" height={1} width={0}><a/>", thumbWidth, thumbHeight, imgSaveStem + "." + imgExt.ToString() ));
            m_HTMLReporter.WriteLine("<center><H3>{0}</H3></center>", OriginFormatting.OriginSymbolToHTML(title));

            StreamWriter rw = new StreamWriter(imgSavePath + ".figure");
            rw.WriteLine(OriginFormatting.OriginSymbolToLatex(title));
            rw.Close();
        }
    }

    class LoopStatAnalyse
    {
        private const int m_LoadWeight = 1000;

        // Energetics (optional)
        private List<double> m_Energy = new List<double>(m_LoadWeight);

        // Cartesian
        private List<double> m_CACRMS = new List<double>(m_LoadWeight);
        private List<double> m_BackboneCRMS = new List<double>(m_LoadWeight);
        private List<double> m_AllAtomCRMS = new List<double>(m_LoadWeight);

        // Angular
        private List<double> m_BackboneARMS = new List<double>(m_LoadWeight);

        private ParticleSystem m_HostSystem = null;
        private PolyPeptide m_HostChain = null;
        private PolyPeptide m_HostLoop = null;

        SegmentDef m_SegDef;
        private int m_StartRes;
        private int m_Length;
        private int m_LengthPlusAnc;

        /// <summary>
        /// Once all fragments have been analysed, we no longer need the parent system referenecs
        /// </summary>
        bool m_MemPurged = false;
        public void FreeParticleSystemMemory()
        {        
            m_HostSystem = null;
            m_HostChain = null;
            m_HostLoop = null;
            m_SegDef = null;
            m_MemPurged = true;
        }

        public double getEne(int index)
        {
            return m_Energy[index];
        }

        public double getRMS_CA(int index)
        {
            return m_CACRMS[index];
        }

        public double getRMS_BB(int index)
        {
            return m_BackboneCRMS[index];
        }

        public double getRMS_AA(int index)
        {
            return m_AllAtomCRMS[index];
        }

        public double getRMS_BBA(int index)
        {
            return m_BackboneARMS[index];
        }

        public LoopStatAnalyse( ParticleSystem hostSystem, SegmentDef segDef )
        {
            m_HostSystem = hostSystem;
            if( m_HostSystem == null || m_HostSystem.MemberCount != 1 ) throw new ArgumentException();
            m_HostChain = m_HostSystem.MemberAt(0) as PolyPeptide;
            if (m_HostChain == null) throw new ArgumentException();
            AssertBackboneAtoms(m_HostChain);

            m_SegDef = segDef;
            m_StartRes = segDef.FirstDSSPIndex-1;
            m_Length = segDef.Length;
            m_LengthPlusAnc = m_Length + 2;
            if (m_StartRes < 0 || m_StartRes >= m_HostChain.Count) throw new ArgumentException();
            if (m_Length < 1 || m_StartRes + m_Length >= m_HostChain.Count ) throw new ArgumentException();

            // Do some chopping
            m_HostLoop = new PolyPeptide(m_HostChain.ChainID);
            int endAt = m_StartRes + m_Length + 1;
            for (int i = m_StartRes - 1; i < endAt; i++)
            {
                m_HostLoop.addMolecule(m_HostChain[i].Clone() as AminoAcid);
            }
            AssertBackboneAtoms(m_HostLoop);

            // Do the required re-setup
            doGenHost(m_HostLoop);
        }

        public void ClearStats()
        {
            m_CACRMS.Clear();
            m_BackboneCRMS.Clear();
            m_AllAtomCRMS.Clear();
            m_BackboneARMS.Clear();
        }

        public void Report(StreamWriter rw, string name, int index)
        {
            if (index < 0 || index >= m_CACRMS.Count) throw new ArgumentOutOfRangeException();
            rw.WriteLine("{0},{1},{2},{3},{4},{5}", name, index, m_CACRMS[index], m_BackboneCRMS[index], m_AllAtomCRMS[index], m_BackboneARMS[index]);
        }

        public void AnalyseFragment(ParticleSystem fragmentPS, double energy)
        {
            AnalyseFragment(fragmentPS, 0, energy);
        }

        private void doGenHost(PolyPeptide clonedPoly)
        {
            ParticleSystem genHost = new ParticleSystem("");
            genHost.BeginEditing();
            genHost.AddMolContainer(clonedPoly);
            // Reassign the mol primitives to cause atom sorting and removal of the N and C prefixes from the fragment
            for (int i = 0; i < clonedPoly.Count; i++)
            {
                clonedPoly[i].setMolPrimitive(true);
            }
            genHost.EndEditing(true, true);

            //UoB.Core.FileIO.PDB.PDB.SaveNew(@"c:\debug.pdb", genHost, true);
        }

        public void AnalyseFragment(ParticleSystem fragmentPS, int anchorFileCount, double energy)
        {
            if (m_MemPurged) throw new Exception("No further fragment analysis allowed following parent PS purge");

            if (fragmentPS == null || fragmentPS.MemberCount != 1) throw new ArgumentException();
            PolyPeptide imcommingFrag = fragmentPS.MemberAt(0) as PolyPeptide;
            AssertIncomingBackoneAtoms(imcommingFrag, anchorFileCount);

            m_Energy.Add(energy);

            PolyPeptide genFrag = null;

            if (anchorFileCount < 0)
            {
                // The loop result PSystem svhould the whole Host PSytem - verify this
                if (imcommingFrag.Count != m_HostChain.Count) throw new Exception();
                PolyPeptide subFrag = new PolyPeptide(imcommingFrag.ChainID);
                int endAt = m_StartRes + m_Length + 1;
                for (int i = m_StartRes - 1; i < endAt; i++)
                {
                    AminoAcid aa = imcommingFrag[i].Clone() as AminoAcid;
                    if (aa == null)
                        throw new Exception();
                    subFrag.addMolecule(aa);
                }
                genFrag = subFrag; // we now want to look at this
            }
            else if (anchorFileCount == 1)
            {
                // do nothing! - we already have what we require - but check that
                if (imcommingFrag.Count != m_LengthPlusAnc)
                {
                    throw new Exception();
                }
                genFrag = imcommingFrag.Clone() as PolyPeptide;

                if (m_MissingHack)
                {
                    // The first and last loop residues have missing backbone atoms! 
                    // - add them from the host chain for analysis of the fragment
                    AddAtomFromDonor(" N  ", genFrag, 0, m_HostLoop, 0);
                    AddAtomFromDonor(" CA ", genFrag, 0, m_HostLoop, 0);
                    int ii = genFrag.Count - 1;
                    AddAtomFromDonor(" CA ", genFrag, ii, m_HostLoop, ii);
                    AddAtomFromDonor(" C  ", genFrag, ii, m_HostLoop, ii);
                    AddAtomFromDonor(" O  ", genFrag, ii, m_HostLoop, ii);
                }
            }
            else if (anchorFileCount == 0)
            {
                // We are going to need to borrow anchor residues from the host chain for analysis ...
                if (imcommingFrag.Count != m_Length)
                {
                    throw new Exception();
                }

                PolyPeptide subFrag = new PolyPeptide(imcommingFrag.ChainID);
                
                // Add the required residues from the host system for the anchors and the fragment for the loop
                subFrag.addMolecule((AminoAcid)m_HostLoop[0].Clone());
                int endAt = m_StartRes + m_Length;
                for (int i = 0; i < m_Length; i++)
                {
                    AminoAcid aa = imcommingFrag[i].Clone() as AminoAcid;
                    if (aa == null)
                        throw new Exception();
                    subFrag.addMolecule(aa);
                }
                subFrag.addMolecule((AminoAcid)m_HostLoop[m_HostLoop.Count - 1].Clone());

                if (m_MissingHack)
                {
                    // The first and last loop residues have missing backbone atoms! 
                    // - add them from the host chain for analysis of the fragment
                    AddAtomFromDonor(" N  ", subFrag, 1, m_HostLoop, 1);
                    AddAtomFromDonor(" CA ", subFrag, 1, m_HostLoop, 1);
                    int ii = subFrag.Count - 2;
                    AddAtomFromDonor(" CA ", subFrag, ii, m_HostLoop, ii);
                    AddAtomFromDonor(" C  ", subFrag, ii, m_HostLoop, ii);
                    AddAtomFromDonor(" O  ", subFrag, ii, m_HostLoop, ii);                   
                }
                
                genFrag = subFrag; // we now want to look at this
            }
            else if (anchorFileCount > 1)
            {
                // There are no current method cases with too many anchor residues defined.
                throw new NotSupportedException();
            }

            // Do the required re-setup
            doGenHost(genFrag);        
            
            // Final assertions
            if (m_HostLoop.Count != m_LengthPlusAnc || genFrag.Count != m_LengthPlusAnc)
            {
                throw new Exception();
            }

            // Check the sequence
            for (int i = 0; i < m_LengthPlusAnc; i++)
            {
                if (0 != String.Compare(genFrag[i].Name_NoPrefix, m_HostLoop[i].Name_NoPrefix))
                {
                    if( 0 != genFrag[i].Name_NoPrefix.CompareTo("HIE") )
                    {
                        throw new Exception();
                    }
                }
                char singID = genFrag[i].moleculePrimitive.SingleLetterID;
                if (i != 0 && i != m_LengthPlusAnc-1 && 
                    Char.ToUpper(m_SegDef.Sequence[i-1]) != Char.ToUpper(singID))
                {                    if (singID == '?')
                    {
                        if( m_SegDef.Sequence[i-1] == 'H' && 
                            (0 == genFrag[i].Name_NoPrefix.CompareTo("HIP") || 0 == genFrag[i].Name_NoPrefix.CompareTo("HSD"))
                            )
                        {
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }

            AssertBackboneAtoms(genFrag);

            ComputeRMS(genFrag);

            return;
        }

        private void AddAtomFromDonor( string name, PolyPeptide frag, int indexF, PolyPeptide donor, int indexD )
        {
            if (null == frag[indexF].AtomOfType(name))
            {
                Atom a = donor[indexD].AtomOfType(name).Clone() as Atom;
                if (a == null) throw new Exception();
                frag[indexF].addAtom(a);
            }
            else
            {
                // Assumption failed
                throw new Exception("No donor atom needed!!");
            }
        }

        private bool m_MissingHack = false;
        public bool MissingHack
        {
            get
            {
                return m_MissingHack;
            }
            set
            {
                m_MissingHack = value;
            }
        }

        private void AssertIncomingBackoneAtoms(PolyPeptide poly, int anchorFileCount)
        {
            if (m_MissingHack)
            {
                for (int i = 0; i < poly.Count; i++)
                {
                    if (null == poly[i].AtomOfType(" N  "))
                    {
                        if (i != 0) throw new Exception();
                    }
                    if (null == poly[i].AtomOfType(" CA "))
                    {
                        if (i != 0 && i != poly.Count - 1) throw new Exception();
                    }
                    if (null == poly[i].AtomOfType(" C  "))
                    {
                        if (i != poly.Count - 1) throw new Exception();
                    }
                    if (null == poly[i].AtomOfType(" O  "))
                    {
                        if (i != poly.Count - 1) throw new Exception();
                    }
                }
            }
            else
            {
                AssertBackboneAtoms(poly);
            }
        }

        private void AssertBackboneAtoms(PolyPeptide poly)
        {
            for( int i = 0; i < poly.Count; i++ )
            {
                if( null == poly[i].AtomOfType(" N  " ) ||
                    null == poly[i].AtomOfType(" C  " ) ||
                    null == poly[i].AtomOfType(" O  " ) ||
                    null == poly[i].AtomOfType(" CA " ) )
                {
                    throw new Exception();
                }
            }
        }

        private void ComputeRMS(PolyPeptide fragment)
        {
            double devCA = 0.0;

            // Compute CA cRMS  
            int start = 1;
            int end = m_LengthPlusAnc-2;
            if (m_MissingHack)
            {
                start++;
                end--;
            }
            for (int i = start; i <= end; i++)
            {
                Atom fragCA = fragment[i].CAlphaAtom;
                Atom hostCA = m_HostLoop[i].CAlphaAtom;
                devCA += fragCA.distanceSquaredTo(hostCA);
            }
            devCA /= (double)(end-start+1);
            m_CACRMS.Add( Math.Sqrt(devCA) );

            // Compute BB cRMS
            double devBB = 0.0;
            int countBB = 0;
            for (int i = 1; i < m_LengthPlusAnc-1; i++)
            {
                if (atomRMS(fragment, m_HostLoop, i, " N  ", ref devBB)) countBB++;
                if (atomRMS(fragment, m_HostLoop, i, " CA ", ref devBB)) countBB++;
                if (atomRMS(fragment, m_HostLoop, i, " C  ", ref devBB)) countBB++;
                if (atomRMS(fragment, m_HostLoop, i, " O  ", ref devBB)) countBB++;
            }
            // All atoms for every residue will all be included and countBB incremented unless the atom is a "hack atom"
            if (!m_MissingHack && countBB != 4 * m_Length) throw new Exception();
            m_BackboneCRMS.Add(Math.Sqrt(devBB / (double)(countBB)));

            // Compute All Atom cRMS
            double devAA = 0.0;
            int countAA = 0;
            for (int i = 1; i < m_LengthPlusAnc - 1; i++)
            {
                Molecule loopResidue = fragment[i];
                Molecule libResidue = m_HostLoop[i];                
                for (int j = 0; j < loopResidue.Count; j++)
                {
                    if (loopResidue[j].atomPrimitive.Element != 'H' &&
                        atomRMS(fragment, m_HostLoop, i, loopResidue[j].PDBType, ref devAA))
                    {
                        countAA++;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            m_AllAtomCRMS.Add(Math.Sqrt(devAA / (double)(countAA)));

            // Now compute the backbone ARMS
            double bbARMSDev = 0.0f;
            int armsCount = 0;
            double delta;
            for (int i = 1; i < m_LengthPlusAnc - 1; i++)
            {
                AminoAcid fragAA = fragment[i];
                AminoAcid hostAA = m_HostLoop[i];

                armsCount++;
                double fragPhi = fragAA.phiAngle;
                double hostPhi = hostAA.phiAngle;
                delta = fragPhi - hostPhi;

                if( delta < 0.0 ) delta = -delta;
		        if( delta > 180.0 ) delta = 360.0 - delta;

                bbARMSDev += (delta * delta);

                armsCount++;
                double fragPsi = fragAA.psiAngle;
                double hostPsi = hostAA.psiAngle;
                delta = fragPsi - hostPsi;

                if (delta < 0.0) delta = -delta;
                if (delta > 180.0) delta = 360.0 - delta;

                bbARMSDev += (delta * delta);

                if (!(m_MissingHack && i == 1)) 
                {
                    armsCount++;
                    double fragOmg = fragAA.omgAngle;
                    double hostOmg = hostAA.omgAngle;
                    delta = fragOmg - hostOmg;

                    if (delta < 0.0) delta = -delta;
                    if (delta > 180.0) delta = 360.0 - delta;

                    bbARMSDev += (delta * delta);
                }
            }
            m_BackboneARMS.Add(Math.Sqrt(bbARMSDev / (double)(armsCount)));
        }

        private bool isHackAtom( int index, string name )
        {
            if (index == 1 && 0 == String.Compare(name, " N  ")) return true;
            if ((index == 1 || index == m_Length) && 0 == String.Compare(name, " CA ")) return true;
            if (index == m_Length && 0 == String.Compare(name, " C  ")) return true;
            if (index == m_Length && 0 == String.Compare(name, " O  ")) return true;
            return false;
        }

        private bool atomRMS(PolyPeptide loop, PolyPeptide host, int index, string atomName, ref double devSum )
        {
            if (m_MissingHack && isHackAtom(index,atomName) )
                return false;
            Atom a = loop[index].AtomOfType(atomName);
            Atom b = host[index].AtomOfType(atomName);
            if (a == null || b == null) 
                throw new Exception();
            double dev = a.distanceSquaredTo(b);
            devSum += dev;
            return true;
        }
    }
}
