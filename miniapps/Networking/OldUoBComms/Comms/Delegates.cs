using System;
using UoB.Comms.ServerSide;

namespace UoB.Comms
{
	public delegate void ConnectionReceived(ConnectionManager cm);
	public delegate void ChatItemEvent(ChatItem c);
}
