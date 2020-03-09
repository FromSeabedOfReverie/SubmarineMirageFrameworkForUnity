//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.FSM.New {
	using System;
	using System.Threading;
	using UnityEngine;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using Process.New;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class TestOwner : BaseProcess, IFiniteStateMachineOwner<TestFSMManager> {
		public TestFSMManager _fsm	{ get; private set; }

		public override void Create() {
			_fsm = new TestFSMManager( this );
			Observable.EveryUpdate().Subscribe( _ => {
				if ( Input.GetKeyDown( KeyCode.A ) ) {
					_updateEvent.Remove( "test" );
				}
			} );
			
///*
			var i = 0;
			_updateEvent.AddLast()
				.Where( _ => Input.GetKeyDown( KeyCode.Space ) )
				.Subscribe( _ => {
					i = (i + 1) % 3;
					switch ( i ) {
						case 0:
							Log.Debug( $"{this.GetAboutName()} change TestStateA" );
							_fsm.ChangeState<TestStateA>( _activeAsyncCancel ).Forget();
							break;
						case 1:
							Log.Debug( $"{this.GetAboutName()} change TestStateB" );
							_fsm.ChangeState<TestStateB>( _activeAsyncCancel ).Forget();
							break;
						case 2:
							Log.Debug( $"{this.GetAboutName()} change TestStateC" );
							_fsm.ChangeState<TestStateC>( _activeAsyncCancel ).Forget();
							break;
					}
				} );
//*/
		}
		public override void Dispose() {
			_fsm.Dispose();
			base.Dispose();
		}
	}

	public class TestFSMManager : FiniteStateMachine<TestOwner, TestFSMManager> {
		public TestFSMManager( TestOwner owner ) : base(
			owner,
			new BaseTestState[] {
				new TestStateA( owner ),
				new TestStateB( owner ),
				new TestStateC( owner ),
			}
		) {
		}
	}

	public abstract class BaseTestState : State<TestOwner, TestFSMManager> {
		public BaseTestState( TestOwner owner ) : base( owner ) {
			_initializeEvent.AddLast( async cancel => await TestProcess( cancel, "initialize" ) );
			_enterEvent.AddLast( async cancel => await TestProcess( cancel, "enter" ) );
			_updateEvent.AddLast( async cancel => await TestProcess( cancel, "update" ) );
			_exitEvent.AddLast( async cancel => await TestProcess( cancel, "exit" ) );
		}
		async UniTask TestProcess( CancellationToken cancel, string functionName ) {
			for ( var i = 1; i < 5; i++ ) {
				Log.Debug( $"{this.GetAboutName()} {functionName} {i}" );
				await UniTaskUtility.Delay( cancel, 400 );
			}
			Log.Debug( $"{this.GetAboutName()} {functionName} 5" );
		}
	}

	public class TestStateA : BaseTestState {
		public TestStateA( TestOwner owner ) : base( owner ) {
/*
			_updateDeltaEvent.AddLast()
				.Where( _ => Input.GetKeyDown( KeyCode.Space ) )
				.Subscribe( _ => {
					Log.Debug( $"{this.GetAboutName()} change TestStateB" );
					_fsm.ChangeState<TestStateB>( owner._activeAsyncCancel ).Forget();
				} );
*/
		}
	}

	public class TestStateB : BaseTestState {
		public TestStateB( TestOwner owner ) : base( owner ) {
/*
			_updateDeltaEvent.AddLast()
				.Where( _ => Input.GetKeyDown( KeyCode.Space ) )
				.Subscribe( _ => {
					Log.Debug( $"{this.GetAboutName()} change TestStateC" );
					_fsm.ChangeState<TestStateC>( owner._activeAsyncCancel ).Forget();
				} );
*/
		}
	}

	public class TestStateC : BaseTestState {
		public TestStateC( TestOwner owner ) : base( owner ) {
/*
			_updateDeltaEvent.AddLast()
				.Where( _ => Input.GetKeyDown( KeyCode.Space ) )
				.Subscribe( _ => {
					Log.Debug( $"{this.GetAboutName()} change TestStateA" );
					_fsm.ChangeState<TestStateA>( owner._activeAsyncCancel ).Forget();
				} );
*/
		}
	}
}