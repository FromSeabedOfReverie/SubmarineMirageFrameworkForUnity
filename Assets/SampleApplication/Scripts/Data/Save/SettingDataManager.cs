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
		var loader = SMServiceLocator.Resolve<SMFileManager>()._cryptoLoader;


		_loadEvent.AddFirst( async canceler => {
			// 展示版の場合、新規情報のまま、未読込
			if ( SettingData.GetEdition() == SMEdition.Exhibition ) {
				Register( REGISTER_INDEX, new SettingData() );
				return;
			}

			// 評価版、製品版の場合
			// 読込成功で、バージョンが等しい場合、登録
			var newData = await loader.Load<SettingData>( SMMainSetting.SETTING_FILE_NAME );
			if ( newData != null ) {
				if ( newData._version == SMMainSetting.APPLICATION_VERSION ) {
					Register( REGISTER_INDEX, newData );
				} else {
					newData.Dispose();
				}
			}
			if ( Get() == null ) {
				Register( REGISTER_INDEX, new SettingData() );
			}
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