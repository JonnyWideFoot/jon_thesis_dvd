using System;
using System.Drawing;

using Tao.OpenGl;
using Tao.Platform.Windows;

namespace UoB.CoreControls.OpenGLView
{
	/// <summary>
	/// Summary description for GLEnvironment.
	/// </summary>
	public class GLEnvironment
	{
		public uint[] texture;		// texture
		public float[] LightAmbient =  {0.5f, 0.5f, 0.5f, 1.0f};
		public float[] LightDiffuse =  {1.0f, 1.0f, 1.0f, 1.0f};
		public float[] LightPosition = {0.0f, 0.0f, 2.0f, 1.0f};

		public GLEnvironment()
		{
			texture = new uint[1];		// storage for texture
		}

		public void GLInitialise()
		{
			Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_AMBIENT,  LightAmbient);	// Setup The Ambient Light
			Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE,  LightDiffuse);	// Setup The Diffuse Light
			Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, LightPosition);	// Position The Light
			Gl.glEnable(Gl.GL_LIGHT1);

			if( System.IO.File.Exists( "water.bmp" ) )
			{
				Bitmap image = new Bitmap("water.bmp");
				image.RotateFlip(RotateFlipType.RotateNoneFlipY);
				System.Drawing.Imaging.BitmapData bitmapdata;
				Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

				bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, 
					System.Drawing.Imaging.PixelFormat.Format24bppRgb);

				Gl.glGenTextures(1, texture);
				Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[0]);
				Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, (int)Gl.GL_RGB8, image.Width, image.Height,
					0, Gl.GL_BGR_EXT, Gl.GL_UNSIGNED_BYTE, bitmapdata.Scan0);
				Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);		// Linear Filtering
				Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);		// Linear Filtering

				image.UnlockBits(bitmapdata);
				image.Dispose();
			}

			// interesting fog for the cube 
//			Gl.glClearColor(0.5f,0.5f,0.5f,1.0f);
//			float[] fogColor = {0.5f, 0.5f, 0.5f, 1.0f};		// Fog Color
//			Gl.glFogi(Gl.GL_FOG_MODE, Gl.GL_LINEAR );		// Fog Mode
//			Gl.glFogfv(Gl.GL_FOG_COLOR, fogColor);			// Set Fog Color
//			Gl.glFogf(Gl.GL_FOG_DENSITY, 0.25f);				// How Dense Will The Fog Be
//			Gl.glHint(Gl.GL_FOG_HINT, Gl.GL_NICEST );			// Fog Hint Value
//			Gl.glFogf(Gl.GL_FOG_START, 4.0f);				// Fog Start Depth
//			Gl.glFogf(Gl.GL_FOG_END, 6.0f);				// Fog End Depth
//			Gl.glEnable(Gl.GL_FOG);					// Enables GL_FOG
		}
	}
}
