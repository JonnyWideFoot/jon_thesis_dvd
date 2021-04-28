using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using UoB.Core.FileIO.DSSP;
using UoB.Methodology.DSSPAnalysis;
using UoB.Core.FileIO.PDB;
using UoB.Core.Structure;
using UoB.Core.Primitives;
using UoB.AppLayer.Common;
using UoB.Core.Tools;

namespace UoB.AppLayer.Plop
{
    class RMSDLine
    {
        public int index;
        public double ene;
        public void Read(string line, int expectedIndex)
        {
            string[] lineParts = UoB.Core.Tools.CommonTools.WhiteSpaceRegex.Split(line.Trim());
            if (lineParts.Length != 8) throw new Exception();
            if (expectedIndex != int.Parse(lineParts[0])) throw new Exception();
            index = int.Parse(lineParts[1]);
            ene = double.Parse(lineParts[2]);
        }
    }

    class PlopProcess : DSSPLoopAnalysis
    {
        bool m_ObfuscateLoop;
        public PlopProcess(AppLayerBase parent)
            : base(parent)
        {
            m_ObfuscateLoop = true;
            m_MethodSuppliesTemplate = m_ObfuscateLoop;
        }

        public void mungeBadRMSDFiles()
        {
            for (int k = 6; k <= EndLoopLength; k++)
            {
                string resPath = this.resultDirectory.FullName + Path.DirectorySeparatorChar + k.ToString() + Path.DirectorySeparatorChar;
                DirectoryInfo di = new DirectoryInfo(resPath);
                FileInfo[] files = di.GetFiles("*.rmsd");
                for (int i = 0; i < files.Length; i++)
                {
                    mungeBadRMSDFiles(files[i]);
                }
            }
        }

        private void mungeBadRMSDFiles(FileInfo file)
        {
            StreamReader re = new StreamReader(file.FullName);
            string line1 = re.ReadLine();
            string line2 = re.ReadLine();
            re.Close();
            if (line1 != null && line1.Length > 0 && line2 == null)
            {
                // Oh booger !
                string outname = file.DirectoryName + Path.DirectorySeparatorChar + 
                    Path.GetFileNameWithoutExtension(file.Name) + ".out";

                List<string> lines = new List<string>();
                re = new StreamReader(outname);
                string line;
                while (null != (line = re.ReadLine()))
                {
                    lines.Add(line);
                }
                re.Close();

                int index = mungeSeek(lines);
                if (index == -1) return;

                StreamWriter rw = new StreamWriter(file.FullName);

                rw.WriteLine("   -1   -1       0.00       0.00  0.00  0.00  0.00  0.00");
                rw.WriteLine("    0    0       0.00       0.00  0.00  0.00  0.00  0.00");

                int jj = 0;
                for (int i = index; i < lines.Count; i++)
                {
                    if (lines[i].Trim().Length == 0) break;
                    rw.WriteLine(
                        munge(lines[i],++jj)
                        );
                    rw.Flush();
                }

                rw.Close();

                return;
            }            
        }

        private string munge(string line, int index)
        {
            string[] lineParts = UoB.Core.Tools.CommonTools.WhiteSpaceRegex.Split(line.Trim());
            string newLine = String.Format("{0,5:G}{1,5:G}   {2,8:N2}   {3,8:N2}{4,6:N2}{5,6:N2}{6,6:N2}{7,6:N2}",
                index, lineParts[0], lineParts[2], 
                lineParts[3], lineParts[4], lineParts[5], lineParts[6], lineParts[7]);
            return newLine;
        }

        private int mungeSeek(List<string> lines)
        {
            bool foundTerm = false;
            for (int i = lines.Count - 1; i > 0; i--)
            {
                if( lines[i].Length >= 17 && 0 == String.CompareOrdinal(lines[i],0," Results so far: ",0,17))
                {
                    if (!foundTerm) throw new Exception();
                    return i + 1;
                }
                if (lines[i].Length >= 20 && 0 == String.CompareOrdinal(lines[i], 0, " TOTAL TIME ELAPSED:", 0, 20))
                {
                    foundTerm = true;
                }
            }
            return -1;
        }

        private void GetFromRMSD(string fileName, out int total, out int bestE, out double bestEValue)
        {
            total = 0;
            bestE = -1;

            StreamReader re = new StreamReader(fileName);
            string line;

            line = re.ReadLine();
            if (line == null) throw new Exception(); // read in -1
            line = re.ReadLine();
            if (line == null) throw new Exception(); // read in 0

            RMSDLine rmsd = new RMSDLine();
            bestEValue = double.MaxValue;
            while (null != (line = re.ReadLine()))
            {
                rmsd.Read(line, ++total);
                if (rmsd.ene < bestEValue)
                {
                    bestEValue = rmsd.ene;
                    bestE = rmsd.index;
                }
            }

            re.Close();
        }

        protected override LoopStatAnalyse DoStructuralAnalysis(string currentName, string pdbID, PDB libPDB, SegmentDef loop, ref bool methodFailure)
        {
            string resPath = resultDirectory.FullName + loop.Length.ToString() + Path.DirectorySeparatorChar;
            string stem = String.Format("{0}_{1}_{2}", pdbID, loop.FirstDSSPIndex - 1, loop.Length);

            // Assert results are valid
            Validity returnVal = PlopFunctions.IsValidReturn(resPath, stem, loop.FirstDSSPIndex - 1, loop.LastDSSPIndex - 1);
            if (Validity.MethodFail == returnVal)
            {
                methodFailure = true;
                Trace.WriteLine("Flagged Method Failure: {0}", stem);
                return null;
            }
            else if (Validity.Invalid == returnVal)
            {
                //throw new Exception();
                methodFailure = true; // there are now only two special 'Invalid' cases ... these have failed, but are odd to detect
                // I have manually looked at each one of these
                Trace.WriteLine("Flagged 'hard-detect' Method Failure: {0}", stem);
                return null;
            }

            // PLOP generated loop file
            PDB loopPDB = new PDB(resPath + stem + ".pdb", true);
            if (loopPDB.PositionDefinitions.Count < 4)
            {
                methodFailure = true;
                return null;
                //throw new Exception();
            }

            // Identify the best loop using the information in the RMSD file
            int total;
            int bestE;
            double bestEValue;
            GetFromRMSD(resPath + stem + ".rmsd", out total, out bestE, out bestEValue);
            if (bestE == -1) throw new Exception();
            if (loopPDB.PositionDefinitions.Count - 3 != total)
            {
                // This should NOT be possible
                throw new Exception();
            }
            // ensure that we load the model 'bestE'! - this is the "best" loop.                        
            // File index -2 is the original structure - this is our '0'
            // -1 is the minimised structure
            //  0 is the minimised structure with sidechain opt
            //  1 is the first model generated, this is not nececarily the bestE
            loopPDB.PositionDefinitions.setPositionsTo(bestE + 2);

            libPDB = new PDB(String.Concat(baseDirectory.FullName, 
                Path.DirectorySeparatorChar, "pdb_source", Path.DirectorySeparatorChar,
                currentName, ".min.pdb"), true);

            if (!libPDB.particleSystem.AssertPositions())
            {
                methodFailure = true;
                return null;
            }
            if (!loopPDB.particleSystem.AssertPositions() )
            {
                methodFailure = true;
                return null;
            }

            LoopStatAnalyse stat = new LoopStatAnalyse(libPDB.particleSystem, loop);
            stat.MissingHack = true;
            stat.AnalyseFragment(loopPDB.particleSystem, m_ObfuscateLoop? 1 : 0, bestEValue); // Perform Analysis

            return stat;
        }        
    }
}
