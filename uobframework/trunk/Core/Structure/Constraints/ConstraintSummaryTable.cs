using System;
using System.Data;

using UoB.Core.FileIO.Tra;
using UoB.Core.Data;

namespace UoB.Core.Structure.Constraints
{
	/// <summary>
	/// Summary description for ConstraintSummaryTable.
	/// </summary>
	public class ConstraintSummaryTable : DataStore_StatTable
	{
		protected MonitorType m_Type;

		public ConstraintSummaryTable( string name, StatModes mode, int expectedEntryLoad, MonitorType type ) 
			: base( name, mode, expectedEntryLoad )
		{
			m_Type = type;
		}

		protected override void SetAxisTitles()
		{
			m_XAxis = "Stats";
			m_YAxis = "Tra Files";
		}

		public void AddColumn( int setToTraIndex, ConstraintList conList )
		{
			if( setToTraIndex == -1 )
			{
				setToTraIndex += conList.TraFile.PositionDefinitions.Count;
			}
			conList.TraFile.PositionDefinitions.Position = setToTraIndex;

			AddColumnForCurrentTraIndex( conList );
		}

		public void AddColumnForCurrentTraIndex( ConstraintList conList )
		{
			if( !IsEditing ) throw new Exception("Editing mode must be active!");

			m_DataBuffer.Clear();

			switch( m_Type )
			{
				case MonitorType.Absolute:
					for( int i = 0; i < conList.ConGrpCount; i++ )
					{
						ConstraintGroup cg = conList[i];
						for( int j = 0; j < cg.CountTo; j++ )
						{
							Constraint con = cg[j];
							m_DataBuffer.AddFloat( con.AbsoluteDataValue );					
						}
					}
					break;
				case MonitorType.DeviationFromIdeal:
					for( int i = 0; i < conList.ConGrpCount; i++ )
					{
						ConstraintGroup cg = conList[i];
						for( int j = 0; j < cg.CountTo; j++ )
						{
							Constraint con = cg[j];
							m_DataBuffer.AddFloat( con.RelativeDataValue );					
						}
					}
					break;
				default:
					throw new Exception("unsupported MonitorType in AddNextColumn call");
			}

			EvaluateInternalDataAndAddColumn( conList.TraFile.InternalName );
		}
	}
}
