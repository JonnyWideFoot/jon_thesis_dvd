using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using UoB.Core.Structure;
using UoB.Core.Structure.Builder;

namespace UoB.CoreControls.Builder
{
	/// <summary>
	/// Summary description for AtomRebuild.
	/// </summary>
	public class AtomRebuild : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button_GoRebuild;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.ComboBox combo_BuildMode;
		private System.Windows.Forms.CheckedListBox list_Chains;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.CheckBox check_keepUnknowns;

		private ParticleSystem m_PartSys;

		public AtomRebuild( ParticleSystem ps )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			m_PartSys = ps;

			// set up the boxes with the contents of the rebuild enums and the available chains of the PS
			foreach( string mode in Enum.GetNames( typeof(RebuildMode) ) )
			{
				combo_BuildMode.Items.Add( mode );
			}
			combo_BuildMode.SelectedIndex = 0;

			PSMolContainer[] members = m_PartSys.Members;
			for( int i = 0; i < members.Length; i++ )
			{
				list_Chains.Items.Add( members[i] );
				list_Chains.SetItemChecked( i, true );
			}
			
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
			this.combo_BuildMode = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.list_Chains = new System.Windows.Forms.CheckedListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.button_GoRebuild = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.check_keepUnknowns = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// combo_BuildMode
			// 
			this.combo_BuildMode.Location = new System.Drawing.Point(88, 128);
			this.combo_BuildMode.Name = "combo_BuildMode";
			this.combo_BuildMode.Size = new System.Drawing.Size(256, 21);
			this.combo_BuildMode.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 128);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 23);
			this.label1.TabIndex = 2;
			this.label1.Text = "Build Mode : ";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// list_Chains
			// 
			this.list_Chains.CheckOnClick = true;
			this.list_Chains.Location = new System.Drawing.Point(88, 8);
			this.list_Chains.Name = "list_Chains";
			this.list_Chains.Size = new System.Drawing.Size(256, 109);
			this.list_Chains.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(96, 64);
			this.label2.TabIndex = 4;
			this.label2.Text = "Keep Chains : ";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// button_GoRebuild
			// 
			this.button_GoRebuild.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_GoRebuild.Location = new System.Drawing.Point(264, 160);
			this.button_GoRebuild.Name = "button_GoRebuild";
			this.button_GoRebuild.Size = new System.Drawing.Size(80, 24);
			this.button_GoRebuild.TabIndex = 6;
			this.button_GoRebuild.Text = "Go Rebuild";
			this.button_GoRebuild.Click += new System.EventHandler(this.button_GoRebuild_Click);
			// 
			// button_Cancel
			// 
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(176, 160);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(80, 24);
			this.button_Cancel.TabIndex = 7;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// check_keepUnknowns
			// 
			this.check_keepUnknowns.Location = new System.Drawing.Point(8, 160);
			this.check_keepUnknowns.Name = "check_keepUnknowns";
			this.check_keepUnknowns.Size = new System.Drawing.Size(160, 24);
			this.check_keepUnknowns.TabIndex = 8;
			this.check_keepUnknowns.Text = "Keep current invalid atoms";
			// 
			// AtomRebuild
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(354, 189);
			this.Controls.Add(this.check_keepUnknowns);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_GoRebuild);
			this.Controls.Add(this.list_Chains);
			this.Controls.Add(this.combo_BuildMode);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AtomRebuild";
			this.ShowInTaskbar = false;
			this.Text = "Atom Rebuild";
			this.ResumeLayout(false);

		}
		#endregion

		private void button_Cancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void button_GoRebuild_Click(object sender, System.EventArgs e)
		{
			PS_Builder builder = new PS_Builder( m_PartSys );
			RebuildMode mode;
			try
			{
				mode = (RebuildMode) Enum.Parse( typeof(RebuildMode), (string)combo_BuildMode.SelectedItem, true );
			}
			catch
			{
				MessageBox.Show( this, "Please input a valid Rebuild Mode", "Mode parsing error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				return;
			}
			try
			{
				for( int i = 0; i < list_Chains.Items.Count; i++ )
				{
					if( !list_Chains.GetItemChecked( i ) )
					{
						m_PartSys.RemoveMember( (PSMolContainer) list_Chains.Items[i] );
					}
				}
				builder.RebuildTemplate( mode, check_keepUnknowns.Checked, true, false, false, false );
			}
			catch( BuilderException ex )
			{
				MessageBox.Show( this, "Builder Exception was thrown : " + ex.ToString(), "Builder Exception", MessageBoxButtons.OK, MessageBoxIcon.Error );
				return;
			}
			Close();	
		}
	}
}
