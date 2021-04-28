using System;
using System.Threading;
using System.Timers;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;

using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;

using UoB.Core.Tools;
using UoB.Core;
using UoB.Core.Structure;
using UoB.Core.Primitives;
using UoB.CoreControls.OpenGLView.RenderManagers;

namespace UoB.CoreControls.OpenGLView
{
	public class GLView : SimpleOpenGlControl 
	{ 
		private Perspective m_Perspective;
		private ArrayList m_RenderList = new ArrayList(5);
		private Axis m_Axis;
		private GLEnvironment m_Environment = new GLEnvironment();

		private CoreIni m_CoreIni = CoreIni.Instance;
		
		private bool m_InvertZoom = true; // default
		private bool m_RequireShiftForZRotation = false;
		private bool m_ShiftIsDown = false;
		private double XStartPoint = 0.0f;
		private double YStartPoint = 0.0f;
		private double ZStartPoint = 0.0f;
		private Point nonRotationStartPoint;
		private bool goLMB = false;
		private bool goRMB = false;

		#region Parent Redraw linking
		readonly EventHandler ParentResizeEventHandler;
		public new Control Parent
		{
			get { return base.Parent; }
			set
			{
				if( base.Parent != null )
				{
					base.Parent.Resize -= ParentResizeEventHandler;
				}
				base.Parent = value;
				if( base.Parent != null )
				{
					base.Parent.Resize += ParentResizeEventHandler;
				}
			}
		}
		#endregion

		public GLView() : base()
		{
			getSettings();

			m_Axis = new Axis( this );

			m_Perspective = new Perspective( 100 );
			m_Perspective.PerspectiveUpdate += new EventHandler(m_Perspective_PerspectiveUpdate);
		 
			MouseDown += new System.Windows.Forms.MouseEventHandler(doMouseDownEvent);
			MouseMove += new System.Windows.Forms.MouseEventHandler(doMouseMoveEvent);
			MouseUp += new System.Windows.Forms.MouseEventHandler(doMouseUpEvent);
			MouseLeave += new EventHandler(GLView_MouseLeave);
			MouseWheel += new MouseEventHandler(GLView_MouseWheel);

			KeyDown += new KeyEventHandler(GLView_KeyDown);
			KeyUp += new KeyEventHandler(GLView_KeyUp);

			// BEGIN JANUS
			base.AccumBits   = (byte) 0;
			base.ColorBits   = (byte) 32;
			base.DepthBits   = (byte) 0;
			base.StencilBits = (byte) 0;

			base.BackColor = System.Drawing.Color.Black;
            
			base.AutoFinish      = true;
			base.AutoCheckErrors = false;
			base.AutoSwapBuffers = true;
			base.AutoMakeCurrent = true;

			base.Dock = DockStyle.Fill;
			base.Size = new System.Drawing.Size(292, 266);
			base.Name = "Renderer";
			base.Location = new Point(0, 0);
			base.TabIndex = 0;
            
			base.Paint  += new PaintEventHandler(DrawFrame);
			base.Resize += new EventHandler(ReshapeView);

			// JANUS COMMENT
			// Experienced some cases in which parent control was resized
			// but OpenGL control wasn't properly redrawn. This allows
			// the renderer to explicitly post its own invalidation message.
			this.ParentResizeEventHandler = new EventHandler(ParentOfRenderer_Resize);

			this.InitializeContexts();
			this.InitializeView();
			// END JANUS
		}

		private void m_Perspective_PerspectiveUpdate(object sender, EventArgs e)
		{
			// redraw the current view ...
			Refresh();
		}


		public Perspective perspective
		{
			get
			{
				return m_Perspective;
			}
		}

		public GLEnvironment GLEnvironment
		{
			get
			{
				return m_Environment;
			}
		}

