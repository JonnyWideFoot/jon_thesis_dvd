using System;
using System.Collections;

using UoB.Core.Primitives;
using UoB.Core.Structure.Primitives;

namespace UoB.Core.Structure
{
	/// <summary>
	/// Summary description for Atom.
	/// </summary>
	public class Atom : Position, ICloneable
	{
		private string m_PDBIdentifier;
		private int m_ArrayIndex;
		private int m_AtomNumber;
		private AtomPrimitiveBase m_AtomPrimitive;
		private Molecule m_ParentMolecule;
		private float m_Occupancy;
		private float m_TempFactor;

		private BondList m_BondList;
		private AtomList m_BondedAtoms;

		public Atom( string name, int arrayIndex, int atomNumber, Molecule parentMolecule, double x, double y, double z, 
			float occupancy, float tempFactor
			) : base(x,y,z)
		{
			m_PDBIdentifier = name;

			m_ArrayIndex = arrayIndex; // unique integer identifier for this atom in the PS
			m_AtomNumber = atomNumber;
			m_ParentMolecule = parentMolecule;

			m_Occupancy = occupancy;
			m_TempFactor = tempFactor;
			
			m_BondList = new BondList(4);
			m_BondedAtoms = new AtomList(4);
		}

		public Atom( string name, int arrayIndex, int atomNumber, Molecule parentMolecule, double x, double y, double z ) : base(x,y,z)
		{
			m_PDBIdentifier = name;

			m_ArrayIndex = arrayIndex; // unique integer identifier for this atom in the PS
			m_AtomNumber = atomNumber;
			m_ParentMolecule = parentMolecule;

			m_Occupancy = -1.0f;
			m_TempFactor = -1.0f;
			
			m_BondList = new BondList(4);
			m_BondedAtoms = new AtomList(4);
		}

		public bool isSolvent
		{
			get
			{
				if( parentMolecule != null )
				{
					return parentMolecule.IsSolvent;
				}
				else
				{
					return false;
				}
			}
		}

		public int ArrayIndex
		{
			get
			{
				return m_ArrayIndex;
			}
			set
			{
				m_ArrayIndex = value;
			}
		}     

		public string PDBType
		{
			get
			{
				return m_PDBIdentifier;
			}
		}

		public string ALTType
		{
			get
			{
				return m_AtomPrimitive.AltName;
			}
		}

		public int bondCount
		{
			get
			{
				return m_BondList.Count;
			}
		}

		public void ClearBondList()
		{
			m_BondedAtoms.Clear();
			m_BondList.Clear();
		}

		public static float MeasureTorsion(Atom a1, Atom a2, Atom a3, Atom a4)
		{
			/* Calculate the vectors C,B,C                                       */
			float xij = a1.xFloat - a2.xFloat;
			float yij = a1.yFloat - a2.yFloat;
			float zij = a1.zFloat - a2.zFloat;
			float xkj = a3.xFloat - a2.xFloat;
			float ykj = a3.yFloat - a2.yFloat;
			float zkj = a3.zFloat - a2.zFloat;
			float xkl = a3.xFloat - a4.xFloat;
			float ykl = a3.yFloat - a4.yFloat;
			float zkl = a3.zFloat - a4.zFloat;

			/* Calculate the normals to the two planes n1 and n2
			   this is given as the cross products:
				AB x BC
			   --------- = n1
			   |AB x BC|

				BC x CD
			   --------- = n2
			   |BC x CD|
			*/
			float dxi = yij * zkj - zij * ykj;     /* Normal to plane 1                */
			float dyi = zij * xkj - xij * zkj;
			float dzi = xij * ykj - yij * xkj;
			float gxi = zkj * ykl - ykj * zkl;     /* Mormal to plane 2                */
			float gyi = xkj * zkl - zkj * xkl;
			float gzi = ykj * xkl - xkj * ykl;

			/* Calculate the length of the two normals                           */
			float bi = dxi * dxi + dyi * dyi + dzi * dzi;
			float bk = gxi * gxi + gyi * gyi + gzi * gzi;
			float ct = dxi * gxi + dyi * gyi + dzi * gzi;

			float boi2 = 1.0f / bi;
			float boj2 = 1.0f / bk;
			bi   = (float) Math.Sqrt( (double)bi );
			bk   = (float) Math.Sqrt( (double)bk );

			float z1   = 1.0f / bi;
			float z2   = 1.0f / bk;
			float bioj = bi * z2;
			float bjoi = bk * z1;
			ct   = ct * z1 * z2;
			if (ct >  1.0)   ct = 1.0f;
			if (ct < (-1.0)) ct = -1.0f;
			float ap = (float) Math.Acos( ct );

			float 	s = xkj * (dzi * gyi - dyi * gzi)
				+ ykj * (dxi * gzi - dzi * gxi)
				+ zkj * (dyi * gxi - dxi * gyi);

			if (s < 0.0) ap = -ap;

			ap = (ap > 0.0) ? (float)Math.PI - ap : -( (float)Math.PI + ap );

			return( ap * 180 / (float)Math.PI );
		}

