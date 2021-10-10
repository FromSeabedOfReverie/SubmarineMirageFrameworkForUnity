//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System;
	using System.Collections;
	using System.Collections.Generic;


	public static class IEnumerableSMExtension {
		public static void ForEachRaw( this IEnumerable self, Action<object> action ) {
			foreach ( var o in self ) {
				action( o );
			}
		}

		public static IEnumerable<T> SelectRaw<T>( this IEnumerable self, Func<object, T> selector ) {
			foreach ( var o in self ) {
				yield return selector( o );
			}
		}

		public static int CountRaw( this IEnumerable self ) {
			var i = 0;
			foreach ( var o in self ) {
				i++;
			}
			return i;
		}
	}
}