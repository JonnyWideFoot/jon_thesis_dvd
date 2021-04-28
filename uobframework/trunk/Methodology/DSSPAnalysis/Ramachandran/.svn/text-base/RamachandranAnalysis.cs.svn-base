using System;
using System.IO;
using System.Collections;
using System.Text;

using UoB.Core.MoveSets.AngleSets;
using UoB.Core.FileIO.DSSP;
using UoB.Core.Sequence;
using UoB.Core.FileIO.Raft;

using UoB.Methodology.TaskManagement;
using UoB.Methodology.DSSPAnalysis;
using UoB.Methodology.OriginInteraction;

namespace UoB.Methodology.DSSPAnalysis.Ramachandran
{
	/// <summary>
	/// Summary description for PhiPsiData.
	/// </summary>
	public sealed class RamachandranAnalysis : DSSPTaskDirecory_PhiPsiData
	{
		private ResidueCountTable m_ResidueCountTable = null;
		private AnglesetPropensityTable m_AnglePropensityTable = null;

		// get the current angle sets
		private AngleSet m_AngleSet = null; // we are going to load the default 6 angle set ...
		private double[] raftPhi = null; // this will hold the current angle sets phi angles
		private double[] raftPsi = null; // ditto for psi angles

		// 2D phi psi angle bins
		private int[,] m_Matrix180 = null; // three different resolutuions of plot are available 
		private int[,] m_Matrix110 = null; // for use depending on data density. All are filled 1st.
		private int[,] m_Matrix72 = null;

		public RamachandranAnalysis( string DBName, DirectoryInfo di ) : base( DBName, di, true )
		{		
			m_AngleSet = new AngleSet(); // load the default angle set to display on the relevent graphs

			// initialise the required matrices
			m_Matrix180 = new int[180,180];
			m_Matrix110 = new int[110,110];
			m_Matrix72 = new int[72,72];
		}
	
		#region ramachandran report
		public void GoFullHTMLRamachandranReport()
		{
			StandardResidues[] multiResTypes  = StandardSeqTools.GetStandardResidueGroups();
			StandardResidues[] singleResTypes = StandardSeqTools.GetIndividualStandardResidues();

			//HTMLReportingBegin( "PDBSelect2004-1.9A" );

			string HTMLTitle = 	"PhiPsi Distribution Report</h1><h2>Using the " + DBName + "database</h2><h1>";

			HTMLReportingBegin( HTMLTitle );

			HTMLStartReportBlock( "Report for: All Residue Types" );
				// get the residue count for "All"
				int allCountAll = DoRamachandranOrigin( -1, DSSPReportingOn.All, DSSPIncludedRegions.OnlyDefinitelyGood, StandardResidues.All );
				int allCountLoop = DoRamachandranOrigin( -1, DSSPReportingOn.LoopsOnly, DSSPIncludedRegions.OnlyDefinitelyGood, StandardResidues.All );
				m_ResidueCountTable = new ResidueCountTable( DelimiterMode.StandardHTMLTable, allCountAll, allCountLoop ); // used in % calculations
			HTMLEndReportBlock();

			HTMLReportingDivider();

			// now to the multi types with no stats
			for( int i = 0; i < multiResTypes.Length; i++ )
			{
				// you cant have stats for these, there are no "standard" raft angles
				HTMLStartReportBlock( "Report for: " + multiResTypes[i].ToString() );
					int multiCountAll = DoRamachandranOrigin( allCountAll, DSSPReportingOn.All, DSSPIncludedRegions.OnlyDefinitelyGood, multiResTypes[i] );
					int multiCountLoop = DoRamachandranOrigin( allCountLoop, DSSPReportingOn.LoopsOnly, DSSPIncludedRegions.OnlyDefinitelyGood, multiResTypes[i] );
					m_ResidueCountTable.AddCountRecord( multiResTypes[i].ToString(), multiCountAll, multiCountLoop );
				HTMLEndReportBlock();

				if( i != multiResTypes.Length-1 )
				{
					HTMLReportingDivider();
				}
			}

			// now to the single types with propensity stats
			m_AnglePropensityTable = new AnglesetPropensityTable( DelimiterMode.StandardHTMLTable, m_AngleSet );
            for (int i = 0; i < singleResTypes.Length; i++)
            {
                char molTypeID = singleResTypes[i].ToString()[0];

                HTMLStartReportBlock("Report for: " + singleResTypes[i].ToString());

                m_AnglePropensityTable.PropensityReset(molTypeID);
                int singleCountAll = DoRamachandranOrigin(allCountAll, DSSPReportingOn.All, DSSPIncludedRegions.OnlyDefinitelyGood, singleResTypes[i]);
                m_AnglePropensityTable.PropensityObtainAllResidues(phiData, psiData); // get stats for All structure types
                int singleCountLoop = DoRamachandranOrigin(allCountLoop, DSSPReportingOn.LoopsOnly, DSSPIncludedRegions.OnlyDefinitelyGood, singleResTypes[i]);
                m_AnglePropensityTable.PropensityObtainLoopResidues(phiData, psiData);// phi/psi data has been updated, redo stats for loops
                m_AnglePropensityTable.BuildCurrentPropensityLine();

                m_ResidueCountTable.AddCountRecord(singleResTypes[i].ToString(), singleCountAll, singleCountLoop);

                m_HTMLReporter.Write("<tr><td colspan=3><h2>");
                m_HTMLReporter.Write("Stats for: ");
                m_HTMLReporter.Write(molTypeID);
                m_HTMLReporter.WriteLine("</h2></td></tr>");

                // start mini individual stat table
                m_HTMLReporter.Write("<tr><td colspan=3>");
                m_AnglePropensityTable.PrintTable(m_HTMLReporter);
                m_HTMLReporter.WriteLine("</td></tr>");

                HTMLEndReportBlock();

                if (i != singleResTypes.Length - 1)
                {
                    HTMLReportingDivider();
                }

                m_HTMLReporter.Flush(); // make sure out output is written per-step
            }

			HTMLStartReportBlock( "Residue Counts For the Database :" );
				m_HTMLReporter.Write("<tr><td colspan=3>");
				m_ResidueCountTable.PrintTable( m_HTMLReporter );
				m_HTMLReporter.Write("</td></tr>");
			HTMLEndReportBlock();

			// RESIDUE COUNTING INFO HERE ...

			HTMLReportingEnd();		
		}

