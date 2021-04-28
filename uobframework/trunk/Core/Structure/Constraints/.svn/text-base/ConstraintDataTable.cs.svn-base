using System;
using System.Data;

using UoB.Core.Data;
using UoB.Core.FileIO.Tra;

namespace UoB.Core.Structure.Constraints
{
	/// <summary>
	/// Summary description for ConstraintDataTable.
	/// </summary>
	public class ConstraintDataTable : DataStore_WithColStats
	{
		protected MonitorType m_Type;

		public ConstraintDataTable( string name, StatModes mode, MonitorType type ) : base( name, mode )
		{
			m_Type = type;
		}

		protected override void SetAxisTitles()
		{
			m_XAxis = "Constraint List";
			m_YAxis = "Tra Files";
		}

		public void InitialiseRowTitles( ConstraintList conList )
		{
			for( int i = 0; i < conList.ConGrpCount; i++ )
			{
				ConstraintGroup cg = conList[i];
				for( int j = 0; j < cg.CountTo; j++ )
				{
					Constraint c = cg[j];
					DataRow r = m_MainTable.NewRow();
					r[m_TitleColumn] = c.GetName(m_Type);
					m_MainTable.Rows.Add(r);
				}
			}
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

			DataColumn c = new DataColumn( conList.TraFile.InternalName, typeof(float) );
			m_MainTable.Columns.Add( c );

			int rowCounter = 0;

			switch( m_Type )
			{
				case MonitorType.Absolute:
					for( int i = 0; i < conList.ConGrpCount; i++ )
					{
						ConstraintGroup cg = conList[i];
						for( int j = 0; j < cg.CountTo; j++ )
						{
							Constraint con = cg[j];
							m_MainTable.Rows[rowCounter++][c] = con.AbsoluteDataValue;					
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
							m_MainTable.Rows[rowCounter++][c] = con.RelativeDataValue;					
						}
					}
					break;
				default:
					throw new Exception("unsupported MonitorType in AddNextColumn call");
			}
		}
	}
}
