using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using UoB.Core;
using UoB.Core.Structure;

namespace UoB.CoreControls.Controls
{
	/// <summary>
	/// Summary description for PolymerSelection.
	/// </summary>
	public class PolymerSelection : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.ComboBox m_Box;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private ParticleSystem m_PS = null;
		private PSMolContainer m_Polymer = null;
		public event UpdateEvent PolymerChanged = null;
		private UpdateEvent m_PSContentUpdateHook = null;

		public PolymerSelection()
		{
			InitializeComponent();
			
			m_PSContentUpdateHook = new UpdateEvent( GetPolymers );
			PolymerChanged = new UpdateEvent( NullFunc );
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.m_Box = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// m_Box
			// 
			this.m_Box.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_Box.Location = new System.Drawing.Point(0, 0);
			this.m_Box.Name = "m_Box";
			this.m_Box.Size = new System.Drawing.Size(152, 21);
			this.m_Box.TabIndex = 0;
			this.m_Box.SelectedIndexChanged += new System.EventHandler(this.m_Box_SelectedIndexChanged);
			// 
			// PolymerSelection
			// 
			this.Controls.Add(this.m_Box);
			this.Name = "PolymerSelection";
			this.Size = new System.Drawing.Size(152, 24);
			this.ResumeLayout(false);

		}
		#endregion

		private void NullFunc()
		{
		}

		public ParticleSystem particleSystem
		{
			get
			{
				return m_PS;
			}
			set
			{
				if( m_PS != null )
				{
					m_PS.ContentUpdate -= m_PSContentUpdateHook;
				}
				m_PS = value;
				SetupControl();
				if( m_PS != null )
				{
					m_PS.ContentUpdate += m_PSContentUpdateHook;
				}
			}
		}

		public PSMolContainer Polymer
		{
			get
			{
				return m_Polymer;
			}
		}

		private void SetControlStatus( bool enable )
		{
			for( int i = 0; i < Controls.Count; i++ )
			{
				Controls[i].Enabled = enable;
			}
		}

		private void SetupControl()
		{
			if( m_PS != null )
			{
				GetPolymers();
				SetControlStatus( true );
			}
			else
			{
				SetControlStatus( false );
			}
		}

		private void GetPolymers()
		{
			m_Box.Items.Clear();
			m_Polymer = null;
			for( int i = 0; i < m_PS.MemberCount; i++ )
			{
				m_Box.Items.Add( m_PS.MemberAt(i) );
			}
			m_Box.SelectedIndex = 0;
		}

		private void m_Box_SelectedIndexChanged( object sender, System.EventArgs e )
		{
			m_Polymer = (PSMolContainer) m_Box.Items[m_Box.SelectedIndex];
			PolymerChanged();
		}
	}
}
