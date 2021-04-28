using System;
using System.Text;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

using UoB.Core.ForceField;
using UoB.Core.FileIO.DSSP;
using UoB.Core.FileIO.PDB;
using UoB.Methodology.DSSPAnalysis;
using UoB.Core.Structure;
using UoB.Core.Primitives;
using UoB.Core.ForceField.PopsSASA;
using UoB.Core.Structure.Alignment;
using UoB.Core.Primitives.Matrix;

namespace UoB.AppLayer
{
    class Tester
    {
        public static void DoFiles(DirectoryInfo di)
        {
            string dbReplacePath = @"y:\_rerun2\pdb\\";
            string exile = di.FullName + "\\exile\\";
            Directory.CreateDirectory(exile);
            DirectoryInfo diReplace = new DirectoryInfo(dbReplacePath);
            FileInfo[] files = diReplace.GetFiles("*.pdb");
            for (int i = 0; i < files.Length; i++)
            {
                string stem = Path.GetFileNameWithoutExtension(files[i].Name);
                FileInfo[] movers = di.GetFiles(stem + "*");
                for (int j = 0; j < movers.Length; j++)
                {
                    movers[j].MoveTo(exile + movers[j].Name);
                }
            }
        }

        public static void RecursiveScanDir(DirectoryInfo di)
        {
            DirectoryInfo[] dis = di.GetDirectories();
            for (int i = 0; i < dis.Length; i++)
            {
                RecursiveScanDir(dis[i]);
            }
            DoFiles(di);
        }

        private static void setToOptimalAllAtomRot( MatrixRotation rot, PSMolContainer mol1, PSMolContainer mol2 )
        {
            List<Position> a1 = new List<Position>();
            List<Position> a2 = new List<Position>();

            for (int j = 0; j < mol1.Count; j++)
            {
                Molecule aa = mol1[j];
                Molecule aa2 = mol2[j];
                for (int k = 0; k < aa.Count; k++)
                {
                    if (aa[k].atomPrimitive.Element != 'H')
                    {
                        Atom a = aa[k];
                        Atom b = aa2.AtomOfType(a.PDBType);
                        a1.Add( new Position(a) );
                        a2.Add( new Position(b) );
                    }
                }
            }

            int[] equiv = new int[a1.Count];
            for (int i = 0; i < equiv.Length; i++)
            {
                equiv[i] = i;
            }

            rot.setToOptimalRotation(a1.ToArray(), a2.ToArray(), equiv);
        }

        private static void cRMSVersus()
        {
            //string mod = "_d";
            string mod = "";

            string root = @"C:\_share\Common\";
            string pre = root + "pdb" + mod + "\\";
            string post = root + "min" + mod + "\\";

            DirectoryInfo di = new DirectoryInfo(pre);
            FileInfo[] files = di.GetFiles("*.pdb");

            PSAlignManager man = new PSAlignManager(AlignmentMethod.Geometric, 400 );

            List<double> rms = new List<double>();
            List<string> names = new List<string>();

            for (int i = 0; i < files.Length; i++)
            {
                Console.Write('.');

                string namestem = Path.GetFileNameWithoutExtension(files[i].Name);
                string minFile = post + namestem + ".min.pdb";
                if( !File.Exists(minFile) )throw new Exception();

                PDB filePre = new PDB( files[i].FullName, true );
                PDB fileMin = new PDB( minFile, true );

                PolyPeptide pP = filePre.particleSystem.MemberAt(0) as PolyPeptide;
                pP.DoGeometrixCenter();
                PolyPeptide pM = fileMin.particleSystem.MemberAt(0) as PolyPeptide;
                pM.DoGeometrixCenter();

                //man.ResetPSMolContainers("bob", 
                //    new AlignSourceDefinition( filePre.particleSystem, 0 ), 
                //    new AlignSourceDefinition( fileMin.particleSystem, 0 ) );
                //man.PerformAlignment();

                //PDB.SaveNew(after + namestem + ".pdb", man.SystemDefinition.particleSystem);

                MatrixRotation rot = new MatrixRotation();
                setToOptimalAllAtomRot(rot, pP, pM);
                rot.transform(pM.Atoms);

                double sum = 0.0;
                double count = 0.0;
                for (int j = 0; j < pP.Count; j++)
                {
                    Molecule aa = pP[j];
                    Molecule aa2 = pM[j];
                    for (int k = 0; k < aa.Count; k++)
                    {
                        if (aa[k].atomPrimitive.Element != 'H')
                        {
                            Atom a = aa[k];
                            Atom b = aa2.AtomOfType(a.PDBType);
                            double d = a.distanceSquaredTo(b);
                            sum += d;
                            count += 1.0;
                        }
                    }
                }

                rms.Add( Math.Sqrt(sum / count) );
                names.Add(namestem);
            }

            StreamWriter rw = new StreamWriter(root + "res.csv");
            for (int i = 0; i < rms.Count; i++)
            {
                rw.WriteLine(names[i] + "," + rms[i]);
            }
            rw.Close();
        }


