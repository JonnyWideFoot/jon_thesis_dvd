using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

using UoB.Research;

namespace UoB.Comms
{
	public abstract class CommNoteQueue
	{
		protected Queue m_Queue;

		public CommNoteQueue()
		{
			m_Queue = new Queue();
		}

		public bool HasItems
		{
			get
			{
				if ( m_Queue.Count > 0 )
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
	}

	public class CommNoteSendQueue : CommNoteQueue
	{
		public event UpdateEvent CommNoteAdded;

		public CommNoteSendQueue()  : base()
		{
		}

		public void AddNote(CommNote theNote)
		{
			m_Queue.Enqueue(theNote);
			CommNoteAdded();
		}

		public byte[] getNoteAsBytes()
		{
			CommNote note = (CommNote) m_Queue.Dequeue();
			if ( note == null ) 
				return new byte[0];
			else
				return note.toByteArray();
		}
	}


	public class CommNoteRecieveQueue : CommNoteQueue
	{

		private string m_CurrentStringBuffer = "";
		public event UpdateEvent CommNoteReceived;

		public CommNoteRecieveQueue() : base()
		{
		}

		public void processRecievedBytes(byte[] dataBytes, int numberRetrievedBytes) // 32 byte buffer processing
		{
			byte[] actualBytes = new byte[numberRetrievedBytes];
			for ( int i = 0; i < numberRetrievedBytes; i++ )
			{
				actualBytes[i] = dataBytes[i];
			}
			convertBytesToCommNoteAndAppend(actualBytes);
		}

		private void convertBytesToCommNoteAndAppend(byte[] theProcessedBytes) // processing of the appended bytes recieved from the client on readSocket
		{
			m_CurrentStringBuffer += Encoding.ASCII.GetString(theProcessedBytes);
			string[] theCommNoteSrings = Regex.Split(m_CurrentStringBuffer, @"#END#");
			for(int i = 0; i < (theCommNoteSrings.Length -1); i++) // -1 as we dont want to include the incomplete note
			{
				addNote(CommNote.process(theCommNoteSrings[i]));
			}
			m_CurrentStringBuffer = theCommNoteSrings[theCommNoteSrings.Length-1];
		}

		private void addNote(CommNote theNote)
		{
			m_Queue.Enqueue(theNote);
			CommNoteReceived();
		}

		public CommNote getNote()
		{
			if(m_Queue.Count > 0)
			{
				return (CommNote)m_Queue.Dequeue();				
			}
			//else return new CommNote_Null();
			else return null;
		}

	}
}
