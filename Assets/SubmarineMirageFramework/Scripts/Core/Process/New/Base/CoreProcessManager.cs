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


	public class CoreProcessManager : MonoBehaviourSingleton<CoreProcessManager> {
		public override Type _type => Type.DontWork;

		public string _foreverSceneName => SceneStateMachine.FOREVER_SCENE_NAME;
		public string _currentSceneName => SceneManager.s_instance._currentSceneName;

		readonly Dictionary< string, Dictionary< Type, List<ProcessHierarchy> > > _hierarchies
			= new Dictionary< string, Dictionary< Type, List<ProcessHierarchy> > >();
		readonly List<ProcessHierarchy> _requestUnregisterHierarchies = new List<ProcessHierarchy>();

#if DEVELOP
		public readonly MultiSubject _onGUIEvent = new MultiSubject();
#endif
		bool _isInitializedInSceneProcesses;


		public async UniTask Create( Func<UniTask> initializePlugin, Func<UniTask> registerProcesses ) {
			_loadEvent.AddLast( async cancel => {
				await initializePlugin();
				await registerProcesses();
			} );

			_initializeEvent.AddLast( async cancel => {
				await RunStateEventWithProcesses( _foreverSceneName, Type.FirstWork, RanState.Creating );
				await RunStateEventWithProcesses( _foreverSceneName, Type.FirstWork, RanState.Loading );
				await RunStateEventWithProcesses( _foreverSceneName, Type.FirstWork, RanState.Initializing );

				await RunStateEventWithProcesses( _foreverSceneName, Type.Work, RanState.Creating );
				await RunStateEventWithProcesses( _foreverSceneName, Type.Work, RanState.Loading );
				await RunStateEventWithProcesses( _foreverSceneName, Type.Work, RanState.Initializing );
			} );

			_enableEvent.AddLast( async cancel => {
				await RunActiveEventWithProcesses( _foreverSceneName, Type.FirstWork );
				await RunActiveEventWithProcesses( _foreverSceneName, Type.Work );
			} );

			_fixedUpdateEvent.AddLast().Subscribe( _ => {
				RunStateEventWithProcesses( _foreverSceneName, Type.FirstWork, RanState.FixedUpdate ).Forget();
				RunStateEventWithProcesses( _foreverSceneName, Type.Work, RanState.FixedUpdate ).Forget();

				RunStateEventWithProcesses( _currentSceneName, Type.FirstWork, RanState.FixedUpdate ).Forget();
				RunStateEventWithProcesses( _currentSceneName, Type.Work, RanState.FixedUpdate ).Forget();
			} );

			_updateEvent.AddLast().Subscribe( _ => {
				RunStateEventWithProcesses( _foreverSceneName, Type.FirstWork, RanState.Update ).Forget();
				RunStateEventWithProcesses( _foreverSceneName, Type.Work, RanState.Update ).Forget();

				RunStateEventWithProcesses( _currentSceneName, Type.FirstWork, RanState.Update ).Forget();
				RunStateEventWithProcesses( _currentSceneName, Type.Work, RanState.Update ).Forget();
			} );

			_lateUpdateEvent.AddLast().Subscribe( _ => {
				RunStateEventWithProcesses( _foreverSceneName, Type.FirstWork, RanState.LateUpdate ).Forget();
				RunStateEventWithProcesses( _foreverSceneName, Type.Work, RanState.LateUpdate ).Forget();

				RunStateEventWithProcesses( _currentSceneName, Type.FirstWork, RanState.LateUpdate ).Forget();
				RunStateEventWithProcesses( _currentSceneName, Type.Work, RanState.LateUpdate ).Forget();
				CheckUnregisterHierarchies();
			} );

			_disableEvent.AddLast( async cancel => {
				await ChangeActiveWithProcesses( _foreverSceneName, Type.Work, false );
				await ChangeActiveWithProcesses( _foreverSceneName, Type.FirstWork, false );
			} );

			_finalizeEvent.AddLast( async cancel => {
				await RunStateEventWithProcesses( _foreverSceneName, Type.Work, RanState.Finalizing );
				await RunStateEventWithProcesses( _foreverSceneName, Type.FirstWork, RanState.Finalizing );
				CheckUnregisterHierarchies();
				Dispose();
			} );


			_disposables.AddFirst(
				Observable.EveryFixedUpdate().Subscribe(	_ => RunStateEvent( RanState.FixedUpdate ).Forget() ),
				Observable.EveryUpdate().Subscribe(			_ => RunStateEvent( RanState.Update ).Forget() ),
				Observable.EveryLateUpdate().Subscribe(		_ => RunStateEvent( RanState.LateUpdate ).Forget() )
			);

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

//			await UniTaskUtility.Yield( _activeAsyncCancel );
			await RunForeverProcesses();

			await UniTaskUtility.DontWait();
		}

#if DEVELOP
		void OnGUI() => _onGUIEvent.Run();
#endif


		public override void Create()
			=> Log.Debug( $"{this.GetAboutName()}.Create()" );


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
		}

		List<ProcessHierarchy> GetHierarchies( string sceneName, Type type, bool isReverse = false ) {
			CreateHierarchies( sceneName );
			var hs = _hierarchies[sceneName][type];
			if ( isReverse ) {
// TODO : 元のリストは、元のまま、入れ替わってないか？
				Log.Debug( $"Work 逆転前\n{_body}" );
				hs = hs.ReverseByClone();
				Log.Debug( $"Work 逆転前\n{_body}" );
			}
// TODO : 使用先で、Add、Removeされた場合、元もちゃんと変更されるか？
			return hs;
		}


