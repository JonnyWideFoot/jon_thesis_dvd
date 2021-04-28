using System;
using System.Collections;
using System.Diagnostics;
using UoB.Core.Primitives;

namespace UoB.Core.FileIO.FormattedInput
{
	
	public interface element // used purely to make sure only elements can be added to the FileObject ArrayList
	{
		Array data
		{
			get;
		}
	}

	public class FileObject
	{
		private string m_Name = "Undefined";
		private bool m_NameHasBeenSet = false;

		private ArrayList m_Elements;

		public FileObject()
		{
			m_Elements = new ArrayList();
		}

		public string name
		{
			get
			{
				return m_Name;
			}
		}

		public int Count
		{
			get
			{
				return m_Elements.Count;
			}
		}

		public void assignName(string name)
		{
			if ( m_NameHasBeenSet == true )
			{
				Debug.WriteLine("Warning : Name of File is being set again ...");
			}
			m_NameHasBeenSet = true;
			m_Name = name;            
		}

		public void addElement(  element theElement )
		{
			m_Elements.Add( theElement );
		}

		public element this[int index] 
		{
			get 
			{
				return((element)m_Elements[index]);
			}
			set 
			{
				m_Elements[index] = value;
			}
		}
	}

	public struct elementString : element
	{
		public string name;
		public string dataString;
		public Array data
		{
			get
			{
				string[] s = new string[1];
				s[0] = dataString;
				return s;
			}
		}
	}

	public struct elementNamedVector : element
	{
		public string name;
		public Vector vector;
		public Array data
		{
			get
			{
				Vector[] s = new Vector[1];
				s[0] = vector;
				return s;
			}
		}
	}

	public struct elementNamedVectorArray : element
	{
		public string name;
		public elementNamedVector[] namedVectors;
		public Array data
		{
			get
			{
				return namedVectors;
			}
		}
	}

	public struct elementNamedFloatArray : element
	{
		public string name;
		public elementNamedFloat[] namedFloats;
		public Array data
		{
			get
			{
				return namedFloats;
			}
		}
		public float[] getFloatArray()
		{
			float[] returnArray = new float[ namedFloats.Length ];
			for ( int i = 0; i < returnArray.Length; i++ )
			{
				returnArray[i] = namedFloats[i].dataFloat;
			}
			return returnArray;
		}
	}

	public struct elementNamedFloat : element
	{
		public string name;
		public float dataFloat;
		public Array data
		{
			get
			{
				float[] f = new float[1];
				f[0] = dataFloat;
				return f;
			}
		}
	}
}
