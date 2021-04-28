using System;
using System.Diagnostics;
using System.Text;

namespace UoB.CoreControls.Reporting
{
	/// <summary>
	/// Summary description for SimpleReportListener.
	/// </summary>
	public class SimpleReportListener : TraceListener
	{
		private SimpleReporter m_Parent;
		private StringBuilder m_StringBuilder;
		private StringBuilder m_TempBuffer;

		public SimpleReportListener(SimpleReporter parent)
		{
			m_Parent = parent;
			m_StringBuilder = new StringBuilder();
			m_TempBuffer = new StringBuilder();

			#if DEBUG
				Debug.Listeners.Add(this);
			#else
				Trace.Listeners.Add(this);
			#endif

			WriteLine( "Report Listener Has Started ..." );
			WriteLine("");
			UpdateParent();
		}

		public void UpdateParent()
		{
			m_Parent.ReportText = m_StringBuilder.ToString();
		}

		public void ClearText()
		{
			m_StringBuilder.Remove(0, m_StringBuilder.Length);
			UpdateParent();
		}

		protected override void WriteIndent()
		{
			for ( int i = 0; i < IndentLevel; i++ )
			{
				m_StringBuilder.Append("   ");
			}
		}

		override public void Write(string message) 
		{   
			m_TempBuffer.Append(message);
		}

		override public void WriteLine(string message) 
		{  
			if ( m_TempBuffer.Length > 0 )
			{
				WriteIndent();
				m_StringBuilder.Append( "\r\n" + m_TempBuffer ); 
				m_TempBuffer.Remove(0, m_TempBuffer.Length);
			}  

			if ( m_StringBuilder.Length > 0 )
			{
				m_StringBuilder.Append("\r\n");
			}

			WriteIndent();
			m_StringBuilder.Append( message ); 
			UpdateParent();
		}
	}
}
