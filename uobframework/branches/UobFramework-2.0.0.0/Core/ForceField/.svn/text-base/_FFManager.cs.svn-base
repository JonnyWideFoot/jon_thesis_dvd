using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

using UoB.Core;
using UoB.Core.Structure;
using UoB.Core.Structure.Primitives;
using UoB.Core.Primitives;
using UoB.Core.ForceField.Definitions;

namespace UoB.Core.ForceField
{
	/// <summary>
	/// Summary description for Params.
	/// </summary>
	public sealed class FFManager
	{
		// Singleton pattern - single initialisation for the application
		private static readonly FFManager instance = new FFManager();
		public static FFManager Instance
		{
			get 
			{
				return instance;
			}
		}
		// End Singleton

		private CoreIni m_CoreIni; // gets all the parameters set in the UoB.ini file in the program root directory
		private string m_ParamFilename;
		private Regex m_Regex;

		private ImagingDetails m_ImagingDetails;
		private MoleculePrimitiveList m_MoleculePrimitives;
		private Hashtable m_AliasList;
		private AtomTypeList m_AtomTypes;
		private BondDefList m_BondDefinitions;
		private AngleDefList m_AngleDefinitions;
		private TorsionDefList m_TorsionDefinitions;

		public string ParamSetName = "";
		public bool ParamSetIdentifierPresent = false;

		private float vdw14scaling = -1.0f;
		private float elec14scaling = -1.0f;
		//private bool m_Autoangles = false;
		//private bool m_Autotorsions = false;

		private FFManager()
		{
			m_CoreIni = CoreIni.Instance;
			m_Regex = new Regex(@"\s+");// allows split by whitespace

			m_ImagingDetails = new ImagingDetails();
			m_MoleculePrimitives = new MoleculePrimitiveList();
			m_AliasList = new Hashtable();
			m_AtomTypes = new AtomTypeList();
			m_BondDefinitions = new BondDefList();
			m_AngleDefinitions = new AngleDefList();
			m_TorsionDefinitions = new TorsionDefList();

			getSettings(); // from the generic Core.ini file parser
			ReadForceFieldParameterFile(); // from the file defined in the .ini

			Rotamers.Assign( m_MoleculePrimitives );
		}

		// this function is required by some of the classes that need to derive information from their peers,
		// but can only do so after the .Instance has been called already. By having a second "constructor" we solve the problem

		public void FinaliseStage2()
		{
			for( int i = 0; i < m_MoleculePrimitives.Count; i++ )
			{
				MoleculePrimitive mp = m_MoleculePrimitives[i];
				for( int j = 0; j < mp.AtomPrimitiveCount; j++ )
				{
					mp[j].FinaliseStage2(); // needs to be called for all atom primitives in order that the radius and other params are set corrently
				}
			}																
		}

		private void getSettings()
		{
			string key = "ForcefieldParams";
			if ( m_CoreIni.ContainsKey( key ) )
			{
				m_ParamFilename = m_CoreIni.DefaultSharedPath + m_CoreIni.ValueOf( key );
			}
			else
			{
				throw new LoadExcpetion("LOAD FAILURE : The standard initiation file does not contain a definition for the Forcefield Parameter Set filename. \nThe file should read \"ForcefieldParams=filename.Xxx\"" + "\r\nLooking for " + key );
			}

			if ( !File.Exists(m_ParamFilename) )
			{
				throw new LoadExcpetion("LOAD FAILURE : The standard initiation file defines \"ForcefieldParams=" + m_ParamFilename + "\" which does not exist.");
			}
		}

		public AtomTypeList AtomTypes
		{
			get
			{
				return m_AtomTypes;
			}
		}

		public BondDefList BondDefinitions
		{
			get
			{
				return m_BondDefinitions;
			}
		}

		public AngleDefList AngleDefinitions
		{
			get
			{
				return m_AngleDefinitions;
			}
		}
		public ImagingDetails Imaging
		{
			get
			{
				return m_ImagingDetails;
			}
		}

		public TorsionDefList TorsionDefinitions
		{
			get
			{
				return m_TorsionDefinitions;
			}
		}

