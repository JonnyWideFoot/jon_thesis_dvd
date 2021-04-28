using System;
using System.Collections;
using System.Drawing;
using System.Diagnostics;

using UoB.Core.Structure.Alignment;
using UoB.Core.Structure;
using UoB.Core.Primitives;
using UoB.Core.Primitives.Matrix;
using UoB.Core.Primitives.Collections;
using UoB.Core;
using UoB.Core.Tools;
using UoB.Core.ForceField;
using UoB.Core.FileIO.PDB;

using UoB.CoreControls.OpenGLView;
using UoB.CoreControls.OpenGLView.Primitives;
using UoB.CoreControls.OpenGLView.RenderManagers;

namespace UoB.CoreControls.PS_Render
{
	/// <summary>
	/// Summary description for ParticleSystemDrawWrapper.
	/// </summary>
	public sealed class ParticleSystemDrawWrapper
	{
		private ParticleSystem m_PS;
		private AtomDrawWrapper[] m_AtomDrawWrappers;
		private Perspective m_Perspective;
		private ArrayList endPoints; // used to hold the indexes of the polypeptide terminal AAs
		private GLView m_Viewer;
		// New for this month only, the all singing all dancing selection mode!
		private ArrayList m_Selections;
		private bool m_SelectionLock = false;
		private object m_LockHolder = null;

		private LineRender lv = null;
		private PointRender pv = null;
		private Surface sm = null;
		private HBonding hb = null;
		private ForceVectors fv = null; // not initialised with the rest as they are a special case used in tra files
		private LineStoreRender el = null; // ditto above

		private ArrayList connections = null;
		private ArrayList midPoints = null;
		private ArrayList singlePoints = null;
		private ArrayList moleculesForCartoon = null;

		private object WriteLock = new object();

		private UpdateEvent m_ContentUpdate;
		private UpdateEvent m_PositionsUpdate;
		private UpdateEvent m_FocusUpdate;

		public ParticleSystemDrawWrapper( ParticleSystem ps, GLView viewer )
		{
			//Debug.WriteLine("DrawWrapper ParticleSystemDrawWrapper constructor fired");

			if( viewer == null )
			{
				throw new Exception("Viewer to write to was null");
			}

			m_Viewer = viewer;
			m_Perspective = viewer.perspective;

			m_ContentUpdate = new  UpdateEvent( onContentUpdate );
			m_PositionsUpdate = new UpdateEvent( onPositionsUpdate );
			m_FocusUpdate = new UpdateEvent( onFocusUpdate );
			DrawExtrasUpdate = new UpdateEvent( m_Viewer.Refresh );

			// creationWeight definition this needs to occur before we initialise the "external" particlesystem field
			int creationWeight = 4000;
			m_Selections = new ArrayList(10); // holds the user defined viewing selections
			
			m_Selections.Add( new GlobalSelection( ps ) );// can cope with null ps

			if( ps != null )
			{
				creationWeight = (int)(ps.Count * 2.2);
			}
					
			connections = new ArrayList(creationWeight);
			midPoints = new ArrayList(creationWeight);
			singlePoints = new ArrayList(200); // roughly the possible number of solvent molecules
			moleculesForCartoon = new ArrayList(500);
			InitialiseCartoon(); // needs to occur only once to initialise the temporary holders

			lv = new LineRender( m_Viewer );
			pv = new PointRender( m_Viewer );
			sm = new Surface( m_Viewer );
			hb = new HBonding( m_Viewer );
			m_Viewer.AddRenderObject( lv );
			m_Viewer.AddRenderObject( pv );
			m_Viewer.AddRenderObject( sm );
			m_Viewer.AddRenderObject( hb );

			particleSystem = ps; // IMPORTANT : call the "external" varsion to properly associate the relevent events
			// particleSystem = ps ---> calls getatomlist()

			//Debug.WriteLine("DrawWrapper ParticleSystemDrawWrapper constructor ended");
		}

		public Perspective perspective
		{
			get
			{
				return m_Perspective;
			}
		}

		public void TriggerViewerRefresh()
		{
			m_Viewer.Refresh();
		}


		private void onFocusUpdate()
		{
			// NO, the Rotationoffset should BE the m_PS.SystemFocus, then there is no need to update
			// this is set in the particle system accessor below
			//perspective.RotationOffset.setTo( m_PS.SystemFocus );

			m_Perspective.RenderOffset.setToZeros(); // the translation is now irrelevent
			onPositionsUpdate();
			m_Viewer.Refresh();
		}

		private void onContentUpdate()
		{
			//Debug.WriteLine("DrawWrapper content update fired");
			if ( m_PS == null )
			{
				NullifyContent();				
			}
			else
			{
				getAtomList(); // sets up m_AtomDrawWrappers
				// reinitialise the selections, they are now invalid
				BeginSelectionEdit();
				GlobalSelection global = (GlobalSelection)m_Selections[0];
				global.ResetPS( m_PS ); // even if its null, reset all ...
				ClearSlections(); // resets current to 0
				EndSelectionEdit(); // calls SetUpRenderList() and 	SelectionUpdated()			
			}	
		}

		private void onPositionsUpdate()
		{	
			//Debug.WriteLine("DrawWrapper positions update fired");
			// resetPositions
			Position offset = m_Perspective.RotationOffset;
			for ( int i = 0; i < m_AtomDrawWrappers.Length; i++ )
			{
				m_AtomDrawWrappers[i].reGetAtomPosition( offset );
			}

			UpdateRenderList(); // then update the positions in the more complicated render objects
		}

