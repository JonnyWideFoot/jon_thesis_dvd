using System;
using System.IO;
using System.Text;

using Origin;
using UoB.Methodology.OriginInteraction;

namespace UoB.Methodology.TaskManagement
{
	/// <summary>
	/// Summary description for TaskDirectoryInteration.
	/// </summary>
	public abstract class TaskDirectoryInteration
	{
		private StringBuilder m_FilenameMaker = null;
		private DirectoryInfo m_BaseDI; 
		private DirectoryInfo m_ReportDir;
		private DirectoryInfo m_ResultDir;
		private DirectoryInfo m_ScriptGenDir;

		/// <summary>
		/// ASSUMPTIONS USED IN THIS CLASS ...
		/// Directory content must not dynamically change following initialisation of 
		/// this class for it to function correctly!
		/// </summary>
		public TaskDirectoryInteration( DirectoryInfo di, bool OriginIsRequired )
		{
			m_FilenameMaker = new StringBuilder();

			if( !di.Exists ) throw new IOException("Directory doesnt exist...");
			m_BaseDI = di;
			m_ReportDir = new DirectoryInfo( di.FullName + "\\Report\\");
			if( !m_ReportDir.Exists ) m_ReportDir.Create();
			m_ResultDir = new DirectoryInfo( di.FullName + "\\Result\\");
			if( !m_ResultDir.Exists ) m_ResultDir.Create();     
			m_ScriptGenDir = new DirectoryInfo( di.FullName + "\\ScriptGen\\");
			if( !m_ScriptGenDir.Exists ) m_ScriptGenDir.Create(); 
 
			// some derived classes will not require Origin interaction, so there is a flag set above
			if( OriginIsRequired ) OriginSetup();
		}


		protected void Assert( bool condition, string messageOnFalse )
		{
			if( !condition ) throw new Exception( messageOnFalse );
		}

		protected void Assert( bool condition )
		{
			if( !condition ) throw new Exception( "UNASSIGNED FAIL" );
		}
		

		#region Filename encoded parameter extraction
		public string GetFilenameForParams( string extension, string[] strings, float[] floats )
		{
			m_FilenameMaker.Remove( 0, m_FilenameMaker.Length );
			for( int i = 0; i < strings.Length; i++ )
			{
				m_FilenameMaker.Append( strings[i] );
				if( i != strings.Length -1 && floats.Length > 0 )
				{
					m_FilenameMaker.Append('_');
				}
			}
			for( int i = 0; i < floats.Length; i++ )
			{
				m_FilenameMaker.Append( floats[i] );
				if( i != floats.Length -1 )
				{
					m_FilenameMaker.Append('_');
				}
			}	
			return m_FilenameMaker.ToString();
		}

		public void ParseFilenameForParams( string filename, int expectedStringCount, int expectedFloatCount, out string[] names, out float[] floats )
		{
			filename = Path.GetFileNameWithoutExtension( filename );
			string[] fileParts = filename.Split('_');
			int expectedTotal = expectedStringCount + expectedFloatCount;
			if( fileParts.Length != expectedTotal )
			{
				throw new Exception("The filename does not contain the expected number of elements");
			}
			names = new string[expectedStringCount];
			for( int i = 0; i < expectedStringCount; i++ )
			{
				names[i] = fileParts[i];
			}
			floats = new float[expectedFloatCount];
			for( int i = 0; i < expectedFloatCount; i++ )
			{
				floats[i] = float.Parse( fileParts[expectedStringCount+i] );
			}
		}

		#endregion

		#region HTMLReporting functions and memeber variables common to all derived HTML Reporters
		private bool m_HTMLReportingStarted = false; // private flag used in the Begin and End function
		// the AssertHTMLReportIsActive(); function should be used in derived classes
		protected StreamWriter m_HTMLReporter = null;
		protected void HTMLReportingBegin( string reportTitle )
		{
			if( m_HTMLReportingStarted )
			{
				throw new Exception("HTML reporting is already initiated");
			}
			m_HTMLReportingStarted = true;
			string reportFilename = reportDirectory.FullName + Path.DirectorySeparatorChar + "_Report.htm";
			m_HTMLReporter = new StreamWriter( reportFilename, false );

			m_HTMLReporter.WriteLine("<html>\r\n<body>\r\n");
			m_HTMLReporter.WriteLine("<style>\r\n#doimages a img {\r\nborder: 1px solid black\r\n}\r\n</style>\r\n");
			m_HTMLReporter.Write("<Center><h1>");
			m_HTMLReporter.Write( reportTitle );
			m_HTMLReporter.Write("</h1></center>\r\n");
			m_HTMLReporter.WriteLine("<center><table width=1000 border=1 bordercolor=black cellpadding=5 cellspacing=0><tr><td>");
		}

		protected void HTMLReportingDivider()
		{
			m_HTMLReporter.WriteLine("</td></tr><tr><td>");
		}

		protected void AssertHTMLReportIsActive()
		{
			if( !m_HTMLReportingStarted )
			{
				throw new Exception("HTMLReporting is not initiated!");
			}
		}

		protected void HTMLReportingEnd()
		{
			if( !m_HTMLReportingStarted )
			{
				throw new Exception("HTML reporting has not been initiated and can thereore not be ended");
			}
			// footer
			m_HTMLReporter.WriteLine("</td></tr></table></body></html>");
			// end footer

			if( !m_HTMLReportingStarted )
			{
				throw new Exception("You have to have begun before you can end !!!");
			}

			m_HTMLReportingStarted = false;
			m_HTMLReporter.Close();
			m_HTMLReporter = null;
		}

		protected void HTMLStartReportBlock( string titleRow )
		{
			AssertHTMLReportIsActive();

			m_HTMLReporter.WriteLine("<center><table id=\"doimages\" width=\"900\" border=\"0\">");

			m_HTMLReporter.Write( "<tr><td colspan=3><h1>" );
			m_HTMLReporter.Write( titleRow );
			m_HTMLReporter.WriteLine( "</h1></td></tr>" );
		}

		protected void HTMLEndReportBlock()
		{
			AssertHTMLReportIsActive();

			m_HTMLReporter.WriteLine("</table></center>");
		}

		#endregion

		#region Directory Accessors
		public DirectoryInfo baseDirectory
		{
			get
			{
				return m_BaseDI;
			}
		}

		public DirectoryInfo resultDirectory
		{
			get
			{
				return m_ResultDir;
			}
		}

		public DirectoryInfo reportDirectory
		{
			get
			{
				return m_ReportDir;
			}
		}

		public DirectoryInfo scriptGenerationDirectory
		{
			get
			{
				return m_ScriptGenDir;
			}
		}
 
		#endregion

		#region Origin initialisation when required by the derievd class
		private OriginInterface m_Origin = null;
		private void OriginSetup()
		{
			// start the interface, origin should load, we hope ...
			// now assert that the template is where it should be and load it ...
			string templateFilename = reportDirectory.FullName + "_Template.opj";
			// load the template in origin
			if( !File.Exists( templateFilename ) )
			{
				throw new Exception("Origins template file was not found!");
			}
			else
			{
				m_Origin = new OriginInterface( true ); 
				m_Origin.LoadTemplateFile( templateFilename );
			}
		}
		private void OriginReset()
		{
			m_Origin.Reset(true,true);
		}
		protected OriginInterface InteractOrigin
		{
			get
			{
				return m_Origin;
			}
		}
		#endregion
	}
}
