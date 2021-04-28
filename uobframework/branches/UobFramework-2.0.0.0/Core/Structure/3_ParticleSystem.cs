using System;
using System.Collections;
using System.Threading;
using System.Diagnostics;

using UoB.Core.Primitives;


namespace UoB.Core.Structure
{
	/// <summary>
	/// Summary description for Graph.
	/// </summary>
		
	public class ParticleSystem : AtomList, ICloneable
	{
		public event UpdateEvent ContentUpdate;
		public event UpdateEvent PositionsUpdate;

		private ReaderWriterLock m_Lock;

		private ArrayList m_Members;
		private string m_Name;
		private FocusDefinition m_SysFocus;

		public ParticleSystem(string name)
		{
			ContentUpdate = new UpdateEvent( nullFunc );
			PositionsUpdate = new UpdateEvent( nullFunc );

			m_Name = name;
			m_SysFocus = new FocusDefinition(this);
			m_Lock = new ReaderWriterLock();
			m_Members = new ArrayList();
		}

		public FocusDefinition SystemFocus
		{
			get
			{
				return m_SysFocus;
			}
		}

		public Atom GetAtom( int polymerID, int residueIndex, string AtomName )
		{
			PSMolContainer mol = this.MemberAt( polymerID );
			Molecule m = mol[residueIndex];
			return m.AtomOfType( AtomName );
		}

		/// <summary>
		/// These base functions are perhaps poorly designed - an exception should not be thrown in this way
		/// </summary>
		/// <param name="theAtom"></param>
		public override void addAtom(Atom theAtom)
		{
			throw new Exception("Code anomaly - An illegal attempt was make to add atoms to the particle system - this is not allowed" );
			// do nothing - atoms MUST be added via members
		}

		public override void addAtomArray(Atom[] theAtoms)
		{			
			throw new Exception("Code anomaly - An illegal attempt was make to add atoms to the particle system - this is not allowed" );
			// do nothing - atoms MUST be added via members
		}

		public override void addAtomList(AtomList theAtoms)
		{			
			Trace.WriteLine("Code anomaly - An illegal attempt was make to add atoms to the particle system - this is not allowed" );
			// do nothing - atoms MUST be added via members
		}

		public void RemoveMember( int index )
		{
			if( index >= 0 && index < m_Members.Count )
			{
				RemoveMember( (PSMolContainer) m_Members[index] );
			}
		}

		public void RemoveMember( PSMolContainer mol )
		{
			bool done = false;
			while( !done )
			{
				try
				{
					AcquireWriterLock( 1000 );

					//we need to remove the members atoms from the PS allAtom holder
					for( int i = 0; i < mol.Count; i++ )
					{
						m_Atoms.Remove( mol[i] );
					}

					m_Members.Remove( mol );
					done = true;
				}
				catch // we didnt get the lock
				{
				}
				finally
				{
					ReleaseWriterLock();
				}
			}
			// we have changed the contents
			ContentUpdate();
		}

		public override void SetPositions(Position[] positions)
		{
			bool done = false;
			while(!done)
			{  
				try
				{
					m_Lock.AcquireWriterLock(1000);
					try
					{
						base.SetPositions (positions);
						done = true;
					}        
					finally
					{
						// Ensure that the lock is released.
						m_Lock.ReleaseWriterLock();
					}
				}
				catch (ApplicationException ex)
				{
					string s = ex.ToString();
					// The reader lock request timed out.
				}
			}
			PositionsUpdate();
		}

		public void AddMolContainer( PSMolContainer mc )
		{
			mc.Parent = this;
			mc.ArrayIndex = m_Members.Count;
			m_Members.Add( mc );
		}

		public int MemberCount
		{
			get
			{
				return m_Members.Count;
			}
		}

		public PSMolContainer[] Members
		{
			get
			{
				return (PSMolContainer[]) m_Members.ToArray( typeof(PSMolContainer) );
			}
		}

		public PSMolContainer MemberAt( int index )
		{
			if( index >= 0 &&  index < m_Members.Count )
			{
				return (PSMolContainer) m_Members[index];
			}
			else
			{
				return null;
			}
		}

		public PSMolContainer MemberWithID( char chainID )
		{
			for( int i = 0; i < m_Members.Count; i++ )
			{
				PSMolContainer member = (PSMolContainer) m_Members[i];
				if ( member.ChainID == chainID )
				{
					return member;
				}
			}
			return null;													
		}

