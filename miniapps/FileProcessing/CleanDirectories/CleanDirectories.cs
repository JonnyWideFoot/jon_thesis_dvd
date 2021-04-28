using System;
using System.IO;
using System.Reflection;

namespace CleanDirectories
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			FileInfo fi = new FileInfo( Assembly.GetEntryAssembly().Location );
			DirectoryInfo di = fi.Directory; // the directory where the executing file is located
			AssessDir( di );

			Console.WriteLine();
			Console.WriteLine("Press any key to continue...");
			Console.ReadLine();
		}

		private static void AssessDir( DirectoryInfo di )
		{
			
			if( String.Compare(di.Name,"obj",true) == 0 )
			{
				try
				{
					di.Delete(true);
				}
				catch
				{
					Console.WriteLine("Fail on delete of: " + di.FullName );
				}
			}
			else if( String.Compare(di.Name,"_Compile",true) == 0  )
			{
				FullClearDirExceptShared( di );
			}
			else
			{
				DirectoryInfo[] diChildren = di.GetDirectories();
				for( int i = 0; i < diChildren.Length; i++ )
				{
					AssessDir( diChildren[i] );
				}
			}
		}

		private static void FullClearDirExceptShared( DirectoryInfo diCompile )
		{
			DirectoryInfo[] diChildren = diCompile.GetDirectories();
			for( int i = 0; i < diChildren.Length; i++ )
			{
				DirectoryInfo di = diChildren[i];
				if( String.Compare(di.Name,"Shared",true) == 0 )
				{
					// leave well alone, all good :D ... except the temp dir ..
					string tempPath = di.FullName + Path.DirectorySeparatorChar + "temp";
					if( Directory.Exists( tempPath ) )
					{
						try
						{
							Directory.Delete( tempPath, true );
						}
						catch
						{
							Console.WriteLine("Fail on delete of: " + di.FullName );
						}
					}
				}
				else
				{
					try
					{
						di.Delete(true);
					}
					catch
					{
						Console.WriteLine("Fail on delete of: " + di.FullName );
					}
				}
			}
		}
	}
}
