using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;

using UoB.Core;
using UoB.Core.Primitives.Collections;
using UoB.Core.FileIO.Tra;
using UoB.Core.Data;
using UoB.CoreControls.Documents;
using UoB.CoreControls.ToolWindows;
using UoB.Core.Structure;

namespace UoB.CoreControls.TraInterface
{
	/// <summary>
	/// Summary description for TraDataBox.
	/// </summary>
	public class TraDataBox : System.Windows.Forms.UserControl, ITool
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel2;

		private System.Windows.Forms.Button button_ToggleShow;
		private System.Windows.Forms.CheckBox check_FollowTra;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.TextBox textBox;

		private DataManager m_DataManager;
		private StringBuilder m_StringBuilder;
		private UpdateEvent m_Update;
		private PS_PositionStore m_PosStore;
		private StringStore m_CommentStore;
		private bool m_ShowingData = true; // otherwise we are showing comments ...
        private bool m_SetupInProgress = false;
        private Button button_Refresh;
		private String[] m_DataTitles = null;

		public TraDataBox(DataManager dataManager)
		{
			m_DataManager = dataManager;
			m_StringBuilder = new StringBuilder();
            InitializeComponent();
            setup();
            m_Update = new UpdateEvent(UpdateValues);
		}

		public TraDataBox()
		{
			m_DataManager = null;
			m_StringBuilder = new StringBuilder();
			InitializeComponent();
            setup();
            m_Update = new UpdateEvent(UpdateValues);
		}

        public void UpdateValues()
        {
            SetRange();
            PopulateText();               
        }

        public void SetRange()
        {
            m_SetupInProgress = true;

            if (m_DataManager != null && m_PosStore != null)
            {
                numericUpDown1.Maximum = m_PosStore.Count - 2;
                numericUpDown1.Minimum = -1;
                numericUpDown1.Value = m_PosStore.Position - 1;
            }
            else
            {
                numericUpDown1.Maximum = 0;
                numericUpDown1.Minimum = 0;
                numericUpDown1.Value = 0;                
            }

            m_SetupInProgress = false;            
        }

		public void PopulateText()
		{
            if (m_DataManager != null && m_PosStore != null)
            {
			    if ( (int)numericUpDown1.Value > -1 )
			    {
				    if( !m_ShowingData && m_CommentStore != null )
				    {
					    m_StringBuilder.Remove(0, m_StringBuilder.Length );

					    m_StringBuilder.Append( "\r\nPosition in Tra : " );
					    m_StringBuilder.Append( m_PosStore.Position - 1 );
    					
					    if( m_CommentStore.currentString.Length == 0 )
					    {
						    m_StringBuilder.Append( "\r\n\r\nNo comment present in the file..." );
					    }
					    else
					    {
						    m_StringBuilder.Append( "\r\n\r\n" );
						    m_StringBuilder.Append( m_CommentStore.currentString );
					    }

					    textBox.Text = m_StringBuilder.ToString();
				    }
				    else // data mode
				    {
					    m_StringBuilder.Remove(0, m_StringBuilder.Length );

					    float[] values;
					    m_DataManager.DataSummary( (int)numericUpDown1.Value, out values ); // entry 0 is the data for the target strcuture, added as 0's, and therefore has no associated TE energy definiton

					    if ( m_DataTitles == null || values.Length != m_DataTitles.Length )
					    {
						    m_DataManager.DataSummaryTitles( out m_DataTitles );
					    }
    				
					    m_StringBuilder.Append( "\r\nPosition in Tra : " );
					    m_StringBuilder.Append( m_PosStore.Position - 1 );
					    m_StringBuilder.Append( "\r\n\r\n" );
					    for ( int i = 0; i < values.Length; i++ )
					    {
						    m_StringBuilder.Append( "   " );
						    m_StringBuilder.Append( m_DataTitles[i] );
						    m_StringBuilder.Append( " : " );
						    m_StringBuilder.Append( values[i] );
						    m_StringBuilder.Append( "\r\n" );
					    }

					    textBox.Text = m_StringBuilder.ToString();
				    }
			    }
			    else
			    {
				    textBox.Text = "\r\nPosition in Tra : T \r\n\r\nThis is the initial system definition and therefore has no associated information.";
			    }

			    textBox.Select(0,0); // stops it doing the weird highlight thing
            }
            else
            {
                textBox.Text = "";
            }
		}

