using System;
using System.Collections;
using System.Diagnostics;

using UoB.Core.Primitives;

namespace UoB.Core.Primitives.Collections
{
	/// <summary>
	/// Summary description for ParticleSystem PS_PositionStore.
	/// </summary>

	public class PositionStore : IndexedStore
	{

		public PositionStore( int arrayLength ) : base( arrayLength )
		{
		}

		public new Position[] this[int index] 
		{
			get 
			{
				return (Position[]) m_InternalArray[index];
			}
		}

		public Position[] GetClonePositionArray( int index )
		{
			if( index >= m_InternalArray.Count )
			{
				throw new Exception("Requested index was out of bounds");
			}
			else
			{
				Position[] source = (Position[]) m_InternalArray[index];
				Position[] destination = new Position[ source.Length ];
				for( int i = 0; i < source.Length; i++ )
				{
					destination[i] = new Position( source[i] );
				}
				return destination;
			}
		}

		public Position[] GetPositionArray( int index )
		{
			if( index >= m_InternalArray.Count )
			{
				throw new Exception("Requested index was out of bounds");
			}
			else
			{
				return (Position[]) m_InternalArray[index];
			}
		}

		public virtual void AddPositionArray( Position[] addPositionArray )
		{
			if ( addPositionArray.Length != m_ArrayWidth )
			{
				Trace.WriteLine("ERROR : Position update ignored by PS_PositionStore manager, Position length did not match ParticleSystem length!");
				return;
			}
			else
			{
				m_InternalArray.Add( addPositionArray );
			}
		}

		public Position[] currentPositionArray
		{
			get
			{
				return (Position[]) m_InternalArray[m_Position];
			}
		}
	}
}
