using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

using System.Reflection;
using Microsoft.Office.Interop.Excel;

namespace AutoExcel
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
		private System.Windows.Forms.Button ExcelShow;
		private System.Windows.Forms.Button FillFrom;
		private System.Windows.Forms.OpenFileDialog openFileDialog;

        private Microsoft.Office.Interop.Excel.Application objApp;
        private Microsoft.Office.Interop.Excel._Workbook objBook;
        private Microsoft.Office.Interop.Excel.Workbooks objBooks;
        private Microsoft.Office.Interop.Excel.Sheets objSheets;
        private Microsoft.Office.Interop.Excel._Worksheet objSheet;
        private Microsoft.Office.Interop.Excel.Range range;
        private Microsoft.Office.Interop.Excel.Chart chart;

		public Form1()
		{
			InitializeComponent();
			try
			{
				LaunchExcel();
			}
			catch(Exception e)
			{
				MessageBox.Show("Fuckup on internal excel launch!\r\n" + e.Message);
				Activate( false );                
			}
		}

		private void Activate( bool Active )
		{
			for( int i = 0; i < this.Controls.Count; i++ )
			{
				Control c = Controls[i];
				c.Enabled = Active;
			}
		}

		private void LaunchExcel()
		{
			// Instantiate Excel and start a new workbook.
            objApp = new Microsoft.Office.Interop.Excel.Application();
			objBooks = objApp.Workbooks;
			objBook = objBooks.Add( Missing.Value );
			objSheets = objBook.Worksheets;
            objSheet = (Microsoft.Office.Interop.Excel._Worksheet)objSheets.get_Item(1);
		}

		private void ShowExcel()
		{
			//Return control of Excel to the user.
			objApp.Visible = true;
			objApp.UserControl = true;
		}

		private void ReobtainData()
		{
//			//Get a reference to the first sheet of the workbook.
//			objSheets = objBook.Worksheets;
//			objSheet = (Excel._Worksheet)objSheets.get_Item(1);
//
//
//			//Get a range of data.
//			range = objSheet.get_Range("A1", "E5");
//
//			//Retrieve the data from the range.
//			Object[,] saRet;
//			saRet = (System.Object[,])range.get_Value( Missing.Value );
//
//			//Determine the dimensions of the array.
//			long iRows;
//			long iCols;
//			iRows = saRet.GetUpperBound(0);
//			iCols = saRet.GetUpperBound(1);
//
//			//Build a string that contains the data of the array.
//			String valueString;
//			valueString = "Array Data\n";
//
//			for (long rowCounter = 1; rowCounter <= iRows; rowCounter++)
//			{
//				for (long colCounter = 1; colCounter <= iCols; colCounter++)
//				{
//
//					//Write the next value into the string.
//					valueString = String.Concat(valueString,
//						saRet[rowCounter, colCounter].ToString() + ", ");
//				}
//
//				//Write in a new line.
//				valueString = String.Concat(valueString, "\n");
//			}
//
//			//Report the value of the array.
//			MessageBox.Show(valueString, "Array Values");
		}

		private int counter = 0;
		private void Fill( string filename )
		{
			counter ++;

			StreamReader re = new StreamReader(filename);
			ArrayList column1 = new ArrayList();
			ArrayList column2 = new ArrayList();
			string line;
			while( null != ( line = re.ReadLine() ) )
			{
                string[] parts = line.Split(',');
				column1.Add( float.Parse( parts[0] ) );
				column2.Add( float.Parse( parts[1] ) );
			}

			float[,] saRet = new float[column1.Count,2];

			for( int i = 0; i < column1.Count; i++ )
			{
				object value1 = column1[i];
				object value2 = column2[i];
				saRet[i,0] = (float)value1;
				saRet[i,1] = (float)value2;
			}

			//Get the range where the starting cell has the address
			//m_sStartingCell and its dimensions are m_iNumRows x m_iNumCols.
			range = objSheet.get_Range('A' + counter.ToString(), Missing.Value);
			range = range.get_Resize(column1.Count, 2);           

			//Set the range value to the array.
			range.set_Value(Missing.Value, saRet );

			PlotGraph();

			ShowExcel();
		}

		private void PlotGraph()
		{
			// Now create the chart.
            chart = (Microsoft.Office.Interop.Excel.Chart)objBook.Charts.Add(Type.Missing, objSheet, Type.Missing, Type.Missing);

			// range is SetAutoScrollMargin from before

            Microsoft.Office.Interop.Excel.Range cellRange = (Microsoft.Office.Interop.Excel.Range)objSheet.Cells[1, 1];

			chart.ChartWizard(
				cellRange.CurrentRegion,
                Microsoft.Office.Interop.Excel.Constants.xlClassic1, 
				Type.Missing,
                Microsoft.Office.Interop.Excel.XlRowCol.xlColumns, 
				1, 
				2, 
				false, 
				objSheet.Name, 
				Type.Missing, 
				Type.Missing, 
				Type.Missing
				);

			// Apply some formatting to the chart.
			chart.Name = objSheet.Name + " Chart";

            Microsoft.Office.Interop.Excel.ChartGroup grp = (Microsoft.Office.Interop.Excel.ChartGroup)chart.ChartGroups(1);
			grp.GapWidth = 20;
			grp.VaryByCategories = true;
			chart.ChartTitle.Font.Size = 16;
			chart.ChartTitle.Shadow = true;
            chart.ChartTitle.Border.LineStyle = Microsoft.Office.Interop.Excel.Constants.xlSolid;

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
			this.FillFrom = new System.Windows.Forms.Button();
			this.ExcelShow = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// FillFrom
			// 
			this.FillFrom.Location = new System.Drawing.Point(8, 8);
			this.FillFrom.Name = "FillFrom";
			this.FillFrom.TabIndex = 0;
			this.FillFrom.Text = "FillFrom";
			this.FillFrom.Click += new System.EventHandler(this.FillMe_Click);
			// 
			// ExcelShow
			// 
			this.ExcelShow.Location = new System.Drawing.Point(8, 32);
			this.ExcelShow.Name = "ExcelShow";
			this.ExcelShow.TabIndex = 2;
			this.ExcelShow.Text = "ExcelShow";
			this.ExcelShow.Click += new System.EventHandler(this.ExcelShow_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "CSV Files|*.csv";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(104, 61);
			this.Controls.Add(this.ExcelShow);
			this.Controls.Add(this.FillFrom);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			System.Windows.Forms.Application.Run(new Form1());
		}

		private void FillMe_Click(object sender, System.EventArgs e)
		{
			if( DialogResult.OK == openFileDialog.ShowDialog() )
			{
				Fill( openFileDialog.FileName );
			}
		}

		private void ExcelShow_Click(object sender, System.EventArgs e)
		{
			ShowExcel();
		}
	}
}
