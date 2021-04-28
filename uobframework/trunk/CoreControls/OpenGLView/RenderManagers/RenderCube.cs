using System;

using Tao.OpenGl;

using UoB.Core.Primitives;
using UoB.Core.Primitives.Matrix;
using UoB.CoreControls.OpenGLView;
using UoB.CoreControls.OpenGLView.Primitives;

namespace UoB.CoreControls.OpenGLView.RenderManagers
{
	/// <summary>
	/// Summary description for RenderCube.
	/// </summary>
	public class RenderCube : RenderManager
	{
		public RenderCube( GLView parent ) : base( parent )
		{
		}

		public override void GLDraw()
		{

			Gl.glEnable(Gl.GL_DEPTH_TEST);
			Gl.glEnable(Gl.GL_LIGHTING);

			Gl.glEnable(Gl.GL_TEXTURE_2D);
			Gl.glBindTexture(Gl.GL_TEXTURE_2D, m_Env.texture[0]);

			Gl.glBegin(Gl.GL_QUADS);

			// Front Face
			Gl.glNormal3f( 0.0f, 0.0f, 1.0f);
			Gl.glTexCoord2f(0.0f, 0.0f);
			Gl.glVertex3f( -1.0f, -1.0f,  1.0f);	// Bottom Left Of The Texture and Quad
			Gl.glTexCoord2f(1.0f, 0.0f);
			Gl.glVertex3f(  1.0f, -1.0f,  1.0f);	// Bottom Right Of The Texture and Quad
			Gl.glTexCoord2f(1.0f, 1.0f);
			Gl.glVertex3f(  1.0f,  1.0f,  1.0f);	// Top Right Of The Texture and Quad
			Gl.glTexCoord2f(0.0f, 1.0f);
			Gl.glVertex3f( -1.0f,  1.0f,  1.0f);	// Top Left Of The Texture and Quad
			// Back Face
			Gl.glNormal3f( 0.0f, 0.0f,-1.0f);
			Gl.glTexCoord2f(1.0f, 0.0f);
			Gl.glVertex3f( -1.0f, -1.0f, -1.0f);	// Bottom Right Of The Texture and Quad
			Gl.glTexCoord2f(1.0f, 1.0f);
			Gl.glVertex3f( -1.0f,  1.0f, -1.0f);	// Top Right Of The Texture and Quad
			Gl.glTexCoord2f(0.0f, 1.0f);
			Gl.glVertex3f(  1.0f,  1.0f, -1.0f);	// Top Left Of The Texture and Quad
			Gl.glTexCoord2f(0.0f, 0.0f);
			Gl.glVertex3f(  1.0f, -1.0f, -1.0f);	// Bottom Left Of The Texture and Quad
			// Top Face
			Gl.glNormal3f( 0.0f, 1.0f, 0.0f);
			Gl.glTexCoord2f(0.0f, 1.0f);
			Gl.glVertex3f( -1.0f,  1.0f, -1.0f);	// Top Left Of The Texture and Quad
			Gl.glTexCoord2f(0.0f, 0.0f);
			Gl.glVertex3f( -1.0f,  1.0f,  1.0f);	// Bottom Left Of The Texture and Quad
			Gl.glTexCoord2f(1.0f, 0.0f);
			Gl.glVertex3f(  1.0f,  1.0f,  1.0f);	// Bottom Right Of The Texture and Quad
			Gl.glTexCoord2f(1.0f, 1.0f);
			Gl.glVertex3f(  1.0f,  1.0f, -1.0f);	// Top Right Of The Texture and Quad
			// Bottom Face
			Gl.glNormal3f( 0.0f,-1.0f, 0.0f);
			Gl.glTexCoord2f(1.0f, 1.0f);
			Gl.glVertex3f( -1.0f, -1.0f, -1.0f);	// Top Right Of The Texture and Quad
			Gl.glTexCoord2f(0.0f, 1.0f);
			Gl.glVertex3f(  1.0f, -1.0f, -1.0f);	// Top Left Of The Texture and Quad
			Gl.glTexCoord2f(0.0f, 0.0f);
			Gl.glVertex3f(  1.0f, -1.0f,  1.0f);	// Bottom Left Of The Texture and Quad
			Gl.glTexCoord2f(1.0f, 0.0f);
			Gl.glVertex3f( -1.0f, -1.0f,  1.0f);	// Bottom Right Of The Texture and Quad
			// Right face
			Gl.glNormal3f( 1.0f, 0.0f, 0.0f);
			Gl.glTexCoord2f(1.0f, 0.0f);
			Gl.glVertex3f(  1.0f, -1.0f, -1.0f);	// Bottom Right Of The Texture and Quad
			Gl.glTexCoord2f(1.0f, 1.0f);
			Gl.glVertex3f(  1.0f,  1.0f, -1.0f);	// Top Right Of The Texture and Quad
			Gl.glTexCoord2f(0.0f, 1.0f);
			Gl.glVertex3f(  1.0f,  1.0f,  1.0f);	// Top Left Of The Texture and Quad
			Gl.glTexCoord2f(0.0f, 0.0f);
			Gl.glVertex3f(  1.0f, -1.0f,  1.0f);	// Bottom Left Of The Texture and Quad
			// Left Face
			Gl.glNormal3f(-1.0f, 0.0f, 0.0f);
			Gl.glTexCoord2f(0.0f, 0.0f);
			Gl.glVertex3f( -1.0f, -1.0f, -1.0f);	// Bottom Left Of The Texture and Quad
			Gl.glTexCoord2f(1.0f, 0.0f);
			Gl.glVertex3f( -1.0f, -1.0f,  1.0f);	// Bottom Right Of The Texture and Quad
			Gl.glTexCoord2f(1.0f, 1.0f);
			Gl.glVertex3f( -1.0f,  1.0f,  1.0f);	// Top Right Of The Texture and Quad
			Gl.glTexCoord2f(0.0f, 1.0f);
			Gl.glVertex3f( -1.0f,  1.0f, -1.0f);	// Top Left Of The Texture and Quad
		
			Gl.glEnd();

			Gl.glDisable(Gl.GL_TEXTURE_2D);
			Gl.glDisable(Gl.GL_LIGHTING);
		}
	}
}