		public void AttachToDocument( Document doc )
		{
			m_DataManager = null;

			if ( m_PosStore != null )
			{
				m_PosStore.IndexChanged -= m_Update;
			}
			m_PosStore = null;
			m_CommentStore = null;

			if ( doc != null )
			{
				for( int i = 0; i < doc.MemberCount; i++ )
				{
					if ( doc[i] is DataManager )
					{
						m_DataManager = (DataManager) doc[i];
						break;
					}
				}

				for( int i = 0; i < doc.MemberCount; i++ ) // two for loops as the datamanager must be found before the Traj
				{
					m_PosStore = doc[i] as PS_PositionStore;
					if ( m_PosStore != null )
					{
						m_PosStore.IndexChanged += m_Update;
						check_FollowTra.Checked = true;
						m_DataManager.DataSummaryTitles( out m_DataTitles );
						break;
					}
				}

				//if ( m_PosStore == null )
				//{
				//might want to turn some shite off if we just have a data manager
				//}

				for( int i = 0; i < doc.MemberCount; i++ )
				{
					Tra t = doc[i] as Tra;
					if( t != null )
					{
						m_CommentStore = t.ExtendedComments;
						break;
					}
				}
			}

			setup(); // from what we have just found ...
		}

		private void setup()
		{
			for( int i = 0; i < Controls.Count; i++ )
			{
				Controls[i].Enabled = ( m_DataManager != null );
			}

			if( m_CommentStore != null && m_DataManager != null )
			{
				button_ToggleShow.Enabled = true;
			}
			else
			{
				button_ToggleShow.Enabled = false;
			}

            UpdateValues();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			m_PosStore.IndexChanged -= m_Update;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TraDataBox));
            this.panel1 = new System.Windows.Forms.Panel();
            this.button_ToggleShow = new System.Windows.Forms.Button();
            this.check_FollowTra = new System.Windows.Forms.CheckBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox = new System.Windows.Forms.TextBox();
            this.button_Refresh = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button_Refresh);
            this.panel1.Controls.Add(this.button_ToggleShow);
            this.panel1.Controls.Add(this.check_FollowTra);
            this.panel1.Controls.Add(this.numericUpDown1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(312, 32);
            this.panel1.TabIndex = 0;
            // 
            // button_ToggleShow
            // 
            this.button_ToggleShow.Location = new System.Drawing.Point(178, 8);
            this.button_ToggleShow.Name = "button_ToggleShow";
            this.button_ToggleShow.Size = new System.Drawing.Size(88, 23);
            this.button_ToggleShow.TabIndex = 8;
            this.button_ToggleShow.Text = "<Comments>";
            this.button_ToggleShow.Click += new System.EventHandler(this.button_ToggleShow_Click);
            // 
            // check_FollowTra
            // 
            this.check_FollowTra.Checked = true;
            this.check_FollowTra.CheckState = System.Windows.Forms.CheckState.Checked;
            this.check_FollowTra.Location = new System.Drawing.Point(36, 6);
            this.check_FollowTra.Name = "check_FollowTra";
            this.check_FollowTra.Size = new System.Drawing.Size(80, 24);
            this.check_FollowTra.TabIndex = 7;
            this.check_FollowTra.Text = "Follow Tra";
            this.check_FollowTra.CheckedChanged += new System.EventHandler(this.check_FollowTra_CheckedChanged);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(116, 6);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(56, 20);
            this.numericUpDown1.TabIndex = 6;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 32);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(312, 3);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBox);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 35);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(312, 389);
            this.panel2.TabIndex = 2;
            // 
            // textBox
            // 
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox.Location = new System.Drawing.Point(0, 0);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox.Size = new System.Drawing.Size(312, 389);
            this.textBox.TabIndex = 7;
            // 
            // button_Refresh
            // 
            this.button_Refresh.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_Refresh.Image = ((System.Drawing.Image)(resources.GetObject("button_Refresh.Image")));
            this.button_Refresh.Location = new System.Drawing.Point(6, 2);
            this.button_Refresh.Name = "button_Refresh";
            this.button_Refresh.Size = new System.Drawing.Size(24, 24);
            this.button_Refresh.TabIndex = 9;
            this.button_Refresh.Click += new System.EventHandler(this.button_Refresh_Click);
            // 
            // TraDataBox
            // 
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Name = "TraDataBox";
            this.Size = new System.Drawing.Size(312, 424);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void check_FollowTra_CheckedChanged(object sender, System.EventArgs e)
		{
			if ( m_PosStore != null )
			{
				if ( check_FollowTra.Checked )
				{
					m_PosStore.IndexChanged += m_Update;
				}
				else
				{
					m_PosStore.IndexChanged -= m_Update;
				}
			}
            UpdateValues();
		}

		private void numericUpDown1_ValueChanged(object sender, System.EventArgs e)
		{
			if( !m_SetupInProgress )
			{
				m_PosStore.Position = ((int) numericUpDown1.Value) + 1;
                // if(check_FollowTra.Checked) then we will respond to the update call, otherwise manual call now
                if (!check_FollowTra.Checked)
                {
                    Update();
                }
			}
		}

		private void button_ToggleShow_Click(object sender, System.EventArgs e)
		{
			m_ShowingData = !m_ShowingData;
			if( m_ShowingData )
			{
				button_ToggleShow.Text = "<Comments>";
			}
			else
			{
				button_ToggleShow.Text = "<Data>";
			}
			PopulateText();
		}

        private void button_Refresh_Click(object sender, EventArgs e)
        {
            UpdateValues();
        }
	}
}
