//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using System;
	///====================================================================================================
	/// <summary>
	/// ■ 一時キャッシュの情報クラス
	///		アプリ起動時のみ確保され、セーブされない、一時キャッシュ。
	///		サーバー受信情報、Resourcesファイル等の、キャッシュ情報を格納。
	/// </summary>
	///====================================================================================================
	public class SMTemporaryCacheData : BaseSMData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>登録鍵になる、書類の階層</summary>
		[SMShow] public string _path	{ get; set; }
		/// <summary>情報の型</summary>
		public Type _type	{ get; set; }
		/// <summary>情報</summary>
		public object _data	{ get; set; }

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ（読込用）
		/// </summary>
		public SMTemporaryCacheData() {
		}

		/// <summary>
		/// ● コンストラクタ（書込用）
		/// </summary>
		public SMTemporaryCacheData( string path, object data ) {
			_path = path;
			Setup( data );
		}

		/// <summary>
		/// ● 設定
		///		作成時にdataが未読込の場合、読込後に再度呼ばれる場合がある。
		/// </summary>
		public void Setup( object data ) {
			_data = data;
			_type = _data?.GetType();
		}
	}
}