using System;

namespace UoB.Core.FileIO.PDB
{
    [Flags]
	public enum PDBExpRslnMethod
	{
		Crystalographic,
		InfraredMicroscopy,
		FiberDiffraction,
		ElectronMicroscopy,
		SingleCrystalElectronMicroscopy,
		NMR,
		NMRMinimized,
		NMRAveraged,
		NMRRepresentative,
		NMRTheoretical,
		NMRRegularizedMean,
		NMRRestrainedRegularizedMean,
		DAVEUnknown,
		Undefined
	}
}
