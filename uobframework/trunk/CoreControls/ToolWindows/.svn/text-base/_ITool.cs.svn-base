using System;
using System.Windows.Forms;
using UoB.CoreControls.Documents;

namespace UoB.CoreControls.ToolWindows
{
	/// <summary>
	/// Summary description for ITool.
	/// </summary>
	public interface ITool
	{
		void AttachToDocument( Document doc );

		string Text
		{
			get;
		}

		System.Drawing.Size Size
		{
			get;
			set;
		}

		System.Drawing.Size ClientSize
		{
			get;
			set;
		}

		Control Parent
		{
			get;
			set;
		}

		DockStyle Dock
		{
			get;
			set;
		}

		void Show();
	}
}
