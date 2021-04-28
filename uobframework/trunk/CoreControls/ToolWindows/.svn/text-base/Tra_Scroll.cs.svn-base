using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using UoB.Core.Structure;
using UoB.Core.FileIO.Tra;

namespace UoB.CoreControls.ToolWindows
{
	/// <summary>
	/// Summary description for TrajScroll.
	/// </summary>
	public class Tra_Scroll : Model_Scroll
	{
		private System.ComponentModel.IContainer components;
		protected System.Windows.Forms.CheckBox check_Reset;
		protected System.Windows.Forms.Label label1;
		protected System.Windows.Forms.Button button_PlayPause;
		protected System.Windows.Forms.Button button_Prev;
		protected System.Windows.Forms.Button button_Next; 
		protected System.Windows.Forms.NumericUpDown numericUpDown;
		protected System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.ImageList imageList1; 

		protected System.Timers.Timer m_Timer;

		protected int m_StepSize = 1; // initially set to smooth
		protected readonly int m_InitialRefresh = 75;// a nice number
		protected readonly int m_MaxRefresh = 25; // 25 FPS

		public Tra_Scroll( Tra tra ) : base( tra.PositionDefinitions )
		{
			InitializeComponent();
			init( tra );
		}

		public Tra_Scroll() : base()
		{
			InitializeComponent();
			init( null );
		}

		private bool m_ReportSkipping = false;
		private int m_StartEntry = 0;
		private int m_SkipBy = 1;
		private int m_Total = 0;

		public void SetupSkippingReport(Tra tra)
		{
			m_StartEntry = tra.TraPreview.startPoint;
			m_SkipBy = tra.TraPreview.skipLength;
			m_Total = tra.TraPreview.numberOfEntries;
			m_ReportSkipping = true;
		}

		public void DisableSkippingReport()
		{
			m_StartEntry = 0;
			m_SkipBy = 1;
			m_Total = 0;
			m_ReportSkipping = false;
		}

		protected override void Dispose(bool disposing)
		{
			m_Timer.Stop();
			base.Dispose(disposing);
		}

		protected override void SetInitialProgressString()
		{
			m_ProgressString.Append("Tra Position ");
		}

		protected override void PrintProgressCenter()
		{
			int pos = m_Models.Position; 
			if( pos == 0 )
			{
				m_ProgressString.Append('T');
			}
			else
			{
				m_ProgressString.Append(pos-1);
			}
			m_ProgressString.Append(' ');
			m_ProgressString.Append('/');
			m_ProgressString.Append(' ');
			m_ProgressString.Append(m_Models.Count - 2);

			if( m_ReportSkipping )
			{
				m_ProgressString.Append(' ');
				m_ProgressString.Append('(');
				if( pos == 0 )
				{
					m_ProgressString.Append('T');
				}
				else
				{
					m_ProgressString.Append(m_StartEntry+((pos-1)*m_SkipBy));
				}
				m_ProgressString.Append(' ');
				m_ProgressString.Append('/');
				m_ProgressString.Append(' ');
				m_ProgressString.Append(m_Total-1);
				m_ProgressString.Append(')');
			}
		}


