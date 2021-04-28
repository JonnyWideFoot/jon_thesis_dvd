using System;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Diagnostics;

using UoB.Core.Primitives;
using UoB.Core.Structure;
using UoB.Core.FileIO.PDB;
using UoB.Core.FileIO.Tra;

namespace UoB.Core.Structure.Constraints
{
	/// <summary>
	/// Summary description for ConstraintList.
	/// </summary>
	public sealed class ConstraintList
	{
		private ArrayList m_ConstraintGroups;
		private Tra m_Tra = null;
		private ParticleSystem m_PS = null;
		private Regex m_SplitLine;

		public ConstraintList()
		{
			m_ConstraintGroups = new ArrayList();
			m_PS = null;
			m_SplitLine = UoB.Core.Tools.CommonTools.WhiteSpaceRegex;
		}

		public ParticleSystem particleSystem
		{
			get
			{
				return m_PS;
			}
		}

		public ConstraintGroup this[ int index ]
		{
			get
			{
				return GetGroup( index );
			}
		}

		public int ConGrpCount
		{
			get
			{
				return m_ConstraintGroups.Count;
			}
		}

		public void ResetAll()
		{
			m_PS = null;
			for( int i = 0; i < m_ConstraintGroups.Count; i++ )
			{
				ConstraintGroup cg = (ConstraintGroup) m_ConstraintGroups[i];
				cg.Reset(); // it now contains nothing
			}
		}

		public ConstraintGroup GetGroup( int groupID )
		{
			if( groupID >= m_ConstraintGroups.Count )
			{
				for( int i = m_ConstraintGroups.Count; i < groupID + 1; i++ )
				{
					m_ConstraintGroups.Add( new ConstraintGroup( i, 0 ) );
				}
			}
			return (ConstraintGroup)m_ConstraintGroups[groupID];
		}


		#region file IO

		public void WriteToFile( string fileName )
		{
			StreamWriter rw = new StreamWriter( fileName );
			rw.WriteLine("CUSTOMFORCELIST");
			for( int i = 0; i < m_ConstraintGroups.Count; i++ )
			{
				if( m_ConstraintGroups[i] != null )
				{
					ConstraintGroup cg = (ConstraintGroup) m_ConstraintGroups[i];
					cg.WriteConstraints( rw );
				}
			}
			rw.WriteLine("ENDCUSTOMFORCELIST");
			rw.Close();
		}

		public void ReloadFromFile( string conFileName, Tra traFile, bool ignoreNonCA )
		{
			// NOTE IM NOT IMPLEMENTING CONSTRAINT GROUPS HERE !!!
			ResetAll(); // clear the ContraintGroup arrays to 0 entries without deleting the internal classes

			m_Tra = traFile;
			m_PS = traFile.particleSystem;

			StreamReader re = new StreamReader( conFileName );
			string line;

			// ASSERT - "CUSTOMFORCELIST"
			bool foundStart = false;
			while( null != ( line = re.ReadLine() ) )
			{
				if( line.Length != 15 )
				{
					continue;
				}
				else
				{
					if( line == "CUSTOMFORCELIST" )
					{
						foundStart = true;
						break;
					}
				}
			}
			if( !foundStart )
			{
				throw new Exception("Invalid or corrupt constraint file, no start definition was found");
			}

			// sample lines ...
			// 80  CA  67  CA  V 4.906268 100 0.1 0.1
			// 67  CA  80  CA  B 4.906268 -3 2

			// Harminic = H
			// Bell-Shaped = B
			// V-Shaped = V

			while( true )
			{
				line = re.ReadLine();
				if( line == null ) goto INCORRECT_TERMIATION;
				if( line.Length == 0 ) continue;
				if( line[0] == '#' ) // could be a comment of a CG-set definition
				{
					if( line.Length < 4 ) continue;
					// test for "#B S" as a begin statement
					if( line[1] == 'B' && line[2] == ' ' )
					{
						// we have a valid multi type def
						switch( line[3] )
						{
							case 'S':
								line = re.ReadLine();
								string line2 = re.ReadLine();
								string termCheck = re.ReadLine();
								if( termCheck != "#E S" ) throw new Exception("Invalid SymetricVShaped termination definition found : " + termCheck );
								ParseSymetricVShaped( line, line2, ignoreNonCA );
								break;
							default:
								throw new Exception("Invalid multi-conponent-potential definition found" );
						}
					}
					else
					{
						// its a comment line, therefore ignore the contents
					}
				}
				else
				{
					// parse the line as normal single def or as a termination def
					if( line.Length == 18 && line == "ENDCUSTOMFORCELIST" ) 
					{
						goto CORRECT_TERMIATION;
					}
					else
					{
						ParseSingleLine( line, ignoreNonCA );
					}
				}
			}
			INCORRECT_TERMIATION:
				re.Close(); // still close the file handle
			throw new Exception("Invalid or corrupt constraint file, no proper termination of definitions, \"ENDCUSTOMFORCELIST\" rrequired ...");
			CORRECT_TERMIATION:
				re.Close();
		}
	
