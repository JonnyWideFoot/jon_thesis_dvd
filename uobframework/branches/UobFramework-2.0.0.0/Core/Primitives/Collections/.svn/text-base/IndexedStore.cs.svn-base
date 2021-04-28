using System;
using System.Collections;

using UoB.Core;

namespace UoB.Core.Primitives.Collections
{
	/// <summary>
	/// Summary description for IndexedStore.
	/// </summary>
	public abstract class IndexedStore : IEnumerable
	{
		protected int m_Position = 0;
		protected int m_ArrayWidth = 0;
		protected ArrayList m_InternalArray;
		public event UpdateEvent IndexChanged;
		protected UpdateEvent linkIndexEvent;
		protected IndexedStore m_LinkedSystem = null;

		public IndexedStore( int arrayWidth )
		{
			m_ArrayWidth = arrayWidth;
			m_InternalArray = new ArrayList();

			IndexChanged = new UpdateEvent( nullFunc );
			linkIndexEvent = new UpdateEvent( linkTo_IndexChanged );
		}

		public void Clear()
		{
			Position = 0; // set to the start
            m_InternalArray.RemoveRange(1,m_InternalArray.Count-1);
		}

		public object[] this[int index] 
		{
			get 
			{
				return (object[]) m_InternalArray[index];
			}
		}

		private void nullFunc()
		{
		}

		public int Width
		{
			get
			{
				return m_ArrayWidth;
			}
		}
		
		protected void InternalIndexChangedCall()
		{
			IndexChanged();
		}

		public void linkIndex( PositionStore linkTo )
		{
			m_LinkedSystem = linkTo;
			linkTo.IndexChanged += linkIndexEvent;
		}

		private void linkTo_IndexChanged()
		{
			Position = m_LinkedSystem.Position;
		}

		private void unlinkIndex()
		{
			m_LinkedSystem.IndexChanged -= linkIndexEvent;
			m_LinkedSystem = null;
		}

		public int Position
		{
			get
			{
				return m_Position;
			}
			set
			{
				if( m_Position != value )
				{
					setPositionsTo(value);
				}
			}
		}

		public int Count
		{
			get
			{
				return m_InternalArray.Count;
			}
		}

		public virtual void setPositionsTo( int position )
		{
			if ( position < 0 || position > m_InternalArray.Count -1 )
			{
				return;
			}
			m_Position = position;
			IndexChanged();
		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new PosEnumerator(this);
		}

		private class PosEnumerator : IEnumerator
		{
			private int position = -1;
			private IndexedStore m_Owner;

			public PosEnumerator( IndexedStore theOwner )
			{
				m_Owner = theOwner;
			}

			// Declare the MoveNext method required by IEnumerator:
			public bool MoveNext()
			{
				if (position < m_Owner.Count - 1)
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
					return m_Owner[ position ];
				}
			}
		}

		#endregion
	}
}
