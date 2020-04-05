//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using File;
	///====================================================================================================
	/// <summary>
	/// ■ 広告失敗情報の管理クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class AdvertisementErrorDataManager
		: CSVDataManager< AdvertisementErrorDataManager.Type, ResultData<AdvertisementErrorDataManager.Type> >
	{
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>型の定数</summary>
		public enum Type {
			/// <summary>成功</summary>
			Success,
			/// <summary>広告が無効</summary>
			Dissable,
			/// <summary>通信未接続</summary>
			NoNetwork,
			/// <summary>非対応</summary>
			NoSupport,
			/// <summary>未初期化</summary>
			NoInitialize,
			/// <summary>配信中</summary>
			Downloading,
			/// <summary>重複呼び出し</summary>
			DuplicateCall,
			/// <summary>その他</summary>
			Other,
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public AdvertisementErrorDataManager()
			: base( "System", "AdvertisementErrors", FileLoader.Type.Resource, 1 )
		{
		}
	}
}