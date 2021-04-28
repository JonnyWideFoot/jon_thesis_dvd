using System;
using System.IO;
using System.Text;
using System.Drawing;

using UoB.CoreControls.PS_Render;
using UoB.Core.Primitives;
using UoB.Core.Structure;

namespace UoB.CoreControls.ToolWindows
{
	/// <summary>
	/// Summary description for SelectionDefFile.
	/// </summary>
	public class SelectionDefFile
	{
		public SelectionDefFile()
		{
		}

		public static void SaveNew( string fileName, ParticleSystemDrawWrapper wrapper )
		{
			try
			{
				StreamWriter rw = new StreamWriter( fileName, false );

				ParticleSystem ps = wrapper.particleSystem;
				FocusDefinition focDef = ps.SystemFocus;

				rw.Write( "CENTERING " );
				rw.Write( (int)focDef.Mode );
				if( focDef.Mode == FocusingMode.FocusByResidue )
				{
					rw.Write(' ');
					rw.Write( (int)focDef.FocusMember.ArrayIndex );
					rw.Write(' ');
					rw.Write( (int)focDef.ResidueIndex );
				}
				else if( focDef.Mode == FocusingMode.GeometricFocus )
				{
					// nothing extra required
				}
				else
				{
					throw new Exception("CODE not implemented ...");
				}
				rw.WriteLine();

				Selection[] sels = wrapper.Selections;
				for( int i = 0; i < sels.Length; i++ )
				{
					switch( sels[i].GetType().ToString() )
					{
						case "UoB.CoreControls.PS_Render.AminoAcidSelection":
							WriteSelection( rw, (AminoAcidSelection)sels[i] );
							break;
						case "UoB.CoreControls.PS_Render.GlobalSelection":
							WriteSelection( rw, (GlobalSelection)sels[i] );
							break;
						case "UoB.CoreControls.PS_Render.Selection_CAlphaEquiv":
							WriteSelection( rw, (Selection_CAlphaEquiv)sels[i] );
							break;
						default:
							break;
					}
				}
				rw.Close();
			}
			catch
			{
			}
		}

		public static void WriteSelection( StreamWriter rw, AminoAcidSelection s )
		{
			rw.Write("AASELECT ");

			rw.Write( s.Molecule.ArrayIndex );
			rw.Write(' ');

			rw.Write( s.Start );
			rw.Write(' ');

			rw.Write( s.Length );
			rw.Write(' ');

			rw.Write( s.Colour1.IntegerRep.ToString() );
			rw.Write(' ');

			rw.Write( s.Colour2.IntegerRep.ToString() );
			rw.Write(' ');

			rw.Write( (int) s.ColourMode );
			rw.Write(' ');

			rw.Write( (int)s.DrawStyle );
			rw.Write(' ');

			rw.Write( (int) s.DisplayMode );
	
			rw.WriteLine();
		}

		public static void WriteSelection( StreamWriter rw, Selection_CAlphaEquiv s )
		{
			rw.Write("CALPSELECT");
			rw.WriteLine();
		}
		
		public static void WriteSelection( StreamWriter rw, GlobalSelection s )
		{
            rw.Write("GLOBAL ");

			rw.Write( s.Colour1.IntegerRep.ToString() );
			rw.Write(' ');

			rw.Write( s.Colour2.IntegerRep.ToString() );
			rw.Write(' ');
			
			rw.Write( (int) s.ColourMode );
			rw.Write(' ');

			rw.Write( (int) s.DrawStyle );
			rw.Write(' ');

			rw.Write( (int) s.DisplayMode );
	
			rw.WriteLine();
		}

		public static void LoadTo( string fileName, ParticleSystemDrawWrapper wrapper )
		{
			try
			{
				wrapper.BeginSelectionEdit();
				wrapper.ClearSlections();

				StreamReader re = new StreamReader( fileName );
				string line;
				while( null != ( line = re.ReadLine() ) )
				{
					string[] lineParts = line.Split(' ');
					if( lineParts.Length == 0 )
					{
						continue;
					}
					switch( lineParts[0] )
					{
						case "CENTERING":
						    TrySetCentering( lineParts, wrapper );
							break;
						case "GLOBAL":
							TrySetGlobal( lineParts, wrapper );
							break;
						case "AASELECT":
							TrySetAARange( lineParts, wrapper );
							break;
						case "CALPSELECT":
							TrySetCALPRange( lineParts, wrapper );
							break;
						default:
							break;
					}
				}
				re.Close();

				wrapper.EndSelectionEdit();
			}
			catch
			{
			}
		}

		private static void TrySetCentering( string[] parseElements, ParticleSystemDrawWrapper dw )
		{
			ParticleSystem ps = dw.particleSystem;
			try
			{
				int modeInt = int.Parse( parseElements[1] );
				FocusingMode mode = (FocusingMode) modeInt;

				switch( mode )
				{
					case FocusingMode.FocusByResidue:
						int residueIndex = int.Parse( parseElements[2] );
						int moleculeIndex = int.Parse( parseElements[3] );
						ps.SystemFocus.SetResidueFocusing( residueIndex, moleculeIndex );
						break;
					default: // default to geometric focusing
						ps.SystemFocus.SetGeometricFocusing(); 
						break;
				}				
			}
			catch
			{
				ps.SystemFocus.SetGeometricFocusing(); 
			}
		}

		private static void TrySetCALPRange( string[] parseElements, ParticleSystemDrawWrapper dw )
		{
		}

		private static void TrySetGlobal( string[] parseElements, ParticleSystemDrawWrapper dw )
		{
			ParticleSystem ps = dw.particleSystem;
			if( parseElements.Length == 6 )
			{
				GlobalSelection a = (GlobalSelection) dw.SelectionAt(0);
				a.Colour1.IntegerRep = int.Parse(parseElements[1]);
				a.Colour2.IntegerRep = int.Parse(parseElements[2]);
				a.ColourMode = (SelectionColourMode) int.Parse( parseElements[3] );
				a.DrawStyle = (AtomDrawStyle) int.Parse( parseElements[4] );
				a.DisplayMode = (AtomDisplayMode) int.Parse( parseElements[5] );
			}
			
		}
		private static void TrySetAARange( string[] parseElements, ParticleSystemDrawWrapper dw )
		{
			try
			{
				ParticleSystem ps = dw.particleSystem;
				if( parseElements.Length == 9 )
				{
					int molIndex = int.Parse(parseElements[1]);
					int startAt = int.Parse(parseElements[2]);
					int endAt = int.Parse(parseElements[3]);

					AminoAcidSelection a = new AminoAcidSelection( ps.MemberAt(molIndex),startAt,endAt);
					
					a.Colour1.IntegerRep = int.Parse(parseElements[4]);
					a.Colour2.IntegerRep = int.Parse(parseElements[5]);
					a.ColourMode = (SelectionColourMode) int.Parse( parseElements[6] );
					a.DrawStyle = (AtomDrawStyle) int.Parse( parseElements[7] );
					a.DisplayMode = (AtomDisplayMode) int.Parse( parseElements[8] );

					dw.AddSelection( a );
				}			
			}
			catch( Exception ex )
			{
				string error = ex.ToString();
				return;
			}
		}
}
}
