//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestTask
namespace SubmarineMirage.Task {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using UniRx;
	using KoganeUnityLib;
	using Base;
	using Modifyler;
	using Event;
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

		SMTask _root	{ get; set; }
		readonly SMTaskMarkerManager _markers;
		public readonly SMModifyler _modifyler = new SMModifyler( nameof( SMTaskManager ) );

		public readonly SMSubject _fixedUpdateEvent = new SMSubject();
		public readonly SMSubject _updateEvent = new SMSubject();
		public readonly SMSubject _lateUpdateEvent = new SMSubject();
		public readonly SMSubject _onGUIEvent = new SMSubject();



#region ToString
		public override void SetToString() {
			base.SetToString();

			_toStringer.Add( nameof( _root ), i => _root?.ToLineString() );
			_toStringer.Add( nameof( GetAlls ), i => GetAlls().ToShowString( i, true, false, false ) );
			_toStringer.Add( nameof( _modifyler ), i => _toStringer.DefaultValue( _modifyler, i, true ) );
			_toStringer.Add( nameof( _markers ), i => _toStringer.DefaultValue( _markers, i, true ) );
		}
#endregion



		public SMTaskManager() {
			_markers = new SMTaskMarkerManager( nameof( SMTaskManager ), this );
			var tasks = new SMTask[] {
				_markers.GetFirst( SMTaskRunType.Dont, true ),
				_markers.GetLast( SMTaskRunType.Dont, true ),
				_markers.GetFirst( SMTaskRunType.Sequential, true ),
				_markers.GetLast( SMTaskRunType.Sequential, true ),
				_markers.GetFirst( SMTaskRunType.Parallel, true ),
				_markers.GetLast( SMTaskRunType.Parallel, true ),
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
					_markers.GetAlls( type, true ).ForEach( task => FixedUpdateTask( task ) );
				} );
				_modifyler._isLock = false;
			} );
			_updateEvent.AddLast().Subscribe( _ => {
				_modifyler._isLock = true;
				RUN_TASK_TYPES.ForEach( type => {
					_markers.GetAlls( type, true ).ForEach( task => UpdateTask( task ) );
				} );
				_modifyler._isLock = false;
			} );
			_lateUpdateEvent.AddLast().Subscribe( _ => {
				_modifyler._isLock = true;
				RUN_TASK_TYPES.ForEach( type => {
					_markers.GetAlls( type, true ).ForEach( task => LateUpdateTask( task ) );
				} );
				_modifyler._isLock = false;
			} );

			_disposables.AddFirst( () => {
				_markers.Dispose();
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
#if TestTask
			_disposables.AddFirst( () =>
				SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( Dispose )} : start\n{this}" )
			);
			_disposables.AddLast( () =>
				SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( Dispose )} : end\n{this}" )
			);
			SMLog.Debug( $"{nameof( SMTaskManager )}() : \n{this}" );
