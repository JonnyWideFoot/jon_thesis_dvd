using System;

using UoB.Core.Sequence;
using UoB.Core.Structure;

namespace UoB.Core.FileIO
{
	/// <summary>
	/// Summary description for FileType_Structural.
	/// </summary>
	public abstract class BaseFileType_Structural : BaseFileType
	{
		protected ParticleSystem m_PS = null;
		protected PSSequence m_Sequence = null;
		protected PS_PositionStore m_Positions = null;
		protected bool m_IsTextBased = false;

		public BaseFileType_Structural( string fileName, bool extractPS ) : base()
		{
			m_PS = null;
			m_Positions = null;
			base.LoadFromFile( fileName );
			if( extractPS && IsValidFile ) ExtractParticleSystem();
		}

		protected abstract void ExtractParticleSystem();

		public bool IsTextbased
		{
			get
			{
				return m_IsTextBased;
			}
		}

		public PS_PositionStore PositionDefinitions
		{
			get
			{
				return m_Positions;
			}
		}

		public ParticleSystem particleSystem
		{
			get
			{
				return m_PS;
			}
		}

		public PSSequence SequenceInfo
		{
			get
			{
				return m_Sequence;
			}
		}
	}
}
