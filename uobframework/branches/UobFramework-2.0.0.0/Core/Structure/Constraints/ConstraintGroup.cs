using System;
using System.Collections;
using System.IO;

using UoB.Core.Primitives.Collections;

namespace UoB.Core.Structure.Constraints
{
	/// <summary>
	/// Summary description for ConstraintGroup.
	/// </summary>
	public sealed class ConstraintGroup : ResetableTypedCollection
	{
		private int m_ID;

		public ConstraintGroup( int id, int initialLoadFactor ) : base( initialLoadFactor )
		{
			m_ID = id;
		}

		protected override void addNewInternal()
		{
			Constraint c = new Constraint( this );
			m_Objects.Add( c );
		}

		public new Constraint getNextItemForInit()
		{
			return (Constraint) base.getNextItemForInit();
		}

		public new Constraint this[ int index ]
		{
			get
			{
				return (Constraint) base[ index ];
			}
		}

		public int GroupID
		{
			get
			{
				return m_ID;
			}
		}

		public void WriteConstraints( StreamWriter rw )
		{
			for( int i = 0; i < CountTo; i++ )
			{
				Constraint c = (Constraint) base[i];
				c.WriteLines( rw );
			}
		}
	}
}
