using System;

namespace UoB.Core.FileIO.Tra
{
	/// <summary>
	/// Summary description for TraSaveInfo.
	/// </summary>
	public sealed class TraSaveInfo : SaveParams
	{
		private TraContents m_ContentTypes;
		public int startIndex = 1;
		public int indexStepping = 1; // >= 1
		public int endIndex = 0;
		private string m_ID0 = "AMBERAA".PadRight(32,'\0'); // Length 32 - custom ID strings
		private string m_ID1 = "null".PadRight(32,'\0');
		private string m_ID2 = "null".PadRight(32,'\0');
		private string m_ID3 = "DAVEGEN".PadRight(32,'\0');
		private string m_CustomPropertyBlock = "";
		private string m_Descriptor = "".PadRight(1024,'\0');                 // Length 1024      - custom 1K descriptor ASCII area
		private string m_Text = "".PadRight(15360,'\0');       // Length 1024 * 15 - custom 15K descriptor ASCII area


		public TraSaveInfo( string fileName ) : base( fileName )
		{
			// default
			m_ContentTypes = TraContents.Positions | TraContents.EnergyInfo | TraContents.TrajectoryEntries;
		}

		public TraSaveInfo( string fileName, TraContents tc ) : base( fileName )
		{
			m_ContentTypes = tc;
		}

		public TraContents contentTypes
		{
			get
			{
				return m_ContentTypes;
			}
			set
			{
				m_ContentTypes = value;
			}
		}

		internal string ID0
		{
			get
			{
				return m_ID0;
			}
			set
			{
				string s = value;
				if ( s.Length > 32 )
				{
					s = s.Substring(0,32);
				}
				s = s.PadRight(32,'\0');
				m_ID0 = s;
			}
		}

		internal string ID1
		{
			get
			{
				return m_ID1;
			}
			set
			{
				string s = value;
				if ( s.Length > 32 )
				{
					s = s.Substring(0,32);
				}
				s = s.PadRight(32,'\0');
				m_ID1 = s;
			}
		}

		internal string ID2
		{
			get
			{
				return m_ID2;
			}
			set
			{
				string s = value;
				if ( s.Length > 32 )
				{
					s = s.Substring(0,32);
				}
				s = s.PadRight(32,'\0');
				m_ID2 = s;
			}
		}

		internal string ID3
		{
			get
			{
				return m_ID3;
			}
			set
			{
				string s = value;
				if ( s.Length > 32 )
				{
					s = s.Substring(0,32);
				}
				s = s.PadRight(32,'\0');
				m_ID3 = s;
			}
		}

		public void setPropertyBlock( TraHeader h )
		{
			m_CustomPropertyBlock = "";
			int jCount = h.customAtomProperty.Length / h.customAtomProperty.GetLength( 0 );
			int iCount = h.customAtomProperty.GetLength( 0 );
			for( int i = 0; i < iCount; i++ )
			{
				for( int j = 0; j < jCount; j++ )
				{
					m_CustomPropertyBlock += h.customAtomProperty[i,j];
				}
			}
			jCount = h.customEnergyEntry.Length / h.customEnergyEntry.GetLength( 0 );
			iCount = h.customEnergyEntry.GetLength( 0 );
			for( int i = 0; i < iCount; i++ )
			{
				for( int j = 0; j < jCount; j++ )
				{
					m_CustomPropertyBlock += h.customEnergyEntry[i,j];
				}
			}
		}

		public string CustomPropertyBlock
		{
			get
			{
				return m_CustomPropertyBlock.PadRight(1536,'\0');
			}
		}

		public string Descriptor
		{
			get
			{
				return m_Descriptor;
			}
			set
			{
				m_Descriptor = value.PadRight(1024,'\0');
			}
		}

		public string Text
		{
			get
			{
				return m_Text;
			}
			set
			{
				m_Text = value.PadRight(15360,'\0'); // 15360 = 1024 * 15
			}
		}
	}
}
