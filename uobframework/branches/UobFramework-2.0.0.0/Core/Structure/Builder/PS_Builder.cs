using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

using UoB.Core.FileIO.PDB;
using UoB.Core.Primitives;
using UoB.Core.Primitives.Matrix;
using UoB.Core.Sequence;
using UoB.Core.Structure;
using UoB.Core.Structure.Primitives;
using UoB.Core.ForceField;
using UoB.Core;
using UoB.Core.Tools;

namespace UoB.Core.Structure.Builder
{
	/// <summary>
	/// Summary description for PS_Builder.
	/// </summary>
	public class PS_Builder
	{
		private CoreIni m_CoreIni;
		private ArrayList m_Selections = new ArrayList(10);
		private Regex m_Regex = new Regex(@"\s+"); // scans whitespace, used for the text splitting function
		private FFManager m_FFParams;
		private Calculations m_Forcefield;

		private RotamerApplier m_RotApplier = new RotamerApplier();
		private RebuildDefinition m_RBD = new RebuildDefinition();
		private ArrayList m_RebuildAtomHolder = new ArrayList(20); // temporary holder during molecule reconstruction

		private ParticleSystem m_PSTemplate;
		private ParticleSystem m_PSModel;
		private char m_TemplateChainID = '\0';
		private string m_ThreadSequence = "";
		private string m_TemplateSequence = "";
		private string m_UndoValidate = "";

		public PS_Builder( ParticleSystem ps )
		{
			m_PSTemplate = ps;
			ConstructorCommonInit();
		}

		public PS_Builder()
		{
			m_PSTemplate = null;
			ConstructorCommonInit();
		}

		private void ConstructorCommonInit()
		{
			m_FFParams = FFManager.Instance;
			m_Forcefield = new Calculations();
			m_CoreIni = CoreIni.Instance;
			m_PSModel = new ParticleSystem("DAVEModel_1");
			InitialiseCurrentTemplate();

			string key = "ThreadSequence";
			if ( m_CoreIni.ContainsKey( key ) )
			{
				try
				{
					m_ThreadSequence = m_CoreIni.ValueOf( key );
				}
				catch // any error in bool parsing - i.e. the string is buggered
				{
					m_CoreIni.AddDefinition( key, m_ThreadSequence.ToString() );
				}
			}
			else
			{
				m_CoreIni.AddDefinition( key, m_ThreadSequence.ToString() );
			}
		}

		private void InitialiseCurrentTemplate()
		{
			if ( m_PSTemplate != null )
			{
				for( int i = 0; i < m_PSTemplate.Members.Length; i++ )
				{
					if( m_PSTemplate.Members[i] is PolyPeptide )
					{   // just get the first one we come to
						PolyPeptide p = (PolyPeptide) m_PSTemplate.Members[i];
						m_TemplateChainID = p.ChainID;
						m_TemplateSequence = p.MonomerString;
						return;
					}
				}
				throw new BuilderException("ParticleSystem contains no polypeptides!");
			}
		}
		public ParticleSystem TemplateSystem
		{
			get
			{
				return m_PSTemplate;
			}
			set
			{
				m_PSTemplate = value;
				InitialiseCurrentTemplate();
			}
		}

		public ParticleSystem ModelSystem
		{
			get
			{
				return m_PSModel;
			}
		}

		public char currentTemplateChainID
		{
			get
			{
				return m_TemplateChainID;
			}
		}


		#region Rebuilding Commands

		public bool RebuildTemplate( RebuildMode mode, bool keepUndefinedExtraAtoms, bool keepOutsideChainHetatmsAndSolvent, bool doErrorOnMissingAtomsOtherThanHydrogen, bool doErrorOnUnknownResidue, bool removeMoleculesWithLessThanThreeAtoms )
		{
			if ( m_PSTemplate == null )
			{ 
				throw new BuilderException("Template Particle System is null");
			}
            return Rebuild( m_PSTemplate, mode, keepUndefinedExtraAtoms, keepOutsideChainHetatmsAndSolvent, doErrorOnMissingAtomsOtherThanHydrogen, doErrorOnUnknownResidue, removeMoleculesWithLessThanThreeAtoms );
		}

