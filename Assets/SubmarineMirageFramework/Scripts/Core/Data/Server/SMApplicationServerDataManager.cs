//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.Server {
	using Service;
	using File;
	using Utility;
	using Setting;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ アプリケーションのサーバー情報の管理クラス
	/// </summary>
	///====================================================================================================
	public class SMApplicationServerDataManager : SMCSVDataManager<SMEdition, SMApplicationServerData> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		SMMainSetting _mainSetting	{ get; set; }
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public SMApplicationServerDataManager()
			: base( SMMainSetting.APPLICATION_DATA_PATH, "", SMFileLocation.Server, 1 )
		{
			_mainSetting = SMServiceLocator.Resolve<SMMainSetting>();

			_loadEvent.AddLast( async canceler => {
				// 中央設定の版を設定
				var data = Get();
				_mainSetting._editionByServer = data._edition;
				_mainSetting._versionByServer = data._version;
				_mainSetting._serverVersionByServer = data._serverVersion;

//				SMLog.Debug( this );

				await UTask.DontWait();
			} );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 取得
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public SMApplicationServerData Get()
			=> Get( _mainSetting._editionBySave );
	}
}