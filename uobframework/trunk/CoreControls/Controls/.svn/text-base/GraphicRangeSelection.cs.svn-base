using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using UoB.Core;
using UoB.Core.Primitives;

namespace UoB.CoreControls.Controls
{
	/// <summary>
	/// Summary description for GraphicRangeSelection.
	/// </summary>
	public class GraphicRangeSelection : BaseRangeControl
	{
		private System.Windows.Forms.ContextMenu contextMenu;
		private System.Windows.Forms.MenuItem menu_ContinualUpdate;
		private System.Windows.Forms.MenuItem menu_ScrollLeft;
		private System.Windows.Forms.MenuItem menu_ScrollRight;
		private System.Windows.Forms.MenuItem menu_SelectAll;
		private System.Windows.Forms.MenuItem menu_ExtendLeft;
		private System.Windows.Forms.MenuItem menu_Seperator;
		private System.Windows.Forms.MenuItem menu_ExtendRight;
		private System.Windows.Forms.MenuItem menu_RetractLeft;
		private System.Windows.Forms.MenuItem menu_RetractRight;

		protected bool m_AllowUserBarSelect = true;

		protected Pen m_Pen = new Pen( Color.Black, 1 );
		protected SolidBrush m_Brush = new SolidBrush( Color.Navy );

		private int m_CacheXOffset = 0; // the pixel offset from the left boundry when the mouse went down
		private int m_RenderStart = 0;
		private int m_RenderLength = 0;
		private int m_MoveMode = 0;
		private int m_BoxStart = 7;
		private int m_BoxWidth = 0;
		private int m_PrevStart = -1; // keep tabs of our last change ...
		private int m_PrevLength = -1; // used in the SetRangeFromRenderVariables() function below

		public GraphicRangeSelection() : base()
		{
			InitializeComponent();	
			SetStyle(ControlStyles.DoubleBuffer |
					ControlStyles.UserPaint |
					ControlStyles.AllPaintingInWmPaint,
					true);

			m_BoxWidth = Width - (2 * m_BoxStart ); // initialise this ...
			Range = null; // initialise for the null state
		}


		#region Internal Events
		/// <summary>
		/// sets the render range variables before repainting
		/// </summary>
		protected override void UpdateDisplayFromRange()
		{
			if( Enabled && m_Range != null )
			{
				double pixelsPerInt = (double) m_BoxWidth / (double) m_Range.GlobalLength;
				m_RenderStart  = m_BoxStart + (int)Math.Round(pixelsPerInt * (double)(m_Range.RangeStart - m_Range.GlobalStart));
				m_RenderLength = (int)Math.Round((double)m_Range.RangeLength * pixelsPerInt);
			}
			Refresh();
		}

