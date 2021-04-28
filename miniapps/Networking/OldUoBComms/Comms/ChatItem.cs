using System;

namespace UoB.Comms
{
	/// <summary>
	/// Summary description for ChatItem.
	/// </summary>
	public class ChatItem
	{
		private string m_Message;
		private ChatItemType m_Type;

		public ChatItem(ChatItemType type, string message)
		{
			m_Type = type;
			m_Message = message;
		}

		public string Message
		{
			get
			{
				return m_Message;
			}
		}

		public ChatItemType Type
		{
			get
			{
				return m_Type;
			}
		}
	}

	public enum ChatItemType
	{
		Error,
		InfoMessage,
	}
}