#endif
		}



		public UniTask Initialize()
			=> _markers.InitializeAll();

		public async UniTask Finalize() {
			await _markers.FinalizeAll();
			Dispose();
		}



		public IEnumerable<SMTask> GetAlls() {
			CheckDisposeError( nameof( GetAlls ) );

			for ( var t = _root; t != null; t = t._next ) {
				yield return t;
			}
		}



		SMModifyType GetType( SMTask task ) => (
			task._type == SMTaskRunType.Parallel	? SMModifyType.Parallel
													: SMModifyType.Normal
		);



		public async UniTask Register( SMTask task, bool isAdjustRun ) {
			CheckTaskNullOrDisposeError( nameof( Register ), task );

			var registerTask = _modifyler.Register(
				nameof( Register ),
				SMModifyType.Normal,
				async () => {
					if ( task._isDispose )	{ return; }

#if TestTask
					SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( Register )} : start\n{task}" );
#endif
					// 基底コンストラクタ → CreateやDispose → 派生コンストラクタ、の順に実行される可能性あり
					// 基底コンストラクタ → 派生コンストラクタ → CreateやDispose、の順に必ず実行させる為、
					// Register前に、1フレーム待機する
					await UTask.NextFrame( task._asyncCancelerOnDispose );

					_markers.LinkLast( task );
#if TestTask
					SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( Register )} : end\n{task}" );
#endif
				},
				() => task.Dispose()
			);

			if ( !isAdjustRun ) {
				await registerTask;
				return;
			}
			await AdjustRunTask( task );
		}



		public async UniTask Unregister( SMTask task ) {
			CheckTaskNullError( nameof( Unregister ), task );

			await _modifyler.Register(
				nameof( Unregister ),
				SMModifyType.Normal,
				async () => {
#if TestTask
					var ids = $"{task._id} ↑{task._previous?._id} ↓{task._next?._id}";
					SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( Unregister )} : start\n{ids}\n{task}" );
#endif
					task.Unlink();
					await UTask.DontWait();
#if TestTask
					ids = $"{task._id} ↑{task._previous?._id} ↓{task._next?._id}";
					SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( Unregister )} : end\n{ids}\n{task}" );
#endif
				}
			);
		}



		public async UniTask CreateTask( SMTask task ) {
			CheckTaskNullOrDisposeError( nameof( CreateTask ), task );

			await _modifyler.Register(
				nameof( CreateTask ),
				GetType( task ),
				() => RunCreateTask( task )
			);
		}

		async UniTask RunCreateTask( SMTask task ) {
			if ( task._ranState != SMTaskRunState.None )	{ return; }

#if TestTask
			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( RunCreateTask )} : start\n{task}" );
#endif
			task.Create();
			task._ranState = SMTaskRunState.Create;
			if ( task._type == SMTaskRunType.Dont ) {
				task._isInitialized = true;
				task._activeState =
					task._isRequestInitialEnable ? SMTaskActiveState.Enable : SMTaskActiveState.Disable;
				task._isRequestInitialEnable = false;
			}
			await UTask.DontWait();
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( RunCreateTask )} : end\n{task}" );
#endif
		}



		public async UniTask SelfInitializeTask( SMTask task ) {
			CheckTaskNullOrDisposeError( nameof( SelfInitializeTask ), task );
			CheckDontTaskError( nameof( SelfInitializeTask ), task );

			await _modifyler.Register(
				nameof( SelfInitializeTask ),
				GetType( task ),
				() => RunSelfInitializeTask( task )
			);
		}

		async UniTask RunSelfInitializeTask( SMTask task ) {
			if ( task._ranState != SMTaskRunState.Create )	{ return; }

#if TestTask
			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( RunSelfInitializeTask )} : start\n{task}" );
#endif
			await task._selfInitializeEvent.Run( task._asyncCancelerOnDispose );
			task._ranState = SMTaskRunState.SelfInitialize;
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( RunSelfInitializeTask )} : end\n{task}" );
#endif
		}



		public async UniTask InitializeTask( SMTask task ) {
			CheckTaskNullOrDisposeError( nameof( InitializeTask ), task );
			CheckDontTaskError( nameof( InitializeTask ), task );

			await _modifyler.Register(
				nameof( InitializeTask ),
				GetType( task ),
				() => RunInitializeTask( task )
			);
		}

		async UniTask RunInitializeTask( SMTask task ) {
			if ( task._ranState != SMTaskRunState.SelfInitialize )	{ return; }

#if TestTask
			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( RunInitializeTask )} : start\n{task}" );
#endif
			await task._initializeEvent.Run( task._asyncCancelerOnDispose );
			task._ranState = SMTaskRunState.Initialize;
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( RunInitializeTask )} : end\n{task}" );
#endif
		}



		public async UniTask InitialEnableTask( SMTask task ) {
			CheckTaskNullOrDisposeError( nameof( InitialEnableTask ), task );
			CheckDontTaskError( nameof( InitialEnableTask ), task );

			await _modifyler.Register(
				nameof( InitialEnableTask ),
				GetType( task ),
				() => RunInitialEnableTask( task )
			);
		}

		async UniTask RunInitialEnableTask( SMTask task ) {
			if ( task._ranState != SMTaskRunState.Initialize )	{ return; }

#if TestTask
			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( RunInitialEnableTask )} : start\n{task}" );
