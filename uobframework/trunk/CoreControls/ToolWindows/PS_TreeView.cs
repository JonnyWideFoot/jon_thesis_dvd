using System;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Threading;
using System.Reflection;

using UoB.Core;
using UoB.Core.Structure;
using UoB.Core.ForceField.Definitions;
using UoB.Core.Primitives;
using UoB.CoreControls.Images;
using UoB.CoreControls.OpenGLView;
using UoB.Core.ForceField;
using UoB.CoreControls.PS_Render;

namespace UoB.CoreControls.ToolWindows
{
	/// <summary>
	/// Summary description for PSTreeView.
	/// </summary>
	public class PSTreeView : TreeView, ITool
	{
		private System.Windows.Forms.ImageList IconList;
		private System.Windows.Forms.ContextMenu treeMenu;
		private System.Windows.Forms.MenuItem Properties;
		private System.Windows.Forms.MenuItem ReLoad;
		private System.ComponentModel.IContainer components;

		private TreeNode CurrentNode;
		private System.Windows.Forms.ColorDialog colorDialog;
		private System.Windows.Forms.MenuItem menu_SetColor;
		private ParticleSystemDrawWrapper m_PSDrawWrapper;
		private ParticleSystem m_ParticleSystem;

		private UpdateEvent m_PSContentUpdate;

		private ImagingDetails m_Imaging;

		public PSTreeView(ParticleSystemDrawWrapper psdw) : base()
		{
			InitializeComponent();
			init();
			m_PSDrawWrapper = psdw;
			InternalSetPS( psdw.particleSystem );
			populate();
		}

		public PSTreeView() : base()
		{
			InitializeComponent();
			init();
			populate();
		}