		private void NullifyContent()
		{
			m_AtomDrawWrappers = new AtomDrawWrapper[0];
			
			lock ( WriteLock )
			{
				// Reinitialise Holders
				connections.Clear();
				midPoints.Clear();
				singlePoints.Clear();
				moleculesForCartoon.Clear();

				pv.vectorArray = new AtomDrawWrapper[0];
		
				lv.vectorArray = m_AtomDrawWrappers;
				lv.connectionList = new int[0];
				lv.midpointArray = new Vector[0];

				SetUpCartoon();

				m_Viewer.Refresh();

			}

			ClearSlections(); // resets current to 0
			GlobalSelection global = (GlobalSelection)m_Selections[0];
			global.ResetPS( null ); // even if its null, reset all ...
			SelectionUpdated();
		}

		private void SetUpRenderList() // hit on particlesystem content update and when graw modes are changed
		{
			// Reinitialise Holders
			connections.Clear();
			midPoints.Clear();
			singlePoints.Clear();
			moleculesForCartoon.Clear();

			lock ( WriteLock )
			{
				if( m_EasyViewMode )
				{
					for( int i = 0; i < m_PS.Count; i++ )
					{
						Atom a = m_PS[i];
						int bonds = a.bondCount;
						if( bonds == 0 )
						{
							singlePoints.Add( m_AtomDrawWrappers[i] );
						}
						else
						{
							for ( int j = 0; j < bonds; j++ )
							{
								AtomDrawWrapper AtomJ = m_AtomDrawWrappers[ a.BondedAtoms[j].ArrayIndex ];
								if ( m_AtomDrawWrappers[i].ShouldDisplay && AtomJ.ShouldDisplay )
								{
									connections.Add( a.ArrayIndex );
									midPoints.Add( Vector.CenterPointBetween( m_AtomDrawWrappers[i], AtomJ ) );
								}
								else
								{
									// add in case atoms become visible later
									connections.Add( -1 );
									midPoints.Add( new Vector(0,0,0) );
								}
							}
						}
					}
				}
				else
				{
					for( int i = 0; i < m_PS.Count; i++ )
					{
						// do checks for each graphics type on each atom and set up the relevent rendering for it                    
						if( (m_AtomDrawWrappers[i].DrawStyle & AtomDrawStyle.Lines) == AtomDrawStyle.Lines)
						{
							if ( m_AtomDrawWrappers[i].DisplayMode == AtomDisplayMode.CAlphaTrace )
							{
								if ( m_AtomDrawWrappers[i].isCAlpha )
								{
									// look for the next c alpha,
									int nextCAlpha = -1;
									for ( int j = i+1; j < m_AtomDrawWrappers.Length; j++ )
									{
										if ( m_AtomDrawWrappers[j].isCAlpha )
										{
											nextCAlpha = j;
											break;
										}
									}
									if ( nextCAlpha != -1 )
									{
										Atom a = m_AtomDrawWrappers[ i ].m_Atom;
										if ( !endPoints.Contains( i ) )
										{
											AtomDrawWrapper AtomJ = m_AtomDrawWrappers[ nextCAlpha ];
								
											if ( AtomJ.ShouldDisplay )
											{
												connections.Add( a.ArrayIndex );
												midPoints.Add( AtomJ );
											}

											if ( (m_AtomDrawWrappers[nextCAlpha].DisplayMode & AtomDisplayMode.CAlphaTrace) == AtomDisplayMode.CAlphaTrace )
											{
												i = nextCAlpha-1; // no point in checking the other atoms in between
											}
											else
											{
                                                // render the next i as normal ...
											}
										}
									}
									else
									{
										// there is no next CA
									}
								}
								else
								{
									continue; // the atom is labeled as part of a cAlpah group of atoms but isnt a cAlpha itself, so no lines are required
								}
							}
							else
							{
								Atom a = m_PS[i];
								int bonds = a.bondCount;
								if( bonds == 0 )
								{
									singlePoints.Add( m_AtomDrawWrappers[i] );
								}
								else
								{
									for ( int j = 0; j < bonds; j++ )
									{
										AtomDrawWrapper AtomJ = m_AtomDrawWrappers[ a.BondedAtoms[j].ArrayIndex ];
										if ( m_AtomDrawWrappers[i].ShouldDisplay && AtomJ.ShouldDisplay )
										{
											connections.Add( a.ArrayIndex );
											midPoints.Add( Vector.CenterPointBetween( m_AtomDrawWrappers[i], AtomJ ) );
										}
										else
										{
											// add in case atoms become visible later
											connections.Add( -1 );
											midPoints.Add( new Vector(0,0,0) );
										}
									}
								}
							}
						}

						if( (m_AtomDrawWrappers[i].DrawStyle & AtomDrawStyle.CPK) == AtomDrawStyle.CPK )
						{
							//m_AtomDrawWrappers[i].getCPKSpheres( dm.m_SphereList );
						}

						if( (m_AtomDrawWrappers[i].DrawStyle & AtomDrawStyle.BallAndStick) == AtomDrawStyle.BallAndStick )
						{
							//m_AtomDrawWrappers[i].getBallandStick( dm.m_SphereList, lineMat );
						}

						if( (m_AtomDrawWrappers[i].DrawStyle & AtomDrawStyle.Ribbon) == AtomDrawStyle.Ribbon )
						{
							Molecule m = m_AtomDrawWrappers[i].m_Atom.parentMolecule;
							if ( !moleculesForCartoon.Contains(m) )
							{
								if ( m is AminoAcid )
								{
									moleculesForCartoon.Add( m );
								}
							}
						}	
					}
				
				}

				pv.vectorArray = (AtomDrawWrapper[]) singlePoints.ToArray( typeof( AtomDrawWrapper ) );
			
				// we will pass references of the atom draw wrappers as the positions and then use the bonds to index them
				lv.vectorArray = m_AtomDrawWrappers;
				lv.connectionList = (int[]) connections.ToArray( typeof( int ) );
				lv.midpointArray = (Vector[]) midPoints.ToArray( typeof( Vector ) );

				SetUpCartoon();

				m_Viewer.Refresh();

			}
		}

