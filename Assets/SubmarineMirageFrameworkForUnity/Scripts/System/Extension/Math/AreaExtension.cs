//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Extension {
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ 範囲の拡張クラス
	///----------------------------------------------------------------------------------------------------
	///		拡張関数を実装している。
	/// </summary>
	///====================================================================================================
	public static class AreaExtension {
		///------------------------------------------------------------------------------------------------
		/// ● 範囲内か？
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 範囲内か？
		/// </summary>
		public static bool IsInside( this Vector2 position, Area area ) {
			return area.IsInside( position );
		}
		/// <summary>
		/// ● 範囲内か？（bool配列を返す）
		/// </summary>
		public static bool IsInside( this Vector2 position, Area area, out bool[] isDetails ) {
			return area.IsInside( position, out isDetails );
		}
		///------------------------------------------------------------------------------------------------
		/// ● 範囲外か？
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 範囲外か？
		/// </summary>
		public static bool IsOutside( this Vector2 position, Area area ) {
			return area.IsOutside( position );
		}
		/// <summary>
		/// ● 範囲外か？（bool配列を返す）
		/// </summary>
		public static bool IsOutside( this Vector2 position, Area area, out bool[] isDetails ) {
			return area.IsOutside( position, out isDetails );
		}
	}
}