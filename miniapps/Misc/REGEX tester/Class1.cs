using System;
using System.Text;
using System.Text.RegularExpressions;


namespace REGEX_tester
{
	class Class1
	{
		[STAThread]
		static void Main(string[] args)
		{
            Regex r = new Regex(@"\s+");// allows split by whitespace
            string bla = "uioehfpe  e eoifhe foefephfe euiohpe ";
            string[] parts = r.Split(bla);
            for (int i = 0; i < parts.Length; i++)
            {
                Console.WriteLine(parts[i]);
            }
            Console.WriteLine("Split, press any key to continue...");
            Console.Read();
		}
    }
}