		public MoleculePrimitiveBase GetMolPrimitive( string moleculePDBName )
		{
			for ( int i = 0; i < m_MoleculePrimitives.Count; i++ )
			{
				if ( m_MoleculePrimitives[i].MolName == moleculePDBName )
				{
					return m_MoleculePrimitives[i];
				}
			}
			return new MoleculePrimitiveBase( moleculePDBName, ( moleculePDBName == "HOH" ) );
		}

		private void ReadForceFieldParameterFile()
		{
			StreamReader re = new StreamReader(m_ParamFilename);

			bool ignore = false;
			string ignoreRelease = "";

			Trace.WriteLine( "Begining Initiation of ForceField Parameters..." );
			Trace.Indent();

			int lineCount = 0;
			int UnknownLineCount = 0;
			string lineBuffer;
			bool errorstatus = false;
			while( (lineBuffer = re.ReadLine()) != null )
			{ 
				lineCount++;

				//Sort out reading in of first word & handling of comments & empty lines
				if ( lineBuffer.Length == 0 ) continue;     //empty line
				if ( lineBuffer[0] == ' ' ) continue;       // rapid parsing - this rule isn't true for block readin later in the file, but the block header will cause a function to be spawned that will call re.ReadLine() itself ...
				lineBuffer = lineBuffer.Trim();
				if ( lineBuffer.Length == 0 ) continue;  // it contained just tabs and is now empty
				if ( lineBuffer[0] == '#' ) continue;       // # represents a comment line

				if ( ignore )
				{
					if( lineBuffer.Length < ignoreRelease.Length )
					{
						continue; // skip this line
					}
					if ( lineBuffer.Substring(0,ignoreRelease.Length) == ignoreRelease )
					{
						// carry on ..
						ignore = false;
						continue;
					}
					else
					{
						continue; // skip this line
					}                    
				}

				if( lineBuffer.Length < 80 ) 
				{
					lineBuffer = lineBuffer.PadRight( 80, ' ' );
				}

				if( lineBuffer.Substring(0,4) == "INFO" )
				{        // INFO - print trailing text
					Trace.WriteLine( lineBuffer.Substring(4, lineBuffer.Length - 4).TrimStart( new char[] { ' ', '\t' } ) );
				}
				else if( lineBuffer.Substring(0,5) == "IDENT" )
				{       // IDENT Forcefield identifier string
					string[] lineParts = m_Regex.Split( lineBuffer );
					if(lineParts.Length < 2)
					{
						Trace.WriteLine("SYNTAX ERROR: Identifier expected after 'IDENT'");
						errorstatus = true;
					}
					else
					{
						ParamSetName = lineParts[1];
						ParamSetIdentifierPresent = true;
						Trace.WriteLine("ForceField Parameter Set Identifier : " + ParamSetName);
						Trace.WriteLine("");
					}
				}
				else if( lineBuffer.Substring(0,5) == "ALIAS" )
				{
					string[] lineParts = m_Regex.Split( lineBuffer );

					if( lineParts.Length < 3 )
					{
						Trace.WriteLine("SYNTAX ERROR: Two identifier expected after 'ALIAS'");
						errorstatus = true;
						continue;
					}

					// save this alias 
					m_AliasList.Add( lineParts[2], lineParts[1] );
				}
				else if( lineBuffer.Substring(0,10) == "AUTOANGLES" )
				{
					// we dont care ...
					//m_Autoangles = true;
				}
				else if( lineBuffer.Substring(0,12) == "AUTOTORSIONS" )
				{ 
					//m_Autotorsions = true;
				}
				else if( lineBuffer.Substring(0,12) == "VDW14SCALING" )
				{     
					string[] lineParts = m_Regex.Split( lineBuffer );
					if( lineParts.Length < 2 )
					{
						Trace.WriteLine("SYNTAX ERROR: Identifier expected after 'VDW14SCALING'");
						errorstatus = true;
						continue;
					}
					try 
					{
						vdw14scaling = float.Parse( lineParts[1] );
					}
					catch
					{
						Trace.WriteLine("SYNTAX ERROR: Problem parsing VDW14SCALING value : " + lineParts[1] ); 
						errorstatus = true;
					}
				}
				else if( lineBuffer.Substring(0,13) == "ELEC14SCALING" )
				{    
					string[] lineParts = m_Regex.Split( lineBuffer );
					if( lineParts.Length < 2 )
					{
						Trace.WriteLine("SYNTAX ERROR: Identifier expected after 'ELEC14SCALING'");
						errorstatus = true;
						continue;
					}
					try 
					{
						elec14scaling = float.Parse( lineParts[1] );
					}
					catch
					{
						Trace.WriteLine("SYNTAX ERROR: Problem parsing ELEC14SCALING value : " + lineParts[1] ); 
						errorstatus = true;
					}   
				}
				else if( lineBuffer.Substring(0,7) == "SECTION" )
				{   // ADDITIONAL SECTION syntax reading/checking
					// ADD : ParseSection( StreamReader re );

					string[] lineParts = m_Regex.Split( lineBuffer );

					if ( lineParts.Length >= 2 )
					{
						if ( lineParts[1] == "DAVE_IMAGING_SETTINGS" )
						{
							ReadDAVEImagingSection( re, ref lineCount );
							continue; // we dont want to initiate the ignore routine section below
						} 
						else if ( lineParts[1] == "DAVE_REBUILD_ATOM_TYPES" )
						{
							ReadRebuildTypesSection( re, ref lineCount );
							continue; // we dont want to initiate the ignore routine section below
						}
						else if ( lineParts[1] == "DAVE_HBOND_TYPES" )
						{
                            ReadHBondingSection( re, ref lineCount );
							continue; // we dont want to initiate the ignore routine section below
						}
					}
					ignore = true;
					ignoreRelease = "ENDSECTION";
				}
				else if( lineBuffer.Substring(0,4) == "TYPE" )
				{   // TYPE syntax reading/checking
					ReadFFAtomType(ref lineBuffer);
				}
				else if( lineBuffer.Substring(0,4) == "BOND" )
				{   // BOND syntax reading/checking
					ReadBondDefinition(ref lineBuffer );
				}
				else if( lineBuffer.Substring(0,5) == "ANGLE" )
				{   // ANGLE syntax reading/checking
					ReadAngleDefinition( ref lineBuffer );
				}
				else if( (lineBuffer.Substring(0,7) == "TORSION" ) || (lineBuffer.Substring(0,8) == "IMPROPER") )
				{    // TORSION/IMPROPER syntax reading/checking
					ReadTorsionDefinition( ref lineBuffer );
				}
				else if( lineBuffer.Substring(0,8) == "MOLECULE" )
				{  //  MOLECULE syntax reading/checking
					string[] lineParts = m_Regex.Split( lineBuffer );
					if( lineParts.Length < 3 )
					{
						Trace.WriteLine("SYNTAX ERROR:  Name expected after MOLECULE");
						errorstatus = true;
						continue;	
					}
					string molName = lineParts[2];
					if( !ReadMoleculeBlock( re, ref lineCount, molName, lineParts[1].ToCharArray()[0] ) )
					{
						errorstatus = true;
						Trace.WriteLine("ERROR: Unexpected error, Molecule definition addition did not complete");
						break;                        
					}
				}
				else
				{
					UnknownLineCount++;
					Trace.WriteLine( "Line " + lineCount.ToString() + " has an unknown Identifier : " + lineBuffer );
				}


			}

			if(ignore)
			{
				errorstatus = true;
				Trace.WriteLine("ERROR: Unexpected end of file - no ignore release : \"" + ignoreRelease + "\" found");
			}
			
			Trace.WriteLine( lineCount.ToString() + " lines have been found in the file and analysed." );
			if ( UnknownLineCount > 0 )
			{
				errorstatus = true;
				Trace.WriteLine( UnknownLineCount.ToString() + " Lines were not processed as the IDENTIFIER was not understood");
			}
			Trace.Unindent();

			Trace.WriteLine("");

			if(!errorstatus)
			{
				Trace.WriteLine("Finished reading Forcefield Definition File - no errors present");
			}
			else
			{
				Trace.WriteLine("Errors occurded during forcefield file readout - check above script");
			}

			Trace.WriteLine("");

			re.Close();
		}


