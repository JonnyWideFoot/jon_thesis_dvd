using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;


using UoB.Research.Modelling.Structure;
using UoB.Comms.ServerSide;
using UoB.Research.Primitives;
using UoB.Research.FileIO.PDB;


namespace UoB.Comms
{
	//CommNotes

	// Unknown
	// 101 Null CommNote / Unknown CommNote type recieved
	// 102 Keep Alive - "Im still here, but nothing to say"
	// 103 UserInfoString - will be first recieved CommNote
	// 104 Im buggering off .. Exit Code .. Exit Gracefully
	// 105 Server Total Connected User Allocation Full

	// Messaging
	// 201 PopupBox
	// 202 Reporter String

	// PDB
	// 301 SendAllAtoms
	// 302 RefreshAllPositions

	// 400 Generic Graph Window Definitons



	public abstract class CommNote
	{

		protected byte[] m_Type;
		protected byte[] m_Data;
		protected int m_typeID;
		protected byte[] m_TerminalCode;

		public CommNote(int i)
		{
			setType(i);			
		}

		private void setType(int i)
		{
			m_typeID = i;
			m_Type = new byte[i.ToString().Length];
			m_Type = Encoding.ASCII.GetBytes(i.ToString());
			m_TerminalCode = Encoding.ASCII.GetBytes("#END#");
		}

		public int typeID
		{
			get { return m_typeID; }
		}

		public virtual void Execute(ConnectionManager targetClient)
		{
		}

		public static CommNote process(string theNoteString)
		{
			try
			{

				if( theNoteString.Length < 3) // Error in packet
				{
					return new CommNote_Null();
				}

				string typeString = theNoteString.Substring(0, 3 );
				string dataString = theNoteString.Substring(3, (theNoteString.Length -3) );

				//Do something based on the found CommNote type code
				switch(int.Parse(typeString))
				{
					case 101:
						return new CommNote_Null();
					case 102:
						return new CommNote_KeepAlive();
					case 103:
						return new CommNote_UserInfo(dataString);
					case 104:
						return new CommNote_KillCode();
					case 202:
						return new CommNote_Reporter(dataString);
					case 301:
						return new CommNote_SendAllAtoms(dataString);
					case 302:
						return new CommNote_SendAllPositions(dataString);
					default:
						return new CommNote_Null();
						//unknown CommNote
				}
			}
			catch(Exception e)
			{
				string error = e.ToString();
				// do something with error ..
				return new CommNote_Null();
			}

		}
		public byte[] toByteArray()
		{
			ArrayList theBytes = new ArrayList();
			foreach(byte theByte in m_Type)
			{
				theBytes.Add(theByte);
			}
			foreach(byte theByte in m_Data)
			{
				theBytes.Add(theByte);
			}
			foreach(byte theByte in m_TerminalCode)
			{
				theBytes.Add(theByte);
			}
			return (byte[]) theBytes.ToArray( typeof(byte) );
		}

	}

	public class CommNote_Null : CommNote
	{
		public CommNote_Null() : base(101)
		{
			m_Data = new byte[0];
		}
	}

	public class CommNote_KeepAlive : CommNote
	{
		public CommNote_KeepAlive() : base(102)
		{
			m_Data = new byte[0];
		}

		public override void Execute(ConnectionManager target)
		{
			target.KeepAlive();
		}
	}

	public class CommNote_UserInfo : CommNote
	{
		private string m_CommNoteString;

		private string m_UserName;
		private string m_UserInfo;

		public override void Execute(ConnectionManager target)
		{
			target.ClientInfo.Username = m_UserName;
			target.ClientInfo.UserInfoString = m_UserInfo;
			target.TriggerClientInfoUpdate();
		}

		public CommNote_UserInfo(string theCommNote) : base(103)
		{
			m_CommNoteString = theCommNote;
			string[] info = m_CommNoteString.Split(':');
			m_UserName = info[0];
			m_UserInfo = info[1];
			m_Data = Encoding.ASCII.GetBytes(theCommNote);
		}

		public string CommNoteString
		{
			get 
			{
				return m_CommNoteString;
			}
		}
	}

	public class CommNote_KillCode : CommNote
	{
		public CommNote_KillCode() : base(104)
		{
			m_Data = new byte[0];
		}

		public override void Execute(ConnectionManager targetClient)
		{
			targetClient.Terminate();
		}
	}

	public class CommNote_Reporter : CommNote
	{
		private string m_ReporterString;

		public CommNote_Reporter(string theCommNote) : base(202)
		{
			m_ReporterString = theCommNote;
			m_Data = Encoding.ASCII.GetBytes(theCommNote);
		}

		public string ReporterString
		{
			get 
			{
				return m_ReporterString;
			}
		}

		public override void Execute(ConnectionManager targetConnection)
		{
			ChatItem c = new ChatItem(ChatItemType.InfoMessage, m_ReporterString);
			targetConnection.Report(c);			
		}
	}

	public class CommNote_SendAllAtoms : CommNote
	{
		public override void Execute(ConnectionManager targetConnection)
		{
			PDB m_PDB = WriteTempFile ( Encoding.ASCII.GetString(m_Data).Split(':') );
			targetConnection.particleSystem = m_PDB.particleSystem;
		}

		public PDB WriteTempFile (string[] PDBLines)
		{
			int i = 0;
			string filePath = System.IO.Path.GetTempPath() + @"\DAVEConnection_" + i.ToString() + ".pdb";
			
			while ( File.Exists( filePath ) )
			{
				i++;
				filePath = System.IO.Path.GetTempPath() + @"\DAVEConnection_" + i.ToString() + ".pdb";
			}
			
			StreamWriter rw = new StreamWriter(filePath, false);


			foreach ( string s in PDBLines )
			{
				if ( s.Length != 0 ) rw.WriteLine( RecievedStringToPDBString( s ) );
			}

			rw.Close();

			return new PDB(filePath, true);
		}

