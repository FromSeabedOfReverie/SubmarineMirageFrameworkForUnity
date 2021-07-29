//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using Cysharp.Threading.Tasks;
using SubmarineMirage.Service;
using SubmarineMirage.File;
using SubmarineMirage.Data.Raw;
using SubmarineMirage.Data.Save;
using SubmarineMirage.Extension;
using SubmarineMirage.Setting;
///========================================================================================================
/// <summary>
/// ■ 遊戯情報のクラス
/// </summary>
///========================================================================================================
public class PlayData : BaseSMSaveData {
	///----------------------------------------------------------------------------------------------------
	/// ● 要素
	///----------------------------------------------------------------------------------------------------
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
	public List<SMTextureRawData> _pictureRawData = new List<SMTextureRawData>();
	/// <summary>スクリーンショット画像の、情報一覧</summary>
	// Sprite型は、そのまま保存できない
	[IgnoreDataMember] public List<Sprite> _pictures = new List<Sprite>();

	///----------------------------------------------------------------------------------------------------
	/// ● 作成、削除
	///----------------------------------------------------------------------------------------------------
	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	public PlayData() {
	}

	/// <summary>
	/// ● 解放（補助）
	/// </summary>
	protected override void DisposeSub() {
		base.DisposeSub();

		_afterHelpNames.Clear();
		_afterEventNames.Clear();
		_afterFieldNames.Clear();
		ResetRawData();
		_pictures.Clear();
	}

	/// <summary>
	/// ● 生情報をリセット
	/// </summary>
	void ResetRawData() {
		_pictureRawData.ForEach( d => d.Dispose() );
		_pictureRawData.Clear();
	}

	///----------------------------------------------------------------------------------------------------
	/// ● 読み書き
	///----------------------------------------------------------------------------------------------------
	/// <summary>
	/// ● 読込
	/// </summary>
	public override async UniTask Load() {
		// 写真生情報から、スプライトを読込
		_pictures = _pictureRawData
			.Select( data => data.ToSprite() )
			.ToList();
		ResetRawData();

		await base.Load();
	}

	/// <summary>
	/// ● 保存
	/// </summary>
	public override async UniTask Save() {
		var fileManager = SMServiceLocator.Resolve<SMFileManager>();
		var loader = fileManager._fileLoader;

		// フォルダ内の、元々ある写真を削除
		var path = Path.Combine( SMMainSetting.SAVE_EXTERNAL_PATH, SMMainSetting.PICTURE_FILE_PATH );
		fileManager.DeletePath( path );

		// 生情報に変換
		ResetRawData();
		_pictureRawData = _pictures
			.Select( data => data.ToRawData( SMTextureRawData.Type.JPG, 90 ) )	// JPG形式で変換
			.ToList();

		// 写真をJPG形式で非同期保存
		await _pictureRawData.Select( async ( data, i ) => {
			var path = Path.Combine(
				SMMainSetting.PICTURE_FILE_PATH,
				string.Format( SMMainSetting.PICTURE_FILE_NAME_FORMAT, i + 1 )
			);
			await loader.SaveExternal( path, data._data );
		} );

		_isSave = true;

		await base.Save();
	}

	///----------------------------------------------------------------------------------------------------
	/// ● 取得
	///----------------------------------------------------------------------------------------------------
	/// <summary>
	/// ● 操作説明後か？
	/// </summary>
	public bool IsAfterHelp( string name )
		=> _afterHelpNames.Contains( name );

	///----------------------------------------------------------------------------------------------------
	/// ● 登録、解除
	///----------------------------------------------------------------------------------------------------
	/// <summary>
	/// ● 操作説明イベントを登録
	/// </summary>
	public void RegisterAfterHelp( string name ) {
		if ( _afterHelpNames.Contains( name ) ) { return; }

		_afterHelpNames.Add( name );
	}

	/// <summary>
	/// ● 写真を登録
	/// </summary>
	public void RegisterPicture( Sprite sprite )
		=> _pictures.Add( sprite );
}