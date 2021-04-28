using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace UoB.PDBDownload.Core
{
    public class DownloadDefinition
    {
        private const int downloadBlockSize = 5120;

        private HttpWebResponse m_Response = null;
        private string m_URL;
        private string m_Filename;
        private FileInfo m_FileInfo;
        private long m_ServerFileSize;
        private long m_FileStart;
        private bool m_ProgressKnown;

        public DownloadDefinition(string url, string filename)
        {
            m_Filename = filename;
            m_FileInfo = new FileInfo(m_Filename);
            m_URL = url;
        }

        public bool Download()
        {
            m_FileStart = 0;

            FileStream fs = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(m_URL);
            request.Credentials = CredentialCache.DefaultCredentials;

            try
            {
                m_Response = (HttpWebResponse)request.GetResponse();
                if (m_Response.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }

                m_ServerFileSize = m_Response.ContentLength;
                m_ProgressKnown = (m_ServerFileSize != -1);

                if (m_ProgressKnown && File.Exists(m_Filename))
                {
                    m_FileStart = m_FileInfo.Length;
                    if (m_FileStart == m_ServerFileSize)
                    {
                        return true; // already present and correct
                    }
                    else if( m_FileStart < m_ServerFileSize)
                    {
                        request = (HttpWebRequest)WebRequest.Create(m_URL);
                        request.Credentials = CredentialCache.DefaultCredentials;
                        // reobtain the response with the new range information
                        request.AddRange((int)m_FileStart);
                        if (m_Response != null) m_Response.Close(); // chuck the old one...
                        m_Response = (HttpWebResponse)request.GetResponse();
                        if(m_Response.StatusCode == HttpStatusCode.OK )
                        {
                            m_FileInfo.Delete(); // HttpStatusCode should be returned - we dont support it ???
                            // break here to test this.
                        }
                        else if (m_Response.StatusCode != HttpStatusCode.PartialContent)
                        {
                            return false;
                        }
                        // else PartialContent, so all is well.
                    }
                    // else its bigger, and that's bad, so leave file size as 0 to download again...
                }
                else if (m_FileInfo.Exists)
                {
                    m_FileInfo.Delete();
                }

                fs = File.Open(m_Filename, FileMode.Append, FileAccess.Write);
                Stream s = m_Response.GetResponseStream();
                byte[] buffer = new byte[downloadBlockSize]; // create the download buffer
                long totalDownloaded = m_FileStart; // update how many bytes have already been read

                // read a block of bytes and get the number of bytes read
                int readCount;
                while ((readCount = s.Read(buffer, 0, downloadBlockSize)) > 0)
                {
                    totalDownloaded += readCount; // update total bytes read
                    fs.Write(buffer, 0, readCount); // save block to end of file
                }
            }
            catch( Exception ex )
            {
                string exp = ex.ToString();
                return false;
            }
            finally
            {
                if (m_Response != null) m_Response.Close();
                if( fs != null ) fs.Close();
            }
            return true;
        }

        public string DownloadState
        {
            get
            {
                return m_Response.StatusCode.ToString();
            }
        }
    }
}
