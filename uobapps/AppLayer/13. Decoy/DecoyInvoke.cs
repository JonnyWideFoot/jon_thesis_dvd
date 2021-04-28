using System;
using System.IO;

using UoB.Core.FileIO.DSSP;
using UoB.AppLayer;
using UoB.AppLayer.Decoy;

namespace UoB.AppLayer
{
    class DecoyInvoke : AppLayerBase
    {
        public DecoyInvoke()
        {
        }

        public override string MethodPrintName
        {
            get
            {
                return "Decoy";
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
            DecoyGenerator iGen = new DecoyGenerator(this);
            //iGen.CreateNativePertJobs();
            //iGen.CreateDecoys();
            iGen.thinDecoys();
            //iGen.ClusterDecoys();
            //iGen.GenerateDockit();
            iGen.GenerateDFire();
        }
    }
}