using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace UoB.PDBDownload.Core
{
    public class DownloadManager
    {
        private const int DOWNLOAD_THREAD_COUNT = 10;

        private object m_QueuePadlock = new object();
        private Queue<DownloadDefinition> m_DownloadList;
        private IniFile m_IniFile;

        private string m_PreString;
        private string m_MidString;
        private string m_PostString;
        private string m_DirectoryName;
        private DirectoryInfo m_OutputDirectory;

        public DownloadManager()
        {
            // override the default connection limit!
            System.Net.ServicePointManager.DefaultConnectionLimit = 6;

            m_DownloadList = new Queue<DownloadDefinition>();
            ProcessIniFile();
        }

        private void ProcessIniFile(ref string setMember, string key)
        {
            if (!m_IniFile.containsKey(key))
            {
                throw new IniException(String.Concat("Could not find a value key/value pair for \"", key, "\"."));
            }
            else
            {
                setMember = m_IniFile.valueOf(key);
            }
        }

        private void ProcessIniFile()
        {
            m_IniFile = new IniFile("getpdbfile.ini");

            ProcessIniFile(ref m_PreString, "prestring");
            ProcessIniFile(ref m_MidString, "midstring");
            ProcessIniFile(ref m_PostString, "poststring");
            ProcessIniFile(ref m_DirectoryName, "targetdir");

            // get our directory info ...
            m_OutputDirectory = new DirectoryInfo(m_DirectoryName);
            if (!m_OutputDirectory.Exists) m_OutputDirectory.Create();
            m_DirectoryName = m_OutputDirectory.FullName;
            if (m_DirectoryName[m_DirectoryName.Length - 1] != Path.DirectorySeparatorChar)
                m_DirectoryName += Path.DirectorySeparatorChar;
        }

        public void Obtain(string listFileName)
        {
            Obtain(listFileName, -1);
        }

        public void Obtain( string listFileName, int startIndex )
        {
            StreamReader re = new StreamReader(listFileName);
            string line;
            while ((line = re.ReadLine()) != null)
            {
                if (startIndex != -1)
                {
                    if (line.Length < (4+startIndex)) continue;
                    line = line.Substring(startIndex, 4);
                }
                else if (line.Length != 4)
                {
                    Console.Write("Warning!: Line Ignored for not looking like a PDBID: '");
                    Console.Write(line);
                    Console.WriteLine("'");
                    continue;
                }
                line = line.Trim();
                if (line.Length != 4) continue;

                // all good to download!
                m_DownloadList.Enqueue(new DownloadDefinition(
                    String.Concat(m_PreString, line, m_MidString, line, m_PostString),
                    String.Concat(m_DirectoryName, line, ".pdb")));
            }          
        }

        private void ChildThread()
        {
            while (true)
            {
                DownloadDefinition def;
                lock (m_QueuePadlock)
                {
                    if (m_DownloadList.Count == 0) break;
                    def = m_DownloadList.Dequeue();
                }
                def.Download();
            }
        }

        public void BeginDownloadThreads()
        {
            // call Next, each one yields a self restarting worker thread
            for (int i = 0; i < DOWNLOAD_THREAD_COUNT; i++)
            {
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ChildThread));
                t.Start();
            }
        }
    }
}
