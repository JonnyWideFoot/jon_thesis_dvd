using System;
using System.IO;

namespace UoB.Core.FileIO.Tra
{
	/// <summary>
	/// Summary description for SystemDef.
	/// </summary>
	public class TraParticleDef
	{
		public const int SYSDEF_BYTESIZE = 220;
		public const int SYSDEF_FLAGSTRING_BYTESIZE = 8;

		public int     atomnumber;                    // atom number

		public string  pdbname;                       // Length 8 - pdb name
		public string  primitivetype;                 // Length 8 - primitive forcefield type
		public string  altname;                       // Length 8 - alternative name

		public int     parentnumber;                  // parent (i.e. residue) number
		public string  parentname;                    // Length 8 - parentname

		public float   targetx,targety,targetz;       // Xray strucuture coordinate if known
		public bool    targetknown;                  

		public int[]   cov12atom;                     // Length 6 - atom indices with bondorder 1
		public int     n_cov12atoms;
		public float   charge;
		public float   radius;
		public float[] customProperty;                // Length 32

		public TraParticleDef()
		{
		}

		public void SetFromReader( BinaryReader r )
		{
			atomnumber = r.ReadInt32();
			pdbname = new string ( Tra.getCharArray(r,8) );              // its pdb name
			primitivetype = new string ( Tra.getCharArray(r,8) );        // its primitive atom type
			altname = new string ( Tra.getCharArray(r,8) );              // its alternative forcefield name - aplication use dependent
			parentnumber = r.ReadInt32();           
			parentname = new string ( Tra.getCharArray(r,8) );        
			targetx = r.ReadSingle();
			targety = r.ReadSingle();
			targetz = r.ReadSingle();
			targetknown = Convert.ToBoolean( r.ReadInt32() );
			cov12atom = Tra.getIntArray(r,6);
			n_cov12atoms = r.ReadInt32();
			charge = r.ReadSingle();
			radius = r.ReadSingle();
			customProperty = Tra.getFloatArray(r,32);
		}
	}
}
