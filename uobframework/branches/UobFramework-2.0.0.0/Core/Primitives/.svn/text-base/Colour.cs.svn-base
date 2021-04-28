using System;
using System.Drawing;

namespace UoB.Core.Primitives
{
	/// <summary>
	/// Summary description for Colour.
	/// </summary>
	public class Colour
	{
		private float m_Red;
		private float m_Blue;
		private float m_Green;

		private static readonly Colour[] StandardColouration = new Colour[] {
											new Colour( 255, 0, 0 ),	// red
											new Colour( 0, 0, 255 ),	// blue
											new Colour( 255, 255, 0 ),	// yellow
											new Colour( 0, 255, 0 ),	// green
											new Colour( 0, 255, 255 ),	// cyan
											new Colour( 255, 255, 0 ),	// magenta
											new Colour( 255, 123, 0 ),	// orange
											new Colour( 0, 255, 200 )	// turquoise
										};

		public static Colour FromIntID( int colourID )
		{
			Colour c = new Colour(0,0,0);
			c.SetToIntID( colourID );
			return c;
		}

		public static Colour FromIntegerRep( int integerRep )
		{
			Colour c = new Colour(0,0,0);
			c.IntegerRep = integerRep;
			return c;
		}

		private static readonly int findRMask = 16711680; //  255 << 16
		private static readonly int findGMask = 65280;  //  255 << 8
		private static readonly int findBMask = 255;   //  255 << 0
		public int IntegerRep
		{
			get
			{
				int playByte = 0; // 00000000-00000000-00000000-0000000 // the bitmask
				playByte = playByte | (Red << 16);
				playByte = playByte | (Green << 8);
				playByte = playByte | Blue; // << 0
				return playByte;
			}
			set
			{

				Red = (findRMask & value) >> 16;
				Green = (findGMask & value) >> 8;
				Blue = findBMask & value;                
			}
		}
        
		public void SetToIntID( int colourID )
		{
			if( colourID >= 0 )
			{
				if( colourID < 100 ) // we want a colour from the list
				{
					colourID = colourID % StandardColouration.Length;
					SetTo( StandardColouration[ colourID ] );
				}
				else
				{
					int indexID1 = (colourID / 100) - 1; // how many nundreds do we have? this picks the colour
					indexID1 = indexID1 % StandardColouration.Length;
					int indexID2 = indexID1 + 1;
					if( indexID2 == StandardColouration.Length )
					{
						indexID2 = 0;
					}
					float percentage = ((float)(colourID % 100)) / 100.0f;
					m_Red = ( StandardColouration[indexID1].RedOver255 + ( StandardColouration[indexID1].RedOver255 - StandardColouration[indexID2].RedOver255 ) ) * percentage;
					m_Blue = ( StandardColouration[indexID1].BlueOver255 + ( StandardColouration[indexID1].BlueOver255 - StandardColouration[indexID2].BlueOver255 ) ) * percentage;
					m_Green = ( StandardColouration[indexID1].GreenOver255 + ( StandardColouration[indexID1].GreenOver255 - StandardColouration[indexID2].GreenOver255 ) ) * percentage;
				}
			}
		}

		public Colour(int red, int green, int blue) // ints give RGB values
		{
			m_Red = oneOver255 * Validate(red);
			m_Green = oneOver255 * Validate(green);
			m_Blue = oneOver255 * Validate(blue);
		}

		public void SetTo( Colour donor )
		{
			m_Red = donor.RedOver255;
			m_Blue = donor.BlueOver255;
			m_Green = donor.GreenOver255;
		}

		public Colour(float red, float green, float blue) // floats give RGB fractions of 1
		{
			m_Red = ValidateFloat(red);
			m_Green = ValidateFloat(green);
			m_Blue = ValidateFloat(blue);
		}

		private int Validate(int colour)
		{
			if ( colour > 255 ) return 255;
			if ( colour < 0 ) return 0;
			return colour;
		}

		private float ValidateFloat(float colour)
		{
			if ( colour > 1.0f ) return 1.0f;
			if ( colour < 0 ) return 0;
			return colour;
		}

		public string Name
		{
			get
			{
				return color.Name; // possibly ???
			}
		}

		public Colour Clone()
		{
            return new Colour(m_Red,m_Green,m_Blue);
		}

		public static Colour[] GetGradient(int arrayLength, Colour colour1, Colour colour2)
		{
			Colour[] gradient = new Colour[arrayLength];

			float redDiff = (float)(colour2.Red - colour1.Red) / arrayLength;
			float greenDiff = (float)(colour2.Green - colour1.Green) / arrayLength;
			float blueDiff = (float)(colour2.Blue - colour1.Blue) / arrayLength;

			for ( int i = 0; i < arrayLength; i++ )
			{
				gradient[i] = new Colour(
					colour1.Red + (int)(redDiff * i),
					colour1.Green + (int)(greenDiff * i),
					colour1.Blue + (int)(blueDiff * i)
				);
			}

			return gradient;
        }

		public static Colour FromColor( Color color )
		{
			return new Colour(	
				Convert.ToInt32( Convert.ToInt32(color.R) ),
				Convert.ToInt32( Convert.ToInt32(color.G) ),
				Convert.ToInt32( Convert.ToInt32(color.B) )
				);
		}

		public static Colour FromName( string name )
		{
			Color c = Color.FromName(name);
			return new Colour(	
				Convert.ToInt32( c.R ),
				Convert.ToInt32( c.G ),
				Convert.ToInt32( c.B )
				);
		}

		public static Colour FromKnownColor( KnownColor kc )
		{
			Color c = Color.FromKnownColor(kc);
			return new Colour(	
				Convert.ToInt32( c.R ),
				Convert.ToInt32( c.G ),
				Convert.ToInt32( c.B )
				);
		}

		public Color color
		{
			get
			{
				return Color.FromArgb(Red, Green, Blue);
			}
			set
			{
				m_Red = oneOver255 * Validate( Convert.ToInt32( value.R ) );
				m_Green = oneOver255 * Validate( Convert.ToInt32( value.G ) );
				m_Blue = oneOver255 * Validate( Convert.ToInt32( value.B ) );
			}

		}

		private const float oneOver255 = ( 1.0f / 255.0f );

		public float RedOver255
		{
			get
			{
				return m_Red;
			}
			set
			{
				m_Red = ValidateFloat( value );
			}
		}

		public float GreenOver255
		{
			get
			{
				return m_Green;
			}
			set
			{
				m_Green = ValidateFloat( value );
			}
		}

		public float BlueOver255
		{
			get
			{
				return m_Blue;
			}
			set
			{
				m_Blue = ValidateFloat( value );
			}
		}

		public int Red
		{
			get
			{
				return (int)(m_Red / oneOver255);
			}
			set
			{
				m_Red = oneOver255 * Validate( value );
			}
		}

		public int Green
		{
			get
			{
				return (int)(m_Green / oneOver255);
			}
			set
			{
				m_Green = oneOver255 * Validate( value );
			}
		}

		public int Blue
		{
			get
			{
				return (int)(m_Blue / oneOver255);
			}
			set
			{
				m_Blue = oneOver255 * Validate( value );
			}
		}
	}
}
