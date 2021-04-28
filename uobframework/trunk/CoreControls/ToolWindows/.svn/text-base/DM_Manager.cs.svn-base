using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Data;

using UoB.Core;
using UoB.Core.Data;
using UoB.Core.Data.Graphing;

using UoB.CoreControls;
using UoB.CoreControls.Documents;

namespace UoB.CoreControls.ToolWindows
{
	/// <summary>
	/// Summary description for PDBCacheOpen.
	/// </summary>
	public class DM_Manager : System.Windows.Forms.UserControl, ITool
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ListView listView_Data;
		private System.Windows.Forms.ColumnHeader column_Name;
		private System.Windows.Forms.ColumnHeader column_Length;
		private System.Windows.Forms.Button button_DataGrid;
		private System.Windows.Forms.ListView listView_Graphs;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Button button_ViewGraphs;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Button button_SaveGraph;
		private System.Windows.Forms.Button button_PlotDataEntries;
		private System.Windows.Forms.Button button_SaveDataEntries;
		private System.Windows.Forms.GroupBox group_AxisPlotMode;
		private System.Windows.Forms.Button button_PlotVersus;
		private System.Windows.Forms.Button button_DeleteGraphs;
		private System.Windows.Forms.RadioButton radio_UseTimeStamp;
		private System.Windows.Forms.RadioButton radio_UseIntegerStepping;
		private System.Windows.Forms.RadioButton radio_UseStepStamp;

		private UpdateEvent m_GraphUpdate;
		private IntProgressEvent m_positionUpdate;
		private DataManager m_DataManager;
		private UpdateEvent m_DataUpdate;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox text_GraphDir;
		private System.Windows.Forms.Button button_Excel;
		private System.Windows.Forms.ImageList ImageList;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button_Clipboard;
		private Document m_Document;

		private void init()
		{

			CoreIni m_CoreIni = CoreIni.Instance;

			string key = "DefaultGraphSaveDir";
			
			if ( m_CoreIni.ContainsKey( key ) )
			{
				try
				{
					text_GraphDir.Text = m_CoreIni.ValueOf( key );
				}
				catch // any error in bool parsing - i.e. the string is buggered
				{
					m_CoreIni.AddDefinition( key, text_GraphDir.Text );
				}
			}
			else
			{
				m_CoreIni.AddDefinition( key, text_GraphDir.Text );
			}

			saveFileDialog.Filter = "CSV File (*.csv)|*.csv";
			saveFileDialog.DefaultExt = "csv";
			Text = "Data Manager";
			m_DataUpdate = new UoB.Core.UpdateEvent(AnalyseManagerData);
			m_GraphUpdate = new UoB.Core.UpdateEvent(AnalyseManagerGraphs);
			m_positionUpdate = new UoB.Core.IntProgressEvent( UpdateChildrenProgress );
		}

		public DM_Manager(DataManager dataManager)
		{
			InitializeComponent();
			init();

			m_DataManager = dataManager;
			m_DataManager.DataPosition += m_positionUpdate;
			m_DataManager.DataUpdated += m_DataUpdate;
			m_DataManager.GraphListUpdated += m_GraphUpdate;

			ValidateManager();
			AnalyseManagerGraphs();
			AnalyseManagerData();
		}

		public DM_Manager()
		{
			InitializeComponent();
			init();

			m_DataManager = null;
			ValidateManager();
		}

		private void controlEnable(bool enabled)
		{
			for( int i = 0; i < Controls.Count; i++ )
			{
				Controls[i].Enabled = enabled;
			}
		}

        private void ValidateManager()
		{
			if ( m_DataManager == null )
			{
				listView_Data.Items.Clear();
				listView_Graphs.Items.Clear();
				controlEnable( false );
				return;
			}
			else
			{
				controlEnable( true );
				int indexTime;
				int indexStep;
				bool containsTime = m_DataManager.ContainsTimeStamp( out indexTime );
				bool containsStep = m_DataManager.ContainsStepStamp( out indexStep );

				if ( !containsTime )
				{
					radio_UseTimeStamp.Enabled = false;
					if ( radio_UseTimeStamp.Checked ) // i.e. if it was checked
					{
						radio_UseIntegerStepping.Checked = true;
					}
				}
				else
				{
					radio_UseTimeStamp.Enabled = true;
				}
				if ( !containsStep )
				{
					radio_UseStepStamp.Enabled = false;
					if ( radio_UseStepStamp.Checked ) // i.e. if it was checked
					{
						radio_UseIntegerStepping.Checked = true;
					}
				}
				else
				{
					radio_UseStepStamp.Enabled = true;
				}
			}
		}



