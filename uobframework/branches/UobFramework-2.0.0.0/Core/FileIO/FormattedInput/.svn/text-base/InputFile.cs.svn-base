using System;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Collections;

namespace UoB.Core.FileIO.FormattedInput
{
	/// <summary>
	/// Summary description for InputFileCreation.
	/// </summary>
	public sealed class InputFile
	{
		private static Regex m_Regex = null;

		private InputFile()
		{
		}

		public static void RandomiseInputFileLineOrder( string fileIn, string fileOut )
		{
			StreamReader re = new StreamReader( fileIn );
			StreamWriter rw = new StreamWriter( fileOut );
			ArrayList lines = new ArrayList();
			Random r = new Random(6262);
			string line;
			while( null != ( line = re.ReadLine() ) )
			{
				lines.Add(line);
			}
			while( lines.Count > 0 )
			{
				int index = r.Next(lines.Count);
				rw.WriteLine( (string) lines[index] );
				lines.RemoveAt( index );
			}
			re.Close();
			rw.Close();
		}

		public static void Create( string templateFileName, string outputFileName, string[] keys, string[] values )
		{
			if( m_Regex == null )
			{
				m_Regex = new Regex( "<%[A-Za-z0-9]+%>", RegexOptions.None );
			}

			if( !File.Exists( templateFileName ) )
			{
				throw new InputException("Template file could not be found");
			}

			if( keys.Length != values.Length )
			{
				throw new InputException("The key and value definitions must contain the same number of entries");
			}

			Hashtable DefinedKeys = new Hashtable();
			ArrayList UsedKeys = new ArrayList();
			for( int i = 0; i < keys.Length; i++ )
			{
				if( DefinedKeys.ContainsKey( keys[i] ) )
				{
					throw new InputException( "Key : " + keys[i] + " was defined more than once" );
				}
				
				DefinedKeys.Add( keys[i].ToUpper(), values[i] );
			}

			StreamReader re = new StreamReader( templateFileName );
			StreamWriter rw = new StreamWriter( outputFileName, false );

			string inputLine;
			string outputLine;
			while( null != ( inputLine = re.ReadLine() ) )
			{
				outputLine = inputLine;
				MatchCollection matches = m_Regex.Matches( inputLine ); // anything in the form <%key%>
				for( int i = 0; i < matches.Count; i++ )
				{
					string key = matches[i].ToString().ToUpper();
					key = key.Substring(2,key.Length-4); // remove the <% %>
					if( DefinedKeys.ContainsKey( key ) )
					{
						string replacement = (string) DefinedKeys[key];
						if( !UsedKeys.Contains( key ) )
						{
							UsedKeys.Add( key ); // found one use of it
						}
						string buffer = 
							outputLine.Substring(0,matches[i].Index) + // before the token
							replacement + // the token, now work out what is left to add
							outputLine.Substring( matches[i].Index + matches[i].Length, outputLine.Length - matches[i].Length - matches[i].Index );
						outputLine = buffer; // the token has now been replaced with the value of the key
					}
					else
					{
						throw new InputException("The key : " + key + " was not defined for the Create constructor");
					}
				}
				rw.WriteLine(outputLine); // write the line following changes
			}

			re.Close();
			rw.Close();

			foreach( string enumerateKey in DefinedKeys.Keys )
			{
				if( !UsedKeys.Contains( enumerateKey ) )
				{
					throw new InputException( "Not all the defined keys were present in the template file" );
				}
			}
		}
	}
}
