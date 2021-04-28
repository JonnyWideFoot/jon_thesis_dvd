using System;
using System.IO;

using UoB.Core.FileIO.DSSP;
using UoB.AppLayer;
using UoB.AppLayer.Plop;

namespace UoB.AppLayer
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    class PlopInvoke : AppLayerBase
    {
        public PlopInvoke()
        {
        }

        public override bool RequiresForcefield
        {
            get
            {
                return true;
            }
        }

        public override string MethodPrintName
        {
            get { return "Plop"; }
        }

        public override void MainStem(string[] args)
        {
            //PlopInputGenerator iGen = new PlopInputGenerator(this);
            //iGen.CreatePlopJobs();

            PlopProcess iProc = new PlopProcess(this);
            //iProc.mungeBadRMSDFiles();
            iProc.ProcessJobs();
        }
    }
}