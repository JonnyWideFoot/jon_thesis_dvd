using System;

namespace UoB.Core.Primitives.Matrix
{
	/// <summary>
	/// Summary description for Matrix3x3.
	/// </summary>
	public class Matrix3x3 : Matrix
	{
		public Matrix3x3()
		{
			r = new double[3,3];
		}


		public override int Rows
		{
			get
			{
				return 3;
			}
		}

		public override int Columns
		{
			get
			{
				return 3;
			}
		}

		public void setTo3V( Vector vi, Vector vj, Vector vk )
		{
			r[0,0] = vi.x;
			r[1,0] = vi.y;
			r[2,0] = vi.z;
			r[0,1] = vj.x;
			r[1,1] = vj.y;
			r[2,1] = vj.z;
			r[0,2] = vk.x;
			r[1,2] = vk.y;
			r[2,2] = vk.z;
		}

		public void setToZero()
		{
			r[0,0] = 0.0;
			r[0,1] = 0.0;
			r[0,2] = 0.0;
			r[1,0] = 0.0;
			r[1,1] = 0.0;
			r[1,2] = 0.0;
			r[2,0] = 0.0;
			r[2,1] = 0.0;
			r[2,2] = 0.0;
		}

		public void setToIdentity()
		{
			//Debug.WriteLine("Matrix Set To Identity");
			r[0,0] = 1.0;
			r[0,1] = 0.0;
			r[0,2] = 0.0;
			r[1,0] = 0.0;
			r[1,1] = 1.0;
			r[1,2] = 0.0;
			r[2,0] = 0.0;
			r[2,1] = 0.0;
			r[2,2] = 1.0;
		}

		public void setToMultiplyMatrix( Matrix3x3 mat1,  Matrix3x3 mat2 )
		{
			//Debug.WriteLine("MatrixMultiply by 2nd matrix : ");
			r[0,0] = mat1.r[0,0] * mat2.r[0,0] + mat1.r[0,1] * mat2.r[1,0] + mat1.r[0,2] * mat2.r[2,0];
			r[1,0] = mat1.r[1,0] * mat2.r[0,0] + mat1.r[1,1] * mat2.r[1,0] + mat1.r[1,2] * mat2.r[2,0];
			r[2,0] = mat1.r[2,0] * mat2.r[0,0] + mat1.r[2,1] * mat2.r[1,0] + mat1.r[2,2] * mat2.r[2,0];

			r[0,1] = mat1.r[0,0] * mat2.r[0,1] + mat1.r[0,1] * mat2.r[1,1] + mat1.r[0,2] * mat2.r[2,1];
			r[1,1] = mat1.r[1,0] * mat2.r[0,1] + mat1.r[1,1] * mat2.r[1,1] + mat1.r[1,2] * mat2.r[2,1];
			r[2,1] = mat1.r[2,0] * mat2.r[0,1] + mat1.r[2,1] * mat2.r[1,1] + mat1.r[2,2] * mat2.r[2,1];

			r[0,2] = mat1.r[0,0] * mat2.r[0,2] + mat1.r[0,1] * mat2.r[1,2] + mat1.r[0,2] * mat2.r[2,2];
			r[1,2] = mat1.r[1,0] * mat2.r[0,2] + mat1.r[1,1] * mat2.r[1,2] + mat1.r[1,2] * mat2.r[2,2];
			r[2,2] = mat1.r[2,0] * mat2.r[0,2] + mat1.r[2,1] * mat2.r[1,2] + mat1.r[2,2] * mat2.r[2,2];
		}

		public void setTo( Matrix3x3 rOther )
		{
			r[0,0] = rOther.r[0,0];
			r[1,0] = rOther.r[1,0];
			r[2,0] = rOther.r[2,0];
			r[0,1] = rOther.r[0,1];
			r[1,1] = rOther.r[1,1];
			r[2,1] = rOther.r[2,1];
			r[0,2] = rOther.r[0,2];
			r[1,2] = rOther.r[1,2];
			r[2,2] = rOther.r[2,2];
		}

