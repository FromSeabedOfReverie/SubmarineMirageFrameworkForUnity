//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Base;
	using MultiEvent;
	using Task;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMFSM<TFSM, TOwner, TState> : SMStandardBase, ISMFSM
		where TFSM : ISMFSM
		where TOwner : ISMFSMOwner<TFSM>
		where TState : class, ISMState<TFSM, TOwner>
	{
		protected TOwner _owner	{ get; private set; }
		[SMHide] SMFSMRunState? _runState => _state?._runState;
		public string _registerEventName	{ get; private set; }

		public readonly Dictionary<Type, TState> _states = new Dictionary<Type, TState>();
		public TState _state	{ get; private set; }
		protected TState _nextState;
		[SMHide] protected Type _startState;

		[SMHide] public bool _isActive	=> _owner._isActive;
		[SMHide] bool _isInitialized;
		bool _isRequestNextState;
		[SMHide] public bool _isChangingState	{ get; private set; }

		[SMHide] public SMMultiAsyncEvent _selfInitializeEvent	=> _owner._selfInitializeEvent;
		[SMHide] public SMMultiAsyncEvent _initializeEvent		=> _owner._initializeEvent;
		[SMHide] public SMMultiSubject _enableEvent				=> _owner._enableEvent;
		[SMHide] public SMMultiSubject _fixedUpdateEvent		=> _owner._fixedUpdateEvent;
		[SMHide] public SMMultiSubject _updateEvent				=> _owner._updateEvent;
		[SMHide] public SMMultiSubject _lateUpdateEvent			=> _owner._lateUpdateEvent;
		[SMHide] public SMMultiSubject _disableEvent			=> _owner._disableEvent;
		[SMHide] public SMMultiAsyncEvent _finalizeEvent		=> _owner._finalizeEvent;

		[SMHide] public SMTaskCanceler _asyncCancelerOnChange	{ get; private set; } = new SMTaskCanceler();


		public SMFSM( TOwner owner, IEnumerable<TState> states, Type startState = null ) {
			_owner = owner;
			_registerEventName = this.GetAboutName();
			states.ForEach( s => _states[s.GetType()] = s );
			_startState = startState;

			_selfInitializeEvent.AddLast(_registerEventName, async canceler => {
				await _states
					.Select( pair => pair.Value )
					.Select( s => {
						s.Set(_owner);
						return s._selfInitializeEvent.Run( canceler );
					} );
			} );
			_initializeEvent.AddLast(_registerEventName, async canceler => {
				await _states
					.Select( pair => pair.Value )
					.Select( s => s._initializeEvent.Run( canceler ) );
				_isInitialized = true;
				var state = _startState ?? _states.First().Value.GetType();
				await ChangeState( state );
			} );
			_finalizeEvent.AddFirst( _registerEventName, async canceler => {
				await ChangeState( null );
				await _states
					.Select( pair => pair.Value )
					.Select( s => s._finalizeEvent.Run( canceler ) );
			} );

			_fixedUpdateEvent.AddLast( _registerEventName ).Subscribe( _ =>
				_state?.RunBehaviourStateEvent( SMTaskRunState.FixedUpdate ).Forget()
			);
			_updateEvent.AddLast( _registerEventName ).Subscribe( _ =>
				_state?.RunBehaviourStateEvent( SMTaskRunState.Update ).Forget()
			);
			_lateUpdateEvent.AddLast( _registerEventName ).Subscribe( _ =>
				_state?.RunBehaviourStateEvent( SMTaskRunState.LateUpdate ).Forget()
			);

			_enableEvent.AddLast( _registerEventName ).Subscribe( _ =>
				_state?._enableEvent.Run()
			);
			_disableEvent.AddFirst( _registerEventName ).Subscribe( _ => {
				_state?.StopActiveAsync();
				_state?._disableEvent.Run();
			} );

			_disposables.AddLast( () => {
				_asyncCancelerOnChange.Dispose();
				_states.ForEach( pair => pair.Value.Dispose() );
				_states.Clear();
				_state = null;
				_nextState = null;
			} );
		}


		public UniTask ChangeState<T>() where T : TState
			=> ChangeState( typeof( T ) );

		public async UniTask ChangeState( Type state ) {
			switch ( _owner._body._ranState ) {
				case SMTaskRunState.None:
				case SMTaskRunState.Create:
				case SMTaskRunState.SelfInitialize:
					throw new InvalidOperationException( $"初期化前の呼び出し : {_owner._body._ranState}" );
				case SMTaskRunState.Initialize:
					if ( _isInitialized )	{ break; }
					throw new InvalidOperationException( $"初期化前の呼び出し : {_owner._body._ranState}" );
				case SMTaskRunState.Finalize:
					if ( state == null )	{ break; }
					throw new InvalidOperationException( $"終了後の呼び出し : {_owner._body._ranState}" );
			}
			if ( state != null && !_states.ContainsKey( state ) ) {
				throw new ArgumentOutOfRangeException( $"{state}", "未定義状態へ遷移" );
			}
			var next = state != null ? _states[state] : null;
			if ( state != null && !IsPossibleChangeState( next ) ) {
				throw new InvalidOperationException( $"状態遷移が不可能 : {state}" );
			}

			_nextState = next;
			_isRequestNextState = true;

			if ( _isChangingState ) {
				await UTask.WaitWhile( _asyncCancelerOnChange, () => _isChangingState );
				return;
			}
			await RunChangeState();
		}

		async UniTask RunChangeState() {
			_isChangingState = true;

			if ( _state != null ) {
				_state.StopActiveAsync();
				await _state.ChangeActive( false );
				await _state.RunStateEvent( SMFSMRunState.Exit );
			}

			if ( _owner._body._ranState == SMTaskRunState.Finalize ) {
				_nextState = null;
			}
			_state = _nextState;
			_nextState = null;
			_isRequestNextState = false;
			if ( _state == null ) {
				_isChangingState = false;
				return;
			}

			await _state.RunStateEvent( SMFSMRunState.Enter );
			if ( _owner._body._ranState == SMTaskRunState.Finalize ) {
				_nextState = null;
				_isRequestNextState = true;
			}

			if ( _owner._isActive && !_isRequestNextState )	{
				await _state.ChangeActive( true );
			}
			if ( _owner._body._ranState == SMTaskRunState.Finalize ) {
				_nextState = null;
				_isRequestNextState = true;
			}

			if ( _isRequestNextState ) {
				await RunChangeState();
			} else {
				_isChangingState = false;
				_state.RunStateEvent( SMFSMRunState.Update ).Forget();
			}
		}


		protected virtual bool IsPossibleChangeState( TState changeState ) {
			return true;
		}


		public override void SetToString() {
			base.SetToString();
			_toStringer.SetValue( nameof( _owner ), i => _owner.GetAboutName() );
			_toStringer.SetValue( nameof( _states ), i => {
				var arrayI = StringSMUtility.IndentSpace( i + 1 );
				return "\n" + string.Join( ",\n", _states.Select( pair =>
					$"{arrayI}{pair.Key} : {pair.Value.ToLineString()}"
				) );
			} );
		}
	}
}