//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Data {
	using System.Linq;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 体裁単語の情報クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class FormatWordData : BaseWordData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得（先頭文字比較）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public char? Get( char c ) {
			var isExist = _commands.Any( s => c == s.ToCharOrNull() );
			if ( isExist ) {
				return _extractionWord.ToCharOrDefault( '\0' );
			}
			return null;
		}
	}
}