		public void setTo( double[,] rOther )
		{
			r[0,0] = rOther[0,0];
			r[1,0] = rOther[1,0];
			r[2,0] = rOther[2,0];
			r[0,1] = rOther[0,1];
			r[1,1] = rOther[1,1];
			r[2,1] = rOther[2,1];
			r[0,2] = rOther[0,2];
			r[1,2] = rOther[1,2];
			r[2,2] = rOther[2,2];
		}

		public bool isSetToIdentity()
		{
			return (
				r[0,0] == 1.0 &&
				r[0,1] == 0.0 &&
				r[0,2] == 0.0 &&
				r[1,0] == 0.0 &&
				r[1,1] == 1.0 &&
				r[1,2] == 0.0 &&
				r[2,0] == 0.0 &&
				r[2,1] == 0.0 &&
				r[2,2] == 1.0    );
		}


		public void doTranspose()
		{
			double swap;
			for(int i = 0; i < 3; i++ )
			{
				for(int j = 0; j < i; j++ )
				{
					swap = r[i,j];
					r[i,j] = r[j,i];
					r[j,i] = swap;
				}
			}
		}

		public double setToNormalised()
		{   // normalises so that sum of the matrix is 9
			double factor =
				Math.Abs(r[0,0]) + Math.Abs(r[1,0]) + Math.Abs(r[2,0]) +
				+ Math.Abs(r[0,1]) + Math.Abs(r[1,1]) + Math.Abs(r[2,1]) +
				+ Math.Abs(r[0,2]) + Math.Abs(r[1,2]) + Math.Abs(r[2,2]);

			factor /= 9.0;
			if(factor == 0) return 1.0;
			factor = 1.0 / factor;
			setToMul(factor);
			return factor;
		}

		public void setToMul(double k)
		{
			r[0,0] *= k;
			r[0,1] *= k;
			r[0,2] *= k;

			r[1,0] *= k;
			r[1,1] *= k;
			r[1,2] *= k;

			r[2,0] *= k;
			r[2,1] *= k;
			r[2,2] *= k;
		}

