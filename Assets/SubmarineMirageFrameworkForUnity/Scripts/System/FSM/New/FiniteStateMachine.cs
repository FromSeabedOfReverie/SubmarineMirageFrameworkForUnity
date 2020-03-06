//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.FSM.New {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using System.Threading;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using Process;
	using MultiEvent;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class FiniteStateMachine<TOwner, TFSM> : IFiniteStateMachine, IDisposable
		where TOwner : IFiniteStateMachineOwner<TFSM>
		where TFSM : IFiniteStateMachine
	{
		public enum RunState {
			Enter,
			Update,
			Exit,
		}
		protected TOwner _owner	{ get; private set; }
		readonly Dictionary< Type, State<TOwner, TFSM> > _states = new Dictionary< Type, State<TOwner, TFSM> >();
		public State<TOwner, TFSM> _state	{ get; private set; }
		protected State<TOwner, TFSM> _nextState;
		public RunState _runState	{ get; private set; }
		public MultiAsyncEvent _initializeEvent	{ get; protected set; }
		CompositeDisposable _fsmUpdateDisposers = new CompositeDisposable();
		CancellationTokenSource _fsmAsyncCanceler = new CancellationTokenSource();

		public FiniteStateMachine( TOwner owner, State<TOwner, TFSM>[] states ) {
			_owner = owner;
			foreach ( var state in states ) {
				var type = state.GetType();
				_states[type] = state;
			}

			_initializeEvent = new MultiAsyncEvent();
			_initializeEvent.AddFirst( async cancel => {
				await UniTask.WhenAll(
					_states.Select( async pair => await pair.Value._initializeEvent.Invoke( cancel ) )
				);
				await ChangeState( cancel, _states.First().Key );

				CoreProcessManager.s_instance._updateEvent
					.Where( _ => _runState == RunState.Update )
					.Subscribe( _ => _state._updateDeltaEvent.Invoke() )
					.AddTo( _fsmUpdateDisposers );
#if DEVELOP
				CoreProcessManager.s_instance._updateEvent
					.Subscribe( _ => {
						DebugDisplay.s_instance.Add( $"{_owner.GetAboutName()}.{this.GetAboutName()}" );
						DebugDisplay.s_instance.Add( $"_state : {_state.GetAboutName()}.{_runState}" );
						DebugDisplay.s_instance.Add( $"_nextState : {(_nextState == null ? "null" : _nextState.GetAboutName())}" );
					} )
					.AddTo( _fsmUpdateDisposers );
#endif
			} );
		}

		public async UniTask ChangeState<TState>( CancellationToken cancel ) {
			await ChangeState( cancel, typeof( TState ) );
		}

		public async UniTask ChangeState( CancellationToken cancel, Type stateType ) {
			if ( !_states.ContainsKey( stateType ) ) {
				Log.Error( $"未定義状態へ遷移 : {stateType}", Log.Tag.FSM );
				return;
			}
			var next = _states[stateType];
			if ( !IsPossibleChangeState( next ) ) {
				Log.Warning( $"状態遷移が不可能 : {stateType}", Log.Tag.FSM );
				return;
			}
			_nextState = next;

			if ( _state == null || _runState == RunState.Update ) {
				Log.Debug( $"{this.GetAboutName()} ChangeStateSub" );
				ChangeStateSub().Forget();
			}
			Log.Debug( $"待機開始 {_state.GetAboutName()}.{_runState}" );
			await UniTaskUtility.WaitWhile( cancel, () => _nextState != null || _runState != RunState.Enter );
			Log.Debug( $"待機終了 {_state.GetAboutName()}.{_runState}" );
		}

		async UniTask ChangeStateSub() {
			StopAsync();

			_runState = RunState.Exit;
			if ( _state != null ) {
				await _state._exitEvent.Invoke( _fsmAsyncCanceler.Token );
			}
			_state = _nextState;
			_nextState = null;

			_runState = RunState.Enter;
			await _state._enterEvent.Invoke( _fsmAsyncCanceler.Token );

			if ( _nextState == null ) {
				_runState = RunState.Update;
				_state._updateEvent.Invoke( _fsmAsyncCanceler.Token ).Forget();
			} else {
				await ChangeStateSub();
			}
		}

		protected virtual bool IsPossibleChangeState( State<TOwner, TFSM> changeState ) {
			return true;
		}

		void StopAsync() {
			_fsmAsyncCanceler.Cancel();
			_fsmAsyncCanceler.Dispose();
			_fsmAsyncCanceler = new CancellationTokenSource();
		}

		// TODO : gameObjectが、非活動化したら処理を停止し、活動化したら再開させる
		public void Enable() {
		}
		public void Disable() {
		}

		public override string ToString() {
			return this.ToDeepString();
		}

		public virtual void Dispose() {
			_states.ForEach( pair => pair.Value.Dispose() );
			_initializeEvent.Dispose();
			_fsmUpdateDisposers.Dispose();
			_fsmAsyncCanceler.Cancel();
			_fsmAsyncCanceler.Dispose();
		}

		~FiniteStateMachine() {
			Dispose();
		}
	}
}