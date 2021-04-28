using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using UoB.Core.Structure;
using UoB.Core.Primitives.Matrix;

namespace UoB.Core.Primitives
{
	/// <summary>
	/// Summary description for Position.
	/// </summary>
	public class Position
	{
		protected double m_X;
		protected double m_Y;
		protected double m_Z;
		protected static Regex m_Regex = new Regex(@"\s+",RegexOptions.Compiled); // scans whitespace, used for the text splitting function

		public Position()
		{
			m_X = 0.0;
			m_Y = 0.0;
			m_Z = 0.0;
		}

		public Position( string parseLine )
		{
			parseLine = parseLine.Trim();
			string[] parts = m_Regex.Split(parseLine);
			if( parts.Length != 3 ) throw new Exception("Parse line was not in the correct format");
			try
			{
				m_X = double.Parse( parts[0] );
				m_Y = double.Parse( parts[1] );
				m_Z = double.Parse( parts[2] );
			}
			catch
			{
				throw new Exception("Not all elements in the Parse line were floats.");
			}
		}

		public Position( double x, double y, double z )
		{
			m_X = x;
			m_Y = y;
			m_Z = z;
		}

		public Position(int X, int Y, int Z)
		{
			m_X = (float)X;
			m_Y = (float)Y;
			m_Z = (float)Z;
		}

		public Position(float X, float Y, float Z)
		{
			m_X = X;
			m_Y = Y;
			m_Z = Z;
		}

		public Position(float[] position)
		{
			if( position.Length != 3 )
			{
				throw new Exception( "Position initialisation can only be performed with a float array of length 3." );
			}
			m_X = position[0];
			m_Y = position[1];
			m_Z = position[2];
		}

		public Position( Position donor )
		{
			m_X = donor.x;
			m_Y = donor.y;
			m_Z = donor.z;
		}

		public void setToZeros()
		{
			m_X = 0.0;
			m_Y = 0.0;
			m_Z = 0.0;
		}

		public void setToMulMat( Matrix3x3 mmat )
		{
			double nx,ny,nz;
			nx = mmat.r[0,0]*x + mmat.r[0,1]*y + mmat.r[0,2]*z;
			ny = mmat.r[1,0]*x + mmat.r[1,1]*y + mmat.r[1,2]*z;
			nz = mmat.r[2,0]*x + mmat.r[2,1]*y + mmat.r[2,2]*z;
			x = nx;
			y = ny;
			z = nz;
		}

		public Position ClonePosition
		{
			get
			{
				return new Position(m_X,m_Y,m_Z);
			}
		}

		public void SetToCentrePoint( Position p1, Position p2 )
		{
			m_X = (p1.x + p2.x) / 2;
			m_Y = (p1.y + p2.y) / 2;
			m_Z = (p1.z + p2.z) / 2;
		}

		public void SetToAMinusB( Position p1, Position p2 )
		{
			m_X = p1.m_X - p2.m_X;
			m_Y = p1.m_Y - p2.m_Y;
			m_Z = p1.m_Z - p2.m_Z;
		}

		public virtual object Clone()
		{
			return new Position( m_X, m_Y, m_Z );
		}

		public static Position CenterPointBetween( Position v1, Position v2 )
		{
			return new Position(
				( (v1.x + v2.x) / 2 ),
				( (v1.y + v2.y) / 2 ),
				( (v1.z + v2.z) / 2 )
				);
		}

		public void setTo( double x, double y, double z )
		{
			m_X = x;
			m_Y = y;
			m_Z = z;
		}

		public void setTo( Position v1 )
		{
			m_X = v1.x;
			m_Y = v1.y;
			m_Z = v1.z;
		}

		public double angleWith( Position v2 )
		{
			double scalar =  (m_X * v2.x + m_Y + v2.y + m_Z * v2.z ) / (Length * v2.Length);  //simple scalar product calculation
			if(scalar>1)return 0;
			if(scalar<-1)return Math.PI;
			return Math.Acos(scalar);
		}

		public double x
		{
			get
			{
				return m_X;
			}
			set
			{
				m_X = value;
			}
		}

		public double y
		{
			get
			{
				return m_Y;
			}
			set
			{
				m_Y = value;
			}
		}

