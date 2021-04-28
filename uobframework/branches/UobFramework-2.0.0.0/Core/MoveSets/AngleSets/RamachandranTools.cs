
using System;

namespace UoB.Core.MoveSets.AngleSets
{
	/// <summary>
	/// Summary description for RamachandranTools.
	/// </summary>
	public class RamachandranTools
	{
		private RamachandranTools()
		{
		}

		public static double SquareDistanceBetween( double angle1Phi, double angle1Psi, double angle2Phi, double angle2Psi )
		{
			double phiDiff = angle1Phi - angle2Phi;
			if( phiDiff < 0 ) phiDiff = -phiDiff;
			if( phiDiff > 180 ) phiDiff = 360.0 - phiDiff;
			double psiDiff = angle1Psi - angle2Psi;
			if( psiDiff < 0 ) psiDiff = -psiDiff;
			if( psiDiff > 180 ) psiDiff = 360.0 - psiDiff;
			return (phiDiff * phiDiff) + (psiDiff * psiDiff);
		}

		public static double DistanceBetween( double angle1Phi, double angle1Psi, double angle2Phi, double angle2Psi )
		{
			double phiDiff = angle1Phi - angle2Phi;
			if( phiDiff < 0 ) phiDiff = -phiDiff;
			if( phiDiff > 180 ) phiDiff = 360.0 - phiDiff;
			double psiDiff = angle1Psi - angle2Psi;
			if( psiDiff < 0 ) psiDiff = -psiDiff;
			if( psiDiff > 180 ) psiDiff = 360.0 - psiDiff;
			return Math.Sqrt( (phiDiff * phiDiff) + (psiDiff * psiDiff) );
		}

		public static  double AddAngle( double angle, double stepBy )
		{
			double ang = angle + stepBy;
			if( ang > 180.0 )
			{
				ang -= 360.0;
			}
			return ang;
		}

		public static  double MinusAngle( double angle, double stepBy )
		{
			double ang = angle - stepBy;
			if( ang < -180.0 )
			{
				ang += 360.0;
			}
			return ang;
		}

		public static  bool AngleIsInRange( double angle, double start1, double End1 )
		{
			if( angle <= End1 && angle >= start1 )
			{
				return true;
			}
			else return false;
		}

		public static  bool AngleIsInRange( double angle, double start1, double End1, double start2, double End2 )
		{
			if( angle <= End1 && angle >= start1 )
			{
				return true;
			}
			else if( angle <= End2 && angle >= start2 )
			{
				return true;
			}
			else return false;
		}	
	}
}
