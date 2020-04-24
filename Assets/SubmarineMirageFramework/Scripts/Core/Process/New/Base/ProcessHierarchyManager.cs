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
	using MultiEvent;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using Type = ProcessBody.Type;
	using RanState = ProcessBody.RanState;


	// TODO : コメント追加、整頓


	public class ProcessHierarchyManager : MonoBehaviourSingleton<ProcessHierarchyManager> {
		public string _foreverSceneName => SceneStateMachine.FOREVER_SCENE_NAME;
		public string _currentSceneName => SceneManager.s_instance._currentSceneName;

		public readonly Dictionary< string, Dictionary< Type, List<ProcessHierarchy> > > _hierarchies
			= new Dictionary< string, Dictionary< Type, List<ProcessHierarchy> > >();
		readonly List<ProcessHierarchy> _requestUnregisterHierarchies = new List<ProcessHierarchy>();

#if DEVELOP
		public readonly MultiSubject _onGUIEvent = new MultiSubject();
#endif
		public bool _isInitializedInScene	{ get; private set; }


		new public static void CreateInstance() {
			if ( s_isCreated )	{ return; }

			s_instanceObject = FindObjectOfType<ProcessHierarchyManager>();
			if ( s_isCreated )	{ return; }

			s_instanceObject = MonoBehaviourSingletonManager.s_instance.AddComponent<ProcessHierarchyManager>();
			s_instanceObject.Constructor();

			Log.Debug( $"作成（Component） : {s_instanceObject.GetAboutName()}", Log.Tag.Singleton );
		}


		public async UniTask Create( Func<UniTask> registerProcesses ) {
			Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Return ) ).Subscribe( _ => {
				_hierarchies.SelectMany( pair => pair.Value ).SelectMany( pair => pair.Value )
					.ForEach( h => Log.Debug( $"{h._processes.FirstOrDefault()?.GetAboutName()} {h._owner}" ) );
			} );

			_loadEvent.AddLast( async cancel => {
				await registerProcesses();
			} );

			_initializeEvent.AddLast( async cancel => {
				await RunAllStateEvents( _foreverSceneName, Type.FirstWork, RanState.Creating );
				await RunAllStateEvents( _foreverSceneName, Type.FirstWork, RanState.Loading );
				await RunAllStateEvents( _foreverSceneName, Type.FirstWork, RanState.Initializing );

				await RunAllStateEvents( _foreverSceneName, Type.Work, RanState.Creating );
				await RunAllStateEvents( _foreverSceneName, Type.Work, RanState.Loading );
				await RunAllStateEvents( _foreverSceneName, Type.Work, RanState.Initializing );
			} );

			_enableEvent.AddLast( async cancel => {
				await RunAllActiveEvents( _foreverSceneName, Type.FirstWork );
				await RunAllActiveEvents( _foreverSceneName, Type.Work );
			} );

			_fixedUpdateEvent.AddLast().Subscribe( _ => {
				RunAllStateEvents( _foreverSceneName, Type.FirstWork, RanState.FixedUpdate ).Forget();
				RunAllStateEvents( _foreverSceneName, Type.Work, RanState.FixedUpdate ).Forget();

				RunAllStateEvents( _currentSceneName, Type.FirstWork, RanState.FixedUpdate ).Forget();
				RunAllStateEvents( _currentSceneName, Type.Work, RanState.FixedUpdate ).Forget();
			} );

			_updateEvent.AddLast().Subscribe( _ => {
				RunAllStateEvents( _foreverSceneName, Type.FirstWork, RanState.Update ).Forget();
				RunAllStateEvents( _foreverSceneName, Type.Work, RanState.Update ).Forget();

				RunAllStateEvents( _currentSceneName, Type.FirstWork, RanState.Update ).Forget();
				RunAllStateEvents( _currentSceneName, Type.Work, RanState.Update ).Forget();
			} );

			_lateUpdateEvent.AddLast().Subscribe( _ => {
				RunAllStateEvents( _foreverSceneName, Type.FirstWork, RanState.LateUpdate ).Forget();
				RunAllStateEvents( _foreverSceneName, Type.Work, RanState.LateUpdate ).Forget();

				RunAllStateEvents( _currentSceneName, Type.FirstWork, RanState.LateUpdate ).Forget();
				RunAllStateEvents( _currentSceneName, Type.Work, RanState.LateUpdate ).Forget();
				CheckUnregisterHierarchies();
			} );

			_disableEvent.AddLast( async cancel => {
				await ChangeAllActives( _foreverSceneName, Type.Work, false );
				await ChangeAllActives( _foreverSceneName, Type.FirstWork, false );
			} );

			_finalizeEvent.AddLast( async cancel => {
				await RunAllStateEvents( _foreverSceneName, Type.Work, RanState.Finalizing );
				await RunAllStateEvents( _foreverSceneName, Type.FirstWork, RanState.Finalizing );
				CheckUnregisterHierarchies();
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
			_disposables.AddFirst( () => {
				_hierarchies
					.SelectMany( pair => pair.Value )
					.SelectMany( pair => pair.Value )
					.ForEach( h => h.Dispose() );
				_hierarchies.Clear();
				_requestUnregisterHierarchies.Clear();
			} );

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


		void CreateHierarchies( string sceneName ) {
			if ( !_hierarchies.ContainsKey( sceneName ) ) {
				_hierarchies[sceneName] = new Dictionary< Type, List<ProcessHierarchy> >();
			}
			var sceneHS = _hierarchies[sceneName];
			if ( !sceneHS.ContainsKey( Type.FirstWork ) ) {
				sceneHS[Type.FirstWork] = new List<ProcessHierarchy>();
			}
			if ( !sceneHS.ContainsKey( Type.Work ) ) {
				sceneHS[Type.Work] = new List<ProcessHierarchy>();
			}
			if ( !sceneHS.ContainsKey( Type.DontWork ) ) {
				sceneHS[Type.DontWork] = new List<ProcessHierarchy>();
			}
		}

		List<ProcessHierarchy> GetHierarchies( string sceneName, Type type, bool isReverse = false ) {
			CreateHierarchies( sceneName );
			var hs = _hierarchies[sceneName][type];
			if ( isReverse ) {
				Log.Debug(
					$"befor reverse : " +
					$"{string.Join( ", ", _hierarchies[sceneName][type].Select( h => h._processes.First().GetAboutName() ) )}"
				);
				hs = hs.ReverseByClone();
				Log.Debug(
					$"new : " +
					$"{string.Join( ", ", hs.Select( h => h._processes.First().GetAboutName() ) )}"
				);
				Log.Debug(
					$"after reverse : " +
					$"{string.Join( ", ", _hierarchies[sceneName][type].Select( h => h._processes.First().GetAboutName() ) )}"
				);
			}
// TODO : 使用先で、Add、Removeされた場合、元もちゃんと変更されるか？
			return hs;
		}


		async UniTask RunAllStateEvents( string sceneName, Type type, RanState state ) {
			var hs = GetHierarchies( sceneName, type, type == Type.FirstWork && state == RanState.Finalizing );
			switch ( type ) {
				case Type.FirstWork:
					foreach ( var h in hs ) {
						await h.RunStateEvent( state );
					}
					return;

				case Type.Work:
					await UniTask.WhenAll(
						hs.Select( h => h.RunStateEvent( state ) )
					);
					return;
			}
		}

		async UniTask ChangeAllActives( string sceneName, Type type, bool isActive ) {
			var hs = GetHierarchies( sceneName, type, type == Type.FirstWork && !isActive );
			switch ( type ) {
				case Type.FirstWork:
					foreach ( var h in hs ) {
						await h.ChangeActive( isActive, true );
					}
					return;

				case Type.Work:
					await UniTask.WhenAll(
						hs.Select( h => h.ChangeActive( isActive, true ) )
					);
					return;
			}
		}

		async UniTask RunAllActiveEvents( string sceneName, Type type ) {
			var hs = GetHierarchies( sceneName, type );
			switch ( type ) {
				case Type.FirstWork:
					foreach ( var h in hs ) {
						await h.RunActiveEvent();
					}
					return;

				case Type.Work:
					await UniTask.WhenAll(
						hs.Select( h => h.RunActiveEvent() )
					);
					return;
			}
		}


		public async UniTask Register( ProcessHierarchy top ) {
			Log.Debug( $"register {top._owner}" );
			if ( top._owner != null ) {
				if ( top._lifeSpan == ProcessBody.LifeSpan.Forever ) {
					DontDestroyOnLoad( top._owner );
				} else {
// TODO : 再登録時に、Forever解除に使う
//					SceneManager.s_instance.DestroyOnLoad( top._owner );
				}
			}

			GetHierarchies( top._belongSceneName, top._type )
				.Add( top );

			switch ( top._type ) {
				case Type.DontWork:
					await UniTaskUtility.Yield( _activeAsyncCancel );
					await top.RunStateEvent( RanState.Creating );
					return;
				case Type.Work:
				case Type.FirstWork:
					if ( _isInitializedInScene ) {
						await UniTaskUtility.Yield( _activeAsyncCancel );
						await top.RunStateEvent( RanState.Creating );
						await top.RunStateEvent( RanState.Loading );
						await top.RunStateEvent( RanState.Initializing );
						await top.RunActiveEvent();
					}
					return;
			}
		}

		public void Unregister( ProcessHierarchy top ) {
// TODO : Modefilerを作成し、追加、削除等をまとめて、ループ走査中以外の時に、実行する
			_requestUnregisterHierarchies.Add( top );
		}

		void CheckUnregisterHierarchies() {
			if ( _requestUnregisterHierarchies.IsEmpty() )	{ return; }
			_requestUnregisterHierarchies.ForEach( h => {
				GetHierarchies(　h._belongSceneName, h._type　).Remove( h );
			} );
			_requestUnregisterHierarchies.Clear();
		}

		public void ReRegister() {
		}


		public async UniTask Delete( ProcessHierarchy top ) {
// TODO : Hierarchy内部を整理
			await top.Delete();
		}

		public async UniTask ChangeActive( ProcessHierarchy top, bool isActive ) {
			await top.ChangeActive( isActive, true );
		}


		async UniTask RunForeverHierarchies() {
//			await UniTaskUtility.Yield( _activeAsyncCancel );
			await RunStateEvent( RanState.Creating );
			await RunStateEvent( RanState.Loading );

			GetHierarchies( _foreverSceneName, Type.DontWork, true )
				.ForEach( h => Log.Debug( h ) );
			return;
			await RunStateEvent( RanState.Initializing );
			await RunActiveEvent();
			Log.Debug( $"{this.GetAboutName()} : 初期化完了", Log.Tag.Process );
		}

		async UniTask DeleteForeverHierarchies() {
			await DeleteSceneHierarchies();
			await ChangeActive( false );
			await RunStateEvent( RanState.Finalizing );
			Log.Debug( $"{this.GetAboutName()} : 破棄完了", Log.Tag.Process );
		}


		public async UniTask RunSceneHierarchies() {
			await RunAllStateEvents( _currentSceneName, Type.FirstWork, RanState.Creating );
			await RunAllStateEvents( _currentSceneName, Type.FirstWork, RanState.Loading );
			await RunAllStateEvents( _currentSceneName, Type.FirstWork, RanState.Initializing );

			await RunAllStateEvents( _currentSceneName, Type.Work, RanState.Creating );
			await RunAllStateEvents( _currentSceneName, Type.Work, RanState.Loading );
			await RunAllStateEvents( _currentSceneName, Type.Work, RanState.Initializing );

			await RunAllActiveEvents( _currentSceneName, Type.FirstWork );
			await RunAllActiveEvents( _currentSceneName, Type.Work );

			_isInitializedInScene = true;
		}

		public async UniTask DeleteSceneHierarchies() {
			await ChangeAllActives( _currentSceneName, Type.Work, false );
			await ChangeAllActives( _currentSceneName, Type.FirstWork, false );

			await RunAllStateEvents( _currentSceneName, Type.Work, RanState.Finalizing );
			await RunAllStateEvents( _currentSceneName, Type.FirstWork, RanState.Finalizing );

			GetHierarchies( _currentSceneName, Type.Work ).Clear();
			GetHierarchies( _currentSceneName, Type.FirstWork ).Clear();

			var hs = GetHierarchies( _currentSceneName, Type.DontWork );
			hs.ForEach( h => h.Dispose() );
			hs.Clear();

			_isInitializedInScene = false;
		}
	}
}