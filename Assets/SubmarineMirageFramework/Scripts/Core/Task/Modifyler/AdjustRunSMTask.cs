//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task.Modifyler {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Modifyler;
	using Debug;


	public class AdjustRunSMTask : SMTaskModifyData {
		[SMShowLine] public override SMModifyType _type => SMModifyType.Normal;
		[SMShowLine] SMTask _previous;
		[SMShowLine] bool _isSetPrevious;


		public AdjustRunSMTask( SMTask task, SMTask previous = null ) : base( task ) {
			_previous = previous;
			_isSetPrevious = _previous == null;
		}


		public override async UniTask Run() {
			if ( _isSetPrevious ) {
				_previous = _task._previous;
			}


			if (	_previous._ranState >= SMTaskRunState.FinalDisable &&
					_task._ranState < SMTaskRunState.FinalDisable &&
					_task._type != SMTaskRunType.Dont
			) {
				await new FinalDisableSMTask( _task ).Run();
			}
			if (	_previous._ranState >= SMTaskRunState.Finalize &&
					_task._ranState < SMTaskRunState.Finalize
			) {
				if ( _task._type != SMTaskRunType.Dont ) {
					await new FinalizeSMTask( _task ).Run();
				} else {
					Dispose();
				}
			}
			if ( _previous._isFinalizing ) { return; }


			if (	_previous._ranState >= SMTaskRunState.Create &&
					_task._ranState < SMTaskRunState.Create
			) {
				await new CreateSMTask( _task ).Run();
			}
			if ( _task._type == SMTaskRunType.Dont ) { return; }


			if (	_previous._ranState >= SMTaskRunState.SelfInitialize &&
					_task._ranState < SMTaskRunState.SelfInitialize
			) {
				await new SelfInitializeSMTask( _task ).Run();
			}
			if (	_previous._ranState >= SMTaskRunState.Initialize &&
					_task._ranState < SMTaskRunState.Initialize
			) {
				await new InitializeSMTask( _task ).Run();
			}
			if (	_previous._ranState >= SMTaskRunState.InitialEnable &&
					_task._ranState < SMTaskRunState.InitialEnable
			) {
				await new InitialEnableSMTask( _task ).Run();
			}
			if ( !_previous._isInitialized ) { return; }


			if (	_previous._isActive &&
					_task._isCanActiveEvent()
			) {
				await new EnableSMTask( _task ).Run();
			} else {
				await new DisableSMTask( _task ).Run();
			}
		}
	}
}