using System;

namespace UoB.Core.Sequence
{
	[Flags]
	public enum StandardResidues : long 
		// a big flagged enumeration requires a "long" ( 8 bytes => 64 bits )
	{
		None = 0,
		All = A | C | D | E | F | G | H | I | K | L | M | N | P | p | Q | R | S | T | V | W | Y,
		NotGlyOrPro = A | C | D | E | F | H | I | K | L | M | N | Q | R | S | T | V | W | Y,
		ShortHydrophobic = A | I | L | V,
        BulkyAromatic = F | H | W | Y,
		Polar = N | C | Q | S | T | Y,
		Charged = R | D | E | H | K,

		A = 	1		  ,	// Ala
		C = 	2		  ,	// Cys
		D = 	4		  ,	// Asp
		E = 	8		  ,	// Glu
		F = 	16		  ,	// Phe
		G = 	32		  ,	// Gly
		H = 	64		  ,	// His
		I = 	128		  ,	// Ile
		K = 	256		  ,	// Lys
		L = 	512		  ,	// Leu
		M = 	1024	  ,	// Met
		N = 	2048	  ,	// Asn
		P = 	4096	  ,	// Pro - Trans
		p = 	8192	  ,	// Pro - Cis
		Q = 	16384	  ,	// Gln
		R = 	32768	  ,	// Arg
		S = 	65536	  ,	// Ser
		T = 	131072	  ,	// Thr
		V = 	262144	  ,	// Val
		W = 	524288	  ,	// Trp
		Y = 	1048576	  ,	// Tyr
	}
}
