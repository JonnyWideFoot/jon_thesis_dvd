using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using UoB.DynamicUpdateEngine.Downloading;

namespace UoB.DynamicUpdateEngine
{
	/// <summary>
	/// Summary description for Main.
	/// </summary>
	public class DynamicUpdateExec
	{
		private DynamicUpdateExec()
		{
			// No initialise
		}

		[STAThread]
		static void Main(string[] args)
		{
			DynamicUpdater updater = new DynamicUpdater();
			updater.BeginApplication(); // the appLoop and any required user prompting ...
		}
	}
}
