using System;
using System.IO;
using System.Collections;

using UoB.Core.Data.Graphing;
using UoB.Core.Tools;
using UoB.Core.Primitives.Collections;

namespace UoB.Core.Data.IO
{
	public class DataWriter
	{

		private DataWriter()
		{
		}

		private static string makeCommaString(int length)
		{
			string returnString = "";
			for ( int i = 0; i < length; i++ )
			{
				returnString += ",";
			}
			return returnString;
		}

		public static void writeGraph( string path, Graph g )
		{
			if ( !Directory.Exists( path ) )
			{
				Directory.CreateDirectory( path );
			}

			DirectoryInfo di = new DirectoryInfo( path );
			string name = g.GraphTitle;

            name = CommonTools.ReturnStringWithIllegalCharsFromFilenameRemoved( name );
						
			string fileName = di.FullName + name + ".csv";

			writeDataColumn( fileName, false, g.xAxisLabel, g.XData );
			writeDataColumn( fileName, true, g.yAxisLabel, g.YData );
		}

		public static void writeDataColumn(string fileName, bool append, string dataTitle, float[] theData)
		{
			writeDataColumn( fileName, append, dataTitle, ArrayTools.convertToDoubleAr( theData ) );
		}

		public static void writeDataColumn(string fileName, bool append, string dataTitle, FloatArrayList theData)
		{
			writeDataColumn( fileName, append, dataTitle, ArrayTools.convertToDoubleAr( theData ) );
		}

		public static void writeDataColumn(string fileName, bool append, string dataTitle, double[] theData)
		{
			StreamWriter rw;

			if ( !append || !File.Exists(fileName))
			{
				rw = new StreamWriter(fileName, false);
				rw.WriteLine(dataTitle);
				foreach ( double d in theData )
				{
					rw.WriteLine(d.ToString());
				}
				rw.Close();
			}
			else
			{
				ArrayList strings = new ArrayList();
				int maxWidth = 0;

				StreamReader reader = new StreamReader(fileName);
				string inputLine;
				while ((inputLine = reader.ReadLine()) != null)
				{
					string[] split = inputLine.Split(',');
					inputLine.Trim('\n','\r');
					strings.Add(inputLine);
					if (split.Length > maxWidth) maxWidth = split.Length;
				}
				reader.Close(); 

				// should work without this
				//				if ( strings.Count == 0 )
				//				{
				//					writeAsColumn(fileName, dataTitle, theData);
				//					return;
				//				}

				for ( int i = 0; i < strings.Count; i++ ) // to fix weird short lines made by excel
				{
					string s = (string) strings[i];
					int count = s.Split(',').Length;
					if ( count < maxWidth )
					{
						s += makeCommaString(maxWidth - count);
					}
					strings[i] = s;
				}

				if( strings.Count == 0 ) // our file was null file size and contained no columns
				{
					rw = new StreamWriter(fileName, false);
					rw.WriteLine(dataTitle);
					foreach ( double d in theData )
					{
						rw.WriteLine(d.ToString());
					}
					rw.Close();
					return;
				}

				string commas = makeCommaString(maxWidth-1); // used if the new data has more rows

				rw = new StreamWriter(fileName);

				int prevDataRowCount = strings.Count;
				int newDataRowCount = theData.Length;
				int countTo;
				bool newDataIsBigger;
				if ( prevDataRowCount > (newDataRowCount + 1) )  // +1 as new data has a title
				{
					countTo = newDataRowCount;
					newDataIsBigger = false;
				}
				else
				{
					countTo = prevDataRowCount;
					newDataIsBigger = true;
				}

				rw.WriteLine((string)strings[0] + "," + dataTitle); // we know strings will be bigger than 0 in length

				for ( int i = 1; i < countTo; i++ )
				{
					rw.WriteLine((string)strings[i] + "," + theData[i-1]);
				}

				if ( newDataIsBigger )
				{
					for ( int i = countTo; i < theData.Length; i++ )
					{
						rw.WriteLine(commas + "," + theData[i]);
					}
				}
				else
				{
					for ( int i = countTo + 1; i < strings.Count; i++ )
					{
						rw.WriteLine((string)strings[i] + ",");
					}
				}

				rw.Close();
			}
		}


