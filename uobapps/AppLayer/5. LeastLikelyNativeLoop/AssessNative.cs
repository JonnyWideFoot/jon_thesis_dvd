using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

using UoB.Core.FileIO.DSSP;
using UoB.Core.MoveSets.AngleSets;
using UoB.Methodology.DSSPAnalysis;

namespace UoB.AppLayer.LeastLikelyNativeLoop
{
    public class LoopEntryComparer : IComparer<LoopEntry>
    {
        public LoopEntryComparer()
        {
        }

        int IComparer<LoopEntry>.Compare(LoopEntry x, LoopEntry y)
        {
            if(x.Length > y.Length) return 1;
            if(x.Length < y.Length) return -1;
            // else same length
            if (x.Probability > y.Probability) return 1;
            if (x.Probability < y.Probability) return -1;
            return 0;
        }
    }

    public class LoopEntry
    {
        private const double ANG_DIST_CUTOFF = 60.0f;

        private int m_Length;
        private double m_Propensity;
        private double m_MaxPropensity;
        private double m_ProbabilityFactor;

        public int Length
        {
            get
            {
                return m_Length;
            }
        }

        public double Probability
        {
            get
            {
                return m_ProbabilityFactor;
            }
        }

        public LoopEntry()
        {
        }

        public bool Calculate(SegmentDef seg, AngleSet angSet)
        {
            m_Length = seg.Length;
            if( !CalculateMaxProbability(seg, angSet) ) return false;
            if( !CalculateProbability(seg, angSet) ) return false;
            if( !CalculateListPlace(seg, angSet) ) return false;
            m_ProbabilityFactor = (m_Propensity / m_MaxPropensity) * 100.0;
            return true;
        }

        private bool CalculateMaxProbability(SegmentDef seg, AngleSet angSet)
        {
            m_MaxPropensity = 1.0f;
            for (int i = 0; i < seg.Length; i++)
            {
                float max = float.MinValue;
                float[] props = angSet.GetPropensities(seg[i].AminoAcidID);
                for (int j = 0; j < props.Length; j++)
                {
                    if (max < props[j]) max = props[j];
                }
                m_MaxPropensity *= max;
            }
            return true;
        }

        private bool CalculateProbability(SegmentDef seg, AngleSet angSet)
        {
            // look to see what the loop angles are represented adequatly by the angleset.
            // If they are within cutoff, then the loop is valid and its propensity should be assessed.
            m_Propensity = 1.0f;
            int ID = 0;
            double dist = 0.0f;
            for (int i = 0; i < seg.Length; i++)
            {
                if (seg[i].Omega < 90.0f && seg[i].Omega > -90.0f) //cis isnt supported yet!
                {
                    return false;
                    //m_Error_Omega += 1.0f;
                }

                dist = angSet.ClosestDistanceTo(seg[i].AminoAcidID, seg[i].Phi, seg[i].Psi );
                if (ANG_DIST_CUTOFF > dist)
                {
                    ID = angSet.ClosestIDTo(seg[i].AminoAcidID, seg[i].Phi, seg[i].Psi);
                    m_Propensity *= angSet[seg[i].AminoAcidID].getPropensity(ID);
                }
                else
                {
                    //m_Error_OutOfRange += 1.0f;
                    return false;
                }
            }
            return true;
        }

        private bool CalculateListPlace(SegmentDef seg, AngleSet angSet)
        {
            return true;
        }

        public override string ToString()
        {
            return String.Format("Length:{0}, Factor:{1:0.000}%, PercDistDownList:{2:0.000}%",
                m_Length,
                m_ProbabilityFactor,
               0.0f // m_PercentageDownList
                );
        }
    }

    /// <summary>
    /// Go through all loops. Measure native angles, find the closest raft angle and look at the propensity information.
    /// Record this for all loops and then decide what the least likely native is. We may want to have a cutoff for
    /// the greatest angle distance away from an angleset angle where the loop is still deemed valid.
    /// </summary>
    public class AssessNative : DSSPTaskDirectory
    {
        private int m_TotalLoopCount;
        private string m_CurrentTraFilePath;
        private List<LoopEntry> m_Results;

        //float m_ErrorCount_Omega = 0.0f;

        public AssessNative(string DBName, DirectoryInfo di)
            : base(DBName, di, false)
        {
        }

        public void FindLeastLikelyLoop( AngleSet angSet )
        {
            ParsingFileIndex = 0; // reset IMPORTANT

            m_Results = new List<LoopEntry>();
            m_TotalLoopCount = 0;
            int traFilesFound = 0;
            m_TotalLoopCount = 0;

            while (true)
            {
                // determine if the current DSSP file has a valid tra file in the culled set

                m_CurrentTraFilePath = "../tra/"
                    + Path.GetFileNameWithoutExtension(CurrentFile.fileInfo.Name)
                    + ".tra";
                
                if (File.Exists(scriptGenerationDirectory.FullName + m_CurrentTraFilePath))
                {
                    Trace.WriteLine(String.Format("Parsing DSSP file for tra file ({0}): {1}", traFilesFound, m_CurrentTraFilePath));
                    traFilesFound++;

                    SegmentDef[] loops = CurrentFile.GetLoops(false, true); // get the current loop set

                    for (int j = 0; j < loops.Length; j++)
                    {
                        LoopEntry le = new LoopEntry();
                        if (le.Calculate(loops[j], angSet)) // loop can be assessed 
                        {
                            m_Results.Add(le);
                            m_TotalLoopCount++;
                        }
                    }
                }

                if (ParsingFileIndex < FileCount - 1)
                {
                    ParsingFileIndex++;
                }
                else
                {
                    break;
                }
            }

            m_Results.Sort(new LoopEntryComparer());

            double minFound = double.MaxValue;
            for (int i = 0; i < m_Results.Count; i++)
            {
                if (minFound > m_Results[i].Probability) minFound = m_Results[i].Probability;
                Trace.WriteLine(m_Results[i].ToString());             
            }

            Trace.WriteLine(String.Format("Total valid tra files found {0}, containing {1} loops", traFilesFound, m_TotalLoopCount));
            Trace.WriteLine(String.Format("Minimum Propensity found : {0}", minFound));
            Console.Read();
        }
    }
}
