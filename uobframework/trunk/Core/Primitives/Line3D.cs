using System;

namespace UoB.Core.Primitives
{
	/// <summary>
	/// Summary description for Line3D.
	/// </summary>
	public class Line3D
	{
		private Position m_Point1;
		private Position m_Point2;
		private Colour m_Colour;

		public Line3D( Position p1, Position p2, Colour c )
		{
			m_Point1 = p1;
			m_Point2 = p2;
			m_Colour = c;
		}

		public Line3D( float[] initArray )
		{
			m_Point1 = new Position( initArray[0], initArray[1], initArray[2] );
			m_Point2 = new Position( initArray[3], initArray[4], initArray[5] );
			m_Colour = new Colour  ( initArray[6], initArray[7], initArray[8] );
		}

		public Line3D( float[] initArray, int colourID )
		{
			m_Point1 = new Position( initArray[0], initArray[1], initArray[2] );
			m_Point2 = new Position( initArray[3], initArray[4], initArray[5] );
			m_Colour = Colour.FromIntID( colourID );
		}

		public Position point1
		{
			get
			{
				return m_Point1;
			}
		}

		public Position point2
		{
			get
			{
				return m_Point2;
			}
		}

		public Colour colour
		{
			get
			{
				return m_Colour;
			}
			set
			{
				m_Colour = value;
			}
		}
	}
}