		private void ParseSymetricVShaped( string line1, string line2, bool ignoreNonCA )
		{
			//	#B S
			//	0  CA  1  CA  V 3.876399 0.64 0.1 1.0
			//	0  CA  1  CA  V 3.876399 0.64 0.1 -1.0
			//	#E S
			string[] lineParts1 = m_SplitLine.Split( line1 );
			string[] lineParts2 = m_SplitLine.Split( line2 );
			if( lineParts1.Length != 9 )
			{
				throw new Exception("Invalid SymetricVShaped Line1 definition found, not enough elements : " + line1 );
			}
			if( lineParts2.Length != 9 )
			{
				throw new Exception("Invalid SymetricVShaped Line2 definition found, not enough elements : " + line2 );
			}
			try
			{
				int resID1L1 = int.Parse( lineParts1[0] );
				PDB.AttemptCorrentIDFormat( ref lineParts1[1] );
				string atomID1L1 = lineParts1[1];
				int resID2L1 = int.Parse( lineParts1[2] );
				PDB.AttemptCorrentIDFormat( ref lineParts1[3] );
				string atomID2L1 = lineParts1[3];

				int resID1L2 = int.Parse( lineParts2[0] );
				PDB.AttemptCorrentIDFormat( ref lineParts2[1] );
				string atomID1L2 = lineParts2[1];
				int resID2L2 = int.Parse( lineParts2[2] );
				PDB.AttemptCorrentIDFormat( ref lineParts2[3] );
				string atomID2L2 = lineParts2[3];

				if( resID1L1 != resID1L2 || atomID1L1 != atomID1L2 || resID2L1 != resID2L2 || atomID2L1 != atomID2L2 )
				{
					throw new Exception("The atom pairing definitions for a symetric potential definition do not match." );
				}

				if( lineParts1[4].Length != 1 || lineParts2[4].Length != 1 || lineParts1[4][0] != 'V' || lineParts2[4][0] != 'V' )
				{
					throw new Exception("Component potential types do not match the global master type 'S'" );
				}

				if( ignoreNonCA && atomID1L1 != " CA " || atomID1L2 != " CA " )
				{
					return;
				}

				float tDistL1 = float.Parse( lineParts1[5] );
				float epsilonL1 = float.Parse( lineParts1[6] );
				float gammaL1 = float.Parse( lineParts1[7] );
				float betaL1 = float.Parse( lineParts1[8] );

				float tDistL2 = float.Parse( lineParts2[5] );
				float epsilonL2 = float.Parse( lineParts2[6] );
				float gammaL2 = float.Parse( lineParts2[7] );
				float betaL2 = float.Parse( lineParts2[8] );

				if( epsilonL1 != epsilonL2 || gammaL1 != gammaL2 )
				{
					throw new Exception("Given parameters are not symmetric" );
				}

				Atom atom1 = m_PS.GetAtom( 0, resID1L1, atomID1L1 );
				Atom atom2 = m_PS.GetAtom( 0, resID2L1, atomID2L1 );

				ConstraintGroup cg = GetGroup( 0 );
				Constraint c = cg.getNextItemForInit();
				c.SetTo(atom1,atom2, PotentialType.Symmetric_VShaped, ( tDistL1 + tDistL2 ) / 2.0f,epsilonL1, gammaL1, ( tDistL1 - tDistL2 ) / 2.0f );               
			}
			catch( Exception ex )
			{
				throw new Exception("Invalid SymetricVShaped definition found : Line1 was \r\n " + line1 + "\r\nLine2 was \r\n " + line2 + "\r\n" + ex.Message );
			}
		}

