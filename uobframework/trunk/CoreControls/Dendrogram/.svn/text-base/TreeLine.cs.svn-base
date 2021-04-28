using System;

using UoB.Core.Primitives;
using UoB.Core.FileIO.Dendrogram;

namespace UoB.CoreControls.Dendrogram
{
	/// <summary>
	/// Summary description for TreeLine.
	/// </summary>
	public class TreeNode : Position
	{
		public int branchLevel = -1;
		public TreeNode ParentNode
		{
			get
			{
				return m_Parent;
			}
			set
			{
				m_Parent = value;
			}
		}
		private TreeNode m_Parent;
		private float m_Distance;
		private static Random rand = new Random(120452);
		private ParentedNode m_WrappedNode;
		public ParentedNode WrappedNode
		{
			get
			{
				return m_WrappedNode;
			}
		}

		public int ParentIndex
		{
			get
			{
				if( m_WrappedNode.Parent != null )
				{
					return m_WrappedNode.Parent.ID;
				}
				else
				{
					return -1;
				}
			}
		}

		private static bool m_PlusMinus = false;

		public TreeNode( ParentedNode node ) : base(rand.NextDouble(),rand.NextDouble(),rand.NextDouble())
		{
			m_WrappedNode = node;
			float length = m_WrappedNode.Length;
			if( length == 0 )
			{
				length = 0.000001f;
			}
			m_Distance = length * 1.0f;
			if( m_PlusMinus )
			{
                // we want all positions to fan out around the origin, not just on the + side
				x = -x; 
				y = -y; 
				z = -z;
			}
			m_PlusMinus = !m_PlusMinus;
		}

		public float IdealDistance
		{
			get
			{
				return m_Distance;
			}
		}
	}
}
