//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Utility;
	using Debug;


	public class FinalDisableSMTask : SMTaskModifyData {
		[SMShowLine] public override SMModifyType _type => GetType( _task );
		[SMShowLine] SMTask _task;


		public FinalDisableSMTask( SMTask task ) {
			_task = task;
		}


		public override async UniTask Run() {
			if ( _task._ranState >= SMTaskRunState.FinalDisable )	{ return; }


			var lastActiveState = _task._activeState;
			_task._activeState = SMTaskActiveState.Disable;
			_task._asyncCancelerOnDisable.Cancel();

			if (	_task._isCanActiveEvent() &&
					lastActiveState != SMTaskActiveState.Disable
			) {
				_task._disableEvent.Run();
			}

			_task._ranState = SMTaskRunState.FinalDisable;

			await UTask.DontWait();
		}
	}
}