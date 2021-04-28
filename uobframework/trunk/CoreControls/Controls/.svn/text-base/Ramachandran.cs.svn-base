using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using NPlot;
using NPlot.Windows;

namespace UoB.CoreControls.Controls
{
	/// <summary>
	/// Override to decide what angle pairs to plot ...
	/// i.e.
	/// PS_Ramachandran
	/// and
	/// AngleSetRamachadran
	/// </summary>
	public class Ramachandran : System.Windows.Forms.UserControl
	{
		private System.ComponentModel.Container components = null;
		protected NPlot.Windows.PlotSurface2D plotSurface;
		protected double[] m_PhiAngles;
		protected double[] m_PsiAngles;

		public Ramachandran()
		{
			InitializeComponent();
			m_PhiAngles = new double[] { 0.0f };
			m_PsiAngles = new double[] { 0.0f };
			SetupPage();
		}

		private void SetupPage()
		{
			plotSurface = new NPlot.Windows.PlotSurface2D();
			plotSurface.Dock = DockStyle.Fill;
			plotSurface.Parent = this;

			plotSurface.Title = "Ramachandran";
			LinearAxis xAxis = new LinearAxis( -180.0, 180.0 );
			xAxis.Label = "PhiAngles";
			xAxis.LargeTickStep = 60.0;
			xAxis.FlipTicksLabel = true;
			xAxis.NumberOfSmallTicks = 5;
			LinearAxis yAxis = new LinearAxis( -180.0, 180.0 );
			yAxis.Label = "PsiAngles";
			yAxis.LargeTickStep = 60.0;
			yAxis.NumberOfSmallTicks = 5;

			plotSurface.YAxis1 = yAxis;
			plotSurface.XAxis1 = xAxis;

			plotSurface.Add(new HorizontalLine(0.0, Color.DarkBlue));
			plotSurface.Add(new VerticalLine(0.0, Color.DarkBlue));

			plotSurface.Add(new HorizontalLine(60.0, Color.LightBlue));
			plotSurface.Add(new HorizontalLine(120.0, Color.LightBlue));
			plotSurface.Add(new HorizontalLine(-60.0, Color.LightBlue));
			plotSurface.Add(new HorizontalLine(-120.0, Color.LightBlue));

			plotSurface.Add(new VerticalLine(60.0, Color.LightBlue));
			plotSurface.Add(new VerticalLine(120.0, Color.LightBlue));
			plotSurface.Add(new VerticalLine(-60.0, Color.LightBlue));
			plotSurface.Add(new VerticalLine(-120.0, Color.LightBlue));

			plotSurface.Refresh();	
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// Ramachandran
			// 
			this.Name = "Ramachandran";
			this.Size = new System.Drawing.Size(368, 352);

		}
		#endregion
	}
}
