using System;
using System.IO;
using System.Text;
using System.Collections;

using UoB.Core.Structure;

namespace UoB.Core.Sequence
{
	/// <summary>
	/// Summary description for Sequence.
	/// </summary>
	public class PSSequence
	{
		protected Hashtable m_SequenceBin; // indexed by the PS chainIDs
		private UpdateEvent m_PSUpdate;
		protected ParticleSystem m_PS;
		protected StringBuilder m_TempBuilder = new StringBuilder();

		public PSSequence( ParticleSystem ps )
		{
			m_SequenceBin = new Hashtable();
			m_PSUpdate = new UpdateEvent(SyncAndObtain);
			particleSystem = ps;
		}

		public PSSequence()
		{
			m_SequenceBin = new Hashtable();
			m_PSUpdate = new UpdateEvent(SyncAndObtain);
		}

		public ParticleSystem particleSystem
		{
			get
			{
				return m_PS;
			}
			set
			{
				if( m_PS != null )
				{
					m_PS.ContentUpdate -= m_PSUpdate;
				}
				m_PS = value;
				if( m_PS != null )
				{
					m_PS.ContentUpdate += m_PSUpdate;
					SyncAndObtain();
				}				
			}
		}

		protected void SyncAndObtain()
		{
			int attempts = 0;
			if( m_PS != null )
			{
				while( true )
				{
					try
					{
						m_PS.AcquireReaderLock(400);
						GetSeq();
						break;
					}
					catch
					{
						attempts++;
						if( attempts > 10 )
						{
							break; // fail
						}
					}
					finally
					{
						m_PS.ReleaseReaderLock();
					}
				}
			}
		}

		protected void GetSeq()
		{
			m_SequenceBin.Clear();
			if( m_PS != null )
			{
				for( int i = 0; i < m_PS.Members.Length; i++ )
				{
					m_SequenceBin.Add( m_PS.Members[i].ChainID, m_PS.Members[i].MonomerString );
				}
			}
		}

		public string this[ char chainID ]
		{
			get
			{
				if( m_SequenceBin.ContainsKey( chainID ) )
				{
					return (string) m_SequenceBin[ chainID ];
				}
				else
				{
					return "No Seqence Present for ChainID : " + chainID;
				}
			}
		}

		public override string ToString()
		{
			m_TempBuilder.Remove(0,m_TempBuilder.Length); // clear to initialise

			m_TempBuilder.Append( "Sequence information extracted from the structural information present in the file.\r\n\r\n" );

			IDictionaryEnumerator myEnumerator = m_SequenceBin.GetEnumerator();
			while ( myEnumerator.MoveNext() )
			{
				m_TempBuilder.Append( m_PS.Name );
				m_TempBuilder.Append( " chainID : " );
				char chainID = (char) myEnumerator.Key;
				if( chainID == '\0' )
				{
					chainID = ' ';
				}
				m_TempBuilder.Append( chainID );
				m_TempBuilder.Append( "\r\n" );
				m_TempBuilder.Append( (string) myEnumerator.Value );
				m_TempBuilder.Append( "\r\n" );
			}

			return m_TempBuilder.ToString();
		}
	}
}
