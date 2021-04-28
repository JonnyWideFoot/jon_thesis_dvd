using System;
using System.Collections;

using UoB.Core.Primitives;

namespace UoB.CoreControls.PS_Render
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public abstract class Selection
	{
		protected String m_Name = "";
		protected ArrayList m_AtomIndexes;
		protected bool m_Active = true;
		protected bool m_Inverted = false;
		protected SelectionColourMode m_ColourMode = SelectionColourMode.ForceFieldDefault;
		protected Colour m_Colour1 = Colour.FromName("Red");
		protected Colour m_Colour2 = Colour.FromName("Blue");
		protected AtomDrawStyle m_DrawStyle = AtomDrawStyle.Lines; // stores the last one set
		protected AtomDisplayMode m_DisplayMode = AtomDisplayMode.AllAtoms; // stores the last one set

		public Selection()
		{
		}

		public void BeginParameterGet()
		{
			m_Counter = 0;
			m_CountFrom = 0;
			if( m_AtomIndexes.Count != 0 && m_Colour2 != null && m_Colour1 != null ) // no divide by zero error
			{
				redDiff = (float)(m_Colour2.Red - m_Colour1.Red) / m_AtomIndexes.Count;
				greenDiff = (float)(m_Colour2.Green - m_Colour1.Green) / m_AtomIndexes.Count;
				blueDiff = (float)(m_Colour2.Blue - m_Colour1.Blue) / m_AtomIndexes.Count;
			}
		}

		public abstract bool Inverted
		{
			get;
			set;
		}

		protected string m_Append
		{
			get
			{
				string append = "";
				if( !m_Active )
				{
					append = "Inactive";
				}
				if( m_Inverted )
				{
					if( append == "" )
					{
						append = "Inverted";
					}
					else
					{
						append += ", Inverted";
					}
				}
				if( !( append == "" ) )
				{
					append += " : ";
				}
				return append;
			}
		}

		private bool m_EasyViewModeOn = false;
		public bool EasyViewModeOn
		{
			get
			{
                return m_EasyViewModeOn;
			}
			set
			{
				m_EasyViewModeOn = value;
			}
		}
		private bool m_IsEasyViewFocus = false;
		public bool IsEasyViewFocus
		{
			get
			{
				return m_IsEasyViewFocus;
			}
			set
			{
				m_IsEasyViewFocus = value;
			}
		}

		public abstract string Name
		{
			get;
			set;
		}

		public override string ToString()
		{
			return m_Name;
		}

		
		private int m_Counter = 0; // used to increment the gradient colour
		private float redDiff = 0.0f;
		private float greenDiff = 0.0f;
		private float blueDiff = 0.0f;

		private int m_CountFrom = 0;

		public void SetColour( AtomDrawWrapper dw, int arrayIndex )
		{
			if(m_EasyViewModeOn)
			{
				if( m_IsEasyViewFocus )
				{
					for( int i = m_CountFrom; i < m_AtomIndexes.Count; i++ )
					{
						if( arrayIndex == (int) m_AtomIndexes[i] )
						{
							dw.colour.Red = 0;
							dw.colour.Green = 255;
							dw.colour.Blue = 0;
							m_CountFrom = i;
							return;
						}
					}
					dw.setDefaultColour();
				}
				else
				{
					dw.setDefaultColour();
				}
			}
			else
			{
				switch( m_ColourMode )
				{
					case SelectionColourMode.Single:

						dw.colour.Red = m_Colour1.Red;
						dw.colour.Green = m_Colour1.Green;
						dw.colour.Blue = m_Colour1.Blue;

						break;

					case SelectionColourMode.Gradient:

						int i = m_Counter++;
						if( m_Counter == m_AtomIndexes.Count )  // we have reached the end of a colouring run
						{
							m_Counter = i = 0;
						}
						dw.colour.Red = m_Colour1.Red + (int)(redDiff * i);
						dw.colour.Green = m_Colour1.Green + (int)(greenDiff * i);
						dw.colour.Blue = m_Colour1.Blue + (int)(blueDiff * i);

						break;

					default: // SelectionColourMode.ForceFieldDefault

						dw.setDefaultColour();

						break;
				}		
			}
		}

		public AtomDrawStyle DrawStyle
		{
			get
			{
				return m_DrawStyle;
			}
			set
			{
				m_DrawStyle = value;
			}
		}

		public AtomDisplayMode DisplayMode
		{
			get
			{
				return m_DisplayMode;
			}
			set
			{
				m_DisplayMode = value;
			}
		}

		public virtual bool IsActive
		{
			get
			{
				return m_Active;
			}
			set
			{
				m_Active = value;
				autoName();
			}
		}

		public abstract void autoName();
		public SelectionColourMode ColourMode
		{
			get
			{
				return m_ColourMode;
			}
			set
			{
				m_ColourMode = value;
			}
		}

		public Colour Colour1
		{
			get
			{
				return m_Colour1;
			}
			set
			{
				m_Colour1 = value;
			}
		}

		public Colour Colour2
		{
			get
			{
				return m_Colour2;
			}
			set
			{
				m_Colour2 = value;
			}
		}

		public int[] AtomIndexes
		{
			get
			{
				return (int[]) m_AtomIndexes.ToArray(typeof(int));
			}
		}
	}
}
