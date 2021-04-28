using System;
using System.IO;

using UoB.Core.FileIO.DSSP;
using UoB.AppLayer;
using UoB.AppLayer.Modeller;

namespace UoB.AppLayer
{
    class ModellerInvoke : AppLayerBase
    {
        public ModellerInvoke()
        {
        }

        public override string MethodPrintName
        {
            get
            {
                return "Modeller";
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
            ModellerInputGenerator iGen = new ModellerInputGenerator(this);
            iGen.CreateModellerJobs();

            //ModellerDopeRun iDope = new ModellerDopeRun(this);
            //iDope.CreateModellerJobs();

            //ModellerProcess iProc = new ModellerProcess(this);
            //iProc.ProcessJobs();
        }
    }
}