		private void ParseSingleLine( string line, bool ignoreNonCA )
		{
			string[] lineParts = m_SplitLine.Split( line );
			if( lineParts.Length < 6 )
			{
				return; // shit line, but continue
			}

			try
			{
				int resID1;
				try
				{
					resID1 = int.Parse( lineParts[0] );
				}
				catch
				{
					Trace.WriteLine("Failure parsing constraint line due to invalid residue index 1 in line : ");
					Trace.WriteLine(line);
					return;
				}
				PDB.AttemptCorrentIDFormat( ref lineParts[1] );
				string pdbID1 = lineParts[1];

				int resID2;
				try
				{
					resID2 = int.Parse( lineParts[2] );
				}
				catch
				{
					Trace.WriteLine("Failure parsing constraint line due to invalid residue index 2 in line : ");
					Trace.WriteLine(line);
					return;
				}
				PDB.AttemptCorrentIDFormat( ref lineParts[3] );
				string pdbID2 =lineParts[3];

				if( ignoreNonCA && pdbID1 != " CA " || pdbID2 != " CA " )
				{
					return;
				}

				if( lineParts[4].Length != 1 ) // its a char
				{
					Trace.WriteLine("Failure parsing constraint line due to invalid potential-type ID in line : ");
					Trace.WriteLine(line);
					return; // crap line, but continue to next line
				}
				char potentialID = lineParts[4][0];

				float idealD;
				try
				{
					idealD = float.Parse( lineParts[5] );
				}
				catch
				{
					Trace.WriteLine("Failure parsing constraint line due to invalid ideal distance definition : ");
					Trace.WriteLine(line);
					return;
				}

				// lets try to get our atoms
				Atom atom1 = m_PS.GetAtom( 0, resID1, pdbID1 );
				Atom atom2 = m_PS.GetAtom( 0, resID2, pdbID2 );

				// get our ConGroup and the next constraint to be defined
				ConstraintGroup cg = GetGroup( 0 );
				Constraint c = cg.getNextItemForInit();

				float epsilon;
				float gamma;
				float beta;	
				
				switch( potentialID )
				{
					case 'V':  // V-Shaped Well Type (one side of the V)
						// takes 3 parameters 
					
						try
						{
							epsilon = float.Parse( lineParts[6] );
						}
						catch
						{
							Trace.WriteLine("Failure parsing v-shaped constraint line due to invalid epsilon definition : ");
							Trace.WriteLine(line);
							return;
						}

						try
						{
							gamma = float.Parse( lineParts[7] );
						}
						catch
						{
							Trace.WriteLine("Failure parsing v-shaped constraint line due to invalid gamma definition : ");
							Trace.WriteLine(line);
							return;
						}

						try
						{

							beta = float.Parse( lineParts[8] );
							if( beta != -1.0f && beta != 1.0f )
							{
								throw new Exception("Beta term on VShaped potential muxt be +1.0 or -1.0");
							}
						}
						catch
						{
							Trace.WriteLine("Failure parsing v-shaped constraint line due to invalid beta definition (Must be +1.0 or -1.0) : ");
							Trace.WriteLine(line);
							return;
						}

						c.SetTo(atom1, atom2, PotentialType.VShaped, idealD, epsilon, gamma, beta);

						break;
					case 'B': // bell-shaped

						try
						{
							epsilon = float.Parse( lineParts[6] );
						}
						catch
						{
							Trace.WriteLine("Failure parsing bellshaped constraint line due to invalid epsilon definition : ");
							Trace.WriteLine(line);
							return;
						}

						try
						{
							gamma = float.Parse( lineParts[7] );
						}
						catch
						{
							Trace.WriteLine("Failure parsing bellshaped constraint line due to invalid gamma definition : ");
							Trace.WriteLine(line);
							return;
						}

						c.SetTo(atom1, atom2, PotentialType.BellShaped, idealD, epsilon,gamma );

						break;
					case 'H':// Harmonic
						// single extra parameter epsilon
						
						try
						{
							epsilon = float.Parse( lineParts[6] );
						}
						catch
						{
							Trace.WriteLine("Failure parsing harmonic constraint line due to invalid epsilon definition : ");
							Trace.WriteLine(line);
							return;
						}

						c.SetTo(atom1, atom2, PotentialType.Harmonic, idealD, epsilon);

						break;
					default:
						Trace.Write("Failure parsing constraint line due to invalid potential-type ID : ");
						Trace.WriteLine(potentialID);
						return;
				}

				return; // cool
			}
			catch( Exception e )
			{
				Trace.Write("Failure parsing constraint line : ");
				Trace.WriteLine(line);
				Trace.Write("Exception : ");
				Trace.WriteLine(e.Message);
				return; // crap line, but continue to next line
			}
		}


