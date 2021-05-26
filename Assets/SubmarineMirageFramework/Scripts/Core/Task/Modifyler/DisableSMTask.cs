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


	public class DisableSMTask : SMTaskModifyData {
		[SMShowLine] public override SMModifyType _type => GetType( _task );
		[SMShowLine] SMTask _task;


		public DisableSMTask( SMTask task ) {
			_task = task;
		}


		public override async UniTask Run() {
			if ( !_task._isCanActiveEvent() )	{ return; }
			if ( _task._isFinalizing )			{ return; }
			if ( !_task._isInitialized ) {
				_task._isRequestInitialEnable = false;
				return;
			}
			if ( _task._activeState == SMTaskActiveState.Disable )	{ return; }

			_task._activeState = SMTaskActiveState.Disable;
			_task._asyncCancelerOnDisable.Cancel();
			_task._disableEvent.Run();

			await UTask.DontWait();
		}
	}
}