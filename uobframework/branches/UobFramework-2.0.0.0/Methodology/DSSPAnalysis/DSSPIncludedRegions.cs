using System;

namespace UoB.Methodology.DSSPAnalysis
{
	public enum DSSPIncludedRegions
	{
        AllAvailable, // all valid phi/psi pairs will be added
		AllExceptTermini, // this will inlcude residues in incomplete loops
		AllExceptIncompleteSegments, // this will include the termini
		OnlyDefinitelyGood // no termini or incomplete loops
	}
}
