//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 文字列の拡張クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public static class StringSMExtension {
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
		///------------------------------------------------------------------------------------------------
		/// ● 挿入
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 挿入
		/// </summary>
		static string Insert( this string self, string findText, string insertText, bool isInsertFirst ) {
			var i = self.IndexOf( findText );
			if ( !isInsertFirst )	{ i += findText.Length; }
			return self.Insert( i, insertText );
		}
		/// <summary>
		/// ● 最初に挿入
		/// </summary>
		public static string InsertFirst( this string self, string findText, string insertText )
			=> Insert( self, findText, insertText, true );
		/// <summary>
		/// ● 最後に挿入
		/// </summary>
		public static string InsertLast( this string self, string findText, string insertText )
			=> Insert( self, findText, insertText, false );
	}
}