		private void init()
		{
			m_PSContentUpdate = new UpdateEvent( populate );
			m_Imaging = FFManager.Instance.Imaging;
			ImageList = PictureContainer.Instance.IconList;
			BorderStyle = BorderStyle.FixedSingle;
			this.Name = "PSTreeView";
			this.Text = "PS TreeView";
			this.Size = new Size( 220, 650 );
			this.MouseUp += new MouseEventHandler(tree_MouseUp);
			this.AfterCheck += new TreeViewEventHandler(doTreeChecked);
			this.menu_SetColor.Click += new EventHandler(menu_SetColor_Click);
			this.Properties.Click += new EventHandler(Properties_Click);
			this.ReLoad.Click += new EventHandler(ReLoad_Click);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PSTreeView));
			this.IconList = new System.Windows.Forms.ImageList(this.components);
			this.treeMenu = new System.Windows.Forms.ContextMenu();
			this.Properties = new System.Windows.Forms.MenuItem();
			this.ReLoad = new System.Windows.Forms.MenuItem();
			this.colorDialog = new System.Windows.Forms.ColorDialog();
			this.menu_SetColor = new System.Windows.Forms.MenuItem();
			// 
			// IconList
			// 
			this.IconList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.IconList.ImageSize = new System.Drawing.Size(16, 16);
			this.IconList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IconList.ImageStream")));
			this.IconList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// treeMenu
			// 
			this.treeMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.Properties,
																					 this.menu_SetColor,
																					 this.ReLoad});
			// 
			// Properties
			// 
			this.Properties.Index = 0;
			this.Properties.Text = "Properties";
			// 
			// ReLoad
			// 
			this.ReLoad.Index = 2;
			this.ReLoad.Text = "ReLoad";
			// 
			// menu_SetColor
			// 
			this.menu_SetColor.Index = 1;
			this.menu_SetColor.Text = "SetColor";

		}

		private void doTreeChecked(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			e.Node.Expand();
			foreach (TreeNode theNode in e.Node.Nodes)
			{
				theNode.Checked = e.Node.Checked;
			}
			Nodes[0].Checked = true;
		}

		private void tree_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// logic to show context menu  
			if(e.Button == MouseButtons.Right)  
			{  
				Point ClickPoint = new Point(e.X,e.Y);   
				CurrentNode = GetNodeAt(ClickPoint);   
				if(CurrentNode == null) 
					return;   
				// Convert from Tree coordinates to Screen    
				Point ScreenPoint = PointToScreen(ClickPoint);   // Convert from Screen to Form   Point 
				Point FormPoint = this.PointToClient(ScreenPoint);     
				treeMenu.Show(this,FormPoint);  // showing the context menu
			}
		}

		private void Properties_Click(object sender, System.EventArgs e)
		{
			if (CurrentNode.Tag == null)
				return;

			if ( CurrentNode.Tag.GetType() == typeof(Atom) )
			{
				MessageBox.Show("Property Sheet Not Implememnted...");
//				//MessageBox.Show(CurrentNode.FullPath);
//				AtomPropSheet aps = new AtomPropSheet((Atom)CurrentNode.Tag);
//				if ( aps.ShowDialog(this) == DialogResult.OK)
//				{
//					m_Document.triggerUpdateEvent();
//				}

			} 
			else if ( CurrentNode.Tag.GetType() == typeof(AminoAcid) )
			{
				MessageBox.Show(CurrentNode.FullPath);
			}
			else
			{
				MessageBox.Show("Property Sheet Not Implememnted For That TagType....");
			}
		}

		private void menu_SetColor_Click(object sender, System.EventArgs e)
		{
			MessageBox.Show("Not Implemented, use atom selections insetead");
//			if (CurrentNode.Tag == null)
//			{
//				return;
//			}
//			else if ( CurrentNode.Tag.GetType() == typeof(Atom) )
//			{
//				Atom a = (Atom)CurrentNode.Tag;
//				if (colorDialog.ShowDialog(this) == DialogResult.OK)
//				{
//					m_PSDrawWrapper.setColour(a.ArrayIndex, Colour.FromColor( colorDialog.Color ) );
//				}
//			} 
//			else if ( typeof(PSMolContainer) == CurrentNode.Tag.GetType() )
//			{
//				PSMolContainer a = (PSMolContainer)CurrentNode.Tag;
//				if (colorDialog.ShowDialog(this) == DialogResult.OK)
//				{
//					m_PSDrawWrapper.colourEditingInProgress = true;
//						AtomList atoms = a.Atoms;
//						if ( atoms != null )
//						{
//							for ( int i = 0; i < atoms.Count; i++ )
//							{
//								m_PSDrawWrapper.setColour(atoms[i].ArrayIndex, Colour.FromColor( colorDialog.Color ) );
//							}
//						}
//					m_PSDrawWrapper.colourEditingInProgress = false;
//				}
//			}
		}

		public void populate()
		{
			Nodes.Clear();
			TreeNode statusNode = new TreeNode("Updating ...",0 ,0);
			Nodes.Add( statusNode );
			Refresh();

			if ( m_ParticleSystem == null ) 
			{
				statusNode.Text = "No System Present";
				return;
			}

			if( IsHandleCreated == false )  // some bizaro thing with the invoke call later from the worker thread...
			{
				this.CreateHandle();
			}

            ThreadPool.QueueUserWorkItem( new WaitCallback( populateWorkThread ) );
		}

		private void populateWorkThread( object nullStateInfoForDelegate )
		{
			bool done = false;
			while(!done)
			{  
				try
				{
					// cant use this - something odd is going on ....
					//m_ParticleSystem.AcquireReaderLock(1000);

						// It is safe for this thread to read from
						// the shared resource.
						TreeNode PSNode = new TreeNode("Particle System",8 ,8);
						TreeGeneration( PSNode, m_ParticleSystem.Members );
						TreeNodeEvent tnEvent = new TreeNodeEvent( AddFinalNode );
						Invoke( tnEvent, new object[] { PSNode } );
						done = true;
       
				}
				catch (ApplicationException)
				{
					// The reader lock request timed out.
				}
				finally
				{
					// Ensure that the lock is released.
					//m_ParticleSystem.ReleaseReaderLock();
				}
			}
		}

		private void AddFinalNode( TreeNode mainNode ) // required for Invoke on main thread following populateWorkThread
		{
			if ( Nodes != null  )
			{
				Nodes.Clear();	
				if ( mainNode != null )
				{
					Nodes.Add(mainNode);
				}
				mainNode.Expand();
				if ( mainNode.Nodes.Count == 1 )
				{
					mainNode.Nodes[0].Expand();
				}
			}
			Refresh();
		}


		private void TreeGeneration(TreeNode node, PSMolContainer[] members)
		{
			foreach ( PSMolContainer member in members )
			{
				TreeNode molContainerNode = new TreeNode( member.ToString(), 1, 1 );
				molContainerNode.Tag = member;

				if ( member.Count != 0 )
				{
					for( int j = 0; j < member.Count; j++ )
					{		
						Molecule m = member[j];						 
						TreeNode molNode = new TreeNode( m.ToString(), 2, 2 );
						for ( int i = 0; i < m.Count; i++ )
						{
							TreeNode atomNode = new TreeNode(
								m[i].ToString(), m_Imaging.getImageNumSel(m[i].atomPrimitive), m_Imaging.getImageNumDesel(m[i].atomPrimitive) );
							atomNode.Tag = m[i];
							molNode.Nodes.Add( atomNode );
						}
						molContainerNode.Nodes.Add( molNode );
					}
				}
				node.Nodes.Add(molContainerNode);
			}
		}

		private void ReLoad_Click(object sender, EventArgs e)
		{
			populate();
		}
		#region ITool Members

		public void AttactToParticleSystem( ParticleSystem ps )
		{
			InternalSetPS( ps );
			m_PSDrawWrapper = null;
			populate();
		}

		private void InternalSetPS( ParticleSystem ps )
		{
			if( m_ParticleSystem != null )
			{
				m_ParticleSystem.ContentUpdate -= m_PSContentUpdate;
			}
			m_ParticleSystem = ps;
			if( m_ParticleSystem != null )
			{
				m_ParticleSystem.ContentUpdate += m_PSContentUpdate;
			}
		}

		public void AttachToDocument(UoB.CoreControls.Documents.Document doc)
		{
			bool found = false;
			if ( doc != null )
			{
				for( int i = 0; i < doc.MemberCount; i++ )
				{
					if ( doc[i].GetType() == typeof(ParticleSystemDrawWrapper) )
					{
						m_PSDrawWrapper = (ParticleSystemDrawWrapper) doc[i];
						InternalSetPS( m_PSDrawWrapper.particleSystem );
						found = true;
						break; // only one drawWrapper should ever be present
						// the draw wrapper PS will be the same as the PS in the member array of the Document
					}
				}
			}
			if( !found ) // if we didnt get one, detach from previous
			{
				InternalSetPS( null );
				m_PSDrawWrapper = null;
			}
			populate();
		}

		#endregion
	}
}
