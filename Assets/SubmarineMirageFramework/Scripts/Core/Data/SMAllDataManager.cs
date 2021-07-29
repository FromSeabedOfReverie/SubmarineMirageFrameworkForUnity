//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestData
namespace SubmarineMirage.Data {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Service;
	using Task;
	using File;
	using Cache;
	using Save;
	using Server;
	using Extension;
	using Setting;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 全情報の管理クラス
	/// </summary>
	///====================================================================================================
	public class SMAllDataManager : SMTask, ISMService {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		public override SMTaskRunType _type => SMTaskRunType.Sequential;

		SMFileManager _fileManager { get; set; }

		/// <summary>全情報の辞書</summary>
		readonly Dictionary<Type, IBaseSMDataManager> _datas = new Dictionary<Type, IBaseSMDataManager>();
		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMAllDataManager() {
			_disposables.AddFirst( () => {
				_datas.ForEach( pair => pair.Value.Dispose() );
				_datas.Clear();

				_fileManager = null;
			} );
		}

		/// <summary>
		/// ● 作成
		/// </summary>
		public override void Create() {
			_fileManager = SMServiceLocator.Resolve<SMFileManager>();
			var setting = SMServiceLocator.Resolve<ISMDataSetting>();


			// 一時キャッシュを登録
			Register( new SMTemporaryCacheDataManager() );
			// 保存キャッシュを登録
			Register( new SMSaveCacheDataManager() );
			// アプリ情報を登録
			Register( new ApplicationDataManager() );
			// アプリ固有の保存情報を登録
			setting.RegisterDatas( SMDataSettingType.Save );


			// アプリ固有のサーバー情報を登録
			setting.RegisterDatas( SMDataSettingType.Server );


			// システム情報を登録
			Register( new SMCSVDataManager<SMSystemInfo, SMSystemInfoData>(
				"System", "SystemInfo", SMFileLocation.Resource, 1
			) );
			// 購入失敗情報を登録
			Register( new SMCSVDataManager< SMPurchaseError, SMResultData<SMPurchaseError> >(
				"System", "PurchaseErrors", SMFileLocation.Resource, 1
			) );
			// 広告失敗情報を登録
			Register( new SMCSVDataManager< SMAdvertisementError, SMResultData<SMAdvertisementError> >(
				"System", "AdvertisementErrors", SMFileLocation.Resource, 1
			) );
			// 購入情報を登録
//			Register( new PurchaseProductDataManager() );
			// 広告情報を登録
			Register( new SMCSVDataManager<SMAdvertisementType, SMAdvertisementData>(
				"System", "AdvertisementIDs", SMFileLocation.Resource, 1 ) );
			// アプリ固有のマスター情報を登録
			setting.RegisterDatas( SMDataSettingType.Master );


			// 情報設定を解放
			SMServiceLocator.Unregister<ISMDataSetting>();


			_selfInitializeEvent.AddLast( async canceler => {
				await LoadAll();
			} );
		}

		///------------------------------------------------------------------------------------------------
		/// ● 登録、解除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録
		/// </summary>
		public T Register<T>( T manager ) where T : class, IBaseSMDataManager {
			var key = typeof( T );
			_datas[key] = manager;
			return manager;
		}

		///------------------------------------------------------------------------------------------------
		/// ● 取得
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得
		/// </summary>
		public T Get<T>() where T : class, IBaseSMDataManager
			=> _datas.GetOrDefault( typeof( T ) ) as T;

		/// <summary>
		/// ● 取得（CSV管理）
		/// </summary>
		public SMCSVDataManager<TKey, TValue> Get<TKey, TValue>() where TValue : SMCSVData<TKey>, new()
			=> _datas
				.Select( pair => pair.Value )
				.FirstOrDefault( manager => manager is SMCSVDataManager<TKey, TValue> )
				as SMCSVDataManager<TKey, TValue>;

		/// <summary>
		/// ● 型から、情報型を取得
		/// </summary>
		public static SMDataType GetDataType( Type type ) => (
			type.IsInheritance<Texture2D>()			? SMDataType.Texture :
			type.IsInheritance<Sprite>()			? SMDataType.Sprite :
			type.IsInheritance<AudioClip>()			? SMDataType.Audio :
			type.IsInheritance<ISMSerializeData>()	? SMDataType.Serialize :
			type.IsInheritance<string>()			? SMDataType.Text
													: SMDataType.Raw
		);

		///------------------------------------------------------------------------------------------------
		/// ● 読み書き
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		public async UniTask LoadAll() {
			var saveCache = Get<SMSaveCacheDataManager>();
			var tempCache = Get<SMTemporaryCacheDataManager>();

			foreach ( var pair in _datas ) {
				await pair.Value._loadEvent.Run( _asyncCancelerOnDispose );
			}

			// 読込成功の場合
			if ( _fileManager._isSuccess ) {
				SMLog.Debug( $"{this.GetAboutName()} : 読込完了", SMLogTag.Data );

				// 更新する必要があり、データをダウンロードした場合、キャッシュ保存
				if ( _application._isUpdateServer && _fileManager._isDownloaded ) {
					await saveCache.Save();
				}
			}
			_fileManager.ResetAllCount();    // 計測初期化

#if TestData
			// キャッシュ読込の確認用
			SMLog.Debug( saveCache );
			SMLog.Debug( tempCache );
#endif
		}

		/// <summary>
		/// ● 保存
		/// </summary>
		public async UniTask SaveAll() {
			foreach ( var pair in _datas ) {
				await pair.Value._saveEvent.Run( _asyncCancelerOnDispose );
			}
		}

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● サーバーのキャッシュを読込可能か？
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public bool IsCanLoadServerCache() {
			var networkManager = SMServiceLocator.Resolve<SMNetworkManager>();

			// 接続切れか、ダウンロード済キャッシュが最新の場合、キャッシュ使用可能
			return !networkManager._isConnecting || !_application._isUpdateServer;
		}
	}
}