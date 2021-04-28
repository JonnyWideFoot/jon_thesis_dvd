using System;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.IO;
using System.Reflection;

namespace UoB.Core
{
    public sealed class CoreIniOverride
    {
        private static string m_LoadPath = null;
        public static string LoadPath
        {
            get
            {
                return m_LoadPath;
            }
            set
            {
                m_LoadPath = value;
            }
        }
    }

	/// <summary>
	/// UoB Init exhibits the Singleton Pattern : Initialisation occurs once from the
	/// file UoB.ini in the shared program folder
	/// </summary>
	public sealed class CoreIni
	{
		private Hashtable m_Parameters;
		private DirectoryInfo m_DefaultSharedDir = null;
		private DirectoryInfo m_DefaultTempDir = null;
		private const string m_INIFileName = "Core.ini";
		private string m_INIFileFullPath;
		private Version m_InternalVersion = null;
        private bool m_InitialisationOK = false;

		private CoreIni()
		{
			// Get Version info from Core.dll
			GetVersion();

			// init member variables
			m_Parameters = new Hashtable(20);

            if (CoreIniOverride.LoadPath != null)
            {
                m_DefaultSharedDir = new DirectoryInfo(CoreIniOverride.LoadPath);
                m_InitialisationOK = m_DefaultSharedDir.Exists;
                if (!m_InitialisationOK) return; // cant find the shared files, so cant load them
            }
            else
            {
                // now look for and initialise the shared directory
                FileInfo fi = new FileInfo(Assembly.GetEntryAssembly().Location);
                DirectoryInfo di = fi.Directory; // the directory where the executing file is located

                // look for the shared directory
                DirectoryInfo diUpTreeLevel = di.Parent;
                if (diUpTreeLevel != null)
                {
                    diUpTreeLevel = diUpTreeLevel.Parent;
                }

                if (diUpTreeLevel != null) // check for null in case we are in c:\, unlikely, but there is no parent
                {
                    m_DefaultSharedDir = new DirectoryInfo(diUpTreeLevel.FullName + @"/shared/");
                    if (!m_DefaultSharedDir.Exists)
                    {
                        m_DefaultSharedDir = null;
                    }
                }

                if (m_DefaultSharedDir == null)
                {
                    // look in the app sub dir
                    m_DefaultSharedDir = new DirectoryInfo(di.FullName + @"/shared/");
                    if (!m_DefaultSharedDir.Exists)
                    {
                        m_DefaultSharedDir = null;
                        Trace.WriteLine("Unable to find the shared path at either of the expected locations!");
                        m_InitialisationOK = false;
                        return; // cant find the shared files, so cant load them
                    }
                }
            }

			Trace.Write("Using shared file path: ");
			Trace.WriteLine( m_DefaultSharedDir.FullName );

			m_DefaultTempDir = new DirectoryInfo( m_DefaultSharedDir.FullName + @"/temp" );
			if( !m_DefaultTempDir.Exists )
			{
				m_DefaultTempDir.Create();
			}
			Trace.Write("Using temp path: ");
			Trace.WriteLine(m_DefaultTempDir.FullName);

			m_INIFileFullPath = m_DefaultSharedDir.FullName + Path.DirectorySeparatorChar + m_INIFileName;
			if ( !File.Exists(  m_INIFileFullPath ) )
				// this is used to override the shared one if one is present in the same dir as the program
			{
				Trace.WriteLine("Unable to find the ini file in the shared path!");
                m_InitialisationOK = false;
				return; // cant find the shared files, so cant load them
			}

			Debug.WriteLine("CoreIni Initialisation occuring on - " + Thread.CurrentThread.Name);
			loadAppParams();
            m_InitialisationOK = true; // all was ok ..
            return;
		}

		private void GetVersion()
		{
			Assembly a = Assembly.GetAssembly( typeof(CoreIni) );
			AssemblyName an = a.GetName();
			m_InternalVersion = an.Version;
		}

		#region Singleton Implementation
		private static readonly CoreIni instance = new CoreIni();
		public static CoreIni Instance
		{
			get 
			{
				return instance; 
			}
		}
		#endregion

		#region IniIO
		private void loadAppParams()
		{
			StreamReader re = File.OpenText(m_INIFileFullPath);
			string inputLine = null;
			while ((inputLine = re.ReadLine()) != null)
			{
				string[] disection = inputLine.Split(new char[] { '=' }, 2);
				if ( disection.Length != 2 ) continue;
				m_Parameters.Add(disection[0], disection[1]);
			}
			re.Close();
		}

		private void saveParamsFile()
		{
			StreamWriter rw = new StreamWriter(m_INIFileFullPath, false);
			foreach ( string key in m_Parameters.Keys )
			{
				rw.WriteLine( key + "=" + m_Parameters[key] );
			}
			rw.Close();
		}
		#endregion

		#region Public Tools
		public string GetTempFileName( string extension )
		{
			string nameRoot = DefaultTempPath + "temp_";
			int fileExt = 0;
			string name = nameRoot + fileExt.ToString("0000") + extension;
			while( File.Exists( nameRoot + fileExt.ToString("0000") + extension ) )
			{
				fileExt++;
				name = nameRoot + fileExt.ToString("0000") + extension;
			}
			return name;
		}
		#endregion

		#region Public Accessors

        public bool IsInitialised
        {
            get
            {
                return m_InitialisationOK;
            }
        }

		public bool ContainsKey( string key )
		{
			return m_Parameters.ContainsKey( key );
		}

		public string ValueOf( string key )
		{
			return (string) m_Parameters[key];
		}

		public void AddDefinition( string key, string defValue )
		{
			m_Parameters[key] = defValue;
			saveParamsFile();
		}
		

		public string Version
		{
			get
			{
				return m_InternalVersion.ToString();
			}
		}

		public DirectoryInfo DefaultSharedDir
		{
			get
			{
				return m_DefaultSharedDir;
			}
		}
		public DirectoryInfo DefaultTempDir
		{
			get
			{
				return m_DefaultTempDir;
			}
		}
		public string DefaultSharedPath
		{
			get
			{
				return m_DefaultSharedDir.FullName;
			}
		}
		public string DefaultTempPath
		{
			get
			{
				return m_DefaultTempDir.FullName;
			}
		}
		#endregion
	}
}
