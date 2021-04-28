using System;
using System.Collections;
using System.Diagnostics;

using UoB.Core.Primitives;

namespace UoB.Core.Primitives.Collections
{
	/// <summary>
	/// Summary description for ParticleSystem PS_StringStore.
	/// </summary>

	public class StringStore : IndexedStore
	{

		public StringStore( int arrayLength ) : base( arrayLength )
		{
		}

        public new string this[int index] 
		{
			get 
			{
                return (string) m_InternalArray[index];
			}
		}

        public string GetString(int index)
		{
			if( index >= m_InternalArray.Count )
			{
				throw new Exception("Requested index was out of bounds");
			}
			else
			{
                return (string) m_InternalArray[index];
			}
		}

        public virtual void AddString(string addstring)
		{
            m_InternalArray.Add(addstring);
		}

        public string currentString
		{
			get
			{
                return (string)m_InternalArray[m_Position];
			}
		}
	}
}
