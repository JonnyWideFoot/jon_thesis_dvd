#define CONSOLEREPORT

using System;
using System.IO;
using System.Collections;

using UoB.Core;
using UoB.Core.Sequence;
using UoB.Core.MoveSets.AngleSets;
using UoB.Methodology.TaskManagement;
using UoB.Core.FileIO.PDB;
using UoB.Core.Tools;

namespace UoB.Methodology.DSSPAnalysis.AngleFitting
{
	/// <summary>
	/// Summary description for EngineOutputToAnglesetFile.
	/// </summary>
	public class AngleFittingEngineOutputConverter : DSSPTaskDirecory_PhiPsiData
	{
		// Temporaty Angle Set holders ... filled by a call to ObtainAngleFitDataSet()
		protected double[] m_FittedPhis = null;
        protected double[] m_FittedPsis = null;
        protected double[] m_Omegas = null;
        protected float[] m_Propensities = null;
        protected RamachandranBound[] m_AngleBoundClasses = null;

		// IO Streams
		protected StreamReader m_AngleStream = null; // used to read each of the .CSV angle output files

		public AngleFittingEngineOutputConverter( string DSSPDatabaseName, DirectoryInfo di ) : base( DSSPDatabaseName, di, false )
		{
		}

        protected AngleFittingEngineOutputConverter(string DSSPDatabaseName, DirectoryInfo di, bool origin)
            : base(DSSPDatabaseName, di, origin)
        {
        }

