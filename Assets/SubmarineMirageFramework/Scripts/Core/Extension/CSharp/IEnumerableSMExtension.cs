//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using System;
	using System.Collections;
	using System.Collections.Generic;


	// TODO : コメント追加、整頓


	public static class IEnumerableSMExtension {
		public static void ForEach( this IEnumerable self, Action<object> action ) {
			foreach ( var o in self ) {
				action( o );
			}
		}

		public static IEnumerable<T> Select<T>( this IEnumerable self, Func<object, T> selector ) {
			foreach ( var o in self ) {
				yield return selector( o );
			}
		}

		public static int Count( this IEnumerable self ) {
			var i = 0;
			foreach ( var o in self ) {
				i++;
			}
			return i;
		}
	}
}