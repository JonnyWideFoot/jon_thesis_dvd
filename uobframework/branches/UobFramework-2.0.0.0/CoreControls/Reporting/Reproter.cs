#define TRACE

using System;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace UoB.CoreControls.Reporting
{
	sealed class Reproter : TraceListener
	{
		private static readonly Reproter instance = new Reproter();
   
		public static Reproter Instance
		{
			get 
			{
				return instance; 
			}
		}

		// if this parameter is set to true, a call to WriteLine will always create a new row
		// (if false, it may be appended to the current buffer created with some Write calls)
		private bool UseCrWl = true;
		private ReporterWrapper DebugForm = new ReporterWrapper();

		private Reproter() 
		{
			#if (DEBUG)
				Init(true,true);
			#else
				Init(false,true);
			#endif
			Debug.WriteLine("Reporter Initialisation occuring on - " + Thread.CurrentThread.Name);
		}

		public Form wrapperWindow
		{
			get
			{
				return DebugForm;
			}
		}			

		public void Init(bool UseDebugOutput, bool UseCrForWriteLine)
		{
			if (UseDebugOutput==true)
				Debug.Listeners.Add(this);
			else
				Trace.Listeners.Add(this);

			this.UseCrWl = UseCrForWriteLine;
		}

		override public void Write(string message) 
		{   
			DebugForm.Buffer.Append(message);
			DebugForm.UpdateCurrentRow(false);
		}

		override public void WriteLine(string message) 
		{     

			if (this.UseCrWl==true) 
			{
				DebugForm.CreateEventRow();
				DebugForm.Buffer=new StringBuilder();
			}

			DebugForm.Buffer.Append(message); 
			DebugForm.UpdateCurrentRow(true);
			DebugForm.Buffer = new StringBuilder(); 
		}
	}
}
