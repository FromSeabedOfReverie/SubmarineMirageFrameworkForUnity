//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using UnityEngine;
	using Utility;
	///====================================================================================================
	/// <summary>
	/// ■ 2次元ベクトルの拡張クラス
	/// </summary>
	///====================================================================================================
	public static class Vector2SMExtension {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 除算
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static Vector2 Div( this Vector2 a, Vector2 b ) {
			return new Vector2( a.x / b.x, a.y / b.y );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 乗算
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static Vector2 Mult( this Vector2 a, Vector2 b ) {
			return new Vector2( a.x * b.x, a.y * b.y );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 小数切り上げ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static Vector2 Ceil( this Vector2 a ) {
			return new Vector2( Mathf.Ceil( a.x ), Mathf.Ceil( a.y ) );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● より小さい
		///	 大きさの比較ではない
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static bool IsLess( this Vector2 a, Vector2 b ) {
			return (
				a.x < b.x &&
				a.y < b.y
			);
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● より大きい
		///	 大きさの比較ではない
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static bool IsGreater( this Vector2 a, Vector2 b ) {
			return (
				a.x > b.x &&
				a.y > b.y
			);
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● より小さい（詳細）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static bool[] IsLessDetails( this Vector2 a, Vector2 b ) {
			return new bool[] {
				a.x < b.x,
				a.y < b.y
			};
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● より大きい（詳細）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static bool[] IsGreaterDetails( this Vector2 a, Vector2 b ) {
			return new bool[] {
				a.x > b.x,
				a.y > b.y
			};
		}
		///------------------------------------------------------------------------------------------------
		/// ● 範囲内か？
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 範囲内か？
		/// </summary>
		public static bool IsInside( this Vector2 position, SMArea area ) {
			return area.IsInside( position );
		}
		/// <summary>
		/// ● 範囲内か？（詳細）
		/// </summary>
		public static bool IsInside( this Vector2 position, SMArea area, out bool[] isDetails ) {
			return area.IsInside( position, out isDetails );
		}
		///------------------------------------------------------------------------------------------------
		/// ● 範囲外か？
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 範囲外か？
		/// </summary>
		public static bool IsOutside( this Vector2 position, SMArea area ) {
			return area.IsOutside( position );
		}
		/// <summary>
		/// ● 範囲外か？（詳細）
		/// </summary>
		public static bool IsOutside( this Vector2 position, SMArea area, out bool[] isDetails ) {
			return area.IsOutside( position, out isDetails );
		}
	}
}