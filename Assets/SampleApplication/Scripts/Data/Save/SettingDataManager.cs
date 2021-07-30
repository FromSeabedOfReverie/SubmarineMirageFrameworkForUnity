//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
using Cysharp.Threading.Tasks;
using UniRx;
using SubmarineMirage.Service;
using SubmarineMirage.File;
using SubmarineMirage.Data;
using SubmarineMirage.Setting;
using SubmarineMirage.Debug;
///========================================================================================================
/// <summary>
/// ■ 設定情報の管理クラス
/// </summary>
///========================================================================================================
public class SettingDataManager : BaseSMDataManager<int, SettingData> {
	///----------------------------------------------------------------------------------------------------
	/// ● 要素
	///----------------------------------------------------------------------------------------------------
	const int REGISTER_INDEX = 0;

	///----------------------------------------------------------------------------------------------------
	/// ● 作成、削除
	///----------------------------------------------------------------------------------------------------
	/// <summary>
	/// ● コンストラクタ（読込用）
	/// </summary>
	public SettingDataManager() {
		var loader = SMServiceLocator.Resolve<SMFileManager>().Get<SMCryptoLoader>();


		_loadEvent.AddFirst( async canceler => {
			var data = await loader.Load<SettingData>( SMMainSetting.SETTING_FILE_NAME );
			if ( data == null )	{ data = new SettingData(); }

			// 保存版とアプリ版が異なる場合、更新
			if ( data._version != SMMainSetting.APPLICATION_VERSION ) {
// TODO : 本当は、更新対応した方が良いが、省略
				data.Dispose();
				data = new SettingData();
			}

			Register( REGISTER_INDEX, data );
		} );


		_saveEvent.AddLast( async canceler => {
			await loader.Save( SMMainSetting.SETTING_FILE_NAME, Get() );
		} );


		if ( !SMDebugManager.IS_DEVELOP )	{ return; }

		// デバッグキー押下の場合、画面ログ表示を切り替え
		var allDataManager = SMServiceLocator.Resolve<SMAllDataManager>();
		var inputManager = SMServiceLocator.Resolve<SMInputManager>();
		var displayLog = SMServiceLocator.Resolve<SMDisplayLog>();

		inputManager.GetKey( SMInputKey.Debug )._enabledEvent.AddLast().Subscribe( _ => {
			var data = Get();
			// 設定を保存
			data._isViewDebug = !data._isViewDebug;
			_saveEvent.Run( allDataManager._asyncCancelerOnDispose ).Forget();
			// 描画切り替え
			displayLog._isDraw = data._isViewDebug;
		} );
	}

	///----------------------------------------------------------------------------------------------------
	/// ● 取得
	///----------------------------------------------------------------------------------------------------
	/// <summary>
	/// ● 取得
	/// </summary>
	public SettingData Get()
		=> Get( REGISTER_INDEX );
}