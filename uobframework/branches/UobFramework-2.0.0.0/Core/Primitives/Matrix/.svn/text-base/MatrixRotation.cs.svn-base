using System;
using System.Diagnostics;
using UoB.Core.Structure;

namespace UoB.Core.Primitives.Matrix
{
	/// <summary>
	/// Summary description for TransformMaths.
	/// </summary>
	public class MatrixRotation : Matrix3x3
	{
		public MatrixRotation() : base()
		{
			setToIdentity();
		}

        private static Matrix_TensorU tensorU = new Matrix_TensorU();
		private static Matrix3x3 tensorUT = new Matrix3x3();

		public void translate( Position[] p, Position translation )
		{
			for( int i = 0; i < p.Length; i++ ) 
			{
				p[i].Minus( translation );
			}
		}

		public void getTranslation( Position setFitXTranslation, Position setFitYTranslation, Position[] p1, Position[] p2, int[] equivs )
		{
			// NOTE : It is not garunteed that the position arrays are all used. The length of the equiv list is what sets the length
			// of the position arrays that we are using

			// minimise translation of the equivelent atoms in cartesian space
			setFitXTranslation.setToZeros();
			setFitYTranslation.setToZeros();
			int equivCount = 0;
			for( int i = 0; i < equivs.Length; i++ )
			{
				if( equivs[i] == -1 )
				{
					// that atom in p1 has no partner
					continue;
				}
				equivCount++;
				setFitXTranslation.Add( p1[i] );
				setFitYTranslation.Add( p2[equivs[i]] );
			}
			float mulFactorAlign = 1.0f / (float)(equivCount);
			setFitXTranslation.Multiply( mulFactorAlign );
			setFitYTranslation.Multiply( mulFactorAlign );
		}

