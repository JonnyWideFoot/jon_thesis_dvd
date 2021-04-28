using System;

namespace UoB.Core
{
	/// <summary>
	/// Summary description for Exceptions.
	/// </summary>
	public class NoImplementationException : Exception
	{
		public NoImplementationException() : base()
		{
		}

		public NoImplementationException( string message ) : base( message )
		{
		}
	}

    class CodeException : Exception
    {
        public CodeException()
            : base()
        {
        }

        public CodeException(string message)
            : base(message)
        {
        }
    }

    class ProcedureException : Exception
    {
        public ProcedureException()
            : base()
        {
        }

        public ProcedureException(string message)
            : base(message)
        {
        }
    }

    class ParseException : Exception
    {
        public ParseException()
            : base()
        {
        }

        public ParseException(string message)
            : base(message)
        {
        }
    }
}
