//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.Server {
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using File;
	///====================================================================================================
	/// <summary>
	/// ■ 製品版の宣伝情報クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class CMData : CSVData<CMData.Platform> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>アプリ実行機種の型</summary>
		public enum Platform {
			/// <summary>Android端末</summary>
			Android,
			/// <summary>Windowsパソコン</summary>
			Windows,
			/// <summary>iOS端末</summary>
			IOS,
			/// <summary>Macパソコン</summary>
			MacOSX,
			/// <summary>Linuxパソコン</summary>
			Linux,
		}

		/// <summary>辞書への登録鍵</summary>
		public override Platform _registerKey => _platform;

		/// <summary>アプリ実行機種</summary>
		public Platform _platform;
		/// <summary>表題名</summary>
		public string _title;
		/// <summary>説明文</summary>
		public string _info;
		/// <summary>バナー画像のURL</summary>
		public string _bannerURL;
		/// <summary>ストアのURL</summary>
		public string _url;
		/// <summary>バナー画像</summary>
		public Sprite _banner;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Set( string fileName, int index, List<string> texts ) {
			_platform	= texts[0].ToEnum<Platform>();
			_title		= texts[1];
			_info		= texts[2];
			_bannerURL	= texts[3];
			_url		= texts[4];
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Load() {
			await base.Load();

			// バナー画像読込
			_banner = await FileManager.s_instance._fileLoader.LoadServer<Sprite>( _bannerURL );
		}
	}
}