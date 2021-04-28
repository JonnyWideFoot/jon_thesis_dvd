

// JON :
// taken from : http://janus.ntsj.com/programming/code_view.php

// Renderer.cs
//
// Skeletal drop-in implementation of an OpenGL control
// class using the Tao framework. This control integrates
// with Windows Forms classes for use in applications that
// include non-rendered content as well as rendered content.
//

// LIBRARY
//
// Drop this class into a new or existing library. Compilation
// needs reference to the Tao.OpenGl.dll and Tao.Platform.Windows.dll
// either via IDE or command-line referencing.

// LICENSING
//
// This source code is released under no restrictions.
//
// Tao.OpenGl and Tao.Platform.Windows upon which this
// class is based are covered in their own license.
// Refer to http://www.randyridge.com/Tao for more
// more information about these libraries and the license
// under which they are distrubuted.

#if DEBUG
#define GL_DEBUG
#endif
//#define GL_DEBUG

using Tao.OpenGl;
using Tao.Platform.Windows;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

namespace Com.Ntsj.Janus.Public
{
	/// 
	/// This class was developed for on-demand (i.e. not continuous)
	/// rendering. It responds to refresh events as a control should
	/// but does not refresh according to a timer as might be expected
	/// from an OpenGL renderer. Neither does it necessarily monopolize
	/// the event loop. In this way, such a control can drop into an
	/// existing windows forms application fairly easily.
	/// 
	/// 
	/// Finally, this class is intended as a starting point for
	/// applications mixing windows forms and OpenGL rendering. This
	/// class may facilitate immediate addition of OpenGL to an
	/// application, however, it will be important to revisit this
	/// control at a later date to customize it for specific needs.
	/// Indeed it may be useful at some point to redesign the rendering
	/// component completely, even reimplementing the functionality
	/// provided by the Tao.Platform.Windows.SimpleOpenGlControl class.
	/// 
	public class Renderer : Tao.Platform.Windows.SimpleOpenGlControl
	{
		readonly EventHandler ParentResizeEventHandler;

		public new Control Parent
		{
			get { return base.Parent; }
			set
			{
				if (value == null)
				{
					return;
				}
				// Avoid leaving a trail of event handlers
				base.Parent.Resize -= ParentResizeEventHandler;
				base.Parent = value;
				base.Parent.Resize += ParentResizeEventHandler;
			}
		}

		public Renderer()
		{
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

			// JANUS
			// Experienced some cases in which parent control was resized
			// but OpenGL control wasn't properly redrawn. This allows
			// the renderer to explicitly post its own invalidation message.
			this.ParentResizeEventHandler = new EventHandler(ParentOfRenderer_Resize);

			this.InitializeContexts();
			this.InitializeView();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.Parent != null)
				{
					this.Parent.Resize -= ParentResizeEventHandler;
				}
			}
			base.Dispose(disposing);
		}

		bool ResetViewportAndProjection()
		{
			// JANUS
			// ToDo: determine why this code got here, I'm not quite
			// sure how it prevents a divide-by-zero exception as the
			// original commenting indicated.
			#region Prevent a Divide-By-Zero Exception ?
			if (Height == 0) 
			{
				// By making the height at least one
				Height = 1;
			}
			#endregion

			double centerX = Width  / 2.0;
			double centerY = Height / 2.0;

			// Reset the current viewport
			Gl.glViewport(0, 0, Width, Height);

			// Select the projection matrix
			Gl.glMatrixMode(Gl.GL_PROJECTION);

			// Reset the projection matrix to identity
			Gl.glLoadIdentity();

			// Select an orthographic projection volume
			Gl.glOrtho(-centerX, centerX, -centerY, centerY, 0.1, 1000.0);

			Gl.glMatrixMode(Gl.GL_MODELVIEW);

			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            
			Gl.glLoadIdentity();
			Glu.gluLookAt(0.0, 0.0, 10.0,    0.0, 0.0, 0.0,    0.0, 1.0, 0.0);

			return true;
		}

		protected void DrawFrame(object sender, PaintEventArgs e)
		{
			ResetViewportAndProjection();

			// render scene frame...
		}

		protected void ReshapeView(object sender, EventArgs e)
		{
			ResetViewportAndProjection();
		}

		protected virtual bool InitializeView()
		{
#if GL_DEBUG
			Gl.glClearColor(1.0f, 1.0f, 0.0f, 0.0f);//Yellow is highly visible
#else
            Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);//Black is more professional
#endif
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			Gl.glDepthFunc(Gl.GL_LEQUAL);

			Gl.glShadeModel(Gl.GL_SMOOTH);

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
	}
}
