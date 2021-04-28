using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;

using UoB.Core;
using UoB.Core.Tools;
using UoB.Core.Data;
using UoB.Core.Data.Graphing;

using NPlot;

namespace UoB.CoreControls.ToolWindows
{
	public class DM_GraphWindow : Form
	{
		private System.Windows.Forms.MenuItem menu_SetAxisRange;
		private NPlot.Windows.PlotSurface2D plotSurface;
		private System.Windows.Forms.ContextMenu m_ContextMenu;
		private System.Windows.Forms.MenuItem menu_DrawProgress;
		private System.Windows.Forms.MenuItem menu_CopyData;
		private System.Windows.Forms.MenuItem menu_CopyImage;
		private System.Windows.Forms.MenuItem menu_Print;
		private System.Windows.Forms.MenuItem menu_PrintPreview;
		private System.Windows.Forms.MenuItem menu_PlotPoint;
		private System.Windows.Forms.MenuItem menu_PlotLine;
		private System.Windows.Forms.MenuItem menu_Divider1;
		private System.Windows.Forms.MenuItem menu_CopyDivider;
		private System.Windows.Forms.MenuItem menu_PlotDivider;	

		private Graph m_Graph = null;
		private DM_GraphBounds m_Bounds = null;

		private LinePlot m_LinePlot = null;
		private PointPlot m_PointPlot = null;

		private PointPlot m_Progressline = null;
		private LabelPointPlot m_ProgressLabel = null;
		private string[] m_ProgressText = null;
		private float[] m_ProgressXData = new float[] { 0.0f };
		private float[] m_ProgressYData = new float[] { 0.0f };
		
		public DM_GraphWindow()
		{
			InitializeComponent();
			m_Bounds = new DM_GraphBounds();
			SetupPage();
			SetName("No Graph.");
		}

		private void SetName( string name )
		{
			Text = name;
			plotSurface.Title = name;
		}

		private void SetupPage()
		{
			LinearAxis xAxis = new LinearAxis();
			xAxis.WorldMin = 0.0;
			xAxis.WorldMax = 10.0;
			plotSurface.XAxis1 = xAxis;

			LinearAxis yAxis = new LinearAxis();
			yAxis.WorldMin = 0.0;
			yAxis.WorldMax = 10.0;
			plotSurface.YAxis1 = yAxis;

			float[] tempData = new float[] { 3.0f, 40f, 5.0f, 6.0f, 7.0f };
			Marker m = new Marker( Marker.MarkerType.Cross1, 2, Color.DarkBlue );
			
			m_PointPlot = new PointPlot( m );
			m_PointPlot.OrdinateData = tempData;
			m_PointPlot.AbscissaData = tempData;
			plotSurface.Add(m_PointPlot, PlotSurface2D.XAxisPosition.Bottom, PlotSurface2D.YAxisPosition.Left ); 

			m_LinePlot = new LinePlot();
			m_LinePlot.OrdinateData = tempData;
			m_LinePlot.AbscissaData = tempData;
			m_LinePlot.Pen = new Pen( Color.DarkBlue, 1.5f );

			m_Progressline = new PointPlot();
			m_Progressline.OrdinateData = m_ProgressYData;
			m_Progressline.AbscissaData = m_ProgressXData;
			m_Progressline.Marker = new Marker( Marker.MarkerType.Circle, 8 );
			m_Progressline.Marker.DropLine = true;
			m_Progressline.Marker.Pen = Pens.Firebrick;
			m_Progressline.Marker.Filled = false;

			m_ProgressLabel = new LabelPointPlot();
			m_ProgressText = new string[1];
			m_ProgressText[0] = "";
			m_ProgressLabel.OrdinateData = m_ProgressYData;
			m_ProgressLabel.AbscissaData = m_ProgressXData;
			m_ProgressLabel.TextData = m_ProgressText;
			m_ProgressLabel.LabelTextPosition = LabelPointPlot.LabelPositions.Above;
			m_ProgressLabel.Marker = new Marker( Marker.MarkerType.None, 10 );
			
			menu_DrawProgress_Click(null,null);
		}

		public Graph GetGraph()
		{
			return m_Graph;
		}

		public void SetGraph( Graph g, int currentProgressIndex )
		{
			m_Graph = g;

			SetName( m_Graph.GraphTitle );

			plotSurface.XAxis1.Label = m_Graph.xAxisLabel;
			plotSurface.YAxis1.Label = m_Graph.yAxisLabel;

			m_PointPlot.AbscissaData = m_Graph.XData;
			m_LinePlot.AbscissaData = m_Graph.XData;
			m_PointPlot.OrdinateData = m_Graph.YData;
			m_LinePlot.OrdinateData = m_Graph.YData;


			// initialise axis ranges for this graph ...
			m_Bounds.graph = m_Graph;
									
			SetProgress( currentProgressIndex );

			RefreshGraph();
		}

		private void RefreshGraph()
		{
			GetBoundsFromMemberBounds();
			plotSurface.Refresh();
		}