		private void AnalyseManagerGraphs()
		{
			listView_Graphs.Items.Clear();
			if ( m_DataManager != null )
			{
				for ( int i = 0; i <  m_DataManager.Graphs.Length; i++ )
				{
					Graph gObj = m_DataManager.Graphs[i];
					ListViewItem theItem = new ListViewItem(gObj.GraphTitle, 0);
					theItem.Tag = i;
					theItem.SubItems.Add(gObj.xAxisLabel);
					theItem.SubItems.Add(gObj.yAxisLabel);
					listView_Graphs.Items.Add(theItem);
				}
			}
		}

		private void AnalyseManagerData()
		{
			listView_Data.Items.Clear();
			if ( m_DataManager != null )
			{
				for ( int i = 0; i <  m_DataManager.DataListings.Length; i++ )
				{
					DataListing dObj = m_DataManager.DataListings[i];
					ListViewItem theItem = new ListViewItem(dObj.Name, 0);
					theItem.Tag = i;
					theItem.SubItems.Add(dObj.Data.Length.ToString());
					listView_Data.Items.Add(theItem);
				}
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DM_Manager));
			this.button_PlotDataEntries = new System.Windows.Forms.Button();
			this.ImageList = new System.Windows.Forms.ImageList(this.components);
			this.listView_Data = new System.Windows.Forms.ListView();
			this.column_Name = new System.Windows.Forms.ColumnHeader();
			this.column_Length = new System.Windows.Forms.ColumnHeader();
			this.button_DataGrid = new System.Windows.Forms.Button();
			this.listView_Graphs = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.button_ViewGraphs = new System.Windows.Forms.Button();
			this.button_SaveGraph = new System.Windows.Forms.Button();
			this.button_SaveDataEntries = new System.Windows.Forms.Button();
			this.group_AxisPlotMode = new System.Windows.Forms.GroupBox();
			this.radio_UseStepStamp = new System.Windows.Forms.RadioButton();
			this.radio_UseIntegerStepping = new System.Windows.Forms.RadioButton();
			this.radio_UseTimeStamp = new System.Windows.Forms.RadioButton();
			this.button_PlotVersus = new System.Windows.Forms.Button();
			this.button_DeleteGraphs = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.label2 = new System.Windows.Forms.Label();
			this.text_GraphDir = new System.Windows.Forms.TextBox();
			this.button_Excel = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.label3 = new System.Windows.Forms.Label();
			this.button_Clipboard = new System.Windows.Forms.Button();
			this.group_AxisPlotMode.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_PlotDataEntries
			// 
			this.button_PlotDataEntries.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_PlotDataEntries.ImageIndex = 1;
			this.button_PlotDataEntries.ImageList = this.ImageList;
			this.button_PlotDataEntries.Location = new System.Drawing.Point(88, 24);
			this.button_PlotDataEntries.Name = "button_PlotDataEntries";
			this.button_PlotDataEntries.Size = new System.Drawing.Size(32, 23);
			this.button_PlotDataEntries.TabIndex = 0;
			this.toolTip1.SetToolTip(this.button_PlotDataEntries, "Plot Singly By Mode");
			this.button_PlotDataEntries.Click += new System.EventHandler(this.button_PlotDataEntries_Click);
			// 
			// ImageList
			// 
			this.ImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.ImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageList.ImageStream")));
			this.ImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// listView_Data
			// 
			this.listView_Data.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							this.column_Name,
																							this.column_Length});
			this.listView_Data.HideSelection = false;
			this.listView_Data.LargeImageList = this.ImageList;
			this.listView_Data.Location = new System.Drawing.Point(8, 56);
			this.listView_Data.Name = "listView_Data";
			this.listView_Data.Size = new System.Drawing.Size(280, 120);
			this.listView_Data.TabIndex = 0;
			this.listView_Data.View = System.Windows.Forms.View.Details;
			this.listView_Data.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView_Data_KeyDown);
			this.listView_Data.DoubleClick += new System.EventHandler(this.listView_Data_DoubleClick);
			// 
			// column_Name
			// 
			this.column_Name.Text = "Name";
			this.column_Name.Width = 140;
			// 
			// column_Length
			// 
			this.column_Length.Text = "DataLength";
			this.column_Length.Width = 74;
			// 
			// button_DataGrid
			// 
			this.button_DataGrid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_DataGrid.ImageIndex = 2;
			this.button_DataGrid.ImageList = this.ImageList;
			this.button_DataGrid.Location = new System.Drawing.Point(8, 24);
			this.button_DataGrid.Name = "button_DataGrid";
			this.button_DataGrid.Size = new System.Drawing.Size(32, 24);
			this.button_DataGrid.TabIndex = 1;
			this.toolTip1.SetToolTip(this.button_DataGrid, "Show DataGrid");
			this.button_DataGrid.Click += new System.EventHandler(this.button_DataGrid_Click);
			// 
			// listView_Graphs
			// 
			this.listView_Graphs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							  this.columnHeader1,
																							  this.columnHeader2,
																							  this.columnHeader3});
			this.listView_Graphs.LargeImageList = this.ImageList;
			this.listView_Graphs.Location = new System.Drawing.Point(8, 368);
			this.listView_Graphs.Name = "listView_Graphs";
			this.listView_Graphs.Size = new System.Drawing.Size(280, 120);
			this.listView_Graphs.TabIndex = 2;
			this.listView_Graphs.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Title";
			this.columnHeader1.Width = 115;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "X Label";
			this.columnHeader2.Width = 74;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Y Label";
			// 
			// button_ViewGraphs
			// 
			this.button_ViewGraphs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_ViewGraphs.Location = new System.Drawing.Point(8, 304);
			this.button_ViewGraphs.Name = "button_ViewGraphs";
			this.button_ViewGraphs.Size = new System.Drawing.Size(64, 23);
			this.button_ViewGraphs.TabIndex = 3;
			this.button_ViewGraphs.Text = "&View";
			this.button_ViewGraphs.Click += new System.EventHandler(this.button_ViewGraphs_Click);
			// 
			// button_SaveGraph
			// 
			this.button_SaveGraph.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_SaveGraph.Location = new System.Drawing.Point(80, 304);
			this.button_SaveGraph.Name = "button_SaveGraph";
			this.button_SaveGraph.Size = new System.Drawing.Size(80, 23);
			this.button_SaveGraph.TabIndex = 4;
			this.button_SaveGraph.Text = "&Save to Text";
			this.button_SaveGraph.Click += new System.EventHandler(this.button_SaveGraph_Click);
			// 
			// button_SaveDataEntries
			// 
			this.button_SaveDataEntries.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_SaveDataEntries.ImageIndex = 3;
			this.button_SaveDataEntries.ImageList = this.ImageList;
			this.button_SaveDataEntries.Location = new System.Drawing.Point(48, 24);
			this.button_SaveDataEntries.Name = "button_SaveDataEntries";
			this.button_SaveDataEntries.Size = new System.Drawing.Size(32, 23);
			this.button_SaveDataEntries.TabIndex = 6;
			this.toolTip1.SetToolTip(this.button_SaveDataEntries, "Save Selected Data");
			this.button_SaveDataEntries.Click += new System.EventHandler(this.button_SaveDataEntries_Click);
			// 
			// group_AxisPlotMode
			// 
			this.group_AxisPlotMode.Controls.Add(this.radio_UseStepStamp);
			this.group_AxisPlotMode.Controls.Add(this.radio_UseIntegerStepping);
			this.group_AxisPlotMode.Controls.Add(this.radio_UseTimeStamp);
			this.group_AxisPlotMode.Location = new System.Drawing.Point(8, 184);
			this.group_AxisPlotMode.Name = "group_AxisPlotMode";
			this.group_AxisPlotMode.Size = new System.Drawing.Size(280, 96);
			this.group_AxisPlotMode.TabIndex = 8;
			this.group_AxisPlotMode.TabStop = false;
			this.group_AxisPlotMode.Text = "Axis Plot Mode";
			// 
			// radio_UseStepStamp
			// 
			this.radio_UseStepStamp.Location = new System.Drawing.Point(8, 40);
			this.radio_UseStepStamp.Name = "radio_UseStepStamp";
			this.radio_UseStepStamp.Size = new System.Drawing.Size(240, 24);
			this.radio_UseStepStamp.TabIndex = 2;
			this.radio_UseStepStamp.Text = "Use StepStamp";
			// 
			// radio_UseIntegerStepping
			// 
			this.radio_UseIntegerStepping.Location = new System.Drawing.Point(8, 64);
			this.radio_UseIntegerStepping.Name = "radio_UseIntegerStepping";
			this.radio_UseIntegerStepping.Size = new System.Drawing.Size(240, 24);
			this.radio_UseIntegerStepping.TabIndex = 1;
			this.radio_UseIntegerStepping.Text = "Use Integer Stepping";
			// 
			// radio_UseTimeStamp
			// 
			this.radio_UseTimeStamp.Checked = true;
			this.radio_UseTimeStamp.Location = new System.Drawing.Point(8, 16);
			this.radio_UseTimeStamp.Name = "radio_UseTimeStamp";
			this.radio_UseTimeStamp.Size = new System.Drawing.Size(240, 24);
			this.radio_UseTimeStamp.TabIndex = 0;
			this.radio_UseTimeStamp.TabStop = true;
			this.radio_UseTimeStamp.Text = "Use TimeStamp";
			// 
			// button_PlotVersus
			// 
			this.button_PlotVersus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_PlotVersus.Location = new System.Drawing.Point(208, 24);
			this.button_PlotVersus.Name = "button_PlotVersus";
			this.button_PlotVersus.Size = new System.Drawing.Size(80, 23);
			this.button_PlotVersus.TabIndex = 9;
			this.button_PlotVersus.Text = "Plot &Versus";
			this.button_PlotVersus.Click += new System.EventHandler(this.button_PlotVersus_Click);
			// 
			// button_DeleteGraphs
			// 
			this.button_DeleteGraphs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_DeleteGraphs.Location = new System.Drawing.Point(168, 304);
			this.button_DeleteGraphs.Name = "button_DeleteGraphs";
			this.button_DeleteGraphs.Size = new System.Drawing.Size(48, 23);
			this.button_DeleteGraphs.TabIndex = 10;
			this.button_DeleteGraphs.Text = "&Delete";
			this.button_DeleteGraphs.Click += new System.EventHandler(this.button_DeleteGraphs_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 288);
			this.label1.Name = "label1";
			this.label1.TabIndex = 11;
			this.label1.Text = "Current Graphs";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 8);
			this.label2.Name = "label2";
			this.label2.TabIndex = 12;
			this.label2.Text = "Data Listings : ";
			// 
			// text_GraphDir
			// 
			this.text_GraphDir.Location = new System.Drawing.Point(64, 336);
			this.text_GraphDir.Name = "text_GraphDir";
			this.text_GraphDir.Size = new System.Drawing.Size(224, 20);
			this.text_GraphDir.TabIndex = 13;
			this.text_GraphDir.Text = "c:\\DAVEGraphOutput\\";
			// 
			// button_Excel
			// 
			this.button_Excel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_Excel.ImageIndex = 4;
			this.button_Excel.ImageList = this.ImageList;
			this.button_Excel.Location = new System.Drawing.Point(128, 24);
			this.button_Excel.Name = "button_Excel";
			this.button_Excel.Size = new System.Drawing.Size(32, 24);
			this.button_Excel.TabIndex = 14;
			this.toolTip1.SetToolTip(this.button_Excel, "Excel");
			this.button_Excel.Click += new System.EventHandler(this.button_Excel_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 336);
			this.label3.Name = "label3";
			this.label3.TabIndex = 15;
			this.label3.Text = "Save Path";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip1.SetToolTip(this.label3, "Enter a valid save path ....");
			// 
			// button_Clipboard
			// 
			this.button_Clipboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_Clipboard.Image = ((System.Drawing.Image)(resources.GetObject("button_Clipboard.Image")));
			this.button_Clipboard.Location = new System.Drawing.Point(168, 24);
			this.button_Clipboard.Name = "button_Clipboard";
			this.button_Clipboard.Size = new System.Drawing.Size(32, 24);
			this.button_Clipboard.TabIndex = 16;
			this.toolTip1.SetToolTip(this.button_Clipboard, "Copy to Clipboard");
			this.button_Clipboard.Click += new System.EventHandler(this.button_Clipboard_Click);
			// 
			// DM_Manager
			// 
			this.Controls.Add(this.button_Clipboard);
			this.Controls.Add(this.text_GraphDir);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.button_Excel);
			this.Controls.Add(this.button_DeleteGraphs);
			this.Controls.Add(this.button_PlotVersus);
			this.Controls.Add(this.group_AxisPlotMode);
			this.Controls.Add(this.button_SaveDataEntries);
			this.Controls.Add(this.button_SaveGraph);
			this.Controls.Add(this.button_ViewGraphs);
			this.Controls.Add(this.listView_Graphs);
			this.Controls.Add(this.button_DataGrid);
			this.Controls.Add(this.listView_Data);
			this.Controls.Add(this.button_PlotDataEntries);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Name = "DM_Manager";
			this.Size = new System.Drawing.Size(296, 496);
			this.group_AxisPlotMode.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion




		#region ITool Members

		public void AttachToDocument(Document doc)
		{
			if ( m_DataManager != null )
			{
				m_DataManager.DataPosition -= m_positionUpdate;
				m_DataManager.DataUpdated -= m_DataUpdate;
				m_DataManager.GraphListUpdated -= m_GraphUpdate;
			}

			m_DataManager = null;
			m_Document = doc;

			if ( doc != null )
			{
				for( int i = 0; i < doc.MemberCount; i++ )
				{
					if ( doc[i].GetType() == typeof(DataManager) )
					{
						m_DataManager = (DataManager) doc[i];
						m_DataManager.DataPosition += m_positionUpdate;
						m_DataManager.DataUpdated += m_DataUpdate;
						m_DataManager.GraphListUpdated += m_GraphUpdate;
						break; // only one DataManager should ever be present, so dont check any more members
					}
				}
			}

			ValidateManager();
			AnalyseManagerGraphs();
			AnalyseManagerData();
		}

		#endregion

		private PlotMode getCurrentMode()
		{
			if ( radio_UseTimeStamp.Checked )
			{
				return PlotMode.TimeStampIfAvailable;
			}
			else 
			{
				if( radio_UseStepStamp.Checked )
				{
					return PlotMode.StepNumberIfAvailable;
				}
				else
				{
					return PlotMode.IntegerStepping;
				}
			}
		}

		private void button_PlotDataEntries_Click(object sender, System.EventArgs e)
		{
			if ( listView_Data.SelectedItems.Count == 0 )
			{
				MessageBox.Show("You must make a selection");
				return;
			}
			PlotAllSelected();			
		}

		private void PlotAllSelected()
		{
			foreach ( ListViewItem theItem in listView_Data.SelectedItems )
			{
				int index = (int) theItem.Tag;
				m_DataManager.AddGraphDefinition( m_DataManager.MainTitle + " : Plot of " + theItem.Text, index, getCurrentMode() );
				PlotGraphAtIndex( m_DataManager.GraphCount -1 );
			}
		}

		private void button_PlotVersus_Click(object sender, System.EventArgs e)
		{
			if ( listView_Data.SelectedItems.Count == 0 )
			{
				MessageBox.Show("You must make a selection");
				return;
			}
			if ( ( ((float)listView_Data.SelectedItems.Count) % 2 ) != 0 )
			{
				MessageBox.Show("You must have an even number of entries selected for versus plot ...");
				return;
			}
			for ( int i = 0; i < listView_Data.SelectedItems.Count; i += 2 )
			{
				ListViewItem xItem = listView_Data.SelectedItems[i];
				int xIndex = (int) xItem.Tag;
				ListViewItem yItem = listView_Data.SelectedItems[i+1];
				int yIndex = (int) yItem.Tag;
				string title = m_DataManager.MainTitle + " : Plot of " + xItem.Text + " versus " + yItem.Text;
				m_DataManager.AddGraphDefinition( title, xIndex, yIndex );
				PlotGraphAtIndex( m_DataManager.GraphCount -1 );
			}
		}

		private CoreIni m_CoreIni = UoB.Core.CoreIni.Instance;
		private void button_Excel_Click(object sender, System.EventArgs e)
		{
			if( !m_CoreIni.ContainsKey("ExcelPath") )
			{
				MessageBox.Show(this,
					"Excel's Path is not defined in the .ini file, the key \"ExcelPath\" should be present",
					"Excel path not defined", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
				return;
			}
			string excelPath = m_CoreIni.ValueOf("ExcelPath");
			if( !File.Exists(excelPath) )
			{
				MessageBox.Show(this,
					"Excel could not be found at the location specified in the .ini file under the key \"ExcelPath\"",
					"Excel path is invlaid", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
				return;
			}
			ArrayList numbers = new ArrayList();
			foreach ( ListViewItem theItem in listView_Data.SelectedItems )
			{
				int index = (int) theItem.Tag;				
				numbers.Add( index );
			}
			if ( numbers.Count == 0 )
			{
				if( DialogResult.Yes == MessageBox.Show(this,
					"There is no selection in the data list, do you want to dump all the data to the file?",
					"No Selection Present", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) )
				{
					foreach ( ListViewItem theItem in listView_Data.Items )
					{
						int index = (int) theItem.Tag;				
						numbers.Add( index );
					}
				}
				else
				{
					return;
				}
			}
			string fileName = m_CoreIni.GetTempFileName(".csv");
			m_DataManager.WriteData( fileName, (int[]) numbers.ToArray( typeof(int) ) );
			System.Diagnostics.Process.Start( excelPath, "\"" + fileName + "\"" );
			return;
		}

		ArrayList m_GraphChildren = new ArrayList();
		private void UpdateChildrenProgress( int progress, int total )
		{
			for( int i = 0; i < m_GraphChildren.Count; i++ )
			{
				DM_GraphWindow g = (DM_GraphWindow) m_GraphChildren[i];
				g.SetProgress( progress );
			}
		}

		private void RemoveChild(object sender, System.EventArgs e)
		{
			//DM_GraphWindow win = (DM_GraphWindow) sender;
            m_GraphChildren.Remove( sender );
		}
		
		private void button_ViewGraphs_Click(object sender, System.EventArgs e)
		{
			foreach ( ListViewItem theItem in listView_Graphs.SelectedItems )
			{
				PlotGraphAtIndex( (int) theItem.Tag );
			}
		}
		
		private void PlotGraphAtIndex( int index )
		{
			DM_GraphWindow win = new DM_GraphWindow();
			win.SetGraph( m_DataManager.Graphs[ index ], m_DataManager.trajectory.Position );
			win.Closed += new EventHandler(RemoveChild);
			m_GraphChildren.Add( win );
			//m_Document.AddOwnedForm( win );
			win.MdiParent = m_Document.MdiParent;
			win.Show();
		}

		private void button_DeleteGraphs_Click(object sender, System.EventArgs e)
		{
			ArrayList numbers = new ArrayList();
			foreach ( ListViewItem theItem in listView_Graphs.SelectedItems )
			{
				int index = (int) theItem.Tag;
				numbers.Add( index );
			}
			if ( numbers.Count == 0 )
			{
				MessageBox.Show("You must make a selection");
				return;
			}
			numbers.Sort();
			numbers.Reverse();
			m_DataManager.RemoveGraphs( (int[]) numbers.ToArray( typeof(int) ) );
		}

		private void button_SaveDataEntries_Click(object sender, System.EventArgs e)
		{
			if( saveFileDialog.ShowDialog() == DialogResult.OK )
			{
				ArrayList numbers = new ArrayList();
				foreach ( ListViewItem theItem in listView_Data.SelectedItems )
				{
					int index = (int) theItem.Tag;				
					numbers.Add( index );
				}
				if ( numbers.Count == 0 )
				{
					if( DialogResult.Yes == MessageBox.Show(this,
						"There is no selection in the data list, do you want to dump all the data to the file?",
						"No Selection Present", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) )
					{
						foreach ( ListViewItem theItem in listView_Data.Items )
						{
							int index = (int) theItem.Tag;				
							numbers.Add( index );
						}
					}
					else
					{
						return;
					}
				}
				string fileName = saveFileDialog.FileName;
				m_DataManager.WriteData( fileName, (int[]) numbers.ToArray( typeof(int) ) );
			}
		}

		private void button_SaveGraph_Click(object sender, System.EventArgs e)
		{
			if( !Directory.Exists( text_GraphDir.Text ) )
			{
				try
				{
					Directory.CreateDirectory( text_GraphDir.Text );
				}
				catch
				{
					MessageBox.Show("CreateDirectory Failed");
				}
			}

			ArrayList numbers = new ArrayList();
			foreach ( ListViewItem theItem in listView_Graphs.SelectedItems )
			{
				int index = (int) theItem.Tag;
				numbers.Add( index );
			}
			if ( numbers.Count == 0 )
			{
				if( DialogResult.Yes == MessageBox.Show(this,
"There is no selection in the graph list, do you want to dump all graphs to files?",
					"No Selection Present", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) )
				{
					foreach ( ListViewItem theItem in listView_Graphs.Items )
					{
						int index = (int) theItem.Tag;
						numbers.Add( index );
					}
				}
				else
				{
					return;
				}
			}
			m_DataManager.SaveGraphs( text_GraphDir.Text + @"\", (int[]) numbers.ToArray( typeof(int) ) );		
		}

		private void button_DataGrid_Click(object sender, System.EventArgs e)
		{
			DataTable dt = new DataTable( m_DataManager.MainTitle );

			DataListing[] data = m_DataManager.DataListings;
			
			for( int i = 0; i < data.Length; i++ )
			{
				dt.Columns.Add( data[i].Name, typeof(float) );
			}

			int rowCount = 0;
			while(true)
			{
				DataRow r = dt.NewRow();
				bool oneHadData = false;
				for( int i = 0; i < data.Length; i++ )
				{
					if( rowCount < data[i].Data.Length )
					{
						r[i] = data[i].Data[rowCount];
						oneHadData = true;
					}
					else
					{
						r[i] = 0.0f;                                              
					}
				}
				if( !oneHadData )
				{
					break;
				}
				else
				{	
					dt.Rows.Add( r );
					rowCount++;
				}
			}
				
			// show the generated table
			Form window = new Form();
			DataGrid dg = new DataGrid();
			dg.ReadOnly = true;
			dg.DataSource = dt;
            dg.Parent = window;
			dg.Dock = DockStyle.Fill;
			m_Document.AddOwnedForm( window );
			window.MdiParent = m_Document.MdiParent;
			window.Show();
		}

		private void listView_Data_DoubleClick(object sender, System.EventArgs e)
		{
			PlotAllSelected();
		}

		private void button_Clipboard_Click(object sender, System.EventArgs e)
		{
			ArrayList titles = new ArrayList();
			ArrayList data   = new ArrayList();
			foreach ( ListViewItem theItem in listView_Data.SelectedItems )
			{		
				DataListing dl = m_DataManager.GetDataListing( (int)theItem.Tag );
				titles.Add( dl.Name );
				data.Add( dl.Data );
			}
			if ( titles.Count == 0 )
			{
				if( DialogResult.Yes == MessageBox.Show(this,
					"There is no selection in the data list, do you want to dump all the available data to the clipboard?",
					"No Selection Present", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) )
				{
					foreach ( ListViewItem theItem in listView_Data.Items )
					{
						DataListing dl = m_DataManager.GetDataListing( (int)theItem.Tag );
						titles.Add( dl.Name );
						data.Add( dl.Data );
					}
				}
				else
				{
					return;
				}
			}

			ClipboardControl.CopyData( 
				(string[]) titles.ToArray(typeof(string)), 
				(float[][]) data.ToArray( typeof(float[]) )
				);

			return;
		}

		private void listView_Data_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if( e.Modifiers == Keys.Control &&
				e.KeyCode == Keys.A )
			{
				// select all
				for( int i = 0; i < listView_Data.Items.Count; i++ )
				{
					listView_Data.Items[i].Selected = true;
				}
			}
		}
	}
}