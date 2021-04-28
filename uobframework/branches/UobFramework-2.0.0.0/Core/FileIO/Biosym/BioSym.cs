using System;
using System.IO;
using UoB.Core.FileIO.PDB;

namespace UoB.Core.FileIO.Biosym
{
	/// <summary>
	/// Summary description for BioSym.
	/// http://instinct.v24.uthscsa.edu/~hincklab/html/soft_packs/msi_docs/insight980/formats980/ClassicFiles.html
	/// file format descriptor
	/// </summary>
	public class BioSym
	{
		private BioSym()
		{
		}

		private static void Assert( bool condition, string exception )
		{
			if( !condition )
			{
				throw new Exception( exception );
			}
		}

		private static void AssertNonNull( string line )
		{
			Assert( line != null, "Given line was null" );
		}

		private static void AssertExclaim( string line )
		{
            AssertNonNull( line );			
			Assert( line[0] == '!', "The header line is corrupt" );
		}


		public static void ConvertToPDB( DirectoryInfo di, OutMode mode )
		{
			foreach( FileInfo fi in di.GetFiles("*.cor") )
			{
				ConvertToPDB( fi.FullName, di.FullName + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension( fi.Name ), mode );
			}
			foreach( FileInfo fi in di.GetFiles("*.car") )
			{
				ConvertToPDB( fi.FullName, di.FullName + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension( fi.Name ), mode );
			}
		}

		public static void ConvertToPDB( string inBiosymFilename, string outPDBFilestem, OutMode mode )
		{
			StreamReader re = null;
			StreamWriter rw = null;
			try
			{

				re = new StreamReader( inBiosymFilename );
			
				if( mode != OutMode.ModelsMultiFile )
				{
					// we only need one outfile 
					rw = new StreamWriter( outPDBFilestem + ".pdb" );
				}

				// O       -4.881914616    4.210723400   -0.688144684 ALA  30     o'      O  -0.380
				// CB      -2.640573263    1.860350370   -1.544302940 ALA  30     c3      C  -0.300
				// HB1     -2.167516947    2.805963993   -1.867924333 ALA  30     h       H   0.100
				// HB2     -1.829617500    1.220069051   -1.147842050 ALA  30     h       H   0.100
				// HB3     -3.029923677    1.363167763   -2.454002142 ALA  30     h       H   0.100
			
				PDBAtom atom = new PDBAtom();
				atom.altLocIndicator = ' ';
				atom.chainID = 'A';
				atom.insertionCode = ' ';
				atom.lineType = "ATOM  ";
				int atomNum = 0;

				string line;
				int model = 1;
				while( true )
				{
					// assert the header is correct
					line = re.ReadLine();
					if( line == null || re.Peek() == -1  ) 
					{
						// its the end of the world as we know it ... and i feel fine !
						// there is usually a 0 length line at the end of the file, we need to parse for this...
						break;
					}

					if( mode == OutMode.ModelsMultiFile ) 
					{
						if( rw != null ) rw.Close();
						rw = new StreamWriter( outPDBFilestem + '.' + model.ToString() + ".pdb" );
					}

					AssertExclaim( line );
					rw.Write( "REMARK " );
					rw.WriteLine( line );

					line = re.ReadLine();
					AssertNonNull( line );
					rw.Write( "REMARK " );
					rw.WriteLine( line );

					line = re.ReadLine();
					AssertNonNull( line );
					rw.Write( "REMARK " );
					rw.WriteLine( line );

					line = re.ReadLine();
					AssertExclaim( line );
					rw.Write( "REMARK " );
					rw.WriteLine( line );

					while( true ) // system get loop
					{
						AssertNonNull( line = re.ReadLine() );
						if( 0 == String.Compare( line, 0, "end", 0, 3, true ) )
						{
							break;
						}

						rw.Write("MODEL ");
						rw.WriteLine( model.ToString().PadLeft(4,' ') );
						model++;

						while( true ) // molecule get loop
						{
							AssertNonNull( line = re.ReadLine() );
							if( 0 == String.Compare( line, 0, "end", 0, 3, true ) )
							{
								break;
							}

							// fill our PDBStyle atom structure
							atom.atomName = ' ' + line.Substring(0,3);
							atom.atomNumber = atomNum++;
							atom.position.x = double.Parse( line.Substring(5,15) );
							atom.position.y = double.Parse( line.Substring(20,15) );
							atom.position.z = double.Parse( line.Substring(35,15) );
							atom.residueName = line.Substring(51,3);
							atom.residueNumber = int.Parse( line.Substring(55,3) );

							// write an atom line
							rw.WriteLine( atom.ToString() );
						}
						atom.chainID++;					
					}	
					rw.WriteLine("ENDMDL");
					
					// exit now if we only want one model
					if( mode == OutMode.ModelFirstOnly )
					{
						break;
					}
					atom.chainID = 'A'; // reset this
				}
			}
			catch( Exception ex )
			{
				throw new Exception( "Fail in conversion of Biosym to PDB : " + ex.Message, ex );
			}
			finally
			{
				if( re != null ) re.Close();
				if( rw != null ) rw.Close();
			}
		}
	}
}