		#endregion

		#region restraint addition

		public void SetGlobalRestraintsAs( ParticleSystem ps, ConstraintMode mode, bool addTopologyChiralityCheck, float epsilon, float gamma, float wellWidth )
		{
			ResetAll();

			m_PS = ps;
			ConstraintGroup cg = GetGroup( 0 );
			Constraint c = null; // assigned below at each assignment

			PolyPeptide pp = m_PS.MemberAt(0) as PolyPeptide;
			if( m_PS.Members.Length != 1 || pp == null )
			{
				throw new Exception("This function call is intended for use on particle systems that contain one polypeptide chain.");
			}
			int total = pp.Count;

			Atom aI = null;
			Atom aJ = null;
			AminoAcid aaI = null;
			AminoAcid aaJ = null;
			Atom aI_CA = null;
			Atom aJ_CA = null;
			Atom aI_C = null;
			Atom aJ_C = null;

			if( addTopologyChiralityCheck )
			{
				if( mode == ConstraintMode.UberAbsolute )
				{
					for( int i = 0; i < m_PS.Count; i++ )
					{
						aI = m_PS[i];
						if(aI.atomPrimitive.Element == 'H') continue;
						c = cg.getNextItemForInit();
						c.SetTo( aI, new Position( aI ), PotentialType.Symmetric_VShaped, 0.0f, epsilon, gamma, wellWidth );
					}
				}
				else
				{
					for( int i = 0; i < total; i++ )
					{
						aI = pp[i].CAlphaAtom;
						c = cg.getNextItemForInit();
						c.SetTo( aI, new Position( aI ), PotentialType.Symmetric_VShaped, 0.0f, 5.0f, 0.4f, 4.0f );
						aI = pp[i].NTerminalAtom;
						c = cg.getNextItemForInit();
						c.SetTo( aI, new Position( aI ), PotentialType.Symmetric_VShaped, 0.0f, 5.0f, 0.4f, 4.0f );
					}
				}

//				AminoAcid aa1 = (AminoAcid) pp[0];
//				AminoAcid aa2 = ParticleSystem.GetFurthestAA( pp, aa1 );
//				AminoAcid aa3 = ParticleSystem.GetFurthestAA( pp, aa1, aa2 );
//				AminoAcid aa4 = ParticleSystem.GetFurthestAA( pp, aa1, aa2, aa3 );
//
//				Atom pos = aa1.CAlphaAtom;
//				c = cg.getNextItemForInit();
//				c.SetTo( pos, new Position( pos ), PotentialType.Symmetric_VShaped, 0.0f, 500.0f, 1.0f, 2.0f );
//				pos = aa2.CAlphaAtom;
//				c = cg.getNextItemForInit();
//				c.SetTo( pos, new Position( pos ), PotentialType.Symmetric_VShaped, 0.0f, 500.0f, 1.0f, 2.0f );
//				pos = aa3.CAlphaAtom;
//				c = cg.getNextItemForInit();
//				c.SetTo( pos, new Position( pos ), PotentialType.Symmetric_VShaped, 0.0f, 500.0f, 1.0f, 2.0f );
//				pos = aa4.CAlphaAtom;
//				c = cg.getNextItemForInit();
//				c.SetTo( pos, new Position( pos ), PotentialType.Symmetric_VShaped, 0.0f, 500.0f, 1.0f, 2.0f );
			}

			switch( mode )
			{
				case ConstraintMode.CaOAllvAll:

					for( int i = 0; i < total; i++ )
					{
						aaI = pp[i];
						aI_CA = aaI.CAlphaAtom;
						aI_C = aaI.AtomOfType(" C  ");
						
						// constrain to the backbone in the relevent areas ...
						for( int j = i + 1; j < total; j++ ) // j=i+1 as there is no point in constraining within the same AA
						{
							aaJ = pp[j];
							aJ_CA = aaJ.CAlphaAtom;
							aJ_C = aaJ.AtomOfType(" C  ");

							c = cg.getNextItemForInit();
							c.SetTo( aI_CA, aJ_CA, PotentialType.Symmetric_VShaped, aI_CA.distanceTo(aJ_CA), epsilon, gamma, wellWidth );
							c = cg.getNextItemForInit();
							c.SetTo( aI_C, aJ_C, PotentialType.Symmetric_VShaped, aI_C.distanceTo(aJ_C), epsilon, gamma, wellWidth );
							c = cg.getNextItemForInit();
							c.SetTo( aI_C, aJ_CA, PotentialType.Symmetric_VShaped, aI_C.distanceTo(aJ_CA), epsilon, gamma, wellWidth );
							c = cg.getNextItemForInit();
							c.SetTo( aI_CA, aJ_C, PotentialType.Symmetric_VShaped, aI_CA.distanceTo(aJ_C), epsilon, gamma, wellWidth );
						}
					}

					break;

				case ConstraintMode.UberAbsolute:

					// done above ...

					break;
				case ConstraintMode.CaOAllvAllLocalSidechain:

					for( int i = 0; i < total; i++ )
					{
						aaI = pp[i];
						aI_CA = aaI.CAlphaAtom;
						aI_C = aaI.AtomOfType(" C  ");

						// constrain the sidechain to other atoms within the amino acid only
						for( int k = 0; k < aaI.Count; k++ )
						{
							aI = aaI[k];
							if( aI.atomPrimitive.Element == 'H' ) continue; // ignore non-Heavy-Atom

							for( int m = k + 1; m < aaI.Count; m++ ) // constrain to those higher in number, the list should have been sorted into the correct order by the molecule primitive
							{
								aJ = aaI[m]; // using aJ to count atoms in aaI, i know it sounds bad ...
								if( aJ.atomPrimitive.Element == 'H' || aI.bondedTo( aJ ) ) continue; // ignore non-Heavy-Atom and atoms that are bonded, these distances are rotation independent

								// only constrain now
								c = cg.getNextItemForInit();
								c.SetTo( aI, aJ, PotentialType.Symmetric_VShaped, aI.distanceTo(aJ), 1.0f, 0.4f, 1.0f );
							}
						}
						
						// constrain the backbone
						for( int j = i + 1; j < total; j++ ) // there is no point in constraining within the same AA, hence + 1
						{
							aaJ = pp[j];
							aJ_CA = aaJ.CAlphaAtom;
							aJ_C = aaJ.AtomOfType(" C  ");

							c = cg.getNextItemForInit();
							c.SetTo( aI_CA, aJ_CA, PotentialType.Symmetric_VShaped, aI_CA.distanceTo(aJ_CA), epsilon, gamma, wellWidth );
							c = cg.getNextItemForInit();
							c.SetTo( aI_C, aJ_C, PotentialType.Symmetric_VShaped, aI_C.distanceTo(aJ_C), epsilon, gamma, wellWidth );
							c = cg.getNextItemForInit();
							c.SetTo( aI_C, aJ_CA, PotentialType.Symmetric_VShaped, aI_C.distanceTo(aJ_CA), epsilon, gamma, wellWidth );
							c = cg.getNextItemForInit();
							c.SetTo( aI_CA, aJ_C, PotentialType.Symmetric_VShaped, aI_CA.distanceTo(aJ_C), epsilon, gamma, wellWidth );
						}
					}

					break;
				case ConstraintMode.BackbonePlusBetaCarbon:
					for( int i = 0; i < m_PS.Count; i++ )
					{
						aI = m_PS[i];
						if( aI.atomPrimitive.IsBackBone || aI.PDBType == " CB " )
						{
							for( int j = i + 1; j < m_PS.Count; j++ )
							{
								aJ = m_PS[j];
								if( aI == aJ || (!aJ.atomPrimitive.IsBackBone && !(aI.PDBType == " CB ") ) ) continue;
								c = cg.getNextItemForInit();
								c.SetTo( aI, aJ, PotentialType.Symmetric_VShaped, aJ.distanceTo(aI), epsilon, gamma, wellWidth );
							}
						}
					}
					break;
				case ConstraintMode.BackboneOnly:
					for( int i = 0; i < m_PS.Count; i++ )
					{
						aI = m_PS[i];
						if( !aI.atomPrimitive.IsBackBone ) continue;
						for( int j = i + 1; j < m_PS.Count; j++ )
						{
							aJ = m_PS[j];
							if( aI == aJ || !aJ.atomPrimitive.IsBackBone ) continue;
							c = cg.getNextItemForInit();
							c.SetTo( aI, aJ, PotentialType.Symmetric_VShaped, aJ.distanceTo(aI), epsilon, gamma, wellWidth );
						}
					}
					break;
				case ConstraintMode.HeavyAtomOnly:
					for( int i = 0; i < m_PS.Count; i++ )
					{
						aI = m_PS[i];
						if( aI.atomPrimitive.Element == 'H' ) continue;
						for( int j = i + 1; j < m_PS.Count; j++ )
						{
							aJ = m_PS[j];
							if( aJ.atomPrimitive.Element == 'H' ) continue;
							c = cg.getNextItemForInit();
							c.SetTo( aI, aJ, PotentialType.Symmetric_VShaped, aJ.distanceTo(aI), epsilon, gamma, wellWidth );
						}
					}
					break;
				case ConstraintMode.CAlpha:
					PSMolContainer mol = m_PS.Members[0];
					for( int i = 0; i < mol.Count; i++ )
					{
						Molecule mI = mol[i];
						aI = mI.AtomOfType( " CA " );
						for( int j = i + 1; j < mol.Count; j++ )
						{
							Molecule mJ = mol[j];
							aJ = mJ.AtomOfType( " CA " );
							c = cg.getNextItemForInit();
							c.SetTo( aI, aJ, PotentialType.Symmetric_VShaped, aJ.distanceTo(aI), epsilon, gamma, wellWidth );
						}
					}
					break;
				case ConstraintMode.AllVsAll:
					for( int i = 0; i < m_PS.Count; i++ )
					{
						aI = m_PS[i];
						for( int j = i + 1; j < m_PS.Count; j++ )
						{
							aJ = m_PS[j];
							c = cg.getNextItemForInit();
							c.SetTo( aI, aJ, PotentialType.Symmetric_VShaped, aJ.distanceTo(aI), epsilon, gamma, wellWidth );
						}
					}
					break;
				case ConstraintMode.SecondaryStructure:
					throw new Exception("ConstraintMode.SecondaryStructure not supported ...");
			}
		}

