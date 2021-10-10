//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ 色の拡張クラス
	///		拡張関数を実装している。
	/// </summary>
	///====================================================================================================
	public static class ColorSMExtension {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 255を補正（0～255 → 0～1）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static Color Correct255( this Color self ) {
			var lastAlpha = self.a;
			var result = self / 255;
			if ( lastAlpha <= 1 ) { result.a = lastAlpha; }
			return result;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● NGUI色に変換
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static string ToNGUIFormat( this Color self ) {
			var s = ColorUtility.ToHtmlStringRGB( self );
			return "[" + s + "]";
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● UGUI色に変換
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static string ToUGUIFormat( this Color self ) {
			var s = ColorUtility.ToHtmlStringRGB( self );
			return "<color=#" + s + ">";
		}
	}
}