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
	/// ■ 購入商品情報の管理クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class PurchaseProductDataManager : CSVDataManager<string, PurchaseProductData> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public PurchaseProductDataManager()
			: base( "System", "PurchaseProducts", FileLoader.Type.Resource, 1 )
		{
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 登録
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Register( string key, PurchaseProductData data ) {
			var isRelease =
#if DEVELOP
				false;
#else
				true;
#endif
			if ( isRelease == data._isRelease ) {
				base.Register( key, data );
			}
		}
	}
}