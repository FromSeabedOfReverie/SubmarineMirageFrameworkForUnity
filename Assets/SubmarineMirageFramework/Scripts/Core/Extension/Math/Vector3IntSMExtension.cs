//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using UnityEngine;


	// TODO : コメント追加、整頓


	public static class Vector3IntSMExtension {
		public static readonly Vector3Int s_positiveInfinity = new Vector3Int(
			int.MaxValue,
			int.MaxValue,
			int.MaxValue
		);
		public static readonly Vector3Int s_negativeInfinity = new Vector3Int(
			int.MinValue,
			int.MinValue,
			int.MinValue
		);


		public static Vector3Int Add( this Vector3Int self, int a ) => new Vector3Int(
			self.x + a,
			self.y + a,
			self.z + a
		);

		public static Vector3Int Sub( this Vector3Int self, int a ) => new Vector3Int(
			self.x - a,
			self.y - a,
			self.z - a
		);


		public static Vector3Int FloorToInt( this Vector3 v )
			=> Vector3Int.FloorToInt( v );


		public static bool IsLess( this Vector3Int self, Vector3Int a ) => (
			self.x < a.x &&
			self.y < a.y &&
			self.z < a.z
		);

		public static bool IsGreater( this Vector3Int self, Vector3Int a ) => (
			self.x > a.x &&
			self.y > a.y &&
			self.z > a.z
		);

		public static bool IsInside( this Vector3Int self, Vector3Int start, Vector3Int end ) => (
			start.IsLess( self ) &&
			self.IsLess( end )
		);

		public static bool IsOutside( this Vector3Int self, Vector3Int start, Vector3Int end )
			=> !IsInside( self, start, end );
	}
}