		public void PhysicallyReCenterSystem()
		{						
			double number = (double) m_Atoms.Count;
			double factX = 0.0;
			double factY = 0.0;
			double factZ = 0.0;

			for( int i = 0; i < m_Atoms.Count; i++ )
			{
				Atom a = (Atom)m_Atoms[i];
				factX += a.x;
				factY += a.y;
				factZ += a.z;
			}

			factX = (factX / number);
			factY = (factY / number); 
			factZ = (factZ / number);

			for( int i = 0; i < Count; i++ )
			{
				this[i].x -= factX;
				this[i].y -= factY;
				this[i].z -= factZ;
			}		
		}

		public void MirrorInXPlane()
		{
			AcquireWriterLock(1000);

			for( int i = 0; i < Count; i++ )
			{
				this[i].x = -this[i].x;
			}	

			ReleaseWriterLock();

			PositionsUpdate();
		}

		public void BeginEditing()
		{
			m_Atoms.Clear();
			for( int i = 0; i < m_Members.Count; i++ )
			{
				PSMolContainer molContainer = (PSMolContainer)m_Members[i];
				molContainer.Atoms.Clear();
				for( int j = 0; j < molContainer.Count; j++ ) // j increments the molecules in each container
				{
					Molecule m = molContainer[j];
					for ( int k = 0; k < m.Count; k++ )
					{
						m[k].ClearBondList();// these will be reassigned following editing
					}
				}
			}
			// only molecule level objects keep a permenant atom list
			// for build operations, higher level objects clear their atom lists to allow atom addition
			// bonds are also cleared as they may need to be reassigned following atom addition / deletion
			// we wouldnt want to be referening null atoms now would we ;-)
		}

		public void EndEditing( bool performForceFieldBonding, bool allowAtomRenumber )
		{
			// the extended bond list in PDB files requires : allowAtomRenumber
			// the numbers refer to the atom numbers in the PDB file
			// in the future we should pass the bond list to this function ... but for the mo ...

			int uniqueAtomCounter = 0; // all atoms must be given a unique ID

			if ( performForceFieldBonding )
			{
				for( int i = 0; i < m_Members.Count; i++ ) // so i increments the PS members - i.e. the Polypeptides hetmolecules and the solvent
				{
					int perPolymerAtomCounter = 0;
					PSMolContainer molContainer = (PSMolContainer)m_Members[i];
					for( int j = 0; j < molContainer.Count; j++ ) // j increments the molecules in each container
					{
						molContainer[j].SortAtomList();
						molContainer.Atoms.addAtomList( molContainer[j] );
						for( int k = 0; k < molContainer[j].Count; k++ ) // k will do the atoms in each molecule
						{
							Atom a = molContainer[j][k];

							// IMPORTANT : we have turned off Atom Addition to the particle system via .Add()
							// so we have to do it manually here
							a.ArrayIndex = uniqueAtomCounter++; // we will also increment the unique array index
							if ( allowAtomRenumber )
							{
								a.AtomNumber = perPolymerAtomCounter++;
							}
							m_Atoms.Add( a );

							// we have added it and reassigned its number Indexers
							// now all we have to do is bond it to something using the partners defined by the 
							// atom primitive

							if( molContainer[j].moleculePrimitive != null )
							{
								if( a.atomPrimitive != null )
								{
									string[] bondingPartners = a.atomPrimitive.BondingPartners;
									foreach( string partner in bondingPartners ) // for each bonding partner we need to truck through the atoms to find it
									{
										AtomList list = null;
										string testPartner;
										if ( partner[0] == '+' ) // the bonding partner is in the next molecule
										{
											testPartner = partner.Substring(1,partner.Length-1) + " "; // get rid of the + for testing the string against atom alt-names
											if( (j+1) >= molContainer.Count ) continue; // partner is not available
											Molecule mol = molContainer[j+1];
											if ( mol == null ) continue;  // partner is not available
											list = mol;
										}
										else if ( partner[0] == '-' )  // the bonding partner is in the previous molecule
										{
											testPartner = partner.Substring(1,partner.Length-1) + " ";
											if( (j-1) < 0 ) continue; // partner is not available
											Molecule mol = molContainer[j-1];
											if ( mol == null ) continue;  // partner is not available
											list = mol;
										} 
										else
										{
											testPartner = partner;
											Molecule mol = molContainer[j];
											if ( mol == null ) continue;  // partner is not available
											list = mol;
										}

										for( int l = 0; l < list.Count; l++ )
										{
											if( testPartner == list[l].atomPrimitive.AltName )
											{
												a.bondTo( list[l] );
												break;
											}
										}
									}
								}
							}
							else
							{
								molContainer[j].PerformProximityBonding();
							}
						}
					}
				}
			}
			else
			{
				m_Atoms.Clear();
				for( int i = 0; i < m_Members.Count; i++ )
				{
					PSMolContainer molContainer = (PSMolContainer)m_Members[i];
					for( int j = 0; j < molContainer.Count; j++ ) // count refers to the number of molecules
					{
						molContainer.Atoms.addAtomList( molContainer[j] );
						for( int k = 0; k < molContainer[j].Count; k++ )
						{
							// IMPORTANT : we have turned off Atom Addition to the particle system via .Add()
							// so we have to do it manually here
							molContainer[j][k].ArrayIndex = uniqueAtomCounter++;
							m_Atoms.Add( molContainer[j][k] );
						}
					}
				}
			}

			m_SysFocus.ReCalc();
			ContentUpdate();
		}

