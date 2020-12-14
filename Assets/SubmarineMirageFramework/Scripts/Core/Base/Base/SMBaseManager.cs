//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Base {
	using System;
	using System.Collections.Generic;
	using KoganeUnityLib;
	using Singleton;


	// TODO : コメント追加、整頓


	public class SMBaseManager : RawSingleton<SMBaseManager> {
		readonly Dictionary<Type, uint> _idCounts = new Dictionary<Type, uint>();


		public uint GetLastID( Type type )
			=> _idCounts.GetOrDefault( type );

		public void SetID( ISMBase smBase ) {
			var type = smBase.GetType();
			smBase._id = _idCounts[type] = GetLastID( type ) + 1;
		}
	}
}