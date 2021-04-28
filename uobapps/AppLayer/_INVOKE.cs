using System;
using System.IO;
using System.Diagnostics;
using UoB.Core.ForceField;

namespace UoB.AppLayer
{
    class MainInvoke
    {
        [STAThread]
        static void Main(string[] args)
        {

#if DEBUG
#else
            try
            {
#endif
                //Tester.Test();
                //return;

                // Treat this region as a simple launch for other areas of the project....
                //e.g. Launch( new CommonTaskInvoke(), args );

                // Launch here ....

                //Job 0
                //Launch( new CommonTaskInvoke(), args );

                //Job 1
                //Launch( new RamachandranAnalysisInvoke(), args );

                //Job 2
                //Launch( new AngleSetFittingInvoke(), args );

                //Job 3
                //Launch(new PlopInvoke(), args);

                //Job 6
                //Launch(new FileConverterInvoke(), args);

                // Job 7
                //Launch(new ModellerInvoke(), args);

                // Job 8
                //Launch(new RapperInvoke(), args);

                // Job 9
                //Launch(new FilterInvoke(), args);

                // Job 10
                //Launch(new PetraInvoke(), args);

                // Job 11
                //Launch(new CloopInvoke(), args);

                // Job 12
                //Launch( new LoopBuilderStage1Invoke(), args );
                //Launch(new MineInvoke(), args);

                // Job 13
                //Launch(new DecoyInvoke(), args);

                // Job 14
                Launch(new ArcusInvoke(), args);


                //DirectoryInfo di = new DirectoryInfo(@"C:\Arcus\ScriptGen_Filt\");
                //FileInfo[] files = di.GetFiles("*.csv");
                //StreamWriter rw = new StreamWriter(di.FullName + "_cat.csv");
                //for (int i = 0; i < files.Length; i++)
                //{
                //    StreamReader re = new StreamReader(files[i].FullName);
                //    rw.WriteLine(re.ReadLine());
                //    re.Close();
                //}
                //rw.Close();
                //return;




#if DEBUG
                return;
#else
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.Read();
            }
            return;
#endif            
        }

        private static void Launch(IAppLayer launchModule, string[] args)
        {
            // Setup stdout file writing
            ConsoleTraceListener console = new ConsoleTraceListener(true);
            Trace.Listeners.Add(console);

            Directory.CreateDirectory(launchModule.LogPath);
            StreamWriter rwMain = new StreamWriter(launchModule.LogPath + "stdout.log");
            TextWriterTraceListener fileListener = new TextWriterTraceListener(rwMain); // Export to a file
            Trace.Listeners.Add(fileListener);

            if (launchModule.RequiresForcefield)
            {
                // do some pre-amble ...
                FFManager fman = FFManager.Instance;
                fman.FinaliseStage2(); // needed hack ...
            }
            launchModule.MainStem(args);

            rwMain.Close();
        }
    }
}
