using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using UoB.Core.Structure;
using UoB.Core.Primitives;
using UoB.Core.Tools;
using UoB.Core.FileIO.DSSP;
using UoB.Core.FileIO.PDB;
using UoB.Methodology.DSSPAnalysis;
using UoB.AppLayer.Common;
using UoB.Compression;
using UoB.Methodology.OriginInteraction;

namespace UoB.AppLayer.Common
{

    class ParaSource
    {
        // File it came from
        public int randSeed = -1;
        public int paraNode = -1;
        public int paraTotal = -1;
    }

    class LoopScopeInfo
    {
        public void CopyFrom(LoopScopeInfo clone)
        {
            parentChain = clone.parentChain;
            startResIndex = clone.startResIndex;
            loopLength = clone.loopLength;
        }

        public bool Matches(LoopScopeInfo b)
        {
            return (loopLength == b.loopLength &&
                     startResIndex == b.startResIndex &&
                     (0 == String.Compare(parentChain, b.parentChain)));
        }

        public string parentChain = "";
        public int startResIndex = -1;
        public int loopLength = -1;
    }

    class FullSimFileInfo
    {
        public ParaSource source = new ParaSource();
        public LoopScopeInfo scope = new LoopScopeInfo();

        public FullSimFileInfo(string filename)
        {
            string[] parts = filename.Substring(5, filename.Length - 13).Split('_');

            string[] nodeString = parts[1].Split('-');
            if (nodeString[0][0] != 'n')
            {
                throw new Exception("Filename is invalid!");
            }
            nodeString[0] = nodeString[0].Substring(1, nodeString[0].Length - 1);

            // Set our source info ...
            source.randSeed = int.Parse(parts[0]);
            source.paraNode = int.Parse(nodeString[0]);
            source.paraTotal = int.Parse(nodeString[1]);

            scope.parentChain = parts[2];
            if (parts.Length == 5)
            {
                scope.startResIndex = int.Parse(parts[3]);
                scope.loopLength = int.Parse(parts[4]);
            }
            else if (parts.Length == 6)
            {
                scope.startResIndex = int.Parse(parts[4]);
                scope.loopLength = int.Parse(parts[5]);
            }
            else
            {
                throw new Exception("Filename is invalid!");
            }
        }

        public void PrintScreen()
        {
            Trace.WriteLine(String.Format("Seed: {0}, Node: {1} of {2}, Start: {3}, Length: {4}, Parent: {5}",
               source.randSeed, source.paraNode, source.paraTotal, scope.startResIndex, scope.loopLength, scope.parentChain));
        }

        public void SetFrom(FullSimFileInfo _info)
        {
            source.randSeed = _info.source.randSeed;
            source.paraNode = _info.source.paraNode;
            source.paraTotal = _info.source.paraTotal;

            scope.parentChain = _info.scope.parentChain;
            scope.startResIndex = _info.scope.startResIndex;
            scope.loopLength = _info.scope.loopLength;
        }
    }

    class LoopEntry
    {
        // Loop structure 
        public string fromTra;
        public List<PDBAtom> loopAtoms = new List<PDBAtom>();
        public float energy = float.MaxValue;
        public int traIndex = -1;

        public double allHeavyAtomRMSTo(LoopEntry loop)
        {
            List<PDBAtom> loopAtomsB = loop.loopAtoms;
            if (loopAtoms.Count != loopAtomsB.Count)
            {
                throw new Exception();
            }
            double sum = 0.0;
            for (int i = 0; i < loopAtoms.Count; i++)
            {
                sum += loopAtoms[i].position.distanceSquaredTo(loopAtomsB[i].position);
            }
            sum /= (double)loopAtoms.Count;
            return Math.Sqrt(sum);
        }
    }

    class Cluster
    {
        public List<int> clusterMembers = null;
        public int representative = -1;
    }

    class LoopSet : LoopScopeInfo
    {
        public List<LoopEntry> loopStore = new List<LoopEntry>();
        public List<Cluster> clusterStore = new List<Cluster>();
        public List<List<double>> rms;
        private bool[] usedFlags = null;
        private double cutoff;

        public int FindSingleLowestEnergy()
        {
            int singleLowestEnergy = -1;
            double lowEne = Double.MaxValue;
            for (int i = 0; i < loopStore.Count; i++)
            {
                if (loopStore[i].energy <= lowEne)
                {
                    lowEne = loopStore[i].energy;
                    singleLowestEnergy = i;
                }
            }
            return singleLowestEnergy;
        }