		#region UoBCore Settings
		private void getSettings()
		{
			string key = "InvertZoom";
			if ( m_CoreIni.ContainsKey( key ) )
			{
				try
				{
					m_InvertZoom = bool.Parse( m_CoreIni.ValueOf( key ) );
				}
				catch // any error in bool parsing - i.e. the string is buggered
				{
					m_CoreIni.AddDefinition( key, m_InvertZoom.ToString() );
				}
			}
			else
			{
				m_CoreIni.AddDefinition( key, m_InvertZoom.ToString() );
			}

			key = "RequireShiftForZRotation";
			if ( m_CoreIni.ContainsKey( key ) )
			{
				try
				{
					m_RequireShiftForZRotation = bool.Parse( m_CoreIni.ValueOf( key ) );
				}
				catch // any error in bool parsing - i.e. the string is buggered
				{
					m_CoreIni.AddDefinition( key, m_RequireShiftForZRotation.ToString() );
				}
			}
			else
			{
				m_CoreIni.AddDefinition( key, m_RequireShiftForZRotation.ToString() );
			}

		}
		#endregion

		#region Render Object Control
		public void AddRenderObject( IRenderable ir )
		{
			m_RenderList.Add( ir );
		}

		public void RemoveRenderObject( IRenderable ir )
		{
			if ( m_RenderList.Contains( ir ) )
			{
				m_RenderList.Remove( ir );
			}
		}

		#endregion
		
		#region Simple GL Rendering (JANUS)

		bool ResetViewportAndProjection()
		{
			Gl.glViewport( 0, 0, Width, Height);

			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			Glu.gluPerspective(45.0f, (double)Size.Width /(double) Size.Height, 0.1f, 100.0f);	
			//Gl.glOrtho( 0, this.ClientSize.Width, this.ClientSize.Height, 0 , 0.1f, 100.0f );
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();

//			double centerX = Width  / 2.0;
//			double centerY = Height / 2.0;
//
//			// Reset the current viewport
//			Gl.glViewport(0, 0, Width, Height);
//
//			// Select the projection matrix
//			Gl.glMatrixMode(Gl.GL_PROJECTION);
//
//			// Reset the projection matrix to identity
//			Gl.glLoadIdentity();
//
//			// Select an orthographic projection volume
//			Gl.glOrtho(-centerX, centerX, -centerY, centerY, 0.1, 1000.0);
//
//			Gl.glMatrixMode(Gl.GL_MODELVIEW);
//
//			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
//            
//			Gl.glLoadIdentity();
//			Glu.gluLookAt(0.0, 0.0, 10.0,    0.0, 0.0, 0.0,    0.0, 1.0, 0.0);

			return true;
		}


		// my hacked 4x4 view matrix for integration with the UoB.Core 3x3 Rotation class
		double[]	Transform =  {  1.0,  0.0,  0.0,  0.0, // an identity matrix
									0.0,  1.0,  0.0,  0.0,
									0.0,  0.0,  1.0,  0.0,
									0.0,  0.0,  0.0,  1.0 };

		protected void DrawFrame(object sender, PaintEventArgs e)
		{
			ResetViewportAndProjection();

			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);	// Clear The Screen And The Depth Buffer
			
			Gl.glLoadIdentity();
			
			double[,] rot = m_Perspective.GlobalRotMat.r;

			Transform[0] = rot[0,0];
			Transform[1] = rot[1,0];
			Transform[2] = rot[2,0];
			Transform[4] = rot[0,1];
			Transform[5] = rot[1,1];
			Transform[6] = rot[2,1];
			Transform[8] = rot[0,2];
			Transform[9] = rot[1,2];
			Transform[10] = rot[2,2];
			Transform[12] = -4.0f;
			Transform[13] = 2.0f;
			Transform[14] = -8.0f;
			Gl.glLoadMatrixd( Transform );

			m_Axis.GLDraw();

			Gl.glLoadIdentity();

			Transform[12] = m_Perspective.RenderOffset.xFloat;
			Transform[13] = m_Perspective.RenderOffset.yFloat;
			Transform[14] = -6.0f;
			Gl.glLoadMatrixd( Transform );

			float zoomFactor = m_Perspective.ZoomPercentage * 0.0015f;
			Gl.glScalef( zoomFactor, zoomFactor, zoomFactor );

			for( int i = 0; i < m_RenderList.Count; i++ )
			{
				IRenderable ir = (IRenderable) m_RenderList[i];
				ir.GLDraw();                
			}

		}

		protected void ReshapeView(object sender, EventArgs e)
		{
			ResetViewportAndProjection();
		}

