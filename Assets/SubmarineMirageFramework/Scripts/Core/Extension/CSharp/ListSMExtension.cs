//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using System.Collections.Generic;


	// TODO : コメント追加、整頓


	public static class ListSMExtension {
		public static List<T> Copy<T>( this List<T> self )
			=> new List<T>( self );

		public static List<T> ReverseByCopy<T>( this List<T> self ) {
			var result = self.Copy();
			result.Reverse();
			return result;
		}
	}
}