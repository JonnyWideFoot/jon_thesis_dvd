using System;

namespace UoB.Methodology.DSSPAnalysis.Ramachandran
{
	/// <summary>
	/// Summary description for SASAPrinting.
	/// </summary>
	public class SASAPrinting : DSSPTaskDirecory
	{
		public SASAPrinting()
		{
			// heavy editing has rendered this class broken. Recoding needs to occur if it is to be used again ...
			throw new NotImplementedException();
		}

		#region SASA reporting
		public void WriteSASASummary( string filename )
		{
			StreamWriter rw = new StreamWriter( filename, false );
			rw.WriteLine("Residue,AverageSASA,MaxPosSASA,Percentage");

			StandardResidues[] resModes = (StandardResidues[]) Enum.GetValues( typeof(StandardResidues) );
			string[] names = (string[]) Enum.GetNames( typeof(StandardResidues) );
			double[] TotalCumulativeSASAs = new double[resModes.Length];
			double[] TotalCumulativeCounts = new double[resModes.Length];
			double dLength = (double) FileCount;
			
			ParsingFileIndex = 0; // reset IMPORTANT
			// calculate the average values
			while( true )
			{
				// task
				for( int i = 0; i < resModes.Length; i++ )
				{
					CurrentFile.AppendAllSASAs( resModes[i], ref TotalCumulativeSASAs[i], ref TotalCumulativeCounts[i] );
				}

				// increment conidtion
				if( ParsingFileIndex < FileCount - 1 )
				{
					ParsingFileIndex++;
				}
				else
				{
					break;
				}
			}

			// write the title block
			for( int i = 0; i < TotalCumulativeSASAs.Length; i++ )
			{
				rw.Write( names[i] );
				rw.Write( ',' );
				double average = TotalCumulativeSASAs[i] / TotalCumulativeCounts[i];
				rw.Write( average );
				rw.Write( ',' );
				rw.Write( m_StoredResidueSASAs[i] );
				rw.Write( ',' );
				rw.WriteLine( (average / m_StoredResidueSASAs[i]) *100.0 );
			}

			rw.Close();
		}

		public void WriteAllFilesSASA( string filename, StandardResidues includedResidues )
		{
			ParsingFileIndex = 0; // reset IMPORTANT
			StreamWriter rw = new StreamWriter( filename, false );
			while( true )
			{
				// task
				CurrentFile.AppendAllSASAs( rw , includedResidues );

				// increment conidtion
				if( ParsingFileIndex < FileCount - 1 )
				{
					ParsingFileIndex++;
				}
				else
				{
					break;
				}
			}
			rw.Close();
		}

		#endregion

		public void AppendAllSASAs( StreamWriter rw, StandardResidues residueInclude )
		{
			// title line
			// rw.WriteLine(Name);

			for( int i = 0; i < m_Residues.Count; i++ )
			{
				ResidueDef rd = (ResidueDef) m_Residues[i];
				if( IsResTypeMatch( residueInclude, rd.AminoAcidID ) )
				{
					//rw.Write( rd.AminoAcidID ); // was an old check
					//rw.Write(',');
					rw.WriteLine( rd.SASA );
				}
			}

			// rw.WriteLine(); // EoF blanking line
		}

		public void AppendAllSASAs( StandardResidues residueInclude, ref double AddToTotalArea, ref double AddToTotalCount )
		{
			for( int i = 0; i < m_Residues.Count; i++ )
			{
				ResidueDef rd = (ResidueDef) m_Residues[i];
				if( IsResTypeMatch( residueInclude, rd.AminoAcidID ) )
				{
					AddToTotalArea += rd.SASA;
					AddToTotalCount += 1.0;
				}
			}
		}
	

	}
}
