using System;
using System.IO;
using System.Data;
using System.Collections.Generic;

using UoB.Core.Data;

namespace DataTableGen
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class DataTableMaker
	{
		DirectoryInfo m_DataDir;

		public DataTableMaker( DirectoryInfo dataDir )
		{
			m_DataDir = dataDir;
			if( !m_DataDir.Exists )
			{
				throw new ArgumentException("Data dir doesnt exist...");
			}
		}

		private void obtainDataV1( DirectoryInfo[] nodeDataDirs, string filename, out float[] column1, out float[] column2 )
		{
			List<float> data1 = new List<float>();
            List<float> data2 = new List<float>();

            string line;
            for (int i = 0; i < nodeDataDirs.Length; i++)
            {
                string openName = nodeDataDirs[i].FullName + Path.DirectorySeparatorChar + filename;
                if (!File.Exists(openName)) throw new Exception("File does not exist...");
                StreamReader re = new StreamReader(openName);
                
                while( null != ( line = re.ReadLine() ) )
                {
                    if (line.Length != 0)
                    {
                        string[] dataParts = line.Split(',');
                        data1.Add(float.Parse(dataParts[0]));
                        data2.Add(float.Parse(dataParts[1]));
                    }
                }

                re.Close();
            }

            column1 = data1.ToArray();
            column2 = data2.ToArray();
            return;
		}

        private void obtainDataV2(DirectoryInfo[] nodeDataDirs, string filename, out float[] column1, out float percentageValid )
        {
            List<float> data1 = new List<float>();

            float valid = 0.0f;
            float total = 0.0f;

            string line;
            for (int i = 0; i < nodeDataDirs.Length; i++)
            {
                string openName = nodeDataDirs[i].FullName + Path.DirectorySeparatorChar + filename;
                if (!File.Exists(openName)) throw new Exception("File does not exist...");
                StreamReader re = new StreamReader(openName);

                while (null != (line = re.ReadLine()))
                {
                    if (line.Length != 0)
                    {
                        total += 1.0f;
                        float data = float.Parse(line);
                        if (data != float.MaxValue)
                        {                            
                            data1.Add(data);
                            valid += 1.0f;
                        }
                    }
                }

                re.Close();
            }

            percentageValid = (valid / total) * 100.0f;
            column1 = data1.ToArray();
            return;
        }

        private void AddConfCountRow( DataTable statTable )
        {
            DataRow confCountRow = statTable.NewRow();
            statTable.Rows.Add(confCountRow);
            confCountRow[0] = "ConformerCount";

            for (int i = 1; i < statTable.Columns.Count; i++)
            {
                // disect the filename ...
                // e.g. out_8_10_9.log
                // i.e.  "out" + all8mers + numOtherAngles + numGlyAngles + ".log"

                string fileName = statTable.Columns[i].ColumnName;
                fileName = fileName.Substring(0, fileName.Length - 4);
                string[] fileNameParts = fileName.Split('_');

                double otherAngCount = double.Parse(fileNameParts[2]);
                double glyAngCount = double.Parse(fileNameParts[3]);

                confCountRow[i] = (float)Math.Log(5.0 * glyAngCount * Math.Pow(otherAngCount, 18),Math.E);
            }
        }

        public void CreateTablesV1(string filenameStem)
		{
			DirectoryInfo[] nodeDataDirs = new DirectoryInfo[ 12 ];
			for( int i = 0; i < nodeDataDirs.Length; i++ )
			{
				nodeDataDirs[i] = new DirectoryInfo( m_DataDir.FullName + Path.DirectorySeparatorChar + (i+1).ToString() );
			}

			FileInfo[] dataFiles = nodeDataDirs[0].GetFiles( "*.log" );

            DataStore_WithColStats_GeneralUse dataTableCRMS = new DataStore_WithColStats_GeneralUse("DataTableCRMS", StatModes.All);
            DataStore_WithColStats_GeneralUse dataTableARMS = new DataStore_WithColStats_GeneralUse("DataTableARMS", StatModes.All);

            dataTableCRMS.BeginEditing();
            dataTableARMS.BeginEditing();
            for( int i = 0; i < dataFiles.Length; i++ )
			{
                float[] cRMS, aRMS;
                obtainDataV1(nodeDataDirs, dataFiles[i].Name, out cRMS, out aRMS);
                dataTableCRMS.AddColumn(dataFiles[i].Name, cRMS);
                dataTableARMS.AddColumn(dataFiles[i].Name, aRMS);
			}
            dataTableCRMS.EndEditing(); // creates the full stat table
            dataTableARMS.EndEditing(); // creates the full stat table

			// now hack the stat table to give us "conformer count" as a row ...
            AddConfCountRow(dataTableCRMS.StatsTable);
            AddConfCountRow(dataTableARMS.StatsTable);
			
			// all done, now save to a CSV file ...
			dataTableCRMS.SaveStatsTo( filenameStem + "cRMS_stat.csv", DataOutputType.CSV );
            dataTableCRMS.SaveMainTo(filenameStem + "cRMS_main.csv", DataOutputType.CSV);
            dataTableARMS.SaveStatsTo(filenameStem + "aRMS_stat.csv", DataOutputType.CSV);
            dataTableARMS.SaveMainTo(filenameStem + "aRMS_main.csv", DataOutputType.CSV);
		}

        public void CreateTablesV2(string filenameStem)
        {
            DirectoryInfo[] nodeDataDirs = new DirectoryInfo[12];
            for (int i = 0; i < nodeDataDirs.Length; i++)
            {
                nodeDataDirs[i] = new DirectoryInfo(m_DataDir.FullName + Path.DirectorySeparatorChar + (i + 1).ToString());
            }

            FileInfo[] dataFiles = nodeDataDirs[0].GetFiles("*.log");

            DataStore_WithColStats_GeneralUse dataTableCRMS = new DataStore_WithColStats_GeneralUse("DataTableCRMS", StatModes.All);

            dataTableCRMS.BeginEditing();
            float[] percentageValid = new float[dataFiles.Length];
            for (int i = 0; i < dataFiles.Length; i++)
            {
                float[] cRMS;
                obtainDataV2(nodeDataDirs, dataFiles[i].Name, out cRMS, out percentageValid[i]);
                dataTableCRMS.AddColumn(dataFiles[i].Name, cRMS);
            }
            dataTableCRMS.EndEditing(); // creates the full stat table

            DataRow percCounts = dataTableCRMS.StatsTable.NewRow();
            dataTableCRMS.StatsTable.Rows.Add(percCounts);
            percCounts[0] = "PercentageValid";

            for (int i = 1; i < dataTableCRMS.StatsTable.Columns.Count; i++)
            {
                percCounts[i] = percentageValid[i-1];
            }

            // now hack the stat table to give us "conformer count" as a row ...
            AddConfCountRow(dataTableCRMS.StatsTable);

            // all done, now save to a CSV file ...
            dataTableCRMS.SaveStatsTo(filenameStem + "cRMS_stat.csv", DataOutputType.CSV);
            dataTableCRMS.SaveMainTo(filenameStem + "cRMS_main.csv", DataOutputType.CSV);
        }


		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
            string dataPath = @"F:\10a - Loop Builder Stage 1 Post IdealGeom and New AngleBounds\_Exec_AngTest\_Result-v5";
			DirectoryInfo di = new DirectoryInfo( dataPath );
			DataTableMaker dm = new DataTableMaker( di );
			dm.CreateTablesV2( dataPath + "\\" );
		}
	}
}