		public bool IsValidAtomTypeID( string AtomID )
		{
			return m_AtomTypes.ContainsID( AtomID );
		}

		private void ReadFFAtomType( ref string lineBuffer )
		{
			// Mikefunction :: AtomTypeReadLine()
			// TYPE    CW         6    12.01    1.9080    0.0860   0.0000	      sp2 arom. 5 memb.ring w/1 N-H and 1 H (HIS)
			//                   (Z)  AtMass     Radius  Epsilon    Charge
			string[] lineParts = m_Regex.Split( lineBuffer );
			AtomType type = new AtomType();
				type.TypeID = lineParts[1].PadRight(2,' ').ToUpper();
				type.AtmoicNumber = int.Parse(lineParts[2]);
				type.AtomicMass = float.Parse(lineParts[3]);
				type.Radius = float.Parse(lineParts[4]);
				type.Epsilion = float.Parse(lineParts[5]);
				type.Charge = float.Parse(lineParts[6]);
				type.Comment = lineParts[7];

				for ( int i = 8; i < lineParts.Length; i++ ) // concatenate the remaining terms, they are the comment;
				{
					type.Comment += ( " " + lineParts[i] );
				}

			m_AtomTypes.addAtomType( type );	
		}

		private void ReadBondDefinition( ref string lineBuffer )
		{
			// BOND     C  CA      1.4090  469.30     1.5     // TYR 

			string[] lineParts = m_Regex.Split( lineBuffer );
			if( lineParts.Length < 6 )
			{
				Trace.WriteLine("Syntax Error : Insufficient BOND parameters");
			}
			BondDefinition ffBondDef = new BondDefinition();

			ffBondDef.AtomType1 = lineParts[1].PadRight(2,' ');
			ffBondDef.AtomType2 = lineParts[2].PadRight(2,' ');

			try
			{
				ffBondDef.Length = float.Parse(lineParts[3]);
			}
			catch
			{
				ffBondDef.Length = 0.0f;
                Trace.WriteLine("Error : Bond length could not be parsed");
			}
			try
			{
				float forceconstant = float.Parse(lineParts[4]) * (float)(4186.8 / 6.002E23); // kcal/mol/rad to J/rad (SI)
				ffBondDef.ForceConstant = forceconstant;
			}
			catch
			{
				ffBondDef.ForceConstant = 0.0f;
				Trace.WriteLine("Error : Bond force constant could not be parsed");
			}
			try
			{
				ffBondDef.BondOrder = float.Parse(lineParts[5]);
			}
			catch
			{
				ffBondDef.BondOrder = 0.0f;
				Trace.WriteLine("Error : Bond bond-order could not be parsed");
			}

			if (   m_AtomTypes.ContainsID(ffBondDef.AtomType1)
				&& m_AtomTypes.ContainsID(ffBondDef.AtomType2) )
			{
				m_BondDefinitions.addBondDef( ffBondDef );
			}
			else
			{
				Trace.WriteLine("ERROR : BOND definition defines atom types that are not defined above - It will be ignored");
			}
		}

