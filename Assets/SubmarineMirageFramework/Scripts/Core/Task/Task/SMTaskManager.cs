//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestTaskManager
namespace SubmarineMirage.Task {
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using KoganeUnityLib;
	using Base;
	using Event;
	using Modifyler;
	using Marker;
	using Service;
	using Extension;
	using Utility;
	using Debug;



	public class SMTaskManager : SMStandardBase, ISMService {
		public static readonly SMTaskRunType[] CREATE_TASK_TYPES = new SMTaskRunType[] {
			SMTaskRunType.Dont, SMTaskRunType.Sequential, SMTaskRunType.Parallel,
		};
		public static readonly SMTaskRunType[] RUN_TASK_TYPES = new SMTaskRunType[] {
			SMTaskRunType.Sequential, SMTaskRunType.Parallel,
		};
		public static readonly SMTaskRunType[] DISPOSE_RUN_TASK_TYPES = RUN_TASK_TYPES.DeepCopy().Reverse();
		public static readonly SMTaskRunType[] DISPOSE_TASK_TYPES = CREATE_TASK_TYPES.DeepCopy().Reverse();

		SMTask _root { get; set; }
		SMTaskMarkerManager _taskMarkers { get; set; }
		public readonly SMModifyler _modifyler = new SMModifyler();

		public readonly SMSubject _fixedUpdateEvent = new SMSubject();
		public readonly SMSubject _updateEvent = new SMSubject();
		public readonly SMSubject _lateUpdateEvent = new SMSubject();
		public readonly SMSubject _onGUIEvent = new SMSubject();



		public SMTaskManager() {
//			_modifyler._isDebug = true;

			_taskMarkers = new SMTaskMarkerManager( this.GetAboutName() );
			var tasks = new SMTask[] {
				GetFirst( SMTaskRunType.Dont, true ),
#if TestTaskManager
				new Test.SMTestTask( "テスト", SMTaskRunType.Dont, false ),
#endif
				GetLast( SMTaskRunType.Dont, true ),
				GetFirst( SMTaskRunType.Sequential, true ),
				GetLast( SMTaskRunType.Sequential, true ),
				GetFirst( SMTaskRunType.Parallel, true ),
				GetLast( SMTaskRunType.Parallel, true ),
			};
			_root = tasks.First();
			SMTask last = null;
			tasks.ForEach( current => {
				current._taskManager = this;
				if ( last != null )	{ last.Link( current ); }
				last = current;
			} );

			_fixedUpdateEvent.AddLast().Subscribe( _ => {
				_modifyler._isLock = true;
				RUN_TASK_TYPES.ForEach( type => {
					GetAlls( type ).ForEach( task => task.FixedUpdate() );
				} );
				_modifyler._isLock = false;
			} );
			_updateEvent.AddLast().Subscribe( _ => {
				_modifyler._isLock = true;
				RUN_TASK_TYPES.ForEach( type => {
					GetAlls( type ).ForEach( task => task.Update() );
				} );
				_modifyler._isLock = false;
			} );
			_lateUpdateEvent.AddLast().Subscribe( _ => {
				_modifyler._isLock = true;
				RUN_TASK_TYPES.ForEach( type => {
					GetAlls( type ).ForEach( task => task.LateUpdate() );
				} );
				_modifyler._isLock = false;
			} );

			_disposables.AddFirst( () => {
				_taskMarkers.Dispose();
				_root = null;
				_modifyler.Dispose();

				_fixedUpdateEvent.Dispose();
				_updateEvent.Dispose();
				_lateUpdateEvent.Dispose();
				_onGUIEvent.Dispose();
			} );
			_disposables.AddFirst(
				Observable.EveryFixedUpdate()	.Subscribe( _ => _fixedUpdateEvent.Run() ),
				Observable.EveryUpdate()		.Subscribe( _ => _updateEvent.Run() ),
				Observable.EveryLateUpdate()	.Subscribe( _ => _lateUpdateEvent.Run() )
			);
			if ( SMDebugManager.IS_DEVELOP ) {
				_disposables.AddFirst(
					UniRxSMExtension.EveryOnGUI().Subscribe( _ => _onGUIEvent.Run() )
				);
			}
		}



