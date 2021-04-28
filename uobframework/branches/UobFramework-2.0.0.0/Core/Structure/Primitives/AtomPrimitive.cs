using System;

using UoB.Core.Primitives;
using UoB.Core.FileIO.PDB;
using UoB.Core.ForceField;
using UoB.Core.ForceField.Definitions;

namespace UoB.Core.Structure.Primitives
{
	/// <summary>
	/// Summary description for AtomPrimitive.
	/// </summary>
	public class AtomPrimitive : AtomPrimitiveBase
	{
		// typical line in the ff defs file
		//ATOM   N      N      0.000  0.000  0.000   N          -C,CA,H

		protected MoleculePrimitive m_Parent;
		protected Position m_DefaultPosition; // never actually used, part of the mike definition
		protected float  m_Radius;
		protected bool   m_IsBackBone;
		protected string m_ForceFieldID;
		protected AtomType m_AtomType; // needs to be set following the initialisation of the FFManager

		protected FFManager m_FFParams;

		public AtomPrimitive( MoleculePrimitive parent, string altName, string pdbName, Position defaultPosition, string ffID, string bondingPartners )
		: base( pdbName ) // really fast, but pointless, initialisation, we overwrite what it does in a second
		{
			//m_FFParams = Params.Instance; // cant do this as AtomPrimitives are used during the initialisation of Params.Instance ...
			m_Parent = parent;
			m_DefaultPosition = defaultPosition;

			// sort out the naming definitions. As they are extracted by whitespace delimitation, they may not be of length 4. We need to make sure that that is so ...
			m_AltName = MakeValidAltname( altName );
			m_PDBName = MakeValidPDBName( pdbName );
			m_ForceFieldID = ffID; // should be of length 2
			setNameIsBackbone(); // uses m_PDBName
					
			m_BondingPartnerAltIDs = bondingPartners.Split(',');
			for ( int i = 0; i < m_BondingPartnerAltIDs.Length; i++ )
			{
				m_BondingPartnerAltIDs[i] = MakeValidAltname( m_BondingPartnerAltIDs[i] );
			}
		}


		public void FinaliseStage2()
		{
			// as atomPrimitives are initialised with the FFManager, this cant be set in the constructor
			// so we set the remaining items that are FF initialisation dependent here. This function must be called
			// following completion of the singleton initialisation of the FFManager class

			m_FFParams = FFManager.Instance;
			m_AtomType = m_FFParams.AtomTypes.GetTypeFromFFID( ref m_ForceFieldID );
		}

		public void setNameIsBackbone()
		{
			if(    0 == String.Compare( PDBAtom.PDBID_BackBoneN, 0, m_PDBName, 0, 4, false )
				|| 0 == String.Compare( PDBAtom.PDBID_BackBoneC, 0, m_PDBName, 0, 4, false )
				|| 0 == String.Compare( PDBAtom.PDBID_BackBoneCA, 0, m_PDBName, 0, 4, false )
				|| 0 == String.Compare( PDBAtom.PDBID_BackBoneO, 0, m_PDBName, 0, 4, false )

				// Ive changed my mind on these ... ;-)
				//|| 0 == String.Compare( PDBAtom.PDBID_BackBoneH, 0, m_PDBName, 0, 4, false )
				//|| 0 == String.Compare( PDBAtom.PDBID_BackBone1HA, 0, m_PDBName, 0, 4, false )
				//|| 0 == String.Compare( PDBAtom.PDBID_BackBone2HA, 0, m_PDBName, 0, 4, false )
				)
			{
				m_IsBackBone = true;
			}
			else
			{
				m_IsBackBone = false;
			}
		}

		public override string ForceFieldID
		{
			get
			{
				return m_ForceFieldID;
			}
		}

		public override bool IsBackBone
		{
			get
			{
				return m_IsBackBone;
			}
		}

		public override string[] BondingPartners
		{
			get
			{
				return m_BondingPartnerAltIDs;
			}
		}

		public override AtomType FFAtomType
		{
			get
			{
				return m_AtomType;
			}
		}

		public override char Element
		{
			get
			{
				return ForceFieldID[0]; // a slight assumption ?!?
			}
		}
	
		public override float DefaultCharge
		{
			get
			{
				return m_DefaultCharge;
			}
			set
			{
				m_DefaultCharge = value;
			}
		}

	}
}