		private void AddExtenedRestraint( int molID, int start, int length )
		{
			//			PolyPeptide pp = null;
			//			try
			//			{
			//				pp = (PolyPeptide)mol;
			//			}
			//			catch
			//			{
			//				throw new Exception("Strand alignments are assumed to be for polypeptides, this isnt one...");
			//			}
			//
			//			Atom a1 = pp[start][0];
			//			Atom a2 = pp[start+length-1][0];
			//
			//			ConstrainAtomsRepulsive( conFile, a1, a2 );
		}

		private const float m_DeemedStrandCutoffSq = 50.0f;
		private void AddStrandPairRestraint( int molID, int startS1, int startS2, int strandLength )
		{
			PSMolContainer mol = m_PS.MemberAt( molID );
			int lengthMin1 = strandLength -1;

			PolyPeptide pp = null;
			try
			{
				pp = (PolyPeptide)mol;
			}
			catch
			{
				throw new Exception("Strand alignments are assumed to be for polypeptides, this isnt one...");
			}
			// first we need to detect the strand alignment from the distance matrix

			float d1 = Position.distanceSquaredBetween( pp[startS1].GetGeometricCenter(true), pp[startS2].GetGeometricCenter(true) );
			float d2 = Position.distanceSquaredBetween( pp[startS1].GetGeometricCenter(true), pp[startS2+lengthMin1].GetGeometricCenter(true) );
			float d3 = Position.distanceSquaredBetween( pp[startS1+lengthMin1].GetGeometricCenter(true), pp[startS2].GetGeometricCenter(true) );
			float d4 = Position.distanceSquaredBetween( pp[startS1+lengthMin1].GetGeometricCenter(true), pp[startS2+lengthMin1].GetGeometricCenter(true) );

			if( d1 < m_DeemedStrandCutoffSq && d4 < m_DeemedStrandCutoffSq )
			{
				// antiParallel = false
				throw new Exception("CODE NOT IMPLEMENTED - parallel sheets are not currently implemented");
			}
			else if ( d2 < m_DeemedStrandCutoffSq && d3 < m_DeemedStrandCutoffSq )
			{
				// antiParallel = true
				int s1Enum = startS1;
				int s2Enum = startS2 + lengthMin1;

				bool doingHBonds;
				Atom s1HBQuery = pp[s1Enum].AtomOfType(" O  ");
				Atom s2HBQuery = pp[s2Enum].AtomOfType(" H  ");

				if( Position.distanceSquaredBetween( s1HBQuery, s2HBQuery ) < 4.0f )
				{
					doingHBonds = true;
				}
				else
				{
					doingHBonds = false;
				}

				// lateral restraints to stop the strand pairings from "curling"

				AminoAcid aaS1Start = (AminoAcid)pp[startS1];
				AminoAcid aaS2Start = (AminoAcid)pp[startS2];
				AminoAcid aaS1End = (AminoAcid)pp[startS1 + lengthMin1];
				AminoAcid aaS2End = (AminoAcid)pp[startS2 + lengthMin1];
                    
				Atom aS1Start = aaS1Start.CAlphaAtom; 
				Atom aS2Start = aaS2Start.CAlphaAtom;
				Atom aS1End = aaS1End.CAlphaAtom; 
				Atom aS2End = aaS2End.CAlphaAtom;

				// diagonals
				ConstrainAtoms( aS1Start, aS2Start, Position.distanceBetween(aS1Start,aS2Start) );
				ConstrainAtoms( aS1End, aS2End, Position.distanceBetween(aS1End,aS2End) );
				// length struts
				ConstrainAtoms( aS1Start, aS1End, Position.distanceBetween(aS1Start,aS1End) );
				ConstrainAtoms( aS2Start, aS2End, Position.distanceBetween(aS2Start,aS2End) );
				
				// restrain strand pairings
				for( int i = 0; i < strandLength; i++ )
				{
					AminoAcid aaS1 = (AminoAcid)pp[s1Enum];
					AminoAcid aaS2 = (AminoAcid)pp[s2Enum];
                    
					Atom aS1 = aaS1.CAlphaAtom; 
					Atom aS2 = aaS2.CAlphaAtom;

					ConstrainAtoms( aS1, aS2, Position.distanceBetween(aS1,aS2) );

					if( doingHBonds )
					{
						aS1 = aaS1.AtomOfType(" O  ");
						aS2 = aaS2.AtomOfType(" H  ");
						ConstrainAtoms( aS1, aS2, Position.distanceBetween(aS1,aS2) );
						aS1 = aaS1.AtomOfType(" H  ");
						aS2 = aaS2.AtomOfType(" O  ");
						ConstrainAtoms( aS1, aS2, Position.distanceBetween(aS1,aS2) );
					}

					doingHBonds = !doingHBonds; // only alternate residues are h-bonded to each other
					s1Enum++;
					s2Enum--;
				}
			}
			else
			{
				throw new Exception("The residues provided do not seem to be very sheety!, distance parsing returned false.");
			}
		}


