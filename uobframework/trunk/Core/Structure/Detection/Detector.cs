using System;

using UoB.Core.Structure;

namespace UoB.Core.Structure.Detection
{
	/// <summary>
	/// Summary description for Detector.
	/// </summary>
	
	public abstract class Detector
	{
		protected PSMolContainer m_Polymer;

		public Detector( PSMolContainer p )
		{
			m_Polymer = p;
		}

		public abstract MatchState GetMatchState();
		public abstract DetectionMatch[] Matches();
		public abstract DetectionMatch[] Matches(int start, int length);
	}
}
