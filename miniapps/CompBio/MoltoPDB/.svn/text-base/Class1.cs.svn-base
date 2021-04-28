using System;
using System.IO;


namespace MoltoPDBCommandPrompt
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
			convert(@"x:\");
		}



		public static void convert(string fullPath)
		{
			char[] xAr = new char[8];
			char[] yAr = new char[8];
			char[] zAr = new char[8];
			char[] typeAr = new char[1];
			string x;
			string y;
			string z;
			string type;

			foreach ( string file in Directory.GetFiles(fullPath, "*.mol") )
			{
				int i = 0;
				Console.WriteLine(file);
				StreamReader re = File.OpenText(file);
				string[] fileParts = file.Split('.');
				string outfile = fileParts[0] + ".pdb";
				StreamWriter rw = new StreamWriter(outfile, false);

				string inputLine = null;
				while ((inputLine = re.ReadLine()) != null)
				{
					char[] theLine = new char[80];
					if (inputLine.Length < 80)
					{
						inputLine = inputLine.PadRight(80);
					}
					theLine = inputLine.ToCharArray(0,80);

					Array.Copy(theLine,2,xAr,0,8);
					Array.Copy(theLine,12,yAr,0,8);
					Array.Copy(theLine,22,zAr,0,8);
					Array.Copy(theLine,31,typeAr,0,1);

					x = new string(xAr);
					y = new string(yAr);
					z = new string(zAr);
					type = new string(typeAr);

					try
					{
						float xf = float.Parse(x);
						float yf = float.Parse(y);
						float zf = float.Parse(z);
					}
					catch
					{
						continue;
					}

					int j = i + 1;
					Console.WriteLine("Atom Num: " + j.ToString() );
					Console.WriteLine(x);
					Console.WriteLine(y);
					Console.WriteLine(z);
					Console.WriteLine("Type: " + type);
					string givenType = Console.ReadLine();

					string output = "HETATM" 
						+ j.ToString().PadLeft(5,' ') 
						+ givenType.PadLeft(5,' ')
						+ " MIS          " // spacer
						+ x
						+ y
						+ z
						;
					output.PadRight(80,' ');

					rw.WriteLine(output);
					i++;
					
				}
				re.Close();
				rw.Close();
			}
			//MessageBox.Show("done");
		}



	}
}
