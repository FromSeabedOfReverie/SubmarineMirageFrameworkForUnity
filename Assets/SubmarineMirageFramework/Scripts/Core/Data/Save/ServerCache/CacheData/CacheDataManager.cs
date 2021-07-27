//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.Save {
	using System.Collections.Generic;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ キャッシュ情報の管理クラス
	///		サーバー受信情報、Resourcesファイル等の、全情報をキャッシュし、格納している。
	///		アプリ起動時のみ確保され、セーブされない、一時キャッシュ。
	/// </summary>
	///====================================================================================================
	public class CacheDataManager : BaseSMData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>全キャッシュ情報の一覧</summary>
		public readonly Dictionary<string, CacheData> _allData = new Dictionary<string, CacheData>();

		/// <summary>サーバーのキャッシュ情報</summary>
		ServerCacheDataManager _serverCache	=> AllDataManager.s_instance._save._cache._data;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public CacheDataManager() {
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込済か？
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public bool IsLoaded( string path ) {
			return _allData.ContainsKey( path );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public CacheData Get( string path ) {
			var data = _allData.GetOrDefault( path );
			return data;
		}
		///------------------------------------------------------------------------------------------------
		/// ● 登録
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録（初登録用）
		/// </summary>
		public void Register( string path, object data ) {
			var cache = Get( path );
			if ( cache != null ) {
				cache.SetData( data );
				return;
			}

			Register( new CacheData( path, data ) );
		}
		/// <summary>
		/// ● 登録（サーバーキャッシュから復元設定用）
		/// </summary>
		public void Register( CacheData data ) {
			_allData[data._path] = data;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録解除
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Unregister( string path ) {
			_allData.Remove( path );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 解放
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Dispose() {
			_allData.Clear();
			_serverCache.Dispose();
		}
	}
}