		private void UpdateRenderList()
		{
			lock ( WriteLock )
			{

				//Debug.WriteLine("DrawWrapper UpdateRenderList fired");

				int count = 0;

					// point only positions stay the same
					// carbon alpha positions stay the same
					// only midpoints for line drawing and cartoons need to be recalculated

				if( m_EasyViewMode )
				{
					for( int i = 0; i < m_PS.Count; i++ )
					{
						Atom a = m_PS[i];
						int bonds = a.bondCount;
						if( bonds == 0 )
						{
							// this was added above, no update is needed
							//singlePoints.Add( m_AtomDrawWrappers[i] );
						}
						else
						{
							for ( int j = 0; j < bonds; j++ )
							{
								int atomJIndex = a.BondedAtoms[j].ArrayIndex;
								if ( m_AtomDrawWrappers[i].ShouldDisplay && m_AtomDrawWrappers[atomJIndex].ShouldDisplay )
								{
									lv.connectionList[count] = a.ArrayIndex;
									lv.midpointArray[count++].SetToCentrePoint( m_AtomDrawWrappers[i], m_AtomDrawWrappers[atomJIndex] );
								}
								else
								{
									lv.connectionList[count] = -1;
									count++; // still need to increment the counter
									// leave the midpoint as it is, we arent using it anyway
								}
							}
						}
					}
				}
				else
				{
					for( int i = 0; i < m_PS.Count; i++ )
					{
						if( (m_AtomDrawWrappers[i].DrawStyle & AtomDrawStyle.Lines) ==  AtomDrawStyle.Lines )
						{
							if ( m_AtomDrawWrappers[i].DisplayMode == AtomDisplayMode.CAlphaTrace )
							{
								if ( m_AtomDrawWrappers[i].isCAlpha )
								{
									// look for the next c alpha,
									int nextCAlpha = -1;
									for ( int j = i+1; j < m_AtomDrawWrappers.Length; j++ )
									{
										if ( m_AtomDrawWrappers[j].isCAlpha )
										{
											nextCAlpha = j;
											break;
										}
									}
									if ( nextCAlpha != -1 )
									{
										Atom a = m_AtomDrawWrappers[ i ].m_Atom;
										if ( !endPoints.Contains( i ) )
										{
											AtomDrawWrapper AtomJ = m_AtomDrawWrappers[ nextCAlpha ];
								
											if ( AtomJ.ShouldDisplay )
											{
												lv.connectionList[count] = a.ArrayIndex;
												lv.midpointArray[count++].setTo( AtomJ );

												//connections.Add( a.ArrayIndex );
												//midPoints.Add( AtomJ );
											}

											if ( (m_AtomDrawWrappers[nextCAlpha].DisplayMode & AtomDisplayMode.CAlphaTrace) == AtomDisplayMode.CAlphaTrace )
											{
												i = nextCAlpha-1; // no point in checking the other atoms in between
											}
											else
											{
												// render the next i as normal ...
											}
										}
									}
									else
									{
										// there is no next CA
									}
								}
								else
								{
									continue; // the atom is labeled as part of a cAlpah group of atoms but isnt a cAlpha itself, so no lines are required
								}
							}
							else
							{
								Atom a = m_PS[i];
								int bonds = a.bondCount;
								if( bonds == 0 )
								{
									// this was added above, no update is needed
									// the line was : singlePoints.Add( m_AtomDrawWrappers[i] );
								}
								else
								{
									for ( int j = 0; j < bonds; j++ )
									{
										int atomJIndex = a.BondedAtoms[j].ArrayIndex;
										if ( m_AtomDrawWrappers[i].ShouldDisplay && m_AtomDrawWrappers[atomJIndex].ShouldDisplay )
										{
											lv.connectionList[count] = a.ArrayIndex;
											lv.midpointArray[count++].SetToCentrePoint( m_AtomDrawWrappers[i], m_AtomDrawWrappers[atomJIndex] );
										}
										else
										{
											lv.connectionList[count] = -1;
											count++; // still need to increment the counter
											// leave the midpoint as it is, we arent using it anyway
										}
									}
								}
							}
						}

						if( (m_AtomDrawWrappers[i].DrawStyle & AtomDrawStyle.CPK) == AtomDrawStyle.CPK )
						{
							//m_AtomDrawWrappers[i].getCPKSpheres( dm.m_SphereList );
						}

						if( (m_AtomDrawWrappers[i].DrawStyle & AtomDrawStyle.BallAndStick) == AtomDrawStyle.BallAndStick )
						{
							//m_AtomDrawWrappers[i].getBallandStick( dm.m_SphereList, lineMat );
						}

						if( (m_AtomDrawWrappers[i].DrawStyle & AtomDrawStyle.Ribbon) == AtomDrawStyle.Ribbon )
						{

							Molecule m = m_AtomDrawWrappers[i].m_Atom.parentMolecule;
							if ( !moleculesForCartoon.Contains(m) )
							{
								if ( m is AminoAcid )
								{
									moleculesForCartoon.Add( m );
								}
							}
						}
					}
				} // end for loop for all atoms
			
				UpdateCartoon();

				m_Viewer.Refresh();

			}
		}


