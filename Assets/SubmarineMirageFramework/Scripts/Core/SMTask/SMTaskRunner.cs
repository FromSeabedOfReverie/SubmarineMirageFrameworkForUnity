//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestSMTask
namespace SubmarineMirage.SMTask {
	using System;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using MultiEvent;
	using FSM;
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
			await SceneManager.s_instance.RunStateEvent( SMTaskRanState.Creating );
			await SceneManager.s_instance.RunStateEvent( SMTaskRanState.Loading );
			await SceneManager.s_instance.RunStateEvent( SMTaskRanState.Initializing );
			await SceneManager.s_instance.RunActiveEvent();

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

			_foreverScene._objects.RunAllStateEvents( SMTaskType.FirstWork, SMTaskRanState.FixedUpdate ).Forget();
			_foreverScene._objects.RunAllStateEvents( SMTaskType.Work, SMTaskRanState.FixedUpdate ).Forget();

			_currentScene._objects.RunAllStateEvents( SMTaskType.FirstWork, SMTaskRanState.FixedUpdate ).Forget();
			_currentScene._objects.RunAllStateEvents( SMTaskType.Work, SMTaskRanState.FixedUpdate ).Forget();
		}

		void Update() {
			return;
			if ( _isDispose )	{ return; }

			_foreverScene._objects.RunAllStateEvents( SMTaskType.FirstWork, SMTaskRanState.Update ).Forget();
			_foreverScene._objects.RunAllStateEvents( SMTaskType.Work, SMTaskRanState.Update ).Forget();

			_currentScene._objects.RunAllStateEvents( SMTaskType.FirstWork, SMTaskRanState.Update ).Forget();
			_currentScene._objects.RunAllStateEvents( SMTaskType.Work, SMTaskRanState.Update ).Forget();
		}

		void LateUpdate() {
			return;
			if ( _isDispose )	{ return; }

			_foreverScene._objects.RunAllStateEvents( SMTaskType.FirstWork, SMTaskRanState.LateUpdate ).Forget();
			_foreverScene._objects.RunAllStateEvents( SMTaskType.Work, SMTaskRanState.LateUpdate ).Forget();

			_currentScene._objects.RunAllStateEvents( SMTaskType.FirstWork, SMTaskRanState.LateUpdate ).Forget();
			_currentScene._objects.RunAllStateEvents( SMTaskType.Work, SMTaskRanState.LateUpdate ).Forget();
		}

#if DEVELOP
		void OnGUI() {
			if ( _onGUIEvent._isDispose )	{ return; }
			_onGUIEvent.Run();
		}
#endif
	}
}