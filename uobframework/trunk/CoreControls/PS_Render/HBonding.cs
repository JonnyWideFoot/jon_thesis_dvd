using System;
using System.Collections;
using Tao.OpenGl;

using UoB.Core.Primitives;
using UoB.Core.Primitives.Matrix;
using UoB.CoreControls.OpenGLView;
using UoB.CoreControls.OpenGLView.Primitives;

using UoB.CoreControls.OpenGLView.RenderManagers;

namespace UoB.CoreControls.PS_Render
{
	/// <summary>
	/// Summary description for HBonding.
	/// </summary>
	public class HBonding : RenderManager
	{
        private AtomDrawWrapper[] m_Acceptors;
		private AtomDrawWrapper[] m_Hydrogens;
		private static readonly double m_DistSquaredCutoff = 5.0;

		public HBonding( GLView parent ) : base( parent )
		{
		}
	
		public void setupForAtoms( AtomDrawWrapper[] atoms )
		{
			ArrayList tempA = new ArrayList();
			ArrayList tempH = new ArrayList();
			for( int i = 0; i < atoms.Length; i++ )
			{
				if( atoms[i].m_Atom.atomPrimitive.FFAtomType.HBondAcceptor )
				{
					tempA.Add( atoms[i] );
				}
				else if( atoms[i].m_Atom.atomPrimitive.FFAtomType.HBondHydrogen )
				{
					tempH.Add( atoms[i] );
				}
			}

			m_Acceptors = (AtomDrawWrapper[]) tempA.ToArray( typeof(AtomDrawWrapper) );
			m_Hydrogens = (AtomDrawWrapper[]) tempH.ToArray( typeof(AtomDrawWrapper) );
		}

		public override void GLDraw()
		{
			if( m_Acceptors != null )
			{
				Gl.glEnable(Gl.GL_BLEND);
				Gl.glEnable(Gl.GL_LINE_STIPPLE);
				Gl.glLineStipple(3, 0xAAAA); // make them dashed

				Gl.glBegin(Gl.GL_LINES);
				Gl.glColor3f( 0.0f, 1.0f, 0.5f ); // nice green

				for ( int i = 0; i < m_Acceptors.Length; i++ )
				{
					for ( int j = 0; j < m_Hydrogens.Length; j++ )
					{
						AtomDrawWrapper p1 = m_Acceptors[i];
						AtomDrawWrapper p2 = m_Hydrogens[j];
						if( p1.ShouldDisplay && p2.ShouldDisplay && Position.distanceSquaredBetween( p1, p2 ) < m_DistSquaredCutoff )
						{
							Gl.glVertex3d( p1.x, p1.y, p1.z );
							Gl.glVertex3d( p2.x, p2.y, p2.z );
						}
					}
				}

				Gl.glEnd();

				Gl.glDisable(Gl.GL_LINE_STIPPLE);
				Gl.glDisable(Gl.GL_BLEND);
			}
		}
	}
}