		public static void writeTitleRow( string fileName, bool append, string[] titles)
		{
			string titleString = "";
			foreach ( string s in titles )
			{
				titleString += s;
				titleString += ",";
			}
            writeTitleRow( fileName, append, titleString );
		}

		public static void writeTitleRow( string fileName, bool append, string commaSeparatedTitles )
		{
            StreamWriter rw = new StreamWriter( fileName, append );
			if ( append )
			{
				rw.WriteLine("");
			}
			rw.WriteLine( commaSeparatedTitles );
			rw.Close();
		}

		public static void writeDataRow(string fileName, bool append, float[] theData)
		{
			writeDataRow(fileName, append, ArrayTools.convertToDoubleAr(theData));
		}

		public static void writeDataRow(string fileName, bool append, double[] theData)
		{
			StreamWriter rw;
			string sumString = "";
			if ( ! File.Exists(fileName) ) File.Create(fileName).Close();

			if ( !append )
			{
				rw = new StreamWriter(fileName, append);
				sumString = "";
				foreach ( double d in theData )
				{
					sumString += d.ToString() + ",";
				}
				rw.WriteLine(sumString);
				rw.Close();
				return;
			}
			else
			{
				ArrayList strings = new ArrayList();

				StreamReader re = new StreamReader(fileName);

				int maxWidth = 0;
				string inputLine;

				while ((inputLine = re.ReadLine()) != null)
				{
					string[] split = inputLine.Split(',');
					inputLine.Trim('\n','\r');
					strings.Add(inputLine);
					if (split.Length > maxWidth) maxWidth = split.Length;
				}

				if ( maxWidth < theData.Length ) maxWidth = theData.Length; // to fix weird short lines made by excel
				for ( int i = 0; i < strings.Count; i++ )
				{
					string s = (string) strings[i];
					int count = s.Split(',').Length;
					if ( count < maxWidth )
					{
						s += makeCommaString(maxWidth - count);
					}
					strings[i] = s;
				}

				re.Close();

				rw = new StreamWriter(fileName);

				foreach ( string s in strings)
				{
					rw.WriteLine(s);
				}

				sumString = "";
				foreach ( double f in theData )
				{
					sumString += f.ToString() + ",";
				}
				rw.WriteLine(sumString);
				rw.Close();
				return;
			}
		}

		/// <summary>
		/// older function needs to be integrated
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="Data3D"></param>
		private void WriteDataTable( string fileName, float[,,] Data3D )
		{
			int xIndexer = Data3D.GetUpperBound(0);
			int yIndexer = Data3D.GetUpperBound(1);
			int zIndexer = Data3D.GetUpperBound(2);

			StreamWriter rw = new StreamWriter( fileName );
            
			for( int k = 0; k <= zIndexer; k++ )
			{
				for( int i = 0; i <= yIndexer; i++ )
				{
					rw.Write( Data3D[0,i,k].ToString("0.000") );
					for( int j = 1; j <= xIndexer; j++ )
					{
						rw.Write(',');
						rw.Write( Data3D[j,i,k].ToString("0.000") );
					}
					rw.WriteLine();
				}

				rw.WriteLine();
			}

			rw.Close();
		}

		/// <summary>
		/// older function needs to be integrated
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="Data3D"></param>
		/// <param name="zIndex"></param>
		private void WriteDataTable( string fileName, float[,,] Data3D, int zIndex )
		{
			int xIndexer = Data3D.GetUpperBound(0);
			int yIndexer = Data3D.GetUpperBound(1);
			int zIndexer = Data3D.GetUpperBound(2);
			if( zIndex > zIndexer )
			{
				throw new ArgumentException( "Zindex was larger than the Z-Bound of the array", "zIndex" );
			}

			StreamWriter rw = new StreamWriter( fileName );
            
			for( int i = 0; i <= yIndexer; i++ )
			{
				rw.Write( Data3D[0,i,zIndex].ToString("0.000") );
				for( int j = 1; j <= xIndexer; j++ )
				{
					rw.Write(',');
					rw.Write( Data3D[j,i,zIndex].ToString("0.000") );
				}
				rw.WriteLine();
			}

			rw.Close();
		}
	}
}
