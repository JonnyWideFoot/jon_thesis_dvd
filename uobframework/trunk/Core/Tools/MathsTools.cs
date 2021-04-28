using System;
using System.Collections.Generic;
using System.Text;

namespace UoB.Core.Tools
{
    public class MathsTools
    {
        public static double Mean(List<double> ar)
        {
            double sum = 0.0;
            for (int i = 0; i < ar.Count; i++)
            {
                sum += ar[i];
            }
            return sum / (double)ar.Count;
        }

        public static double Max(List<double> ar)
        {
            double max = double.MinValue;
            for (int i = 0; i < ar.Count; i++)
            {
                if (ar[i] > max)
                {
                    max = ar[i];
                }
            }
            return max;
        }

        public static double StdDev(List<double> ar)
        {
            double mean = Mean(ar);
            double sum = 0.0;
            for (int i = 0; i < ar.Count; i++)
            {
                double diff = ar[i] - mean;
                sum += (diff*diff);
            }
            return Math.Sqrt( sum / (double)(ar.Count - 1) );
        }

        public static double Min(List<double> ar)
        {
            double min = double.MaxValue;
            for (int i = 0; i < ar.Count; i++)
            {
                if (ar[i] < min)
                {
                    min = ar[i];
                }
            }
            return min;
        }

        public static double PercLower(List<double> ar, double cutoff)
        {
            int count = 0;
            for (int i = 0; i < ar.Count; i++)
            {
                if (ar[i] < cutoff)
                {
                    count++;
                }
            }
            return (double)count/(double)ar.Count;
        }

        /// <summary>
        /// This is most likely not at all the most efficient alrorithm, but it will do...
        /// </summary>
        /// <param name="ar"></param>
        /// <param name="percCutoff"></param>
        /// <param name="canSort"></param>
        /// <returns></returns>
        public static double ValueAtPercentageCutoff(List<double> ar, double percCutoff, bool canSort)
        {
            if (percCutoff <= 0.0 || percCutoff >= 1.0) throw new ArgumentOutOfRangeException("The precentage must be represented by a fraction");

            if (!canSort)
            {
                List<double> arClone = new List<double>(ar.Count);
                arClone.AddRange(ar);
                ar = arClone;
            }

            // Sort the data, lowest first
            ar.Sort();

            // What integer index is the percCutoff sitting on?
            int count = (int)(Math.Floor((double)ar.Count * percCutoff)) - 1;
            if (count < 0) count = 0;

            // Return the value of the data point sitting on the 'percCutoff' index of the sorted data set
            return ar[count];
        }
        

        /// <summary>
        /// Implemented from http://ndevilla.free.fr/median/median.pdf
        /// Fast median search: an ANSI C implementation
        /// Nicolas Devillard - ndevilla AT free DOT fr
        /// July 1998
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static double Median(List<double> m)
        {
            if (m.Count == 0) throw new ArgumentException("Array size cannot be 0");

            int i, less, greater, equal;
            int n = m.Count;
            double min, max, guess, maxltguess, mingtguess;
            min = max = m[0];
            for (i = 1; i < n; i++)
            {
                if (m[i] < min) min = m[i];
                if (m[i] > max) max = m[i];
            }
            while (true)
            {
                guess = (min + max) / 2;
                less = 0; greater = 0; equal = 0;
                maxltguess = min;
                mingtguess = max;
                for (i = 0; i < n; i++)
                {
                    if (m[i] < guess)
                    {
                        less++;
                        if (m[i] > maxltguess) maxltguess = m[i];
                    }
                    else if (m[i] > guess)
                    {
                        greater++;
                        if (m[i] < mingtguess) mingtguess = m[i];
                    }
                    else equal++;
                }
                if (less <= (n + 1) / 2 && greater <= (n + 1) / 2) break;
                else if (less > greater) max = maxltguess;
                else min = mingtguess;
            }
            if (less >= (n + 1) / 2) return maxltguess;
            else if (less + equal >= (n + 1) / 2) return guess;
            else return mingtguess;
        }
    }
}
