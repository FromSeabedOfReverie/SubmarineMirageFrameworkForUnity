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
	using Group.Manager.Modifyler;
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
			await SMSceneManager.s_instance._modifyler.RegisterAndRun( new CreateSMBehaviour() );
			await SMSceneManager.s_instance._modifyler.RegisterAndRun( new SelfInitializeSMBehaviour() );
			await SMSceneManager.s_instance._modifyler.RegisterAndRun( new InitializeSMBehaviour() );
			await SMSceneManager.s_instance._modifyler.RegisterAndRun( new InitialEnableSMBehaviour( true ) );

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
			SMGroupManagerApplyer.FixedUpdate( _foreverScene._groups );
			SMGroupManagerApplyer.FixedUpdate( _currentScene._groups );
		}

		void Update() {
			return;
			if ( _isDispose )	{ return; }
			SMGroupManagerApplyer.Update( _foreverScene._groups );
			SMGroupManagerApplyer.Update( _currentScene._groups );
		}

		void LateUpdate() {
			return;
			if ( _isDispose )	{ return; }
			SMGroupManagerApplyer.LateUpdate( _foreverScene._groups );
			SMGroupManagerApplyer.LateUpdate( _currentScene._groups );
		}

#if DEVELOP
		void OnGUI() {
			if ( _onGUIEvent._isDispose )	{ return; }
			_onGUIEvent.Run();
		}
#endif
	}
}