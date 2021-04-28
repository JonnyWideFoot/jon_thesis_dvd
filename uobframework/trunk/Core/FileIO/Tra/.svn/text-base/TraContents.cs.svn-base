using System;

namespace UoB.Core.FileIO.Tra
{
	/// <summary>
	/// A flag enumeration describing the contents of a Tra File
	/// </summary>
	
	[Flags]
	public enum TraContents : int
	{
		Positions = 1,
		Impropers = 2,
		REBUILDP = 4,
		TrajectoryEntries = 8,
		EnergyInfo = 16,
		All = Positions | Impropers | REBUILDP | TrajectoryEntries | EnergyInfo
	}

}
