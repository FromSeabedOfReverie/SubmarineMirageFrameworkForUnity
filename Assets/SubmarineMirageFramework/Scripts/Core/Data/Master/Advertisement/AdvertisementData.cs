//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Data {
	using System.Collections.Generic;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 広告の情報クラス
	///----------------------------------------------------------------------------------------------------
	///		広告情報を保存している。
	/// </summary>
	///====================================================================================================
	public class AdvertisementData : CSVData<AdvertisementData.Type> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>型</summary>
		public enum Type {
			/// <summary>AdMobの登録アプリ番号</summary>
			AdMobApplication,
			/// <summary>AdMobの無番号</summary>
			AdMobUnknown,
			/// <summary>AdMobの端末試験番号</summary>
			AdMobDeviceTest,
			/// <summary>AdMobの帯広告番号</summary>
			AdMobBanner,
			/// <summary>AdMobの帯広告試験番号</summary>
			AdMobBannerTest,
			/// <summary>AdMobの間質広告番号</summary>
			AdMobInterstitial,
			/// <summary>AdMobの間質広告試験番号</summary>
			AdMobInterstitialTest,
			/// <summary>AdMobの動画広告番号</summary>
			AdMobMovie,
			/// <summary>AdMobの動画広告試験番号</summary>
			AdMobMovieTest,
			/// <summary>UnityAdsの動画広告番号</summary>
			UnityAds,
		}

		/// <summary>辞書への登録鍵</summary>
		public override Type _registerKey => _type;

		/// <summary>型</summary>
		public Type _type { get; private set; }
		/// <summary>Android番号</summary>
		public string _androidID { get; private set; }
		/// <summary>iOS番号</summary>
		public string _iOSID { get; private set; }
		/// <summary>補足説明</summary>
		public string _info { get; private set; }
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Set( string fileName, int index, List<string> texts ) {
			_type		= texts[0].ToEnum<Type>();
			_androidID	= texts[1];
			_iOSID		= texts[2];
			_info		= texts[3];
		}
	}
}