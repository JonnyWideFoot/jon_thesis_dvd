using System;
using System.IO;
using System.Diagnostics;
using System.Text;

using UoB.Core.Structure;
using UoB.Core.ForceField;

namespace UoB.Core.FileIO.PQR
{
	/// <summary>
	/// Summary description for PQR.
	/// </summary>
	public sealed class PQR
	{
		private PQR()
		{
		}

		public static void outputPQRFile( string filePath, ParticleSystem ps, int radiusSet, float[] charges )
		{
			if ( ps.Count != charges.Length )
			{
				Trace.WriteLine( "Charge Array does not contain the same number of entries as the particleSystem" );
				return;
			}

			StreamWriter rw = new StreamWriter(filePath, false);

			for ( int i = 0; i < ps.Count; i++ )
			{
				rw.WriteLine( getPQRLine( ps[i], charges[i], i ) );
			}

			rw.WriteLine("");
			rw.WriteLine("TER");
			rw.Close();
		}

		private static string getPQRLine( Atom atom, float charge, int lineNumber )
		{
			//ATOM     22 1HE2 GLN     1      -0.252   2.494  11.660   0.42510   1.15000

			if( lineNumber > 99 ) throw new OverflowException("You can only have a max of 99 atoms");

			StringBuilder sb = new StringBuilder("ATOM  ", 80);
			sb.Append( atom.AtomNumber.ToString().PadLeft(5,' ') );
			sb.Append("  ");

			char element = atom.PDBType[1];

			sb.Append( ( element.ToString() + atom.AtomNumber.ToString() ).PadRight(3,' ') );

			sb.Append(" ");
			sb.Append( atom.parentMolecule.Name.PadRight(3,' ').Substring(0,3) );
			sb.Append( atom.parentMolecule.ResidueNumber.ToString().PadLeft(6,' ') );
			sb.Append( atom.xFloat.ToString("0.000").PadLeft(12,' ') );
			sb.Append( atom.yFloat.ToString("0.000").PadLeft(8,' ') );
			sb.Append( atom.zFloat.ToString("0.000").PadLeft(8,' ') );
			sb.Append( charge.ToString("0.00000").PadLeft(10,' ') );
			sb.Append( atom.Radius.ToString("0.00000").PadLeft(10,' ') );

			return sb.ToString();
		}
	}
}
