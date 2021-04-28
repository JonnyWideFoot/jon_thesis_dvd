using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using System.Reflection;
using Excel; //= Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace ExcelGen
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public class WebForm1 : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label errLabel;
		protected System.Web.UI.WebControls.Button Button2;
		protected System.Web.UI.WebControls.Button Button1;
  		private void Button1_Click(object sender, System.EventArgs e)
		{
			SqlParameter[] parms = {new SqlParameter("@customerid","ALFKI") };        
			CreateExcelWorkbook( "custordersorders"        , parms);
		}
  		private void RemoveFiles(string strPath)
		{			 
			System.IO.DirectoryInfo di = new DirectoryInfo(strPath);
			FileInfo[] fiArr = di.GetFiles();
         	foreach (FileInfo fri in fiArr)
			{
               
				if(fri.Extension.ToString() ==".xls" || fri.Extension.ToString()==".csv")
				{
					TimeSpan min = new TimeSpan(0,0,60,0,0);
					if(fri.CreationTime < DateTime.Now.Subtract(min))
				{
					fri.Delete();
				}
				}
			}
           
		}

private void CreateExcelWorkbook(string spName, SqlParameter[] parms)
		{
			string strCurrentDir = Server.MapPath(".") + "\\";
			RemoveFiles(strCurrentDir); // utility method to clean up old files			
			Excel.Application oXL;
			Excel._Workbook oWB;
			Excel._Worksheet oSheet;
			Excel.Range oRng;

			try
			{
				GC.Collect();// clean up any other excel guys hangin' around...
				oXL = new Excel.Application();
				oXL.Visible = false;
				//Get a new workbook.
				oWB = (Excel._Workbook)(oXL.Workbooks.Add( Missing.Value ));
				oSheet = (Excel._Worksheet)oWB.ActiveSheet;
				//get our Data     
 
string strConnect = System.Configuration.ConfigurationSettings.AppSettings["connectString"];
				SPGen sg = new SPGen(strConnect,spName,parms);				 
				SqlDataReader myReader = sg.RunReader();               
				// Create Header and sheet...
				int iRow =2;                
				for(int j=0;j<myReader.FieldCount;j++)
				{
					oSheet.Cells[1, j+1] = myReader.GetName(j).ToString();                  
				}
				// build the sheet contents
				while (myReader.Read())
				{ 
					for(int k=0;k < myReader.FieldCount;k++)
					{
						oSheet.Cells[iRow,k+1]= myReader.GetValue(k).ToString();
					}
					iRow++;
				}// end while
				myReader.Close();
				myReader=null;
				//Format A1:Z1 as bold, vertical alignment = center.
				oSheet.get_Range("A1", "Z1").Font.Bold = true;
	oSheet.get_Range("A1", "Z1").VerticalAlignment =Excel.XlVAlign.xlVAlignCenter;
				//AutoFit columns A:Z.
				oRng = oSheet.get_Range("A1", "Z1");
				oRng.EntireColumn.AutoFit();
				oXL.Visible = false;
				oXL.UserControl = false;
				string strFile ="report" + System.DateTime.Now.Ticks.ToString() +".xls";
oWB.SaveAs( strCurrentDir + strFile,Excel.XlFileFormat.xlWorkbookNormal,null,null,false,false,Excel.XlSaveAsAccessMode.xlShared,false,false,null,null,null);
				// Need all following code to clean up and extingush all references!!!
				oWB.Close(null,null,null);
				oXL.Workbooks.Close();
				oXL.Quit();
				System.Runtime.InteropServices.Marshal.ReleaseComObject (oRng);
				System.Runtime.InteropServices.Marshal.ReleaseComObject (oXL);
				System.Runtime.InteropServices.Marshal.ReleaseComObject (oSheet);
				System.Runtime.InteropServices.Marshal.ReleaseComObject (oWB);
				oSheet=null;
				oWB=null;
				oXL = null;
				GC.Collect();  // force final cleanup!
				string  strMachineName = Request.ServerVariables["SERVER_NAME"];
				errLabel.Text="<A href=http://" + strMachineName +"/ExcelGen/" +strFile + ">Download Report</a>";       
            
			}
			catch( Exception theException ) 
			{
				String errorMessage;
				errorMessage = "Error: ";
				errorMessage = String.Concat( errorMessage, theException.Message );
				errorMessage = String.Concat( errorMessage, " Line: " );
				errorMessage = String.Concat( errorMessage, theException.Source );          
				errLabel.Text= errorMessage ;
			}
		}

