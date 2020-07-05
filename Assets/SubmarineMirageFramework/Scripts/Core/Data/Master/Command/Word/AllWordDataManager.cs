//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Debug;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ 全単語情報の管理クラス
	///----------------------------------------------------------------------------------------------------
	///		文章の構文解析に使用する。
	/// </summary>
	///====================================================================================================
	public class AllWordDataManager : BaseDataManager<string, IBaseDataManager> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>単語の読込深度</summary>
		public enum WordDepth {
			/// <summary>最優先で、単語読込</summary>
			_1,
			/// <summary>優先で、単語読込</summary>
			_2,
			/// <summary>普通に、読込</summary>
			_3,
			/// <summary>後で、読込</summary>
			_4,
			/// <summary>最後に、単語読込</summary>
			_5,
		}

		/// <summary>単語の読込深度の一覧</summary>
		public readonly List<WordDepth> _depths;
		/// <summary>入力文字を整形させる為の、単語情報</summary>
		readonly WordDataManager<FormatWordData> _formatWord	= new WordDataManager<FormatWordData>( "FormatWord" );
		/// <summary>単語情報</summary>
		readonly WordDataManager<WordData> _word				= new WordDataManager<WordData>( "Word" );
		/// <summary>単語から、文章抽出の為の情報</summary>
		readonly WordDataManager<ParseTextData> _parseText		= new WordDataManager<ParseTextData>( "ParseText" );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public AllWordDataManager() : base() {
			// 深度走査用に、型定数配列を予め作成
			_depths = EnumUtils.GetValues<WordDepth>().ToList();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Load() {
			Register( _formatWord._fileName, _formatWord );
			Register( _word._fileName, _word );
			Register( _parseText._fileName, _parseText );

			await base.Load();

//			Test();
		}
		///------------------------------------------------------------------------------------------------
		/// ● 文章の抽出
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文章の抽出（抽出前、文章から）
		/// </summary>
		public List< List<string> > ExtractionTexts( string text ) {
			var words = ExtractionWords( text );
//			Log.Debug( words.ToDeepString() );
			return ExtractionTextsFromFormattedWords( words );
		}
		/// <summary>
		/// ● 単語の抽出
		/// </summary>
		public List<string> ExtractionWords( string text ) {
			text = UnificationWords( text );	// 単語を一定の様式に統一

			var words = _depths
				.SelectMany( d => _word.Get( d ) )
				.SelectMany( data => {
					// 文章から単語を全取得
					var resultWords = new List<string>();
					while ( true ) {
						var word = data.GetAndDelete( ref text );
						if ( word.IsNullOrEmpty() )	{ break; }
//						Log.Debug( $"{word} : {text}" );
						resultWords.Add( word );
					}
					return resultWords;
				})
				.Where( s => !s.IsNullOrEmpty() )
				.Distinct()
				.ToList();

			return words;
		}
		/// <summary>
		/// ● 文章の抽出（抽出済、単語配列から）
		/// </summary>
		List< List<string> > ExtractionTextsFromFormattedWords( List<string> words ) {
			var resultWords = _depths
				.SelectMany( d => _parseText.Get( d ) )
				.Select( data => data.Gets( words ) )
				.Where( list => !list.IsNullOrEmpty() )
				.ToList();

			return resultWords;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 単語の統一
		///		半角英数字、全角カタカナに丸め込む。
		///		string デフォルトエンコードは UTF-16 を使用している
		/// </summary>
		///------------------------------------------------------------------------------------------------
		string UnificationWords( string text ) {
			// 文章を走査し、書式を統一する
			var formatText = text
				.Select(
					c => {
						var word = _depths
							.SelectMany( d => _formatWord.Get( d ) )
							.Select( data => data.Get( c ) )
							.FirstOrDefault( resultC => resultC.HasValue );
						return word.HasValue ? word.Value : c;
					}
				)
				.Where( c => c != '\0' )
				.ToArray();

			return new string( formatText );	// 文字列型に戻す
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 構文解析のテスト
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void Test() {
			Log.Debug( UnificationWords(
				"０Ａｚだが断る。"
			) );

			Log.Debug( ExtractionWords(
				"大蛇龍お釈迦様死霊幽質の霊魂霊魂。たましい、神霊イエス"
			).ToDeepString() );


			Log.Debug( ExtractionTexts(
				"龍様、雨を、降らせて下さい"
			).ToDeepString() );
			Log.Debug( ExtractionTextsFromFormattedWords(
				new List<string>() { "龍", "雨", "降ル" }
			).ToDeepString() );
			Log.Debug( ExtractionTextsFromFormattedWords(
				new List<string>() { "降ル", "雨", "龍" }
			).ToDeepString() );


			Log.Debug( ExtractionTexts(
				"高級霊魂様、救って下さい、霊的向上を望みます"
			).ToDeepString() );
			Log.Debug( ExtractionTextsFromFormattedWords(
				new List<string>() { "高級霊", "救ウ", "向上" }
			).ToDeepString() );
			Log.Debug( ExtractionTextsFromFormattedWords(
				new List<string>() { "向上", "救ウ", "高級霊" }
			).ToDeepString() );
		}
	}
}