using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace UoB.Compression
{
    public enum CompressionMode
    {
        zip7,
        bzip2
    }

    public class CompressedFileManager
    {
        private string m_Ext;
        private string m_Exec;
        private string m_ExtPath = null;

        private ProcessPriorityClass m_Prio = ProcessPriorityClass.Normal;

        public CompressedFileManager( string execEngine, string ext )
        {
            m_Exec = execEngine;
            m_Ext = ext;
        }

        public CompressedFileManager(string execEngine, string ext, ProcessPriorityClass _LaunchPrio)
        {
            m_Exec = execEngine;
            m_Ext = ext;
            m_Prio = _LaunchPrio;
        }

        public void SetExtractionPath(string path)
        {
            if (Directory.Exists(path))
            {
                m_ExtPath = path;
                if (m_ExtPath[m_ExtPath.Length - 1] != Path.DirectorySeparatorChar)
                {
                    m_ExtPath += Path.DirectorySeparatorChar;
                }
            }
        }

        private bool isTar(string fullname)
        {
            string subname = Path.GetFileNameWithoutExtension(fullname);
            int dotIndex = subname.LastIndexOf('.');
            if (dotIndex == -1) return false;
            return (0 == String.Compare(subname, dotIndex, ".tar", 0, 4, true)) ;
        }

        private string tarName(string fullname)
        {
            return Path.GetFileNameWithoutExtension(fullname);
        }

        private string nameStem(string fullname)
        {
            while (-1 != fullname.LastIndexOf('.'))
            {
                fullname = Path.GetFileNameWithoutExtension(fullname);
            }
            return fullname;
        }

        public DirectoryInfo OutPath(string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            string stem = nameStem(fileName);
            if (m_ExtPath != null)
            {
                return new DirectoryInfo(m_ExtPath + stem + Path.DirectorySeparatorChar);
            }
            else
            {
                return Directory.CreateDirectory(file.DirectoryName + Path.DirectorySeparatorChar + stem + Path.DirectorySeparatorChar);
            }
        }

        public void CompressDir(string dirName, bool removeFollowing)
        {
            CompressDir(dirName, CompressionMode.zip7, removeFollowing);
        }

        public void CompressDir(string dirName, CompressionMode comp, bool removeFollowing)
        {
            DirectoryInfo di = new DirectoryInfo(dirName);
            if (!File.Exists(m_Exec)) throw new Exception("Could not find app");
            if (!di.Exists) throw new IOException("Cannot find DIR to compress");

            string args = null;
            Process p = null;

            switch (comp)
            {
                case CompressionMode.zip7:

                    string fileName = di.FullName;
                    if (fileName[fileName.Length - 1] == Path.DirectorySeparatorChar || fileName[fileName.Length - 1] == Path.AltDirectorySeparatorChar)
                    {
                        fileName = fileName.Substring(0, fileName.Length - 1);
                    }
                    fileName += ".7za";

                    args = "a \"" + fileName + "\"" + dirName;
                    p = new Process();
                    p.StartInfo.Arguments = args;
                    p.StartInfo.FileName = m_Exec;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    p.Start();
                    p.PriorityClass = m_Prio;

                    p.WaitForExit();

                    break;

                case CompressionMode.bzip2:

                    string fileNameStem = di.FullName;
                    if (fileNameStem[fileNameStem.Length - 1] == Path.DirectorySeparatorChar || fileNameStem[fileNameStem.Length - 1] == Path.AltDirectorySeparatorChar)
                    {
                        fileNameStem = fileNameStem.Substring(0, fileNameStem.Length - 1);
                    }
                    string fileName1 = fileNameStem + ".tar";
                    string fileName2 = fileName1 + ".bz2";

                    args = "a -ttar \"" + fileName1 + "\" " + dirName;
                    p = new Process();
                    p.StartInfo.Arguments = args;
                    p.StartInfo.FileName = m_Exec;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    p.Start();
                    p.PriorityClass = m_Prio;

                    p.WaitForExit();

                    args = "a -tbzip2 \"" + fileName2 + "\" " + fileName1;
                    p = new Process();
                    p.StartInfo.Arguments = args;
                    p.StartInfo.FileName = m_Exec;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    p.Start();
                    p.PriorityClass = m_Prio;

                    p.WaitForExit();

                    File.Delete(fileName1);

                    break;

                default:
                    throw new Exception("Unknown Compression Method");
            }

            if (removeFollowing) di.Delete(true);
        }

        public DirectoryInfo Uncompress(string fileName)
        {
            return Uncompress(fileName, -1);
        }

        public DirectoryInfo Uncompress(string fileName, long timeout)
        {
            if (!File.Exists(m_Exec)) throw new Exception("Could not find app");
            if (!File.Exists(fileName)) throw new Exception("Could not find archive: " + fileName);

            FileInfo file = new FileInfo( fileName );          
            DirectoryInfo di = OutPath( fileName );           
            string outDir = di.FullName;

            if (di.Exists)
            {
                try
                {
                    di.Delete(true);
                }
                catch
                {
                    Trace.WriteLine("Warning, could not remove temp path");
                }
            }
            
            string args = "e \"" + fileName + "\" -y -o" + outDir + "\"";
            Process p = new Process();
            p.StartInfo.Arguments = args;
            p.StartInfo.FileName = m_Exec;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;            
            p.Start();
            try
            {
                p.PriorityClass = m_Prio;
            }
            catch
            {
                // Sometimes the app finises very quickly, and this throws an exception
            }

            if (timeout == -1)
            {
                p.WaitForExit();
            }
            else
            {
                long d = DateTime.Now.Ticks;
                bool ok = false;                
                while (DateTime.Now.Ticks - d < timeout)
                {
                    if (p.HasExited)
                    {
                        ok = true;
                        break;
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
                if (!ok)
                {
                    p.Kill();
                    return null;
                }
            }

            // Uncompress the internal tar
            if (isTar(fileName))
            {
                string tarFileName = tarName(fileName);
                tarFileName = outDir + Path.DirectorySeparatorChar + Path.GetFileName(tarFileName);

                if (!File.Exists(tarFileName))
                {
                    throw new Exception("Could not find extracted tar file!");
                }

                p = new Process();
                p.StartInfo.Arguments = "e \"" + tarFileName + "\" -y -o" + outDir;
                p.StartInfo.FileName = m_Exec;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.Start();
                try
                {
                    p.PriorityClass = m_Prio;
                }
                catch
                {
                    // Sometimes the app finises very quickly, and this throws an exception
                }

                if (timeout == -1)
                {
                    p.WaitForExit();
                }
                else
                {
                    long d = DateTime.Now.Ticks;
                    bool ok = false;
                    while (DateTime.Now.Ticks - d < timeout)
                    {
                        if (p.HasExited)
                        {
                            ok = true;
                            break;
                        }
                    }
                    if (!ok)
                    {
                        p.Kill();
                        return null;
                    }
                }

                File.Delete(tarFileName);
            }

            return di;
        }

        public void CleanUp(string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            string stem = nameStem(fileName);
            DirectoryInfo diOut = OutPath(fileName);
            try { diOut.Delete(true); }
            catch { }
        }
    }
}