		private void init( Tra tra )
		{
			if( tra != null )
			{
				SetupSkippingReport(tra);
			}
			
			Text = "Tra Scroll";
			m_Timer = new System.Timers.Timer( m_InitialRefresh );
			m_Timer.Elapsed += new System.Timers.ElapsedEventHandler( Advance );
			numericUpDown.Value = (int)(1000.0f / (float)m_InitialRefresh);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Tra_Scroll));
			this.button_PlayPause = new System.Windows.Forms.Button();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.numericUpDown = new System.Windows.Forms.NumericUpDown();
			this.check_Reset = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.button_Prev = new System.Windows.Forms.Button();
			this.button_Next = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// trackBar
			// 
			this.trackBar.Name = "trackBar";
			this.trackBar.Size = new System.Drawing.Size(176, 42);
			this.trackBar.Scroll += new System.EventHandler(this.trackBar_Scroll);
			// 
			// label_Position
			// 
			this.label_Position.Location = new System.Drawing.Point(8, 24);
			this.label_Position.Name = "label_Position";
			this.label_Position.Size = new System.Drawing.Size(152, 16);
			this.label_Position.Text = "Tra Number # / #";
			this.label_Position.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// button_PlayPause
			// 
			this.button_PlayPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_PlayPause.ImageIndex = 1;
			this.button_PlayPause.ImageList = this.imageList1;
			this.button_PlayPause.Location = new System.Drawing.Point(48, 48);
			this.button_PlayPause.Name = "button_PlayPause";
			this.button_PlayPause.Size = new System.Drawing.Size(24, 23);
			this.button_PlayPause.TabIndex = 1;
			this.button_PlayPause.Click += new System.EventHandler(this.button_PlayPause_Click);
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// numericUpDown
			// 
			this.numericUpDown.Location = new System.Drawing.Point(112, 72);
			this.numericUpDown.Maximum = new System.Decimal(new int[] {
																		  999,
																		  0,
																		  0,
																		  0});
			this.numericUpDown.Minimum = new System.Decimal(new int[] {
																		  1,
																		  0,
																		  0,
																		  0});
			this.numericUpDown.Name = "numericUpDown";
			this.numericUpDown.Size = new System.Drawing.Size(56, 20);
			this.numericUpDown.TabIndex = 3;
			this.numericUpDown.Value = new System.Decimal(new int[] {
																		20,
																		0,
																		0,
																		0});
			this.numericUpDown.KeyUp += new System.Windows.Forms.KeyEventHandler(this.numericUpDown_KeyUp);
			this.numericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
			// 
			// check_Reset
			// 
			this.check_Reset.Checked = true;
			this.check_Reset.CheckState = System.Windows.Forms.CheckState.Checked;
			this.check_Reset.ImageIndex = 4;
			this.check_Reset.ImageList = this.imageList1;
			this.check_Reset.Location = new System.Drawing.Point(112, 48);
			this.check_Reset.Name = "check_Reset";
			this.check_Reset.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.check_Reset.Size = new System.Drawing.Size(40, 24);
			this.check_Reset.TabIndex = 5;
			this.toolTip.SetToolTip(this.check_Reset, "Reset When Reach End Of Tra-Entries");
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 72);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(112, 24);
			this.label1.TabIndex = 6;
			this.label1.Text = "Desired Speed (FPS)";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// button_Prev
			// 
			this.button_Prev.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_Prev.ImageIndex = 0;
			this.button_Prev.ImageList = this.imageList1;
			this.button_Prev.Location = new System.Drawing.Point(16, 48);
			this.button_Prev.Name = "button_Prev";
			this.button_Prev.Size = new System.Drawing.Size(24, 23);
			this.button_Prev.TabIndex = 8;
			this.button_Prev.Click += new System.EventHandler(this.button_Prev_Click);
			// 
			// button_Next
			// 
			this.button_Next.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button_Next.ImageIndex = 3;
			this.button_Next.ImageList = this.imageList1;
			this.button_Next.Location = new System.Drawing.Point(80, 48);
			this.button_Next.Name = "button_Next";
			this.button_Next.Size = new System.Drawing.Size(24, 23);
			this.button_Next.TabIndex = 9;
			this.button_Next.Click += new System.EventHandler(this.button_Next_Click);
			// 
			// Tra_Scroll
			// 
			this.Controls.Add(this.numericUpDown);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.check_Reset);
			this.Controls.Add(this.button_Prev);
			this.Controls.Add(this.button_PlayPause);
			this.Controls.Add(this.button_Next);
			this.Name = "Tra_Scroll";
			this.Size = new System.Drawing.Size(176, 104);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TrajScroll_KeyDown);
			this.Controls.SetChildIndex(this.trackBar, 0);
			this.Controls.SetChildIndex(this.button_Next, 0);
			this.Controls.SetChildIndex(this.button_PlayPause, 0);
			this.Controls.SetChildIndex(this.button_Prev, 0);
			this.Controls.SetChildIndex(this.check_Reset, 0);
			this.Controls.SetChildIndex(this.label1, 0);
			this.Controls.SetChildIndex(this.numericUpDown, 0);
			this.Controls.SetChildIndex(this.label_Position, 0);
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void Advance( object sender, System.Timers.ElapsedEventArgs e )
		{
			if( m_Models != null )
			{
				if( m_Models.Position + m_StepSize < m_Models.Count )
				{
					m_Models.Position += m_StepSize;
				}
				else if ( check_Reset.Checked )
				{
					m_Models.Position = 0;
				}
				else
				{
					TimerEnabled = false;
				}
			}
			else
			{
				TimerEnabled = false;
			}
		}

		private void numericUpDown_ValueChanged(object sender, System.EventArgs e)
		{
			setSpeed();
		}

		private void setSpeed()
		{
			int speed = (int)numericUpDown.Value;
			if( speed <= m_MaxRefresh )
			{
				m_StepSize = 1;
				m_Timer.Interval = (int)(1000 / (float)speed);
			}
			else
			{
				m_StepSize = (speed / m_MaxRefresh) + 1;
				int desiredInterval = (int)( 1000 / (float)speed );
				m_Timer.Interval = ( desiredInterval * m_StepSize );
			}
		}

		private bool TimerEnabled
		{
			get
			{
				return m_Timer.Enabled;
			}
			set
			{
				m_Timer.Enabled = value;
				if( value )
				{
					button_PlayPause.ImageIndex = 2;
				}
				else
				{
					button_PlayPause.ImageIndex = 1;
				}
			}
		}

		private void TrajScroll_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch(	e.KeyCode )
			{
				case Keys.Left:
				{
					numericUpDown.Value--;
					break;
				}
				case Keys.Right:
				{
					numericUpDown.Value++;
					break;
				}
				case Keys.Up:
				{
					numericUpDown.Value++;
					break;
				}
				case Keys.Down:
				{
					numericUpDown.Value--;
					break;
				}
				case Keys.Add:
				{
					numericUpDown.Value++;
					break;
				}
				case Keys.Subtract:
				{
					numericUpDown.Value--;
					break;
				}
			}
		}

		private void button_PlayPause_Click(object sender, EventArgs e)
		{
			TimerEnabled = !TimerEnabled;
		}

		private void button_Prev_Click(object sender, System.EventArgs e)
		{
			TimerEnabled = false;
			if ( m_Models != null )
			{
				m_Models.Position--;
			}
		}

		private void button_Next_Click(object sender, System.EventArgs e)
		{
			TimerEnabled = false;
			if ( m_Models != null )
			{
				m_Models.Position++;
			}
		}

		private void trackBar_Scroll(object sender, System.EventArgs e)
		{
			TimerEnabled = false;	
		}

		private void numericUpDown_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			setSpeed();
		}

		
		public void AttachTo( Tra traj )
		{
			if( traj != null )
			{
				base.AttachTo( traj.PositionDefinitions );
				SetupSkippingReport( traj );
			}
			else
			{
				base.AttachTo( null );
				DisableSkippingReport();
			}
		}

		public override void AttachToDocument( UoB.CoreControls.Documents.Document doc )
		{
			DisableSkippingReport(); // do this first, we will re-enable if we find a Tra

			if ( doc != null )
			{
				for( int i = 0; i < doc.MemberCount; i++ )
				{
					// first look for a tra file to setup skipping from
					if( typeof(Tra) == doc[i].GetType() )
					{
						SetupSkippingReport( (Tra) doc[i] );
						break;
					}
				}
			}

			base.AttachToDocument( doc ); // does m_Models
		}

	}
}
