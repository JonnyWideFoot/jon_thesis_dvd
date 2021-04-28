using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using UoB.Core.Structure;
using UoB.Core.FileIO.PDB;
using UoB.Core.Primitives;

namespace UoB.Core.Structure.Alignment
{
	/// <summary>
	/// Summary description for AlignFile.
	/// </summary>
	public sealed class AlignFile
	{
		private FileInfo m_FileInfo;
		private PSAlignManager m_Manager = null;
		//private AlignmentMethod m_Method = AlignmentMethod.Undefined;
		private ArrayList m_SysDefs = new ArrayList();

		// scan file results
		private int m_AlignReportCount = 0;
		private int m_AlignTaskCount = 0;

		public AlignFile( string fileName )
		{
			ReassignFile( fileName );
		}

		public void ClearAll()
		{
			m_SysDefs.Clear();
		}

		public void ReassignFile( string fileName )
		{
			m_FileInfo = new FileInfo( fileName );
			if( !m_FileInfo.Exists ) throw new Exception("Filename given to alignfile class does not exist!");
			ScanFile();
			InitAlignReports();
		}

		public void DoAlignTasks()
		{
		}

		private void InitAlignReports()
		{
			for( int i = 0; i < m_AlignReportCount; i++ )
			{
				m_SysDefs.Add(null); // dont assign them yet, just get the tally
			}
		}

		#region Accessors

		public PSAlignManager AliManager
		{
			get
			{
				return m_Manager;
			}
		}

		public AlignmentSysDef this[ int index ]
		{
			get
			{
				if( m_SysDefs[index] == null ) // null on initialisation
				{
					GetAlignReport(index);
					return (AlignmentSysDef) m_SysDefs[index];
				}
				else
				{
					return (AlignmentSysDef) m_SysDefs[index];
				}
			}
		}

		public int SysDefCount
		{
			get
			{
				return m_SysDefs.Count;
			}
		}

		public bool hasAlignTasks
		{
			get
			{
				return m_AlignTaskCount > 0;
			}
		}
		public bool hasAlignReports
		{
			get
			{
				return m_AlignReportCount > 0;
			}
		}

		#endregion

		#region ReadinFunctions

		private void ScanFile()
		{
			// testing for :
			// BEGINALIGNTASK -> ENDALIGNTASK
			// BEGINALIGNREPORT -> ENDMOLREPORT
			StreamReader re = null;

			int countBEGINALIGNTASK = 0;
			int countENDALIGNTASK = 0;
			int countBEGINALIGNREPORT = 0;
			int countENDALIGNREPORT = 0;
			
			try
			{
				re = new StreamReader( m_FileInfo.FullName );

				string line;
				while( null != ( line = re.ReadLine() ) )
				{
					line = line.Trim();
					if( line.Length == 0 || line[0] == '#' ) // # = comment line
					{
						continue;
					}
					string[] lineParts = UoB.Core.Tools.CommonTools.WhiteSpaceRegex.Split( line );
					if( lineParts.Length < 1 )
					{
						continue;
					}

					switch( lineParts[0].ToUpper() )
					{
						case "BEGINALIGNTASK":
							countBEGINALIGNTASK++;
							break;
						case "ENDALIGNTASK":
							countENDALIGNTASK++;
							break;
						case "BEGINALIGNREPORT":
							countBEGINALIGNREPORT++;
							break;
						case "ENDALIGNREPORT":
							countENDALIGNREPORT++;
							break;
						default:
							break;
					}
				}
			}
			catch( Exception ex )
			{
				throw ex;
			}
			finally
			{
				if( re != null )
				{
					re.Close();
				}
			}
			if( countBEGINALIGNTASK != countENDALIGNTASK ) throw new Exception("File is Corrupt : The \"AlignTask\" begin and end counts do not match.");
			if( countBEGINALIGNREPORT != countENDALIGNREPORT ) throw new Exception("File is Corrupt : The \"AlignTask\" begin and end counts do not match.");
			
			// count complete
			m_AlignReportCount = countBEGINALIGNREPORT;
			m_AlignTaskCount = countBEGINALIGNTASK;
		}


