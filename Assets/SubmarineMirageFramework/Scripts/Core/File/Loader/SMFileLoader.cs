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
	using Data.Raw;
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
		SMNetworkManager _networkManager	{ get; set; }
		SMMainSetting _setting				{ get; set; }

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMFileLoader( SMFileManager fileManager ) : base( fileManager ) {
			_disposables.AddFirst( () => {
				_networkManager = null;
				_setting = null;
			} );
		}

		/// <summary>
		/// ● 設定
		/// </summary>
		public override void Setup() {
			_networkManager = SMServiceLocator.Resolve<SMNetworkManager>();
			_setting = SMServiceLocator.Resolve<SMMainSetting>();
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
			var path = GetLoadPath( SMFileLocation.Server, url );
			// 可能なら、キャッシュ読込
			var data = await Load<T>( SMFileLocation.Server, path, false,
				// キャッシュ読込不可能の場合、ダウンロード要求
				() => LoadRequest<T>( path, audioType )
			);
			// ダウンロード失敗の場合
			if ( data == null ) {
				_errorCount--;	// 二重エラーを防止
				// 強制キャッシュ読込
				data = await Load<T>( SMFileLocation.Server, path, true, async () => {
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
		public async UniTask<T> LoadExternal<T>( string path, bool isUseCache = SMMainSetting.IS_DEFAULT_USE_CACHE,
													AudioType audioType = AudioType.UNKNOWN
		) {
			// 階層を取得
			path = GetLoadPath( SMFileLocation.External, path );
			// 共通読込
			return await Load<T>( SMFileLocation.External, path, isUseCache,
				// キャッシュに無い場合、読込要求
				() => LoadRequest<T>( path, audioType )
			);
		}

		/// <summary>
		/// ● 読込（リソース）
		///		※拡張子は含まない。
		/// </summary>
		public async UniTask<T> LoadResource<T>( string path, bool isUseCache = SMMainSetting.IS_DEFAULT_USE_CACHE )
			where T : UnityEngine.Object
		{
			// 階層を取得
			path = GetLoadPath( SMFileLocation.Resource, path );
			// 共通読込
			return await Load<T>( SMFileLocation.Resource, path, isUseCache,
				// キャッシュに無い場合、リソース読込
				async () => await Resources.LoadAsync<T>( path ).ToUniTask( _asyncCanceler )
			);
		}

		/// <summary>
		/// ● 読込（要求）
		/// </summary>
		async UniTask<object> LoadRequest<T>( string path, AudioType audioType = AudioType.UNKNOWN ) {
			var request = UnityWebRequest.Get( path );
			var dataType = SMAllDataManager.GetDataType( typeof( T ) );
			_downloadingCount++;
			// 各型の配信取扱を作成
			switch ( dataType ) {
				case SMDataType.Texture:
				case SMDataType.Sprite:
					request.downloadHandler = new DownloadHandlerTexture( true );
					break;
				case SMDataType.Audio:
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
					case SMDataType.Texture:
						o = ( request.downloadHandler as DownloadHandlerTexture ).texture;
						break;
					case SMDataType.Sprite:
						var texture = ( request.downloadHandler as DownloadHandlerTexture ).texture;
						o = texture.ToSprite();
						break;
					case SMDataType.Audio:
						o = ( request.downloadHandler as DownloadHandlerAudioClip ).audioClip;
						break;
					case SMDataType.Serialize:
						o = SerializerSMUtility.Deserialize<T>( request.downloadHandler.data );
						break;
					case SMDataType.Text:
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
		async UniTask<T> Load<T>( SMFileLocation location, string path, bool isUseCache,
									Func< UniTask<object> > loadEvent
		) {
			try {
				// キャッシュ使用の場合
				isUseCache = IsCanUseCache( location, isUseCache );
				if ( isUseCache ) {
					// キャッシュ読込
					var cache = _fileManager._tempCaches.Get( path );
					if ( cache != null ) {
						// 既に読込要求済の場合、読込まで待機（無登録中は、読込中フラグ）
						await UTask.WaitWhile( _asyncCanceler, () => cache._data == null );
						SMLog.Debug( $"キャッシュ読込成功 : {cache._data}\n{path}", SMLogTag.File );
						return ( T )cache._data;
					}
					_fileManager._tempCaches.Register( path, null );	// 読込中フラグとして、無を登録
				}

				_loadingCount++;
				var result = await loadEvent();

				// 情報存在の場合
				if ( result != null ) {
					// キャッシュ使用中か、サーバーダウンロードデータの場合
					if ( isUseCache || location == SMFileLocation.Server ) {
						_fileManager._tempCaches.Register( path, result );	// 情報を登録
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
			if ( isUseCache )	{ _fileManager._tempCaches.Unregister( path ); }
			return default;
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
				path = Path.Combine( SMMainSetting.SAVE_EXTERNAL_PATH, path );  // 外部保存のみ対応
				byte[] rawData = null;

				var dataType = SMAllDataManager.GetDataType( typeof( T ) );
				switch ( dataType ) {
					case SMDataType.Texture: {
						var rawTexture = ( data as Texture2D ).ToRawData( encodeType.Value, encodeOption );
						rawData = rawTexture._data;
						break;
					}
					case SMDataType.Sprite: {
						var rawTexture = ( data as Sprite ).texture.ToRawData( encodeType.Value, encodeOption );
						rawData = rawTexture._data;
						break;
					}
					case SMDataType.Audio:
						rawData = ( data as AudioClip ).EncodeToWAV();
						break;
					case SMDataType.Serialize:
						rawData = SerializerSMUtility.Serialize( data );
						break;
					case SMDataType.Text:
// TODO : BOM除去したいが、出来てない
						var encoding = new UTF8Encoding( false );
						rawData = encoding.GetBytes( data as string );
						break;
					case SMDataType.Raw:
						rawData = data as byte[];
						break;
				}

				// フォルダを作成
				var folder = Path.GetDirectoryName( path );
				PathSMUtility.Create( folder );

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

		///------------------------------------------------------------------------------------------------
		/// ● 判定
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● キャッシュ使用可能か？
		/// </summary>
		public bool IsCanUseCache( SMFileLocation location, bool isWantUseCache ) {
			switch ( location ) {
				// サーバー通信読込の場合、使用希望か、接続切れか、ダウンロード済キャッシュが最新の場合
				case SMFileLocation.Server:
					return isWantUseCache || !_networkManager._isConnecting || !_setting._isRequestUpdateServer;

				// それ以外の場合、希望通りに可能
				default:
					return isWantUseCache;
			}
		}

		///------------------------------------------------------------------------------------------------
		/// ● 取得
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込階層を取得
		/// </summary>
		string GetLoadPath( SMFileLocation location, string path ) {
			var topPath = "";
			switch ( location ) {
				case SMFileLocation.Server:		topPath = "";											break;
				case SMFileLocation.External:	topPath = $"file://{SMMainSetting.LOAD_EXTERNAL_PATH}";	break;
				case SMFileLocation.Resource:	topPath = SMMainSetting.LOAD_RESOURCE_PATH;				break;
			}
			// 階層を結合
			return Path.Combine( topPath, path );
		}
	}
}