		// Variables used in the generation of cartoon views
        private ExtrudeShape[] basicshape;
		private Vector vc; // temporary holder while we work out unit vectors
		private peptidegroup[] pg;
		private int[] shapetype; // array holding 0 or 1 depending on ture of flat ribbon
		private readonly int ribbonres = 6; // number of shapes per peptide group
		private Vector e_center = new Vector(0.0f,0.0f,0.0f);
		private Vector e_across = new Vector(0.0f,0.0f,0.0f);
		private Vector e_up = new Vector(0.0f,0.0f,0.0f);
		private Vector e_forward = new Vector(0.0f,0.0f,0.0f);
		private MatrixRotation rmat = new MatrixRotation(); // used in cartoon rendering, rotation of vectors

		private void InitialiseCartoon()
		{
			basicshape = new ExtrudeShape[2]; // array with two basic shapes - the square for the side of the ribbon and the rectangle for the face
			basicshape[0] = new ExtrudeShape();
			basicshape[0].setToSquareTube();
			basicshape[1] = new ExtrudeShape();
			basicshape[1].setToStrand();
		}

		private int nullNextMolCount = 0;
		private void SetUpCartoon()
		{
			sm.Colours = null;
			sm.Indexes = null;
			sm.Normals = null;
			sm.Vertices = null;

			nullNextMolCount = 0;

			int vertices = 0; // counter used later
			int faces = 0; // counter used later

			pg = new peptidegroup[ moleculesForCartoon.Count ];
			shapetype = new int [ moleculesForCartoon.Count ];
			Vector[] vertex = new Vector [ moleculesForCartoon.Count * 4 * ribbonres ];        // points to hold the coordinates of ribbon mesh
			Vector[] normal = new Vector [ moleculesForCartoon.Count * 4 * ribbonres ];        // normals of the surface at the above points
			PolygonIndexes[] face = new PolygonIndexes [ moleculesForCartoon.Count  * 4 * ribbonres * 2 ];    // two faces per new point

			// find the peptide group atoms & calcualte the orientation vectors
			//m_AtomDrawWrappers;
			for( int jr = 0; jr < moleculesForCartoon.Count - 1 ; jr++ ) // -1 cos we dont use the last one, its partner is null
			{
				Molecule m = (Molecule) moleculesForCartoon[jr];
				Molecule nextMol = m.Parent.nextMolecule(m);

				if ( nextMol == null ) // hack !!!!
				{
					nullNextMolCount++;
					continue;
				}

				int ir = jr - nullNextMolCount; // hack !!!!

				pg[ir] = new peptidegroup();

				for ( int j = 0; j < m.Count; j++ )
				{
					switch ( m[j].PDBType )
					{
						case " CA ":
							pg[ir].CA1 = m[j].ArrayIndex;
							break;
						case " C  ":
							pg[ir].C = m[j].ArrayIndex;
							break;
						case " O  ":
							pg[ir].O = m[j].ArrayIndex;
							break;
						default:
							break;
					}
				}

				for ( int j = 0; j < nextMol.Count; j++ )
				{
					switch ( nextMol[j].PDBType )
					{
						case " CA ":
							pg[ir].CA2 = nextMol[j].ArrayIndex;
							break;
						case " N  ":
							pg[ir].N =   nextMol[j].ArrayIndex;
							break;
						default:
							break;
					}
				}

				if(pg[ir].CA1 == -1) continue; 
				if(pg[ir].CA2 == -1) continue;
				if(pg[ir].C == -1) continue;
				if(pg[ir].N == -1) continue;

				pg[ir].vca1 = m_AtomDrawWrappers[pg[ir].CA1].CloneVector;
				pg[ir].vca2 = m_AtomDrawWrappers[pg[ir].CA2].CloneVector;
				pg[ir].center = Vector.CenterPointBetween( pg[ir].vca1, pg[ir].vca2 );
				pg[ir].forward = pg[ir].vca2 - pg[ir].vca1;
				vc = m_AtomDrawWrappers[pg[ir].C].CloneVector - m_AtomDrawWrappers[pg[ir].N].CloneVector;

				pg[ir].up = Vector.crossProduct( pg[ir].forward, vc );
				pg[ir].across = Vector.crossProduct( pg[ir].forward, pg[ir].up);

				pg[ir].forward.MakeUnitVector( out pg[ir].length ); // we want to store the original length
				pg[ir].across.MakeUnitVector();
				pg[ir].up.MakeUnitVector();

				// flatten out sheets by flipping the across vector when not smoothly joined
				// with previous one
				if(ir>0) // there is no previous one for the first molecule
				{
					float angle = (float) pg[ir].across.angleWith( pg[ir-1].across );
					if( angle < 0 )angle *= -1;
					if(angle > (Math.PI/2))
					{
						pg[ir].across *= -1;
						pg[ir].up     *= -1;
					}
				}

				shapetype[ir] = (ir/8)%2;
			}

			// Now create the extrusion

			ExtrudeShape e_shape = new ExtrudeShape();
	
			float t;
			for( int ir = 0 ; ir < moleculesForCartoon.Count - 2 - nullNextMolCount; ir++)
			{         // make splines - woohoo

				for( int r = 0; r < ribbonres; r++)
				{
					t = (float)r / (float)(ribbonres); // parameteric variable t

					// make the spline interpolations between the key points

					e_center.Interpolate_3(
						pg[ir].center, (float)Math.Pow( 1.0f-t , 2),
						pg[ir+1].vca1,  2.0f*(1.0f-t)*t,
						pg[ir+1].center, (float)Math.Pow( t, 2)
						);

					e_across.Interpolate_2(
						pg[ir].across,  (1.0f-t),
						pg[ir+1].across,  t
						);

					e_forward.Interpolate_2(
						pg[ir].forward,  (1.0f-t),
						pg[ir+1].forward, t
						);

					e_up.Interpolate_2(
						pg[ir].up,  (1.0f-t),
						pg[ir+1].up,  t);

					e_across.MakeUnitVector();              // make sure the direction vectors i,j & k are
					e_up.MakeUnitVector();                  // unit vectors
					e_forward.MakeUnitVector(); 

					rmat.setTo3V(e_across,  e_up, e_forward);  // create rotation matrix

					e_shape.setToInterpolation( 
						basicshape[shapetype[ir+1]],
						basicshape[shapetype[ir]  ],
						t
						);

					e_shape.rotate( rmat );  // apply rotation matrix to ratate it to the respective orientation
					e_shape.addVector( e_center );     // move coordinates to the center position

					for(int n = 0; n < e_shape.p.Length ; n++)
					{
						normal[vertices]= (Vector) e_shape.normal[n].Clone();     // use the rotated coordinates as normals
						normal[vertices].MakeUnitVector();                 // make sure they're unit vectors
						vertex[vertices] = (Vector) e_shape.p[n].Clone();     // also use the rotated coordinates as vertices
						vertices++;
					}

					if(ir!=0)
					{   // add faces

						for(int n = 0; n < e_shape.p.Length; n++)
						{
							face[faces].i = vertices - e_shape.p.Length*2     + n;
							face[faces].j = vertices - e_shape.p.Length       + (n + 1)%e_shape.p.Length;
							face[faces].k = vertices - e_shape.p.Length       + n;
							faces++;
							face[faces].i = vertices - e_shape.p.Length*2     + n;
							face[faces].j = vertices - e_shape.p.Length*2     + (n + 1)%e_shape.p.Length;
							face[faces].k = vertices - e_shape.p.Length       + (n + 1)%e_shape.p.Length;
							faces++;
						}
					}

				}


			}

			sm.Vertices = vertex;
			sm.Normals = normal;
			sm.Indexes = face;
			Colour c = Colour.FromName("Red");
			sm.Colours = new Colour[ vertex.Length ];
			for ( int i = 0; i < vertex.Length; i++ )
			{
				sm.Colours[i] = c;
			}
		}
     		
