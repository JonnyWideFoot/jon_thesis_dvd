using System;
using System.Net.Sockets;
using System.Threading;

using UoB.Research;
using UoB.Comms;
using UoB.Research.Modelling.Structure;

namespace UoB.Comms.ClientSide
{
	/// <summary>
	/// Summary description for ClientUoB.Comms.
	/// </summary>
	public class ClientComms
	{
		private ConnectionProperties m_PropertiesWindow;
		private ConnectionManager m_Connection;

		private ClientInfo m_ClientInfo;
		public event UpdateEvent ClientInfoUpdate;
		public event UpdateEvent ConnectionTerminationCall;

		public ClientComms()
		{
			m_PropertiesWindow = new ConnectionProperties(this);
			m_ClientInfo = new ClientInfo(); // not in use

			ClientInfoUpdate = new UpdateEvent(nullFunc);
			ConnectionTerminationCall = new UpdateEvent(nullFunc);
		}

		private void nullFunc()
		{
		}

		public void SendNote(CommNote c)
		{
			m_Connection.SendNote(c);
		}

		public void Connect(string IPAddress, int Port)
		{
			m_Connection = new ConnectionManager(IPAddress, Port);
		}

		public void DisConnect()
		{
			m_Connection.Terminate();
		}

		public void ShowSettingsWindow()
		{
			m_PropertiesWindow.Show();
		}


	}
}