        private static Position[] overlayAtoms = null;
        private static int[] overlayEquiv = null;
        private static ParticleSystem TrimSystem(ParticleSystem ps)
        {
            ParticleSystem munge = ps.Clone() as ParticleSystem;
            munge.BeginEditing();

            PolyPeptide poly = munge.MemberAt(0) as PolyPeptide;
            poly.ChainID = 'A';

            List<Atom> anchorAtoms = new List<Atom>();

            for (int i = 0; i < poly.Count; i++)
            {
                AminoAcid aa = poly[i] as AminoAcid;
                for (int j = aa.Count - 1; j >= 0; j--)
                {
                    Atom a = aa[j];
                    if (a.atomPrimitive.Element == 'H')
                    {
                        aa.RemoveAtomAt(j);
                    }
                    else if (i == 2 || i == 5 || i == 18 || i == 23)
                    {
                        if (a.atomPrimitive.IsBackBone ||
                            a.PDBType == " CB "
                            )
                        {
                            anchorAtoms.Add(a);
                        }
                    }
                    //else if (i == 10 || i == 11)
                    //{
                    //    if (a.atomPrimitive.IsBackBone)
                    //    {
                    //        anchorAtoms.Add(a);
                    //    }
                    //    else if (!a.atomPrimitive.IsBackBone)
                    //    {
                    //        aa.RemoveAtomAt(j);
                    //    }
                    //}
                    else
                    {
                        if (!a.atomPrimitive.IsBackBone)
                        {
                            aa.RemoveAtomAt(j);
                        }
                    }
                }
            }
            munge.EndEditing(true, true);

            // Calculate the center of geometry for the anchor atoms only
            Position cog = new Position();
            for (int i = 0; i < anchorAtoms.Count; i++)
            {
                cog.Add(anchorAtoms[i]);
            }
            cog.Divide((double)anchorAtoms.Count);

            // move the polypeptide so that the cog of the anchor atoms lies on the origin
            poly.TranslateAll(cog);

            // Record the anchor atom positions
            List<Position> anchor = new List<Position>();
            for (int i = 0; i < anchorAtoms.Count; i++)
            {
                anchor.Add(new Position(anchorAtoms[i]));
            }

            if (overlayAtoms == null)
            {
                overlayAtoms = anchor.ToArray();
                List<int> _overlayEquiv = new List<int>();
                for( int p = 0; p < overlayAtoms.Length; p++ )
                {
                    _overlayEquiv.Add(p);
                }
                overlayEquiv = _overlayEquiv.ToArray();
            }
            else
            {
                if (anchor.Count != overlayAtoms.Length) throw new Exception();

                // Do superimposition of our system!!
                MatrixRotation rot = new MatrixRotation();
                rot.setToOptimalRotation(overlayAtoms, anchor.ToArray(), overlayEquiv);
                poly.TransformAll(rot);
            }
            
            return munge;
        }

        private static void ZincFingerOverlay()
        {
            string path = @"C:\_work\5 - Active Site Overlay\";
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles("*.ent");

            bool singleModel = false;

            StreamWriter rw = new StreamWriter(path + "out.pdb");

            int model = 1;
            for (int i = 0; i < files.Length; i++)
            {
                PDB file = new PDB(files[i].FullName,true);
                ParticleSystem ps = file.particleSystem;

                string seq = file.SequenceInfo.getStructuralSeq( ps.MemberAt(0).ChainID );
                Console.WriteLine(seq);

                for (int j = 0; j < (singleModel ? 1 : file.PositionDefinitions.Count); j++)
                {
                    file.PositionDefinitions.Position = j;
                    ParticleSystem munge = TrimSystem(file.particleSystem);
                    rw.WriteLine("MODEL {0}",model++);
                    PDB.SaveAtomsTo(rw,munge);
                    rw.WriteLine("ENDMDL");
                }                
            }

            rw.Close();

            return;
        }

        public static void Test()
        {
            ZincFingerOverlay();
            return;

            //cRMSVersus();

            //string scanPath = @"y:\Cloop\ReturnData\";
            //string scanPath = @"y:\Rapper\Result\";
            //string scanPath = @"y:\PLOP\_MainResults\Result\";
            //string scanPath = @"y:\Modeller\CompleteData_8v2\";
            //RecursiveScanDir(new DirectoryInfo(scanPath));
            //return;      

            //SegmentDef def = new SegmentDef(0);
            //for (int i = 0; i < poly.Count; i++)
            ////for (int i = 1; i < poly.Count - 1; i++)
            //{
            //    ResidueDef res = new ResidueDef();
            //    res.FileIndex = i + 1; // DSSP scale
            //    res.AminoAcidID = poly[i].moleculePrimitive.SingleLetterID;
            //    res.InsertionCode = poly[i].InsertionCode;
            //    res.ResidueNumber = poly[i].ResidueNumber;
            //    def.AddResidue(res);
            //}

            //// Numerical SASA analysis
            //UoB.Core.ForceField.LoopAnalyticalSASA calc = new UoB.Core.ForceField.LoopAnalyticalSASA();
            //calc.connectLoop(poly, def);
            //calc.keepAtomics = true;
            //calc.calcLoopSASAInfo();
            //double numericalFract = calc.totalFraction;

            //List<double> numericalList = calc.SASAs;
            //for (int i = 0; i < numericalList.Count; i++)
            //{
            //    Trace.WriteLine("{0}:\t{1:F4}", i, numericalList[i]);
            //}

            //numericalList = calc.atomicSASAs;
            //for (int i = 0; i < numericalList.Count; i++)
            //{
            //    Trace.WriteLine("{0}-{1}: {2:F4}", i, poly.Atoms[i].PDBType, numericalList[i]);
            //}

            //return;
        }
    }
}