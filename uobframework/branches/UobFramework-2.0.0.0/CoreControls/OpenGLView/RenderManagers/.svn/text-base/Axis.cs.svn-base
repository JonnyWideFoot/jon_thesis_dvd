using System;

using Tao.OpenGl;
using Tao.Platform.Windows;

namespace UoB.CoreControls.OpenGLView.RenderManagers
{
	/// <summary>
	/// Summary description for Axis.
	/// </summary>
	public class Axis : RenderManager
	{
		public Axis( GLView parent ) : base( parent )
		{
		}

		#region IRenderable Members

		public override void GLDraw()
		{
			Gl.glBlendFunc( Gl.GL_SRC_ALPHA, Gl.GL_ONE );
			Gl.glEnable(Gl.GL_BLEND);		// Turn Blending On

			Gl.glBegin(Gl.GL_LINES);

			Gl.glColor3f(1.0f,1.0f,1.0f);
			Gl.glVertex3d(0.0, 0.0, 0.0);
			Gl.glVertex3d(1.0, 0.0, 0.0);
			Gl.glVertex3d(0.0, 0.0, 0.0);
			Gl.glVertex3d(0.0, 1.0, 0.0);
			Gl.glVertex3d(0.0, 0.0, 0.0);
			Gl.glVertex3d(0.0, 0.0, 1.0);

			Gl.glEnd();

			Gl.glDisable(Gl.GL_BLEND);
		}

		#endregion
	}
}
