using System;
using System.Collections;

using UoB.Core.Primitives;
using UoB.Core.ForceField.Definitions;

namespace UoB.Core.Structure.Primitives
{
	/// <summary>
	/// Summary description for AtomPrimitive.
	/// </summary>
	public class AtomPrimitiveBase
	{
		protected string m_AltName;
		protected string[] m_BondingPartnerAltIDs = new string[] {}; // null bonding partners
		protected string m_PDBName;
		protected float  m_DefaultCharge;

		public AtomPrimitiveBase( string fileReadinName )
		{
            string store = fileReadinName;

			m_AltName = m_PDBName = MakeValidPDBName( fileReadinName );

            if (m_PDBName == "  H ")
            {
                m_AltName = m_PDBName = MakeValidPDBName(store);
                return;
            }
		}

		public static string MakeValidAltname( string altName )
		{
			if ( altName.Length > 4 )
			{
				altName = altName.Substring(0,4);
			}
			else if ( altName.Length < 4 ) 
			{
				altName = altName.PadRight(4,' ');
			}
			return altName.ToUpper();
		}

		public static string MakeValidPDBName( string pdbName )
		{
			pdbName = pdbName.PadRight(4,' ');

			if ( !char.IsNumber( pdbName[0] ) )
			{
				// the PDBID doesnt have a number as the first character, therefore they all have to move along one ...
				// i.e. "N   " should be " N  " ... but "1H  " should stay as it is ...
				// then additional check to take "HH12" into account
				if ( pdbName[0] != ' ' && 
                    !( 
					char.IsLetterOrDigit( pdbName[0] ) &&
					char.IsLetterOrDigit( pdbName[1] ) &&
					char.IsLetterOrDigit( pdbName[2] ) &&
					char.IsLetterOrDigit( pdbName[3] )  
					) )
				{
					pdbName = " " + pdbName;
				}
			}
			if ( pdbName.Length > 4 ) 
			{
				pdbName = pdbName.Substring(0,4);
			}
			else if ( pdbName.Length < 4 ) 
			{
				pdbName = pdbName.PadRight(4,' ');
			}
			return pdbName.ToUpper();
		}


		public string AltName
		{
			get
			{
				return m_AltName;
			}
		}

		public string PDBIdentifier
		{
			get
			{
				return m_PDBName;
			}
		}
		
		public virtual string ForceFieldID
		{
			get
			{
				return "  "; // blank it, we have no idea what it is
			}
		}

		public virtual bool IsBackBone
		{
			get
			{
				return false; // It will never be a valid backbone atom if its not defined in the forcefield file
			}
		}

		public virtual string[] BondingPartners
		{
			get
			{
				return m_BondingPartnerAltIDs; // initialised to a 0 length array above
			}
		}

		public virtual AtomType FFAtomType
		{
			get
			{
				return AtomType.NullType;
			}
		}

		public virtual char Element
		{
			get
			{
                if (m_PDBName[0] == 'H')
                {
                    return 'H'; // Names like 'HG22'
                }
                else
                {
                    return m_PDBName[1]; // guess it from the input name, might be wrong, but we only really use this for colour prediction
                }
			}
		}

		public virtual float Radius
		{
			get
			{
				return -1.0f;
			}
		}
		
		public virtual float DefaultCharge
		{
			get
			{
				return -1.0f;
			}
			set
			{
				m_DefaultCharge = value;
			}
		}
	}
}