		private void GetBoundsFromMemberBounds()
		{
			plotSurface.XAxis1.WorldMin = m_Bounds.XMin_Plot;
			plotSurface.XAxis1.WorldMax = m_Bounds.XMax_Plot;
			plotSurface.YAxis1.WorldMin = m_Bounds.YMin_Plot;
			plotSurface.YAxis1.WorldMax = m_Bounds.YMax_Plot;	
		}

		public void SetProgress( int dataIndex )
		{
			// NOTE : the data length is 1 less than the Tra length, and this has to be taken into account here...
			
			if( (dataIndex    == 0                    ) || 
			    (dataIndex-1) >= m_Graph.XData.Length ) // is invalid, default to off ...
			{
				// the target structure
				ProgressAxisState_HasValue( false );
			}
			else
			{
				// xAxisDataIndex-1 as the tra index 0 refers to the target structure, and is threfore supposed to be -1, not 0...
				// all the other entries move up a peg ...
				m_ProgressXData[0] = m_Graph.XData[ dataIndex - 1 ];
				m_ProgressYData[0] = m_Graph.YData[ dataIndex - 1 ];
				m_ProgressText[0] = String.Concat( m_ProgressXData[0].ToString(), ", ", m_ProgressYData[0] );
				ProgressAxisState_HasValue( true );
			}
		}

		private void menu_DrawProgress_Click(object sender, System.EventArgs e)
		{
			menu_DrawProgress.Checked = !menu_DrawProgress.Checked;
			ProgressAxisState_Enabled( menu_DrawProgress.Checked );
			plotSurface.Refresh();
		}

		private void menu_SetAxisRange_Click(object sender, System.EventArgs e)
		{
			DM_GraphRangeDialog dialog = new DM_GraphRangeDialog( m_Bounds, 
				new UpdateEvent( RefreshGraph ) );

			if( DialogResult.OK == dialog.ShowDialog(this) )
			{
				RefreshGraph();
			}
		}

		private void menu_CopyData_Click(object sender, System.EventArgs e)
		{
			ClipboardControl.CopyData( m_Graph );
		}

		private void menu_CopyImage_Click(object sender, System.EventArgs e)
		{
			plotSurface.CopyToClipboard();		
		}

		#region PlotType Change
		private void menu_PlotPoint_Click(object sender, System.EventArgs e)
		{
			if( !menu_PlotPoint.Checked )
			{
				menu_PlotPoint.Checked = true;
				menu_PlotLine.Checked = false;
				SetPointPlotting();
			}
		}

		private void menu_PlotLine_Click(object sender, System.EventArgs e)
		{
			if( !menu_PlotLine.Checked )
			{
				menu_PlotPoint.Checked = false;
				menu_PlotLine.Checked = true;
				SetLinePlotting();
			}		
		}

		private void SetLinePlotting()
		{
			plotSurface.Remove( m_PointPlot, false );
			plotSurface.Add( m_LinePlot, PlotSurface2D.XAxisPosition.Bottom, PlotSurface2D.YAxisPosition.Left );
			plotSurface.Refresh();
		}

		private void SetPointPlotting()
		{
			plotSurface.Remove( m_LinePlot, false );
			plotSurface.Add( m_PointPlot, PlotSurface2D.XAxisPosition.Bottom, PlotSurface2D.YAxisPosition.Left );
			plotSurface.Refresh();
		}

		#endregion

		#region Printing
		private void menu_Print_Click(object sender, System.EventArgs e)
		{
			plotSurface.Print( false );			
		}

		private void menu_PrintPreview_Click(object sender, System.EventArgs e)
		{
			plotSurface.Print( true );			
		}

		#endregion

		#region Progress line control
		private bool m_ProgressIsEnabled = false;
		private bool m_PrograssHasValue = false;
		private bool m_ProgressVisible = false; // will be flagges true when both the above are true, and the line plot added to the plotsurface...

		private void ProgressAxisState_Enabled( bool enabled )
		{		
			m_ProgressIsEnabled = enabled;
			ProgressAxisState_SetState();			
		}

		private void ProgressAxisState_HasValue( bool hasValue )
		{
			m_PrograssHasValue = hasValue;
			ProgressAxisState_SetState();	
		}

		private void ProgressAxisState_SetState()
		{
			if( m_PrograssHasValue && m_ProgressIsEnabled )
			{
				if( !m_ProgressVisible )
				{
					plotSurface.Add(m_Progressline);
					plotSurface.Add(m_ProgressLabel);
					m_ProgressVisible = true;
				}
			}
			else
			{
				if( m_ProgressVisible )
				{
					plotSurface.Remove(m_Progressline,false);
					plotSurface.Remove(m_ProgressLabel,false);
					m_ProgressVisible = false;
				}
			}
			RefreshGraph();
		}
		#endregion

