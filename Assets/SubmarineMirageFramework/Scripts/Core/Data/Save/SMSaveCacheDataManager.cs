//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.Save {
	using System;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Service;
	using Event;
	using File;
	using Data.Raw;
	using Data.Cache;
	using Extension;
	using Utility;
	using Setting;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 保存キャッシュ情報の管理クラス
	///		このキャッシュに保存された情報は、必ずセーブされる。
	///		主に、サーバー受信情報のキャッシュ情報を格納。
	/// </summary>
	///====================================================================================================
	public class SMSaveCacheDataManager : BaseSMSaveData, IBaseSMDataManager {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>名前</summary>
		[IgnoreDataMember, SMShow] public string _name { get; protected set; }
		/// <summary>版</summary>
		[SMShow] public string _version = SMMainSetting.INITIAL_VERSION;

		// 以下辞書達は、基底クラスの辞書で纏めると、シリアライザ非対応の為、復元されない
		// その為、仕方無くジェネリック型ごとに分離した
		/// <summary>画像の生情報の辞書</summary>
		[SMShow] public Dictionary< string, SMSaveCacheData<SMTextureRawData> > _textureDatas
			= new Dictionary< string, SMSaveCacheData<SMTextureRawData> >();
		/// <summary>音の生情報の辞書</summary>
		[SMShow] public Dictionary< string, SMSaveCacheData<SMAudioRawData> > _audioDatas
			= new Dictionary< string, SMSaveCacheData<SMAudioRawData> >();
		/// <summary>文章情報の辞書</summary>
		[SMShow] public Dictionary< string, SMSaveCacheData<string> > _textDatas
			= new Dictionary< string, SMSaveCacheData<string> >();
		/// <summary>生情報の辞書</summary>
		[SMShow] public Dictionary< string, SMSaveCacheData<byte[]> > _rawDatas
			= new Dictionary< string, SMSaveCacheData<byte[]> >();

		[IgnoreDataMember] SMTemporaryCacheDataManager _tempCaches	{ get; set; }

		[IgnoreDataMember] public SMAsyncEvent _loadEvent	{ get; private set; } = new SMAsyncEvent();
		[IgnoreDataMember] public SMAsyncEvent _saveEvent	{ get; private set; } = new SMAsyncEvent();

#region ToString
/*
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文章変換を追加
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override string AddToString( int indent ) => string.Join( ",\n",
			"",
			_textureDatas.ToShowString( indent, true ),
			_audioDatas.ToShowString( indent, true ),
			_textDatas.ToShowString( indent, true ),
			_rawDatas.ToShowString( indent, true )
		);
*/
#endregion

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ（読込用）
		/// </summary>
		public SMSaveCacheDataManager() {
			_name = nameof( SMSaveCacheDataManager );

			_tempCaches = SMServiceLocator.Resolve<SMAllDataManager>().Get<SMTemporaryCacheDataManager>();
			var loader = SMServiceLocator.Resolve<SMFileManager>().Get<SMCryptoLoader>();
			var setting = SMServiceLocator.Resolve<SMMainSetting>();


			_loadEvent.AddLast( async canceler => {
				Reset();    // 既存の保存キャッシュを消去

				var data = await loader.Load<SMSaveCacheDataManager>( SMMainSetting.CACHE_FILE_NAME );

				if ( data == null ) {
					SMLog.Debug(
						$"データが存在しない為、初期化生成\n{nameof( SMSaveCacheDataManager )}", SMLogTag.Data );
					await loader.Save( SMMainSetting.CACHE_FILE_NAME, this );

				} else {
					// 中央設定の版を設定
					setting._serverVersionBySave = data._version;

					if ( data._version != setting._serverVersionByServer ) {
						data.Dispose();
						SMLog.Debug(
							$"データが古い為、初期化生成\n{nameof( SMSaveCacheDataManager )}", SMLogTag.Data );
						await loader.Save( SMMainSetting.CACHE_FILE_NAME, this );

					} else {
						_textureDatas = data._textureDatas;
						_audioDatas = data._audioDatas;
						_textDatas = data._textDatas;
						_rawDatas = data._rawDatas;
						// data.Dispose時に、自身のデータも解放される為、参照を切る
						data._textureDatas = null;
						data._audioDatas = null;
						data._textDatas = null;
						data._rawDatas = null;
						data.Dispose();
						SMLog.Debug( $"読込成功\n{nameof( SMSaveCacheDataManager )}", SMLogTag.Data );
					}
				}

				// サーバー版をダウンロード版に適用
				_version = setting._serverVersionByServer;

				await Enumerable.Empty<UniTask>()
					.Concat( _textureDatas.Select( pair => pair.Value.Load() ) )
					.Concat( _audioDatas.Select( pair => pair.Value.Load() ) )
					.Concat( _textDatas.Select( pair => pair.Value.Load() ) )
					.Concat( _rawDatas.Select( pair => pair.Value.Load() ) );

				// 一時キャッシュに格納
				_textureDatas.ForEach( pair => RegisterTempCaches( pair.Value ) );
				_audioDatas.ForEach( pair => RegisterTempCaches( pair.Value ) );
				_textDatas.ForEach( pair => RegisterTempCaches( pair.Value ) );
				_rawDatas.ForEach( pair => RegisterTempCaches( pair.Value ) );

				Reset();    // メモリ負荷を抑える為、保存キャッシュを消去

/*
				if ( SMDebugManager.IS_DEVELOP ) {
					// デバッグ情報を表示
					SMLog.Debug(
						string.Join( "\n",
							$"アプリ版 : サーバー{setting._versionByServer} : 保存{setting._versionBySave}",
							$"サーバー版 : サーバー{setting._serverVersionByServer} : " +
								$"保存{setting._serverVersionBySave}",
							$"アプリ : {( setting._isRequestUpdateApplication ? "更新が必要" : "最新" )}",
							$"読込 : {( setting._isRequestUpdateServer ? "サーバー" : "キャッシュ" )}"
						),
						SMLogTag.Server
					);
				}
*/

				await Load();
			} );


			_saveEvent.AddLast( async canceler => {
				Reset();	// 既存の保存キャッシュを消去

				// サーバー情報のみ、一時キャッシュを保存
				_tempCaches.GetAlls()
					.Where( d => PathSMUtility.IsURL( d._path ) )
					.ForEach( d => Register( d ) );

				await Enumerable.Empty<UniTask>()
					.Concat( _textureDatas.Select( pair => pair.Value.Save() ) )
					.Concat( _audioDatas.Select( pair => pair.Value.Save() ) )
					.Concat( _textDatas.Select( pair => pair.Value.Save() ) )
					.Concat( _rawDatas.Select( pair => pair.Value.Save() ) );

				await loader.Save( SMMainSetting.CACHE_FILE_NAME, this );

				Reset();	// メモリ負荷を抑える為、保存キャッシュを消去

				await Save();
			} );
		}

		/// <summary>
		/// ● 解放（補助）
		/// </summary>
		protected override void DisposeSub() {
			base.DisposeSub();

			Reset();
			_loadEvent.Dispose();
			_saveEvent.Dispose();
		}

		/// <summary>
		/// ● リセット
		/// </summary>
		public void Reset() {
			_textureDatas?.ForEach( pair => pair.Value.Dispose() );
			_textureDatas?.Clear();
			_audioDatas?.ForEach( pair => pair.Value.Dispose() );
			_audioDatas?.Clear();
			_textDatas?.ForEach( pair => pair.Value.Dispose() );
			_textDatas?.Clear();
			_rawDatas?.ForEach( pair => pair.Value.Dispose() );
			_rawDatas?.Clear();
		}

		///------------------------------------------------------------------------------------------------
		/// ● 登録、解除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録
		/// </summary>
		void Register( SMTemporaryCacheData data ) {
			var type = SMAllDataManager.GetDataType( data._type );

			switch ( type ) {
				case SMDataType.Texture:
				case SMDataType.Sprite:
					_textureDatas[data._path] = new SMSaveCacheData<SMTextureRawData>( data, type );
					break;

				case SMDataType.Audio:
					_audioDatas[data._path] = new SMSaveCacheData<SMAudioRawData>( data, type );
					break;

				case SMDataType.Text:
					_textDatas[data._path] = new SMSaveCacheData<string>( data, type );
					break;

				case SMDataType.Raw:
					_rawDatas[data._path] = new SMSaveCacheData<byte[]>( data, type );
					break;

				default:
					throw new InvalidOperationException( string.Join( "\n",
						$"保存情報が未対応 : {type}",
						$"{nameof( SMSaveCacheDataManager )}.{nameof( Register )}",
						$"{nameof( data )} : {data}",
						$"{this}"
					) );
			}
		}

		/// <summary>
		/// ● 一時キャッシュに登録
		/// </summary>
		void RegisterTempCaches<T>( SMSaveCacheData<T> data ) where T : class {
			_tempCaches.Register( data._cacheData );
			// Dispose時に、解放されないように、参照を切る
			data._cacheData = null;
		}

		/// <summary>
		/// ● 登録解除
		/// </summary>
		void Unregister( string path ) {
			if ( _textureDatas.DisposeAndRemove( path ) )	{ return; }
			if ( _audioDatas.DisposeAndRemove( path ) )		{ return; }
			if ( _textDatas.DisposeAndRemove( path ) )		{ return; }
			if ( _rawDatas.DisposeAndRemove( path ) )		{ return; }
		}
	}
}