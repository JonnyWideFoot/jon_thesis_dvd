using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using UoB.AppLayer.FileConverter;

namespace UoB.AppLayer
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    class FileConverterInvoke : AppLayerBase
    {
        public FileConverterInvoke()
        {
#if DEBUG
            // use a limited set of files for testing ... 100 here
            //TaskDir = new DirectoryInfo(@"G:\10a - Loop Builder Stage 1 Post IdealGeom and New AngleBounds\");
            //TaskDir = new DirectoryInfo(@"G:\Loop_Join_Dist_Test\");
            TaskDir = new DirectoryInfo(@"C:\_CalibrateMethod_New_v2\");
#endif
        }

        public override string MethodPrintName
        {
            get
            {
                return "FileConverter";
            }
        }

        public override void MainStem(string[] args)
        {
            FileConversion conv = new FileConversion(TaskDir);

            conv.ConvertTraToPdb("tra_minimised_aa", "pdb_minimised_aa", "*.PDB_Min.tra", 2);
            conv.ConvertTraToPdb("tra_minimised_ua", "pdb_minimised_ua", "*.PDB_Min.tra", 2);

            return;
        }
    }
}