		public void UpdateCartoon()
		{
			int vertices = 0; // counter used later
			int faces = 0; // counter used later

			// find the peptide group atoms & calcualte the orientation vectors
			//m_AtomDrawWrappers;
			for( int ir = 0; ir < moleculesForCartoon.Count - 1 - nullNextMolCount; ir++ )
			{
				Molecule m = (Molecule) moleculesForCartoon[ir];

				pg[ir].vca1.setTo( m_AtomDrawWrappers[pg[ir].CA1] );
				pg[ir].vca2.setTo( m_AtomDrawWrappers[pg[ir].CA2] );
				pg[ir].center.SetToCentrePoint( pg[ir].vca1, pg[ir].vca2 );
				pg[ir].forward.SetToAMinusB( pg[ir].vca2, pg[ir].vca1 );
				vc = m_AtomDrawWrappers[pg[ir].C].CloneVector - m_AtomDrawWrappers[pg[ir].N].CloneVector;

				pg[ir].up = Vector.crossProduct( pg[ir].forward, vc );
				pg[ir].across = Vector.crossProduct( pg[ir].forward, pg[ir].up);

				pg[ir].forward.MakeUnitVector( out pg[ir].length ); // we want to store the original length
				pg[ir].across.MakeUnitVector();
				pg[ir].up.MakeUnitVector();

				// flatten out sheets by flipping the across vector when not smoothly joined
				// with previous one
				if(ir>0) // there is no previous one for the first molecule
				{
					float angle = (float) pg[ir].across.angleWith( pg[ir-1].across );
					if( angle < 0 )angle *= -1;
					if(angle > (Math.PI/2))
					{
						pg[ir].across *= -1;
						pg[ir].up     *= -1;
					}
				}

				shapetype[ir] = (ir/8)%2;
			}

			// Now create the extrusion


			ExtrudeShape e_shape = new ExtrudeShape();

			float t;
			for( int ir = 0 ; ir < moleculesForCartoon.Count - 2 - nullNextMolCount; ir++)
			{         // make splines - woohoo

				for( int r = 0; r < ribbonres; r++)
				{
					t = (float)r / (float)(ribbonres); // parameteric variable t

					// make the spline interpolations between the key points

					e_center.Interpolate_3(
						pg[ir].center, (float)Math.Pow( 1.0f-t , 2),
						pg[ir+1].vca1,  2.0f*(1.0f-t)*t,
						pg[ir+1].center, (float)Math.Pow( t, 2)
						);

					e_across.Interpolate_2(
						pg[ir].across,  (1.0f-t),
						pg[ir+1].across,  t
						);

					e_forward.Interpolate_2(
						pg[ir].forward,  (1.0f-t),
						pg[ir+1].forward, t
						);

					e_up.Interpolate_2(
						pg[ir].up,  (1.0f-t),
						pg[ir+1].up,  t);

					e_across.MakeUnitVector();              // make sure the direction vectors i,j & k are
					e_up.MakeUnitVector();                  // unit vectors
					e_forward.MakeUnitVector(); 

					rmat.setTo3V(e_across,  e_up, e_forward);  // create rotation matrix

					e_shape.setToInterpolation( 
						basicshape[shapetype[ir+1]],
						basicshape[shapetype[ir]  ],
						t
						);

					e_shape.rotate( rmat );  // apply rotation matrix to ratate it to the respective orientation
					e_shape.addVector( e_center );     // move coordinates to the center position

					for(int n = 0; n < e_shape.p.Length ; n++)
					{
						sm.Normals[vertices] = (Vector) e_shape.normal[n].Clone();     // use the rotated coordinates as normals
						sm.Normals[vertices].MakeUnitVector();                 // make sure they're unit vectors
						sm.Vertices[vertices] = (Vector) e_shape.p[n].Clone();     // also use the rotated coordinates as vertices
						vertices++;
					}

					if(ir!=0)
					{   // add faces

						for(int n = 0; n < e_shape.p.Length; n++)
						{
							sm.Indexes[faces].i = vertices - e_shape.p.Length*2     + n;
							sm.Indexes[faces].j = vertices - e_shape.p.Length       + (n + 1)%e_shape.p.Length;
							sm.Indexes[faces].k = vertices - e_shape.p.Length       + n;
							faces++;
							sm.Indexes[faces].i = vertices - e_shape.p.Length*2     + n;
							sm.Indexes[faces].j = vertices - e_shape.p.Length*2     + (n + 1)%e_shape.p.Length;
							sm.Indexes[faces].k = vertices - e_shape.p.Length       + (n + 1)%e_shape.p.Length;
							faces++;
						}
					}

				}


			}

			Colour c = Colour.FromName("Red");
			sm.Colours = new Colour[ sm.Vertices.Length ];
			for ( int i = 0; i < sm.Vertices.Length; i++ )
			{
				sm.Colours[i] = c;
			}
		}

