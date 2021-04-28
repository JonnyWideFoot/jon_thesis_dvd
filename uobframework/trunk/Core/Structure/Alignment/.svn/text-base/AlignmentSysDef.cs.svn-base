using System;
using System.Text;
using System.Collections;

using UoB.Core.FileIO.PDB;
using UoB.Core.Primitives;
using UoB.Core.Structure;

namespace UoB.Core.Structure.Alignment
{
	/// <summary>
	/// Summary description for AlignmentSysDef.
	/// </summary>
	public sealed class AlignmentSysDef
	{
		// particle system containers
		private ParticleSystem m_PartSys; // holds the original cloned PS
		private AlignSourceDefinition m_Def1; // hols the origainal location of the clones member
		private AlignSourceDefinition m_Def2;
		private PSMolContainer m_PS1; // holds the cloned member
		private PSMolContainer m_PS2;
		private PS_PositionStore m_PosStore;
		private string m_Report = "";

		// model definitions
		private ModelList m_Models;

		public AlignmentSysDef( PS_PositionStore modelContainer, int molIndex1, int molIndex2, AlignSourceDefinition def1, AlignSourceDefinition def2 )
		{
			m_PosStore = modelContainer;
			m_PartSys = m_PosStore.particleSystem;
			m_Def1 = def1;
			m_Def2 = def2;
			m_PS1 = m_PartSys.MemberAt(molIndex1); 
			m_PS2 = m_PartSys.MemberAt(molIndex2);
			m_Models = new ModelList(m_PS1,m_PS2);
		}

		public AlignSourceDefinition SourceDef1
		{
			get
			{
				return m_Def1;
			}
		}

		public AlignSourceDefinition SourceDef2
		{
			get
			{
				return m_Def2;
			}
		}

		public string Report
		{
			get
			{
				return m_Report;
			}
			set
			{
				m_Report = value;
			}
		}

		public PSMolContainer PS1
		{
			get
			{
				return m_PS1;
			}
		}

		public PSMolContainer PS2
		{
			get
			{
				return m_PS2;
			}
		}

		public ParticleSystem particleSystem
		{
			get
			{
				return m_PosStore.particleSystem;
			}
		}

		public PS_PositionStore ModelStore
		{
			get
			{
				// this stores all of the generated models of a given alignment system, 0 represents pre-alignment
				// 1 -> n is best to worst
				return m_PosStore;
			}
		}

		public ModelList Models
		{
			get
			{
				// this stores all of the generated models of a given alignment system, 0 represents pre-alignment
				// 1 -> n is best to worst
				return m_Models;
			}
		}
	}
}