		public bool Rebuild( ParticleSystem ps, RebuildMode mode, bool keepUndefinedExtraAtoms, bool keepOutsideChainHetatmsAndSolvent, bool doErrorOnMissingAtomsOtherThanHydrogen, bool doErrorOnUnknownResidue, bool removeMoleculesWithLessThanThreeAtoms )
		{
			bool errorFlagged = false;

			Trace.WriteLine( "Rebuilder initiated for : " + ps.Name );
			if ( ps == null )
			{ 
				throw new BuilderException("ParticleSystem given to PS Rebuilder is null");
			}

			bool done = false;
			while (!done)
			{
				try
				{
					ps.AcquireWriterLock( 500 );
					done = true;
				}
				catch
				{
				}
			}

			Trace.WriteLine( "PSBuilder entering ParticleSystem editing mode..." );
			Trace.Indent();

			ps.BeginEditing();

			PSMolContainer[] molContainers = ps.Members;
			for( int i = 0; i < molContainers.Length; i++ )
			{
				PSMolContainer psMolCont = molContainers[i];
				if( !(psMolCont is Solvent) && !(psMolCont is HetMolecules) )
				{
					int j = 0;
					while( j < psMolCont.Count )
					{
						if( removeMoleculesWithLessThanThreeAtoms && psMolCont[j].Count < 3 )
						{
							// rebuild impossible, delete the residue
							Trace.WriteLine("WARNING: Removed molecule for having insufficient rebuild atoms");
							psMolCont.RemoveMolAt(j);
							continue;
						}
						else if( !Rebuild( psMolCont[j], mode, keepUndefinedExtraAtoms, doErrorOnMissingAtomsOtherThanHydrogen, doErrorOnUnknownResidue ) )
						{
							errorFlagged = true;
						}	
						j++;
					}
				}
				else if( !keepOutsideChainHetatmsAndSolvent )
				{
					ps.RemoveMember( psMolCont );
				}
			}
	

			Trace.Unindent();
			Trace.WriteLine( "Leaving ParticleSystem editing mode" );

			ps.EndEditing(true,true);

			ps.ReleaseWriterLock();

			return !errorFlagged;
		}