		public double z
		{
			get
			{
				return m_Z;
			}
			set
			{
				m_Z = value;
			}
		}

		
		public float xFloat
		{
			get
			{
				return (float)m_X;
			}
		}

		public float yFloat
		{
			get
			{
				return (float)m_Y;
			}
		}

		public float zFloat
		{
			get
			{
				return (float)m_Z;
			}
		}

		public static Position operator -( Position v )
		{
			Position tmp = new Position(
			-v.x,
			-v.y,
			-v.z);
			return tmp;
		}

		public static Position operator +( Position v1, Position v2 )
		{
			Position tmp = new Position(
			v1.x + v2.x,
			v1.y + v2.y,
			v1.z + v2.z);
			return tmp;
		}

		public static Position operator *( Position v1, float f )
		{
			Position tmp = new Position(
			v1.x * f,
			v1.y * f,
			v1.z * f);
			return tmp;
		}

		public static Position operator *( Position v1, Position v2 )
		{
			Position tmp = new Position(
			v1.x * v2.x,
			v1.y * v2.y,
			v1.z * v2.z);
			return tmp;
		}

		public static Position operator /( Position v1, Position v2 )
		{
			Position tmp = new Position(
			v1.x / v2.x,
			v1.y / v2.y,
			v1.z / v2.z);
			return tmp;
		}

		public static Position operator /( Position v1, float f )
		{
			Position tmp = new Position(
			v1.x / f,
			v1.y / f,
			v1.z / f);
			return tmp;
		}

		public static Position operator -( Position v1, Position v2 )
		{
			Position tmp = new Position(
			v1.x - v2.x,
			v1.y - v2.y,
			v1.z - v2.z);
			return tmp;
		}

		public void Add( Position v )
		{
			m_X += v.x;
			m_Y += v.y;
			m_Z += v.z;
		}

		public void Minus( Position v )
		{
			m_X -= v.x;
			m_Y -= v.y;
			m_Z -= v.z;
		}

		public void Divide( Position v )
		{
			m_X /= v.x;
			m_Y /= v.y;
			m_Z /= v.z;
		}

		public void Multiply( Position v )
		{
			m_X *= v.x;
			m_Y *= v.y;
			m_Z *= v.z;
		}

		public void Divide( double d )
		{
			m_X /= d;
			m_Y /= d;
			m_Z /= d;
		}

		public void Multiply( double d )
		{
			m_X *= d;
			m_Y *= d;
			m_Z *= d;
		}

		public static double length ( Position v )
		{
           return Math.Sqrt( Math.Pow( v.x, 2 ) + Math.Pow( v.y, 2 ) + Math.Pow( v.z, 2 ) );
		}

		public double Length
		{
			get
			{
				return Math.Sqrt( Math.Pow( m_X, 2 ) + Math.Pow( m_Y, 2 ) + Math.Pow( m_Z, 2 ) );
			}
		}

		public static double AngleBetweenUnitVectors( Position v1, Position v2 )
		{
			double dotProduct = v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
			if( dotProduct > 1.000 ) dotProduct = 1.0;
			return Math.Acos( dotProduct );
		}

		public static double AngleBetween( Position v1, Position v2 )
		{
			double length1 = Math.Sqrt( Math.Pow( v1.x, 2 ) + Math.Pow( v1.y, 2 ) + Math.Pow( v1.z, 2 ) );
			double length2 = Math.Sqrt( Math.Pow( v2.x, 2 ) + Math.Pow( v2.y, 2 ) + Math.Pow( v2.z, 2 ) );

			double x1 = v1.x / length1;
			double y1 = v1.y / length1;
			double z1 = v1.z / length1;
			double x2 = v2.x / length2;
			double y2 = v2.y / length2;
			double z2 = v2.z / length2;
			double dotProduct = x1 * x2 + y1 * y2 + z1 * z2;

			if( dotProduct > 1 ) dotProduct = 1.0;
			return Math.Acos( dotProduct );
		}

		const float oneEightyOverPi = (float)(180.0 / Math.PI);
		public static double AngleBetween2( Position v1, Position v2 )
		{
			double scalar = (v1.x * v2.x + v1.y * v2.y + v1.z * v2.z) / 
				 ( Math.Sqrt(v1.x * v1.x + v1.y * v1.y + v1.z * v1.z) 
				 * Math.Sqrt(v2.x * v2.x + v2.y * v2.y + v2.z * v2.z)
				);
			if        (scalar > 1)
				return 0.0;
			if        (scalar < -1)
				return (float)Math.PI;
			return Math.Acos( scalar );
		}