		public void bondTo(Atom theAtom)
		{	
			bondTo( theAtom, 1.0f, BondType.Covalent );
		}
		public void bondTo(Atom theAtom, float bondOrder )
		{	
			bondTo( theAtom, bondOrder, BondType.Covalent );
		}
		public void bondTo(Atom theAtom, BondType type )
		{	
			bondTo( theAtom, 1.0f, type );
		}
		public void bondTo( Atom theAtom, float bondOrder, BondType type )
		{
			if(theAtom != null)
			{
				Bond theBond = new Bond(this, theAtom, bondOrder, type);
				m_BondedAtoms.addAtom( theAtom );
				m_BondList.addBond( theBond );
			}
		}

		public bool bondedTo(Atom theAtom)
		{
			return m_BondedAtoms.Contains( theAtom );
		}

		public BondList Bonds
		{
			get
			{
				return m_BondList;
			}
		}

		public AtomList BondedAtoms
		{
			get
			{
				return m_BondedAtoms;
			}
		}

		//Accessors

		public AtomPrimitiveBase atomPrimitive
		{
			get 
			{ 
				return m_AtomPrimitive; 
			}
			set
			{
				m_AtomPrimitive = value;
			}
		}

		public int AtomNumber
		{
			set 
			{
				m_AtomNumber = value; // required in builder operations
			}
			get 
			{ 
				return m_AtomNumber; 
			}
		}

		public string DebugString
		{
			get
			{
				return UoB.Core.FileIO.PDB.PDB.MakePDBStringFromAtom( this );
			}
		}

		public float Occupancy
		{
			get
			{ 
				return m_Occupancy;
			}
		}

		public Molecule parentMolecule
		{
			get
			{
				return m_ParentMolecule;
			}
			set
			{
				m_ParentMolecule = value;
			}
		}

		public float TempFactor
		{
			get
			{
				return m_TempFactor;
			}
		}

		public float Radius
		{
			get
			{
				if( m_AtomPrimitive != null )
				{
					return m_AtomPrimitive.Radius;
				}
				else
				{
					return -1.0f;
				}
			}
		}

		public override string ToString()
		{
			return m_AtomPrimitive.PDBIdentifier + " " + m_AtomNumber.ToString();
		}

		#region ICloneable Members

		public object CloneShallow()
		{
			return (Atom)MemberwiseClone();
		}

		public override object Clone()
		{
			Atom a = new Atom( m_PDBIdentifier, m_ArrayIndex, m_AtomNumber, null, m_X, m_Y, m_Z, m_Occupancy, m_TempFactor );
			a.atomPrimitive = atomPrimitive;
			return a;
		}

		#endregion
	}
}
