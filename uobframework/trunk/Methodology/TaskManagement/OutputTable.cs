using System;
using System.Text;
using System.IO;
using System.Collections;

namespace UoB.Methodology.TaskManagement
{
	/// <summary>
	/// Summary description for OutPutTable.
	/// </summary>
	public abstract class OutputTable
	{
		// print delimiters
		protected StringBuilder m_SBuild = new StringBuilder(); // used to build each line
		protected ArrayList m_RecordLineStrings = new ArrayList(); // holds each line as a string...
		
		// line separation elements
		protected string m_TableStartLine;
		protected string m_TableEndLine;
		protected string m_InitLine;
		protected string m_EndLine;
		protected string m_Delimiter;

		public OutputTable( DelimiterMode mode )
		{
			switch( mode )
			{
				case DelimiterMode.StandardHTMLTable:
					SetDelimiters_StandardHTMLTable();
					break;
				case DelimiterMode.CSV:
					SetDelimiters_CSV();
					break;
				default:
					throw new Exception("Code not implemented ...");
			}
		}

		protected void SetDelimiters( string start, string end, string delimiter )
		{
			m_TableStartLine = null;
			m_TableEndLine = null;
			m_InitLine = start;
			m_EndLine = end;
			m_Delimiter = delimiter;
		}

		protected void SetDelimiters_StandardHTMLTable()
		{
			m_TableStartLine = "<center><table width=800 border=1 bordercolor=black cellpadding=3 cellspacing=0>";
			m_TableEndLine = "</table></center>";
			m_InitLine = "<tr><td width=100>";
			m_EndLine = "</td></tr>";
			m_Delimiter = "</td><td width=100>";
		}

		protected void SetDelimiters_CSV()
		{
			m_TableStartLine = null;
			m_TableEndLine = null;
			m_InitLine = "";
			m_EndLine = ",";
			m_Delimiter = ",";
		}

		public void PrintTable( StreamWriter rw )
		{
			if( m_TableStartLine != null )
			{
				rw.WriteLine(m_TableStartLine);
			}

			for( int i = 0; i < m_RecordLineStrings.Count; i++ )
			{
				rw.WriteLine( (string)m_RecordLineStrings[i] );
			}

			if( m_TableStartLine != null )
			{
				rw.WriteLine(m_TableEndLine);
			}
		}

		public void PrintTable( string fileName, bool append )
		{
			StreamWriter rw = new StreamWriter( fileName, append );
			PrintTable( rw );
			rw.Close();
		}
	}
}
