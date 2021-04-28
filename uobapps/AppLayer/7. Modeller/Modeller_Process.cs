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

using UoB.Compression;

namespace UoB.AppLayer.Modeller
{
    enum ModellerScore
    {
        Dope,
        molpdf
    }

    class ModellerProcess : DSSPLoopAnalysis
    {
        public ModellerProcess(AppLayerBase parent)
            : base(parent)
        {
            //StartLoopLength = 9;
        }

        private OptimisationSchedule m_OptSched = OptimisationSchedule.very_fast;
        private ModellerScore m_ScoreFunc = ModellerScore.molpdf;
        private bool m_ExtendedAnalysis = true;

        private LoopStatAnalyse m_StatRef = null;
        private List<double> m_TempEne = new List<double>(m_ModelCount);
        private List<LoopStatAnalyse> m_LoopAnalysis = new List<LoopStatAnalyse>();

        private const int m_ModelCount = 1000;

        #region RMSFinder Delegate and complementary functions

        private delegate double LoopStatAnalyse_Delegate(LoopStatAnalyse loop, int index);

        private double getCARMS( LoopStatAnalyse loop, int index )
        {
            return loop.getRMS_CA( index );
        }

        private double getBBRMS( LoopStatAnalyse loop, int index )
        {
            return loop.getRMS_BB( index );
        }

        private double getAARMS( LoopStatAnalyse loop, int index )
        {
            return loop.getRMS_AA( index );
        }

        private double getBBARMS( LoopStatAnalyse loop, int index )
        {
            return loop.getRMS_BBA( index );
        }

        /// <summary>
        /// Obtain the RMS of the structure with the lowest energy within a given number of models
        /// The chosen RMS type is selected by the functor
        /// </summary>
        /// <param name="loop"></param>
        /// <param name="functor"></param>
        /// <param name="max"></param>
        private void RMSFinder(LoopStatAnalyse loop, LoopStatAnalyse_Delegate rmsFunctor, int max, ref double totalBestRMS, ref double totalBestEneRMS )
        {
            double bestEne = double.MaxValue;
            double bestRMS = double.MaxValue;
            double rmsOfBestEne = double.MaxValue;
            for( int i = 0; i < max; i++ )
            {
                double ene = loop.getEne(i);
                double rms = rmsFunctor(loop, i);
                if (ene < bestEne)
                {
                    bestEne = ene;
                    rmsOfBestEne = rms;
                }
                if (rms < bestRMS)
                {
                    bestRMS = rms;
                }
            }
            totalBestRMS += bestRMS;
            totalBestEneRMS += rmsOfBestEne;
        }

        #endregion

        private void PrintAnalysis( int loopLength, int modelMax )
        {
            StreamWriter statWriter = new StreamWriter(reportDirectory.FullName + Path.DirectorySeparatorChar + "stat" + loopLength.ToString() + ".csv");

            LoopStatAnalyse_Delegate funcCA = new LoopStatAnalyse_Delegate( getCARMS );
            LoopStatAnalyse_Delegate funcBB = new LoopStatAnalyse_Delegate( getBBRMS );
            LoopStatAnalyse_Delegate funcAA = new LoopStatAnalyse_Delegate( getAARMS );
            LoopStatAnalyse_Delegate funcBBA = new LoopStatAnalyse_Delegate( getBBARMS );

            for( int modelCount = 0; modelCount < modelMax; modelCount++ )
            {
                double sumCABestRMS = 0.0;
                double sumBBBestRMS = 0.0;
                double sumAABestRMS = 0.0;
                double sumBBABestRMS = 0.0;
                double sumCABestEneRMS = 0.0;
                double sumBBBestEneRMS = 0.0;
                double sumAABestEneRMS = 0.0;
                double sumBBABestEneRMS = 0.0;

                for (int i = 0; i < m_LoopAnalysis.Count; i++)
                {
                    RMSFinder(m_LoopAnalysis[i], funcCA, modelCount, ref sumCABestRMS, ref sumCABestEneRMS);
                    RMSFinder(m_LoopAnalysis[i], funcBB, modelCount, ref sumBBBestRMS, ref sumBBBestEneRMS);
                    RMSFinder(m_LoopAnalysis[i], funcAA, modelCount, ref sumAABestRMS, ref sumAABestEneRMS);
                    RMSFinder(m_LoopAnalysis[i], funcBBA, modelCount, ref sumBBABestRMS, ref sumBBABestEneRMS);
                }

                double modelCountDouble = (double) modelCount;
                statWriter.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                        modelCount,
                        sumCABestRMS / modelCountDouble,
                        sumBBBestRMS / modelCountDouble,
                        sumAABestRMS / modelCountDouble,
                        sumBBABestRMS / modelCountDouble,
                        sumCABestEneRMS / modelCountDouble,
                        sumBBBestEneRMS / modelCountDouble,
                        sumAABestEneRMS / modelCountDouble,
                        sumBBABestEneRMS / modelCountDouble);        
            }

