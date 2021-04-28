using System;
using System.IO;

using UoB.Core.Sequence;
using UoB.Core.FileIO.DSSP;

using UoB.Methodology.TaskManagement;
using UoB.Methodology.DSSPAnalysis;

namespace UoB.Methodology.DSSPAnalysis.Ramachandran
{
	/// <summary>
	/// Summary description for PhiPsiPrinting.
	/// </summary>
	public class PhiPsiPrinting : DSSPTaskDirecory_PhiPsiData
	{
		public PhiPsiPrinting( string DSSPDatabaseName, DirectoryInfo di ) 
			: base( DSSPDatabaseName, di, false )
		{
			// empty
		}


		#region Public phi psi printing functions
		public void Write2DBinLoopPhiPsis( int binCount, DSSPIncludedRegions mode, StandardResidues includedResidues )
		{
			string reportFileName = reportDirectory.FullName + "PsiPsiList_AllStructure.binned.csv";
			float binCountF = (float) binCount;
			if( binCountF < 0.0f || binCountF > 360000.0f ) throw new Exception("Bin resolution is invalid");
			float binSize = 360.0f / binCountF;

			double[] buckets = new double[ binCount ];
			int[,] bins = new int[binCount,binCount];
			int halflength = buckets.Length / 2;
			for( int i = 0; i < buckets.Length; i++ )
			{
				int j = i - halflength;
				buckets[i] = (float)j * binSize;
			}

			ParsingFileIndex = 0; // reset IMPORTANT
			StreamWriter rw = new StreamWriter( reportFileName, false );
			while( true )
			{
				// task
				AppendLoopPhiPsis( CurrentFile, buckets, bins, mode, includedResidues );

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

			bool doTitle = false;

			if( doTitle )
			{
				rw.Write(',');
				for( int i = 0; i < buckets.Length; i++ )
				{
					rw.Write( buckets[i] );
					rw.Write( ',' );
				}
				rw.WriteLine();
			}

			for( int i = 0; i < binCount; i++ )
			{
				if( doTitle )
				{
					rw.Write( buckets[i] );
					rw.Write( ',' );
				}

				for( int j = 0; j < binCount; j++ )
				{
					rw.Write( bins[j,i] );
					rw.Write( ',' );
				}

				rw.WriteLine();
			}

			rw.Close();
		}

		public void WriteAllFilesAllPhiPsis( StandardResidues includedResidues )
		{
			string reportFileName = reportDirectory.FullName + Path.DirectorySeparatorChar + "PsiPsiList_AllStructure.list.csv";

			ParsingFileIndex = 0; // reset IMPORTANT
			StreamWriter rw = new StreamWriter( reportFileName, false );
			while( true )
			{
				// task
				AppendAllPhiPsis( CurrentFile, rw, includedResidues );

				// the per-file task
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

		public void WriteAllFilesLoopPhiPsis( DSSPIncludedRegions mode, StandardResidues includedResidues )
		{
			string reportFileName = reportDirectory.FullName + "PsiPsiList_Loops.list.csv";

			ParsingFileIndex = 0; // reset IMPORTANT
			StreamWriter rw = new StreamWriter( reportFileName, false );
			while( true )
			{
				// the per-file task
				AppendLoopPhiPsis( CurrentFile, rw , mode, includedResidues );

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

		public void WriteAllFilesSecondaryPhiPsis( DSSPIncludedRegions mode, StandardResidues includedResidues )
		{
			string reportFileName = reportDirectory.FullName + "PsiPsiList_Secondary.list.csv";

			ParsingFileIndex = 0; // reset IMPORTANT
			StreamWriter rw = new StreamWriter( reportFileName, false );
			while( true )
			{
				// the per-file task
				AppendSecondaryPhiPsis( CurrentFile, rw , mode, includedResidues );

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

		#region Stream Appending functions for Output
		private void AppendLoopPhiPsis( DSSPFile currentFile, double[] buckets, int[,] bins, DSSPIncludedRegions mode, StandardResidues residueInclude )
		{
			bool includeTermini = (mode != DSSPIncludedRegions.AllExceptTermini) && ( mode != DSSPIncludedRegions.OnlyDefinitelyGood );
			SegmentDef[] segments = currentFile.GetLoops( includeTermini );

			for( int i = 0; i < segments.Length; i++ )
			{
				if( mode == DSSPIncludedRegions.AllExceptIncompleteSegments || mode == DSSPIncludedRegions.OnlyDefinitelyGood )
				{
					if( segments[i].IsIncompleteInPDB )
					{
						continue; // the loop is incomplete in the PDB, the phi and psi of the respective anchors will be 360
					}
				}
				for( int j = 0; j < segments[i].Length; j++ )
				{
					if( StandardSeqTools.IsResTypeMatch( residueInclude, segments[i].Sequence[j] ) )
					{
						double phi = segments[i].GetPhi(j);
						double psi = segments[i].GetPsi(j);
						int phiIndex = -1;
						if( phi < buckets[0] )
						{
							continue; // out of range ..
						}
						else
						{
							phiIndex = 0;
						}
						for( int k = 1; k < buckets.Length; k++ )
						{
							if( phi < buckets[k] )
							{
								phiIndex = k;
								break;
							}
						}
						if( phiIndex == -1 ) continue; // that phi angle wasnt in bounds
						for( int k = 0; k < buckets.Length; k++ )
						{
							if( psi < buckets[k] )
							{
								bins[phiIndex,k]++;
								break; // found it
							}
						}
					}
				}
			}
		}

		private void AppendLoopPhiPsis( DSSPFile currentFile, StreamWriter rw, DSSPIncludedRegions mode, StandardResidues residueInclude )
		{
			bool includeTermini = (mode != DSSPIncludedRegions.AllExceptTermini) && ( mode != DSSPIncludedRegions.OnlyDefinitelyGood );
			SegmentDef[] segments = currentFile.GetLoops( includeTermini );

			for( int i = 0; i < segments.Length; i++ )
			{
				if( mode == DSSPIncludedRegions.AllExceptIncompleteSegments || mode == DSSPIncludedRegions.OnlyDefinitelyGood )
				{
					if( segments[i].IsIncompleteInPDB )
					{
						continue; // the loop is incomplete in the PDB, the phi and psi of the respective anchors will be 360
					}
				}
				for( int j = 0; j < segments[i].Length; j++ )
				{
					if( StandardSeqTools.IsResTypeMatch( residueInclude, segments[i].Sequence[j] )
						&& segments[i][j].PhiAndPsiNotNull 
						)
					{
						rw.Write( segments[i].GetPhi(j) );
						rw.Write(',');
						rw.WriteLine( segments[i].GetPsi(j) );
					}
				}
			}

			rw.Flush();
		}

		private void AppendAllPhiPsis( DSSPFile currentFile, StreamWriter rw, StandardResidues residueInclude )
		{
			ResidueDef[] res = currentFile.GetResidues();
			for( int i = 0; i < res.Length; i++ )
			{
				ResidueDef rd = (ResidueDef) res[i];
				if( rd.AminoAcidID != '!' 
					&& StandardSeqTools.IsResTypeMatch( residueInclude, rd.AminoAcidID )
					&& rd.PhiAndPsiNotNull )
				{
					rw.Write(rd.Phi);
					rw.Write(',');
					rw.Write(rd.Psi);
					rw.WriteLine();
				}
			}

			rw.Flush();
		}

		private void AppendSecondaryPhiPsis( DSSPFile currentFile, StreamWriter rw, DSSPIncludedRegions mode, StandardResidues residueInclude )
		{
			SegmentDef[] segments = currentFile.GetSecondaryStructures();

			for( int i = 0; i < segments.Length; i++ )
			{
				if( mode == DSSPIncludedRegions.AllExceptIncompleteSegments || mode == DSSPIncludedRegions.OnlyDefinitelyGood )
				{
					if( segments[i].IsIncompleteInPDB )
					{
						continue; // the loop is incomplete in the PDB, the phi and psi of the respective anchors will be 360
					}
				}
				for( int j = 0; j < segments[i].Length; j++ )
				{
					if( StandardSeqTools.IsResTypeMatch( residueInclude, segments[i].Sequence[j] ) 
						&& segments[i][j].PhiAndPsiNotNull 
						)
					{
						rw.Write( segments[i].GetPhi(j) );
						rw.Write(',');
						rw.WriteLine( segments[i].GetPsi(j) );
					}
				}
			}

			rw.Flush();
		}
		#endregion
	}
}