		// above rebuild must first be called for writer lock !
		// hence this is private
		private bool Rebuild( Molecule m, RebuildMode mode,  bool keepUndefinedExtraAtoms, bool doErrorOnMissingAtomsOtherThanHydrogen, bool doErrorOnUnknownResidue )
		{
			bool errorConditionInBuild = false; // overall error flag

			m_RBD.molecule = m;
			MoleculePrimitive mp = m.moleculePrimitive as MoleculePrimitive;
			if( mp == null )
			{
				// return false if we want an error condition i.e. doErrorOnUnknownResidue is true
				return !doErrorOnUnknownResidue;
			}

			// we are just going to use the rotamer as a template. The sidechain rotations in the template will be maintained
			// note that we need at least 3 bonded atoms to rebuild a residue : 0-1-2
			// if many atoms are missing, the residue will effectively be made into the rotamer

			// first step is to identify missing atoms and the 2 atoms that are required to place them
			m_RebuildAtomHolder.Clear(); // will hold the primitives for atoms that we need to build in


			if( m.Count < 3 )
			{
				Trace.WriteLine( "REBUILD FAIL : Molecule : " + m.ToString() + ". Not enough atoms are present for rebuild. Atom count : " + m.Count.ToString() );
				return false;
			}

			// # there is a section in the rotamer file that defines the follwing for all FF types
			// # This is used to define which atom types the rebuilder will include in rebuild operations
			// # 0 = Any other atoms - i.e. the aliphatic hydrogens
			// # 1 = Heavy Atoms Polar Hydrogens AromaticHydrogens
			// # 2 = Heavy Atoms Polar Hydrogens
			// # 3 = Heavy Atoms
			// # Types are given the highest number they can have ...

			int RequiredBuilderLevel = (int) mode;

			//Trace.WriteLine("Builder level : " + RequiredBuilderLevel.ToString() );


			for( int i = 0; i < mp.AtomPrimitiveCount; i++ )
			{
				AtomPrimitive ap = mp[i];
				// then there should be that atom in our moulecule, lets see if it is!
				// wea re going to rebuil all atoms even if they are not required in the end molecule.
				// this is because if we dont, there might be atoms that we do want, which depend on the reconstruction
				// of atoms that we dont want. there is no other way to build in these dependedents.
				bool present = false; // in the molecule
				for ( int j = 0; j < m.Count; j++ )
				{
					if ( m[j].atomPrimitive == ap )
					{
						present = true;
						break;
					}
				}
				if ( !present )
				{
					if( doErrorOnMissingAtomsOtherThanHydrogen && ap.Element != 'H' )
					{
						// check that each heavy atom is present here
						errorConditionInBuild = true;
					}
					m_RebuildAtomHolder.Add( ap );
				}
			}

			// no no no, if we quit here, then unwanted atoms are not removed in stage 3!
			//			if ( m_RebuildAtomHolder.Count == 0 ) 
			//			{
			//				return;
			//				// our work here is done ;-)
			//			}


            // if there are missing atoms, lets build them in from the rotamer

			// we will take the first bonding partners out of the bonding partner list
			// idealy positioning of replacement atoms should occur using heavy atoms as reference points
			// this is fine because the bond lists in the amber.ff file all start with heavy atoms
			// rather than hydrogens

			// truck through the list and remove the atom primitives from it as soon as the atom has been added
			// some primitives may not be able to be added until others have first, so the process may cycle many times
			int currentIndex = 0; // lets start with atomPrimitive 0

			// we need to check if further definitions have been added or if we have reached stalemate
			int previousHolderCount = m_RebuildAtomHolder.Count;

			while( m_RebuildAtomHolder.Count > 0 )
			{
				if ( currentIndex >= m_RebuildAtomHolder.Count )
				{
					// we have reached the end of the index, time to restart
					if( previousHolderCount == m_RebuildAtomHolder.Count )
					{
						// no improvements have been made since the last cycle, we have reached stalemate
						errorConditionInBuild = true; // not all atoms rebuilt!
						break;
					}
					else
					{
						// improvements were made, lets reset the counter
						previousHolderCount = m_RebuildAtomHolder.Count;
						currentIndex = 0;
					}
				}

				AtomPrimitive ap = (AtomPrimitive) m_RebuildAtomHolder[currentIndex];

				string[] bondingPartners = ap.BondingPartners;
				if( bondingPartners.Length == 0 )
				{
					errorConditionInBuild = true;
					Trace.WriteLine( "REBUILD FAIL : " + m.ToString() + " has no bonding partners, placement is therefore impossible" );
					m_RebuildAtomHolder.RemoveAt( currentIndex );
				}
				else if( m_RBD.RebuildAtom( ap ) )
				{
					m_RebuildAtomHolder.RemoveAt( currentIndex ); // we have round the required anchor atoms
					// no need to advance the counter for the next one as we have removed one
				}
				else
				{
					// atom rebuild failed for some reason, advance the conuter
					currentIndex++;
				}
			}

			// final step is to remove unwanted atoms
			// seems backwards, but this has to be done after the first step, because some of the atoms
			// that we need can only be positioned using the hydrogens, and therefore they have to be built
			// in first, and then removed now if we dont want them

			if( keepUndefinedExtraAtoms )
			{
				for ( int i = m.Count -1; i >= 0; i-- )
				{
					if( m[i].atomPrimitive.FFAtomType.AtomBuilderLevel < RequiredBuilderLevel )
					{
						m.RemoveAtom( m[i] );
					}
				}
			}
			else
			{
				for ( int i = m.Count -1; i >= 0; i-- )
				{
					if( !(m[i].atomPrimitive is AtomPrimitive) || m[i].atomPrimitive.FFAtomType.AtomBuilderLevel < RequiredBuilderLevel )
					{
						m.RemoveAtom( m[i] );
					}
				}
			}

			// we are now done, however, renumbering must now occur
			// all new atoms have atom numbers and indexes of -1

			return !errorConditionInBuild;
		}


