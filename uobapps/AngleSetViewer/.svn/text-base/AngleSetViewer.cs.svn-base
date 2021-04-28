using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using UoB.Core.MoveSets.AngleSets;

namespace AngleSetViewer
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class AngleSetViewer : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private UoB.CoreControls.Controls.AngleSet_Ramachandran angleSet_Ramachandran1;
		private System.Windows.Forms.MenuItem menu_Open;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AngleSetViewer()
		{
			InitializeComponent();
		}

		private void TryLoadAngleSet( string fileName )
		{
			AngleSet angSet = new AngleSet( fileName );
            angleSet_Ramachandran1.angleSet = angSet;            			
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menu_Open = new System.Windows.Forms.MenuItem();
			this.angleSet_Ramachandran1 = new UoB.CoreControls.Controls.AngleSet_Ramachandran();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menu_Open});
			this.menuItem1.Text = "&File";
			// 
			// menu_Open
			// 
			this.menu_Open.Index = 0;
			this.menu_Open.Text = "&Open";
			this.menu_Open.Click += new System.EventHandler(this.menu_Open_Click);
			// 
			// angleSet_Ramachandran1
			// 
			this.angleSet_Ramachandran1.angleSet = null;
			this.angleSet_Ramachandran1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.angleSet_Ramachandran1.Location = new System.Drawing.Point(0, 0);
			this.angleSet_Ramachandran1.Name = "angleSet_Ramachandran1";
			this.angleSet_Ramachandran1.Size = new System.Drawing.Size(392, 353);
			this.angleSet_Ramachandran1.TabIndex = 0;
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "AngleSets|*.angleset";
			// 
			// AngleSetViewer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(392, 353);
			this.Controls.Add(this.angleSet_Ramachandran1);
			this.Menu = this.mainMenu1;
			this.Name = "AngleSetViewer";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
		#endregion

		private void menu_Open_Click(object sender, System.EventArgs e)
		{
			if( DialogResult.OK == openFileDialog.ShowDialog(this) )
			{
				TryLoadAngleSet( openFileDialog.FileName );
			}
		}
	}
}
