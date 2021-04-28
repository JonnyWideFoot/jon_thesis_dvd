using System;
using System.Text;
using System.IO;

using UoB.Core.Structure;
using UoB.Core.Primitives;

namespace UoB.Core.Structure.Constraints
{
	/// <summary>
	/// Summary description for Constraint.
	/// </summary>
	public sealed class Constraint // sealed, internal knowledge of enums is required for the structuing of the parent classes
	{
		private static StringBuilder m_StringBuilder = new StringBuilder();

		private PotentialType m_PotentialType = PotentialType.Undefined;
		
		private Atom m_Atom1 = null; // CRITICAL : the TRA position of the Atoms coordinates muct be set by the parent class prior to data mining
		private Position m_Pos2 = null; // can be filled with an atom to allow a relative restraint

		private ConstraintGroup m_ParentGroup = null;
		private float[] m_Args;

		public Constraint( ConstraintGroup parent )
		{
			m_ParentGroup = parent;
			m_Args = new float[4];
		}

		#region public functions

		public void SetTo( Atom a, Position b, PotentialType type, params float[] args )
		{
			m_Atom1 = a;
			m_Pos2 = b;
			
			m_PotentialType = type;

			// make assertions :
			switch( type )
			{
				case PotentialType.Symmetric_VShaped:
					// [0] = desireddistance
					// [1] = epsilon
					// [2] = gamma 
					// [3] = well width
					if( args.Length != 4 )
					{
						DoError("Incorrect arguments length for PotentialType.Symmetric_VShaped in \"SetTo()\" call");
					}
					break;
				case PotentialType.Harmonic:
					if( args.Length != 2 )
					{
						DoError("Incorrect arguments length for PotentialType.Harmonic in \"SetTo()\" call");
					}
					break;
				case PotentialType.VShaped:
					if( args.Length != 4 )
					{
						DoError("Incorrect arguments length for PotentialType.VShaped in \"SetTo()\" call");
					}
					break;
				case PotentialType.BellShaped:
					if( args.Length != 3 )
					{
						DoError("Incorrect arguments length for PotentialType.VShaped in \"SetTo()\" call");
					}
					break;
				case PotentialType.Undefined:
					DoError("Undefined PotentialType in \"SetTo()\" call");
					break;
				default:
					DoError("Unknown PotentialType in \"SetTo()\" call");
					break;
			}

			// all ok, set those bitches
			for( int i = 0; i < args.Length; i++ )
			{
				m_Args[i] = args[i];
			}
		}


		#endregion

		#region Accessors

		public Atom atom
		{
			get
			{
				return m_Atom1;
			}
		}

		public Position anchor
		{
			get
			{
				return m_Pos2;
			}
		}
		public bool IsAbsoluteConstraint
		{
			get
			{
				return !(m_Pos2 is Atom);
			}
		}

		public bool IsRelativeRestraint
		{
			get
			{
				return m_Pos2 is Atom;
			}
		}

		public PotentialType potential
		{
			get
			{
				return m_PotentialType;
			}
		}

		public float IdealValue
		{
			get
			{
				return m_Args[0];
			}
		}

		public string GetName( MonitorType type )
		{
			GenerateInternalName( type );
			return m_StringBuilder.ToString();
		}


		#endregion

		#region helper functions

		private void GenerateInternalName( MonitorType type )
		{
			// clear and then store in m_StringBuilder

			m_StringBuilder.Remove( 0, m_StringBuilder.Length );

			switch( type )
			{
				case MonitorType.Absolute:
					m_StringBuilder.Append( "DAbs: " );
					m_StringBuilder.Append( m_Atom1.parentMolecule.moleculePrimitive.MolName );
                    m_StringBuilder.Append( ' ' );
					m_StringBuilder.Append( m_Atom1.parentMolecule.ResidueNumber );
					m_StringBuilder.Append( ' ' );
					m_StringBuilder.Append( m_Atom1.atomPrimitive.PDBIdentifier );
					m_StringBuilder.Append( " from " );
					m_StringBuilder.Append( '(' );
					m_StringBuilder.Append( m_Pos2.xFloat );
					m_StringBuilder.Append( ',' );
					m_StringBuilder.Append( m_Pos2.yFloat );
					m_StringBuilder.Append( ',' );
					m_StringBuilder.Append( m_Pos2.zFloat );
					m_StringBuilder.Append( ')' );
					break;
				case MonitorType.DeviationFromIdeal:
					m_StringBuilder.Append( "DDev: Idl: " );
					// m_Info[0] == Ideal Value
					m_StringBuilder.Append( m_Args[0] );

					m_StringBuilder.Append( m_Atom1.parentMolecule.moleculePrimitive.MolName );
					m_StringBuilder.Append( ' ' );
					m_StringBuilder.Append( m_Atom1.parentMolecule.ResidueNumber );
					m_StringBuilder.Append( ' ' );
					m_StringBuilder.Append( m_Atom1.atomPrimitive.PDBIdentifier );

					m_StringBuilder.Append( " from " );

					if( m_Pos2 is Atom )
					{
						Atom a2 = (Atom) m_Pos2;
						m_StringBuilder.Append( a2.parentMolecule.moleculePrimitive.MolName );
						m_StringBuilder.Append( ' ' );
						m_StringBuilder.Append( a2.parentMolecule.ResidueNumber );
						m_StringBuilder.Append( ' ' );
						m_StringBuilder.Append( a2.atomPrimitive.PDBIdentifier );
					}
					else
					{
						m_StringBuilder.Append( '(' );
						m_StringBuilder.Append( m_Pos2.xFloat );
						m_StringBuilder.Append( ',' );
						m_StringBuilder.Append( m_Pos2.yFloat );
						m_StringBuilder.Append( ',' );
						m_StringBuilder.Append( m_Pos2.zFloat );
						m_StringBuilder.Append( ')' );
					}
					break;
				default:
					throw new Exception("Unknown Monitor Type Given");
			}
		}


