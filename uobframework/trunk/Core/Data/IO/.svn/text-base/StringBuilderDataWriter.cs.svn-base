using System;
using System.Text;

namespace UoB.Core.Data.IO
{
	/// <summary>
	/// Summary description for StringBuilderDataWriter.
	/// </summary>
	public class StringBuilderDataWriter
	{
		private StringBuilderDataWriter()
		{
		}

		public static string GetString( string[] titles, char delimiter, params float[][] data )
		{
			if( titles.Length != data.Length )
			{
				throw new ArgumentException("The data array number and title count dont match");
			}
			if( data.Length == 0 )
			{
				throw new ArgumentException("The data array contains no data to write!");
			}
			StringBuilder report = new StringBuilder( data[0].Length * data.Length * 6 );

			int longestArray = 0;
			for( int i = 0; i < titles.Length; i++ )
			{
				report.Append( titles[i] );
				if( i < titles.Length - 1 )
				{
					report.Append( delimiter );
				}
				else
				{
					report.Append( "\r\n" );
				}
				if( data[i].Length > longestArray )
				{
					longestArray = data[i].Length;
				}
			}

			int dataIndex = 0;
			while( dataIndex < longestArray )
			{
				for( int j = 0; j < data.Length; j++ )
				{
					if( dataIndex < data[j].Length ) 
					{ report.Append( data[j][dataIndex] ); }
					else { report.Append( '-' ); }

					if( j < data.Length - 1 )
					{
						report.Append( delimiter );
					}
					else
					{
						report.Append( "\r\n" );
					}
				}
				dataIndex++;
			}

			return report.ToString();
		}
	}
}
