using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using UoB.Core;
using UoB.Core.Structure;

using UoB.External.Common;

namespace UoB.External.Minimisation
{
	/// <summary>
	/// Summary description for MinimizationInterface.
	/// </summary>
	public class MinimisationInterface : ProcessInterface
	{
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox box_ChainID;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox box_MinimisationLength;
		private System.Windows.Forms.Label label3;

		public event StringEvent FileDone;
		private ParticleSystem m_ParticleSystem;

		public MinimisationInterface( ParticleSystem ps ) : base()
		{
			InitializeComponent();
			m_ParticleSystem = ps;
			char chainID = ps.Members[0].ChainID; // just use the first one to start with
			box_ChainID.Text = new string( chainID, 1 ); 
			m_Process = new Minimization( ps, chainID );
			m_Process.FileDone += new UoB.Core.StringEvent(m_Minimisation_FileDone);
		}

		private void m_Minimisation_FileDone( string s )
		{
			if( !m_Process.WasKilled )
			{
				FileDone(s);
			}
			onProcessEnd();
		}

		protected override void SetEnabledForRunning( bool running )
		{
			base.SetEnabledForRunning ( running );
			box_MinimisationLength.Enabled = !running;
			box_ChainID.Enabled = !running;
		}

		protected override void button_Start_Click(object sender, System.EventArgs e)
		{
			try
			{
				int length = int.Parse( box_MinimisationLength.Text );
				if( length < 0 )
				{
					MessageBox.Show(this, "Length is invalid, it must be larger than 0", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    return;
				}
				((Minimization)m_Process).Length = length;

				char chainID = box_ChainID.Text[0];
				if( m_ParticleSystem.MemberWithID( chainID ) == null )
				{
					MessageBox.Show(this, "The Particle System does not contain a member with that chainID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					return;
				}
			}
			catch
			{
				MessageBox.Show(this, "Length is invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}

			SetEnabledForRunning(true);
            
			m_Process.Start();

			base.button_Start_Click( sender, e ); // starts the timer for progress bar update and sets m_IsRunning to true
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
			this.label3 = new System.Windows.Forms.Label();
			this.box_MinimisationLength = new System.Windows.Forms.TextBox();
			this.box_ChainID = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 136);
			this.label3.Name = "label3";
			this.label3.TabIndex = 6;
			this.label3.Text = "Steps : ";
			// 
			// box_MinimisationLength
			// 
			this.box_MinimisationLength.Location = new System.Drawing.Point(72, 136);
			this.box_MinimisationLength.Name = "box_MinimisationLength";
			this.box_MinimisationLength.TabIndex = 6;
			this.box_MinimisationLength.Text = "20000";
			// 
			// box_ChainID
			// 
			this.box_ChainID.Location = new System.Drawing.Point(72, 112);
			this.box_ChainID.MaxLength = 1;
			this.box_ChainID.Name = "box_ChainID";
			this.box_ChainID.TabIndex = 7;
			this.box_ChainID.Text = "";
			this.box_ChainID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.box_ChainID_KeyPress);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 112);
			this.label4.Name = "label4";
			this.label4.TabIndex = 8;
			this.label4.Text = "ChainID : ";
			// 
			// MinimizationInterface
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(336, 165);
			this.Controls.Add(this.box_ChainID);
			this.Controls.Add(this.box_MinimisationLength);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.Name = "MinimizationInterface";
			this.Text = "Minimization Interface";
			this.Controls.SetChildIndex(this.label4, 0);
			this.Controls.SetChildIndex(this.label3, 0);
			this.Controls.SetChildIndex(this.box_MinimisationLength, 0);
			this.Controls.SetChildIndex(this.box_ChainID, 0);
			this.ResumeLayout(false);

		}
		#endregion

		private void box_ChainID_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			box_ChainID.Text = new string ( e.KeyChar, 1 ).ToUpper();
		}

	}
}
