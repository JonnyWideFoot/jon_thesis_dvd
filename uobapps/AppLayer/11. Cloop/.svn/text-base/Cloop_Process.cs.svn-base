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

namespace UoB.AppLayer.Cloop
{
    class CloopProcess : DSSPLoopAnalysis
    {
        public CloopProcess(AppLayerBase parent)
            : base(parent)
        {
            m_MethodSuppliesTemplate = true;
        }

        string m_CharmVString = "c30b2";

        protected override LoopStatAnalyse DoStructuralAnalysis(string currentName, string pdbID, PDB libPDB, SegmentDef loop, ref bool methodFailure)
        {
            // This should have been disabled
            if (libPDB != null) throw new Exception();

            string resPath = baseDirectory.FullName + "Result_" + m_CharmVString + Path.DirectorySeparatorChar + loop.Length.ToString() + Path.DirectorySeparatorChar;
            string stem = String.Format("{0}_{1}_{2}", pdbID, loop.FirstDSSPIndex, loop.Length);

            // Assert results are valid
            Validity returnVal = CloopFunctions.IsValidReturn(resPath, stem);
            if (Validity.Fail == returnVal)
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

            string archive = resPath + stem + ".tar.bz2";
            string archiveOut = resPath + stem + ".out.bz2";
            m_Comp.Uncompress(archive);
            //m_Comp.Uncompress(archiveOut); - not needed for normal analysis

            DirectoryInfo archiveDI = m_Comp.OutPath(archive);
            string archivePath = archiveDI.FullName + Path.DirectorySeparatorChar;

            if (/*returnVal != Validity.Valid &&*/ !CloopFunctions.CheckArchive(m_Comp, resPath, stem, loop.Length, false))
            {
                throw new Exception();
            }

            // Determine the lowest energy conformation from the ensemble of 1000 structures
            int bestEne = -1;
            double bestValue = double.MaxValue;
            StreamReader re = new StreamReader(String.Concat(archivePath, currentName, ".ene"));

            // Ignore header
            re.ReadLine(); // " NONE *"
            re.ReadLine(); // "  DATE:     3/14/ 7     16:44:43      CREATED BY USER: jr0407"

            string line;
            int wantIndex = 1;
            while (null != (line = re.ReadLine()))
            {
                line = line.Trim();
                string[] lineParts = UoB.Core.Tools.CommonTools.WhiteSpaceRegex.Split(line);
                if (0 != String.Compare(lineParts[0], currentName)) throw new Exception();
                int index = int.Parse(lineParts[3]);
                double ene = double.Parse(lineParts[6]);
                if (index != wantIndex++) throw new Exception();
                if (ene < bestValue)
                {
                    bestEne = index;
                    bestValue = ene;
                }
            }
            re.Close();
            if (wantIndex != 1001) throw new Exception("We havent got all the conformations required!");

            // The libPDB has been reoriented and therefore must be loaded from the simulation output and not from the database
            libPDB = new PDB( String.Concat(archivePath, currentName, ".pdb"), true);
            CharmCTerminusOHack(libPDB.particleSystem);

            string confName = String.Format("{0}conf{1}.pdb", archivePath, bestEne.ToString().PadLeft(3, '0'));
            PDB loopPDB = new PDB(confName, true);

            LoopStatAnalyse stat = new LoopStatAnalyse(libPDB.particleSystem, loop);
            stat.MissingHack = false;
            stat.AnalyseFragment(loopPDB.particleSystem, 1, bestEne); // Perform Analysis

            m_Comp.CleanUp(archive);

            return stat;
        }

        private void CharmCTerminusOHack(ParticleSystem ps)
        {
            ps.BeginEditing();
            PolyPeptide poly = ps.MemberAt(0) as PolyPeptide;
            if (poly == null) throw new Exception();
            AminoAcid aa = poly[poly.Count - 1];
            Atom a = aa.AtomOfType(" OT1");
            Atom b = new Atom(" O  ",a.ArrayIndex,a.AtomNumber,a.parentMolecule,a.x, a.y, a.z, a.Occupancy, a.TempFactor );
            aa.RemoveAtom( a );
            aa.addAtom( b );
            aa.setMolPrimitive(true);
            ps.EndEditing(true, true );
            return;
        }
    }
}

