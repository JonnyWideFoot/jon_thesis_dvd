using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;

using UoB.Core.Structure;
using UoB.Core.FileIO.PDB;

namespace UoB.CoreControls.ToolWindows
{
	/// <summary>
	/// Summary description for TrajScroll.
	/// </summary>
	public class Model_Scroll : System.Windows.Forms.UserControl, ITool
	{
		protected System.Windows.Forms.TrackBar trackBar;
		protected System.Windows.Forms.Label label_Position;

		private int m_InitialProgressStringLength;
		protected StringBuilder m_ProgressString = new StringBuilder();
		protected PS_PositionStore m_Models;
		protected UoB.Core.UpdateEvent m_IndexUpdateEvent; // used so that the control can be unsubscribed from a trajectory when the active doc is changed

		public Model_Scroll( PS_PositionStore theModels )
		{
			init();
			AttachTo( theModels ); // calls UpdateDetails()
		}

		public Model_Scroll()
		{
			init();
			UpdateDetails();
		}

		private void init()
		{
			Text = "Model Scroll";
			InitializeComponent();
			m_IndexUpdateEvent = new UoB.Core.UpdateEvent( UpdateDetails );
			SetInitialProgressString();
			m_InitialProgressStringLength = m_ProgressString.Length;
			SetNullProgressText();
		}


		#region Setup Text
		protected virtual void SetInitialProgressString()
		{
			m_ProgressString.Append("Model Number ");
		}

		private void SetNullProgressText()
		{
			m_ProgressString.Remove(m_InitialProgressStringLength,m_ProgressString.Length-m_InitialProgressStringLength);
			m_ProgressString.Append("# / #");
			label_Position.Text = m_ProgressString.ToString();
		}

		protected virtual void PrintProgressCenter()
		{
			m_ProgressString.Append(m_Models.Position);
			m_ProgressString.Append(' ');
			m_ProgressString.Append('/');
			m_ProgressString.Append(' ');
			m_ProgressString.Append(m_Models.Count -1);
		}

		private void SetProgresText()
		{
			m_ProgressString.Remove(m_InitialProgressStringLength,m_ProgressString.Length-m_InitialProgressStringLength);
			PrintProgressCenter();
			label_Position.Text = m_ProgressString.ToString();
		}

		private void UpdateDetails()
		{
			if ( m_Models != null )
			{
				trackBar.Minimum = 0;
				trackBar.Maximum = m_Models.Count - 1;
				trackBar.Value = m_Models.Position;
				SetProgresText();
			}
			else
			{
				trackBar.Minimum = 0;
				trackBar.Maximum = 0;
				trackBar.Value = 0;
				SetNullProgressText();
			}
		}

		#endregion

		private void setControlState()
		{
			bool enabled = ( m_Models != null && m_Models.Count > 1 );
			for( int i = 0; i < Controls.Count; i++ )
			{
				Controls[i].Enabled = enabled;
			}
		}
		private void trackBar_Scroll(object sender, System.EventArgs e)
		{
			if ( m_Models != null )
			{
				m_Models.Position = trackBar.Value;
			}
		}


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.trackBar = new System.Windows.Forms.TrackBar();
			this.label_Position = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
			this.SuspendLayout();
			// 
			// trackBar
			// 
			this.trackBar.Dock = System.Windows.Forms.DockStyle.Top;
			this.trackBar.Location = new System.Drawing.Point(0, 0);
			this.trackBar.Name = "trackBar";
			this.trackBar.Size = new System.Drawing.Size(432, 42);
			this.trackBar.TabIndex = 0;
			this.trackBar.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackBar.Scroll += new System.EventHandler(this.trackBar_Scroll);
			// 
			// label_Position
			// 
			this.label_Position.BackColor = System.Drawing.Color.Transparent;
			this.label_Position.Location = new System.Drawing.Point(16, 24);
			this.label_Position.Name = "label_Position";
			this.label_Position.Size = new System.Drawing.Size(128, 16);
			this.label_Position.TabIndex = 7;
			this.label_Position.Text = "Model Number # / #";
			this.label_Position.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// Model_Scroll
			// 
			this.Controls.Add(this.label_Position);
			this.Controls.Add(this.trackBar);
			this.Name = "Model_Scroll";
			this.Size = new System.Drawing.Size(432, 48);
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

  		#region ITool Members
		public PS_PositionStore PositionArray
		{
			get
			{
				return m_Models;
			}
		}

		public void AttachTo( PS_PositionStore molStore )
		{
			if ( m_Models != null )
			{
				m_Models.IndexChanged -= m_IndexUpdateEvent;
			}

			m_Models = molStore;

			if ( m_Models != null )
			{
				m_Models.IndexChanged += m_IndexUpdateEvent;
			}

			UpdateDetails(); // tackbar and text
			setControlState(); // enabled state of all controls
		}

		public virtual void AttachToDocument(UoB.CoreControls.Documents.Document doc)
		{
			if ( doc != null )
			{
				for( int i = 0; i < doc.MemberCount; i++ )
				{
					PS_PositionStore ps = doc[i] as PS_PositionStore;
					if ( ps != null )
					{
						AttachTo( ps );
						break;
					}
				}
			}
		}

		#endregion

	}
}
