using System;
using System.Collections;
using UoB.Core.Primitives;

namespace UoB.Core.Structure
{
	/// <summary>
	/// Summary description for Solvent.
	/// </summary>
	public class Solvent : PSMolContainer
	{
		public Solvent() : base ( '~' ) // effectively a null character
		{
		}

		public override string ToString()
		{
			return "Explicit Solvent";
		}

		#region ICloneable Members

		public override object Clone()
		{
			Solvent s = new Solvent();
			for ( int i = 0; i < m_Molecules.Count; i++ )
			{
				Molecule m = (Molecule) m_Molecules[i];
				s.addMolecule( (Molecule) m.Clone() );
			}
			return s;
		}

		#endregion
	}
}
