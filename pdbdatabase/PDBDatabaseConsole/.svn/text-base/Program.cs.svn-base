using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;

using UoB.PDBDownload.Core;

using UoB.Core.Structure;
using UoB.Core.Structure.Builder;
using UoB.Core.FileIO.PDB;
using UoB.Core.Sequence;
using UoB.Core.ForceField;
using UoB.Core.Structure.Primitives;

namespace UoB.PDBDownload.ConsoleApp
{
    class Program
    {
        static DirectoryInfo EnsureDirectory(string inputLine)
        {
            DirectoryInfo di = new DirectoryInfo(inputLine);
            if (!di.Exists) throw new IOException("Input directory doesnt exist!");
            return di;
        }

        static protected List<Alias> alias_list = new List<Alias>();

        static protected void GetAliases()
        {
            alias_list.Clear();
            StreamReader re = new StreamReader(@"H:\uobframework\trunk\Shared\default.alias");
            string line;
            while (null != (line = re.ReadLine()))
            {
                if (line.Length == 0) continue;
                if (line[0] == '#') continue;
                string[] lineparts = UoB.Core.Tools.CommonTools.WhiteSpaceRegex.Split(line);
                if (lineparts.Length == 0) continue;
                if (0 != String.Compare(lineparts[0], "ALIAS", true)) continue;
                Alias a = new Alias();
                a.alias = lineparts[1];
                a.name = lineparts[2];
                alias_list.Add(a);
            }
            re.Close();
        }

        static protected bool DBNameMungeNowOK(PSMolContainer poly)
        {
            for (int i = 0; i < poly.Count; i++)
            {
                Molecule mol = poly[i];
                MoleculePrimitive prim = mol.moleculePrimitive as MoleculePrimitive;
                if (prim == null)
                {
                    string mungeName = mol.Name_NoPrefix;

                    bool isFixed = false;

                    for (int j = 0; j < alias_list.Count; j++)
                    {
                        if (mungeName == alias_list[j].alias)
                        {
                            mol.ResetName(alias_list[j].name,true);
                            prim = mol.moleculePrimitive as MoleculePrimitive;
                            if (prim == null)
                            {
                                Console.WriteLine("ERROR: Critical Database fail! '" + mungeName + "' in '" + poly.Parent.Name + '\'');
                                return false;
                            }
                            else
                            {
                                Console.WriteLine("Underpanticle RESOLVED! '" + mungeName + "' to '" + alias_list[j].name + "' in '" + poly.Parent.Name + '\'');
                                isFixed = true;
                                break;
                            }
                        }
                    }

                    if (!isFixed)
                    {
                        Console.WriteLine("Totally unknown underpanticle found '" + mungeName + "' in '" + poly.Parent.Name + '\'');
                        return false;
                    }
                }
            }
            return true;
        }

        protected static bool noBreaks(PSMolContainer mol)
        {
            for (int i = 0; i < mol.Count - 1; i++)
            {
                Atom c = mol[i].AtomOfType(" C  ");
                Atom n = mol[i+1].AtomOfType(" N  ");
                if (c == null || n == null)
                {
                    Console.WriteLine("WARNING!! Atoms missing! File:{0}, From mol {1} to mol {2}", 
                        mol.Parent.Name, mol[i].ToString(), mol[i+1].ToString());
                    return false;
                }
                double dist = c.distanceTo(n);
                if (dist > 2.5)
                {
                    Console.WriteLine("WARNING!! Break Found! File:{0}, From mol {1} to mol {2}. Dist: {3}",
                        mol.Parent.Name, mol[i].ToString(), mol[i + 1].ToString(), dist);
                    return false;
                }
            }
            return true;
        }

        public class Alias
        {
            public string alias = null;
            public string name = null;
        }

        static void Main(string[] args)
        {
            try
            {
                FFManager.Instance.FinaliseStage2(); // AAAhggggggg!!!!
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);
                return;
            }

            // Force argument in this version ...
            //args = new string[] { "download" };
            args = new string[] { "obtain_chains" };

            if( args.Length == 0 )
            {
                Console.WriteLine("Please give a program mode!");
                return;
            }

            //string dir = @"..\..\..\lib\pisces\";
            string dir = @"H:\pdbdatabase\lib\pisces\";
            if (!Directory.Exists(dir))
                throw new Exception();

            string filename = dir + @"cullpdb_pc70_res1.8_R0.25_d060604_chains3359";
            if (!File.Exists(filename))
                throw new Exception();

