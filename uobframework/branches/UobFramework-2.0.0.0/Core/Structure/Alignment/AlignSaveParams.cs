using System;

namespace UoB.Core.Structure.Alignment
{
	/// <summary>
	/// Summary description for AlignSaveParams.
	/// </summary>
	[Flags]
	public enum AlignSaveParams : int
	{
		None = 0,
		ExplicitPositions = 1,
		PSInfoBlock = 2,
		AlignReport = 4,
		ModelsDefined = 8,
		All = ExplicitPositions | PSInfoBlock | AlignReport | ModelsDefined,
		Standard = PSInfoBlock | AlignReport | ModelsDefined
	}
}
