using System;
using System.Collections;

namespace UoB.Core.Structure.Primitives
{
	/// <summary>
	/// Summary description for Line3DList.
	/// </summary>
	public sealed class MoleculePrimitiveList : IEnumerable
	{
		private ArrayList m_Primitives;

		public MoleculePrimitiveList()
		{
			m_Primitives = new ArrayList();
		}

		public void addPrimitive( MoleculePrimitive molPrim )
		{
			m_Primitives.Add( molPrim );
		}

		public MoleculePrimitive getPrimitive( string ID )
		{
			foreach ( MoleculePrimitive mp in m_Primitives )
			{
				if ( mp.MolName == ID )
				{
					return mp;
				}
			}
			return null;
		}

		public int Count
		{
			get
			{
				return m_Primitives.Count;
			}
		}

		public MoleculePrimitive this[int index] 
		{
			get 
			{
				return (MoleculePrimitive) m_Primitives[index];
			}
			set 
			{
				m_Primitives[index] = value;
			}
		}		
		
		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new MoleculePrimitiveListEnumerator(this);
		}

		private class MoleculePrimitiveListEnumerator : IEnumerator
		{
			private int position = -1;
			private MoleculePrimitiveList ownerLL;

			public MoleculePrimitiveListEnumerator(MoleculePrimitiveList theLL)
			{
				ownerLL = theLL;
			}

			// Declare the MoveNext method required by IEnumerator:
			public bool MoveNext()
			{
				if (position < ownerLL.Count - 1)
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
					return ownerLL[position];
				}
			}
		}

		#endregion
	}
}
