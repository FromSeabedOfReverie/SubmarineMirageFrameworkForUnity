//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System;
	using System.Collections.Generic;
	using KoganeUnityLib;



	public static class DictionarySMExtension {
		public static bool DisposeAndRemove<TKey, TValue>( this Dictionary<TKey, TValue> self, TKey key )
			where TValue : IDisposable
		{
			var value = self.GetOrDefault( key );
			if ( value == null )	{ return false; }

			value.Dispose();
			return self.Remove( key );
		}
	}
}