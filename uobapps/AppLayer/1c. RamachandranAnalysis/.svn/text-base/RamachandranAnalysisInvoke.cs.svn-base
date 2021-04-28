using System;
using System.IO;
using System.Diagnostics;

using UoB.Core.Sequence;

using UoB.Methodology.DSSPAnalysis;
using UoB.Methodology.DSSPAnalysis.Ramachandran;
using UoB.Methodology.DSSPAnalysis.Angleset;
using UoB.Methodology.DSSPAnalysis.AngleSetAnalysis;

using UoB.AppLayer;

namespace UoB.AppLayer
{
	/// <summary>
	/// Summary description for AngleAnalysis.
	/// </summary>
	public class RamachandranAnalysisInvoke : AppLayerBase
	{
		public RamachandranAnalysisInvoke()
		{
		}

        public override string MethodPrintName
        {
            get { return "Common"; }
        }

		public override void MainStem( string[] args )
		{
			// Uses the classes in the UoB.Methodology.Ramachandran namespace ...

            //string dbName = "PICSES:1.7";

			// Most important ...
			// job 1: Full ramachandran plot analysis in UoB.Methodology
			// includes propensity and residue occurentce tables of information
            //RamachandranAnalysis analysis = new RamachandranAnalysis( dbName, TaskDir );
			//analysis.GoFullHTMLRamachandranReport();
			
			// job 2: 
            //PhiPsiPrinting analysis = new PhiPsiPrinting(dbName, TaskDir);
			//analysis.WriteAllFilesAllPhiPsis( StandardResidues.NotGlyOrPro );
			//analysis.WriteAllFilesLoopPhiPsis( DSSPIncludedRegions.OnlyDefinitelyGood, StandardResidues.NotGlyOrPro );
			//analysis.WriteAllFilesSecondaryPhiPsis( DSSPIncludedRegions.OnlyDefinitelyGood, StandardResidues.NotGlyOrPro );

            // Job 3:
            //AngleSetStats stats = new AngleSetStats(dbName, TaskDir);
            //stats.FurtherstDistanceStats();

            //AngleSetQualityReport rep = new AngleSetQualityReport(dbName, TaskDir);
            //rep.GoHTMLAngleFittingReport();           
		}

		private void OBSOLETECODE()
		{

			//			string stem = @"C:\Main Backup 21.03.05\_My Calculations\09d - My Loop DataBase\PDBSelect2004\DSSP\";
			//
			//			DSSPParser dp = new DSSPParser( new DirectoryInfo(stem) );
			//			DirectoryInfo outDir = new DirectoryInfo(stem + "\\out\\");
			//			string[] names = Enum.GetNames( typeof(StandardResidues) );
			//			StandardResidues[] resTypes = (StandardResidues[]) Enum.GetValues( typeof(StandardResidues) );

			// Report Output
			//			dp.WriteReportsForAllFiles( outDir, true );

			// ReportAllPhiPsi
			//			for( int i = 0; i < names.Length; i++ )
			//			{
			//				if( resTypes[i] == StandardResidues.BulkyAromatic )
			//				{
			//					//string filename = outDir.FullName + Path.DirectorySeparatorChar + "loopPhiPsi-DefGood-" + names[i] + ".csv";
			//					string filename = outDir.FullName + Path.DirectorySeparatorChar + "All_" + names[i] + ".csv";
			//					if( File.Exists(filename) ) File.Delete(filename);
			//					dp.WriteAllFilesAllPhiPsis( filename,resTypes[i] );
			//				}
			//			}

			// Loop Output
			//			for( int i = 0; i < names.Length; i++ )
			//			{
			//				if( resTypes[i] == StandardResidues.BulkyAromatic )
			//				{
			//					//string filename = outDir.FullName + Path.DirectorySeparatorChar + "loopPhiPsi-DefGood-" + names[i] + ".csv";
			//					string filename = outDir.FullName + Path.DirectorySeparatorChar + "Loop_" + names[i] + ".csv";
			//					if( File.Exists(filename) ) File.Delete(filename);
			//					dp.WriteAllFilesLoopPhiPsis( filename,PhiPsiReportMode.DefinitelyGood,resTypes[i] ); // the enum type
			//				}
			//			}

			// Loop PhiPsi 2D-Bin Analysis
			//			for( int i = 0; i < names.Length; i++ )
			//			{
			//				string filename = outDir.FullName + Path.DirectorySeparatorChar + "loopPhiPsi-2DBin-DefGood-" + names[i] + ".csv";
			//				if( File.Exists(filename) ) File.Delete(filename);
			//				dp.Write2DBinLoopPhiPsis( filename, 120, PhiPsiReportMode.DefinitelyGood,resTypes[i] ); // the enum type
			//			}

			// SASA Output
			//			for( int i = 0; i < names.Length; i++ )
			//			{
			//				string filename = outDir.FullName + Path.DirectorySeparatorChar + "SASA-DefGood-" + names[i] + ".csv";
			//				if( File.Exists(filename) ) File.Delete(filename);
			//				dp.WriteAllFilesSASA( filename, resTypes[i] ); // the enum type
			//			}

			// SASA Average Table
			//			dp.WriteSASASummary( outDir.FullName + Path.DirectorySeparatorChar + "SASA-Summary.csv" );
		}
	}
}