            string execMode = args[0];
            if (0 == String.Compare(execMode, "download", true))
            {
                // -----------------------
                // Downloads
                // -----------------------
                                
                DownloadManager downloads = new DownloadManager();
                downloads.Obtain(filename, 0);
                downloads.BeginDownloadThreads();
            }
            else if (0 == String.Compare(execMode, "obtain_chains", true))
            {
                // Read bitches from file before we start.
                GetAliases();

                string pdbDir = @".\PDB\";
                Directory.CreateDirectory(pdbDir);
                string outDir = @".\chains\";
                Directory.CreateDirectory(outDir);
                string filteredDir = @".\filtered\";
                Directory.CreateDirectory(filteredDir);
                string filteredDir2 = @".\filtered2\";
                Directory.CreateDirectory(filteredDir2);
                string rebuildDir = @".\rebuild\";
                Directory.CreateDirectory(rebuildDir);

                StreamReader re = new StreamReader(filename);
                string line;
                int offsetIndex = 0;
                while (null != (line = re.ReadLine()))
                {
                    if (line.Length < (5 + offsetIndex)) continue;
                    line = line.Substring(offsetIndex, 5);
                    line = line.Trim();
                    if (line.Length != 5) continue;

                    string pdbID = line.Substring(0, 4);
                    char chainID = line[4];

                    if (chainID == '0') chainID = '_';
                    string outFilestem = pdbID + chainID + ".pdb";
                    if (chainID == '_') chainID = ' ';

                    if (!File.Exists(outDir + outFilestem))
                    {
                        string pdbFileName = pdbDir + pdbID + ".pdb";

                        //if (!File.Exists(pdbFileName)) continue; // This is only for debug .. normally we would want a warning
                        if (!File.Exists(pdbFileName)) throw new Exception("Cant find PDB file");

                        PDB file = new PDB(pdbFileName, true);
                        ParticleSystem ps = file.particleSystem;
                        PSMolContainer mol = ps.MemberWithID(chainID);
                        if (mol == null) throw new Exception();

                        // Remove residues that dont contain enough info to rebuild them.. treat them as undefined.
                        if (mol[0].Name_NoPrefix == "ACE")
                        {
                            mol.RemoveMolAt(0);
                            mol[0].ResetName(mol[0].Name_NoPrefix, 'N', true);
                        }
                        for (int kk = mol.Count - 1; kk >= 0; kk--)
                        {
                            if (mol[kk].Count < 3)
                            {
                                mol.RemoveMolAt(kk);
                            }
                        }
                        if (DBNameMungeNowOK(mol))
                        {
                            ParticleSystem psNew = new ParticleSystem(pdbID + chainID);
                            psNew.BeginEditing();
                            psNew.AddMolContainer((PSMolContainer)mol.Clone());
                            psNew.DetectDisulphides();
                            psNew.EndEditing(true, true);

                            PDB.SaveNew(outDir + outFilestem, psNew, true, file.ExtendedInformation);

                            PS_Builder build = new PS_Builder(psNew);
                            build.RebuildTemplate(RebuildMode.AllAtoms, false, false, true, true, true);

                            PDB.SaveNew(rebuildDir + outFilestem, psNew, true, file.ExtendedInformation);

                            // Get the rebuilt mol back
                            mol = psNew.MemberAt(0);
                            if (mol == null) throw new Exception();

                            if (mol.Count <= 350)
                            {
                                PDB.SaveNew(filteredDir + outFilestem, psNew, true, file.ExtendedInformation);
                                if (noBreaks(mol))
                                {
                                    PDB.SaveNew(filteredDir2 + outFilestem, psNew, true, file.ExtendedInformation);
                                }
                            }
                        }
                        else
                        {
                            // We have a problem in that there is no ff-def for this residue
                            string mungeDir = @".\mungefail\";
                            Directory.CreateDirectory(mungeDir);
                            File.Copy(pdbDir + pdbID + ".pdb", mungeDir + pdbID + ".pdb", true);
                        }
                    }
                }

                re.Close();
            }
            else if (0 == String.Compare(execMode, "download_single", true))
            {
                string url = args[1];
                string savePath = args[2];
                DownloadDefinition d = new DownloadDefinition(url, savePath);
                d.Download();
            }
            else
            {
                Console.WriteLine("Not a valid mode: '" + args[0] + "'");
            }
        }
    }
}
