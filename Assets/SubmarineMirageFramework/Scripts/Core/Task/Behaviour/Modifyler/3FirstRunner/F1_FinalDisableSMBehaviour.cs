//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestBehaviourModifyler
namespace SubmarineMirage.Task.Behaviour.Modifyler {
	using Cysharp.Threading.Tasks;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class FinalDisableSMBehaviour : SMBehaviourModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.FirstRunner;
		bool _isActiveInHierarchy	{ get; set; }


		public FinalDisableSMBehaviour( bool isActiveInHierarchy )
			=> _isActiveInHierarchy = isActiveInHierarchy;

		public override void Set( SMBehaviourBody owner ) {
			base.Set( owner );
			_owner._isFinalizing = true;
		}


		public override async UniTask Run() {
			if ( _owner._ranState >= SMTaskRunState.FinalDisable )	{ return; }


			if ( _isActiveInHierarchy && _owner._activeState != SMTaskActiveState.Disable ) {
				_owner._activeState = SMTaskActiveState.Disable;
				_owner.StopAsyncOnDisable();
				_owner._disableEvent.Run();
			}

			_owner._isRunFinalize = _owner._ranState >= SMTaskRunState.SelfInitialize;
			_owner._ranState = SMTaskRunState.FinalDisable;

			await UTask.DontWait();
		}
	}
}