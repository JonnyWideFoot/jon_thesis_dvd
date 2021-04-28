using System;

namespace UoB.Core.Structure.Builder
{
	/// <summary>
	/// Summary description for SequenceInputTypes.
	/// </summary>
	public enum RebuildMode
	{
		AllAtoms = 0,
		PolarAndAromatic = 1,
		PolarHydrogens = 2,
		HeavyAtomsOnly = 3
	}
}
