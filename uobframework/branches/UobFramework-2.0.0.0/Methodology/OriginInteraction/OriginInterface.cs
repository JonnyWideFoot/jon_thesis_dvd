using System;

namespace UoB.Methodology.OriginInteraction
{
	/// <summary>
	/// Summary description for OriginCommand.
	/// </summary>
	public class OriginInterface
	{
		private Origin.ApplicationSI oApp = null;

		public OriginInterface( bool show )
		{
			oApp = new Origin.ApplicationSI();

			if( show )
			{
				oApp.Execute("doc -mc 1",null);
			}
		}

		public void LoadTemplateFile( string filename )
		{
			oApp.Load( filename );
		}

//		private void MakeScatter()
//		{
//			//string returnName = oApp.CreatePage((int)PageTypes.Graph,"Scatter","SCATTER",2);
//			//GraphType.IDM_PLOT_SCATTER
//			//layer.plotxy(Xdataset, Ydataset[, PlotType])
//			//oApp.Execute("layer -i " + "PhiPsi_b",null);
//		}

		public void SetDataPointPlotSize( string graphName, string dataName, int size )
		{
			//set PhiPsi_b -z 2;
			ActivateWindow( graphName );
			oApp.Execute( "set " + dataName.ToString() + " -z " + size.ToString(), null );
		}

		public void ChangeLabel( string PageName, string LabelName, string LabelText )
		{
			oApp.Execute( "window -b " + PageName, null); // activate the page
			//oApp.Execute( LabelName + ".text$ = " + LabelText, null ); // rename the label
			oApp.Execute( "label -n " + LabelName + " " + LabelText, null ); // rename the label
		}

		public void ActivateWindow( string windowName )
		{
			oApp.Execute( "window -a " + windowName, null );
		}

		// HMMMMM, not sure about these working properly
//		public void LoadColorMap( string graphName, string fileName )
//		{
//			ActivateWindow( graphName );
//			oApp.Execute( "layer -cm L " + fileName, null );
//		}
//
//		public void SaveColorMap( string graphName, string fileName )
//		{
//			ActivateWindow( graphName );
//			oApp.Execute( "layer -cm S " + fileName, null );
//		}

		public void RescaleWithoutScalingZ( string graphName )
		{
			ActivateWindow( graphName );
			oApp.Execute( "layer -az;", null );
		}

		public void RunContourRangeScript( string graphName, double maxValue )
		{
			ActivateWindow( graphName );
			oApp.Execute( "layer -a;", null );
			oApp.Execute( "SetGradientRange(" + graphName + "," + maxValue.ToString() + ")", null );
		}

		public void SetZBound( string graphName, float fromZ, float toZ )
		{
			ActivateWindow( graphName );		
			oApp.Execute( "z1 = " + fromZ, null );
			oApp.Execute( "z2 = " + toZ, null );

			// for some odd reason, this command changes the Y axis ... :-s
			//oApp.Execute( graphName + "!Layer0.Z.from = " + fromZ.ToString(), null );
			//oApp.Execute( graphName + "!Layer0.Z.to = "   + toZ.ToString(),   null  );
		}

		public void SetBounds( string graphName, float fromX, float toX, float fromY, float toY )
		{	
			oApp.Execute( graphName + "!Layer0.X.from = " + fromX.ToString(), null );
			oApp.Execute( graphName + "!Layer0.X.to = " + toX.ToString(), null  );
			oApp.Execute( graphName + "!Layer0.Y.from = " + fromY.ToString(), null  );
			oApp.Execute( graphName + "!Layer0.Y.to = " + toY.ToString(), null  );
		}

//		public void CreatePage()
//		{
//			//string returnName = oApp.CreatePage((int)PageType.Worksheet,"PhiPsi","Origin",2);
//		}

		public void Reset( bool worksheetTotal, bool matrixTotal )
		{
			oApp.Reset(worksheetTotal,matrixTotal);
		}

		public void SavePicture( string fileName, string pageName, int width, int height )
		{
			ActivateWindow( pageName );
			oApp.Execute( "Image.FileName$ = " + fileName, null );
			oApp.Execute( "Image.showOptions = false", null ); // dont want the dialog box to pop-up
			// 24 = bitrate
			// 10 = compression rate
			oApp.Execute( "Image.Export.PagePixel( JPG, " + width.ToString() + ", " + height.ToString() + ", 24, 10 )", null );
		}

		public void UpdateWorksheet( string LookupName, int columnID, double[] Data )
		{
			AssertExistence( LookupName, PageTypes.Worksheet );
			oApp.PutWorksheet( LookupName, Data, 0, columnID );
		}

		public void UpdateWorksheet( string LookupName, double[] xData, double[] yData )
		{
			AssertExistence( LookupName, PageTypes.Worksheet );
			oApp.PutWorksheet(LookupName, xData, 0, 0);
			oApp.PutWorksheet(LookupName, yData, 0, 1);
		}

		public void UpdateMatrix( string LookupName, int[,] matrix )
		{
			AssertExistence( LookupName, PageTypes.Matrix );
			oApp.PutMatrix( LookupName, matrix );
		}

		private void AssertExistence( string LookupName, PageTypes type )
		{
			//Check if specified page exists as the given type
			string str = "d=exist(" + LookupName + ")";
			double d = -1.0f;
			if( oApp.Execute(str,null) )
			{
				d = oApp.get_LTVar("d");
			}
			if( d != (double)type )
			{
				throw new Exception("Worksheet of name : " + LookupName + " and of type " + type.ToString() + " does not exist" );
			}
		}

	}
}
