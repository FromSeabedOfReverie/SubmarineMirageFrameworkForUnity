//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace Game {
	using System.Collections.Generic;
	using SubmarineMirageFramework;



	/// <summary>
	/// ■ 登録データの設定クラス
	/// </summary>
	public class SMDataSetting : BaseSMDataSetting {

		/// <summary>
		/// ● 設定
		/// </summary>
		public override void Setup() {
			base.Setup();

			_datas = new Dictionary< SMDataSettingType, List<IBaseSMDataManager> > {
				{
					// セーブデータを登録
					SMDataSettingType.Save,
					new List<IBaseSMDataManager> {
						// 設定データ
						new SettingDataManager(),
						// プレイデータ
						new PlayDataManager(),
					}
				}, {
					// サーバーデータを登録
					SMDataSettingType.Server,
					new List<IBaseSMDataManager> {
						new CMDataManager(),
						new SMCSVDataManager<int, ApplicationCMData>(
							SMMainSetting.APPLICATION_CM_DATA_PATH, "", SMFileLocation.Server, 1 ),
					}
				}, {
					// マスターデータを登録
					SMDataSettingType.Master,
					new List<IBaseSMDataManager> {
						// アイテムデータ
						new SMCSVDataManager<string, ItemData>( "Item", "TestItem", SMFileLocation.Resource, 1 ),
					}
				},
			};
		}
	}
}