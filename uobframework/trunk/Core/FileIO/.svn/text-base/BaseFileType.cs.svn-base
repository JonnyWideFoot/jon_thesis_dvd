using System;
using System.Security;
using System.IO;

namespace UoB.Core.FileIO
{
	/// <summary>
	/// Summary description for FileType.
	/// </summary>
	public abstract class BaseFileType
	{
		protected ExtendedInfo m_ExtendedInfo;
		protected FileInfo m_FileInfo;
		protected string m_InternalName;
		protected bool m_CanSave = false;

		public BaseFileType()
		{
			m_ExtendedInfo = null;
			m_FileInfo = null;
			m_InternalName = string.Empty;
		}

        protected bool m_Silent = true;
        public virtual bool Silent
        {
            get
            {
                return m_Silent;
            }
            set
            {
                m_Silent = value;
            }
        }

		private bool IsPathValid( string path )
		{
			if (path == null || path.Trim().Length == 0)
				return false;
			try
			{
				Path.GetFullPath(path);
				return true;
			}
			catch (ArgumentException)
			{
			}
			catch (SecurityException)
			{
			}
			catch (NotSupportedException)
			{
			}
			catch (PathTooLongException)
			{
			}
			return false;
		}

		public abstract void Save( SaveParams saveParams );
		public abstract void Save( string fileName );

		public void LoadFromFile( string fileName )
		{
			if( IsValidFile )
			{
				throw new Exception("The file is already fully loaded, reloading cannot occur!");
			}
			else
			{
				Initialise( fileName );
			}
		}

		protected virtual void Initialise( string fileName )
		{
			if( IsPathValid( fileName ) )
			{
				FileInfo fi = new FileInfo( fileName );
				if( fi.Exists )
				{
					m_FileInfo = fi;
					m_InternalName = m_FileInfo.Name;
					ExtractExtendedInfo();
				}
			}
		}

		protected virtual void ExtractExtendedInfo()
		{
			m_ExtendedInfo = new ExtendedInfo();
		}

		public bool IsValidFile
		{
			get
			{
				return m_FileInfo != null;
			}
		}

		public string InternalName
		{
			get
			{
				return m_InternalName;
			}
//			set
//			{
//				m_Name = value;
//			}
		}

		public ExtendedInfo ExtendedInformation
		{
			get
			{
				return m_ExtendedInfo;
			}
		}

		public FileInfo fileInfo
		{
			get
			{
				return m_FileInfo;
			}
		}

		public string FullFileName
		{
			get
			{
				return m_FileInfo.FullName;
			}
		}

		public bool CanSave
		{
			get
			{
				return m_CanSave;
			}
		}
	}
}
