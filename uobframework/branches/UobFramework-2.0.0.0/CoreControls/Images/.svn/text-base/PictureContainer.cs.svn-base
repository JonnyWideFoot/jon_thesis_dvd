using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace UoB.CoreControls.Images
{
	/// <summary>
	/// Summary description for PictureContainer.
	/// </summary>
	public sealed class PictureContainer : System.Windows.Forms.Form
	{
		private static readonly PictureContainer instance = new PictureContainer();
   
		public static PictureContainer Instance
		{
			get 
			{
				return instance; 
			}
		}

		private PictureContainer()
		{
			InitializeComponent();
		}

		public System.Windows.Forms.ImageList IconList;
		private System.ComponentModel.IContainer components;


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PictureContainer));
			this.IconList = new System.Windows.Forms.ImageList(this.components);
			// 
			// IconList
			// 
			this.IconList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.IconList.ImageSize = new System.Drawing.Size(16, 16);
			this.IconList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IconList.ImageStream")));
			this.IconList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// PictureContainer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(112, 45);
			this.Name = "PictureContainer";
			this.Text = "PictureContainer";

		}
		#endregion
	}
}
