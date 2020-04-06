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
	/// ■ 広告情報の管理クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class AdvertisementDataManager : CSVDataManager<AdvertisementData.Type, AdvertisementData> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public AdvertisementDataManager()
			: base( "System", "AdvertisementIDs", FileLoader.Type.Resource, 1 )
		{
		}
	}
}