		public void getTranslation( Position setFitXTranslation, Position setFitYTranslation, Position[] p1, Position[] p2 )
		{
			#if DEBUG
            if( p1.Length != p2.Length ) throw new Exception("code error, position arrays were recieved that were not of the same length");
			#endif

			// minimise translation of the equivelent atoms in cartesian space, both positions arrays are assumed to be the same length
			setFitXTranslation.setToZeros();
			setFitYTranslation.setToZeros();
			int equivCount = p1.Length;
			for( int i = 0; i < equivCount; i++ )
			{
				setFitXTranslation.Add( p1[i] );
				setFitYTranslation.Add( p2[i] );
			}
			float mulFactorAlign = 1.0f / (float)(equivCount);
			setFitXTranslation.Multiply( mulFactorAlign );
			setFitYTranslation.Multiply( mulFactorAlign );
		}

//		private double performXRotation()
//			// adapted from the source code of SupPos
//			/* calculates angle and determines second derivative
//				   since tan(a) = +/- sin(a) / cos(a)
//				   a = +/- atan(sin(a)/cos(a)) = atan(+/-sin(a)/cos(a))
//				   if cos(a) * arg2 + sin(a) * arg1 > 0  solution is a minimum
//				   returns absolute value of sinus of angle.
//				   on 386 with coprocessor this function is approximately three times
//				   faster as compared with spp_sin_angle_slow using atan2, sin and cos.
//				   */
//		{
//
//			arg1 = tensorU.r[1,2] - tensorU.r[2,1];
//			arg2 = tensorU.r[1,1] + tensorU.r[2,2];
//
//			if (arg1 == 0.0) return 0.0;
//			/* sin(x) = tan(x)/sqrt(1.0+tan(x)**2); cos(x) = 1.0/sqrt(1.0+tan(x)**2) */
//			tang = arg1/arg2;
//			cose = 1.0/Math.Sqrt(1.0+tang*tang);
//			if (arg2 < 0.0) cose = -cose;
//			sine = tang*cose;
//			if (cose*arg2 + sine*arg1 < 0.0) sine = -sine;  /* second solution */
//			if (sine < 0.0) 
//			{
//				absAngle = -sine;
//			}
//			else            
//			{
//				absAngle = sine;
//			}
//
//			if( absAngle < noRotCutoff )
//			{
//				return absAngle;
//			}
//
//			// we now know the absolute angle that we are rotating by
//			// the values of cose and sine are used below to change the rotation matrix
//
//			/* calculates the matrix product c = a * mat where a is a rotation
//				   around axis X. */
//
//			// here we change the meaning of arg1 and arg2, they are just temporary holders
//
//			// first mess with the tensor U
//			arg1 = cose * tensorU.r[1,0] - sine * tensorU.r[2,0];
//			arg2 = sine * tensorU.r[1,0] + cose * tensorU.r[2,0];
//			tensorU.r[1,0] = arg1;
//			tensorU.r[2,0] = arg2;
//
//			arg1 = cose * tensorU.r[1,1] - sine * tensorU.r[2,1];
//			arg2 = sine * tensorU.r[1,1] + cose * tensorU.r[2,1];
//			tensorU.r[1,1] = arg1;
//			tensorU.r[2,1] = arg2;
//
//			arg1 = cose * tensorU.r[1,2] - sine * tensorU.r[2,2];
//			arg2 = sine * tensorU.r[1,2] + cose * tensorU.r[2,2];
//			tensorU.r[1,2] = arg1;
//			tensorU.r[2,2] = arg2;
//
//			// now mess with the global rotation matrix
//			arg1 = cose * r[1,0] - sine * r[2,0];
//			arg2 = sine * r[1,0] + cose * r[2,0];
//			r[1,0] = arg1;
//			r[2,0] = arg2;
//
//			arg1 = cose * r[1,1] - sine * r[2,1];
//			arg2 = sine * r[1,1] + cose * r[2,1];
//			r[1,1] = arg1;
//			r[2,1] = arg2;
//
//			arg1 = cose * r[1,2] - sine * r[2,2];
//			arg2 = sine * r[1,2] + cose * r[2,2];
//			r[1,2] = arg1;
//			r[2,2] = arg2;
//
//			return absAngle;
//		}	
//
//		private double performYRotation()
//			// same as X but for Y ;-)
//		{
//			arg1 = tensorU.r[0,2] - tensorU.r[2,0];
//			arg2 = tensorU.r[0,0] + tensorU.r[2,2];
//
//			if (arg1 == 0.0) return 0.0;
//			/* sin(x) = tan(x)/sqrt(1.0+tan(x)**2); cos(x) = 1.0/sqrt(1.0+tan(x)**2) */
//			tang = arg1/arg2;
//			cose = 1.0/Math.Sqrt(1.0+tang*tang);
//			if (arg2 < 0.0) cose = -cose;
//			sine = tang*cose;
//			if (cose*arg2 + sine*arg1 < 0.0) sine = -sine;  /* second solution */
//			if (sine < 0.0) 
//			{
//				absAngle = -sine;
//			}
//			else            
//			{
//				absAngle = sine;
//			}
//
//			if( absAngle < noRotCutoff )
//			{
//				return absAngle;
//			}
//
//			// first mess with the tensor U
//			arg1 =  cose * tensorU.r[0,0] - sine * tensorU.r[2,0];
//			arg2 =  sine * tensorU.r[0,0] + cose * tensorU.r[2,0];
//			tensorU.r[0,0] = arg1;
//			tensorU.r[2,0] = arg2;
//
//			arg1 =  cose * tensorU.r[0,1] - sine * tensorU.r[2,1];
//			arg2 =  sine * tensorU.r[0,1] + cose * tensorU.r[2,1];
//			tensorU.r[0,1] = arg1;
//			tensorU.r[2,1] = arg2;
//
//			arg1 =  cose * tensorU.r[0,2] - sine * tensorU.r[2,2];
//			arg2 =  sine * tensorU.r[0,2] + cose * tensorU.r[2,2];
//			tensorU.r[0,2] = arg1;
//			tensorU.r[2,2] = arg2;
//
//			// now mess with the global rotation matrix
//			arg1 =  cose * r[0,0] - sine * r[2,0];
//			arg2 =  sine * r[0,0] + cose * r[2,0];
//			r[0,0] = arg1;
//			r[2,0] = arg2;
//
//			arg1 =  cose * r[0,1] - sine * r[2,1];
//			arg2 =  sine * r[0,1] + cose * r[2,1];
//			r[0,1] = arg1;
//			r[2,1] = arg2;
//
//			arg1 =  cose * r[0,2] - sine * r[2,2];
//			arg2 =  sine * r[0,2] + cose * r[2,2];
//			r[0,2] = arg1;
//			r[2,2] = arg2;
//
//			return absAngle;
//		}	
//
//		private double performZRotation()
//			// same as X but for Y ;-)
//		{
//			arg1 = tensorU.r[0,1] - tensorU.r[1,0];
//			arg2 = tensorU.r[0,0] + tensorU.r[1,1];
//
//			if (arg1 == 0.0) return 0.0;
//			/* sin(x) = tan(x)/sqrt(1.0+tan(x)**2); cos(x) = 1.0/sqrt(1.0+tan(x)**2) */
//			tang = arg1/arg2;
//			cose = 1.0/Math.Sqrt(1.0+tang*tang);
//			if (arg2 < 0.0) cose = -cose;
//			sine = tang*cose;
//			if (cose*arg2 + sine*arg1 < 0.0) sine = -sine;  /* second solution */
//			if (sine < 0.0) 
//			{
//				absAngle = -sine;
//			}
//			else            
//			{
//				absAngle = sine;
//			}
//
//			if( absAngle < noRotCutoff )
//			{
//				return absAngle;
//			}
//
//			// first mess with the tensor U
//			arg1 = cose * tensorU.r[0,0] - sine * tensorU.r[1,0];
//			arg2 = sine * tensorU.r[0,0] + cose * tensorU.r[1,0];
//			tensorU.r[0,0] = arg1;
//			tensorU.r[1,0] = arg2;
//
//			arg1 = cose * tensorU.r[0,1] - sine * tensorU.r[1,1];
//			arg2 = sine * tensorU.r[0,1] + cose * tensorU.r[1,1];
//			tensorU.r[0,1] = arg1;
//			tensorU.r[1,1] = arg2;
//
//			arg1 = cose * tensorU.r[0,2] - sine * tensorU.r[1,2];
//			arg2 = sine * tensorU.r[0,2] + cose * tensorU.r[1,2];
//			tensorU.r[0,2] = arg1;
//			tensorU.r[1,2] = arg2;
//
//			// now mess with the global rotation matrix
//			arg1 = cose * r[0,0] - sine * r[1,0];
//			arg2 = sine * r[0,0] + cose * r[1,0];
//			r[0,0] = arg1;
//			r[1,0] = arg2;
//
//			arg1 = cose * r[0,1] - sine * r[1,1];
//			arg2 = sine * r[0,1] + cose * r[1,1];
//			r[0,1] = arg1;
//			r[1,1] = arg2;
//
//			arg1 = cose * r[0,2] - sine * r[1,2];
//			arg2 = sine * r[0,2] + cose * r[1,2];
//			r[0,2] = arg1;
//			r[1,2] = arg2;
//
//			return absAngle;
//		}