		private void ReadAngleDefinition( ref string lineBuffer )
		{

			// Reads out a line like this and saves it in itself
			// ANGLE     C   C   O     120.00   80.05 

			string[] lineParts = m_Regex.Split( lineBuffer );
			if( lineParts.Length < 6 )
			{
				Trace.WriteLine("Syntax Error : Insufficient ANGLE parameters");
			}
			AngleDefinition ffAngDef = new AngleDefinition();
			ffAngDef.AtomTypeA = lineParts[1].ToUpper().PadRight(2,' ');
			ffAngDef.AtomTypeB = lineParts[2].ToUpper().PadRight(2,' ');
			ffAngDef.AtomTypeC = lineParts[3].ToUpper().PadRight(2,' ');
			try
			{
				float angle = float.Parse(lineParts[4]);
				ffAngDef.angle = angle * (float)(Math.PI / 180);
			}
			catch
			{
				ffAngDef.angle = 0.0f;
				Trace.WriteLine("Syntax Error : ANGLE parameter for angle is incorrect");
			}
			try
			{
				float forceconstant = float.Parse(lineParts[5]) * (float)(4186.8 / 6.002E23); // kcal/mol/rad to J/rad (SI)
				ffAngDef.forceconstant = forceconstant;
			}
			catch
			{
				ffAngDef.forceconstant = 0.0f;
				Trace.WriteLine("Syntax Error : ANGLE parameter for forceconstant is incorrect");
			}

			if (   m_AtomTypes.ContainsID(ffAngDef.AtomTypeA)
				&& m_AtomTypes.ContainsID(ffAngDef.AtomTypeB)
				&& m_AtomTypes.ContainsID(ffAngDef.AtomTypeC) )
			{
				m_AngleDefinitions.addAngleDef( ffAngDef );
			}
			else
			{
				Trace.WriteLine("ERROR : ANGLE definition defines atom types that are not defined above - It will be ignored");
			}
		}

