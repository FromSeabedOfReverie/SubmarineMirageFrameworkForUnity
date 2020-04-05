//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Data.Server {
	using System.Linq;
	using UniRx.Async;
	using UniRx;
	using File;
	using Save;
	using Debug;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ サーバー情報の管理クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class ServerDataManager : ClassDataManager<IBaseDataManager> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>アプリケーション情報</summary>
		public readonly ApplicationDataManager _application = new ApplicationDataManager();
		/// <summary>製品版の宣伝情報</summary>
		public readonly CMDataManager _cm = new CMDataManager();
		/// <summary>他アプリの宣伝情報</summary>
		public readonly ApplicationCMDataManager _applicationCM = new ApplicationCMDataManager();

		/// <summary>書類読込</summary>
		FileLoader _loader			=> FileManager.s_instance._fileLoader;
		/// <summary>保存情報</summary>
		SaveDataManager _saveData	=> AllDataManager.s_instance._save;
		/// <summary>設定情報</summary>
		SettingDataManager _setting	=> _saveData._setting;
		/// <summary>サーバーのキャッシュ情報</summary>
		ServerCacheDataSaver _cache	=> _saveData._cache;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● サーバーのキャッシュを読込可能か？
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public bool IsCanLoadServerCache() {
			// 接続切れか、ダウンロード済キャッシュが最新の場合、キャッシュ使用可能
			return !NetworkManager.s_instance._isConnecting || !_application._isUpdateServer;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Load() {
			Register( _application );	// アプリ情報を登録
			Register( _cm );			// CM情報を登録
			Register( _applicationCM );	// アプリCM情報を登録

			await base.Load();			// 全読込
			await CheckLoadSuccess();	// 読込成功判定

			FileManager.s_instance.ResetAllCount();  // 計測初期化

#if DEVELOP && false
			// キャッシュ読込の確認用
			Log.Debug( FileManager.s_instance._cacheData._allData.Keys.ToList().ToDeepString() );
			Log.Debug( _cache._data._allData.Keys.ToList().ToDeepString() );
#endif
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込の成功判定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		async UniTask CheckLoadSuccess() {
			// 読込成功の場合
			if ( FileManager.s_instance._isSuccess ) {
				// 更新する必要があり、データをダウンロードした場合、キャッシュ保存
				if ( _application._isUpdateServer && FileManager.s_instance._isDownloaded ) {
					await _cache.Save();
				}
				Log.Debug( $"{this.GetAboutName()} : 読込完了", Log.Tag.Server );
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 保存
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Save() {
			Log.Error( $"保存機能が無 : { GetType() }", Log.Tag.Server );
			await UniTask.Delay( 0 );
		}
	}
}