		private void InitializeComponent()
		{
			this.plotSurface = new NPlot.Windows.PlotSurface2D();
			this.m_ContextMenu = new System.Windows.Forms.ContextMenu();
			this.menu_CopyDivider = new System.Windows.Forms.MenuItem();
			this.menu_CopyData = new System.Windows.Forms.MenuItem();
			this.menu_CopyImage = new System.Windows.Forms.MenuItem();
			this.menu_Print = new System.Windows.Forms.MenuItem();
			this.menu_PrintPreview = new System.Windows.Forms.MenuItem();
			this.menu_Divider1 = new System.Windows.Forms.MenuItem();
			this.menu_SetAxisRange = new System.Windows.Forms.MenuItem();
			this.menu_DrawProgress = new System.Windows.Forms.MenuItem();
			this.menu_PlotPoint = new System.Windows.Forms.MenuItem();
			this.menu_PlotLine = new System.Windows.Forms.MenuItem();
			this.menu_PlotDivider = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// plotSurface
			// 
			this.plotSurface.AutoScaleAutoGeneratedAxes = false;
			this.plotSurface.AutoScaleTitle = false;
			this.plotSurface.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.plotSurface.ContextMenu = this.m_ContextMenu;
			this.plotSurface.DateTimeToolTip = false;
			this.plotSurface.Dock = System.Windows.Forms.DockStyle.Fill;
			this.plotSurface.Legend = null;
			this.plotSurface.LegendZOrder = -1;
			this.plotSurface.Location = new System.Drawing.Point(0, 0);
			this.plotSurface.Name = "plotSurface";
			this.plotSurface.Padding = 10;
			this.plotSurface.RightMenu = null;
			this.plotSurface.ShowCoordinates = true;
			this.plotSurface.Size = new System.Drawing.Size(384, 293);
			this.plotSurface.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
			this.plotSurface.TabIndex = 1;
			this.plotSurface.Title = "";
			this.plotSurface.TitleFont = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.plotSurface.XAxis1 = null;
			this.plotSurface.XAxis2 = null;
			this.plotSurface.YAxis1 = null;
			this.plotSurface.YAxis2 = null;
			// 
			// m_ContextMenu
			// 
			this.m_ContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						  this.menu_CopyDivider,
																						  this.menu_Print,
																						  this.menu_PrintPreview,
																						  this.menu_Divider1,
																						  this.menu_SetAxisRange,
																						  this.menu_DrawProgress,
																						  this.menu_PlotDivider});
			// 
			// menu_CopyDivider
			// 
			this.menu_CopyDivider.Index = 0;
			this.menu_CopyDivider.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							 this.menu_CopyData,
																							 this.menu_CopyImage});
			this.menu_CopyDivider.Text = "Copy To Clipboard";
			// 
			// menu_CopyData
			// 
			this.menu_CopyData.Index = 0;
			this.menu_CopyData.Text = "As Data";
			this.menu_CopyData.Click += new System.EventHandler(this.menu_CopyData_Click);
			// 
			// menu_CopyImage
			// 
			this.menu_CopyImage.Index = 1;
			this.menu_CopyImage.Text = "As Image";
			this.menu_CopyImage.Click += new System.EventHandler(this.menu_CopyImage_Click);
			// 
			// menu_Print
			// 
			this.menu_Print.Index = 1;
			this.menu_Print.Text = "&Print";
			this.menu_Print.Click += new System.EventHandler(this.menu_Print_Click);
			// 
			// menu_PrintPreview
			// 
			this.menu_PrintPreview.Index = 2;
			this.menu_PrintPreview.Text = "P&rint Preview";
			this.menu_PrintPreview.Click += new System.EventHandler(this.menu_PrintPreview_Click);
			// 
			// menu_Divider1
			// 
			this.menu_Divider1.Index = 3;
			this.menu_Divider1.Text = "-";
			// 
			// menu_SetAxisRange
			// 
			this.menu_SetAxisRange.Index = 4;
			this.menu_SetAxisRange.Text = "&Set Axis Range ...";
			this.menu_SetAxisRange.Click += new System.EventHandler(this.menu_SetAxisRange_Click);
			// 
			// menu_DrawProgress
			// 
			this.menu_DrawProgress.Index = 5;
			this.menu_DrawProgress.Text = "&Draw Progress";
			this.menu_DrawProgress.Click += new System.EventHandler(this.menu_DrawProgress_Click);
			// 
			// menu_PlotPoint
			// 
			this.menu_PlotPoint.Checked = true;
			this.menu_PlotPoint.Index = 0;
			this.menu_PlotPoint.RadioCheck = true;
			this.menu_PlotPoint.Text = "P&oint Plot";
			this.menu_PlotPoint.Click += new System.EventHandler(this.menu_PlotPoint_Click);
			// 
			// menu_PlotLine
			// 
			this.menu_PlotLine.Index = 1;
			this.menu_PlotLine.RadioCheck = true;
			this.menu_PlotLine.Text = "L&ine Plot";
			this.menu_PlotLine.Click += new System.EventHandler(this.menu_PlotLine_Click);
			// 
			// menu_PlotDivider
			// 
			this.menu_PlotDivider.Index = 6;
			this.menu_PlotDivider.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							 this.menu_PlotPoint,
																							 this.menu_PlotLine});
			this.menu_PlotDivider.Text = "Plot As";
			// 
			// DM_GraphWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(384, 293);
			this.Controls.Add(this.plotSurface);
			this.Name = "DM_GraphWindow";
			this.ResumeLayout(false);

		}
	}
}
