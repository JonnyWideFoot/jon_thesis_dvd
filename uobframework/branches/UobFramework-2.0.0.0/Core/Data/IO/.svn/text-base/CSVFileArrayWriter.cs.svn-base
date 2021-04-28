using System;
using System.IO;

namespace UoB.Core.Data.IO
{
	/// <summary>
	/// Summary description for DebugTools.
	/// </summary>
	public sealed class CSVFileArrayWriter
	{
		private StreamWriter m_WriteTo;
		private int m_ReportID = 0;
		private bool m_IsOpen = false;
		private string m_NameStem;
		private DirectoryInfo m_WritePath;

		public CSVFileArrayWriter( string nameStem )
		{
			m_WritePath = CoreIni.Instance.DefaultTempDir;
			m_NameStem = nameStem;
		}

		public CSVFileArrayWriter( DirectoryInfo outDir, string nameStem )
		{
			m_WritePath = outDir;
			m_NameStem = nameStem;
		}

		public void ClearMatchingReportFiles()
		{
			FileInfo[] files = m_WritePath.GetFiles( m_NameStem + "_*" + ".csv" );
			for( int i = 0; i < files.Length; i++ )
			{
				files[i].Delete();
			}
		}

		public DirectoryInfo WriteDirectory
		{
			get
			{
				return m_WritePath;
			}
			set
			{
				if( !m_WritePath.Exists )
				{
					throw new IOException("Given Path does not exist");
				}
				m_WritePath = value;
			}
		}

		public string NameStem
		{
			get
			{
				return m_NameStem;
			}
			set
			{
				m_NameStem = Core.Tools.CommonTools.ReturnStringWithIllegalCharsFromFilenameRemoved( value );
			}
		}

		public bool IsOpen
		{
			get
			{
				return m_IsOpen;
			}
		}
		
		private void openFileWrite()
		{
			if( m_IsOpen )
			{
				throw new IOException("The csv filestream is already open!");
			}
			m_IsOpen = true;
			m_WriteTo = new StreamWriter( m_WritePath.FullName + Path.DirectorySeparatorChar 
				+ m_NameStem + '_' + m_ReportID.ToString() + ".csv" );
		}

		private void CloseFileWrite()
		{
			if( !m_IsOpen )
			{
				throw new IOException("The csv filestream isn't open for writing!");
			}
			m_IsOpen = false;
			m_WriteTo.Close();
		}

		public void SetCSVReportID( int index )
		{
			m_ReportID = index;
		}

		public void AdvanceCSVReportID()
		{
			m_ReportID++;
		}

		public void ResetCSVReportID()
		{
			m_ReportID = 0;
		}

		public void WriteReport( int[,,] data )
		{
			openFileWrite();
			AppendReport( data );
			CloseFileWrite();
			AdvanceCSVReportID();
		}

		public void AppendReport( int[,,] data )
		{
			int numRows = data.GetUpperBound(0) + 1;
			int numColumns = data.GetUpperBound(1) + 1;
			int depth = data.GetUpperBound(2) + 1;

			m_WriteTo.Write(',');
			for( int j = 0; j < numColumns; j++ )
			{
				m_WriteTo.Write( 'Y' + j.ToString() + ',' );
			}
			m_WriteTo.WriteLine();

			for( int i = 0; i < numRows; i++ )
			{
				m_WriteTo.Write( 'X' + i.ToString() + ',' );
				for( int j = 0; j < numColumns; j++ )
				{
					m_WriteTo.Write( '\'' ); // stops excel doing "helpful" stuff like tirning my numbers into dates
					for( int k = 0; k < depth; k++ )
					{
						m_WriteTo.Write( data[i,j,k].ToString() + ':' );
					}
					m_WriteTo.Write( ',' );
				}
				m_WriteTo.WriteLine();
			}
		}

		public void WriteReport( int[,] data )
		{
			openFileWrite();
			AppendReport( data );
			CloseFileWrite();
			AdvanceCSVReportID();
		}

		public void AppendReport( int[,] data )
		{
			int numRows = data.GetUpperBound(0) + 1;
			int numColumns = data.GetUpperBound(1) + 1;

			m_WriteTo.Write(',');
			for( int j = 0; j < numColumns; j++ )
			{
				m_WriteTo.Write( 'Y' + j.ToString() + ',' );
			}
			m_WriteTo.WriteLine();

			for( int i = 0; i < numRows; i++ )
			{
				m_WriteTo.Write( 'X' + i.ToString() + ',' );
				for( int j = 0; j < numColumns; j++ )
				{
					m_WriteTo.Write( data[i,j].ToString() + ',' );
				}
				m_WriteTo.WriteLine();
			}
		}

		public void WriteReport( float[,] data )
		{
			openFileWrite();
			AppendReport( data );
			CloseFileWrite();
			AdvanceCSVReportID();
		}

		public void AppendReport( float[,] data )
		{
			int numRows = data.GetUpperBound(0) + 1;
			int numColumns = data.GetUpperBound(1) + 1;

			m_WriteTo.Write(',');
			for( int j = 0; j < numColumns; j++ )
			{
				m_WriteTo.Write( 'Y' + j.ToString() + ',' );
			}
			m_WriteTo.WriteLine();

			for( int i = 0; i < numRows; i++ )
			{
				m_WriteTo.Write( 'X' + i.ToString() + ',' );
				for( int j = 0; j < numColumns; j++ )
				{
					m_WriteTo.Write( data[i,j].ToString() + ',' );
				}
				m_WriteTo.WriteLine();
			}
		}
	}
}
