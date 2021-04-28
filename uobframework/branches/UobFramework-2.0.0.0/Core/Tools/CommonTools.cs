using System;
using System.Reflection;
using System.Text.RegularExpressions;

using UoB.Core.Primitives.Collections;

namespace UoB.Core.Tools
{
	/// <summary>
	/// Summary description for Tools.
	/// </summary>
	public class CommonTools
	{
		private CommonTools()
		{
		}

		public static readonly Regex WhiteSpaceRegex = new Regex(@"\s+",RegexOptions.Compiled); // scans whitespace, used for the text splitting function

		public static float GetPercentage( int done, int total )
		{
			float doneF = (float) done;
			float totalF = (float) total;
			return ( doneF / totalF ) * 100.0f;
		}

		#region UnitConversions

		public static float degToRad( float deg )
		{
			return (float) (deg * (Math.PI / 180 ));
		}

		public static float radToDeg( float Rad )
		{
			return (float) ( Rad * (180 / Math.PI) );
		}

		#endregion

		#region SystemTypeTools

		[Obsolete]
		public static bool supportsInterface(string interfaceName, object theObject)
		{
			Type objType = theObject.GetType();
			Type theInterface = objType.GetInterface( interfaceName , true );
			return ( theInterface != null );
		}

		[Obsolete]
		public static bool derivesFrom( Type queryType, object theObject )
		{
			if ( theObject.GetType() == queryType ) return true;
			if ( theObject.GetType() == typeof(Object) ) return false;
			return derivesFrom( queryType, theObject.GetType().BaseType );
		}

		[Obsolete]
		private static bool derivesFrom( Type queryType, Type compareType )
		{
			if ( compareType == queryType ) return true;
			if ( compareType == typeof(Object) ) return false;
			return derivesFrom( queryType, compareType.BaseType );
		}

		#endregion

		#region FilenameTools

		private static char[] illegalChars = { '\\', '/', ':', '*', '?', '<', '>', '|' };
		public static string ReturnStringWithIllegalCharsFromFilenameRemoved( string name )
		{
			string returnString = "";
			foreach( char c in name )
			{
				bool isIllegal = false;
				for( int i = 0; i < illegalChars.Length; i++ )
				{
					if( c == ':' )
					{
						isIllegal = true;
						returnString += '-';
						break;
					}
					else if( c == illegalChars[i] )
					{
						isIllegal = true;
						break;
					}
				}
				if( !isIllegal )
				{
					returnString += c;
				}
			}
			return returnString;
		}

		#endregion
	}
}