		public bool GetSelectionLockState()
		{
			return m_SelectionLock;
		}

		public void SetSelectionLock( object masterObject, bool lockValue )
		{
			if( lockValue )
			{
				if( m_SelectionLock )
				{
					throw new Exception("The selection list is already locked, it must be released before a different control can obtain a lock!");
				}
				else
				{
					m_LockHolder = masterObject;
					m_SelectionLock = true;
					SelectionUpdated();
				}
			}
			else
			{
				if( m_LockHolder == null || m_SelectionLock == false )
				{
					throw new Exception("The selection list isnt locked!");
				}
				else
				{
					if( m_LockHolder != masterObject )
					{
						throw new Exception("a lock can only be released by the object that created it!");
					}
					else
					{
						m_LockHolder = null;
						m_SelectionLock = false;
						SelectionUpdated();
					}
				}
			}
		}
		public int SelectionCount 
		{
			get
			{
				return m_Selections.Count;
			}
		}

		public Selection getSelection( int index )
		{
			if( index >= 0 && index < m_Selections.Count )
			{
                return (Selection) m_Selections[index];
			}
			else
			{
				return null;
			}
		}

		public Selection CurrentSelection
		{
			get
			{
				return (Selection) m_Selections[m_CurrentSelectionIndex];
			}
		}

		public Selection SelectionAt( int index )
		{
			return (Selection) m_Selections[ index ];
		}

		public Selection[] Selections
		{
			get
			{
				return (Selection[]) m_Selections.ToArray(typeof(Selection));
			}
		}

		public int AtomWraperCount
		{
			get
			{
				return m_AtomDrawWrappers.Length;
			}
		}
        

		public ParticleSystem particleSystem
		{
			get
			{
				return m_PS;
			}
			set
			{
				//Debug.WriteLine("PS field set fired");

				if ( m_PS != null ) 
				{ 
					m_PS.ContentUpdate -= m_ContentUpdate;
					m_PS.PositionsUpdate -= m_PositionsUpdate;
					m_PS.SystemFocus.FocusUpdate -= m_FocusUpdate;
				}

				m_PS = value;
				if ( m_PS != null )
				{
					m_Viewer.perspective.RotationOffset = m_PS.SystemFocus;
				}
				
				onContentUpdate(); // SelectionUpdated(); is called by this
				
				// now all is setup, resubscribe to the events
				if ( m_PS != null )
				{
					m_PS.ContentUpdate += m_ContentUpdate;
					m_PS.PositionsUpdate += m_PositionsUpdate;
					m_PS.SystemFocus.FocusUpdate += m_FocusUpdate;			
				}
			}
		}

		private void getAtomList() // occurs only on init and on PS content update
		{
			lock ( WriteLock )
			{
				//Debug.WriteLine("getAtomList fired");

				bool done = false;
				while(!done)
				{  
					try
					{
						m_PS.AcquireReaderLock(1000);
						try
						{

							// get the terminal cAlphas
							endPoints = new ArrayList();
							for ( int q = 0; q < m_PS.Members.Length; q++ )
							{
								if( m_PS.Members[q].GetType() == typeof(PolyPeptide) )
								{
									PolyPeptide pp = (PolyPeptide) m_PS.Members[q];
									AminoAcid aa = (AminoAcid) pp[ pp.Count-1 ];
							
									for ( int s = 0; s < aa.Count; s++ )
									{
										if( aa[s].PDBType == PDBAtom.PDBID_BackBoneCA )
										{
											endPoints.Add( m_PS.IndexOf( aa[s] ) );
											break;
										}
									}
								}
							}

							m_AtomDrawWrappers = new AtomDrawWrapper[ m_PS.Count ];
							
							for ( int i = 0; i < m_PS.Count; i++ )
							{
								m_AtomDrawWrappers[i] = new AtomDrawWrapper( m_PS[i], m_Perspective.RotationOffset );
							}

							hb.setupForAtoms( m_AtomDrawWrappers );
						}
						finally
						{
							// Ensure that the lock is released.
							m_PS.ReleaseReaderLock();
							done = true;
						}
					}
					catch (ApplicationException)
					{
						// The reader lock request timed out.
					}
				}
			}
		}

