using System;
using System.IO;

using UoB.Core.FileIO.DSSP;
using UoB.AppLayer;
using UoB.AppLayer.Petra;

namespace UoB.AppLayer
{
    class PetraInvoke : AppLayerBase
    {
        public PetraInvoke()
        {
        }

        public override string MethodPrintName
        {
            get
            {
                if (m_PurePetra)
                {
                    return "Petra";
                }
                else
                {
                    return "Coda";
                }
            }
        }

        public override string FolderName
        {
            get
            {
                return "Petra";
            }
        }

        public override bool RequiresForcefield
        {
            get
            {
                return true;
            }
        }

        protected bool m_PurePetra = true;

        public override void MainStem(string[] args)
        {
            //PetraInputGenerator iGen = new PetraInputGenerator(this);
            //iGen.CreatePetraJobs();

            PetraProcess iProc = new PetraProcess(this);
            iProc.PurePetra = m_PurePetra;
            iProc.ProcessJobs();
        }
    }
}