		public UniTask Initialize()
			=> _taskMarkers.InitializeAll();

		public async UniTask Finalize() {
			await _taskMarkers.FinalizeAll();
			Dispose();
		}



		public SMTask GetFirst( SMTaskRunType type, bool isRaw = false )
			=> _taskMarkers.GetFirst( type, isRaw );

		public SMTask GetLast( SMTaskRunType type, bool isRaw = false )
			=> _taskMarkers.GetLast( type, isRaw );



		public IEnumerable<SMTask> GetAlls( SMTaskRunType type )
			=> _taskMarkers.GetAlls( type, true );

		public IEnumerable<SMTask> GetAlls() {
			for ( var t = _root; t != null; t = t._next ) {
				yield return t;
			}
		}



		SMModifyType GetType( SMTask task ) => (
			task._type == SMTaskRunType.Parallel	? SMModifyType.Parallel
													: SMModifyType.Normal
		);



		public async UniTask Register( SMTask task, bool isAdjustRun ) {
			var uniTask = _modifyler.Register(
				nameof( Register ),
				SMModifyType.Normal,
				async () => {
					var last = GetLast( task._type, true )._previous;
					last.Link( task );
					await UTask.DontWait();
				},
				() => task.Dispose()
			);

			if ( !isAdjustRun ) {
				await uniTask;
				return;
			}
			await AdjustRunTask( task );
		}

		public UniTask Unregister( SMTask task ) => _modifyler.Register(
			nameof( Unregister ),
			SMModifyType.Normal,
			async () => {
				task.Unlink();
				await UTask.DontWait();
			}
		);



		public UniTask CreateTask( SMTask task ) => _modifyler.Register(
			nameof( CreateTask ),
			GetType( task ),
			() => RunCreateTask( task )
		);

		async UniTask RunCreateTask( SMTask task ) {
			if ( task._ranState != SMTaskRunState.None )	{ return; }

			task.Create();
			task._ranState = SMTaskRunState.Create;
			await UTask.DontWait();
		}



		public UniTask SelfInitializeTask( SMTask task ) => _modifyler.Register(
			nameof( SelfInitializeTask ),
			GetType( task ),
			() => RunSelfInitializeTask( task )
		);

		async UniTask RunSelfInitializeTask( SMTask task ) {
			if ( task._ranState != SMTaskRunState.Create )	{ return; }

			await task._selfInitializeEvent.Run( task._asyncCancelerOnDispose );
			task._ranState = SMTaskRunState.SelfInitialize;
		}



		public UniTask InitializeTask( SMTask task ) => _modifyler.Register(
			nameof( InitializeTask ),
			GetType( task ),
			() => RunInitializeTask( task )
		);

		async UniTask RunInitializeTask( SMTask task ) {
			if ( task._ranState != SMTaskRunState.SelfInitialize )	{ return; }

			await task._initializeEvent.Run( task._asyncCancelerOnDispose );
			task._ranState = SMTaskRunState.Initialize;
		}



		public UniTask InitialEnableTask( SMTask task ) => _modifyler.Register(
			nameof( InitialEnableTask ),
			GetType( task ),
			() => RunInitialEnableTask( task )
		);

		async UniTask RunInitialEnableTask( SMTask task ) {
			if ( task._ranState != SMTaskRunState.Initialize )	{ return; }

			if ( task._isRequestInitialEnable ) {
				task._isRequestInitialEnable = false;

				if (	task._isCanActiveEvent() &&
						task._activeState != SMTaskActiveState.Enable
				) {
					task._asyncCancelerOnDisable.Recreate();
					task._enableEvent.Run();
					task._activeState = SMTaskActiveState.Enable;
				}
			}

			task._ranState = SMTaskRunState.InitialEnable;
			await UTask.DontWait();
		}



		public UniTask FinalDisableTask( SMTask task ) => _modifyler.Register(
			nameof( FinalDisableTask ),
			GetType( task ),
			() => RunFinalDisableTask( task )
		);

