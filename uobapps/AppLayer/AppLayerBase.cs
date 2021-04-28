using System;
using System.IO;

namespace UoB.AppLayer
{
	/// <summary>
	/// Summary description for AppLayerBase.
	/// </summary>
	public abstract class AppLayerBase : IAppLayer
	{
		private DirectoryInfo m_ExecDir;
		private DirectoryInfo m_TaskDir;

		public AppLayerBase()
		{
			FileInfo startingFile = new FileInfo( System.Reflection.Assembly.GetEntryAssembly().Location );
            m_ExecDir = startingFile.Directory;
#if DEBUG
            m_TaskDir = new DirectoryInfo(DriveRoot(FolderName) + FolderName + Path.DirectorySeparatorChar);
#else
            m_TaskDir = new DirectoryInfo(ExecDir.Parent.FullName + Path.DirectorySeparatorChar);
#endif
		}

        public string DriveRoot(string _end)
        {
            if (_end[_end.Length - 1] != Path.DirectorySeparatorChar) 
                _end += Path.DirectorySeparatorChar;

            string try1 = null;

            try1 = @"C:\";
            if (Directory.Exists(try1 + _end)) return try1;
            try1 = @"E:\";
            if (Directory.Exists(try1 + _end)) return try1;
            try1 = @"S:\";
            if (Directory.Exists(try1 + _end)) return try1;
            try1 = @"M:\_DataDriveOverflow\";
            if (Directory.Exists(try1 + _end)) return try1;
            try1 = @"I:\_Work\"; // home
            if (Directory.Exists(try1 + _end)) return try1;
            try1 = @"Z:\maxsync\"; // beast
            if (Directory.Exists(try1 + _end)) return try1;
            try1 = @"Y:\";
            if (Directory.Exists(try1 + _end)) return try1;
            
            throw new Exception("Could not find target directory");
        }

        public abstract string MethodPrintName
        {
            get;
        }

        public virtual string FolderName
        {
            get
            {
                return MethodPrintName;
            }
        }

        protected string m_DBName = "";
        public virtual string DBName
        {
            get
            {
                return m_DBName;
            }
        }

        public string LogPath
        {
            get
            {
                return m_TaskDir.FullName + "Report" + Path.DirectorySeparatorChar;
            }
        }

		public virtual void MainStem( string[] args )
		{
			// nothing
		}

		public virtual bool RequiresForcefield
		{
			get
			{
				return false;
			}
		}

		public DirectoryInfo ExecDir
		{
			get
			{
                return m_ExecDir;
			}
		}

        public void AssignTaskDir(string path)
        {
            TaskDir = new DirectoryInfo(path);            
        }

		public DirectoryInfo TaskDir
		{
			get
			{
				return m_TaskDir;
			}
			set
			{
                m_TaskDir = value;
                if (!m_TaskDir.Exists)
                {
                    throw new ArgumentException("Task Directory doesnt exist!");
                }	
                if (m_TaskDir.FullName[m_TaskDir.FullName.Length - 1] != Path.DirectorySeparatorChar)
                {
                    m_TaskDir = new DirectoryInfo(m_TaskDir.FullName + Path.DirectorySeparatorChar);
                }          
			}
		}
	}
}
