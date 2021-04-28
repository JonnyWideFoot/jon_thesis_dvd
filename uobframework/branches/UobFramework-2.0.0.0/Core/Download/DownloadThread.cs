using System;

namespace UoB.Core.Download
{
	/// <summary>
	/// Summary description for DownloadThread.
	/// </summary>
	public class DownloadThread
	{
		public event DownloadCompleteHandler CompleteCallback;
		public event IntProgressEvent ProgressCallback;

		public string _downloadUrl = "";
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
			if ( CompleteCallback != null && DownloadUrl != "" )
			{
				WebDownload webDL = new WebDownload();

				byte[] downloadedData;
				try
				{
					downloadedData = webDL.Download(DownloadUrl, ProgressCallback );
				}
				catch (System.Net.WebException)
				{
					downloadedData = new byte[0];
				}
				finally           
				{                
					webDL = null;            
				}

				CompleteCallback( downloadedData );
			}
		}
	}
}
