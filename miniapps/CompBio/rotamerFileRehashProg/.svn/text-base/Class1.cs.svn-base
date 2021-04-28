using System;
using System.IO;

namespace ConsoleApplication1
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			StreamWriter rw = new StreamWriter( "output.pdb", false );
			StreamReader re = new StreamReader( "rotamers.ff" );

			string lineBuffer;
			int resBeginMonitor = -1;
			string nameChangeMonitor = "";
			int atomCounterInit = 6;
			int atomCounter = atomCounterInit;

			bool proSet = false;


			while( (lineBuffer = re.ReadLine()) != null )
			{
                if ( lineBuffer.Length == 0 )
				{
					rw.WriteLine("");
					continue;
				}

				if( lineBuffer.Length < 80 )
				{
					lineBuffer = lineBuffer.PadRight(80,' ');
				}

				if( lineBuffer.Length > 80 )
				{
					rw.WriteLine(lineBuffer);
					continue; // assume that the line isnt an atom def
				}


				if( lineBuffer.Substring(0,11) == "GLOBALSTART" )
				{
					rw.WriteLine(lineBuffer);
					while( (lineBuffer = re.ReadLine()) != null )
					{
						if( lineBuffer.Substring(0,9) == "GLOBALEND" )
						{
							rw.WriteLine(lineBuffer);
							break;
						}
						rw.WriteLine(lineBuffer);
					}
					continue;
				}

				if( lineBuffer.Substring(0,4) == "NEXT" )
				{
                    rw.WriteLine( lineBuffer );
					if ( resBeginMonitor == 0 || resBeginMonitor == -1 )
					{
						rw.WriteLine( new string(' ',80) );
					}
					continue;
				}
                
				if ( lineBuffer[0] == '#' )
				{
					rw.WriteLine(lineBuffer);
					continue;
				}

				if( lineBuffer.Substring(0,4) == "ATOM" )
				{

					if ( lineBuffer.Substring(12,4) == " C  " ) continue;
					if ( lineBuffer.Substring(12,4) == " N  " ) continue;
					if ( lineBuffer.Substring(12,4) == " O  " ) continue;
					if ( lineBuffer.Substring(12,4) == " CA " ) continue;
					if ( lineBuffer.Substring(12,4) == " H  " ) continue;


                    int currentResNum = int.Parse( lineBuffer.Substring( 22, 4 ) );  
					string currentResName = lineBuffer.Substring(17,3);

					if ( resBeginMonitor != currentResNum || nameChangeMonitor != currentResName )
					{
						if ( resBeginMonitor != currentResNum )
						{
							rw.WriteLine(new string(' ',80) );
						}
						resBeginMonitor = currentResNum;
						nameChangeMonitor = currentResName;
						atomCounter = atomCounterInit;
					}
	
					int replaceAtomCount = atomCounter++;
					string atomCount = replaceAtomCount.ToString().PadLeft(4,' ');

					if ( lineBuffer.Substring(17,3) == "PRO" & !proSet )
					{
						rw.WriteLine( @"ATOM      5 ~H   PRO     0      NULL                  
ATOM      6 +HT1 PRO     0      NULL                  
ATOM      7 +HT2 PRO     0      NULL                  
ATOM      8 +HT3 PRO     0      NULL                  
ATOM      6 +HN1 PRO     0      -0.055  -0.592  -0.841
ATOM      7 +HN2 PRO     0      -0.055  -0.592   0.841" );
						proSet= true;
						atomCounter += 2;
					}

                    rw.WriteLine( lineBuffer.Substring(0,7) + atomCount + lineBuffer.Substring(11,lineBuffer.Length-10-1) );  
                  
					continue;
				}

				rw.WriteLine(lineBuffer); // we just had a random line
			}
				

			re.Close();
			rw.Close();

		}
	}
}
