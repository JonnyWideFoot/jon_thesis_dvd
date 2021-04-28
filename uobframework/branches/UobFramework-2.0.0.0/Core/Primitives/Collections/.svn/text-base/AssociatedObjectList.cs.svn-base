using System;
using System.Collections;

namespace UoB.Core.Primitives.Collections
{
	/// <summary>
	/// Summary description for AssociatedObjectList.
	/// </summary>
	public sealed class AssociatedObjectList
	{
		private ArrayList m_ObjList1;
		private ArrayList m_ObjList2;

		public AssociatedObjectList()
		{
			m_ObjList1 = new ArrayList();
            m_ObjList2 = new ArrayList();
		}

		public void Add( object obj1, object obj2 )
		{
			m_ObjList1.Add( obj1 );
			m_ObjList2.Add( obj2 );
		}

		public int Count
		{
			get
			{
				return m_ObjList1.Count;
			}
		}

		public void Clear()
		{
			m_ObjList1.Clear();
			m_ObjList2.Clear();
		}

		public void RemoveAt( int index )
		{
			m_ObjList1.RemoveAt( index );
			m_ObjList2.RemoveAt( index );
		}

		public object Get1( int index )
		{
			return m_ObjList1[index];
		}

		public object Get2( int index )
		{
			return m_ObjList2[index];
		}

		public bool Contains1( object o )
		{
			return m_ObjList1.Contains(o);
		}

		public bool Contains2( object o )
		{
			return m_ObjList2.Contains(o);
		}
	}
}
