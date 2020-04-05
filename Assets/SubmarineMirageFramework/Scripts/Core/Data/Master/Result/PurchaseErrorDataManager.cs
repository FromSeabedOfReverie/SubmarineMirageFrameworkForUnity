//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Data {
	using File;
	///====================================================================================================
	/// <summary>
	/// ■ 購入失敗情報の管理クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class PurchaseErrorDataManager
		: CSVDataManager< PurchaseErrorDataManager.Type, ResultData<PurchaseErrorDataManager.Type> >
	{
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>型の定数</summary>
		public enum Type {
			/// <summary>成功</summary>
			Success,
			/// <summary>課金が無効</summary>
			Dissable,
			/// <summary>通信未接続</summary>
			NoNetwork,
			/// <summary>購入復元に非対応</summary>
			NoSupport,
			/// <summary>未初期化</summary>
			NoInitialize,
			/// <summary>未販売商品指定</summary>
			UnknownProduct,
			/// <summary>領収書無効</summary>
			InvalidReceipt,
			/// <summary>重複呼び出し</summary>
			DuplicateCall,
			/// <summary>その他</summary>
			Other,

			// UnityIAPのPurchaseFailureReasonをラッピング
			/// <summary>購入不可</summary>
			PurchasingUnavailable,
			/// <summary>既存購入保留中</summary>
			ExistingPurchasePending,
			/// <summary>商品利用不可</summary>
			ProductUnavailable,
			/// <summary>署名無効</summary>
			SignatureInvalid,
			/// <summary>利用者取消</summary>
			UserCancelled,
			/// <summary>支払い拒否</summary>
			PaymentDeclined,
			/// <summary>重複取引</summary>
			DuplicateTransaction,

			// UnityIAPのInitializationFailureReasonをラッピング
			/// <summary>使用可能商品が無</summary>
			NoProductsAvailable,
			/// <summary>知らないアプリ</summary>
			AppNotKnown,
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public PurchaseErrorDataManager()
			: base( "System", "PurchaseErrors", FileLoader.Type.Resource, 1 )
		{
		}
	}
}