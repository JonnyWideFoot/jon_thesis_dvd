using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using UoB.CoreControls.PS_Render;
using UoB.Core.Structure;
using UoB.Core.Primitives;
using UoB.Core.Primitives.Matrix;
using UoB.CoreControls.OpenGLView;
using UoB.CoreControls.Documents;
using UoB.Core;

namespace UoB.CoreControls.ToolWindows
{
	/// <summary>
	/// Summary description for AtomSelections.
	/// </summary>
	public class PS_Rotation : System.Windows.Forms.UserControl, ITool
	{
		private ParticleSystemDrawWrapper m_DrawWrapper;
		private Perspective m_Perspective;
		private MatrixRotation m_RotMat;
		private System.Windows.Forms.CheckBox check_X;
		private System.Windows.Forms.CheckBox check_Y;
		private System.Windows.Forms.CheckBox check_Z;
		private System.Windows.Forms.Button button_GO;
		private Timer m_Timer;
		private double m_AngChng = 0.01;
		private bool m_X = false;
		private bool m_Y = true;
		private System.Windows.Forms.GroupBox group;
		private System.Windows.Forms.NumericUpDown BoxAngleChange;
		private System.Windows.Forms.RadioButton radio_Geo;
		private System.Windows.Forms.RadioButton radio_EyeBall;
		private System.Windows.Forms.RadioButton radio_Random;
		private System.Windows.Forms.CheckBox m_StopOnChange;
		private bool m_Z = false;

		public PS_Rotation()
		{
			Init();
		}

		public PS_Rotation( ParticleSystemDrawWrapper drawWrapper ) // setup in static mode
		{
			Init();

			// NOTE, this is the "EXTERNAL" version of setting the m_DrawWrapper to allow proper event registering
			PSDrawWrapper = drawWrapper; // will set up the buttons too, and also take care of event subscription
		}

		private void Init()
		{
			Text = "PS Rotation";
			InitializeComponent();

			m_RotMat = new MatrixRotation();
			setupMatrix();

			m_Timer = new Timer();
			m_Timer.Interval = 50; // 20 FPS, sounds ok to me
			m_Timer.Tick += new EventHandler(m_Timer_Tick);

			setupButtons();
		}

		public void AttachToDocument( Document doc )
		{
			// get the wrapper

			if( m_StopOnChange.Checked && m_Timer.Enabled )
			{
				this.button_GO_Click(null,null); // stop all rotation
			}
			
			m_DrawWrapper = null;
			m_Perspective = null;
			if ( doc != null )
			{
				for( int i = 0; i < doc.MemberCount; i++ )
				{
					if ( doc[i].GetType() == typeof(ParticleSystemDrawWrapper) )
					{
						PSDrawWrapper = (ParticleSystemDrawWrapper) doc[i];
						// Important to use the "external" version
					
						break; // only one drawWrapper should ever be present, so dont check any more tools
					}		
				}
			}

			setupButtons();
		}
		
		private void setupButtons()
		{
			if( m_DrawWrapper == null )
			{
				for( int i = 0; i < Controls.Count; i++ )
				{
					Controls[i].Enabled = false;
				}
			}
			else
			{
				for( int i = 0; i < Controls.Count; i++ )
				{
					Controls[i].Enabled = true;
				}
			}
		}


