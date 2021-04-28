using System;
using System.IO;
using System.Collections.Generic;

using UoB.Core.Sequence;
using UoB.Core.FileIO.DSSP;
using UoB.Core.MoveSets.AngleSets;

using UoB.Methodology.TaskManagement;
using UoB.Methodology.DSSPAnalysis;

namespace UoB.Methodology.DSSPAnalysis.Angleset
{
    public class AngleSetStats : DSSPTaskDirecory_PhiPsiData
    {
        public AngleSetStats(string DBName, DirectoryInfo di)
            : base(DBName, di, false)
        {
        }

        public void FurtherstDistanceStats()
        {
            List<AngleSet> anglsSets = new List<AngleSet>();
            List<string> angleSetName = new List<string>();

            string angPath = baseDirectory.FullName + "angleset" + Path.DirectorySeparatorChar;

            anglsSets.Add(new AngleSet(angPath + "0_0_loop.angleset"));
            angleSetName.Add("0");
            anglsSets.Add(new AngleSet(angPath + "1_0_loop.angleset"));
            angleSetName.Add("1");

            anglsSets.Add(new AngleSet(angPath + "3_0_loop.angleset"));
            angleSetName.Add("3");
            anglsSets.Add(new AngleSet(angPath + "4_0_loop.angleset"));
            angleSetName.Add("4");
            anglsSets.Add(new AngleSet(angPath + "5_0_loop.angleset"));
            angleSetName.Add("5");
            anglsSets.Add(new AngleSet(angPath + "6_0_loop.angleset"));
            angleSetName.Add("6");
            anglsSets.Add(new AngleSet(angPath + "7_0_loop.angleset"));
            angleSetName.Add("7");
            anglsSets.Add(new AngleSet(angPath + "8_0_loop.angleset"));
            angleSetName.Add("8");
            anglsSets.Add(new AngleSet(angPath + "9_0_loop.angleset"));
            angleSetName.Add("9");
            anglsSets.Add(new AngleSet(angPath + "10_0_loop.angleset"));
            angleSetName.Add("10");

            for (int i = 0; i < anglsSets.Count; i++) anglsSets[i].CaseSplitCis = true;

            anglsSets.Add(new AngleSet(angPath + "Original_RAFT.angleset")); // load the default angle set to display on the relevent graphs
            angleSetName.Add("RAFT");
            anglsSets.Add(new AngleSet(angPath + "Optimal_6_Loop.angleset"));
            angleSetName.Add("Optimal6");
            anglsSets.Add(new AngleSet(angPath + "default.loop.angleset"));
            angleSetName.Add("default.loop.angleset");
            anglsSets.Add(new AngleSet(angPath + "big.loop.angleset"));
            angleSetName.Add("big.loop.angleset");            
            

            string k12name = reportDirectory.FullName + @"_K12" + ".csv";
            StreamWriter rw_k12 = new StreamWriter(k12name);
            rw_k12.Write("Set Name");

            StandardResidues[] singleResTypes = StandardSeqTools.GetIndividualStandardResidues();
            //StandardResidues[] singleResTypes = new StandardResidues[] { StandardResidues.p };

            List<char> closestDistType = new List<char>();
            for (int i = 0; i < singleResTypes.Length; i++)
            {
                char molID = singleResTypes[i].ToString()[0];
                closestDistType.Add(molID);
                rw_k12.Write(",{0}", singleResTypes[i]);
            }
            rw_k12.WriteLine(",Ave");
                        
            List<List<Double>> closestDistHolder = new List<List<double>>();
            for (int j = 0; j < anglsSets.Count; j++)
            {
                for (int i = 0; i < singleResTypes.Length; i++)
                {
                    char molID = singleResTypes[i].ToString()[0];
                    char cisTrans = Char.IsUpper(molID) ? 'T' : 'C';
                    StreamWriter rw = new StreamWriter(reportDirectory.FullName + @"Dist_" + angleSetName[j] + "_" + molID + cisTrans + ".csv");

                    List<Double> closestDist = new List<double>(10000);
                    closestDistHolder.Add(closestDist);                    

                    rw.WriteLine(molID);
                    ParsingFileIndex = 0; // reset IMPORTANT
                    while (true)
                    {
                        SegmentDef[] loops = CurrentFile.GetLoops(false, true);

                        for (int k = 0; k < loops.Length; k++)
                        {
                            SegmentDef loop = loops[k];
                            for (int p = 0; p < loop.Length; p++)
                            {
                                if (molID == loop.Sequence[p])
                                {
                                    double d = anglsSets[j].ClosestDistanceTo(molID, loop.GetPhi(p), loop.GetPsi(p));
                                    closestDist.Add(d);
                                    rw.WriteLine("{0:N3}",d);
                                }
                            }
                        }

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

                }

                StreamWriter rw_cum = new StreamWriter(reportDirectory.FullName + @"Cumulative_" + angleSetName[j] + ".csv");
                CumulativePercentage(rw_cum, closestDistType, closestDistHolder);
                rw_cum.Close();

                CalcK12(rw_k12, angleSetName[j], closestDistHolder);

                closestDistHolder.Clear();
            }
            rw_k12.Close();

            UoB.Core.Tools.ArrayTools.TransposeCSV(k12name,k12name);

        }


        private void CalcK12(StreamWriter rw, string set, List<List<double>> data)
        {
            rw.Write(set);
            double sum = 0.0;
            for (int j = 0; j < data.Count; j++)
            {
                double d = K12(data[j]);
                sum += d;
                rw.Write(",{0:N3}", d);
            }
            rw.WriteLine(",{0:N3}", sum / ((double)data.Count));
        }

        private double K12(List<double> data)
        {
            data.Sort();
            return data[data.Count / 2];
        }

        private void CumulativePercentage(StreamWriter rw, List<char> closestDistType, List<List<double>> data)
        {
            rw.Write("Cutoff");
            for (int j = 0; j < closestDistType.Count; j++)
            {
                rw.Write(",{0}", closestDistType[j]);
            }
            rw.WriteLine(",Ave");
            for (double i = 0.0; i <= 120.0; i++)
            {
                rw.Write("{0:N3}", i);
                double sum = 0.0;
                for( int j = 0; j < closestDistType.Count; j++ )
                {
                    double cum = CumulativePercentage( i, data[j] );
                    rw.Write(",{0:N3}", cum);
                    sum += cum;
                }
                rw.WriteLine(",{0:N3}", sum / ((double)closestDistType.Count));
            }
        }
        private double CumulativePercentage( double cutoff, List<double> data )
        {
            int count = 0;
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] < cutoff) count++;
            }
            return ((double)count / (double)data.Count) * 100.0; 
        }
    }




    //    public void LoopLengthCounting()
    //    {
    //        int[] loopLengthCounts = new int[150];
    //        // find loops of length 0 to 150, the integers are incremented when a 
    //        // loop of that length is found ...

    //        ParsingFileIndex = 0; // reset IMPORTANT
    //        while (true)
    //        {
    //            CurrentFile.CountLoopLengths(loopLengthCounts);
    //            // increment conidtion
    //            if (ParsingFileIndex < FileCount - 1)
    //            {
    //                ParsingFileIndex++;
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }

    //        // write the derived stats to the report directory ...
    //        StreamWriter rw = new StreamWriter(reportDirectory.FullName + "_LoopLengthStats.csv");
    //        for (int i = 0; i < loopLengthCounts.Length; i++)
    //        {
    //            rw.Write(i);
    //            rw.Write(',');
    //            rw.WriteLine(loopLengthCounts[i]);
    //        }
    //        rw.Close();
    //    }

    //    public void CountDisallowed()
    //    {
    //        int goodLoops = 0;
    //        int badLoops = 0;
    //        int goodResiduesInLoops = 0;
    //        int badResiduesInLoops = 0;

    //        ParsingFileIndex = 0; // reset IMPORTANT
    //        while (true)
    //        {
    //            CurrentFile.CountDisallowed(ref goodLoops, ref badLoops,
    //                ref goodResiduesInLoops, ref badResiduesInLoops);
    //            // increment conidtion
    //            if (ParsingFileIndex < FileCount - 1)
    //            {
    //                ParsingFileIndex++;
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }

    //        StreamWriter rw = new StreamWriter(reportDirectory.FullName + "_LoopWithDisallowedResiduesStats.csv");
    //        rw.WriteLine(goodLoops);
    //        rw.WriteLine(badLoops);
    //        rw.WriteLine(goodResiduesInLoops);
    //        rw.WriteLine(badResiduesInLoops);

    //        float f_goodLoops = (float)goodLoops;
    //        float f_badLoops = (float)badLoops;
    //        float f_goodResiduesInLoops = (float)goodResiduesInLoops;
    //        float f_badResiduesInLoops = (float)badResiduesInLoops;

    //        rw.WriteLine((f_badLoops / (f_goodLoops + f_badLoops)) * 100.0f);
    //        rw.WriteLine((f_badResiduesInLoops / (f_badResiduesInLoops + f_goodResiduesInLoops)) * 100.0f);
    //        rw.Close();
    //    }

    //    public void DeviantAngleStats()
    //    {
    //        StreamWriter rw = new StreamWriter(reportDirectory.FullName + "_DeviantAngleAnalysis.csv");
    //        for (float cutoff = 1.0f; cutoff < 180.0f; cutoff += 1.0f)
    //        {
    //            CurrentFile.PercentageLoopsWithDeviantAngleStats_InitialiseForCutoff();

    //            ParsingFileIndex = 0; // reset IMPORTANT
    //            while (true)
    //            {
    //                //CurrentFile.StatFinction();
    //                CurrentFile.PercentageLoopsWithDeviantAngleStats(m_AngleSet, cutoff);
    //                // increment conidtion
    //                if (ParsingFileIndex < FileCount - 1)
    //                {
    //                    ParsingFileIndex++;
    //                }
    //                else
    //                {
    //                    break;
    //                }
    //            }

    //            CurrentFile.PercentageLoopsWithDeviantAngleStats_EndForCutoff(cutoff, rw);
    //            rw.Flush();
    //        }
    //        rw.Close();
    //    }


    //    public void CountDisallowed(ref int goodLoops, ref int badLoops, ref int goodResiduesInLoops, ref int badResiduesInLoops)
    //    {
    //        for (int i = 1; i < m_LoopDefs.Count - 1; i++) // -1 +1 as we dont want the termini
    //        {
    //            LoopDef ld = (LoopDef)m_LoopDefs[i];
    //            if (ld.Length == -1)
    //            {
    //                continue; // the loop is incomplete in the PDB, the phi and psi of the respective anchors will be 360
    //            }

    //            bool flagBad = false;
    //            for (int j = 0; j < ld.Length; j++)
    //            {
    //                ResidueDef res = ld[j];
    //                if (res.IsDisallowed)
    //                {
    //                    flagBad = true;
    //                    badResiduesInLoops++;
    //                }
    //                else
    //                {
    //                    goodResiduesInLoops++;
    //                }
    //            }
    //            if (flagBad)
    //            {
    //                badLoops++;
    //            }
    //            else
    //            {
    //                goodLoops++;
    //            }
    //        }
    //    }

    //    public void CountLoopLengths(int[] loopLengths)
    //    {
    //        for (int i = 1; i < m_LoopDefs.Count - 1; i++) // -1 +1 as we dont want the termini
    //        {
    //            LoopDef ld = (LoopDef)m_LoopDefs[i];
    //            if (ld.Length == -1)
    //            {
    //                continue; // the loop is incomplete in the PDB, the phi and psi of the respective anchors will be 360
    //            }

    //            if (ld.Length > 50)
    //            {
    //                for (int j = 0; j < ld.Length; j++)
    //                {
    //                    ResidueDef res = ld[j];
    //                    if (res.AminoAcidID != 'G')
    //                    {
    //                        if (res.Phi < 64.0 && res.Phi > 56.0 &&
    //                            res.Psi < -115 && res.Psi > -135)
    //                        {
    //                            break;
    //                        }
    //                    }
    //                }
    //            }


    //            loopLengths[ld.Length]++;
    //        }
    //    }

    //    private int m_LoopAnglesFineCount;
    //    private int m_LoopAnglesDeviantCount;
    //    private int m_LoopContainsNonProCis;
    //    private int m_LoopUnknownResidues;
    //    private int m_LoopTotal;
    //    public void PercentageLoopsWithDeviantAngleStats_InitialiseForCutoff()
    //    {
    //        m_LoopAnglesFineCount = 0;
    //        m_LoopAnglesDeviantCount = 0;
    //        m_LoopContainsNonProCis = 0;
    //        m_LoopUnknownResidues = 0;
    //        m_LoopTotal = 0;
    //    }

    //    public void PercentageLoopsWithDeviantAngleStats_EndForCutoff(double cutoff, StreamWriter deviantResults)
    //    {
    //        float good = (float)m_LoopAnglesFineCount;
    //        float total = (float)m_LoopTotal;
    //        float percentage = (good / total) * 100.0f;
    //        deviantResults.Write(cutoff);
    //        deviantResults.Write(',');
    //        deviantResults.Write(m_LoopContainsNonProCis);
    //        deviantResults.Write(',');
    //        deviantResults.Write(m_LoopUnknownResidues);
    //        deviantResults.Write(',');
    //        deviantResults.Write(m_LoopAnglesFineCount);
    //        deviantResults.Write(',');
    //        deviantResults.Write(m_LoopAnglesDeviantCount);
    //        deviantResults.Write(',');
    //        deviantResults.Write(m_LoopTotal);
    //        deviantResults.Write(',');
    //        deviantResults.WriteLine(percentage);

    //    }
    //    public void PercentageLoopsWithDeviantAngleStats(AngleSet angleSet, double cutoff)
    //    {
    //        bool XFlag;
    //        bool CisFlag;
    //        bool DeviantFlag;

    //        for (int i = 1; i < m_LoopDefs.Count - 1; i++) // -1 + 1 as we dont want the termini
    //        {
    //            // reset flags per loop
    //            XFlag = false;
    //            CisFlag = false;
    //            DeviantFlag = false;

    //            LoopDef ld = (LoopDef)m_LoopDefs[i];
    //            if (ld.Length == -1)
    //            {
    //                continue; // the loop is incomplete in the PDB, the phi and psi of the respective anchors will be 360
    //            }

    //            for (int j = 0; j < ld.Length; j++)
    //            {
    //                char molID = ld.Sequence[j];
    //                if (molID == 'X')
    //                {
    //                    XFlag = true;
    //                    break;
    //                }
    //                if (Char.IsLower(molID) && molID != 'p')
    //                {
    //                    CisFlag = true; // its a non-pro cis residue
    //                    break;
    //                }
    //                double dist = angleSet.ClosestDistanceTo(molID, ld.GetPhi(j), ld.GetPsi(j));
    //                if (cutoff < dist)
    //                {
    //                    DeviantFlag = true;
    //                }
    //            }

    //            if (XFlag)
    //            {
    //                m_LoopUnknownResidues++;
    //            }
    //            if (CisFlag)
    //            {
    //                m_LoopContainsNonProCis++; // its an unknown residue
    //            }
    //            if (DeviantFlag)
    //            {
    //                m_LoopAnglesDeviantCount++;
    //            }
    //            if (!XFlag && !CisFlag && !DeviantFlag)
    //            {
    //                m_LoopAnglesFineCount++;
    //            }
    //            m_LoopTotal++;
    //        }
    //    }
}