public void  GenerateCSVReport( string strSPName, SqlParameter[] parms)
		{
	     string strCurrentDir = Server.MapPath(".") + "\\";
	         RemoveFiles(strCurrentDir);
string strConnect = System.Configuration.ConfigurationSettings.AppSettings["connectString"];
			SPGen sg = new SPGen(strConnect,strSPName,parms);			 
			SqlDataReader myReader = sg.RunReader();
			StringBuilder sb = new StringBuilder(); // to hold csv text file
			// Create Header and sheet...
			string quoter = @""""""; //  
			for(int j=0;j<myReader.FieldCount;j++)
			{
				sb.Append( myReader.GetName(j).ToString() ); // headings
				sb.Append(","); // delimiter
			}
	           sb.Append("\n");
			// build the csv contents
			string replVal = String.Empty;
			while (myReader.Read())
			{ 
				for(int k=0;k < myReader.FieldCount;k++)
				{
					if(myReader.GetValue(k).ToString()==null)
					{                      
						sb.Append("\"" + myReader.GetValue(k).ToString()+" " + ",");
					}
					else
						replVal=myReader.GetValue(k).ToString().Replace("\"",quoter);  
					replVal+= " " +",";
					sb.Append(replVal);
				}//end if
				sb.Append("\n"); // new row                       
			}// end while
       		myReader.Close();
			myReader=null;
 string strFile ="report" + System.DateTime.Now.Ticks.ToString() +".csv";
 string strFileContent= sb.ToString();
 FileInfo fi   = new FileInfo( Server.MapPath(strFile));
 FileStream sWriter = fi.Open(FileMode.Create , FileAccess.Write, FileShare.ReadWrite);
 sWriter.Write(System.Text.Encoding.ASCII.GetBytes(strFileContent), 0, strFileContent.Length);
 sWriter.Flush();
 sWriter.Close();
fi = null;
sWriter= null;
string  strMachineName = Request.ServerVariables["SERVER_NAME"];
errLabel.Text="<A href=http://" + strMachineName +"/ExcelGen/" +strFile + ">Download Report</a>";
}
   
		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Button1.Click += new System.EventHandler(this.Button1_Click);
			this.Button2.Click += new System.EventHandler(this.Button2_Click);
			 

		}
		#endregion

		private void Button2_Click(object sender, System.EventArgs e)
		{
			 SqlParameter[] parms = {new SqlParameter("@customerid","ALFKI") };
			GenerateCSVReport( "custordersorders"        , parms);
		}
  
	}

	public class SPGen
	{
		public string sprocName="";
		public SqlParameter[] parmArray = null;
		public string DBConnectString = String.Empty;
		
	public	SPGen(string connectString, string sprocName, SqlParameter[] parms)
		{
            this.DBConnectString =connectString;
			this.sprocName =sprocName;
			this.parmArray = parms;
		}
		
	public SqlDataReader RunReader()
		{
			SqlConnection cn = new SqlConnection(this.DBConnectString);
			cn.Open();
			SqlCommand cmd = new SqlCommand();
			cmd.Connection =cn;
			cmd.CommandType=CommandType.StoredProcedure;
			cmd.CommandText=this.sprocName;
			foreach (SqlParameter prm in this.parmArray)
			{
				cmd.Parameters.Add(prm);
			}

	SqlDataReader dr=cmd.ExecuteReader(CommandBehavior.CloseConnection);
	return dr;
	}

 }
}
