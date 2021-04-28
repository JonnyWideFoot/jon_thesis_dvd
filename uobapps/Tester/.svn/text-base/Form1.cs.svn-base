using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

using UoB.CoreControls.Controls;
using UoB.Core;
using UoB.Core.FileIO.PDB;
using UoB.Core.Structure;
using UoB.Core.ForceField;
using UoB.CoreControls.ToolWindows;



namespace UoB.Tester
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			InitializeComponent();
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
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Name = "Form1";
			this.Text = "Form1";
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Form1 parentForm = new Form1();


			Random r = new Random();
			FastRandom rF = new FastRandom();

			int max = 1000;

			StreamWriter rw = new StreamWriter(@"c:\randomS.csv");
			DateTime dtR_B = DateTime.Now;
       		for( int i = 0; i < max; i++ )
			{
				for( int j = 0; j < max; j++ )
				{
					r.NextDouble();
				}
			}
			DateTime dtR_E = DateTime.Now;
			rw.Close();

			rw = new StreamWriter(@"c:\randomF.csv");
			DateTime dtRF_B = DateTime.Now;
			for( int i = 0; i < max; i++ )
			{
				for( int j = 0; j < max; j++ )
				{
					rF.NextDouble();
				}
			}
			DateTime dtRF_E = DateTime.Now;
			rw.Close();

			Console.WriteLine( dtR_E - dtR_B );
			Console.WriteLine( dtRF_E - dtRF_B );

//			FFManager ffMan = FFManager.Instance;
//			ffMan.FinaliseStage2();
//			string filename = @"C:\_DaveRelease\shared\PDB\1BW8.pdb";
//			PDB file = new PDB( filename, true );
//			PS_Ramachandran ps = new PS_Ramachandran();
//			ps.particleSystem = file.particleSystem;
//			ps.Parent = parentForm;
//			ps.Dock = DockStyle.Fill;
//			ps.Show();

            
//			GraphicRangeSelection ps = new GraphicRangeSelection();
//			ps.Parent = parentForm;
//			ps.Dock = DockStyle.Fill;
//			ps.Show();
//
//			ps.Range = new UoB.Core.Primitives.IntRange_EventFire( -2, 4, -5, 50);
//
//			parentForm.m_TestChild = ps;
//			ps.KeyUp += new System.Windows.Forms.KeyEventHandler(parentForm.Form1_KeyUp);
//
//			Application.Run(parentForm);
		}

		public Control m_TestChild = null;

		public void Form1_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if( m_TestChild != null && e.KeyCode == Keys.Enter )
			{
				m_TestChild.Enabled = !m_TestChild.Enabled;
			}		
		}
	}
}
