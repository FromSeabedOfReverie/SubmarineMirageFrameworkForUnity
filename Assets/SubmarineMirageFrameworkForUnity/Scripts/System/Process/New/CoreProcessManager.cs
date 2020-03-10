//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process.New {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using Singleton.New;
	using MultiEvent;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class CoreProcessManager : MonoBehaviourSingleton<CoreProcessManager> {
		public enum ExecutedState {
			Create,
			Load,
			Initialize,
			Enable,
			FixedUpdate,
			Update,
			LateUpdate,
			Disable,
			Finalize,
		}
		public enum ProcessType {
			DontWork,
			Work,
			FirstWork,
		}
		public enum ProcessLifeSpan {
			InScene,
			Forever,
		}
		readonly Dictionary< string, List<IProcess> > _processes
			= new Dictionary< string, List<IProcess> >();
		readonly List<IProcess> _deleteProcesses = new List<IProcess>();
		bool _isProcessDeleting;
#if DEVELOP
		public MultiSubject _onGUIEvent = new MultiSubject();
#endif
		CompositeDisposable _updateDisposer = new CompositeDisposable();


		public async UniTask Create( Func<UniTask> initializePlugin, Func<UniTask> registerProcesses ) {
			await initializePlugin();
			RegisterEvents( registerProcesses );
			Create();
			await _loadEvent.Invoke( _activeAsyncCancel );
			await _initializeEvent.Invoke( _activeAsyncCancel );

			Log.Debug( $"{this.GetAboutName()} : 初期化完了", Log.Tag.Process );

			Observable.EveryUpdate()
				.Where( _ => Input.GetKeyDown( KeyCode.Return ) )
				.Take( 1 )
				.Subscribe( _ => Clear().Forget() )
				.AddTo( _updateDisposer );
		}


		void RegisterEvents( Func<UniTask> registerProcesses ) {
		}


		public override void Create() {
		}


		public void Register( IProcess process ) {
			RegisterSub( process ).Forget();
		}


		async UniTask RegisterSub( IProcess process ) {
			process.Create();
			switch ( process._type ) {
				case ProcessType.DontWork:
					return;

				case ProcessType.Work:
					await UniTaskUtility.WaitUntil( process._activeAsyncCancel, () => _isInitialized );
					break;

				case ProcessType.FirstWork:
					break;
			}
			var name = process._lifeSpan == ProcessLifeSpan.Forever ? "Forever"
				: SceneManager.GetActiveScene().name;
			var list = _processes.GetOrDefault( name );
			if ( list == null )	{ _processes[name] = new List<IProcess>(); }
			_processes[name].Add( process );

			await _processes
				.SelectMany( pair => pair.Value )
				.Select( async p => await p._loadEvent.Invoke( p._activeAsyncCancel ) );
			await _processes
				.SelectMany( pair => pair.Value )
				.Select( async p => {
					await p._initializeEvent.Invoke( p._activeAsyncCancel );
					p._isInitialized = true;
				} );
			await _processes
				.SelectMany( pair => pair.Value )
				.Select( async p => {
					if ( !p._isInitialized ) {
						await p._loadEvent.Invoke( p._activeAsyncCancel );
						await p._initializeEvent.Invoke( p._activeAsyncCancel );
						p._isInitialized = true;
					}
					await p._enableEvent.Invoke( p._activeAsyncCancel );
					p._isActive = true;
				} );
			Observable.EveryFixedUpdate().Subscribe( _ => {
				_processes
					.SelectMany( pair => pair.Value )
					.Where( p => p._isActive )
					.ForEach( p => p._fixedUpdateEvent.Invoke() );
			} )
			.AddTo( _updateDisposer );
			Observable.EveryUpdate().Subscribe( _ => {
				_processes
					.SelectMany( pair => pair.Value )
					.Where( p => p._isActive )
					.ForEach( p => p._updateEvent.Invoke() );
			} )
			.AddTo( _updateDisposer );
			Observable.EveryLateUpdate().Subscribe( _ => {
				_processes
					.SelectMany( pair => pair.Value )
					.Where( p => p._isActive )
					.ForEach( p => p._lateUpdateEvent.Invoke() );
			} )
			.AddTo( _updateDisposer );
		}


		async UniTask Clear() {
			Log.Debug( "clear process" );
			_updateDisposer.Dispose();
			await _processes
				.SelectMany( pair => pair.Value )
				.Select( async p => {
					p._isActive = false;
					p.StopActiveAsync();
					await p._disableEvent.Invoke( p._finalizeAsyncCancel );
				} );
			await _processes
				.SelectMany( pair => pair.Value )
				.Select( async p => await p._finalizeEvent.Invoke( p._finalizeAsyncCancel ) );
			_processes
				.SelectMany( pair => pair.Value )
				.ForEach( p => p.Dispose() );
			_processes.Clear();
		}


		public bool ChangeExecutedState( IProcess process, ExecutedState newState,
											bool isCheckMonoBehaviour = false
		) {
			var isChange = false;		// 状態遷移可能か？
			var isProcessable = false;	// 関数実行可能か？

			// 読込処理か？
			var isLoader = process is IProcessLoader;
			// 更新処理か？
			var isUpdater =
				process is IProcessUpdater ||
				( isCheckMonoBehaviour && process is MonoBehaviour );

			var currentState = process._executedState;	// 現在の状態
			var delta = currentState - newState;		// 新規状態から見た、状態の遷移差


			// 繊維先状態で分岐
			switch ( newState ) {
				// 生成は、如何なる状態からも遷移不可能
				case ExecutedState.Create:
					isChange = false;
					isProcessable = isChange;
					break;

				// 読込は、前から順番に遷移可能
				case ExecutedState.Load:
					isChange =
						isLoader &&
						delta == -1;
					isProcessable = isChange;
					break;

				// 初期化は、前から順番か、読込を飛ばして遷移可能
				case ExecutedState.Initialize:
					isChange = delta == ( isLoader ? -1 : -2 );
					isProcessable = isChange;
					break;

				// 更新は、前から順番に遷移可能で、前の更新状態は好きに実行できる
				case ExecutedState.FixedUpdate:
				case ExecutedState.Update:
				case ExecutedState.LateUpdate:
					isChange =
						isUpdater &&
						process._isInitialized &&
						delta == -1;
					isProcessable =
						isUpdater &&
						process._isInitialized &&
						-1 <= delta &&
						currentState <= ExecutedState.LateUpdate;
					break;

				// 破棄は、同状態以外なら、何処からでも遷移可能
				case ExecutedState.Finalize:
					isChange = delta != 0;
					isProcessable = isChange;
					break;
			}


			// 遷移可能な場合、状態遷移
			if ( isChange )	{ process._executedState = newState; }
			// 関数実行可能か？を返す
			return isProcessable;
		}
	}
}