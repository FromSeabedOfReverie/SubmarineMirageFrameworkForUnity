//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.Save {
	using System;
	using Cysharp.Threading.Tasks;
	using Utility;
	///====================================================================================================
	/// <summary>
	/// ■ キャッシュの情報クラス
	///		サーバー受信情報、Resourcesファイル等の、キャッシュ情報を格納。
	///		アプリ起動時のみ確保され、セーブされない、一時キャッシュ。
	/// </summary>
	///====================================================================================================
	public class CacheData : BaseSMData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>登録鍵になる、書類の階層</summary>
		public string _path;
		/// <summary>情報の型</summary>
		public Type _type;
		/// <summary>情報</summary>
		public object _data;

		/// <summary>サーバーのキャッシュ情報</summary>
		ServerCacheDataManager _serverCache	=> AllDataManager.s_instance._save._cache._data;
		///------------------------------------------------------------------------------------------------
		/// ● コンストラクタ
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ（サーバーキャッシュの読込時用）
		/// </summary>
		public CacheData() {
		}
		/// <summary>
		/// ● コンストラクタ（キャッシュ登録時用）
		/// </summary>
		public CacheData( string path, object data ) {
			_path = path;
			SetData( data );

			RegisterServerCache().Forget();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● サーバーキャッシュ登録
		/// </summary>
		///------------------------------------------------------------------------------------------------
		async UniTask RegisterServerCache() {
			if ( !PathSMUtility.IsURL( _path ) )	{ return; }

			var c = new System.Threading.CancellationTokenSource();
			await UTask.WaitUntil( c.Token, () => _data != null );	// 読込後に再登録される為、最初はnullとなる
			c.Dispose();
			_serverCache.Register( this );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 情報を設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void SetData( object data ) {
			_data = data;
			_type = _data?.GetType();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public object Get() {
			return _data;
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● デストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		~CacheData() {
			if ( PathSMUtility.IsURL( _path ) ) {
				_serverCache.Unregister( _path );
			}
		}
	}
}