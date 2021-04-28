using System;
using System.Data;
using System.IO;

namespace UoB.Core.Data
{
	/// <summary>
	/// Summary description for DataStore.
	/// </summary>
	public abstract class DataStore
	{
		private bool m_IsEditing = false;
        protected DataSet m_DataSet;
		protected DataTable m_MainTable;
		protected DataColumn m_TitleColumn = null;
		protected string m_XAxis;
		protected string m_YAxis;
		protected static readonly string m_TitleColumnString = "TitleColumn";
		protected bool m_Initialised = false;

		/// <summary>
		/// I eventually intend the data manager to dervie from this and implement the graph and event layers
		/// This is intended as a base data manager
		/// </summary>
		
		public DataStore( string name )
		{
			m_DataSet = new DataSet( name );
			m_MainTable = new DataTable( "MainTable" );
			m_DataSet.Tables.Add( m_MainTable );
			SetAxisTitles();
			Reinitialise(); // on the first run
		}

		public DataTable MainTable
		{
			get
			{
				return m_MainTable;
			}
		}

		public bool IsEditing
		{
			get
			{
				return m_IsEditing;
			}
		}

		protected virtual void PrimaryInit()
		{
			m_Initialised = true;
		}

		public virtual void BeginEditing()
		{
			if( !m_Initialised ) PrimaryInit(); // this only needs to be done once ... needs to be done following base constructor call in a subsequenct call
			if( m_IsEditing ) throw new Exception("Editing mode is already active!");
			m_IsEditing = true;
		}

		/// <summary>
		///  To be called following addition of all columns that are going to be added, prior to a write call
		/// </summary>
		public virtual void EndEditing()
		{
			if( !m_IsEditing ) throw new Exception("Editing mode is not already active!");
			m_IsEditing = false;

			m_MainTable.AcceptChanges();
		}

		/// <summary>
		///  The public accessor to the reinitialisation process
		/// </summary>
		public void ClearAndReinitialise()
		{
			if( !m_IsEditing ) throw new Exception("Editing mode must be active!");
			Clear();
			Reinitialise();
		}
		
		protected virtual void Clear()
		{
			m_MainTable.Clear();
			m_MainTable.Columns.Clear();			
		}

		protected virtual void Reinitialise()
		{
			m_TitleColumn = new DataColumn( m_TitleColumnString, typeof( string ) );
			m_MainTable.Columns.Add( m_TitleColumn );
		}

		protected virtual void SetAxisTitles()
		{
			m_XAxis = "";
			m_YAxis = "";
		}

		#region FileIO

		/// <summary>
		/// Public accessor functions to private save functions
		/// </summary>

		public void SaveAllToXML( string fileName )
		{
			if( m_IsEditing ) throw new Exception("Editing mode is still active!");

			SaveDataSetToXML( fileName, m_DataSet );
		}
		/// <summary>
		/// Public accessor functions to private save functions
		/// </summary>


		public void SaveMainTo( string fileName, DataOutputType outFileType )
		{
            SaveTo( fileName, outFileType, m_MainTable );
		}

		protected void SaveTo( string fileName, DataOutputType outFileType, DataTable dTable )
		{
			if( m_IsEditing ) throw new Exception("Editing mode is still active!");

			switch( outFileType )
			{
				case DataOutputType.CSV:
					StreamWriter rw = new StreamWriter( fileName );
					ProduceCSV( dTable, rw, true );
					rw.Close();
					break;
				case DataOutputType.XMLExcel:
					SaveDataTableToXML( fileName, dTable );
					break;
				default:
					throw new Exception( "Unsuppored DataOutputType in SaveMainTo() call" );
			}
		}

		#region CSV
		private void ProduceCSV(DataTable dt, System.IO.StreamWriter file, bool WriteHeader)
		{
			if(WriteHeader)
			{
				string[] arr = new String[dt.Columns.Count];
				for(int i = 0; i<dt.Columns.Count; i++)
				{
					arr[i] = dt.Columns[i].ColumnName;
					arr[i] = GetWriteableValue(arr[i]);
				}

				file.WriteLine(string.Join(",", arr));
			}

			for(int j = 0; j<dt.Rows.Count; j++)
			{
				string[] dataArr = new String[dt.Columns.Count];
				for(int i = 0; i<dt.Columns.Count; i++)
				{
					object o = dt.Rows[j][i];
					dataArr[i] = GetWriteableValue(o);
				}
				file.WriteLine(string.Join(",", dataArr));
			}
		}

