using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;

using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.GZip;

using UoB.DynamicUpdateEngine.Downloading;

namespace UoB.DynamicUpdateEngine
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public sealed class DynamicUpdater
	{
		private ProgressForm m_UserForm;
		private DownloadThread m_DownloadThread;

		private DirectoryInfo m_UpdateExecDir;
		private DirectoryInfo m_AppDir;
		private DirectoryInfo m_ArchiveDir;

		private Version m_CurrentVersion = null;
		private Version m_OnlineVersion = null;

		public DynamicUpdater()
		{	
			// Direcories
			FileInfo executable = new FileInfo( Assembly.GetEntryAssembly().Location );
			m_UpdateExecDir = new DirectoryInfo( executable.Directory.FullName );
			m_AppDir = m_UpdateExecDir.Parent;
			m_ArchiveDir = new DirectoryInfo( m_AppDir.FullName + "/Archive/" );

			// User interaction
			m_UserForm = new ProgressForm();

			// Downloading
			m_DownloadThread = new DownloadThread();
			m_DownloadThread.CompleteCallback += new DownloadCompleteHandler( DownloadCompleteCallback );
			m_DownloadThread.ProgressCallback += new IntProgressEvent( m_UserForm.SetProgress );
		}

		public void BeginApplication()
		{
			// user interface 
			m_UserForm.Show();

			// decide if we want to update 
			// including if we are going to kill the current instances
			if( PromptUpdate() )
			{
				// start downloading and initialise the user interface
				m_UserForm.SetControlState( true );
				StartDownloadThread();
			}
			else
			{
				// flag not doing anything
			}

			Application.Run( m_UserForm );
		}


		private bool GetCurrentVersion()
		{
			try
			{
				string checkVersionFile = m_AppDir.FullName + Path.DirectorySeparatorChar + UpdateDef.VersionCheckFilename;
				string copyFileName = m_UpdateExecDir.FullName + Path.DirectorySeparatorChar + UpdateDef.VersionCheckFilename;
				File.Copy( checkVersionFile, copyFileName, true );
				if( File.Exists( checkVersionFile ) )
				{
					Assembly a = Assembly.LoadFile( copyFileName );
					AssemblyName an = a.GetName();
					m_CurrentVersion = an.Version;
					return true;
				}
				else
				{
					m_CurrentVersion = new Version(0,0,0,0);
					return false;
				}
			}
			catch
			{
				m_CurrentVersion = new Version(0,0,0,0);
				return false;
			}
		}

		private bool GetOnlineVersion()
		{
			Downloader d = new Downloader();
			byte[] file = null;
			try
			{
				// try to download the version descriptor of the online version
				file = d.Download( UpdateDef.QueryURL );
			}
			catch
			{
				m_UserForm.SetReportText("Online VesrionID check failed, the file could not be downloaded.");
				return false;
			}

			// check the versioning information
			Encoding ascii = Encoding.ASCII;
			string contents = ascii.GetString(file);
			string[] lines = Regex.Split( contents, "\r\n" );
			
			if( lines.Length != 2 )
			{
				m_UserForm.SetReportText("Online download descriptor is invalid.");
				return false;
			}

			string versionString = lines[0];
			if( 0 != String.Compare( "VesrionID=", 0, versionString, 0, 10, true ) )
			{
				m_UserForm.SetReportText("Online VesrionID check failed following download.");
				return false;
			}
			versionString = versionString.Substring( 10, versionString.Length - 10 );
			m_OnlineVersion = new Version( versionString );

			// get the update URL from def file
			string URL = lines[1];
			if( 0 != String.Compare( "URL=", 0, URL, 0, 4, true  ))
			{
				m_UserForm.SetReportText("Online URL check failed follwing download.");
				return false;
			}

			m_DownloadThread.DownloadUrl = URL.Substring(4, URL.Length - 4); 
			m_UserForm.SetURLText( "Using URL: " + m_DownloadThread.DownloadUrl );
			return true;
		}

		private bool UpdateRequired()
		{
			return m_OnlineVersion > m_CurrentVersion;
		}

		public bool PromptUpdate()
		{
			// assert version information
			if( !GetOnlineVersion() )
			{
				MessageBox.Show(m_UserForm,
					"Critical failure!\r\nUpdate descriptor could not be downloaded correctly.",
					"Cannot obtain a valid update descriptor",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error );
				m_UserForm.SetReportText( "Application Abort: Update descriptor could not be downloaded correctly." );
				return false; // we cant continue, we dont know what to download!
			}

			// assert version information
			if( !GetCurrentVersion() )
			{
				if( DialogResult.No == MessageBox.Show(m_UserForm,
					"The installed version identifier could not be determined.\r\nWould you like to install anyway?",
					"Cannot find current version!",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Exclamation,
					MessageBoxDefaultButton.Button1) )
				{
					m_UserForm.SetReportText( "User Abort: Installed version could not be determined, user chose not to force an update..." );
					return false;
				}
				else
				{
					// fall through to try instance check below....
				}
			}

			if( !UpdateRequired() )
			{
				if( DialogResult.No == MessageBox.Show(m_UserForm,
					"The installed version is already current, no update is needed.\r\nWould you like to force an update?",
					"No update is required",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Question,
					MessageBoxDefaultButton.Button2) )
				{
					m_UserForm.SetReportText( "User Abort: No update was required, no forced update was requested..." );
					return false;
				}
				else
				{
					// fall through to try instance check below....
				}
			}

			if( m_ArchiveDir.Exists && m_ArchiveDir.GetFiles().Length > 0 )
			{
				switch(	
					MessageBox.Show( 
					m_UserForm, 
					"Unwanted backup files from the previous installation are present in the Archive directory (" + m_ArchiveDir.FullName + ").\r\nThis folder will be cleared, so you should check to make sure you dont need the contents...\r\nWould you like to view the folder before it is cleared?",
					"Check archive folder",
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Warning )
					)
				{
					case  DialogResult.Cancel:
						m_UserForm.SetReportText( "User Abort: Did not want to clear the Archive..." );
						return false;
					case DialogResult.No:
						// do nothing and clear the folder
						break;
					default:
						// DialogResult.Yes by definiton
						System.Diagnostics.Process.Start( m_ArchiveDir.FullName );
						MessageBox.Show("Click ok to continue!\r\n(Archive will be cleared on OK)","Checking archive folder...",
							MessageBoxButtons.OK, MessageBoxIcon.Hand );
						break;
				}
			}

			m_UserForm.SetReportText( "Update will be attempted..." );
			// try to kill current instances after prompting
			if( GetProcessInstanceCount() > 0 )
			{
				if( DialogResult.No == MessageBox.Show(m_UserForm,
					"Processes are running, these must be killed to continue. Would you like to kill these processes?",
					"Process is active!",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Exclamation,
					MessageBoxDefaultButton.Button2) )
				{
					m_UserForm.SetReportText( "User abort: didn't want to kill running processes." );
					return false;
				}
				else
				{
					m_UserForm.SetReportText( "Attempting to kill all active processes..." );
					KillInstances();
				}
			}
			return true; // all clear to download!
		}


		private int GetProcessInstanceCount()
		{
			Process[] killProcess = Process.GetProcessesByName( UpdateDef.KillProcess );
			return killProcess.Length; // kill all instances
		}

		private void KillInstances()
		{
			Process[] killProcess = Process.GetProcessesByName( UpdateDef.KillProcess );
			int killed = killProcess.Length; // kill all instances
			for( int i = 0; i < killProcess.Length; i++ )
			{
				m_UserForm.SetReportText( "Killing process! PID=" + killProcess[i].Id );
				killProcess[i].Kill();
			}
		}

		private void StartDownloadThread()
		{
			System.Threading.Thread t = new System.Threading.Thread( 
				new System.Threading.ThreadStart(
				m_DownloadThread.Download ));
			t.Start();
		}

		private void DownloadCompleteCallback( byte[] dataDownloaded )
		{
			m_UserForm.SetReportText( "Download Complete. Begining file extraction." );
			m_UserForm.SetProgress(dataDownloaded.Length,dataDownloaded.Length);
			if ( dataDownloaded.Length != 0 )
			{
				m_UserForm.SetReportText( "Clearing Archive Directory" );
				ClearArcive();
				m_UserForm.SetReportText( "Archiving previous installation" );
				ArchiveCurrent();
				m_UserForm.SetReportText( "Clearing Application Directory" );
				ClearAppDir();
				if( !Unzip( dataDownloaded ) )
				{
					m_UserForm.SetReportText( "Extraction Error!" );
					m_UserForm.SetReportText( "Clearing Application Directory" );
					ClearAppDir();
					m_UserForm.SetReportText( "Restoring previous installation" );
					RestoreFromArchive();
				}
				else
				{
					m_UserForm.SetReportText( "Extraction Complete!" );
					if( DialogResult.Yes == 
						MessageBox.Show( m_UserForm, "The update has completed successfully. The original installation and any additional files present in the directory have all been moved into the Archive directory (" + m_ArchiveDir.FullName + "). You should check this folder to make sure you havent lost anything...\r\nWould you like to view the folder?","Check archive folder",
						MessageBoxButtons.YesNo, MessageBoxIcon.Information ) )
					{
						System.Diagnostics.Process.Start( m_ArchiveDir.FullName );
					}
				}				
			}
			else
			{
				m_UserForm.SetReportText( "ERROR! - Downloaded size is 0 bytes!" );
			}
		}
		

		private bool Unzip( byte[] downloadedData )
		{
			bool errorCondition = false;

			// make a new memory stream for the downloaded zip file
			MemoryStream stream = new MemoryStream( downloadedData );
			ZipInputStream s = new ZipInputStream( stream );

			string directoryName = m_AppDir.FullName + Path.DirectorySeparatorChar;
			Directory.CreateDirectory(directoryName);
		
			ZipEntry theEntry;
			while ((theEntry = s.GetNextEntry()) != null) 
			{
				string checkName = Path.GetFileName( theEntry.Name );	
				string copyName = directoryName + theEntry.Name;

				if( checkName == String.Empty ) continue;
				if( IsFileException(checkName) && File.Exists(copyName) ) continue; // skip file
				// i.e. dont replace core.ini it contains user settings
				
				// OK to copy the file
				FileStream streamWriter = null;
				try
				{
					//string fileSavePath = directoryName + fileName;
					m_UserForm.SetURLText("Unpacking : " + theEntry.Name );
					FileInfo createFile = new FileInfo( copyName );
					DirectoryInfo creatFilesDir = createFile.Directory;
					if( !creatFilesDir.Exists )
					{
						creatFilesDir.Create();
					}
					streamWriter = File.Create( createFile.FullName );
			
					int size = -1;
					byte[] dataBuffer = new byte[2048];

					m_UserForm.SetProgress( 0, (int)theEntry.Size );
					int addedData = 0;
					while (true) 
					{
						size = s.Read(dataBuffer, 0, dataBuffer.Length);
						addedData += size;
						m_UserForm.SetProgress( addedData, (int)theEntry.Size );
						if (size > 0) 
						{
							streamWriter.Write(dataBuffer, 0, size);
						} 
						else 
						{
							break;
						}
					}
				}
				catch( Exception e )
				{
					Debug.WriteLine( e.Message );
					errorCondition = true;
				}
				finally
				{		
					if( streamWriter != null )
					{
						streamWriter.Close();
					}
				}
			}

			s.Close(); // the Zip file stream
			return !errorCondition;
		}

		#region File Archive Functions

		private void ClearArcive()
		{
			ClearDirectory( m_ArchiveDir );
		}

		private void ArchiveCurrent()
		{
			CopyDirectory( m_AppDir, m_ArchiveDir );									 
		}

		private void ClearAppDir()
		{
			ClearDirectory( m_AppDir );
		}

		private void RestoreFromArchive()
		{
			CopyDirectory( m_ArchiveDir, m_AppDir );	
		}

		private bool IsDirectoryException( string dirName )
		{
			return( 0 == String.Compare(dirName, "Archive", true) || 0 == String.Compare(dirName, "DynamicUpdate", true) );
		}

		private bool IsFileException( string fileName )
		{
			return( fileName == "Core.ini" );
		}

		private void CopyDirectory( DirectoryInfo sourceDir, DirectoryInfo targetRoot )
		{
			DirectoryInfo[] dirs = sourceDir.GetDirectories();
			for( int i = 0; i < dirs.Length; i++ )
			{
				if( !IsDirectoryException(dirs[i].Name) )
				{
					try
					{
						DirectoryInfo newSub = targetRoot.CreateSubdirectory(dirs[i].Name);
						CopyDirectory( dirs[i], newSub ); // recursive function
					}
					catch
					{
						m_UserForm.SetReportText("Fail on directory clear of: " + dirs[i].FullName );
					}
				}
			}
			CopyFiles( sourceDir, targetRoot );
		}

		private void CopyFiles( DirectoryInfo sourceDir, DirectoryInfo targetDir )
		{
			// now clear the remaining files
			FileInfo[] files = sourceDir.GetFiles();
			for( int i = 0; i < files.Length; i++ )
			{
				string copyName = targetDir.FullName + Path.DirectorySeparatorChar + files[i].Name;
				if( IsFileException(files[i].Name) && File.Exists(copyName) ) continue; // skip file
				try
				{
					files[i].CopyTo(copyName, true); 
				}
				catch
				{
					m_UserForm.SetReportText("Fail on file copy: " + files[i].FullName );
				}
			}
		}

		private void ClearDirectory( DirectoryInfo dir )
		{
			if( !dir.Exists )
			{
				dir.Create();
			}
			else
			{
				DirectoryInfo[] dirs = dir.GetDirectories();
				for( int i = 0; i < dirs.Length; i++ )
				{
					if( !IsDirectoryException(dirs[i].Name) )
					{
						try
						{
							dirs[i].Delete(true); // clear the whole thing
						}
						catch
						{
							this.m_UserForm.SetReportText("Fail on directory clear of: " + dirs[i].FullName );
						}
					}
				}
				// now clear the remaining files
				FileInfo[] files = dir.GetFiles();
				for( int i = 0; i < files.Length; i++ )
				{
					if( !IsFileException(files[i].Name) )
					{
						try
						{
							files[i].Delete(); 
						}
						catch
						{
							this.m_UserForm.SetReportText("Fail on file delete: " + files[i].FullName );
						}
					}
				}
			}
		}
		#endregion
	}
}
