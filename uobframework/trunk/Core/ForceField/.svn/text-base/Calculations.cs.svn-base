using System;
using System.IO;
using System.Collections.Generic;
using UoB.Core.Primitives;
using UoB.Core.Structure;
using UoB.Core.ForceField;
using UoB.Core.FileIO.DSSP;

namespace UoB.Core.ForceField
{
    public class LoopAnalyticalSASA : Calculations
	{
		private PolyPeptide m_Poly;
        private SegmentDef m_SegDef;
        private int m_N = -1;
		
		public LoopAnalyticalSASA() : base()
		{
            totalFraction = 0.0;
            totalSASA = 0.0;
            totalMaxSASA = 0.0;
            fractSASAs = new List<double>();
            SASAs = new List<double>();
            maxSASAs = new List<double>();
            atomicFractions = new List<double>();
            atomicSASAs = new List<double>();
            atomicMaxSASAs = new List<double>();
		}
		
		public void connectLoop( PolyPeptide poly, SegmentDef def )
		{
            int n = poly.Atoms.Count;
			m_AtomList = poly.Atoms;
			m_Poly = poly;
			m_SegDef = def;
			if ( m_AtomList == null ) throw new Exception();
            if (n > m_N)
            {
                // grow memory store
                distanceMatrix = new float[n, n]; // absolute max
                m_N = n;
            }
		}

        public void calcSquareDistanceMatrix(AminoAcid amino)
        {
            int n = m_AtomList.Count;
            for (int i = 0; i < amino.Count; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    float distance = Atom.distanceSquaredBetween(amino[i], m_AtomList[j]);
                    distanceMatrix[i, j] = distance;
                    distanceMatrix[j, i] = distance;
                }
            }
        }

