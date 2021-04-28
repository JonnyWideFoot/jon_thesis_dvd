using System;

namespace UoB.External.Common
{
	/// <summary>
	/// Summary description for BuilderException.
	/// </summary>
	public class InteropException : Exception
	{
		public InteropException( string error ) : base ( error )
		{
		}

		public override string ToString()
		{
			return Message;
		}
	}
}