		public CommNote_SendAllAtoms( string theCommNote ) : base(301)
		{
			m_Data = Encoding.ASCII.GetBytes(theCommNote);
		}

		public static string RecievedStringToPDBString(string theString )
		{
			// ATOM   1076  N   LEU A 316      59.941  10.539  13.317  1.00 57.04           N  
			StringBuilder s = new StringBuilder(80);
			s.Append( "ATOM  " );
			s.Append( theString.Substring(0,4).PadLeft(5,' ') ); //Atom Number
			s.Append( theString.Substring(4,3).PadLeft(5,' ') ); // PDB ID
			s.Append( theString.Substring(7,3).PadLeft(4,' ') ); // Parent Molecule Type
			s.Append( theString.Substring(10,1).PadLeft(2,' ') ); // Chain ID
			s.Append( theString.Substring(11,3).PadLeft(4,' ') ); // Residue Number
			s.Append( "    " );
			s.Append( theString.Substring(14,7).PadLeft(8,' ') ); // x
			s.Append( theString.Substring(21,7).PadLeft(8,' ') ); // y
			s.Append( theString.Substring(28,7).PadLeft(8,' ') ); // z

			return s.ToString().PadRight(80,' ');
		}

		public static string MakeSingleAtomSendString(Atom a)
		{
			//1076  NLEUA316 59.941 10.539 13.317
			string temp;
			string returnString = "";
            
			temp = a.AtomNumber.ToString().PadLeft(4);
				returnString += temp ;
			temp = a.PDBType.PadLeft(3);
				returnString += temp ;
			temp = a.parentMolecule.moleculePrimitive.MolName.PadLeft(3,' ');
				returnString += temp ;
			temp = a.parentMolecule.parentChainID.ToString();
				returnString += temp ;
			temp = a.parentMolecule.ResidueNumber.ToString().PadLeft(3,' ');
				returnString += temp ;
			temp = a.xFloat.ToString("0.000").PadLeft(7,' ');
				returnString += temp ;
			temp = a.yFloat.ToString("0.000").PadLeft(7,' ');
				returnString += temp ;
			temp = a.zFloat.ToString("0.000").PadLeft(7,' ');
				returnString += temp ;

			if ( returnString.Length != 35 ) throw new Exception();
			return returnString;
		}

		public CommNote_SendAllAtoms( ParticleSystem ps ) : base(301)
		{
			StringBuilder builder = new StringBuilder(30000);

			bool done = false;
			while(!done)
			{  
				try
				{
					ps.AcquireReaderLock(1000);
					try
					{
						// It is safe for this thread to read from
						// the shared resource.

						for ( int i = 0; i < ps.Count; i++ )
						{
							builder.Append( MakeSingleAtomSendString( ps[i] ) );
							builder.Append(":");
						}
						m_Data = Encoding.ASCII.GetBytes( builder.ToString() );

						done = true;

					}        
					finally
					{
						// Ensure that the lock is released.
						ps.ReleaseReaderLock();
					}
				}
				catch (ApplicationException)
				{
					// The reader lock request timed out.
				}
			}
		}
	}


	
	public class CommNote_SendAllPositions : CommNote
	{
		public override void Execute(ConnectionManager targetConnection)
		{
			string[] positionStrings = Encoding.ASCII.GetString(m_Data).Split(':');
			Vector[] thePositions = new Vector[ positionStrings.Length ];
			for ( int i = 0; i < positionStrings.Length; i++ )
			{
				thePositions[i] = RecievedStringToVector( positionStrings[i] );
			}											   
			targetConnection.UpdateAtomPositions( thePositions );
		}


		public CommNote_SendAllPositions( string theCommNote ) : base(302)
		{
			m_Data = Encoding.ASCII.GetBytes(theCommNote);
		}

		public CommNote_SendAllPositions( ParticleSystem ps ) : base(302)
		{
			StringBuilder builder = new StringBuilder(30000);


			bool done = false;
			while(!done)
			{  
				try
				{
					ps.AcquireReaderLock(1000);
					try
					{
						// It is safe for this thread to read from
						// the shared resource.

						for ( int i = 0; i < ps.Count -1 ; i++ ) // -1 as we want to ignore the trailing ":"
						{
							builder.Append( MakeSinglePositionString( ps[i] ) );
							builder.Append(":");
						}
						builder.Append( MakeSinglePositionString( ps[ps.Count-1] ) );
						// Dont want a trailing ":"
						m_Data = Encoding.ASCII.GetBytes( builder.ToString() );

						done = true;

					}        
					finally
					{
						// Ensure that the lock is released.
						ps.ReleaseReaderLock();
					}
				}
				catch (ApplicationException)
				{
					// The reader lock request timed out.
				}
			}
		}

		public static Vector RecievedStringToVector( string theString )
		{
			if ( theString.Length != 21 ) throw new ArgumentException();
			float x = float.Parse( theString.Substring(0,7) );
			string Y = theString.Substring(7,7);
			float y = float.Parse( Y );
			float z = float.Parse( theString.Substring(14,7) );
			return new Vector(x,y,z);
		}

		public static string MakeSinglePositionString(Atom a)
		{
			string returnString = "";
            
			returnString += a.xFloat.ToString("0.000").PadLeft(7,' ');
			returnString += a.yFloat.ToString("0.000").PadLeft(7,' ');
			returnString += a.zFloat.ToString("0.000").PadLeft(7,' ');

			if ( returnString.Length != 21 ) throw new Exception();
			return returnString;
		}

	}


}
