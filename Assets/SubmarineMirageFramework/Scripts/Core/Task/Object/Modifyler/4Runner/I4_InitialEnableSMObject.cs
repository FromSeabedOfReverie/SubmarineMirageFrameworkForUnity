//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Object.Modifyler {
	using Cysharp.Threading.Tasks;
	using Behaviour.Modifyler;
	using Debug;


	// TODO : コメント追加、整頓


	public class InitialEnableSMObject : SMObjectModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Runner;


		public InitialEnableSMObject( SMTaskRunAllType runType ) : base( runType ) {}


		public override async UniTask Run() {
			if ( _owner._ranState != SMTaskRunState.Initialize )	{ return; }


			var isActiveInHierarchy = SMObjectApplyer.IsActiveInHierarchy( _owner );
			await RunLower( _runType, b => new InitialEnableSMBehaviour( isActiveInHierarchy ) );

			if ( _runType == SMTaskRunAllType.Parallel ) {
				_owner._activeState = SMTaskActiveState.Enable;
				_owner._ranState = SMTaskRunState.InitialEnable;
			}
		}
	}
}