		public void setToDiagonaliseSymetric( out double l1, out double l2, out double l3, 
										      out Position p1, out Position p2, out Position p3 )
		{
			double[]    lambda;      // eigenvalues
			double      det;            // determinant
			double      a1,a2,a3;       // polynomial coefficients
			Matrix3x3   A;              
			Vector     v,w;           
			double      factor; 
//
			Matrix3x3 cacheOldVals = new Matrix3x3();
			cacheOldVals.setTo( this );
       
			A = new Matrix3x3();
			v = new Vector();
			w = new Vector();
			p1 = new Position();
			p2 = new Position();
			p3 = new Position();

			factor = setToNormalised();

			//printf("THIS: \n");
			//this.print();

			// enforce symetricality - WARNING: this obliterates any non symetric matrix !
			r[1,0] = r[0,1];
			r[2,0] = r[0,2];
			r[2,1] = r[1,2];

			// to calculate the eigenvalues, solve det(A-lambdaI)=0 

			// construct characteristic polynomial by determining the coefficients from
			// the determinant formula
			a1 = -(r[0,0]+r[1,1]+r[2,2]);
			a2 = -(r[2,1]*r[1,2] + r[0,1]*r[1,0] + r[0,2]*r[2,0] - r[0,0]*r[2,2] - r[0,0]*r[1,1] - r[1,1]*r[2,2]);
			a3 = -(r[0,0]*r[1,1]*r[2,2] - r[0,0]*r[2,1]*r[1,2] - r[2,2]*r[0,1]*r[1,0] - r[0,2]*r[2,0]*r[1,1] + r[0,1]*r[2,0]*r[1,2] + r[2,0]*r[1,0]*r[2,1]);



			// find the roots of the polynomial, which will equal the three lambda's (not nessessarily different !)
			if( 1 == solveCubicPolynomial( a1,a2,a3, out lambda ) )
			{
				// a symetric matrix *should* have three real eigenvalues - if not, something's seriously wrong
				throw new ApplicationException( "MATH ERROR: MatrixRotation() : diagonaliseSymetric(..): \"Characteristic Polynomial with only 1 real root\" \n" );
			}
 
			// give lambdas to the caller
			l1 = lambda[0];
			l2 = lambda[1];
			l3 = lambda[2];

			// now find eigenvectors by solving (A-lambdaI)V=0 for each eigenvalue lambda
			for(int lam=0;lam<=2;lam++)
			{
				A.setTo(this);
				A.r[0,0] -= lambda[lam];
				A.r[1,1] -= lambda[lam];
				A.r[2,2] -= lambda[lam];

				//printf("A-lambdaI= \n");
				//A.print();
		
				det = A.r[0,0]*A.r[1,1] - A.r[0,1]*A.r[1,0];
				//printf("determinant == %.20lf\n",det);
				if(Math.Abs(det) < 1e-100)
				{  // if subdeterminant is 0 then we dont need to solve 
					//printf("determinant == 0\n");
            	
					v.setTo((-A.r[0,1]-A.r[0,2])/A.r[0,0],1,1);
					w.setTo(v);
					w.setToMulMat(A);
					if(w.Length>0.0000000001)
					{ 
						v.setTo(1,1,(-A.r[2,0]-A.r[2,1])/A.r[2,2]);
						w.setTo(v);
						w.setToMulMat(A);
						if(w.Length>0.000000001)
						{ 
							v.setTo(1,(-A.r[1,0]-A.r[1,2])/A.r[1,1],1);
						}
					}

				}
				else
				{         // if subdeterminant is not 0, then we can solve the top two
					// linear equations by 2x2 matrix inversion
					v.setTo(  ( A.r[1,1]*(-A.r[0,2]) - A.r[0,1]*(-A.r[1,2])),
						(-A.r[1,0]*(-A.r[0,2]) + A.r[0,0]*(-A.r[1,2])),
						det );
                     
				}
				if(lam==0)p1.setTo(v);
				else if(lam==1)p2.setTo(v);
				else if(lam==2)p3.setTo(v);

				//v.print(); 
				if(v.Length<1e-100)
				{ 
					throw new Exception("MATHS ERROR: trivial solution (0,0,0) found in Diag..Symetric(...)\n");
				}
		
				//v1.print();
				w.setTo(v);
				w.setToMulMat(A);
				if(w.Length>0.001)
				{ 
					string errorString = "MATH ERROR: Invalid solution found in DiagonaliseSymetric(..)\n";
					errorString += "Eigenvector: for lambda=%.20f : ";
					errorString += v.ToString();
					errorString += "Result (should be 0,0,0):";
					errorString += w.ToString();
					errorString += "Original matrix (rescaled by factor= " + factor.ToString() + "): \n";
					errorString += this.ToString();
					errorString += "Original matrix (rescaled) minus lambda (also rescaled) \n";
					errorString += A.ToString();
					errorString += "det: " + det.ToString();

					throw new Exception(errorString);
				}
			}
  
			// now rescale eigenvalues back to what they should have been
			// without the normalisation
			// the vectors are left as they are, since they are redundant 
			// in terms of their length
			l1 *= factor;
			l2 *= factor;
			l3 *= factor;

		}

		private int solveCubicPolynomial(double a1,  double a2,  double a3, out double[] lamda )
		{
			lamda = new double[3];

			double Q = (Math.Pow(a1,2) - 3.0*a2)/9.0;
			double R = (2*Math.Pow(a1,3) - 9.0*a1*a2 + 27.0*a3) / 54.0;
			double Qcubed = Q * Q * Q;
			double D = Qcubed - R * R;

			// Three real roots 
			if (D >= 0) 
			{
				double theta = Math.Acos(R / (Math.Sqrt(Q)*Q));
				double sqrtQ = Math.Sqrt(Q);
				lamda[0] = -2.0 * sqrtQ * Math.Cos(theta/3.0) - a1/3.0;
				lamda[1] = -2.0 * sqrtQ * Math.Cos((theta + 2.0 * Math.PI)/3.0) - a1/3.0;
				lamda[2] = -2.0 * sqrtQ * Math.Cos((theta + 4.0 * Math.PI)/3.0) - a1/3.0;
				return 3;
			} 
			else 
			{ // one root 
				double e = Math.Pow((double)(Math.Sqrt(-D) + Math.Abs(R)),(double)(1.0/3.0));
				if (R>0)e*=-1.0;
				lamda[0] = (e+Q/e) - a1/3.0;
				return 1;
			}
		}