		protected virtual bool InitializeView()
		{
			Gl.glEnable(Gl.GL_LINE_SMOOTH);
			Gl.glShadeModel(Gl.GL_SMOOTH);							// Enable Smooth Shading
			Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);				// Black Background
			Gl.glClearDepth(1.1f);									// Depth Buffer Setup
			//Gl.glEnable(Gl.GL_DEPTH_TEST);							// Enables Depth Testing
			Gl.glDepthFunc(Gl.GL_LEQUAL);								// The Type Of Depth Testing To Do
			Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);	// Really Nice Perspective Calculations

			m_Environment.GLInitialise();

			return ResetViewportAndProjection();
		}

		void ParentOfRenderer_Resize(object sender, EventArgs e)
		{
			if (this.Parent != sender)
			{
				return;
			}
			this.Invalidate();
		}

		#endregion	

		#region User event control

		private void doMouseDownEvent(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			//Debug.WriteLine("MouseDown : LMB - " + goLMB.ToString() + " : RMB - " + goRMB.ToString());

			nonRotationStartPoint = new Point(e.X, e.Y);

			if (e.Button==System.Windows.Forms.MouseButtons.Left) 
			{
				goLMB = true;
				XStartPoint = e.X;
				YStartPoint = e.Y;
			}

			if (e.Button==System.Windows.Forms.MouseButtons.Right) 
			{
				goRMB = true;
				ZStartPoint = e.X;
			}
		}

		private void doMouseMoveEvent(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (goLMB && goRMB)
			{
				float zoomModY = (nonRotationStartPoint.Y - e.Y);
				if ( m_InvertZoom )
				{
					zoomModY *= -1;
				}
				m_Perspective.ZoomPercentage += zoomModY;
				if ( m_Perspective.ZoomPercentage < 1.0f ) 
				{
					m_Perspective.ZoomPercentage = 1.0f;
				}

				if ( m_RequireShiftForZRotation )
				{
					if( m_ShiftIsDown )
					{
						m_Perspective.setAngleChanges( e.X, ZStartPoint );
					}
				}
				else
				{
					m_Perspective.setAngleChanges( e.X, ZStartPoint );
				}
			}
			if (goLMB && !goRMB)
			{
				m_Perspective.setAngleChanges( e.X, e.Y, XStartPoint, YStartPoint );
			}
			if (!goLMB && goRMB)
			{
				float translateModX = (nonRotationStartPoint.X - e.X) * -0.003f;
				float translateModY = (nonRotationStartPoint.Y - e.Y) * 0.003f;

				m_Perspective.RenderTranslate( translateModX, translateModY );
			}

			nonRotationStartPoint = new Point(e.X, e.Y); // reset the start point, but only for non-rotations
		}

		private void doMouseUpEvent(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button==System.Windows.Forms.MouseButtons.Left) 
			{
				goLMB = false;
				XStartPoint = e.X;
				YStartPoint = e.Y;
			}
			else if (e.Button==System.Windows.Forms.MouseButtons.Right) 
			{
				goRMB = false;
				ZStartPoint = e.X;
			}
			m_Perspective.ApplyChanges();
			
			nonRotationStartPoint.X = e.X; 
			nonRotationStartPoint.Y = e.Y;
		}

		private void GLView_MouseLeave(object sender, EventArgs e)
		{
			////Debug.WriteLine("MouseLeave : LMB - " + goLMB.ToString() + " : RMB - " + goRMB.ToString());
			goRMB = false;
			goLMB = false;
			m_Perspective.ApplyChanges();
			nonRotationStartPoint.X = 0;
			nonRotationStartPoint.Y = 0;
			XStartPoint = 0;
			YStartPoint = 0;
			ZStartPoint = 0;
		}

		private void GLView_KeyDown(object sender, KeyEventArgs e)
		{
			if( e.KeyCode == Keys.Shift )
			{
				m_ShiftIsDown = true;
           	}
		}

		private void GLView_KeyUp(object sender, KeyEventArgs e)
		{
			if( e.KeyCode == Keys.Shift )
			{
				m_ShiftIsDown = false;
			}
		}

		private void GLView_MouseWheel(object sender, MouseEventArgs e)
		{
			float factor = ((float)e.Delta) / 20.0f;
            m_Perspective.ZoomPercentage -= factor;
			if ( m_Perspective.ZoomPercentage < 1.0f ) 
			{
				m_Perspective.ZoomPercentage = 1.0f;
			}
		}
		#endregion
	}
}
