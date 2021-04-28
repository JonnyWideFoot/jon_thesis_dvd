using System;
using System.IO;
using System.Collections;

namespace UoB.Methodology.DSSPAnalysis.AngleFitting
{
	/// <summary>
	/// Summary description for SearchFittingResumeData.
	/// </summary>
	public class SearchFittingResumeData
	{
		private bool m_ResumeAll_DataPresent = false;
		private bool m_ResumeAll_DataComplete = false;
		private double[] m_ResumeAll_LastCompletePhiData;
		private double[] m_ResumeAll_LastCompletePsiData;

		private bool m_ResumeLoop_DataPresent = false;
		private bool m_ResumeLoop_DataComplete = false;
		private double[] m_ResumeLoop_LastCompletePhiData;
		private double[] m_ResumeLoop_LastCompletePsiData;

		private FileInfo m_ParseFile = null;

		public SearchFittingResumeData( string filename )
		{
			m_ParseFile = new FileInfo( filename );
		}

		public void ObtainResumeData( int expectedAngleCount, char expectedResidue )
		{
			Console.WriteLine("Resume data is being collected....");

			// EXAMPLE LINES ...
			// Y,All,-136.083966447778,152.223021068016,-115.126019354735,112.602903320036,-78.5134443817723,140.01752254391,-101.706744263976,5.92922293088828,-63.3951835724968,-40.9191830760888,56.1256006870538,39.9488578091603,-111.351735823719,-27.0327131373494,70.4730160715864,-1.78821070831651,70.2130683244732,-164.8053902715,10901578.6372783
			// Y,All,-147.674988339737,155.830462641236,-114.294306067188,100.206550387063,-74.1893140448672,138.52750908445,-63.542220890076,-40.9004076753745,-100.99433879717,6.93817101392808,59.5961223752685,32.8086599578655,63.3685015630296,-113.993916552279,-117.705005324075,142.399278351991,-111.293479960316,-21.2551145105414,9256868.41089265
			// Y,All,-118.743479680383,148.906959678702,-114.701894600504,119.517440561213,-149.18831448289,155.814503125993,-64.1786843886035,-41.2437562368549,-102.545092374573,-3.08315747817192,59.6026407704235,32.8140371652851,-119.915618987482,59.704738163019,-74.0592128883857,140.180812716336,67.8300654968854,-105.678116506514,9225239.23163346
			// Y,All,-117.041030229787,120.445402893212,-73.342773555239,138.023767328133,-118.525586877962,149.482553378205,-64.2453453767789,-41.2100137633784,-102.671359395688,-2.8007871556565,59.600939814514,32.8071304505259,59.9666931193633,-115.204061833538,-118.705460558741,61.1334660032921,-148.320426046765,157.472024607459,9030201.46448113
			// Y,All,-117.276139665058,120.497019705594,-73.3928860856187,137.664509849001,-117.026818284778,149.386883222678,-64.2424309697199,-41.2119640597198,-102.639840757772,-2.94397327161578,59.5991739191111,32.854583551574,59.9388772435203,-115.243537059633,-118.509390030293,60.9223304362607,-148.409822304459,157.54877335045,9027957.23541827
			// Y,All,-147.946270591321,155.814342630708,-114.420195076578,101.637916383398,-74.2616426175282,138.498636930249,-62.3975967559489,-42.6455762242646,-89.1605726735017,-20.6781636477346,59.5981153226914,32.816615599355,-110.53621968489,9.36604490730262,63.3025930839603,-113.962149143434,-117.860966876783,143.326131477871,8939911.70573867
			// Time taken : ALL 9_Y_1_-1 : 15:51:02.1406250

			// NOTE : As we use Flush() when writing to the file during anglefitting, we garuntee that the file contains complete lines...
			// Resume therefore just needs the last line from the All and Loop Sets...

			StreamReader resumeStream = null;
			string lastLoopLine = null;
			string lastAllResLine = null;

			try // to obtain the last fill line deposited for both the allres and loop modes
			{
				resumeStream = new StreamReader( m_ParseFile.FullName );

				string line;
				while( null != ( line = resumeStream.ReadLine() ) )
				{
					if( line.Length > 6 && String.Compare( line, 1, ",All,", 0, 5, true ) == 0 )
					{
						lastAllResLine = line;
						m_ResumeAll_DataPresent = true;
					}
					else if( line.Length > 12 && String.Compare( line, 1, ",LoopsOnly,", 0, 11, true ) == 0 )
					{
						lastLoopLine = line;
						m_ResumeLoop_DataPresent = true;
					}
					else if( line.Length > 16 && String.Compare( line, 0, "Time taken : ALL", 0, 16, true ) == 0 )
					{
						m_ResumeAll_DataComplete = true;
					}
					else if( line.Length > 17 && String.Compare( line, 0, "Time taken : LOOP", 0, 17, true ) == 0 )
					{
						m_ResumeLoop_DataComplete = true;
					}
				}
			}
			catch
			{
				Console.WriteLine("ERROR : Exception during resumeStream parsing");
			}
			finally
			{
				if( resumeStream != null )
				{
					resumeStream.Close();
				}
			}


            if( lastAllResLine != null )
			{
				string[] lineParts = lastAllResLine.Split(',');

				// begin validation procedures
				if( lineParts.Length != ((expectedAngleCount*2)+3) )
				{
					throw new Exception("Parseline did not contain the correct number of entries");
				}
				// NOTE : CHARs_CASE HERE IN "expectedResidue" is important : cis vs. trans
				if( (lineParts[0].Length != 1) || (lineParts[0][0] != expectedResidue ) )
				{
					throw new Exception("Parseline did not contain the correct expectedResidue");
				}
				// end validation, parse the data

				m_ResumeAll_LastCompletePhiData = new double[ expectedAngleCount ];
				for( int i = 0; i < expectedAngleCount; i++ )
				{
					m_ResumeAll_LastCompletePhiData[i] = double.Parse( lineParts[ (i*2) + 2 ] );
				}

				m_ResumeAll_LastCompletePsiData = new double[ expectedAngleCount ];
				for( int i = 0; i < expectedAngleCount; i++ )
				{
					m_ResumeAll_LastCompletePsiData[i] = double.Parse( lineParts[ (i*2) + 3 ] );
				}
			}

			if( lastLoopLine != null )
			{
				string[] lineParts = lastLoopLine.Split(',');

				// begin validation procedures
				if( lineParts.Length != ((expectedAngleCount*2)+3) )
				{
					throw new Exception("Parseline did not contain the correct number of entries");
				}
				// NOTE : CHARs_CASE HERE IN "expectedResidue" is important : cis vs. trans
				if( (lineParts[0].Length != 1) || (lineParts[0][0] != expectedResidue ) )
				{
					throw new Exception("Parseline did not contain the correct expectedResidue");
				}
				// end validation, parse the data

				m_ResumeLoop_LastCompletePhiData = new double[ expectedAngleCount ];
				for( int i = 0; i < expectedAngleCount; i++ )
				{
					m_ResumeLoop_LastCompletePhiData[i] = double.Parse( lineParts[ (i*2) + 2 ] );
				}

				m_ResumeLoop_LastCompletePsiData = new double[ expectedAngleCount ];
				for( int i = 0; i < expectedAngleCount; i++ )
				{
					m_ResumeLoop_LastCompletePsiData[i] = double.Parse( lineParts[ (i*2) + 3 ] );
				}
			}

			// report to the user ...
			if( m_ResumeAll_DataComplete )
			{
				Console.WriteLine("Complete data found for allres");				
			}
			else if( m_ResumeAll_DataPresent )
			{
				Console.WriteLine("Resume data found for allres");
			}
			else
			{
				Console.WriteLine("No data found for allres");
			}

			if( m_ResumeLoop_DataComplete )
			{
				Console.WriteLine("Complete data found for loopres");				
			}
			else if( m_ResumeLoop_DataPresent )
			{
				Console.WriteLine("Resume data found for loopres");
			}
			else
			{
				Console.WriteLine("No data found for loopres");
			}
		}

		public bool HasLoopFitData
		{
			get
			{
				return m_ResumeLoop_DataPresent;
			}
		}

		public bool HasAllResFitData
		{
			get
			{
				return m_ResumeAll_DataPresent;
			}
		}

		public bool HasCompleteLoopFitData
		{
			get
			{
				return m_ResumeLoop_DataComplete;
			}
		}

		public bool HasCompleteAllResFitData
		{
			get
			{
				return m_ResumeAll_DataComplete;
			}
		}

		public double[] ResumeAllResPhi
		{
			get
			{
				return m_ResumeAll_LastCompletePhiData;
			}
		}

		public double[] ResumeAllResPsi
		{
			get
			{
				return m_ResumeAll_LastCompletePsiData;
			}
		}

		public double[] ResumeLoopPhi
		{
			get
			{
				return m_ResumeLoop_LastCompletePhiData;
			}
		}

		public double[] ResumeLoopPsi
		{
			get
			{
				return m_ResumeLoop_LastCompletePsiData;
			}
		}
	}
}
