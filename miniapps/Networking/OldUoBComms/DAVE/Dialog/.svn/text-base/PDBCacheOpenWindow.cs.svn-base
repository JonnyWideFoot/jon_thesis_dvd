using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

using UoB.DAVE;
using UoB.Research;


namespace UoB.DAVE.Dialog
{
	/// <summary>
	/// Summary description for PDBCacheOpen.
	/// </summary>
	public class PDBCacheOpenWindow : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ImageList largeIL;
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button button_Close;
		private System.ComponentModel.IContainer components;

		public PDBCacheOpenWindow()
		{
			InitializeComponent();
			Init();
		}

		private void Init()
		{
			string directory = UoBInit.Instance.DefaultSharedPath + @"PDB\";
			String[] theFiles = Directory.GetFiles(directory);
			foreach (string theFile in theFiles)
			{
				string[] fileComponents = theFile.Split('\\');
				string fileName = fileComponents[fileComponents.Length-1];
				ListViewItem theItem = new ListViewItem(fileName, 0);
				theItem.Tag = theFile;
				//theItem.
				listView.Items.Add(theItem);
			}
		}

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PDBCacheOpenWindow));
			this.button1 = new System.Windows.Forms.Button();
			this.largeIL = new System.Windows.Forms.ImageList(this.components);
			this.listView = new System.Windows.Forms.ListView();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel1 = new System.Windows.Forms.Panel();
			this.button_Close = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(8, 8);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(136, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "&Open Selected Files";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// largeIL
			// 
			this.largeIL.ImageSize = new System.Drawing.Size(32, 32);
			this.largeIL.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("largeIL.ImageStream")));
			this.largeIL.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// listView
			// 
			this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView.LargeImageList = this.largeIL;
			this.listView.Location = new System.Drawing.Point(0, 40);
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(488, 285);
			this.listView.TabIndex = 0;
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(0, 40);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(488, 3);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.button_Close);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(488, 40);
			this.panel1.TabIndex = 2;
			// 
			// button_Close
			// 
			this.button_Close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Close.Location = new System.Drawing.Point(400, 8);
			this.button_Close.Name = "button_Close";
			this.button_Close.TabIndex = 1;
			this.button_Close.Text = "&Close";
			this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
			// 
			// PDBCacheOpenWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(488, 325);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.listView);
			this.Controls.Add(this.panel1);
			this.Name = "PDBCacheOpenWindow";
			this.Text = "PDB Cache List";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		private ArrayList m_Files;
		public string[] FileList
		{
			get
			{
				if ( m_Files != null )
				{
					return (string[]) m_Files.ToArray( typeof( string ) );
				}
				else
				{
					return new string[0];
				}
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			m_Files = new ArrayList();

			foreach ( ListViewItem theItem in listView.SelectedItems )
			{
				m_Files.Add ( (string)theItem.Tag );
			}

			Close();
		}

		private void button_Close_Click(object sender, System.EventArgs e)
		{
			Close();
		}
	}
}
