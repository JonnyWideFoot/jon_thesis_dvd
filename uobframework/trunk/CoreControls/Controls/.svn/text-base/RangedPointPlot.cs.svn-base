using System;
using System.Drawing;

using NPlot;
using NPlot.Windows;

namespace UoB.CoreControls.Controls
{
	/// <summary>
	/// Special extension to allow plotting a limited range of point data via the
	/// SetRange() method call
	/// </summary>
	public class RangedPointPlot : PointPlot
	{
		private int m_StartIndex = 0;
		private int m_EndIndex  = 0;

		public RangedPointPlot() : base()
		{
		}

		public RangedPointPlot( Marker marker ) : base( marker )
		{
		}

		public void SetRange( int startIndex, int endIndex )
		{
			m_StartIndex = startIndex;
			m_EndIndex = endIndex;
		}

		public override void Draw(System.Drawing.Graphics g, PhysicalAxis xAxis, PhysicalAxis yAxis)
		{
			SequenceAdapter data_ = 
				new SequenceAdapter( this.DataSource, this.DataMember, this.OrdinateData, this.AbscissaData );

			for (int i = m_StartIndex; i <= m_EndIndex; ++i)
			{
				if ( !Double.IsNaN(data_[i].X) && !Double.IsNaN(data_[i].Y) )
				{
					PointF xPos = xAxis.WorldToPhysical( data_[i].X, false);
					PointF yPos = yAxis.WorldToPhysical( data_[i].Y, false);
					Marker.Draw( g, (int)xPos.X, (int)yPos.Y );
					if (Marker.DropLine)
					{
						PointD yMin = new PointD( data_[i].X, Math.Max( 0.0f, yAxis.Axis.WorldMin ) );
						PointF yStart = yAxis.WorldToPhysical( yMin.Y, false );
						g.DrawLine( Marker.Pen, new Point((int)xPos.X,(int)yStart.Y), new Point((int)xPos.X,(int)yPos.Y) );
					}
				}
			}
		}
	}
}
