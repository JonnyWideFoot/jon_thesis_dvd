using System;
using System.Collections;

using UoB.Core.Primitives;
using UoB.Core.Structure;

namespace UoB.Core.Structure.Detection
{
	/// <summary>
	/// Summary description for IGDetector.
	/// </summary>
	public class IGDetector : Detector
	{
		internal class GeoCenter
		{
			private Position newCenter = new Position( 0, 0, 0 );
			private Position oldCenter = new Position( 0, 0, 0 );
			public MoleculeList AminoAcids = new MoleculeList();

			public GeoCenter( Position p )
			{
                newCenter = p;
			}

			public Position Center
			{
				get
				{
					return newCenter;
				}
			}

			public void ReassignCenter()
			{

				oldCenter = newCenter;

                newCenter = new Position( 0, 0, 0 ); 		

				for( int i = 0; i < AminoAcids.Count; i++ )
				{
					newCenter += AminoAcids[i].GetGeometricCenter(true);
				}

				newCenter /= AminoAcids.Count;
				               
			}

			public AminoAcid FurthestMember( out float distanceToMember )
			{
				AminoAcid furthestAA = null;
				distanceToMember = 0.0f;
				for( int i = 0; i < AminoAcids.Count; i++ )
				{
					float distance = Position.distanceBetween( AminoAcids[i].GetGeometricCenter(true), newCenter );
					if( distanceToMember < distance )
					{
						furthestAA = (AminoAcid) AminoAcids[i];
						distanceToMember = distance;
					}
				}
				return furthestAA;
			}

			public void ClearList()
			{
				AminoAcids.Clear();
			}

			public bool CenterHasChanged
			{
				get
				{
					return (
						( newCenter.xFloat != oldCenter.xFloat ) ||
						( newCenter.yFloat != oldCenter.yFloat ) ||
						( newCenter.zFloat != oldCenter.zFloat )  );				
				}
			}

			public bool OutsideThreshold
			{
				get
				{
					float cutoff = 25.0f;
					//float cutoff = 30.0f; - too big for some domains
					int invalidCount = 0;
					float debugFurthestDistance = 0.0f;
					for( int i = 0; i < AminoAcids.Count; i++ )
					{
						float distance = Position.distanceBetween( AminoAcids[i].GetGeometricCenter(true), newCenter );
						if( cutoff < distance )
						{
							invalidCount++;
						}
						if( debugFurthestDistance < distance )
						{
							debugFurthestDistance = distance;
						}
					}
					if( invalidCount > 10 )
					{
						return true;
					}				
					else
					{
						return false;
					}
				}
			}
		
		}

		public IGDetector( PolyPeptide p ) : base( p )
		{
            
		}

		private ArrayList GeoCenters;

		private void ClearGeoCenterLists()
		{
			for( int j = 0; j < GeoCenters.Count; j++ )
			{
				((GeoCenter)GeoCenters[j]).ClearList();
			}
		}

		private void ReassignAllGeoCenters()
		{
			for( int j = 0; j < GeoCenters.Count; j++ )
			{
				((GeoCenter)GeoCenters[j]).ReassignCenter();
			}
		}

		private void AssignCenters( int startIndex, int length )
		{
			ClearGeoCenterLists();

			for( int i = 0; i < length; i++ )
			{
				AminoAcid aai = (AminoAcid) m_Polymer[i+startIndex];
				int geoID = -1;
				float bestGeoDistance = float.MaxValue;
				for( int j = 0; j < GeoCenters.Count; j++ )
				{
					float distance = Vector.distanceBetween( ((GeoCenter)GeoCenters[j]).Center, aai.GetGeometricCenter(true) );
					if( distance < bestGeoDistance )
					{
						bestGeoDistance = distance;
						geoID = j;  
					}
						                      
				}
				((GeoCenter)GeoCenters[geoID]).AminoAcids.addMolecule( aai );
			}

            ReassignAllGeoCenters();
		}

