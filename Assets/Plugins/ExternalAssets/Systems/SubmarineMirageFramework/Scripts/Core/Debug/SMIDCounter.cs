//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Debug {
	using System;
	using System.Collections.Generic;
	using KoganeUnityLib;
	using Base;



	public static class SMIDCounter {
		static readonly Dictionary<Type, uint> _idCounts = new Dictionary<Type, uint>();



		public static uint GetLastID( Type type )
			=> _idCounts.GetOrDefault( type );

		public static uint GetNewID( IBaseSM baseSM ) {
			var type = baseSM.GetType();
			return _idCounts[type] = GetLastID( type ) + 1;
		}
	}
}