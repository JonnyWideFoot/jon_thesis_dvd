using System;
using System.Collections;


namespace PotentialGraph
{




	public class PotentialGraph
	{
		public class slope
		{
			public float m;
			public float c;

			public slope(float M, float C)
			{
				m = M;
				c = C;
			}

			public float yAt(float x)
			{
				return (x * m) + c;
			}
		}

		public class circle
		{
			public float radius;
			public float x;
			public float y;
			public float leftCutOff;
			public float rightCutoff;

			public circle(float theRadius, float theX, float theY)
			{
                radius = theRadius;
				x = theX;
				y = theY;
			}
		}

		public struct point
		{
			public float x;
			public float y;
			public point(float theX, float theY)
			{
				x = theX;
				y = theY;
			}
		}


		public PotentialGraph(float optimalVDWDistance, float[] wellDepths)
		{
			thePoints = new point[wellDepths.Length + 2];

			thePoints[0] = new point(0, yAxisHeightCutoff);
			thePoints[thePoints.Length - 1] = new point(graphCutoff,0);

			for ( int i = 0; i < wellDepths.Length; i++)
			{
				float x = (optimalVDWDistance + (waterRadius * 0.5f * i));
				float y = wellDepths[i];
				thePoints[i+1] = new point(x,y);
			}

			theSlopes = new slope[wellDepths.Length + 1];
            generateSlopes();

			theCircles = new circle[wellDepths.Length];
			generateCircles();
		}

		private void generateSlopes()
		{
			for (int i = 0; i < thePoints.Length -1; i++)
			{
				point point1 = thePoints[i];
				point point2 = thePoints[i+1];

				float m = ( (point1.y - point2.y) / (point1.x - point2.x) );
				float c = ( point1.y - ( m * point1.x ) );
                
				theSlopes[i] = new slope(m,c);
			}

		}

		private void generateCircles()
		{
			for (int i = 0; i < theSlopes.Length - 1; i++)
			{
				slope slope1 = theSlopes[i];
				slope slope2 = theSlopes[i+1];

				float thetaRad = (float) Math.Atan( ( (slope2.m - slope1.m) / ( 1 + ( slope1.m * slope2.m ) ) ) );
				float thetaDeg = thetaRad * (float)(180 / Math.PI);
				float radius = (float) (lineBacktrackForCircle * Math.Tan(0.5 * thetaRad));
				if (radius < 0) radius *= -1;
			
				//float x = ( ( slope2.c - slope1.c ) / ( slope1.m - slope2.m ) );
				//float y = ( ( (slope2.m * slope1.c) - ( slope1.m * slope2.c ) ) / ( slope2.m - slope1.m ) );

				slope tangent1 = slopeOfTangent(lineBacktrackForCircle, thePoints[i+1], slope1);
				
				//theCircles[i] = new circle(radius, x, y);
			}
		}

		private slope slopeOfTangent(float L, point thePoint, slope theSlope)
		{
			float sigma = (float)Math.Atan(theSlope.m);
			float dv =  (float)( L * Math.Sin(sigma) );
			float dh =  (float)( L * Math.Cos(sigma) );
            float X1t = (float)( thePoint.x - dh );
			float Y1t = (float)( thePoint.y - dv );

			float M1t = ( 1 / theSlope.m );
			float C1t = (( X1t * ( theSlope.m - ( 1/ theSlope.m ) ) ) - theSlope.c );

            return new slope( M1t, C1t );         

		}


		private const float lineBacktrackForCircle = 2.0f;
		private const float graphCutoff = 8.0f;
		private const float yAxisHeightCutoff = 20.0f;
		private const float waterRadius = 1.6f;
		private point[] thePoints;
		private slope[] theSlopes;
		private circle[] theCircles;

		private void setWellDepth(int wellID, float setTo)
		{
            


		}        

		public float valueAtPoint(float x)
		{
			if ( x > graphCutoff || x < 0 ) return 0;


            return 0;
		}


	}







	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{

			float[] theFloats = new float[3];
			theFloats[0] = -4.0f;
			theFloats[1] = 2.0f;
			theFloats[2] = -1.0f;

			PotentialGraph graph = new PotentialGraph(2.0f, theFloats);


		}
	}
}