		#endregion

		#region Building Commands

		public bool BuildModel( bool keepUndefinedExtraAtoms, bool keepOutsideChainHetatmsAndSolvent, bool doErrorOnUnknownResidue )
		{
			ParticleSystem ps =  (ParticleSystem) m_PSTemplate.Clone();
			ps.Name = "Model From: " + m_PSTemplate.Name;

			ps.BeginEditing();

			if( !keepOutsideChainHetatmsAndSolvent )
			{
				for( int i = 0; i < ps.Members.Length; i++ )
				{
					if( !(ps.Members[i] is PolyPeptide) )
					{
						ps.RemoveMember( i );
					}
				}
			}

			bool errorCondition = false;

			for( int i = 0; i < m_Selections.Count; i++ )
			{
				Selection s = (Selection)m_Selections[i];
				PolyPeptide pp = (PolyPeptide) ps.MemberWithID( s.ChainID );

				for ( int j = 0; j < s.templateLength; j++ )
				{
					if( m_TemplateSequence[ s.templateStart + j ] != m_ThreadSequence[ s.threadStart + j ] )
					{
						int threadStartPlusJ = s.threadStart + j;
						int templateStartPlusJ = s.templateStart + j; // the current template position that we are dealing with
						string name = null;
						char preFix = ' ';
						char moleculeID = m_ThreadSequence[ threadStartPlusJ ];

						// set the name and the prefix according to the new molecule name
						for( int q = 0; q < PDB.singleLetterAAs.Length; q++ )
						{
							if( PDB.singleLetterAAs[q] == moleculeID )
							{
								// is the Amino acid N or C terminal ?
								if( (templateStartPlusJ) == 0 )
								{
									name = PDB.threeLetterAminoAcids[q];
									preFix = 'N';
								}
								else if( (templateStartPlusJ) == (m_TemplateSequence.Length -1) )
								{
									name = PDB.threeLetterAminoAcids[q];
									preFix = 'C';
								}
								else
								{
									name = PDB.threeLetterAminoAcids[q];
								}
								break;
							}
						}
						if( name == null )
						{
							throw new BuilderException("Error : an unknown AminoAcid Signle letter code was given to the ChangeAminoAcid Function");
						}

						AminoAcid aa = (AminoAcid)pp[templateStartPlusJ];
						aa.ResetName( name, preFix, false );
						
						int g = 0;
						while( g < aa.Count )
						{
							if( !aa[g].atomPrimitive.IsBackBone || !aa.moleculePrimitive.ContainsAtomWithAltID( aa[g].ALTType ) )
							{
								aa.RemoveAtom( aa[g] );
							}
							else
							{
								g++;
							}
						}

						aa.setAtomPrimitives();

						// Rebuild requires that the molecule is properly parented 
						// and that the next and prev AAs are defined
						if( !Rebuild( aa, 
							RebuildMode.AllAtoms, 
							keepUndefinedExtraAtoms, 
							false, 
							doErrorOnUnknownResidue ) )
						{
							// we have kept the backbone, but we will now need to glue on the relevent sidechain
							errorCondition = true;
						}
					}
				}
			}

			ps.EndEditing(true,true);
			m_PSModel = ps;
			return !errorCondition;
		}

		private struct moleculeEnergyPair
		{
			public float energy;
			public int molIndex;
		}

		private class molEnergyPairComparer : IComparer
		{
			public molEnergyPairComparer()
			{
			}		

			public int Compare(object x, object y)
			{
				try
				{
					moleculeEnergyPair xMol = (moleculeEnergyPair)x;
					moleculeEnergyPair yMol = (moleculeEnergyPair)y;	
					if( xMol.energy > yMol.energy )
					{
						return -1;
					}
					else if( xMol.energy < yMol.energy )
					{
                        return 1;
					}
					else
					{
						return 0;
					}
				}
				catch
				{
					return -1;
				}
			}
		}