		public void doPreMultiply( Matrix3x3 mmat )
		{
			// if C = AB
			// you premultiply B by A

			// if C = BA
			// you postmultiply B by A

			// r' = mmat r
			// we are premultiplying r (this) by mmat

			//Debug.WriteLine("MatrixMultiply by 2nd matrix : ");

			double[,] re = new double[3,3];

			for( int i = 0; i < 3; i++ )
			{
				for( int j = 0; j < 3; j++ )
				{
					re[i,j] = mmat.r[i,0] * r[0,j] 
						+ mmat.r[i,1] * r[1,j] 
						+ mmat.r[i,2] * r[2,j];
				}
			}

			//			re[0,0] = mmat.r[0,0] * r[0,0] + mmat.r[0,1] * r[1,0] + mmat.r[0,2] * r[2,0];
			//			re[1,0] = mmat.r[1,0] * r[0,0] + mmat.r[1,1] * r[1,0] + mmat.r[1,2] * r[2,0];
			//			re[2,0] = mmat.r[2,0] * r[0,0] + mmat.r[2,1] * r[1,0] + mmat.r[2,2] * r[2,0];
			//
			//			re[0,1] = mmat.r[0,0] * r[0,1] + mmat.r[0,1] * r[1,1] + mmat.r[0,2] * r[2,1];
			//			re[1,1] = mmat.r[1,0] * r[0,1] + mmat.r[1,1] * r[1,1] + mmat.r[1,2] * r[2,1];
			//			re[2,1] = mmat.r[2,0] * r[0,1] + mmat.r[2,1] * r[1,1] + mmat.r[2,2] * r[2,1];
			//
			//			re[0,2] = mmat.r[0,0] * r[0,2] + mmat.r[0,1] * r[1,2] + mmat.r[0,2] * r[2,2];
			//			re[1,2] = mmat.r[1,0] * r[0,2] + mmat.r[1,1] * r[1,2] + mmat.r[1,2] * r[2,2];
			//			re[2,2] = mmat.r[2,0] * r[0,2] + mmat.r[2,1] * r[1,2] + mmat.r[2,2] * r[2,2];

			r[0,0] = re[0,0];
			r[1,0] = re[1,0];
			r[2,0] = re[2,0];
			r[0,1] = re[0,1];
			r[1,1] = re[1,1];
			r[2,1] = re[2,1];
			r[0,2] = re[0,2];
			r[1,2] = re[1,2];
			r[2,2] = re[2,2];
		}

		public void doPostMultiply( Matrix3x3 mmat )
		{
			// r' = r mmat
			// we are postmultiplying r (this) by mmat

			//Debug.WriteLine("MatrixMultiply by 2nd matrix : ");

			double[,] re = new double[3,3];

			for( int i = 0; i < 3; i++ )
			{
				for( int j = 0; j < 3; j++ )
				{
					re[i,j] = r[i,0] * mmat.r[0,j] 
						+ r[i,1] * mmat.r[1,j] 
						+ r[i,2] * mmat.r[2,j];
				}
			}

			r[0,0] = re[0,0];
			r[1,0] = re[1,0];
			r[2,0] = re[2,0];
			r[0,1] = re[0,1];
			r[1,1] = re[1,1];
			r[2,1] = re[2,1];
			r[0,2] = re[0,2];
			r[1,2] = re[1,2];
			r[2,2] = re[2,2];
		}



	}
}
