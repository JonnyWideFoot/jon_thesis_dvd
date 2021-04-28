using System;
using System.IO;
using System.Collections;

using UoB.Core.Sequence;
using UoB.Core.FileIO.DSSP;

namespace UoB.Methodology.DSSPAnalysis
{
	/// <summary>
	/// DSSPTaskDirecory_PhiPsiData:
	/// Used for any derived classes of DSSPTaskDirectory which require Phi and Psi data extracted
	/// from all the files in the current DSSP directory.
	/// </summary>
	public abstract class DSSPTaskDirecory_PhiPsiData : DSSPTaskDirectory
	{
		protected double[] phiData = null; // filled with database angles by ObtainData()
		protected double[] psiData = null; // data from all DSSP files in the current directory

		public DSSPTaskDirecory_PhiPsiData( string DSSPDatabaseName, DirectoryInfo di, bool OriginInteractionRequired ) 
			: base( DSSPDatabaseName, di, OriginInteractionRequired )
		{
		}


		#region PhiPsi Distribution DataSetup
		// "DSSPIncludedRegions mode" : only relevent when doReportOn == DSSPReportingOn.LoopsOnly or DSSPReportingOn.SecondaryOnly
		protected int ObtainPhiPsiData( DSSPReportingOn doReportOn, DSSPIncludedRegions mode, StandardResidues resTypes )
		{
			// Temporary PhiPsiData Arraylist holders
			ArrayList obtainPhis = new ArrayList();
			ArrayList obtainPsis = new ArrayList();

			// the phiData and psiData arrays
			ParsingFileIndex = 0; // reset IMPORTANT
			
			// task
			if( doReportOn == DSSPReportingOn.LoopsOnly )
			{
				while( true )
				{
					CurrentFileAppendLoopPhiPsis( obtainPhis, obtainPsis, mode, resTypes );
					// increment conidtion
					if( ParsingFileIndex < FileCount - 1 && ParsingFileIndex < MaxFileImport )
					{
						ParsingFileIndex++;
					}
					else
					{
						break;
					}
				}
			}
			else if ( doReportOn == DSSPReportingOn.All )
			{
				while( true )
				{
					CurrentFileAppendAllPhiPsis( obtainPhis, obtainPsis, resTypes );
					// increment conidtion
					if( ParsingFileIndex < FileCount - 1 && ParsingFileIndex < MaxFileImport )
					{
						ParsingFileIndex++;
					}
					else
					{
						break;
					}
				}
			}
			else if( doReportOn == DSSPReportingOn.SecondaryOnly )
			{
				while( true )
				{
					CurrentFileAppendSecondaryPhiPsis( obtainPhis, obtainPsis, mode, resTypes );
					// increment conidtion
					if( ParsingFileIndex < FileCount - 1 && ParsingFileIndex < MaxFileImport )
					{
						ParsingFileIndex++;
					}
					else
					{
						break;
					}
				}
			}
			else
			{
				throw new NotImplementedException();
			}				

			// transfer data to member arrays
			phiData = (double[]) obtainPhis.ToArray( typeof(double) );
			psiData = (double[]) obtainPsis.ToArray( typeof(double) );

			return phiData.Length; // rerturn the number of angles that were extracted
		}

		private void CurrentFileAppendLoopPhiPsis( ArrayList phiList, ArrayList psiList, DSSPIncludedRegions mode, StandardResidues residueInclude )
		{
			SegmentDef[] loops = CurrentFile.GetLoops(); // get the current loop set

			int startCountFrom = 0;
			int endCountAt = loops.Length;

			// assign which loops we want
			if( mode == DSSPIncludedRegions.AllExceptTermini || mode == DSSPIncludedRegions.OnlyDefinitelyGood )
			{
				startCountFrom++;
				endCountAt--; // we ignore the termini, these are always there, even if they have a length of 0
			}

			for( int i = startCountFrom; i < endCountAt; i++ )
			{
				SegmentDef ld = loops[i];
				if( mode == DSSPIncludedRegions.AllExceptIncompleteSegments || mode == DSSPIncludedRegions.OnlyDefinitelyGood )
				{
					if( ld.Length == -1 )
					{
						continue; // if -1 the loop is incomplete in the PDB, the phi and psi of the respective anchors will be 360
					}
				}
				for( int j = 0; j < ld.Length; j++ )
				{
					// could have unknown '!' residues, but these will be ignored
					if( StandardSeqTools.IsResTypeMatch( residueInclude, ld.Sequence[j] )					
						&& ld[j].PhiAndPsiNotNull )
					{
						phiList.Add( ld.GetPhi(j) );
						psiList.Add( ld.GetPsi(j) );
					}
				}
			}
		}

		private void CurrentFileAppendSecondaryPhiPsis( ArrayList phiList, ArrayList psiList, DSSPIncludedRegions mode, StandardResidues residueInclude )
		{
			SegmentDef[] secStructures = CurrentFile.GetSecondaryStructures(); // get the current loop set

			for( int i = 0; i < secStructures.Length; i++ )
			{
				SegmentDef segDef = secStructures[i];
				if( mode == DSSPIncludedRegions.AllExceptIncompleteSegments || mode == DSSPIncludedRegions.OnlyDefinitelyGood )
				{
					if( segDef.Length == -1 )
					{
						continue; // if -1 the loop is incomplete in the PDB, the phi and psi of the respective anchors will be 360
					}
				}
				for( int j = 0; j < segDef.Length; j++ )
				{
					// could have unknown '!' residues, but these will be ignored
					if( StandardSeqTools.IsResTypeMatch( residueInclude, segDef.Sequence[j] )					
						&& segDef[j].PhiAndPsiNotNull )
					{
						phiList.Add( segDef.GetPhi(j) );
						psiList.Add( segDef.GetPsi(j) );
					}
				}
			}
		}

		private void CurrentFileAppendAllPhiPsis( ArrayList phiList, ArrayList psiList, StandardResidues residueInclude )
		{
			ResidueDef[] residues = CurrentFile.GetResidues();
			for( int i = 0; i < residues.Length; i++ )
			{
				ResidueDef rd = residues[i];
				if( rd.AminoAcidID != '!' 
					&& StandardSeqTools.IsResTypeMatch( residueInclude, rd.AminoAcidID )
					&& rd.PhiAndPsiNotNull ) // only add a complete pair ...
				{
					phiList.Add( rd.Phi );
					psiList.Add( rd.Psi );
				}
			}
		}
		#endregion
	}
}
