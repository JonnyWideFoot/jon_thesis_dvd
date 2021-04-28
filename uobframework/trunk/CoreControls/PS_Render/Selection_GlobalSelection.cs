using System;
using System.Collections;

using UoB.Core.Structure;

namespace UoB.CoreControls.PS_Render
{
	/// <summary>
	/// Summary description for GLobalSelection.
	/// </summary>
	public class GlobalSelection : Selection
	{
		public GlobalSelection( ParticleSystem ps )
		{
			m_Name = "Global Selection";
			if( ps != null )
			{
				m_AtomIndexes = new ArrayList( ps.Count );
				for( int i = 0; i < ps.Count; i++ )
				{
					m_AtomIndexes.Add( ps[i].ArrayIndex );
				}
			}
			else
			{
				m_AtomIndexes = new ArrayList();
			}
		}

		public void ResetPS( ParticleSystem ps )
		{
			if( ps != null )
			{
				m_AtomIndexes.Clear();
				for( int i = 0; i < ps.Count; i++ )
				{
					m_AtomIndexes.Add( ps[i].ArrayIndex );
				}
			}
			else
			{
				m_AtomIndexes.Clear();
			}
		}

		public override bool IsActive
		{
			get
			{
				return true;
			}
			set
			{
				// meaningless for global selection, but have to overrirde the base class
			}
		}

		public override bool Inverted // irelevent, but we have to set
		{
			get
			{
				// meaningless for global selection, but have to overrirde the base class
				return m_Inverted;
			}
			set
			{
				// meaningless for global selection, but have to overrirde the base class
                m_Inverted = value;
			}
		}

		public override void autoName()
		{
			// meaningless for global selection, but have to overrirde the base class
            // name stays as "Global"
		}

		public override string Name
		{
			get
			{
				return m_Name;
			}
			set
			{
				// meaningless for global selection, but have to overrirde the base class
			}
		}
	}
}
