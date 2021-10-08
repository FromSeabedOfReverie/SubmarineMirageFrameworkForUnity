//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Scene {
	using Base;
	using FSM;
	using Service;



	public abstract class BaseSMSceneSetting : SMStandardBase, ISMService {
		public SMFSMGenerateList _datas	{ get; protected set; } = new SMFSMGenerateList();



		public BaseSMSceneSetting() {
			_disposables.AddFirst( () => {
				_datas.Dispose();
			} );
		}

		public abstract void Setup();
	}
}