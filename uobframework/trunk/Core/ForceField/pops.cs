using System;
using System.IO;
using System.Collections.Generic;
using UoB.Core.Primitives;
using UoB.Core.Structure;

namespace UoB.Core.ForceField.PopsSASA
{
    class PopsAtomBase
    {
        public PopsAtomBase()
        {
            radius = 0.0;
            param = 0.0;
            hydrophilic = false;
        }

        public double radius;
        public double param;
        public bool hydrophilic;
    }

    class PopsAtomType : PopsAtomBase
    {
        public PopsAtomType()
        {
        }

        public void parse(string _Line)
        {
            string[] parts = UoB.Core.Tools.CommonTools.WhiteSpaceRegex.Split(_Line);
            if (parts.Length != 6) throw new ParseException("POPS: Atom set initial line contains the wrong number of elements");
            if (0 != parts[0].CompareTo("ATOM")) throw new Exception("Only atom lines should ever be fed in here!");

            atomName = parts[1];
            resName = parts[2];

            int endCol = int.Parse(parts[5]);
            radius = Double.Parse(parts[3]);
            param = Double.Parse(parts[4]);
            hydrophilic = (endCol != 0);

            return;
        }

        public string atomName;
        public string resName;
    }

    class PopsAtom : PopsAtomBase
    {
        public PopsAtom()
        {
            sasa = 0.0;
            parentIndex = -1;
            NOverlap = 0;
            pos = null;
            fractMax = 0.0;
            max = 0.0;
        }

        public PopsAtom(PopsAtomType cloneLib, int _ParentIndex, Position _pos)
        {
            radius = cloneLib.radius;
            param = cloneLib.param;
            hydrophilic = cloneLib.hydrophilic;
            parentIndex = _ParentIndex;
            pos = _pos;
            fractMax = 0.0;
            max = 0.0;
        }

        public int parentIndex;
        public int NOverlap;
        public double sasa;
        public double fractMax;
        public double max;
        public Position pos;
    };

    class PopsDat
    {
        public PopsDat()
        {
            expectedAtomTypeCount = 0;
            b12 = 0.0;
            b13 = 0.0;
            b14 = 0.0;
            bOther = 0.0;
        }

        public void parse(string _Line)
        {
            string[] parts = UoB.Core.Tools.CommonTools.WhiteSpaceRegex.Split(_Line);
            if (parts.Length != 5) throw new ParseException("POPS: Atom set initial line contains the wrong number of elements");

            expectedAtomTypeCount = int.Parse(parts[0]);
            b12 = Double.Parse(parts[1]);
            b13 = Double.Parse(parts[2]);
            b14 = Double.Parse(parts[3]);
            bOther = Double.Parse(parts[4]);

            return;
        }

        public int expectedAtomTypeCount;
        public List<PopsAtomType> atoms = new List<PopsAtomType>();
        public double b12;
        public double b13;
        public double b14;
        public double bOther;
    };

    public class OverallSASA
    {
        public OverallSASA()
        {
            SASA = 0.0;
            hydrophilicSASA = 0.0;
            hydrophobicSASA = 0.0;
        }

        public double SASA;
        public double hydrophilicSASA;
        public double hydrophobicSASA;
    };

    public enum PopsMode
    {
        Coarse,
        AllAtom
    };

    public class Pops
    {
        public Pops()
        {
            m_Poly = null;
            m_CurrentDat = null;
            m_Mode = PopsMode.AllAtom;
            ProbeRadius = 1.4;
            m_DataRead = false;
            twoProbe = 0.0;
        }

        private void obtainLine(StreamReader _stream, ref string line)
        {
            // Pump lines out of the file, discarding those which are comments and empty
            while (true)
            {
                line = _stream.ReadLine();
                if (line == null) throw new ParseException("POPS: Unexpected 'stream error' whilst parsing data");
                line = line.Trim();
                if (line.Length == 0 || line[0] == '#')
                {
                    continue; // ignore blank lines and comments
                }
                break;
            }
        }

