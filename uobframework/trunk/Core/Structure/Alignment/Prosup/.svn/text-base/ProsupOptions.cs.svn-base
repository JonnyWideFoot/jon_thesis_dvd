using System;
using System.IO;

namespace UoB.Core.Structure.Alignment.Prosup
{
	/// <summary>
	/// Summary description for Options
	/// All the parameters for the prosup method
	/// </summary>
	public class ProsupOptions : Options
	{
		public ProsupOptions()
		{
		}

		public override void WriteToStream( StreamWriter rw )
		{
			rw.WriteLine("\tSeed Generation Params");
			rw.Write("\t\tInitial Seed Length : ");
			rw.WriteLine(Seed_InitialLength.ToString());
			rw.Write("\t\tSeed cRMS Cutoff Squared: ");
			rw.WriteLine(Seed_CutoffCRMSSquared.ToString());
			rw.Write("\t\tBeta Vector Filter Enabled : ");
			rw.WriteLine(BetaFilter_C.ToString());
			rw.Write("\t\tBeta Vector distance cutoff : ");
			rw.WriteLine(BetaFilter_CutoffDistance.ToString());
			rw.WriteLine();

			rw.WriteLine("\tSeed Extension Parameters");
			rw.Write("\t\tMinimum Stretch Length For Equivelence : ");
			rw.WriteLine(Iterate_MinStretchLength.ToString());
			rw.Write("\t\tMinimum Total Equivelencies For Validity : ");
			rw.WriteLine(Iterate_MinTotalEquivsForValidity.ToString());
			rw.WriteLine();

			rw.WriteLine("\tSeed Refinement Parameters");
			rw.Write("\t\tPerform Path Refinement Process : ");
			rw.WriteLine(Refine_PerformPathRefignment.ToString());
			rw.Write("\t\tCSquared Value For Refinement : ");
			rw.WriteLine(Refine_CSquared.ToString());
			rw.Write("\t\tMaximum Model Count To Pass To Refinement : ");
			rw.WriteLine(Refine_MaxModels.ToString());
			rw.Write("\t\tPath Refinement Gap Penalty : ");
			rw.WriteLine(Refine_GapPenalty.ToString());
		}

		// initial seed parameters
		public readonly int    Seed_InitialLength = 8;
		//public readonly float  Seed_CutoffCRMS = 2.0f;
		public readonly float  Seed_CutoffCRMSSquared = 4.0f; // the one that is used

		// cBetaFilter for the above seed-gen process
		public readonly bool   BetaFilter_C = false;
		//public readonly float  BetaFilter_CutoffDistance = 1.148f; // roughly 45 degrees based on 1.5A bond length
		//public readonly float  BetaFilter_CutoffDistanceSquared = 1.31f;
		public readonly float  BetaFilter_CutoffDistance = 1.5f; // roughly 60 degrees based on 1.5A bond length
		public readonly float  BetaFilter_CutoffDistanceSquared = 2.25f;

		//iteration
		public readonly int    Iterate_MinStretchLength = 5;
		public readonly int    Iterate_MinTotalEquivsForValidity = 10;

		// refinement best path search parameters
		public readonly bool   Refine_PerformPathRefignment = false;
		public readonly float  Refine_CSquared  = 2.5f; // very tight - no score is given to positions less than 
		public readonly int    Refine_MaxModels = 20; // max number of models to pass to the refinement stage
		public readonly int    Refine_GapPenalty = 10; // used in the score matrix to find the best path whilst minimising gaps

        // iterative geoCentre move at end
		//public bool endRefineGeoCenter = true;
		
		
	}
}
