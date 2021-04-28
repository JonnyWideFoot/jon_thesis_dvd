using System;
using System.Collections;
using System.Diagnostics;

using UoB.Core.Primitives;
using UoB.Core.FileIO.PDB;
using UoB.Core.Structure;
using UoB.Core.ForceField.Definitions;

namespace UoB.Core.Structure.Primitives
{
	/// <summary>
	/// Summary description for MoleculePrimitive.
	/// </summary>
	public class MoleculePrimitiveBase : IComparer, IEnumerable
	{
		protected bool m_IsSolvent = false;
		protected char m_MoleculeSingleLetterID;
		protected string m_MoleculeName; // can be of length 3 or 4 ... depending on the definition in the forcefield file
		
		public MoleculePrimitiveBase( string moleculeName )
		{
			m_MoleculeName = moleculeName;
			m_MoleculeSingleLetterID = '-';
		}

		public MoleculePrimitiveBase( string moleculeName, bool isSolvent )
		{
			m_IsSolvent = isSolvent;
			m_MoleculeName = moleculeName;
			m_MoleculeSingleLetterID = '?';
		}

		public char SingleLetterID
		{
			get
			{
				return m_MoleculeSingleLetterID;
			}
		}

		public string MolName
		{
			get
			{
				return m_MoleculeName;
			}
		}

		public bool IsSolvent
		{
			get
			{
				return m_IsSolvent;
			}
		}

		public virtual int AtomPrimitiveCount
		{
			get
			{
				return 0;
			}
		}

		public virtual AtomPrimitiveBase GetAtomPrimitiveFromAltID( string AtomID )
		{
			return new AtomPrimitiveBase( AtomID );
		}

		public virtual AtomPrimitiveBase GetAtomPrimitiveFromPDBID( string atomPDBName )
		{
			return new AtomPrimitiveBase( atomPDBName );
		}

		public virtual int TorsionDefinitionCount
		{
			get
			{
				return 0;
			}
		}

		public virtual bool ContainsAtomWithAltID( string atomID )
		{
			return false;
		}

		public virtual bool ContainsAtomWithPDBID( string atomID )
		{
			return false;
		}

		public virtual Torsion[] Torsions
		{
			get
			{
				return new Torsion[0];
			}
		}

		#region IComparer Members

		public virtual int Compare(object x, object y)
		{
			return 0; // we dont give a damn, you cant sort an undefined molecule
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new MoleculePrimitiveEnumerator(this);
		}

		private class MoleculePrimitiveEnumerator : IEnumerator
		{
			private int position = -1;
			private MoleculePrimitiveBase ownerLL;

			public MoleculePrimitiveEnumerator(MoleculePrimitiveBase theLL)
			{
				ownerLL = theLL;
			}

			// Declare the MoveNext method required by IEnumerator:
			public bool MoveNext()
			{
				if (position < ownerLL.AtomPrimitiveCount - 1)
				{
					position++;
					return true;
				}
				else
				{
					return false;
				}
			}

			// Declare the Reset method required by IEnumerator:
			public void Reset()
			{
				position = -1;
			}

			// Declare the Current property required by IEnumerator:
			public object Current
			{
				get
				{
					// problem alert !!!! - we cant index by integers
					// if you need this see 
					// http://www.thecodeproject.com/csharp/hashlistarticle.asp
					return null; // ownerLL[position];
				}
			}
		}

		#endregion
	}
}
