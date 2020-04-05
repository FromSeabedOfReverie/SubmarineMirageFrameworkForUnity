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

		public string _foreverSceneName => ProcessBody.FOREVER_SCENE_NAME;
		public string _currentSceneName => SceneManager.s_instance._currentSceneName;

		readonly Dictionary< string, Dictionary< Type, List<ProcessHierarchy> > > _processes
			= new Dictionary< string, Dictionary< Type, List<ProcessHierarchy> > >();
		readonly List<ProcessHierarchy> _requestUnregisterProcesses = new List<ProcessHierarchy>();

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
				await RunEventWithFirstProcesses( _foreverSceneName, RanState.Creating );
				await RunEventWithFirstProcesses( _foreverSceneName, RanState.Loading );
				await RunEventWithFirstProcesses( _foreverSceneName, RanState.Initializing );
				await RunEventWithProcesses( _foreverSceneName, RanState.Creating );
				await RunEventWithProcesses( _foreverSceneName, RanState.Loading );
				await RunEventWithProcesses( _foreverSceneName, RanState.Initializing );
			} );

			_enableEvent.AddLast( async cancel => {
				await ChangeActiveWithFirstProcesses( _foreverSceneName, true );
				await ChangeActiveWithProcesses( _foreverSceneName, true );
			} );

			_fixedUpdateEvent.AddLast().Subscribe( _ => {
				RunEventWithFirstProcesses( _foreverSceneName, RanState.FixedUpdate ).Forget();
				RunEventWithProcesses( _foreverSceneName, RanState.FixedUpdate ).Forget();
				RunEventWithFirstProcesses( _currentSceneName, RanState.FixedUpdate ).Forget();
				RunEventWithProcesses( _currentSceneName, RanState.FixedUpdate ).Forget();
			} );

			_updateEvent.AddLast().Subscribe( _ => {
				RunEventWithFirstProcesses( _foreverSceneName, RanState.Update ).Forget();
				RunEventWithProcesses( _foreverSceneName, RanState.Update ).Forget();
				RunEventWithFirstProcesses( _currentSceneName, RanState.Update ).Forget();
				RunEventWithProcesses( _currentSceneName, RanState.Update ).Forget();
			} );

			_lateUpdateEvent.AddLast().Subscribe( _ => {
				RunEventWithFirstProcesses( _foreverSceneName, RanState.LateUpdate ).Forget();
				RunEventWithProcesses( _foreverSceneName, RanState.LateUpdate ).Forget();
				RunEventWithFirstProcesses( _currentSceneName, RanState.LateUpdate ).Forget();
				RunEventWithProcesses( _currentSceneName, RanState.LateUpdate ).Forget();
				CheckUnregisterProcesses();
			} );

			_disableEvent.AddLast( async cancel => {
				await ChangeActiveWithFirstProcesses( _foreverSceneName, false, true );
				await ChangeActiveWithProcesses( _foreverSceneName, false );
			} );

			_finalizeEvent.AddLast( async cancel => {
				await RunEventWithFirstProcesses( _foreverSceneName, RanState.Finalizing, true );
				await RunEventWithProcesses( _foreverSceneName, RanState.Finalizing );
				CheckUnregisterProcesses();
				Dispose();
			} );


			_disposables.AddFirst(
				Observable.EveryFixedUpdate().Subscribe( _ => RunStateEvent( RanState.FixedUpdate ).Forget() ),
				Observable.EveryUpdate().Subscribe( _ => RunStateEvent( RanState.Update ).Forget() ),
				Observable.EveryLateUpdate().Subscribe( _ => RunStateEvent( RanState.LateUpdate ).Forget() )
			);


#if DEVELOP && false
			_lateUpdateEvent.AddLast().Subscribe( _ => {
				DebugDisplay.s_instance.Add( Color.cyan );
				DebugDisplay.s_instance.Add( $"● {this.GetAboutName()}" );
				DebugDisplay.s_instance.Add( Color.white );
				GetAllProcesses().ForEach( p => DebugDisplay.s_instance.Add( $"\t{p.GetAboutName()}" ) );
			} );
#endif
///*
			_disposables.AddFirst(
				Observable.EveryUpdate()
					.Where( _ => Input.GetKeyDown( KeyCode.Return ) )
					.Take( 1 )
					.Subscribe( _ => {
						Log.Debug( "KeyDown Return" );
						DeleteForeverProcesses().Forget();
					} )
			);
