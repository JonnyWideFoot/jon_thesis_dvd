using System;
using System.IO;
using System.Collections;
using System.Text;

using UoB.Core.MoveSets.AngleSets;
using UoB.Core.FileIO.DSSP;
using UoB.Core.Sequence;
using UoB.Core.FileIO.Raft;
using UoB.Methodology.OriginInteraction;
using UoB.Methodology.DSSPAnalysis;
using UoB.Methodology.DSSPAnalysis.AngleFitting;

namespace UoB.Methodology.DSSPAnalysis.AngleSetAnalysis
{
	/// <summary>
	/// Summary description for AngleFittingReport.
	/// </summary>
    public sealed class AngleSetQualityReport : AngleFittingEngineOutputConverter
	{
		// Angle Set holders ...
        private double[] phiAngleSet_A = null; // A = all
        private double[] psiAngleSet_A = null;
        private double[] phiAngleSet_L = null; // L = loop
        private double[] psiAngleSet_L = null;

        private double[] m_RAFTPhi = null;
        private double[] m_RAFTPsi = null;
        private AngleSet m_RAFTAngleSet;

		public AngleSetQualityReport( string DSSPDatabaseName, DirectoryInfo di ) : base( DSSPDatabaseName, di, true )
		{
			m_RAFTAngleSet = new AngleSet( SpecifiedAngleSet.OriginalRAFT );
		}
				
		#region HTML reporting
		public void GoHTMLAngleFittingReport()
		{
			StandardResidues[] singleResTypes = StandardSeqTools.GetIndividualStandardResidues();
            //StandardResidues[] singleResTypes = new StandardResidues[] { StandardResidues.p };

			HTMLReportingBegin( "Angle Fitting Report</h1><h2>Using the " + DBName + "database</h2><h1>" );

            for (int mode = 0; mode <= 0; mode++)
            {
                for (int i = 0; i < singleResTypes.Length; i++)
                {
                    for (int a = 2; a <= 10; a++)
                    {
                        char modeID = mode.ToString()[0];
                        char molTypeID = singleResTypes[i].ToString()[0];

                        if (StartAngleSetImport(a, molTypeID, modeID))
                        {
                            HTMLStartReportBlock("Report for: " + molTypeID + " " + a.ToString() + " Angle Set under mode " + modeID);
                            GetAngleFitToOrigin(a, singleResTypes[i], modeID);
                            HTMLEndReportBlock();
                            HTMLReportingDivider();
                            EndAngleSetImport();
                        }
                    }
                }
            }

			HTMLReportingEnd();		
		}

		private void GetAngleFitToOrigin( int angleCount, StandardResidues resTypes, char mode )
		{
			// clear current data
			AngleFitReset();

            // Scatter
            ObtainPhiPsiData(DSSPReportingOn.LoopsOnly, DSSPIncludedRegions.OnlyDefinitelyGood, resTypes);
            InteractOrigin.UpdateWorksheet("Scatter", phiData, psiData);            

			// put the data back in
			AngleFitUpdatePsiPsi_RAFT( resTypes );

			ObtainAngleFitDataSet( angleCount, DSSPReportingOn.All, resTypes, true );
            phiAngleSet_A = (double[])m_FittedPhis.Clone();
            psiAngleSet_A = (double[])m_FittedPsis.Clone();
			AngleFitUpdatePsiPsi_All();            

			ObtainAngleFitDataSet( angleCount, DSSPReportingOn.LoopsOnly, resTypes, true );
            phiAngleSet_L = (double[])m_FittedPhis.Clone();
            psiAngleSet_L = (double[])m_FittedPsis.Clone();
			AngleFitUpdatePsiPsi_Loops();

			// make array length assertions
			Assert( ( phiAngleSet_A.Length == psiAngleSet_A.Length ), "Angle array length mismatch" );
			Assert( ( phiAngleSet_L.Length == psiAngleSet_A.Length ), "Angle array length mismatch" );
			Assert( ( phiAngleSet_L.Length == psiAngleSet_L.Length ), "Angle array length mismatch" );

			// setup naming
			string name = resTypes.ToString();
			m_OutputFileStem = name + '_' + angleCount.ToString() + '_' + mode;
			m_GraphTitle = angleCount.ToString() + " Angles fitted for \"" + name + "\" under mode " + mode;

			//AngleFitUpdateOriginGraph();
	
			HTMLAngleFitReport();	

			return; // done
		}

		private bool StartAngleSetImport( int angleCount, char resID, char modeID )
		{
			//m_AngleStream = new StreamReader( reportDirectory.FullName + angleCount.ToString() + "_Angles.csv" );

            char cisChar = 'T';
            if (Char.IsLower(resID)) cisChar = 'C';

			FileInfo[] matchingFiles = this.resultDirectory.GetFiles( angleCount.ToString() + '_' + resID + cisChar + '_' + modeID + '*' );
			if( matchingFiles.Length == 0 )
			{
				return false;
			}
			else if(  matchingFiles.Length > 1 )
			{
				throw new Exception("Ambiguous file descriptor");
			}
			else
			{
				m_AngleStream = new StreamReader( matchingFiles[0].FullName );
				return true;
			}
		}

