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


	public class InitialEnableSMTask : SMTaskModifyData {
		[SMShowLine] public override SMModifyType _type => GetType( _task );


		public InitialEnableSMTask( SMTask task ) : base( task ) {}


		public override async UniTask Run() {
			if ( _task._ranState != SMTaskRunState.Initialize )	{ return; }


			if ( _task._isRequestInitialEnable ) {
				_task._isRequestInitialEnable = false;

				if (	_task._isCanActiveEvent() &&
						_task._activeState != SMTaskActiveState.Enable
				) {
					_task._asyncCancelerOnDisable.Recreate();
					_task._enableEvent.Run();
					_task._activeState = SMTaskActiveState.Enable;
				}
			}

			_task._ranState = SMTaskRunState.InitialEnable;

			await UTask.DontWait();
		}
	}
}