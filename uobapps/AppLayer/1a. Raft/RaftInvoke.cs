using System;
using System.IO;
using System.Collections;

using UoB.Core.Structure;
using UoB.Core.FileIO.Raft;
using UoB.AppLayer;

namespace UoB.AppLayer.Raft
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class RaftInvoke : AppLayerBase
	{
		public void MainStem( string[] args )
		{
			//			string pathStem = @"C:\JTRANS\";
			//			string jobStem = "2005_04_29";
			//			
			//			string pdbFileaname = "1rhz-TM23-82-111.pdb";

			//WriteRaftFiles( pathStem, jobStem, pdbFileaname );




			// CALL ONE OF THE FUNCTIONS BELOW OR WRITE A NEW ONE ....



			return;
		}
		
		public void WriteRaftExhaustiveBinRestrictionCRMSTest( string dirPath, string jobStem, PolyPeptide polymer, int length )
		{
			string templateDir = AssertDir( dirPath, "_Template\\" );
			string pdbLoopLoadPath = AssertDir( dirPath, "PDBLoop\\" );
			string autoDir = AssertDir( dirPath, "_autogen\\" );

			// seq file is per input and is always needed
			WriteRaftSeqFile( autoDir + jobStem + ".seq", polymer );

			int confCount = WriteRaftCnfFile_AllAnglesInSameGlobalBinAsNative( autoDir + jobStem + ".init.cnf", polymer );

			EmcFillParams emcParams = new EmcFillParams();
			emcParams.genStart = confCount;
			emcParams.parentPass = confCount;
			emcParams.genCount = 1;
			emcParams.midConfCount = 0;
			emcParams.midCoordCount = 0;
			emcParams.lastConfCount = confCount;
			emcParams.lastCoordCount = (confCount<50) ? confCount : 50;
			emcParams.mutationRate = 0.0f;

			WriteRaftEmcFile( autoDir + jobStem + ".emc", emcParams );

			string libPath = "../_Exec/Lib/"; // reletive path : back out of the job working directory one level
			string[] keys   = new string[] { "LibPath", "filestem", "PDBTemplateFile" }; // keys
			string[] values = new string[] { libPath, jobStem, "" + jobStem + ".pdb" };   // values
			WriteRaftInpFile( templateDir, autoDir, jobStem, keys, values );
		}

		public static void WriteRaftExhaustiveCRMSTest( string dirPath, string jobStem, string loopHostPDBFile, string assertTheSequence, int startID, char insertionCode, int length )
		{
			string templateDir = AssertDir( dirPath, "_Template\\" );
			string pdbLoadPath = AssertDir( dirPath, "PDB\\" );
			string pdbLoopSavePath = AssertDir( dirPath, "PDBLoop\\" );
			string autoDir = AssertDir( dirPath, "_autogen\\" );

			// get our PDB file
			PDB.PDB file = new PDB.PDB( pdbLoadPath + loopHostPDBFile, true );
			PolyPeptide p = (PolyPeptide) file.particleSystem.MemberAt(0);
			MolRange loopMRange = new MolRange( p, startID, insertionCode, length ); // the loop definition
			
			// clone the loop purely so that PDB file that we give to raft is known to be standard
			ParticleSystem ps = new ParticleSystem("Clone");
			ps.BeginEditing();
			PolyPeptide pClone = (PolyPeptide) loopMRange.CloneFromRange();
			ps.AddMolContainer( pClone );
			// our loop needs corrent primitives at the ends for the rebuild opparation
			AminoAcid newNTerminus = (AminoAcid) pClone[0];
			newNTerminus.setMolPrimitive( 'N', true );
			AminoAcid newCTerminus = (AminoAcid) pClone[pClone.Count-1];
			newCTerminus.setMolPrimitive( 'C', true );
			// set, now we are done edting the clone...
			ps.EndEditing(true,true);

			PS_Builder rebuilder = new PS_Builder( ps );
			// CHECK THIS IS WHAT WE WANT
			rebuilder.RebuildTemplate( RebuildMode.HeavyAtomsOnly, false, false, false, false );

			string loopPDBSaveName = pdbLoopSavePath + jobStem + ".pdb";
			PDB.PDB.SaveNew( loopPDBSaveName, ps ); // save it
			// Done

			// check that the sequences match
			if( assertTheSequence != pClone.MonomerString )
			{
				throw new Exception("The sequence extracted from the DSSP file does not match the loop sequence!");
			}

			// write cnf file if needed
			string cnfName = length.ToString() + ".init.cnf";
			string cnfPath = autoDir + cnfName;
			if( !File.Exists( cnfPath ) )
			{
				WriteRaftCnfFile( cnfPath, length, 0, 1 /* - doesnt matter what it is if the length is 0, use default 1*/ );
			}

			// write emc file if needed
			string emcName = length.ToString() + ".emc";
			string emcPath = autoDir + emcName;
			if( !File.Exists( emcPath ) )
			{
				EmcFillParams emcParams = new EmcFillParams();
				int confCount = (int) Math.Pow( 6.0, (double)length );
				emcParams.genStart = confCount;
				emcParams.parentPass = confCount;
				emcParams.genCount = 1;
				emcParams.midConfCount = 0;
				emcParams.midCoordCount = 0;
				emcParams.lastConfCount = 100;
				emcParams.lastCoordCount = 100;
				emcParams.mutationRate = 0.0f;

				WriteRaftEmcFile( emcPath, emcParams );
			}

			// seq file is per input and is always needed
			WriteRaftSeqFile( autoDir + jobStem + ".seq", pClone );

			//string libPath = "/mem/jr0407/Raft/Lib/"; // specific path
			string libPath = "../_Exec/Lib/"; // reletive path : back out of the job working directory one level
			string[] keys   = new string[] { "LibPath", "filestem", "PDBTemplateFile", "cnfname", "emcname" }; // keys
			string[] values = new string[] { libPath, jobStem, "" + jobStem + ".pdb", cnfName, emcName };   // values
			WriteRaftInpFile( templateDir, autoDir, jobStem, keys, values );
		}

		private static void Write_IGNORE_INITIALISATION_TEMPATE( string dirPath, string jobStem, string pdbFileName )
		{
			string templateDir = AssertDir( dirPath, "_Template\\" );
			string autoDir = AssertDir( dirPath, "_autogen\\" );
			string pdbLoadPath = AssertDir( dirPath, "PDB\\" );

			// clone the system purely so that PDB file that we give to raft is known to be standard
			PDB.PDB file = new PDB.PDB( pdbLoadPath + pdbFileName, true );

			PolyPeptide p = (PolyPeptide) file.particleSystem.MemberAt(0);
			PolyPeptide pClone = (PolyPeptide) p.Clone();

			ParticleSystem ps = new ParticleSystem("Clone");
			ps.BeginEditing();
			ps.AddMolContainer( pClone );
			ps.EndEditing(true,true);

			PDB.PDB.SaveNew( autoDir + pdbFileName, ps );

			WriteRaftCnfFile( autoDir + jobStem + ".init.cnf", p.Count, 0, 5 );
			//WriteRaftRstFile( autoDir + jobStem + ".rst", pClone, 5 );
			WriteRaftSeqFile( autoDir + jobStem + ".seq", pClone );

			// .inp and emc files require some specific precalculation to fill the fields in the template files
			int confCount = (int) Math.Pow( 6.0, (double) pClone.Count );
			
			string[] keys   = new string[] { "filestem", "Gen0ConfCount", "PDBTemplateFile" }; // keys
			string[] values = new string[] { jobStem, confCount.ToString().PadLeft(10,' '), pdbFileName };   // values
			WriteRaftInpFile( templateDir, autoDir, jobStem, keys, values );	
	
			EmcFillParams emcParams = new EmcFillParams();
			emcParams.genStart = confCount;
			emcParams.parentPass = confCount;
			emcParams.genCount = 1;
			emcParams.midConfCount = 0;
			emcParams.midCoordCount = 0;
			emcParams.lastConfCount = 100;
			emcParams.lastCoordCount = 100;
			WriteRaftEmcFile( autoDir + jobStem + ".emc", emcParams );
		}
	}
}
