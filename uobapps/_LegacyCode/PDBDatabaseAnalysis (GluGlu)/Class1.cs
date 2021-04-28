using System;
using System.IO;
using System.Text;

using UoB.Core.Primitives;
using UoB.Core.FileIO.PDB;
using UoB.Core.Structure;

namespace PDBDatabaseAnalysis
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		private string m_TitleString = "PDBID frormat is XXXXY.pdb where XXXX is the PDB identifier and\r\nY is the chainID.\r\nThe Residue info is: Residue Name, Residue Number, Chain Insertion Code.\r\nAll files in a 1.8A resolution cutoff database were\r\nscanned for Glu residues closer than 7A within the same polypeptide chain.\r\n\r\nContact jon.rea@bris.ac.uk for more information... Enjoy ;-)\r\n\r\nPDBID       \tNumRes\tExptlMthd\tResln\tDist\tAngle\tResidue1 Info\t Residue2 Info";
		private StreamWriter m_RW;
		private StringBuilder m_SB;

		private Position m_Midpoint1;
		private Position m_Midpoint2;
		private Vector m_AA1_CToOMid; // the vector pointing from the CG atom of Glu to the O-O midpoint
		private Vector m_AA2_CToOMid;

		public Class1()
		{
			m_SB = new StringBuilder();
			m_Midpoint1 = new Position();
			m_Midpoint2 = new Position();
			m_AA1_CToOMid = new Vector();
			m_AA2_CToOMid = new Vector();
		}

		public void Go( DirectoryInfo di )
		{
			m_RW = new StreamWriter( di.FullName + "_report.log" );

			m_RW.WriteLine( m_TitleString );
			Console.WriteLine( m_TitleString );

			FileInfo[] files = di.GetFiles("*.pdb");
			for( int q = 0; q < files.Length; q++ )
			{
				PDB file = new PDB( files[q].FullName, true );
				ParticleSystem ps = file.particleSystem;

				for( int i = 0; i < ps.MemberCount; i++ )
				{
					// look for Glu Pairs within the same chain.

					PolyPeptide pp = ps.MemberAt(i) as PolyPeptide;
					if( pp != null )
					{
						for( int j = 0; j < pp.Count; j++ )
						{
							for( int k = j+1; k < pp.Count; k++ )
							{
								// we found a Glu pair within the same polypeptide. Is it any good?
								Assess( file.InternalName, file.ExtendedInformation, pp[j], pp[k] );
							}
						}
					}
				}

				m_RW.Flush(); // dump the current stream buffer to disk. We can look at data while the program is running this way...
			}

			m_RW.Close();
		}

		private void Assess( string PDBID, PDBInfo ext, AminoAcid aa1, AminoAcid aa2 )
		{
			if( 0 == String.CompareOrdinal( aa1.Name, 0, "GLU", 0, 3 ) &&
				0 == String.CompareOrdinal( aa2.Name, 0, "GLU", 0, 3 ) )
			{
				//ATOM    517  CG  GLU    32      13.973  54.945  56.083  1.00 27.57           C  
				//ATOM    521  OE1 GLU    32      15.802  54.776  54.508  1.00 34.50           O  
				//ATOM    522  OE2 GLU    32      14.224  56.201  54.090  1.00 36.48           O  

				Atom aa1_OE1 = aa1.AtomOfType( " OE1" );
				Atom aa1_OE2 = aa1.AtomOfType( " OE2" );
				Atom aa1_CT  = aa1.AtomOfType( " CG " );

				Atom aa2_OE1 = aa2.AtomOfType( " OE1" );
				Atom aa2_OE2 = aa2.AtomOfType( " OE2" );
				Atom aa2_CT  = aa2.AtomOfType( " CG " );

				if( aa1_OE1 != null &&
					aa1_OE2 != null &&
					aa2_CT  != null &&
					aa2_OE1 != null &&
					aa2_OE2 != null && 
					aa2_CT  != null
					)
				{
					// all the required atoms exist
					m_Midpoint1.SetToCentrePoint( aa1_OE1, aa1_OE2 );
					m_Midpoint2.SetToCentrePoint( aa2_OE1, aa2_OE2 );

					float distance = m_Midpoint1.distanceTo( m_Midpoint2 );
					if( distance < 7.0f )
					{
						m_AA1_CToOMid.SetToAMinusB( m_Midpoint1, aa1_CT );
						m_AA2_CToOMid.SetToAMinusB( m_Midpoint2, aa2_CT );

//						Position.DebugReport( new Position(), 'S' );
//
//						Position.DebugReport( m_AA1_CToOMid, 'C' );
//						Position.DebugReport( aa1_OE1, 'O' );
//						Position.DebugReport( aa1_OE2, 'O' );
//						Position.DebugReport( m_Midpoint1, 'Q' );
//						Position.DebugReport( aa1_CT, 'N' );
//
//						Position.DebugReport( m_AA2_CToOMid, 'C' );
//						Position.DebugReport( aa2_OE1, 'O' );
//						Position.DebugReport( aa2_OE2, 'O' );
//						Position.DebugReport( m_Midpoint2, 'Q' );
//						Position.DebugReport( aa2_CT, 'N' );

						// calculate the angle between the two CG to OMidPoint vectors
						double angle = Vector.AngleBetween( m_AA1_CToOMid, m_AA2_CToOMid );
						double angle2 = Vector.AngleBetween_Degrees( m_AA1_CToOMid, m_AA2_CToOMid );

						// valid pair
						m_SB.Remove(0,m_SB.Length); // clear the report string
						m_SB.Append( PDBID );
						m_SB.Append( '\t' );
						m_SB.AppendFormat( "{0,4:G}", aa1.Parent.Count );
						m_SB.Append( '\t' );
						m_SB.Append( ext.ExptlReslnMethod.ToString() );
						m_SB.Append( '\t' );
						m_SB.AppendFormat( "{0,-5:N}", ext.Resolution );
						m_SB.Append( '\t' );
						m_SB.AppendFormat( "{0,-5:N}", distance );
						m_SB.Append( '\t' );
						m_SB.AppendFormat( "{0:#.##}", angle2 );
						m_SB.Append( '(' );
						m_SB.AppendFormat( "{0:#.##}", angle );
						m_SB.Append( ")\t" );
						m_SB.Append( aa1.Name );
						m_SB.Append( ' ' );
						m_SB.AppendFormat( "{0,4:G}", aa1.ResidueNumber );
						m_SB.Append( aa1.InsertionCode );
						m_SB.Append( '\t' );
						m_SB.Append( aa2.Name );
						m_SB.Append( ' ' );
						m_SB.AppendFormat( "{0,4:G}", aa2.ResidueNumber );
						m_SB.Append( aa2.InsertionCode );
						m_SB.Append( '\t' );
						m_SB.Append( aa2.ArrayIndex - aa1.ArrayIndex );
						m_SB.Append( '\t' );

						// report the data
						string reportLine = m_SB.ToString();
						Console.WriteLine( reportLine );
						m_RW.WriteLine( reportLine );
					}
				}
			}
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			string pathToPDB = @"C:\Main Backup 21.03.05\_My Calculations\09d - My Loop DataBase\PDBSelectRevamp2\ProcessedChains_1.8\";
			Class1 c = new Class1();
			c.Go( new DirectoryInfo(pathToPDB) );
		}
	}
}
