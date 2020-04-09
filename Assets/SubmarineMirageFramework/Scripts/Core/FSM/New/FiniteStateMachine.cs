//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.New {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using System.Threading;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using Process.New;
	using MultiEvent;
	using Extension;
	using Utility;
	using Debug;
	using ActiveState = Process.New.ProcessBody.ActiveState;


	// TODO : コメント追加、整頓


	public abstract class FiniteStateMachine<TFSM, TOwner, TState> : IFiniteStateMachine
		where TFSM : IFiniteStateMachine
		where TOwner : IFiniteStateMachineOwner<TFSM>
		where TState : class, IState<TFSM, TOwner>
	{
		enum RunState {
			Enter,
			Update,
			Exit,
		}
		protected TOwner _owner	{ get; private set; }
		string _name;
		public bool _isActive => _owner._isActive;
		ActiveState _activeState => _owner._body._activeState;
		RunState _runState;

		protected readonly Dictionary<Type, TState> _states = new Dictionary<Type, TState>();
		public TState _state	{ get; private set; }
		protected TState _nextState;

		CancellationTokenSource _asyncCanceler = new CancellationTokenSource();
		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public FiniteStateMachine( TOwner owner, TState[] states ) {
			_owner = owner;
			_name = this.GetAboutName();
			states.ForEach( s => _states[s.GetType()] = s );

			_owner._loadEvent.AddLast( _name, async cancel => await UniTask.WhenAll(
				_states
					.Select( pair => pair.Value )
					.Select( state => {
						state.Set( _owner );
						return state._loadEvent.Run( cancel );
					} )
			) );
			_owner._initializeEvent.AddLast( _name, async cancel => {
				await UniTask.WhenAll(
					_states
						.Select( pair => pair.Value )
						.Select( state => state._initializeEvent.Run( cancel ) )
				);
				await ChangeState( cancel, _states.First().Value.GetType() );
			} );
			_owner._finalizeEvent.AddLast( _name, async cancel => {
				StopAsync();
				await _state._exitEvent.Run( cancel );
				await _state._finalizeEvent.Run( cancel );
			} );

			_owner._enableEvent.AddLast( _name, async cancel => {
				await _state._enableEvent.Run( cancel );
			} );

			_owner._disableEvent.AddLast( _name, async cancel => {
				StopAsync();
				await _state._disableEvent.Run( cancel );
			} );

			_owner._fixedUpdateEvent.AddLast( _name )
				.Where( _ => _runState == RunState.Update )
				.Subscribe( _ => _state._fixedUpdateDeltaEvent.Run() );
			_owner._updateEvent.AddLast( _name )
				.Where( _ => _runState == RunState.Update )
				.Subscribe( _ => _state._updateDeltaEvent.Run() );
			_owner._lateUpdateEvent.AddLast( _name )
				.Where( _ => _runState == RunState.Update )
				.Subscribe( _ => _state._lateUpdateDeltaEvent.Run() );

#if DEVELOP && false
			_disposables.AddLast( Observable.EveryUpdate().Subscribe( _ => {
				DebugDisplay.s_instance.Add( $"{_owner.GetAboutName()}.{_name}" );
				DebugDisplay.s_instance.Add( $"_state : {_state.GetAboutName()}.{_runState}" );
				DebugDisplay.s_instance.Add(
					$"_nextState : {( _nextState == null ? "null" : _nextState.GetAboutName() )}"
				);
			} ) );
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
				throw new ArgumentOutOfRangeException( $"{stateType}", "未定義状態へ遷移" );
			}
			var next = _states[stateType];
			if ( !IsPossibleChangeState( next ) ) {
				throw new InvalidOperationException( $"状態遷移が不可能 : {stateType}" );
			}
			_nextState = next;

			if ( _isActive && ( _state == null || _runState == RunState.Update ) ) {
				Log.Debug( $"{_name} ChangeStateSub" );
				ChangeStateSub().Forget();
			}
			Log.Debug( $"wait start {_state.GetAboutName()}.{_runState}" );
			await UniTaskUtility.WaitWhile( cancel, () => _nextState != null || _runState != RunState.Enter );
			Log.Debug( $"wait end {_state.GetAboutName()}.{_runState}" );
		}


		async UniTask ChangeStateSub() {
			StopAsync();

			_runState = RunState.Exit;
			if ( _activeState != ActiveState.Enabling && _state != null ) {
				await _state._exitEvent.Run( _asyncCanceler.Token );
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
			await _state._enterEvent.Run( _asyncCanceler.Token );

			if ( _activeState == ActiveState.Disabling || _nextState != null ) {
				await ChangeStateSub();
			} else {
				_runState = RunState.Update;
				_state._updateEvent.Run( _asyncCanceler.Token ).Forget();
			}
		}


		protected virtual bool IsPossibleChangeState( TState changeState ) {
			return true;
		}


		public override string ToString() {
			var result = $"{this.GetAboutName()}(\n"
				+ $"    _owner : {_owner.GetAboutName()}\n"
				+ $"    _name : {_name}\n"
				+ $"    _isActive : {_isActive}\n"
				+ $"    _activeState : {_activeState}\n"
				+ $"    _runState : {_runState}\n"
				+ $"    _state : {_state}\n"
				+ $"    _nextState : {_nextState}\n"
				+ $"    _states : \n";
			_states.ForEach( pair => result += $"        {pair.Value}\n" );
			result += ")";
			return result;
		}
	}
}