using System;
using System.IO;

namespace UoB.Core.Structure.Alignment
{
	/// <summary>
	/// Summary description for Options.
	/// </summary>
	public class Options
	{
		public virtual void WriteToStream( StreamWriter rw )
		{
			rw.WriteLine("Option Set used was of the base (null options) type");
		}
	}
}
