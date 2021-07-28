//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.Server {
	using System.Collections.Generic;
	using Save;
	///====================================================================================================
	/// <summary>
	/// ■ アプリケーションの情報クラス
	/// </summary>
	///====================================================================================================
	public class ApplicationData : URLData<string> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>アプリケーション名</summary>
		public string _applicationName;
		/// <summary>アプリケーション版</summary>
		public string _version;
		/// <summary>サーバー版</summary>
		public string _serverVersion;

		/// <summary>辞書への登録鍵</summary>
		public override string _registerKey			=> _applicationName;
		/// <summary>URL読込の初期添字</summary>
		protected override int _setURLStartIndex	=> 3;
		/// <summary>設定情報</summary>
		SettingData _settingData					=> AllDataManager.s_instance._save._setting._data;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Setup( string fileName, int index, List<string> texts ) {
			_applicationName	= texts[0];

			_version			= texts[1];
			if ( _version == "?" )	{ _version = _settingData._version; }

			_serverVersion		= texts[2];

			base.Setup( fileName, index, texts );
		}
	}
}