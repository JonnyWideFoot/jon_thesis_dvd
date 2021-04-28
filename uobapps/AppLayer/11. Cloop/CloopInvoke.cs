using System;
using System.IO;

using UoB.Core.FileIO.DSSP;
using UoB.AppLayer;
using UoB.AppLayer.Cloop;

namespace UoB.AppLayer
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    class CloopInvoke : AppLayerBase
    {
        public CloopInvoke()
        {
        }


        public override string MethodPrintName
        {
            get
            {
                return "Cloop";
            }
        }

        public override bool RequiresForcefield
        {
            get
            {
                return true;
            }
        }

        public override void MainStem(string[] args)
        {
            //CloopInputGenerator iGen = new CloopInputGenerator(this);
            //iGen.CreateCloopJobs();

            CloopProcess iProc = new CloopProcess(this);
            iProc.ProcessJobs();
        }
    }
}