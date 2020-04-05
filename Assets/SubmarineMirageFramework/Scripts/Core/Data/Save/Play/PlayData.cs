//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.Save {
	using System;
	using System.IO;
	using System.Linq;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using UnityEngine;
	using UniRx.Async;
	using KoganeUnityLib;
	using File;
	using Extension;
	///====================================================================================================
	/// <summary>
	/// ■ 遊戯情報のクラス
	///----------------------------------------------------------------------------------------------------
	/// </summary>
	///====================================================================================================
	public class PlayData : SaveData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>セーブ情報の保存階層</summary>
		static readonly string SUB_PATH = Path.Combine( CryptoLoader.SUB_PATH, "Pictures" );
		/// <summary>スクリーンショット画像の保存書類名</summary>
		const string PICTURE_NAME_FORMAT = "Picture{0}.jpg";

		/// <summary>一度でも保存したか？</summary>
		public bool _isSave;
		/// <summary>日付</summary>
		public DateTime _date;
		/// <summary>プレイヤーの状態</summary>
//		public PlayerStatus _playerStatus;
		/// <summary>実行後の、操作説明名一覧</summary>
		public List<string> _afterHelpNames = new List<string>();
		/// <summary>実行後の、イベント名一覧</summary>
		public List<string> _afterEventNames = new List<string>();
		/// <summary>実行後の、地形名一覧</summary>
		public List<string> _afterFieldNames = new List<string>();
		/// <summary>スクリーンショット画像の、生情報一覧</summary>
		public List<TextureRawData> _pictureRawData = new List<TextureRawData>();
		/// <summary>スクリーンショット画像の、情報一覧</summary>
		// Sprite型は、そのまま保存できない
		[IgnoreDataMember] public List<Sprite> _pictures = new List<Sprite>();

		/// <summary>書類読み書き</summary>
		FileLoader _loader	=> FileManager.s_instance._fileLoader;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 操作説明後か？
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public bool IsAfterHelp( string name ) {
			return _afterHelpNames.Contains( name );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 操作説明イベントを登録
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void RegisterAfterHelp( string name ) {
			if ( !_afterHelpNames.Contains( name ) ) {
				_afterHelpNames.Add( name );
			}
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 写真を登録
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void RegisterPicture( Sprite sprite ) {
			_pictures.Add( sprite );
			var data = sprite.ToRawData( TextureRawData.Type.JPG, 90 );	// JPG形式で変換
			_pictureRawData.Add( data );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Load() {
			// 写真生情報から、スプライトを読込
			_pictures = _pictureRawData
				.Select( data => data.ToSprite() )
				.ToList();

			await base.Load();
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 保存
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override async UniTask Save() {
			// フォルダ内の、元々ある写真を削除
			var folder = Path.Combine( FileManager.SAVE_EXTERNAL_PATH, SUB_PATH );
			FileManager.s_instance.DeletePath( folder );

			// 写真をJPG形式で保存
			var tasks = _pictureRawData
				.Select( ( data, i ) => {
					var path = Path.Combine( SUB_PATH, string.Format( PICTURE_NAME_FORMAT, i + 1 ) );
					return _loader.SaveExternal( path, data._data );
				} );
			// 非同期、保存待機
			await UniTask.WhenAll( tasks );

			await base.Save();

			_isSave = true;
		}
	}
}