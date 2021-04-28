using System;
using System.Collections;
using System.Drawing;
using System.Threading;

using UoB.CoreControls.OpenGLView;
using UoB.CoreControls.OpenGLView.Primitives;
using UoB.Core.FileIO.Dendrogram;
using UoB.Core;


namespace UoB.CoreControls.Dendrogram
{
	/// <summary>
	/// Summary description for TreeRenderer.
	/// </summary>
	public sealed class TreeRenderer
	{
		private DendroTree m_Tree;
		private GLView m_Viewer;
		private NodeList m_Nodes;
		public Perspective m_Perspective;

		public TreeRenderer( DendroTree tree, GLView viewer )
		{
			m_Viewer = viewer;
			doRefresh = new UpdateEvent( m_Viewer.Refresh );
			m_Perspective = m_Viewer.perspective;
			m_Tree = tree;

			m_Nodes = new NodeList( m_Viewer );
			m_Nodes.AddNodes( m_Tree );

			m_Viewer.AddRenderObject( m_Nodes );
		}

		private bool m_Running = false;
		private UpdateEvent doRefresh;
		public void beginSimulation()
		{
			ThreadStart ts = new ThreadStart( simulate );
			Thread t = new Thread( ts );
			t.Priority = ThreadPriority.Lowest;
			t.Start();	
		}

		private void simulate()
		{
			if( !m_Running )
			{
				m_Running = true;
				while( m_Running )
				{
					m_Nodes.doStep();
					m_Viewer.Parent.Invoke( doRefresh );				
				}				
			}
		}

	}
}