		private void ConstrainAtomsRepulsive( Atom atom1, Atom atom2 )
		{
			//			Constraint c1 = new Constraint_VShaped( atom1, atom2, 0.0f, 500.0f, 0.3f, false );
			//			// double symetric potential, the beta term is inverted for the second potential
			//			conFile.AddConstraint( 0, c1 );
			//
			//			Constraint c2 = new Constraint_VShaped( atom2, atom1, 0.0f, 500.0f, 0.3f, true );
			//			// double symetric potential, the beta term is inverted for the second potential
			//			conFile.AddConstraint( 0, c2 );			
		}
        
		private void ConstrainAtoms( Atom atom1, Atom atom2, float idealDistance )
		{
			//			Constraint c1 = new Constraint_SymetricVShaped( atom1, atom2, idealDistance, 100.0f, 0.1f, 0.1f );
			//			// double symetric potential, the beta term is inverted for the second potential
			//			conFile.AddConstraint( 0, c1 );
			//			Constraint c2 = new Constraint_BellShaped( atom1, atom2, idealDistance, -3.0f, 2.0f );
			//			conFile.AddConstraint( 0, c2 );
		}


		#endregion

		#region Accessors

		public Tra TraFile
		{
			get
			{
				return m_Tra;
			}
		}

		public int TotalConCount
		{
			get
			{
				int total = 0;
				int countTo = m_ConstraintGroups.Count;
				for( int i = 0; i < countTo; i++ )
				{
					ConstraintGroup cg = this[i];
					total += cg.CountTo;
				}
				return total;
			}
		}


		#endregion
	}
}
