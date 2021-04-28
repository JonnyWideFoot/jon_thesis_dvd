using System;
using System.IO;

using UoB.Methodology.PhiPsiAnalysis;

namespace AngleFitting
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Fitting
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{

#if DEBUG
			args = new string[] { "6", "A", "200", "0" };			
#endif

            // need to know ...
			// 0 = number of angles
			// 1 = Residue ID
			// 2 = maxfiles
			// 3 = mode

			int angleCount = -1;
			char resID = '~';
			int maxFiles = -1;
			int mode = -1;


			try
			{
				angleCount = int.Parse( args[0] );
			}
			catch
			{
				Console.WriteLine("Angle count incorrect");
				return;
			}
			try
			{
				resID = args[1][0];
			}
			catch
			{
				Console.WriteLine("resID incorrect");
				return;
			}
			try
			{
				maxFiles = int.Parse( args[2] );
			}
			catch
			{
				Console.WriteLine("maxFiles count incorrect");
				return;
			}
			try
			{
				mode = int.Parse( args[3] );
			}
			catch
			{
				Console.WriteLine("mode incorrect");
				return;
			}

			FileInfo startingFile = new FileInfo( System.Reflection.Assembly.GetEntryAssembly().Location );
			DirectoryInfo di = startingFile.Directory.Parent;
#if DEBUG
				di = new DirectoryInfo( @"C:\Main Backup 21.03.05\_My Calculations\09d - My Loop DataBase\PDBSelect2004\4. 1.9 or less" );
#endif
			
			PhiPsiData dataAnal = new PhiPsiData( "PDBSelect2004-1.9A", di );
			//dataAnal.GoAngleFitting( angleCount, resID, maxFiles, mode );
			dataAnal.ConvertFitsToAngleFile();		
		}
	}
}
