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

namespace UoB.AppLayer.Rapper
{
    class RapperProcess : DSSPLoopAnalysis
    {
        public RapperProcess(AppLayerBase parent)
            : base(parent)
        {
        }
        
        protected override LoopStatAnalyse DoStructuralAnalysis(string currentName, string pdbID, PDB libPDB, SegmentDef loop, ref bool methodFailure)
        {
            string resPath = resultDirectory.FullName + loop.Length.ToString() + Path.DirectorySeparatorChar;
            string stem = String.Format("{0}_{1}_{2}", pdbID, loop.FirstDSSPIndex - 1, loop.Length);

            // Assert results are valid
            Validity returnVal = RapperFunctions.IsValidReturn(m_Comp, resPath, stem);
            if (Validity.MethodFail == returnVal)
            {
                methodFailure = true;
                Trace.WriteLine("Flagged Method Failure: {0}", stem);
                return null;
            }
            else if (Validity.Fail == returnVal)
            {
                //throw new Exception();
                methodFailure = true; // there are now only two special 'Invalid' cases ... these have failed, but are odd to detect
                // I have manually looked at each one of these
                Trace.WriteLine("Flagged 'hard-detect' Method Failure: {0}", stem);
                return null;
            }

            string archive = resPath + stem + ".tar.bz2";
            m_Comp.Uncompress(archive);
            string archivePath = m_Comp.OutPath(archive).FullName + Path.DirectorySeparatorChar;

            // These files are all not needed, although they signify that the run went to plan.
            if (
                !File.Exists(archivePath + "benchmark.dat") ||
                !File.Exists(archivePath + "framework.pdb") ||
                !File.Exists(archivePath + "looptest-best.pdb") || // oddly this file contains the best local CA CRMS and not the best energy
                !File.Exists(archivePath + "native.pdb"))
            {
                throw new FileNotFoundException();
            }

            FileInfo loopPDB = new FileInfo(archivePath + "looptest.pdb");
            if (!loopPDB.Exists)
            {
                throw new FileNotFoundException("Cannot find looptest.pdb");
            }

            FileInfo modelDat = new FileInfo(archivePath + "models.dat");
            if (!modelDat.Exists)
            {
                throw new FileNotFoundException("Cannot find models.dat");
            }

            // As rapper is not kind enough to give us a file containing the structure with the best energy, we will have to find it ourselves.
            // Determine the lowest energy conformation from the ensemble of 1000 structures
            int bestEne = -1;
            double bestValue = double.MaxValue;
            StreamReader re = new StreamReader(modelDat.FullName);

            // Ignore header
            re.ReadLine(); // "# [Model ID] [Model Name] [Main Chain RMSD]" ... etc.

            string line;
            int wantIndex = 0;
            int index = 0;
            while (null != (line = re.ReadLine()))
            {
                line = line.Trim();
                string[] lineParts = UoB.Core.Tools.CommonTools.WhiteSpaceRegex.Split(line);
                index = int.Parse(lineParts[0]);
                double ene = double.Parse(lineParts[14]);
                if (index != wantIndex++) throw new Exception();
                if (ene < bestValue)
                {
                    bestEne = index;
                    bestValue = ene;
                }
            }
            re.Close();
            if (wantIndex != 1000)
            {
                //throw new Exception("We havent got all the conformations required!");
                Trace.WriteLine("We havent got all 1000 the models! " + index + " was the final entry");
            }

            PDB loopPS = new PDB(loopPDB.FullName, true);
            loopPS.PositionDefinitions.Position = bestEne; // seek to the best loop

            LoopStatAnalyse stat = new LoopStatAnalyse(libPDB.particleSystem, loop);
            stat.MissingHack = false;
            stat.AnalyseFragment(loopPS.particleSystem, bestEne); // Perform Analysis

            m_Comp.CleanUp(archive);

            return stat;
        }        
    }
}

