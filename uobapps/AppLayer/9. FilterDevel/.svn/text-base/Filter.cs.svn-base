using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using UoB.Core.FileIO.DSSP;
using UoB.Core.FileIO.PDB;
using UoB.Methodology.DSSPAnalysis;
using UoB.Core.Structure;
using UoB.Core.Primitives;
using UoB.Core.ForceField.PopsSASA;

namespace UoB.AppLayer.Filter
{
    class FilterGenerator : DSSPTaskDirectory
    {
        private Pops pop = null;
        //int LOOP_BEGIN_LENGTH = 10;
        //int LOOP_END_LENGTH = 10;
        //int LOOP_BEGIN_LENGTH = 8;
        //int LOOP_END_LENGTH = 8;
        int LOOP_BEGIN_LENGTH = 1;
        int LOOP_END_LENGTH = 25;

        bool USE_MIN = true;

        public FilterGenerator(string DBName, DirectoryInfo di)
            : base(DBName, di, false)
        {
            pop = new Pops();
            string path = UoB.Core.CoreIni.Instance.DefaultSharedPath;
            pop.readDat(path + "Pops-dna.dat");
        }

        public void doFilter()
        {
            StreamWriter rw = new StreamWriter(resultDirectory.FullName + "distfan.dat");
            StreamWriter rwSASA = new StreamWriter(resultDirectory.FullName + "sasa.dat");
            StreamWriter rwSASAVersus = new StreamWriter(resultDirectory.FullName + "sasaAAvsCoarse.dat");
                         
            ParsingFileIndex = 0; // reset IMPORTANT
            while (true)
            {
                string currentName = CurrentFile.InternalName.Substring(0, 5);
                string pdbPath = null;
                if (USE_MIN)
                {
                    pdbPath = baseDirectory.FullName + "//PDB_Min//" + currentName + ".min.pdb";
                }
                else
                {
                    pdbPath = baseDirectory.FullName + "//PDB//" + currentName + ".pdb";
                }
                Trace.Write(currentName);
                Trace.Write(" -> ");

                // Extract the system
                PDB file = new PDB(pdbPath, true);
                ParticleSystem ps = file.particleSystem;
                if (ps.MemberCount != 1) throw new Exception();
                PolyPeptide poly = ps.MemberAt(0) as PolyPeptide;
                if (poly == null) throw new Exception();

                for (int loopLength = LOOP_BEGIN_LENGTH; loopLength <= LOOP_END_LENGTH; loopLength++)
                {
                    SegmentDef[] loops = CurrentFile.GetLoops(loopLength, false, true);
                   
                    for (int i = 0; i < loops.Length; i++)
                    {
                        Trace.Write('.');
                        AnalyseLoopDistFan(rw, currentName, loops[i], poly);
                        //AnalyseLoopSASA(rwSASA, rwSASAVersus, currentName, loops[i], poly);
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

            rw.Close();
            rwSASA.Close();
            rwSASAVersus.Close();
        }

        double max = double.MinValue;
        double min = double.MaxValue;
        SegmentDef maxDef = null;
        SegmentDef minDef = null;
        string maxName = null;
        string minName = null;

        private static readonly string numFormat = "{0:F4}"; // 4 decimal places

        void AnalyseLoopDistFan( StreamWriter rw, string name, SegmentDef def, PolyPeptide poly )
        {
            string seq = def.Sequence;
            if (seq.Length != def.Length) throw new Exception();
            int divider = (def.Length + 1) / 2;

            AminoAcid aaS = null;
            AminoAcid aaE = null;
            AminoAcid aaA1 = null;
            AminoAcid aaA2 = null;
            if (USE_MIN)
            {
                aaS = poly.GetMolecule(def.FirstDSSPIndex, ' ');
                aaE = poly.GetMolecule(def.LastDSSPIndex, ' ');
            }
            else
            {
                aaS = poly.GetMolecule(def.FirstResidueIndex, def.FirstResidueInsertionCode);
                aaE = poly.GetMolecule(def.LastResidueIndex, def.LastResidueInsertionCode);
            }
            aaA1 = poly.previousMolecule(aaS) as AminoAcid;
            aaA2 = poly.nextMolecule(aaE) as AminoAcid;
            if( aaS == null || aaE == null || aaA1 == null || aaA2 == null ) throw new Exception();

            int indexDiff = poly.IndexOf(aaE) - poly.IndexOf(aaS);
            if (indexDiff+1 != def.Length) throw new Exception();

            Atom a1A = aaA1.CAlphaAtom;
            Atom a1B = aaA1.CTerminalAtom;
            Atom a2A = aaA2.CAlphaAtom;
            Atom a2B = aaA2.NTerminalAtom;
            if (a1A == null || a1B == null || a2A == null || a2B == null) throw new Exception();

            double anchorSep = a1A.distanceTo(a2A);
            double anchorSep2 = a1B.distanceTo(a2B);
            double sepDiff = anchorSep - anchorSep2;
            //Position anchorCenter = Position.CenterPointBetween(a1B, a2B);

            if (sepDiff < min)
            {
                min = sepDiff;
                minDef = def;
                minName = name;
            }
            if (sepDiff > max)
            {
                max = sepDiff;
                maxDef = def;
                maxName = name;
            }

            rw.Write(def.Length);
            rw.Write(',');

            rw.Write(numFormat,anchorSep);

            AminoAcid aa = poly.nextMolecule(aaA1) as AminoAcid;
            for (int i = 0; i < def.Length; i++)
            {
                // Assert correct residue and 
                if (aa.moleculePrimitive.SingleLetterID != char.ToUpper(seq[i]))
                {
                    if (0 != String.Compare(aa.moleculePrimitive.MolName, "CYS"))
                    {
                        throw new Exception();
                    }
                }

                // get dist
                double dist = double.MaxValue;
                if (i < divider)
                {
                    dist = aa.CAlphaAtom.distanceTo(a2B);
                }
                else
                {
                    dist = aa.CAlphaAtom.distanceTo(a1B);
                }

                // Report
                rw.Write(',');
                rw.Write(numFormat, dist);
 
                // get the next AA
                aa = poly.nextMolecule(aa) as AminoAcid; 
            }

            rw.Write(',');
            rw.Write(numFormat, sepDiff);

            rw.WriteLine();

            //rw.Flush();

            return;
        }

        void AnalyseLoopSASA(StreamWriter rw, StreamWriter rwSASAVersus, string name, SegmentDef def, PolyPeptide poly)
        {
            string seq = def.Sequence;
            if (seq.Length != def.Length) throw new Exception();
            UoB.Core.ForceField.LoopAnalyticalSASA calc = new UoB.Core.ForceField.LoopAnalyticalSASA();

            //if(! (def.NTerminalDSSPType != 'H' || def.CTerminalDSSPType != 'E') )
            //{
            //    return;
            //}

            AminoAcid aaS = poly.GetMolecule(def.FirstResidueIndex, def.FirstResidueInsertionCode);
            AminoAcid aaE = poly.GetMolecule(def.LastResidueIndex, def.LastResidueInsertionCode);
            AminoAcid aaA1 = poly.previousMolecule(aaS) as AminoAcid;
            AminoAcid aaA2 = poly.nextMolecule(aaE) as AminoAcid;
            if (aaS == null || aaE == null || aaA1 == null || aaA2 == null) throw new Exception();

            int indexDiff = poly.IndexOf(aaE) - poly.IndexOf(aaS);
            if (indexDiff != def.Length - 1) throw new Exception();

            Atom a1A = aaA1.CAlphaAtom;
            Atom a1B = aaA1.CTerminalAtom;
            Atom a2A = aaA2.CAlphaAtom;
            Atom a2B = aaA2.NTerminalAtom;
            if (a1A == null || a1B == null || a2A == null || a2B == null) throw new Exception();

            double anchorSep = a1A.distanceTo(a2A);
            double anchorSep2 = a1B.distanceTo(a2B);
            double sepDiff = anchorSep - anchorSep2;
            Position anchorCenter = Position.CenterPointBetween(a1B, a2B);

            // Numerical analysis
            calc.connectLoop(poly,def);            
            calc.calcLoopSASAInfo();
            double numericalFract = calc.totalFraction;
            List<double> numericalList = calc.SASAs;
            List<double> numericMax = calc.maxSASAs;
            List<double> numericFrac = calc.fractSASAs;

            // All heavy atom analysis
            List<double> aaList = new List<double>();
            List<double> aaListFrac = new List<double>();
            List<double> aaListMax = new List<double>();
            pop.setTo(poly, PopsMode.AllAtom);
            pop.calc();
            for (int i = 0; i < def.Length; i++)
            {
                int calcIndex = i + def.FirstDSSPIndex - 1;
                aaList.Add(pop.resSASA(calcIndex));
                aaListFrac.Add(pop.resFract(calcIndex));
                aaListMax.Add(pop.resMax(calcIndex));
            }

            // All heavy atom analysis
            List<double> coarseList = new List<double>();
            List<double> coarseListFrac = new List<double>();
            List<double> coarseListMax = new List<double>();
            pop.setTo(poly, PopsMode.Coarse);
            pop.calc();
            for (int i = 0; i < def.Length; i++)
            {
                int calcIndex = i + def.FirstDSSPIndex - 1;
                coarseList.Add(pop.resSASA(calcIndex));
                coarseListFrac.Add(pop.resFract(calcIndex));
                coarseListMax.Add(pop.resMax(calcIndex));
            }

            // Print to rw1
            rw.Write(name);
            rw.Write(',');
            rw.Write(def.FirstDSSPIndex);
            rw.Write(',');
            rw.Write(def.Length);
            rw.Write(',');
            rw.Write(numFormat, anchorSep);
            rw.Write(',');  
            rw.Write(numFormat, sepDiff);
            rw.Write(',');

            double maxNumericWholeLoop = 0.0;
            double sumNumericSASA = 0.0;
            for (int i = 0; i < numericalList.Count; i++)
            {
                sumNumericSASA += numericalList[i];
                maxNumericWholeLoop += numericMax[i];
                rw.Write(numFormat, numericalList[i]);
                rw.Write(',');
            }
            rw.Write(numFormat, sumNumericSASA);
            rw.Write(',');
            rw.Write(numFormat, numericalFract);
            rw.Write(',');

            double maxAAWholeLoop = 0.0;
            double sumAALoopSASA = 0.0;
            for (int i = 0; i < aaList.Count; i++)
            {
                maxAAWholeLoop += aaListMax[i];
                sumAALoopSASA += aaList[i];
                rw.Write(numFormat, aaList[i]);
                rw.Write(',');
            }
            rw.Write(numFormat, sumAALoopSASA);
            rw.Write(',');
            rw.Write(numFormat, maxAAWholeLoop);
            rw.Write(',');
            rw.Write(numFormat, sumAALoopSASA / maxAAWholeLoop);
            rw.Write(',');

            double maxCoarseWholeLoop = 0.0;
            double sumCoarseLoopSASA = 0.0;
            for (int i = 0; i < coarseList.Count; i++)
            {
                maxCoarseWholeLoop += coarseListMax[i];
                sumCoarseLoopSASA += coarseList[i];
                rw.Write(numFormat, coarseList[i]);
                rw.Write(',');
            }
            rw.Write(numFormat, sumCoarseLoopSASA);
            rw.Write(',');
            rw.Write(numFormat, maxCoarseWholeLoop);
            rw.Write(',');
            rw.Write(numFormat, sumCoarseLoopSASA / maxCoarseWholeLoop);

            rw.WriteLine();
            rw.Flush();

            for (int i = 0; i < def.Length; i++)
            {
                rwSASAVersus.Write(name);
                rwSASAVersus.Write(',');
                rwSASAVersus.Write(def.FirstDSSPIndex);
                rwSASAVersus.Write(',');
                rwSASAVersus.Write(def.Length);
                rwSASAVersus.Write(',');
                rwSASAVersus.Write(i);
                rwSASAVersus.Write(',');
                rwSASAVersus.Write(seq[i]);
                rwSASAVersus.Write(',');
                rwSASAVersus.Write(numFormat, anchorSep);
                rwSASAVersus.Write(',');               
                rwSASAVersus.Write(numFormat, sepDiff);
                rwSASAVersus.Write(',');
                rwSASAVersus.Write(numFormat, numericalList[i]);
                rwSASAVersus.Write(',');
                rwSASAVersus.Write(numFormat, numericMax[i]);
                rwSASAVersus.Write(',');
                rwSASAVersus.Write(numFormat, numericFrac[i]);
                rwSASAVersus.Write(',');
                rwSASAVersus.Write(numFormat, aaList[i]);
                rwSASAVersus.Write(',');
                rwSASAVersus.Write(numFormat, aaListMax[i]);
                rwSASAVersus.Write(',');
                rwSASAVersus.Write(numFormat, aaListFrac[i]);
                rwSASAVersus.Write(',');
                rwSASAVersus.Write(numFormat, coarseList[i]);
                rwSASAVersus.Write(',');
                rwSASAVersus.Write(numFormat, coarseListMax[i]);
                rwSASAVersus.Write(',');
                rwSASAVersus.Write(numFormat, coarseListFrac[i]);                
            }
            rwSASAVersus.WriteLine();
            rwSASAVersus.Flush();
         
            //pop.detail();

            //for (int i = 0; i < poly.Atoms.Count; i++)
            //{
            //    double sasa = pop.atomSASA(i);
            //    if (sasa != 0.0)
            //    {
            //        Trace.Write(poly.Atoms[i].ToString());
            //        Trace.WriteLine(": Atom SASA: {0}", sasa);
            //    }
            //}

            //for (int i = 0; i < poly.Count; i++)
            //{
            //    Trace.WriteLine("Res SASA: {0}", pop.resSASA(i));
            //}

            return;
        }
    }
}
