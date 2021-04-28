using System;

using Tao.OpenGl;

using UoB.CoreControls.OpenGLView;
using UoB.CoreControls.OpenGLView.Primitives;
using UoB.Core.Primitives;
using UoB.Core.Primitives.Matrix;

namespace UoB.CoreControls.OpenGLView.RenderManagers
{
	/// <summary>
	/// Summary description for PointRender.
	/// </summary>
	public class PointRender : RenderManager
	{
		public ColouredVector[] vectorArray = new ColouredVector[0];

		public PointRender( GLView parent ) : base( parent )
		{
		}

		private static readonly double size = 0.3f;

		public override void GLDraw()
		{
			Gl.glBlendFunc( Gl.GL_SRC_ALPHA, Gl.GL_ONE );
			Gl.glEnable(Gl.GL_BLEND);		// Turn Blending On

			Gl.glBegin(Gl.GL_LINES);

			for ( int i = 0; i < vectorArray.Length; i++ )
			{
				ColouredVector p = vectorArray[i];
				Gl.glColor3f( 
					p.colour.RedOver255, 
					p.colour.GreenOver255, 
					p.colour.BlueOver255 );
				Gl.glVertex3d( p.x - size, p.y, p.z );
				Gl.glVertex3d( p.x + size, p.y, p.z );
				Gl.glVertex3d( p.x, p.y - size, p.z );
				Gl.glVertex3d( p.x, p.y + size, p.z );
				Gl.glVertex3d( p.x, p.y, p.z - size );
				Gl.glVertex3d( p.x, p.y, p.z + size );
			}

			Gl.glEnd();

			Gl.glDisable(Gl.GL_BLEND);
		}
	}
}
