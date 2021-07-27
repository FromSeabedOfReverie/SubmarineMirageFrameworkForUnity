//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.Save {
	using Cysharp.Threading.Tasks;
	using Data.File;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ サーバーキャッシュ情報の保存クラス
	///----------------------------------------------------------------------------------------------------
	///		サーバー配信のキャッシュ情報を格納。
	///		このキャッシュに保存された情報は、必ずセーブされる。
	/// </summary>
	///====================================================================================================
	public class ServerCacheDataSaver : BaseSMSaveData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>サーバーキャッシュ情報の、保存書類名</summary>
		const string FILE_NAME = "CacheData.data";

		/// <summary>サーバーキャッシュ情報</summary>
		public ServerCacheDataManager _data	{ get; private set; } = new ServerCacheDataManager();

		/// <summary>暗号化書類の読み書き</summary>
		CryptoLoader _loader		=> FileManager.s_instance._cryptoLoader;
		/// <summary>設定情報</summary>
		SettingDataManager _setting	=> AllDataManager.s_instance._save._setting;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Load() {
			var newData = await _loader.Load<ServerCacheDataManager>( FILE_NAME );
			if ( newData != null )	{ _data = newData; }

			await _data.Load();

			await base.Load();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 保存
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Save() {
			// サーバー版をダウンロード版に変更し、保存
			_data._serverVersion = AllDataManager.s_instance._server._application.Get()._serverVersion;

			await _data.Save();
			await _loader.Save( FILE_NAME, _data );

			await base.Save();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 解放
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Dispose() {
			_data.Dispose();
		}
	}
}