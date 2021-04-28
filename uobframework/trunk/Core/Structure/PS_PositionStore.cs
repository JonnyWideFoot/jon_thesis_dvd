using System;
using System.Collections;
using System.Diagnostics;

using UoB.Core.Primitives;
using UoB.Core.Primitives.Collections;

namespace UoB.Core.Structure
{
	/// <summary>
	/// Summary description for ParticleSystemPS_PositionStore.
	/// </summary>

	public class PS_PositionStore : PositionStore
	{
		private bool m_SetPSPositionsOnUpdate = true;
		private ParticleSystem m_ParticleSystem;

		public PS_PositionStore( ParticleSystem ps ) : base( ps.Count )
		{
			m_ParticleSystem = ps;
			m_Position = 0; // the origainl PS Positions

			bool done = false;
			while( !done )
			{  
				try
				{
					m_ParticleSystem.AcquireReaderLock(1000);
					try
					{
						// It is safe for this thread to read from
						// the shared resource.

						Position[] positions = new Position[m_ParticleSystem.Count];
						for ( int i = 0; i < m_ParticleSystem.Count; i++ )
						{
							positions[i] = new Position( m_ParticleSystem[i] );
						}
						m_InternalArray.Add(positions);

						done = true;
					}        
					finally
					{
						// Ensure that the lock is released.
						m_ParticleSystem.ReleaseReaderLock();
					}
				}
				catch (ApplicationException)
				{
					// The reader lock request timed out.
					// But done hasnt been set to true, so we can have another go ...
				}
			}
		}

		public ParticleSystem particleSystem
		{
			get
			{
				return m_ParticleSystem;
			}
		}

		public override void setPositionsTo( int position )
		{
			if ( position < 0 || position > m_InternalArray.Count -1 )
			{
                throw new ArgumentOutOfRangeException("setPositionsTo() call not within array bounds");
			}

			m_Position = position;
			InternalIndexChangedCall(); // update any watching arrays first before setting the PS, which results in a content update and therefore any associated rendering
			
			Position[] positions = (Position[]) m_InternalArray[position];
			m_ParticleSystem.SetPositions( positions ); // ps utilises the writer lock
		
		}

		public void addPositionArray( Position[] addPositionArray, bool setPositionsFlag )
		{
			if ( addPositionArray.Length != m_ArrayWidth )
			{
				throw new Exception("ERROR : Position update ignored by PS_PositionStore manager, Position length did not match ParticleSystem length!");
			}
			else
			{
				m_InternalArray.Add( addPositionArray );
			}
			if ( m_SetPSPositionsOnUpdate && setPositionsFlag )
			{
				setPositionsTo(m_InternalArray.Count);
			}
		}
	}
}
