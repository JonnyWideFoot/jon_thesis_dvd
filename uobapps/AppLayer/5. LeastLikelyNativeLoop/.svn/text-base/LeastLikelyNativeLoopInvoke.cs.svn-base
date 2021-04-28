using System;
using System.IO;
using System.Diagnostics;

using UoB.Core.FileIO.DSSP;
using UoB.Core.MoveSets.AngleSets;
using UoB.AppLayer;

namespace UoB.AppLayer.LeastLikelyNativeLoop
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class LeastLikelyNativeLoopInvoke : AppLayerBase
	{
        public LeastLikelyNativeLoopInvoke()
		{
			#if DEBUG
				// use a limited set of files for testing ... 100 here
                TaskDir = new DirectoryInfo(@"y:\10a - Loop Builder Stage 1 Post IdealGeom and New AngleBounds");
            #endif
        }

        public override string MethodPrintName
        {
            get
            {
                return "NativeAnalysis";
            }
        }

		public override void MainStem( string[] args )
		{
			try
			{
                AngleSet angSet = new AngleSet( TaskDir.FullName + Path.DirectorySeparatorChar + "6_8.angleset" );
                AssessNative assNat = new AssessNative("PDBSelect2004-1.8", TaskDir );
                assNat.FindLeastLikelyLoop(angSet);
			}
			catch( Exception ex )
			{
				Trace.WriteLine("Error! : " + ex.ToString() );
				return;
			}	
		
			return;
		}
	}
}