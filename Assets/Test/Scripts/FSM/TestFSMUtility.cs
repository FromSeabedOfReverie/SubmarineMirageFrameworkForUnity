//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestFSM {
	using System;
	using System.Linq;
	using System.Threading;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using MultiEvent;
	using UTask;
	using SMTask;
	using SMTask.Modifyler;
	using FSM.New;
	using Extension;
	using Debug;
	using TestSMTask;
	using RunState = FSM.New.FiniteStateMachineRunState;



	// TODO : コメント追加、整頓



	public static class TestFSMUtility {
		public static MultiDisposable SetRunKey( BaseTestState state ) {
			var disposables = new MultiDisposable();

			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha1 ) ).Subscribe( _ => {
					Log.Warning( "key down Creating" );
					state.RunBehaviourStateEvent( SMTaskRanState.Creating ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha2 ) ).Subscribe( _ => {
					Log.Warning( "key down Loading" );
					state.RunBehaviourStateEvent( SMTaskRanState.Loading ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha3 ) ).Subscribe( _ => {
					Log.Warning( "key down Initializing" );
					state.RunBehaviourStateEvent( SMTaskRanState.Initializing ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha4 ) ).Subscribe( _ => {
					Log.Warning( "key down FixedUpdate" );
					state.RunBehaviourStateEvent( SMTaskRanState.FixedUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha5 ) ).Subscribe( _ => {
					Log.Warning( "key down Update" );
					state.RunBehaviourStateEvent( SMTaskRanState.Update ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha6 ) ).Subscribe( _ => {
					Log.Warning( "key down LateUpdate" );
					state.RunBehaviourStateEvent( SMTaskRanState.LateUpdate ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Alpha7 ) ).Subscribe( _ => {
					Log.Warning( "key down Finalizing" );
					state.RunBehaviourStateEvent( SMTaskRanState.Finalizing ).Forget();
				} )
			);

			// _state.ChangeActiveは、デバッグの必要が無い
			// _state.ChangeState、_owner.ChangeActive以外に、単体で呼ばれない為
			// 必ず親かChangeStateから呼ばれる
			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.A ) ).Subscribe( _ => {
					Log.Warning( "key down Entering" );
					state.RunStateEvent( RunState.Entering ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.S ) ).Subscribe( _ => {
					Log.Warning( "key down Update" );
					state.RunStateEvent( RunState.Update ).Forget();
				} ),
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.D ) ).Subscribe( _ => {
					Log.Warning( "key down Exiting" );
					state.RunStateEvent( RunState.Exiting ).Forget();
				} )
			);

			disposables.AddLast(
				Observable.EveryUpdate().Where( _ => Input.GetKeyDown( KeyCode.Backspace ) ).Subscribe( _ => {
					Log.Warning( "key down StopActiveAsync" );
					state.StopActiveAsync();
				} )
			);

			return disposables;
		}


		public static void SetEvent( ISMBehaviour behaviour ) {
			var name = behaviour.GetAboutName();
			var id = (
				behaviour is BaseM	? ( (BaseM)behaviour )._id :
				behaviour is BaseB	? ( (BaseB)behaviour )._id
									: (int?)null
			);

			behaviour._loadEvent.AddLast( async cancel => {
				Log.Debug( $"start : {name}( {id} ).{nameof( behaviour._loadEvent )}" );
				await UTask.Delay( cancel, 1000 );
				Log.Debug( $"end : {name}( {id} ).{nameof( behaviour._loadEvent )}" );
			} );
			behaviour._initializeEvent.AddLast( async cancel => {
				Log.Debug( $"start : {name}( {id} ).{nameof( behaviour._initializeEvent )}" );
				await UTask.Delay( cancel, 1000 );
				Log.Debug( $"end : {name}( {id} ).{nameof( behaviour._initializeEvent )}" );
			} );
			behaviour._enableEvent.AddLast( async cancel => {
				Log.Debug( $"start : {name}( {id} ).{nameof( behaviour._enableEvent )}" );
				await UTask.Delay( cancel, 1000 );
				Log.Debug( $"end : {name}( {id} ).{nameof( behaviour._enableEvent )}" );
			} );
			behaviour._fixedUpdateEvent.AddLast().Subscribe( _ => {
				Log.Debug( $"{name}( {id} ).{nameof( behaviour._fixedUpdateEvent )}" );
			} );
			behaviour._updateEvent.AddLast().Subscribe( _ => {
				Log.Debug( $"{name}( {id} ).{nameof( behaviour._updateEvent )}" );
			} );
			behaviour._lateUpdateEvent.AddLast().Subscribe( _ => {
				Log.Debug( $"{name}( {id} ).{nameof( behaviour._lateUpdateEvent )}" );
			} );
			behaviour._disableEvent.AddLast( async cancel => {
				Log.Debug( $"start : {name}( {id} ).{nameof( behaviour._disableEvent )}" );
				await UTask.Delay( cancel, 1000 );
				Log.Debug( $"end : {name}( {id} ).{nameof( behaviour._disableEvent )}" );
			} );
			behaviour._finalizeEvent.AddLast( async cancel => {
				Log.Debug( $"start : {name}( {id} ).{nameof( behaviour._finalizeEvent )}" );
				await UTask.Delay( cancel, 1000 );
				Log.Debug( $"end : {name}( {id} ).{nameof( behaviour._finalizeEvent )}" );
			} );
		}
	}



	public class TestOwner : SMBehaviour, IFiniteStateMachineOwner<TestFSMManager> {
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.Forever;
		public TestFSMManager _fsm	{ get; private set; }
		BaseTestState[] _states;

		public TestOwner( BaseTestState[] states ) {
			_states = states;
		}
		public override void Create() {
			_disposables.AddFirst( _fsm = new TestFSMManager( this, _states ) );
		}
	}
	public class TestFSMManager : FiniteStateMachine<TestFSMManager, TestOwner, BaseTestState> {
		public TestFSMManager( TestOwner owner, BaseTestState[] states ) : base( owner, states ) {}
	}

	public abstract class BaseTestState : State<TestFSMManager, TestOwner> {
		public BaseTestState() {
			_loadEvent.AddLast(			async cancel => await TestTask( cancel, $"{nameof( _loadEvent )}" ) );
			_initializeEvent.AddLast(	async cancel => await TestTask( cancel, $"{nameof( _initializeEvent )}" ) );
			_finalizeEvent.AddLast(		async cancel => await TestTask( cancel, $"{nameof( _finalizeEvent )}" ) );

			_enableEvent.AddLast(	async cancel => await TestTask( cancel, $"{nameof( _enableEvent )}" ) );
			_disableEvent.AddLast(	async cancel => await TestTask( cancel, $"{nameof( _disableEvent )}" ) );

			_enterEvent.AddLast(	async cancel => await TestTask( cancel, $"{nameof( _enterEvent )}" ) );
			_exitEvent.AddLast(		async cancel => await TestTask( cancel, $"{nameof( _exitEvent )}" ) );
			_updateEvent.AddLast(	async cancel => await TestTask( cancel, $"{nameof( _updateEvent )}", 5 ) );

			_fixedUpdateDeltaEvent.AddLast().Subscribe( _ =>
				Log.Debug( $"{this.GetAboutName()}.{nameof( _fixedUpdateDeltaEvent )}" ) );
			_updateDeltaEvent.AddLast().Subscribe( _ =>
				Log.Debug( $"{this.GetAboutName()}.{nameof( _updateDeltaEvent )}" ) );
			_lateUpdateDeltaEvent.AddLast().Subscribe( _ =>
				Log.Debug( $"{this.GetAboutName()}.{nameof( _lateUpdateDeltaEvent )}" ) );
		}
		async UniTask TestTask( CancellationToken cancel, string functionName, int count = 2 ) {
			for ( var i = 0; i < count; i++ ) {
				Log.Debug( $"{this.GetAboutName()}.{functionName} : {i}" );
				await UTask.Delay( cancel, 1000 );
			}
		}
	}
	public class TestStateA : BaseTestState {
		public TestStateA() {}
	}
	public class TestStateB : BaseTestState {
		public TestStateB() {}
	}
	public class TestStateC : BaseTestState {
		public TestStateC() {}
	}
}