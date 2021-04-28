using System;
using System.IO;
using System.Text;
using System.Collections;

namespace UoB.Core.FileIO.Dendrogram
{
	public abstract class ParentedNode
	{
		protected static StringBuilder m_StaticStringBuild = new StringBuilder();
		public static ArrayList AllCreatedNodes;
		public static int NodeCreationCount = 0;
		public ParentedNode m_Parent;
		public ParentedNode Parent
		{
			get
			{
				return m_Parent;
			}
		}
		protected float m_Distance;
		public ArrayList Children = new ArrayList();

		public static void PrintContainedEndNodesToFile( string fileName, ParentedNode commonParent )
		{
			StreamWriter rw = new StreamWriter( fileName );
			EndNode[] nodes = commonParent.allContainedEndNodes;
			for( int i = 0; i < nodes.Length; i++ )
			{
				rw.WriteLine( nodes[i].Label );
			}
			rw.Close();
		}

		public EndNode[] allContainedEndNodes
		{
			get
			{
				ArrayList ends = new ArrayList();
				ScanChildrenForEnds( ends, this );
				return (EndNode[])ends.ToArray(typeof(EndNode));
			}
		}

		private void ScanChildrenForEnds( ArrayList foundEnds, ParentedNode parent )
		{
			for( int i = 0; i < parent.Children.Count; i++ )
			{
				if( parent.Children[i] is EndNode )
				{
					foundEnds.Add( parent.Children[i] );
				}
				else
				{
                    ScanChildrenForEnds( foundEnds, (ParentedNode) parent.Children[i] );
				}
			}
		}

		private int m_ID = -1;
		public int ID
		{
			get
			{
				return m_ID;
			}
		}

		public ParentedNode( ParentedNode parent )
		{
			AllCreatedNodes.Add( this );
			m_ID = NodeCreationCount++;
			m_Parent = parent;
		}

		public float Length
		{
			get
			{
				return m_Distance;
			}
			set
			{
				m_Distance = value;            
			}
		}

		private float readNumber( StreamReader re )
		{
			int nextChar;
			while( true )
			{
				nextChar = re.Peek();// dont consume the character, only peek at it
				switch( nextChar )
				{
					case 10:
						re.Read();
						break;
					case 13:
						re.Read();
						break;
					case -1:
						goto StringDone;
					case ':':
						goto StringDone;
					case ',':
						goto StringDone;
					case ')':
						goto StringDone;
					default:
						m_StaticStringBuild.Append( (char)re.Read() ); // now we can consume it
						break;
				}
			}	
			StringDone:
			{
				string returnNumber = m_StaticStringBuild.ToString();
				m_StaticStringBuild.Remove(0,m_StaticStringBuild.Length);  
				float length = -1.0f;
				try
				{
					length = float.Parse(returnNumber);
				}
				catch
				{
				}
				return length;
			}         			
		}

		private string readName( StreamReader re )
		{
			int nextChar;
			while( true )
			{
				nextChar = re.Peek();
				switch( nextChar )
				{
					case 10:
						re.Read();
						break;
					case 13:
						re.Read();
						break;
					case -1:
						goto StringDone;
					case ':':
						goto StringDone;
					case ',':
						goto StringDone;
					case ')':
						goto StringDone;
					default:
						m_StaticStringBuild.Append( (char)re.Read() );
						break;
				}
			}	
			StringDone:
			{
				string returnString = m_StaticStringBuild.ToString();
                m_StaticStringBuild.Remove(0,m_StaticStringBuild.Length);            
				return returnString;
			}         			
		}

		private ParentedNode returnTypedNode( StreamReader re )
		{
			ParentedNode nextNode;
			// is the next node also a branch node ?
			while( true )
			{
				char peek = (char) re.Peek();
				switch( peek )
				{
					case (char)10: // ignore new line characters
						re.Read();
						break;
					case (char)13: // ignore new line characters
						re.Read();
						break;
					case '(': // its is a branch point
						nextNode = new BranchNode( this ); // we have more tree levels to climb, we will return to the lower code later when some branches have closed
						Children.Add( nextNode );
						nextNode.ReadNextConnection( re );
						goto NodeObtained;
					default: // its not a branch point
						nextNode = new EndNode( this, readName( re ) );
						Children.Add( nextNode );
						goto NodeObtained;
				}
			}
			NodeObtained:
			{
				return nextNode;
			}
		}

		public void ReadNextConnection( StreamReader re )
		{
			ParentedNode nextNode = null;
			int nextChar;
			while( true )
			{
				nextChar = re.Read();
				if( nextChar == -1 )
				{
					return;
				}
				switch( (char)nextChar )
				{
					case (char)10:
						break;
					case (char)13:
						break;
					case '(':
                        nextNode = returnTypedNode(re);
						break;
					case ':':
						// its about to define a length
						float length = readNumber( re );
						if( nextNode == null ) // we are defining the length from this nodes parent node
						{
							m_Distance = length;
							return;
						}
						else
						{
							// we are giving a length to a new node
							nextNode.Length = length;
							nextNode = null;
						}
						break;
					case ';':
						// we are done, this should be the final node
						return;
					case ',':
						// this node is done, but expect another one now
						nextNode = returnTypedNode(re);
						break;
					case ')':
						return; // always return on finding an ending bracket
					default:
						// shouldnt get here as if there was a name, it has already been read above...
						throw new Exception("Code assumption error in ParentedNode.Read()");				
				}
			}
		}
	}
}
