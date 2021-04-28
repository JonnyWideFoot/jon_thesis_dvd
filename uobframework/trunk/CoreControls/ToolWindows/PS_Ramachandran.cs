using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

using UoB.CoreControls.Controls;
using UoB.Core.Structure;
using UoB.Core.Tools;
using UoB.Core;

using NPlot;
using NPlot.Windows;

namespace UoB.CoreControls.ToolWindows
{
	/// <summary>
	/// Summary description for Ramachandran.
	/// </summary>
	public class PS_Ramachandran : Ramachandran, ITool
	{
		private System.Windows.Forms.Panel panel_View;
		private UoB.CoreControls.Controls.PolymerSelectionAndRange polymerSelectionAndRange;
		private System.Windows.Forms.Button EnableRange;
		private System.Windows.Forms.CheckBox Check_Range;

		private RangedPointPlot m_PointPlot;
		private UpdateEvent m_UpdateEvent;
		private ParticleSystem m_ParticleSystem;
		private int m_CountTo = 0;
	
		public PS_Ramachandran(ParticleSystem ps) : base()
		{
			init();
			particleSystem = ps;
		}

		public PS_Ramachandran() : base()
		{
			init();
			particleSystem = null;
		}

		private void init()
		{
			InitializeComponent();
			m_UpdateEvent = new UoB.Core.UpdateEvent(GetData);
			m_CountTo = 1;

			Marker m = new Marker( Marker.MarkerType.Cross1, 2, Color.Green );
			m_PointPlot = new RangedPointPlot( m );
			m_PointPlot.OrdinateData = m_PsiAngles;
			m_PointPlot.AbscissaData = m_PhiAngles;
			m_PointPlot.SetRange( 0, 0 );
			m_PointPlot.Label = "DefaultPlot";
			plotSurface.Add(m_PointPlot); 
		}

		public ParticleSystem particleSystem
		{
			get
			{
				return m_ParticleSystem;				     
			}
			set
			{
				if ( m_ParticleSystem != null )
				{
					m_ParticleSystem.PositionsUpdate -= m_UpdateEvent;
				}
				m_ParticleSystem = null; 
				m_ParticleSystem = value;

				if ( m_ParticleSystem != null )
				{
					Name = "Ramachandran for : " + value.Name;
					Text = Name;
					m_ParticleSystem.PositionsUpdate += m_UpdateEvent;
				}
				else
				{
					Name = "Ramachandran for : No System Present";
					Text = Name;
				}	

				EnsureArraySize_ForPS();
				// psi/phi angle holders are set to the required size for holding all possible PS data
				
				polymerSelectionAndRange.particleSystem = m_ParticleSystem;
				if( !Check_Range.Checked )
				{
					GetData();	
				}

				SetControlState();
			}
		}

		private void SetControlState()
		{
			bool enable = m_ParticleSystem != null;
			for( int i = 0; i < Controls.Count; i++ )
			{
				Controls[i].Enabled = enable;
			}
		}

		private void GetData()
		{
			m_CountTo = 0;
			if ( m_ParticleSystem != null )
			{
				m_ParticleSystem.AcquireReaderLock(1000);

				if( Check_Range.Checked )
				{
					PolyPeptide pp = polymerSelectionAndRange.Polymer as PolyPeptide;
					if ( pp != null )
					{
						for ( int j = polymerSelectionAndRange.StartPolymerIndex; j <= polymerSelectionAndRange.EndPolymerIndex; j++ )
						{
							m_PhiAngles[m_CountTo] = pp[j].phiAngle;
							m_PsiAngles[m_CountTo] = pp[j].psiAngle;
							if( m_PhiAngles[m_CountTo] != float.NegativeInfinity &&
								m_PsiAngles[m_CountTo] != float.NegativeInfinity
								)
							{
								// the pair is valid
								m_CountTo++;
							}
						}
					}
				}
				else
				{
					for ( int i = 0; i < m_ParticleSystem.MemberCount; i++ )
					{
						PolyPeptide pp = m_ParticleSystem.MemberAt(i) as PolyPeptide;
						if ( pp != null )
						{
							for ( int j = 1; j < pp.Count -1; j++ )
							{
								m_PhiAngles[m_CountTo] = pp[j].phiAngle;
								m_PsiAngles[m_CountTo] = pp[j].psiAngle;
								if( m_PhiAngles[m_CountTo] != float.NegativeInfinity &&
									m_PsiAngles[m_CountTo] != float.NegativeInfinity
									)
								{
									// the pair is valid
									m_CountTo++;
								}
							}
						}
					}
				}

				m_ParticleSystem.ReleaseReaderLock();
			}
			
			m_PointPlot.SetRange( 0, m_CountTo - 1 );

			plotSurface.Refresh(); // show the data
		}

		private void EnsureArraySize( int size )
		{
			if( m_PhiAngles.Length < size )
			{
				m_PhiAngles = new double[ size ];
				m_PsiAngles = new double[ size ];
				m_PointPlot.OrdinateData = m_PsiAngles;
				m_PointPlot.AbscissaData = m_PhiAngles;
			}
		}