#endif
			if ( task._isRequestInitialEnable ) {
				task._isRequestInitialEnable = false;

				if (	task._isCanActive &&
						task._activeState != SMTaskActiveState.Enable
				) {
					task._asyncCancelerOnDisable.Recreate();
					task._enableEvent.Run();
					task._activeState = SMTaskActiveState.Enable;
				}
			}

			task._ranState = SMTaskRunState.InitialEnable;
			task._isInitialized = true;
			await UTask.DontWait();
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( RunInitialEnableTask )} : end\n{task}" );
#endif
		}



		public async UniTask FinalDisableTask( SMTask task ) {
			CheckTaskNullOrDisposeError( nameof( FinalDisableTask ), task );
			CheckDontTaskError( nameof( FinalDisableTask ), task );

			await _modifyler.Register(
				nameof( FinalDisableTask ),
				GetType( task ),
				() => RunFinalDisableTask( task )
			);
		}

		async UniTask RunFinalDisableTask( SMTask task ) {
			if ( task._ranState >= SMTaskRunState.FinalDisable )	{ return; }

#if TestTask
			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( RunFinalDisableTask )} : start\n{task}" );
#endif
			if ( task._activeState != SMTaskActiveState.Disable ) {
				task._activeState = SMTaskActiveState.Disable;
				task._asyncCancelerOnDisable.Cancel( false );

				if ( task._isCanActive ) {
					task._disableEvent.Run();
				}
			}

			task._ranState = SMTaskRunState.FinalDisable;
			await UTask.DontWait();
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( RunFinalDisableTask )} : end\n{task}" );
#endif
		}



		public async UniTask FinalizeTask( SMTask task ) {
			CheckTaskNullOrDisposeError( nameof( FinalizeTask ), task );
			CheckDontTaskError( nameof( FinalizeTask ), task );

			await _modifyler.Register(
				nameof( FinalizeTask ),
				GetType( task ),
				() => RunFinalizeTask( task )
			);
		}

		async UniTask RunFinalizeTask( SMTask task ) {
			if ( task._ranState != SMTaskRunState.FinalDisable )	{ return; }

#if TestTask
			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( RunFinalizeTask )} : start\n{task}" );
#endif
			await task._finalizeEvent.Run( task._asyncCancelerOnDispose );
			task._ranState = SMTaskRunState.Finalize;
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( RunFinalizeTask )} : end\n{task}" );
#endif
		}



		public async UniTask DisposeTask( SMTask task ) {
			CheckTaskNullOrDisposeError( nameof( DisposeTask ), task );

			await _modifyler.Register(
				nameof( DisposeTask ),
				GetType( task ),
				() => RunDisposeTask( task )
			);
		}

		async UniTask RunDisposeTask( SMTask task ) {
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( RunDisposeTask )} : start\n{task}" );
#endif
			task.Dispose();
			await UTask.DontWait();
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( RunDisposeTask )} : end\n{task}" );
#endif
		}



		void FixedUpdateTask( SMTask task ) {
			CheckTaskNullError( nameof( FixedUpdateTask ), task );
			if ( !task._isOperable )	{ return; }
			if ( !task._isActive )		{ return; }
			if ( task._ranState < SMTaskRunState.InitialEnable )	{ return; }

#if TestTask
//			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( FixedUpdateTask )} : start\n{task}" );
#endif
			task._fixedUpdateEvent.Run();

			if ( task._ranState == SMTaskRunState.InitialEnable ) {
				task._ranState = SMTaskRunState.FixedUpdate;
#if TestTask
				SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( FixedUpdateTask )} : first call\n{task}" );
#endif
			}
#if TestTask
//			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( FixedUpdateTask )} : end\n{task}" );
#endif
		}

		void UpdateTask( SMTask task ) {
			CheckTaskNullError( nameof( UpdateTask ), task );
			if ( !task._isOperable )	{ return; }
			if ( !task._isActive )		{ return; }
			if ( task._ranState < SMTaskRunState.FixedUpdate )	{ return; }

#if TestTask
//			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( UpdateTask )} : start\n{task}" );
#endif
			task._updateEvent.Run();

			if ( task._ranState == SMTaskRunState.FixedUpdate ) {
				task._ranState = SMTaskRunState.Update;
#if TestTask
				SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( UpdateTask )} : first call\n{task}" );
#endif
			}
