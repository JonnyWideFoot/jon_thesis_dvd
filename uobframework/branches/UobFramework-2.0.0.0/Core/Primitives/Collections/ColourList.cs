using System;
using System.Collections;

namespace UoB.Core.Primitives.Collections
{
	/// <summary>
	/// Summary description for ColourList.
	/// </summary>
	public class ColourList : IEnumerable
	{
		private ArrayList m_Colours;

		public ColourList()
		{
			m_Colours = new ArrayList();
		}

		public void addColour( Colour colour )
		{
			m_Colours.Add( colour );
		}

		public int Count
		{
			get
			{
				return m_Colours.Count;
			}
		}

		public void Clear()
		{
			m_Colours.Clear();
		}

		public Colour this[int index] 
		{
			get 
			{
				return (Colour) m_Colours[index];
			}
			set 
			{
				m_Colours[index] = value;
			}
		}		
		
		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return new ColourListEnumerator(this);
		}

		private class ColourListEnumerator : IEnumerator
		{
			private int position = -1;
			private ColourList ownerCL;

			public ColourListEnumerator(ColourList theCL)
			{
				ownerCL = theCL;
			}

			// Declare the MoveNext method required by IEnumerator:
			public bool MoveNext()
			{
				if (position < ownerCL.Count - 1)
				{
					position++;
					return true;
				}
				else
				{
					return false;
				}
			}

			// Declare the Reset method required by IEnumerator:
			public void Reset()
			{
				position = -1;
			}

			// Declare the Current property required by IEnumerator:
			public object Current
			{
				get
				{
					return ownerCL[position];
				}
			}
		}

		#endregion
	}
}
