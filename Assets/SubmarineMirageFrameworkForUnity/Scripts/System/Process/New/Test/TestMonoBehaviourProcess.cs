//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process.New {
	using System;
	using System.Linq;
	using System.Threading;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using Singleton.New;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class TestMonoBehaviourProcess : MonoBehaviourProcess {
		public override ProcessBody.Type _type => ProcessBody.Type.FirstWork;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.Forever;

		public override void Create() {
			Log.Debug( "Create()" );
			_loadEvent.AddLast( async cancel => {
				Log.Debug( "_loadEvent start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( "_loadEvent end" );
			} );
			_initializeEvent.AddLast( async cancel => {
				Log.Debug( "_initializeEvent start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( "_initializeEvent end" );
			} );
			_enableEvent.AddLast( async cancel => {
				Log.Debug( "_enableEvent start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( "_enableEvent end" );
			} );
			_fixedUpdateEvent.AddLast().Subscribe( _ => {
				Log.Debug( "_fixedUpdateEvent" );
			} );
			_updateEvent.AddLast().Subscribe( _ => {
				Log.Debug( "_updateEvent" );
			} );
			_lateUpdateEvent.AddLast().Subscribe( _ => {
				Log.Debug( "_lateUpdateEvent" );
			} );
			_disableEvent.AddLast( async cancel => {
				Log.Debug( "_disableEvent start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( "_disableEvent end" );
			} );
			_finalizeEvent.AddLast( async cancel => {
				Log.Debug( "_finalizeEvent start" );
				await UniTaskUtility.Delay( cancel, 1000 );
				Log.Debug( "_finalizeEvent end" );
			} );
		}
	}


	

	public class TestMonoBehaviourProcessManager : Singleton<TestMonoBehaviourProcessManager> {
		new MonoBehaviourProcess _process;

		public TestMonoBehaviourProcessManager() {
			var text = GameObject.Find( "Canvas/Text" ).GetComponent<Text>();
			var go = new GameObject( "TestMonoBehaviourProcess" );
			_process = go.AddComponent<TestMonoBehaviourProcess>();

			_process._disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ =>
				text.text =
					$"{_process.GetAboutName()}(\n"
					+ $"    {_process._belongSceneName}\n"
					+ $"    _isInitialized {_process._isInitialized}\n"
					+ $"    _isActive {_process._isActive}\n"
					+ $"    isActiveAndEnabled {_process.isActiveAndEnabled}\n"
					+ $"    {_process._process._ranState}\n"
					+ $"    {_process._process._activeState}\n"
					+ $"    next {_process._process._nextActiveState}\n"
					+ $")"
			) );

			_process._disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					Log.Warning( "key down Creating" );
/*
					UniTask.Void( async () => {
						await _process.RunStateEvent( ProcessBody.RanState.Creating );
						await _process.RunStateEvent( ProcessBody.RanState.Loading );
						await _process.RunStateEvent( ProcessBody.RanState.Initializing );
					} );
*/
					_process.RunStateEvent( ProcessBody.RanState.Creating ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( "key down Loading" );
					_process.RunStateEvent( ProcessBody.RanState.Loading ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha3 ) ).Subscribe( _ => {
					Log.Warning( "key down Initializing" );
					_process.RunStateEvent( ProcessBody.RanState.Initializing ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha4 ) ).Subscribe( _ => {
					Log.Warning( "key down FixedUpdate" );
					_process.RunStateEvent( ProcessBody.RanState.FixedUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha5 ) ).Subscribe( _ => {
					Log.Warning( "key down Update" );
					_process.RunStateEvent( ProcessBody.RanState.Update ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha6 ) ).Subscribe( _ => {
					Log.Warning( "key down LateUpdate" );
					_process.RunStateEvent( ProcessBody.RanState.LateUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha7 ) ).Subscribe( _ => {
					Log.Warning( "key down Finalizing" );
					_process.RunStateEvent( ProcessBody.RanState.Finalizing ).Forget();
				} )
			);
			_process._disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Z ) ).Subscribe( _ => {
					Log.Warning( "key down Enabling" );
					_process.ChangeActive( true ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.X ) ).Subscribe( _ => {
					Log.Warning( "key down Disabling" );
					_process.ChangeActive( false ).Forget();
				} )
			);
			_process._disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down Dispose" );
					_process.Dispose();
				} )
			);

			UniTask.Void( async () => {
				return;
				while ( true ) {
					Log.Debug(
						$"IsCancellationRequested : {_process._activeAsyncCancel.IsCancellationRequested}" );
					await UniTaskUtility.Delay( _process._activeAsyncCancel, 1000 );
				}
			} );

			UniTask.Void( async () => {
				GameObject top = null;
				Transform t = null;
				10.Times( () => {
					var g = new GameObject();
					var h = g.AddComponent<TestHoge>();
					g.name = $"TestHoge {h._id}";
					if ( top == null )	{ top = g; }
					if ( t != null )	{ g.SetParent( t ); }
					g.SetActive( false );
					t = g.transform;

					g = new GameObject();
					h = g.AddComponent<TestHoge>();
					g.name = $"TestHoge {h._id}";
					if ( t != null )	{ g.SetParent( t ); }
					g.SetActive( false );
				} );

				var ts = top.GetComponentsIn1HierarchyChildren<TestHoge>( true );
				ts.ForEach( testHoge => Log.Debug( testHoge._id ) );
				var tsa = top.transform.GetComponentsInChildrenWithoutSelf<TestHoge>( true );
				tsa.Reverse().ForEach( testHoge => Log.Debug( testHoge._id ) );
				await UniTaskUtility.DontWait();
			} );

			_process._disposables.AddLast( () => text.text = string.Empty );

			_disposables.AddLast( _process );
		}

		~TestMonoBehaviourProcessManager() => Log.Debug( "~TestMonoBehaviourProcessManager" );

		public override void Create() {}
	}

	public class TestHoge : MonoBehaviour {
		static int s_count;
		public int _id	{ get; private set; }
		void Awake() {
			_id = s_count++;
		}
	}
}