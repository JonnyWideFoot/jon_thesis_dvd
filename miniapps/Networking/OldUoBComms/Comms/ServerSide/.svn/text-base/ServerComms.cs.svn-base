using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Diagnostics;

using UoB.Research;

namespace UoB.Comms.ServerSide
{
	public sealed class ServerComms
	{
		public event ConnectionReceived ConnectionReceived;

		private CommWindow m_CommWindow;
		private Thread m_listeningThread;
		private TcpListener m_TcpListener;
		private ArrayList m_Connections;

		private bool m_Running = true;
		private bool m_Listening = true;
		private int m_Port;
		private UoBInit m_UoBInit;
   
		public ServerComms()
		{
			m_UoBInit = UoBInit.Instance;
			m_Connections = new ArrayList();
			m_Port = getListeningPort();
			m_CommWindow = new CommWindow(this);
			InitListener();
		}

		private int getListeningPort()
		{
			string key = "ListeningPort";
			int defaultValue = 8002;

			if ( m_UoBInit.ContainsKey( key ) )
			{
				try
				{
					defaultValue = int.Parse( m_UoBInit.ValueOf( key ) );
				}
				catch // any error in bool parsing - i.e. the string is buggered
				{
					m_UoBInit.AddDefinition( key, defaultValue.ToString() );
				}
			}
			else
			{
				m_UoBInit.AddDefinition( key, defaultValue.ToString() );
			}
			return defaultValue;
		}

		public bool listening
		{
			get
			{
				return m_Listening;
			}
			set
			{
				m_Listening = value;
				if(value)
				{
					InitListener();
				}
				else
				{
					FinaliseListener();
				}
			}
		}

		public void ShowCommsWindow(Form MDIParent)
		{
			m_CommWindow.MdiParent = MDIParent;
			m_CommWindow.Show();
		}


		internal ConnectionManager[] Connections
		{
			get
			{
				return (ConnectionManager[]) m_Connections.ToArray( typeof ( ConnectionManager ) );
			}
		}

		private void InitListener()
		{
			try
			{
				string strHostName = Dns.GetHostName();
				IPHostEntry ipEntry = Dns.GetHostByName( strHostName );
				IPAddress[] aryLocalAddr = ipEntry.AddressList;

				m_TcpListener = new TcpListener(aryLocalAddr[0], m_Port);			
				m_TcpListener.Start();
				Trace.WriteLine("Listener Started on port : " + m_Port.ToString());
				m_listeningThread = new Thread(new ThreadStart(WaitingForClient));
				m_listeningThread.Name = "TCPListener Thread";
				m_listeningThread.Start();
			}
			catch 
			{
				Debug.WriteLine("Comms Listener Initiation Failed");
			}
		}

		private void FinaliseListener()
		{
			if ( m_listeningThread != null )
			{
				m_listeningThread.Abort();
				m_listeningThread = null;
			}

			if ( m_TcpListener != null )
			{
				m_TcpListener.Stop();
				m_TcpListener = null;
			}
		}



		private delegate void ConnectionEvent( ConnectionManager cm );

		private void WaitingForClient() 
		// utilises the TCPListener to create a Client Manager object upon receipt of a socket connection
		{
			while(m_Running)
			{
				// Accept will block until someone connects
				try
				{
					Socket s = m_TcpListener.AcceptSocket();

					ConnectionManager cm = new ConnectionManager(s);

					Debug.WriteLine("Connection Recieved by Listener, adding to list ...");
					Debug.Indent();
					Debug.WriteLine("Current Thread Name - " + Thread.CurrentThread.Name);
					Debug.Unindent();

					lock(m_Connections)
					{
						m_Connections.Add(cm);
					}

					m_CommWindow.UpdateClientList();
					ConnectionReceived( cm );
				}
			
				catch ( System.Net.Sockets.SocketException )
				{
					// probably TcpListener being terminated
				}
			}
		}

		public void FinaliseAndExit()
		{
			m_Running = false;
			FinaliseListener();
			killAllClients();
		}

		private void killAllClients()
		{
			foreach ( ConnectionManager cm in m_Connections )
			{
				cm.Terminate();
			}
		}

	}
}
