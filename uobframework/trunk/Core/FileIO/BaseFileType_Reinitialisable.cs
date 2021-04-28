using System;
using System.IO;

namespace UoB.Core.FileIO
{
	/// <summary>
	/// Summary description for FileType_Reinitialisable.
	/// </summary>
	public abstract class BaseFileType_Reinitialisable : BaseFileType
	{
		public BaseFileType_Reinitialisable() : base()
		{
		}

		/// <summary>
		/// Should be callable at any time
		/// </summary>
		public virtual void ClearFile() 
		{
			m_ExtendedInfo = null;
			m_FileInfo = null;
			m_InternalName = string.Empty;
		}

		public void Reinitialise( string fileName )
		{
			ClearFile(); // m_FileInfo is set to null, and therefore the file is flagged as invalid
			Initialise( fileName ); // call this again with the new filename
		}
	}
}
