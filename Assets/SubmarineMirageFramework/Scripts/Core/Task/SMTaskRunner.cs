//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestTaskRunner
namespace SubmarineMirage.Task {
	using System;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using MultiEvent;
	using Behaviour.Modifyler;
	using FSM;
	using Singleton;
	using Scene;
	using Main;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMTaskRunner : SMRawMonoBehaviourSingleton<SMTaskRunner> {
		public ForeverSMScene _foreverScene => SMSceneManager.s_instance._fsm._foreverScene;
		public SMScene _currentScene => SMSceneManager.s_instance._fsm._scene;
		public bool _isEnterInForever	=> _foreverScene._groups._isEnter;
		public bool _isEnterInScene		=> _currentScene._groups._isEnter;
#if DEVELOP
		public readonly SMMultiSubject _onGUIEvent = new SMMultiSubject();
#endif


		protected override void Awake() {
			base.Awake();
#if DEVELOP
			_disposables.Add( _onGUIEvent );
#endif
			_disposables.Add(
				Observable.OnceApplicationQuit().Subscribe( _ => SubmarineMirage.DisposeInstance() )
			);
		}


		public async UniTask RunForeverTasks( Func<UniTask> registerBehaviours ) {
#if TestTaskRunner
			SMLog.Debug( $"{nameof( SMTaskRunner )}.{nameof( RunForeverTasks )} : start" );
#endif

#if TestTaskRunner
			SMLog.Debug( $"{nameof( SMTaskRunner )}.{nameof( registerBehaviours )} : start" );
#endif
			await registerBehaviours();
#if TestTaskRunner
			SMLog.Debug( $"{nameof( SMTaskRunner )}.{nameof( registerBehaviours )} : end" );
#endif
//			return;

#if TestTaskRunner
			SMLog.Debug( $"{nameof( SMTaskRunner )}.{nameof( _foreverScene )}.Entering : start" );
#endif
// TODO : シーン読込後に、テストオブジェクトを作成してしまう
			await _foreverScene.RunStateEvent( SMFSMRunState.Entering );
#if TestTaskRunner
			SMLog.Debug( $"{nameof( SMTaskRunner )}.{nameof( _foreverScene )}.Entering : end" );
#endif

// TODO : デバッグ用、暫定
			await RunStateSMBehaviour.RegisterAndRun( SMSceneManager.s_instance, SMTaskRunState.Create );
			await RunStateSMBehaviour.RegisterAndRun( SMSceneManager.s_instance, SMTaskRunState.SelfInitialize );
			await RunStateSMBehaviour.RegisterAndRun( SMSceneManager.s_instance, SMTaskRunState.Initialize );
			await ChangeActiveSMBehaviour.RegisterAndRunInitial( SMSceneManager.s_instance );

#if TestTaskRunner
			SMLog.Debug( $"{nameof( SMTaskRunner )}.{nameof( RunForeverTasks )} : end" );
#endif

			SMLog.Debug( $"{this.GetAboutName()} : 初期化完了", SMLogTag.Task );
		}


		public async UniTask EndForeverTasks() {
			await _currentScene._fsm.ChangeScene( null );
			await _foreverScene.RunStateEvent( SMFSMRunState.Exiting );
			Dispose();

			SMLog.Debug( $"{this.GetAboutName()} : 破棄完了", SMLogTag.Task );
		}


		void FixedUpdate() {
			return;
			if ( _isDispose )	{ return; }

			_foreverScene._groups.RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.FixedUpdate ).Forget();
			_foreverScene._groups.RunAllStateEvents( SMTaskType.Work, SMTaskRunState.FixedUpdate ).Forget();

			_currentScene._groups.RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.FixedUpdate ).Forget();
			_currentScene._groups.RunAllStateEvents( SMTaskType.Work, SMTaskRunState.FixedUpdate ).Forget();
		}

		void Update() {
			return;
			if ( _isDispose )	{ return; }

			_foreverScene._groups.RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.Update ).Forget();
			_foreverScene._groups.RunAllStateEvents( SMTaskType.Work, SMTaskRunState.Update ).Forget();

			_currentScene._groups.RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.Update ).Forget();
			_currentScene._groups.RunAllStateEvents( SMTaskType.Work, SMTaskRunState.Update ).Forget();
		}

		void LateUpdate() {
			return;
			if ( _isDispose )	{ return; }

			_foreverScene._groups.RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.LateUpdate ).Forget();
			_foreverScene._groups.RunAllStateEvents( SMTaskType.Work, SMTaskRunState.LateUpdate ).Forget();

			_currentScene._groups.RunAllStateEvents( SMTaskType.FirstWork, SMTaskRunState.LateUpdate ).Forget();
			_currentScene._groups.RunAllStateEvents( SMTaskType.Work, SMTaskRunState.LateUpdate ).Forget();
		}

#if DEVELOP
		void OnGUI() {
			if ( _onGUIEvent._isDispose )	{ return; }
			_onGUIEvent.Run();
		}
#endif
	}
}