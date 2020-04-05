//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.Save {
	using UniRx.Async;
	using File;
	using Debug;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ 保存情報の管理クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class SaveDataManager : ClassDataManager<SaveData> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>遊戯情報</summary>
		public readonly PlayDataManager _play = new PlayDataManager();
		/// <summary>設定情報</summary>
		public readonly SettingDataManager _setting = new SettingDataManager();
		/// <summary>サーバーのキャッシュ情報</summary>
		public readonly ServerCacheDataSaver _cache = new ServerCacheDataSaver();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Load() {
			Register( _setting );
			Register( _play );
			Register( _cache );

			await base.Load();

			// 読込成功の場合
			if ( FileManager.s_instance._isSuccess ) {
				Log.Debug( $"{this.GetAboutName()} : 読込完了", Log.Tag.File );
			}
			FileManager.s_instance.ResetAllCount();	// 計測初期化
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 保存
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Save() {
			await base.Save();
		}
	}
}