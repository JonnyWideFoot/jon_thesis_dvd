using System;
using System.IO;

namespace UoB.Core.FileIO.Raft
{
	/// <summary>
	/// Summary description for RaftRstFile.
	/// </summary>
	public class RaftEmcFile
	{
		private RaftEmcFile()
		{
		}

		public static void Write( string fileName, EmcFillParams emcParam )
		{
			StreamWriter rw = new StreamWriter( fileName );

			//HEADER block
			rw.WriteLine("%EMC FILE");
			rw.WriteLine("%VERSION  1.0");
			rw.WriteLine("#        number of   select by        size next    output N    output N   mut    param1       param2       param3");
			rw.WriteLine("# seed     parents    energy    flag  generatn     conf desc   conf cord  meth  "); 
			rw.WriteLine("#.......   .......   ..........   ..   .........   .........   .........   ..   ..........   ..........   ..........");
			//end HEADER block

			// next write format
			//"10"
			//"45789245     50000    -10000.0     1     3000000         100          10    2         50.0     2.00          3.00"
						
			// write the number of generations to be read in by RAFT
			rw.WriteLine( emcParam.genCount.ToString() );

			for( int i = 0; i < emcParam.genCount - 1; i++ )
			{
				WriteEMCLine( rw, emcParam.randomNumber, emcParam.genStart, emcParam.parentPass, emcParam.midConfCount, emcParam.midCoordCount, emcParam.mutationRate );
			}
			WriteEMCLine( rw, emcParam.randomNumber, emcParam.genStart, emcParam.parentPass, emcParam.lastConfCount, emcParam.lastCoordCount, emcParam.mutationRate );

			rw.Close();
		}

		public static void WriteEMCLine( StreamWriter rw, int randomSeed, int genStart, int parentPass, int ConfCount, int CoordCount, float mutationRate )
		{
			rw.Write( randomSeed.ToString().PadLeft(8,'0') );
			rw.Write( parentPass.ToString().PadLeft(10,' ') );
			rw.Write( "    -10000.0     1" );
			rw.Write( genStart.ToString().PadLeft(12,' ') );
			rw.Write( ConfCount.ToString().PadLeft(12,' ') );
			rw.Write( CoordCount.ToString().PadLeft(12,' ') );
			rw.Write( "    2 " ); // mut method - we want 2 ...
			rw.Write( mutationRate.ToString("0.0").PadLeft(12,' ') );
			rw.WriteLine( "     2.00          3.00" );
		}
	}
}
