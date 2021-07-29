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
	/// ■ アプリケーションの情報クラス
	/// </summary>
	///====================================================================================================
	public class ApplicationData : SMURLData<SMEdition> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>辞書への登録鍵</summary>
		public override SMEdition _registerKey		=> _edition;
		/// <summary>URL読込の初期添字</summary>
		protected override int _setURLStartIndex	=> 3;

		/// <summary>商品版</summary>
		public SMEdition _edition		{ get; private set; }
		/// <summary>更新版</summary>
		public string _version			{ get; private set; }
		/// <summary>サーバー版</summary>
		public string _serverVersion	{ get; private set; }

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		public override void Setup( string fileName, int index, List<string> texts ) {
			_edition = texts[0].ToEnum<SMEdition>();
			_version = texts[1];
			if ( _version == "?" )	{ _version = SMMainSetting.APPLICATION_VERSION; }
			_serverVersion = texts[2];

			base.Setup( fileName, index, texts );
		}
	}
}