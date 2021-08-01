//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
using SubmarineMirage.File;
using SubmarineMirage.Data;
using SubmarineMirage.Utility;
using SubmarineMirage.Setting;
using SubmarineMirage.Debug;
///========================================================================================================
/// <summary>
/// ■ 製品版の宣伝情報の管理クラス
/// </summary>
///========================================================================================================
public class CMDataManager : SMCSVDataManager<SMPlatformType, CMData> {
	///----------------------------------------------------------------------------------------------------
	/// ● 要素
	///----------------------------------------------------------------------------------------------------

	///----------------------------------------------------------------------------------------------------
	/// ● 作成、削除
	///----------------------------------------------------------------------------------------------------
	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	public CMDataManager() : base( SMMainSetting.CM_DATA_PATH, "", SMFileLocation.Server, 1 ) {
/*
		_loadEvent.AddLast( async canceler => {
			SMLog.Debug( this );
			await UTask.DontWait();
		} );
*/
	}

	///----------------------------------------------------------------------------------------------------
	/// ● 取得
	///----------------------------------------------------------------------------------------------------
	/// <summary>
	/// ● 取得
	///		起動中の端末に適した情報を取得。
	/// </summary>
	public CMData Get()
		=> Get( SMMainSetting.PLATFORM );
}