		/// <summary>
		/// </summary>
		/// <param name="totalResiduesInAllDatabase">Used for reporting the percentage of the current StandardResidues type compared to that of the total in the database</param>
		/// <param name="doReportOn"></param>
		/// <param name="mode"></param>
		/// <param name="resTypes"></param>
		/// <returns></returns>
		private int DoRamachandranOrigin( int totalResiduesInAllDatabase, DSSPReportingOn doReportOn, DSSPIncludedRegions mode, StandardResidues resTypes )
		{
			// Main call to get all the Data from the .DSSP files present in the m_DI directory
			int occurrenceCount = ObtainData( doReportOn, mode, resTypes );
			string name = resTypes.ToString();
            char cisTrans = 'T';
            string cisTransS = "Trans";
            if (name.Length == 1)
            {
                if (Char.IsLower(name[0]))
                {
                    cisTrans = 'C';
                    cisTransS = "Cis";
                }
            }

			if( doReportOn == DSSPReportingOn.All )
			{
				// make a filename based on the above params (**ignore the DSSPIncludedRegions for now )
				m_OutputFileStem = name + cisTrans + "_All";
                m_GraphTitle = "Residue type : \"" + name + ' ' + cisTransS + "\" in all structures\r\n";
			}
			else if( doReportOn == DSSPReportingOn.LoopsOnly )
			{
				// make a filename based on the above params (**ignore the DSSPIncludedRegions for now )
				m_OutputFileStem = name + cisTrans + "_Loop";
                m_GraphTitle = "Residue type : \"" + name + ' ' + cisTransS + "\" in loop regions only\r\n";
			}
			else
			{
				throw new Exception("unknown DSSPReportingOn type");
			}

			string occurenceString = occurrenceCount.ToString("N"); // N means number format, commas and all
			occurenceString = occurenceString.Substring(0,occurenceString.Length-3);
			if( totalResiduesInAllDatabase != -1 ) // this is therefore for "ALL" types, we need to find the total !
			{
				float percentage = ((float)occurrenceCount / (float)totalResiduesInAllDatabase) * 100.0f;
				m_GraphTitle += occurenceString + " Occurrences (" + percentage.ToString("0.00") + "%)";
			}
			else
			{
				m_GraphTitle += occurenceString + " Occurrences";
			}
			
			PhiPsiUpdateOriginData(); // update the contents of all worksheets and matrices from the new internal data	

			// get some distribution stats and decide which resolution of graph to use
			string useGraph = null;
			int[,] useMatrix = null;
			double secondHighest;

			float decideResolutionCount = occurrenceCount;

			if( decideResolutionCount > 110000 )
			{
				useGraph = "ContourHiRes";
				useMatrix = m_Matrix180;
				GetContourCullHighValue( useMatrix, out secondHighest );
			}
			else if( decideResolutionCount > 17000 )
			{
				useGraph = "ContourLowRes";
				useMatrix = m_Matrix110;
				GetContourCullHighValue( useMatrix, out secondHighest );
			}
			else
			{
				useGraph = "ContourSuperLowRes";
				useMatrix = m_Matrix72;
				GetContourCullHighValue( useMatrix, out secondHighest );
			}

            // Update Titles
            //PhiPsiUpdateOriginm_GraphTitles();

            // Update Ranges
            PhiPsiUpdateOriginGraphRanges(useGraph, useMatrix, secondHighest * 2.5);

			HTMLPhiPsiReportPictures( doReportOn, new string[] { "Contour3D", useGraph, "Scatter" } );						

			return occurrenceCount; // report what we found back to the caller
		}

