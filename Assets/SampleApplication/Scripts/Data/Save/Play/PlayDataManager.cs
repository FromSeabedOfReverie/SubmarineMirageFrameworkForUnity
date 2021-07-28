//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.Save {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using File;
	using Extension;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 遊戯情報の管理クラス
	///----------------------------------------------------------------------------------------------------
	///		まとめてファイル保存すると、1つのセーブデータだけ変更なのに、全体を保存し直す事になる。
	///		それを避ける為、セーブデータごとに、複数ファイルを保存する。
	/// </summary>
	///====================================================================================================
	public class PlayDataManager : BaseSMSaveData {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>書類保存名の書式</summary>
		const string FILE_NAME_FORMAT = "PlayData{0}.data";
		/// <summary>セーブ可能、書類数</summary>
		const int MAX_DATA_COUNT = 1;

		/// <summary>全セーブ情報の一覧</summary>
		readonly List<PlayData> _allData = new List<PlayData>();
		/// <summary>現在のセーブ情報</summary>
		public PlayData _currentData	{ get; private set; } = new PlayData();

		/// <summary>セーブ情報一覧での、使用中セーブ情報の添字</summary>
		public int _index {
			get { return _setting._data._playDataIndex; }
			private set { _setting._data._playDataIndex = value; }
		}
		/// <summary>設定情報</summary>
		SettingDataManager _setting	=> SMAllDataManager.s_instance._save._setting;
		/// <summary>暗号化書類の読み書き</summary>
		SMCryptoLoader _loader		=> SMFileManager.s_instance._cryptoLoader;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public PlayDataManager() {
			MAX_DATA_COUNT.Times( () => _allData.Add( new PlayData() ) );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 再設定（現在情報）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void ResetCurrentData() {
			_currentData = new PlayData();
		}
		///------------------------------------------------------------------------------------------------
		/// ● 読込
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		public override async UniTask Load() {
			// バージョンアップする必要があるか、判定
			var isUpdate = _setting._data._version != Application.version;

			// 全情報を非同期読込
			await _allData.Select( async ( data, i ) => {
				var name = string.Format( FILE_NAME_FORMAT, i );
				data = await _loader.Load<PlayData>( name );

				// 存在しない場合、新規作成
				if ( data == null ) {
					data = new PlayData();
//					Log.Debug( "データが存在しない為、初期化生成", Log.Tag.Data );

				// 更新対応
				} else if ( isUpdate ) {
					// TODO : 本当は、更新対応した方が良いが、省略
					data = new PlayData();
//					Log.Debug( "データが古い為、初期化生成", Log.Tag.Data );

				// 読み込み成功
				} else {
//					Log.Debug( "読み込み成功", Log.Tag.Data );
				}

				await data.Load();
				_allData[i] = data;
			} );
			// 現在情報を読込
			await LoadCurrentData();

			await base.Load();
		}
		/// <summary>
		/// ● 読込（現在情報）
		/// </summary>
		public async UniTask LoadCurrentData( int? index = null ) {
			if ( index.HasValue ) {
				_index = index.Value;
				await _setting.Save();
			}
			_currentData = _allData[_index].DeepCopy();
			await _currentData.Load();  // 複製後でも、テクスチャは参照で複製されない為、再読込
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 保存（現在情報）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public async UniTask SaveCurrentData( int? index = null ) {
			if ( index.HasValue ) {
				_index = index.Value;
				await _setting.Save();
			}

			await _currentData.Save();
			var name = string.Format( FILE_NAME_FORMAT, _index );
			await _loader.Save( name, _currentData );
			_allData[_index] = _currentData.DeepCopy();

			// TODO : バックアップを作成し、読込失敗の場合、バックアップを読むようにする
		}
	}
}