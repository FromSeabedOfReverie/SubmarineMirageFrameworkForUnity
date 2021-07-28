//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.Server {
	using Cysharp.Threading.Tasks;
	using File;
	using Save;
	using Utility;
	using Setting;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ アプリケーション情報の管理クラス
	/// </summary>
	///====================================================================================================
	public class ApplicationDataManager : SMCSVDataManager<string, ApplicationData> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>設定情報</summary>
		SettingData _settingData	=> AllDataManager.s_instance._save._setting._data;
		/// <summary>書類読込</summary>
		FileLoader _loader			=> FileManager.s_instance._fileLoader;

		/// <summary>アプリケーションを更新するべきか？</summary>
		public bool _isUpdateApplication	{ get; private set; }	= true;
		/// <summary>サーバーキャッシュ情報を更新するべきか？</summary>
		public bool _isUpdateServer			{ get; private set; }	= true;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public ApplicationDataManager() : base(
			"https://docs.google.com/spreadsheets/d/e/2PACX-1vQZjl0KQ3qdx1ghjDLczrLpmWQ11Ao75IdaSobLMoFHjuzhG4pTCX0bXvZgGl_P4-2fjLCdCbBKHaRE/pub?gid=1615869423&single=true&output=csv",
			"",
			SMFileLocation.Server, 1
		) {
			_loadEvent.AddLast( async canceler => {
				var currentApp = _settingData._version;
				var currentServer = AllDataManager.s_instance._save._cache._data._serverVersion;
				var serverData = NetworkManager.s_instance._isConnecting ? Get() : null;
				var newApp = serverData?._version;
				var newServer = serverData?._serverVersion;
				// 更新状況を判定
				// ダウンロード失敗の場合、更新を見送る
				_isUpdateApplication = newApp != null && currentApp != newApp;
				_isUpdateServer = newServer != null && currentServer != newServer;

				if ( SMDebugManager.IS_DEVELOP ) {
					// デバッグ情報を表示
					SMLog.Debug(
						string.Join( "\n",
							$"アプリ : 新{newApp} : 現{currentApp}",
							$"サーバー : 新{newServer} : 現{currentServer}",
							$"アプリ : {( _isUpdateApplication ? "更新が必要" : "最新" )}",
							$"読込 : {( _isUpdateServer ? "サーバー" : "キャッシュ" )}"
						),
						SMLogTag.Server
					);
				}
				await UTask.DontWait();
			} );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public ApplicationData Get() {
			var name = _settingData._edition == SettingData.Edition.Trial ? "Trial" : "Product";
			return Get( name );
		}
	}
}