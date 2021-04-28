using System;
using System.Collections;
using System.Data;
using System.IO;
using UoB.Core.Primitives;

namespace UoB.Core.Structure
{
	/// <summary>
	/// Summary description for Bonding.
	/// </summary>
	/// 

	public class Bond
	{
		private Atom m_Atom;
		private Atom m_AdjoiningAtom;
		private float m_Order;
		private BondType m_Type;

		public Atom sourceAtom
		{
			get 
			{
				return m_Atom;
			}
		}

		public override string ToString()
		{
			return "Bond between : " + m_Atom.ToString() + " and " + m_AdjoiningAtom.ToString();
		}

		public BondType bondType
		{
			get
			{
				return m_Type;
			}
		}

		public Atom farAtom
		{
			get 
			{
				return m_AdjoiningAtom;
			}
		}

		public Bond(Atom thisAtom, Atom atom2, float order, BondType type)
		{
			m_Type = type;
			m_Order = order;
			m_Atom = thisAtom;
			m_AdjoiningAtom = atom2;
		}

		public double length
		{
			get 
			{	
				double X = (m_Atom.xFloat - m_AdjoiningAtom.xFloat);
				double Y = (m_Atom.yFloat - m_AdjoiningAtom.yFloat);
				double Z = (m_Atom.zFloat - m_AdjoiningAtom.zFloat);
				return Math.Sqrt( (X * X) + (Y * Y) + (Z * Z) );
			}
		}
	}
}
