using System;
using System.IO;

namespace FileSyncroniser
{
	/// <summary>
	/// Summary description for FileSyncroniser.
	/// </summary>
    /// 
	public delegate void StringEvent( string s );
    public delegate void NullEvent();
	public class FileSyncroniser
	{
		private string m_SourcePath;
		private string m_DestniationPath;
        private bool m_WriteLogfile;
        private bool m_InformExisting = false;
        public bool InformExisting
        {
            get
            {
                return m_InformExisting;
            }
            set
            {
               m_InformExisting = value;
            }
        }

        private bool m_Killed = false;

        public void Kill()
        {
            m_Killed = true;
        }

        public event StringEvent FileProcess = null;
        public event StringEvent DirProcess = null;
        public event NullEvent Tick = null;

		public FileSyncroniser( string sourcePath, string destniationPath, bool consolePrint, bool writeLogfile )
		{
            if (consolePrint && writeLogfile)
            {
                FileProcess = new StringEvent(ReportInternal_CF);
            }
            else if (consolePrint)
            {
                FileProcess = new StringEvent(ReportInternal_C);
            }
            else if (writeLogfile)
            {
                FileProcess = new StringEvent(ReportInternal_F);
            }
            else
            {
                FileProcess = new StringEvent(ReportInternal_None);
            }

            DirProcess = new StringEvent(ReportInternal_None);
            Tick = new NullEvent(fNull);

            m_WriteLogfile = writeLogfile;

            SetPath( sourcePath, destniationPath );	
		}

        public bool WasKilled
        {
            get
            {
                return m_Killed;
            }
        }

        private StreamWriter rw = null;
        public void Execute()
        {
            m_Killed = false;
            if (m_WriteLogfile) rw = new StreamWriter(m_SourcePath + "sync_out.log", false);
            DoDirectory(new DirectoryInfo(m_SourcePath), new DirectoryInfo(m_DestniationPath));
            if (m_WriteLogfile) rw.Close();
        }

        private void fNull()
        {
        }

		private void ReportInternal_None( string s )
		{
            return;
		}

        private void ReportInternal_F(string s)
        {
            rw.WriteLine(s);
        }

        private void ReportInternal_C(string s)
        {
            Console.WriteLine(s);
        }

        private void ReportInternal_CF(string s)
        {
            Console.WriteLine(s);
            rw.WriteLine(s);
        }

        private static void Shuffle(DirectoryInfo[] arr)
        {
            Random rnd = new Random();
            int newPos;
            DirectoryInfo tempObj;
            int index = arr.Length;
            while (--index >= 0)
            {
                // new position for element at index
                newPos = rnd.Next(arr.Length);
                // swap the elements at newPos and index
                tempObj = arr[index];
                arr[index] = arr[newPos];
                arr[newPos] = tempObj;
            }
        }

		public void SetPath( string sourcePath, string destniationPath )
		{
			if( !Directory.Exists( sourcePath ) )
			{
				throw new Exception("Source path given was invalid");
			}
			else
			{
				m_SourcePath = sourcePath;
			}
			if( !Directory.Exists( destniationPath ) )
			{
				throw new Exception("Destination path given was invalid");
			}
			else
			{
				m_DestniationPath = destniationPath;
			}
		}

        private bool isSkip(string dirname)
        {
            return 0 == String.Compare(dirname, "$RECYCLE.BIN") ||
                0 == String.Compare(dirname, "RECYCLER") ||
                0 == String.Compare(dirname, "System Volume Information") ||
                0 == String.Compare(dirname, "OT_MS_W") ||
                0 == String.Compare(dirname, "OT_MS_W_MYDOC");
        }

