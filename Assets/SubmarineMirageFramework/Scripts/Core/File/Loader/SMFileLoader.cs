//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.File {
	using System;
	using System.IO;
	using System.Text;
	using UnityEngine;
	using UnityEngine.Networking;
	using Cysharp.Threading.Tasks;
	using Service;
	using Data;
	using Extension;
	using Utility;
	using Setting;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 書類の読み書きクラス
	///		サーバー、外部、リソース書類の、読み書きを行う。
	///		object型で管理しているので、画像等、何でも読み書きできる。
	/// </summary>
	///====================================================================================================
	public class SMFileLoader : BaseSMDataLoader {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>書類の種類</summary>
		public enum Type {
			/// <summary>サーバー読み書き</summary>
			Server,
			/// <summary>アプリ外読み書き</summary>
			External,
			/// <summary>リソース読み書き</summary>
			Resource,
		}
		/// <summary>情報の種類</summary>
		public enum DataType {
			/// <summary>テクスチャ画像</summary>
			Texture,
			/// <summary>スプライト画像</summary>
			Sprite,
			/// <summary>音</summary>
			Audio,
			/// <summary>シリアル化情報</summary>
			Serialize,
			/// <summary>文章</summary>
			Text,
			/// <summary>生情報（どれにも当て嵌まらない時用）</summary>
			Raw,
		}
		/// <summary>無指定の場合、キャッシュを使うか？</summary>
		const bool IS_DEFAULT_USE_CACHE = false;

		AllDataManager _allDataManager	{ get; set; }
		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		public override void Setup( SMFileManager fileManager ) {
			base.Setup( fileManager );

			_allDataManager = SMServiceLocator.Resolve<AllDataManager>();
		}
		///------------------------------------------------------------------------------------------------
		/// ● 読込
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込（サーバー）
		///		audioType : 音書類で拡張子が不明の場合、指定する。（MP3非対応）
		///		キャッシュデータ使用か？を、ユーザーは選択できない
		///			（サーバーキャッシュが違う → ダウンロード → ダウンロード失敗 → 強制キャッシュ読込
		///			のように、キャッシュ使用か？をシステムから変更する為）
		/// </summary>
		public async UniTask<T> LoadServer<T>( string url, AudioType audioType = AudioType.UNKNOWN ) {
			// 階層を取得
			var path = GetLoadPath( Type.Server, url );
			// 可能なら、キャッシュ読込
			var data = await Load<T>( Type.Server, path, false,
				// キャッシュ読込不可能の場合、ダウンロード要求
				() => LoadRequest<T>( path, audioType )
			);
			// ダウンロード失敗の場合
			if ( data == null ) {
				_errorCount--;	// 二重エラーを防止
				// 強制キャッシュ読込
				data = await Load<T>( Type.Server, path, true, async () => {
					// 強制キャッシュ読込失敗の場合、無取得
					await UTask.DontWait();
					return null;
				} );
			}
			return data;
		}

		/// <summary>
		/// ● 読込（アプリ外階層）
		///		※拡張子を含む。
		///		audioType : 音書類で拡張子が不明の場合、指定する。（MP3非対応）
		/// </summary>
		public async UniTask<T> LoadExternal<T>( string path, bool isUseCache = IS_DEFAULT_USE_CACHE,
													AudioType audioType = AudioType.UNKNOWN
		) {
			// 階層を取得
			path = GetLoadPath( Type.External, path );
			// 共通読込
			return await Load<T>( Type.External, path, isUseCache,
				// キャッシュに無い場合、読込要求
				() => LoadRequest<T>( path, audioType )
			);
		}

		/// <summary>
		/// ● 読込（リソース）
		///		※拡張子は含まない。
		/// </summary>
		public async UniTask<T> LoadResource<T>( string path, bool isUseCache = IS_DEFAULT_USE_CACHE )
			where T : UnityEngine.Object
		{
			// 階層を取得
			path = GetLoadPath( Type.Resource, path );
			// 共通読込
			return await Load<T>( Type.Resource, path, isUseCache,
				// キャッシュに無い場合、リソース読込
				async () => await Resources.LoadAsync<T>( path ).ToUniTask( _asyncCanceler )
			);
		}

		/// <summary>
		/// ● 読込（要求）
		/// </summary>
		async UniTask<object> LoadRequest<T>( string path, AudioType audioType = AudioType.UNKNOWN ) {
			var request = UnityWebRequest.Get( path );
			var dataType = GetDataType<T>();
			_downloadingCount++;
			// 各型の配信取扱を作成
			switch ( dataType ) {
				case DataType.Texture:
				case DataType.Sprite:
					request.downloadHandler = new DownloadHandlerTexture( true );
					break;
				case DataType.Audio:
					request.downloadHandler = new DownloadHandlerAudioClip( path, audioType );
					break;
				default:
					request.downloadHandler = new DownloadHandlerBuffer();
					break;
			}

			// 配信を待機
			await request.SendWebRequest().ToUniTask( _asyncCanceler );

			// 配信成功の場合
			if ( request.IsSuccess() ) {
				object o = null;
				// 各型に合わせた結果を取得
				switch ( dataType ) {
					case DataType.Texture:
						o = ( request.downloadHandler as DownloadHandlerTexture ).texture;
						break;
					case DataType.Sprite:
						var texture = ( request.downloadHandler as DownloadHandlerTexture ).texture;
						o = texture.ToSprite();
						break;
					case DataType.Audio:
						o = ( request.downloadHandler as DownloadHandlerAudioClip ).audioClip;
						break;
					case DataType.Serialize:
						o = SerializerSMUtility.Deserialize<T>( request.downloadHandler.data );
						break;
					case DataType.Text:
						o = request.downloadHandler.text;
						break;
					default:
						o = request.downloadHandler.data;
						break;
				}
				_downloadingCount--;
				_downloadedCount++;
				SMLog.Debug( $"配信成功 : {request.url}\n{o}", SMLogTag.Server );
				return o;

			// 配信失敗の場合
			} else {
				_errorCount++;
				SMLog.Error( $"配信中失敗 : {request.url}\n{request.error}", SMLogTag.Server );
				return null;
			}
		}

		/// <summary>
		/// ● 読込
		/// </summary>
		async UniTask<T> Load<T>( Type type, string path, bool isUseCache, Func< UniTask<object> > loadEvent ) {
			try {
				// キャッシュ使用の場合
				isUseCache = IsCanUseCache( type, isUseCache );
				if ( isUseCache ) {
					// キャッシュ読込
					var temp = _fileManager._cacheData.Get( path );
					if ( temp != null ) {
						// 既に読込要求済の場合、読込まで待機（無登録中は、読込中フラグ）
						await UTask.WaitUntil( _asyncCanceler, () => temp.Get() != null );
						SMLog.Debug( $"キャッシュ読込成功 : {temp.Get()}\n{path}", SMLogTag.File );
						return ( T )temp.Get();
					}
					_fileManager._cacheData.Register( path, null );	// 読込中フラグとして、無を登録
				}

				_loadingCount++;
				var result = await loadEvent();

				// 情報存在の場合
				if ( result != null ) {
					// キャッシュ使用中か、サーバーダウンロードデータの場合
					if ( isUseCache || type == Type.Server ) {
						_fileManager._cacheData.Register( path, result );	// 情報を登録
					}
					_loadingCount--;
					SMLog.Debug( $"読込成功 : {path}\n{result}", SMLogTag.File );
					return ( T )result;
				}


			} catch ( Exception e ) {
				SMLog.Error( $"読込中に問題発生 : {path}\n{e}", SMLogTag.File );
			}


			// 未読込の為、失敗
			_errorCount++;
			SMLog.Error( $"読込失敗 : {path}", SMLogTag.File );
			// キャッシュ使用中の場合、読込フラグの無を登録解除
			if ( isUseCache )	{ _fileManager._cacheData.Unregister( path ); }
			return default;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得（読込階層）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		string GetLoadPath( Type type, string path ) {
			var topPath = "";
			switch ( type ) {
				case Type.Server:	topPath = "";											break;
				case Type.External:	topPath = $"file://{SMFileManager.LOAD_EXTERNAL_PATH}";	break;
				case Type.Resource:	topPath = SMFileManager.LOAD_RESOURCE_PATH;				break;
			}
			// 階層を結合
			return Path.Combine( topPath, path );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 配信型を取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public DataType GetDataType<T>( System.Type type = null ) {
			if ( type == null )	{ type = typeof( T ); }
			return (
				type.IsInheritance<Texture2D>()			? DataType.Texture :
				type.IsInheritance<Sprite>()			? DataType.Sprite :
				type.IsInheritance<AudioClip>()			? DataType.Audio :
				type.IsInheritance<ISMSerializeData>()	? DataType.Serialize :
				type.IsInheritance<string>()			? DataType.Text
														: DataType.Raw
			);
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● キャッシュ使用可能か？
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public bool IsCanUseCache( Type type, bool isWantUseCache ) {
			switch ( type ) {
				// サーバー通信読込の場合、使用希望か、キャッシュ読込可能の場合、可能
				case Type.Server:
					return isWantUseCache || _allDataManager._server.IsCanLoadServerCache();

				// それ以外の場合、希望通りに可能
				default:
					return isWantUseCache;
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 保存（アプリ外階層）
		///		※拡張子を含む。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public async UniTask SaveExternal<T>( string path, T data,
												SMTextureRawData.Type? encodeType = null, int? encodeOption = null
		) {
			try {
				_savingCount++;
				path = Path.Combine( SMFileManager.SAVE_EXTERNAL_PATH, path );	// 外部保存のみ対応
				byte[] rawData = null;

				var dataType = GetDataType<T>();
				switch ( dataType ) {
					case DataType.Texture: {
						var rawTexture = ( data as Texture2D ).ToRawData( encodeType.Value, encodeOption );
						rawData = rawTexture._data;
						break;
					}
					case DataType.Sprite: {
						var rawTexture = ( data as Sprite ).texture.ToRawData( encodeType.Value, encodeOption );
						rawData = rawTexture._data;
						break;
					}
					case DataType.Audio:
						rawData = ( data as AudioClip ).EncodeToWAV();
						break;
					case DataType.Serialize:
						rawData = SerializerSMUtility.Serialize( data );
						break;
					case DataType.Text:
						// TODO : BOM除去したいが、出来てない
						var encoding = new UTF8Encoding( false );
						rawData = encoding.GetBytes( data as string );
						break;
					case DataType.Raw:
						rawData = data as byte[];
						break;
				}

				// フォルダを作成
				var folder = Path.GetDirectoryName( path );
				_fileManager.CreatePath( folder );

				// 上書き保存
				var file = new FileStream( path, FileMode.Create, FileAccess.Write );
				await file.WriteAsync( rawData, 0, rawData.Length, _asyncCanceler.ToToken() );
				file.Close();

				SMLog.Debug( $"保存成功 : {path}\n{data}", SMLogTag.File );
				_savingCount--;


			} catch ( Exception e ) {
				SMLog.Error( $"保存中に問題発生 : {path}\n{data}\n{e}", SMLogTag.File );
				_errorCount++;
			}
		}
	}
}