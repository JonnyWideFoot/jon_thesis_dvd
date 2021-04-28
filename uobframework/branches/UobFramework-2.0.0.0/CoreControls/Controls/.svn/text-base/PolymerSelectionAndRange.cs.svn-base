using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using UoB.Core;
using UoB.Core.Primitives;
using UoB.Core.Structure;

namespace UoB.CoreControls.Controls
{
	/// <summary>
	/// Summary description for PolymerSelectionAndRange.
	/// </summary>
	public class PolymerSelectionAndRange : System.Windows.Forms.UserControl
	{
		private UoB.CoreControls.Controls.PolymerSelection m_PolymerSelection;
		private UoB.CoreControls.Controls.PolymerRange m_PolymerRange;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public event UpdateEvent PolymerChanged;
		public event UpdateEvent RangeChanged;
		private ParticleSystem m_PS = null;

		public PolymerSelectionAndRange()
		{
			InitializeComponent();
			CommonInit();

			particleSystem = null;
		}

		public PolymerSelectionAndRange( ParticleSystem ps )
		{
			InitializeComponent();
			CommonInit();

			// external accessor call
			particleSystem = ps;
		}

		private void CommonInit()
		{
			m_PolymerSelection.PolymerChanged += new UpdateEvent( PolymerChangedCall );
			m_PolymerRange.Range.RangeUpdated += new RangeChange( RangeChangeCall );
		}

		public PSMolContainer Polymer
		{
			get
			{
				return m_PolymerSelection.Polymer;
			}
		}

		public int StartPolymerIndex
		{
			get
			{
				return m_PolymerRange.StartID;
			}
		}

		public int EndPolymerIndex
		{
			get
			{
				return m_PolymerRange.EndID;
			}
		}

		public ParticleSystem particleSystem
		{
			get
			{
				return m_PS;
			}
			set
			{
				m_PS = value;
				m_PolymerSelection.particleSystem = m_PS;
			}
		}

		private void PolymerChangedCall()
		{
			m_PolymerRange.Polymer = m_PolymerSelection.Polymer;
			if( PolymerChanged != null )
			{
				PolymerChanged();
			}
		}

		private void RangeChangeCall( object sender, IntRange_EventFire range )
		{
			if( RangeChanged != null )
			{
				RangeChanged();
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.m_PolymerSelection = new UoB.CoreControls.Controls.PolymerSelection();
			this.m_PolymerRange = new UoB.CoreControls.Controls.PolymerRange();
			this.SuspendLayout();
			// 
			// m_PolymerSelection
			// 
			this.m_PolymerSelection.Location = new System.Drawing.Point(0, 0);
			this.m_PolymerSelection.Name = "m_PolymerSelection";
			this.m_PolymerSelection.particleSystem = null;
			this.m_PolymerSelection.Size = new System.Drawing.Size(152, 24);
			this.m_PolymerSelection.TabIndex = 0;
			// 
			// m_PolymerRange
			// 
			this.m_PolymerRange.Location = new System.Drawing.Point(0, 24);
			this.m_PolymerRange.Name = "m_PolymerRange";
			this.m_PolymerRange.Polymer = null;
			this.m_PolymerRange.Size = new System.Drawing.Size(152, 72);
			this.m_PolymerRange.TabIndex = 1;
			// 
			// PolymerSelectionAndRange
			// 
			this.Controls.Add(this.m_PolymerRange);
			this.Controls.Add(this.m_PolymerSelection);
			this.Name = "PolymerSelectionAndRange";
			this.Size = new System.Drawing.Size(152, 96);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
