using System;
using Tao.OpenGl;
using System.Diagnostics;

using UoB.Core.Structure;
using UoB.Core.Primitives;
using UoB.Core.Primitives.Collections;
using UoB.Core.Primitives.Matrix;
using UoB.CoreControls.OpenGLView;
using UoB.CoreControls.OpenGLView.Primitives;
using UoB.CoreControls.OpenGLView.RenderManagers;

namespace UoB.CoreControls.PS_Render
{
	/// <summary>
	/// Summary description for LineRender.
	/// </summary>
	public class ForceVectors : RenderManager
	{
		public AtomDrawWrapper[] m_Atoms = null;
		public PositionStore m_ForceVectors = null;
		private Perspective m_Perspective;

		public ForceVectors( GLView parent ) : base( parent )
		{
			m_Perspective = parent.perspective;
		}

		public void setupForAtoms( AtomDrawWrapper[] allAtoms, PositionStore forceVectors )
		{
			if( allAtoms == null || forceVectors == null )
			{
				if( m_ForceVectors.Width != m_Atoms.Length )
				{
					Debug.WriteLine("Error in forceVector initialisation. The Position arrays did not match in size, the call has been ignored");
					m_Atoms = null;
					m_ForceVectors = null;
				}
			}
			m_Atoms = allAtoms;
			m_ForceVectors = forceVectors;
		}

		public override void GLDraw()
		{
			if( m_Atoms != null && m_ForceVectors != null )
			{
				Position[] fvs = m_ForceVectors.currentPositionArray;
				
				if( fvs != null )
				{
					Gl.glEnable(Gl.GL_BLEND);

					Gl.glBegin(Gl.GL_LINES);
					Gl.glColor3f( 1.0f, 0.9f, 0.2f ); // yellowish

					for ( int i = 0; i < fvs.Length; i++ )
					{
						if( m_Atoms[i].ShouldDisplay )
						{
							Gl.glVertex3d( m_Atoms[i].x, m_Atoms[i].y, m_Atoms[i].z );
							Gl.glVertex3d( 
								fvs[i].x - m_Perspective.RotationOffset.x,
								fvs[i].y - m_Perspective.RotationOffset.y,
								fvs[i].z - m_Perspective.RotationOffset.z
								);
						}
					}

					Gl.glEnd();

					Gl.glDisable(Gl.GL_BLEND);
				}				
			}			
		}
	}
}