		private void ReadTorsionDefinition( ref string lineBuffer )
		{
			//# Atom      A   B   C   D      Vm(kcal/mol) gamma(deg) n  Reference
			string[] lineParts = m_Regex.Split( lineBuffer );
			if( lineParts.Length < 8 )
			{
				Trace.WriteLine("Syntax Error : Insufficient TORSION parameters");
			}
			string A = lineParts[1].ToUpper().PadRight(2, ' ');
			string B = lineParts[2].ToUpper().PadRight(2, ' ');
			string C = lineParts[3].ToUpper().PadRight(2, ' ');
			string D = lineParts[4].ToUpper().PadRight(2, ' ');

			TorsionDefinition ffTorDef = new TorsionDefinition( A, B, C, D );
			
			if ( lineParts[0].ToUpper() == "TORSION" )
			{
				ffTorDef.isImproperTorstion = false;
			}
			else 
			{
				ffTorDef.isImproperTorstion = true;
				if ( lineParts[0].ToUpper() != "IMPROPER" )
				{
					Trace.WriteLine("CODE ERROR: ReadTorsionDefinition() --> neither IMPROPER nor TORSION ??");
				}
			}
			float newVm;
			try
			{
				newVm = float.Parse(lineParts[5]) * (float)(4186.8 / 6.002E23); // kcal/mol/rad to J/rad (SI)
			}
			catch
			{
				newVm = -1.0f;
				Trace.WriteLine("Syntax Error : TORSION parameter for Vm is incorrect");
			}
			float newGamma;
			try
			{
				float angle = float.Parse(lineParts[6]); //deg --> rad
				newGamma = angle * (float)(Math.PI / 180);
			}
			catch
			{
				newGamma = -1.0f;
				Trace.WriteLine("Syntax Error : TORSION parameter for Gamma is incorrect");
			}
			float newN;
			try
			{
				newN = float.Parse(lineParts[7]);
			}
			catch
			{
				newN = -1.0f;
				Trace.WriteLine("Syntax Error : TORSION parameter for N is incorrect");
			}
			
			int prevtors;
			//now find out if there was a previous torsion of the same type (i.e. 
			//we need to add a new fourier term, not an entirely new torsion type
			for(prevtors=0;prevtors<m_TorsionDefinitions.Count;prevtors++)
			{
				TorsionDefinition prevDef = m_TorsionDefinitions[prevtors];
				 
				if( (prevDef.AtomTypeA == ffTorDef.AtomTypeA) &&
					(prevDef.AtomTypeB == ffTorDef.AtomTypeB) &&
					(prevDef.AtomTypeC == ffTorDef.AtomTypeC) &&
					(prevDef.AtomTypeD == ffTorDef.AtomTypeD) &&
					(prevDef.isImproperTorstion == ffTorDef.isImproperTorstion )  )
				{
					break;
				}
			}

			if(prevtors < m_TorsionDefinitions.Count)
			{    // yep there was a previous torsion of same type
				((TorsionDefinition)m_TorsionDefinitions[prevtors]).addTerm(newVm, newN, newGamma); // ok, add to previous one
			}
			else
			{   // no previous torsion of same type found
				ffTorDef.addTerm(newVm, newN, newGamma); // check this, should it be ffTorDef = ffTorDef.addterm ???
				m_TorsionDefinitions.addTorsionDef( ffTorDef );
			}
		}