		private void GetAlignReport( int entryIndex )
		{
			StreamReader re = new StreamReader( m_FileInfo.FullName );

			int totalAlignments = m_AlignReportCount + m_AlignTaskCount;

			if( entryIndex < m_AlignReportCount )
			{
				bool found = false;
				for( int i = 0; i < entryIndex; i++ )
				{
					found = ReadUntil( re, "ENDALIGNREPORT" );
					AssertBlankLine(re);
					AssertBlankLine(re);
				}
				if( found || entryIndex == 0 )
				{
					getAlignReportFromCurrentPos( re, entryIndex );
				}
				else
				{
					throw new Exception("Unexpected error : read-past end of file whilst tring to find BEGINALIGNREPORT entry : " + entryIndex.ToString() );
				}
			}
			else if ( entryIndex < totalAlignments )
			{
				throw new Exception("CODE NOT IMPLEMENTED : cannot deal with this situation");
			}
			else
			{
				throw new Exception("The given entry is greater than the number of defined systems");
			}
		}

		// this function assumes that we are already at the correct position
		private void getAlignReportFromCurrentPos( StreamReader re, int assignToPos )
		{
			string lineCache = re.ReadLine();
			string[] lineParts;

			AssertLine( ref lineCache, "BEGINALIGNREPORT", "Error: Did not find BEGINALIGNREPORT where expected");

			lineCache = re.ReadLine();
			string name = lineCache.Substring(5,lineCache.Length-5);
			lineCache = re.ReadLine();
			lineParts = lineCache.Trim().Split (new char[] { ' ' } );
			AlignSaveParams m_SaveParams = (AlignSaveParams) Enum.Parse(typeof(AlignSaveParams),int.Parse(lineParts[1]).ToString());

			AssertBlankLine( re );

			StringBuilder molReport = new StringBuilder();

			if( ( m_SaveParams & AlignSaveParams.AlignReport ) == AlignSaveParams.AlignReport )
			{
				// assert we have one
				if( !AssertLine( re, "BEGINREPORT" ) ) 
				{
					throw new Exception("Type definitions are incorrect, BEGINREPORT not found");
				}
				string line;
				while( (line = re.ReadLine()) != null )
				{
					if( line.Length >= 9 && "ENDREPORT" == line.Substring(0,9) ) 
					{
						break;
					}	
					molReport.Append(line);
					molReport.Append("\r\n");
				}
			}
			AssertBlankLine( re );

			AssertLine( re, "BEGINSYSTEMDEF", "Type definitions are incorrect, BEGINREPORT not found" );

			ParticleSystem ps; // defined below in some way ...
			PS_PositionStore modelPositions;
			AlignmentSysDef def;

			if( ( m_SaveParams & AlignSaveParams.ExplicitPositions ) == AlignSaveParams.ExplicitPositions )
			{
				throw new Exception("CODE NOT IMPLEMENTED");
			}
			else
			{
				// first get the system count

				lineCache = re.ReadLine().Trim();
				AssertLine( ref lineCache, "SYSTEMCOUNT ", "SYSTEMCOUNT check not present in system definition");
				int systemCount = int.Parse( lineCache );
				if( systemCount != 2 ) throw new Exception("System counts of more than 2 are not yet supported");
				
				// read mol defs				
				//		BEGINFILEDEF
				//		FileName C:\_Gen Ig Database 04.10.04\newOut\12e8_H_117-213.pdb
				//		ChainID H
				//		StartID 117
				//		EndID 213
				//		ENDFILEDEF

				AlignSourceDefinition def1 = null;
				AlignSourceDefinition def2 = null;

				AssertLine( re, "BEGINFILEDEF", "BEGINFILEDEF check not passed");
				try
				{
					def1 = new AlignSourceDefinition( re );
				}
				catch(Exception ex)
				{
					throw new Exception("Error during AlignSourceDefinition parsing..." + ex.ToString() );
				}
				AssertLine( re, "ENDFILEDEF", "ENDFILEDEF check not passed");

				AssertLine( re, "BEGINFILEDEF", "BEGINFILEDEF check not passed");
				try
				{
					def2 = new AlignSourceDefinition( re );
				}
				catch(Exception ex)
				{
					throw new Exception("Error during AlignSourceDefinition parsing..." + ex.ToString() );
				}
				AssertLine( re, "ENDFILEDEF", "ENDFILEDEF check not passed");

				ps = new ParticleSystem( name );
				ps.BeginEditing();
				ps.AddMolContainer( def1.MolRange.CloneFromRange() );
				ps.AddMolContainer( def2.MolRange.CloneFromRange() );
				ps.EndEditing(true,true);
				modelPositions = new PS_PositionStore( ps );
				def = new AlignmentSysDef( modelPositions, 0, 1, def1, def2 );
				
				AssertLine( re, "ENDSYSTEMDEF", "ENDSYSTEMDEF check not passed");
				AssertBlankLine( re );
			}

			if( ( m_SaveParams & AlignSaveParams.PSInfoBlock ) == AlignSaveParams.PSInfoBlock )
			{
				// dont care about the contents of this block, just for user info in the file
				ReadUntil( re, "ENDEXTINFO" );
				AssertBlankLine( re );
			}

			if( ( m_SaveParams & AlignSaveParams.ModelsDefined ) == AlignSaveParams.ModelsDefined )
			{
				AssertLine( re, "BEGINMODELS", "BEGINMODELS check not passed");

				lineCache = re.ReadLine().Trim();
				AssertLine( ref lineCache, "MODELCOUNT ", "MODELCOUNT definition not present in system definition");
				AssertBlankLine( re );
				int modelCount = int.Parse( lineCache );
				
				for( int q = 0; q < modelCount; q++ )
				{
					AssertLine( re, "BEGINMODEL", "BEGINMODEL check not passed");
					Model m = new Model( re );
					AssertLine( re, "ENDMODEL", "ENDMODEL check not passed");
					AssertBlankLine( re );
					def.Models.AddModel( m );
				}

				// now set the position store from the model definitions.
				for( int q = 0; q < modelCount; q++ )
				{
					Position[] pos = def.ModelStore.GetClonePositionArray(0);
					int length1 = def.particleSystem.MemberAt(0).Atoms.Count;
					int length2 = def.particleSystem.MemberAt(1).Atoms.Count;

					def.ModelStore.addPositionArray( pos, false ); // dont set the, we need orig positions for next translation
					// now perform the transform to overlay the system based on model 'q'
					Translate( pos, def.Models[q].TranslationX, 0, length1 );
					Translate( pos, def.Models[q].TranslationY, length1, length2 );
					def.Models[q].RotMatrix.transform( pos, length1, length2 );
				}

				AssertLine( re, "ENDMODELS", "ENDMODELS check not passed");

				m_SysDefs[assignToPos] = def;				
			}
		}
		#endregion

