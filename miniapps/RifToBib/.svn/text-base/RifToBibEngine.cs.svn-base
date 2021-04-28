using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RifToBib
{
    class BibEntry
    {
        //AU: William J. Wedemeyer, Harold A. Scheraga
        //TI: Exact analytical loop closure in proteins using polynomial equations
        //SO: Journal of Computational Chemistry
        //VL: 20
        //NO: 8
        //PG: 819-844
        //YR: 1999
        //CP: Copyright © 1999 John Wiley & Sons, Inc.
        //ON: 1096-987X
        //PN: 0192-8651
        //AD: Baker Laboratory of Chemistry, Cornell University, Ithaca, New York 14853-1301
        //DOI: 10.1002/(SICI)1096-987X(199906)20:8<819::AID-JCC8>3.0.CO;2-Y
        //US: http://dx.doi.org/10.1002/(SICI)1096-987X(199906)20:8<819::AID-JCC8>3.0.CO;2-Y

        public string Authors = null;
        public string Title = null;
        public string Journal = null;
        public string Volume = null;
        public string Number = null;
        public string Page = null;
        public string Year = null;
        public string DOI = null;

        private bool CodeMatch(string code, string line)
        {
            return 0 == String.CompareOrdinal(line, 0, code, 0, code.Length);
        }

        private string Data(string line)
        {
            return line.Substring(4, line.Length - 4).Trim();
        }

        public void Parse(string line)
        {
            line = line.Trim();
            if (line.Length < 4) return;
            if (CodeMatch("AU", line))
            {
                Authors = Data(line);
            }
            else if (CodeMatch("TI", line))
            {
                Title = Data(line);
            }
            else if (CodeMatch("SO", line))
            {
                Journal = Data(line);
            }
            else if (CodeMatch("VL", line))
            {
                Volume = Data(line);
            }
            else if (CodeMatch("NO", line))
            {
                Number = Data(line);
            }
            else if (CodeMatch("PG", line))
            {
                Page = Data(line);
            }
            else if (CodeMatch("YR", line))
            {
                Year = Data(line);
            }
            else if (CodeMatch("DOI", line))
            {
                DOI = Data(line);
            }
            else
            {
                // ignore
                return;
            }
        }

        private string AuthCode()
        {
            if (Authors == null || Authors.Length == 0) return "<NONAME>";
            int lengthFromStart = Authors.IndexOf(',');
            if (lengthFromStart < 0)
            {
                lengthFromStart = Authors.Length;
            }
            int startIndex = Authors.LastIndexOf(' ', lengthFromStart - 1, lengthFromStart);
            if( startIndex == -1 ) startIndex = 0;
            string year = Year != null ? Year : "";
            int consume = lengthFromStart - startIndex;
            string auth = consume > 0 ? Authors.Substring(startIndex, consume) : "";
            return "AUTO:" + auth + year;
        }

        private void TryWrite( StringWriter rw, string element, string data )
        {
            if( data != null )
            {
                rw.Write(element);
                rw.Write(" = {");
                rw.Write(data);
                rw.WriteLine("},");
            }
        }

        //@article{SilvioC.E. Tosatto04012002,
        //author = {Tosatto, Silvio C.E. and Bindewald, Eckart and Hesser, Jurgen and Manner, Reinhard},
        //title = {{A divide and conquer approach to fast loop modeling}},
        //journal = {Protein Eng.},
        //volume = {15},
        //number = {4},
        //pages = {279-286},
        //doi = {10.1093/protein/15.4.279},
        //year = {2002},
        //abstract = {We describe a fast ab initio method for modeling local segments in protein structures. The algorithm is based on a divide and conquer approach and uses a database of precalculated look-up tables, which represent a large set of possible conformations for loop segments of variable length. The target loop is recursively decomposed until the resulting conformations are small enough to be compiled analytically. The algorithm, which is not restricted to any specific loop length, generates a ranked set of loop conformations in 20-180 s on a desktop PC. The prediction quality is evaluated in terms of global RMSD. Depending on loop length the top prediction varies between 1.06 A RMSD for three-residue loops and 3.72 A RMSD for eight-residue loops. Due to its speed the method may also be useful to generate alternative starting conformations for complex simulations.
        //},
        //URL = {http://peds.oxfordjournals.org/cgi/content/abstract/15/4/279},
        //eprint = {http://peds.oxfordjournals.org/cgi/reprint/15/4/279.pdf}
        //}

        public void Print(StringWriter rw)
        {
            rw.Write("@article{");
            rw.Write(AuthCode());
            rw.WriteLine(',');
            TryWrite(rw,"author", Authors);
            TryWrite(rw,"title", Title);
            TryWrite(rw,"journal", Journal);
            TryWrite(rw,"volume", Volume);
            TryWrite(rw,"number", Number);
            TryWrite(rw,"pages", Page);            
            TryWrite(rw,"doi", DOI);
            TryWrite(rw,"year", Year);
            rw.WriteLine('}');
        }
    }

    class RifToBibEngine
    {
        private RifToBibEngine() {}

        public static string RifToBib( TextReader re )
        {
            BibEntry bib = new BibEntry();

            string line;
            while (null != (line = re.ReadLine()))
            {
                bib.Parse(line);                
            }

            StringWriter rw = new StringWriter();
            bib.Print(rw);
            return rw.ToString();
        }
    }
}
