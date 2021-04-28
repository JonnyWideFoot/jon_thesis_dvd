using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using UoB.Methodology.OriginInteraction;

namespace _SummaryGraph
{
    class Program
    {
        const string startLine = "<center><table width=800 border=1 bordercolor=black cellpadding=0 cellspacing=0 >";
        const string endLine = "</table></center><br>";

        // rms / length / method / cutoff
        double[, , ,] data = new double[3, 6, 9, 3];

        List<string> bb = new List<string>();
        List<string> aa = new List<string>();
        List<string> bba = new List<string>();
        List<string> methName = new List<string>();

        int methodIndex = 0;

        void doData()
        {
            for (int i = 0; i < bb.Count; i++)
            {
                string[] lineParts;
                lineParts = bb[i].Split('&');
                data[0, i, methodIndex, 0] = double.Parse(lineParts[5].Trim());
                data[0, i, methodIndex, 1] = double.Parse(lineParts[6].Trim());
                data[0, i, methodIndex, 2] = double.Parse(lineParts[7].Trim());
                lineParts = aa[i].Split('&');
                data[1, i, methodIndex, 0] = double.Parse(lineParts[5].Trim());
                data[1, i, methodIndex, 1] = double.Parse(lineParts[6].Trim());
                data[1, i, methodIndex, 2] = double.Parse(lineParts[7].Trim());
                lineParts = bba[i].Split('&');
                data[2, i, methodIndex, 0] = double.Parse(lineParts[5].Trim());
                data[2, i, methodIndex, 1] = double.Parse(lineParts[6].Trim());
                data[2, i, methodIndex, 2] = double.Parse(lineParts[7].Trim());
            }
            methodIndex++;
        }

        void printData()
        {
            // rms / length / method / cutoff
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    rwSummary.Write(",");
                    for (int m = 0; m < 6; m++)
                    {
                        rwSummary.Write("{0},", m + 6);
                    }
                    rwSummary.WriteLine();
                    for (int k = 0; k < 8; k++)
                    {
                        rwSummary.Write("{0},",methName[k]);
                        for (int m = 0; m < 6; m++)
                        {
                            rwSummary.Write("{0},",data[j,m,k,i]);
                        }
                        rwSummary.WriteLine();
                    }
                    rwSummary.WriteLine();
                    rwSummary.WriteLine();
                }
                rwSummary.WriteLine();
                rwSummary.WriteLine();
            }

            string root = @"E:\_thesis\MethodComp\_graph\";

            double[] dat = new double[6];
            OriginInterface o = new OriginInterface(true);

            string _T = "backbone";
            o.LoadTemplateFile(root + _T + ".opj");
            double max = double.MinValue;
            for (int k = 0; k < 8; k++)
            {
                for (int i = 0; i < 6; i++)
                {
                    double d = data[0, i, k, 1];
                    if (d > max) max = d;
                    dat[i] = d;
                }
                o.UpdateWorksheet("data2", 1 + k, dat);
            }
            o.SetYAxisIncrement("Graph1", 10.0);
            o.SetBoundY("graph1", 0.0f, (float)(max + 10));
            o.Save(root + _T + ".out.opj");
            o.SaveEPSPicture(root + _T + ".eps", "Graph1");
            o.SavePicture(root + _T, "graph1", 1200, 800);

            _T = "heavyatom";
            o.LoadTemplateFile(root + _T + ".opj");
            max = double.MinValue;
            for (int k = 0; k < 8; k++)
            {
                for (int i = 0; i < 6; i++)
                {
                    double d = data[1, i, k, 2];
                    if (d > max) max = d;
                    dat[i] = d;
                }
                o.UpdateWorksheet("data2", 1 + k, dat);
            }
            o.SetYAxisIncrement("Graph1", 10.0);
            o.SetBoundY("graph1", 0.0f, (float)(max + 10));
            o.Save(root + _T + ".out.opj");
            o.SaveEPSPicture(root + _T + ".eps", "Graph1");
            o.SavePicture(root + _T, "graph1", 1200, 800);

            _T = "torsion";
            o.LoadTemplateFile(root + _T + ".opj");
            max = double.MinValue;
            for (int k = 0; k < 8; k++)
            {
                for (int i = 0; i < 6; i++)
                {
                    double d = data[2, i, k, 2];
                    if (d > max) max = d;
                    dat[i] = d;
                }
                o.UpdateWorksheet("data2", 1 + k, dat);
            }
            o.SetYAxisIncrement("Graph1", 10.0);
            o.SetBoundY("graph1", 0.0f, (float)(max + 10));
            o.Save(root + _T + ".out.opj");
            o.SaveEPSPicture(root + _T + ".eps", "Graph1");
            o.SavePicture(root + _T, "graph1", 1200, 800);