        // Setup
        public void readDat(string _DatFileName)
        {
            StreamReader re = new StreamReader(_DatFileName);

            string line = null; // Temporary line container

            // Find file data begining, assert that this is a POPS dat file
            while (true)
            {
                obtainLine(re, ref line);
                if (0 == line.CompareTo("BEGIN POPS"))
                {
                    break;
                }
                throw new ParseException("POPS: POPS dat format is not correct");
            }

            PopsDat dat = null;
            // Extract our data
            while (true)
            {
                obtainLine(re, ref line);
                if (0 == String.Compare(line, 0, "ATOM", 0, 4))
                {
                    if (dat == null)
                    {
                        throw new ParseException("POPS: Atoms are being defined without a corresponding BEGIN statment.");
                    }
                    PopsAtomType a = new PopsAtomType();
                    a.parse(line);
                    dat.atoms.Add(a);
                }
                else if (0 == line.CompareTo("BEGIN ATOMIC"))
                {
                    // We are defining an atomic resolution set
                    // clear all existing data
                    m_Atomic = new PopsDat();
                    dat = m_Atomic;
                    obtainLine(re, ref line);
                    dat.parse(line);
                }
                else if (0 == line.CompareTo("BEGIN COARSE"))
                {
                    // We are defining a coarsse resolution set
                    // clear all existing data
                    m_Coarse = new PopsDat();
                    dat = m_Coarse;
                    obtainLine(re, ref line);
                    dat.parse(line);
                }
                else if (0 == line.CompareTo("END"))
                {
                    if (dat == null)
                    {
                        // This is the file's concluding END statement
                        break;
                    }
                    else
                    {
                        // We are ending a BEGIN xxx statement
                        if (dat.expectedAtomTypeCount != dat.atoms.Count)
                        {
                            throw new ParseException("POPS: The incorrect number of atom have been imported.");
                        }
                        dat = null;
                    }
                }
                else
                {
                    Console.WriteLine("POPS: Parse WARNING: Uninterpreted line '{0}'", line);
                }
            }
            m_DataRead = true;

            re.Close();
        }