		#region Extended Drawers Implementation

		public event UpdateEvent DrawExtrasUpdate;
		private bool m_EnableHBonding = true;
		private bool m_EnableExtLines = false;
		private bool m_EnableForceVectors = false;
		private bool m_HasForceVectors = false;
		private bool m_HasExtendedVectors = false;

		public bool HasForceVectors
		{
			get
			{
				return m_HasForceVectors;
			}
		}

		public bool HasExtendedVectors
		{
			get
			{
				return m_HasExtendedVectors;
			}
		}

		public void SetupExtendedVectors( Line3DStore lineStore )
		{
			if( lineStore != null )
			{
				m_HasExtendedVectors = true;
				if( el == null )
				{
					el = new LineStoreRender( m_Viewer );
				}
				el.Setlist( lineStore );
				EnableExtLines = true;
			}
			else
			{
				m_HasExtendedVectors = false;
				m_EnableExtLines = false;
				m_Viewer.RemoveRenderObject( el );
				DrawExtrasUpdate();
			}
		}

		public void SetupForceVectors( PositionStore forceVectors )
		{
			if( forceVectors != null )
			{
				m_HasForceVectors = true;
				if( fv == null )
				{
					fv = new ForceVectors( m_Viewer );
				}
				fv.setupForAtoms( m_AtomDrawWrappers, forceVectors );
				EnableForceVectors = true; // adds render object and calls update event
			}
			else
			{
				m_HasForceVectors = false;
				m_EnableForceVectors = false;
				m_Viewer.RemoveRenderObject( fv );
				DrawExtrasUpdate();
			}
		}
		
		public bool EnableForceVectors
		{
			get
			{
				return m_EnableForceVectors;
			}
			set
			{
				if( value == m_EnableForceVectors && m_HasForceVectors )
				{
					return;
				}
				else
				{
					m_EnableForceVectors = value;
					if( m_EnableForceVectors )
					{
						m_Viewer.AddRenderObject( fv );
					}
					else
					{ 
						m_Viewer.RemoveRenderObject( fv );
					}
					DrawExtrasUpdate();
				}
			}
		}

		public bool EnableHBonding
		{
			get
			{
				return m_EnableHBonding;
			}
			set
			{
				if( value == m_EnableHBonding )
				{
					return;
				}
				else
				{
					m_EnableHBonding = value;
					if( m_EnableHBonding )
					{
						m_Viewer.AddRenderObject( hb );
					}
					else
					{ 
						m_Viewer.RemoveRenderObject( hb );
					}
					DrawExtrasUpdate();
				}
			}
		}

		public bool EnableExtLines
		{
			get
			{
				return m_EnableExtLines;
			}
			set
			{
				if( value == m_EnableExtLines && m_HasExtendedVectors )
				{
					return;
				}
				else
				{
					m_EnableExtLines = value;
					if( m_EnableExtLines )
					{
						m_Viewer.AddRenderObject( el );
					}
					else
					{ 
						m_Viewer.RemoveRenderObject( el );
					}
					DrawExtrasUpdate();
				}
			}
		}

		#endregion

		#region Selection Mode Implementation

		private bool m_SelectionEditing = false;
		public event UpdateEvent SelectionUpdated = new UpdateEvent(nullFunc);

		public AtomDrawStyle GlobalDrawStyle
		{
			get
			{
				Selection s = (Selection)m_Selections[0];
				return s.DrawStyle;
			}
			set
			{
				Selection s = (Selection)m_Selections[0];
				s.DrawStyle = value;
			}
		}

		public AtomDisplayMode GlobalDisplayMode
		{
			get
			{
				Selection s = (Selection)m_Selections[0];
				return s.DisplayMode;
			}
			set
			{
				Selection s = (Selection)m_Selections[0];
				s.DisplayMode = value;
			}
		}

		private static void nullFunc()
		{
		}

		private int m_CurrentSelectionIndex = 0;
		public int CurrentSelectionIndex
		{
			get
			{
				return m_CurrentSelectionIndex;
			}
			set
			{
				if( value >= 0 && value < m_Selections.Count )
				{
					m_CurrentSelectionIndex = value;
				}
				if( m_EasyViewMode )
				{
                    EasyViewMode = true; // resets which chain we are looking at
				}
			}
		}

		private bool m_EasyViewMode = false;
		public bool EasyViewMode
		{
			get
			{
				return m_EasyViewMode;
			}
			set
			{
				BeginSelectionEdit();
				m_EasyViewMode = value;
				for( int i = 0; i < m_Selections.Count; i++ )
				{
					Selection s = (Selection)m_Selections[i];
					s.EasyViewModeOn = value;
					s.IsEasyViewFocus = false;
				}
				Selection sCurrent = (Selection)m_Selections[m_CurrentSelectionIndex];
				sCurrent.IsEasyViewFocus = true; // doesnt matter if we always set this even if value == false
				EndSelectionEdit();// need to set colours and such
			}
		}

		public bool CurrentSelectionEnabledState
		{
			get
			{
				Selection s = (Selection)m_Selections[m_CurrentSelectionIndex];
				return s.IsActive;
			}
			set
			{
				Selection s = (Selection)m_Selections[m_CurrentSelectionIndex];
				s.IsActive = value;
			}
		}