            return;
        }

        List<int> m_tots = null;
        private void MethodFailProcess(FileInfo file)
        {
            List<int> tots = new List<int>();
            StreamReader re = new StreamReader(file.FullName);
            string line;
            string name = file.Name.Split('_')[0];
            string a = name + " #,";
            string b = name + " %,";
            while (null != (line = re.ReadLine()))
            {
                string[] lineP = line.Split(',');
                int fail = int.Parse(lineP[1]);
                int tot = int.Parse(lineP[2]);
                tots.Add(tot);
                a += String.Format("{0},", fail);
                b += String.Format("{0},", 100.0*((double)fail/(double)tot));
            }
            re.Close();

            if (m_tots == null)
            {
                m_tots = tots;
                rwFail.Write("-,");
                for (int i = 0; i < tots.Count; i++)
                {
                    rwFail.Write("{0},", 6 + i);
                }
                rwFail.WriteLine();
                rwFail.Write("-,");
                for (int i = 0; i < tots.Count; i++)
                {
                    rwFail.Write("{0},", tots[i]);
                }
                rwFail.WriteLine();
            }
            else
            {
                for (int i = 0; i < tots.Count; i++)
                {
                    if (tots[i] != m_tots[i]) throw new Exception();
                }
            }

            //rwFail.WriteLine(a);
            rwFail.WriteLine(b);
        }

        public Program()
        {
        }

        StreamWriter rwFail;
        StreamWriter rwLatex;
        StreamWriter rwSummary;

        private string lineMe(StreamReader re)
        {
            string line = re.ReadLine();
            if (line == null) throw new Exception();
            return line;
        }

        private void LatexTablePreamble()
        {
            rwLatex.WriteLine("\\begin{sidewaystable}[p]");
            rwLatex.WriteLine("\\begin{center}");
            rwLatex.WriteLine("\\begin{small}");
            rwLatex.WriteLine("\\begin{tabular}{+l^c^c^c^c^c^c^c^c^c^c^c^c}");
            rwLatex.WriteLine("\\toprule");
        }

        private void LatexTableHeadRow(bool torsional)
        {
            string unit = !torsional ? "\\AA" : "\\degree";
            rwLatex.WriteLine("&& \\multicolumn{{5}}{{c}}{{\\textbf{{Common Statistics}}}} & \\multicolumn{{3}}{{c}}{{$\\mathbf{{<x}}$\\textbf{{{0}\\ \\drms\\ (\\%)}}}} & \\multicolumn{{3}}{{c}}{{$\\mathbf{{y\\%}}$\\textbf{{ are within ({0})}}}} \\\\[0.2cm]",
                unit);
            rwLatex.WriteLine("\\rowstyle{\\bfseries}");
            rwLatex.WriteLine("RMS Type & Length & Mean & Median & Max & Min & StdDev & $\\mathbf{{x={0}}}$ & $\\mathbf{{x={1}}}$ & $\\mathbf{{x={2}}}$  &  $\\mathbf{{y={3}}}$  &   $\\mathbf{{y={4}}}$ & $\\mathbf{{y={5}}}$ \\\\",
                torsional ? "10.0" : "1.0", torsional ? "20.0" : "2.0", torsional ? "30.0" : "3.0",
                "50.0", "75.0", "90.0");
            rwLatex.WriteLine("\\midrule");
        }

        private void LatexTablePostamble(string method)
        {
            rwLatex.WriteLine("\\bottomrule");
            rwLatex.WriteLine("\\end{tabular}");
            rwLatex.WriteLine("\\caption{{RMS distribution statistics for {0}.}}", method);
            rwLatex.WriteLine("\\label{{table:appendix_raw:{0}}}", Regex.Replace(method, " ", "_"));
            rwLatex.WriteLine("\\end{small}");
            rwLatex.WriteLine("\\end{center}");
            rwLatex.WriteLine("\\end{sidewaystable}");
        }

        private string LatexConvertTable(string line)
        {
            line = Regex.Replace(line, "<tr>", "  ");
            line = Regex.Replace(line, "</td><td>", " & ");
            line = Regex.Replace(line, "</tr>", " \\\\");
            line = Regex.Replace(line, "<td>", "");
            line = Regex.Replace(line, "</td>", "");
            int i = line.IndexOf('&');
            line = line.Substring(i + 1).Trim();
            return line;
        }

        private void import(StreamReader re)
        {
            string line1 = lineMe(re);
            string line2 = lineMe(re);
            string line3 = lineMe(re);
            string line4 = lineMe(re);
            string line5 = lineMe(re);
            string line6 = lineMe(re);
            string line7 = lineMe(re);
            string line8 = lineMe(re);

            // Process lines ...
            bb.Add(LatexConvertTable(line4));
            aa.Add(LatexConvertTable(line5));
            bba.Add(LatexConvertTable(line8));

            return;
        }

        private void Process(FileInfo repFile, string method)
        {
            StreamReader re = new StreamReader(repFile.FullName);

            LatexTablePreamble();

            string line;
            while (null != (line = re.ReadLine()))
            {
                line = line.Trim();
                if (line.Length < startLine.Length) continue;
                if (0 == String.CompareOrdinal(startLine, line))
                {
                    // Found table block ...
                    import(re);
                    line = lineMe(re).Trim();
                    if (0 != String.CompareOrdinal(endLine, line))
                    {
                        throw new Exception();
                    }
                }
            }

            LatexTableHeadRow(false);
            for (int i = 0; i < bb.Count; i++)
            {
                if (i == 0) rwLatex.Write("\\multirow{{{0}}}{{*}}{{\\textbf{{Backbone}}}}& ", bb.Count); else rwLatex.WriteLine("  & ");
                rwLatex.Write(" \\textbf{{{0}}} & ", i + 6);
                rwLatex.WriteLine(bb[i]);
            }

            rwLatex.WriteLine("\\midrule");

            LatexTableHeadRow(false);
            for (int i = 0; i < aa.Count; i++)
            {
                if (i == 0) rwLatex.Write("\\multirow{{{0}}}{{*}}{{\\textbf{{All heavy-atom}}}}& ", aa.Count); else rwLatex.WriteLine("  & ");
                rwLatex.Write(" \\textbf{{{0}}} & ", i + 6);
                rwLatex.WriteLine(aa[i]);
            }

            rwLatex.WriteLine("\\midrule");

            LatexTableHeadRow(true);
            for (int i = 0; i < bba.Count; i++)
            {
                if (i == 0) rwLatex.Write("\\multirow{{{0}}}{{*}}{{\\textbf{{Backbone Torsion}}}}& ", bba.Count); else rwLatex.WriteLine("  & ");
                rwLatex.Write(" \\textbf{{{0}}} & ", i + 6);
                rwLatex.WriteLine(bba[i]);
            }

            LatexTablePostamble(method);

            re.Close();

            doData();

            bb.Clear();
            aa.Clear();
            bba.Clear();
        }

        private string methname(DirectoryInfo di, int i)
        {
            if (i == 2) return di.Name;
            if (i == 3) return di.Parent.Name + " with the " + di.Name + " forcefield";
            throw new Exception();
        }

        private void doDir(DirectoryInfo di, int i)
        {
            DirectoryInfo[] dis = di.GetDirectories();
            foreach (DirectoryInfo d in dis) doDir(d, i + 1);
            FileInfo repFile = new FileInfo(di.FullName + Path.DirectorySeparatorChar + "_report.htm");
            if (!repFile.Exists)
                return;
            
            // Data Process
            string _methName = methname(di, i);
            methName.Add(_methName);
            Process(repFile, _methName);

            // Method Fail Stats
            FileInfo failFile = di.GetFiles("*_methodFail.csv")[0];
            MethodFailProcess(failFile);
        }

        public void go(DirectoryInfo di)
        {
            // string root = di.FullName + Path.DirectorySeparatorChar;
            string root = @"E:\_thesis\jon_thesis\thesis\09-MethodComparison\";
            rwLatex = new StreamWriter(root + "tables.tex");
            rwFail = new StreamWriter(root + "methfail.csv");
            doDir(di, 1);
            rwFail.Close();
            rwLatex.Close();
            rwSummary = new StreamWriter(root + "summary.csv");
            printData();
            rwSummary.Close();
        }

        static void Main(string[] args)
        {
            string path = @"E:\_thesis\MethodComp\";
            DirectoryInfo di = new DirectoryInfo(path);
            Program p = new Program();
            p.go(di);
        }
    }
}
