using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using UoB.Core.Primitives;
using UoB.Core.Structure;
using UoB.Core.FileIO.PDB;
using UoB.Core.Primitives.Matrix;

namespace OverLord
{
    enum RMSMode
    {
        CA,
        AllHeavy
    }

    class OverlayEngine
    {
        public OverlayEngine(ParticleSystem ps, RMSMode mode, string toFile)
        {
            caAtoms.Add(" CA ");
            m_Mode = mode;
            m_PS = ps;
            m_Mol1 = m_PS.MemberAt(0) as PSMolContainer;
            geoCentre(m_Mol1);
            if (toFile != null)
            {
                PDB.SaveNew(toFile, ps);
            }
        }

        List<string> caAtoms = new List<string>();

        private void geoCentre(PSMolContainer mol)
        {
            switch (m_Mode)
            {
                case RMSMode.AllHeavy:
                    mol.DoGeometrixCenter();
                    break;
                case RMSMode.CA:
                    mol.DoGeometrixCenter(caAtoms);
                    break;
                default:
                    throw new Exception();
            }
        }

        public double next(ParticleSystem ps, string toFile )
        {            
            PSMolContainer mol2 = ps.MemberAt(0) as PSMolContainer;

            geoCentre(mol2);

            MatrixRotation rot = new MatrixRotation();
            setToOptimalAllAtomRot(rot, m_Mol1, mol2);
            rot.transform(mol2.Atoms);
            double rms = getRMS(m_Mol1, mol2);
            if (toFile != null)
            {
                PDB.SaveNew(toFile, ps);
            }
            return rms;
        }

        private double getRMS(PSMolContainer pP, PSMolContainer pM)
        {
            double sum = 0.0;
            double count = 0.0;
            for (int j = 0; j < pP.Count; j++)
            {
                Molecule aa = pP[j];
                Molecule aa2 = pM[j];
                for (int k = 0; k < aa.Count; k++)
                {
                    switch (m_Mode)
                    {
                        case RMSMode.AllHeavy:
                            if (aa[k].atomPrimitive.Element != 'H')
                            {
                                Atom a = aa[k];
                                Atom b = aa2.AtomOfType(a.PDBType);
                                double d = a.distanceSquaredTo(b);
                                sum += d;
                                count += 1.0;
                            }
                            break;
                        case RMSMode.CA:
                            if (aa[k].atomPrimitive.PDBIdentifier == " CA ")
                            {
                                Atom a = aa[k];
                                Atom b = aa2.AtomOfType(a.PDBType);
                                double d = a.distanceSquaredTo(b);
                                sum += d;
                                count += 1.0;
                            }
                            break;
                        default:
                            throw new Exception();
                    }
                }
            }

            return Math.Sqrt(sum / count);
        }

        private void setToOptimalAllAtomRot(MatrixRotation rot, PSMolContainer mol1, PSMolContainer mol2)
        {
            List<Position> a1 = new List<Position>();
            List<Position> a2 = new List<Position>();

            for (int j = 0; j < mol1.Count; j++)
            {
                Molecule aa = mol1[j];
                Molecule aa2 = mol2[j];
                for (int k = 0; k < aa.Count; k++)
                {
                    switch (m_Mode)
                    {
                        case RMSMode.AllHeavy:
                            if (aa[k].atomPrimitive.Element != 'H')
                            {
                                Atom a = aa[k];
                                Atom b = aa2.AtomOfType(a.PDBType);
                                a1.Add(new Position(a));
                                a2.Add(new Position(b));
                            }
                            break;
                        case RMSMode.CA:
                            if (aa[k].atomPrimitive.PDBIdentifier == " CA ")
                            {
                                Atom a = aa[k];
                                Atom b = aa2.AtomOfType(a.PDBType);
                                a1.Add(new Position(a));
                                a2.Add(new Position(b));
                            }
                            break;
                        default:
                            break;
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

        RMSMode m_Mode;
        private List<double> m_RMS;
        private ParticleSystem m_PS;
        private PSMolContainer m_Mol1;
    }

    class Program
    {
        static int Main(string[] args)
        {
            // Grumble ...
            UoB.Core.ForceField.FFManager.Instance.FinaliseStage2();

            if (args.Length < 3)
            {
                Console.WriteLine("Usage: 'overlord mode templateFile matchPattern'");
                Console.WriteLine("mode = CA or (more to come)");
                Console.WriteLine("TemplateFile: The name of the structural template PDB filename");
                Console.WriteLine("matchPattern: A normal file descriptor like *.pdb");
                return 0;
            }

            FileInfo file = new FileInfo(args[0]);
            if (!file.Exists)
            {
                Console.WriteLine("Template file does not exist: " + args[0]);
                return -1;
            }

            RMSMode mode;
            try
            {
                mode = (RMSMode)Enum.Parse(typeof(RMSMode), args[1]);
            }
            catch
            {
                Console.WriteLine("Failed to parse mode: " + args[1]);
                return -1;
            }

            FileInfo fi = new FileInfo( Assembly.GetEntryAssembly().Location );
			DirectoryInfo di = fi.Directory; // the directory where the executing file is located
            DirectoryInfo outDir = di.CreateSubdirectory("output");

            FileInfo[] files = di.GetFiles(args[2]);
            if (files.Length == 0 || (files.Length == 1 && files[0].FullName == file.FullName))
            {
                Console.WriteLine("No files matching pattern: " + args[2]);
                return -1;
            }

            PDB template = new PDB(file.FullName, true);
            OverlayEngine overlord = new OverlayEngine(template.particleSystem, mode, outDir.FullName + Path.DirectorySeparatorChar + file.Name);

            StreamWriter rw = new StreamWriter(outDir.FullName + Path.DirectorySeparatorChar + "_RMS.csv");
            for( int i = 0; i < files.Length; i++ )
            {
                try
                {
                    if (files[i].FullName != file.FullName)
                    {
                        PDB overlayFile = new PDB(files[i].FullName, true);
                        if (overlayFile.PositionDefinitions.Count > 1)
                            Console.WriteLine("Warning, {0} contains multiple models, 1st one used", files[i].Name);
                        double rms = overlord.next(overlayFile.particleSystem, outDir.FullName + Path.DirectorySeparatorChar + files[i].Name);
                        String s = String.Format("{0},{1:F3}", files[i].Name, rms);
                        Console.WriteLine(s);
                        rw.WriteLine(s);
                    }
                }
                catch
                {
                    Console.WriteLine("ERROR!!! during: " + files[i].FullName);
                }
            }
            rw.Close();

            return 0;
        }
    }
}
