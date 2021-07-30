//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using SubmarineMirage.File;
using SubmarineMirage.Data;
using SubmarineMirage.Setting;



public class SMDataSetting : BaseSMDataSetting {
	public override void Setup() {
		base.Setup();

		_datas = new Dictionary< SMDataSettingType, List<IBaseSMDataManager> > {
			{
				SMDataSettingType.Save,
				new List<IBaseSMDataManager> {
					new SettingDataManager(),
					new PlayDataManager(),
				}
			}, {
				SMDataSettingType.Server,
				new List<IBaseSMDataManager> {
					new CMDataManager(),
					new SMCSVDataManager<int, ApplicationCMData>(
						SMMainSetting.APPLICATION_CM_DATA_PATH, "", SMFileLocation.Server, 1 ),
				}
			}, {
				SMDataSettingType.Master,
				new List<IBaseSMDataManager> {
//					new AllWordDataManager(),
//					new AllAIDataManager(),
					new SMCSVDataManager<string, ItemData>( "Item", "TestItem", SMFileLocation.Resource, 1 ),
				}
			},
		};
	}
}