		private int ObtainData( DSSPReportingOn doReportOn, DSSPIncludedRegions mode, StandardResidues resTypes )
		{
			// fill the raftPhi and raftPsi arrays
			string name = Enum.GetName( typeof( StandardResidues ), resTypes ); 
			if( name == null || name.Length != 1 ) // 1 means its a single, multiples are groups
			{
				// we have multiple types, or the name is not present in our list
				// ignore and dont plot later
				raftPhi = new double[0];
				raftPsi = new double[0];
			}
			else
			{
				char id = name[0];
				raftPhi = m_AngleSet.GetPhis(id); 
				raftPsi = m_AngleSet.GetPsis(id); 
			}

			// make the call to the base class
			int obtainedCount = ObtainPhiPsiData( doReportOn, mode, resTypes );
			
			// now use this data to fill the Matrices
			ResetMatrices(); // set all counts to 0's
			SetupMatrices(); // reapply the counts	

			return obtainedCount;
		}

		#endregion

		#region Ramachandran HTML report generation

        private void saveFigureFile( string filename, string label)
        {
            StreamWriter rw = new StreamWriter(filename);
            rw.WriteLine(label);
            rw.Close();
        }

		private void HTMLPhiPsiReportPictures( DSSPReportingOn DSSPReportingOn, string[] pageNames )
		{
            ImageType iType = ImageType.PNG;
            string ext = "." + iType.ToString();

			// print title row
			m_HTMLReporter.Write("<tr><td colspan=3><h2>");
			m_HTMLReporter.Write( m_OutputFileStem + ':' + DSSPReportingOn.ToString() ); 
			m_HTMLReporter.WriteLine("</h2></td></tr>");
		
			m_HTMLReporter.WriteLine("<tr>");
			for( int i = 0; i < pageNames.Length; i++ )
			{
				m_HTMLReporter.WriteLine("<td>");
				string thumbName = m_OutputFileStem + "_T_" + pageNames[i];
				if( "Scatter" == pageNames[i] )
				{
					InteractOrigin.SetDataPointPlotSize("Scatter","PhiPsi_b",3);
				}
                string saveStem = reportDirectory.FullName + thumbName;
                saveFigureFile(saveStem + ".figure", pageNames[i] + ":\r\n" + m_GraphTitle);
				InteractOrigin.SavePicture( iType, saveStem, pageNames[i], 350, 350 );

				string imgName = m_OutputFileStem + "_B_" + pageNames[i];
				if( "Scatter" == pageNames[i] )
				{
					InteractOrigin.SetDataPointPlotSize("Scatter","PhiPsi_b",1);
				}
                InteractOrigin.SavePicture(iType, reportDirectory.FullName + imgName, pageNames[i], 1000, 1000);

                InteractOrigin.SaveEPSPicture(reportDirectory.FullName + imgName + ".eps", pageNames[i]);
                InteractOrigin.Save(reportDirectory.FullName + imgName );

				m_HTMLReporter.Write("<a href=\"");
				m_HTMLReporter.Write(imgName + ext); // relative path
				m_HTMLReporter.Write("\"><img border=0 src=\"");
				m_HTMLReporter.Write(thumbName + ext);
				m_HTMLReporter.WriteLine("\"></a>");
				m_HTMLReporter.WriteLine("</td>");
			}
			m_HTMLReporter.WriteLine("</tr>");		

			m_HTMLReporter.Flush();
		}
		
		#endregion

		#region Origintalk: to Ramachandran Plots in Template file

		private void PhiPsiUpdateOriginData()
		{
			InteractOrigin.Reset(true,true);
			InteractOrigin.UpdateWorksheet( "PhiPsi", phiData, psiData );	
			InteractOrigin.UpdateWorksheet( "Raft", raftPhi, raftPsi ); 
			InteractOrigin.UpdateMatrix( "Matrix180", m_Matrix180 );
			InteractOrigin.UpdateMatrix( "Matrix110", m_Matrix110 );
			InteractOrigin.UpdateMatrix(  "Matrix72",  m_Matrix72 );
		}

		private void PhiPsiUpdateOriginGraphRanges( string contourName, int[,] matrix, double maxContourValue )
		{
			InteractOrigin.SetZBound( "Contour3D", 0.0f, (float)GetMaxValue( m_Matrix72 ) );
			InteractOrigin.RunContourRangeScript( contourName, maxContourValue );
		}