		public double[] getDoubleArTransform( Position point ) // used in openGL calls
		{
			double[] pos = new double[3];
			pos[0] = r[0,0]*point.x + r[0,1]*point.y + r[0,2]*point.z;
			pos[1] = r[1,0]*point.x + r[1,1]*point.y + r[1,2]*point.z;
			pos[2] = r[2,0]*point.x + r[2,1]*point.y + r[2,2]*point.z;
			return pos;
		}

		public void transform( Position point )
		{
			double[] pos = new double[3];
			pos[0] = r[0,0]*point.x + r[0,1]*point.y + r[0,2]*point.z;
			pos[1] = r[1,0]*point.x + r[1,1]*point.y + r[1,2]*point.z;
			pos[2] = r[2,0]*point.x + r[2,1]*point.y + r[2,2]*point.z;
			point.x = pos[0];
			point.y = pos[1];
			point.z = pos[2];
		}

		public void transformToHolder( Position initialTranslation, Position[] donor, Position[] acceptor )
		{
			double x;
			double y;
			double z;
			for( int i = 0; i < donor.Length; i++ )
			{
				x = donor[i].x-initialTranslation.x;
				y = donor[i].y-initialTranslation.y;
				z = donor[i].z-initialTranslation.z;
				acceptor[i].x = r[0,0]*x + r[0,1]*y + r[0,2]*z;
				acceptor[i].y = r[1,0]*x + r[1,1]*y + r[1,2]*z;
				acceptor[i].z = r[2,0]*x + r[2,1]*y + r[2,2]*z;
			}
		}

		public void transform( AtomList atoms ) // used for rotamers at the min
		{
			double nx,ny,nz;
			for( int i = 0; i < atoms.Count; i++ )
			{
				nx = r[0,0]*atoms[i].x + r[0,1]*atoms[i].y + r[0,2]*atoms[i].z;
				ny = r[1,0]*atoms[i].x + r[1,1]*atoms[i].y + r[1,2]*atoms[i].z;
				nz = r[2,0]*atoms[i].x + r[2,1]*atoms[i].y + r[2,2]*atoms[i].z;
				atoms[i].x = nx;
				atoms[i].y = ny;
				atoms[i].z = nz;
			}
		}

