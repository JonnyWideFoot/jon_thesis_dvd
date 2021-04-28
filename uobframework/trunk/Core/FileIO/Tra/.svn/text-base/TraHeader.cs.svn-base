using System;
using System.IO;

namespace UoB.Core.FileIO.Tra
{
	/// <summary>
	/// Summary description for Header of a Tra File.
	/// </summary>
	public class TraHeader
	{
		public const int HEADER_BYTESIZE = 18084;
		// =(9*4) + (4*32) + (32*16) + (64*16) + 1024 + (1024*15)

		public int      version;                        // version - currently 2
		public int      type;                           // type descriptors of what is contained within the file
		public int      residues;                       // nr of residues
		public int      atoms;                          // nr of atoms
		public int      blocksize;                      // blocksize of trajectory entry
		public int      trajectorystart;                // byte position of the start of first trajectory entry
		public int      dateday,datemonth,dateyear;     // date
		public string   ID0;                            // Length 32 - custom ID strings
		public string   ID1;
		public string   ID2;
		public string   ID3;
		public char[,]  customAtomProperty;             // Length 32 * 16   - names of custom Properties
		public char[,]  customEnergyEntry;              // Length 64 * 16   - names of custom energies
		public string   descriptor;                     // Length 1024      - custom 1K descriptor ASCII area
		public string   text;                           // Length 1024 * 15 - custom 15K descriptor ASCII area

		// custom format extensions
		public int      extnesion_EXTDVECT = 0; // the number of extended vectors to expect per time step
		public int      extension_EXTDCMNT = 0; // the size of the extended comment buffer. 0 if it doesnt exist.
		// more here later ....

		public TraHeader( BinaryReader r )
		{
			version = r.ReadInt32();
			type = r.ReadInt32();
			residues = r.ReadInt32();
			atoms = r.ReadInt32();
			blocksize = r.ReadInt32();
			trajectorystart = r.ReadInt32();
			if( trajectorystart < HEADER_BYTESIZE )
			{
				throw new Exception("TraHeader read error: The trajectory start is within the scope of the header!" );
			}
			dateday = r.ReadInt32();
			datemonth = r.ReadInt32();
			dateyear = r.ReadInt32();
			ID0 = new string ( Tra.getCharArray(r,32) );
			ID1 = new string ( Tra.getCharArray(r,32) );
			ID2 = new string ( Tra.getCharArray(r,32) );
			ID3 = new string ( Tra.getCharArray(r,32) );
			customAtomProperty = Tra.getCharArray(r,32,16);
			customEnergyEntry =  Tra.getCharArray(r,64,16);
			descriptor = new string ( Tra.getCharArray( r,1024 ) );
			text = new string ( Tra.getCharArray( r, 1024*15 ) );
		}	
	}

}
