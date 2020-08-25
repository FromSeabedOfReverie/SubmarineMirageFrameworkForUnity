//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using System;
	using Cysharp.Threading.Tasks;
	using MultiEvent;
	using UTask;
	using SMTask;
	using Extension;
	using RunState = FiniteStateMachineRunState;


	// TODO : コメント追加、整頓


	public abstract class State<TFSM, TOwner> : IState<TFSM, TOwner>
		where TFSM : IFiniteStateMachine
		where TOwner : IFiniteStateMachineOwner<TFSM>
	{
		public TFSM _fsm		{ get; private set; }
		public TOwner _owner	{ get; private set; }

		public RunState _runState	{ get; private set; } = RunState.Exited;
		SMTaskActiveState _activeState;
		SMTaskActiveState? _nextActiveState;

		public bool _isActive =>	_activeState == SMTaskActiveState.Enabled;
		public bool _isDispose =>	_disposables._isDispose;

		protected readonly MultiAsyncEvent _loadEvent = new MultiAsyncEvent();
		protected readonly MultiAsyncEvent _initializeEvent = new MultiAsyncEvent();
		protected readonly MultiAsyncEvent _enableEvent = new MultiAsyncEvent();
		protected readonly MultiAsyncEvent _enterEvent = new MultiAsyncEvent();
		protected readonly MultiAsyncEvent _updateEvent = new MultiAsyncEvent();
		protected readonly MultiSubject _fixedUpdateDeltaEvent = new MultiSubject();
		protected readonly MultiSubject _updateDeltaEvent = new MultiSubject();
		protected readonly MultiSubject _lateUpdateDeltaEvent = new MultiSubject();
		protected readonly MultiAsyncEvent _exitEvent = new MultiAsyncEvent();
		protected readonly MultiAsyncEvent _disableEvent = new MultiAsyncEvent();
		protected readonly MultiAsyncEvent _finalizeEvent = new MultiAsyncEvent();

		public UTaskCanceler _activeAsyncCanceler	{ get; private set; } = new UTaskCanceler();

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public State() {
			// TODO : 代入物が変わる場合、_disposables.AddLast( _activeAsyncCanceler );で破棄できないか、確認する
			_disposables.AddLast( () => _activeAsyncCanceler.Dispose() );
			_disposables.AddLast(
				_loadEvent,
				_initializeEvent,
				_enableEvent,
				_enterEvent,
				_updateEvent,
				_fixedUpdateDeltaEvent,
				_updateDeltaEvent,
				_lateUpdateDeltaEvent,
				_exitEvent,
				_disableEvent,
				_finalizeEvent
			);
		}

		public void Set( TOwner owner ) {
			_fsm = owner._fsm;
			_owner = owner;

			_activeAsyncCanceler.Dispose();
			_activeAsyncCanceler = _owner._activeAsyncCanceler.CreateChild();
		}

		public void Dispose() => _disposables.Dispose();

		~State() => Dispose();


		public void StopActiveAsync() => _activeAsyncCanceler.Cancel();


		public async UniTask RunStateEvent( RunState state ) {
			switch ( state ) {
				case RunState.Entering:
					switch ( _runState ) {
						case RunState.Exited:
							_runState = RunState.Entering;
							try {
								await _enterEvent.Run( _fsm._changeStateAsyncCanceler );
							} catch {
								_runState = RunState.Exited;
								throw;
							}
							_runState = RunState.Entered;
							return;
					}
					return;

				case RunState.Update:
					switch ( _runState ) {
						case RunState.Entered:
						case RunState.BeforeUpdate:
							_runState = RunState.BeforeUpdate;
							if ( _owner._isActive ) {
								_runState = RunState.Update;
								await _updateEvent.Run( _activeAsyncCanceler );
							}
							return;
					}
					return;

				case RunState.Exiting:
					switch ( _runState ) {
						case RunState.Entered:
						case RunState.BeforeUpdate:
						case RunState.Update:
							var lastRunState = _runState;
							_runState = RunState.Exiting;
							try {
								await _exitEvent.Run( _fsm._changeStateAsyncCanceler );
							} catch {
								_runState = lastRunState;
								throw;
							}
							_runState = RunState.Exited;
							return;
					}
					return;

				case RunState.Entered:
				case RunState.BeforeUpdate:
				case RunState.Exited:
					throw new ArgumentOutOfRangeException( $"{state}", $"実行不可能な型を指定" );
			}
		}


		public async UniTask ChangeActive( bool isActive ) {
			_nextActiveState = isActive ? SMTaskActiveState.Enabling : SMTaskActiveState.Disabling;
			await RunActiveEvent();
		}

		async UniTask RunActiveEvent() {
			if ( !_nextActiveState.HasValue )	{ return; }

			var canceler = (
				_fsm._isChangingState							? _fsm._changeStateAsyncCanceler :
				_nextActiveState == SMTaskActiveState.Enabling	? _owner._activeAsyncCanceler :
				_nextActiveState == SMTaskActiveState.Disabling	? _owner._inActiveAsyncCanceler
																: default
			);

			switch ( _nextActiveState ) {
				case SMTaskActiveState.Enabling:
					switch ( _activeState ) {
						case SMTaskActiveState.Enabling:
							_nextActiveState = null;
							await UTask.WaitWhile( canceler, () => _activeState == SMTaskActiveState.Enabling );
							return;

						case SMTaskActiveState.Enabled:
							_nextActiveState = null;
							return;

						case SMTaskActiveState.Disabling:
							await UTask.WaitWhile( canceler, () => _activeState == SMTaskActiveState.Disabling );
							await RunActiveEvent();
							return;

						case SMTaskActiveState.Disabled:
							_nextActiveState = null;
							_activeState = SMTaskActiveState.Enabling;
							try {
								await _enableEvent.Run( canceler );
							} catch {
								_activeState = SMTaskActiveState.Disabled;
								throw;
							}
							_activeState = SMTaskActiveState.Enabled;
							if ( _runState == RunState.Update )	{ _runState = RunState.BeforeUpdate; }
							return;
					}
					return;

				case SMTaskActiveState.Disabling:
					switch ( _activeState ) {
						case SMTaskActiveState.Disabling:
							_nextActiveState = null;
							await UTask.WaitWhile( canceler, () => _activeState == SMTaskActiveState.Disabling );
							return;

						case SMTaskActiveState.Disabled:
							_nextActiveState = null;
							return;

						case SMTaskActiveState.Enabling:
							await UTask.WaitWhile( canceler, () => _activeState == SMTaskActiveState.Enabling );
							await RunActiveEvent();
							return;

						case SMTaskActiveState.Enabled:
							_nextActiveState = null;
							_activeState = SMTaskActiveState.Disabling;
							try {
								await _disableEvent.Run( canceler );
							} catch {
								_activeState = SMTaskActiveState.Enabled;
								throw;
							}
							_activeState = SMTaskActiveState.Disabled;
							return;
					}
					return;

				case SMTaskActiveState.Enabled:
				case SMTaskActiveState.Disabled:
					throw new ArgumentOutOfRangeException( $"{_nextActiveState}", $"実行不可能な型を指定" );
			}
		}


		public async UniTask RunBehaviourStateEvent( SMTaskRanState state ) {
			switch ( state ) {
				case SMTaskRanState.Loading:
					await _loadEvent.Run( _owner._activeAsyncCanceler );
					return;

				case SMTaskRanState.Initializing:
					await _initializeEvent.Run( _owner._activeAsyncCanceler );
					return;

				case SMTaskRanState.FixedUpdate:
					if ( !_isActive )	{ return; }
					switch ( _runState ) {
						case RunState.BeforeUpdate:
						case RunState.Update:
							_fixedUpdateDeltaEvent.Run();
							return;
					}
					return;

				case SMTaskRanState.Update:
					if ( !_isActive )	{ return; }
					switch ( _runState ) {
						case RunState.BeforeUpdate:
						case RunState.Update:
							RunStateEvent( RunState.Update ).Forget();
							_updateDeltaEvent.Run();
							return;
					}
					return;

				case SMTaskRanState.LateUpdate:
					if ( !_isActive )	{ return; }
					switch ( _runState ) {
						case RunState.BeforeUpdate:
						case RunState.Update:
							_lateUpdateDeltaEvent.Run();
							return;
					}
					return;

				case SMTaskRanState.Finalizing:
					await _finalizeEvent.Run( _owner._inActiveAsyncCanceler );
					return;

				case SMTaskRanState.None:
				case SMTaskRanState.Creating:
				case SMTaskRanState.Created:
				case SMTaskRanState.Loaded:
				case SMTaskRanState.Initialized:
				case SMTaskRanState.Finalized:
					throw new ArgumentOutOfRangeException( $"{state}", $"実行不可能な型を指定" );
			}
		}


		public override string ToString()
			=> $"{this.GetAboutName()}( {_runState}, {_activeState}, next:{_nextActiveState} )";
	}
}