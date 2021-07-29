//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
using System.Linq;
using System.Collections.Generic;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.File;
using SubmarineMirage.Data;
using SubmarineMirage.Setting;



public class SMDataSetting : SMStandardBase, ISMDataSetting {
	SMAllDataManager _allDataManager	{ get; set; }
	public Dictionary< SMDataSettingType, List<IBaseSMDataManager> > _datas	{ get; private set; }



	public SMDataSetting() {
		_allDataManager = SMServiceLocator.Resolve<SMAllDataManager>();

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

		_disposables.AddLast( () => {
			_datas
				.SelectMany( pair => pair.Value )
				.ForEach( d => d.Dispose() );
			_datas.Clear();

			_allDataManager = null;
		} );
	}



	public void RegisterDatas( SMDataSettingType type ) {
		_datas[type].ForEach( d => _allDataManager.Register( d ) );
		// Dispose前に、参照を切る
		_datas[type].Clear();
		_datas.Remove( type );
	}
}