//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process.New {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using Singleton.New;
	using FSM.New;
	using MultiEvent;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using Type = ProcessBody.Type;
	using RanState = ProcessBody.RanState;


	// TODO : コメント追加、整頓


	public class ProcessRunner : MonoBehaviourSingleton<ProcessRunner> {
		public ForeverScene _foreverScene => SceneManager.s_instance._fsm._foreverScene;
		public BaseScene _currentScene => SceneManager.s_instance._fsm._scene;
		public bool _isEnterInForever	=> _foreverScene._hierarchies._isEnter;
		public bool _isEnterInScene		=> _currentScene._hierarchies._isEnter;
#if DEVELOP
		public readonly MultiSubject _onGUIEvent = new MultiSubject();
#endif


		new public static void CreateInstance() {
			if ( s_isCreated )	{ return; }

			s_instanceObject = FindObjectOfType<ProcessRunner>();
			if ( s_isCreated )	{ return; }

			s_instanceObject = MonoBehaviourSingletonManager.s_instance.AddComponent<ProcessRunner>();
// TODO : 多分使わないので、未設定でもエラーにならない
//			s_instanceObject._hierarchy = MonoBehaviourSingletonManager.s_instance._hierarchy;
			s_instanceObject.Constructor();

			Log.Debug( $"作成（Component） : {s_instanceObject.GetAboutName()}", Log.Tag.Singleton );
		}


		public async UniTask Create( Func<UniTask> registerProcesses ) {
			_loadEvent.AddLast( async cancel => {
				await registerProcesses();
			} );

			_initializeEvent.AddLast( async cancel => {
// TODO : シーン読込後に、テストオブジェクトを作成してしまう
				await _foreverScene.RunStateEvent( FiniteStateMachineRunState.Entering );
			} );

			_fixedUpdateEvent.AddLast().Subscribe( _ => {
				_foreverScene._hierarchies.RunAllStateEvents( Type.FirstWork, RanState.FixedUpdate ).Forget();
				_foreverScene._hierarchies.RunAllStateEvents( Type.Work, RanState.FixedUpdate ).Forget();

				_currentScene._hierarchies.RunAllStateEvents( Type.FirstWork, RanState.FixedUpdate ).Forget();
				_currentScene._hierarchies.RunAllStateEvents( Type.Work, RanState.FixedUpdate ).Forget();
			} );

			_updateEvent.AddLast().Subscribe( _ => {
				_foreverScene._hierarchies.RunAllStateEvents( Type.FirstWork, RanState.Update ).Forget();
				_foreverScene._hierarchies.RunAllStateEvents( Type.Work, RanState.Update ).Forget();

				_currentScene._hierarchies.RunAllStateEvents( Type.FirstWork, RanState.Update ).Forget();
				_currentScene._hierarchies.RunAllStateEvents( Type.Work, RanState.Update ).Forget();
			} );

			_lateUpdateEvent.AddLast().Subscribe( _ => {
				_foreverScene._hierarchies.RunAllStateEvents( Type.FirstWork, RanState.LateUpdate ).Forget();
				_foreverScene._hierarchies.RunAllStateEvents( Type.Work, RanState.LateUpdate ).Forget();

				_currentScene._hierarchies.RunAllStateEvents( Type.FirstWork, RanState.LateUpdate ).Forget();
				_currentScene._hierarchies.RunAllStateEvents( Type.Work, RanState.LateUpdate ).Forget();
			} );

			_finalizeEvent.AddLast( async cancel => {
				await _foreverScene.RunStateEvent( FiniteStateMachineRunState.Exiting );
				Dispose();
			} );

/*
			_disposables.AddFirst(
				Observable.EveryFixedUpdate().Subscribe(	_ => RunStateEvent( RanState.FixedUpdate ).Forget() ),
				Observable.EveryUpdate().Subscribe(			_ => RunStateEvent( RanState.Update ).Forget() ),
				Observable.EveryLateUpdate().Subscribe(		_ => RunStateEvent( RanState.LateUpdate ).Forget() )
			);
*/

#if DEVELOP
			_disposables.AddFirst( _onGUIEvent );
#endif
			_disposables.AddLast(
				Observable.OnceApplicationQuit().Subscribe( _ => DisposeInstance() )
			);

			await RunForeverHierarchies();
		}

#if DEVELOP
		void OnGUI() {
			if ( !_onGUIEvent._isDispose ) {
				_onGUIEvent.Run();
			}
		}
#endif

		public override void Create() {}


		async UniTask RunForeverHierarchies() {
			await UniTaskUtility.Yield( _activeAsyncCancel );
			await RunStateEvent( RanState.Creating );
			await RunStateEvent( RanState.Loading );
//			return;
			await RunStateEvent( RanState.Initializing );
			await RunActiveEvent();
			Log.Debug( $"{this.GetAboutName()} : 初期化完了", Log.Tag.Process );
		}

		async UniTask EndForeverHierarchies() {
			await _currentScene._fsm.ChangeScene( null );
			await ChangeActive( false );
			await RunStateEvent( RanState.Finalizing );
			Log.Debug( $"{this.GetAboutName()} : 破棄完了", Log.Tag.Process );
		}
	}
}