using System;

using UoB.Core.FileIO.FormattedInput;

namespace UoB.Core.FileIO.Raft
{
	/// <summary>
	/// Summary description for RaftInpFile.
	/// </summary>
	public class RaftInpFile
	{
		private RaftInpFile()
		{
		}

		public static void WriteRaftInpFile( string templateDir, string autoDir, string jobStem, string[] keys, string[] values )
		{
			InputFile.Create( templateDir + "inp.templ", autoDir + jobStem + ".inp", keys, values );
		}
	}
}
