using System;
using System.Drawing;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
//using AtomicClasses;
using WebDownload;
using System.Text;

// found at
// http://www.thecodeproject.com/csharp/webdownload.asp#xx240293xx

namespace DAVE
{

    public delegate void FileDoneHandler(byte[] dataDownloaded, string PDBIdentifier);


    /// <summary>
    /// Summary description for WebDownloadForm.
    /// </summary>
    /// 
    public class WebDownloadForm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.GroupBox outputGroupBox;
        private System.Windows.Forms.Button downloadBtn;
        private System.Windows.Forms.Label downloadProgressLbl;
        private System.Windows.Forms.Label bytesDownloadedLbl;
        private System.Windows.Forms.Label totalBytesLbl;
        private System.Windows.Forms.TextBox bytesDownloadedTextBox;
        private System.Windows.Forms.TextBox totalBytesTextBox;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox boxReport;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ListView workerThreadListView;
        private System.Windows.Forms.Button fillListFromFile;
        private System.Windows.Forms.TextBox box_PDBNames;
        private System.Windows.Forms.Button button_Count;
        private System.Windows.Forms.TextBox box_Count;
        private System.Windows.Forms.TextBox url1;
        private System.Windows.Forms.TextBox url3;
        private System.Windows.Forms.TextBox url2;
        private System.ComponentModel.IContainer components;