        public int FindSingleLowestEnergy( ref double lowEne )
        {
            int singleLowestEnergy = -1;
            lowEne = Double.MaxValue;
            for (int i = 0; i < loopStore.Count; i++)
            {
                if (loopStore[i].energy <= lowEne)
                {
                    lowEne = loopStore[i].energy;
                    singleLowestEnergy = i;
                }
            }
            return singleLowestEnergy;
        }

        private int ClusteredBestLoop()
        {
            throw new NotImplementedException();
        }

        public int ClusterLoops(double _cutoff)
        {
            clusterStore.Clear();
            usedFlags = new bool[loopStore.Count];
            cutoff = _cutoff;
            GenRMSMatrix();
            Assign();
            Reassign(); // Recluster to make sure every entry is assigned to its closest representative. 
            ResetRepresentate(); // Set the representative member of each cluster to its lowest energy member.
            return ClusteredBestLoop();
        }

        public int LowestAvailableEne()
        {
            int index = -1;
            double lowEne = Double.MaxValue;
            for (int i = 0; i < loopStore.Count; i++)
            {
                if (usedFlags[i] == false && loopStore[i].energy < lowEne)
                {
                    lowEne = loopStore[i].energy;
                    index = i;
                }
            }
            return index;
        }

        private void GenRMSMatrix()
        {
            // Make our jagged array...
            rms = new List<List<double>>(loopStore.Count);
            for (int i = 0; i < loopStore.Count; i++)
            {
                rms[i] = new List<double>(loopStore.Count - i);
                for (int j = i + 1; j < loopStore.Count; j++)
                {
                    rms[i][j] = loopStore[i].allHeavyAtomRMSTo(loopStore[j]);
                }
            }
        }

        private void Assign()
        {
            int index;
            while ((index = LowestAvailableEne()) != -1)
            {
                Cluster c = new Cluster();
                c.representative = index;
                c.clusterMembers.Add(index);
                usedFlags[index] = true;

                // Look for members ...
                for (int i = 0; i < loopStore.Count; i++)
                {
                    if (!usedFlags[i])
                    {
                        double getRMS = (i > index) ? rms[i][index] : rms[index][i];
                        if (getRMS < cutoff)
                        {
                            // This is a cluster member...
                            c.clusterMembers.Add(i);
                            usedFlags[i] = true;
                        }
                    }
                }

                // Add to our global cluster container
                clusterStore.Add(c);
            }
        }

        private void Reassign() // Reassign each LoopEntry to its closest cluster center
        {
            for (int j = 0; j < clusterStore.Count; j++)
            {
                clusterStore[j].clusterMembers.Clear();
                clusterStore[j].clusterMembers.Add(clusterStore[j].representative);
            }

            for (int i = 0; i < loopStore.Count; i++)
            {
                int bestIndex = -1;
                double bestRMS = Double.MaxValue;
                bool isRep = false;
                for (int j = 0; j < clusterStore.Count; j++)
                {
                    if (clusterStore[j].representative == i)
                    {
                        isRep = true;
                        break;
                    }
                    int rep = clusterStore[j].representative;
                    double getRMS = (i > rep) ? rms[i][rep] : rms[rep][i];
                    if (getRMS < bestRMS)
                    {
                        bestRMS = getRMS;
                        bestIndex = j;
                    }
                }
                if (!isRep)
                {
                    clusterStore[bestIndex].clusterMembers.Add(i);
                }
            }
        }

        private void ResetRepresentate() // For each cluster, reset the representative to the member with the lowest energy
        {
            for (int j = 0; j < clusterStore.Count; j++)
            {
                int index = -1;
                double lowEne = Double.MaxValue;
                List<int> members = clusterStore[j].clusterMembers;
                for (int i = 0; i < members.Count; i++)
                {
                    if (loopStore[members[i]].energy < lowEne)
                    {
                        lowEne = loopStore[members[i]].energy;
                        index = members[i];
                    }
                }
                if (clusterStore[j].representative != index)
                {
                    Trace.WriteLine(String.Format("Cluster center has changed from {0} to {1).", clusterStore[j].representative, index));
                    clusterStore[j].representative = index;
                }
            }
        }
    }
}