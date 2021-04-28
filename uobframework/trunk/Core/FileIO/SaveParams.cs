using System;

namespace UoB.Core.FileIO
{
	/// <summary>
	/// Summary description for SaveParams.
	/// </summary>
	public class SaveParams
	{
		protected string m_FileName;
		protected bool m_SaveSequence;

		public SaveParams( string fileName, bool saveSequence )
		{
			m_FileName = fileName;
			m_SaveSequence = saveSequence;
		}

		public SaveParams( string fileName )
		{
			m_FileName = fileName;
			m_SaveSequence = false;
		}

		public string FileName
		{
			get
			{
				return m_FileName;
			}
		}

		public bool SaveSequence
		{
			get
			{
				return m_SaveSequence;
			}
		}
	}
}
