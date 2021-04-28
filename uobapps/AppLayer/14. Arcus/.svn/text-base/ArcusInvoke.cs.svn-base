using System;
using System.IO;

using UoB.Core.FileIO.DSSP;
using UoB.AppLayer;
using UoB.AppLayer.Arcus;

namespace UoB.AppLayer
{
    class ArcusInvoke : AppLayerBase
    {
        public ArcusInvoke()
        {
        }

        public override string MethodPrintName
        {
            get
            {
                return "Arcus";
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
            ArcusGenerator iGen = new ArcusGenerator(this);
            for (int i = 6; i <= 11; i++)
            {
                iGen.printList(i);
            }            
            //iGen.printListS3(8);

            //ArcusProcess iProc = new ArcusProcess(this);
            //iProc.ProcessJobs();
        }
    }
}