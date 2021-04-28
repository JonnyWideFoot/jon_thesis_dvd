using System;
using System.IO;
using System.Diagnostics;

using UoB.Core.FileIO.DSSP;
using UoB.AppLayer;

using UoB.AppLayer.LoopBuilderStage1;

namespace UoB.AppLayer
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class LoopBuilderStage1Invoke : AppLayerBase
	{
        public LoopBuilderStage1Invoke()
        {
        }

        public override string MethodPrintName
        {
            get
            {
                return "Prearcus";
            }
        }

		public override void MainStem( string[] args )
		{

#if DEBUG
            args = new string[] { "6", "7", "8", "9" };
#endif

			if( args.Length < 1 )
			{
				Trace.WriteLine("Please supply some space separated loop lengths ....");
				return;
			}

            int[] lengths;
			try
			{
				lengths = new int[ args.Length ];
				for( int i = 0; i < args.Length; i++ )
				{
					lengths[i] = int.Parse(args[i]);
				}
			}
			catch
			{
				Trace.WriteLine("ERROR : Argument is not a valid integer (loop lengths are all integers...)");
				return;
			}


            DSSPLoopDefFileGeneration defGen = new DSSPLoopDefFileGeneration(
                "PDBSelect2004-1.8", TaskDir);

            //defGen.WriteLoopDefFiles( 12, lengths, new int[]{6}, new int[]{8} );
            //defGen.WriteSimpleDefFile(lengths);

            //defGen.WriteLoopCalibrationTest(lengths, true);
            //defGen.ResultsLoopCalibrationTest();
            //defGen.ResultsLoopCalibrationTestB();
            //defGen.ResultsLoopCalibrationTestC();

            //defGen.WriteLoopCalibrationTest_Stage2(lengths, false);
            //defGen.ResultsLoopCalibrationTest_Stage2();

            defGen.CondorStage1_Generate(lengths);

			return;
		}
	}
}