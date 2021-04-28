using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Diagnostics;

using UoB.Core.Data.IO;
using UoB.Core.Data.Graphing;
using UoB.Core.FileIO.Tra;
using UoB.Core.Structure;

namespace UoB.Core.Data
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class DataManager
	{
		private ArrayList m_DataListings;
		private ArrayList m_Graphs;

		private DataListing m_CachedStepNumberXAxis = null;

		public event IntProgressEvent DataPosition;
		public event UpdateEvent DataUpdated;
		public event UpdateEvent GraphListUpdated;
		private bool m_Editing = false;
		private string m_MainTitle;
		private PS_PositionStore m_Tra = null;

		public DataManager()
		{
			m_MainTitle = "Untitled";
			init();
		}

		public void Clear()
		{
			m_DataListings.Clear();
			m_Graphs.Clear();
			DataUpdated();
			GraphListUpdated();
		}

        public DataManager( string mainTitle )
		{
			m_MainTitle = mainTitle;
			init();
		}

		public DataManager( string mainTitle, PS_PositionStore tra )
		{
			if( tra != null )
			{
				m_Tra = tra;
				tra.IndexChanged += new UpdateEvent(tra_IndexChanged);
			}
			m_MainTitle = mainTitle;
			init();
		}

		private void init()
		{
			m_DataListings = new ArrayList(10);
			m_Graphs = new ArrayList(10);

			DataUpdated = new UpdateEvent( nullFunc );
			GraphListUpdated = new UpdateEvent( nullFunc );
			DataPosition = new IntProgressEvent( nullFunc );
		}

		private void tra_IndexChanged()
		{
			DataPosition( m_Tra.Position, m_Tra.Count );
		}

		private void nullFunc()
		{
		}

		private void nullFunc(int i, int j)
		{
		}

		public void AddDataColumn(string title, float[] data)
		{
			DataListing l = new DataListing(title, data);
			m_DataListings.Add(l);
			if ( !m_Editing )
			{
				DataUpdated();
			}
		}

		public bool EditingData
		{
			get
			{
				return m_Editing;
			}
			set
			{
				m_Editing = value;
				if ( m_Editing == false )
				{
					GraphListUpdated();
					DataUpdated();
				}
			}
		}

		public void RemoveGraphs( int[] indexes )
		{
			for ( int i = 0; i < indexes.Length; i++ )
			{
				m_Graphs.RemoveAt( indexes[i] );
			}
			GraphListUpdated();
		}

		public void SaveGraphs( string path, int[] indexes )
		{
			for ( int i = 0; i < indexes.Length; i++ )
			{
				DataWriter.writeGraph(path, (Graph) m_Graphs[indexes[i]]);
			}
		}

		private bool isInRange( int number )
		{
			return ( number >= 0 && number < m_DataListings.Count );
		}

		public bool MatchesTimeStamp( string name )
		{
			return Regex.IsMatch( name, "Time", RegexOptions.IgnoreCase );
		}

		public bool MatchesStepStamp( string name )
		{
			return Regex.IsMatch( name, "Step", RegexOptions.IgnoreCase );
		}

		public void WriteData( string fileName, int[] indexes )
		{
			for ( int i = 0; i < indexes.Length; i++ )
			{
				DataListing dl = (DataListing) m_DataListings[indexes[i]];
				DataWriter.writeDataColumn( fileName, true, dl.Name, dl.Data );
			}
		}

		public bool ContainsStepStamp( out int index )
		{
			for ( int i = 0; i < m_DataListings.Count; i++ )
			{
				DataListing d = (DataListing) m_DataListings[i];
				if ( MatchesStepStamp( d.Name ) )
				{
					index = i; // we are assuming one is present ... hmmmm
					return true;
				}
			}

			index = -1;
			return false;
		}

		public bool ContainsTimeStamp( out int index )
		{
			for ( int i = 0; i < m_DataListings.Count; i++ )
			{
				DataListing d = (DataListing) m_DataListings[i];
				if ( MatchesTimeStamp( d.Name ) )
				{
					index = i; // we are assuming one is present ... hmmmm
					return true;
				}
			}

			index = -1;
			return false;
		}

		public void AddGraphDefinition( string title, int indexY, PlotMode mode )
		{
			int indexTimeStamp;
			if ( (mode == PlotMode.TimeStampIfAvailable) && ContainsTimeStamp( out indexTimeStamp ) )
			{
				AddGraphDefinition( title, indexTimeStamp, indexY );
			}
			else
			{
				int indexStepStamp;
				if( (mode == PlotMode.StepNumberIfAvailable) && ContainsStepStamp( out indexStepStamp ) )
				{
					AddGraphDefinition( title, indexStepStamp, indexY );
				}
				else
				{
					AddGraphDefinition( title, -1, indexY );
				}
			}
				
		}

		public void AddGraphDefinition( string title, int indexX, int indexY )
		{
			if ( isInRange(indexX) && isInRange(indexY) )
			{
				Graph g = new Graph( title, (DataListing) m_DataListings[indexX],  (DataListing) m_DataListings[indexY] );
				m_Graphs.Add( g );
			}
			else
			{
				if ( isInRange( indexY ) && indexX == -1 )
				{
					DataListing d = (DataListing) m_DataListings[indexY];

					if ( m_CachedStepNumberXAxis == null || m_CachedStepNumberXAxis.Data.Length != d.Data.Length )
					{
						float[] x = new float[ d.Data.Length ];
						for ( int i = 0; i < d.Data.Length; i++ )
						{
							x[i] = (float)i;
						}
						m_CachedStepNumberXAxis = new DataListing( "Step Number", x );
					}

					Graph g = new Graph( title, m_CachedStepNumberXAxis,  (DataListing) m_DataListings[indexY] );
					m_Graphs.Add( g );
				}
			}
			if ( !m_Editing )
			{
				GraphListUpdated();
			}
		}

		public string MainTitle
		{
			get
			{
				return m_MainTitle;
			}
		}

		public void DataSummaryTitles( out string[] titles )
		{
			titles = new string[ m_DataListings.Count ];

			try
			{
				for( int i = 0; i < m_DataListings.Count; i++ )
				{
					DataListing dl = (DataListing) m_DataListings[i];
					titles[i] = dl.Name;
				}
			}
			catch
			{
				Debug.WriteLine("Something fishy is going on in the data manager!");
			}
		}

		public void DataSummary( int dataIndex, out float[] values )
		{
			values = new float[ m_DataListings.Count ];

			try
			{
				for( int i = 0; i < m_DataListings.Count; i++ )
				{
					DataListing dl = (DataListing) m_DataListings[i];
					values[i] = dl.Data[dataIndex];
				}
			}
			catch
			{
				Debug.WriteLine("Something fishy is going on in the data manager!");
			}
		}

		public DataListing[] DataListings
		{
			get
			{
				return (DataListing[]) m_DataListings.ToArray( typeof(DataListing) );
			}
		}

		public DataListing GetDataListing( int atIndex )
		{
			if( atIndex >= 0 && atIndex < m_DataListings.Count )
			{
				return (DataListing) m_DataListings[ atIndex ];
			}
			else
			{
				throw new ArgumentOutOfRangeException( "atIndex" );
			}
		}

		public int GraphCount
		{
			get
			{
				return m_Graphs.Count;
			}
		}

		public Graph[] Graphs
		{
			get
			{
				return (Graph[]) m_Graphs.ToArray( typeof(Graph) );
			}
		}

		public PS_PositionStore trajectory
		{
			get
			{
				return m_Tra;
			}
		}
	}
}
