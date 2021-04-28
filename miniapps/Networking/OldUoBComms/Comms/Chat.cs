using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace UoB.Comms
{
	/// <summary>
	/// Summary description for ClientChat.
	/// </summary>
	public class Chat
	{
		public event ChatItemEvent MessageReceived;

		public Chat()
		{
			Debug.WriteLine("Chat class being created on : " + Thread.CurrentThread.Name);
			MessageReceived += new ChatItemEvent( TraceIt );
		}

		public void ItemRecieved( ChatItem item )
		{			
			MessageReceived( item );
		}

		private void TraceIt( ChatItem item )
		{
			Trace.WriteLine( item.Message );
		}
	}
}