		private void SetRangeFromRenderVariables()
		{
			// calc start
			double fract = (double)(m_RenderStart-m_BoxStart) / (double)m_BoxWidth;
			fract *= m_Range.GlobalLength;
			int start = m_Range.GlobalStart + (int)Math.Round(fract);

			// calc length
			fract = (double)m_RenderLength / (double)m_BoxWidth;
			fract *= m_Range.GlobalLength;
			int length = (int)Math.Round(fract);

			if( length <= 0 ) // occurs when we try and scroll past the end ...
			{
				length = 1;
				if( start > m_Range.GlobalEnd )
				{
					start = m_Range.GlobalEnd;
				}		
			}
			else if( start < m_Range.GlobalStart ) // occurs when we try and scroll past the start ...
			{
				start = m_Range.GlobalStart;
			}
			
			// only reset the range if it has changed
			if( start == m_PrevStart &&
				length == m_PrevLength )
			{
				Invalidate(); // repaint
			}
			else
			{
				m_Range.SetRange(this,start,length);
				m_PrevStart = start;
				m_PrevLength = length;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
	
			if( Enabled && m_Range != null ) 
			{
				m_Brush.Color = Color.Navy;
				e.Graphics.FillRectangle( m_Brush, m_BoxStart, 1, m_BoxWidth, Height - 2);

				if( m_MoveMode > 0 ) m_Brush.Color = Color.Gainsboro;
				else m_Brush.Color = Color.CornflowerBlue;

				e.Graphics.FillRectangle( m_Brush, m_RenderStart + 1, 1, m_RenderLength, Height - 2);
				e.Graphics.DrawRectangle( m_Pen, m_RenderStart, 0, m_RenderLength + 1, Height - 1);
				e.Graphics.DrawRectangle( m_Pen, 0, 0, Width - 1, Height - 1);
			}
			else 
			{
				m_Brush.Color = Color.DarkGray;
				e.Graphics.DrawRectangle( m_Pen, 0, 0, Width - 1, Height - 1);
				e.Graphics.FillRectangle( m_Brush, 1, 1, Width - 2, Height - 2);
			}
		}
		
		private void menu_ContinualUpdate_Click(object sender, System.EventArgs e)
		{
			menu_ContinualUpdate.Checked = !menu_ContinualUpdate.Checked;		
		}
		private void GraphicRangeSelection_Resize(object sender, System.EventArgs e)
		{
			m_BoxWidth = Width - (2 * m_BoxStart );
			UpdateDisplayFromRange();	
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			SetMenuEnabled();
			UpdateDisplayFromRange();
		}

		private void SetMenuEnabled()
		{
			bool enable = Enabled && m_AllowUserBarSelect && m_Range != null;
			for( int i = 0; i < contextMenu.MenuItems.Count; i++ )
			{
				contextMenu.MenuItems[i].Enabled = enable;
			}
		}

		#endregion

		#region Public Accessors
		public bool AllowUserBarSelect
		{
			get
			{
				return m_AllowUserBarSelect;
			}
			set
			{
				m_AllowUserBarSelect = value;
				SetMenuEnabled();
			}
		}
		#endregion

		#region Mouse Events
		private void GraphicRangeSelection_MouseLeave(object sender, System.EventArgs e)
		{
			Cursor = Cursors.Default;
			BackColor = Color.Firebrick;
			m_MoveMode = 0;
		}
		private void GraphicRangeSelection_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( m_Range != null && Enabled && m_AllowUserBarSelect )
			{
				// what move mode are we in ?
				if( m_MoveMode <= 0 )
				{
					// no drag mode is currently on
					// Cursor changes ?
					BackColor = Color.Firebrick;
					if( e.X < m_RenderStart )
					{
						// scroll left : m_MoveMode == -1 
						Cursor = Cursors.PanWest;
						BackColor = Color.OrangeRed;
					}
					else if( e.X > m_RenderStart + m_RenderLength )
					{
						// scroll right : m_MoveMode == -2
						Cursor = Cursors.PanEast;
						BackColor = Color.OrangeRed;
					}
					else if( e.X >= m_RenderStart && e.X <= m_RenderStart + 3 )
					{
						// mode 1 (left margin)
						Cursor = Cursors.VSplit;
					}
					else if ( ( e.X >= m_RenderStart + m_RenderLength - 2 && e.X <= m_RenderStart + m_RenderLength ) )
					{
						// mode 2 (right margin)
						Cursor = Cursors.VSplit;
					}
					else if( e.X >= m_RenderStart && e.X <= m_RenderStart + m_RenderLength )
					{
						// mode 3 (center)
						Cursor = Cursors.SizeAll;
					}
					else
					{
						// mode 0 (no movement)
						Cursor = Cursors.Default;	
					}

					return; // return, no setting of range variables
				}
				else if( m_MoveMode == 1 ) // we are flagged as moving the left margin
				{
					int currentEnd = m_RenderStart + m_RenderLength + 1;
					m_RenderStart = e.X;
					if( m_RenderStart < m_BoxStart ) m_RenderStart = m_BoxStart;
					else if( m_RenderStart >= currentEnd ) m_RenderStart = currentEnd-1;
					m_RenderLength = currentEnd - m_RenderStart - 1;
					Cursor = Cursors.VSplit;
				}
				else if( m_MoveMode == 2 ) // we are flagged as moving the right margin
				{
					int newEnd = e.X;
					if( newEnd >= (m_BoxStart+m_BoxWidth) ) newEnd = (m_BoxStart+m_BoxWidth) - 1;
					else if( m_RenderStart > newEnd ) newEnd = m_RenderStart;
					m_RenderLength = newEnd - m_RenderStart;
					Cursor = Cursors.VSplit;
				}
				else if( m_MoveMode == 3 ) // we are flagged as scrolling the selection area.
				{
					// mouse down sets m_CacheXOffset = e.X - m_RenderStart;
					m_RenderStart = e.X - m_CacheXOffset;
					if( m_RenderStart < m_BoxStart ) m_RenderStart = m_BoxStart;
					else if( m_RenderStart + m_RenderLength > (m_BoxStart+m_BoxWidth) ) m_RenderStart = (m_BoxStart+m_BoxWidth) - m_RenderLength;
					Cursor = Cursors.SizeAll;
				}

				if( menu_ContinualUpdate.Checked )
				{
					SetRangeFromRenderVariables();
					//UpdateDisplayFromRange(); // can call this to "snap" to the closest via an update call
				}
				else
				{
					Invalidate();
				}
			}
		}

