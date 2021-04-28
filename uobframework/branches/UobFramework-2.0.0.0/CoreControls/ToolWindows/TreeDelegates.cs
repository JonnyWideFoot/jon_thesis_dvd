using System;
using System.Windows.Forms;

namespace UoB.CoreControls.ToolWindows
{
	public delegate void TreeNodeEvent ( TreeNode tn );
	public delegate void ParentedTreeNodeEvent ( TreeNode tn, TreeNode parent );
}
