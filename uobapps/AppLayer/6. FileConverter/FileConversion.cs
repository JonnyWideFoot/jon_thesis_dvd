using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using UoB.Core.Structure;
using UoB.Core.FileIO.Tra;
using UoB.Core.FileIO.PDB;

namespace UoB.AppLayer.FileConverter
{
    class FileConversion
    {
        public FileConversion( DirectoryInfo diSource )
        {
            m_Di = diSource;
        }

        private DirectoryInfo m_Di;

        private void ConvertFile(string inname, string outname, int posSave )
        {
            Trace.Write("Converting: " + inname + "...");
            Tra tFile = new Tra(inname);
            tFile.LoadTrajectory();
            tFile.PositionDefinitions.setPositionsTo(posSave);
            ParticleSystem ps = tFile.particleSystem;
            ps.BeginEditing();
            PSMolContainer psm = ps.MemberAt(0);
            for (int i = 0; i < psm.Count; i++)
            {
                string name = psm[i].Name_NoPrefix;
                if ((name.CompareTo("HIP") == 0) ||
                    (name.CompareTo("HIE") == 0) ||
                    (name.CompareTo("HID") == 0))
                {
                    psm[i].ResetName("HIS", true);
                }
                else if ((name.CompareTo("CYX") == 0))
                {
                    psm[i].ResetName("CYS", true);
                }
            }
            ps.EndEditing(true, true);
            PDB.SaveNew(outname, ps, true);
            Trace.WriteLine("Done!");
        }

        public void ConvertTraToPdb(string inPath, string outPath, string fileFilter, int importEntry)
        {
            DirectoryInfo diSource = new DirectoryInfo( m_Di.FullName + Path.DirectorySeparatorChar + inPath + Path.DirectorySeparatorChar );
            DirectoryInfo diOutput = new DirectoryInfo( m_Di.FullName + Path.DirectorySeparatorChar + outPath + Path.DirectorySeparatorChar);

            if (!diSource.Exists) throw new IOException();
            if (!diOutput.Exists) diOutput.Create();

            FileInfo[] traFiles = diSource.GetFiles(fileFilter);

            for (int i = 0; i < traFiles.Length; i++)
            {
                string nameStem = traFiles[i].Name;
                nameStem = nameStem.Substring(0,nameStem.Length-fileFilter.Length+1);
                string outName = diOutput.FullName + Path.DirectorySeparatorChar + nameStem + ".min.pdb";
                ConvertFile(traFiles[i].FullName, outName, importEntry);
            }
        }
    }
}
