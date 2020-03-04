//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process {
	using System;
	using System.Linq;
	using System.Threading;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using Singleton;
	using Extension;
	using Utility;
	using Debug;

	public abstract class TestNewProcess : IDisposable {
		public virtual TestNewProcessManager.Type _type => TestNewProcessManager.Type.Process;
		public virtual TestNewProcessManager.LifeSpan _lifeSpan => TestNewProcessManager.LifeSpan.InScene;
		public bool _isInitialized	{ get; private set; }
		CancellationTokenSource _asyncCanceler = new CancellationTokenSource();
		public CancellationToken _asyncCancelerToken => _asyncCanceler.Token;
		CancellationTokenSource _finalizeCanceler = new CancellationTokenSource();
		public CancellationToken _finalizeCancelerToken => _finalizeCanceler.Token;
		public MultiAsyncEvent _loadEvent		{ get; protected set; }
		public MultiAsyncEvent _initializeEvent	{ get; protected set; }
		public MultiEvent _updateEvent			{ get; protected set; }
		public MultiAsyncEvent _finalizeEvent	{ get; protected set; }

		protected TestNewProcess() {
			_loadEvent = new MultiAsyncEvent();
			_initializeEvent = new MultiAsyncEvent();
			_updateEvent = new MultiEvent();
			_finalizeEvent = new MultiAsyncEvent();
			TestNewProcessManager.s_instance.Register( this );
		}
		public abstract void Create();
		public void Cancel() {
			_asyncCanceler.Cancel();
		}
		public void Dispose() {
			_asyncCanceler.Cancel();
			_finalizeCanceler.Cancel();
			_asyncCanceler.Dispose();
			_finalizeCanceler.Dispose();
		}
		public override string ToString() {
			return this.ToDeepString();
		}
	}

	public class TestHogeProcess : TestNewProcess {
		public override void Create() {
			_loadEvent.AddLast( async ( cancel ) => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "load 1 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 1 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 1 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 1 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 1 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"load 1 end {TimeManager.s_instance.StopMeasure()}" );
			} );
			_initializeEvent.AddLast( async ( cancel ) => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "initialize 1 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 1 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 1 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 1 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 1 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"initialize 1 end {TimeManager.s_instance.StopMeasure()}" );
			} );
			_updateEvent.AddLast( () => {
				Log.Debug( "update 1" );
			} );
			_finalizeEvent.AddLast( async ( cancel ) => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "finalize 1 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 1 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 1 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 1 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 1 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"finalize 1 end {TimeManager.s_instance.StopMeasure()}" );
			} );
		}
	}
	public class TestHogeProcess2 : TestHogeProcess {
		public override void Create() {
			base.Create();
			_loadEvent.AddLast( async ( cancel ) => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "load 2 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 2 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 2 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 2 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "load 2 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"load 2 end {TimeManager.s_instance.StopMeasure()}" );
			} );
			_initializeEvent.AddLast( async ( cancel ) => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "initialize 2 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 2 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 2 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 2 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "initialize 2 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"initialize 2 end {TimeManager.s_instance.StopMeasure()}" );
			} );
			_updateEvent.AddLast( () => {
				Log.Debug( "update 2" );
			} );
			_finalizeEvent.AddLast( async ( cancel ) => {
				TimeManager.s_instance.StartMeasure();
				Log.Debug( "finalize 2 start" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 2 1" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 2 2" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 2 3" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( "finalize 2 4" );
				await UniTaskUtility.Delay( cancel, 100 );
				Log.Debug( $"finalize 2 end {TimeManager.s_instance.StopMeasure()}" );
			} );
		}
		~TestHogeProcess2() {
			Log.Debug( "Delete TestHogeProcess2" );
		}
	}


	public class TestNewProcessManager : MonoBehaviourSingleton<TestNewProcessManager> {
		public enum Type {
			DontProcess,
			Process,
			FirstProcess,
		}
		public enum LifeSpan {
			InScene,
			Forever,
		}
		readonly Dictionary< string, List<TestNewProcess> > _processes
			= new Dictionary< string, List<TestNewProcess> >();
		IDisposable processUpdateDispose;

		protected override void Constructor() {
			Observable.EveryUpdate()
				.Where( _ => Input.GetKeyDown( KeyCode.Return ) )
				.Take( 1 )
				.Subscribe( _ => Clear().Forget() );
		}

		public void Register( TestNewProcess process ) {
			RegisterSub( process ).Forget();
		}

		async UniTask RegisterSub( TestNewProcess process ) {
			process.Create();
			switch ( process._type ) {
				case Type.DontProcess:
					return;

				case Type.Process:
					await UniTaskUtility.WaitUntil( process._asyncCancelerToken, () => _isInitialized );
					break;

				case Type.FirstProcess:
					break;
			}
			var name = process._lifeSpan == LifeSpan.Forever ? LifeSpan.Forever.ToString()
				: SceneManager.GetActiveScene().name;
			var list = _processes.GetOrDefault( name );
			if ( list == null )	{ _processes[name] = new List<TestNewProcess>(); }
			_processes[name].Add( process );

			await _processes
				.SelectMany( pair => pair.Value )
				.Select( async p => await p._loadEvent.Invoke( p._asyncCancelerToken ) );
			await _processes
				.SelectMany( pair => pair.Value )
				.Select( async p => await p._initializeEvent.Invoke( p._asyncCancelerToken ) );

			processUpdateDispose = Observable.EveryUpdate().Subscribe( _ => {
				_processes
					.SelectMany( pair => pair.Value )
					.ForEach( p => p._updateEvent.Invoke() );
			} );
		}

		async UniTask Clear() {
			Log.Debug( "clear process" );
			processUpdateDispose.DisposeIfNotNull();
			_processes
				.SelectMany( pair => pair.Value )
				.ForEach( p => p.Cancel() );
			await _processes
				.SelectMany( pair => pair.Value )
				.Select( async p => {
					await p._finalizeEvent.Invoke( p._finalizeCancelerToken );
					p.Dispose();
				} );
			_processes.Clear();
		}
	}
}