// TODO : RunStateEventの、子変更オプションを、finalizeに適用させる
		async UniTask RunStateEventWithProcesses( string sceneName, Type type, RanState state ) {
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

		async UniTask ChangeActiveWithProcesses( string sceneName, Type type, bool isActive ) {
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

		async UniTask RunActiveEventWithProcesses( string sceneName, Type type ) {
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


		public async UniTask Register( ProcessHierarchy hierarchy ) {
			if ( hierarchy._owner != null ) {
				if ( hierarchy._lifeSpan == ProcessBody.LifeSpan.Forever ) {
					DontDestroyOnLoad( hierarchy._owner );
				} else {
// TODO : 再登録時に、Forever解除に使う
//					SceneManager.s_instance.DestroyOnLoad( hierarchy._owner );
				}
			}

			switch ( hierarchy._type ) {
				case Type.DontWork:
					await UniTaskUtility.Yield( _activeAsyncCancel );
					await hierarchy.RunStateEvent( RanState.Creating );
					return;

				case Type.Work:
				case Type.FirstWork:
					GetHierarchies( hierarchy._belongSceneName, hierarchy._type )
						.Add( hierarchy );

					if ( _isInitializedInSceneProcesses ) {
						await UniTaskUtility.Yield( _activeAsyncCancel );
						await hierarchy.RunStateEvent( RanState.Creating );
						await hierarchy.RunStateEvent( RanState.Loading );
						await hierarchy.RunStateEvent( RanState.Initializing );
						await hierarchy.RunActiveEvent();
					}
					return;
			}
		}

		public void Unregister( ProcessHierarchy hierarchy ) {
// TODO : Modefilerを作成し、追加、削除等をまとめて、ループ走査中以外の時に、実行する
			_requestUnregisterHierarchies.Add( hierarchy );
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


		public async UniTask Delete( ProcessHierarchy hierarchy ) {
// TODO : Hierarchy内部を整理
			await hierarchy.Destroy();
		}

		public async UniTask ChangeActive( ProcessHierarchy hierarchy, bool isActive ) {
			await hierarchy.ChangeActive( isActive, true );
		}


		async UniTask RunForeverProcesses() {
			await UniTaskUtility.WaitWhile( _activeAsyncCancel, () => _body._ranState != RanState.Created );
			await RunStateEvent( RanState.Loading );
			return;
			await RunStateEvent( RanState.Initializing );
			await ChangeActive( true );
			Log.Debug( $"{this.GetAboutName()} : 初期化完了", Log.Tag.Process );
		}

		async UniTask DeleteForeverProcesses() {
			await DeleteSceneProcesses();
			await ChangeActive( false );
			await RunStateEvent( RanState.Finalizing );
			Log.Debug( $"{this.GetAboutName()} : 破棄完了", Log.Tag.Process );
		}


		public async UniTask RunSceneProcesses() {
			await RunStateEventWithProcesses( _currentSceneName, Type.FirstWork, RanState.Creating );
			await RunStateEventWithProcesses( _currentSceneName, Type.FirstWork, RanState.Loading );
			await RunStateEventWithProcesses( _currentSceneName, Type.FirstWork, RanState.Initializing );

			await RunStateEventWithProcesses( _currentSceneName, Type.Work, RanState.Creating );
			await RunStateEventWithProcesses( _currentSceneName, Type.Work, RanState.Loading );
			await RunStateEventWithProcesses( _currentSceneName, Type.Work, RanState.Initializing );

			await RunActiveEventWithProcesses( _currentSceneName, Type.FirstWork );
			await RunActiveEventWithProcesses( _currentSceneName, Type.Work );

			_isInitializedInSceneProcesses = true;
		}

		public async UniTask DeleteSceneProcesses() {
			await ChangeActiveWithProcesses( _currentSceneName, Type.Work, false );
			await ChangeActiveWithProcesses( _currentSceneName, Type.FirstWork, false );

			await RunStateEventWithProcesses( _currentSceneName, Type.Work, RanState.Finalizing );
			await RunStateEventWithProcesses( _currentSceneName, Type.FirstWork, RanState.Finalizing );

			GetHierarchies( _currentSceneName, Type.Work ).Clear();
			GetHierarchies( _currentSceneName, Type.FirstWork ).Clear();

			_isInitializedInSceneProcesses = false;
		}
	}
}