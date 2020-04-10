//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestFSM {
	using System;
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using System.Threading;
	using NUnit.Framework;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.TestTools;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using Process.New;
	using FSM.New;
	using Extension;
	using Utility;
	using Debug;
	using Test;
	using UnityObject = UnityEngine.Object;
	using RanState = Process.New.ProcessBody.RanState;
	using ActiveState = Process.New.ProcessBody.ActiveState;


	// TODO : コメント追加、整頓


	public class TestState : Test {
		Text _text;
		TestOwner _process;


		protected override void Create() {
			Application.targetFrameRate = 30;
			_process = new TestOwner();

			UnityObject.Instantiate( Resources.Load<GameObject>( "TestCamera" ) );
			var go = UnityObject.Instantiate( Resources.Load<GameObject>( "TestCanvas" ) );
			_text = go.GetComponentInChildren<Text>();
			_disposables.AddLast( Observable.EveryLateUpdate().Subscribe( _ => {
				if ( _process == null ) {
					_text.text = string.Empty;
					return;
				}
				_text.text = $"{_process._fsm}";
			} ) );

			_disposables.AddLast( _process );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() {
			yield return UniTask.ToCoroutine( async () => {
				await _process.RunStateEvent( RanState.Creating );
				await _process.RunStateEvent( RanState.Loading );
				await _process.RunStateEvent( RanState.Initializing );
				await _process.RunActiveEvent();
			} );

			var state = _process._fsm._state;
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					Log.Warning( "key down Loading" );
					state.RunProcessStateEvent( RanState.Loading, _asyncCancel ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( "key down Initializing" );
					state.RunProcessStateEvent( RanState.Initializing, _asyncCancel ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha3 ) ).Subscribe( _ => {
					Log.Warning( "key down FixedUpdate" );
					state.RunProcessStateEvent( RanState.FixedUpdate, _asyncCancel ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha4 ) ).Subscribe( _ => {
					Log.Warning( "key down Update" );
					state.RunProcessStateEvent( RanState.Update, _asyncCancel ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha5 ) ).Subscribe( _ => {
					Log.Warning( "key down LateUpdate" );
					state.RunProcessStateEvent( RanState.LateUpdate, _asyncCancel ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha6 ) ).Subscribe( _ => {
					Log.Warning( "key down Finalizing" );
					state.RunProcessStateEvent( RanState.Finalizing, _asyncCancel ).Forget();
				} ),

				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Keypad1 ) ).Subscribe( _ => {
					Log.Warning( "key down None" );
					state.RunProcessStateEvent( RanState.None, _asyncCancel ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Keypad2 ) ).Subscribe( _ => {
					Log.Warning( "key down Creating" );
					state.RunProcessStateEvent( RanState.Creating, _asyncCancel ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Keypad3 ) ).Subscribe( _ => {
					Log.Warning( "key down Created" );
					state.RunProcessStateEvent( RanState.Created, _asyncCancel ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Keypad4 ) ).Subscribe( _ => {
					Log.Warning( "key down Loaded" );
					state.RunProcessStateEvent( RanState.Loaded, _asyncCancel ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Keypad5 ) ).Subscribe( _ => {
					Log.Warning( "key down Initialized" );
					state.RunProcessStateEvent( RanState.Initialized, _asyncCancel ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Keypad6 ) ).Subscribe( _ => {
					Log.Warning( "key down Finalized" );
					state.RunProcessStateEvent( RanState.Finalized, _asyncCancel ).Forget();
				} )
			);
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.A ) ).Subscribe( _ => {
					Log.Warning( "key down Enabling" );
					state.RunProcessActiveEvent( ActiveState.Enabling, _asyncCancel ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.S ) ).Subscribe( _ => {
					Log.Warning( "key down Disabling" );
					state.RunProcessActiveEvent( ActiveState.Disabling, _asyncCancel ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.D ) ).Subscribe( _ => {
					Log.Warning( "key down Enabled" );
					state.RunProcessActiveEvent( ActiveState.Enabled, _asyncCancel ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.F ) ).Subscribe( _ => {
					Log.Warning( "key down Disabled" );
					state.RunProcessActiveEvent( ActiveState.Disabled, _asyncCancel ).Forget();
				} )
			);
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Z ) ).Subscribe( _ => {
					Log.Warning( "key down Enter" );
					state.RunStateEvent( FiniteStateMachineRunState.Enter ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.X ) ).Subscribe( _ => {
					Log.Warning( "key down Update" );
					state.RunStateEvent( FiniteStateMachineRunState.Update ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.C ) ).Subscribe( _ => {
					Log.Warning( "key down Exit" );
					state.RunStateEvent( FiniteStateMachineRunState.Exit ).Forget();
				} )
			);
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down StopActiveAsync" );
					state.StopActiveAsync();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Delete ) ).Subscribe( _ => {
					Log.Warning( "key down Owner StopActiveAsync" );
					_process.StopActiveAsync();
				} )
			);

			while ( true )	{ yield return null; }
		}



		public class TestOwner : BaseProcess, IFiniteStateMachineOwner<TestFSMManager> {
			public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.Forever;
			public TestFSMManager _fsm	{ get; private set; }

			public override void Create() {
				_disposables.AddFirst( _fsm = new TestFSMManager( this ) );
			}
		}



		public class TestFSMManager : FiniteStateMachine<TestFSMManager, TestOwner, BaseTestState> {
			public TestFSMManager( TestOwner owner ) : base(
				owner,
				new BaseTestState[] {
					new TestStateA(),
				}
			) {
			}
		}



		public abstract class BaseTestState : State<TestFSMManager, TestOwner> {
		}

		public class TestStateA : BaseTestState {
			public TestStateA() {
				_loadEvent.AddLast( async cancel => await TestProcess( cancel, "_loadEvent" ) );
				_initializeEvent.AddLast( async cancel => await TestProcess( cancel, "_initializeEvent" ) );
				_finalizeEvent.AddLast( async cancel => await TestProcess( cancel, "_finalizeEvent" ) );

				_enableEvent.AddLast( async cancel => await TestProcess( cancel, "_enableEvent" ) );
				_disableEvent.AddLast( async cancel => await TestProcess( cancel, "_disableEvent" ) );

				_enterEvent.AddLast( async cancel => await TestProcess( cancel, "_enterEvent" ) );
				_exitEvent.AddLast( async cancel => await TestProcess( cancel, "_exitEvent" ) );
				_updateEvent.AddLast( async cancel => await TestProcess( cancel, "_updateEvent" ) );

				_fixedUpdateDeltaEvent.AddLast().Subscribe( _ =>
					Log.Debug( $"{this.GetAboutName()}._fixedUpdateDeltaEvent" ) );
				_updateDeltaEvent.AddLast().Subscribe( _ =>
					Log.Debug( $"{this.GetAboutName()}._updateDeltaEvent" ) );
				_lateUpdateDeltaEvent.AddLast().Subscribe( _ =>
					Log.Debug( $"{this.GetAboutName()}._lateUpdateDeltaEvent" ) );
			}
			async UniTask TestProcess( CancellationToken cancel, string functionName ) {
				for ( var i = 0; i < 2; i++ ) {
					Log.Debug( $"{this.GetAboutName()}.{functionName} : {i}" );
					await UniTaskUtility.Delay( cancel, 1000 );
				}
			}
		}
	}
}