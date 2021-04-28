using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using UoB.Core.Tools;
using UoB.Core.Sequence;
using UoB.Core.Structure;
using UoB.Core.Primitives;
using UoB.CoreControls.Documents;

namespace UoB.CoreControls.ToolWindows
{
	/// <summary>
	/// Summary description for AtomSelections.
	/// </summary>
	public class PS_SequenceBox : System.Windows.Forms.UserControl, ITool
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ColorDialog colorDialog;

        private PSSequence m_Seq = null;
		private System.Windows.Forms.TextBox text_Info;
		private PSSequence m_InternalSeq = new PSSequence();

		public PS_SequenceBox()
		{
            Text = "Sequence Window";
			InitializeComponent();
			setup();
		}

		public PS_SequenceBox(PSSequence seq) // setup in static mode
		{
			m_Seq = seq;
			InitializeComponent();
			setup();
		}

		public void AttachToDocument( Document doc )
		{
			m_Seq = null;
			if ( doc != null )
			{
				for( int i = 0; i < doc.MemberCount; i++ )
				{
					m_Seq = doc[i] as PSSequence;
					if( m_Seq != null )
					{
						break; // only one seq should ever be present, so dont check any more tools
					}
				}
				if( m_Seq == null )
				{
					for( int i = 0; i < doc.MemberCount; i++ )
					{
						ParticleSystem ps = doc[i] as ParticleSystem;
						if( ps != null )
						{
							m_Seq = m_InternalSeq;
							m_InternalSeq.particleSystem = ps;
							break; // only one ps should ever be present, so dont check any more tools
						}
					}
				}
			}
			setup();
		}

		private void setup()
		{
			if( m_Seq != null )
			{
				text_Info.Text = m_Seq.ToString();
			}	
			else
			{
				text_Info.Text = "No Sequence information is present for the current document.";
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
			this.colorDialog = new System.Windows.Forms.ColorDialog();
			this.text_Info = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// text_Info
			// 
			this.text_Info.BackColor = System.Drawing.Color.White;
			this.text_Info.Dock = System.Windows.Forms.DockStyle.Fill;
			this.text_Info.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.text_Info.Location = new System.Drawing.Point(0, 0);
			this.text_Info.Multiline = true;
			this.text_Info.Name = "text_Info";
			this.text_Info.ReadOnly = true;
			this.text_Info.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.text_Info.Size = new System.Drawing.Size(280, 488);
			this.text_Info.TabIndex = 0;
			this.text_Info.Text = "";
			this.text_Info.WordWrap = false;
			// 
			// PS_SequenceBox
			// 
			this.Controls.Add(this.text_Info);
			this.Name = "PS_SequenceBox";
			this.Size = new System.Drawing.Size(280, 488);
			this.ResumeLayout(false);

		}
		#endregion

	}
}