		#region File Writing functions
		public static void ClearReportFile( string fileName )
		{
			StreamWriter rw = new StreamWriter( fileName );
			rw.Close();
		}
		public void ClearReportFile()
		{
			StreamWriter rw = new StreamWriter( m_FileInfo.FullName, false );
			rw.Close();
		}

		public void SaveReport( string fileName, bool append, AlignSaveParams param, Options opts )
		{
			StreamWriter rw = new StreamWriter(fileName,append);
			for( int q = 0; q < m_SysDefs.Count; q++ )
			{
				AlignmentSysDef def = (AlignmentSysDef) m_SysDefs[q];
				SaveReport( rw, def, param, opts );
			}
			rw.Close(); 
		}

		public static void SaveReport( string fileName, AlignmentSysDef def, bool append, AlignSaveParams param, Options opts )
		{
			StreamWriter rw = new StreamWriter(fileName,append);
			SaveReport( rw, def, param, opts );
			rw.Close(); 
		}

		private static void SaveReport( StreamWriter rw, AlignmentSysDef def, AlignSaveParams param, Options opts )
		{
			rw.WriteLine( "BEGINALIGNREPORT" );

			rw.WriteLine( "NAME " + def.particleSystem.Name );
			rw.WriteLine( "SAVETYPES " + ((int)param).ToString() ); // record the current save method as an int

			rw.WriteLine();
		
			bool alignReport = ( ( param & AlignSaveParams.AlignReport ) == AlignSaveParams.AlignReport );
			if ( alignReport )
			{
				// write the align report
				rw.WriteLine("BEGINREPORT");
				rw.WriteLine(def.Report);
				rw.WriteLine();
				opts.WriteToStream( rw );
				rw.WriteLine("ENDREPORT");
				rw.WriteLine();
			}
			rw.WriteLine("BEGINSYSTEMDEF");

			bool savePositions = ( ( param & AlignSaveParams.ExplicitPositions ) == AlignSaveParams.ExplicitPositions );
			if ( savePositions )
			{
				rw.WriteLine("\tBEGINPDBPSDEF");
				PDB.SaveAtomsTo( rw, def.particleSystem );
				rw.WriteLine("\tENDPDBPSDEF");
			}
			else
			{
				rw.WriteLine("\tSYSTEMCOUNT 2");
				rw.WriteLine("\tBEGINFILEDEF");
				rw.Write( def.SourceDef1.ToString() );
				rw.WriteLine("\tENDFILEDEF");
				rw.WriteLine("\tBEGINFILEDEF");
				rw.Write( def.SourceDef2.ToString() );
				rw.WriteLine("\tENDFILEDEF");
			}
			rw.WriteLine("ENDSYSTEMDEF");
			rw.WriteLine();

			bool savePSInfo = ( ( param & AlignSaveParams.PSInfoBlock ) == AlignSaveParams.PSInfoBlock );
			if ( savePSInfo )
			{
				rw.WriteLine("BEGINEXTINFO");
				// extended info such as PSLength must be written
				rw.WriteLine( "PS1.Length : " + def.PS1.Count );
				rw.WriteLine( "PS2.Length : " + def.PS2.Count );
				rw.WriteLine("ENDEXTINFO");
				rw.WriteLine();
			}

			bool saveModelListings = ( ( param & AlignSaveParams.ModelsDefined ) == AlignSaveParams.ModelsDefined );
			if ( saveModelListings )
			{
				rw.WriteLine("BEGINMODELS");
				rw.WriteLine("\tMODELCOUNT " + def.Models.ModelCount.ToString() );
				rw.WriteLine();

				for( int i = 0; i < def.Models.ModelCount; i++ )
				{
					rw.WriteLine("\tBEGINMODEL");
					def.Models[i].WriteDefinition( rw );
					rw.WriteLine("\tENDMODEL");
					rw.WriteLine();
				}
				rw.WriteLine("ENDMODELS");
				rw.WriteLine();
			}

			rw.WriteLine( "ENDALIGNREPORT" );
			rw.WriteLine();
			rw.WriteLine();		           
		}	

