using System;

namespace WebDownload
{
	public delegate void DownloadCompleteHandler( byte[] dataDownloaded, string fullFilePath, int threadNumber );

	/// <summary>
	/// Summary description for DownloadThread.
	/// </summary>
	public class DownloadThread
	{
		public event DownloadCompleteHandler CompleteCallback;
		public event DownloadProgressHandler ProgressCallback;

		private string m_fullPathName;
		private int m_threadNumber;

		public string _downloadUrl = "";

		public DownloadThread(string theFileName, int threadNumber)
		{
			m_fullPathName = theFileName;
			m_threadNumber = threadNumber;
		}

		public string DownloadUrl
		{
			get
			{
				return _downloadUrl;
			}
			set
			{
				_downloadUrl = value;
			}
		}

		public void Download()
		{
			if ( CompleteCallback != null && 
				  DownloadUrl != "" )
			{
				WebDownload webDL = new WebDownload( m_threadNumber );
				byte[] downloadedData = webDL.Download(DownloadUrl, ProgressCallback );
				CompleteCallback( downloadedData, m_fullPathName, m_threadNumber );
			}
		}
	}
}
