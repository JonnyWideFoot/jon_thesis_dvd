using System;
using System.IO;

using UoB.Core.FileIO.DSSP;
using UoB.AppLayer;
using UoB.AppLayer.Filter;

namespace UoB.AppLayer
{
    class FilterInvoke : AppLayerBase
    {
        public FilterInvoke()
        {
        }

        public override string MethodPrintName
        {
            get
            {
                return "Filter";
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
            FilterGenerator iGen = new FilterGenerator("", TaskDir);
            iGen.doFilter();
        }
    }
}