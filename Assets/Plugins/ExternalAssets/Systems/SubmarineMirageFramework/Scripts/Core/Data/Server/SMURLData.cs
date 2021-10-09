//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.Server {
	using System.Collections.Generic;
	using KoganeUnityLib;
	using Setting;
	///====================================================================================================
	/// <summary>
	/// ■ WEBサイトURLの情報クラス
	/// </summary>
	///====================================================================================================
	public abstract class SMURLData<T> : SMCSVData<T> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>URL情報の一覧</summary>
		[SMShow] readonly Dictionary<SMPlatformType, string> _urls = new Dictionary<SMPlatformType, string>();
		/// <summary>URL読込の初期添字</summary>
		protected abstract int _setURLStartIndex { get; }

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		public override void Setup( string fileName, int index, List<string> texts ) {
			_urls[SMPlatformType.Windows]	= texts[_setURLStartIndex + 0];
			_urls[SMPlatformType.MacOSX]	= texts[_setURLStartIndex + 1];
			_urls[SMPlatformType.Linux]		= texts[_setURLStartIndex + 2];
			_urls[SMPlatformType.Android]	= texts[_setURLStartIndex + 3];
			_urls[SMPlatformType.IOS]		= texts[_setURLStartIndex + 4];
			_urls[SMPlatformType.WebGL]		= texts[_setURLStartIndex + 5];
		}

		///------------------------------------------------------------------------------------------------
		/// ● 取得
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● URLを取得
		/// </summary>
		public string GetURL( SMPlatformType? type = null ) => (
			type.HasValue	? _urls.GetOrDefault( type.Value )
							: _urls[SMMainSetting.PLATFORM]
		);
	}
}