		private string GetWriteableValue(object o)
		{
			if(o==null || o == Convert.DBNull)
				return "";
			else if(o.ToString().IndexOf(",")==-1)
				return o.ToString();
			else
				return "\"" + o.ToString() + "\"";

		}

		#endregion
		#region XML

		private static readonly string startExcelXML = "<?xml version=\"1.0\"?>\r\n<?mso-application progid=\"Excel.Sheet\"?>\r\n<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n xmlns:x=\"urn:schemas-microsoft-com:office:excel\"\r\n xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\" xmlns:html=\"http://www.w3.org/TR/REC-html40\"\r\n xmlns:u1=\"urn:schemas-    microsoft-com:office:excel\" >\r\n <Styles>\r\n <Style ss:ID=\"Default\" ss:Name=\"Normal\">\r\n <Alignment ss:Vertical=\"Bottom\"/>\r\n <Borders/>\r\n <Font/>\r\n <Interior/>\r\n <NumberFormat/>\r\n <Protection/>\r\n </Style>\r\n <Style ss:ID=\"BoldColumn\">\r\n <Font x:Family=\"Swiss\" ss:Bold=\"1\"/>\r\n </Style>\r\n <Style     ss:ID=\"StringLiteral\">\r\n <NumberFormat ss:Format=\"@\"/>\r\n </Style>\r\n <Style ss:ID=\"Decimal\">\r\n <NumberFormat ss:Format=\"0.0000\"/>\r\n </Style>\r\n <Style ss:ID=\"Integer\">\r\n <NumberFormat ss:Format=\"0\"/>\r\n </Style>\r\n <Style ss:ID=\"DateLiteral\">\r\n <NumberFormat ss:Format=\"mm/dd/yyyy;@\"/>\r\n </Style>\r\n </Styles>\r\n ";
		private static readonly string endExcelXML = "</Workbook>";

		protected void SaveDataSetToXML( string fileName, DataSet source )
		{
			StreamWriter excelDoc = new System.IO.StreamWriter(fileName);

			int rowCount = 0;
			int sheetCount = 1;

			excelDoc.Write(startExcelXML);
			excelDoc.WriteLine("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
			excelDoc.WriteLine("<Table>");
			excelDoc.WriteLine("<Row>");
			for(int x = 0; x < source.Tables[0].Columns.Count; x++)
			{
				excelDoc.Write("<Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">");
				excelDoc.Write(source.Tables[0].Columns[x].ColumnName);
				excelDoc.WriteLine("</Data></Cell>");
			}
			excelDoc.WriteLine("</Row>");

			foreach(DataRow x in source.Tables[0].Rows)
			{
				rowCount++;
				if(rowCount==64000) //if the number of rows is > 64000 create a new page to continue output
				{
					rowCount = 0;
					sheetCount++;
					excelDoc.WriteLine("</Table>");
					excelDoc.WriteLine(" </Worksheet>");
					excelDoc.WriteLine("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
					excelDoc.WriteLine("<Table>");
				}
				excelDoc.WriteLine("<Row>"); //ID=" + rowCount + "
				for(int y = 0; y < source.Tables[0].Columns.Count; y++)
				{
					switch( x[y].GetType().ToString() )
					{
						case "System.String":
							string XMLstring = x[y].ToString();
							XMLstring = XMLstring.Trim();
							XMLstring = XMLstring.Replace("&","&amp;");
							XMLstring = XMLstring.Replace(">","&gt;");
							XMLstring = XMLstring.Replace("<","&lt;");
							excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\"><Data ss:Type=\"String\">");
							excelDoc.Write(XMLstring);
							excelDoc.WriteLine("</Data></Cell>");
							break;
						case "System.DateTime":
							//Excel has a specific Date Format of YYYY-MM-DD followed by the letter 'T' then hh:mm:sss.lll Example 2005-01-31T24:01:21.000
							//The Following Code puts the date stored in XMLDate to the format above
							DateTime XMLDate = (DateTime)x[y];
							string XMLDatetoString = ""; //Excel Converted Date
							XMLDatetoString = XMLDate.Year.ToString() +
								"-" + 
								(XMLDate.Month < 10 ? "0" + XMLDate.Month.ToString() : XMLDate.Month.ToString()) +
								"-" +
								(XMLDate.Day < 10 ? "0" + XMLDate.Day.ToString() : XMLDate.Day.ToString()) +
								"T" +
								(XMLDate.Hour < 10 ? "0" + XMLDate.Hour.ToString() : XMLDate.Hour.ToString()) +
								":" +
								(XMLDate.Minute < 10 ? "0" + XMLDate.Minute.ToString() : XMLDate.Minute.ToString()) +
								":" +
								(XMLDate.Second < 10 ? "0" + XMLDate.Second.ToString() : XMLDate.Second.ToString()) + 
								".000";
							excelDoc.Write("<Cell ss:StyleID=\"DateLiteral\"><Data ss:Type=\"DateTime\">");
							excelDoc.Write(XMLDatetoString);
							excelDoc.WriteLine("</Data></Cell>");
							break;
						case "System.Boolean":
							excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\"><Data ss:Type=\"String\">");
							excelDoc.Write(x[y].ToString());
							excelDoc.WriteLine("</Data></Cell>");
							break;
						case "System.Int16":
						case "System.Int32":
						case "System.Int64":
						case "System.Byte":
							excelDoc.Write("<Cell ss:StyleID=\"Integer\"><Data ss:Type=\"Number\">");
							excelDoc.Write(x[y].ToString());
							excelDoc.WriteLine("</Data></Cell>");
							break;
						case "System.Decimal":
						case "System.Single":
						case "System.Double":
							excelDoc.Write("<Cell ss:StyleID=\"Decimal\"><Data ss:Type=\"Number\">");
							excelDoc.Write(x[y].ToString());
							excelDoc.WriteLine("</Data></Cell>");
							break;
						case "System.DBNull":
							excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\"><Data ss:Type=\"String\">");
							excelDoc.Write("");
							excelDoc.WriteLine("</Data></Cell>");
							break;
						default:
							throw(new Exception( x[y].GetType().ToString() + " not handled."));
					}
				}
				excelDoc.WriteLine("</Row>");
			}

			excelDoc.WriteLine("</Table>");
			excelDoc.WriteLine("</Worksheet>");
			excelDoc.WriteLine(endExcelXML);
			excelDoc.Close();
		}


