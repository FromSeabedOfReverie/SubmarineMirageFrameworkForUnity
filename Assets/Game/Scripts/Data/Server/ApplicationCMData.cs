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
	using SubmarineMirage.Data.Server;
	///====================================================================================================
	/// <summary>
	/// ■ 他アプリケーションの宣伝情報クラス
	/// </summary>
	///====================================================================================================
	public class ApplicationCMData : SMURLData<int> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>辞書への登録鍵</summary>
		public override int _registerKey => _cmID;
		/// <summary>URL読込の初期添字</summary>
		protected override int _setURLStartIndex => 5;

		/// <summary>番号</summary>
		[SMShow] public int _cmID		{ get; private set; }
		/// <summary>表題名</summary>
		[SMShow] public string _title	{ get; private set; }
		/// <summary>種類</summary>
		[SMShow] public string _genre	{ get; private set; }
		/// <summary>説明文</summary>
		[SMShow] public string _info	{ get; private set; }
		/// <summary>新規情報か？</summary>
		[SMShow] public bool _isNew		{ get; private set; }
		/// <summary>アイコン画像のURL</summary>
		[SMShow] public string _iconURL	{ get; private set; }
		/// <summary>アイコン画像</summary>
		public Sprite _icon				{ get; private set; }

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		public override void Setup( string fileName, int index, List<string> texts ) {
			_cmID		= index;
			_title		= texts[0];
			_genre		= texts[1];
			_info		= texts[2];
			_isNew		= texts[3].ToBoolean();
			_iconURL	= texts[4];

			base.Setup( fileName, index, texts );
		}

		///------------------------------------------------------------------------------------------------
		/// ● 読み書き
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		public override async UniTask Load() {
			// アイコン画像読込
			var loader = SMServiceLocator.Resolve<SMFileManager>().Get<SMFileLoader>();
			_icon = await loader.LoadServer<Sprite>( _iconURL );

			await base.Load();
		}
	}
}