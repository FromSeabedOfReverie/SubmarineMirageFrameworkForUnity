//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask {
	using System;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Singleton;
	using FSM;
	using MultiEvent;
	using Scene;
	using Main;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMTaskRunner : MonoBehaviourSingleton<SMTaskRunner> {
		public ForeverScene _foreverScene => SceneManager.s_instance._fsm._foreverScene;
		public BaseScene _currentScene => SceneManager.s_instance._fsm._scene;
		public bool _isEnterInForever	=> _foreverScene._objects._isEnter;
		public bool _isEnterInScene		=> _currentScene._objects._isEnter;
#if DEVELOP
		public readonly MultiSubject _onGUIEvent = new MultiSubject();
#endif


		new public static void CreateInstance() {
			if ( s_isCreated )	{ return; }

			s_instanceObject = FindObjectOfType<SMTaskRunner>();
			if ( s_isCreated )	{ return; }

			s_instanceObject = MonoBehaviourSingletonManager.s_instance.AddComponent<SMTaskRunner>();
// TODO : 多分使わないので、未設定でもエラーにならない
//			s_instanceObject._object = MonoBehaviourSingletonManager.s_instance._object;
			s_instanceObject.Constructor();

			Log.Debug( $"作成（{nameof( SMTask )}） : {s_instanceObject.GetAboutName()}", Log.Tag.Singleton );
		}


		public async UniTask Create( Func<UniTask> registerBehaviours ) {
			_loadEvent.AddLast( async canceler => {
				await registerBehaviours();
			} );

			_initializeEvent.AddLast( async canceler => {
// TODO : シーン読込後に、テストオブジェクトを作成してしまう
				await _foreverScene.RunStateEvent( FiniteStateMachineRunState.Entering );
// TODO : デバッグ用、暫定
				await SceneManager.s_instance.RunStateEvent( SMTaskRanState.Creating );
				await SceneManager.s_instance.RunStateEvent( SMTaskRanState.Loading );
				await SceneManager.s_instance.RunStateEvent( SMTaskRanState.Initializing );
				await SceneManager.s_instance.RunActiveEvent();
			} );

			_fixedUpdateEvent.AddLast().Subscribe( _ => {
				_foreverScene._objects.RunAllStateEvents(
					SMTaskType.FirstWork, SMTaskRanState.FixedUpdate ).Forget();
				_foreverScene._objects.RunAllStateEvents(
					SMTaskType.Work, SMTaskRanState.FixedUpdate ).Forget();

				_currentScene._objects.RunAllStateEvents(
					SMTaskType.FirstWork, SMTaskRanState.FixedUpdate ).Forget();
				_currentScene._objects.RunAllStateEvents(
					SMTaskType.Work, SMTaskRanState.FixedUpdate ).Forget();
			} );

			_updateEvent.AddLast().Subscribe( _ => {
				_foreverScene._objects.RunAllStateEvents(
					SMTaskType.FirstWork, SMTaskRanState.Update ).Forget();
				_foreverScene._objects.RunAllStateEvents(
					SMTaskType.Work, SMTaskRanState.Update ).Forget();

				_currentScene._objects.RunAllStateEvents(
					SMTaskType.FirstWork, SMTaskRanState.Update ).Forget();
				_currentScene._objects.RunAllStateEvents(
					SMTaskType.Work, SMTaskRanState.Update ).Forget();
			} );

			_lateUpdateEvent.AddLast().Subscribe( _ => {
				_foreverScene._objects.RunAllStateEvents(
					SMTaskType.FirstWork, SMTaskRanState.LateUpdate ).Forget();
				_foreverScene._objects.RunAllStateEvents(
					SMTaskType.Work, SMTaskRanState.LateUpdate ).Forget();

				_currentScene._objects.RunAllStateEvents(
					SMTaskType.FirstWork, SMTaskRanState.LateUpdate ).Forget();
				_currentScene._objects.RunAllStateEvents(
					SMTaskType.Work, SMTaskRanState.LateUpdate ).Forget();
			} );

			_finalizeEvent.AddLast( async canceler => {
				await _foreverScene.RunStateEvent( FiniteStateMachineRunState.Exiting );
				Dispose();
			} );

/*
			_disposables.AddFirst(
				Observable.EveryFixedUpdate().Subscribe(
					_ => RunStateEvent( SMTaskRanState.FixedUpdate ).Forget() ),
				Observable.EveryUpdate().Subscribe(
					_ => RunStateEvent( SMTaskRanState.Update ).Forget() ),
				Observable.EveryLateUpdate().Subscribe(
					_ => RunStateEvent( SMTaskRanState.LateUpdate ).Forget() )
			);
*/

#if DEVELOP
			_disposables.AddFirst( _onGUIEvent );
#endif
			_disposables.AddLast(
				Observable.OnceApplicationQuit().Subscribe( _ => SubmarineMirage.DisposeInstance() )
			);

			await RunForeverTasks();
		}

#if DEVELOP
		void OnGUI() {
			if ( !_onGUIEvent._isDispose )	{ _onGUIEvent.Run(); }
		}
#endif

		public override void Create()	{}


		async UniTask RunForeverTasks() {
			await RunStateEvent( SMTaskRanState.Creating );
			await RunStateEvent( SMTaskRanState.Loading );
//			return;
			await RunStateEvent( SMTaskRanState.Initializing );
			await RunActiveEvent();
			Log.Debug( $"{this.GetAboutName()} : 初期化完了", Log.Tag.SMTask );
		}

		async UniTask EndForeverTasks() {
			await _currentScene._fsm.ChangeScene( null );
			await ChangeActive( false );
			await RunStateEvent( SMTaskRanState.Finalizing );
			Log.Debug( $"{this.GetAboutName()} : 破棄完了", Log.Tag.SMTask );
		}
	}
}