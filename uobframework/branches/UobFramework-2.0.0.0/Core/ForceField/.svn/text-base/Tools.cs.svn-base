using System;
using System.Collections;

using UoB.Core.Structure;

namespace UoB.Core.ForceField
{
	public class Tools
	{
		private Tools()
		{
		}

		public static AtomList getAllAtomsSurrounding(ParticleSystem ps, Atom theAtom, float cutoffRadius, bool includeAnyExplicitWater)
		{
			AtomList foundAtoms = new AtomList();
			foreach ( Atom a in ps )
			{
				if ( cutoffRadius > Atom.distanceBetween( theAtom, a ) )
				{
					foundAtoms.addAtom( a );
				}
			}
			return foundAtoms;
		}
	}
}