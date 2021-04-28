using System;
using Tao.OpenGl;

using UoB.Core.Primitives;
using UoB.Core.Primitives.Matrix;
using UoB.CoreControls.OpenGLView;
using UoB.CoreControls.OpenGLView.Primitives;

namespace UoB.CoreControls.OpenGLView.RenderManagers
{
	/// <summary>
	/// Summary description for LineRender.
	/// </summary>
	public class LineRender : RenderManager
	{
		public ColouredVector[] vectorArray = new ColouredVector[0];
		public Vector[] midpointArray = new Vector[0];
		public int[] connectionList = new int[0]; // holds the integer index of the m_Atom that each midpoint connects to

		public LineRender( GLView parent ) : base( parent )
		{
		}

		public override void GLDraw()
		{
			Gl.glBlendFunc( Gl.GL_SRC_ALPHA, Gl.GL_ONE );
			Gl.glEnable(Gl.GL_BLEND);		// Turn Blending On

			Gl.glBegin(Gl.GL_LINES);

			for ( int i = 0; i < connectionList.Length; i++ )
			{
				if( connectionList[i] != -1 )
				{
					ColouredVector p = vectorArray[ connectionList[i] ];
					Colour c = p.colour;
					Gl.glColor3f( c.RedOver255, c.GreenOver255, c.BlueOver255 );
					Gl.glVertex3d( p.x, p.y, p.z );
					Gl.glVertex3d( midpointArray[i].x, midpointArray[i].y, midpointArray[i].z );
				}
			}

			Gl.glEnd();

			Gl.glDisable(Gl.GL_BLEND);
		}
	}
}