		private const int m_WorstChangeNum = 3; // the number of worst residues to exhaustively mutate per opt step
		public void OptimiseRotamers()
		{

			Trace.WriteLine("Initiating Rotamer Optimisation For The Model System");
			Trace.WriteLine("");
			Trace.Indent();

			// Optimise away ...
			// 1. Find worst 5 residues
			// 2. change their rotamers exhaustively and find best
			// 3. recurse until you cant improve
			// 4. find the worst 1 and mutate it and its neighbours recursively
			// done ?

			// Connect the forcefield to the model system
			Trace.WriteLine("Connecting the forcefield to the model system");

			m_Forcefield.connectToAtomList( m_PSModel );

			MoleculeList ChangeableMolecules = new MoleculeList( m_PSModel.Count / 10 ); // ish loading

			// first obtain a list of all the molecules that we have that can be changed to a different rotamer
			for( int i = 0; i < m_PSModel.Members.Length; i++ )
			{
				for( int j = 0; j < m_PSModel.Members[i].Count; j++ )
				{
					MoleculePrimitiveBase mpb = m_PSModel.Members[i][j].moleculePrimitive;
					if( mpb is MoleculePrimitive )
					{
						MoleculePrimitive mp = (MoleculePrimitive) mpb;
						if( mp.RotamerCount > 0 )
						{
							ChangeableMolecules.addMolecule( m_PSModel.Members[i][j] );
						}
					}
				}
			}

			Trace.WriteLine(ChangeableMolecules.Count.ToString() + " molecules submitted for rotamer mutation");

			// we now need to find the worst <-WorstCount-> molecules and exhaustively search their rotamers
			
			ArrayList moleculeScores = new ArrayList( ChangeableMolecules.Count );
			m_Forcefield.calcDistanceMatrix();

			float[] atomStericEnergies = m_Forcefield.calcSoftSterics();
			// CODE NOTE : this should ignore 1:2 and 1:3 bonds and 1/2 ignore 1:4 bonds

			for( int i = 0; i < ChangeableMolecules.Count; i++ )
			{
				moleculeEnergyPair molEPair = new moleculeEnergyPair();
				molEPair.molIndex = i;
				for( int j = 0; j < ChangeableMolecules[i].Count; j++ )
				{
					molEPair.energy += atomStericEnergies[ ChangeableMolecules[i][j].ArrayIndex ];       
				}
                moleculeScores.Add( molEPair );
			}

			moleculeScores.Sort( new molEnergyPairComparer() );

//			for( int i = 0; i < worstCount; i++ )
//			{
//				molEPair = 
//
//			}

			Trace.WriteLine("");
			Trace.Unindent();
			Trace.WriteLine("Rotamer Optimisation Complete");


		}

		public void ApplyRotamer( int RotamerID, char chainID, int residueIndex )
		{
			bool done = false;
			while (!done)
			{
				try
				{
					m_PSModel.AcquireWriterLock( 1000 );
					done = true;
				}
				catch
				{
				}
			}

			// Safe to write to the system

			PolyPeptide p;

			try
			{
				p = (PolyPeptide) m_PSModel.MemberWithID( chainID );
			}
			catch
			{
				throw new BuilderException( "The chain is not a polypeptide!");
			}

			if ( p == null )
			{
				throw new BuilderException( "That chain ID was not found in the particle system");
			}

			if ( residueIndex < 0 )
			{
				throw new BuilderException( "A residue index cant be less than 0");
			}

			if ( residueIndex >= p.Count )
			{
				throw new BuilderException( "ResidueIndex is not present in the polyPeptide - it isnt that long");
			}

			// all looks OK, begin editing

			AminoAcid a = (AminoAcid) p[residueIndex];
			m_RotApplier.molecule = a;
			m_RotApplier.ApplyRotamer(RotamerID);

			// Writing done
			m_PSModel.ReleaseWriterLock();
		}