            statWriter.Close();
        }

        protected override void JobEndHook( int loopLength )
        {
            PrintAnalysis(loopLength, m_ModelCount);
        }

        protected override bool PostAnalysisHook(string currentName, string pdbID, PDB libPDB, SegmentDef loop)
        {
            if (m_StatRef == null) throw new Exception();

            // The archive has already been extracted, but we need to do the cleanup here
            string stem = String.Format("{0}_{1}_{2}", pdbID, loop.FirstDSSPIndex - 1, loop.Length);
            string resPath = resultDirectory.FullName + loop.Length.ToString() + Path.DirectorySeparatorChar;
            string archive = resPath + stem + ".tar.bz2";
            string archivePath = m_Comp.OutPath(archive).FullName + Path.DirectorySeparatorChar;

            if (m_ExtendedAnalysis)
            {
                m_StatRef.ClearStats();
                if (m_TempEne.Count != m_ModelCount) throw new Exception();
                for (int i = 1; i <= m_ModelCount; i++)
                {
                    string fragName = String.Concat(archivePath, stem, ".BL", i.ToString().PadLeft(4, '0'), "0001.pdb");
                    if (!File.Exists(fragName)) throw new FileNotFoundException(); // odd because we have already checked for this
                    PDB loopPS = new PDB(fragName, true);
                    m_StatRef.AnalyseFragment(loopPS.particleSystem, -1, m_TempEne[i - 1]); // Perform Analysis. -1 means we should have the entire protein per loop.
                }
                m_StatRef.FreeParticleSystemMemory();
                m_LoopAnalysis.Add(m_StatRef); // Add this to the analysis collection, things could get BIG!
            }

            m_Comp.CleanUp(archive);            
            m_StatRef = null; // we have done with the reference, null it to make sure its not used more than once

            return true;
        }

