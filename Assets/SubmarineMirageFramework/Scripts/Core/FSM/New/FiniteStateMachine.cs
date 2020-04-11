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
	using MultiEvent;
	using Process.New;
	using Extension;
	using Utility;
	using Debug;
	using RunState = FiniteStateMachineRunState;
	using RanState = Process.New.ProcessBody.RanState;
	using ActiveState = Process.New.ProcessBody.ActiveState;


	// TODO : コメント追加、整頓


	public abstract class FiniteStateMachine<TFSM, TOwner, TState> : IFiniteStateMachine
		where TFSM : IFiniteStateMachine
		where TOwner : IFiniteStateMachineOwner<TFSM>
		where TState : class, IState<TFSM, TOwner>
	{
		protected TOwner _owner	{ get; private set; }
		public bool _isActive => _owner._isActive;
		RunState? _runState => _state?._runState;
		public string _registerEventName	{ get; private set; }

		protected readonly Dictionary<Type, TState> _states = new Dictionary<Type, TState>();
		public TState _state	{ get; private set; }
		protected TState _nextState;
		bool _isRequestNextState;
		bool _isChangingState => _state != null && _runState != RunState.Update;

		public MultiAsyncEvent _loadEvent		=> _owner._loadEvent;
		public MultiAsyncEvent _initializeEvent	=> _owner._initializeEvent;
		public MultiAsyncEvent _enableEvent		=> _owner._enableEvent;
		public MultiSubject _fixedUpdateEvent	=> _owner._fixedUpdateEvent;
		public MultiSubject _updateEvent		=> _owner._updateEvent;
		public MultiSubject _lateUpdateEvent	=> _owner._lateUpdateEvent;
		public MultiAsyncEvent _disableEvent	=> _owner._disableEvent;
		public MultiAsyncEvent _finalizeEvent	=> _owner._finalizeEvent;

		CancellationTokenSource _changeStateAsyncCanceler = new CancellationTokenSource();
		public CancellationToken _changeStateAsyncCancel => _changeStateAsyncCanceler.Token;

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public FiniteStateMachine( TOwner owner, TState[] states, Type startState = null ) {
			_owner = owner;
			_registerEventName = this.GetAboutName();
			states.ForEach( s => _states[s.GetType()] = s );

			_loadEvent.AddLast( _registerEventName, async cancel => {
				await UniTask.WhenAll(
					_states
						.Select( pair => pair.Value )
						.Select( state => {
							state.Set( _owner );
							return state.RunProcessStateEvent( RanState.Loading, cancel );
						} )
				);
			} );
			_initializeEvent.AddLast( _registerEventName, async cancel => {
				await UniTask.WhenAll(
					_states.Select( pair => pair.Value.RunProcessStateEvent( RanState.Initializing, cancel ) )
				);
				var start = startState ?? _states.First().Value.GetType();
				await ChangeState( start );
			} );
			_finalizeEvent.AddFirst( _registerEventName, async cancel => {
				await ChangeState( null );
				Log.Debug( "_state is null and stop _updateEvent" );
				await UniTask.WhenAll(
					_states.Select( pair => pair.Value.RunProcessStateEvent( RanState.Finalizing, cancel ) )
				);
			} );

			_fixedUpdateEvent.AddLast( _registerEventName ).Subscribe( _ =>
				_state?.RunProcessStateEvent( RanState.FixedUpdate ).Forget()
			);
			_updateEvent.AddLast( _registerEventName ).Subscribe( _ =>
				_state?.RunProcessStateEvent( RanState.Update ).Forget()
			);
			_lateUpdateEvent.AddLast( _registerEventName ).Subscribe( _ =>
				_state?.RunProcessStateEvent( RanState.LateUpdate ).Forget()
			);

// TODO : 状態遷移中に、活動状態変更すると、別の状態のenable、disableが呼ばれる為、双方を待機
			_enableEvent.AddLast( _registerEventName, async cancel => {
				await UniTaskUtility.WaitWhile( cancel, () => _isChangingState );
				if ( _state != null ) {
					await _state.RunProcessActiveEvent( ActiveState.Enabling, cancel );
				}
			} );
			_disableEvent.AddFirst( _registerEventName, async cancel => {
				await UniTaskUtility.WaitWhile( cancel, () => _isChangingState );
				if ( _state != null ) {
					await _state.RunProcessActiveEvent( ActiveState.Disabling, cancel );
				}
			} );

			_disposables.AddFirst( () => {
				_changeStateAsyncCanceler.Cancel();
				_changeStateAsyncCanceler.Dispose();
			} );
			_disposables.AddLast( () => {
				_states.ForEach( pair => pair.Value.Dispose() );
				_states.Clear();
				_state = null;
				_nextState = null;
			} );

#if DEVELOP && false
			_disposables.AddLast( Observable.EveryUpdate().Subscribe( _ => {
				DebugDisplay.s_instance.Add( $"{_owner.GetAboutName()}.{_registerEventName}" );
				DebugDisplay.s_instance.Add( $"_state : {_state?.GetAboutName()}.{_runState}" );
				DebugDisplay.s_instance.Add(
					$"_nextState : {_nextState?.GetAboutName()}"
				);
			} ) );
#endif
		}

		public void Dispose() => _disposables.Dispose();

		~FiniteStateMachine() => Dispose();


		public async UniTask ChangeState<T>() where T : TState
			=> await ChangeState( typeof( T ) );

		public async UniTask ChangeState( Type state ) {
			if ( state != null && !_states.ContainsKey( state ) ) {
				throw new ArgumentOutOfRangeException( $"{state}", "未定義状態へ遷移" );
			}
			var next = state != null ? _states[state] : null;
			if ( state != null && !IsPossibleChangeState( next ) ) {
				throw new InvalidOperationException( $"状態遷移が不可能 : {state}" );
			}
			switch ( _owner._body._ranState ) {
				case RanState.None:
				case RanState.Creating:
				case RanState.Created:
				case RanState.Loading:
				case RanState.Loaded:
					throw new InvalidOperationException( $"初期化前の呼び出し : {_owner._body._ranState}" );
				case RanState.Finalized:
					throw new InvalidOperationException( $"終了後の呼び出し : {_owner._body._ranState}" );
			}
			_nextState = next;
			_isRequestNextState = true;
			await RunChangeState( true );
		}

		async UniTask RunChangeState( bool isWait ) {
			Log.Debug( $"call RunChangeState()" );
			if ( isWait && _isChangingState ) {
				Log.Debug( $"wait {_state?.GetAboutName()}.{_runState}" );
				await UniTaskUtility.WaitWhile( _changeStateAsyncCancel, () => _isChangingState );
				Log.Debug( $"end wait {_state?.GetAboutName()}.{_runState}" );
				return;
			}
// TODO : 状態遷移中に、活動状態変更すると、別の状態のenable、disableが呼ばれる為、双方を待機
			switch( _owner._body._activeState ) {
				case ActiveState.Enabling:
				case ActiveState.Disabling:
					var lastActiveState = _owner._body._activeState;
					await UniTaskUtility.WaitWhile(
						_changeStateAsyncCancel, () => lastActiveState == _owner._body._activeState );
					break;
			}

			if ( _state != null ) {
				await _state.RunStateEvent( RunState.Exit );
			}
			_state = _nextState;
			_nextState = null;
			_isRequestNextState = false;
			if ( _state == null || _owner._body._ranState == RanState.Finalizing ) {
				Log.Debug( "set state null" );
				return;
			}
			await _state.RunStateEvent( RunState.Enter );

			if ( _state != null && _owner._body._ranState == RanState.Finalizing ) {
				_nextState = null;
				_isRequestNextState = true;
			}

			if ( _isRequestNextState ) {
				Log.Debug( "restart RunChangeState()" );
				await RunChangeState( false );
			} else {
				_state.RunStateEvent( RunState.Update ).Forget();
			}
		}


		protected virtual bool IsPossibleChangeState( TState changeState ) {
			return true;
		}


		public override string ToString() {
			var result = $"{this.GetAboutName()}(\n"
				+ $"    _owner : {_owner.GetAboutName()}\n"
				+ $"    _registerEventName : {_registerEventName}\n"
				+ $"    _state : {_state}\n"
				+ $"    _nextState : {_nextState}\n"
				+ $"    _isRequestNextState : {_isRequestNextState}\n"
				+ $"    _states : \n";
			_states.ForEach( pair => result += $"        {pair.Value}\n" );
			result += ")";
			return result;
		}
	}
}