using System;

namespace UoB.Core.FileIO.Tra
{
	/// <summary>
	/// Summary description for TraPreview.
	/// </summary>
	public class TraPreview
	{
		public int  TEStartPoint;
		public int  numberOfEntries; // position definitions in the tra file
		public long fileSize;  // in bytes
		public int  TEBlockSize; // the size of an entire definition block
		public int  startPoint; // of the position definitions in the file
		public int  endPoint;
		public int  skipLength; // how often to skip importing a definition

		public void SetToImport_FirstAndLastOnly()
		{
			startPoint = 0;
			endPoint = numberOfEntries;
			skipLength = numberOfEntries - 1; // so that the enumerator % skipLength == 0 on the last entry
		}
	}
}