		async UniTask RunFinalDisableTask( SMTask task ) {
			if ( task._ranState >= SMTaskRunState.FinalDisable )	{ return; }

			var lastActiveState = task._activeState;
			task._activeState = SMTaskActiveState.Disable;
			task._asyncCancelerOnDisable.Cancel( false );

			if (	task._isCanActiveEvent() &&
					lastActiveState != SMTaskActiveState.Disable
			) {
				task._disableEvent.Run();
			}

			task._ranState = SMTaskRunState.FinalDisable;
			await UTask.DontWait();
		}



		public UniTask FinalizeTask( SMTask task ) => _modifyler.Register(
			nameof( FinalizeTask ),
			GetType( task ),
			() => RunFinalizeTask( task )
		);

		async UniTask RunFinalizeTask( SMTask task ) {
			if ( task._ranState != SMTaskRunState.FinalDisable )	{ return; }

			await task._finalizeEvent.Run( task._asyncCancelerOnDispose );
			task._ranState = SMTaskRunState.Finalize;
			task.Dispose();
		}



		public UniTask EnableTask( SMTask task ) => _modifyler.Register(
			nameof( EnableTask ),
			GetType( task ),
			() => RunEnableTask( task )
		);

		async UniTask RunEnableTask( SMTask task ) {
			if ( !task._isCanActiveEvent() )	{ return; }
			if ( task._isFinalizing )			{ return; }
			if ( !task._isInitialized ) {
				task._isRequestInitialEnable = true;
				return;
			}
			if ( task._activeState == SMTaskActiveState.Enable )	{ return; }

			task._asyncCancelerOnDisable.Recreate();
			task._enableEvent.Run();
			task._activeState = SMTaskActiveState.Enable;
			await UTask.DontWait();
		}



		public UniTask DisableTask( SMTask task ) => _modifyler.Register(
			nameof( DisableTask ),
			GetType( task ),
			() => RunDisableTask( task )
		);

		async UniTask RunDisableTask( SMTask task ) {
			if ( !task._isCanActiveEvent() )	{ return; }
			if ( task._isFinalizing )			{ return; }
			if ( !task._isInitialized ) {
				task._isRequestInitialEnable = false;
				return;
			}
			if ( task._activeState == SMTaskActiveState.Disable )	{ return; }

			task._activeState = SMTaskActiveState.Disable;
			task._asyncCancelerOnDisable.Cancel( false );
			task._disableEvent.Run();
			await UTask.DontWait();
		}



		public UniTask AdjustRunTask( SMTask task, SMTask previous = null ) => _modifyler.Register(
			nameof( AdjustRunTask ),
			SMModifyType.Normal,
			() => RunAdjustRunTask( task, previous )
		);

		async UniTask RunAdjustRunTask( SMTask task, SMTask previous = null ) {
			if ( previous == null )	{ previous = task._previous; }


			if (	previous._ranState >= SMTaskRunState.FinalDisable &&
					task._ranState < SMTaskRunState.FinalDisable &&
					task._type != SMTaskRunType.Dont
			) {
				await RunFinalDisableTask( task );
			}
			if (	previous._ranState >= SMTaskRunState.Finalize &&
					task._ranState < SMTaskRunState.Finalize
			) {
				if ( task._type != SMTaskRunType.Dont ) {
					await RunFinalizeTask( task );
				} else {
					Dispose();
				}
			}
			if ( previous._isFinalizing )	{ return; }


			if (	previous._ranState >= SMTaskRunState.Create &&
					task._ranState < SMTaskRunState.Create
			) {
				await RunCreateTask( task );
			}
			if ( task._type == SMTaskRunType.Dont )	{ return; }


			if (	previous._ranState >= SMTaskRunState.SelfInitialize &&
					task._ranState < SMTaskRunState.SelfInitialize
			) {
				await RunSelfInitializeTask( task );
			}
			if (	previous._ranState >= SMTaskRunState.Initialize &&
					task._ranState < SMTaskRunState.Initialize
			) {
				await RunInitializeTask( task );
			}
			if (	previous._ranState >= SMTaskRunState.InitialEnable &&
					task._ranState < SMTaskRunState.InitialEnable
			) {
				await RunInitialEnableTask( task );
			}
			if ( !previous._isInitialized )	{ return; }


			if (	previous._isActive &&
					task._isCanActiveEvent()
			) {
				await RunEnableTask( task );
			} else {
				await RunDisableTask( task );
			}
		}
	}
}