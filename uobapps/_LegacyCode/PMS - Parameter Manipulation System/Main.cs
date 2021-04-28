using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

using UoB.Research;
using UoB.Research.Primitives;
using UoB.Generic.OpenGLView;
using UoB.Generic.PS_Render;
using UoB.Research.Modelling.ForceField;
using UoB.Research.Modelling.Structure;
using UoB.Research.FileIO.FormattedOutput;

using UoB.Research.Tools;

namespace UoB.PMS
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button_parsedir;
		private System.Windows.Forms.TextBox textbox_Path;
		private System.Windows.Forms.DataGrid datagrid_Output;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panel_View;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.Label label_PSTitle;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textbox_Path4MeadOut;
		private System.Windows.Forms.Button button_Optimise;
		private System.Windows.Forms.NumericUpDown num_QuantumTable;
		private System.Windows.Forms.Button button_SASACalc;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBox_RootDir;

		private PostMeadPFO.ProcessedDirectoryObject[] m_PostMeadPDOArray;
		private FFManager m_Defs = FFManager.Instance;
		private ArrayList m_ParticleSystems = new ArrayList();
		private Quantum2PFO.ProcessedFileObject[] pfoArray;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox text_Geom_In;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox text_Geom_Pos;
		private System.Windows.Forms.Button button_Geom;
		private System.Windows.Forms.TextBox text_Geom_Out;
		private System.Windows.Forms.TextBox text_QuantumString;
		private System.Windows.Forms.Button button_ValidateFolder;
		private GLView m_View;
		private ParticleSystemDrawWrapper m_DrawWrap;
		private System.Windows.Forms.Button button_OptimiseAll;

		private static readonly string AtomTypeHeaders_NonArea = "C,C Aromatic,N,N Polar,O,O Polar,S";
		private System.Windows.Forms.Button button_DumpInfo;
		private static readonly string AtomTypeHeaders = "MolName,Total_SASA,C_Area,CAR_Area,N_Area,NPolar_Area,O_Area,OPolar_Area,S_Area";
		//private static readonly string AtomTypeHeaders = "MolName,Total_SASA,C_Area,CAR_Area,N_Area,O_Area,S_Area";
		//private static readonly string AtomTypeHeaders =  "MolName,Total SASA,Total SASA";

		public MainForm()
		{
			InitializeComponent();
			m_View = new GLView();
			m_DrawWrap = new ParticleSystemDrawWrapper( null, m_View );
			m_View.Parent = panel_View;
			m_View.Dock = DockStyle.Fill;
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
			this.textbox_Path = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.button_parsedir = new System.Windows.Forms.Button();
			this.datagrid_Output = new System.Windows.Forms.DataGrid();
			this.panel_View = new System.Windows.Forms.Panel();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.label_PSTitle = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textbox_Path4MeadOut = new System.Windows.Forms.TextBox();
			this.button_Optimise = new System.Windows.Forms.Button();
			this.num_QuantumTable = new System.Windows.Forms.NumericUpDown();
			this.button_SASACalc = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox_RootDir = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.text_Geom_In = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.text_Geom_Pos = new System.Windows.Forms.TextBox();
			this.button_Geom = new System.Windows.Forms.Button();
			this.text_Geom_Out = new System.Windows.Forms.TextBox();
			this.text_QuantumString = new System.Windows.Forms.TextBox();
			this.button_ValidateFolder = new System.Windows.Forms.Button();
			this.button_OptimiseAll = new System.Windows.Forms.Button();
			this.button_DumpInfo = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.datagrid_Output)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.num_QuantumTable)).BeginInit();
			this.SuspendLayout();
			// 
			// textbox_Path
			// 
			this.textbox_Path.Location = new System.Drawing.Point(112, 96);
			this.textbox_Path.Name = "textbox_Path";
			this.textbox_Path.TabIndex = 0;
			this.textbox_Path.Text = "Initial Quantum Files";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 96);
			this.label1.Name = "label1";
			this.label1.TabIndex = 1;
			this.label1.Text = "Directory to parse";
			// 
			// button_parsedir
			// 
			this.button_parsedir.Location = new System.Drawing.Point(216, 96);
			this.button_parsedir.Name = "button_parsedir";
			this.button_parsedir.Size = new System.Drawing.Size(64, 24);
			this.button_parsedir.TabIndex = 2;
			this.button_parsedir.Text = "<-- Get";
			this.button_parsedir.Click += new System.EventHandler(this.button_parsedir_Click);
			// 
			// datagrid_Output
			// 
			this.datagrid_Output.DataMember = "";
			this.datagrid_Output.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.datagrid_Output.Location = new System.Drawing.Point(8, 128);
			this.datagrid_Output.Name = "datagrid_Output";
			this.datagrid_Output.Size = new System.Drawing.Size(544, 656);
			this.datagrid_Output.TabIndex = 3;
			// 
			// panel_View
			// 
			this.panel_View.Location = new System.Drawing.Point(552, 128);
			this.panel_View.Name = "panel_View";
			this.panel_View.Size = new System.Drawing.Size(416, 656);
			this.panel_View.TabIndex = 4;
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(584, 104);
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(56, 20);
			this.numericUpDown1.TabIndex = 5;
			this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
			// 
			// label_PSTitle
			// 
			this.label_PSTitle.Location = new System.Drawing.Point(656, 104);
			this.label_PSTitle.Name = "label_PSTitle";
			this.label_PSTitle.Size = new System.Drawing.Size(136, 23);
			this.label_PSTitle.TabIndex = 6;
			this.label_PSTitle.Text = "Undefined";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(288, 96);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(120, 23);
			this.button1.TabIndex = 7;
			this.button1.Text = "Output to PQR -->";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(216, 64);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(64, 24);
			this.button2.TabIndex = 10;
			this.button2.Text = "<-- Get";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 72);
			this.label2.Name = "label2";
			this.label2.TabIndex = 9;
			this.label2.Text = "Directory to parse";
			// 
			// textbox_Path4MeadOut
			// 
			this.textbox_Path4MeadOut.Location = new System.Drawing.Point(112, 72);
			this.textbox_Path4MeadOut.Name = "textbox_Path4MeadOut";
			this.textbox_Path4MeadOut.TabIndex = 8;
			this.textbox_Path4MeadOut.Text = "_OutPutProcess";
			// 
			// button_Optimise
			// 
			this.button_Optimise.Location = new System.Drawing.Point(288, 64);
			this.button_Optimise.Name = "button_Optimise";
			this.button_Optimise.Size = new System.Drawing.Size(120, 23);
			this.button_Optimise.TabIndex = 11;
			this.button_Optimise.Text = "Optimise For Set -->";
			this.button_Optimise.Click += new System.EventHandler(this.button_Optimise_Click);
			// 
			// num_QuantumTable
			// 
			this.num_QuantumTable.Location = new System.Drawing.Point(416, 64);
			this.num_QuantumTable.Name = "num_QuantumTable";
			this.num_QuantumTable.Size = new System.Drawing.Size(56, 20);
			this.num_QuantumTable.TabIndex = 12;
			// 
			// button_SASACalc
			// 
			this.button_SASACalc.Location = new System.Drawing.Point(416, 96);
			this.button_SASACalc.Name = "button_SASACalc";
			this.button_SASACalc.Size = new System.Drawing.Size(104, 23);
			this.button_SASACalc.TabIndex = 13;
			this.button_SASACalc.Text = "Output SASAs -->";
			this.button_SASACalc.Click += new System.EventHandler(this.button_SASACalc_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 24);
			this.label3.TabIndex = 14;
			this.label3.Text = "Root Directory";
			// 
			// textBox_RootDir
			// 
			this.textBox_RootDir.Location = new System.Drawing.Point(64, 8);
			this.textBox_RootDir.Name = "textBox_RootDir";
			this.textBox_RootDir.Size = new System.Drawing.Size(304, 20);
			this.textBox_RootDir.TabIndex = 15;
			this.textBox_RootDir.Text = "C:\\_All Backup\\_Work\\03d - testing diels\\";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 40);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(184, 23);
			this.label4.TabIndex = 16;
			this.label4.Text = "Directory to parse for template files";
			// 
			// text_Geom_In
			// 
			this.text_Geom_In.Location = new System.Drawing.Point(184, 40);
			this.text_Geom_In.Name = "text_Geom_In";
			this.text_Geom_In.Size = new System.Drawing.Size(144, 20);
			this.text_Geom_In.TabIndex = 17;
			this.text_Geom_In.Text = "_PostQuantumGeometry\\in";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(344, 32);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(184, 32);
			this.label5.TabIndex = 18;
			this.label5.Text = "Directory to parse for post geom optimisation position files";
			// 
			// text_Geom_Pos
			// 
			this.text_Geom_Pos.Location = new System.Drawing.Point(512, 40);
			this.text_Geom_Pos.Name = "text_Geom_Pos";
			this.text_Geom_Pos.Size = new System.Drawing.Size(152, 20);
			this.text_Geom_Pos.TabIndex = 19;
			this.text_Geom_Pos.Text = "_PostQuantumGeometry\\pos";
			// 
			// button_Geom
			// 
			this.button_Geom.Location = new System.Drawing.Point(672, 40);
			this.button_Geom.Name = "button_Geom";
			this.button_Geom.Size = new System.Drawing.Size(64, 24);
			this.button_Geom.TabIndex = 20;
			this.button_Geom.Text = "Get -->";
			this.button_Geom.Click += new System.EventHandler(this.button_Geom_Click);
			// 
			// text_Geom_Out
			// 
			this.text_Geom_Out.Location = new System.Drawing.Point(744, 40);
			this.text_Geom_Out.Name = "text_Geom_Out";
			this.text_Geom_Out.Size = new System.Drawing.Size(152, 20);
			this.text_Geom_Out.TabIndex = 21;
			this.text_Geom_Out.Text = "_PostQuantumGeometry\\out";
			// 
			// text_QuantumString
			// 
			this.text_QuantumString.Location = new System.Drawing.Point(512, 8);
			this.text_QuantumString.Name = "text_QuantumString";
			this.text_QuantumString.Size = new System.Drawing.Size(424, 20);
			this.text_QuantumString.TabIndex = 22;
			this.text_QuantumString.Text = "# B3LYP/cc-pVTZ POP=(CHELPG,Dipole) SCRF=(PCM,Solvent=water,read)";
			// 
			// button_ValidateFolder
			// 
			this.button_ValidateFolder.Location = new System.Drawing.Point(376, 8);
			this.button_ValidateFolder.Name = "button_ValidateFolder";
			this.button_ValidateFolder.Size = new System.Drawing.Size(128, 23);
			this.button_ValidateFolder.TabIndex = 23;
			this.button_ValidateFolder.Text = "Validate folder + Subs";
			this.button_ValidateFolder.Click += new System.EventHandler(this.button_ValidateFolder_Click);
			// 
			// button_OptimiseAll
			// 
			this.button_OptimiseAll.Location = new System.Drawing.Point(480, 64);
			this.button_OptimiseAll.Name = "button_OptimiseAll";
			this.button_OptimiseAll.Size = new System.Drawing.Size(120, 23);
			this.button_OptimiseAll.TabIndex = 24;
			this.button_OptimiseAll.Text = "Optimise Output All";
			this.button_OptimiseAll.Click += new System.EventHandler(this.button_OptimiseAll_Click);
			// 
			// button_DumpInfo
			// 
			this.button_DumpInfo.Location = new System.Drawing.Point(616, 64);
			this.button_DumpInfo.Name = "button_DumpInfo";
			this.button_DumpInfo.Size = new System.Drawing.Size(112, 24);
			this.button_DumpInfo.TabIndex = 25;
			this.button_DumpInfo.Text = "Dump Info File";
			this.button_DumpInfo.Click += new System.EventHandler(this.button_DumpInfo_Click);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(968, 789);
			this.Controls.Add(this.button_DumpInfo);
			this.Controls.Add(this.button_OptimiseAll);
			this.Controls.Add(this.button_ValidateFolder);
			this.Controls.Add(this.text_QuantumString);
			this.Controls.Add(this.text_Geom_Out);
			this.Controls.Add(this.text_Geom_Pos);
			this.Controls.Add(this.text_Geom_In);
			this.Controls.Add(this.textBox_RootDir);
			this.Controls.Add(this.textbox_Path4MeadOut);
			this.Controls.Add(this.textbox_Path);
			this.Controls.Add(this.button_Geom);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.button_SASACalc);
			this.Controls.Add(this.num_QuantumTable);
			this.Controls.Add(this.button_Optimise);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label_PSTitle);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.panel_View);
			this.Controls.Add(this.datagrid_Output);
			this.Controls.Add(this.button_parsedir);
			this.Controls.Add(this.label1);
			this.Name = "MainForm";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.datagrid_Output)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.num_QuantumTable)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Params preLoad_Params = Params.Instance;
			Application.Run(new MainForm());
		}


		private void button_parsedir_Click(object sender, System.EventArgs e)
		{

			ArrayList directoryObjects = new ArrayList();

			string[] directories = Directory.GetDirectories(this.textBox_RootDir.Text + this.textbox_Path.Text);
			for ( int i = 0; i < directories.Length; i++ )
			{
				ArrayList fileObjects = new ArrayList();
				string parseFilePath = directories[i] + @"\parse.set";
				if ( !File.Exists( parseFilePath ) )
				{
					Debug.WriteLine(directories[i] + " ignored as no parse.set file found...");
					continue;
				}

				string[] parseLines = FileScanTools.readParseFile( parseFilePath );

				string pattern = parseLines[0];

				string[] commandLines = new string[parseLines.Length-1];
				Array.Copy(parseLines,1,commandLines,0,parseLines.Length-1);


				string[] files = Directory.GetFiles(directories[i], pattern);
				for ( int j = 0; j < files.Length; j++ )
				{
					fileObjects.Add( FileScanTools.parseFile(files[j], commandLines ) );                                        					
				}
				directoryObjects.Add(fileObjects);
			}

			// files all parsed
			// Begin Assembly Of Files into information structures

			ArrayList firstFileSetFromPDB = (ArrayList) directoryObjects[0];
			FileObject[] foArray = (FileObject[]) firstFileSetFromPDB.ToArray(typeof(FileObject));

			pfoArray = new Quantum2PFO.ProcessedFileObject[foArray.Length];
			// will hold the information for each molecule

			for ( int i = 0; i < foArray.Length; i++ )
			{
				pfoArray[i] = new Quantum2PFO.ProcessedFileObject();
				elementNamedVectorArray positions = (elementNamedVectorArray) foArray[i][0];
				pfoArray[i].PDB_limitedPositions = positions.namedVectors;
				pfoArray[i].Charges = new elementNamedFloatArray[directoryObjects.Count - 1];
			}

			// positions and PDB atom names have been extracted from the files in the "1_PDB" directory
			// NOTE - in hindsight this was stupid cos the PDB files dont have hydrogens - the positions are replaced below from gaussian

            // Now we want the molecule name and the charge sets from each of the other folders.

			for ( int i = 1; i < directoryObjects.Count; i++ )
			{
				ArrayList nextFileSetFromPDB = (ArrayList) directoryObjects[i];
				FileObject[] nextfoArray = (FileObject[]) nextFileSetFromPDB.ToArray(typeof(FileObject));

				for ( int j = 0; j < nextfoArray.Length; j++ )
				{
					elementString molName = (elementString) nextfoArray[j][0];
					pfoArray[j].Name = molName.dataString;
					elementNamedVectorArray atomPositions = (elementNamedVectorArray) nextfoArray[j][1]; //Atomic Positions
					pfoArray[j].Positions = atomPositions.namedVectors;

					elementNamedFloatArray atomCharges = (elementNamedFloatArray) nextfoArray[j][2]; // Muliken Charges
					pfoArray[j].Charges[i-1] = atomCharges;
					for ( int q = 0; q < i-2; q++ )
					{
						if ( pfoArray[j].Charges[q].data.Length != atomCharges.data.Length )
						{
							throw new Exception( pfoArray[j].Name + " is buggered at Level 3 ID : J = " + j.ToString() + " and Q = " + q.ToString() );
						}
					}

					if ( atomPositions.data.Length != atomCharges.data.Length )
					{
						throw new Exception( pfoArray[j].Name + " is buggered at Level 1 ID : J = " + j.ToString() + " and I = " + i.ToString() );
					}
				}                
			}

			// Validate data

			for ( int i = 0; i < pfoArray.Length; i++ )
			{	
				for ( int j = 0; j < pfoArray[0].Charges.Length; j++ )
				{
					if ( pfoArray[i].Positions.Length != pfoArray[i].Charges[j].data.Length )
					{
						Console.WriteLine( pfoArray[i].Name + " is buggered at Level 2 ID : J = " + j.ToString() + " and I = " + i.ToString() );
					}
				}
			}


			// Now we want to show our data !


			DataSet data = new DataSet("TheDataSet");


			for ( int i = 0; i < pfoArray.Length; i++ )
			{		
				Molecule m = new Molecule( 0, m_Defs.GetMolPrimitive( "MIS" ) );
				AtomList collectedAtoms = new AtomList();

				DataTable table = new DataTable( pfoArray[i].Name );
				table.Columns.Add("AtomName", typeof(string));
				table.Columns.Add("X", typeof(float));
				table.Columns.Add("Y", typeof(float));
				table.Columns.Add("Z", typeof(float));
				for ( int j = 0; j < pfoArray[0].Charges.Length; j++ )
				{
					table.Columns.Add(pfoArray[0].Charges[j].name + " : Atom Names", typeof(string));
					table.Columns.Add(pfoArray[0].Charges[j].name + " : Atom Charges", typeof(float));
				}

				for ( int j = 0; j < pfoArray[i].Positions.Length; j++ )
				{
					DataRow dr = table.NewRow();

					string parseName;
					// here we are assuming that gaussian added the hydrogens at the end of its definition files.
					// therefore the first files take the name in the PDB file, the rest are hydrogens
					if ( j < pfoArray[i].PDB_limitedPositions.Length )
					{
						if ( pfoArray[i].Positions[j].name == "H" )
						{
							Trace.WriteLine("Arg, Jim-lad - an error !!");
						}
						parseName = (" " + pfoArray[i].PDB_limitedPositions[j].name).PadRight(4,' ');
					}
					else
					{
						if ( pfoArray[i].Positions[j].name != "H" )
						{
							Trace.WriteLine("Arg, Jim-lad - an error !!");
						}
						parseName = " H  ";
					}

					AtomPrimitive ap = m.moleculePrimitive.GetAtomPrimitiveFromPDBID( parseName );
					if ( ap == null )
					{
						Trace.WriteLine("Null atom primitive at ID : " + j.ToString() );
					}

					Atom a = new Atom(
						j,
						j,
						ap,
						m,
						pfoArray[i].Positions[j].vector.xFloat,
						pfoArray[i].Positions[j].vector.yFloat,
						pfoArray[i].Positions[j].vector.zFloat
						);

					m.addAtom(a);
					collectedAtoms.addAtom(a);

					dr["AtomName"] = pfoArray[i].Positions[j].name;
					dr["X"] = pfoArray[i].Positions[j].vector.xFloat;
					dr["Y"] = pfoArray[i].Positions[j].vector.yFloat;
					dr["Z"] = pfoArray[i].Positions[j].vector.zFloat;

					for ( int k = 0; k < pfoArray[i].Charges.Length; k++ )
					{
						dr[pfoArray[i].Charges[k].name + " : Atom Names"] = pfoArray[i].Charges[k].namedFloats[j].name;
						dr[pfoArray[i].Charges[k].name + " : Atom Charges"] = pfoArray[i].Charges[k].namedFloats[j].dataFloat;
					}

					table.Rows.Add( dr );
					table.AcceptChanges();
				}
				data.Tables.Add(table);

				ParticleSystem ps = new ParticleSystem( pfoArray[i].Name );

				ps.BeginEditing();

					HetMolecules hm = new HetMolecules();
					hm.addMolecule( m );
					UoB.Research.Tools.PS_Tools.PerformProximityBonding( m );
					ps.AddMolContainer( hm );

				ps.EndEditing( false, true );

				m_ParticleSystems.Add(ps);
			}
			datagrid_Output.DataSource = data;
			numericUpDown1.Minimum = 0;
			numericUpDown1.Value = 0;
			numericUpDown1.Maximum = pfoArray.Length-1;
			//data.WriteXml(@"c:\out.jon");
		}

		private void numericUpDown1_ValueChanged(object sender, System.EventArgs e)
		{
			int val = (int)numericUpDown1.Value;
			ParticleSystem ps = (ParticleSystem) m_ParticleSystems[val];
			m_DrawWrap.particleSystem = ps;
			label_PSTitle.Text = ps.Name;
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			if ( pfoArray == null || m_ParticleSystems == null)
			{
				MessageBox.Show("Files Must FIrst Be Parsed ...");
			}
			if ( pfoArray.Length != m_ParticleSystems.Count )
			{
				MessageBox.Show("pfoArray.Length != m_ParticleSystems.Count");
			}

			string basePath = @"c:\_output\";
			if( !Directory.Exists( basePath ) )
			{
				Directory.CreateDirectory( basePath );
			}

			for ( int i = 0; i < pfoArray.Length; i++ )
			{
				for ( int j = 0; j < pfoArray[i].Charges.Length; j++ )
				{
					if( !Directory.Exists( basePath + j.ToString() ) )
					{
						Directory.CreateDirectory( basePath + j.ToString() );
					}

					string path = basePath + j.ToString() + @"\" + pfoArray[i].Name + ".pqr";
					ParticleSystem ps = (ParticleSystem) m_ParticleSystems[i];
					float[] charges = pfoArray[i].Charges[j].getFloatArray();

					PQR.outputPQRFile( path, ps, 2, charges );
				}
			}
		}

		private void button2_Click(object sender, System.EventArgs e)
		{

			ArrayList directoryObjects = new ArrayList();

			string[] directories = Directory.GetDirectories(this.textBox_RootDir.Text + this.textbox_Path4MeadOut.Text);
			for ( int i = 0; i < directories.Length; i++ )
			{
				ArrayList fileObjects = new ArrayList();
				string parseFilePath = directories[i] + @"\parse.set";
				if ( !File.Exists( parseFilePath ) )
				{
					Debug.WriteLine(directories[i] + " ignored as no parse.set file found...");
					continue;
				}

				string[] parseLines = FileScanTools.readParseFile( parseFilePath );

				string pattern = parseLines[0];

				string[] commandLines = new string[parseLines.Length-1]; // not passed by ref, but by value, therefore conserved per file
				Array.Copy(parseLines,1,commandLines,0,parseLines.Length-1);

				string[] files = Directory.GetFiles(directories[i], pattern);
				for ( int j = 0; j < files.Length; j++ )
				{
					fileObjects.Add( FileScanTools.parseFile(files[j], commandLines ) );                                        					
				}
				directoryObjects.Add(fileObjects);
			}


			// files all parsed
			// Begin Assembly Of Files into information structures

			ArrayList ar = (ArrayList) directoryObjects[0];
			ArrayList PostMeadPDOArrayList = new ArrayList();

			string lastMolName;
			string currentName;
			ArrayList dielStore = new ArrayList();
			ArrayList solvEStore = new ArrayList();

			// Firstly get the dgtr vaues from the .pdb files in the 1st directory object
			Hashtable dgtrValues = new Hashtable(100);
			ArrayList firstFileSet = (ArrayList) directoryObjects[0];
			FileObject[] firstfoArray = (FileObject[]) firstFileSet.ToArray(typeof(FileObject));
			for ( int i = 0; i < firstfoArray.Length; i++ )
			{
				FileInfo fi = new FileInfo( firstfoArray[i].name );
				string hashName = fi.Name.Split('.')[0];
				elementNamedFloat dgtr = (elementNamedFloat) firstfoArray[i][0];
				dgtrValues[hashName] = dgtr.dataFloat;                                
			}
			//All dgtr values have been extracted and indexed by molecule name

			//Now we get the solvation energies per file per quantum method
			//each directory object represents a set of quantum calcs for a single quantum method
			for ( int i = 1; i < directoryObjects.Count; i++ )
			{
				ArrayList PostMeadPFOArrayList = new ArrayList();
				ArrayList nextFileSet = (ArrayList) directoryObjects[i];
				FileObject[] nextfoArray = (FileObject[]) nextFileSet.ToArray(typeof(FileObject));

				lastMolName = "";
				bool firstTime = true;

				int j = 0;
				while( j < nextfoArray.Length )
				{
					if ( nextfoArray[j].Count != 3 ) Trace.WriteLine("Error --> nextfoArray.Length != 3");
					elementString molName = (elementString) nextfoArray[j][0];
					elementString internalDielec = (elementString) nextfoArray[j][1];
					elementNamedFloat solvationEnergy = (elementNamedFloat) nextfoArray[j][2];
					molName.dataString = molName.dataString.Trim();
					internalDielec.dataString = internalDielec.dataString.Trim();
					currentName = molName.dataString;

					if ( firstTime )
					{
						lastMolName = molName.dataString;
						firstTime = false;
					}

					if ( lastMolName != molName.dataString )
					{
						PostMeadPFO.ProcessedFileObject item = new PostMeadPFO.ProcessedFileObject();

						item.molName = lastMolName;

						item.dgtr = (float) dgtrValues[ item.molName ];

						item.dielectrics = (float[]) dielStore.ToArray( typeof(float) );
						dielStore.Clear();

						item.solvEnergies = (float[]) solvEStore.ToArray( typeof(float) );
						solvEStore.Clear();

						lastMolName = molName.dataString;

						PostMeadPFOArrayList.Add( item );
					}

					dielStore.Add( float.Parse( internalDielec.dataString ) );
					solvEStore.Add( solvationEnergy.dataFloat );
					j++;
				}   

				//repeat for last item 
				PostMeadPFO.ProcessedFileObject itemX = new PostMeadPFO.ProcessedFileObject();
				itemX.molName = lastMolName;
				itemX.dgtr = (float) dgtrValues[ itemX.molName ];
				itemX.dielectrics = (float[]) dielStore.ToArray( typeof(float) );
				dielStore.Clear();
				itemX.solvEnergies = (float[]) solvEStore.ToArray( typeof(float) );
				solvEStore.Clear();
				PostMeadPFOArrayList.Add( itemX );
				// end repeat

				PostMeadPFO.ProcessedFileObject[] array = (PostMeadPFO.ProcessedFileObject[]) PostMeadPFOArrayList.ToArray( typeof(PostMeadPFO.ProcessedFileObject) );
				PostMeadPFOArrayList.Clear();
				PostMeadPFO.ProcessedDirectoryObject obj = new UoB.PMS.PostMeadPFO.ProcessedDirectoryObject();
				obj.ProcessedFileObjects = array;
				DirectoryInfo di = new DirectoryInfo( directories[i] );
				obj.Name = di.Name;
				obj.Name = obj.Name.Split('_')[1];
				PostMeadPDOArrayList.Add( obj );
			}

			m_PostMeadPDOArray = (PostMeadPFO.ProcessedDirectoryObject[]) PostMeadPDOArrayList.ToArray( typeof(PostMeadPFO.ProcessedDirectoryObject) );
			PostMeadPDOArrayList.Clear();




			// Now we want to show our data !

			DataSet data = new DataSet("TheDataSet");

			bool firstDielSetAttempt = true;
			for ( int i = 0; i < m_PostMeadPDOArray.Length; i++ )
			{		
				ArrayList collectedAtoms = new ArrayList();

				DataTable table = new DataTable( m_PostMeadPDOArray[i].Name );

				//Add Columns
				table.Columns.Add("MolName", typeof(string));
				table.Columns.Add("DGtr", typeof(float));
				float[] diels = m_PostMeadPDOArray[i].ProcessedFileObjects[0].dielectrics;

				for ( int j = 0; j < diels.Length; j++ )
				{
					if ( firstDielSetAttempt )
					{
						DielOptMol.dielectricValues = diels;
						firstDielSetAttempt = false;
					}
					table.Columns.Add(diels[j].ToString(), typeof(float));
				}


				for ( int j = 0; j < m_PostMeadPDOArray[i].ProcessedFileObjects.Length; j++ )
				{
					PostMeadPFO.ProcessedFileObject pfo = m_PostMeadPDOArray[i].ProcessedFileObjects[j];

					DataRow dr = table.NewRow();

					dr["MolName"] = pfo.molName;
					dr["DGtr"] = pfo.dgtr;
					for ( int k = 0; k < pfo.dielectrics.Length; k++ )
					{
						dr[pfo.dielectrics[k].ToString()] = pfo.solvEnergies[k];
					}

					table.Rows.Add( dr );
					table.AcceptChanges();
				}
				data.Tables.Add(table);
			}
			datagrid_Output.DataSource = data;
			//data.WriteXml(@"c:\out_PostMead.jon");
		
		}

		private DielOptMol[] m_DielOptMol;

		private Random m_Random = new Random();

		public float soluteDiEl;
		public float bestSoluteDiEl; // best one so far during whole run
		public float bestScore;
		public float currentScore;

		public float[] bestConstants;
		public float[] currentConstants;

		private void button_Optimise_Click(object sender, System.EventArgs e)
		{
			// First get the SASA data generated previously
			string SASAFilename = this.textBox_RootDir.Text + @"_outputSASA\smallMoleculeSASAs.dat";

			if ( !File.Exists( SASAFilename ) )
			{
				MessageBox.Show( "SASA file for all molecules must first be generated from particle systems : " + "\n" + @"c:\_outputSASA\smallMoleculeSASAs.dat" );
				return;
			}

			if( !Directory.Exists( textBox_RootDir.Text + @"_results\" ) )
			{
				Directory.CreateDirectory( textBox_RootDir.Text + @"_results\" );
			}

			StreamReader re = new StreamReader( SASAFilename );
			Hashtable SASAs = new Hashtable();
			string inputLine = null;

			string titleLine = re.ReadLine();

			const int numberOfBlankCells = 2;

			while ((inputLine = re.ReadLine()) != null)
			{
				string[] SASAdata = inputLine.Split(',');
				//if ( SASAdata.Length != 6 ) throw new Exception( "SASA file is corrupt..." );
				// now using the 1st line to recognise the number of terms
				float[] AtomSASAs = new float[ SASAdata.Length - numberOfBlankCells ]; // -1 as the 1st cell is a title, the second is a total
				try
				{
					for( int i = 0; i < SASAdata.Length - numberOfBlankCells; i++ ) // 1st cell is a title, the second is a total
					{
						AtomSASAs[i] = float.Parse( SASAdata[i + numberOfBlankCells ] );
					}
				}
				catch( Exception ex )
				{
					throw new Exception( ex.ToString() + "\n\r\n\rThe following line could not be processed for a molecule : " + "\"" + inputLine + "\"" );
				}
				SASAs[ SASAdata[0] ] = 	AtomSASAs;		
			}


			DataSet data = (DataSet) datagrid_Output.DataSource;
			int tableNum = (int) num_QuantumTable.Value;

			if ( tableNum < 0 || tableNum >= data.Tables.Count )
			{
				MessageBox.Show( "That table number doesnt exist ..." );
				return;
			}

			DataTable table = data.Tables[tableNum];

			for ( int j = 0; j < table.Rows.Count; j++ )
			{
				m_DielOptMol[j] = new DielOptMol();
				DataRow row = table.Rows[j];
				m_DielOptMol[j].Name = (string) row["molName"];
				m_DielOptMol[j].DGtr = (float) row["DGtr"];

				m_DielOptMol[j].AtomTypeSASAs = (float[]) SASAs[ (string) row["molName"] ];

				m_DielOptMol[j].solvationEValues = new float[ table.Columns.Count - 2 ];
				for ( int i = 0; i < m_DielOptMol[j].solvationEValues.Length; i++ )
				{
					m_DielOptMol[j].solvationEValues[i] = (float) row[ table.Columns[i+2] ];
				}
			}

			optimise( m_DielOptMol[0].AtomTypeSASAs.Length );

		}

		private void button_OptimiseAll_Click(object sender, System.EventArgs e)
		{
			// First get the SASA data generated previously
			string SASAFilename = this.textBox_RootDir.Text + @"_outputSASA\smallMoleculeSASAs.dat";

			if ( !File.Exists( SASAFilename ) )
			{
				MessageBox.Show( "SASA file for all molecules must first be generated from particle systems : " + "\n" + @"c:\_outputSASA\smallMoleculeSASAs.dat" );
				return;
			}
			StreamReader re = new StreamReader( SASAFilename );
			Hashtable SASAs = new Hashtable();
			string inputLine = null;

			string titleLine = re.ReadLine();

			const int numberOfBlankCells = 2;

			while ((inputLine = re.ReadLine()) != null)
			{
				string[] SASAdata = inputLine.Split(',');
				//if ( SASAdata.Length != 6 ) throw new Exception( "SASA file is corrupt..." );
				// now using the 1st line to recognise the number of terms
				float[] AtomSASAs = new float[ SASAdata.Length - numberOfBlankCells ]; // -1 as the 1st cell is a title, the second is a total
				try
				{
					for( int i = 0; i < SASAdata.Length - numberOfBlankCells; i++ ) // 1st cell is a title, the second is a total
					{
						AtomSASAs[i] = float.Parse( SASAdata[i + numberOfBlankCells ] );
					}
				}
				catch( Exception ex )
				{
					throw new Exception( ex.ToString() + "\n\r\n\rThe following line could not be processed for a molecule : " + "\"" + inputLine + "\"" );
				}
				SASAs[ SASAdata[0] ] = 	AtomSASAs;		
			}

			DataSet data = (DataSet) datagrid_Output.DataSource;
			m_DielOptMol = new DielOptMol[ data.Tables[0].Rows.Count ];

			StreamWriter rw = new StreamWriter( this.textBox_RootDir.Text + @"_results/AllOutput.csv" );

			for ( int tableNum = 0; tableNum < data.Tables.Count; tableNum++ )
			{
				DataTable table = data.Tables[tableNum];

				for ( int j = 0; j < table.Rows.Count; j++ )
				{
					m_DielOptMol[j] = new DielOptMol();
					DataRow row = table.Rows[j];
					m_DielOptMol[j].Name = (string) row["molName"];
					m_DielOptMol[j].DGtr = (float) row["DGtr"];

					m_DielOptMol[j].AtomTypeSASAs = (float[]) SASAs[ (string) row["molName"] ];

					m_DielOptMol[j].solvationEValues = new float[ table.Columns.Count - 2 ];
					for ( int i = 0; i < m_DielOptMol[j].solvationEValues.Length; i++ )
					{
						m_DielOptMol[j].solvationEValues[i] = (float) row[ table.Columns[i+2] ];
					}
				}
				int constantCount = m_DielOptMol[0].AtomTypeSASAs.Length;

				rw.WriteLine("Starting for Table : " + tableNum.ToString() );
				rw.WriteLine("Dielectric Value,Best Score," + AtomTypeHeaders_NonArea );

				bestScore = float.MaxValue;
				currentScore = float.MaxValue;
				bestConstants = new float[ constantCount ];
				currentConstants = new float[ constantCount ];
				for( int i = 0; i < bestConstants.Length; i++ )
				{
					bestConstants[i] = 0.007f; // the default - i.e. the non atom specific constant
				}
				currentConstants = (float[])bestConstants.Clone();


				for ( int i = 0; i < DielOptMol.dielectricValues.Length; i++ )
				{
					soluteDiEl = DielOptMol.dielectricValues[i];
					bestScore = float.MaxValue;
					currentScore = float.MaxValue;
            
					int count = 0; // counting unsuccessful tries;
					while ( true )
					{
						for( int f = 0; f < currentConstants.Length; f++ )
						{
							mutate( ref currentConstants[f], 0.0001f );
						}
						currentScore = calculateScore();
						if ( currentScore < bestScore )
						{
							keep(true);
							count = 0;
						}
						else 
						{
							keep(false);
							count++;
							if ( count > 1000 )
							{
								break; // end the while loop ... the optimisation is over ...
							}
						}
					}

					rw.Write( soluteDiEl.ToString() + "," );
					rw.Write( bestScore.ToString() + "," );
					for( int k = 0; k < constantCount; k++ )
					{
						rw.Write( bestConstants[k] + "," );
					}
					rw.WriteLine();

				}

				rw.WriteLine("Table Done");
				rw.WriteLine();
				rw.WriteLine();
			}

			rw.Close();
			re.Close();		
		}

		private void optimise( int constantCount )
		{

			if( !Directory.Exists( textBox_RootDir.Text + @"_results\" ) )
			{
				 Directory.CreateDirectory( textBox_RootDir.Text + @"_results\" );
			}

			soluteDiEl = 1.0f; // initial value prior to optimisation
			bestSoluteDiEl = soluteDiEl;
			bestScore = float.MaxValue;
			currentScore = float.MaxValue;

			bestConstants = new float[ constantCount ];
			currentConstants = new float[ constantCount ];

			for( int i = 0; i < bestConstants.Length; i++ )
			{
				bestConstants[i] = 0.007f; // the default - i.e. the non atom specific constant
			}
			currentConstants = (float[])bestConstants.Clone();

//			// Run to calc scores for all available internal dielectrics
//			float[] scores = new float[ DielOptMol.dielectricValues.Length ];
//			for ( int i = 0; i < DielOptMol.dielectricValues.Length; i++ )
//			{
//				soluteDiEl = DielOptMol.dielectricValues[i];
//				scores[i] = calculateScore();
//			}

			int count = 0; // counting unsuccessful tries;
			while ( true )
			{
				//mutate( ref soluteDiEl, 0.1f );
				for( int i = 0; i < currentConstants.Length; i++ )
				{
					mutate( ref currentConstants[i], 0.0001f );
				}
				currentScore = calculateScore();
				if ( currentScore < bestScore )
				{
					keep(true);
					count = 0;
				}
				else 
				{
					keep(false);
					count++;
					if ( count > 1000 )
					{
						writeResult(true);
						break; // end the while loop ... the optimisation is over ...
					}
				}
			}

			string constString = "";
			foreach( float constant in bestConstants )
			{
				constString += constant.ToString();
				constString += ", ";
			}

			MessageBox.Show( 
				"Score : " + bestScore.ToString() + "\r\n" 
				+ "Best Dielectric : " + bestSoluteDiEl.ToString() + "\r\n"
				+ constString
				);
		}


		private float energyOfCurrentSoluteDiEl(DielOptMol mt)
		{
			for ( int i = 0; i < DielOptMol.dielectricValues.Length; i++ )
			{
				if ( soluteDiEl == DielOptMol.dielectricValues[i] )
				{
					return mt.solvationEValues[i];
				}
			}
			int lower;
			int upper;
			for ( int i = 0; i < DielOptMol.dielectricValues.Length; i++ )
			{
				if ( soluteDiEl <= DielOptMol.dielectricValues[i] )
				{
					if ( i == 0 ) break;
					lower = i - 1;
					upper = i;

					return ( ( ( ( soluteDiEl - DielOptMol.dielectricValues[lower] ) / ( DielOptMol.dielectricValues[upper]  -DielOptMol.dielectricValues[lower] ) ) * ( mt.solvationEValues[upper] - mt.solvationEValues[lower] ) ) + mt.solvationEValues[lower] );
				}
			}

			soluteDiEl = 1;
			return mt.solvationEValues[0];
			//throw new Exception("Requested soluteDiel is not present in the list");
		}

		private float calculateScore()
		{
			double tempTotal = 0.0; // used to find the square differences
			for ( int i = 0; i < m_DielOptMol.Length; i++ )
			{
				// Experimntal Hydration Energy = DGpol (from PB calc) + DGcavity + DGvdw
				// DGcavity + DGvdw ~ 0.007 kCal/mol/A^2 of SASA
				float surfaceTerm = 0.0f;
				for( int j = 0; j < bestConstants.Length; j++ )
				{
					surfaceTerm += m_DielOptMol[i].AtomTypeSASAs[j] * currentConstants[j];
				}
				float calcEnergy = energyOfCurrentSoluteDiEl(m_DielOptMol[i]) + surfaceTerm;
				float energyDiff = -m_DielOptMol[i].DGtr - calcEnergy ;
				tempTotal += ( (double)energyDiff * (double)energyDiff );
			}
			return (float) Math.Sqrt( tempTotal );         

		}

		public void mutate( ref float mutateVar, float mutateFactor )
		{
			mutateVar += (float)(getSign() * mutateFactor * (m_Random.NextDouble()));
		}

		private int getSign() // returns -1, 0, or +1
		{
			return m_Random.Next(-1,2); // for some reason 2 means between -1 min and 1 max ???
		}

		int count = 0;
		private void writeResult( bool forceWrite )
		{
			if( forceWrite || (0 == ( count % 100 )) ) // dump every 100 or if forced to do so
			{
				string directory = textBox_RootDir.Text + @"_results\";
				StreamWriter rw = new StreamWriter(directory + "_result.csv");

				if (forceWrite) Console.WriteLine("Forced Write");
				rw.WriteLine( AtomTypeHeaders );
				Console.WriteLine( AtomTypeHeaders );
				rw.WriteLine("");

				rw.WriteLine( "Score" + "," + "Best Dielectric" );
				string line = bestScore.ToString() + "," + bestSoluteDiEl.ToString() + ",";

				foreach( float constant in bestConstants )
				{
					line += constant.ToString() + ",";
				}
				rw.WriteLine( line );
				Console.WriteLine( line );

				rw.WriteLine("");

				rw.WriteLine("Molname,DGtr,currentE,Surface Term,Sum of surface and solvation,Difference");

				for( int i = 0; i < m_DielOptMol.Length; i++ )
				{
					float surfaceTerm = 0.0f;
					for( int j = 0; j < m_DielOptMol[i].AtomTypeSASAs.Length; j++ )
					{
						surfaceTerm += m_DielOptMol[i].AtomTypeSASAs[j] * currentConstants[j];
					}
					float currentEnergy = energyOfCurrentSoluteDiEl(m_DielOptMol[i]);
					rw.WriteLine(
						m_DielOptMol[i].Name + "," 
						+ (-m_DielOptMol[i].DGtr).ToString() + "," 
						+ currentEnergy.ToString() + ","
						+ surfaceTerm.ToString() + "," 
						+ ((float)(surfaceTerm + currentEnergy)).ToString() + ","
						+ ((float)(-m_DielOptMol[i].DGtr - (surfaceTerm + currentEnergy))).ToString()  
						);
				}
				rw.Close();
			}
			count++;
		}

		public void keep(bool keep)
		{
			if ( keep )
			{
				bestScore = currentScore;
				bestSoluteDiEl = soluteDiEl;
				bestConstants = (float[])currentConstants.Clone();
				writeResult(false);
			}
			else
			{
				currentScore = bestScore;
				soluteDiEl = bestSoluteDiEl;
				currentConstants = (float[])bestConstants.Clone();
			}
		}

		private void button_SASACalc_Click(object sender, System.EventArgs e)
		{
			if ( pfoArray == null || m_ParticleSystems == null)
			{
				MessageBox.Show("Files Must First Be Parsed ...");
			}
			if ( pfoArray.Length != m_ParticleSystems.Count )
			{
				MessageBox.Show("pfoArray.Length != m_ParticleSystems.Count");
			}

			string basePath = this.textBox_RootDir.Text +  @"_outputSASA\";
			if( !Directory.Exists( basePath ) )
			{
				Directory.CreateDirectory( basePath );
			}

			StreamWriter rw = new StreamWriter( basePath + "smallMoleculeSASAs.dat" );

			rw.WriteLine( AtomTypeHeaders );

			Calculations forceField = new Calculations();

			for ( int i = 0; i < m_ParticleSystems.Count; i++ )
			{
				ParticleSystem ps = (ParticleSystem) m_ParticleSystems[i];

				float C_Areas = 0.0f;
				float CAR_Areas = 0.0f;
				float N_Areas = 0.0f;
				float NPolar_Areas = 0.0f;
				float S_Areas = 0.0f;
				float O_Areas = 0.0f;
				float OPolar_Areas = 0.0f;

				AtomList nonHydrogenAtoms = new AtomList();
				for( int k = 0; k < ps.Count; k++ )
				{
					if ( ps[k].atomPrimitive.Element != 'H' )
					{
						nonHydrogenAtoms.addAtom( ps[k] );
					}
				}
				forceField.connectToAtomList( nonHydrogenAtoms );
				float[] atomSASAs = forceField.calcSurface( SASAParamList.areaSASA );

				// H,CA,CBB,CH,CH2,C2B,C2C,CH3,CAR,CA2,CAB,CIM,CKA,CKC,SH,S,NBB,NAT,NIM,NH2,NR2,NH3,OBB,OAM,OCA,OH,OHA,HOH

				for ( int j = 0; j < nonHydrogenAtoms.Count; j++ )
				{
					switch( ps[j].atomPrimitive.Element )
					{
						// CA,CBB,CH,CH2,C2B,C2C,CH3,CAR,CA2,CAB,CIM,CKA,CKC
						case 'C':
							// CA,CBB,CH,CH2,C2B,C2C,CH3,CIM,CKA,CKC
							if ( ps[j].atomPrimitive.PDBIdentifier == " CA " ||
								 ps[j].atomPrimitive.PDBIdentifier == " CBB" ||
								 ps[j].atomPrimitive.PDBIdentifier == " CH " ||
								 ps[j].atomPrimitive.PDBIdentifier == " CH2" ||
								 ps[j].atomPrimitive.PDBIdentifier == " C2B" ||
								 ps[j].atomPrimitive.PDBIdentifier == " C2C" ||
								 ps[j].atomPrimitive.PDBIdentifier == " CH3" ||
								 ps[j].atomPrimitive.PDBIdentifier == " CIM" ||
								 ps[j].atomPrimitive.PDBIdentifier == " CKA" ||
								 ps[j].atomPrimitive.PDBIdentifier == " CKC"     )
							{
								C_Areas += atomSASAs[j];
							} 
							// CAR,CA2,CAB
							else if ( ps[j].atomPrimitive.PDBIdentifier == " CAR" ||
								      ps[j].atomPrimitive.PDBIdentifier == " CAB" ||
								      ps[j].atomPrimitive.PDBIdentifier == " CA2" )
							{
								CAR_Areas += atomSASAs[j];
							}
							else
							{
								throw new Exception("Non-standard C type found");							
							}
							break;
						case 'N':
							// NBB,NAT,NIM,NH2,NR2,NH3

							// NH2,NH3
							if ( ps[j].atomPrimitive.PDBIdentifier == " NH2" ||
								 ps[j].atomPrimitive.PDBIdentifier == " NH3"
							   )
							{

								NPolar_Areas += atomSASAs[j];
							}

							//NBB,NAT,NIM,NR2
							else if ( ps[j].atomPrimitive.PDBIdentifier == " NBB" ||
								      ps[j].atomPrimitive.PDBIdentifier == " NAT" ||
							     	  ps[j].atomPrimitive.PDBIdentifier == " NR2" ||
								      ps[j].atomPrimitive.PDBIdentifier == " NIM"    )
							{
								
								N_Areas += atomSASAs[j];
							}
							else
							{
								throw new Exception("Non-standard N type found");
							}
							break;
						case 'O':
							// OBB,OAM,OCA,OH,OHA,HOH
							// we will never find the HOH in small molecules - therefore counts as non-standard

							// OBB,OAM
							if ( ps[j].atomPrimitive.PDBIdentifier == " OCA" )
							{
								OPolar_Areas += atomSASAs[j];
							}
							// OBB,OAM,OH,OHA
							else if ( ps[j].atomPrimitive.PDBIdentifier == " OBB" ||
								      ps[j].atomPrimitive.PDBIdentifier == " OAM" ||
									  ps[j].atomPrimitive.PDBIdentifier == " OH " ||
									  ps[j].atomPrimitive.PDBIdentifier == " OHA"    )
							{
								O_Areas += atomSASAs[j];
							}
							else
							{
								throw new Exception("Non-standard O type found");
							}
							break;
						case 'S':
							if ( ps[j].atomPrimitive.PDBIdentifier == " S  " ||
								 ps[j].atomPrimitive.PDBIdentifier == " SH "    )
							{
								S_Areas += atomSASAs[j];
							}
							else
							{
								throw new Exception("Non-standard S type found");
							}
							break;
						default:
							throw new Exception("Unrecognised element : " + ps[j].atomPrimitive.Element.ToString() );
					}
				}

				float totalSASA = C_Areas + CAR_Areas + N_Areas + NPolar_Areas + O_Areas + OPolar_Areas + S_Areas;
//				rw.WriteLine( 
//					ps.Name + "," + 
//					totalSASA.ToString() + "," +
//					totalSASA.ToString()
//						);

//				rw.WriteLine( // just the atom types


				rw.WriteLine( 
					ps.Name + "," + 
					totalSASA.ToString() + "," + 
					C_Areas.ToString() + "," + 
					CAR_Areas.ToString() + "," + 
					N_Areas.ToString() + "," + 
					NPolar_Areas.ToString() + "," + 
					O_Areas.ToString() + "," + 
					OPolar_Areas.ToString() + "," + 
					S_Areas.ToString() 
					);
			}

			rw.Close();
		}

		private void button_Geom_Click(object sender, System.EventArgs e)
		{
			string inDir = this.textBox_RootDir.Text + this.text_Geom_In.Text;
			if( !Directory.Exists(inDir) ) 
			{
				MessageBox.Show("InDir Doesnt Exist");
				return;
			}
			string outDir = this.textBox_RootDir.Text + this.text_Geom_Out.Text;
			if( !Directory.Exists(outDir) ) 
			{
				MessageBox.Show("OutDir Doesnt Exist");
				return;
			}
			string posDir = this.textBox_RootDir.Text + this.text_Geom_Pos.Text;
			if( !Directory.Exists(posDir) ) 
			{
				MessageBox.Show("PosDir Doesnt Exist");
				return;
			}

			ArrayList positionFileObjects = new ArrayList();
			string parseFilePath = posDir + @"\parse.set";
			if ( !File.Exists( parseFilePath ) )
			{
				MessageBox.Show("no parse.set file found in pos dir ...");
				return;
			}

			string[] parseLines = FileScanTools.readParseFile( parseFilePath );

			string pattern = parseLines[0];

			string[] commandLines = new string[parseLines.Length-1];
			Array.Copy(parseLines,1,commandLines,0,parseLines.Length-1); // ignoring the 1st line in the file which defines the file filter pattern
            
			string[] files = Directory.GetFiles(posDir, pattern);
			for ( int j = 0; j < files.Length; j++ )
			{
				positionFileObjects.Add( FileScanTools.parseFile(files[j], commandLines ) );                                        					
			}

			files = Directory.GetFiles(inDir, @"*.com");
			if( files.Length != positionFileObjects.Count )
			{
				MessageBox.Show("The In-Dir does not contain the same number of valid files as the pos-Dir");
				return;
			}

			StreamWriter rw;
			StreamReader re;
			ArrayList lines = new ArrayList();

			for( int i = 0; i < files.Length; i++ )
			{
				FileObject fo = (FileObject) positionFileObjects[i];
				FileInfo fi = new FileInfo( files[i] );
				string filename = fi.Name;
				elementString foEs = (elementString) fo[0];
				elementNamedVectorArray foENFA = (elementNamedVectorArray) fo[1];

				if ( filename != foEs.dataString )
				{
					MessageBox.Show("FileName does not match the dataString in the position file");
					return;
				}

				re = new StreamReader( files[i] );
				lines.Clear();

				string lineBuffer;
				while( ( lineBuffer = re.ReadLine() ) != null )
				{
					lines.Add( lineBuffer );
				}

				re.Close();

				string outPath = textBox_RootDir.Text + text_Geom_Out.Text + @"\" + filename + ".com";
				rw = new StreamWriter( outPath, false );
				rw.WriteLine(this.text_QuantumString.Text);
				int writeLineCount = 1;

				for ( int j = 1; j < 5; j++ )
				{
					rw.WriteLine( (string) lines[j] );
					writeLineCount++;
				}

				for ( int k = 0; k < foENFA.namedVectors.Length; k++ )
				{
					elementNamedVector envK = foENFA.namedVectors[k];
					string originalLine = (string) lines[writeLineCount];
					string newLine = originalLine.Substring( 0, 18 );
					newLine += envK.vector.x.ToString("0.00000000").PadLeft(12,' ');
					newLine += "  ";
					newLine += envK.vector.y.ToString("0.00000000").PadLeft(12,' ');
					newLine += "  ";
					newLine += envK.vector.z.ToString("0.00000000").PadLeft(12,' ');
					newLine += "  ";
					rw.WriteLine( newLine );
					writeLineCount++;
				}

				for( int m = writeLineCount; m < lines.Count; m++ )
				{
					rw.WriteLine( (string) lines[m] );
				}
				
				rw.Close();
			}
            MessageBox.Show("Done, wow ...");     
		}

		private void button_ValidateFolder_Click(object sender, System.EventArgs e)
		{
			string directory = this.textBox_RootDir.Text;
			if ( !Directory.Exists( directory ) )
			{
				MessageBox.Show("The directory in teh root text box doesnt exist");
			}


			int fileCounter = 0;
			int dirCounter = 0;
			bool allOK = true;
			string fileTypes = "*.log";

			allOK = assessDirectory( ref directory, ref fileTypes, ref fileCounter, ref dirCounter );
			if( !allOK )
			{
				MessageBox.Show("First Directory failed");
				return;
			}

			if ( fileCounter % 72 != 0 ) 
			{
				MessageBox.Show("Hu ???");
			}
			if ( fileCounter / 72 != dirCounter ) 
			{
				MessageBox.Show("Hu ???");
			}

			MessageBox.Show( fileCounter.ToString() + " files have been checked in " + dirCounter + " direcories and are ok." );
		}

		private bool assessDirectory( ref string directory, ref string fileTypes, ref int fileCounter, ref int dirCounter )
		{
			string compareString = null;
			string[] commandLines = new string[] {
													 "deadlines,95",
													 "readline,GaussinaCommand",
													 "done"
												 };

			string[] directories = Directory.GetDirectories(directory);

			if ( directories.Length != 0 )
			{
				foreach ( string dir in directories )
				{
					string forwardToDir = dir;
					bool allOK = assessDirectory( ref forwardToDir, ref fileTypes, ref fileCounter, ref dirCounter );
					if( !allOK )
					{
						MessageBox.Show("Directory : " + dir + "Contains invalid files");
						return false;
					}
				}
			}
			else
			{
				string[] files = Directory.GetFiles( directory, fileTypes );
				if( files.Length == 0 ) return true; // this isnt a quantum directory
				dirCounter++; // we only want to count the dirs with files - i.e. the ones containing the log files
				foreach ( string file in files )
				{
					if( files.Length != 72 )
					{
						MessageBox.Show("hu?????");
					}
					fileCounter++;
					FileObject fo = FileScanTools.parseFile(file, commandLines ); 
					elementString es = (elementString) fo[0];
					if ( compareString == null )
					{
						compareString = es.dataString;
					}
					else
					{
						if( compareString != es.dataString )
						{
							MessageBox.Show("Error found - File : " + file + " Does not contain the same gaussian descriptor" );
							return false;
						}
					}
				}
			}
			return true;
		}

		private void button_DumpInfo_Click(object sender, System.EventArgs e)
		{
			DataTable table = ((DataSet)datagrid_Output.DataSource).Tables[0];

			StreamWriter rw = new StreamWriter( this.textBox_RootDir.Text + @"_results/InfoDump.csv" );
			rw.WriteLine("Type,Name,InfoName,Experimental Solvation Energy");

			for ( int j = 0; j < table.Rows.Count; j++ )
			{
				DataRow row = table.Rows[j];
				string infoName = (string) row["molName"];
				string[] molNametemp = infoName.Split('_');
				string molName = molNametemp[molNametemp.Length-1];
				string molType = molNametemp[0];

				if( molNametemp.Length > 2 )
				{
					if( molNametemp[1] == "Cyc" )
					{
						// no need
						//molName = "Cyclic " + molName;
					}
					else
					{
						molType = "\"" + molType + ", " + molNametemp[1] + "\"";
					}
				}

				// scan for numbers
				int i = 0;
				while( i < molName.Length )
				{
					if( char.IsNumber( molName[i] ) )
					{
						string temp1 = "";
						if( i-1 >= 0 )
						{
								temp1 = molName.Substring( 0, i ) + '-';
						}
						string temp2 = molName.Substring( i+1, molName.Length - i - 1);
						char number = molName[i];
						molName = temp1 + number + '-' + temp2;
                        i += 2; // we have added a '-'
					}
					i++;
				}

				// scan triplets
				for( int k = 0; k < molName.Length; k++ )
				{
					if( k == molName.Length - 3 )
					{
						break;
					}

					char char1 = molName[k];
					char char2 = molName[k+1];
					char char3 = molName[k+2];

					if( char.IsNumber( char1 ) && char2 == '-' && char.IsNumber( char3 ) )
					{
						string temp1 = molName.Substring( 0, k+1 );
						string temp2 = molName.Substring( k+2, molName.Length - k - 2);
						if( molName[0] != '"' )
						{
							molName = "\"" + temp1 + ',' + temp2 + "\"";
						}
						else
						{
							molName = temp1 + ',' + temp2;
						}
					}
				}

				// Capitalise

				int p = 0;
				while( p < molName.Length )
				{
					if( molName[p] == '-' || molName[p] == ' ' )
					{
						string temp1 = "";
						if( p-1 >= 0 )
						{
							temp1 = molName.Substring( 0, p+1 );
						}
						string temp2 = molName.Substring( p+2, molName.Length - p - 2);
						char capChar = molName[p+1];
						capChar = char.ToUpper( capChar );
						molName = temp1 + capChar + temp2;
						p += 2;
					}
					p++;
				}
			

				rw.Write(molType);
				rw.Write(',');
				rw.Write(molName);
				rw.Write(',');
				rw.Write(infoName);
				rw.Write(',');
				rw.Write( ((float) row["DGtr"]).ToString() );
				rw.WriteLine();

			}

			rw.Close();

		}


	}
}
