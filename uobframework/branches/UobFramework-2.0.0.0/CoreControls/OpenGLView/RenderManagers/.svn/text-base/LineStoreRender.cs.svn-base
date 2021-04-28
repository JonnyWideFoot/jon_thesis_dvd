using System;

using Tao.OpenGl;

using UoB.Core.Primitives;
using UoB.Core.Primitives.Matrix;
using UoB.Core.Primitives.Collections;
using UoB.CoreControls.OpenGLView;
using UoB.CoreControls.OpenGLView.Primitives;

namespace UoB.CoreControls.OpenGLView.RenderManagers
{
	/// <summary>
	/// Summary description for LineStoreRender.
	/// </summary>
	public class LineStoreRender : RenderManager
	{
		private Line3DStore m_Lines = null;

		public LineStoreRender( GLView parent ) : base( parent )
		{
		}

		public void Setlist( Line3DStore lineList )
		{
			m_Lines = lineList;
		}

		private Position p = new Position();
		public override void GLDraw()
		{
			if( m_Lines != null )
			{
				Gl.glBlendFunc( Gl.GL_SRC_ALPHA, Gl.GL_ONE );
				Gl.glEnable(Gl.GL_BLEND);		// Turn Blending On

				Gl.glBegin(Gl.GL_LINES);

				Line3D[] lines = m_Lines.currentLine3DArray;
				
				for ( int i = 0; i < lines.Length; i++ )
				{
					Colour c = lines[i].colour;
					Gl.glColor3f( c.RedOver255, c.GreenOver255, c.BlueOver255 );
					p.SetToAMinusB( lines[i].point1, m_Parent.perspective.RotationOffset );
					Gl.glVertex3d( p.x, p.y, p.z );
					p.SetToAMinusB( lines[i].point2, m_Parent.perspective.RotationOffset );
					Gl.glVertex3d( p.x, p.y, p.z );
				}

				Gl.glEnd();

				Gl.glDisable(Gl.GL_BLEND);
			}
		}
	}
}