		public static double AngleBetween_Degrees( Position v1, Position v2 )
		{
			double scalar = (v1.x * v2.x + v1.y * v2.y + v1.z * v2.z) / 
				( Math.Sqrt(v1.x * v1.x + v1.y * v1.y + v1.z * v1.z) 
				* Math.Sqrt(v2.x * v2.x + v2.y * v2.y + v2.z * v2.z)
				);
			if        (scalar > 1)
				return 0.0;
			if        (scalar < -1)
				return 180.0f;
			return Math.Acos( scalar ) * oneEightyOverPi;
		}
		
		public static double AngleBetween( double dotProduct, double length1, double length2 )
		{
			return Math.Acos( dotProduct / ( length1 * length2 ) );
		}

		public override string ToString()
		{
			return xFloat.ToString() + " " + yFloat.ToString() + " " + zFloat.ToString();
		}

		public float distanceTo(Position thePoint)
		{
			return Position.distanceBetween(this, thePoint);
		}

		public float distanceSquaredTo(Position thePoint)
		{
			return Position.distanceSquaredBetween(this, thePoint);
		}

		public static float distanceSquaredBetween(Position point1, Position point2)
		{
			double X = (point1.x - point2.x);
			double Y = (point1.y - point2.y);
			double Z = (point1.z - point2.z);
			return (float)( (X * X) + (Y * Y) + (Z * Z) );
		}

		public static float distanceBetween(Position point1, Position point2)
		{
			double X = (point1.x - point2.x);
			double Y = (point1.y - point2.y);
			double Z = (point1.z - point2.z);
			return (float)Math.Sqrt( (X * X) + (Y * Y) + (Z * Z) );
		}

		public bool PositionIsAtOrigin
		{
			get
			{
				return ( m_X == 0.0 && m_Y == 0.0 && m_Z == 0.0 );
			}
		}


#if DEBUG // debug shite for reporting to me
		private static int atomNumber = 0;
		private static int chainNumber = 0;
		private static StringBuilder s = new StringBuilder(80);
		private static StreamWriter rw;

		public static void DebugWriteFilePart1( AtomList v1 )
		{
			rw = new StreamWriter( @"c:\debugwriteX.pdb", false );

			string name = "C  ";

			for( int i = 0; i < v1.Count; i++ )
			{

				if( name.Length > 3 ) name = name.Substring(0,3);
				if( name.Length < 3 ) name.PadRight(3,' ');

				s.Remove(0, s.Length);

				s.Append( "ATOM  " );
				s.Append( atomNumber++.ToString().PadLeft(5,' ') );
				s.Append("  ");
				s.Append( name );
				s.Append(" ");
				s.Append( "mol" );
				s.Append( " " );
				s.Append( 'A' );
				s.Append( chainNumber.ToString().PadLeft(4,' ') );
				s.Append( "    " );
				s.Append( v1[i].x.ToString("0.000").PadLeft(8,' ') );
				s.Append( v1[i].y.ToString("0.000").PadLeft(8,' ') );
				s.Append( v1[i].z.ToString("0.000").PadLeft(8,' ') );

				rw.WriteLine(s.ToString());
			}

			DebugReportNewMol();
		}

		public static void DebugWriteFilePart2( AtomList v2 )
		{
			string name = "N  ";

			for( int i = 0; i < v2.Count; i++ )
			{

				if( name.Length > 3 ) name = name.Substring(0,3);
				if( name.Length < 3 ) name.PadRight(3,' ');

				s.Remove(0, s.Length);

				s.Append( "ATOM  " );
				s.Append( atomNumber++.ToString().PadLeft(5,' ') );
				s.Append("  ");
				s.Append( name );
				s.Append(" ");
				s.Append( "mol" );
				s.Append( " " );
				s.Append( 'A' );
				s.Append( chainNumber.ToString().PadLeft(4,' ') );
				s.Append( "    " );
				s.Append( v2[i].x.ToString("0.000").PadLeft(8,' ') );
				s.Append( v2[i].y.ToString("0.000").PadLeft(8,' ') );
				s.Append( v2[i].z.ToString("0.000").PadLeft(8,' ') );

				rw.WriteLine(s.ToString());
			}

			rw.Close();
		}