		private bool ReadMoleculeBlock( StreamReader re, ref int lineCount, string moleculeName, char oneLetterCode )
		{
			//			float x,y,z,charge,mass,radius;
			//			char  atname[50],pdbname[50];
			//			int   atnum,n,k;

			bool status = true;

			MoleculePrimitive molPrimitive = new MoleculePrimitive( moleculeName, oneLetterCode );

			string lineBuffer;
			while( (lineBuffer = re.ReadLine()) != null )
			{ 
				lineCount++;
				if ( lineBuffer.Length == 0 ) continue;
				if ( lineBuffer[0] == '#' ) continue; // comment line

				lineBuffer = lineBuffer.Trim();

				if( lineBuffer.Length >= 11 ) 
				{
					if( lineBuffer.Substring(0,11) == "ENDMOLECULE" ) break; // ditto
					if( lineBuffer.Substring(0,12) == "ENDAMINOACID" ) break; // exit the streamreader loop, the molecule definbition is complete
				}

				if( lineBuffer.Substring(0,4) == "ATOM" )
				{
					// ATOM   N      N      0.000  0.000  0.000   N          -C,CA,H
					string[] lineParts = m_Regex.Split( lineBuffer );

					if ( lineParts.Length < 7 )
					{
						Trace.WriteLine( "SYNTAX ERROR: Error Parsing Atom Directive, an incorect number of parameters is present : " + lineParts.Length.ToString() );
						status = false;
						continue;
					}

					float[] defaultPosition = new float[3];
					defaultPosition[0] = 0.0f;
					try
					{
						defaultPosition[0] = float.Parse( lineParts[3] );
					}
					catch
					{
					}
					defaultPosition[1] = 0.0f;
					try
					{
						defaultPosition[1] = float.Parse( lineParts[4] );
					}
					catch
					{
					}
					defaultPosition[2] = 0.0f;
					try
					{
						defaultPosition[2] = float.Parse( lineParts[5] );
					}
					catch
					{
					}

					string altName = lineParts[1].ToUpper().PadRight(4, ' ');
					string PDBName = lineParts[2].ToUpper().PadRight(4, ' ');
					string ffID    = lineParts[6].ToUpper().PadRight(2, ' ');

					AtomPrimitive atomPrimitive = new AtomPrimitive(molPrimitive,
						altName,
						PDBName,
						new Position(defaultPosition),
						ffID,
						lineParts[7] ); // the description of the type

					if ( !IsValidAtomTypeID( atomPrimitive.ForceFieldID ) )
					{
						Trace.WriteLine("SYNTAX ERROR: Atom definition references an invalid forcefield type : " + atomPrimitive.ForceFieldID );
						Trace.WriteLine("Atom will be ignored");
						status = false;
					}
					else
					{
						molPrimitive.AddPrimitive( atomPrimitive );
					}

				}
				else if( lineBuffer.Substring(0,8) == "IMPROPER" )
				{
					//     IMPROPER -C  CA   N   H

					string[] lineParts = m_Regex.Split( lineBuffer );
					if ( lineParts.Length < 5 )
					{
						Trace.WriteLine("IMPROPER Not Correctly Defined");
						status = false;
						continue;
					}
                    Torsion improper = new Torsion();
                    improper.Type_1 = lineParts[1].PadRight(4,' '); 
					improper.Type_2 = lineParts[2].PadRight(4,' '); 
					improper.Type_3 = lineParts[3].PadRight(4,' '); 
					improper.Type_4 = lineParts[4].PadRight(4,' '); 

					string check1 = improper.Type_1;
					if( check1[0] == '-' ) check1 = check1.Substring(1,3) + " ";
					if( check1[0] == '+' ) check1 = check1.Substring(1,3) + " ";
					string check2 = improper.Type_2;
					if( check2[0] == '-' ) check2 = check2.Substring(1,3) + " ";
					if( check2[0] == '+' ) check2 = check2.Substring(1,3) + " ";
					string check3 = improper.Type_3;
					if( check3[0] == '-' ) check3 = check3.Substring(1,3) + " ";
					if( check3[0] == '+' ) check3 = check3.Substring(1,3) + " ";
					string check4 = improper.Type_4;
					if( check4[0] == '-' ) check4 = check4.Substring(1,3) + " ";
					if( check4[0] == '+' ) check4 = check4.Substring(1,3) + " ";

					if ( !molPrimitive.IsValidAtomID( check1 ) ||
						!molPrimitive.IsValidAtomID( check2 ) ||
						!molPrimitive.IsValidAtomID( check3 ) ||
						!molPrimitive.IsValidAtomID( check4 ) )
					{
						Trace.WriteLine("IMPROPER Not Correctly Defined, one or more Types are not valid");
						status = false;
						continue;
					}																	

					molPrimitive.AddTorsion( improper );
				}
				else if( lineBuffer.Substring(0,7) == "TORSION" )
				{
					Trace.WriteLine("CODE IMPLEMENTATION ERROR : Molecule block torsion is not coded ...");
				}
				else if( lineBuffer.Substring(0,9) == "ISSOLVENT" )
				{
					molPrimitive.SetIsSolvent( true );
				}
				else if( lineBuffer.Substring(0,8) == "GEOMETRY" )
				{
					// DAVE doesnt use this, but get it anyway
					string[] lineParts = m_Regex.Split( lineBuffer );
					if ( lineParts.Length < 2 )
					{
						Trace.WriteLine("ERROR: Character string expected in ATOM geometry directive");
						status = false;
						continue;
					}
					molPrimitive.SetGeometry( lineParts[1] );
				}
				else if( lineBuffer.Substring(0,6) == "CHARGE" )
				{
					string[] lineParts = m_Regex.Split( lineBuffer );
					if ( lineParts.Length < 3 )
					{
						Trace.WriteLine("ERROR: insufficient number of parameters in ATOM charge directive");
						status = false;
						continue;
					}
					try
					{
						float charge = float.Parse( lineParts[2] );
						string AtomID = lineParts[1].ToUpper().PadRight(4,' ');
						if ( molPrimitive.ContainsAtomWithAltID( AtomID ) )
						{
							molPrimitive.GetAtomPrimitiveFromAltID( AtomID ).DefaultCharge = charge;
						}
						else
						{
							throw new Exception("Mol primitive did not contain an atom specified by a charge definition");
						}
					}
					catch
					{
						Trace.WriteLine("ERROR: Float parsing failed for ATOM charge directive");
						status = false;
						continue;
					}
				}
				else if( lineBuffer.Substring(0,9) == "VDWRADIUS" )
				{
					//Trace.WriteLine("PROGRAM CODE WARNING !! - No Code Present to deal with \"VDWRADIUS\" ATOM directive");
				}
				else if( lineBuffer.Substring(0,4) == "MASS" )
				{
					//Trace.WriteLine("PROGRAM CODE WARNING !! - No Code Present to deal with \"MASS\" ATOM directive");
				}

			} // "ENDMOLECULE" directive has been reached ...
 			m_MoleculePrimitives.addPrimitive( molPrimitive );
			return status;
		}

