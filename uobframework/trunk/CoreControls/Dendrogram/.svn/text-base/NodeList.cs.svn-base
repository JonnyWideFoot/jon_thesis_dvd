using System;
using System.Collections;
using System.Drawing;

using Tao.OpenGl;

using UoB.Core.Primitives;
using UoB.Core.Primitives.Matrix;
using UoB.CoreControls.OpenGLView;
using UoB.CoreControls.OpenGLView.Primitives;
using UoB.CoreControls.OpenGLView.RenderManagers;
using UoB.Core.FileIO.Dendrogram;


namespace UoB.CoreControls.Dendrogram
{
	/// <summary>
	/// Summary description for LineList.
	/// </summary>
	public class NodeList : RenderManager
	{
		private TreeNode[] m_Nodes;
		private Random m_Rand = new Random();

		public NodeList( GLView parent ) : base( parent )
		{
		}

		private Vector m_MoveVec = new Vector();
		private Vector m_MoveVec2 = new Vector();

		// positive factor is towards the "inRelationTo" ...
		private void movePoint( Position moveThis, Position inRelationTo, double factor )
		{
			m_MoveVec.SetToAMinusB( inRelationTo, moveThis ); // vector is towards the "moveThis"
			m_MoveVec.MakeUnitVector();
			m_MoveVec.Multiply( factor );
			moveThis.Add( m_MoveVec );  
		}

		public void doStep()
		{
			// move all nodes to restore optimal distance
			TreeNode orig = (TreeNode) m_Nodes[0];
			for( int i = 1; i < m_Nodes.Length; i++ )
			{
				TreeNode n = (TreeNode) m_Nodes[i];
				double factor = ( (Position.distanceBetween( n, n.ParentNode ) - n.IdealDistance) );
				movePoint( n, n.ParentNode, factor );
				movePoint( n, orig, -0.2 );

				for( int j = 0; j < n.ParentNode.WrappedNode.Children.Count; j++ )
				{
					ParentedNode child =  (ParentedNode) n.ParentNode.WrappedNode.Children[j];
					TreeNode treeN = (TreeNode) m_Nodes[child.ID];
					if( treeN == n )
					{
						continue;
					}
					else
					{
						movePoint( n, treeN, -0.2 );
					}
				}

				m_MoveVec.setToZeros();
				for( int j = 0; j < m_Nodes.Length; j++ )
				{
					if( i == j ) continue;
					TreeNode nComp = (TreeNode) m_Nodes[j];
					if( ( nComp.branchLevel ) >= n.branchLevel )
					{
						continue;
					}
					double distance = Position.distanceSquaredBetween( n, nComp );
					double factor2;
					if( distance < 0.0001 )
					{
						factor2 = 0;
					}
					else
					{
						factor2 = ( -1 / distance );
					}
                    m_MoveVec2.SetToAMinusB( nComp, n ); 
					m_MoveVec2.MakeUnitVector();
					m_MoveVec2.Multiply( factor2 );
					m_MoveVec.Add( m_MoveVec2 );
				}   
				m_MoveVec.MakeUnitVector();
				m_MoveVec.Multiply( 0.05 );
				n.Add( m_MoveVec ); 
 
				TreeNode nParPar = n.ParentNode.ParentNode;
				if( nParPar == null )
				{
					continue;
				}
				movePoint( n, nParPar, 0.05 );  // towards the parents parent

              
				for( int j = 0; j < nParPar.WrappedNode.Children.Count; j++ )
				{
					ParentedNode child =  (ParentedNode) nParPar.WrappedNode.Children[j];
					TreeNode treeN = (TreeNode) m_Nodes[child.ID];
					if( treeN == n )
					{
						continue;
					}
					else
					{
						movePoint( n, treeN, -0.4 );
					}
				}
       		}
		}

		public void AddNodes( DendroTree tree )
		{
			ParentedNode[] nodes = tree.AllNodes;

			ArrayList allNodes = new ArrayList();

            TreeNode origin = new TreeNode( nodes[0] );
			origin.setToZeros(); // we want the root node to be at 0,0,0
			allNodes.Add( origin );
			for( int i = 1; i < nodes.Length; i++ )
			{
				allNodes.Add( new TreeNode( nodes[i] ) );
			}
			for( int i = 0; i < nodes.Length; i++ )
			{
				TreeNode node = ((TreeNode)allNodes[i]);
				int parentIndex = node.ParentIndex;
				if( parentIndex != -1 )
				{
					TreeNode parent = ((TreeNode)allNodes[parentIndex]);
					node.ParentNode = parent;
				}
			}

			m_Nodes = (TreeNode[])allNodes.ToArray(typeof(TreeNode));

			// set the branch levels
			setLevel( m_Nodes[0], 0 );
		}

		private void setLevel( TreeNode node, int prevLevel )
		{
			node.branchLevel = prevLevel;
			prevLevel++;
			for( int i = 0; i < node.WrappedNode.Children.Count; i++ )
			{
				ParentedNode pn = (ParentedNode)node.WrappedNode.Children[i];
				TreeNode tn = m_Nodes[ pn.ID ];
				setLevel( tn, prevLevel );
			}
		}

		public override void GLDraw()
		{
			Gl.glBegin(Gl.GL_LINES);
			Gl.glColor3f( 1.0f, 1.0f, 1.0f );

			for ( int i = 1; i < m_Nodes.Length; i++ )
			{
				TreeNode p1 = (TreeNode)m_Nodes[i];
				TreeNode p2 = p1.ParentNode;
				Gl.glVertex3d( p1.x, p1.y, p1.z );
				Gl.glVertex3d( p2.x, p2.y, p2.z );
			}

			Gl.glEnd();
		}

	}
}
