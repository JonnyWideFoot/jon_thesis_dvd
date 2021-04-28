using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

using UoB.Core.Primitives;

namespace UoB.Core.FileIO.FormattedInput
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class FileScanTools
	{
		private FileScanTools()
		{

		}

		public static string[] readParseFile( string filePath ) // Parse.set - the config for info extraction
		{
			ArrayList lines = new ArrayList();
			StreamReader re = new StreamReader( filePath );
			string inputLine = null;
			while ((inputLine = re.ReadLine()) != null)
			{
				lines.Add( inputLine );
			}
			return (string[]) lines.ToArray(typeof(string));
		}

		private static void breakdownCommand( string linepart, out string part1, out string part2 )
		{
			if ( linepart[0] == '<' &&
				linepart[1] == '-' &&
				linepart[linepart.Length-2] == '-' &&
				linepart[linepart.Length-1] == '>'
				)
			{
				linepart = linepart.Substring( 2, linepart.Length - 4 );
				string[] lineParts = linepart.Split('~');
				part1 = lineParts[0];
				part2 = lineParts[1];
			}
			else
			{
				part1 = "";
				part2 = "";
			}									  
		}

		public static elementNamedFloat returnElementNamedFloat( ref string line, ref string[] commandList )
		{

			if ( commandList.Length < 2 )
			{
				Debug.WriteLine("ParseFileError, inputstring is incorrect");
				return new elementNamedFloat();
			}

			elementNamedFloat theENF = new elementNamedFloat();
			theENF.name = commandList[1];
       
			for ( int i = 2; i < commandList.Length; i++ )
			{
				string commandType;
				string commmandOrder;
				breakdownCommand( commandList[i], out commandType, out commmandOrder );

				int number;

				switch ( commandType.ToLower() )
				{
					case "ignore":
						number = int.Parse( commmandOrder );
						line = line.Remove(0, number);
						break;
					case "name":
						number = int.Parse( commmandOrder );
						theENF.name = line.Substring(0,number);
						line = line.Remove(0, number);
						break;
					case "float":
						number = int.Parse( commmandOrder );
						if ( line.Length-1 < number )
						{
							theENF.dataFloat = float.Parse( line );
							line = "";
						}
						else
						{
							theENF.dataFloat = float.Parse( line.Substring(0,number) );
							line = line.Remove(0, number);
						}

						break;
					default:
						Debug.WriteLine("Error in readstring, parse command not found");
						break;
				}
			}
			return theENF;
		}

		public static elementNamedVector returnElementNamedVector( ref string line, ref string[] commandList )
		{
			bool containsNameDirective = false;
			bool containsX = false;
			bool containsY = false;
			bool containsZ = false;

			elementNamedVector elemVector = new elementNamedVector();

			for ( int i = 2; i < commandList.Length; i++ )
			{
				if ( Regex.IsMatch( commandList[i], "<-name~" ) )
				{
					containsNameDirective = true;
					continue;
				}
				if ( Regex.IsMatch( commandList[i], "<-x~" ) )
				{
					containsX = true;
					continue;
				}
				if ( Regex.IsMatch( commandList[i], "<-y~" ) )
				{
					containsY = true;
					continue;
				}
				if ( Regex.IsMatch( commandList[i], "<-z~" ) )
				{
					containsZ = true;
					continue;
				}
			}

			elemVector.name = commandList[1];

			if( commandList.Length < 6 || !containsX || !containsY || !containsZ || !containsNameDirective )
			{
				Debug.WriteLine("ParseFileError, readnamedvector format is incorrect");
				Debug.WriteLine("Length = " + commandList.Length.ToString() );
				Debug.WriteLine("containsNameDirective = " + containsNameDirective.ToString() );
				Debug.WriteLine("containsX = " + containsX.ToString() );
				Debug.WriteLine("containsY = " + containsY.ToString() );
				Debug.WriteLine("containsZ = " + containsZ.ToString() );
				return new elementNamedVector();
			}

			elemVector.vector = new Vector(0,0,0);

			try
			{
				for ( int i = 2; i < commandList.Length; i++ )
				{
					string commandType;
					string valueOfCommand;
					breakdownCommand( commandList[i], out commandType, out valueOfCommand );
					int number = int.Parse( valueOfCommand );

					switch ( commandType.ToLower() )
					{
						case "ignore":
							line = line.Remove(0, number);
							break;
						case "name":
							elemVector.name = line.Substring(0,number);
							line = line.Remove(0, number);
							break;
						case "x":
							elemVector.vector.x = float.Parse( line.Substring(0,number) );
							line = line.Remove(0, number);
							break;
						case "y":
							elemVector.vector.y = float.Parse( line.Substring(0,number) );
							line = line.Remove(0, number);
							break;
						case "z":
							elemVector.vector.z = float.Parse( line.Substring(0,number) );
							line = line.Remove(0, number);
							break;
						default:
							Debug.WriteLine("Unknown parse command in readnamedfloats");
							break;
					}
										
				}
			}
			catch(Exception e)
			{
				Debug.WriteLine("Error prob due to normal end of parsing the vectors ...");
				Debug.WriteLine(e.ToString());
			}

			return elemVector;
		}
		public static elementNamedVectorArray returnElementNamedVectorArray( ref int position, ArrayList lines, ref string[] commandList )
		{
			bool containsNameDirective = false;
			bool containsX = false;
			bool containsY = false;
			bool containsZ = false;
			ArrayList foundVectors = new ArrayList();
			elementNamedVectorArray vectorArray = new elementNamedVectorArray();
			vectorArray.name = commandList[1];

			for ( int i = 2; i < commandList.Length; i++ )
			{
				if ( Regex.IsMatch( commandList[i], "<-name~" ) )
				{
					containsNameDirective = true;
					continue;
				}
				if ( Regex.IsMatch( commandList[i], "<-x~" ) )
				{
					containsX = true;
					continue;
				}
				if ( Regex.IsMatch( commandList[i], "<-y~" ) )
				{
					containsY = true;
					continue;
				}
				if ( Regex.IsMatch( commandList[i], "<-z~" ) )
				{
					containsZ = true;
					continue;
				}
			}

			if( commandList.Length < 6 || !containsX || !containsY || !containsZ || !containsNameDirective )
			{
				Debug.WriteLine("ParseFileError, readnamedvectors format is incorrect");
				Debug.WriteLine("Length = " + commandList.Length.ToString() );
				Debug.WriteLine("containsNameDirective = " + containsNameDirective.ToString() );
				Debug.WriteLine("containsX = " + containsX.ToString() );
				Debug.WriteLine("containsY = " + containsY.ToString() );
				Debug.WriteLine("containsZ = " + containsZ.ToString() );
				return new elementNamedVectorArray();
			}

			string line;

			while(true)
			{
				elementNamedVector elemVector = new elementNamedVector();
				elemVector.vector = new Vector(0,0,0);

				if( position >= lines.Count ) break;

				line = (string)lines[position++];
				if ( line != null && line.Length < 80 )
				{
					line = line.PadRight(80,' ');
				}

				try
				{
					for ( int i = 2; i < commandList.Length; i++ )
					{
						
						string commandType;
						string commmandOrder;
						breakdownCommand( commandList[i], out commandType, out commmandOrder );
						int number = int.Parse( commmandOrder );

						switch ( commandType.ToLower() )
						{
							case "ignore":
								line = line.Remove(0, number);
								break;
							case "name":
								elemVector.name = line.Substring(0,number);
								line = line.Remove(0, number);
								break;
							case "x":
								elemVector.vector.x = float.Parse( line.Substring(0,number) );
								line = line.Remove(0, number);
								break;
							case "y":
								elemVector.vector.y = float.Parse( line.Substring(0,number) );
								line = line.Remove(0, number);
								break;
							case "z":
								elemVector.vector.z = float.Parse( line.Substring(0,number) );
								line = line.Remove(0, number);
								break;
							default:
								Debug.WriteLine("Unknown parse command in readnamedfloats");
								break;
						}
										
					}
				}
				catch(Exception e)
				{
					//Debug.WriteLine("Error prob due to normal end of parsing the vectors ...");
					string error = e.ToString();
					break;
				}

				foundVectors.Add( elemVector );
			}
			vectorArray.namedVectors = (elementNamedVector[]) foundVectors.ToArray(typeof(elementNamedVector));
			return vectorArray;
		}

		public static elementString returnElementString( ref string line, ref string[] commandList )
		{
			if ( commandList.Length < 2 )
			{
				if( commandList[0] == "readline" )
				{
					elementString es = new elementString();
					es.name = commandList[1];
					es.dataString = line;
					return es;
				}
				throw new Exception("ParseFileError, inputstring is incorrect");
			}

			elementString theES = new elementString();
			theES.name = commandList[1];
       
			for ( int i = 2; i < commandList.Length; i++ )
			{
				string commandType;
				string commmandOrder;
				breakdownCommand( commandList[i], out commandType, out commmandOrder );

				int number;

				switch ( commandType.ToLower() )
				{
					case "ignore":
						number = int.Parse( commmandOrder );
						line = line.Remove(0, number);
						break;
					case "unknownstring":
						theES.dataString = Regex.Split( line , commmandOrder )[0];
						break;
					case "string":
						number = int.Parse( commmandOrder );
						theES.dataString = line.Substring(0,number);
						break;
					default:
						Debug.WriteLine("Error in readstring, parse command not found");
						break;
				}
			}
			return theES;
		}

		public static elementNamedFloatArray returnElementNamedFloatArray( ref int position, ArrayList lines, ref string[] commandList )
		{
			bool containsFloat = false;
			bool containsNameDirective = false;

			ArrayList foundFloats = new ArrayList();
			elementNamedFloatArray array = new elementNamedFloatArray();
			array.name = commandList[1];

			for ( int i = 2; i < commandList.Length; i++ )
			{
				if ( Regex.IsMatch( commandList[i], "<-name~" ) )
				{
					containsNameDirective = true;
					continue;
				}
				if ( Regex.IsMatch( commandList[i], "<-float~" ) )
				{
					containsFloat = true;
					continue;
				}
			}

			if( commandList.Length < 4 || !containsFloat || !containsNameDirective )
			{
				Debug.WriteLine("ParseFileError, readnamedfloats format is incorrect");
				Debug.WriteLine("Length = " + commandList.Length.ToString() );
				Debug.WriteLine("containsNameDirective = " + containsNameDirective.ToString() );
				Debug.WriteLine("ncontainsFloat = " + containsFloat.ToString() );
				return new elementNamedFloatArray();
			}

			string line;

			while(true)
			{
				elementNamedFloat elemFloat = new elementNamedFloat();

				line = (string)lines[position++];
				try
				{
					for ( int i = 2; i < commandList.Length; i++ )
					{
						string commandType;
						string commmandOrder;
						breakdownCommand( commandList[i], out commandType, out commmandOrder );
						int number = int.Parse( commmandOrder );

						switch ( commandType.ToLower() )
						{
							case "ignore":
								line = line.Remove(0, number);
								break;
							case "name":
								elemFloat.name = line.Substring(0,number);
								line = line.Remove(0, number);
								break;
							case "float":
								elemFloat.dataFloat = float.Parse( line.Substring(0,number) );
								line = line.Remove(0, number);
								break;
							default:
								Debug.WriteLine("Unknown parse command in readnamedfloats");
								break;
						}
							
					}
				}
				catch
				{
					break;
				}

				foundFloats.Add( elemFloat );
			}
			array.namedFloats = (elementNamedFloat[]) foundFloats.ToArray(typeof(elementNamedFloat));
			return array;
		}

		public static FileObject parseFile(string filename, string[] parseLines) // get the info from the file
		{
			ArrayList fileLineBuffer = new ArrayList();

			StreamReader re = new StreamReader(filename);
			string line;
			while ( ( line = re.ReadLine() ) != null )
			{
				fileLineBuffer.Add( line );
			}
			re.Close();

			FileObject theFileObject = new FileObject();
			theFileObject.assignName( filename );

			if ( !runCommands(theFileObject, fileLineBuffer, ref parseLines ) )
			{           
				throw new Exception("Reached end of commands, but no done statement...");
			}

			return theFileObject;
		}

		private static bool runCommands(FileObject theFileObject, ArrayList fileLineBuffer, ref string[] commandLines)
		{
			int lineCounter = 0;

			foreach ( string command in commandLines )
			{
				string line;
				string[] parts = command.Split(',');

				switch ( parts[0].ToLower() )
				{
					case "readuntillastpattern":

						if ( parts.Length != 2 )
						{
							Debug.WriteLine("ParseFileError, readuntilpattern is incorrect");
							return false;
						}
						int finalLineNumber = -1;
						while(true)
						{
                            if ( lineCounter >= fileLineBuffer.Count ) break; // we have reached the end
							line = (string)fileLineBuffer[lineCounter++];
							if( line.Length < parts[1].Length ) continue;
							string itemToMatch = line.Substring(0,parts[1].Length);
							string pattern = parts[1];
							if ( Regex.IsMatch(itemToMatch,pattern) )
							{
								finalLineNumber = lineCounter;
							}
						}
						if ( finalLineNumber == -1 ) return false; // we reached the end of the file without getting a match.
						lineCounter = finalLineNumber;
						break;
                    
					case "readuntilpattern":

						if ( parts.Length != 2 )
						{
							Debug.WriteLine("ParseFileError, readuntilpattern is incorrect");
							return false;
						}
						while(true)
						{
							line = (string)fileLineBuffer[lineCounter++];
							if( line.Length < parts[1].Length ) continue;
							string itemToMatch = line.Substring(0,parts[1].Length);
							string pattern = parts[1];
							if ( Regex.IsMatch(itemToMatch,pattern) )
							{
								break;
							}
						}
						break;

					case "readline":

						line = (string)fileLineBuffer[lineCounter++];
						if ( line.Length < 80 )
						{
							line = line.PadRight(80,' ');
						}
						theFileObject.addElement( returnElementString( ref line, ref parts ) );

						break;

					case "readstring":

						line = (string)fileLineBuffer[lineCounter++];
						if ( line.Length < 80 )
						{
							line = line.PadRight(80,' ');
						}
						theFileObject.addElement( returnElementString( ref line, ref parts ) );
						break;

					case "readnamedfloat":

						line = (string)fileLineBuffer[lineCounter++];
						if ( line.Length < 80 )
						{
							line = line.PadRight(80,' ');
						}
						theFileObject.addElement( returnElementNamedFloat( ref line, ref parts ) );
                        
						break;

					case "readnamedvector":

						line = (string)fileLineBuffer[lineCounter++];
						if ( line.Length < 80 )
						{
							line = line.PadRight(80,' ');
						}
						theFileObject.addElement( returnElementNamedVector( ref line, ref parts ) );
                        
						break;

					case "deadlines":

						try
						{
							lineCounter += int.Parse(parts[1]);
						}
						catch
						{
							Debug.WriteLine("Error reading the file or inputlinecommand deadlines is corrupt ...");
							return false;
						}
						break;

					case "readnamedvectors":

						theFileObject.addElement( returnElementNamedVectorArray( ref lineCounter, fileLineBuffer, ref parts ) );
						break;

					case "readnamedfloats":

						theFileObject.addElement( returnElementNamedFloatArray( ref lineCounter, fileLineBuffer, ref parts ) );
						break;

					case "done":

						return true;

					default:

						Debug.WriteLine("Error in parsefile - command not recognised in line - " + command);
						break;
				}
			}
			return false;
		}
	}
}