        protected override LoopStatAnalyse DoStructuralAnalysis(string currentName, string pdbID, PDB libPDB, SegmentDef loop, ref bool methodFailure)
        {
            string resPath = baseDirectory.FullName + "CompleteData_8v2" + Path.DirectorySeparatorChar +
                "ScriptGen_" + m_OptSched.ToString() + Path.DirectorySeparatorChar + loop.Length.ToString() + Path.DirectorySeparatorChar;
            string stem = String.Format("{0}_{1}_{2}", pdbID, loop.FirstDSSPIndex - 1, loop.Length);

            // Assert results are valid
            Validity returnVal = ModellerFunctions.IsValidReturn(resPath, stem, m_OptSched);
            if (Validity.MethodFail == returnVal)
            {
                methodFailure = true;
                Trace.WriteLine("Flagged Method Failure: {0}", stem);
                return null;
            }
            else if (Validity.MethodFail == returnVal)
            {
                //throw new Exception();
                methodFailure = true; // there are now only two special 'Invalid' cases ... these have failed, but are odd to detect
                // I have manually looked at each one of these
                Trace.WriteLine("Flagged 'hard-detect' Method Failure: {0}", stem);
                return null;
            }

            string archive = resPath + stem + ".tar.bz2";
            string archivePath = ModellerFunctions.AssertArchive(m_Comp, archive, stem, m_ModelCount, true, false);

            // Identify this from the relevent source
            int bestEne = -1;
            double bestValue = double.MaxValue;
            m_TempEne.Clear(); // important

            if (m_ScoreFunc == ModellerScore.molpdf)
            {
                StreamReader re = new StreamReader(archivePath + stem + ".log");
                string line;

                bool found = false;
                while (null != (line=re.ReadLine()))
                {
                    if( 0 == String.CompareOrdinal(line, ">> Summary of successfully produced loop models:"))
                    {
                        found = true;
                        break;
                    }
                }
                if( !found )
                {
                    throw new Exception();
                }

                // Eat two lines
                line = re.ReadLine();
                if (line == null) throw new Exception();
                line = re.ReadLine();
                if (line == null) throw new Exception();

                // **Important**
                // I mildly stupidly generated 1001 modeller models for each loop, 
                // but calculated the DOPE energy for 1000 starting from model 1 not 0
                // Here we can just ignore model 0.
                line = re.ReadLine(); // ignore 1st model
                if (line == null) throw new Exception();

                for( int wantIndex = 1; wantIndex <= m_ModelCount; wantIndex++ )
                {
                    if (null == (line = re.ReadLine())) throw new NullReferenceException("We havent got all the conformations required!");
                    line = line.Trim();
                    string[] lineParts = UoB.Core.Tools.CommonTools.WhiteSpaceRegex.Split(line);
                    
                    string[] modelIDParts = lineParts[0].Split('.');
                    if (modelIDParts.Length != 3) throw new Exception();
                    if (0 != string.Compare(modelIDParts[0], stem)) throw new Exception();
                    if (0 != string.Compare(modelIDParts[2], "pdb")) throw new Exception();
                    if (10 != modelIDParts[1].Length) throw new Exception();
                    if (0 != String.Compare("BL", 0, modelIDParts[1], 0, 2, false)) throw new Exception();
                    if (0 != String.Compare("0001", 0, modelIDParts[1], modelIDParts[1].Length-4, 4, false)) throw new Exception();

                    int index = int.Parse(modelIDParts[1].Substring(2,4));
                    if (index != wantIndex) throw new Exception();

                    double ene = double.Parse(lineParts[1]);
                    m_TempEne.Add(ene);
                    if (ene < bestValue)
                    {
                        bestEne = index;
                        bestValue = ene;
                    }
                }

                re.Close();
            }
            else
            {
                FileInfo dopeDat = new FileInfo(resPath + stem + ".dope");
                if (!dopeDat.Exists)
                {
                    throw new FileNotFoundException("Cannot find dopeDat");
                }

                // As Modeller is not kind enough to give us a file containing the structure with the best energy, we will have to find it ourselves.
                // Determine the lowest energy conformation from the ensemble of 'm_ModelCount' (1000) structures
                StreamReader re = new StreamReader(dopeDat.FullName);

                string line;
                int wantIndex = 1;
                while (null != (line = re.ReadLine()))
                {
                    line = line.Trim();
                    string[] lineParts = UoB.Core.Tools.CommonTools.WhiteSpaceRegex.Split(line);
                    int index = int.Parse(lineParts[0]);
                    double ene = double.Parse(lineParts[1]);
                    if (index != wantIndex++) throw new Exception();
                    m_TempEne.Add(ene);
                    if (ene < bestValue)
                    {
                        bestEne = index;
                        bestValue = ene;
                    }
                }
                re.Close();
                if (wantIndex != m_ModelCount+1) throw new Exception("We havent got all the conformations required!");
            }

            string loopFileName = String.Concat(archivePath, stem, ".BL", bestEne.ToString().PadLeft(4, '0'), "0001.pdb");
            PDB loopPS = new PDB(loopFileName, true);

            LoopStatAnalyse stat = new LoopStatAnalyse(libPDB.particleSystem, loop);
            stat.MissingHack = false;
            stat.AnalyseFragment(loopPS.particleSystem, -1, bestEne); // Perform Analysis. -1 Means we should have the whole protein to analyse.

            // We dont want to do this, as there is an override of the PostAnalysis exit hook above - this will do the cleanup
            // m_Comp.CleanUp(archive);

            // Keep a temporary reference to what we just made
            m_StatRef = stat;

            return stat;
        }
    }
}