		public override void MinusAll(Position p)
		{
			AcquireWriterLock(1000);
				base.MinusAll (p);
			ReleaseWriterLock();
			PositionsUpdate();
		}


		public string Name
		{
			get
			{
				return m_Name;
			}
			set
			{
				m_Name = value;
			}
		}

		private void nullFunc()
		{
		}

		public void AcquireReaderLock(int timeOut)
		{
			m_Lock.AcquireReaderLock(timeOut);
		}

		public void AcquireWriterLock(int timeOut)
		{
			m_Lock.AcquireWriterLock(timeOut);
		}

		public void ReleaseReaderLock()
		{
			m_Lock.ReleaseReaderLock();
		}
		public void ReleaseWriterLock()
		{
			m_Lock.ReleaseWriterLock();
		}

		public AtomList CreateSolventAtomList
		{
			get
			{
				AtomList SolventMemberAtoms = new AtomList();
				for ( int i = 0; i < m_Atoms.Count; i++ )
				{
					Atom a = (Atom) m_Atoms[i];
					if( a.isSolvent )
					{
						SolventMemberAtoms.addAtom( this[i] );
					}
				}                    
				return SolventMemberAtoms;
			}
		}

		public void SortMemberAtoms()
		{
			int counter = 0; // we need to reassign the atom indexes following list sorting
			for( int i = 0; i < m_Members.Count; i++ )
			{
				PSMolContainer psm = (PSMolContainer) m_Members[i];
				for( int j = 0; j < psm.Count; j++ )
				{
					psm[j].SortAtomList();
					for( int k = 0; k < psm[j].Count; k++ )
					{
						psm[j][k].ArrayIndex = counter++; 
					}
				}
			}
		}

		#region Helper Functions

		public static AminoAcid GetFurthestAA( PolyPeptide pp, params AminoAcid[] aas )
		{
			float bestDistanceSum = float.MinValue;
			AminoAcid best = null;

			for( int i = 0; i < pp.Count; i++ )
			{
				for( int j = 0; j < aas.Length; j++ )
				{
					if( aas[j] == pp[i] )
					{
						// not allowed to include bests as one already in the list...
						goto SKIPAA; // continue in the outer loop ...
					}
				}
				float distanceSum = 0.0f;
				for( int j = 0; j < aas.Length; j++ )
				{
					distanceSum += Position.distanceBetween( pp[i].CAlphaAtom, aas[j].CAlphaAtom );
				}
				if( distanceSum > bestDistanceSum )
				{
					bestDistanceSum = distanceSum;
					best = pp[i];
				}

			SKIPAA:
				continue;
			}

			return best;
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			ParticleSystem ps = null;
			bool done = false;
			while(!done)
			{  
				try
				{
					AcquireReaderLock(1000);
					try
					{
						// It is safe for this thread to read from
						// the shared resource.


						ps = new ParticleSystem("Clone of : " + this.Name);
						ps.BeginEditing();

									
						for( int i = 0; i < m_Members.Count; i++ )
						{
							ps.AddMolContainer( (PSMolContainer)((PSMolContainer)m_Members[i]).Clone() );
						}

						ps.EndEditing(true,true);
						done = true;

					}        
					finally
					{
						// Ensure that the lock is released.
						ReleaseReaderLock();
					}
				}
				catch (ApplicationException)
				{
					// The reader lock request timed out.
				}
			}
			return ps;
		}

		#endregion
	}
}
