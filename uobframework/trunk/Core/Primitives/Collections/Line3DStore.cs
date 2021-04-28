using System;
using System.Collections;
using System.Diagnostics;

using UoB.Core.Structure;
using UoB.Core.Primitives;

namespace UoB.Core.Primitives.Collections
{
	/// <summary>
	/// Summary description for ParticleSystemPS_PositionStore.
	/// </summary>

	public class Line3DStore : IndexedStore
	{

		public Line3DStore( int arrayLength ) : base( arrayLength )
		{
		}

		public new Line3D[] this[int index] 
		{
			get 
			{
				return (Line3D[]) m_InternalArray[index];
			}
		}

		public virtual void addLine3DArray( Line3D[] addLine3DArray )
		{
			if ( addLine3DArray.Length != m_ArrayWidth )
			{
				Trace.WriteLine("ERROR : Line3D update ignored by Line3DStore manager, Line3D length did not match internal width!");
				return;
			}
			else
			{
				m_InternalArray.Add( addLine3DArray );
			}
		}

		public Line3D[] currentLine3DArray
		{
			get
			{
				return (Line3D[]) m_InternalArray[m_Position];
			}
		}
	}
}
