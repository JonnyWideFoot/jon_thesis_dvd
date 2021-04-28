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

namespace UoB.AppLayer.Petra
{
    class PetraProcess : DSSPLoopAnalysis
    {
        public PetraProcess(AppLayerBase parent)
            : base(parent)
        {
            EndLoopLength = 8;
        }

        private bool m_PurePetra = true;
        public bool PurePetra
        {
            get
            {
                return m_PurePetra;
            }
            set
            {
                m_PurePetra = value;
            }
        }

        protected override LoopStatAnalyse DoStructuralAnalysis(string currentName, string pdbID, PDB libPDB, SegmentDef loop, ref bool methodFailure)
        {
            string resPath = resultDirectory.FullName + loop.Length.ToString() + Path.DirectorySeparatorChar;
            string stem = String.Format("{0}_{1}_{2}", pdbID, loop.FirstDSSPIndex, loop.Length);

            // Assert results are valid
            Validity returnVal = PetraFunctions.IsValidReturn(m_Comp, resPath, stem, loop);
            if (Validity.Valid != returnVal && Validity.Invalid != returnVal)
            {
                methodFailure = true;
                Trace.WriteLine("Flagged Method Failure");
                return null;
            }
            else if (Validity.Invalid == returnVal)
            {
                //throw new Exception();
                methodFailure = true; // there are now only two special 'Invalid' cases ... these have failed, but are odd to detect
                // I have manually looked at each one of these
                Trace.WriteLine("Flagged 'hard-detect' Method Failure");
                return null;
            }

            string archive = resPath + stem + ".tar.bz2";
            m_Comp.Uncompress(archive);
            DirectoryInfo archivePath = m_Comp.OutPath(archive);

            string ext;
            if (m_PurePetra)
            {
                ext = ".petra.pdb";
            }
            else
            {
                ext = ".coda.pdb";
            }
            PDB loopPDB = new PDB(resPath + stem + ext, true);

            m_Comp.CleanUp(archive);

            LoopStatAnalyse stat = new LoopStatAnalyse(libPDB.particleSystem, loop );
            stat.MissingHack = false;
            stat.AnalyseFragment(loopPDB.particleSystem, -1, 0.0); // Perform Analysis

            return stat;
        }         
    }
}

