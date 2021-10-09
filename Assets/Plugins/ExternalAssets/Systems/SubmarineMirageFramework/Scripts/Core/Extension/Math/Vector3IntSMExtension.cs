//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using UnityEngine;


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


		public static Vector3Int Add( this Vector3Int self, int n ) => new Vector3Int(
			self.x + n,
			self.y + n,
			self.z + n
		);

		public static Vector3Int Sub( this Vector3Int self, int n ) => new Vector3Int(
			self.x - n,
			self.y - n,
			self.z - n
		);


		public static Vector3Int FloorToInt( this Vector3 v )
			=> Vector3Int.FloorToInt( v );


		public static bool IsLess( this Vector3Int self, Vector3Int v ) => (
			self.x < v.x &&
			self.y < v.y &&
			self.z < v.z
		);

		public static bool IsGreater( this Vector3Int self, Vector3Int v ) => (
			self.x > v.x &&
			self.y > v.y &&
			self.z > v.z
		);

		public static bool IsInside( this Vector3Int self, Vector3Int start, Vector3Int end ) => (
			start.IsLess( self ) &&
			self.IsLess( end )
		);

		public static bool IsOutside( this Vector3Int self, Vector3Int start, Vector3Int end )
			=> !IsInside( self, start, end );
	}
}