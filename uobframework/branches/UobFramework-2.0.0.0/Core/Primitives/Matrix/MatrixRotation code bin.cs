		//private static double angCycleTot; // used in the stop condition for align looping
		//private static double tang, cose, sine, absAngle, arg1, arg2, noRotCutoff;



//		public void optimiseRotationForSuperimposition( Position[] p1, Position[] p2, int[] equivelencies, double precisionEpsilon )
//		{
//
//			noRotCutoff = precisionEpsilon * 0.25;
//
//			// minimise translation of the equivelent atoms in cartesian space
//			fittingTransationX.setToZeros();
//			fittingTransationY.setToZeros();
//			int equivCount = 0;
//			for( int i = 0; i < p1.Length; i++ )
//			{
//				if( equivelencies[i] == -1 )
//				{
//					// that atom in p1 has no partner
//					continue;
//				}
//				equivCount++;
//				fittingTransationX.Add( p1[i] );
//				fittingTransationY.Add( p2[equivelencies[i]] );
//			}
//			float mulFactorAlign = 1.0f / (float)(equivCount);
//			fittingTransationX.Multiply( mulFactorAlign );
//			fittingTransationY.Multiply( mulFactorAlign );
//
//			// now move the lot by the geometric center defined by the fragment atoms alone
//			for( int i = 0; i < p1.Length; i++ ) 
//			{
//				p1[i].Minus( fittingTransationX );
//			}
//			for( int i = 0; i < p2.Length; i++ )
//			{
//				p2[i].Minus( fittingTransationY );
//			}
//
//			setToIdentity(); // the global matrix, null rotation
//
//			// Setup the mixed tensor from the atomX and atomY positions
//			tensorU.setToTransposedTensor( p1, p2, equivelencies );
//
//			// minimise rotation of the equivelent atoms in cartesian space
//			int cycleCount = 0;
//			while(cycleCount < 300) // rotate those bitches
//			{
//				angCycleTot = 0.0; // used in the stop condition
//
//				// minimise each rotation in turn
//				angCycleTot += performXRotation();
//				angCycleTot += performYRotation();
//				angCycleTot += performZRotation();
//
//				if( angCycleTot < precisionEpsilon ) 
//				{
//					break; // our work here is done
//				}
//				cycleCount++;
//			}
//			transform(p2);
//		}