		private void GraphicRangeSelection_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( e.Button == MouseButtons.Left && m_Range != null && Enabled && m_AllowUserBarSelect )
			{
			     if( e.X < m_RenderStart )
				 {
					 m_MoveMode = -1;
					 ScrollLeft();
				 }
				 else if( e.X > m_RenderStart + m_RenderLength )
				 {
					 m_MoveMode = -2;
					 ScrollRight();
				 }
				 else if( e.X >= m_RenderStart && e.X <= m_RenderStart + 3 )
				 {
					 m_MoveMode = 1;
				 }
				 else if ( ( e.X >= m_RenderStart + m_RenderLength - 2 && e.X <= m_RenderStart + m_RenderLength ) )
				 {
					 m_MoveMode = 2;
				 }
				 else if( e.X >= m_RenderStart && e.X <= m_RenderStart + m_RenderLength )
				 {
					 m_MoveMode = 3;
					 m_CacheXOffset = e.X - m_RenderStart;
				 }
				 else
				 {
					 m_MoveMode = 0;	
				 }
				Invalidate(); // repaint changes the colour of the scroll bar
			}		
		}

		private void GraphicRangeSelection_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( e.Button == MouseButtons.Left && m_Range != null && Enabled && m_AllowUserBarSelect )
			{
				m_MoveMode = 0;
				SetRangeFromRenderVariables();
				UpdateDisplayFromRange();
			}
		}

		private void GraphicRangeSelection_DoubleClick(object sender, System.EventArgs e)
		{
//			if( Enabled && m_Range != null && m_AllowUserBarSelect )
//			{
//				m_Range.SetRangeToFull( this );
//				UpdateDisplayFromRange();
//			}
		}

		#endregion

		#region menu range control

		private void menu_ExtendLeft_Click(object sender, System.EventArgs e)
		{
			if( m_Range.RangeStart > m_Range.GlobalStart + 1 )
			{
				m_Range.SetRange( this, m_Range.RangeStart - 1, m_Range.RangeLength + 1);
				UpdateDisplayFromRange();
			}
		}

		private void menu_ExtendRight_Click(object sender, System.EventArgs e)
		{
			if( (m_Range.RangeStart + m_Range.RangeLength) < (m_Range.GlobalStart + m_Range.GlobalLength) )
			{
				m_Range.SetRange( this, m_Range.RangeStart, m_Range.RangeLength + 1);
				UpdateDisplayFromRange();
			}
		}

		private void menu_RetractLeft_Click(object sender, System.EventArgs e)
		{
			if( m_Range.RangeLength > 1 )
			{
				m_Range.SetRange( this, m_Range.RangeStart + 1, m_Range.RangeLength - 1 );
				UpdateDisplayFromRange();
			}            		
		}

		private void menu_RetractRight_Click(object sender, System.EventArgs e)
		{
			if( m_Range.RangeLength > 1 )
			{
				m_Range.SetRange( this, m_Range.RangeStart, m_Range.RangeLength - 1 );
				UpdateDisplayFromRange();
			}  		
		}

		private void menu_ScrollLeft_Click(object sender, System.EventArgs e)
		{
			ScrollLeft();
		}

		private void ScrollLeft()
		{
			if( m_Range.RangeStart > m_Range.GlobalStart )
			{
				m_Range.SetRange( this, m_Range.RangeStart - 1, m_Range.RangeLength );
				UpdateDisplayFromRange();
			}
		}

		private void menu_ScrollRight_Click(object sender, System.EventArgs e)
		{
			ScrollRight();	
		}

		private void ScrollRight()
		{
			if( (m_Range.RangeStart + m_Range.RangeLength) < (m_Range.GlobalStart + m_Range.GlobalLength))
			{
				m_Range.SetRange( this, m_Range.RangeStart + 1, m_Range.RangeLength );
				UpdateDisplayFromRange();
			}	
		}

		private void menu_SelectAll_Click(object sender, System.EventArgs e)
		{
			// if( Enabled && m_AllowUserBarSelect ) // no need
			m_Range.SetRangeToFull( this );
			UpdateDisplayFromRange();	
		}

		#endregion

		#region Hide Vis Studio
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.menu_ContinualUpdate = new System.Windows.Forms.MenuItem();
			this.menu_Seperator = new System.Windows.Forms.MenuItem();
			this.menu_SelectAll = new System.Windows.Forms.MenuItem();
			this.menu_ScrollLeft = new System.Windows.Forms.MenuItem();
			this.menu_ScrollRight = new System.Windows.Forms.MenuItem();
			this.menu_ExtendLeft = new System.Windows.Forms.MenuItem();
			this.menu_ExtendRight = new System.Windows.Forms.MenuItem();
			this.menu_RetractLeft = new System.Windows.Forms.MenuItem();
			this.menu_RetractRight = new System.Windows.Forms.MenuItem();
			// 
			// contextMenu
			// 
			this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.menu_ContinualUpdate,
																						this.menu_Seperator,
																						this.menu_SelectAll,
																						this.menu_ScrollLeft,
																						this.menu_ScrollRight,
																						this.menu_ExtendLeft,
																						this.menu_ExtendRight,
																						this.menu_RetractLeft,
																						this.menu_RetractRight});
			// 
			// menu_ContinualUpdate
			// 
			this.menu_ContinualUpdate.Checked = true;
			this.menu_ContinualUpdate.Index = 0;
			this.menu_ContinualUpdate.Text = "Update Range on Scroll";
			this.menu_ContinualUpdate.Click += new System.EventHandler(this.menu_ContinualUpdate_Click);
			// 
			// menu_Seperator
			// 
			this.menu_Seperator.Index = 1;
			this.menu_Seperator.Text = "-";
			// 
			// menu_SelectAll
			// 
			this.menu_SelectAll.Index = 2;
			this.menu_SelectAll.Text = "Select &All";
			this.menu_SelectAll.Click += new System.EventHandler(this.menu_SelectAll_Click);
			// 
			// menu_ScrollLeft
			// 
			this.menu_ScrollLeft.Index = 3;
			this.menu_ScrollLeft.Text = "Scroll Left";
			this.menu_ScrollLeft.Click += new System.EventHandler(this.menu_ScrollLeft_Click);
			// 
			// menu_ScrollRight
			// 
			this.menu_ScrollRight.Index = 4;
			this.menu_ScrollRight.Text = "Scroll Right";
			this.menu_ScrollRight.Click += new System.EventHandler(this.menu_ScrollRight_Click);
			// 
			// menu_ExtendLeft
			// 
			this.menu_ExtendLeft.Index = 5;
			this.menu_ExtendLeft.Text = "Extend Left";
			this.menu_ExtendLeft.Click += new System.EventHandler(this.menu_ExtendLeft_Click);
			// 
			// menu_ExtendRight
			// 
			this.menu_ExtendRight.Index = 6;
			this.menu_ExtendRight.Text = "Extend Right";
			this.menu_ExtendRight.Click += new System.EventHandler(this.menu_ExtendRight_Click);
			// 
			// menu_RetractLeft
			// 
			this.menu_RetractLeft.Index = 7;
			this.menu_RetractLeft.Text = "Retract Left";
			this.menu_RetractLeft.Click += new System.EventHandler(this.menu_RetractLeft_Click);
			// 
			// menu_RetractRight
			// 
			this.menu_RetractRight.Index = 8;
			this.menu_RetractRight.Text = "Retract Right";
			this.menu_RetractRight.Click += new System.EventHandler(this.menu_RetractRight_Click);
			// 
			// GraphicRangeSelection
			// 
			this.BackColor = System.Drawing.Color.Firebrick;
			this.ContextMenu = this.contextMenu;
			this.Enabled = false;
			this.Name = "GraphicRangeSelection";
			this.Size = new System.Drawing.Size(160, 16);
			this.Resize += new System.EventHandler(this.GraphicRangeSelection_Resize);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GraphicRangeSelection_MouseUp);
			this.DoubleClick += new System.EventHandler(this.GraphicRangeSelection_DoubleClick);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GraphicRangeSelection_MouseMove);
			this.MouseLeave += new System.EventHandler(this.GraphicRangeSelection_MouseLeave);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GraphicRangeSelection_MouseDown);

		}
		#endregion
		#endregion
	}
}
