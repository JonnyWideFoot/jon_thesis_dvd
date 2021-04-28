using System;
using System.Diagnostics;

using System.Collections;
using UoB.Core.Primitives.Collections;

namespace UoB.Core.ForceField.Definitions
{
	/// <summary>
	/// Summary description for TorsionDefinition.
	/// </summary>
	public class TorsionDefinition
	{
		public string AtomTypeA;
		public string AtomTypeB;
		public string AtomTypeC;
		public string AtomTypeD;
		public bool isImproperTorstion;

		private FloatArrayList Gamma;
		private FloatArrayList Vn; // the individual parameters for each term
		private int fourierTerms;// how many fourier terms
		private FloatArrayList n;

		public TorsionDefinition( string A, string B, string C, string D )
		{
			isImproperTorstion = false;
			AtomTypeA = A;
			AtomTypeB = B;
			AtomTypeC = C;
			AtomTypeD = D;

			fourierTerms = 0;
			Gamma = new FloatArrayList();
			Vn = new FloatArrayList();
			n = new FloatArrayList();
		}

		public int CountFourierTerms
		{
			get
			{
				return fourierTerms;
			}
		}

		public void addTerm(float newVn, float newN, float newGamma)
		{
			Vn.AddFloat(newVn);
			n.AddFloat(newN);
			Gamma.AddFloat(newGamma);
			fourierTerms++;
		}
	}
}
