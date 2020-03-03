//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process {
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using Singleton;
	using Extension;
	using Debug;

	public abstract class TestNewProcess {
		public virtual TestNewProcessManager.Type _type => TestNewProcessManager.Type.Process;
		public virtual TestNewProcessManager.LifeSpan _lifeSpan => TestNewProcessManager.LifeSpan.InScene;
		public bool _isInitialized	{ get; private set; }

		public MultiAsyncEvent _loadEvent		{ get; protected set; } = new MultiAsyncEvent();
		public MultiAsyncEvent _initializeEvent	{ get; protected set; } = new MultiAsyncEvent();
		public MultiEvent _updateEvent			{ get; protected set; } = new MultiEvent();
		public MultiAsyncEvent _finalizeEvent	{ get; protected set; } = new MultiAsyncEvent();

		protected TestNewProcess() {
			TestNewProcessManager.s_instance.Register( this );
		}
		public abstract void Create();
		public override string ToString() {
			return this.ToDeepString();
		}
	}

	public class TestHogeProcess : TestNewProcess {
		public override void Create() {
			_loadEvent.AddLast( "load 2", async () => {
				Log.Debug( "load 2" );
				await UniTask.Delay( 500 );
			} );
			_loadEvent.InsertFirst( "load 2", "load 1", async () => {
				Log.Debug( "load 1" );
				await UniTask.Delay( 500 );
			} );
			_loadEvent.InsertLast( "load 2", "load 3", async () => {
				Log.Debug( "load 3" );
				await UniTask.Delay( 500 );
			} );

			_updateEvent.AddLast( "update 2", () => {
				Log.Debug( "update 2" );
			} );
			_updateEvent.InsertFirst( "update 2", "update 1", () => {
				Log.Debug( "update 1" );
			} );
			_updateEvent.InsertLast( "update 2", "update 3", () => {
				Log.Debug( "update 3" );
			} );
		}
		~TestHogeProcess() {
			Log.Debug( "Delete TestHogeProcess" );
		}
	}
	public class TestHogeProcess2 : TestHogeProcess {
		public override void Create() {
			base.Create();

			_loadEvent.InsertFirst( "load 1", "load 0", async () => {
				Log.Debug( "load 0 override" );
				await UniTask.Delay( 500 );
			} );
			_loadEvent.InsertLast( "load 3", "load 4", async () => {
				Log.Debug( "load 4 override" );
				await UniTask.Delay( 500 );
			} );

			_updateEvent.InsertFirst( "update 1", "update 0", () => {
				Log.Debug( "update 0 override" );
			} );
			_updateEvent.InsertLast( "update 3", "update 4", () => {
				Log.Debug( "update 4 override" );
			} );
			_updateEvent.InsertLast( "update 5", "update 6", () => {
				Log.Debug( "update 6 override" );
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

		public void Register( TestNewProcess process ) {
			RegisterSub( process ).Forget();
		}

		async UniTaskVoid RegisterSub( TestNewProcess process ) {
			process.Create();
			switch ( process._type ) {
				case Type.DontProcess:
					return;

				case Type.Process:
					await UniTask.WaitUntil( () => _isInitialized );
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
				.Select( async p => await p._loadEvent.Invoke() );

			Observable.EveryUpdate().Subscribe( _ => {
				_processes
					.SelectMany( pair => pair.Value )
					.ForEach( p => p._updateEvent.Invoke() );
			} );
			Observable.EveryUpdate()
				.Where( _ => Input.GetKeyDown( KeyCode.Return ) )
				.Take( 1 )
				.Subscribe( _ => {
					_processes.Clear();
					Log.Debug( "clear process" );
				} );
		}
	}
}