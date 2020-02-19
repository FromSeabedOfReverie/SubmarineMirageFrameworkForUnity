//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Extension {
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 文字列の拡張クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public static class StringExtension {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 無か、空か、改行のみか？
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static bool IsNullOrEmptyOrNewLine( this string self ) {
			if ( self.IsNullOrEmpty() )	{ return true; }

			var noLine = self.Replace( "\n", "" ).Replace( "\r", "" );
			return noLine == string.Empty;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 改行コードを統一
		///		※\nに統一する。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static string UnifyNewLine( this string self ) {
			return self.Replace("\r\n", "\n").Replace( "\r", "\n" );
		}
	}
}