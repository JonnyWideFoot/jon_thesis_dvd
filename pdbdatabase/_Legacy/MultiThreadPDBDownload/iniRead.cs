using System;
using System.IO;
using System.Collections;

namespace getPDBFile
{
	/// <summary>
	/// Summary description for iniRead.
	/// </summary>
	public class iniRead
	{
		private Hashtable m_Hashtable;
		private string m_Path;

		public iniRead(string filePath)
		{
			m_Path = filePath;
			m_Hashtable = new Hashtable();

			getIniInfo();
		}

		public bool containsKey( string ID )
		{
			return m_Hashtable.ContainsKey( ID );
		}

		public bool containsValue( string Value )
		{
			return m_Hashtable.ContainsValue( Value );
		}

		public string valueOf( string ID )
		{
			if ( m_Hashtable.ContainsKey(ID) )
			{
				return (string) m_Hashtable[ ID ];
			}
			else
			{
				return null;
			}
		}

		private void getIniInfo()
		{
			StreamReader re = new StreamReader( m_Path );
			string line;
			string[] lineparts;
			while ( ( line = re.ReadLine() ) != null )
			{
				lineparts = line.Split(new char[] { '=' } ,2);
				m_Hashtable[ lineparts[0].ToLower() ] = lineparts[1];				
			}
			re.Close();
		}
	}
}
