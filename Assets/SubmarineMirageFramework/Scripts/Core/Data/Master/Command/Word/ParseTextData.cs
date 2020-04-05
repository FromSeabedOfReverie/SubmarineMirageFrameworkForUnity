//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Data {
	using System.Linq;
	using System.Collections.Generic;
	using KoganeUnityLib;
	using Extension;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 構文解析文章の情報クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class ParseTextData : BaseWordData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>複数の単語表現と一致した場合、最終的に出力する、文章構成単語の一覧</summary>
		public readonly List<string> _extractionWords = new List<string>();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得（複数）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public List<string> Gets( List<string> words ) {
			if ( words.IsNullOrEmpty() )	{ return null; }

			var isExist = true;
			_commands
				.Where( _ => isExist )
				.Select( command => words.FindIndex( word => word == command ) )
				.Where( i => i == -1 )
				// このForEach()が無いと、isExistが初期値のまま、スキップされる
				.ForEach( _ => isExist = false );

//			Log.Debug( $"{isExist}\n{this.ToDeepString()}" );
			return isExist ? _extractionWords : null;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 単語を設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		protected override void SetWord() {
			_extractionWords.AddRange( _extractionWord.Split( ':' ) );
		}
	}
}