		public void SetCurrentChainID( char findChainID )
		{
			if ( m_PSTemplate != null )
			{
				for( int i = 0; i < m_PSTemplate.Members.Length; i++ )
				{
					if( m_PSTemplate.Members[i] is PolyPeptide )
					{
						PolyPeptide p = (PolyPeptide) m_PSTemplate.Members[i];

						if( p.ChainID == findChainID )
						{
							m_TemplateChainID = p.ChainID;
							m_TemplateSequence = p.MonomerString;
							return;
						}
					}
				}
				throw new BuilderException("The chainID could not be found in the template");
			}
			else
			{
				throw new BuilderException("No template particle system is defined");
			}
		}

		public void RemoveSelection( int index )
		{
			try
			{
				m_Selections.RemoveAt( index );
			}
			catch
			{
				throw new BuilderException("Selection deletion failed at index : " + index.ToString() );
			}
		}

		public void RemoveSelection( int[] index )
		{
			ArrayList indexes = new ArrayList( index.Length );
			indexes.AddRange( index );
			indexes.Sort();
			for( int i = index.Length -1; i >= 0; i-- )
			{
				m_Selections.RemoveAt( (int) indexes[i] );
			}
		}

		public void GetCurrentSequenceInfo( out char chainID, out string templateMonomerList, out string threadMonomerList )
		{
			threadMonomerList = m_ThreadSequence;
			templateMonomerList = m_TemplateSequence;
			chainID = m_TemplateChainID;            
		}

		public string SequenceFollowingBuild
		{
			get
			{
				char[] resultant = m_TemplateSequence.ToCharArray();

				for( int i = 0; i < m_Selections.Count; i++ )
				{
					Selection s = (Selection) m_Selections[i];
					if( s.ChainID != m_TemplateChainID ) // the chainID that we are viewing is not influenced by this selection
					{
						continue;
					}
					for( int j = 0; j < s.templateLength; j++ )
					{
						resultant[ s.templateStart + j ] = m_ThreadSequence[ s.threadStart + j ];
					}
				}

				return new string( resultant );
			}
		}

		public string ValidateNewThreadString( ref string validateString, SequenceInputTypes type )
		{
			m_UndoValidate = m_ThreadSequence; // so that we can restore it if things go pear shaped
			m_Selections.Clear(); // if the thread sequence is changed, the selections will no longer be valid

			StringBuilder bin = new StringBuilder();
			StringBuilder keep = new StringBuilder();

			switch( type )
			{
				case SequenceInputTypes.SingleLetter:

					for ( int i = 0; i < validateString.Length; i++ )
					{
						bool found = false;
						for( int j = 0; j < PDB.singleLetterAAs.Length; j++ )
						{
							if ( PDB.singleLetterAAs[j] == validateString[i] )
							{
								found = true;
							}
						}
						if ( found )
						{
							keep.Append( validateString[i] );
						}
						else
						{
							bin.Append( validateString[i] );
						}
					}
					m_ThreadSequence = keep.ToString();
					return keep.Length + " AminoAcids were found. " + bin.Length.ToString() + " Characters were removed.\r\n" + "Removed Characters : " + bin.ToString();


				case SequenceInputTypes.ThreeLetter:

					string[] elements = m_Regex.Split( validateString );
					for( int i = 0; i < elements.Length; i++ )
					{
						bool found = false;
						for( int j = 0; j < PDB.threeLetterAminoAcids.Length; j++ )
						{
							if( elements[i] == PDB.threeLetterAminoAcids[j] )
							{
								keep.Append( PDB.singleLetterAAs[j] );
								found = true;
								break;
							}
						}
						if(!found)
						{
							bin.Append( elements[i] );
						}
					}

					m_ThreadSequence = keep.ToString();
					return keep.Length + " AminoAcids were found. " + bin.Length.ToString() + " Characters were removed.\r\n" + "Removed Characters : " + bin.ToString();

				case SequenceInputTypes.FASTA:
                    throw new NotSupportedException("FASTA support is not yet implemented");

				default:
                    throw new NotSupportedException("PS_Builder is not awear of this class of sequence ...");
			}
		}

