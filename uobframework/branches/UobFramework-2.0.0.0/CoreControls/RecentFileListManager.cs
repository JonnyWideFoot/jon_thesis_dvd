using System;
using System.IO;
using UoB.Core;

namespace UoB.CoreControls
{
	/// <summary>
	/// Summary description for RecentFileListManager.
	/// </summary>
	public sealed class RecentFileListManager
	{
		private string[] m_RecentFiles;
		private string m_HostName;
		private CoreIni m_CoreIni;
		private int m_Length;

		public RecentFileListManager( string HostName, int Length )
		{
			m_CoreIni = CoreIni.Instance;
			m_Length = Length;
			m_HostName = HostName;
			m_RecentFiles = new string[Length];	
			populateFromParamsFile();
		}

		private void saveToParamsFile()
		{
			for ( int i = 0; i < m_Length; i++ )
			{
				string keyName = m_HostName + "File" + i.ToString();
				m_CoreIni.AddDefinition( keyName, m_RecentFiles[i] );
			}
		}

		private void populateFromParamsFile()
		{
			for ( int i = 0; i < m_Length; i++ )
			{
				string keyName = m_HostName + "File" + i.ToString();
				if ( m_CoreIni.ContainsKey( keyName ) )
				{
                    m_RecentFiles[i] = m_CoreIni.ValueOf( keyName );
				}
				else
				{
					m_RecentFiles[i] = "";
				}
			}			
		}

		public string[] recentFileList
		{
			get 
			{ 
				return m_RecentFiles; 
			}
		}	

		private void bringRecentFileToTop(int i)
		{
			string buffer = m_RecentFiles[i];
			for ( int j = i; j > 0; j-- )
			{
				m_RecentFiles[j] = m_RecentFiles[j-1];
			}
			m_RecentFiles[0] = buffer;
			saveToParamsFile();
		}

		private void newRecentFile(string fileName)
		{
			string[] temp = new string[m_Length];
			temp[0] = fileName;
			for ( int i = 0; i < m_Length -1; i++ )
			{
				temp[i+1] = m_RecentFiles[i];
			}
			m_RecentFiles = temp;
			saveToParamsFile();
		}

		public void AddRecentFile( string fileName )
		{
			if ( File.Exists( fileName ) )
			{
				for ( int i = 0; i < m_RecentFiles.Length; i++ )
				{
					if ( m_RecentFiles[i] == fileName )
					{
						bringRecentFileToTop(i);
						return;
					}
				}
				newRecentFile(fileName);
			}
		}
	}
}
