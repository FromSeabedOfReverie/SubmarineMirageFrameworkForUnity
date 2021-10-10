//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	///====================================================================================================
	/// <summary>
	/// ■ 一時キャッシュ情報の管理クラス
	///		アプリ起動時のみ確保され、セーブされない、一時キャッシュ。
	///		サーバー受信情報、Resourcesファイル等の、全情報をキャッシュし、格納している。
	/// </summary>
	///====================================================================================================
	public class SMTemporaryCacheDataManager : BaseSMDataManager<string, SMTemporaryCacheData> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMTemporaryCacheDataManager() {
		}

		///------------------------------------------------------------------------------------------------
		/// ● 登録、解除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録（書込用）
		/// </summary>
		public void Register( string path, object data ) {
			var cacheData = Get( path );
			if ( cacheData != null ) {
				cacheData.Setup( data );
				return;
			}
			_datas[path] = new SMTemporaryCacheData( path, data );
		}

		/// <summary>
		/// ● 登録（読込用）
		/// </summary>
		public void Register( SMTemporaryCacheData data ) {
			_datas[data._path] = data;
		}
	}
}