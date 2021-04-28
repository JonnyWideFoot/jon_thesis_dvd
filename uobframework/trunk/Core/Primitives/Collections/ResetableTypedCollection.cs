using System;
using System.Collections;

namespace UoB.Core.Primitives.Collections
{
	/// <summary>
	/// Summary description for TypedResetCollection.
	/// </summary>
	public abstract class ResetableTypedCollection
	{
		protected ArrayList m_Objects;
		private int m_CountTo = 0;

		public ResetableTypedCollection( int initialLoadFactor )
		{
			m_Objects = new ArrayList( initialLoadFactor );
			initialise( initialLoadFactor );
		}

		protected void initialise( int toLoad )
		{
			for( int i = 0; i < toLoad; i++ )
			{
				addNewInternal();
			}
		}

		protected abstract void addNewInternal();

		public object getNextItemForInit()
		{
			if( m_CountTo == m_Objects.Count )
			{
				addNewInternal();
			}
			m_CountTo++;
			return m_Objects[m_CountTo-1];			
		}

		public void Reset()
		{
			m_CountTo = 0;
		}

		public int CountTo
		{
			get
			{
				return m_CountTo;
			}
		}

		public object this[ int i ]
		{
			get
			{
				return m_Objects[i];
			}
		}
	}
}
