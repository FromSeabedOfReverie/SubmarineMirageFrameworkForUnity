//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Data.File {
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx.Async;
	using KoganeUnityLib;
	using Extension;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ CSVの読み書きクラス
	///----------------------------------------------------------------------------------------------------
	///		文字列をCSVに変換し、配列に格納し、その逆も行う。
	///		区切り文字ごとに動的配列に保存し、書類名の辞書に保存する。
	///		
	///		UnityのTextAssetはCSVのみ対応し、xlsxを読めない為、他者はjson、ScriptableObjectに態々変換している。
	///		しかし、修正の度に一々変換するのは面倒なので、対応するCSVをランタイムで読み書き可能にした。
	///		Microsoftのライブラリは、Windows以外や、実行時に参照できない。
	/// </summary>
	///====================================================================================================
	public class CSVLoader : DataLoader {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>書類の拡張子</summary>
		public const string EXTENSION = ".csv";
		/// <summary>書類読み書き</summary>
		FileLoader _loader	=> FileManager.s_instance._fileLoader;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		///		※拡張子は含まない。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public async UniTask< List< List<string> > > Load( FileLoader.Type type, string path,
															bool isUseCache = false )
		{
			string texts = null;

			// 指定階層の書類を読み込み
			switch ( type ) {
				case FileLoader.Type.Server:
					texts = await _loader.LoadServer<string>( path );
					break;

				case FileLoader.Type.External:
					path = $"{path}{EXTENSION}";	// 階層を結合
					texts = await _loader.LoadExternal<string>( path, isUseCache );
					break;

				case FileLoader.Type.Resource:
					var asset = await _loader.LoadResource<TextAsset>( path, isUseCache );
					if ( asset != null ) {
						texts = asset.text;
						if ( !isUseCache )	{ Resources.UnloadAsset( asset ); }
					}
					break;
			}

			// 書類からデータに変換
			return TextToCSV( texts );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 変換（文章からCSV配列）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public List< List<string> > TextToCSV( string texts ) {
			// データが無の場合、未処理
			if ( texts == null )	{ return null; }

			var fileData = new List< List<string> >();	// 書類内の情報
			var lineData = new List<string>();			// 行内の情報
			var text = "";								// 分割文字列の情報
			var isDoubleQuotation = false;				// ダブルクォーテーション内か？
			var isComment = false;						// コメント内か？


			// 改行コードを統一
			texts = $"{texts.UnifyNewLine()}\n";	// 末尾読込の為、念の為改行を追加
			// 全文字を走査
			foreach ( var c in texts ) {
				// コメント中の場合
				if ( isComment ) {
					// 改行文字の場合、コメント終了
					if ( c == '\n' ) {
						isComment = false;
//						Log.Debug( "コメント終了", Log.Tag.File );
					}
					continue;
				}
				// 以下、コメント中でない場合


				// ダブルクォーテーション文字の場合、フラグを反転
				if ( c == '"' ) {
					isDoubleQuotation = !isDoubleQuotation;
//					Log.Debug( $"セル内複数行{( isDoubleQuotation ? "開始" : "終了" )}", Log.Tag.File );
					continue;
				}
				// 以下、ダブルクォーテーション文字でない場合


				// セル内複数行中の場合、文字を追加
				if ( isDoubleQuotation ) {
					text += c;
					continue;
				}
				// 以下、セル内複数行中でない場合


				// コメント文字の場合
				if ( text.IndexOf( "//" ) == 0 ) {
					text = "";
					isComment = true;
//					Log.Debug( "コメント開始", Log.Tag.File );
					continue;
				}
				// 以下、コメント文字でない場合


				switch ( c ) {
					// 改行文字の場合
					case '\n':
						AddData( ref lineData, ref text );
						NextLine( ref fileData, ref lineData );
						break;

					// カンマ文字の場合
					case ',':
						AddData( ref lineData, ref text );
						break;

					// 通常文字の場合
					default:
						text += c;
						break;
				}
			}


			return fileData;
		}
		/// <summary>
		/// ● 追加（情報）
		/// </summary>
		void AddData( ref List<string> lineData, ref string text ) {
			lineData.Add( text );
			text = "";
		}
		/// <summary>
		/// ● 次の行へ
		/// </summary>
		void NextLine( ref List< List<string> > fileData, ref List<string> lineData ) {
			// 行内に、1つでも空でない情報が存在するか？
			var isExist = lineData.Any( s => !s.IsNullOrEmptyOrNewLine() );
			// 情報が存在する場合、書類情報にコピー追加
			if ( isExist )	{ fileData.Add( new List<string>( lineData ) ); }
			lineData.Clear();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 保存（アプリ外階層）
		///		※拡張子は含まない。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public async UniTask SaveExternal( string path, List< List<string> > data ) {
			var texts = CSVToText( data );
			path = $"{path}{EXTENSION}";				// 階層を結合
			await _loader.SaveExternal( path, texts );	// 保存
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 変換（CSV配列から文章）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public string CSVToText( List< List<string> > data ) {
			var texts = data.Select( line => {
				var ss = line.Select( s => {
					s = s.UnifyNewLine();
					// 改行が含まれる場合、ダブルクォーテーションで囲む
					if ( s.Contains( "\n" ) ) {
						s = $"\"{s}\"";
					}
					return s;
				} );
				return string.Join( ",", ss );
			} );
			return string.Join( "\n", texts );
		}
	}
}