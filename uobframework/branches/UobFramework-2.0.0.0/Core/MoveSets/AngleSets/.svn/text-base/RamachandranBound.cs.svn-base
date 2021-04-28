using System;

namespace UoB.Core.MoveSets.AngleSets
{
	/// Meant to be used in conjunction with the RamachandranRegions class.
	/// Dynamic cast back to int for explicit sorting once the angles have all been alocated
	public enum RamachandranBound : int
	{
		// normal residues
		Beta = 0, // 'B'
        Alpha = 1, // 'A'
		ReverseAlpha = 2, // 'L'

		// glycine only
        GlyWest = 3, // 'W'
		GlyEast = 4, // 'E'
		GlyDispersed = 5, // 'D'

		// proline only
		ProCisAlpha = 6, // 'S'
		ProCisBeta = 7, // 'N'
		ProTransMiddle = 8, // 'M'

		OutsideBounds = 9 // '_'- no fitted angles should occupy this ?
	}
}
