//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using System;
	using System.Collections.Generic;


	public static class ListSMExtension {
		public static List<T> Copy<T>( this List<T> self )
			=> new List<T>( self );

		public static List<T> ReverseByCopy<T>( this List<T> self ) {
			var result = self.Copy();
			result.Reverse();
			return result;
		}

		public static bool RemoveFind<T>( this List<T> self, Predicate<T> findEvent ) {
			var i = self.FindIndex( findEvent );
			if ( i == -1 )	{ return false; }

			self.RemoveAt( i );
			return true;
		}
	}
}