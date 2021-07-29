//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
using System.Linq;
using Cysharp.Threading.Tasks;
using SubmarineMirage.Service;
using SubmarineMirage.File;
using SubmarineMirage.Data;
using SubmarineMirage.Extension;
using SubmarineMirage.Setting;
using SubmarineMirage.Debug;
///========================================================================================================
/// <summary>
/// ■ 遊戯情報の管理クラス
///		まとめてファイル保存すると、1つのセーブデータだけ変更なのに、全体を保存し直す事になる。
///		それを避ける為、セーブデータごとに、複数ファイルを保存する。
/// </summary>
///========================================================================================================
public class PlayDataManager : BaseSMDataManager<int, PlayData> {
	///----------------------------------------------------------------------------------------------------
	/// ● 要素
	///----------------------------------------------------------------------------------------------------
	/// <summary>現在のセーブ情報</summary>
	public PlayData _currentData	{ get; private set; }

	/// <summary>セーブ情報一覧での、使用中セーブ情報の添字</summary>
	public int _index {
		get => _setting.Get()._playDataIndex;
		private set => _setting.Get()._playDataIndex = value;
	}

	/// <summary>暗号化書類の読み書き</summary>
	SMCryptoLoader _loader	{ get; set; }
	/// <summary>全情報</summary>
	SMAllDataManager _allDataManager { get; set; }
	/// <summary>設定情報</summary>
	SettingDataManager _setting { get; set; }

	///----------------------------------------------------------------------------------------------------
	/// ● 作成、削除
	///----------------------------------------------------------------------------------------------------
	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	public PlayDataManager() {
		_loader = SMServiceLocator.Resolve<SMFileManager>()._cryptoLoader;
		_allDataManager = SMServiceLocator.Resolve<SMAllDataManager>();
		_setting = _allDataManager.Get<SettingDataManager>();


		_loadEvent.AddFirst( async canceler => {
			// バージョンアップする必要があるか、判定
			var isUpdate = _setting.Get()._version != SMMainSetting.APPLICATION_VERSION;

			// 全情報を非同期読込
			await Enumerable
				.Range( 0, SMMainSetting.MAX_PLAY_DATA_COUNT - 1 )
				.Select( async i => {
					var name = string.Format( SMMainSetting.PLAY_FILE_NAME_FORMAT, i );
					var data = await _loader.Load<PlayData>( name );

					// 存在しない場合、新規作成
					if ( data == null ) {
						Register( i, new PlayData() );
						SMLog.Debug( "データが存在しない為、初期化生成", SMLogTag.Data );
						return;
					}

					// 更新対応
					if ( isUpdate ) {
// TODO : 本当は、更新対応した方が良いが、省略
						data.Dispose();
						Register( i, new PlayData() );
						SMLog.Debug( "データが古い為、初期化生成", SMLogTag.Data );
						return;
					}

					// 読み込み成功
					Register( i, data );
					SMLog.Debug( "読み込み成功", SMLogTag.Data );
				} );
		} );

		_loadEvent.AddLast( async canceler => {
			// 現在情報を読込
			await LoadCurrentData();
		} );


		_disposables.AddLast( () => {
			_currentData?.Dispose();

			_loader = null;
			_allDataManager = null;
			_setting = null;
		} );
	}

	/// <summary>
	/// ● 現在情報をリセット
	/// </summary>
	public void ResetCurrentData() {
		_currentData?.Dispose();
		_currentData = new PlayData();
	}

	///----------------------------------------------------------------------------------------------------
	/// ● 読み書き
	///----------------------------------------------------------------------------------------------------
	/// <summary>
	/// ● 現在情報を読込
	/// </summary>
	public async UniTask LoadCurrentData( int? index = null ) {
		if ( index.HasValue ) {
			_index = index.Value;
			await _setting._saveEvent.Run( _allDataManager._asyncCancelerOnDispose );
		}
		_currentData = _datas[_index].DeepCopy();
		await _currentData.Load();  // 複製後でも、テクスチャは参照で複製されない為、再読込
	}

	/// <summary>
	/// ● 現在情報を保存
	/// </summary>
	public async UniTask SaveCurrentData( int? index = null ) {
		if ( index.HasValue ) {
			_index = index.Value;
			await _setting._saveEvent.Run( _allDataManager._asyncCancelerOnDispose );
		}

		await _currentData.Save();
		var name = string.Format( SMMainSetting.PLAY_FILE_NAME_FORMAT, _index );
		await _loader.Save( name, _currentData );
		_datas[_index] = _currentData.DeepCopy();

// TODO : バックアップを作成し、読込失敗の場合、バックアップを読むようにする
	}
}