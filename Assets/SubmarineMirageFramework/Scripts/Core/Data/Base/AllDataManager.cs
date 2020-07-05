//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using Singleton;
	using Server;
	using Save;
	using Debug;
	using Extension;
	using File;
	///====================================================================================================
	/// <summary>
	/// ■ 全情報の管理クラス
	///----------------------------------------------------------------------------------------------------
	///		シングルトンとデータ管理クラスを、同時に継承できない為、データを格納した。
	/// </summary>
	///====================================================================================================
	public class AllDataManager : Singleton<AllDataManager> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>全情報の辞書</summary>
		readonly ClassDataManager<IBaseDataManager> _allData = new ClassDataManager<IBaseDataManager>();
		/// <summary>保存情報の管理</summary>
		public readonly SaveDataManager _save = new SaveDataManager();
		/// <summary>サーバー情報の管理</summary>
		public readonly ServerDataManager _server = new ServerDataManager();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public T Get<T>() where T : IBaseDataManager {
			return _allData.Get<T>();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Load() {
			// 保存情報を登録
			_allData.Register( _save );
			// サーバー情報を登録
			_allData.Register( _server );

			// システム情報を登録
			_allData.Register( new SystemInfoDataManager() );
			// 購入失敗情報を登録
			_allData.Register( new PurchaseErrorDataManager() );
			// 広告失敗情報を登録
			_allData.Register( new AdvertisementErrorDataManager() );
			// 購入情報を登録
			_allData.Register( new PurchaseProductDataManager() );
			// 広告情報を登録
			_allData.Register( new AdvertisementDataManager() );
			// アイテム情報を登録
			_allData.Register( new ItemDataManager() );

			// 単語情報を登録
			_allData.Register( new AllWordDataManager() );
			// AI情報を登録
			_allData.Register( new AllAIDataManager() );


			await _allData.Load();
			await base.Load();


			// 読込成功の場合
			if ( FileManager.s_instance._isSuccess ) {
				Log.Debug( $"{this.GetAboutName()} : 読込完了", Log.Tag.Data );
			}
			FileManager.s_instance.ResetAllCount();  // 計測初期化

#if false
			// キャッシュ読込の確認用
			Log.Debug( FileManager.s_instance._cacheData._allData.Keys.ToList().ToDeepString() );
			Log.Debug( _save._cache._data._allData.Keys.ToList().ToDeepString() );
#endif
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 保存
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public async UniTask Save() {
//			await _allData.Save();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 解放
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Dispose() {
			_allData.Dispose();
		}
	}
}