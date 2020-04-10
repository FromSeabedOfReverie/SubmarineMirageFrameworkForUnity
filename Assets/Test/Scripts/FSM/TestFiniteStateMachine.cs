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


	public class TestFiniteStateMachine : Test {
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
				_text.text =
					$"{_process.GetAboutName()}(\n"
					+ $"    _isInitialized : {_process._isInitialized}\n"
					+ $"    _isActive : {_process._isActive}\n"
					+ $"    _ranState : {_process._body._ranState}\n"
					+ $"    _activeState : {_process._body._activeState}\n"
					+ $"    _nextActiveState : {_process._body._nextActiveState}\n"
					+ ")\n";
				_text.text += $"{_process._fsm}";
			} ) );

			_disposables.AddLast( _process );
		}


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
			var i = 0;
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Space ) ).Subscribe( _ => {
					Log.Warning( "key down change state" );
					i = (i + 1) % 3;
					switch ( i ) {
						case 0:
							Log.Debug( $"{this.GetAboutName()} change TestStateA" );
							_process._fsm.ChangeState<TestStateA>().Forget();
							break;
						case 1:
							Log.Debug( $"{this.GetAboutName()} change TestStateB" );
							_process._fsm.ChangeState<TestStateB>().Forget();
							break;
						case 2:
							Log.Debug( $"{this.GetAboutName()} change TestStateC" );
							_process._fsm.ChangeState<TestStateC>().Forget();
							break;
					}
				} )
			);

			while ( true )	{ yield return null; }
		}



		public class TestOwner : BaseProcess, IFiniteStateMachineOwner<TestFSMManager> {
			public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.Forever;
			public TestFSMManager _fsm	{ get; private set; }

			public override void Create() {
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
				_finalizeEvent.AddLast( async cancel => {
					Log.Debug( "_finalizeEvent start" );
					await UniTaskUtility.Delay( cancel, 1000 );
					Log.Debug( "_finalizeEvent end" );
				} );

				_enableEvent.AddLast( async cancel => {
					Log.Debug( "_enableEvent start" );
					await UniTaskUtility.Delay( cancel, 1000 );
					Log.Debug( "_enableEvent end" );
				} );
				_disableEvent.AddLast( async cancel => {
					Log.Debug( "_disableEvent start" );
					await UniTaskUtility.Delay( cancel, 1000 );
					Log.Debug( "_disableEvent end" );
				} );

				_fixedUpdateEvent.AddLast().Subscribe( _ => Log.Debug( "_fixedUpdateEvent" ) );
				_updateEvent.AddLast().Subscribe( _ => Log.Debug( "_updateEvent" ) );
				_lateUpdateEvent.AddLast().Subscribe( _ => Log.Debug( "_lateUpdateEvent" ) );

				_disposables.AddFirst( _fsm = new TestFSMManager( this ) );
			}
		}



		public class TestFSMManager : FiniteStateMachine<TestFSMManager, TestOwner, BaseTestState> {
			public TestFSMManager( TestOwner owner ) : base(
				owner,
				new BaseTestState[] {
					new TestStateA(),
					new TestStateB(),
					new TestStateC(),
				}
			) {
			}
		}



		public abstract class BaseTestState : State<TestFSMManager, TestOwner> {
			public BaseTestState() {
				_loadEvent.AddLast( async cancel => await TestProcess( cancel, "_loadEvent" ) );
				_initializeEvent.AddLast( async cancel => await TestProcess( cancel, "_initializeEvent" ) );
				_finalizeEvent.AddLast( async cancel => await TestProcess( cancel, "_finalizeEvent" ) );

				_enableEvent.AddLast( async cancel => await TestProcess( cancel, "_enableEvent" ) );
				_disableEvent.AddLast( async cancel => await TestProcess( cancel, "_disableEvent" ) );

				_enterEvent.AddLast( async cancel => await TestProcess( cancel, "_enterEvent" ) );
				_exitEvent.AddLast( async cancel => await TestProcess( cancel, "_exitEvent" ) );
				_updateEvent.AddLast( async cancel => await TestProcess( cancel, "_updateEvent", 5 ) );

				_fixedUpdateDeltaEvent.AddLast().Subscribe( _ =>
					Log.Debug( $"{this.GetAboutName()}._fixedUpdateDeltaEvent" ) );
				_updateDeltaEvent.AddLast().Subscribe( _ =>
					Log.Debug( $"{this.GetAboutName()}._updateDeltaEvent" ) );
				_lateUpdateDeltaEvent.AddLast().Subscribe( _ =>
					Log.Debug( $"{this.GetAboutName()}._lateUpdateDeltaEvent" ) );
			}
			async UniTask TestProcess( CancellationToken cancel, string functionName, int count = 2 ) {
				for ( var i = 0; i < count; i++ ) {
					Log.Debug( $"{this.GetAboutName()}.{functionName} : {i}" );
					await UniTaskUtility.Delay( cancel, 1000 );
				}
			}
		}

		public class TestStateA : BaseTestState {
			public TestStateA() {
/*
				_updateDeltaEvent.AddLast()
					.Where( _ => Input.GetKeyDown( KeyCode.Space ) )
					.Subscribe( _ => {
						Log.Debug( $"{this.GetAboutName()} change TestStateB" );
						_fsm.ChangeState<TestStateB>( _owner._activeAsyncCancel ).Forget();
					} );
*/
			}
		}

		public class TestStateB : BaseTestState {
			public TestStateB() {
/*
				_updateDeltaEvent.AddLast()
					.Where( _ => Input.GetKeyDown( KeyCode.Space ) )
					.Subscribe( _ => {
						Log.Debug( $"{this.GetAboutName()} change TestStateC" );
						_fsm.ChangeState<TestStateC>( _owner._activeAsyncCancel ).Forget();
					} );
*/
			}
		}

		public class TestStateC : BaseTestState {
			public TestStateC() {
/*
				_updateDeltaEvent.AddLast()
					.Where( _ => Input.GetKeyDown( KeyCode.Space ) )
					.Subscribe( _ => {
						Log.Debug( $"{this.GetAboutName()} change TestStateA" );
						_fsm.ChangeState<TestStateA>( _owner._activeAsyncCancel ).Forget();
					} );
*/
			}
		}
	}
}