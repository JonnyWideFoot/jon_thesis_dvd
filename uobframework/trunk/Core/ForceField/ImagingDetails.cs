using System;
using System.Collections;
using UoB.Core.Primitives;
using UoB.Core.Structure.Primitives;
using UoB.Core.Structure;

namespace UoB.Core.ForceField
{
	/// <summary>
	/// Summary description for ImagingDetails.
	/// </summary>
	public class ImagingDetails
	{
		private Hashtable m_Colours;
		private Hashtable m_SelectedNumbers;
		private Hashtable m_DeselectedNumbers;

		public ImagingDetails()
		{
			m_SelectedNumbers = new Hashtable();
			m_DeselectedNumbers = new Hashtable();
			m_Colours = new Hashtable();
		}

		public void addAtomTypeDefinition( string AtomType, Colour c, int imageSelNum, int imageDeselNum )
		{
			m_SelectedNumbers.Add( AtomType, imageSelNum );
			m_DeselectedNumbers.Add( AtomType, imageDeselNum );
			m_Colours.Add( AtomType, c );
		}

		public int getImageNumSel( AtomPrimitiveBase primitive )
		{
			if ( (primitive is AtomPrimitive) && m_SelectedNumbers.ContainsKey( primitive.ForceFieldID ) )
			{
				return (int) m_SelectedNumbers[ primitive.ForceFieldID ];
			}
			else
			{
				return 7; // unknown atom picture
			}
		}
		public int getImageNumDesel( AtomPrimitiveBase primitive )
		{
			if ( (primitive is AtomPrimitive) && m_DeselectedNumbers.ContainsKey( primitive.ForceFieldID ) )
			{
				return (int) m_DeselectedNumbers[ primitive.ForceFieldID ];
			}
			else
			{
				return 7;
			}
		}

		public static readonly Colour cDarkGrey = Colour.FromName("DarkGray");
		public static readonly Colour cBlue = Colour.FromName("Blue");
		public static readonly Colour cRed = Colour.FromName("Red");
		public static readonly Colour cYellow = Colour.FromName("Yellow");
		public static readonly Colour cGreen = Colour.FromName("Green");

		public void SetAtomColour( Atom atom, Colour colour )
		{
			Colour c;

			if ( m_Colours.ContainsKey( atom.atomPrimitive.ForceFieldID ) )
			{
				c = (Colour) m_Colours[ atom.atomPrimitive.ForceFieldID ];
			}
			else
			{
				// guess colour
				if ( atom.PDBType[1] == 'C' )
				{
					c = cDarkGrey;
				} 
				else if ( atom.PDBType[1] == 'N' )
				{
					c = cBlue;
				}
				else if ( atom.PDBType[1] == 'O' )
				{
					c = cRed;
				}
				else if ( atom.PDBType[1] == 'S' )
				{
					c = cYellow;
				}
				else
				{
					c = cGreen;
				}
			}

			colour.Red = c.Red;
			colour.Blue = c.Blue;
			colour.Green = c.Green;
		}
	}
}