		private void ReadRebuildTypesSection( StreamReader re, ref int lineCount )
		{
			string lineBuffer;
			while( (lineBuffer = re.ReadLine()) != null )
			{
				lineCount++;
				lineBuffer = lineBuffer.PadRight(80,' ');
				if( lineBuffer.Substring( 0, 10 ) == "ENDSECTION" )
				{
					return; // the section has ended ...
				}

				lineBuffer = lineBuffer.TrimStart( new char[] { ' ', '\t' } );

				string[] lineParts = m_Regex.Split( lineBuffer );
				if ( lineParts.Length == 0 ) continue;
				if ( lineParts[0] != "TYPE" ) continue;
				if ( lineParts.Length < 3 )
				{
					Trace.WriteLine( "SYNTAX ERROR : DAVE Rebuild Types definitions line is incomplete" );
					continue;
				}
				try
				{
					bool isSet = false;
					string ffType = lineParts[1].PadRight(2,' ').ToUpper();
					if( ffType.Length > 2 ) ffType = ffType.Substring(0,2);

					for ( int i = 0; i < m_AtomTypes.Count; i++ )
					{
						if( m_AtomTypes[i].TypeID == ffType )
						{
							m_AtomTypes[i].AtomBuilderLevel = int.Parse( lineParts[2] );
							isSet = true;
							break;
						}
					}

					if ( !isSet )
					{
						Trace.WriteLine( "DAVE Rebuild definitions Could not find the forcefield atom type : " + lineParts[1] );
					}
				}
				catch
				{
					Trace.WriteLine( "SYNTAX ERROR : DAVE Rebuild definitions parse error" );
				}
			}
		}

