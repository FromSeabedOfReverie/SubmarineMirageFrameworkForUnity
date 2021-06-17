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


	public class EnableSMTask : SMTaskModifyData {
		[SMShowLine] public override SMModifyType _type => GetType( _task );
		[SMShowLine] SMTask _task;


		public EnableSMTask( SMTask task ) {
			_task = task;
		}


		public override async UniTask Run() {
			if ( !_task._isCanActiveEvent() )	{ return; }
			if ( _task._isFinalizing )			{ return; }
			if ( !_task._isInitialized ) {
				_task._isRequestInitialEnable = true;
				return;
			}
			if ( _task._activeState == SMTaskActiveState.Enable )	{ return; }

			_task._asyncCancelerOnDisable.Recreate();
			_task._enableEvent.Run();
			_task._activeState = SMTaskActiveState.Enable;

			await UTask.DontWait();
		}
	}
}