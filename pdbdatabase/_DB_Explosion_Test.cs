            // do some pre-amble ...
            FFManager fman = FFManager.Instance;
            fman.FinaliseStage2(); // needed hack ...
            
            float total = 0.0f;
            float duff = 0.0f;
            bool posExplosion = false;
            bool dodgyPDB = false;
            List<string> duffs = new List<string>();
            DirectoryInfo di = new DirectoryInfo(@"F:\Petra\pdb\");
            FileInfo[] files = di.GetFiles("*.pdb");
            for (int kk = 0; kk < files.Length; kk++)
            {
                PDB file = new PDB(files[kk].FullName, true);
                ParticleSystem ps = file.particleSystem;
                PolyPeptide poly = ps.MemberAt(0) as PolyPeptide;

                bool duffBond = false;
                for (int j = 0; j < poly.Atoms.Count; j++)
                {
                    Atom a = poly.Atoms[j];

                    for (int i = 0; i < a.Bonds.Count; i++)
                    {
                        Bond b = a.Bonds[i];
                        if (b.length > 3.0)
                        {
                            duffBond = true;
                            duffs.Add(files[kk].Name);
                            goto OUTER;
                        }
                    }
                }

                PDB fileTempl = new PDB(@"F:\Mine\pdb\" + files[kk].Name.Substring(0, 5) + ".pdb", true);
                ParticleSystem psTempl = fileTempl.particleSystem;
                PolyPeptide polyTempl = psTempl.MemberAt(0) as PolyPeptide;

                dodgyPDB = false;
                if (poly.Count != polyTempl.Count)
                {
                    dodgyPDB = true;
                    string dbRoot = @"F:\_rerun2\_DBEdit\rebuild\";
                    string dbName = files[kk].Name.Substring(0, 5);
                    string dbNameStem = dbRoot + dbName;
                    if (File.Exists(dbNameStem + ".pdb"))
                    {
                        File.Copy(dbNameStem + ".pdb", @"F:\_rerun2\pdb\" + dbName + ".pdb");
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("BEARD!!! " + files[kk].Name);
                        Console.WriteLine();
                        throw new Exception();
                    }
                    goto OUTER;
                }

                posExplosion = false;
                for( int jj = 0; jj < polyTempl.Count; jj++ )
                {
                    Atom a = polyTempl[jj].CAlphaAtom;
                    Atom b = poly[jj].CAlphaAtom;
                    double distSq = a.distanceSquaredTo(b);
                    if (distSq  > 3.0)
                    {
                        posExplosion = true;
                        duffs.Add(files[kk].Name);
                        goto OUTER;
                    }
                }

            OUTER:
                if (duffBond)
                {
                    duff += 1.0f;
                    Console.Write('x');
                }
                else if (dodgyPDB)
                {
                    duff += 1.0f;
                    Console.Write('q');
                }
                else if (posExplosion)
                {
                    duff += 1.0f;
                    Console.Write('y');
                }
                else
                {
                    Console.Write('.');
                }
                total += 1.0f;
                continue;
            }

            for (int t = 0; t < duffs.Count; t++)
            {
                Console.WriteLine(duffs[t]);
                File.Copy(@"F:\Mine\pdb\" + duffs[t].Substring(0, 5) + ".pdb", @"F:\_rerun2\pdb\" + duffs[t].Substring(0, 5) + ".pdb");
            }

            di = new DirectoryInfo(@"F:\_Rerun2\pdb\");
            files = di.GetFiles("*.pdb");
            for (int i = 0; i < files.Length; i++)
            {
                String stem = files[i].Name.Substring(0, 5);
                File.Copy(@"F:\Mine\dssp\" + stem + ".pdb.dssp", @"F:\_rerun2\dssp\" + stem + ".pdb.dssp");
            }

            return;