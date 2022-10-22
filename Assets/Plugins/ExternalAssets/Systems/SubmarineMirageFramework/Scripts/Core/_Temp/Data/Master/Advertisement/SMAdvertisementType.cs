//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {



	/// <summary>広告の型</summary>
	public enum SMAdvertisementType {
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
}