        public void setTo(PolyPeptide poly, PopsMode _Mode)
        {
            m_Poly = poly;
            m_Mode = _Mode;

            m_AtomIndexes.Clear();
            m_Atoms.Clear();

            switch (m_Mode)
            {
                case PopsMode.AllAtom:
                    {
                        m_CurrentDat = m_Atomic;
                        if (m_Atomic.atoms.Count == 0)
                        {
                            throw new ProcedureException("POPS: No data has yet been loaded for AllAtom mode.");
                        }
                        for (int i = 0; i < poly.Atoms.Count; i++)
                        {
                            if (poly.Atoms[i].atomPrimitive.Element == 'H')
                            {
                                m_AtomIndexes.Add(-1);
                                continue;
                            }
                            else
                            {
                                m_AtomIndexes.Add(m_Atoms.Count);
                            }

                            bool found = false;
                            for (int j = 0; j < m_Atomic.atoms.Count; j++)
                            {
                                if (0 == m_Atomic.atoms[j].atomName.CompareTo(poly.Atoms[i].PDBType.Trim()) &&
                                    0 == m_Atomic.atoms[j].resName.CompareTo(poly.Atoms[i].parentMolecule.Name_NoPrefix))
                                {
                                    m_Atoms.Add(new PopsAtom(m_Atomic.atoms[j], i, poly.Atoms[i]));
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                for (int j = 0; j < m_Atomic.atoms.Count; j++)
                                {
                                    if (0 == m_Atomic.atoms[j].resName.CompareTo("*") &&
                                        0 == m_Atomic.atoms[j].atomName.CompareTo(poly.Atoms[i].PDBType.Trim()))
                                    {
                                        m_Atoms.Add(new PopsAtom(m_Atomic.atoms[j], i, poly.Atoms[i]));
                                        found = true;
                                        break;
                                    }
                                }
                            }
                            if (!found)
                            {
                                String error = String.Format("POPS: Cannot find atom type {0} {1} in the database."
                                    , poly.Atoms[i].PDBType, poly.Atoms[i].parentMolecule.Name_NoPrefix);
                                throw new ProcedureException(error);
                            }
                        }
                        break;
                    }
                case PopsMode.Coarse:
                    {
                        m_CurrentDat = m_Coarse;
                        if (m_Coarse.atoms.Count == 0)
                        {
                            throw new ProcedureException("POPS: No data has yet been loaded for AllAtom mode.");
                        }
                        for (int i = 0; i < poly.Atoms.Count; i++)
                        {
                            if (0 != String.Compare(poly.Atoms[i].PDBType, " CA "))
                            {
                                m_AtomIndexes.Add(-1);
                                continue;
                            }
                            else
                            {
                                m_AtomIndexes.Add(m_Atoms.Count);
                            }

                            bool found = false;
                            for (int j = 0; j < m_Coarse.atoms.Count; j++)
                            {
                                if (0 == m_Coarse.atoms[j].atomName.CompareTo(poly.Atoms[i].PDBType.Trim()) &&
                                    0 == m_Coarse.atoms[j].resName.CompareTo(poly.Atoms[i].parentMolecule.Name_NoPrefix))
                                {
                                    m_Atoms.Add(new PopsAtom(m_Coarse.atoms[j], i, poly.Atoms[i]));
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                for (int j = 0; j < m_Coarse.atoms.Count; j++)
                                {
                                    if (0 == m_Coarse.atoms[j].resName.CompareTo("*") &&
                                        0 == m_Coarse.atoms[j].atomName.CompareTo(poly.Atoms[i].PDBType.Trim()))
                                    {
                                        m_Atoms.Add(new PopsAtom(m_Coarse.atoms[j], i, poly.Atoms[i]));
                                        found = true;
                                        break;
                                    }
                                }
                            }
                            if (!found)
                            {
                                String error = String.Format("POPS: Cannot find atom type {0} {1} in the database.",
                                    poly.Atoms[i].PDBType,
                                    poly.Atoms[i].parentMolecule.Name_NoPrefix);
                                throw new ProcedureException(error);
                            }
                        }
                        break;
                    }
                default:
                    {
                        throw new Exception("Unknown PopsMode encountered!");
                    }
            }
        }

        // Info
        public void detail()
        {
            CoreAsserts();
            info();
            Console.WriteLine("SASA Particle List:");
            for (int i = 0; i < m_Atoms.Count; i++)
            {
                m_Poly.Atoms[m_Atoms[i].parentIndex].ToString();
                Console.WriteLine(" Radius: {0:N3} SASA: {1:N3} NOverlap {2}",
                    m_Atoms[i].radius, m_Atoms[i].sasa, m_Atoms[i].NOverlap);
            }
        }

        public void info()
        {
            CoreAsserts();
            OverallSASA sasa = getSASA();

            switch (m_Mode)
            {
                case PopsMode.AllAtom:
                    {
                        Console.WriteLine("Pops All-Atom Info:");
                        break;
                    }
                case PopsMode.Coarse:
                    {
                        Console.WriteLine("Pops Coarse Info:");
                        break;
                    }
                default:
                    {
                        throw new Exception("Unknown PopsMode encountered!");
                    }
            }

            Console.WriteLine("\tSASA atoms: {0}", m_Atoms.Count);
            Console.WriteLine("\tProbe Radius: {0:N2}", ProbeRadius);
            Console.WriteLine("\tTotal SASA: {0:N3}", sasa.SASA);
            Console.WriteLine("\tHydrophobic SASA: {0:N3}", sasa.hydrophobicSASA);
            Console.WriteLine("\tHydrophilic SASA: {0:N3}", sasa.hydrophilicSASA);
        }

        /// Calculate the SASA
        public void calc()
        {
            CoreAsserts();

            // Reset
            m_SASAInfo = new OverallSASA();
            twoProbe = 2.0 * ProbeRadius; // double the probe radius

            // Calculate all atomic SASAs
            for (int i = 0; i < m_Atoms.Count; i++)
            {
                calcCore(i);
                if (m_Atoms[i].hydrophilic)
                {
                    m_SASAInfo.hydrophilicSASA += m_Atoms[i].sasa;
                }
                else
                {
                    m_SASAInfo.hydrophobicSASA += m_Atoms[i].sasa;
                }
                m_SASAInfo.SASA += m_Atoms[i].sasa;
            }
        }

        // Query
        public OverallSASA getSASA()
        {
            CoreAsserts();
            return m_SASAInfo;
        }

        public double atomSASA(int ia)
        {
            CoreAsserts();
            if (!(ia >= 0 && ia < m_Poly.Atoms.Count)) throw new Exception("Atom request is out of range");
            int index = m_AtomIndexes[ia];
            return index == -1 ? 0.0 : m_Atoms[index].sasa;
        }

        public double atomFract(int ia)
        {
            CoreAsserts();
            if (!(ia >= 0 && ia < m_Poly.Atoms.Count)) throw new Exception("Atom request is out of range");
            int index = m_AtomIndexes[ia];
            return index == -1 ? 0.0 : m_Atoms[index].fractMax;
        }

        public double resFract(int ir)
        {
            CoreAsserts();
            if (!(ir >= 0 && ir < m_Poly.Count)) throw new Exception("Atom request is out of range");
            switch (m_Mode)
            {
                case PopsMode.AllAtom:
                    {
                        double sum = 0.0f;
                        double sumMax = 0.0f;
                        AminoAcid aa = m_Poly[ir];
                        for (int i = 0; i < aa.Count; i++)
                        {
                            int lookup = m_AtomIndexes[m_Poly.Atoms.IndexOf(aa[i])];
                            if (lookup != -1)
                            {
                                double sasa = m_Atoms[lookup].sasa;
                                if (sasa == -1.0) throw new CodeException("Assumption is not true");
                                sum += sasa;
                                sumMax += m_Atoms[lookup].max;
                            }
                        }
                        return sum / sumMax;
                    }
                case PopsMode.Coarse:
                    {
                        int CAIndex = m_Poly.Atoms.IndexOf(m_Poly[ir].CAlphaAtom);
                        int index = m_AtomIndexes[CAIndex];
                        if (m_AtomIndexes[CAIndex] == -1) throw new CodeException("Assumption is not true");
                        return m_Atoms[index].fractMax;
                    }
                default:
                    {
                        throw new Exception("Unknown PopsMode encountered!");
                    }
            }
        }

        public double resMax(int ir)
        {
            CoreAsserts();
            if (!(ir >= 0 && ir < m_Poly.Count)) throw new Exception("Atom request is out of range");
            switch (m_Mode)
            {
                case PopsMode.AllAtom:
                    {
                        double sumMax = 0.0f;
                        AminoAcid aa = m_Poly[ir];
                        for (int i = 0; i < aa.Count; i++)
                        {
                            int lookup = m_AtomIndexes[m_Poly.Atoms.IndexOf(aa[i])];
                            if (lookup != -1)
                            {
                                double sasa = m_Atoms[lookup].sasa;
                                if (sasa == -1.0) throw new CodeException("Assumption is not true");
                                sumMax += m_Atoms[lookup].max;
                            }
                        }
                        return sumMax;
                    }
                case PopsMode.Coarse:
                    {
                        int CAIndex = m_Poly.Atoms.IndexOf(m_Poly[ir].CAlphaAtom);
                        int index = m_AtomIndexes[CAIndex];
                        if (m_AtomIndexes[CAIndex] == -1) throw new CodeException("Assumption is not true");
                        return m_Atoms[index].max;
                    }
                default:
                    {
                        throw new Exception("Unknown PopsMode encountered!");
                    }
            }
        }

        public double resSASA(int ir)
        {
            CoreAsserts();
            if (!(ir >= 0 && ir < m_Poly.Count)) throw new Exception("Atom request is out of range");
            switch (m_Mode)
            {
                case PopsMode.AllAtom:
                    {
                        double sum = 0.0f;
                        AminoAcid aa = m_Poly[ir];
                        for (int i = 0; i < aa.Count; i++)
                        {
                            int lookup = m_AtomIndexes[ m_Poly.Atoms.IndexOf(aa[i]) ];
                            if (lookup != -1)
                            {
                                double sasa = m_Atoms[lookup].sasa;
                                if (sasa == -1.0) throw new CodeException("Assumption is not true");
                                sum += sasa;
                            }
                        }
                        return sum;
                    }
                case PopsMode.Coarse:
                    {
                        int CAIndex = m_Poly.Atoms.IndexOf(m_Poly[ir].CAlphaAtom);
                        int index = m_AtomIndexes[CAIndex];
                        if (m_AtomIndexes[CAIndex] == -1) throw new CodeException("Assumption is not true");
                        return m_Atoms[index].sasa;
                    }
                default:
                    {
                        throw new Exception("Unknown PopsMode encountered!");
                    }
            }
        }

        // Public settings
        double ProbeRadius;

        // Private functions
        void CoreAsserts()
        {
            if (!m_DataRead) throw new CodeException("POPS::readDat() has not been called");
            if (m_Poly == null) throw new CodeException("POPS::setTo() has not been called");
            if (m_Poly.Atoms.Count != m_AtomIndexes.Count) throw new CodeException("Internal Code Error");
        }

        int getBondOrder(int q, int r)
        {
            if (q == r) return 0;
            if (m_Mode == PopsMode.Coarse)
            {
                int iri = m_Poly.IndexOf(m_Poly.Atoms[q].parentMolecule);
                int irj = m_Poly.IndexOf(m_Poly.Atoms[r].parentMolecule);
                return System.Math.Abs(iri - irj);
            }
            else
            {
                Atom ia = m_Poly.Atoms[q];
                Atom ix = m_Poly.Atoms[r];

                if (ia.bondedTo(ix)) return 1;

                for (int i = 0; i < ia.bondCount; i++)
                {
                    Atom ib = ia.BondedAtoms[i];
                    if (ib.bondedTo(ix))
                    {
                        return 2;
                    }
                    else
                    {
                        for (int j = 0; j < ib.bondCount; j++)
                        {
                            if (ib.BondedAtoms[j].bondedTo(ix))
                            {
                                return 3;
                            }
                        }
                    }
                }

                return 100;
            }
        }

        static readonly double FourPI = System.Math.PI * 4.0;

        void calcCore(int i)
        {
            // Atom i
            PopsAtom si = m_Atoms[i];
            int qi = si.parentIndex;
            double Ri = si.radius;
            //const Maths::dvector& posi = m_Poly.atomxyz(qi);

            // calc Si for this atom
            double Si = si.radius + ProbeRadius;
            Si *= Si;
            Si *= FourPI;
            si.max = Si;
            double invSi = 1.0 / Si;

            // Now lets calc the sasa
            si.NOverlap = 0;
            si.sasa = Si; // Initialise to the max possible SASA

            // Loop over other atoms in the list
            int to = m_Atoms.Count;
            for (int j = 0; j < to; j++)
            {
                if (i == j) continue;

                PopsAtom sj = m_Atoms[j];
                int qj = sj.parentIndex;
                double Rj = sj.radius;

                //double rij = m_Poly.atomxyz(qj).dist( posi );
                double rij = si.pos.distanceTo(sj.pos);
                double cutoff = Rj + Ri + twoProbe;
                if (rij < cutoff)
                {
                    si.NOverlap++; // We have overlapping spheres

                    // Get the paramter pij dependent on the bond order
                    double pij;
                    int orderij = getBondOrder(qi, qj);

                    if (orderij == 1)
                    {
                        pij = m_CurrentDat.b12;
                    }
                    else if (orderij == 2)
                    {
                        pij = m_CurrentDat.b13;
                    }
                    else if (orderij == 3)
                    {
                        pij = m_CurrentDat.b14;
                    }
                    else
                    {
                        pij = m_CurrentDat.bOther;
                    }

                    double bij = System.Math.PI *
                        (Ri + ProbeRadius) *
                        (Ri + Rj + twoProbe - rij) *
                        (1 + ((Rj - Ri) * (1.0 / rij)));
                    si.sasa *= 1.0 - (si.param * pij * bij * invSi);

                    si.fractMax = si.sasa / Si;
                }
            }
        }

        // Internal flags
        bool m_DataRead;
        PopsMode m_Mode;
        double twoProbe;

        // Member data and proxies
        PolyPeptide m_Poly = null;
        OverallSASA m_SASAInfo = new OverallSASA();
        PopsDat m_CurrentDat = null;
        PopsDat m_Coarse = new PopsDat();
        PopsDat m_Atomic = new PopsDat();
        List<int> m_AtomIndexes = new List<int>();
        List<PopsAtom> m_Atoms = new List<PopsAtom>();
    }
}