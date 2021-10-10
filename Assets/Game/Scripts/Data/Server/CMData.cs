//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace Game {
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using SubmarineMirage;
	using SubmarineMirage.Service;
	using SubmarineMirage.File;
	using SubmarineMirage.Data;
	using SubmarineMirage.Setting;
	///====================================================================================================
	/// <summary>
	/// ■ 製品版の宣伝情報クラス
	/// </summary>
	///====================================================================================================
	public class CMData : SMCSVData<SMPlatformType> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>辞書への登録鍵</summary>
		public override SMPlatformType _registerKey => _platform;

		/// <summary>アプリ実行機種</summary>
		[SMShow] public SMPlatformType _platform	{ get; private set; }
		/// <summary>表題名</summary>
		[SMShow] public string _title				{ get; private set; }
		/// <summary>説明文</summary>
		[SMShow] public string _info				{ get; private set; }
		/// <summary>バナー画像のURL</summary>
		[SMShow] public string _bannerURL			{ get; private set; }
		/// <summary>ストアのURL</summary>
		[SMShow] public string _url					{ get; private set; }
		/// <summary>バナー画像</summary>
		public Sprite _banner						{ get; private set; }

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		public override void Setup( string fileName, int index, List<string> texts ) {
			_platform	= texts[0].ToEnum<SMPlatformType>();
			_title		= texts[1];
			_info		= texts[2];
			_bannerURL	= texts[3];
			_url		= texts[4];
		}

		///------------------------------------------------------------------------------------------------
		/// ● 読み書き
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		public override async UniTask Load() {
			// バナー画像読込
			var loader = SMServiceLocator.Resolve<SMFileManager>().Get<SMFileLoader>();
			_banner = await loader.LoadServer<Sprite>( _bannerURL );

			await base.Load();
		}
	}
}