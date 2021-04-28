using System;
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace UoB.Core.FileIO.FASTA
{
	/// <summary>
	/// Summary description for FASTA.
	/// </summary>
	public sealed class FASTA
	{
		private ArrayList m_Names;
		private ArrayList m_Sequences;

		public FASTA()
		{
			m_Names = new ArrayList();
			m_Sequences = new ArrayList();
		}

		public void AddSequence( string name, string singleLetterSequence )
		{
			if( name == null || singleLetterSequence == null )
			{
                throw new Exception("The name and sequence data cannot be null");
			}	
			m_Names.Add( name );
			m_Sequences.Add( singleLetterSequence );
		}

		public string Contents
		{
			get
			{
				string returnString = "";

				for( int i = 0; i < m_Names.Count; i++ )
				{
					//first write the FASTA name line
					string name = '>' + (string) m_Names[i];
					if( name.Length > 70 ) 
					{
						Trace.WriteLine("Warning : FASTA name lines cannot be longer than 70 characters, the name line was truncated");
						Trace.WriteLine("NameLine : " + name );
						name = name.Substring(0,70);
					}
					returnString += name;
					returnString += "\r\n";

					// nest write the sequence line
					string seq = (string) m_Sequences[i];
					if( seq.Length <= 70 )
					{
						returnString += seq;
						returnString += "\r\n";
					}
					else
					{
						while( seq.Length > 70 )
						{
							returnString +=  seq.Substring(0,70);
							returnString += "\r\n";
							seq = seq.Substring(70, seq.Length - 70 );
							if( seq.Length <= 70 )
							{
								returnString += seq;
								returnString += "\r\n";
								break;
							}
						}
					}
					
					// write a blank line, cos it looks nicer
					returnString += "\r\n";
				}

                return returnString;                
			}
		}

		public void WriteFile( string fullFileName )
		{

			StreamWriter rw = new StreamWriter(fullFileName);

			for( int i = 0; i < m_Names.Count; i++ )
			{
				//first write the FASTA name line
				string name = '>' + (string) m_Names[i];
				if( name.Length > 70 ) 
				{
					Trace.WriteLine("Warning : FASTA name lines cannot be longer than 70 characters, the name line was truncated");
					Trace.WriteLine("NameLine : " + name );
					name = name.Substring(0,70);
				}
				rw.WriteLine( name );

				// nest write the sequence line
				string seq = (string) m_Sequences[i];
				if( seq.Length <= 70 )
				{
					rw.WriteLine(seq);
				}
				else
				{
					while( seq.Length > 70 )
					{
						rw.WriteLine( seq.Substring(0,70) );
						seq = seq.Substring(70, seq.Length - 70 );
						if( seq.Length <= 70 )
						{
							rw.WriteLine(seq);
							break;
						}
					}
				}
					
				// write a blank line, cos it looks nicer
				rw.WriteLine();
			}
			
			// done, close the file handle
			rw.Close();
		}
	}
}