		public void UndoThreadSequenceChange()
		{
			m_ThreadSequence = m_UndoValidate;
		}

		public void NewSelection( int templateResidueStart, int templateResidueEnd, int threadResidueStart, int threadResidueEnd )
		{
			Selection s = new Selection( m_TemplateChainID, templateResidueStart, templateResidueEnd, threadResidueStart, threadResidueEnd);

			ValidateSelection( s );
			m_Selections.Add( s );
		}

		public void ClearSelections()
		{
			m_Selections.Clear();
		}

		public string[] SelectionsStrings
		{
			get
			{
				string[] returnStrings = new string[ m_Selections.Count ];
				for( int i = 0; i < m_Selections.Count; i++ )
				{
					returnStrings[i] = ((Selection) m_Selections[i] ).ToString();
				}
				return returnStrings;
			}
		}
        		
		#endregion

		#region building functions

		private char[] getAvailableChainIDs()
		{
			ArrayList ar = new ArrayList();
			for( int i = 0; i < m_PSTemplate.Members.Length; i++ )
			{
				if( m_PSTemplate.Members[i] is PolyPeptide )
				{
					PolyPeptide p = (PolyPeptide) m_PSTemplate.Members[i];
					ar.Add( p.ChainID );
				}
			}
			return (char[]) ar.ToArray( typeof(char) );
		}

		private bool ValidateSelection( Selection newSelection )
		{
			if( newSelection.templateLength != newSelection.threadLength )
			{
				throw new BuilderException("Replacement stretches must match in length");
			}

			if( newSelection.templateLength == 0
				|| newSelection.threadLength == 0 )
			{
				throw new BuilderException("Alignments must have a length greater than 0!");
			}

			if( newSelection.templateStart < 0 ||
				newSelection.templateLength < 0 ||
				newSelection.threadStart < 0 ||
				newSelection.threadLength < 0 )
			{
				throw new BuilderException("Selections for both template and thread must be made");
			}

			char[] availableChainIDs = getAvailableChainIDs();

			for( int i = 0; i < m_Selections.Count; i++ )
			{   // re-validate all current pairs in case the user has added a hand-typed one ...
				for( int j = i+1; j < m_Selections.Count; j++ )
				{
					Selection si = (Selection) m_Selections[i];
					Selection sj = (Selection) m_Selections[j];
					ValidateSelectionPair( si, sj, ref availableChainIDs );
				}
			}

			for( int i = 0; i < m_Selections.Count; i++ )
			{
				Selection selection = (Selection) m_Selections[i];
				ValidateSelectionPair( selection, newSelection, ref availableChainIDs ); 
			}
			return true;
		}