		private void PhiPsiUpdateOriginm_GraphTitles()
		{
			// set title labels
			InteractOrigin.ChangeLabel("ContourSuperLowRes", "Text",  "Ramachandran Contour Plot\r\n" + m_GraphTitle);
			InteractOrigin.ChangeLabel("ContourLowRes", "Text",  "Ramachandran Contour Plot\r\n" + m_GraphTitle);
			InteractOrigin.ChangeLabel("Scatter","Text1",  "Ramachandran Scatter Plot\r\n" + m_GraphTitle);
			InteractOrigin.ChangeLabel("Contour3D","Text1","Ramachandran Landscape Plot\r\n" + m_GraphTitle);
			InteractOrigin.ChangeLabel("ContourHiRes", "Text",  "Ramachandran Contour Plot\r\n" + m_GraphTitle);
		}

		#endregion

		#region matrix functions
		private int GetMaxValue( int[,] matrix )
		{
			int maxValue = -1;			
			int countToX = matrix.GetUpperBound(0) + 1;
			int countToY = matrix.GetUpperBound(1) + 1;
			for( int i = 0; i < countToX; i++ )
			{
				for( int j = 0; j < countToY; j++ )
				{
					int val = matrix[i,j];
					if( val > maxValue )
					{
						maxValue = val;
					}
				}
			}
			return maxValue;
		}

		private void SetupMatrices()
		{
			BinToMatrix( m_Matrix180, phiData, psiData );
			BinToMatrix( m_Matrix110, phiData, psiData );
			BinToMatrix( m_Matrix72, phiData, psiData );
		}

		private void ResetMatrices()
		{
			ResetMatrix( m_Matrix180 );
			ResetMatrix( m_Matrix110 );
			ResetMatrix( m_Matrix72 );
		}

		private void ResetMatrix( int[,] matrix )
		{
			int countToX = matrix.GetUpperBound(0) + 1;
			int countToY = matrix.GetUpperBound(1) + 1;
			for( int i = 0; i < countToX; i++ )
			{
				for( int j = 0; j < countToY; j++ )
				{
					matrix[i,j] = 0;
				}
			}
		}

		private void BinToMatrix( int[,] matrix, double[] xVals, double[] yVals )
		{
			int countTo = xVals.Length;
			if( countTo != yVals.Length )
			{
				throw new Exception();
			}
			
			int boundX = matrix.GetUpperBound(0) + 1;
			int boundY = matrix.GetUpperBound(1) + 1;

			if( boundX != boundY )
			{
				throw new Exception("matrix is not square");
			}
			if( boundX % 2 != 0 )
			{
				throw new Exception("marix is the wrong size");
			}


			double binInc = 360.0 / (double)boundX;
			for( int q = 0; q < countTo; q++ )
			{
				double x = phiData[q] + 180.0; // make all values +ve
				double y = psiData[q] + 180.0;
				int indexX = (int)(x / binInc);
				int indexY = (int)(y / binInc);
				if( indexX >= boundX )
				{
					indexX = 0; // -180 == 180 by definition
					continue;
				}
				if( indexY >= boundY )
				{
					indexY = 0; // -180 == 180 by definition
					continue;
				}
				matrix[indexY,indexX]++;
			}
		}

		private void GetContourCullHighValue( int[,] matrix, out double secondHighest )
		{
			int boundX = matrix.GetUpperBound(0) + 1;
			double binInc = 360.0 / (double)boundX;

			double[] anglesPhi = raftPhi;
			if( anglesPhi.Length == 0 )
			{
				anglesPhi = new double[] { -140.0f,-72.0f,-122.0f,-82.0f,-61.0f,57.0f };
			}
			double[] anglesPsi = raftPsi;
			if( anglesPsi.Length == 0 )
			{
				anglesPsi = new double[] {153.0f,145.0f,117.0f,-14.0f,-41.0f,39.0f };
			}

			int[] raftCounts = new int[ anglesPsi.Length ];

			for( int q = 0; q < anglesPhi.Length; q++ )
			{
				double x = anglesPhi[q] + 180.0; // make all values +ve
				double y = anglesPsi[q] + 180.0;
				int indexX = (int)(x / binInc);
				int indexY = (int)(y / binInc);
				raftCounts[q] = GetLocalHigh( matrix, indexY, indexX, 20, 2 );
			}

			Array.Sort( raftCounts );

			secondHighest = raftCounts[raftCounts.Length-2];
		}
		private int GetLocalHigh( int[,] matrix, int y, int x, int plusMinus, int dispersion )
		{
			int max = 0;
			for( int i = y - plusMinus; i <= y + plusMinus; i += dispersion )
			{
				for( int j = x - plusMinus; j <= x + plusMinus; j += dispersion )
				{
					if( max < matrix[y,x] )
					{
						max = matrix[y,x];
					}
				}
			}
			return max;
		}
		#endregion
	}
}
