//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using UnityEngine;



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

		public static Vector3 Mod( this Vector3 self, int n ) => new Vector3(
			self.x % n,
			self.y % n,
			self.z % n
		);


		public static Vector3Int Sign( this Vector3 self ) => new Vector3Int(
			(int)Mathf.Sign( self.x ),
			(int)Mathf.Sign( self.y ),
			(int)Mathf.Sign( self.z )
		);


		public static Vector3Int OutsideDirection( this Vector3 self ) {
			var v = self.Mod( 1 );

			if ( v.x < 0 )	{ v.x += 1; }
			if ( v.y < 0 )	{ v.y += 1; }
			if ( v.z < 0 )	{ v.z += 1; }

			return new Vector3Int(
				v.x < 0.5 ? -1 : 1,
				v.y < 0.5 ? -1 : 1,
				v.z < 0.5 ? -1 : 1
			);
		}



		public static bool IsLess( this Vector3 self, Vector3 v ) => (
			self.x < v.x &&
			self.y < v.y &&
			self.z < v.z
		);

		public static bool IsGreater( this Vector3 self, Vector3 v ) => (
			self.x > v.x &&
			self.y > v.y &&
			self.z > v.z
		);

		public static bool IsInside( this Vector3 self, Vector3 start, Vector3 end ) => (
			start.IsLess( self ) &&
			self.IsLess( end )
		);

		public static bool IsOutside( this Vector3 self, Vector3 start, Vector3 end )
			=> !IsInside( self, start, end );
	}
}