		public void BeginSelectionEdit()
		{
			if( m_SelectionEditing )
			{
				throw new Exception("Editing is already in progress");
			}
			m_SelectionEditing = true;
		}

		public void PromoteCurrentSelection()
		{
			if( m_CurrentSelectionIndex == 0 || m_CurrentSelectionIndex == m_Selections.Count -1 ) // cant promote
			{
				return;
			}
			Selection sPromote = (Selection) m_Selections[ m_CurrentSelectionIndex ];
			Selection sDemotee = (Selection) m_Selections[ m_CurrentSelectionIndex + 1 ];
			m_Selections[ m_CurrentSelectionIndex + 1 ] = sPromote;
            m_Selections[ m_CurrentSelectionIndex ] = sDemotee;
            SelectionUpdated();
		}

		public void DemoteCurrentSelection()
		{
			if( m_CurrentSelectionIndex == 0 || m_CurrentSelectionIndex == 1 ) // cant demote
			{
				return;
			}
			Selection sDermote = (Selection) m_Selections[ m_CurrentSelectionIndex - 1 ];
			Selection sPromotee = (Selection) m_Selections[ m_CurrentSelectionIndex ];
			m_Selections[ m_CurrentSelectionIndex ] = sPromotee;
			m_Selections[ m_CurrentSelectionIndex - 1 ] = sDermote;  
			SelectionUpdated();
		}

		public void EndSelectionEdit()
		{
			if( !m_SelectionEditing )
			{
				throw new Exception("Editing mode is not in progress, cannot be ended");
			}
			m_SelectionEditing = false;

			// update all the drawWrappers

			int[] DWSelections = new int[ m_AtomDrawWrappers.Length ]; // initialised to 0's therefore initialised to point to the global selection
			//if there are further active selections, then set their indexes in DWSelections

			GlobalSelection glo = (GlobalSelection) m_Selections[0]; // do 1st, no need to set indeces, they are the default
			glo.BeginParameterGet();

			for( int i = 1; i < m_Selections.Count; i++ )
			{
				Selection s = (Selection) m_Selections[i];
				if( s.IsActive )
				{
					s.BeginParameterGet();
					int[] atomIndexes = s.AtomIndexes;
					for( int j = 0; j < atomIndexes.Length; j++ )
					{
						DWSelections[ atomIndexes[j] ] = i;
					}
				}
			}

			for( int i = 0; i < DWSelections.Length; i++ )
			{
				Selection s = (Selection) m_Selections[ DWSelections[i] ];
				m_AtomDrawWrappers[i].DisplayMode = s.DisplayMode;
				m_AtomDrawWrappers[i].DrawStyle = s.DrawStyle;
				s.SetColour( m_AtomDrawWrappers[i], i ); // Selection knows how many it will have and increments
			}	
			
			SetUpRenderList();
			SelectionUpdated();
		}

		public Selection AddSelection( int mol1Index, int mol2Index, int[] equivs )
		{
			Selection s = new Selection_CAlphaEquiv( m_PS.MemberAt(mol1Index), m_PS.MemberAt(mol2Index), equivs );
			m_Selections.Add( s );
			m_CurrentSelectionIndex = m_Selections.Count - 1;
			return s;
		}

		public Selection AddSelection( Selection s )
		{
			m_Selections.Add( s );
			m_CurrentSelectionIndex = m_Selections.Count - 1;
			return s;
		}

		public Selection AddSelection( ModelList models, int position )
		{
			Selection s = new Selection_CAlphaEquiv( models.Mol1, models.Mol2, models[position].Equivalencies );
			m_Selections.Add( s );
			m_CurrentSelectionIndex = m_Selections.Count - 1;
			return s;
		}
		
		public Selection AddSelection( PSMolContainer mol, int startMolIndex, int length )
		{
			Selection s = new AminoAcidSelection( mol, startMolIndex, length );
			m_Selections.Add( s );
			m_CurrentSelectionIndex = m_Selections.Count - 1;
			return s;
		}

		public void CopyCurrentSelection()
		{
			if( CurrentSelection is GlobalSelection )
			{
				return;
			}
			AminoAcidSelection s = (AminoAcidSelection) CurrentSelection;
			AminoAcidSelection newS = new AminoAcidSelection( s.Molecule, s.Start, s.Length );
			m_Selections.Add( newS );
			int requiredIndex = CurrentSelectionIndex + 1;
			if( requiredIndex == m_Selections.Count - 1 )
			{
				return; // its already in the right place
			}
			else
			{
				// we have to move the bastard to the required position in the array
				// newS is at the last position in the array
				// move everything else up one
				for( int moveIndexCount = m_Selections.Count - 2; moveIndexCount >= requiredIndex; moveIndexCount-- )
				{
					Selection selMove = (Selection) m_Selections[moveIndexCount];
					m_Selections[moveIndexCount+1] = selMove;
				}
				m_Selections[requiredIndex] = newS;
			}
		}

		public void RemoveCurrentSelection()
		{
			Selection s = (Selection) m_Selections[m_CurrentSelectionIndex];
			if( s is GlobalSelection )
			{
				return;
			}
			m_Selections.RemoveAt( m_CurrentSelectionIndex );
			m_CurrentSelectionIndex--;

			// NOOO
			//SetUpRenderList();
			//SelectionUpdated();
			// EndSelectionEdit() should be called after selection addition
		}

		public void ClearSlections()
		{
			m_CurrentSelectionIndex = 0;
            m_Selections.RemoveRange(1,m_Selections.Count-1);
		}


		#endregion
	}
}