//*/
			_loadEvent.AddFirst(	async cancel => Log.Debug( $"{this.GetAboutName()}._loadEvent start" ) );
			_loadEvent.AddLast(		async cancel => Log.Debug( $"{this.GetAboutName()}._loadEvent end" ) );

			_initializeEvent.AddFirst(	async cancel => Log.Debug( $"{this.GetAboutName()}._initializeEvent start" ) );
			_initializeEvent.AddLast(	async cancel => Log.Debug( $"{this.GetAboutName()}._initializeEvent end" ) );

			_enableEvent.AddFirst(	async cancel => Log.Debug( $"{this.GetAboutName()}._enableEvent start" ) );
			_enableEvent.AddLast(	async cancel => Log.Debug( $"{this.GetAboutName()}._enableEvent end" ) );

			_fixedUpdateEvent.AddFirst().Subscribe(	_ => Log.Debug( $"{this.GetAboutName()}._fixedUpdateEvent start" ) );
			_fixedUpdateEvent.AddLast().Subscribe(	_ => Log.Debug( $"{this.GetAboutName()}._fixedUpdateEvent end" ) );

			_updateEvent.AddFirst().Subscribe(	_ => Log.Debug( $"{this.GetAboutName()}._updateEvent start" ) );
			_updateEvent.AddLast().Subscribe(	_ => Log.Debug( $"{this.GetAboutName()}._updateEvent end" ) );

			_lateUpdateEvent.AddFirst().Subscribe(	_ => Log.Debug( $"{this.GetAboutName()}._lateUpdateEvent start" ) );
			_lateUpdateEvent.AddLast().Subscribe(	_ => Log.Debug( $"{this.GetAboutName()}._lateUpdateEvent end" ) );

			_disableEvent.AddFirst(	async cancel => Log.Debug( $"{this.GetAboutName()}._disableEvent start" ) );
			_disableEvent.AddLast(	async cancel => Log.Debug( $"{this.GetAboutName()}._disableEvent end" ) );

			_finalizeEvent.AddFirst(	async cancel => Log.Debug( $"{this.GetAboutName()}._finalizeEvent start" ) );
			_finalizeEvent.AddLast(		async cancel => Log.Debug( $"{this.GetAboutName()}._finalizeEvent end" ) );


#if DEVELOP
			_disposables.AddFirst( _onGUIEvent );
#endif
			_disposables.AddFirst( GetAllProcesses() );
			_disposables.AddFirst( () => {
				_processes.Clear();
				_requestUnregisterProcesses.Clear();
			} );

			Observable.OnceApplicationQuit().Subscribe( _ => DisposeInstance() );


//			await UniTaskUtility.Yield( _activeAsyncCancel );
			await RunForeverProcesses();
		}

#if DEVELOP
		void OnGUI() => _onGUIEvent.Run();
#endif


		public override void Create() {
			Log.Debug( $"{this.GetAboutName()}.Create()" );
		}


		void CreateProcesses( string sceneName ) {
			if ( !_processes.ContainsKey( sceneName ) ) {
				_processes[sceneName] = new Dictionary< Type, List<ProcessHierarchy> >();
			}
			var scenePS = _processes[sceneName];
			if ( !scenePS.ContainsKey( Type.FirstWork ) ) {
				scenePS[Type.FirstWork] = new List<ProcessHierarchy>();
			}
			if ( !scenePS.ContainsKey( Type.Work ) ) {
				scenePS[Type.Work] = new List<ProcessHierarchy>();
			}
		}

		List<ProcessHierarchy> GetProcesses( string sceneName, Type type, bool isReverse = false ) {
			CreateProcesses( sceneName );
			var ps = _processes[sceneName][type];
			if ( isReverse ) {
// TODO : 元のリストは、元のまま、入れ替わってないか？
				Log.Debug( $"Work 逆転前\n{_process.ToDeepString()}" );
				ps = ps.ReverseByClone();
				Log.Debug( $"Work 逆転前\n{_process.ToDeepString()}" );
			}
// TODO : 使用先で、Add、Removeされた場合、元もちゃんと変更されるか？
			return ps;
		}

		IEnumerable<ProcessHierarchy> GetAllProcesses() {
			return _processes
				.SelectMany( pair => pair.Value )
				.SelectMany( pair => pair.Value );
		}


