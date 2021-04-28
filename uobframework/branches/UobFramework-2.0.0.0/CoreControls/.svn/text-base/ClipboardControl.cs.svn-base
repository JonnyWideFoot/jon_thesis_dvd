using System;
using System.Text;
using System.Windows.Forms;

using UoB.Core.Data.IO;
using UoB.Core.Data.Graphing;

namespace UoB.CoreControls
{
	/// <summary>
	/// Summary description for ClipboardControl.
	/// </summary>
	public class ClipboardControl
	{
		public static void CopyData( string[] titles, params float[][] data )
		{
			Clipboard.SetDataObject(
				StringBuilderDataWriter.GetString( titles, '\t', data ) );
		}

		public static void CopyData( Graph g )
		{
			Clipboard.SetDataObject(
				StringBuilderDataWriter.GetString( new string[] { g.xAxisLabel, g.yAxisLabel }, '\t', g.XData, g.YData ) );
		}
	}
}
