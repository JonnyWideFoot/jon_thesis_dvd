using System;

namespace UoB.Core.Primitives
{
	/// <summary>
	/// Source File containing all the different universal primilive data types
	/// used throughout the programme.
	/// </summary>

	public class xyzAngles
	{
		/// <summary>
		/// Camera Angle struct
		/// </summary>
		/// 

		public float x;
		public float y;
		public float z;

		public xyzAngles(float thex, float they, float thez)
		{
			x = 0.0f;
			y = 0.0f;
			z = 0.0f;
		}

		public void addToX( float X )
		{
			x += X;
		}

		public void addToY( float Y )
		{
			y += Y;
		}

		public void addToZ( float Z )
		{
			z += Z;
		}

		public void addToAll( float X, float Y, float Z )
		{
			x += X;
            y += Y;
			z += Z;
		}

		public xyzAngles(xyzAngles donorAngles)
		{
			x = 0.0f;
			y = 0.0f;
			z = 0.0f;
		}

		public void clone(xyzAngles donorAngles)
		{
			x = donorAngles.x;
			y = donorAngles.y;
			z = donorAngles.z;
		}


		public override string ToString()
		{
			string returnString = "x: " + x.ToString() + "y: " + y.ToString() + "z: " + z.ToString();
			return returnString;
		}
	}
}
