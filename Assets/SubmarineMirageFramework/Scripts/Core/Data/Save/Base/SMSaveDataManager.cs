//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.Save {
	using System;
	using Cysharp.Threading.Tasks;
	using Service;
	using File;
	using Extension;
	using Setting;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 保存情報の管理クラス
	/// </summary>
	///====================================================================================================
	public class SMSaveDataManager : BaseSMDataManager<Type, BaseSMSaveData> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
// TODO : PlayDataManager、SettingDataManagerを、プロジェクト固有箇所に分離する
		/// <summary>遊戯情報</summary>
//		public readonly PlayDataManager _play = new PlayDataManager();
		/// <summary>設定情報</summary>
//		public readonly SettingDataManager _setting = new SettingDataManager();
		/// <summary>サーバーのキャッシュ情報</summary>
		public readonly ServerCacheDataSaver _cache = new ServerCacheDataSaver();

		///------------------------------------------------------------------------------------------------
		/// ● 読み書き
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		public override async UniTask Load() {
//			Register( _setting.GetType(), _setting );
//			Register( _play.GetType(), _play );
			Register( _cache.GetType(), _cache );

			await base.Load();

			// 読込成功の場合
			var fileManager = SMServiceLocator.Resolve<SMFileManager>();
			if ( fileManager._isSuccess ) {
				SMLog.Debug( $"{this.GetAboutName()} : 読込完了", SMLogTag.File );
			}
			fileManager.ResetAllCount();	// 計測初期化
		}

		/// <summary>
		/// ● 保存
		/// </summary>
		public override async UniTask Save() {
			await base.Save();
		}
	}
}