		protected void SaveDataTableToXML( string fileName, DataTable table )
		{
			StreamWriter excelDoc = new System.IO.StreamWriter(fileName);

			const string startExcelXML = "<xml version>\r\n<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n xmlns:x=\"urn:schemas-    microsoft-com:office:excel\"\r\n xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">\r\n <Styles>\r\n <Style ss:ID=\"Default\" ss:Name=\"Normal\">\r\n <Alignment ss:Vertical=\"Bottom\"/>\r\n <Borders/>\r\n <Font/>\r\n <Interior/>\r\n <NumberFormat/>\r\n <Protection/>\r\n </Style>\r\n <Style ss:ID=\"BoldColumn\">\r\n <Font x:Family=\"Swiss\" ss:Bold=\"1\"/>\r\n </Style>\r\n <Style     ss:ID=\"StringLiteral\">\r\n <NumberFormat ss:Format=\"@\"/>\r\n </Style>\r\n <Style ss:ID=\"Decimal\">\r\n <NumberFormat ss:Format=\"0.0000\"/>\r\n </Style>\r\n <Style ss:ID=\"Integer\">\r\n <NumberFormat ss:Format=\"0\"/>\r\n </Style>\r\n <Style ss:ID=\"DateLiteral\">\r\n <NumberFormat ss:Format=\"mm/dd/yyyy;@\"/>\r\n </Style>\r\n </Styles>\r\n ";
			const string endExcelXML = "</Workbook>";

			int rowCount = 0;
			int sheetCount = 1;

			/*
			<xml version>
			<Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"
			xmlns:o="urn:schemas-microsoft-com:office:office"
			xmlns:x="urn:schemas-microsoft-com:office:excel"
			xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">
			<Styles>
			<Style ss:ID="Default" ss:Name="Normal">
				<Alignment ss:Vertical="Bottom"/>
				<Borders/>
				<Font/>
				<Interior/>
				<NumberFormat/>
				<Protection/>
			</Style>
			<Style ss:ID="BoldColumn">
				<Font x:Family="Swiss" ss:Bold="1"/>
			</Style>
			<Style ss:ID="StringLiteral">
				<NumberFormat ss:Format="@"/>
			</Style>
			<Style ss:ID="Decimal">
				<NumberFormat ss:Format="0.0000"/>
			</Style>
			<Style ss:ID="Integer">
				<NumberFormat ss:Format="0"/>
			</Style>
			<Style ss:ID="DateLiteral">
				<NumberFormat ss:Format="mm/dd/yyyy;@"/>
			</Style>
			</Styles>
			<Worksheet ss:Name="Sheet1">
			</Worksheet>
			</Workbook>
		*/
			excelDoc.Write(startExcelXML);
			excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
			excelDoc.Write("<Table>");
			excelDoc.Write("<Row>");
			for(int x = 0; x < table.Columns.Count; x++)
			{
				excelDoc.Write("<Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">");
				excelDoc.Write(table.Columns[x].ColumnName);
				excelDoc.Write("</Data></Cell>");
			}
			excelDoc.Write("</Row>");

			foreach(DataRow x in table.Rows)
			{
				rowCount++;
				if(rowCount==64000) //if the number of rows is > 64000 create a new page to continue output
				{
					rowCount = 0;
					sheetCount++;
					excelDoc.Write("</Table>");
					excelDoc.Write(" </Worksheet>");
					excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
					excelDoc.Write("<Table>");
				}
				excelDoc.Write("<Row>"); //ID=" + rowCount + "
				for(int y = 0; y < table.Columns.Count; y++)
				{
					System.Type rowType;
					rowType = x[y].GetType();
					switch(rowType.ToString())
					{
						case "System.String":
							string XMLstring = x[y].ToString();
							XMLstring = XMLstring.Trim();
							XMLstring = XMLstring.Replace("&","&amp;");
							XMLstring = XMLstring.Replace(">","&gt;");
							XMLstring = XMLstring.Replace("<","&lt;");
							excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\"><Data ss:Type=\"String\">");
							excelDoc.Write(XMLstring);
							excelDoc.Write("</Data></Cell>");
							break;
						case "System.DateTime":
							//Excel has a specific Date Format of YYYY-MM-DD followed by the letter 'T' then hh:mm:sss.lll Example 2005-01-31T24:01:21.000
							//The Following Code puts the date stored in XMLDate to the format above
							DateTime XMLDate = (DateTime)x[y];
							string XMLDatetoString = ""; //Excel Converted Date
							XMLDatetoString = XMLDate.Year.ToString() +
								"-" + 
								(XMLDate.Month < 10 ? "0" + XMLDate.Month.ToString() : XMLDate.Month.ToString()) +
								"-" +
								(XMLDate.Day < 10 ? "0" + XMLDate.Day.ToString() : XMLDate.Day.ToString()) +
								"T" +
								(XMLDate.Hour < 10 ? "0" + XMLDate.Hour.ToString() : XMLDate.Hour.ToString()) +
								":" +
								(XMLDate.Minute < 10 ? "0" + XMLDate.Minute.ToString() : XMLDate.Minute.ToString()) +
								":" +
								(XMLDate.Second < 10 ? "0" + XMLDate.Second.ToString() : XMLDate.Second.ToString()) + 
								".000";
							excelDoc.Write("<Cell ss:StyleID=\"DateLiteral\"><Data ss:Type=\"DateTime\">");
							excelDoc.Write(XMLDatetoString);
							excelDoc.Write("</Data></Cell>");
							break;
						case "System.Boolean":
							excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\"><Data ss:Type=\"String\">");
							excelDoc.Write(x[y].ToString());
							excelDoc.Write("</Data></Cell>");
							break;
						case "System.Int16":
						case "System.Int32":
						case "System.Int64":
						case "System.Byte":
							excelDoc.Write("<Cell ss:StyleID=\"Integer\"><Data ss:Type=\"Number\">");
							excelDoc.Write(x[y].ToString());
							excelDoc.Write("</Data></Cell>");
							break;
						case "System.Decimal":
						case "System.Double":
							excelDoc.Write("<Cell ss:StyleID=\"Decimal\"><Data ss:Type=\"Number\">");
							excelDoc.Write(x[y].ToString());
							excelDoc.Write("</Data></Cell>");
							break;
						case "System.DBNull":
							excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\"><Data ss:Type=\"String\">");
							excelDoc.Write("");
							excelDoc.Write("</Data></Cell>");
							break;
						default:
							throw(new Exception(rowType.ToString() + " not handled."));
					}
				}
				excelDoc.Write("</Row>");
			}

			excelDoc.Write("</Table>");
			excelDoc.Write(" </Worksheet>");
			excelDoc.Write(endExcelXML);
			excelDoc.Close();
		}

		#endregion
		#endregion
	}
}
