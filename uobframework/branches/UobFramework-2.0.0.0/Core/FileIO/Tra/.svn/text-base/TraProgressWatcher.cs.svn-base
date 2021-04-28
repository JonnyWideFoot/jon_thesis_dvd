using System;
using System.IO;

namespace UoB.Core.FileIO.Tra
{
	/// <summary>
	/// Summary description for TraProgressWatcher.
	/// </summary>
	public class TraProgressWatcher
	{
		private float m_Progress = 0.0f;
		private string m_FileName;
		private bool m_HeaderScanned = false;
		private long m_ExpectedTotalSize = 1;
		private long m_TotalNumberOfEntries = 1;
		private int trajectorystart = 0;

		public TraProgressWatcher( string fileName, int totalNumberOfEntries )
		{
			m_TotalNumberOfEntries = totalNumberOfEntries;
			if( totalNumberOfEntries < 0 )
			{
				throw new TraException("The number of entries cannot be less than 0");
			}
			m_FileName = fileName;
		}

		public void ScanHeader()
		{
			try
			{
				FileStream fs = new FileStream(m_FileName, FileMode.Open, FileAccess.Read);
				if( fs.Length < 24 ) return; // the header hasnt written yet ...
				
				BinaryReader r = new BinaryReader(fs);
				
				int version = r.ReadInt32();
				int type = r.ReadInt32();
				int residues = r.ReadInt32();
				int atoms = r.ReadInt32();
				int blocksize = r.ReadInt32();
				trajectorystart = r.ReadInt32();

														// 8 = "TRASTART"
				m_ExpectedTotalSize = trajectorystart + 8 + ( blocksize * m_TotalNumberOfEntries );

                r.Close();

			}
			catch
			{
				// we couldnt open the file, or the file is a pile of shite, so we dont want to read it anyhow
			}
			m_HeaderScanned = true; // the header was scanned sucessfully
		}

		public float Progress
		{
			get
			{
				if( !m_HeaderScanned )
				{
					ScanHeader();
				}
				try
				{
					FileInfo fileInfo = new FileInfo( m_FileName );
					// - 18084.0f ... the size of the header, not a good measure of progress if you leave it in ...
					m_Progress = (float) ( fileInfo.Length - trajectorystart ) / (float) ( m_ExpectedTotalSize - trajectorystart );
				}
				catch
				{
				}
				return m_Progress;
			}
		}
	}
}
