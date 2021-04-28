using System;

using UoB.Core.Structure;
using UoB.Core.Primitives;

namespace UoB.Core.Structure.Detection
{
	/// <summary>
	/// Summary description for Match.
	/// </summary>
	public class DetectionMatch : MolRange
	{
		protected Position m_Center;

		public DetectionMatch( PSMolContainer molecule, int startIndex, char insertionCode, int length, Position matchCenter  )
			: base( molecule, startIndex, insertionCode, length )
		{
			m_Center = matchCenter;
		}

		public Position MatchCenter
		{
			get
			{
				return m_Center;
			}
		}
	}
}
