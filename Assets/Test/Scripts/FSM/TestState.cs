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
	using SMTask;
	using FSM.New;
	using Extension;
	using Utility;
	using Debug;
	using Test;
	using UnityObject = UnityEngine.Object;
	using RunState = FSM.New.FiniteStateMachineRunState;


	// TODO : コメント追加、整頓


	public class TestState : Test {
		Text _text;
		TestOwner _process;
		BaseTestState _state => _process._fsm._state;


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
					$"_process : {_process._body._activeState}\n" +
					$"\n" +
					$"{_process._fsm}";
			} ) );

			_disposables.AddLast( _process );
		}


		[UnityTest]
		[Timeout( int.MaxValue )]
		public IEnumerator TestManual() {
			yield return From( async () => {
				await _process.RunStateEvent( SMTaskRanState.Creating );
			} );

			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					Log.Warning( "key down Loading" );
					_state.RunBehaviourStateEvent( SMTaskRanState.Loading ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( "key down Initializing" );
					_state.RunBehaviourStateEvent( SMTaskRanState.Initializing ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha3 ) ).Subscribe( _ => {
					Log.Warning( "key down FixedUpdate" );
					_state.RunBehaviourStateEvent( SMTaskRanState.FixedUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha4 ) ).Subscribe( _ => {
					Log.Warning( "key down Update" );
					_state.RunBehaviourStateEvent( SMTaskRanState.Update ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha5 ) ).Subscribe( _ => {
					Log.Warning( "key down LateUpdate" );
					_state.RunBehaviourStateEvent( SMTaskRanState.LateUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha6 ) ).Subscribe( _ => {
					Log.Warning( "key down Finalizing" );
					_state.RunBehaviourStateEvent( SMTaskRanState.Finalizing ).Forget();
				} ),

				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Keypad1 ) ).Subscribe( _ => {
					Log.Warning( "key down None" );
					_state.RunBehaviourStateEvent( SMTaskRanState.None ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Keypad2 ) ).Subscribe( _ => {
					Log.Warning( "key down Creating" );
					_state.RunBehaviourStateEvent( SMTaskRanState.Creating ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Keypad3 ) ).Subscribe( _ => {
					Log.Warning( "key down Created" );
					_state.RunBehaviourStateEvent( SMTaskRanState.Created ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Keypad4 ) ).Subscribe( _ => {
					Log.Warning( "key down Loaded" );
					_state.RunBehaviourStateEvent( SMTaskRanState.Loaded ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Keypad5 ) ).Subscribe( _ => {
					Log.Warning( "key down Initialized" );
					_state.RunBehaviourStateEvent( SMTaskRanState.Initialized ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Keypad6 ) ).Subscribe( _ => {
					Log.Warning( "key down Finalized" );
					_state.RunBehaviourStateEvent( SMTaskRanState.Finalized ).Forget();
				} )
			);

			// _state.ChangeActiveは、デバッグの必要が無い。
			// _state.ChangeState、_owner.ChangeActive以外に、単体で呼ばれない為。
			// 必ず親かChangeStateから呼ばれる。
			
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.A ) ).Subscribe( _ => {
					Log.Warning( "key down Entering" );
					_state.RunStateEvent( RunState.Entering ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.S ) ).Subscribe( _ => {
					Log.Warning( "key down Update" );
					_state.RunStateEvent( RunState.Update ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.D ) ).Subscribe( _ => {
					Log.Warning( "key down Exiting" );
					_state.RunStateEvent( RunState.Exiting ).Forget();
				} ),

				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.F ) ).Subscribe( _ => {
					Log.Warning( "key down Entered" );
					_state.RunStateEvent( RunState.Entered ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.G ) ).Subscribe( _ => {
					Log.Warning( "key down BeforeUpdate" );
					_state.RunStateEvent( RunState.BeforeUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.H ) ).Subscribe( _ => {
					Log.Warning( "key down Exited" );
					_state.RunStateEvent( RunState.Exited ).Forget();
				} )
			);
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Z ) ).Subscribe( _ => {
					Log.Warning( "key down Owner enable" );
					_process.ChangeActive( true ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.X ) ).Subscribe( _ => {
					Log.Warning( "key down Owner disable" );
					_process.ChangeActive( false ).Forget();
				} )
			);
			_disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down StopActiveAsync" );
					_state.StopActiveAsync();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Delete ) ).Subscribe( _ => {
					Log.Warning( "key down Owner StopActiveAsync" );
					_process.StopActiveAsync();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.RightShift ) ).Subscribe( _ => {
					Log.Warning( "key down Owner finalize" );
					_process.RunStateEvent( SMTaskRanState.Finalizing ).Forget();
				} )
			);

			yield return From( async () => {
				await _process.RunStateEvent( SMTaskRanState.Loading );
				await _process.RunStateEvent( SMTaskRanState.Initializing );
			} );

			while ( true )	{ yield return null; }
		}



		public class TestOwner : SMBehavior, IFiniteStateMachineOwner<TestFSMManager> {
			public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.Forever;
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
	}
}