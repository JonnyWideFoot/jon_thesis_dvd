using System;

namespace UoB.Core.Primitives
{
	/// <summary>
	/// Summary description for Vector.
	/// </summary>
	public class Vector : Position, ICloneable
	{
		public Vector() : base( 0, 0, 0 )
		{
		}

		public Vector( double x, double y, double z ) : base( x, y, z )
		{
		}

		public Vector(float[] pos) : base( pos )
		{
		}

		public Vector(int x, int y, int z) : base( x, y, z )
		{
		}

		public Vector(float x, float y, float z)  : base( x, y, z )
		{
		}

		public Vector( Position donor ) : base( donor )
		{
		}

		public static double dotProduct( Position vec1, Position vec2 )
		{
			return vec1.x * vec2.x + vec1.y * vec2.y + vec1.z * vec2.z;
		}

		public void SetToCrossProduct( Position vec1, Position vec2 )
		{
			m_X = vec1.y * vec2.z - vec1.z * vec2.y;
			m_Y	= vec1.z * vec2.x - vec1.x * vec2.z;
			m_Z	= vec1.x * vec2.y - vec1.y * vec2.x;
		}

		public static Vector crossProduct( Position vec1, Position vec2 )
		{

			Vector cp = new Vector(
				vec1.y * vec2.z - vec1.z * vec2.y,
				vec1.z * vec2.x - vec1.x * vec2.z,
				vec1.x * vec2.y - vec1.y * vec2.x );
			return cp;

			//			void Position::crossProduct(Position *v1, Position *v2){
			//			x = (v1->y*v2->z - v1->z*v2->y);
			//			y = (v1->z*v2->x - v1->x*v2->z);
			//			z = (v1->x*v2->y - v1->y*v2->x);

		}
		
		public void SetToUnitVectorOf( Position p )
		{
			m_X = p.x;
			m_Y = p.y;
			m_Z = p.z;
			MakeUnitVector();
		}

		public void SetToUnitVectorOf( Position source, Position destination )
		{
			SetToAMinusB(destination,source);
			MakeUnitVector();
		}

		public void MakeUnitVector( out double length )
		{
			length = Math.Sqrt( Math.Pow( m_X, 2 ) + Math.Pow( m_Y, 2 ) + Math.Pow( m_Z, 2 ) );
			if( length != 0.0 )
			{
				m_X = m_X / length;
				m_Y = m_Y / length;
				m_Z = m_Z / length;
			}
			else
			{
				m_X = 0.0;
				m_Y = 0.0;
				m_Z = 0.0;
			}
		}

		public void MakeUnitVector()
		{
			double length = Math.Sqrt( Math.Pow( m_X, 2 ) + Math.Pow( m_Y, 2 ) + Math.Pow( m_Z, 2 ) );
			if( length != 0.0 )
			{
				m_X = m_X / length;
				m_Y = m_Y / length;
				m_Z = m_Z / length;
			}
			else
			{
				m_X = 0.0;
				m_Y = 0.0;
				m_Z = 0.0;
			}
		}

		public static void MakeUnitVector( Vector v )
		{
			double length = Math.Sqrt( Math.Pow( v.x, 2 ) + Math.Pow( v.y, 2 ) + Math.Pow( v.z, 2 ) );
			if( length != 0.0 )
			{
				v.x = v.x / length;
				v.y = v.y / length;
				v.z = v.z / length;
			}
			else
			{
				v.x = 0.0;
				v.y = 0.0;
				v.z = 0.0;
			}
		}

		public static void MakeUnitVector( Vector v, out double length )
		{
			length = Math.Sqrt( Math.Pow( v.x, 2 ) + Math.Pow( v.y, 2 ) + Math.Pow( v.z, 2 ) );
			if( length != 0.0 )
			{
				v.x = v.x / length;
				v.y = v.y / length;
				v.z = v.z / length;
			}
			else
			{
				v.x = 0.0;
				v.y = 0.0;
				v.z = 0.0;
			}
		}

		public static Vector StaticInterpolate_2( Position v1, float p1, Position v2, float p2 )
		{
			Vector v = new Vector(
				v1.x * p1 + v2.x * p2,
				v1.y * p1 + v2.y * p2,
				v1.z * p1 + v2.z * p2);
			return v;
		}