		public ParticleSystemDrawWrapper PSDrawWrapper
		{
			get
			{
				return m_DrawWrapper;
			}
			set
			{
				if( value != null )
				{
					m_DrawWrapper = value;
					m_Perspective = m_DrawWrapper.perspective;
				}
				else
				{
					m_DrawWrapper = null;
					m_Perspective = null;
				}
				setupButtons();
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.group = new System.Windows.Forms.GroupBox();
			this.BoxAngleChange = new System.Windows.Forms.NumericUpDown();
			this.button_GO = new System.Windows.Forms.Button();
			this.check_Z = new System.Windows.Forms.CheckBox();
			this.check_Y = new System.Windows.Forms.CheckBox();
			this.check_X = new System.Windows.Forms.CheckBox();
			this.radio_Geo = new System.Windows.Forms.RadioButton();
			this.radio_EyeBall = new System.Windows.Forms.RadioButton();
			this.radio_Random = new System.Windows.Forms.RadioButton();
			this.m_StopOnChange = new System.Windows.Forms.CheckBox();
			this.group.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.BoxAngleChange)).BeginInit();
			this.SuspendLayout();
			// 
			// group
			// 
			this.group.Controls.Add(this.m_StopOnChange);
			this.group.Controls.Add(this.radio_Random);
			this.group.Controls.Add(this.radio_EyeBall);
			this.group.Controls.Add(this.radio_Geo);
			this.group.Controls.Add(this.BoxAngleChange);
			this.group.Controls.Add(this.button_GO);
			this.group.Controls.Add(this.check_Z);
			this.group.Controls.Add(this.check_Y);
			this.group.Controls.Add(this.check_X);
			this.group.Location = new System.Drawing.Point(8, 8);
			this.group.Name = "group";
			this.group.Size = new System.Drawing.Size(168, 144);
			this.group.TabIndex = 0;
			this.group.TabStop = false;
			this.group.Text = "Params";
			// 
			// BoxAngleChange
			// 
			this.BoxAngleChange.Location = new System.Drawing.Point(8, 88);
			this.BoxAngleChange.Maximum = new System.Decimal(new int[] {
																		   1000,
																		   0,
																		   0,
																		   0});
			this.BoxAngleChange.Minimum = new System.Decimal(new int[] {
																		   1,
																		   0,
																		   0,
																		   0});
			this.BoxAngleChange.Name = "BoxAngleChange";
			this.BoxAngleChange.Size = new System.Drawing.Size(64, 20);
			this.BoxAngleChange.TabIndex = 5;
			this.BoxAngleChange.Value = new System.Decimal(new int[] {
																		 10,
																		 0,
																		 0,
																		 0});
			this.BoxAngleChange.ValueChanged += new System.EventHandler(this.BoxAngleChange_ValueChanged);
			// 
			// button_GO
			// 
			this.button_GO.Location = new System.Drawing.Point(80, 88);
			this.button_GO.Name = "button_GO";
			this.button_GO.TabIndex = 3;
			this.button_GO.Text = "Start";
			this.button_GO.Click += new System.EventHandler(this.button_GO_Click);
			// 
			// check_Z
			// 
			this.check_Z.Location = new System.Drawing.Point(112, 64);
			this.check_Z.Name = "check_Z";
			this.check_Z.Size = new System.Drawing.Size(48, 24);
			this.check_Z.TabIndex = 2;
			this.check_Z.Text = "Z";
			this.check_Z.CheckedChanged += new System.EventHandler(this.checkChange);
			// 
			// check_Y
			// 
			this.check_Y.Checked = true;
			this.check_Y.CheckState = System.Windows.Forms.CheckState.Checked;
			this.check_Y.Location = new System.Drawing.Point(112, 40);
			this.check_Y.Name = "check_Y";
			this.check_Y.Size = new System.Drawing.Size(48, 24);
			this.check_Y.TabIndex = 1;
			this.check_Y.Text = "Y";
			this.check_Y.CheckedChanged += new System.EventHandler(this.checkChange);
			// 
			// check_X
			// 
			this.check_X.Location = new System.Drawing.Point(112, 16);
			this.check_X.Name = "check_X";
			this.check_X.Size = new System.Drawing.Size(48, 24);
			this.check_X.TabIndex = 0;
			this.check_X.Text = "X";
			this.check_X.CheckedChanged += new System.EventHandler(this.checkChange);
			// 
			// radio_Geo
			// 
			this.radio_Geo.Checked = true;
			this.radio_Geo.Location = new System.Drawing.Point(8, 16);
			this.radio_Geo.Name = "radio_Geo";
			this.radio_Geo.TabIndex = 6;
			this.radio_Geo.TabStop = true;
			this.radio_Geo.Text = "Geometric";
			this.radio_Geo.CheckedChanged += new System.EventHandler(this.checkChange);
			// 
			// radio_EyeBall
			// 
			this.radio_EyeBall.Location = new System.Drawing.Point(8, 40);
			this.radio_EyeBall.Name = "radio_EyeBall";
			this.radio_EyeBall.TabIndex = 7;
			this.radio_EyeBall.Text = "Eye-Ball";
			this.radio_EyeBall.CheckedChanged += new System.EventHandler(this.checkChange);
			// 
			// radio_Random
			// 
			this.radio_Random.Location = new System.Drawing.Point(8, 64);
			this.radio_Random.Name = "radio_Random";
			this.radio_Random.TabIndex = 8;
			this.radio_Random.Text = "Random";
			this.radio_Random.CheckedChanged += new System.EventHandler(this.checkChange);
			// 
			// m_StopOnChange
			// 
			this.m_StopOnChange.Location = new System.Drawing.Point(8, 112);
			this.m_StopOnChange.Name = "m_StopOnChange";
			this.m_StopOnChange.Size = new System.Drawing.Size(144, 24);
			this.m_StopOnChange.TabIndex = 9;
			this.m_StopOnChange.Text = "Stop on Doc Change";
			// 
			// PS_Rotation
			// 
			this.Controls.Add(this.group);
			this.Name = "PS_Rotation";
			this.Size = new System.Drawing.Size(184, 160);
			this.group.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.BoxAngleChange)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		int randChangeCount = 0;
		private void m_Timer_Tick(object sender, EventArgs e)
		{
			if( m_Perspective != null )
			{
				if( radio_Random.Checked )
				{
					if( randChangeCount > 10 )
					{
						// randomise the angle
						m_X = (m_Rand.NextDouble() < 0.5);
						m_Y = (m_Rand.NextDouble() < 0.5);
						m_Z = (m_Rand.NextDouble() < 0.5);
						setupMatrix();
						randChangeCount = 0;
					}
				}
				else if( radio_EyeBall.Checked )
				{
					if( goingup )
					{
						m_AngChng += eyeballFactor;
						if( m_AngChng > 0.03 )
						{
							goingup = false;
							XvY = !XvY;
						}
					}
					else
					{
						m_AngChng -= eyeballFactor;
						if( m_AngChng < -0.03 )
						{
							goingup = true;
						}
					}
					setupMatrix();
				}
				m_Perspective.RotateTick( m_RotMat );
				m_DrawWrapper.TriggerViewerRefresh();
				randChangeCount++;
			}
		}
		double eyeballFactor = 0.004;
		bool goingup = true;
		bool XvY = false;

