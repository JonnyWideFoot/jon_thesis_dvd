using System;
using System.Windows.Forms;

namespace UoB.Comms
{

	public class ClientInfo
	{
		public ClientInfo()
		{
			logonTime = DateTime.Now;
		}
		public string Username = "Undefined";
		public string UserInfoString = "Undefined";
		public DateTime logonTime;

		public string timeString
		{
			get
			{
				return logonTime.ToShortDateString() + " " + logonTime.ToLongTimeString();
			}
		}

		public int keepAliveMsgCount = 0;
		public int totalMsgCount = 0;

		public System.Windows.Forms.ListViewItem makeListItem()
		{
			ListViewItem returnItem = new ListViewItem();

			returnItem.Text = "";
			returnItem.SubItems.Add(Username);
			returnItem.SubItems.Add(UserInfoString);	
			returnItem.SubItems.Add(timeString);
			returnItem.SubItems.Add(keepAliveMsgCount.ToString());
			returnItem.SubItems.Add(totalMsgCount.ToString());

			return returnItem;
		}
	}

}
