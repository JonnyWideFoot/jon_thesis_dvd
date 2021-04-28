/**
 * Author: Andrew Deren 
 * Date: 10/15/03
 * Application: SourceCount
 * (C) AderSoftware 2003
 */

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using System.IO;
using System.Threading;

namespace SourceCount
{
	/// <summary>
	/// Main Form
	/// </summary>
	public class SourceCountForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtDirectory;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button cmdBrowser;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox txtMask;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ListView listFiles;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.TreeView tree;
		private System.Windows.Forms.Button cmdCount;
		private System.Windows.Forms.StatusBar statusBar;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ColumnHeader columnHeaderFile;
		private System.Windows.Forms.ColumnHeader columnHeaderDirectory;
		private System.Windows.Forms.ColumnHeader columnHeaderLines;

		bool running;				// if we are currently running, if set to false, the running thread will stop executing
		ArrayList files;			// list of FileEntry
		string startPath;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtTotal;
		private System.Windows.Forms.LinkLabel linkAbout;			// root path where we are looking now
		ColumnSorter columnSorter;

		public SourceCountForm(string startPath)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			files = new ArrayList();
			columnSorter = new ColumnSorter();
			listFiles.ListViewItemSorter = columnSorter;
			this.startPath = startPath;
			this.txtDirectory.Text = startPath;
			this.linkAbout.Links.Add(0, linkAbout.Text.Length, "www.adersoftware.com");
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SourceCountForm));
            this.txtDirectory = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdBrowser = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMask = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listFiles = new System.Windows.Forms.ListView();
            this.columnHeaderFile = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderDirectory = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderLines = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.tree = new System.Windows.Forms.TreeView();
            this.cmdCount = new System.Windows.Forms.Button();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.linkAbout = new System.Windows.Forms.LinkLabel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtDirectory
            // 
            this.txtDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDirectory.Location = new System.Drawing.Point(60, 4);
            this.txtDirectory.Name = "txtDirectory";
            this.txtDirectory.Size = new System.Drawing.Size(464, 20);
            this.txtDirectory.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Directory:";
            // 
            // cmdBrowser
            // 
            this.cmdBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdBrowser.Location = new System.Drawing.Point(528, 4);
            this.cmdBrowser.Name = "cmdBrowser";
            this.cmdBrowser.Size = new System.Drawing.Size(72, 23);
            this.cmdBrowser.TabIndex = 2;
            this.cmdBrowser.Text = "Browse...";
            this.cmdBrowser.Click += new System.EventHandler(this.cmdBrowser_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(4, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 23);
            this.label2.TabIndex = 4;
            this.label2.Text = "Mask:";
            // 
            // txtMask
            // 
            this.txtMask.Items.AddRange(new object[] {
            "*.aspx",
            "*.c;*.cpp;*.h",
            "*.cs",
            "*.cfm",
            "*.html",
            "*.java",
            "*.jsp",
            "*.vb"});
            this.txtMask.Location = new System.Drawing.Point(60, 32);
            this.txtMask.Name = "txtMask";
            this.txtMask.Size = new System.Drawing.Size(128, 21);
            this.txtMask.TabIndex = 5;
            this.txtMask.Text = "*.c;*.cpp;*.h";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.listFiles);
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.tree);
            this.panel1.Location = new System.Drawing.Point(4, 60);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(596, 352);
            this.panel1.TabIndex = 6;
            // 
            // listFiles
            // 
            this.listFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderFile,
            this.columnHeaderDirectory,
            this.columnHeaderLines});
            this.listFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listFiles.Location = new System.Drawing.Point(215, 0);
            this.listFiles.MultiSelect = false;
            this.listFiles.Name = "listFiles";
            this.listFiles.Size = new System.Drawing.Size(381, 352);
            this.listFiles.SmallImageList = this.imageList1;
            this.listFiles.TabIndex = 2;
            this.listFiles.UseCompatibleStateImageBehavior = false;
            this.listFiles.View = System.Windows.Forms.View.Details;
            this.listFiles.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listFiles_ColumnClick);
            // 
            // columnHeaderFile
            // 
            this.columnHeaderFile.Text = "Filename";
            this.columnHeaderFile.Width = 120;
            // 
            // columnHeaderDirectory
            // 
            this.columnHeaderDirectory.Text = "Directory";
            this.columnHeaderDirectory.Width = 175;
            // 
            // columnHeaderLines
            // 
            this.columnHeaderLines.Text = "Lines";
            this.columnHeaderLines.Width = 75;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(212, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 352);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // tree
            // 
            this.tree.Dock = System.Windows.Forms.DockStyle.Left;
            this.tree.ImageIndex = 0;
            this.tree.ImageList = this.imageList1;
            this.tree.Location = new System.Drawing.Point(0, 0);
            this.tree.Name = "tree";
            this.tree.SelectedImageIndex = 0;
            this.tree.Size = new System.Drawing.Size(212, 352);
            this.tree.TabIndex = 0;
            this.tree.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterCollapse);
            this.tree.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterExpand);
            // 
            // cmdCount
            // 
            this.cmdCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCount.Location = new System.Drawing.Point(524, 416);
            this.cmdCount.Name = "cmdCount";
            this.cmdCount.Size = new System.Drawing.Size(75, 23);
            this.cmdCount.TabIndex = 7;
            this.cmdCount.Text = "Count";
            this.cmdCount.Click += new System.EventHandler(this.cmdCount_Click);
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 444);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(604, 22);
            this.statusBar.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.Location = new System.Drawing.Point(4, 416);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 23);
            this.label3.TabIndex = 9;
            this.label3.Text = "Total:";
            // 
            // txtTotal
            // 
            this.txtTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtTotal.Location = new System.Drawing.Point(44, 416);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(84, 20);
            this.txtTotal.TabIndex = 10;
            // 
            // linkAbout
            // 
            this.linkAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkAbout.Location = new System.Drawing.Point(468, 32);
            this.linkAbout.Name = "linkAbout";
            this.linkAbout.Size = new System.Drawing.Size(128, 20);
            this.linkAbout.TabIndex = 11;
            this.linkAbout.TabStop = true;
            this.linkAbout.Text = "(C) AderSoftware 2003";
            this.linkAbout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkAbout_LinkClicked);
            // 
            // SourceCountForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(604, 466);
            this.Controls.Add(this.linkAbout);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.cmdCount);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtMask);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmdBrowser);
            this.Controls.Add(this.txtDirectory);
            this.Controls.Add(this.label1);
            this.Name = "SourceCountForm";
            this.Text = "SourceCount";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			
			if (startPath.Length > 0)
				this.cmdCount_Click(null, EventArgs.Empty);
		}


		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) 
		{
            string startPath;
            if (args.Length == 0)
            {

                // set the initial browse directory to the execution path.
                FileInfo fi = new FileInfo(Assembly.GetEntryAssembly().Location);
                DirectoryInfo di = fi.Directory; // the directory where the executing file is located
                startPath = di.FullName;
            }
            else if (Directory.Exists(args[0]))
            {
                startPath = args[0];
            }
            else
            {
                startPath = String.Empty;
            }
			SourceCountForm form = new SourceCountForm(startPath);
			Application.Run(form);
		}

		private void cmdBrowser_Click(object sender, System.EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			dlg.SelectedPath = this.txtDirectory.Text;
			if (dlg.ShowDialog() == DialogResult.OK)
				this.txtDirectory.Text = dlg.SelectedPath;
		}

		private int Count(string dir, string[] mask, TreeNode parent)
		{
			this.statusBar.Text = dir;
			this.statusBar.Update();

			// go thru directories first
			int totalCount = 0;
			string[] dirs = Directory.GetDirectories(dir);

			foreach (string innerdir in dirs)
			{
				if (!running)
					return 0;

				TreeNode node = new TreeNode(Path.GetFileName(innerdir));
				node.ImageIndex = 0;
				node.SelectedImageIndex = 0;
				int count = Count(innerdir, mask, node);
				if (count > 0)
				{
					node.Text += " (" + count + ")";
					totalCount += count;
					parent.Nodes.Add(node);
				}
			}

			string dirName = dir.Substring(this.startPath.Length);
			foreach (string pattern in mask)
			{
				string[] filenames = Directory.GetFiles(dir, pattern);
				foreach (string file in filenames)
				{
					if (!running)
						return 0;

					int count = CountFile(file);
					if (count > 0)
					{
						string name = Path.GetFileName(file);
						TreeNode node = new TreeNode(name + " (" + count +")");
						node.ImageIndex = 1;
						node.SelectedImageIndex = 1;
						parent.Nodes.Add(node);
						totalCount += count;

						FileEntry entry = new FileEntry(name, dirName, count);
						this.files.Add(entry);
					}
				}
			}
			return totalCount;
		}

		private int CountFile(string path)
		{
			int count = 0;
			try
			{
				StreamReader reader = new StreamReader(path);

				while (reader.ReadLine() != null)
					count++;

				reader.Close();
			}
			catch (IOException)
			{
			}

			return count;
		}

		private void CountStart()
		{
			this.cmdCount.Text = "Cancel";
			running = true;
			TreeNode root = new TreeNode("Root");
			root.ImageIndex = 0;
			this.startPath = txtDirectory.Text;
			string[] mask = this.txtMask.Text.Split(';');
			int count = Count(txtDirectory.Text, mask, root);

			root.Text = "All Files & Directories (" + count + ")";

			tree.Invoke(new CountFinishedDelegate(CountFinished), new object[]{root, count});
		}

		public delegate void CountFinishedDelegate(TreeNode root, int count);

		private void CountFinished(TreeNode root, int count)
		{
			tree.Nodes.Clear();
			listFiles.Items.Clear();

			if (running)
			{
				tree.Nodes.Add(root);
				tree.Nodes[0].Expand();
			
				// disable column sorting
				this.listFiles.ListViewItemSorter = null;

				ListViewItem[] items = new ListViewItem[files.Count];
				for (int i=0; i<files.Count; i++)
				{
					FileEntry entry = (FileEntry)files[i];
					string[] names = {entry.Name, entry.Dir, entry.Lines.ToString()};
					ListViewItem item = new ListViewItem(names);
					item.Tag = entry;
					items[i] = item;
				}
				this.listFiles.Items.AddRange(items);
				columnSorter.SortOrder = SortOrder.Descending;
				columnSorter.Column = 2;
				listFiles.ListViewItemSorter = columnSorter;
				this.txtTotal.Text = count.ToString("#,#");
			}
			else
				this.txtTotal.Text = string.Empty;

			this.statusBar.Text = string.Empty;
			this.running = false;
			this.cmdCount.Text = "Count";
		}

		private void cmdCount_Click(object sender, System.EventArgs e)
		{
			if (running)
			{
				running = false;
			}
			else
			{
                if (Directory.Exists(this.txtDirectory.Text))
                {

                    Thread thread = new Thread(new ThreadStart(CountStart));
                    thread.Start();
                }
                else
                {
                    MessageBox.Show("Start directory not found!");
                }
			}
		}

		private void tree_AfterExpand(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			e.Node.ImageIndex = 2;
			e.Node.SelectedImageIndex = 2;
		}

		private void tree_AfterCollapse(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			e.Node.ImageIndex = 0;
			e.Node.SelectedImageIndex = 0;
		}

		private void listFiles_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
		{
			if (columnSorter.Column == e.Column)
				columnSorter.FlipSortOrder();
			else
			{
				columnSorter.SortOrder = SortOrder.Ascending;
				columnSorter.Column = e.Column;
			}
			listFiles.Sort();
		}

		private void linkAbout_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.adersoftware.com");
		}
	}

	class ColumnSorter : IComparer
	{
		int column;
		SortOrder sortOrder;

		public ColumnSorter()
		{
			this.column = 0;
			this.sortOrder = SortOrder.Ascending;
		}

		public void FlipSortOrder()
		{
			if (sortOrder == SortOrder.Ascending)
				sortOrder = SortOrder.Descending;
			else
				sortOrder = SortOrder.Ascending;
		}

		public int Compare(object r1, object r2)
		{
			ListViewItem row1 = (ListViewItem)r1;
			ListViewItem row2 = (ListViewItem)r2;

			FileEntry e1 = (FileEntry)row1.Tag;
			FileEntry e2 = (FileEntry)row2.Tag;

			if (sortOrder == SortOrder.Ascending)
			{

				if (column == 0)
					return e1.Name.CompareTo(e2.Name);
				else if (column == 1)
					return e1.Dir.CompareTo(e2.Dir);
				else if (column == 2)
					return e1.Lines < e2.Lines ? -1 : e1.Lines > e2.Lines ? 1 : 0;
				else
					return 0;
			}
			else
			{
				if (column == 0)
					return e2.Name.CompareTo(e1.Name);
				else if (column == 1)
					return e2.Dir.CompareTo(e1.Dir);
				else if (column == 2)
					return e2.Lines < e1.Lines ? -1 : e2.Lines > e1.Lines ? 1 : 0;
				else
					return 0;

			}
		}

		public int Column
		{
			get { return this.column; }
			set { this.column = value; }
		}

		public System.Windows.Forms.SortOrder SortOrder
		{
			get { return this.sortOrder; }
			set { this.sortOrder = value; }
		}
	}
}
