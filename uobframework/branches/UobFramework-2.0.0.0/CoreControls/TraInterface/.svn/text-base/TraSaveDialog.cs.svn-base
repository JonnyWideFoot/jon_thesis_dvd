using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using UoB.Core.FileIO.Tra;
using UoB.Core.FileIO.PDB;

namespace UoB.CoreControls.TraInterface
{
	/// <summary>
	/// Summary description for TraSaveDialog.
	/// </summary>
	public class TraSaveDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.RadioButton radio_SaveTra;
		private System.Windows.Forms.RadioButton radio_SavePDB;
		private System.Windows.Forms.GroupBox groupBox_SaveType;
		private System.Windows.Forms.Button button_Save;
		private System.Windows.Forms.GroupBox groupBox_TraPosition;
		private System.Windows.Forms.Button button_Cancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox check_InitialPositons;
		private System.Windows.Forms.CheckBox check_TraEntries;
		private System.Windows.Forms.TextBox box_StartIndex;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox check_EnergyInformation;
		private System.Windows.Forms.TextBox box_Interval;
		private System.Windows.Forms.TextBox box_EndIndex;
		private System.Windows.Forms.CheckBox check_ImproperTorsions;
		private System.Windows.Forms.CheckBox check_RebuildP;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button button_AdvancedEnable;
		private System.Windows.Forms.TextBox text_Descriptor;
		private System.Windows.Forms.TextBox text_AssociatedText;
		private System.Windows.Forms.Button button_RestoreDescriptor;



		private UoB.CoreControls.ToolWindows.Tra_Scroll tra_Scroll;
		private bool m_AdvancedIsExtended = true;
		private System.Windows.Forms.Label label6;
		private Tra m_Tra;

		public TraSaveDialog( Tra tra )
		{
			InitializeComponent();

			m_Tra = tra;

			if( tra == null )
			{
				MessageBox.Show( "TraFile was null!");
				Close();
			}

			tra_Scroll.AttachTo( tra );

			saveFileDialog.Filter = "All DAVE Readable Files (*.TRA;*.PDB)|*.TRA;*.PDB|PDB Files (*.PDB)|*.PDB|TRA files (*.TRA)|*.TRA|All files (*.*)|*.*"; 
	
			// now setup the advanced boxes

			DateTime date = DateTime.Now;
			string descriptor = "Dave tra generation from : " + tra.InternalName + "\r\nOccured on : " + date.ToShortDateString() + " at " + date.ToShortTimeString();
			descriptor += tra.Descriptor;
			if( descriptor.Length > 1024 )
			{
				descriptor = descriptor.Substring(0,1024);
			}            
			text_Descriptor.Text = descriptor;
			text_AssociatedText.Text = tra.AssociatedText;
			box_EndIndex.Text = (m_Tra.PositionDefinitions.Count -1).ToString();

		}

		private void ToggleAdvanced(object sender, System.EventArgs e)
		{
			if( m_AdvancedIsExtended )
			{
				Size = new Size( 296, 256 );
				m_AdvancedIsExtended = false;
			}
			else
			{
				Size = new Size( 760, 440 );
				m_AdvancedIsExtended = true;
			}
		}

