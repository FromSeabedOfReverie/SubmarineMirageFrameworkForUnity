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


	public class FinalDisableSMObject : SMObjectModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.FirstRunner;


		public FinalDisableSMObject( SMTaskRunAllType runType ) : base( runType ) {}

		public override void Set( SMObject owner ) {
			base.Set( owner );
			_owner._isFinalizing = true;
		}


		public override async UniTask Run() {
			if ( _owner._ranState >= SMTaskRunState.FinalDisable )	{ return; }


			_owner._activeState = SMTaskActiveState.Disable;
			_owner._isDisabling = true;

			var isActiveInHierarchy = SMObjectApplyer.IsActiveInHierarchy( _owner );
			await RunLower( _runType, b => new FinalDisableSMBehaviour( isActiveInHierarchy ) );

			if ( _runType == SMTaskRunAllType.ReverseSequential ) {
				_owner._isDisabling = false;
				_owner._ranState = SMTaskRunState.FinalDisable;
			}
		}
	}
}