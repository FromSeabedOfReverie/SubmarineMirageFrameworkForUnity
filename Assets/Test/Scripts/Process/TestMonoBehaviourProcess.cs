//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestProcess {
	using System;
	using System.Linq;
	using System.Threading;
	using System.Collections;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.TestTools;
	using UniRx;
	using UniRx.Async;
	using Process.New;
	using Extension;
	using Utility;
	using Debug;
	using Test;
	using UnityObject = UnityEngine.Object;
	using RanState = Process.New.ProcessBody.RanState;
	using ActiveState = Process.New.ProcessBody.ActiveState;



	// TODO : コメント追加、整頓



	public class TestMonoBehaviourProcessManager : Test {
		MonoBehaviourProcess _process;
		Text _text;


		protected override void Create() {
			Application.targetFrameRate = 30;
			var go = new GameObject( "TestMonoBehaviourProcess" );
			_process = go.AddComponent<TestMonoBehaviourProcess>();
//			_process.enabled = false;
//			go.SetActive( false );

			UnityObject.Instantiate( Resources.Load<GameObject>( "TestCamera" ) );
			go = UnityObject.Instantiate( Resources.Load<GameObject>( "TestCanvas" ) );
			_text = go.GetComponentInChildren<Text>();
			_disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ => {
				if ( _process == null ) {
					_text.text = string.Empty;
					return;
				}
				_text.text =
					$"{_process.GetAboutName()}(\n"
					+ $"    _isInitialized {_process._isInitialized}\n"
					+ $"    _isActive {_process._isActive}\n"
					+ $"    isActiveAndEnabled {_process.isActiveAndEnabled}\n"
					+ $"    {_process._body._ranState}\n"
					+ $"    {_process._body._activeState}\n"
					+ $"    next {_process._body._nextActiveState}\n"
					+ $")";
			} ) );
			_disposables.AddLast( () => _text.text = string.Empty );

			_disposables.AddLast( _process );
			_disposables.AddLast( () => {
				if ( _process != null )	{ UnityObject.Destroy( _process.gameObject ); }
			} );
		}


		[UnityTest]
		public IEnumerator TestRunErrorState() => From( async () => {
			var errorRuns = new RanState[] {
				RanState.None, RanState.Created, RanState.Loaded, RanState.Initialized, RanState.Finalized,
			};
			foreach ( var state in errorRuns ) {
				try						{ await _process.RunStateEvent( state ); }
				catch ( Exception e )	{ Log.Error( e ); }
			}
		} );


		[UnityTest]
		public IEnumerator TestRunStateEvent() => From( async () => {
			var states = new RanState[] {
				RanState.Creating, RanState.Loading, RanState.Initializing, RanState.FixedUpdate,
				RanState.Update, RanState.LateUpdate, RanState.Finalizing,
			};
			foreach( var run in states ) {
				Log.Debug( $"request : {run}" );
				await _process.RunStateEvent( run );
			}
		} );


		[UnityTest]
		public IEnumerator TestStopActiveAsync() => From( async () => {
			UniTask.Void( async () => {
				await UniTaskUtility.Delay( _asyncCancel, 3000 );
				Log.Debug( "StopActiveAsync" );
				_process.StopActiveAsync();
			} );
			try {
				while ( true ) {
					Log.Debug( " Runnning " );
					await UniTaskUtility.Delay( _process._activeAsyncCancel, 1000 );
				}
			} catch ( OperationCanceledException ) {
			}
			Log.Debug( "end TestStopActiveAsync()" );
		} );


		[UnityTest]
		public IEnumerator TestDispose() => From( async () => {
			UniTask.Void( async () => {
				await UniTaskUtility.Delay( _asyncCancel, 3000 );
				Log.Debug( "Dispose" );
				_process.Dispose();
			} );
			try {
				while ( true ) {
					Log.Debug( " Runnning " );
					await UniTaskUtility.Delay( _process._activeAsyncCancel, 1000 );
				}
			} catch ( OperationCanceledException ) {
			}
			Log.Debug( "end TestDispose()" );
		} );


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() {
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					Log.Warning( "key down Creating" );
					_process.RunStateEvent( RanState.Creating ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( "key down Loading" );
					_process.RunStateEvent( RanState.Loading ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha3 ) ).Subscribe( _ => {
					Log.Warning( "key down Initializing" );
					_process.RunStateEvent( RanState.Initializing ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha4 ) ).Subscribe( _ => {
					Log.Warning( "key down FixedUpdate" );
					_process.RunStateEvent( RanState.FixedUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha5 ) ).Subscribe( _ => {
					Log.Warning( "key down Update" );
					_process.RunStateEvent( RanState.Update ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha6 ) ).Subscribe( _ => {
					Log.Warning( "key down LateUpdate" );
					_process.RunStateEvent( RanState.LateUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha7 ) ).Subscribe( _ => {
					Log.Warning( "key down Finalizing" );
					_process.RunStateEvent( RanState.Finalizing ).Forget();
				} )
			);
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Z ) ).Subscribe( _ => {
					Log.Warning( "key down Enabling" );
					_process.ChangeActive( true ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.X ) ).Subscribe( _ => {
					Log.Warning( "key down Disabling" );
					_process.ChangeActive( false ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.C ) ).Subscribe( _ => {
					Log.Warning( "key down RunActiveEvent" );
					_process.RunActiveEvent().Forget();
				} )
			);
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down Dispose" );
					_process.Dispose();
					_process = null;
				} )
			);

			while ( true )	{ yield return null; }
		}
	}



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
}