		private void EndAngleSetImport()
		{
			m_AngleStream.Close();
			m_AngleStream = null;
		}

		private void HTMLAngleFitReport()
		{
			string pageName = "fitgraph";

            string thumbName = m_OutputFileStem + "_T_" + pageName;
			InteractOrigin.SavePicture( ImageType.PNG, reportDirectory.FullName + thumbName, pageName, 450, 450 );
			string imgName = m_OutputFileStem + "_B_" + pageName;
            InteractOrigin.SavePicture( ImageType.PNG, reportDirectory.FullName + imgName, pageName, 1000, 1000);
            //InteractOrigin.SaveEPSPicture(reportDirectory.FullName + imgName + ".eps", pageName);

			m_HTMLReporter.WriteLine("<tr>");

			m_HTMLReporter.WriteLine("<td>");
			m_HTMLReporter.Write("<a href=\"");
			m_HTMLReporter.Write(imgName); // relative path
			m_HTMLReporter.Write(".png\"><img border=0 src=\"");
			m_HTMLReporter.Write(thumbName);
			m_HTMLReporter.WriteLine(".png\"></a>");
			m_HTMLReporter.WriteLine("</td>");

			// Write the result table here ...

			m_HTMLReporter.WriteLine("<td>");

			m_HTMLReporter.WriteLine("<table width=470 border=1 bordercolor=black cellpadding=2 cellspacing=0>");
			m_HTMLReporter.WriteLine("<tr><td width=50>-</td><td width=140>RAFT</td><td width=140>All</td><td width=140>Loop</td></tr>");

			for( int i = 0; i < phiAngleSet_A.Length; i++ )
			{
				m_HTMLReporter.WriteLine("<tr>");

				m_HTMLReporter.WriteLine("<td width=50>");

				m_HTMLReporter.Write( i );

				m_HTMLReporter.WriteLine("</td>");
				m_HTMLReporter.WriteLine("<td width=140>");

				if( i < m_RAFTPhi.Length )
				{
					m_HTMLReporter.Write( m_RAFTPhi[i].ToString("0.00") );
					m_HTMLReporter.Write( ", " );
					m_HTMLReporter.Write( m_RAFTPsi[i].ToString("0.00") );
				}
				else
				{
					m_HTMLReporter.Write( "-" );
				}

				m_HTMLReporter.WriteLine("</td>");
				m_HTMLReporter.WriteLine("<td width=140>");

				m_HTMLReporter.Write( phiAngleSet_A[i].ToString("0.00") );
				m_HTMLReporter.Write( ", " );
				m_HTMLReporter.Write( psiAngleSet_A[i].ToString("0.00") );

				m_HTMLReporter.WriteLine("</td>");
				m_HTMLReporter.WriteLine("<td width=140>");

				m_HTMLReporter.Write( phiAngleSet_L[i].ToString("0.00") );
				m_HTMLReporter.Write( ", " );
				m_HTMLReporter.Write( psiAngleSet_L[i].ToString("0.00") );

				m_HTMLReporter.WriteLine("</td>");

				m_HTMLReporter.WriteLine("</tr>");
			}
			
			m_HTMLReporter.WriteLine("</table>");

			m_HTMLReporter.WriteLine("</td>");
			m_HTMLReporter.WriteLine("</tr>");		

			m_HTMLReporter.Flush();
		}

		#endregion

		#region Origintalk
		private void AngleFitReset()
		{
			InteractOrigin.Reset(true,true);
		}
		private void AngleFitUpdatePsiPsi_RAFT( StandardResidues resTypes )
		{
			char id = resTypes.ToString()[0]; // get the resID
			m_RAFTPhi = m_RAFTAngleSet.GetPhis(id); // update the angleset residue we are looking at
			m_RAFTPsi = m_RAFTAngleSet.GetPsis(id); // update the angleset residue we are looking at
			InteractOrigin.UpdateWorksheet( "RAFT", m_RAFTPhi, m_RAFTPsi );	
		}
		private void AngleFitUpdatePsiPsi_All()
		{
			InteractOrigin.UpdateWorksheet( "All", phiAngleSet_A, psiAngleSet_A );	
		}
		private void AngleFitUpdatePsiPsi_Loops()
		{
			InteractOrigin.UpdateWorksheet( "Loop", phiAngleSet_L, psiAngleSet_L );	
		}
		private void AngleFitUpdateOriginGraph()
		{
			InteractOrigin.ChangeLabel( "fitgraph", "TextyBob", "Angle Fitting Summary Plot\r\n" + m_GraphTitle );
		}
		#endregion
	}
}
