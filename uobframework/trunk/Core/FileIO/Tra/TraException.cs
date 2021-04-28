using System;

namespace UoB.Core.FileIO.Tra
{
	/// <summary>
	/// Summary description for BuilderException.
	/// </summary>
	public class TraException : Exception
	{
		public TraException( string error ) : base ( error )
		{
		}

		public override string ToString()
		{
			return Message;
		}
	}
}
