using System;
using System.IO;
using System.Text;

namespace UoB.Methodology.OriginInteraction
{
    public delegate string SymbolDelegate(char s);

    public class OriginFormatting
    {
        private OriginFormatting()
        {
        }

        private static string SymbolToHTML(char c)
        {
            switch (c)
            {
                case 'A':
                    return "&Alpha;";
                case 'F':
                    return "&Phi;";
                case 'Y':
                    return "&Psi;";
                case 'W':
                    return "&Omega;";

                case 'a':
                    return "&alpha;";
                case 'f':
                    return "&phi;";
                case 'y':
                    return "&psi;";
                case 'w':
                    return "&omega;";

                default:
                    throw new Exception("Unknown symbol");
            }
        }

        private static string SymbolToLatex(char c)
        {
            switch (c)
            {
                case 'a':
                    return "\\al";
                case 'F':
                    return "\\phi";
                case 'Y':
                    return "\\psi";
                case 'W':
                    return "\\omg";
                default:
                    throw new Exception("Unknown symbol");
            }
        }

        public static string OriginSymbolToFormat(string s, SymbolDelegate formatProvider)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            bool symbolMode = false;
            while (i < s.Length)
            {
                if (symbolMode)
                {
                    if (s[i] == ')')
                    {
                        symbolMode = false;
                    }
                    else if (Char.IsLetter(s[i]))
                    {
                        sb.Append(formatProvider(s[i]));
                    }
                    else
                    {
                        sb.Append(s[i]);
                    }
                }
                else
                {
                    if (i < s.Length - 4 && s[i] == '\\' && s[i + 1] == 'g' && s[i + 2] == '(')
                    {
                        symbolMode = true;
                        i += 2;
                    }
                    else
                    {
                        sb.Append(s[i]);
                    }
                }
                i++;
            }
            return sb.ToString();
        }

        public static string OriginSymbolToHTML(string s)
        {
            return OriginSymbolToFormat(s, new SymbolDelegate(SymbolToHTML));
        }

        public static string OriginSymbolToLatex(string s)
        {
            return OriginSymbolToFormat(s, new SymbolDelegate(SymbolToLatex));
        }
    }

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

        public void Save(string filename)
        {
            oApp.Execute("save -di " + filename, null); 
        }

        private string TempFileSave()
        {
            // you have to 'save' the file to stop origin asking you if you want to... 
            // if we save to a temp file, we can then delete it once termination has occured...
            string tempName = Path.GetTempFileName();
            oApp.Execute("save -di " + tempName, null);
            return tempName;
        }

        private void TempFileRemove( string tempName)
        {
            if (File.Exists(tempName))
            {
                try
                {
                    File.Delete(tempName);
                }
                catch
                {
                }
            }
        }

        public void Terminate()
        {
            string tempName = TempFileSave();
            oApp.Execute("exit", null);
            TempFileRemove(tempName);
        }

		public void LoadTemplateFile( string filename )
		{
            string tempName = TempFileSave();
            oApp.Load(filename);
            TempFileRemove(tempName);			
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

        public void ChangeYAxisLabel(string pageName, string label)
        {
            ActivateWindow(pageName);
            oApp.Execute("label -yb " + label, null);
        }

        public void ChangeXAxisLabel(string pageName, string label)
        {
            ActivateWindow( pageName );
            oApp.Execute("label -xb " + label, null);
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

        public void SetYAxisIncrement(string graphName, double inc)
        {
            oApp.Execute(graphName + "!Layer0.Y.inc = " + inc.ToString(), null);
        }

        public void SetXAxisIncrement(string graphName, double inc)
        {
            oApp.Execute(graphName + "!Layer0.X.inc = " + inc.ToString(), null);
        }

        public void SetHistogramBin(string graphName, string dataName, double binStart, double binSize, double binEnd )
        {
            // Data name is the name of the data that is being plotted in the active graph e.g. data_a
            ActivateWindow(graphName);
            oApp.Execute(String.Format("set {0} -hbb {1};", dataName, binStart), null);
            oApp.Execute(String.Format("set {0} -hbe {1};", dataName, binEnd), null);
            oApp.Execute(String.Format("set {0} -hbs {1};", dataName, binSize), null);
        }

        public void RescaleAuto(string graphName)
        {
            ActivateWindow(graphName);
            oApp.Execute("layer -s 1;", null); // select layer 1
            oApp.Execute("rescale;", null); // call rescale
        }

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
            SetBoundY(graphName, fromY, toY);
            SetBoundX(graphName, fromX, toX);
        }

        public void SetBoundY(string graphName, float fromY, float toY)
        {
            oApp.Execute(graphName + "!Layer0.Y.from = " + fromY.ToString(), null);
            oApp.Execute(graphName + "!Layer0.Y.to = " + toY.ToString(), null);
        }

        public void SetBoundX( string graphName, float fromX, float toX )
        {
            oApp.Execute(graphName + "!Layer0.X.from = " + fromX.ToString(), null);
            oApp.Execute(graphName + "!Layer0.X.to = " + toX.ToString(), null);
		}

//		public void CreatePage()
//		{
//			//string returnName = oApp.CreatePage((int)PageType.Worksheet,"PhiPsi","Origin",2);
//		}

		public void Reset( bool worksheetTotal, bool matrixTotal )
		{
			oApp.Reset(worksheetTotal,matrixTotal);
		}

        public void SaveEPSPicture(string fileName, string pageName)
        {
            ActivateWindow(pageName);
            oApp.Execute("Image.FileName$ = " + fileName, null);
            oApp.Execute("Image.showOptions = true", null); // dont want the dialog box to pop-up
            string command = "Image.Export.PageDPI( EPS, 96, 24, 0 )";
            oApp.Execute(command, null);
        }

        public void SavePicture(string fileStem, string pageName, int width, int height)
		{
            SavePicture(ImageType.JPG, fileStem, pageName, width, height);
        }

        public void SavePicture(ImageType type, string fileStem, string pageName, int width, int height)
		{
            int compression = 0;
            switch (type)
            {
                case ImageType.JPG:
                    compression = 10;
                    break;
                case ImageType.GIF:
                    compression = 1;
                    break;
                case ImageType.TIF:
                    compression = 1;
                    break;
                case ImageType.BMP:
                    compression = 0;
                    break;
                case ImageType.PNG:
                    compression = 1;
                    break;
                default:
                    break;
            }		

            // Autodetect file extension
            string ext = type.ToString().ToLower();
            if( 0 != String.Compare( System.IO.Path.GetExtension(fileStem), ext, true ) )
            {
                fileStem += '.';
                fileStem += ext;
            }

            ActivateWindow(pageName);
            oApp.Execute("Image.FileName$ = " + fileStem, null);
			oApp.Execute( "Image.showOptions = false", null ); // dont want the dialog box to pop-up
			// 24 = bitrate
			// 10 = compression rate
            //oApp.Execute("Image.Export.PagePixel( " + type.ToString() + ", " + width.ToString() + ", " + height.ToString() + ", 24, 10 )", null);
            string command = String.Concat("Image.Export.PagePixel( ", type, ", ", width, ", ", height, ", 24, ", compression, " )");
            oApp.Execute(command, null);
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
