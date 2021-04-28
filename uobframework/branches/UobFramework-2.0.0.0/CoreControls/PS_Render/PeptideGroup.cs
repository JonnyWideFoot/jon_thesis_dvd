using System;

using UoB.Core.Primitives;
using UoB.CoreControls.OpenGLView;

namespace UoB.CoreControls.PS_Render
{
	/// <summary>
	/// Summary description for PeptideGroup.
	/// </summary>
	public class peptidegroup
	{
		public int CA1,N,H,C,O,CA2;
		public Vector vca1; // c alpha 1
		public Vector vca2; // c alpha 2
		public Vector center; // the centre point between the two c alphas
		public Vector caca; // vector from calpha 1 to calpha 2
		public Vector up; // unit vector upwards from the centre point - the normal to the plane of the peptide grouop
		public Vector forward; // unit vector of the caca
		public Vector across; // unit vector in the "across direction" perpendicular to the peptide group
		public double length; // distance from calpha1 to calpha2

		public peptidegroup()
		{
			CA1=-1;
			N=-1;
			H=-1;
			C=-1;
			O=-1;
			CA2=-1;
		}
	};
}