		public static void DebugWriteFile( Position[] v1, Position[] v2 )
		{
			StreamWriter rw = new StreamWriter( @"c:\debugwrite.pdb", false );

			string name = "C  ";

			for( int i = 0; i < v1.Length; i++ )
			{

				if( name.Length > 3 ) name = name.Substring(0,3);
				if( name.Length < 3 ) name.PadRight(3,' ');

				s.Remove(0, s.Length);

				s.Append( "ATOM  " );
				s.Append( atomNumber++.ToString().PadLeft(5,' ') );
				s.Append("  ");
				s.Append( name );
				s.Append(" ");
				s.Append( "mol" );
				s.Append( " " );
				s.Append( 'A' );
				s.Append( chainNumber.ToString().PadLeft(4,' ') );
				s.Append( "    " );
				s.Append( v1[i].x.ToString("0.000").PadLeft(8,' ') );
				s.Append( v1[i].y.ToString("0.000").PadLeft(8,' ') );
				s.Append( v1[i].z.ToString("0.000").PadLeft(8,' ') );

				rw.WriteLine(s.ToString());
			}

			DebugReportNewMol();
			name = "N  ";

			for( int i = 0; i < v1.Length; i++ )
			{

				if( name.Length > 3 ) name = name.Substring(0,3);
				if( name.Length < 3 ) name.PadRight(3,' ');

				s.Remove(0, s.Length);

				s.Append( "ATOM  " );
				s.Append( atomNumber++.ToString().PadLeft(5,' ') );
				s.Append("  ");
				s.Append( name );
				s.Append(" ");
				s.Append( "mol" );
				s.Append( " " );
				s.Append( 'A' );
				s.Append( chainNumber.ToString().PadLeft(4,' ') );
				s.Append( "    " );
				s.Append( v2[i].x.ToString("0.000").PadLeft(8,' ') );
				s.Append( v2[i].y.ToString("0.000").PadLeft(8,' ') );
				s.Append( v2[i].z.ToString("0.000").PadLeft(8,' ') );

				rw.WriteLine(s.ToString());
			}

			rw.Close();
		}

		public static void DebugReportModelStatement()
		{
			Trace.WriteLine("MODEL");
		}

		public static void DebugReportEndModelStatement()
		{
			Trace.WriteLine("ENDMDL");
		}

		public static void DebugReport( Position[] v1, Position[] v2 )
		{
			for( int i = 0; i < v1.Length; i++ )
			{
				DebugReport( v1[i], 'C' );
			}
			DebugReportNewMol();
			for( int i = 0; i < v2.Length; i++ )
			{
				DebugReport( v2[i], 'N' );
			}
		}

		public static void DebugReport( Position[] v, char AtomID )
		{
			for( int i = 0; i < v.Length; i++ )
			{
				DebugReport( v[i], AtomID );
			}
		}

		public static void DebugReport( Position v, char AtomID )
		{
			string s = AtomID + "  ";
			DebugReport( v, s );
		}
		public static void DebugReport( Position v, string name )
		{
			if( name.Length > 3 ) name = name.Substring(0,3);
			if( name.Length < 3 ) name.PadRight(3,' ');

			s.Remove(0, s.Length);

			s.Append( "ATOM  " );
			s.Append( atomNumber++.ToString().PadLeft(5,' ') );
			s.Append("  ");
			s.Append( name );
			s.Append(" ");
			s.Append( "mol" );
			s.Append( " " );
			s.Append( 'A' );
			s.Append( chainNumber.ToString().PadLeft(4,' ') );
			s.Append( "    " );
			s.Append( v.x.ToString("0.000").PadLeft(8,' ') );
			s.Append( v.y.ToString("0.000").PadLeft(8,' ') );
			s.Append( v.z.ToString("0.000").PadLeft(8,' ') );

			Debug.WriteLine(s.ToString());
		}
		public static void DebugReportNewMol()
		{
			chainNumber++;
			atomNumber = 0;
		}
#endif




	}
}