#if TestTask
//			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( UpdateTask )} : end\n{task}" );
#endif
		}

		void LateUpdateTask( SMTask task ) {
			CheckTaskNullError( nameof( LateUpdateTask ), task );
			if ( !task._isOperable )	{ return; }
			if ( !task._isActive )		{ return; }
			if ( task._ranState < SMTaskRunState.Update )	{ return; }

#if TestTask
//			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( LateUpdateTask )} : start\n{task}" );
#endif
			task._lateUpdateEvent.Run();

			if ( task._ranState == SMTaskRunState.Update ) {
				task._ranState = SMTaskRunState.LateUpdate;
#if TestTask
				SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( LateUpdateTask )} : first call\n{task}" );
#endif
			}
#if TestTask
//			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( LateUpdateTask )} : end\n{task}" );
#endif
		}



		public async UniTask EnableTask( SMTask task ) {
			CheckTaskNullOrDisposeError( nameof( EnableTask ), task );
			CheckDontTaskError( nameof( EnableTask ), task );

			await _modifyler.Register(
				nameof( EnableTask ),
				GetType( task ),
				() => RunEnableTask( task )
			);
		}

		async UniTask RunEnableTask( SMTask task ) {
			if ( !task._isCanActive )	{ return; }
			if ( task._isFinalize )		{ return; }
			if ( !task._isInitialized ) {
				task._isRequestInitialEnable = true;
				return;
			}
			if ( task._activeState == SMTaskActiveState.Enable )	{ return; }

#if TestTask
			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( RunEnableTask )} : start\n{task}" );
#endif
			task._asyncCancelerOnDisable.Recreate();
			task._enableEvent.Run();
			task._activeState = SMTaskActiveState.Enable;
			await UTask.DontWait();
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( RunEnableTask )} : end\n{task}" );
#endif
		}



		public async UniTask DisableTask( SMTask task ) {
			CheckTaskNullOrDisposeError( nameof( DisableTask ), task );
			CheckDontTaskError( nameof( DisableTask ), task );

			await _modifyler.Register(
				nameof( DisableTask ),
				GetType( task ),
				() => RunDisableTask( task )
			);
		}

		async UniTask RunDisableTask( SMTask task ) {
			if ( !task._isCanActive )	{ return; }
			if ( task._isFinalize )		{ return; }
			if ( !task._isInitialized ) {
				task._isRequestInitialEnable = false;
				return;
			}
			if ( task._activeState == SMTaskActiveState.Disable )	{ return; }

#if TestTask
			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( RunDisableTask )} : start\n{task}" );
#endif
			task._activeState = SMTaskActiveState.Disable;
			task._asyncCancelerOnDisable.Cancel( false );
			task._disableEvent.Run();
			await UTask.DontWait();
#if TestTask
			SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( RunDisableTask )} : end\n{task}" );
