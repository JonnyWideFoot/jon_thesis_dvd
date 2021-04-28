using System;
using System.Net;
using System.IO;
using System.Threading;

namespace UoB.DynamicUpdateEngine.Downloading
{
	public class Downloader
	{
		private const int BUFFER_SIZE = 1024;
		private byte[] BufferRead;
		private bool useFastBuffers;
		private byte[] dataBufferFast;
		private System.Collections.ArrayList dataBufferSlow;
		private int dataLength;
		private int bytesProcessed;
		private WebRequest Request;

		public Downloader()
		{
			BufferRead = new byte[BUFFER_SIZE];
			Request = null;
			dataLength = -1;
			bytesProcessed = 0;
			useFastBuffers = true;
		}
			
		public byte[] Download( string url )
		{
			Uri httpSite = new Uri(url);
			Request = WebRequest.Create(httpSite);
			WebResponse resp = Request.GetResponse();

			// Find the data size from the headers.
			string strContentLength = resp.Headers["Content-Length"];
			if ( strContentLength != null )
			{
				dataLength = Convert.ToInt32( strContentLength );
				dataBufferFast = new byte[ dataLength ];
			}
			else
			{
				useFastBuffers = false;
				dataBufferSlow = new System.Collections.ArrayList( BUFFER_SIZE );
			}

			while( true )
			{
				//  Start reading data from the response stream.
				Stream ResponseStream = resp.GetResponseStream();

				//  Pass do.BufferRead to BeginRead.
				int bytesRead = ResponseStream.Read(BufferRead, 0, BUFFER_SIZE);

				if (bytesRead > 0)
				{
					if ( useFastBuffers )
					{
						System.Array.Copy(BufferRead, 0, 
							dataBufferFast, bytesProcessed, 
							bytesRead );
					}
					else
					{
						for ( int b=0; b<bytesRead; b++ )
							dataBufferSlow.Add( BufferRead[b] );
					}
					bytesProcessed += bytesRead;
				}
				else
				{
					ResponseStream.Close();
					break;
				}
			}
			if ( useFastBuffers )
			{
				return dataBufferFast;
			}
			else
			{
				return (byte[])dataBufferSlow.ToArray(typeof(byte));
			}
		}
	}
}
