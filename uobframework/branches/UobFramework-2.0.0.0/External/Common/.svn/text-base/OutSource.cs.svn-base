using System;
using System.Diagnostics;

using UoB.Core;

namespace UoB.External.Common
{
	/// <summary>
	/// Summary description for Process.
	/// </summary>
	public abstract class OutSource : IProgress
	{
		public event StringEvent FileDone; 
		protected bool m_WasKilled = false;
		protected string m_FileName;
		protected string m_Arguments;
		protected Process m_Process;

		public OutSource()
		{
		}

		public bool WasKilled
		{
			get
			{
				return m_WasKilled;
			}
		}

		public virtual float ProgressPercentage
		{
			get
			{
				return -1;
			}
		}

		public void SetPriority( ProcessPriorityClass level )
		{
			try
			{
				m_Process.PriorityClass = level;
			}
			catch
			{
				Trace.WriteLine("An attemt to set the priority level of a spawned process has failed");
			}
		}

		public virtual void Kill()
		{
			m_WasKilled = true;
			m_Process.Kill();
		}

		protected void CallFileDone( string fileName )
		{
			FileDone( fileName );
		}

		public virtual void Start()
		{
		}


	}
}
