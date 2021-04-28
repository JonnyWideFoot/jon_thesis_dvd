using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class Position
{
    public Position(double _x, double _y, double _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public double x;
    public double y;
    public double z;
}

namespace sphere
{
    class Program
    {
        static void Main(string[] args)
        {
            string root = @"C:\_sphere\";

            DirectoryInfo di = new DirectoryInfo(root);
            if (!di.Exists)
            {
                Console.WriteLine("Path does not exist");
                return;
            }

            FileInfo[] files = di.GetFiles("elec*.txt");

            List<Position> pos = new List<Position>();

            for( int j = 0; j < files.Length; j++ )
            {
                pos.Clear();                

                string name = files[j].Name;
                string[] points = name.Split('.');
                if (points.Length != 4)
                {
                    Console.WriteLine("Filename is invalid");
                    return;
                }

                StreamReader re = new StreamReader(files[j].FullName);

                string line;
                while (true)
                {
                    if (null == (line = re.ReadLine())) break;
                    double x = Double.Parse(line);
                    if (null == (line = re.ReadLine())) break;
                    double y = Double.Parse(line);
                    if (null == (line = re.ReadLine())) break;
                    double z = Double.Parse(line);
                    pos.Add(new Position(x, y, z));
                }

                re.Close();

                StreamWriter rw = new StreamWriter(root + "sphere." + points[2] + ".csv");
                StreamWriter rwPDB = new StreamWriter(root + "sphere." + points[2] + ".pdb");
                double fac = 30.0; // the scaling factor to make the PDB points viewable
                for (int i = 0; i < pos.Count; i++)
                {
                    rw.WriteLine("{0:F5},{1:F5},{2:F5}", pos[i].x, pos[i].y, pos[i].z);
                    rwPDB.WriteLine("ATOM  {0,5:D}  N   MOL A   1    {1,8:F3}{2,8:F3}{3,8:F3}  1.00  1.00           N",
                      i, pos[i].x * fac, pos[i].y * fac, pos[i].z * fac);
                }
                rw.Close();
                rwPDB.Close();
            }

            return;
        }
    }
}