		private void DoError( string message )
		{
			throw new ArgumentException( message );
		}


		#endregion

		#region file writing

		// example line
		// 3 C 7 N 1 V 4.70   10.0 1.0 1.0

		public void WriteLines( StreamWriter rw )
		{
			m_StringBuilder.Remove( 0, m_StringBuilder.Length );

			// now the type-based layer			
			switch( m_PotentialType )
			{
				case PotentialType.Harmonic:
					// Constraint_Harmonic : Epsilon
					WriteCommonStart( rw );
					m_StringBuilder.Append( "H " ); // the well type ID
					m_StringBuilder.Append( m_Args[0].ToString() ); // desired distance
					m_StringBuilder.Append( ' ' );
					m_StringBuilder.Append( m_Args[1].ToString() ); // epsilon
					break;
				case PotentialType.VShaped:
					// Constraint_VShaped : Epsilon Gamma Beta
					WriteCommonStart( rw );
					m_StringBuilder.Append( "V " ); // the well type ID
					m_StringBuilder.Append( m_Args[0].ToString() ); // desired distance
					m_StringBuilder.Append( ' ' );
					m_StringBuilder.Append( m_Args[1].ToString() ); // epsilon
					m_StringBuilder.Append( ' ' );
					m_StringBuilder.Append( m_Args[2].ToString() ); // gamma
					m_StringBuilder.Append( ' ' );
					m_StringBuilder.Append( m_Args[3].ToString() ); // beta
					break;
				case PotentialType.Symmetric_VShaped:
					// Constraint_SymetricVShaped : Epsilon Gamma WellWidth
					m_StringBuilder.Append("#B S\r\n"); // ConMiniGroup Definition ID

					// potential to bring atom1 towards atom2
					WriteCommonStart( rw );
					m_StringBuilder.Append( "V " ); // the well type ID
					m_StringBuilder.Append( (m_Args[0] + m_Args[3]).ToString() ); // desired distance plus well width
					m_StringBuilder.Append( ' ' );
					m_StringBuilder.Append( m_Args[1].ToString() ); // epsilon
					m_StringBuilder.Append( ' ' );
					m_StringBuilder.Append( m_Args[2].ToString() ); // gamma
					m_StringBuilder.Append( " 1.0" ); // beta

					m_StringBuilder.Append("\r\n");

					// potential to bring atom2 towards atom1
					WriteCommonStart( rw );
					m_StringBuilder.Append( "V " ); // the well type ID
					m_StringBuilder.Append( (m_Args[0] - m_Args[3]).ToString() ); // desired distance minus well width
					m_StringBuilder.Append( ' ' );
					m_StringBuilder.Append( m_Args[1].ToString() ); // epsilon
					m_StringBuilder.Append( ' ' );
					m_StringBuilder.Append( m_Args[2].ToString() ); // gamma
					m_StringBuilder.Append( " -1.0" ); // minus beta

					m_StringBuilder.Append("\r\n#E S");
					break;
				case PotentialType.BellShaped:
					// Constraint_BellShaped Epsilon Gamma
					// potential to bring atom1 towards atom2
					WriteCommonStart( rw );
					m_StringBuilder.Append( "B " ); // the well type ID
					m_StringBuilder.Append( m_Args[0].ToString() );  // desired distance
					m_StringBuilder.Append( ' ' );
					m_StringBuilder.Append( m_Args[1].ToString() ); // epsilon
					m_StringBuilder.Append( ' ' );
					m_StringBuilder.Append( m_Args[2].ToString() ); // gamma
					break;
				case PotentialType.Undefined:
					throw new Exception("PotentialType.Undefined found. Definition must occur.");
				default:
					throw new Exception("Unknown PotentialType encountered");
			}

			rw.WriteLine( m_StringBuilder );
		}

		private void WriteCommonStart( StreamWriter rw )
		{
			m_StringBuilder.Append( m_Atom1.parentMolecule.ResidueNumber );
			m_StringBuilder.Append( ' ' );
			m_StringBuilder.Append( m_Atom1.PDBType );
			m_StringBuilder.Append( ' ' );
			if( m_Pos2 is Atom )
			{
				Atom m_Atom2 = (Atom) m_Pos2;
				m_StringBuilder.Append( m_Atom2.parentMolecule.ResidueNumber );
				m_StringBuilder.Append( ' ' );
				m_StringBuilder.Append( m_Atom2.PDBType );
				m_StringBuilder.Append( ' ' );
			}
			else
			{
				m_StringBuilder.Append( "999" );
				m_StringBuilder.Append( ' ' );
				m_StringBuilder.Append( '(' );
				m_StringBuilder.Append( m_Pos2.xFloat );
				m_StringBuilder.Append( ',' );
				m_StringBuilder.Append( m_Pos2.yFloat );
				m_StringBuilder.Append( ',' );
				m_StringBuilder.Append( m_Pos2.zFloat );
				m_StringBuilder.Append( ')' );
				m_StringBuilder.Append( ' ' );
			}
			// not in current implementation
			//m_StringBuilder.Append( m_ParentGroup.GroupID.ToString() );
			//m_StringBuilder.Append( ' ' );
		}


		#endregion

		#region data mining

		public float AbsoluteDataValue
		{
			get
			{
				return Position.distanceBetween( m_Atom1, m_Pos2 );
			}
		}

		public float RelativeDataValue
		{
			get
			{
				return Position.distanceBetween( m_Atom1, m_Pos2 ) - m_Args[0];
			}
		}
		#endregion
	}
}
	
