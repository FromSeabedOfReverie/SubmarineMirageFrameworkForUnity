//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data {
	using System.Collections.Generic;
	///====================================================================================================
	/// <summary>
	/// ■ システムの情報クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class SystemInfoData : ResultData<SystemInfoData.Type> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>型の定数</summary>
		public enum Type {
			/// <summary>広告未接続失敗</summary>
			AdvertisementErrorNoNetwork,
			/// <summary>広告未初期化失敗</summary>
			AdvertisementErrorNoInitialize,
			/// <summary>広告未配信失敗</summary>
			AdvertisementErrorDownloading,

			/// <summary>課金未接続失敗</summary>
			PurchaseErrorNoNetwork,
			/// <summary>課金未初期化失敗</summary>
			PurchaseErrorNoInitialize,
			/// <summary>課金未許可失敗</summary>
			PurchaseErrorNoPermission,
			/// <summary>課金謎失敗</summary>
			PurchaseErrorUnknown,
			/// <summary>課金復元失敗</summary>
			PurchaseErrorRestore,
			/// <summary>課金復元成功</summary>
			PurchaseSuccessRestore,

			/// <summary>アプリ更新通知</summary>
			ApplicationUpdateNotice,
			/// <summary>寄付通知</summary>
			PurchasedDonationNotice,
		}

		/// <summary>題目</summary>
		public string _title { get; private set; }
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Set( string fileName, int index, List<string> texts ) {
			base.Set( fileName, index, texts );

			_title = texts[2];
		}
	}
}