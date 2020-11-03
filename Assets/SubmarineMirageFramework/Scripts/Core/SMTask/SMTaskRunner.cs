//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTask
namespace SubmarineMirage.SMTask {
	using System;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using MultiEvent;
	using FSM;
	using Modifyler;
	using Singleton;
	using Scene;
	using Main;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMTaskRunner : RawMonoBehaviourSingleton<SMTaskRunner> {
		public ForeverScene _foreverScene => SceneManager.s_instance._fsm._foreverScene;
		public BaseScene _currentScene => SceneManager.s_instance._fsm._scene;
		public bool _isEnterInForever	=> _foreverScene._objects._isEnter;
		public bool _isEnterInScene		=> _currentScene._objects._isEnter;
#if DEVELOP
		public readonly MultiSubject _onGUIEvent = new MultiSubject();
#endif


		void Awake() {
#if DEVELOP
			_disposables.AddLast( _onGUIEvent );
#endif
			_disposables.AddLast(
				Observable.OnceApplicationQuit().Subscribe( _ => SubmarineMirage.DisposeInstance() )
			);
		}


		public async UniTask RunForeverTasks( Func<UniTask> registerBehaviours ) {
#if TestSMTask
			Log.Debug( $"{nameof( SMTaskRunner )}.{nameof( RunForeverTasks )} : start" );
#endif

#if TestSMTask
			Log.Debug( $"{nameof( SMTaskRunner )}.{nameof( registerBehaviours )} : start" );
#endif
			await registerBehaviours();
#if TestSMTask
			Log.Debug( $"{nameof( SMTaskRunner )}.{nameof( registerBehaviours )} : end" );
#endif
//			return;

#if TestSMTask
			Log.Debug( $"{nameof( SMTaskRunner )}.{nameof( _foreverScene )}.Entering : start" );
#endif
// TODO : シーン読込後に、テストオブジェクトを作成してしまう
			await _foreverScene.RunStateEvent( FiniteStateMachineRunState.Entering );
#if TestSMTask
			Log.Debug( $"{nameof( SMTaskRunner )}.{nameof( _foreverScene )}.Entering : end" );
#endif

// TODO : デバッグ用、暫定
			await RunStateSMBehaviour.RegisterAndRun( SceneManager.s_instance, SMTaskRunState.Create );
			await RunStateSMBehaviour.RegisterAndRun( SceneManager.s_instance, SMTaskRunState.SelfInitializing );
			await RunStateSMBehaviour.RegisterAndRun( SceneManager.s_instance, SMTaskRunState.Initializing );
			await ChangeActiveSMBehaviour.RegisterAndRunInitial( SceneManager.s_instance );

#if TestSMTask
			Log.Debug( $"{nameof( SMTaskRunner )}.{nameof( RunForeverTasks )} : end" );
#endif

			Log.Debug( $"{this.GetAboutName()} : 初期化完了", Log.Tag.SMTask );
		}


		public async UniTask EndForeverTasks() {
			await _currentScene._fsm.ChangeScene( null );
			await _foreverScene.RunStateEvent( FiniteStateMachineRunState.Exiting );
			Dispose();

			Log.Debug( $"{this.GetAboutName()} : 破棄完了", Log.Tag.SMTask );
		}


		void FixedUpdate() {
			return;
			if ( _isDispose )	{ return; }

			_foreverScene._objects.RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.FixedUpdate ).Forget();
			_foreverScene._objects.RunAllStateEvents( SMTaskType.Work, SMTaskRunState.FixedUpdate ).Forget();

			_currentScene._objects.RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.FixedUpdate ).Forget();
			_currentScene._objects.RunAllStateEvents( SMTaskType.Work, SMTaskRunState.FixedUpdate ).Forget();
		}

		void Update() {
			return;
			if ( _isDispose )	{ return; }

			_foreverScene._objects.RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.Update ).Forget();
			_foreverScene._objects.RunAllStateEvents( SMTaskType.Work, SMTaskRunState.Update ).Forget();

			_currentScene._objects.RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.Update ).Forget();
			_currentScene._objects.RunAllStateEvents( SMTaskType.Work, SMTaskRunState.Update ).Forget();
		}

		void LateUpdate() {
			return;
			if ( _isDispose )	{ return; }

			_foreverScene._objects.RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.LateUpdate ).Forget();
			_foreverScene._objects.RunAllStateEvents( SMTaskType.Work, SMTaskRunState.LateUpdate ).Forget();

			_currentScene._objects.RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.LateUpdate ).Forget();
			_currentScene._objects.RunAllStateEvents( SMTaskType.Work, SMTaskRunState.LateUpdate ).Forget();
		}

#if DEVELOP
		void OnGUI() {
			if ( _onGUIEvent._isDispose )	{ return; }
			_onGUIEvent.Run();
		}
#endif
	}
}