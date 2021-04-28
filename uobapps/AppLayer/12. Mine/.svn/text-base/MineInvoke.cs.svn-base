using System;
using System.IO;

using UoB.Core.FileIO.DSSP;
using UoB.AppLayer;
using UoB.AppLayer.PreArcus;

namespace UoB.AppLayer
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    class MineInvoke : AppLayerBase
    {
        public MineInvoke()
        {
        }

        public override string MethodPrintName
        {
            get
            {
                return "PreArcus";
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
            PreArcusProcess iProc = new PreArcusProcess(this);
            //iProc.DoCluster = false;
            //iProc.ClusterCutoff = 0.5;
            //iProc.DumpDecoys = false;
            //iProc.DumpPoint = 0;
            //iProc.completeAttemptedMoves();
            //iProc.assertFiles();
            iProc.ProcessJobs();
        }
    }
}