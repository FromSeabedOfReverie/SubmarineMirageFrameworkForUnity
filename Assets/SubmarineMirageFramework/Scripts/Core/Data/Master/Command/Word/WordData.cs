//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using System.Linq;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 単語の情報クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class WordData : BaseWordData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得（文字列比較）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public string Get( string text ) {
			var isExist = _commands.Any( s => text.IndexOf( s ) != -1 );
			return isExist ? _extractionWord : null;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得して削除
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public string GetAndDelete( ref string text ) {
			var t = text;
			var word = _commands.FirstOrDefault( s => t.IndexOf( s ) != -1 );
			if ( word.IsNullOrEmpty() )	{ return null; }

			text = text.Replace( word, "" );	// 抽出単語を削除
			return _extractionWord;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 単語を設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected override void SetWord() {
			_commands.Add( _extractionWord );	// 抽出する自身の単語も、最後に捜索する
		}
	}
}