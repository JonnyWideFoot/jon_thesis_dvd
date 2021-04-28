using System;

using UoB.CoreControls.OpenGLView;

namespace UoB.CoreControls.OpenGLView.RenderManagers
{
	/// <summary>
	/// Summary description for RenderManager.
	/// </summary>
	public abstract class RenderManager : IRenderable
	{
		protected GLView m_Parent;
		protected GLEnvironment m_Env;

		public RenderManager( GLView parent )
		{
			m_Parent = parent;
			m_Env = m_Parent.GLEnvironment;
		}

		#region IRenderable Members

		public virtual void GLDraw()
		{
		}

		#endregion
	}
}
