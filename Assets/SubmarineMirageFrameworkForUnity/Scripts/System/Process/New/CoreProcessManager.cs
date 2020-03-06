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
	using Singleton;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class CoreProcessManager : MonoBehaviourSingleton<CoreProcessManager> {
		public enum ProcessType {
			DontWork,
			Work,
			FirstWork,
		}
		public enum ProcessLifeSpan {
			InScene,
			Forever,
		}
		readonly Dictionary< string, List<BaseProcess> > _processes
			= new Dictionary< string, List<BaseProcess> >();
		CompositeDisposable _updateDisposer = new CompositeDisposable();

		protected override void Constructor() {
			Observable.EveryUpdate()
				.Where( _ => Input.GetKeyDown( KeyCode.Return ) )
				.Take( 1 )
				.Subscribe( _ => Clear().Forget() );
		}

		public void Register( BaseProcess process ) {
			RegisterSub( process ).Forget();
		}

		async UniTask RegisterSub( BaseProcess process ) {
			process.Create();
			switch ( process._type ) {
				case ProcessType.DontWork:
					return;

				case ProcessType.Work:
					await UniTaskUtility.WaitUntil( process._activeAsyncCancel, () => _isInitialized );
					break;

				case ProcessType.FirstWork:
					break;
			}
			var name = process._lifeSpan == ProcessLifeSpan.Forever ? "Forever"
				: SceneManager.GetActiveScene().name;
			var list = _processes.GetOrDefault( name );
			if ( list == null )	{ _processes[name] = new List<BaseProcess>(); }
			_processes[name].Add( process );

			await _processes
				.SelectMany( pair => pair.Value )
				.Select( async p => await p._loadEvent.Invoke( p._activeAsyncCancel ) );
			await _processes
				.SelectMany( pair => pair.Value )
				.Select( async p => await p._initializeEvent.Invoke( p._activeAsyncCancel ) );
			await _processes
				.SelectMany( pair => pair.Value )
				.Select( async p => await p._enableEvent.Invoke( p._activeAsyncCancel ) );
			Observable.EveryUpdate().Subscribe( _ => {
				_processes
					.SelectMany( pair => pair.Value )
					.Where( p => p._isActive )
					.ForEach( p => p._updateEvent.Invoke() );
			} )
			.AddTo( _updateDisposer );
		}

		async UniTask Clear() {
			Log.Debug( "clear process" );
			_updateDisposer.Dispose();
			await _processes
				.SelectMany( pair => pair.Value )
				.Select( async p => await p._disableEvent.Invoke( p._finalizeAsyncCancel ) );
			await _processes
				.SelectMany( pair => pair.Value )
				.Select( async p => await p._finalizeEvent.Invoke( p._finalizeAsyncCancel ) );
			_processes
				.SelectMany( pair => pair.Value )
				.ForEach( p => p.Dispose() );
			_processes.Clear();
		}
	}
}