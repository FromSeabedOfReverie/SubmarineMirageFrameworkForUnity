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
		public override ProcessBody.Type _type => ProcessBody.Type.DontWork;

		readonly Dictionary< string, Dictionary< ProcessBody.Type, List<IProcess> > > _processes
			= new Dictionary< string, Dictionary< ProcessBody.Type, List<IProcess> > >();
		readonly List<IProcess> _deleteProcesses = new List<IProcess>();
		bool _isProcessDeleting;

#if DEVELOP
		public readonly MultiSubject _onGUIEvent = new MultiSubject();
#endif
		CompositeDisposable _updateDisposer = new CompositeDisposable();
		string _foreverSceneName = ProcessBody.LifeSpan.Forever.ToString();


		public async UniTask Create( Func<UniTask> initializePlugin, Func<UniTask> registerProcesses ) {
			await initializePlugin();
			await registerProcesses();
			RegisterEvents();

			Create();
			await RunStateEvent( ProcessBody.RanState.Loading );
			await RunStateEvent( ProcessBody.RanState.Initializing );

			Log.Debug( $"{this.GetAboutName()} : 初期化完了", Log.Tag.Process );
/*
			Observable.EveryUpdate()
				.Where( _ => Input.GetKeyDown( KeyCode.Return ) )
				.Take( 1 )
				.Subscribe( _ => Clear().Forget() )
				.AddTo( _updateDisposer );
*/
		}


		public override void Create() {}


		async UniTask RunEventWithFirstProcesses( string sceneName, ProcessBody.RanState state,
												bool isReverse = false
		) {
			var ps = _processes
				.GetOrDefault( sceneName )
				?.GetOrDefault( ProcessBody.Type.FirstWork, new List<IProcess>() );
			if ( isReverse ) {
// TODO : 本当に元のリストが、入れ替わってないか、要検証
				ps = ps.AsEnumerable().Reverse().ToList();
			}
			foreach ( var p in ps ) {
				await p.RunStateEvent( state );
			}
		}

		async UniTask RunEventWithProcesses( string sceneName, ProcessBody.RanState state ) {
			var ps = _processes
				.GetOrDefault( sceneName )
				?.GetOrDefault( ProcessBody.Type.Work, new List<IProcess>() );
			await UniTask.WhenAll(
				ps.Select( async p => await p.RunStateEvent( state ) )
			);
		}

		async UniTask ChangeActiveWithFirstProcesses( string sceneName, bool isActive, bool isReverse = false
		) {
			var ps = _processes
				.GetOrDefault( sceneName )
				?.GetOrDefault( ProcessBody.Type.FirstWork, new List<IProcess>() );
			if ( isReverse ) {
// TODO : 本当に元のリストが、入れ替わってないか、要検証
				ps = ps.AsEnumerable().Reverse().ToList();
			}
			foreach ( var p in ps ) {
				await p.ChangeActive( isActive );
			}
		}

		async UniTask ChangeActiveWithProcesses( string sceneName, bool isActive ) {
			var ps = _processes
				.GetOrDefault( sceneName )
				?.GetOrDefault( ProcessBody.Type.Work, new List<IProcess>() );
			await UniTask.WhenAll(
				ps.Select( async p => await p.ChangeActive( isActive ) )
			);
		}


		void RegisterEvents() {
			_loadEvent.AddLast( async cancel => {
				await RunEventWithFirstProcesses( _foreverSceneName, ProcessBody.RanState.Loading );
				await RunEventWithFirstProcesses( _foreverSceneName, ProcessBody.RanState.Initializing );
			} );

			_initializeEvent.AddLast( async cancel => {
				await RunEventWithProcesses( _foreverSceneName, ProcessBody.RanState.Loading );
				await RunEventWithProcesses( _foreverSceneName, ProcessBody.RanState.Initializing );
			} );

			_enableEvent.AddLast( async cancel => {
				await ChangeActiveWithFirstProcesses( _foreverSceneName, true );
				await ChangeActiveWithProcesses( _foreverSceneName, true );
			} );

			_fixedUpdateEvent.AddLast().Subscribe( _ => {
				RunEventWithFirstProcesses( _foreverSceneName, ProcessBody.RanState.FixedUpdate ).Forget();
				RunEventWithProcesses( _foreverSceneName, ProcessBody.RanState.FixedUpdate ).Forget();
			} );

			_updateEvent.AddLast().Subscribe( _ => {
				RunEventWithFirstProcesses( _foreverSceneName, ProcessBody.RanState.Update ).Forget();
				RunEventWithProcesses( _foreverSceneName, ProcessBody.RanState.Update ).Forget();
			} );

			_lateUpdateEvent.AddLast().Subscribe( _ => {
				RunEventWithFirstProcesses( _foreverSceneName, ProcessBody.RanState.LateUpdate ).Forget();
				RunEventWithProcesses( _foreverSceneName, ProcessBody.RanState.LateUpdate ).Forget();
				CheckDeleteProcesses().Forget();
#if DEVELOP
				DebugDisplay.s_instance.Add( Color.cyan );
				DebugDisplay.s_instance.Add( $"● {this.GetAboutName()}" );
				DebugDisplay.s_instance.Add( Color.white );
				_processes
					.SelectMany( pair => pair.Value )
					.SelectMany( pair => pair.Value )
					.ForEach( p => DebugDisplay.s_instance.Add( $"\t{p.GetAboutName()}" ) );
#endif
			} );

			_disableEvent.AddLast( async cancel => {
				await ChangeActiveWithFirstProcesses( _foreverSceneName, false, true );
				await ChangeActiveWithProcesses( _foreverSceneName, false );
			} );

			_finalizeEvent.AddLast( async cancel => {
				await RunEventWithFirstProcesses( _foreverSceneName, ProcessBody.RanState.Finalizing, true );
				await RunEventWithProcesses( _foreverSceneName, ProcessBody.RanState.Finalizing );
				_processes
					.GetOrDefault( _foreverSceneName )
					?.Where( pair => pair.Key != ProcessBody.Type.DontWork )
					.SelectMany( pair => pair.Value )
					.ForEach( p => UnRegister( p ) );
				await CheckDeleteProcesses();
			} );

			Observable.OnceApplicationQuit().Subscribe(
				async _ => await RunStateEvent( ProcessBody.RanState.Finalizing )
			)
			.AddTo( _updateDisposer );

			Observable.EveryFixedUpdate().Subscribe( _ =>
				RunStateEvent( ProcessBody.RanState.FixedUpdate ).Forget()
			)
			.AddTo( _updateDisposer );

			Observable.EveryUpdate().Subscribe( _ =>
				RunStateEvent( ProcessBody.RanState.Update ).Forget()
			)
			.AddTo( _updateDisposer );
			Observable.EveryLateUpdate().Subscribe( _ =>
				RunStateEvent( ProcessBody.RanState.LateUpdate ).Forget()
			)
			.AddTo( _updateDisposer );
		}


#if DEVELOP
		void OnGUI() {
//			if ( _process._ranState == ProcessBody.RanState.LateUpdate ) {
				_onGUIEvent.Invoke();
//			}
		}
#endif


		public async UniTask Register( IProcess process ) {
			await process.RunStateEvent( ProcessBody.RanState.Create );
			if ( process._type == ProcessBody.Type.DontWork ) {
				return;
			}

			var name = process._lifeSpan == ProcessBody.LifeSpan.Forever ? _foreverSceneName
				: SceneManager.GetActiveScene().name;
			var dictionary = _processes.GetOrDefault( name );
			if ( dictionary == null ) {
				_processes[name] = dictionary = new Dictionary< ProcessBody.Type, List<IProcess> >();
			}
			var list = dictionary.GetOrDefault( process._type );
			if ( list == null ) {
				dictionary[process._type] = list = new List<IProcess>();
			}
			_processes[name][process._type].Add( process );
		}

		public override void Dispose() {
			_updateDisposer.Dispose();
			_processes.Clear();
			base.Dispose();
		}
	}
}