		private void DoDirectory( DirectoryInfo source, DirectoryInfo destination )
		{
            DirectoryInfo[] sourceDirectories = source.GetDirectories();
			DirectoryInfo[] destinationDirectories = destination.GetDirectories();

            // Randomize source dir list
            Shuffle(sourceDirectories);

            // Delete any dest directories that are not present at the source
			for( int i = 0; i < destinationDirectories.Length; i++ )
			{
				DirectoryInfo destFile = destinationDirectories[i];
                if( isSkip(destFile.Name) ) continue;

				bool present = false;
				DirectoryInfo sourceFile = null;
				for( int j = 0; j < sourceDirectories.Length; j++ )
				{
					sourceFile = sourceDirectories[j];
					if( 0 == String.Compare(sourceFile.Name, destFile.Name) )
					{
						present = true;
						break;
					}
				}
				if( !present )
				{					
                    try
                    {
                        FileProcess(destFile.FullName + " [dir] deleted as not present at source.");
                        destFile.Delete(true);
                    }
                    catch (Exception ex)
                    {
                        DoError( String.Concat("Delete Directory **FAILED**!!: ", destFile.FullName, "\r\n", ex.ToString()));
                    }
				}
			}
			
            // Copy source directories
			for( int i = 0; i < sourceDirectories.Length; i++ )
			{
				DirectoryInfo sourceDir = sourceDirectories[i];
                if (isSkip(sourceDir.Name)) continue;

				string destPath = destination.FullName + '\\' + sourceDir.Name;
				if( !Directory.Exists( destPath ) )
				{
                    try
                    {
                        Directory.CreateDirectory(destPath);
                        FileProcess("Created Directory : " + destPath);
                    }
                    catch
                    {
                        FileProcess("Couldn't Create Directory : " + destPath);
                        continue;
                    }						
				}
                DirProcess(destPath);
				DirectoryInfo di = new DirectoryInfo( destPath );
				DoDirectory( sourceDir, di );
                if (m_Killed) return;
			}

			FileInfo[] destFiles = destination.GetFiles();
			FileInfo[] sourceFiles = source.GetFiles();
			for( int i = 0; i < destFiles.Length; i++ )
			{
                if (m_Killed) return;
				FileInfo destFile = destFiles[i];
				bool present = false;
				for( int j = 0; j < sourceFiles.Length; j++ )
				{
					FileInfo sourceFile = sourceFiles[j];
					if( sourceFile.Name == destFile.Name && sourceFile.Length == destFile.Length )
					{
						present = true;
					}
				}
				if( !present )
				{
                    try
                    {
                        destFile.Delete();
                        FileProcess("Deleted file : " + destFile);
                    }
                    catch (Exception ex)
                    {
                       DoError( String.Concat("Delete File **FAILED**!!: ", destFile.FullName, "\r\n", ex.ToString()));
                    }				
				}
			}

			for( int j = 0; j < sourceFiles.Length; j++ )
			{
                if (m_Killed) return;
				FileInfo sourceFile = sourceFiles[j];
				try
				{
					string name = destination.FullName + '\\' + sourceFile.Name;
					if( !File.Exists(name) )
					{
                        try
                        {
                            sourceFile.CopyTo(name, true);
                            FileProcess("Copyed file : " + sourceFile.Name);
                        }
                        catch (Exception ex)
                        {
                            DoError(String.Concat("Copy File **FAILED**!!: ", sourceFile.FullName, "\r\n", ex.ToString()));
                        }
					}
					else
					{
						FileInfo destFile = new FileInfo( name );
						if( destFile.Length != sourceFile.Length )
						{
                            try
                            {
                                sourceFile.CopyTo(name, true);
                                FileProcess("Copyed file : " + sourceFile.Name);
                            }
                            catch(Exception ex)
                            {
                                DoError(String.Concat("Copy File **FAILED**!!: ", sourceFile.FullName, "\r\n", ex.ToString()));
                            }
						}
                        else if (m_InformExisting)
                        {
                            FileProcess("File already valid : " + sourceFile.Name);
                        }
                        else
                        {
                            Tick();
                        }
					}
				}
				catch(Exception ex)
				{
                    DoError("**Source File** Exception during copying" + ex.ToString());              
				}
			}            
		}

        private void DoError( string error )
        {
            try
            {
                FileProcess(error);
                StreamWriter rw = new StreamWriter(m_SourcePath + "sync_error.log", true);
                rw.WriteLine(DateTime.Now);
                rw.WriteLine(error);
                rw.Close();
            }
            catch
            {
                return;
            }
        }
	}
}
