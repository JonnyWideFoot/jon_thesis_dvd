using System;
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace UoB.Core.FileIO.Dendrogram
{
	/// <summary>
	/// Summary description for Tree.
	/// Class intended to be used with the .ph output from ClustalW
	/// </summary>
	public sealed class DendroTree
	{
		private BranchNode m_RootNode;
		private ArrayList m_AllNodes;

		public DendroTree( string filename )
		{
			ParentedNode.AllCreatedNodes = new ArrayList();
			ParentedNode.NodeCreationCount = 0;

				m_RootNode = new BranchNode( null );
				StreamReader re = new StreamReader( filename );
				ReadTree( re );
				re.Close();

			m_AllNodes = ParentedNode.AllCreatedNodes;
			ParentedNode.AllCreatedNodes = null;
			ParentedNode.NodeCreationCount = 0;
		}

		private void ReadTree( StreamReader re )
		{
			int nextChar = re.Peek();
			while( nextChar != '(' ) // read to the start of the tree - the opening bracket
			{
				nextChar = re.Read();
				if( nextChar == -1 ) return; // the end of the stream, shouldnt get here, we want a tree
			}
			// we found our bracket ...
			m_RootNode.ReadNextConnection( re );
		}

		public ParentedNode[] AllNodes
		{
			get
			{
				return (ParentedNode[])m_AllNodes.ToArray(typeof(ParentedNode));
			}
		}

		public EndNode scanForNode( string id )
		{
			return scanForNode( id, m_RootNode );
		}

		public EndNode scanForNode( string id, ParentedNode checkNode )
		{
			for( int i = 0; i < checkNode.Children.Count; i++ )
			{
				ParentedNode node = (ParentedNode)checkNode.Children[i];
				if( node is EndNode )
				{
                    EndNode endNode = (EndNode)node;
					if( endNode.Label == id )
					{
						return endNode;
					}
				}
				else
				{
					EndNode endNode = scanForNode( id, node );
					if( endNode != null )
					{
						return endNode;
					}
				}
			}
			return null;
		}

		public ParentedNode scanForCommonParent( EndNode id1, EndNode id2 )
		{
			int count = 0;
			ParentedNode parentID1 = id1.Parent;
			while( parentID1.Parent != m_RootNode )
			{
				ParentedNode parentID2 = id2.Parent;
				while( parentID2.Parent != m_RootNode )
				{
					if( parentID1 == parentID2 )
					{
						return parentID2;
					}
					parentID2 = parentID2.Parent;
				}
				parentID1 = parentID1.Parent;
				count++;
			}
			return null;
		}

	}
}