		/// <summary>
		/// Converts the output ".csv" files in the result directory to a single ".angleset" file
		/// </summary>
		/// <param name="outputAngleSetName">The name given to the .angleset output file</param>
		public void ConvertFiles( DSSPReportingOn expectedStructureTypes, int wantedAngleCount, int wantedMode )
		{
			string outNameStem = wantedAngleCount.ToString() + '_' + wantedMode.ToString();

			// e.g: 10_A_0_-1.csv // -1 signals that all files were used
			string filter = wantedAngleCount.ToString();
			filter += "_*_";
			filter += wantedMode.ToString();
			filter += "_-1.csv";
			FileInfo[] files = resultDirectory.GetFiles(filter);

			#if CONSOLEREPORT
				Console.Write("Starting stem: ");
				Console.WriteLine( outNameStem );
				Console.Write("Using file filter: ");
				Console.Write( filter );
				Console.Write(' ');
				Console.Write(files.Length);
				Console.WriteLine(" files found.");
			#endif

			StreamWriter loopAngleSet = null, allAngleSet = null;
			if( expectedStructureTypes == DSSPReportingOn.All )
			{
				loopAngleSet = new StreamWriter( reportDirectory.FullName + Path.DirectorySeparatorChar + outNameStem + "_loop.angleset" );
				allAngleSet =  new StreamWriter( reportDirectory.FullName + Path.DirectorySeparatorChar + outNameStem + "_all.angleset" );

				PrintAngleSetHeader( loopAngleSet, "AngleCount:" + wantedAngleCount.ToString() + " FittingMode:" + wantedMode.ToString() + " LoopResidues", files.Length );
				PrintAngleSetHeader(  allAngleSet, "AngleCount:" + wantedAngleCount.ToString() + " FittingMode:" + wantedMode.ToString() + " AllResidues",   files.Length );
			}
			else if( expectedStructureTypes == DSSPReportingOn.LoopsOnly )
			{
				loopAngleSet = new StreamWriter( reportDirectory.FullName + Path.DirectorySeparatorChar + outNameStem + "_loop.angleset" );
				PrintAngleSetHeader( loopAngleSet, "AngleCount:" + wantedAngleCount.ToString() + " FittingMode:" + wantedMode.ToString() + " LoopResidues", files.Length );
			}
			else if( expectedStructureTypes == DSSPReportingOn.SecondaryOnly )
			{
				allAngleSet =  new StreamWriter( reportDirectory.FullName + Path.DirectorySeparatorChar + outNameStem + "_all.angleset" );
				PrintAngleSetHeader(  allAngleSet, "AngleCount:" + wantedAngleCount.ToString() + " FittingMode:" + wantedMode.ToString() + " AllResidues",   files.Length );
			}
			else
			{
				throw new NotImplementedException();
			}

			for( int i = 0; i < files.Length; i++ )
			{
				m_AngleStream = new StreamReader(files[i].FullName);

				string[] nameParts = files[i].Name.Split('_');
				int angleCount = int.Parse( nameParts[0] );
				string resID = nameParts[1][0].ToString();
				if( nameParts[1][1] == 'C' ) // cis
				{
					resID = resID.ToLower();
				}
				else if( nameParts[1][1] == 'T' ) // trans
				{
					resID = resID.ToUpper();					
				}
				else
				{
					throw new Exception("The filename contained an incorrect omega descriptor");
				}

				#if CONSOLEREPORT
					Console.Write("\tDoing file: ");
					Console.WriteLine( files[i].Name );
				#endif

				StandardResidues standardResID = (StandardResidues)Enum.Parse( typeof(StandardResidues), resID );
				
				if( expectedStructureTypes == DSSPReportingOn.All || 
					expectedStructureTypes == DSSPReportingOn.SecondaryOnly )
				{

					// 1. get the Phi Psi data from the DSSP files.
					#if CONSOLEREPORT
						Console.WriteLine("\t\tGetting PhiPsi data: ALL");
					#endif
					base.ObtainPhiPsiData( DSSPReportingOn.All, DSSPIncludedRegions.OnlyDefinitelyGood, standardResID );
					// 2. get the fitted angles from the .csv output and calculate an angleset file, including propensity information gathered from the 
					// phi/psi data taken in the above call in stage 1.
					#if CONSOLEREPORT
						Console.WriteLine("\t\tGetting anglefit data: ALL");
					#endif
					ObtainAngleFitDataSet( angleCount, DSSPReportingOn.All, standardResID, true );
					// 3. print the angleset
					#if CONSOLEREPORT
						Console.WriteLine("\t\tPrinting Result: ALL");
					#endif
					PrintToAngleSet( allAngleSet, resID[0], angleCount );
					allAngleSet.Flush(); // print as we go ...
				}

				if( expectedStructureTypes == DSSPReportingOn.All || 
					expectedStructureTypes == DSSPReportingOn.LoopsOnly )
				{
					// repreat for loop phi/psi data and loop angle fitting results
					#if CONSOLEREPORT
						Console.WriteLine("\t\tGetting PhiPsi data: Loop");
					#endif
					base.ObtainPhiPsiData( DSSPReportingOn.LoopsOnly, DSSPIncludedRegions.OnlyDefinitelyGood, standardResID );
					#if CONSOLEREPORT
						Console.WriteLine("\t\tGetting anglefit data: Loop");
					#endif
					ObtainAngleFitDataSet( angleCount, DSSPReportingOn.LoopsOnly, standardResID, true );
					#if CONSOLEREPORT
						Console.WriteLine("\t\tPrinting Result: Loop");
					#endif
					PrintToAngleSet( loopAngleSet, resID[0], angleCount );
					loopAngleSet.Flush();

					m_AngleStream.Close();
					m_AngleStream = null;
				}
			}

			if( expectedStructureTypes == DSSPReportingOn.All )
			{
				loopAngleSet.Close();
				allAngleSet.Close();
			}
			else if( expectedStructureTypes == DSSPReportingOn.LoopsOnly )
			{
				loopAngleSet.Close();
			}
			else if( expectedStructureTypes == DSSPReportingOn.SecondaryOnly )
			{
				allAngleSet.Close();
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		private void PrintAngleSetHeader( StreamWriter rw, string name, int resCount )
		{
			//  e.g.
			//	%ANGLE_SET
			//	%VERSION 1.0  
			//	%DESCRIPTOR Original RAFT angleset
			//	%RESIDUE_COUNT 20
			rw.WriteLine("%ANGLE_SET");
			rw.WriteLine("%VERSION 1.0");
			rw.Write("%DESCRIPTOR ");
			rw.WriteLine(name);
			rw.Write("%RESIDUE_COUNT ");
			rw.WriteLine(resCount);
		}

		private void PrintToAngleSet( StreamWriter angleSet, char resID, int angleCount )
		{
			//A	Ala	6
			angleSet.Write( resID );
			angleSet.Write( '\t' );
			angleSet.Write( PDB.PDBSingleLetterToThreeLetter( resID ) );
			angleSet.Write( '\t' );
			angleSet.WriteLine( angleCount );

			//-140	153	180	0.135	B
			for( int j = 0; j < angleCount; j++ )
			{
				angleSet.Write( m_FittedPhis[j].ToString("0.00") );
				angleSet.Write( '\t' );
				angleSet.Write( m_FittedPsis[j].ToString("0.00") );
				angleSet.Write( '\t' );
				angleSet.Write( m_Omegas[j].ToString("0.00") );
				angleSet.Write( '\t' );
				angleSet.Write( m_Propensities[j].ToString("0.0000") );
				angleSet.Write( '\t' );
				angleSet.WriteLine( RamachandranRegions.GetBoundSymbol(m_AngleBoundClasses[j]) );
			}
		}

		
		/// <summary>
		/// All parameters are soley to validate the data set is what it says it is ..
		/// </summary>
		/// <param name="angleCount"></param>
		/// <param name="reportOn"></param>
		/// <param name="resTypes"></param>
		/// <param name="sortByOriginalAngles"></param>
		protected void ObtainAngleFitDataSet( int angleCount, DSSPReportingOn reportOn, StandardResidues resTypes, bool sortByOriginalAngles )
		{
			// .CSV file example lines .... we need to parse this
			//A,All,-59.7576567496909,-37.6611372989888,-88.1828920450911,-8.6936824166653,-112.28498423625,70.8449308838905,-112.756175632941,136.14968227183,-67.7173275815868,144.136521036335,-151.557390939704,155.98312733601,35398579.1931107
			//A,All,58.8458750945728,34.2631570823784,-62.9373684581077,-38.9054855983266,-91.1114528035333,-4.23656988248074,-110.474895035603,107.564379131219,-71.6426392270451,145.361875241326,-144.155290880778,153.465681521439,22077585.2361915
			//A,All,58.8458750945728,34.1512517370987,-62.9448556769383,-38.9407732980052,-91.0792575478923,-4.24477108812188,-110.409246687968,108.015395554721,-71.6018766772942,145.418524496919,-144.262954903423,153.529480421697,22077451.758107
			//A,All,58.8458750945728,33.8457442735534,-91.1531821811354,-4.25954863953407,-62.9444127324709,-38.9301471560868,-110.416747795612,108.334281391154,-71.5939428348066,145.438176093828,-144.383202206521,153.561962296983,22077386.5032598
			//Time taken : ALL_A : 03:32:24.3125000

			string line = null;
			string prevLine = null;
			do
			{
				prevLine = line;
				line = m_AngleStream.ReadLine();
				if( String.Compare( line, 0, "Time taken : ", 0, 13 ) == 0 )
				{
					// we have an end string
					FillInternalDataFromLine( prevLine, angleCount, reportOn, resTypes );
					line = m_AngleStream.ReadLine(); // should be a blanking line ...
					if( line != null && line.Length != 0 )
					{
						throw new Exception("Parseline was expected to be blank.");
					}
					break; // all is well, we have found our data
				}
			}
			while( true );
		}

		private void FillInternalDataFromLine( string line, int angleCount, DSSPReportingOn doReportOn, StandardResidues resTypes )
		{
			string[] lineParts = line.Split(',');

			// begin validation procedures
			if( lineParts.Length != ((angleCount*2)+3) )
			{
				throw new Exception("Parseline did not contain the correct number of entries");
			}
			if( (lineParts[0].Length != 1) || (lineParts[0][0] != resTypes.ToString()[0]) )
			{
				throw new Exception("Parseline did not contain the correct molID");
			}
			if( lineParts[1] != doReportOn.ToString() )
			{
				throw new Exception("Parseline did not contain the correct repotring mode string");
			}
			// end validation, parse the data

			m_FittedPhis = new double[ angleCount ];
			for( int i = 0; i < angleCount; i++ )
			{
				m_FittedPhis[i] = double.Parse( lineParts[ (i*2) + 2 ] );
			}

			m_FittedPsis = new double[ angleCount ];
			for( int i = 0; i < angleCount; i++ )
			{
				m_FittedPsis[i] = double.Parse( lineParts[ (i*2) + 3 ] );
			}

			m_AngleBoundClasses = new RamachandranBound[ angleCount ];
			for( int i = 0; i < angleCount; i++ )
			{
				m_AngleBoundClasses[i] = RamachandranRegions.GetAngleClass(resTypes,m_FittedPhis[i],m_FittedPsis[i]);
			}

			// sort the current angle set by angleclass
			QuickSort( m_AngleBoundClasses, m_FittedPhis, m_FittedPsis );

			m_Propensities = new float[ angleCount ];
			for( int i = 0; i < phiData.Length; i++ )
			{
                m_Propensities[AngleSet.ClosestIDTo(m_FittedPhis, m_FittedPsis, phiData[i], psiData[i])] += 1.0f;
			}

			// make grid into fractions of the total
			for( int i = 0; i < angleCount; i++ )
			{
				m_Propensities[i] = m_Propensities[i] / (float)phiData.Length;
			}

			m_Omegas = new double[ angleCount ];
			if( char.IsUpper( resTypes.ToString(), 0 ) )
			{
				for( int i = 0; i < angleCount; i++ )
				{
					m_Omegas[i] = 180.0f;
				}
			}		
			else
			{
				// do nothing, they are initiated to 0.0f anyway
				//m_Omegas[i] = 0.0f;
			}

			return;
		}

		private static void QuickSort( RamachandranBound[] angleClasses, params double[][] otherArrays )
		{
			for( int i = 0; i < otherArrays.Length; i++ )
			{
				if( angleClasses.Length != otherArrays[i].Length )
				{
					throw new ArgumentException("Arrays must all be the same length");
				}
			}
			double[] pivotCache = new double[otherArrays.Length];
			q_sort( angleClasses, pivotCache, otherArrays, 0, angleClasses.Length - 1 );
		}

		private static void q_sort( RamachandranBound[] angleClasses, double[] pivotCache, double[][] otherArrays, int left, int right )
		{
			int l_hold = left;
			int r_hold = right;
			RamachandranBound pivot = angleClasses[left];

			for( int i = 0; i < otherArrays.Length; i++ )
			{
				pivotCache[i] = otherArrays[i][left];
			}

			while (left < right)
			{
				while (((float)angleClasses[right] >= (float)pivot) && (left < right))
					right--;
				if (left != right)
				{
					angleClasses[left] = angleClasses[right];
					for( int i = 0; i < otherArrays.Length; i++ )
					{
						otherArrays[i][left] = otherArrays[i][right];
					}
					left++;
				}
				while (((float)angleClasses[left] <= (float)pivot) && (left < right))
					left++;
				if (left != right)
				{
					angleClasses[right] = angleClasses[left];
					for( int i = 0; i < otherArrays.Length; i++ )
					{
						otherArrays[i][right] = otherArrays[i][left];
					}
					right--;
				}
			}
			angleClasses[left] = pivot;
			for( int i = 0; i < otherArrays.Length; i++ )
			{
				otherArrays[i][left] = pivotCache[i];
			}
            int newPivot = left;
			left = l_hold;
			right = r_hold;
			if (left < newPivot)
				q_sort(angleClasses, pivotCache, otherArrays, left, newPivot-1);
			if (right > newPivot)
				q_sort(angleClasses, pivotCache, otherArrays, newPivot+1, right);
		}
	}
}
