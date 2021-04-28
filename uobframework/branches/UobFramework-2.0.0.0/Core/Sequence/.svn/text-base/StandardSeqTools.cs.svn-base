using System;
using System.Collections;

namespace UoB.Core.Sequence
{
	/// <summary>
	/// Summary description for SeqTool.
	/// </summary>
	public class StandardSeqTools
	{
		private StandardSeqTools()
		{
		}

		/// <summary>
		/// Should be these ....
		/// NotGlyOrPro
		/// ShortHydrophobic
		/// BulkyAromatic
		/// Polar
		/// Charged
		/// </summary>
		/// <returns></returns>
		public static StandardResidues[] GetStandardResidueGroups()
		{
			StandardResidues[] resTypes = (StandardResidues[]) Enum.GetValues( typeof(StandardResidues) );
			string[] resTypeNames = Enum.GetNames( typeof(StandardResidues) );
			ArrayList list = new ArrayList();
			for( int i = 0; i < resTypeNames.Length; i++ )
			{
				if( resTypeNames[i].Length > 1 && resTypes[i] != StandardResidues.All )
				{
					list.Add( resTypes[i] );
				}
			}
			return (StandardResidues[]) list.ToArray( typeof( StandardResidues ) ); 
		}

		/// <summary>
		/// Should be ...
		/// A, C, D, E, F, G, H, I, K, L, M, N, P, p, Q, R, S, T, V, W, Y
		/// P = trans proline, p = cis proline
		/// </summary>
		/// <returns></returns>
		public static StandardResidues[] GetIndividualStandardResidues()
		{
			StandardResidues[] resTypes = (StandardResidues[]) Enum.GetValues( typeof(StandardResidues) );
			string[] resTypeNames = Enum.GetNames( typeof(StandardResidues) );
			ArrayList list = new ArrayList();
			for( int i = 0; i < resTypeNames.Length; i++ )
			{
				if( resTypeNames[i].Length == 1 )
				{
					list.Add( resTypes[i] );
				}
			}
			return (StandardResidues[]) list.ToArray( typeof( StandardResidues ) ); 
		}

		public static bool IsResTypeMatch( StandardResidues includes, char qureyResidue )
		{
			try
			{
				// THIS IS Case Sensitive : a lower case letter represents the cis-peptide conformation
				StandardResidues givenType = (StandardResidues) Enum.Parse( typeof( StandardResidues ), qureyResidue.ToString(), false );
				return( 0 < (givenType & includes) );
			}
			catch(System.ArgumentException)
			{
				return false;
			}
		}
	}
}
