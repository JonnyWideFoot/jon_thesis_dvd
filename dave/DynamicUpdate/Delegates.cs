using System;

namespace UoB.DynamicUpdateEngine
{
	public delegate void DownloadCompleteHandler( byte[] dataDownloaded );
	public delegate void UpdateEvent();
	public delegate void IntProgressEvent( int progress, int total );
}
