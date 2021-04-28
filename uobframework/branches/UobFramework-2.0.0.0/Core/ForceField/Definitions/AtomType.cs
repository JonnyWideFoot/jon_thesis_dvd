using System;

namespace UoB.Core.ForceField.Definitions
{
	/// <summary>
	/// Summary description for AtomType.
	/// </summary>
	public class AtomType
	{
		//#   Groups      .---<atom>----.  .-----vdw-------.  .--elec--. .--- other ---->
		//#   Type          (Z)  AtMass     Radius  Epsilon    Charge     Solv.    hBond   Additional
		//# Backbone
		// TYPE    C          6    12.01    1.9080    0.0860   0.0000	      sp2 C carbonyl group 
		public string TypeID;
		public int AtmoicNumber;
		public float AtomicMass;
		public float Radius;
		public float Epsilion;
		public float Charge;
		public string Comment;

		// now with addtional section
		// DAVE_REBUILD_ATOM_TYPES
		// required by the builder to decide which atoms to include
		// and also in various forcefield operations
		public int AtomBuilderLevel;

		// another addition
		public bool HBondAcceptor = false;
		public bool HBondHydrogen = false;

		private static AtomType m_NullType;
		public static AtomType NullType
		{
			get
			{
				if( m_NullType == null )
				{
					m_NullType = new AtomType();
					m_NullType.AtmoicNumber = 0;
					m_NullType.AtomicMass = 0.0f;
					m_NullType.Charge = 0.0f;
					m_NullType.Comment = "";
					m_NullType.Epsilion = 0.0f;
					m_NullType.Radius = 0.0f;
					m_NullType.TypeID = "  ";
					m_NullType.AtomBuilderLevel = -1; // i.e. dont try rebuild cos it cant be >= 0
				}
				return m_NullType;
			}
		}
	}
}
