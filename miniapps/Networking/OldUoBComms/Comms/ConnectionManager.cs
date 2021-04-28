using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

using UoB.Research;
using UoB.Research.Modelling.ForceField;
using UoB.Research.Modelling.Structure;
using UoB.Research.Primitives;

namespace UoB.Comms
{
	public class ConnectionManager
	{
		private ClientInfo m_ClientInfo;
		private ParticleSystem m_ParticleSystem;
		private PS_PositionStore m_PSTrajectory;
		private Chat m_Chat;

		private System.Timers.Timer m_KeepAliveTimer;
		public event UpdateEvent ClientInfoUpdate;
		public event UpdateEvent ClientConnectionTerminated;
		public event UpdateEvent ParticleSystemAssigned;

		private Socket m_SendSocket;
        private Socket m_ReceiveSocket;
		private CommNoteRecieveQueue m_ReceiveQueue;
		private CommNoteSendQueue m_SendQueue;
		private Thread m_ReceiveThread;

		private bool m_RunComms = true;

		public void TriggerClientInfoUpdate()
		{
			ClientInfoUpdate();
		}

		public void Setup()
		{
			ClientInfoUpdate = new UpdateEvent(nullFunc);
			ClientConnectionTerminated = new UpdateEvent(nullFunc);
			ParticleSystemAssigned = new UpdateEvent(nullFunc);

			m_ReceiveQueue = new CommNoteRecieveQueue();
			m_ReceiveQueue.CommNoteReceived += new UpdateEvent(NoteReceived);

			m_SendQueue = new CommNoteSendQueue();
			m_SendQueue.CommNoteAdded += new UpdateEvent(SendOnSocket);

			m_ClientInfo = new ClientInfo();
			m_Chat = new Chat();

			m_KeepAliveTimer = new System.Timers.Timer(1500);
			m_KeepAliveTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_KeepAliveTimer_Elapsed);
		}

		private void nullFunc()
		{
		}

		public ConnectionManager(string IPAddressSt, int Port) //Invoked By The Client
		{
			Setup();
			m_SendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
			IPEndPoint EPhost = new IPEndPoint( IPAddress.Parse(IPAddressSt) , Port );
			m_SendSocket.Connect( EPhost );
		}
      
		public ConnectionManager(Socket receiveSocket) // Invoked By the server upon receiving a client connection request
		{
			Setup();
			m_ReceiveSocket = receiveSocket;
			m_ReceiveThread = new Thread(new ThreadStart(ReadSocket));
			m_ReceiveThread.Name = "RecieveThread";
			m_ReceiveThread.Start();

			try
			{
				m_SendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
				EndPoint EPhost = m_ReceiveSocket.RemoteEndPoint;
				m_SendSocket.Connect( EPhost );
			}
			catch(SocketException e)
			{
				string error = e.ToString();
				//MessageBox.Show("Send Socket Not Accepted..." + error);
			}
		}

		public Chat chat
		{
			get
			{
				return m_Chat;
			}
		}

		public string clientString
		{
			get
			{
				if ( m_ClientInfo != null )
				{
					return m_ClientInfo.Username + " : " + m_ClientInfo.UserInfoString;
				}
				else
				{
					return "Client Info Not Received Yet...";
				}
			}
		}

		public void Report(ChatItem c)
		{
			m_Chat.ItemRecieved(c);
		}

		public void KeepAlive()
		{
			m_KeepAliveTimer.Stop();
			if ( m_ClientInfo != null ) m_ClientInfo.keepAliveMsgCount++;
			m_KeepAliveTimer.Start();
			ClientInfoUpdate();
		}

		private void ReadSocket()
		{
			while(m_RunComms)
			{
				if(m_ReceiveSocket.Connected)
				{

					//Firstly lets ask the socket for some bytes
					Byte[] receiveBuffer = new Byte[512] ;
					try
					{
						// Receive will block until data coming
						// ret is 0 or Exception happens when Socket connection is broken
						int numberRetrievedBytes = m_ReceiveSocket.Receive(receiveBuffer, receiveBuffer.Length, 0);

						if (numberRetrievedBytes <= 0)
						{
							continue;
						}

						//Now that we have some bytes, give them to the message manager
						m_ReceiveQueue.processRecievedBytes(receiveBuffer, numberRetrievedBytes);
					}
					catch (Exception e) 
					{
						if( !m_ReceiveSocket.Connected )
						{
							Terminate();
						}
						else
						{
							throw new Exception("An error not involving socket disconnection has occured : " + e.ToString() );
						}
					}
				}
				else
				{
					Terminate();
				}
			}
		}

		public void Terminate()
		{
			m_SendQueue.AddNote( new CommNote_KillCode() );
			ClientConnectionTerminated();

			m_RunComms = false;
			if (m_ReceiveSocket != null)
				m_ReceiveSocket.Close();
			if(m_ReceiveThread != null)
				if(m_ReceiveThread.IsAlive)
					m_ReceiveThread.Abort();
			if (m_SendSocket != null)
				m_SendSocket.Close();
		}

		public void SendNote(CommNote c)
		{
			if ( m_SendQueue != null )
			{
				m_SendQueue.AddNote( c );
			}
        }

		private void SendOnSocket()
		{
			ThreadPool.QueueUserWorkItem( new WaitCallback( SendOnSocketThread ) );
		}

		private void SendOnSocketThread( Object stateInfo )
		{
			if (m_SendSocket.Connected)
			{
				while( m_SendQueue.HasItems )
				{
					m_SendSocket.Send( m_SendQueue.getNoteAsBytes() );
				}
			}
		}

		public ClientInfo ClientInfo
		{
			get
			{
				return m_ClientInfo;
			}
			set
			{
				m_ClientInfo = value;
				ClientInfoUpdate();
			}
		}

		private void InitialiseAtomDefinitions(Atom[] theAtoms)
		{
//			AtomListFullDefinitions definitions = new AtomListFullDefinitions(new Atom[0]);
			
//			m_Document = new ConnectedViewerDocument(definitions);
//			m_ConnectedViewer = new ConnectedViewer(m_Document);
		}

		public ParticleSystem particleSystem
		{
			get
			{
				return m_ParticleSystem;
			}
			set
			{
				m_ParticleSystem = value;
				m_PSTrajectory = new PS_PositionStore( m_ParticleSystem );
				ParticleSystemAssigned();
			}
		}

		public void UpdateAtomPositions(Vector[] thePositions)
		{
			if ( thePositions.Length != m_ParticleSystem.Count )
			{
				throw new Exception( " Positions Update Length Does Not Match Number of Atoms!" );
			}

			bool done = false;
			while(!done)
			{  
				try
				{
					m_ParticleSystem.AcquireWriterLock(1000);
					try
					{
						// It is safe for this thread to read from
						// the shared resource.

						m_ParticleSystem.SetPositions( thePositions );

						done = true;

					}        
					finally
					{
						// Ensure that the lock is released.
						m_ParticleSystem.ReleaseWriterLock();
					}
				}
				catch (ApplicationException)
				{
					// The reader lock request timed out.
				}
			}

		}

		private void m_KeepAliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			m_SendQueue.AddNote( new CommNote_Reporter( "KeepAlive not received, terminating connection..." ) );
			Terminate();
		}

		private void NoteReceived()
		{
			m_ReceiveQueue.getNote().Execute(this);
		}
	}
}