		private void findCenters( int startIndex, int length )
		{
			GeoCenters = new ArrayList();
			GeoCenters.Add( new GeoCenter( new Position(0,0,0) ) ); // we want at least one to start
			
			bool notDone = true;
			do
			{
				// for each amino acid, assign it to its nearest geometric center
				// then move it to the the center of all the positions and reassign 
				// until no change occurs
				while(true)
				{
					AssignCenters( startIndex, length );
					bool changed = false;
					for( int j = 0; j < GeoCenters.Count; j++ )
					{
						changed = ((GeoCenter)GeoCenters[j]).CenterHasChanged;
						if( changed ) break;
					}
					if( !changed ) break; // we have found the best possible distribution
				}
				// each geocentre now has all the amino acids that are closest to it
				// these may not be totally correct as some loops could be closer to the wrong
				// center. We must therefore iterate the sequence and assign correctly

				int[] geoID = new int[length];
				for( int i = 0; i < length; i++ )
				{
					AminoAcid aai = (AminoAcid) m_Polymer[startIndex + i];
					float bestGeoDistance = float.MaxValue;
					int bestgeoID = -1;
					for( int j = 0; j < GeoCenters.Count; j++ )
					{
						float distance = Vector.distanceBetween( ((GeoCenter)GeoCenters[j]).Center, aai.GetGeometricCenter(true) );
						if( distance < bestGeoDistance )
						{
							bestGeoDistance = distance;
							bestgeoID = j;  
						}						                      
					}
					geoID[i] = bestgeoID;
				}


				// Now, are we actually in a new domain, or are we in a weird loop that is nearest the
				// wrong center. We must make sure all stretches are assigned to the same geocenter.

				// the algorythum below doesnt work properly
				// a better way would be to look for stretches of 50 AA or more, and assign the rest to the nearest in sequence



				const int readAhead = 15;
                int lastGeoID = geoID[0];
				for( int i = 1; i < length; i++ )
				{
					if( geoID[i] != lastGeoID )
					{
						// then readAhead to see if the next few are, if not, these are just in loop
						int readTo = (i + readAhead);
						if( readTo > length )
						{
							//assign the rest to the last ID, nothing else makes sense, the rest must belong to the last domain
							geoID[i] = lastGeoID;
						}
						else // could possibly be a loop
						{
							bool resetOccured = false;
							for( int j = i + 1; j < readTo; j++ )
							{
								if ( geoID[i] != geoID[j] )
								{
									// we are in a loop, one of the read aheads has the same ID as the previous geogroup !
									geoID[i] = geoID[j];
									resetOccured = true;
									break;
								}
							}
							if( !resetOccured )
							{
								lastGeoID = geoID[i];
							}
						}
					}
				}	
			

				// now assign the constant regions to their proper geocenter
				ClearGeoCenterLists();
				for( int j = 0; j < geoID.Length; j++ )
				{
					((GeoCenter)GeoCenters[ geoID[j] ]).AminoAcids.addMolecule( (AminoAcid) m_Polymer[startIndex + j] );
				}
				ReassignAllGeoCenters();


				// now if all centers are within a given threshold then we quit, else 
				// we make a new center at the most distant c-Alpha and do the process again
				for( int j = 0; j < GeoCenters.Count; j++ )
				{
					notDone = ((GeoCenter)GeoCenters[j]).OutsideThreshold;
					if( notDone ) break;
					//Position.DebugReport( ((GeoCenter)GeoCenters[j]).Center, 'G' );
				}

				

				if( notDone ) // make a new geocenter
				{				
					float furthestDistance = 0.0f;
					AminoAcid aaToUSe = null;
					int debugIndex = -1;
					for( int j = 0; j < GeoCenters.Count; j++ )
					{
						float distance;
						AminoAcid aa = ((GeoCenter)GeoCenters[j]).FurthestMember( out distance );
						if( distance > furthestDistance )
						{
							furthestDistance = distance;
							aaToUSe = aa;
							debugIndex = j;
						}
					}
					GeoCenter g = new GeoCenter( aaToUSe.GetGeometricCenter(true) );
					GeoCenters.Add( g );
				}
			}
			while( notDone );
		}

		public override MatchState GetMatchState()
		{
			return MatchState.None;
		}

		public override DetectionMatch[] Matches()
		{
			return Matches(0,m_Polymer.Count);
		}

		public override DetectionMatch[] Matches( int AAstartIndex, int length )
		{
			findCenters( AAstartIndex, length );
			// we now have GeoCenters corresponding to the domains and any tails present
			// we dont want the tails, and therefore we need to trim them

			DetectionMatch[] detectionMatches = new DetectionMatch[ GeoCenters.Count ];
			
			for( int i = 0; i < GeoCenters.Count; i++ )
			{
				GeoCenter g = (GeoCenter) GeoCenters[i];

				// we need to find any amino acids that do not have neighbours that are outside their local chain
				// i.e. any "tails"

				const float sheetDistanceThreshold = 5.5f;
				const float coreDistanceThreshold = 5.5f;
				ArrayList notValids = new ArrayList();

				for( int j = 0; j < g.AminoAcids.Count; j++ )
				{
					bool hasNonLocalPartner = false;
					for( int k = 0; k < g.AminoAcids.Count; k++ )
					{
						if( k == j ) continue;
						int indexGap = k - j;
						if( indexGap < 0 )
						{
							indexGap = -indexGap;
						}
						if( indexGap < 5 )
						{
							continue;
						}

						// to look at the local hydrogen bonding, this will apply to beta sheets ...
						float distanceNO = Position.distanceBetween( g.AminoAcids[k].AtomOfType(" O  "), g.AminoAcids[j].AtomOfType(" N  ") );
						float distanceON = Position.distanceBetween( g.AminoAcids[k].AtomOfType(" N  "), g.AminoAcids[j].AtomOfType(" O  ") );
						// for residues not in a beta sheet, but still very close to the core domain
						float distanceCC = Position.distanceBetween( g.AminoAcids[k].AtomOfType(" N  "), g.AminoAcids[j].AtomOfType(" O  ") );
						
						if( (distanceNO < sheetDistanceThreshold &&  distanceON < sheetDistanceThreshold) ||  distanceCC < coreDistanceThreshold )
						{
							hasNonLocalPartner = true;
							break;
						}
					}
					if( !hasNonLocalPartner )
					{
						notValids.Add( j );
					}
				}

				// we now have any non valids - i.e. residues in extensions away from the main domain
				// however, we only want to discount the tails ...
				int startIndex = 0;
				int endIndex = g.AminoAcids.Count -1;
				for( int h = 0; h < notValids.Count; h++ )
				{
					int notValid = (int) notValids[h];
					if( startIndex == notValid )
					{
						startIndex++;
					}
					else
					{
						break;
					}
				}
				for( int h = notValids.Count -1; h >= 0; h-- )
				{
					int notValid = (int) notValids[h];
					if( endIndex == notValid )
					{
						endIndex--;
					}
					else
					{
						break;
					}
				}
				// the tails have now been defined, so we can create the match classes
				// first we need to relate the indexes of the GeoCenter to the indexes
				// of the polymer ...

				int indexOfPPStart = m_Polymer.IndexOf( g.AminoAcids[startIndex] );
				int indexOfPPEnd = m_Polymer.IndexOf( g.AminoAcids[endIndex] );

				detectionMatches[i] = new DetectionMatch( 
					m_Polymer,
					indexOfPPStart,
					'~',
					indexOfPPEnd, 
					g.Center
					);
			}
			return detectionMatches;
		}
	}
}