#endif
		}



		public UniTask ChangeActiveTask( SMTask task, bool isActive )
			=> isActive ? EnableTask( task ) : DisableTask( task );



		public async UniTask AdjustRunTask( SMTask task, SMTask previous = null ) {
			CheckTaskNullOrDisposeError( nameof( AdjustRunTask ), task );

			await _modifyler.Register(
				nameof( AdjustRunTask ),
				SMModifyType.Normal,
				() => RunAdjustRunTask( task, previous )
			);
		}

		async UniTask RunAdjustRunTask( SMTask task, SMTask previous = null ) {
			if ( task._isDispose )	{ return; }
			if ( previous == null )	{ previous = task._previous; }
			if ( previous == null ) {
				throw new InvalidOperationException( string.Join( "\n",
					$"実行状態の合わせ元のタスクが無 : ",
					$"{nameof( SMTaskManager )}.{nameof( RunAdjustRunTask )}",
					$"{nameof( previous )} : \n{previous}"
				) );
			}

#if TestTask
			SMLog.Debug( string.Join( "\n",
				$"{nameof( SMTaskManager )}.{nameof( RunAdjustRunTask )} : start",
				$"{nameof( previous )} : {previous}",
				$"{nameof( task )} : {task}"
			) );
#endif
			if ( previous._isFinalize ) {
				if (	task._type != SMTaskRunType.Dont &&
						previous._ranState >= SMTaskRunState.FinalDisable &&
						task._ranState < SMTaskRunState.FinalDisable
				) {
					await RunFinalDisableTask( task );
				}
				if (	task._type != SMTaskRunType.Dont &&
						previous._ranState >= SMTaskRunState.Finalize &&
						task._ranState < SMTaskRunState.Finalize
				) {
					await RunFinalizeTask( task );
				}
				if (	previous._ranState >= SMTaskRunState.Dispose &&
						task._ranState < SMTaskRunState.Dispose
				) {
					await RunDisposeTask( task );
				}

			} else {
				if (	previous._ranState >= SMTaskRunState.Create &&
						task._ranState < SMTaskRunState.Create
				) {
					await RunCreateTask( task );
				}
				if (	task._type != SMTaskRunType.Dont &&
						previous._ranState >= SMTaskRunState.SelfInitialize &&
						task._ranState < SMTaskRunState.SelfInitialize
				) {
					await RunSelfInitializeTask( task );
				}
				if (	task._type != SMTaskRunType.Dont &&
						previous._ranState >= SMTaskRunState.Initialize &&
						task._ranState < SMTaskRunState.Initialize
				) {
					await RunInitializeTask( task );
				}
				if (	task._type != SMTaskRunType.Dont &&
						previous._ranState >= SMTaskRunState.InitialEnable &&
						task._ranState < SMTaskRunState.InitialEnable
				) {
					await RunInitialEnableTask( task );
				}

				if (	task._type != SMTaskRunType.Dont &&
						previous._isInitialized
				) {
					if ( previous._isActive )	{ await RunEnableTask( task ); }
					else						{ await RunDisableTask( task ); }
				}
			}
#if TestTask
			SMLog.Debug( string.Join( "\n",
				$"{nameof( SMTaskManager )}.{nameof( RunAdjustRunTask )} : end",
				$"{nameof( previous )} : {previous}",
				$"{nameof( task )} : {task}"
			) );
#endif
		}



		public async UniTask DestroyTask( SMTask task ) {
			CheckTaskNullOrDisposeError( nameof( DestroyTask ), task );

			await _modifyler.Register(
				nameof( DestroyTask ),
				SMModifyType.Normal,
				async () => {
#if TestTask
					SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( DestroyTask )} : start\n{task}" );
#endif
					// Parallelだと実行順序が乖離しそうな為、Normalの逐次処理で、Run系を直接実行
					if ( task._type != SMTaskRunType.Dont ) {
						await RunFinalDisableTask( task );
						await RunFinalizeTask( task );
					}
					await RunDisposeTask( task );
#if TestTask
					SMLog.Debug( $"{nameof( SMTaskManager )}.{nameof( DestroyTask )} : end\n{task}" );
#endif
				}
			);
		}



		void CheckDontTaskError( string name, SMTask task ) {
			if ( task._type != SMTaskRunType.Dont )	{ return; }

			throw new NotSupportedException( string.Join( "\n",
				$"未対応 : {SMTaskRunType.Dont}",
				$"{nameof( SMTaskManager )}.{name}",
				$"{nameof( task )} : \n{task}"
			) );
		}

		void CheckTaskNullError( string name, SMTask task ) {
			if ( task != null )	{ return; }

			throw new ArgumentNullException( string.Join( "\n",
				$"引数に無を指定 : {nameof( task )}",
				$"{nameof( SMTaskManager )}.{name}",
				$"{nameof( task )} : \n{task}"
			) );
		}

		void CheckTaskNullOrDisposeError( string name, SMTask task ) {
			if ( task != null && !task._isDispose )	{ return; }

			throw new ObjectDisposedException( string.Join( "\n",
				$"既に解放済 : {nameof( task )}",
				$"{nameof( SMTaskManager )}.{name}",
				$"{nameof( task )} : \n{task}"
			) );
		}
	}
}