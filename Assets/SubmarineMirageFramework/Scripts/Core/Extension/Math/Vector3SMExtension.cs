//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using UnityEngine;
	using Utility;


	// TODO : コメント追加、整頓


	public static class Vector3SMExtension {
		public static Vector3 Mult( this Vector3 self, Vector3 v ) => new Vector3(
			self.x * v.x,
			self.y * v.y,
			self.z * v.z
		);

		public static Vector3 Div( this Vector3 self, Vector3 v ) => new Vector3(
			self.x / v.x,
			self.y / v.y,
			self.z / v.z
		);
	}
}