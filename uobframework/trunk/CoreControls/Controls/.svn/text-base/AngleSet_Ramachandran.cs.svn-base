using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using UoB.Core.MoveSets.AngleSets;

using NPlot;
using NPlot.Windows;

namespace UoB.CoreControls.Controls
{
	/// <summary>
	/// Summary description for AngleSet_Ramachandran.
	/// </summary>
	public class AngleSet_Ramachandran : Ramachandran
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private int m_ColourKey = 0;
		private AngleSet m_AngleSet = null;
		private System.Windows.Forms.ContextMenu contextMenu;
		private RangedPointPlot[] m_RangedPlots = null;

		public AngleSet_Ramachandran() : base()
		{
			InitializeComponent();
			CommonInit();
			angleSet = null;
		}

		public AngleSet_Ramachandran( AngleSet angSet ) : base()
		{
			InitializeComponent();
			CommonInit();
			angleSet = angSet;
		}

		private void CommonInit()
		{
			MenuItem selAll = new MenuItem( "Select &All", new EventHandler(selAllClick) );
			contextMenu.MenuItems.Add( selAll );
			MenuItem selNone = new MenuItem( "Select &None", new EventHandler(selNoneClick) );
			contextMenu.MenuItems.Add( selNone );
		}

		public AngleSet angleSet
		{
			get
			{
				return m_AngleSet;
			}
			set
			{
				m_AngleSet = value;
				GetAngles();
			}
		}

		private void selAllClick( object sender, System.EventArgs e )
		{
			SelectAll();
			plotSurface.Refresh();
		}

		private void SelectAll()
		{
			if( m_AngleSet != null )
			{
				for( int i = 2; i < contextMenu.MenuItems.Count; i++ )
				{
					MenuItem m = contextMenu.MenuItems[i];
					if( !m.Checked )
					{
						m.Checked = true;
						plotSurface.Add( m_RangedPlots[ m.Index - 2] );
					}	
				}
			}
		}

		private void selNoneClick( object sender, System.EventArgs e )
		{
			DeSelectAll();
			plotSurface.Refresh();
		}

		private void DeSelectAll()
		{
			if( m_AngleSet != null )
			{
				for( int i = 2; i < contextMenu.MenuItems.Count; i++ )
				{
					MenuItem m = contextMenu.MenuItems[i];
					if( m.Checked )
					{
						m.Checked = false;
						plotSurface.Remove( m_RangedPlots[ m.Index - 2 ], false );
					}	
				}
			}
		}

		private void GetAngles()
		{
			if( m_AngleSet != null )
			{
				int requiredCount = m_AngleSet.TotalAnglesInAllResidueAnglesets;
				if( m_PhiAngles.Length < requiredCount )
				{
					m_PhiAngles = new double[ requiredCount ];
					m_PsiAngles = new double[ requiredCount ];
				}
			}
			GeneratePlotRanges();
			GenerateMenu();
			SelectAll();
			plotSurface.Refresh();
		}

		private void plotSurface_DoubleClick(object sender, System.EventArgs e)
		{
			if( m_AngleSet != null )
			{
				int currentSelection = 2;
				for( int i = 2; i < contextMenu.MenuItems.Count; i++ )
				{
					MenuItem m = contextMenu.MenuItems[i];
					if( m.Checked )
					{
						currentSelection = i + 1;
						if( currentSelection == contextMenu.MenuItems.Count )
						{
							currentSelection = 2;
						}
						break;
					}
				}	
				DeSelectAll();
				contextMenu.MenuItems[currentSelection].PerformClick();
				plotSurface.Refresh();		
			}
		}

		private void GenerateMenu()
		{
			for( int i = contextMenu.MenuItems.Count - 1; i >= 2; i-- )
			{
				contextMenu.MenuItems.RemoveAt(i);
			}
			if( m_AngleSet != null )
			{
				for( int i = 0; i < m_RangedPlots.Length; i++ )
				{
					ResidueAngleSet theASet = m_AngleSet[ i ];
					MenuItem menuItem = new MenuItem( String.Concat( theASet.Name, " (", theASet.ID.ToString() + ")" ), new EventHandler(menuItemClicked) );
					menuItem.Select += new EventHandler(menuItem_Select);
					contextMenu.MenuItems.Add( menuItem );
				}
			}
			else
			{
				contextMenu.MenuItems.Add("No Items");
			}
		}

		private void menuItemClicked( object sender, System.EventArgs e )
		{
			MenuItem m = (MenuItem) sender;
			if( m.Checked )
			{
				m.Checked = false;
				plotSurface.Remove( m_RangedPlots[m.Index - 2], false );
			}
			else
			{
				m.Checked = true;
				plotSurface.Add( m_RangedPlots[m.Index - 2] );
			}		
			plotSurface.Refresh();
		}

		private Color NextColor()
		{
			m_ColourKey++;
			if( 46 == m_ColourKey || // very light colours
				48 == m_ColourKey ||
				42 == m_ColourKey ||
				50 == m_ColourKey ) 
			{
				m_ColourKey++;
			}
			return Color.FromKnownColor( (KnownColor)m_ColourKey );
		}

		private void menuItem_Select(object sender, EventArgs e)
		{
			MenuItem mi = (MenuItem) sender;
			int plotID = mi.Index - 2;
			for( int i = 0; i < m_RangedPlots.Length; i++ )
			{
				//m_RangedPlots[i].Marker.Size = 8;
				m_RangedPlots[i].Marker.Type = Marker.MarkerType.Cross1;
			}
			//m_RangedPlots[plotID].Marker.Size = 15;
			m_RangedPlots[plotID].Marker.Type = Marker.MarkerType.FilledCircle;
			plotSurface.Refresh();
		}

		private void GeneratePlotRanges()
		{
			m_ColourKey = 36;
			if( m_RangedPlots != null )
			{
				for( int i = 0; i < m_RangedPlots.Length; i++ )
				{
					plotSurface.Remove( m_RangedPlots[i], false );
				}
			}
			m_RangedPlots = null;

			if( m_AngleSet != null )
			{
				int mainArrayCounted = 0;
				m_RangedPlots = new RangedPointPlot[m_AngleSet.ResidueAnglesets];

				for( int i = 0; i < m_RangedPlots.Length; i++ )
				{
					Marker m = new Marker( Marker.MarkerType.Cross1, 8, NextColor() );
					m_RangedPlots[i] = new RangedPointPlot(m );
					m_RangedPlots[i].OrdinateData = m_PsiAngles;
					m_RangedPlots[i].AbscissaData = m_PhiAngles;

					ResidueAngleSet theASet = m_AngleSet[ i ];
					m_RangedPlots[i].SetRange( mainArrayCounted, mainArrayCounted + theASet.AngleCount - 1 );
					for( int j = 0; j < theASet.AngleCount; j++ )
					{
						m_PhiAngles[mainArrayCounted] = theASet.getPhi(j);
						m_PsiAngles[mainArrayCounted] = theASet.getPsi(j);
						mainArrayCounted++;
					}
				}
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
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.SuspendLayout();
			// 
			// plotSurface
			// 
			this.plotSurface.ContextMenu = this.contextMenu;
			this.plotSurface.Name = "plotSurface";
			this.plotSurface.Size = new System.Drawing.Size(448, 416);
			this.plotSurface.DoubleClick += new System.EventHandler(this.plotSurface_DoubleClick);
			// 
			// AngleSet_Ramachandran
			// 
			this.Name = "AngleSet_Ramachandran";
			this.Size = new System.Drawing.Size(448, 416);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