		public static Vector StaticInterpolate_3( Position v1,float p1, Position v2, float p2, Position v3, float p3 )
		{
			Vector v = new Vector(
				v1.x * p1 + v2.x * p2 + v3.x * p3,
				v1.y * p1 + v2.y * p2 + v3.y * p3,
				v1.z * p1 + v2.z * p2 + v3.z * p3);
			return v;
		}
		public static Vector StaticInterpolate_4( Position v1, float p1, Position v2, float p2, Position v3, float p3, Position v4, float p4 )
		{
			Vector v = new Vector(
				v1.x * p1 + v2.x * p2 + v3.x * p3 + v4.x * p4,
				v1.y * p1 + v2.y * p2 + v3.y * p3 + v4.y * p4,
				v1.z * p1 + v2.z * p2 + v3.z * p3 + v4.z * p4);
			return v;
		}

		
		public void Interpolate_2( Position v1, float p1, Position v2, float p2 )
		{
			m_X = v1.x * p1 + v2.x * p2;
			m_Y = v1.y * p1 + v2.y * p2;
			m_Z = v1.z * p1 + v2.z * p2;
		}

		public void Interpolate_3( Position v1,float p1, Position v2, float p2, Position v3, float p3 )
		{
			m_X = v1.x * p1 + v2.x * p2 + v3.x * p3;
			m_Y = v1.y * p1 + v2.y * p2 + v3.y * p3;
			m_Z = v1.z * p1 + v2.z * p2 + v3.z * p3;
		}
		public void Interpolate_4( Position v1, float p1, Position v2, float p2, Position v3, float p3, Position v4, float p4 )
		{
			m_X = v1.x * p1 + v2.x * p2 + v3.x * p3 + v4.x * p4;
			m_Y = v1.y * p1 + v2.y * p2 + v3.y * p3 + v4.y * p4;
			m_Z = v1.z * p1 + v2.z * p2 + v3.z * p3 + v4.z * p4;
		}

		public double scalarProduct( Position v2 )
		{
			return m_X * v2.x + m_Y + v2.y + m_Z * v2.z;
		}

		
		public static Vector operator -( Vector v )
		{
			Vector tmp = new Vector(
				-v.x,
				-v.y,
				-v.z);
			return tmp;
		}

		public static Vector operator +( Vector v1, Vector v2 )
		{
			Vector tmp = new Vector(
				v1.x + v2.x,
				v1.y + v2.y,
				v1.z + v2.z);
			return tmp;
		}

		public static Vector operator *( Vector v1, float f )
		{
			Vector tmp = new Vector(
				v1.x * f,
				v1.y * f,
				v1.z * f);
			return tmp;
		}

		public static Vector operator *( Vector v1, Vector v2 )
		{
			Vector tmp = new Vector(
				v1.x * v2.x,
				v1.y * v2.y,
				v1.z * v2.z);
			return tmp;
		}

		public static Vector operator /( Vector v1, Vector v2 )
		{
			Vector tmp = new Vector(
				v1.x / v2.x,
				v1.y / v2.y,
				v1.z / v2.z);
			return tmp;
		}

		public static Vector operator /( Vector v1, float f )
		{
			Vector tmp = new Vector(
				v1.x / f,
				v1.y / f,
				v1.z / f);
			return tmp;
		}

		public static Vector operator -( Vector v1, Vector v2 )
		{
			Vector tmp = new Vector(
				v1.x - v2.x,
				v1.y - v2.y,
				v1.z - v2.z);
			return tmp;
		}

		public static Vector CenterPointBetween( Vector v1, Vector v2 )
		{
			return new Vector(
				( (v1.x + v2.x) / 2 ),
				( (v1.y + v2.y) / 2 ),
				( (v1.z + v2.z) / 2 )
				);
		}

		public override object Clone()
		{
			return new Vector( m_X, m_Y, m_Z );
		}

		public Vector CloneVector
		{
			get
			{
				return new Vector( m_X, m_Y, m_Z );
			}
		}
	}
}
