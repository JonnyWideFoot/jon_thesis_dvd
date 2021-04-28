using System;
using System.Collections;
using UoB.Core.Primitives;

namespace UoB.Core.Structure
{
	/// <summary>
	/// Summary description for Solvent.
	/// </summary>
	public class HetMolecules : PSMolContainer
	{
		public HetMolecules( ) : base ( '#' )
		{
		}

		public override string ToString()
		{
			return "HetMolecules";
		}

		#region ICloneable Members

		public override object Clone()
		{
			HetMolecules hm = new HetMolecules( );
			for ( int i = 0; i < m_Molecules.Count; i++ )
			{
				Molecule m = (Molecule) m_Molecules[i];
				hm.addMolecule( (Molecule) m.Clone() );
			}
			return hm;
		}

		#endregion
	}
}
