//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using Task.Base;
	using Task.Modifyler.Base;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class FinalDisableSMBehaviour : SMBehaviourModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.FirstRunner;
		[SMShowLine] bool _isActiveInHierarchy	{ get; set; }


		public FinalDisableSMBehaviour( bool isActiveInHierarchy )
			=> _isActiveInHierarchy = isActiveInHierarchy;

		public override void Set( SMBehaviourBody owner ) {
			base.Set( owner );
			_target._isFinalizing = true;
		}


		public override async UniTask Run() {
			if ( _target._ranState >= SMTaskRunState.FinalDisable )	{ return; }


			var lastActiveState = _target._activeState;
			_target._activeState = SMTaskActiveState.Disable;
			_target.StopAsyncOnDisable();

			if (	_isActiveInHierarchy && _target.IsActiveInComponent() &&
					lastActiveState != SMTaskActiveState.Disable
			) {
				_target._disableEvent.Run();
			}

			_target._isRunFinalize = _target._ranState >= SMTaskRunState.SelfInitialize;
			_target._ranState = SMTaskRunState.FinalDisable;

			await UTask.DontWait();
		}
	}
}