		private void ReadHBondingSection( StreamReader re, ref int lineCount )
		{
			string lineBuffer;
			while( (lineBuffer = re.ReadLine()) != null )
			{
				lineCount++;
				lineBuffer = lineBuffer.PadRight(80,' ');
				if( lineBuffer.Substring( 0, 10 ) == "ENDSECTION" )
				{
					return; // the section has ended ...
				}

				lineBuffer = lineBuffer.Trim( new char[] { ' ', '\t' } );

				string[] lineParts = m_Regex.Split( lineBuffer );
				if ( lineParts.Length == 0 ) continue;
				if ( lineParts[0].Length == 0 ) continue;
				if ( lineParts[0][0] == '#' ) continue;

				if ( lineParts[0] == "ACCEPTOR" )
				{
					for( int i = 1; i < lineParts.Length; i++ )
					{
						try
						{
							string ffType = lineParts[i].PadRight(2,' ').ToUpper();
							if( ffType.Length > 2 ) ffType = ffType.Substring(0,2);
							AtomType fft = AtomTypes.GetTypeFromFFID( ref ffType );
							fft.HBondAcceptor = true;
						}
						catch
						{
						}
					}
				}
				else if( lineParts[0] == "HYDROGENS" )
				{
					for( int i = 1; i < lineParts.Length; i++ )
					{
						try
						{
							string ffType = lineParts[i].PadRight(2,' ').ToUpper();
							if( ffType.Length > 2 ) ffType = ffType.Substring(0,2);
							AtomType fft = AtomTypes.GetTypeFromFFID( ref ffType );
							fft.HBondHydrogen = true;
						}
						catch
						{
						}
					}
				}
			}
		}
	
		private void ReadDAVEImagingSection( StreamReader re, ref int lineCount )
		{
			string lineBuffer;
			while( (lineBuffer = re.ReadLine()) != null )
			{
				lineCount++;
				lineBuffer = lineBuffer.PadRight(80,' ');
				if( lineBuffer.Substring( 0, 10 ) == "ENDSECTION" )
				{
					return; // the section has ended ...
				}

				lineBuffer = lineBuffer.TrimStart( new char[] { ' ', '\t' } );

				string[] lineParts = m_Regex.Split( lineBuffer );
				if ( lineParts.Length == 0 ) continue;
				if ( lineParts[0].Length == 0 ) continue;
				if ( lineParts[0][0] == '#' ) continue;
				if ( lineParts[0] != "DAVEIMAGINGTYPE" ) continue;
				if ( lineParts.Length < 5 )
				{
					Trace.WriteLine( "SYNTAX ERROR : DAVE Imaging definitions line is incomplete" );
					continue;
				}
				try
				{
					m_ImagingDetails.addAtomTypeDefinition( lineParts[1].ToUpper().PadRight(2,' '),
						Colour.FromName( lineParts[2] ), int.Parse( lineParts[3] ), int.Parse( lineParts[4] ) );
				}
				catch
				{
					Trace.WriteLine( "SYNTAX ERROR : DAVE Imaging definitions parse error" );
				}
			}
		}
	}
}
