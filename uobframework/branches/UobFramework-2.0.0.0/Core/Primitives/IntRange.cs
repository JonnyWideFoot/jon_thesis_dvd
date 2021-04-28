using System;

namespace UoB.Core.Primitives
{
	public delegate void RangeChange( object sender, IntRange_EventFire range );

	public class IntRange_EventFire : IntRangeBase
	{
		public event RangeChange RangeUpdated;

		public IntRange_EventFire( int rangeStart, int rangeLength, int globalStart, int globalLength )
			: base( rangeStart, rangeLength, globalStart, globalLength )
		{
		}

		public IntRange_EventFire() : base()
		{
		}

		public void SetRange( object functionCaller, int rangeStart, int rangeLength )
		{
			m_RangeStart = rangeStart;
			m_RangeLength = rangeLength;
			if( !Validate() )
			{
				throw new ArgumentException("Range is invalid");
			}
			if( RangeUpdated != null )
			{
				RangeUpdated(functionCaller,this);
			}
		}

		public void SetRange( object functionCaller, int rangeStart, int rangeLength, int globalStart, int globalLength )
		{
			m_GlobalStart = globalStart;
			m_GlobalLength = globalLength;
			m_RangeStart = rangeStart;
			m_RangeLength = rangeLength;
			if( !Validate() )
			{
				throw new ArgumentException("Range is invalid");
			}
			if( RangeUpdated != null )
			{
				RangeUpdated(functionCaller,this);
			}
		}

		public void SetRangeToFull( object functionCaller )
		{
			m_RangeStart = m_GlobalStart;
			m_RangeLength = m_GlobalLength;
			if( RangeUpdated != null )
			{
				RangeUpdated(functionCaller,this);
			}
		}
	}

	public class IntRange : IntRangeBase
	{
		public IntRange( int rangeStart, int rangeLength, int globalStart, int globalLength )
			: base( rangeStart, rangeLength, globalStart, globalLength )
		{
		}

		public IntRange() : base()
		{
		}

		public void SetRange( int rangeStart, int rangeLength )
		{
			m_RangeStart = rangeStart;
			m_RangeLength = rangeLength;
			if( !Validate() )
			{
				throw new ArgumentException("Range is invalid");
			}
		}

		public void SetRange( int rangeStart, int rangeLength, int globalStart, int globalLength )
		{
			m_GlobalStart = globalStart;
			m_GlobalLength = globalLength;
			m_RangeStart = rangeStart;
			m_RangeLength = rangeLength;
			if( !Validate() )
			{
				throw new ArgumentException("Range is invalid");
			}
		}

		public void SetRangeToFull()
		{
			m_RangeStart = m_GlobalStart;
			m_RangeLength = m_GlobalLength;
		}
	}

	public abstract class IntRangeBase
	{
		protected int m_GlobalStart;
		protected int m_GlobalLength;
		protected int m_RangeStart;
		protected int m_RangeLength;

		private int m_MinimumLength = 1;

		public IntRangeBase( int rangeStart, int rangeLength, int globalStart, int globalLength )
		{
			m_GlobalStart = globalStart;
			m_GlobalLength = globalLength;
			m_RangeStart = rangeStart;
			m_RangeLength = rangeLength;
			if( !Validate() )
			{
				throw new ArgumentException("Range is invalid");
			}
		}

		public IntRangeBase()
		{
			m_GlobalStart = 0;
			m_GlobalLength = 1;
			m_RangeStart = 0;
			m_RangeLength = 1;
		}
		
		protected bool Validate()
		{
			if( m_GlobalLength < m_MinimumLength || 
				m_RangeStart < m_GlobalStart || 
				m_RangeLength < m_MinimumLength || 
				( m_RangeStart + m_RangeLength ) > ( m_GlobalStart + m_GlobalLength ) 
				)
			{
				return false;
			}
			else
			{
				return true;
			}				
		}

		public int GlobalStart
		{
			get
			{
				return m_GlobalStart;
			}
		}

		public int GlobalEnd
		{
			get
			{
				return m_GlobalStart + m_GlobalLength - 1;
			}
		}

		public int GlobalLength
		{
			get
			{
				return m_GlobalLength;
			}
		}

		public int RangeStart
		{
			get
			{
				return m_RangeStart;
			}
		}

		public int RangeLength
		{
			get
			{
				return m_RangeLength;
			}
		}

		public int RangeEnd
		{
			get
			{
				return m_RangeStart + m_RangeLength - 1;
			}
		}
	}
}