        public WebDownloadForm()
        {
            InitializeComponent();
            this.workerThreadListView.Columns.Add("Thread Number", 75, HorizontalAlignment.Left);
            this.workerThreadListView.Columns.Add("PDB ID", 75, HorizontalAlignment.Left);
            this.workerThreadListView.Columns.Add("Progress bytes", 120, HorizontalAlignment.Left);
            this.workerThreadListView.Columns.Add("The URL", 100, HorizontalAlignment.Left);
            this.workerThreadListView.Columns.Add("The FileName", 300, HorizontalAlignment.Left);
            m_DelegatePDBID = new MyDelegate(getNextPDBID_HostThread);
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        [STAThread]
        static void Main()
        {
            Application.Run(new WebDownloadForm());

        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.outputGroupBox = new System.Windows.Forms.GroupBox();
            this.totalBytesTextBox = new System.Windows.Forms.TextBox();
            this.bytesDownloadedTextBox = new System.Windows.Forms.TextBox();
            this.bytesDownloadedLbl = new System.Windows.Forms.Label();
            this.downloadProgressLbl = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.totalBytesLbl = new System.Windows.Forms.Label();
            this.downloadBtn = new System.Windows.Forms.Button();
            this.url1 = new System.Windows.Forms.TextBox();
            this.url3 = new System.Windows.Forms.TextBox();
            this.boxReport = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.fillListFromFile = new System.Windows.Forms.Button();
            this.workerThreadListView = new System.Windows.Forms.ListView();
            this.box_PDBNames = new System.Windows.Forms.TextBox();
            this.button_Count = new System.Windows.Forms.Button();
            this.box_Count = new System.Windows.Forms.TextBox();
            this.url2 = new System.Windows.Forms.TextBox();
            this.outputGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // outputGroupBox
            // 
            this.outputGroupBox.Controls.Add(this.totalBytesTextBox);
            this.outputGroupBox.Controls.Add(this.bytesDownloadedTextBox);
            this.outputGroupBox.Controls.Add(this.bytesDownloadedLbl);
            this.outputGroupBox.Controls.Add(this.downloadProgressLbl);
            this.outputGroupBox.Controls.Add(this.progressBar);
            this.outputGroupBox.Controls.Add(this.totalBytesLbl);
            this.outputGroupBox.Enabled = false;
            this.outputGroupBox.Location = new System.Drawing.Point(8, 392);
            this.outputGroupBox.Name = "outputGroupBox";
            this.outputGroupBox.Size = new System.Drawing.Size(384, 120);
            this.outputGroupBox.TabIndex = 2;
            this.outputGroupBox.TabStop = false;
            this.outputGroupBox.Text = "Current Progress";
            // 
            // totalBytesTextBox
            // 
            this.totalBytesTextBox.Location = new System.Drawing.Point(120, 56);
            this.totalBytesTextBox.Name = "totalBytesTextBox";
            this.totalBytesTextBox.ReadOnly = true;
            this.totalBytesTextBox.Size = new System.Drawing.Size(256, 20);
            this.totalBytesTextBox.TabIndex = 4;
            this.totalBytesTextBox.Text = "";
            this.totalBytesTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bytesDownloadedTextBox
            // 
            this.bytesDownloadedTextBox.Location = new System.Drawing.Point(120, 24);
            this.bytesDownloadedTextBox.Name = "bytesDownloadedTextBox";
            this.bytesDownloadedTextBox.ReadOnly = true;
            this.bytesDownloadedTextBox.Size = new System.Drawing.Size(256, 20);
            this.bytesDownloadedTextBox.TabIndex = 3;
            this.bytesDownloadedTextBox.Text = "";
            this.bytesDownloadedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bytesDownloadedLbl
            // 
            this.bytesDownloadedLbl.Location = new System.Drawing.Point(16, 28);
            this.bytesDownloadedLbl.Name = "bytesDownloadedLbl";
            this.bytesDownloadedLbl.TabIndex = 2;
            this.bytesDownloadedLbl.Text = "Bytes Downloaded";
            // 
            // downloadProgressLbl
            // 
            this.downloadProgressLbl.Location = new System.Drawing.Point(16, 88);
            this.downloadProgressLbl.Name = "downloadProgressLbl";
            this.downloadProgressLbl.Size = new System.Drawing.Size(104, 23);
            this.downloadProgressLbl.TabIndex = 1;
            this.downloadProgressLbl.Text = "Download Progress";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(120, 88);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(256, 23);
            this.progressBar.TabIndex = 0;
            // 
            // totalBytesLbl
            // 
            this.totalBytesLbl.Location = new System.Drawing.Point(16, 60);
            this.totalBytesLbl.Name = "totalBytesLbl";
            this.totalBytesLbl.TabIndex = 2;
            this.totalBytesLbl.Text = "Total Bytes";
            // 
            // downloadBtn
            // 
            this.downloadBtn.Location = new System.Drawing.Point(8, 8);
            this.downloadBtn.Name = "downloadBtn";
            this.downloadBtn.Size = new System.Drawing.Size(88, 23);
            this.downloadBtn.TabIndex = 3;
            this.downloadBtn.Text = "Download";
            this.downloadBtn.Click += new System.EventHandler(this.downloadBtn_Click);
            // 
            // url1
            // 
            this.url1.Location = new System.Drawing.Point(112, 0);
            this.url1.Name = "url1";
            this.url1.Size = new System.Drawing.Size(280, 20);
            this.url1.TabIndex = 5;
            this.url1.Text = "http://www.rcsb.org/pdb/cgi/export.cgi/";
            // 
            // url3
            // 
            this.url3.Location = new System.Drawing.Point(112, 48);
            this.url3.Name = "url3";
            this.url3.Size = new System.Drawing.Size(280, 20);
            this.url3.TabIndex = 6;
            this.url3.Text = "&compression=None";
            // 
            // boxReport
            // 
            this.boxReport.Location = new System.Drawing.Point(400, 392);
            this.boxReport.Multiline = true;
            this.boxReport.Name = "boxReport";
            this.boxReport.Size = new System.Drawing.Size(344, 120);
            this.boxReport.TabIndex = 8;
            this.boxReport.Text = "";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(80, 80);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(184, 20);
            this.textBox2.TabIndex = 11;
            this.textBox2.Text = "No Implementation";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(264, 80);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(128, 23);
            this.button3.TabIndex = 10;
            this.button3.Text = "Fill List From URL";
            // 
            // fillListFromFile
            // 
            this.fillListFromFile.Location = new System.Drawing.Point(416, 8);
            this.fillListFromFile.Name = "fillListFromFile";
            this.fillListFromFile.Size = new System.Drawing.Size(128, 23);
            this.fillListFromFile.TabIndex = 9;
            this.fillListFromFile.Text = "Fill List From File";
            this.fillListFromFile.Click += new System.EventHandler(this.fillListFromFile_Click);
            // 
            // workerThreadListView
            // 
            this.workerThreadListView.Location = new System.Drawing.Point(8, 112);
            this.workerThreadListView.Name = "workerThreadListView";
            this.workerThreadListView.Size = new System.Drawing.Size(560, 272);
            this.workerThreadListView.TabIndex = 12;
            this.workerThreadListView.View = System.Windows.Forms.View.Details;
            // 
            // box_PDBNames
            // 
            this.box_PDBNames.Location = new System.Drawing.Point(576, 8);
            this.box_PDBNames.Multiline = true;
            this.box_PDBNames.Name = "box_PDBNames";
            this.box_PDBNames.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.box_PDBNames.Size = new System.Drawing.Size(168, 376);
            this.box_PDBNames.TabIndex = 7;
            this.box_PDBNames.Text = "";
            this.box_PDBNames.TextChanged += new System.EventHandler(this.box_PDBNames_TextChanged);
            // 
            // button_Count
            // 
            this.button_Count.Location = new System.Drawing.Point(472, 40);
            this.button_Count.Name = "button_Count";
            this.button_Count.TabIndex = 13;
            this.button_Count.Text = "Count";
            this.button_Count.Click += new System.EventHandler(this.button_Count_Click);
            // 
            // box_Count
            // 
            this.box_Count.Location = new System.Drawing.Point(472, 64);
            this.box_Count.Name = "box_Count";
            this.box_Count.Size = new System.Drawing.Size(72, 20);
            this.box_Count.TabIndex = 14;
            this.box_Count.Text = "";
            // 
            // url2
            // 
            this.url2.Location = new System.Drawing.Point(112, 24);
            this.url2.Name = "url2";
            this.url2.Size = new System.Drawing.Size(280, 20);
            this.url2.TabIndex = 15;
            this.url2.Text = ".pdb?format=PDB&pdbId=";
            // 
            // WebDownloadForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(754, 519);
            this.Controls.Add(this.url2);
            this.Controls.Add(this.box_Count);
            this.Controls.Add(this.button_Count);
            this.Controls.Add(this.workerThreadListView);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.fillListFromFile);
            this.Controls.Add(this.boxReport);
            this.Controls.Add(this.box_PDBNames);
            this.Controls.Add(this.url3);
            this.Controls.Add(this.url1);
            this.Controls.Add(this.downloadBtn);
            this.Controls.Add(this.outputGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "WebDownloadForm";
            this.Text = "WebDownload Tester";
            this.Load += new System.EventHandler(this.WebDownloadForm_Load);
            this.outputGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        int[] progressBytes = new int[numberOfWorkerThreads];
        string[] threadNames = new string[numberOfWorkerThreads];

        private void DownloadProgressCallback(int bytesSoFar, int totalBytes, int threadNumber)
        {
            //			bytesDownloadedTextBox.Text = bytesSoFar.ToString("#,##0");
            //
            //			if ( totalBytes != -1 )
            //			{
            //				progressBar.Minimum = 0;
            //				progressBar.Maximum = totalBytes;
            //				progressBar.Value = bytesSoFar;
            //				totalBytesTextBox.Text = totalBytes.ToString("#,##0");
            //			}
            //			else
            //			{
            //				progressBar.Visible = false;
            //				totalBytesTextBox.Text = "Total File Size Not Known";
            //			}

            updateProgress(bytesSoFar, threadNumber);

        }

        private void updateProgress(int bytesSoFar, int threadNumber)
        {

            for (int i = 0; i < workerThreadListView.Items.Count; i++)
            {
                ListViewItem theItem = workerThreadListView.Items[i];

                if ((int)theItem.Tag == threadNumber)
                {

                    int index = workerThreadListView.Items.IndexOf(theItem);


                    workerThreadListView.Items[index].SubItems[2].Text = bytesSoFar.ToString();
                }
            }



        }

        private void DownloadCompleteCallback(byte[] dataDownloaded, string fullFilePath, int threadNumber)
        {
            StreamWriter rw = new StreamWriter(fullFilePath, false);
            Encoding ascii = Encoding.ASCII;
            rw.Write(ascii.GetString(dataDownloaded));
            rw.Close();

            removeListItem(threadNumber);
            threadsInUse[threadNumber] = false;
            activeTreads--;

            this.boxReport.AppendText("Worker thread ended" + "\n");
        }

        private void removeListItem(int threadNumber)
        {
            for (int i = 0; i < workerThreadListView.Items.Count; i++)
            {
                ListViewItem theItem = workerThreadListView.Items[i];

                if ((int)theItem.Tag == threadNumber)
                {
                    workerThreadListView.Items[i].Remove();
                }
            }
        }

        private object m_Lock = new object();
        private string m_LockObtainID = null;
        private delegate void MyDelegate();
        MyDelegate m_DelegatePDBID = null;

        private string getNextPDBID()
        {
            lock (m_Lock)
            {
                Invoke(m_DelegatePDBID);
            }
            return m_LockObtainID;
        }

        private void getNextPDBID_HostThread()
        {
            if (this.box_PDBNames.Lines.Length == 0)
            {
                m_LockObtainID = null;
                return;
            }

            string tempFirst = this.box_PDBNames.Lines[0];

            if (this.box_PDBNames.Lines.Length > 1)
            {
                string tempAll = this.box_PDBNames.Text.ToString();
                string newString = tempAll.Substring(tempFirst.Length + 2, (tempAll.Length - (tempFirst.Length + 2)));
                this.box_PDBNames.Clear();
                this.box_PDBNames.Text = newString;
            }
            else
            {
                this.box_PDBNames.Clear();
            }

            m_LockObtainID = tempFirst;
            return;
        }


        private static int activeTreads = 0;
        private static int numberOfWorkerThreads = 10;
        private static bool[] threadsInUse = new bool[numberOfWorkerThreads];
        private string cacheDir = @"c:\__MassPDB\";

        private void startThreadManager()
        {
            if (!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
            }

            while (true)
            {
                if (activeTreads >= numberOfWorkerThreads)
                {
                    continue;
                }

                int threadToUse = -1;
                for (int i = 0; i < threadsInUse.Length; i++)
                {
                    if (threadsInUse[i] == false)
                    {
                        threadToUse = i;
                        threadsInUse[i] = true;
                        break;
                    }
                }

                if (threadToUse < 0) continue;
                try
                {
                    int result = makeWorkerThread(threadToUse);
                    if (result == 0) break;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }

                activeTreads++;
            }

            boxReport.AppendText("Downloading Complete !!!" + "\n");
        }

        private int makeWorkerThread(int threadNumber)
        {
            string nextID = getNextPDBID();
            if (nextID == null) return 0;

            string theURL = doConcat(nextID);
            string theFileName = cacheDir + nextID + ".pdb";

            System.Windows.Forms.ListViewItem newListItem = new ListViewItem();

            newListItem.Text = threadNumber.ToString();
            newListItem.SubItems.Add(nextID);
            newListItem.SubItems.Add("0");
            newListItem.SubItems.Add(theURL);
            newListItem.SubItems.Add(theFileName);
            newListItem.Tag = threadNumber;

            workerThreadListView.Items.Add(newListItem);

            if (theURL != "")
            {
                this.outputGroupBox.Enabled = true;

                this.bytesDownloadedTextBox.Text = "";
                this.totalBytesTextBox.Text = "";
                this.progressBar.Minimum = 0;
                this.progressBar.Maximum = 0;
                this.progressBar.Value = 0;

                DownloadThread dl = new DownloadThread(theFileName, threadNumber);
                dl.DownloadUrl = theURL;
                dl.CompleteCallback += new DownloadCompleteHandler(DownloadCompleteCallback);
                dl.ProgressCallback += new DownloadProgressHandler(DownloadProgressCallback);

                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(dl.Download));
                t.Start();
                boxReport.AppendText("Worker thread started: " + threadNumber.ToString() + " on " + theFileName + "\n");

            }
            else
            {
                MessageBox.Show("Please enter a path ...");
            }

            return 1;
        }

        private string doConcat(string idString)
        {
            return url1.Text + idString + url2.Text + idString + url3.Text;
        }


        private void downloadBtn_Click(object sender, System.EventArgs e)
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(startThreadManager));
            t.Start();
            boxReport.AppendText("Manager thread started" + "\n");
        }

        private void fillListFromFile_Click(object sender, System.EventArgs e)
        {
            this.box_PDBNames.Clear();

            string fullPathName = cacheDir + "scop_parse.txt";
            StreamReader re = File.OpenText(fullPathName);
            string inputLine = null;
            while ((inputLine = re.ReadLine()) != null)
            {
                parseLine(inputLine);
            }
            re.Close();

            //Now elements haev been generated, add PDB's to the listbox
            StreamWriter rw = new StreamWriter(cacheDir + "output.lst", false);

            for (int i = 0; i < scopEntriesArray.Count; i++)
            {
                scopEntry theEntry = (scopEntry)scopEntriesArray[i];

                rw.Write(this.url1.Text
                    + theEntry.PDD_ID
                    + this.url2.Text
                    + theEntry.PDD_ID
                    + this.url3.Text
                    + "\r\n"
                    );

                box_PDBNames.AppendText(theEntry.PDD_ID + "\r\n");
            }

            rw.Close();
        }

        private void parseLine(string theLine)
        {
            string[] elements = theLine.Split('\t');

            if (elements.Length != 6) { return; } // ignore the header lines

            bool oktoadd = true;

            foreach (scopEntry theEntry in scopEntriesArray)
            {
                if (elements[1] == theEntry.PDD_ID || elements[3] == theEntry.SCOP_DEF)
                {
                    oktoadd = false;
                    break;
                }
            }

            if (oktoadd == true)
            {
                scopEntry theEntry = new scopEntry();
                theEntry.PDD_ID = elements[1];
                theEntry.SCOP_DEF = elements[3];
                scopEntriesArray.Add(theEntry);
            }
        }

        private ArrayList scopEntriesArray = new ArrayList();

        private void box_PDBNames_TextChanged(object sender, System.EventArgs e)
        {
        }

        private void button_Count_Click(object sender, System.EventArgs e)
        {
            box_Count.Text = box_PDBNames.Lines.Length.ToString();
        }

        private void WebDownloadForm_Load(object sender, System.EventArgs e)
        {
        }

        private class scopEntry
        {
            public string PDD_ID;
            public string SCOP_DEF;
        }
    }
}