		#endregion

		#region Helper Functions

		private void Translate( Position[] positions, Position translation, int startIndex, int length )
		{
			int end = startIndex + length;
			for( int i = startIndex; i < end; i++ )
			{
				positions[i].Minus( translation );
			}										 
		}

		private bool nullReadBlankingLines( StreamReader re, int count )
		{
			try
			{
				for( int i = 0; i < count; i++ )
				{
					re.ReadLine();
				}
				return true;
			}
			catch
			{
				re.Close();
				return false;
			}
		}

		private void AssertBlankLine( StreamReader re )
		{
			string read = re.ReadLine();
			if( read != "" )
			{
				throw new Exception("Blankline assertion failed, found : " + read);
			}
		}

		private void AssertLine( ref string line, string checkStart, string exceptionOnFalse )
		{
			string lineType = line.Substring(0,checkStart.Length);
			if( lineType != checkStart )
			{
				throw new Exception( exceptionOnFalse );
			}
			line = line.Substring(checkStart.Length,line.Length-checkStart.Length);
		}

		private bool AssertLine( StreamReader re, string check )
		{
			return check == re.ReadLine().Substring(0,check.Length);
		}

		private void AssertLine( StreamReader re, string check, string exceptionOnFalse )
		{
			string line = re.ReadLine().Trim();
			if( check.Length > line.Length )
			{
				throw new Exception(exceptionOnFalse);
			}
			else if( check == line.Substring(0,check.Length) )
			{
				return;
			}
			else
			{
				throw new Exception(exceptionOnFalse);
			}
		}


		private bool ReadUntil( StreamReader re, string stop )
		{
			string linePassBack;
			while( (linePassBack = re.ReadLine().Trim()) != null )
			{
				if( linePassBack.Length < stop.Length )
				{
					continue; // we dont want to parse it
				}
				else if( stop == linePassBack.Substring(0,stop.Length) ) 
				{
					return true;
				}					
			}
			return false;
		}

		private bool ReadUntil( StreamReader re, string stop, out string linePassBack )
		{
			while( (linePassBack = re.ReadLine().Trim() ) != null )
			{
				if( stop.Length < stop.Length )
				{
					continue; // we dont want to parse it
				}
				if( stop != re.ReadLine().Substring(0,stop.Length) ) 
				{
					return true;
				}					
			}
			return false;
		}
		#endregion
	}
}
