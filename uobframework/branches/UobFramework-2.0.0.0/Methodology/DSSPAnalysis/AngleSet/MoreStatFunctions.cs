
// these are very broken: use as code snippets only ....

class xxxx
{
	public void LoopLengthCounting()
	{
		int[] loopLengthCounts = new int[150]; 
		// find loops of length 0 to 150, the integers are incremented when a 
		// loop of that length is found ...

		ParsingFileIndex = 0; // reset IMPORTANT
		while( true )
		{
			CurrentFile.CountLoopLengths(loopLengthCounts);
			// increment conidtion
			if( ParsingFileIndex < FileCount - 1 )
			{
				ParsingFileIndex++;
			}
			else
			{
				break;
			}
		}

		// write the derived stats to the report directory ...
		StreamWriter rw = new StreamWriter( reportDirectory.FullName + "_LoopLengthStats.csv" );
		for( int i = 0; i < loopLengthCounts.Length; i++ )
		{
			rw.Write( i );
			rw.Write( ',' );
			rw.WriteLine( loopLengthCounts[i] );
		}
		rw.Close();
	}

	public void CountDisallowed()
	{
		int goodLoops = 0;
		int badLoops = 0;
		int goodResiduesInLoops = 0; 
		int badResiduesInLoops = 0;

		ParsingFileIndex = 0; // reset IMPORTANT
		while( true )
		{
			CurrentFile.CountDisallowed( ref goodLoops, ref badLoops, 
				ref goodResiduesInLoops, ref badResiduesInLoops );
			// increment conidtion
			if( ParsingFileIndex < FileCount - 1 )
			{
				ParsingFileIndex++;
			}
			else
			{
				break;
			}
		}

		StreamWriter rw = new StreamWriter( reportDirectory.FullName + "_LoopWithDisallowedResiduesStats.csv" );
		rw.WriteLine( goodLoops );
		rw.WriteLine( badLoops );
		rw.WriteLine( goodResiduesInLoops );
		rw.WriteLine( badResiduesInLoops );

		float f_goodLoops = (float) goodLoops;
		float f_badLoops = (float) badLoops;
		float f_goodResiduesInLoops = (float) goodResiduesInLoops;
		float f_badResiduesInLoops = (float) badResiduesInLoops;

		rw.WriteLine( (f_badLoops / ( f_goodLoops + f_badLoops) ) * 100.0f );
		rw.WriteLine( (f_badResiduesInLoops / ( f_badResiduesInLoops + f_goodResiduesInLoops) ) * 100.0f );
		rw.Close();
	}

	public void DeviantAngleStats()
	{
		StreamWriter rw = new StreamWriter( reportDirectory.FullName + "_DeviantAngleAnalysis.csv" );
		for( float cutoff = 1.0f; cutoff < 180.0f; cutoff += 1.0f )
		{
			CurrentFile.PercentageLoopsWithDeviantAngleStats_InitialiseForCutoff();
				
			ParsingFileIndex = 0; // reset IMPORTANT
			while( true )
			{
				//CurrentFile.StatFinction();
				CurrentFile.PercentageLoopsWithDeviantAngleStats(m_AngleSet,cutoff);
				// increment conidtion
				if( ParsingFileIndex < FileCount - 1 )
				{
					ParsingFileIndex++;
				}
				else
				{
					break;
				}
			}

			CurrentFile.PercentageLoopsWithDeviantAngleStats_EndForCutoff( cutoff, rw );
			rw.Flush();
		}
		rw.Close();
	}

	public void FurtherstDistanceStats()
	{
		StandardResidues[] singleResTypes = GetSingleResTypes();
		for( int i = 12; i < 14; i++ )
		{
			char molID = singleResTypes[i].ToString()[0];
			StreamWriter rw = new StreamWriter(@"c:\Dist_" + molID + Char.IsUpper(molID).ToString()[0] + ".csv");
	
			rw.WriteLine( molID );
			// the phiData and psiData arrays
			ParsingFileIndex = 0; // reset IMPORTANT
			while( true )
			{
				//CurrentFile.StatFinction();
				CurrentFile.FurthestAngleStats(m_AngleSet,singleResTypes[i],rw);
				// increment conidtion
				if( ParsingFileIndex < FileCount - 1 )
				{
					ParsingFileIndex++;
				}
				else
				{
					break;
				}
			}

			rw.Close();
		}
	}


	public void CountDisallowed( ref int goodLoops, ref int badLoops, ref int goodResiduesInLoops, ref int badResiduesInLoops )
	{
		for( int i = 1; i < m_LoopDefs.Count -1; i++ ) // -1 +1 as we dont want the termini
		{
			LoopDef ld = (LoopDef) m_LoopDefs[i];
			if( ld.Length == -1 )
			{
				continue; // the loop is incomplete in the PDB, the phi and psi of the respective anchors will be 360
			}
                
			bool flagBad = false;
			for( int j = 0; j < ld.Length; j++ )
			{
				ResidueDef res = ld[j];
				if( res.IsDisallowed )
				{
					flagBad = true;
					badResiduesInLoops++;
				}
				else
				{
					goodResiduesInLoops++;
				}
			}
			if( flagBad )
			{
				badLoops++;
			}
			else
			{
				goodLoops++;
			}
		}
	}