// TODO : RunStateEventの、子変更オプションを、finalizeに適用させる
		async UniTask RunEventWithFirstProcesses( string sceneName, RanState state, bool isReverse = false ) {
			var ps = GetProcesses( sceneName, Type.FirstWork, isReverse );
			foreach ( var p in ps ) {
				await p.RunStateEvent( state );
			}
		}

		async UniTask RunEventWithProcesses( string sceneName, RanState state ) {
			var ps = GetProcesses( sceneName, Type.Work );
			await UniTask.WhenAll(
				ps.Select( async p => await p.RunStateEvent( state ) )
			);
		}

		async UniTask ChangeActiveWithFirstProcesses( string sceneName, bool isActive, bool isReverse = false ) {
			var ps = GetProcesses( sceneName, Type.FirstWork, isReverse );
			foreach ( var p in ps ) {
				await p.ChangeActive( isActive, true );
			}
		}

		async UniTask ChangeActiveWithProcesses( string sceneName, bool isActive ) {
			var ps = GetProcesses( sceneName, Type.Work );
			await UniTask.WhenAll(
				ps.Select( async p => await p.ChangeActive( isActive, true ) )
			);
		}


		public async UniTask Register( ProcessHierarchy process ) {
			GetProcesses( process._belongSceneName, process._type ).Add( process );

			if ( _isInitializedInSceneProcesses ) {
				await UniTaskUtility.Yield( _activeAsyncCancel );
				await process.RunStateEvent( RanState.Creating );
				await process.RunStateEvent( RanState.Loading );
				await process.RunStateEvent( RanState.Initializing );
// TODO : 初期化時のChangeActiveは、RunActiveEventを呼ぶよう、変更（活動状態が分からない為）
				await process.RunActiveEvent();
			}
		}

		public void Unregister( ProcessHierarchy process ) {
			_requestUnregisterProcesses.Add( process );
		}

		void CheckUnregisterProcesses() {
			if ( _requestUnregisterProcesses.IsEmpty() )	{ return; }
			_requestUnregisterProcesses
				.ForEach( p => GetProcesses(　p._belongSceneName, p._type　).Remove( p ) );
			_requestUnregisterProcesses.Clear();
		}


// TODO : 削除時に、ゲーム物に2つ以上Processがくっついている場合を考慮し、指定するのはゲーム物にして、
//			一々GetComponentsで取得走査し、全部Finalize後に、ゲーム物を削除させる
		public async UniTask Delete( ProcessHierarchy process ) {
			await process.ChangeActive( false, true );
			await process.RunStateEvent( RanState.Finalizing );
		}

		public async UniTask ChangeActive( ProcessHierarchy process, bool isActive ) {
			await process.ChangeActive( isActive, true );
		}


		async UniTask RunForeverProcesses() {
			await UniTaskUtility.WaitWhile( _activeAsyncCancel, () => _process._ranState != RanState.Created );
			await RunStateEvent( RanState.Loading );
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
			await RunEventWithFirstProcesses( _currentSceneName, RanState.Creating );
			await RunEventWithFirstProcesses( _currentSceneName, RanState.Loading );
			await RunEventWithFirstProcesses( _currentSceneName, RanState.Initializing );
			await RunEventWithProcesses( _currentSceneName, RanState.Creating );
			await RunEventWithProcesses( _currentSceneName, RanState.Loading );
			await RunEventWithProcesses( _currentSceneName, RanState.Initializing );

			await ChangeActiveWithFirstProcesses( _currentSceneName, true );
			await ChangeActiveWithProcesses( _currentSceneName, true );

			_isInitializedInSceneProcesses = true;
		}

		public async UniTask DeleteSceneProcesses() {
			await ChangeActiveWithProcesses( _currentSceneName, false );
			await ChangeActiveWithFirstProcesses( _currentSceneName, false, true );

			await RunEventWithProcesses( _currentSceneName, RanState.Finalizing );
			await RunEventWithFirstProcesses( _currentSceneName, RanState.Finalizing, true );

			_isInitializedInSceneProcesses = false;
		}
	}
}