		private void setState()
		{
			if( m_Timer.Enabled )
			{
				button_GO.Text = "Stop";
			}
			else
			{
				button_GO.Text = "Start";
			}
		}

		private void button_GO_Click(object sender, System.EventArgs e)
		{
			m_Timer.Enabled = !m_Timer.Enabled;
			setState();		
		}

        private Random m_Rand = new Random();
		private void setupMatrix()
		{
			if( radio_Random.Checked )
			{
				m_RotMat.setToAxisRot( new double[]
					{
						m_X ? 1 : 0,
						m_Y ? 1 : 0,
						m_Z ? 1 : 0 
					},
					(m_Rand.NextDouble()-0.5) * (m_AngChng * 2.0) // -0.5 -> both + and - numbers
					);
			}
			else if( radio_EyeBall.Checked )
			{
				m_RotMat.setToAxisRot( new double[]
					{
						XvY ? 1 : 0,
						XvY ? 0 : 1,
						0 
					},
					m_AngChng
					);
			}
			else // its the geo option
			{
				m_RotMat.setToAxisRot( new double[]
					{
						m_X ? 1 : 0,
						m_Y ? 1 : 0,
						m_Z ? 1 : 0 
					},
					m_AngChng
					);
			}
		}

		private void checkChange(object sender, System.EventArgs e)
		{	
			check_X.Enabled = radio_Geo.Checked;
			check_Y.Enabled = radio_Geo.Checked;
			check_Z.Enabled = radio_Geo.Checked;

			if( radio_Geo.Checked ) // reset to current
			{
				m_X = check_X.Checked;
				m_Y = check_Y.Checked;
				m_Z = check_Z.Checked;
			} else if( radio_EyeBall.Checked )
			{
				m_AngChng = 0.0;
			}

			setupMatrix();
		}

		private void BoxAngleChange_ValueChanged(object sender, System.EventArgs e)
		{
			m_AngChng = ((double)BoxAngleChange.Value / 1000.0 );
			setupMatrix();
		}
	}
}
