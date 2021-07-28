//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using SubmarineMirage.Base;
using SubmarineMirage.File;
using SubmarineMirage.Data;



public class SMDataSetting : SMStandardBase, ISMDataSetting {
	public List<IBaseSMDataManager> _datas	{ get; private set; }



	public SMDataSetting() {
		_datas = new List<IBaseSMDataManager> {
			new SettingDataManager(),
			new PlayDataManager(),

//			new AllWordDataManager(),
//			new AllAIDataManager(),

			new SMCSVDataManager<string, ItemData>( "Item", "TestItem", SMFileLocation.Resource, 1 ),
		};


		_disposables.AddLast( () => {
			_datas.ForEach( d => d.Dispose() );
			_datas.Clear();
		} );
	}
}