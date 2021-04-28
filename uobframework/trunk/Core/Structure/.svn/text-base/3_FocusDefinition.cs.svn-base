using System;
using UoB.Core.Primitives;

namespace UoB.Core.Structure
{
	/// <summary>
	/// Summary description for CentreDefinition.
	/// </summary>
	/// 
	public enum FocusingMode
	{
		GeometricFocus,
		FocusByResidue
	}

	public class FocusDefinition : Position
	{
		private bool m_AutoUpdateOnPosChange;
		private PSMolContainer m_FocusMember = null;
		private int m_ResidueIndex = -1;
		private ParticleSystem m_Parent;
		private FocusingMode m_Mode = FocusingMode.GeometricFocus;
		public event UpdateEvent FocusUpdate;
		private UpdateEvent m_PosUpdate;

		public FocusDefinition( ParticleSystem ps ) : base()
		{
			m_Parent = ps;
			FocusUpdate = new UpdateEvent( ReCalc );
			m_PosUpdate = new UpdateEvent( AutoReCalc );
			m_Parent.PositionsUpdate += m_PosUpdate;

			// try to find the default state of the autorecenter
			try
			{
				if( CoreIni.Instance.ContainsKey( "AutoRecenterDefaultOn" ) )
				{
					m_AutoUpdateOnPosChange = bool.Parse( CoreIni.Instance.ValueOf( "AutoRecenterDefaultOn" ) );
				}
			}
			catch
			{
				// we dont really care
				m_AutoUpdateOnPosChange = true; // but default to this
			}
		}

		private void nullFunc()
		{
		}

		public bool AutoUpdateOnPosChange
		{
			get
			{
				return m_AutoUpdateOnPosChange;
			}
			set
			{
				m_AutoUpdateOnPosChange = value;
			}
		}

		public char ChainID
		{
			get
			{
				if( m_FocusMember != null )
				{
					return m_FocusMember.ChainID;
				}
				else
				{
					return '\0';
				}
			}
		}

		public PSMolContainer FocusMember
		{
			get
			{
				return m_FocusMember;
			}
			set
			{
				m_FocusMember = value;
				m_ResidueIndex = 0; // the origainal value is now meaningless
				FocusUpdate();
			}
		}

		public int ResidueIndex
		{
			get
			{
				return m_ResidueIndex;
			}
			set
			{
				if( value < m_FocusMember.Count && value >= 0 )
				{
					m_ResidueIndex = value;
				}
				else
				{
					m_ResidueIndex = 0; // set to zero if the number is bolox
				}
				FocusUpdate();
			}
		}

		public FocusingMode Mode
		{
			get
			{
				return m_Mode;
			}
			set
			{
				if( m_Mode != value ) // dont want to reset members if the mode is still the same
				{
					m_Mode = value;
					switch( m_Mode )
					{
						case FocusingMode.GeometricFocus:
						{
							m_ResidueIndex = -1;
							m_FocusMember = null;
							break;
						}
						case FocusingMode.FocusByResidue:
						{
							m_ResidueIndex = 0;
							m_FocusMember = m_Parent.MemberAt(0);
							break;
						}
						default:
							throw new Exception("Code not implemented");
					}
					FocusUpdate();	
				}
			}
		}

		public void SetResidueFocusing( char chainID, int resID )
		{
			SetResidueFocusing( m_Parent.MemberWithID( chainID ), resID );
		}

		public void SetResidueFocusing( int memberIndex, int resID )
		{
			SetResidueFocusing( m_Parent.MemberAt( memberIndex ), resID );
		}

		public void SetResidueFocusing( PSMolContainer mol, int resID )
		{
			m_Mode = FocusingMode.FocusByResidue;
			m_FocusMember = mol;
			ResidueIndex = resID; // call external version
            //internalSetResidueFocusing(); is set by calling focusupdate()
			FocusUpdate();
		}

		private void internalSetResidueFocusing()
		{
			Molecule m = m_FocusMember[m_ResidueIndex];
			setTo( m[0] ); // assume that this is a good plan
		}

		public void SetGeometricFocusing()
		{
			m_Mode = FocusingMode.GeometricFocus;
			// internalSetGeometricFocusing(); again called by focusupdate()
			FocusUpdate();
		}

		private void internalSetGeometricFocusing()
		{
			int number = m_Parent.Count;
			double sumX = 0.0;
			double sumY = 0.0;
			double sumZ = 0.0;

			for( int i = 0; i < m_Parent.Count; i++ )
			{
				Atom a = m_Parent[i];
				sumX += a.x;
				sumY += a.y;
				sumZ += a.z;
			}

			m_X = (sumX / number);
			m_Y = (sumY / number); 
			m_Z = (sumZ / number);
		}

		public void CallRecenter()
		{
			FocusUpdate();
		}

		public void AutoReCalc()
		{
			if( m_AutoUpdateOnPosChange )
			{
				ReCalc();
			}
		}

		public void ReCalc()
		{
			switch( m_Mode )
			{
				case FocusingMode.GeometricFocus:
				{
					internalSetGeometricFocusing();
					break;
				}
				case FocusingMode.FocusByResidue:
				{
					internalSetResidueFocusing();
					break;
				}
				default:
					throw new Exception("Code not implemented");
			}
		}

		public void PhysicallyApplyToPS()
		{
			m_Parent.MinusAll( this );
		}
	}
}
