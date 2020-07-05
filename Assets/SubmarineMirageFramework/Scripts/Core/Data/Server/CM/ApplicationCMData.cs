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
	/// ■ 他アプリケーションの宣伝情報クラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class ApplicationCMData : URLData<int> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>辞書への登録鍵</summary>
		public override int _registerKey => _id;
		/// <summary>URL読込の初期添字</summary>
		protected override int _setURLStartIndex => 5;

		/// <summary>番号</summary>
		public int _id;
		/// <summary>表題名</summary>
		public string _title;
		/// <summary>種類</summary>
		public string _genre;
		/// <summary>説明文</summary>
		public string _info;
		/// <summary>新規情報か？</summary>
		public bool _isNew;
		/// <summary>アイコン画像のURL</summary>
		public string _iconURL;
		/// <summary>アイコン画像</summary>
		public Sprite _icon;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void Set( string fileName, int index, List<string> texts ) {
			_id			= index;
			_title		= texts[0];
			_genre		= texts[1];
			_info		= texts[2];
			_isNew		= texts[3].ToBoolean();
			_iconURL	= texts[4];

			base.Set( fileName, index, texts );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Load() {
			await base.Load();

			// アイコン画像読込
			_icon = await FileManager.s_instance._fileLoader.LoadServer<Sprite>( _iconURL );
		}
	}
}