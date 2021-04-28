using System;

using UoB.Core.Primitives;

namespace UoB.CoreControls.OpenGLView.Primitives
{
	/// <summary>
	/// Summary description for ColouredPoint.
	/// </summary>
	public class ColouredVector : Vector
	{
		public Colour colour;

		public ColouredVector() : base()
		{
		}

		public ColouredVector( double x, double y, double z )
			: base( x, z, z )
		{
		}

		public ColouredVector(int x, int y, int z)
			: base( x, z, z )
		{
		}

		public ColouredVector(float x, float y, float z)
			: base( x, z, z )
		{
		}

		public ColouredVector( float[] position )
			: base( position )
		{
		}

		public ColouredVector( Position donor )
			: base( donor )
		{
		}
	}
}
