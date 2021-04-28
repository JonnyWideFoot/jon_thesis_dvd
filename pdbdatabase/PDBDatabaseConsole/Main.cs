using System;
using System.IO;
using System.Text;
using System.Collections;


namespace getPDBFile
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class MainClass
	{
        public static void SingleProgressChanged(object sender, DownloadEventArgs e)
        {
            //this.Invoke(this.singlePercentChanger, new object[] { e.PercentDone });
            Console.WriteLine("{0}", e.PercentDone);
        }
        public static void StateChanged(object sender, DownloadEventArgs e)
        {
            //this.Invoke(this.statusChanger, new object[] { e.DownloadState });
            Console.WriteLine("{0}", e.DownloadState);
        }

        class SessionState
        {
            public string filename;
            public string URL;
        }

        static void SingleDownload(object sessionState)
        {
            SessionState s = (SessionState)sessionState;
            FileDownloader f = new FileDownloader();
            f.ProgressChanged += new DownloadProgressHandler(SingleProgressChanged);
            f.StateChanged += new DownloadProgressHandler(StateChanged);
            f.Download(s.URL, s.filename, new AutoResetEvent(false));
            Console.WriteLine("Done!");
        }

        public static void Queue(int num)
        {
            SessionState s = new SessionState();
            s.filename = @"c:\bob" + num.ToString() + ".exe";
            s.URL = "http://bch-boing.bch.bris.ac.uk/condorupdate/CondorMSI.exe";
            ThreadPool.QueueUserWorkItem(new WaitCallback(SingleDownload), s);
        }





		private iniRead m_IniInfo;
		private Queue m_JobQueue;
		private string m_TargetDir;




		public MainClass( string[] PDBIDs )
		{
			m_JobQueue = new Queue();
			for ( int i = 0; i < PDBIDs.Length; i++ )
			{
				if ( PDBIDs[i].Length == 4 )
				{
					m_JobQueue.Enqueue( PDBIDs[i] );
				}
				else
				{
					Console.WriteLine( PDBIDs[i] + " was ignored as it is an invalid ID" );
				}
			}

			string currentDir = Directory.GetCurrentDirectory() + @"\";
			m_IniInfo = new iniRead( currentDir + "getpdbfile.ini" );

			m_TargetDir = m_IniInfo.valueOf("targetdir");
			if ( m_TargetDir == null )
			{
				m_TargetDir = currentDir;
			}
			if ( !Directory.Exists( m_TargetDir ) )
			{
				Console.WriteLine( "targetDir did not exist, creating : " + m_TargetDir );
				Directory.CreateDirectory(m_TargetDir );
			}

			triggerJobs();
		}

		private void triggerJobs()
		{
			while (activeTreads < numberOfWorkerThreads)
			{
				if ( m_JobQueue.Count == 0 ) break;
				string PDBID = (string) m_JobQueue.Dequeue();

				string theURL = doConcat(PDBID);
				string theFileName = m_TargetDir + PDBID + ".pdb";

				//Console.WriteLine( "URL : " + theURL );
				Console.WriteLine( "FileName : " + theFileName );

				int threadToUse = -1;
				for (int i = 0; i < threadsInUse.Length; i++)
				{
					if (threadsInUse[i] == false)
					{
						threadToUse = i;
						threadsInUse[i] = true;
						break;
					}
				}
				if ( threadToUse < 0 ) throw new Exception("Should never get here");

				DownloadThread dl = new DownloadThread(theFileName, threadToUse);
				dl.DownloadUrl = theURL;
				dl.CompleteCallback += new DownloadCompleteHandler( DownloadCompleteCallback );
				dl.ProgressCallback += new DownloadProgressHandler( DownloadProgressCallback );

				System.Threading.Thread t = new System.Threading.Thread( 
					new System.Threading.ThreadStart(
					dl.Download ));
				Console.WriteLine("Download Thread " + threadToUse.ToString() + " is set up, begining thread...");
				t.Start();
				activeTreads++;
			}
		}

		private int m_PrintCounter = 0;
		private void DownloadProgressCallback ( int bytesSoFar, int totalBytes, int threadNumber )
		{
			if ( (m_PrintCounter % 60) == 0 )
			{
				if ( totalBytes == -1 )
				{
					Console.WriteLine( "Thread " + threadNumber.ToString() + " Recieved : " + bytesSoFar.ToString() + " bytes" );
				}
				else
				{
					int percentageDone = (int) ( ( (float)bytesSoFar / (float)totalBytes ) * 100 );
					Console.WriteLine( "Thread " + threadNumber.ToString() + " Percentage Done : " + percentageDone.ToString() + "%" );	
				}
			}
			m_PrintCounter++;
	
		}

		private void DownloadCompleteCallback ( byte[] dataDownloaded, string fullFilePath, int threadNumber )
		{
			StreamWriter rw = new StreamWriter(fullFilePath, false);
			Encoding ascii = Encoding.ASCII;
			rw.Write(ascii.GetString(dataDownloaded));
			rw.Close();

			//removeListItem(threadNumber); // remove item from queue
			threadsInUse[threadNumber] = false;
			activeTreads--;
			Console.WriteLine("Worker Thread done! : Thread " + threadNumber.ToString() );
			triggerJobs();
		}

		private string doConcat(string idString)
		{
			return 
				m_IniInfo.valueOf("prestring") + 
				idString + 
				m_IniInfo.valueOf("midstring") + 
				idString + 
				m_IniInfo.valueOf("poststring");
		}




		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
            // override the default connection limit!
            System.Net.ServicePointManager.DefaultConnectionLimit = 6;

			//args = new string[1];
			//string PDBID = "1BW8";
			//args[0] = "down.list";



			if ( args[0].Length == 4 )
			{			
				new MainClass( new string[] { args[0] }  );
			}
			else 
			{
				if ( args[0].Split('.').Length == 2)
				{
					if ( args[0].Split('.')[1].ToLower() == "list" )
					{
						if ( !File.Exists( args[0] ) )
						{
							Console.WriteLine( "File doesnt exist in the program folder : " + args[0] );
							return;
						}
						new MainClass( getEntries( args[0] ) );
					}
				}
				else
				{			
					Console.WriteLine("PDB-ID does not match the correct length / no .list file given" );
					return;
				}
			}
		}

		static string[] getEntries(string fileName)
		{
			ArrayList entries = new ArrayList();
			StreamReader re = new StreamReader( fileName );
			string line;
			while ( ( line = re.ReadLine() ) != null )
			{
				entries.Add( line );
			}
			return (string[]) entries.ToArray( typeof( string ) );
		}
	}
}