		private void ValidateSelectionPair( Selection sel1, Selection sel2, ref char[] availableChainIDs )
		{
			if ( sel1 == sel2 )
			{
				throw new BuilderException( "The two selections that just got compared are the same object - eh ?");
			}

			if ( !ArrayContainsChar( ref availableChainIDs, sel1.ChainID) )
			{
				throw new BuilderException( "Selection chainID is not valid for the template\r\n" + sel1.ToString() );
			}
			if ( !ArrayContainsChar( ref availableChainIDs, sel2.ChainID) )
			{
				throw new BuilderException( "Selection chainID is not valid for the template\r\n" + sel1.ToString() );
			}

			if ( sel1.ChainID != sel2.ChainID )
			{
				return; // if the CharIDs dont match then the strings must be compatible
			}

			// make initial sensibility checks
			if( sel1.templateStart >= sel1.templateEnd )
				throw new BuilderException("Selection 1's template start index is greater than the end index!\r\nSelection 1 : " + sel1.ToString());
			if( sel1.threadStart >= sel1.threadEnd )
				throw new BuilderException("Selection 1's thread start index is greater than the end index!\r\nSelection 1 : " + sel1.ToString());
			if( sel2.templateStart >= sel2.templateEnd )
				throw new BuilderException("Selection 2's template start index is greater than the end index!\r\nSelection 2 : " + sel2.ToString());
			if( sel2.threadStart >= sel2.threadEnd )
				throw new BuilderException("Selection 2's thread start index is greater than the end index!\r\nSelection 2 : " + sel2.ToString());

			// check that the selections are within the bounds of the sequences that we have for the template and the candidate
			int templateLength = m_TemplateSequence.Length;
			int threadLength = m_ThreadSequence.Length;

			if( sel1.templateStart < 0 || sel1.templateEnd >= templateLength )
				throw new BuilderException("Selection 1's template indexes are not within the bounds of the sequence!\r\nSelection 1 : " + sel1.ToString());
			if( sel1.threadStart < 0 || sel1.threadEnd >= threadLength )
				throw new BuilderException("Selection 1's thread indexes are not within the bounds of the sequence!\r\nSelection 1 : " + sel1.ToString());
			if( sel2.templateStart < 0 || sel2.templateEnd >= templateLength )
				throw new BuilderException("Selection 2's template indexes are not within the bounds of the sequence!\r\nSelection 2 : " + sel2.ToString());
			if( sel2.threadStart < 0 || sel2.threadEnd >= threadLength )
				throw new BuilderException("Selection 2's thread indexes are not within the bounds of the sequence!\r\nSelection 2 : " + sel2.ToString());

			// check that the selections dont overlap
			if ( sel1.templateStart <= sel2.templateStart && sel1.templateEnd >= sel2.templateStart )	
				throw new BuilderException("Template Selection 1 crossses the start position of selection 2 : \r\nSelection 1 : " + sel1.ToString() + "\r\nSelection 2 : " + sel2.ToString() );
			if ( sel1.templateStart <= sel2.templateEnd && sel1.templateEnd >= sel2.templateEnd )		
				throw new BuilderException("Template Selection 1 crosses the emd position of selection 2 : \r\nSelection 1 : " + sel1.ToString() + "\r\nSelection 2 : " + sel2.ToString() );
			if ( sel1.templateStart >= sel2.templateStart && sel1.templateEnd <= sel2.templateEnd )		
				throw new BuilderException("Template Selection 1 lies within selection 2 : \r\nSelection 1 : " + sel1.ToString() + "\r\nSelection 2 : " + sel2.ToString() );
			if ( sel2.templateStart >= sel1.templateStart && sel2.templateEnd <= sel1.templateEnd )		
				throw new BuilderException("Template Selection 2 lies within selection 1 : \r\nSelection 1 : " + sel1.ToString() + "\r\nSelection 2 : " + sel2.ToString() );

			if ( sel1.threadStart <= sel2.threadStart && sel1.threadEnd >= sel2.threadStart )	
				throw new BuilderException("Thread Selection 1 crossses the start position of selection 2 : \r\nSelection 1 : " + sel1.ToString() + "\r\nSelection 2 : " + sel2.ToString() );
			if ( sel1.threadStart <= sel2.threadEnd && sel1.threadEnd >= sel2.threadEnd )		
				throw new BuilderException("Thread Selection 1 crosses the emd position of selection 2 : \r\nSelection 1 : " + sel1.ToString() + "\r\nSelection 2 : " + sel2.ToString() );
			if ( sel1.threadStart >= sel2.threadStart && sel1.threadEnd <= sel2.threadEnd )		
				throw new BuilderException("Thread Selection 1 lies within selection 2 : \r\nSelection 1 : " + sel1.ToString() + "\r\nSelection 2 : " + sel2.ToString() );
			if ( sel2.threadStart >= sel1.threadStart && sel2.threadEnd <= sel1.threadEnd )		
				throw new BuilderException("Thread Selection 2 lies within selection 1 : \r\nSelection 1 : " + sel1.ToString() + "\r\nSelection 2 : " + sel2.ToString() );

			return; // my god, it passed all the tests ...
		}

		private bool ArrayContainsChar(ref char[] chars, char c)
		{
			for( int i = 0; i < chars.Length; i++ )
			{
				if ( chars[i] == c )
				{
					return true;
				}
			}
			return false;
		}

		
		#endregion
	}
}