		public void transform( Position[] vectors, int startIndex, int length ) // used for alignment engine
		{
			double nx,ny,nz;
			for( int i = startIndex; i < startIndex + length; i++ )
			{
				nx = r[0,0]*vectors[i].x + r[0,1]*vectors[i].y + r[0,2]*vectors[i].z;
				ny = r[1,0]*vectors[i].x + r[1,1]*vectors[i].y + r[1,2]*vectors[i].z;
				nz = r[2,0]*vectors[i].x + r[2,1]*vectors[i].y + r[2,2]*vectors[i].z;
				vectors[i].x = nx;
				vectors[i].y = ny;
				vectors[i].z = nz;
			}
		}

		public void transform( Position[] vectors ) // used for rotamers at the min
		{
			double nx,ny,nz;
			for( int i = 0; i < vectors.Length; i++ )
			{
				nx = r[0,0]*vectors[i].x + r[0,1]*vectors[i].y + r[0,2]*vectors[i].z;
				ny = r[1,0]*vectors[i].x + r[1,1]*vectors[i].y + r[1,2]*vectors[i].z;
				nz = r[2,0]*vectors[i].x + r[2,1]*vectors[i].y + r[2,2]*vectors[i].z;
				vectors[i].x = nx;
				vectors[i].y = ny;
				vectors[i].z = nz;
			}
		}

		public void transform( double[] points )
		{
			double nx,ny,nz;
			int counter = 0;
			for( int i = 0; i < (int)((float)points.Length / 3); i++ )
			{
				nx = r[0,0]*points[counter+0] + r[0,1]*points[counter+1] + r[0,2]*points[counter+2];
				ny = r[1,0]*points[counter+0] + r[1,1]*points[counter+1] + r[1,2]*points[counter+2];
				nz = r[2,0]*points[counter+0] + r[2,1]*points[counter+1] + r[2,2]*points[counter+2];
				points[counter+0] = nx;
				points[counter+1] = ny;
				points[counter+2] = nz;
				counter += 3;
			}
		}


		public void setToOptimalRotation( Position[] p1, Position[] p2, int[] equivelencies )
		{	
			Vector a1 = new Vector();
			Vector a2 = new Vector();
			Vector b1 = new Vector();
			Vector b2 = new Vector();
			//Vector a3, b3 // set in static operation below

			setToIdentity(); // the global matrix, null rotation

			// Setup the mixed tensor from the atomX and atomY positions
			tensorU.setToTensor( p1, p2, equivelencies );
			tensorUT.setTo( tensorU );
			tensorUT.doTranspose();
			tensorUT.doPostMultiply( tensorU );
			double l1, l2, l3;
			Position pOut1, pOut2, pOut3;

			//Position.DebugWriteFile( p1, p2 );

			tensorUT.setToDiagonaliseSymetric( out l1, out l2, out l3, out pOut1, out pOut2, out pOut3); 

			// sorting operation, max of 3 bool evaluations
			// a1 should be set to the smallest, a2 should be the second smallest
			// a3 shoul dbe the cross product of the two
			// orig line : a1.setTo(&v[lindex[2]]);    // set a1,a2 according to l1>l2>l3

			// L1 corresponds to pOut1 etc.

			if( l2 <= l3 )
			{
				if( l1 <= l2 )
				{
					// L1 < L2 < L3
					a1.setTo( pOut1 );
					a2.setTo( pOut2 );
				}
				else
				{
					if( l1 <= l3 )
					{
						// L2 < L1 < L3
						a1.setTo( pOut2 );
						a2.setTo( pOut1 );
					}
					else
					{
						// L2 < L3 < L1
						a1.setTo( pOut2 );
						a2.setTo( pOut3 );
					}
				}
			}
			else
			{
				if( l1 <= l3 )
				{
					// L1 < L3 < L2
					a1.setTo( pOut1 );
					a2.setTo( pOut3 );
				}
				else
				{
					if( l1 <= l2 )
					{
						// L3 < L1 < L2
						a1.setTo( pOut3 );
						a2.setTo( pOut1 );
					}
					else
					{
						// L3 < L2 < L1
						a1.setTo( pOut3 );
						a2.setTo( pOut2 );
					}
				}
			}

			Vector a3 = Vector.crossProduct(a1,a2);

			a1.MakeUnitVector();
			a2.MakeUnitVector();
			a3.MakeUnitVector();

			b1.setTo(a1);
			b1.setToMulMat(tensorU);
			b1.MakeUnitVector();
			b2.setTo(a2);
			b2.setToMulMat(tensorU);
			b2.MakeUnitVector();
			Vector b3 = Vector.crossProduct(b1,b2);


			// we have the rot matrix .. use it 
			r[0,0] = b1.x * a1.x +  b2.x * a2.x +  b3.x * a3.x;
			r[0,1] = b1.x * a1.y +  b2.x * a2.y +  b3.x * a3.y;
			r[0,2] = b1.x * a1.z +  b2.x * a2.z +  b3.x * a3.z;

			r[1,0] = b1.y * a1.x +  b2.y * a2.x +  b3.y * a3.x;
			r[1,1] = b1.y * a1.y +  b2.y * a2.y +  b3.y * a3.y;
			r[1,2] = b1.y * a1.z +  b2.y * a2.z +  b3.y * a3.z;

			r[2,0] = b1.z * a1.x +  b2.z * a2.x +  b3.z * a3.x;
			r[2,1] = b1.z * a1.y +  b2.z * a2.y +  b3.z * a3.y;
			r[2,2] = b1.z * a1.z +  b2.z * a2.z +  b3.z * a3.z;
		}
		
