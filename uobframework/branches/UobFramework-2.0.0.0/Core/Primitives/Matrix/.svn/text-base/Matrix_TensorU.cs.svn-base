using System;
using UoB.Core.Structure;

namespace UoB.Core.Primitives.Matrix
{
	/// <summary>
	/// Summary description for Matrix_TensorU.
	/// </summary>
	public class Matrix_TensorU : Matrix3x3
	{
		public Matrix_TensorU()
		{
		}

		private static double xx,xy,xz,yx,yy,yz; // temp holders for tensorU calculation

		public void setToTransposedTensor( Position[] p1, Position[] p2, int[] equivelencies )
		{
			setToZero();
			for( int i = 0; i < p1.Length; i++ )
			{
				if( equivelencies[i] == -1 )
				{
					// that atom in p1 has no partner
					continue;
				}

				xx = p1[i].x; 
				xy = p1[i].y; 
				xz = p1[i].z;
				yx = p2[equivelencies[i]].x; 
				yy = p2[equivelencies[i]].y; 
				yz = p2[equivelencies[i]].z;

				r[0,0] += xx * yx; r[0,1] += xy * yx; r[0,2] += xz * yx;
				r[1,0] += xx * yy; r[1,1] += xy * yy; r[1,2] += xz * yy;
				r[2,0] += xx * yz; r[2,1] += xy * yz; r[2,2] += xz * yz;
			}
		}

		public void setToTensor( Position[] p1, Position[] p2, int[] equivelencies )
		{
			// NOTE : It is not garunteed that the position arrays are all used. The length of the equivelencies 
			// list is what sets the length of the position arrays that we are using

			setToZero();
			for( int i = 0; i < equivelencies.Length; i++ )
			{
				if( equivelencies[i] == -1 )
				{
					// that atom in p1 has no partner
					continue;
				}

				xx = p1[i].x; 
				xy = p1[i].y; 
				xz = p1[i].z;
				yx = p2[equivelencies[i]].x; 
				yy = p2[equivelencies[i]].y; 
				yz = p2[equivelencies[i]].z;

				r[0,0] += xx * yx; r[1,0] += xy * yx; r[2,0] += xz * yx;
				r[0,1] += xx * yy; r[1,1] += xy * yy; r[2,1] += xz * yy;
				r[0,2] += xx * yz; r[1,2] += xy * yz; r[2,2] += xz * yz;
			}
		}
	}
}
