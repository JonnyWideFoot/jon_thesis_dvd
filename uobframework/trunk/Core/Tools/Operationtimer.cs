using System;
using System.Text;

namespace UoB.Core.Tools
{
	/// <summary>
	/// Summary description for Operationtimer.
	/// </summary>
	public class Operationtimer
	{
		private string m_Name;
		private StringBuilder m_Builder = new StringBuilder();
		private DateTime m_Start;
		private DateTime m_Last;
		private TimeSpan m_Span;

		public Operationtimer( string name )
		{
			m_Name = name;
			ClearInternalReport();
			ResetStartTimeToNow();
		}

		public void ClearInternalReport()
		{
			m_Builder.Remove( 0, m_Builder.Length );
            m_Builder.Append( "Report timer : " + m_Name + "\r\n" );
		}

		public void ResetStartTimeToNow()
		{
			m_Start = DateTime.Now;
			m_Last = m_Start;
			m_Builder.Append( "Start time set to : " + m_Start.ToString() + "\r\n" );
		}

		public void ReportInternal( string timerMeaning, TimerReportMode mode )
		{
			DateTime now = DateTime.Now;

			m_Builder.Append( "Title : " );
			m_Builder.Append( timerMeaning );
			m_Builder.Append( "\r\n" );

			m_Builder.Append( "\tTimestamped at : " );
			m_Builder.Append( now.ToString() );
			m_Builder.Append( "\r\n" );

			switch( mode )
			{
				case TimerReportMode.TimeSinceLast:
					m_Span = now - m_Last;
					m_Builder.Append( "\tTime since last timestamp : " );
					m_Builder.Append( m_Span.ToString() );
					break;
				case TimerReportMode.TimeSinceStart:
					m_Span = now - m_Start;
					m_Builder.Append( "\tTime since start timestamp : " );
					m_Builder.Append( m_Span.ToString() );
					break;
				default:
					m_Builder.Append( "\tUnsupported TimerReportMode used in report call, ignoring call." );
					break;
			}

			m_Last = now;
			m_Builder.Append( "\r\n" );
		}

		public void ReportToConsole( string timerMeaning, TimerReportMode mode )
		{
			DateTime now = DateTime.Now;

			Console.WriteLine();

			Console.Write( "Title : " );
			Console.WriteLine( timerMeaning );

			Console.Write( "\tTimestamped at : " );
			Console.WriteLine( now.ToString() );

			switch( mode )
			{
				case TimerReportMode.TimeSinceLast:
					m_Span = now - m_Last;
					Console.Write( "\tTime since last timestamp : " );
					Console.WriteLine( m_Span.ToString() );
					break;
				case TimerReportMode.TimeSinceStart:
					m_Span = now - m_Start;
					Console.Write( "\tTime since start timestamp : " );
					Console.WriteLine( m_Span.ToString() );
					break;
				default:
					Console.WriteLine( "\tUnsupported TimerReportMode used in report call, ignoring call." );
					break;
			}	
	
			m_Last = now;
			Console.WriteLine();
		}

		public void ReportBoth( string timerMeaning, TimerReportMode mode )
		{
			DateTime now = DateTime.Now;

			m_Builder.Append( "Title : " );
			m_Builder.Append( timerMeaning );
			m_Builder.Append( "\r\n" );

			m_Builder.Append( "\tTimestamped at : " );
			m_Builder.Append( now.ToString() );
			m_Builder.Append( "\r\n" );

			Console.WriteLine();

			Console.Write( "Title : " );
			Console.WriteLine( timerMeaning );

			Console.Write( "\tTimestamped at : " );
			Console.WriteLine( now.ToString() );

			switch( mode )
			{
				case TimerReportMode.TimeSinceLast:
					m_Span = now - m_Last;
					m_Builder.Append( "\tTime since last timestamp : " );
					Console.Write( "\tTime since last timestamp : " );
					m_Builder.Append( m_Span.ToString() );
					Console.WriteLine( m_Span.ToString() );
					break;
				case TimerReportMode.TimeSinceStart:
					m_Span = now - m_Start;
					m_Builder.Append( "\tTime since start timestamp : " );
					Console.Write( "\tTime since start timestamp : " );
					m_Builder.Append( m_Span.ToString() );
					Console.WriteLine( m_Span.ToString() );
					break;
				default:
					m_Builder.Append( "\tUnsupported TimerReportMode used in report call, ignoring call." );
					Console.WriteLine( "\tUnsupported TimerReportMode used in report call, ignoring call." );
					break;
			}

			m_Last = now;
			m_Builder.Append( "\r\n" );
			Console.WriteLine();
		}

		public override string ToString()
		{
			return m_Builder.ToString();
		}

	}
}
