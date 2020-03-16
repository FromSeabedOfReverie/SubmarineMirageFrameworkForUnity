//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Extension {
	using System.Collections.Generic;


	// TODO : コメント追加、整頓


	public static class ListExtension {
		public static List<T> Clone<T>( this List<T> self ) {
			return new List<T>( self );
		}

		public static List<T> ReverseByClone<T>( this List<T> self ) {
			return new List<T>( self );
		}
	}
}