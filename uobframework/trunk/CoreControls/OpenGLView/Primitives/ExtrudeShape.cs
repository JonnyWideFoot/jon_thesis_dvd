using System;

using UoB.Core.Primitives;
using UoB.Core.Primitives.Matrix;

namespace UoB.CoreControls.OpenGLView.Primitives
{
	/// <summary>
	/// Summary description for ExtrudeShape.
	/// </summary>
	public class ExtrudeShape
	{
		public ExtrudeShape()
		{
		}

		public Vector[] p = new Vector[4];  // both the relative position vector
		public Vector[] normal = new Vector[4]; // normal vector at that point

		void setTo(ExtrudeShape es)
		{ // copy constructor
            p = es.p;
			normal = es.normal;
		}

		public void rotate( MatrixRotation rm )
		{ 
			rm.transform( p );
			rm.transform( normal );
		}

		public void addVector( Vector av )
		{         // add vector to all positions (but not normals)
			for( int i = 0; i < p.Length; i++)
			{
				p[i] += av;
			}
		}

		public void setToInterpolation( ExtrudeShape s1, ExtrudeShape s2, float t)
		{    // makes a mix between two types t=0..1
			for(int i = 0; i < p.Length; i++)
			{
				p[i] = Vector.StaticInterpolate_2(s1.p[i],t,s2.p[i],1.0f-t);
				normal[i] = Vector.StaticInterpolate_2(s1.normal[i],t,s2.normal[i],1.0f-t);
			}
		}

		public void setToStrand()
		{           // makes a flat rectangle
			p[0] = new Vector( -1.0f, 0.3f,0.0f );
			p[1] = new Vector( 1.0f, 0.3f,0.0f );
			p[2] = new Vector( 1.0f,-0.3f,0.0f );
			p[3] = new Vector( -1.0f,-0.3f,0.0f );
			normal[0] = new Vector( -0.5f, 0.5f,0.0f);
			normal[1] = new Vector( 0.5f, 0.5f,0.0f);
			normal[2] = new Vector( 0.5f,-0.5f,0.0f);
			normal[3] = new Vector( -0.5f,-0.5f,0.0f);
		}
		public void setToSquareTube()
		{       // makes a square tube
			p[0] = new Vector( -0.3f, 0.3f,0.0f);
			p[1] = new Vector(  0.3f, 0.3f,0.0f);
			p[2] = new Vector(  0.3f,-0.3f,0.0f);
			p[3] = new Vector( -0.3f,-0.3f,0.0f);
			normal[0] = new Vector( -0.5f, 0.5f,0.0f);
			normal[1] = new Vector(  0.5f, 0.5f,0.0f);
			normal[2] = new Vector(  0.5f,-0.5f,0.0f);
			normal[3] = new Vector( -0.5f,-0.5f,0.0f);
		}
	}
}
