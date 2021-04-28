using System;

using Tao.OpenGl;

using UoB.Core.Primitives.Matrix;
using UoB.Core.Primitives;
using UoB.CoreControls.OpenGLView;
using UoB.CoreControls.OpenGLView.Primitives;

namespace UoB.CoreControls.OpenGLView.RenderManagers
{
	/// <summary>
	/// Summary description for Surface.
	/// </summary>
	public class Surface : RenderManager
	{
		public PolygonIndexes[] Indexes = new PolygonIndexes[0];
		public Vector[] Vertices = new Vector[0];
		public Vector[] Normals = new Vector[0];
		public Colour[] Colours = new Colour[0];

		public Surface( GLView parent ) : base( parent )
		{
		}

		public override void GLDraw()
		{
			if ( Vertices != null )
			{
				Colour c;

				Gl.glEnable(Gl.GL_LIGHTING);
				Gl.glEnable(Gl.GL_DEPTH_TEST);
				Gl.glDisable(Gl.GL_BLEND);

				Gl.glBegin(Gl.GL_TRIANGLES);

				for ( int i = 0; i < Indexes.Length; i++ )
				{

					PolygonIndexes polygon = Indexes[i];

					c = Colours[polygon.i];
					Gl.glColor3f( c.RedOver255, c.GreenOver255, c.BlueOver255 );
					Gl.glNormal3d( Normals[polygon.i].x, Normals[polygon.i].y, Normals[polygon.i].z );
					Gl.glVertex3d( Vertices[polygon.i].x, Vertices[polygon.i].y, Vertices[polygon.i].z );

					c = Colours[polygon.j];
					Gl.glColor3f( c.RedOver255, c.GreenOver255, c.BlueOver255 );
					Gl.glNormal3d( Normals[polygon.j].x, Normals[polygon.j].y, Normals[polygon.j].z );
					Gl.glVertex3d( Vertices[polygon.j].x, Vertices[polygon.j].y, Vertices[polygon.j].z );

					c = Colours[polygon.k];
					Gl.glColor3f( c.RedOver255, c.GreenOver255, c.BlueOver255 );
					Gl.glNormal3d( Normals[polygon.k].x, Normals[polygon.k].y, Normals[polygon.k].z );
					Gl.glVertex3d( Vertices[polygon.k].x, Vertices[polygon.k].y, Vertices[polygon.k].z );
				}

				Gl.glEnd();

				Gl.glDisable(Gl.GL_LIGHTING);
				Gl.glDisable(Gl.GL_DEPTH_TEST);
				//Gl.glEnable( Gl.GL_BLEND );
                
			}			
		}

	}
}