	public void CountLoopLengths( int[] loopLengths )
	{
		for( int i = 1; i < m_LoopDefs.Count -1; i++ ) // -1 +1 as we dont want the termini
		{
			LoopDef ld = (LoopDef) m_LoopDefs[i];
			if( ld.Length == -1 )
			{
				continue; // the loop is incomplete in the PDB, the phi and psi of the respective anchors will be 360
			}
                
			if( ld.Length > 50 )
			{
				for( int j = 0; j < ld.Length; j++ )
				{
					ResidueDef res = ld[j];
					if( res.AminoAcidID != 'G' )
					{
						if( res.Phi < 64.0 && res.Phi > 56.0 &&
							res.Psi < -115 && res.Psi > -135 )
						{
							break;
						}
					}
				}
			}


			loopLengths[ ld.Length ]++;
		}
	}

	private int m_LoopAnglesFineCount;
	private int m_LoopAnglesDeviantCount;
	private int m_LoopContainsNonProCis;
	private int m_LoopUnknownResidues;
	private int m_LoopTotal;
	public void PercentageLoopsWithDeviantAngleStats_InitialiseForCutoff()
	{
		m_LoopAnglesFineCount = 0;
		m_LoopAnglesDeviantCount = 0;
		m_LoopContainsNonProCis = 0;
		m_LoopUnknownResidues = 0;
		m_LoopTotal = 0;
	}

	public void PercentageLoopsWithDeviantAngleStats_EndForCutoff( double cutoff, StreamWriter deviantResults )
	{
		float good = (float)m_LoopAnglesFineCount;
		float total = (float)m_LoopTotal;
		float percentage = ( good / total ) * 100.0f;
		deviantResults.Write( cutoff );
		deviantResults.Write( ',' );
		deviantResults.Write( m_LoopContainsNonProCis );
		deviantResults.Write( ',' );
		deviantResults.Write( m_LoopUnknownResidues );
		deviantResults.Write( ',' );
		deviantResults.Write( m_LoopAnglesFineCount );
		deviantResults.Write( ',' );
		deviantResults.Write( m_LoopAnglesDeviantCount );
		deviantResults.Write( ',' );
		deviantResults.Write( m_LoopTotal );
		deviantResults.Write( ',' );
		deviantResults.WriteLine( percentage );
			
	}
	public void PercentageLoopsWithDeviantAngleStats( AngleSet angleSet, double cutoff )
	{
		bool XFlag;
		bool CisFlag;
		bool DeviantFlag;

		for( int i = 1; i < m_LoopDefs.Count -1; i++ ) // -1 + 1 as we dont want the termini
		{
			// reset flags per loop
			XFlag = false;
			CisFlag = false;
			DeviantFlag = false;

			LoopDef ld = (LoopDef) m_LoopDefs[i];
			if( ld.Length == -1 )
			{
				continue; // the loop is incomplete in the PDB, the phi and psi of the respective anchors will be 360
			}

			for( int j = 0; j < ld.Length; j++ )
			{
				char molID = ld.Sequence[j];
				if( molID == 'X' )
				{
					XFlag = true;
					break;
				}
				if( Char.IsLower(molID) && molID != 'p' )
				{
					CisFlag = true; // its a non-pro cis residue
					break;
				}
				double dist = angleSet.ClosestDistanceTo( molID, ld.GetPhi(j),ld.GetPsi(j) );
				if( cutoff < dist )
				{
					DeviantFlag = true;
				}
			}

			if( XFlag )
			{
				m_LoopUnknownResidues++;
			}
			if( CisFlag )
			{
				m_LoopContainsNonProCis++; // its an unknown residue
			}
			if( DeviantFlag )
			{
				m_LoopAnglesDeviantCount++;
			}
			if( !XFlag && !CisFlag && !DeviantFlag )
			{
				m_LoopAnglesFineCount++;
			}
			m_LoopTotal++;
		}
	}

	public void FurthestAngleStats( AngleSet raft, StandardResidues dsspResID, StreamWriter writeClosestDistTo )
	{
		throw new Exception("Need to check for X residues and cis residues that are not proline");
			
		//			char molID = dsspResID.ToString()[0];
		//			for( int i = 1; i < m_LoopDefs.Count -1; i++ )
		//			{
		//				LoopDef ld = (LoopDef) m_LoopDefs[i];
		//				if( ld.Length == -1 )
		//				{
		//					continue; // the loop is incomplete in the PDB, the phi and psi of the respective anchors will be 360
		//				}
		//				for( int j = 0; j < ld.Length; j++ )
		//				{
		//					char checkID = ld.Sequence[j];
		//					if( IsResTypeMatch( dsspResID, checkID ) ) // checkID is now case sensitive
		//					{
		//						writeClosestDistTo.WriteLine( 
		//							raft.ClosestDistanceTo( molID,ld.GetPhi(j),ld.GetPsi(j) ) 
		//							);
		//					}
		//				}
		//			}
	}
}