		private void InitializeComponent()
		{
			this.polymerSelectionAndRange = new UoB.CoreControls.Controls.PolymerSelectionAndRange();
			this.EnableRange = new System.Windows.Forms.Button();
			this.Check_Range = new System.Windows.Forms.CheckBox();
			this.panel_View = new System.Windows.Forms.Panel();
			this.panel_View.SuspendLayout();
			this.SuspendLayout();
			// 
			// plotSurface
			// 
			this.plotSurface.Name = "plotSurface";
			this.plotSurface.Size = new System.Drawing.Size(392, 400);
			// 
			// polymerSelectionAndRange
			// 
			this.polymerSelectionAndRange.Location = new System.Drawing.Point(8, 8);
			this.polymerSelectionAndRange.Name = "polymerSelectionAndRange";
			this.polymerSelectionAndRange.particleSystem = null;
			this.polymerSelectionAndRange.Size = new System.Drawing.Size(152, 96);
			this.polymerSelectionAndRange.TabIndex = 0;
			this.polymerSelectionAndRange.Visible = false;
			// 
			// EnableRange
			// 
			this.EnableRange.Location = new System.Drawing.Point(8, 8);
			this.EnableRange.Name = "EnableRange";
			this.EnableRange.Size = new System.Drawing.Size(48, 23);
			this.EnableRange.TabIndex = 1;
			this.EnableRange.Text = "X<->Y";
			this.EnableRange.Click += new System.EventHandler(this.EnableRange_Click);
			// 
			// Check_Range
			// 
			this.Check_Range.Location = new System.Drawing.Point(8, 32);
			this.Check_Range.Name = "Check_Range";
			this.Check_Range.Size = new System.Drawing.Size(48, 24);
			this.Check_Range.TabIndex = 2;
			this.Check_Range.Text = "---->";
			this.Check_Range.CheckedChanged += new System.EventHandler(this.Check_Range_CheckedChanged);
			// 
			// panel_View
			// 
			this.panel_View.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel_View.Controls.Add(this.polymerSelectionAndRange);
			this.panel_View.Location = new System.Drawing.Point(64, 8);
			this.panel_View.Name = "panel_View";
			this.panel_View.Size = new System.Drawing.Size(176, 112);
			this.panel_View.TabIndex = 3;
			this.panel_View.Visible = false;
			// 
			// PS_Ramachandran
			// 
			this.Controls.Add(this.Check_Range);
			this.Controls.Add(this.panel_View);
			this.Controls.Add(this.EnableRange);
			this.Name = "PS_Ramachandran";
			this.Size = new System.Drawing.Size(392, 400);
			this.Controls.SetChildIndex(this.plotSurface, 0);
			this.Controls.SetChildIndex(this.EnableRange, 0);
			this.Controls.SetChildIndex(this.panel_View, 0);
			this.Controls.SetChildIndex(this.Check_Range, 0);
			this.panel_View.ResumeLayout(false);
			this.ResumeLayout(false);

		}


		private void EnsureArraySize_ForPS()
		{
			m_CountTo = 0;
			try
			{
				if ( m_ParticleSystem != null )
				{
					m_ParticleSystem.AcquireReaderLock(1000);

					for ( int i = 0; i < m_ParticleSystem.MemberCount; i++ )
					{
						PolyPeptide pp = m_ParticleSystem.MemberAt(i) as PolyPeptide;
						if ( pp != null )
						{
							for ( int j = 1; j < pp.Count; j++ )
							{
								m_CountTo++;
							}
						}
					}
					m_ParticleSystem.ReleaseReaderLock();
				}
			}
			catch
			{
			}
			EnsureArraySize( m_CountTo );
		}

		#region ITool Members

		public void AttachToDocument(UoB.CoreControls.Documents.Document doc)
		{
			if ( doc != null )
			{
				for( int i = 0; i < doc.MemberCount; i++ )
				{
					ParticleSystem ps = doc[i] as ParticleSystem;
					if ( ps != null )
					{
						particleSystem = ps;
						return; // only one ParticleSystem should ever be present, so dont check any more tools
					}
				}
			}
			// none found ...
			particleSystem = null;			
		}

		#endregion

		private void Check_Range_CheckedChanged(object sender, System.EventArgs e)
		{
			if( Check_Range.Checked )
			{
				polymerSelectionAndRange.PolymerChanged += m_UpdateEvent;
				polymerSelectionAndRange.RangeChanged += m_UpdateEvent;
			}
			else
			{
				polymerSelectionAndRange.PolymerChanged -= m_UpdateEvent;
				polymerSelectionAndRange.RangeChanged -= m_UpdateEvent;
			}		
			GetData();
		}

		private void EnableRange_Click(object sender, System.EventArgs e)
		{
            panel_View.Visible = !panel_View.Visible;	
			polymerSelectionAndRange.Visible = panel_View.Visible;	
		}
	}
}