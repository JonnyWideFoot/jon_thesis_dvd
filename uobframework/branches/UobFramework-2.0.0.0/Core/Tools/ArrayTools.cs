using System;
using UoB.Core.Primitives.Collections;

namespace UoB.Core.Tools
{
	/// <summary>
	/// Summary description for ArrayTools.
	/// </summary>
	public class ArrayTools
	{
		private ArrayTools()
		{
		}

		public static void QuickSort( IComparable[] numbers, params IComparable[][] otherArrays )
		{
			for( int i = 0; i < otherArrays.Length; i++ )
			{
				if( numbers.Length != otherArrays[i].Length )
				{
					throw new ArgumentException("Arrays must all be the same length");
				}
			}
			IComparable[] pivotCache = new IComparable[otherArrays.Length];
			q_sort( numbers, pivotCache, otherArrays, 0, numbers.Length - 1 );
		}

		private static void q_sort( IComparable[] numbers, IComparable[] pivotCache, IComparable[][] otherArrays, int left, int right )
		{
			int l_hold = left;
			int r_hold = right;
			IComparable pivot = numbers[left];

			for( int i = 0; i < otherArrays.Length; i++ )
			{
				pivotCache[i] = otherArrays[i][left];
			}

			while (left < right)
			{
				//while ((numbers[right] >= pivot) && (left < right))
				while ((numbers[right].CompareTo( pivot )<=0) && (left < right))
					right--;
				if (left != right)
				{
					numbers[left] = numbers[right];
					for( int i = 0; i < otherArrays.Length; i++ )
					{
						otherArrays[i][left] = otherArrays[i][right];
					}
					left++;
				}
				//while ((numbers[left] <= pivot) && (left < right))
				while ((numbers[left].CompareTo(pivot)<=0) && (left < right))
					left++;
				if (left != right)
				{
					numbers[right] = numbers[left];
					for( int i = 0; i < otherArrays.Length; i++ )
					{
						otherArrays[i][right] = otherArrays[i][left];
					}
					right--;
				}
			}
			numbers[left] = pivot;
			for( int i = 0; i < otherArrays.Length; i++ )
			{
				otherArrays[i][left] = pivotCache[i];
			}
			int newPivot = left;
			left = l_hold;
			right = r_hold;
			if (left < newPivot)
				q_sort(numbers, pivotCache, otherArrays, left, newPivot-1);
			if (right > newPivot)
				q_sort(numbers, pivotCache, otherArrays, newPivot+1, right);
		}

		public static void QuickSort( int[] numbers, params int[][] otherArrays )
		{
			for( int i = 0; i < otherArrays.Length; i++ )
			{
				if( numbers.Length != otherArrays[i].Length )
				{
					throw new ArgumentException("Arrays must all be the same length");
				}
			}
			int[] pivotCache = new int[otherArrays.Length];
			q_sort( numbers, pivotCache, otherArrays, 0, numbers.Length - 1 );
		}

		private static void q_sort( int[] numbers, int[] pivotCache, int[][] otherArrays, int left, int right )
		{
			int l_hold = left;
			int r_hold = right;
			int pivot = numbers[left];

			for( int i = 0; i < otherArrays.Length; i++ )
			{
				pivotCache[i] = otherArrays[i][left];
			}

			while (left < right)
			{
				while ((numbers[right] >= pivot) && (left < right))
					right--;
				if (left != right)
				{
					numbers[left] = numbers[right];
					for( int i = 0; i < otherArrays.Length; i++ )
					{
						otherArrays[i][left] = otherArrays[i][right];
					}
					left++;
				}
				while ((numbers[left] <= pivot) && (left < right))
					left++;
				if (left != right)
				{
					numbers[right] = numbers[left];
					for( int i = 0; i < otherArrays.Length; i++ )
					{
						otherArrays[i][right] = otherArrays[i][left];
					}
					right--;
				}
			}
			numbers[left] = pivot;
			for( int i = 0; i < otherArrays.Length; i++ )
			{
				otherArrays[i][left] = pivotCache[i];
			}
			pivot = left;
			left = l_hold;
			right = r_hold;
			if (left < pivot)
				q_sort(numbers, pivotCache, otherArrays, left, pivot-1);
			if (right > pivot)
				q_sort(numbers, pivotCache, otherArrays, pivot+1, right);
		}

		public static void QuickSort( int[] numbers )
		{
			q_sort( numbers, 0, numbers.Length - 1 );
		}

		private static void q_sort( int[] numbers, int left, int right )
		{
			int l_hold = left;
			int r_hold = right;
			int pivot = numbers[left];

			while (left < right)
			{
				while ((numbers[right] >= pivot) && (left < right))
					right--;
				if (left != right)
				{
					numbers[left] = numbers[right];
					left++;
				}
				while ((numbers[left] <= pivot) && (left < right))
					left++;
				if (left != right)
				{
					numbers[right] = numbers[left];
					right--;
				}
			}
			numbers[left] = pivot;
			pivot = left;
			left = l_hold;
			right = r_hold;
			if (left < pivot)
				q_sort(numbers, left, pivot-1);
			if (right > pivot)
				q_sort(numbers, pivot+1, right);
		}

		public static float sumFloatArray( float[] floats )
		{
			float sum = 0.0f;
			for ( int i = 0; i < floats.Length; i++ )
			{
				sum += floats[i];
			}
			return sum;
		}

		public static double[] convertToDoubleAr( FloatArrayList theValues )
		{
			double[] returnValues = new double[ theValues.Count ];
			for ( int i = 0; i < returnValues.Length; i++ )
			{
				returnValues[i] = theValues[i];
			}
			return returnValues;
		}
		
		public static double[] convertToDoubleAr( float[] theValues )
		{
			double[] returnValues = new double[ theValues.Length ];
			for ( int i = 0; i < theValues.Length; i++ )
			{
				returnValues[i] = theValues[i];
			}
			return returnValues;
		}
	}
}
