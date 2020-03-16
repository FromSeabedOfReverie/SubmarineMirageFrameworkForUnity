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
	using MultiEvent;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class FiniteStateMachine<TFSM, TOwner, TState> : IFiniteStateMachine
		where TFSM : IFiniteStateMachine
		where TOwner : IFiniteStateMachineOwner<TFSM>
		where TState : class, IState<TFSM, TOwner>
	{
		enum ActiveState {
			Disabled,
			Disabling,
			Enabled,
			Enabling,
		}
		enum RunState {
			Enter,
			Update,
			Exit,
		}
		protected TOwner _owner	{ get; private set; }
		string _name;
		public bool _isActive => _activeState == ActiveState.Enabled;
		ActiveState _activeState;
		RunState _runState;

		protected readonly Dictionary<Type, TState> _states = new Dictionary<Type, TState>();
		public TState _state	{ get; private set; }
		protected TState _nextState;

		CancellationTokenSource _asyncCanceler = new CancellationTokenSource();

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public FiniteStateMachine( TOwner owner, TState[] states ) {
			_owner = owner;
			foreach ( var state in states ) {
				var type = state.GetType();
				_states[type] = state;
			}
			_name = this.GetAboutName();

			_owner._initializeEvent.AddLast( _name, async cancel => {
				await UniTask.WhenAll(
					_states
						.Select( pair => pair.Value )
						.Select( async state => {
							state.Set( _owner );
							await state._initializeEvent.Invoke( cancel );
						} )
				);
				_nextState = _states.First().Value;
			} );

			_owner._enableEvent.AddLast( _name, async cancel => {
				_activeState = ActiveState.Enabling;
				ChangeStateSub().Forget();
				await UniTaskUtility.WaitWhile( cancel, () => _activeState == ActiveState.Enabling );
			} );

			_owner._disableEvent.AddLast( _name, async cancel => {
				_activeState = ActiveState.Disabling;
				if ( _runState == RunState.Update ) {
					ChangeStateSub().Forget();
				}
				await UniTaskUtility.WaitWhile( cancel, () => _activeState == ActiveState.Disabling );
				StopAsync();
			} );

			_owner._updateEvent.AddLast( _name )
				.Where( _ => _runState == RunState.Update )
				.Subscribe( _ => _state._updateDeltaEvent.Invoke() );

			_owner._fixedUpdateEvent.AddLast( _name )
				.Where( _ => _runState == RunState.Update )
				.Subscribe( _ => _state._fixedUpdateDeltaEvent.Invoke() );

			_owner._lateUpdateEvent.AddLast( _name )
				.Where( _ => _runState == RunState.Update )
				.Subscribe( _ => _state._lateUpdateDeltaEvent.Invoke() );
#if DEVELOP && false
			_disposables.AddLast(
				Observable.EveryUpdate().Subscribe( _ => {
					DebugDisplay.s_instance.Add( $"{_owner.GetAboutName()}.{_name}" );
					DebugDisplay.s_instance.Add( $"_state : {_state.GetAboutName()}.{_runState}" );
					DebugDisplay.s_instance.Add(
						$"_nextState : {( _nextState == null ? "null" : _nextState.GetAboutName() )}"
					);
				} )
			);
#endif

			SetAsyncCancelerDisposable();
			_disposables.AddLast( () => {
				_states.ForEach( pair => pair.Value.Dispose() );
				_states.Clear();
				_state = null;
				_nextState = null;
			} );
		}

		void SetAsyncCancelerDisposable() {
			_disposables.AddFirst( "_asyncCanceler", () => {
				_asyncCanceler.Cancel();
				_asyncCanceler.Dispose();
			} );
		}

		public void Dispose() => _disposables.Dispose();

		~FiniteStateMachine() => Dispose();


		void StopAsync() {
			_disposables.Remove( "_asyncCanceler" );
			_asyncCanceler = new CancellationTokenSource();
			SetAsyncCancelerDisposable();
		}


		public async UniTask ChangeState<T>( CancellationToken cancel ) where T : TState {
			await ChangeState( cancel, typeof( T ) );
		}


		public async UniTask ChangeState( CancellationToken cancel, Type stateType ) {
			if ( !_states.ContainsKey( stateType ) ) {
				Log.Error( $"未定義状態へ遷移 : {stateType}", Log.Tag.FSM );
				return;
			}
			var next = _states[stateType];
			if ( !IsPossibleChangeState( next ) ) {
				Log.Error( $"状態遷移が不可能 : {stateType}", Log.Tag.FSM );
				return;
			}
			_nextState = next;

			if ( _isActive && ( _state == null || _runState == RunState.Update ) ) {
				Log.Debug( $"{_name} ChangeStateSub" );
				ChangeStateSub().Forget();
			}
			Log.Debug( $"待機開始 {_state.GetAboutName()}.{_runState}" );
			await UniTaskUtility.WaitWhile( cancel, () => _nextState != null || _runState != RunState.Enter );
			Log.Debug( $"待機終了 {_state.GetAboutName()}.{_runState}" );
		}


		async UniTask ChangeStateSub() {
			StopAsync();

			_runState = RunState.Exit;
			if ( _activeState != ActiveState.Enabling && _state != null ) {
				await _state._exitEvent.Invoke( _asyncCanceler.Token );
			}
			if ( _activeState == ActiveState.Disabling ) {
				_activeState = ActiveState.Disabled;
				return;
			}
			if ( _nextState != null ) {
				_state = _nextState;
				_nextState = null;
			}
			if ( _activeState == ActiveState.Enabling ) {
				_activeState = ActiveState.Enabled;
			}
			_runState = RunState.Enter;
			await _state._enterEvent.Invoke( _asyncCanceler.Token );

			if ( _activeState == ActiveState.Disabling || _nextState != null ) {
				await ChangeStateSub();
			} else {
				_runState = RunState.Update;
				_state._updateEvent.Invoke( _asyncCanceler.Token ).Forget();
			}
		}


		protected virtual bool IsPossibleChangeState( TState changeState ) {
			return true;
		}


		public override string ToString() => this.ToDeepString();
	}
}