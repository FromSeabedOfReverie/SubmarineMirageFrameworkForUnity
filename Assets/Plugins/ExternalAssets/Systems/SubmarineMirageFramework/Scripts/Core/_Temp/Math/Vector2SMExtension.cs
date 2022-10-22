//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ 2次元ベクトルの拡張クラス
	/// </summary>
	///====================================================================================================
	public static class Vector2SMExtension {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 小数切り上げ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static Vector2 Ceil( this Vector2 self ) => new Vector2(
			Mathf.Ceil( self.x ),
			Mathf.Ceil( self.y )
		);
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● より小さい
		///	 長さの比較ではない
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static bool IsLess( this Vector2 self, Vector2 v ) => (
			self.x < v.x &&
			self.y < v.y
		);
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● より大きい
		///	 長さの比較ではない
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static bool IsGreater( this Vector2 self, Vector2 v ) => (
			self.x > v.x &&
			self.y > v.y
		);
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● より小さい（詳細）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static bool[] IsLessDetails( this Vector2 self, Vector2 v ) => new bool[] {
			self.x < v.x,
			self.y < v.y
		};
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● より大きい（詳細）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static bool[] IsGreaterDetails( this Vector2 self, Vector2 v ) => new bool[] {
			self.x > v.x,
			self.y > v.y
		};
		///------------------------------------------------------------------------------------------------
		/// ● 範囲内か？
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 範囲内か？
		/// </summary>
		public static bool IsInside( this Vector2 self, SMArea area )
			=> area.IsInside( self );
		/// <summary>
		/// ● 範囲内か？（詳細）
		/// </summary>
		public static bool IsInside( this Vector2 self, SMArea area, out bool[] isDetails )
			=> area.IsInside( self, out isDetails );
		///------------------------------------------------------------------------------------------------
		/// ● 範囲外か？
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 範囲外か？
		/// </summary>
		public static bool IsOutside( this Vector2 self, SMArea area )
			=> area.IsOutside( self );
		/// <summary>
		/// ● 範囲外か？（詳細）
		/// </summary>
		public static bool IsOutside( this Vector2 self, SMArea area, out bool[] isDetails )
			=> area.IsOutside( self, out isDetails );
	}
}