        public void calcDistanceMatrix( AminoAcid amino )
        {
            int n = m_AtomList.Count;
            for (int i = 0; i < amino.Count; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    float distance = Atom.distanceBetween(amino[i], m_AtomList[j]);
                    distanceMatrix[i, j] = distance;
                    distanceMatrix[j, i] = distance;
                }
            }
        }

        public double totalFraction;
        public double totalSASA;
        public double totalMaxSASA;
        public List<double> fractSASAs;
        public List<double> SASAs;
        public List<double> maxSASAs;
        public bool keepAtomics = false;
        public List<double> atomicFractions;
        public List<double> atomicSASAs;
        public List<double> atomicMaxSASAs;

        private void reinitContainers()
        {
            // reinit containers
            totalFraction = 0.0;
            totalSASA = 0.0;
            totalMaxSASA = 0.0;
            fractSASAs.Clear();
            SASAs.Clear();
            maxSASAs.Clear();
            atomicFractions.Clear();
            atomicSASAs.Clear();
            atomicMaxSASAs.Clear();
        }

        public void calcLoopSASAInfo()
		{
            reinitContainers();        

			int n = m_AtomList.Count;		
			Vector atomSphereRepresentation = new Vector();
            double twoWater = 2 * waterRadius;

            for (int a = 0; a < m_SegDef.Length; a++)
            {
                int index = m_SegDef.FirstDSSPIndex - 1 + a;
                AminoAcid amino = m_Poly[index];
                calcSquareDistanceMatrix(amino);

                double aminoSASA = 0.0;
                double aminoMaxSASA = 0.0;

                for (int i = 0; i < amino.Count; i++)
                {
                    Atom AtomI = amino[i];
                    float radiusIandProbe = AtomI.Radius + waterRadius;
                    int validPoints = numSpherePoints;

                    for (int p = 0; p < standardSphere.Length; p++)
                    {
                        Vector theSpherePoint = standardSphere[p];
                        atomSphereRepresentation.setTo(
                            (theSpherePoint.xFloat * radiusIandProbe) + AtomI.xFloat,
                            (theSpherePoint.yFloat * radiusIandProbe) + AtomI.yFloat,
                            (theSpherePoint.zFloat * radiusIandProbe) + AtomI.zFloat
                            );

                        bool valid = true;
                        for (int j = 0; j < n; j++)
                        {
                            Atom AtomJ = m_AtomList[j];
                            if (AtomJ == AtomI) continue;
                            double radTerm = (AtomI.Radius + AtomJ.Radius + twoWater);
                            radTerm *= radTerm; // we are comparing with distance squared
                            if (distanceMatrix[i, j] > radTerm) continue;
                            
                            // if not, see if it occludes the point of interest
                            radTerm = AtomJ.Radius + waterRadius;
                            radTerm *= radTerm;
                            if (
                                Atom.distanceSquaredBetween(AtomJ, atomSphereRepresentation) 
                                < radTerm)
                            {
                                valid = false;
                                break; // if point isnt valid, why bother checking any more AtomJ's ..
                            }
                        }
                        if (valid == false) validPoints--;
                    }

                    // Atom SASA: What fraction of points are valid?
                    double fraction = ((float)validPoints / (float)numSpherePoints);
                    double max = radiusIandProbe * radiusIandProbe * 4.0 * (float)Math.PI;
                    double sasa = fraction * max;

                    if (keepAtomics)
                    {
                        // report all atoms
                        atomicFractions.Add(fraction);
                        atomicSASAs.Add(sasa);
                        atomicMaxSASAs.Add(max);
                    }

                    // add to AA totals
                    aminoSASA += sasa;
                    aminoMaxSASA += max;
                }

                fractSASAs.Add( aminoSASA / aminoMaxSASA );
                SASAs.Add( aminoSASA );
                maxSASAs.Add( aminoMaxSASA );

                totalSASA += aminoSASA;
                totalMaxSASA += aminoMaxSASA;
            }

            totalFraction = totalSASA / totalMaxSASA;
		
			return;			
		}
	}

	/// <summary>
	/// Summary description for ForceFieldClass.
	/// </summary>
	public class Calculations
	{
		public static Vector[] standardSphere;
		private bool m_SphereLoaded = false;
		public float[,] distanceMatrix;
		public const float waterRadius = 1.4f;
        protected const int numSpherePoints = 282; // the number in the sphere dat file now

		protected AtomList m_AtomList;

		public Calculations( AtomList atoms )
		{
			commonInit();
			connectToAtomList( atoms );
		}

		public Calculations( )
		{
			commonInit();
		}

		private void commonInit()
		{
			if( !m_SphereLoaded )
			{
				loadStandardSphere();
			}
		}

		public void connectToAtomList(AtomList atomL)
		{
			
			m_AtomList = atomL;
			if ( atomL == null ) return;
			calcDistanceMatrix();
		}

		public void loadStandardSphere()
		{
			if ( !m_SphereLoaded )
			{
				m_SphereLoaded = true;

				standardSphere = new Vector[numSpherePoints];

				string fullPathName = UoB.Core.CoreIni.Instance.DefaultSharedPath + "sphere.282.csv";
				StreamReader re = File.OpenText(fullPathName);
				string inputLine = null;

				int spherePointsSoFar = 0;
			
				while ((inputLine = re.ReadLine()) != null && spherePointsSoFar < numSpherePoints)
				{
					string[] position = inputLine.Split(',');
					Vector thePoint = new Vector(
						float.Parse(position[0]),
						float.Parse(position[1]),
						float.Parse(position[2])
						);
					standardSphere[spherePointsSoFar] = thePoint;
					spherePointsSoFar++;
				}

				if ( spherePointsSoFar != numSpherePoints ) throw new Exception("Sphere is incomplete");

				re.Close();
			}
		}

		public void calcDistanceMatrix()
		{
			int n = m_AtomList.Count;
			distanceMatrix = new float[n,n];

			for ( int i = 0 ; i < n; i++ )
			{
				for ( int j = i + 1; j < n; j++ )
				{
					float distance = Atom.distanceBetween(m_AtomList[i], m_AtomList[j]);
					distanceMatrix[i,j] = distance;
					distanceMatrix[j,i] = distance;
				}                
			}
        }


		public float[] calcSurface(SASAParamList SASAParam)
		{
			float probeRadius = 0.0f;

			if (SASAParam == SASAParamList.areaSASA || 
				SASAParam == SASAParamList.areaSASAModifiedVDW ||
				SASAParam == SASAParamList.fractionSASA
				)
			{
				probeRadius = waterRadius;
			}

			int n = m_AtomList.Count;
			Atom AtomI;
			Atom AtomJ;
			float distJP;

			int valid = 1;
			int validPoints = numSpherePoints;

			float currentRadiusI;

			float[] returnValues = new float[n];
			float[] fractions = new float[n];

			for ( int i = 0 ; i < n; i++ )
			{
				AtomI = m_AtomList[i];
				currentRadiusI = AtomI.Radius + probeRadius;

				for ( int p = 0; p < standardSphere.Length; p++)
				{
					Vector theSpherePoint = standardSphere[p];
					Vector atomSphereRepresentation = new Vector(
						(theSpherePoint.xFloat * (AtomI.Radius + probeRadius)) + AtomI.xFloat,
						(theSpherePoint.yFloat * (AtomI.Radius + probeRadius)) + AtomI.yFloat,
						(theSpherePoint.zFloat * (AtomI.Radius + probeRadius)) + AtomI.zFloat
						);

					valid = 1;
					for ( int j = 0 ; j < n; j++ )
					{
						if ( i == j ) continue;
						AtomJ = m_AtomList[j];
						if ( distanceMatrix[i,j] > (AtomI.Radius + AtomJ.Radius + (2 * probeRadius) ) ) continue;
						// if not, see if it occludes the point of interest
						distJP = Atom.distanceBetween(AtomJ, atomSphereRepresentation);

						if ( distJP < (AtomJ.Radius + probeRadius) ) 
						{
							valid = 0;
							break; // if point isnt valid, why bother checking any more AtomJ's ..
						}
					}
					if(valid != 1) validPoints--;
				}

				fractions[i] = ((float)validPoints / (float)numSpherePoints);
				validPoints = numSpherePoints;

				switch ( SASAParam )
				{
					case SASAParamList.areaSASA:
						returnValues[i] = fractions[i] * currentRadiusI * currentRadiusI * 4 * (float)Math.PI;
						break;
					case SASAParamList.areaSASAModifiedVDW:
						returnValues[i] = fractions[i] * AtomI.Radius * AtomI.Radius * 4 * (float)Math.PI;
						break;
					case SASAParamList.areaVDW:
						returnValues[i] = fractions[i] * currentRadiusI * currentRadiusI * 4 * (float)Math.PI;
						break;
					case SASAParamList.fractionSASA:
						returnValues[i] = fractions[i];
						break;
					case SASAParamList.fractionVDW:
						returnValues[i] = fractions[i];
						break;
					default:
						throw new NotSupportedException("That calcArea enum type is not supported : " + SASAParam.ToString() );
				}


			}


			

			return returnValues;
		}

		private float[] calcSurfaceAccessibleFraction(float probeRadius)
		{
			int n = m_AtomList.Count;
			Atom AtomI;
			Atom AtomJ;
			float distJP;

			int valid = 1;
			const int totalPoints = numSpherePoints;
			int validPoints = totalPoints;

			float[] returnSASAs = new float[n];

			for ( int i = 0 ; i < n; i++ )
			{
				AtomI = m_AtomList[i];

				for ( int p = 0; p < standardSphere.Length; p++)
				{
					Vector theSpherePoint = standardSphere[p];
					Vector atomSphereRepresentation = new Vector(
						(theSpherePoint.xFloat * (AtomI.Radius + probeRadius)) + AtomI.xFloat,
						(theSpherePoint.yFloat * (AtomI.Radius + probeRadius)) + AtomI.yFloat,
						(theSpherePoint.zFloat * (AtomI.Radius + probeRadius)) + AtomI.zFloat
						);

					valid = 1;
					for ( int j = 0 ; j < n; j++ )
					{
						if ( i == j ) continue;
						AtomJ = m_AtomList[j];
						if ( distanceMatrix[i,j] > (AtomI.Radius + AtomJ.Radius + (2 * probeRadius) ) ) continue;
						// if not, see if it occludes the point of interest
						distJP = Atom.distanceBetween(AtomJ, atomSphereRepresentation);

						if ( distJP < (AtomJ.Radius + probeRadius) ) 
						{
							valid = 0;
							break; // if point isnt valid, why bother checking any more AtomJ's ..
						}
					}
					if(valid != 1) validPoints--;
				}
				returnSASAs[i] = ((float)validPoints / (float)totalPoints);
				validPoints = totalPoints;
			}
			return returnSASAs;
		}

		private const float stericHardness = 10.0f; // kCal/mol : the height of the intersection with the y-axis - the hardness of out steric penalty
		private const float tailoffX = 20.0f;

		public float[] calcSoftSterics()
		{
			float[] returnEnergies = new float[ m_AtomList.Count ];
			for( int i = 0; i < m_AtomList.Count; i++ ) 
			{
				for( int j = i + 1; j < m_AtomList.Count; j++ )
				{
					double pairEpsilon = Math.Sqrt( m_AtomList[i].atomPrimitive.FFAtomType.Epsilion * m_AtomList[j].atomPrimitive.FFAtomType.Epsilion );
                    // the geometric average of the epsilions of the two atom types
					// not used in the soft function : double pairSigma = ( ( m_AtomList[i].Radius / Math.Pow(2,1.0/6.0) ) + ( m_AtomList[j].Radius / Math.Pow(2,1.0/6.0) ) ) / 2.0;
					double VDWIdealDistance = m_AtomList[i].Radius + m_AtomList[i].Radius;
					// where the two atoms should be if they want optimum VDW energy
					double rIJ = distanceMatrix[i,j];

					if( rIJ <= VDWIdealDistance )
					{
						double pairEnergy = ( ( ( pairEpsilon - stericHardness ) / VDWIdealDistance ) * rIJ ) + stericHardness;
                        returnEnergies[i] += ( (float) ( pairEnergy / 2.0 ) );   
						returnEnergies[j] += ( (float) ( pairEnergy / 2.0 ) );  
					}
					else
					{
						double pairEnergy = ( ( ( - pairEpsilon ) / ( tailoffX - VDWIdealDistance ) ) * ( rIJ - VDWIdealDistance ) ) + pairEpsilon;
						returnEnergies[i] += ( (float) ( pairEnergy / 2.0 ) );   
						returnEnergies[j] += ( (float) ( pairEnergy / 2.0 ) );  
					}
				}
			}
			return returnEnergies;
		}
	}
}