		private void AdvancedEnabled(object sender, System.EventArgs e)
		{
			check_EnergyInformation.Enabled = radio_SaveTra.Checked;
			check_ImproperTorsions.Enabled = radio_SaveTra.Checked;
			check_InitialPositons.Enabled = radio_SaveTra.Checked;
			check_RebuildP.Enabled = radio_SaveTra.Checked;
			check_TraEntries.Enabled = radio_SaveTra.Checked;
			box_EndIndex.Enabled = radio_SaveTra.Checked;
			box_Interval.Enabled = radio_SaveTra.Checked;
			box_StartIndex.Enabled = radio_SaveTra.Checked;
			text_Descriptor.Enabled = radio_SaveTra.Checked;
			text_AssociatedText.Enabled = radio_SaveTra.Checked;
		}		

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
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
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.groupBox_SaveType = new System.Windows.Forms.GroupBox();
			this.radio_SavePDB = new System.Windows.Forms.RadioButton();
			this.radio_SaveTra = new System.Windows.Forms.RadioButton();
			this.button_Save = new System.Windows.Forms.Button();
			this.groupBox_TraPosition = new System.Windows.Forms.GroupBox();
			this.tra_Scroll = new UoB.CoreControls.ToolWindows.Tra_Scroll();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.button_AdvancedEnable = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this.check_RebuildP = new System.Windows.Forms.CheckBox();
			this.check_ImproperTorsions = new System.Windows.Forms.CheckBox();
			this.box_EndIndex = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.box_Interval = new System.Windows.Forms.TextBox();
			this.check_TraEntries = new System.Windows.Forms.CheckBox();
			this.check_EnergyInformation = new System.Windows.Forms.CheckBox();
			this.check_InitialPositons = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.box_StartIndex = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.text_Descriptor = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.text_AssociatedText = new System.Windows.Forms.TextBox();
			this.button_RestoreDescriptor = new System.Windows.Forms.Button();
			this.groupBox_SaveType.SuspendLayout();
			this.groupBox_TraPosition.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox_SaveType
			// 
			this.groupBox_SaveType.Controls.Add(this.radio_SavePDB);
			this.groupBox_SaveType.Controls.Add(this.radio_SaveTra);
			this.groupBox_SaveType.Location = new System.Drawing.Point(8, 120);
			this.groupBox_SaveType.Name = "groupBox_SaveType";
			this.groupBox_SaveType.Size = new System.Drawing.Size(272, 72);
			this.groupBox_SaveType.TabIndex = 0;
			this.groupBox_SaveType.TabStop = false;
			this.groupBox_SaveType.Text = "Save FileType";
			// 
			// radio_SavePDB
			// 
			this.radio_SavePDB.Location = new System.Drawing.Point(8, 40);
			this.radio_SavePDB.Name = "radio_SavePDB";
			this.radio_SavePDB.Size = new System.Drawing.Size(200, 24);
			this.radio_SavePDB.TabIndex = 1;
			this.radio_SavePDB.Text = "Save Frame as PDB File";
			this.radio_SavePDB.CheckedChanged += new System.EventHandler(this.AdvancedEnabled);
			// 
			// radio_SaveTra
			// 
			this.radio_SaveTra.Checked = true;
			this.radio_SaveTra.Location = new System.Drawing.Point(8, 16);
			this.radio_SaveTra.Name = "radio_SaveTra";
			this.radio_SaveTra.Size = new System.Drawing.Size(200, 24);
			this.radio_SaveTra.TabIndex = 0;
			this.radio_SaveTra.TabStop = true;
			this.radio_SaveTra.Text = "Save as Tra File";
			this.radio_SaveTra.CheckedChanged += new System.EventHandler(this.AdvancedEnabled);
			// 
			// button_Save
			// 
			this.button_Save.Location = new System.Drawing.Point(208, 200);
			this.button_Save.Name = "button_Save";
			this.button_Save.TabIndex = 2;
			this.button_Save.Text = "Save";
			this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
			// 
			// groupBox_TraPosition
			// 
			this.groupBox_TraPosition.Controls.Add(this.tra_Scroll);
			this.groupBox_TraPosition.Location = new System.Drawing.Point(8, 8);
			this.groupBox_TraPosition.Name = "groupBox_TraPosition";
			this.groupBox_TraPosition.Size = new System.Drawing.Size(272, 104);
			this.groupBox_TraPosition.TabIndex = 4;
			this.groupBox_TraPosition.TabStop = false;
			this.groupBox_TraPosition.Text = "Tra Position To Save";
			// 
			// tra_Scroll
			// 
			this.tra_Scroll.Location = new System.Drawing.Point(56, 16);
			this.tra_Scroll.Name = "tra_Scroll";
			this.tra_Scroll.Size = new System.Drawing.Size(160, 80);
			this.tra_Scroll.TabIndex = 0;
			// 
			// button_Cancel
			// 
			this.button_Cancel.Location = new System.Drawing.Point(128, 200);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.TabIndex = 5;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// button_AdvancedEnable
			// 
			this.button_AdvancedEnable.Location = new System.Drawing.Point(8, 200);
			this.button_AdvancedEnable.Name = "button_AdvancedEnable";
			this.button_AdvancedEnable.TabIndex = 12;
			this.button_AdvancedEnable.Text = "Advanced";
			this.button_AdvancedEnable.Click += new System.EventHandler(this.ToggleAdvanced);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.check_RebuildP);
			this.groupBox1.Controls.Add(this.check_ImproperTorsions);
			this.groupBox1.Controls.Add(this.box_EndIndex);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.box_Interval);
			this.groupBox1.Controls.Add(this.check_TraEntries);
			this.groupBox1.Controls.Add(this.check_EnergyInformation);
			this.groupBox1.Controls.Add(this.check_InitialPositons);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.box_StartIndex);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(8, 232);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(272, 200);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Tra File Settings";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(8, 136);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(248, 32);
			this.label6.TabIndex = 11;
			this.label6.Text = "Note : Entry 0 is the target structure and is therefore always saved into the res" +
				"ultant Tra file";
			// 
			// check_RebuildP
			// 
			this.check_RebuildP.Location = new System.Drawing.Point(8, 64);
			this.check_RebuildP.Name = "check_RebuildP";
			this.check_RebuildP.Size = new System.Drawing.Size(256, 24);
			this.check_RebuildP.TabIndex = 10;
			this.check_RebuildP.Text = "\"REBUILDP\" Blanking Block";
			// 
			// check_ImproperTorsions
			// 
			this.check_ImproperTorsions.Location = new System.Drawing.Point(8, 40);
			this.check_ImproperTorsions.Name = "check_ImproperTorsions";
			this.check_ImproperTorsions.Size = new System.Drawing.Size(256, 24);
			this.check_ImproperTorsions.TabIndex = 9;
			this.check_ImproperTorsions.Text = "Improper Torsion Definitions";
			// 
			// box_EndIndex
			// 
			this.box_EndIndex.Location = new System.Drawing.Point(200, 112);
			this.box_EndIndex.Name = "box_EndIndex";
			this.box_EndIndex.Size = new System.Drawing.Size(56, 20);
			this.box_EndIndex.TabIndex = 5;
			this.box_EndIndex.Text = "-1";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(168, 112);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(40, 24);
			this.label3.TabIndex = 8;
			this.label3.Text = "To :";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// box_Interval
			// 
			this.box_Interval.Location = new System.Drawing.Point(128, 112);
			this.box_Interval.Name = "box_Interval";
			this.box_Interval.Size = new System.Drawing.Size(40, 20);
			this.box_Interval.TabIndex = 4;
			this.box_Interval.Text = "1";
			// 
			// check_TraEntries
			// 
			this.check_TraEntries.Checked = true;
			this.check_TraEntries.CheckState = System.Windows.Forms.CheckState.Checked;
			this.check_TraEntries.Location = new System.Drawing.Point(8, 88);
			this.check_TraEntries.Name = "check_TraEntries";
			this.check_TraEntries.Size = new System.Drawing.Size(256, 24);
			this.check_TraEntries.TabIndex = 2;
			this.check_TraEntries.Text = "Trajectory Entries";
			// 
			// check_EnergyInformation
			// 
			this.check_EnergyInformation.Checked = true;
			this.check_EnergyInformation.CheckState = System.Windows.Forms.CheckState.Checked;
			this.check_EnergyInformation.Location = new System.Drawing.Point(8, 168);
			this.check_EnergyInformation.Name = "check_EnergyInformation";
			this.check_EnergyInformation.Size = new System.Drawing.Size(256, 24);
			this.check_EnergyInformation.TabIndex = 1;
			this.check_EnergyInformation.Text = "Energy Information";
			// 
			// check_InitialPositons
			// 
			this.check_InitialPositons.Checked = true;
			this.check_InitialPositons.CheckState = System.Windows.Forms.CheckState.Checked;
			this.check_InitialPositons.Enabled = false;
			this.check_InitialPositons.Location = new System.Drawing.Point(8, 16);
			this.check_InitialPositons.Name = "check_InitialPositons";
			this.check_InitialPositons.Size = new System.Drawing.Size(256, 24);
			this.check_InitialPositons.TabIndex = 0;
			this.check_InitialPositons.Text = "Atomic Positions and Bonding (Mandatory)";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(88, 112);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 23);
			this.label2.TabIndex = 7;
			this.label2.Text = "Every :";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// box_StartIndex
			// 
			this.box_StartIndex.Location = new System.Drawing.Point(48, 112);
			this.box_StartIndex.Name = "box_StartIndex";
			this.box_StartIndex.Size = new System.Drawing.Size(40, 20);
			this.box_StartIndex.TabIndex = 3;
			this.box_StartIndex.Text = "1";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 112);
			this.label1.Name = "label1";
			this.label1.TabIndex = 6;
			this.label1.Text = "From :";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// text_Descriptor
			// 
			this.text_Descriptor.Location = new System.Drawing.Point(288, 40);
			this.text_Descriptor.MaxLength = 1024;
			this.text_Descriptor.Multiline = true;
			this.text_Descriptor.Name = "text_Descriptor";
			this.text_Descriptor.Size = new System.Drawing.Size(456, 112);
			this.text_Descriptor.TabIndex = 8;
			this.text_Descriptor.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(288, 24);
			this.label4.Name = "label4";
			this.label4.TabIndex = 9;
			this.label4.Text = "Descriptor";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(288, 160);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(168, 23);
			this.label5.TabIndex = 10;
			this.label5.Text = "Associated File Information";
			// 
			// text_AssociatedText
			// 
			this.text_AssociatedText.Location = new System.Drawing.Point(288, 176);
			this.text_AssociatedText.MaxLength = 5120;
			this.text_AssociatedText.Multiline = true;
			this.text_AssociatedText.Name = "text_AssociatedText";
			this.text_AssociatedText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.text_AssociatedText.Size = new System.Drawing.Size(456, 256);
			this.text_AssociatedText.TabIndex = 11;
			this.text_AssociatedText.Text = "";
			// 
			// button_RestoreDescriptor
			// 
			this.button_RestoreDescriptor.Location = new System.Drawing.Point(584, 16);
			this.button_RestoreDescriptor.Name = "button_RestoreDescriptor";
			this.button_RestoreDescriptor.Size = new System.Drawing.Size(160, 23);
			this.button_RestoreDescriptor.TabIndex = 13;
			this.button_RestoreDescriptor.Text = "Restore from Source Tra File";
			this.button_RestoreDescriptor.Click += new System.EventHandler(this.button_RestoreDescriptor_Click);
			// 
			// TraSaveDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(752, 437);
			this.Controls.Add(this.button_RestoreDescriptor);
			this.Controls.Add(this.text_AssociatedText);
			this.Controls.Add(this.text_Descriptor);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.groupBox_TraPosition);
			this.Controls.Add(this.button_Save);
			this.Controls.Add(this.groupBox_SaveType);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.button_AdvancedEnable);
			this.Name = "TraSaveDialog";
			this.Text = "TraSaveDialog";
			this.groupBox_SaveType.ResumeLayout(false);
			this.groupBox_TraPosition.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void button_Save_Click(object sender, System.EventArgs e)
		{

			if( radio_SavePDB.Checked )
			{
				saveFileDialog.Filter = "PDB Files (*.PDB)|*.pdb";
				saveFileDialog.DefaultExt = "pdb";
			}
			else
			{
				saveFileDialog.DefaultExt = "tra";
				saveFileDialog.Filter = "TRA files (*.TRA)|*.tra"; 
			}

			try
			{
				// to save the file

				if( radio_SavePDB.Checked )
				{
					if(saveFileDialog.ShowDialog() == DialogResult.OK)
					{
						if( saveFileDialog.FileName == "" )
						{
							MessageBox.Show("A filename must be specified.");
							return;
						}
						else
						{
							PDB.SaveNew( saveFileDialog.FileName, m_Tra.particleSystem );
						}
					}
				}
				else
				{
					
					TraContents typeFlags = TraContents.Positions;

					if( check_EnergyInformation.Checked )
					{
						typeFlags |= TraContents.EnergyInfo;
					}

					if( check_ImproperTorsions.Checked )
					{
						typeFlags |= TraContents.Impropers;
					}

					if( check_RebuildP.Checked )
					{
						typeFlags |= TraContents.REBUILDP;
					}

					if( check_TraEntries.Checked )
					{
						typeFlags |= TraContents.TrajectoryEntries;
					}

					TraSaveInfo saveInfo = new TraSaveInfo( saveFileDialog.FileName, typeFlags );

					try
					{
						saveInfo.startIndex = int.Parse(box_StartIndex.Text);
						saveInfo.indexStepping = int.Parse(box_Interval.Text);
						saveInfo.endIndex = int.Parse(box_EndIndex.Text);
					}
					catch
					{
						MessageBox.Show(this,"The TE Import Indexes Are Not Numbers", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
						return;
					}

					int numberOfEntries = m_Tra.PositionDefinitions.Count;

					if( saveInfo.startIndex == 0 )
					{
						MessageBox.Show(this,"The Start Index should not be set to zero, this is the target structure and is automatically included", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
						return;
					}
					else if ( saveInfo.startIndex < 0 || saveInfo.startIndex > numberOfEntries -1 )
					{
						MessageBox.Show(this,"The Start Index is not in the valid range of numbers", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
						return;
					}
					if ( saveInfo.indexStepping < 1 )
					{
						MessageBox.Show(this,"Index Stepping cannot be less that 1", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
						return;
					}
					if ( saveInfo.endIndex < saveInfo.startIndex || saveInfo.endIndex > numberOfEntries -1 )
					{
						MessageBox.Show(this,"The End Index is not in the valid range of numbers", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
						return;
					}

					saveInfo.Text = text_AssociatedText.Text;
					saveInfo.Descriptor = text_Descriptor.Text;

					if(saveFileDialog.ShowDialog() == DialogResult.OK)
					{
						if( saveFileDialog.FileName == "" )
						{
							MessageBox.Show("A filename must be specified.");
							return;
						}
						else
						{
							m_Tra.Save( saveInfo );
						}
					}
				}
			}
			catch( TraException ex )
			{
				MessageBox.Show( this, ex.ToString(), "TraException", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			catch( System.IO.IOException ex )
			{
				MessageBox.Show( this, "Save Operation Failed.\r\n Could not open the file for writing\r\nError: " + ex.ToString(), "IOException", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			catch( Exception ex )
			{
				MessageBox.Show( this, "Save Operation Failed.\r\nError: " + ex.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			
			Close();
		}

		private void button_Cancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void button_RestoreDescriptor_Click(object sender, System.EventArgs e)
		{
			text_Descriptor.Text = m_Tra.Descriptor;
		}


	}
}
