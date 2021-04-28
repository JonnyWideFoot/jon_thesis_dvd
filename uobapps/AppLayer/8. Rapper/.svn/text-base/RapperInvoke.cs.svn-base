using System;
using System.IO;

using UoB.Core.FileIO.DSSP;
using UoB.AppLayer;
using UoB.AppLayer.Rapper;

namespace UoB.AppLayer
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    class RapperInvoke : AppLayerBase
    {
        public RapperInvoke()
        {
        }

        public override string MethodPrintName
        {
            get
            {
                return "Rapper";
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
            RapperInputGenerator iGen = new RapperInputGenerator(this);
            iGen.CreateRapperJobs();

            //RapperProcess iProc = new RapperProcess(this);
            //iProc.ProcessJobs();
        }
    }
}