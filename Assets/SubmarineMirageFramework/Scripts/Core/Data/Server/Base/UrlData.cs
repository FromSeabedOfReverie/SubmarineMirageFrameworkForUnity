//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.Server {
	using System.Collections.Generic;
	///====================================================================================================
	/// <summary>
	/// ■ WEBサイトURLの情報クラス
	/// </summary>
	///====================================================================================================
	public abstract class URLData<T> : SMCSVData<T> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>AndroidのURL</summary>
		public string _androidURL	{ get; private set; }
		/// <summary>WindowsのURL</summary>
		public string _windowsURL	{ get; private set; }
		/// <summary>iOSのURL</summary>
		public string _iOSURL		{ get; private set; }
		/// <summary>MacOSXのURL</summary>
		public string _macOSXURL	{ get; private set; }
		/// <summary>LinuxのURL</summary>
		public string _linuxURL		{ get; private set; }
		/// <summary>URL読込の初期添字</summary>
		protected abstract int _setURLStartIndex { get; }
		///------------------------------------------------------------------------------------------------
		/// ● アクセサ
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● URLを取得
		/// </summary>
		public string _url
#if UNITY_ANDROID
			=> _androidURL;

#elif UNITY_STANDALONE_WIN || UNITY_WSA
			=> _windowsURL;

#elif UNITY_IOS
			=> _iOSURL;

#elif UNITY_STANDALONE_OSX
			=> _macOSXURL;

#elif UNITY_STANDALONE_LINUX
			=> _linuxURL;
#endif
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Setup( string fileName, int index, List<string> texts ) {
			_androidURL	= texts[_setURLStartIndex];
			_windowsURL	= texts[_setURLStartIndex + 1];
			_iOSURL		= texts[_setURLStartIndex + 2];
			_macOSXURL	= texts[_setURLStartIndex + 3];
			_linuxURL	= texts[_setURLStartIndex + 4];
		}
	}
}