		public void setToXAxisRot(double angle)
		{
			// these efectively set the matrix to an identity matrix with the X-rotation
			double sinAngle = Math.Sin( angle );
			double cosAngle = Math.Cos( angle );
			r[0,0] = 1; 
			r[0,1] = 0;
			r[0,2] = 0;
			r[1,0] = 0;
			r[1,1] = cosAngle;
			r[1,2] = -sinAngle;
			r[2,0] = 0;
			r[2,1] = sinAngle;
			r[2,2] = cosAngle;
		}

		public void setToYAxisRot(double angle)
		{
			// these efectively set the matrix to an identity matrix with the Y-rotation
			double sinAngle = Math.Sin( angle );
			double cosAngle = Math.Cos( angle );
			r[0,0] = cosAngle; 
			r[0,1] = 0;
			r[0,2] = sinAngle;
			r[1,0] = 0;
			r[1,1] = 1;
			r[1,2] = 0;
			r[2,0] = -sinAngle;
			r[2,1] = 0;
			r[2,2] = cosAngle;
		}

		public void setToZAxisRot(double angle)
		{
			double sinAngle = Math.Sin( angle );
			double cosAngle = Math.Cos( angle );
			r[0,0] = cosAngle; 
			r[0,1] = -sinAngle;
			r[0,2] = 0;
			r[1,0] = sinAngle;
			r[1,1] = cosAngle;
			r[1,2] = 0;
			r[2,0] = 0;
			r[2,1] = 0;
			r[2,2] = 1;
		}

		public void setToAxisRot( Vector axis, double angle )
		{
			double[] axisDoubles = new double[3];
            axisDoubles[0] = axis.x;
			axisDoubles[1] = axis.y;
			axisDoubles[2] = axis.z;
			setToAxisRot(axisDoubles, angle);
		}
 
		public void setToAxisRot(double[] axis, double angle)
		{

			double magnitude = Math.Sqrt(axis[0]*axis[0]+axis[1]*axis[1]+axis[2]*axis[2]);
			if ( magnitude != 0.0f )
			{
				axis[0] /= magnitude;
				axis[1] /= magnitude;
				axis[2] /= magnitude;
				magnitude=1.0f;
			}
			else
			{
				return; // you cant rotate around (0,0,0)
			}

			double c = Math.Cos(angle);         
			double s = Math.Sin(angle);
			double t = 1 - c;
			double tx = t * axis[0];

			double sx = s * axis[0];
			double sy = s * axis[1];
			double sz = s * axis[2];
			double tyz = t * axis[1] * axis[2];

			r[0,0] =  tx * axis[0] + c;                
			r[0,1] =  tx * axis[1] - sz;
			r[0,2] =  tx * axis[2] + sy;

			r[1,0] =  tx * axis[1] + sz;
			r[1,1] =  t * Math.Pow(axis[1],2) + c;
			r[1,2] =  tyz - sx;

			r[2,0] =  tx * axis[2] - sy;
			r[2,1] =  tyz + sx;
			r[2,2] =  t * Math.Pow(axis[2],2) + c;
		}
	}
}
