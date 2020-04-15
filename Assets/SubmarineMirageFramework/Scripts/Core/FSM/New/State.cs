//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.New {
	using System;
	using System.Threading;
	using UniRx;
	using UniRx.Async;
	using MultiEvent;
	using Extension;
	using Utility;
	using Debug;
	using RunState = FiniteStateMachineRunState;
	using RanState = Process.New.ProcessBody.RanState;
	using ActiveState = Process.New.ProcessBody.ActiveState;


	// TODO : コメント追加、整頓


	public abstract class State<TFSM, TOwner> : IState<TFSM, TOwner>
		where TFSM : IFiniteStateMachine
		where TOwner : IFiniteStateMachineOwner<TFSM>
	{
		public TFSM _fsm		{ get; private set; }
		public TOwner _owner	{ get; private set; }

		public RunState _runState	{ get; private set; } = RunState.Exited;
		ActiveState _activeState;
		ActiveState? _nextActiveState;
		public bool _isActive => _activeState == ActiveState.Enabled;

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

		CancellationTokenSource _activeAsyncCanceler = new CancellationTokenSource();
		public CancellationToken _activeAsyncCancel => _activeAsyncCanceler.Token;

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public State() {
			SetActiveAsyncCancelerDisposable();
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

		void SetActiveAsyncCancelerDisposable() {
			_disposables.AddFirst( "_activeAsyncCanceler", () => {
				_activeAsyncCanceler.Cancel();
				_activeAsyncCanceler.Dispose();
			} );
		}

		public void Set( TOwner owner ) {
			_fsm = owner._fsm;
			_owner = owner;
			StopActiveAsync();
			_owner._activeAsyncCancelEvent.AddLast().Subscribe( _ => StopActiveAsync() );
		}

		public void Dispose() => _disposables.Dispose();

		~State() => Dispose();


		public void StopActiveAsync() {
			Log.Debug( $"{this.GetAboutName()}.StopActiveAsync()" );
			_disposables.Remove( "_activeAsyncCanceler" );
			using ( var canceler = new CancellationTokenSource() ) {
				_activeAsyncCanceler = canceler.Token.Add( _owner._activeAsyncCancel );
			}
			SetActiveAsyncCancelerDisposable();
		}


		public async UniTask RunStateEvent( RunState state ) {
			Log.Debug( $"call RunStateEvent : {state}" );

			switch ( state ) {
				case RunState.Entering:
					switch ( _runState ) {
						case RunState.Exited:
							Log.Debug( "RunStateEvent : Entering" );
							_runState = RunState.Entering;
							try {
								await _enterEvent.Run( _fsm._changeStateAsyncCancel );
							} catch {
								_runState = RunState.Exited;
								throw;
							}
							_runState = RunState.Entered;
							Log.Debug( "end RunStateEvent : Entering" );
							return;
					}
					return;

				case RunState.Update:
					switch ( _runState ) {
						case RunState.Entered:
						case RunState.BeforeUpdate:
							Log.Debug( "RunStateEvent : Update" );
							_runState = RunState.BeforeUpdate;
							if ( _owner._isActive ) {
								Log.Debug( "Run _updateEvent" );
								_runState = RunState.Update;
								await _updateEvent.Run( _activeAsyncCancel );
							}
							Log.Debug( "end RunStateEvent : Update" );
							return;
					}
					return;

				case RunState.Exiting:
					switch ( _runState ) {
						case RunState.Entered:
						case RunState.BeforeUpdate:
						case RunState.Update:
							Log.Debug( "RunStateEvent : Exiting" );
							var lastRunState = _runState;
							_runState = RunState.Exiting;
							try {
								await _exitEvent.Run( _fsm._changeStateAsyncCancel );
							} catch {
								_runState = lastRunState;
								throw;
							}
							_runState = RunState.Exited;
							Log.Debug( "end RunStateEvent : Exiting" );
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
			_nextActiveState = isActive ? ActiveState.Enabling : ActiveState.Disabling;
			await RunActiveEvent();
		}

		async UniTask RunActiveEvent() {
			if ( !_nextActiveState.HasValue )	{ return; }

			var cancel = (
				_fsm._isChangingState						? _fsm._changeStateAsyncCancel :
				_nextActiveState == ActiveState.Enabling	? _owner._activeAsyncCancel :
				_nextActiveState == ActiveState.Disabling	? _owner._inActiveAsyncCancel
															: default
			);

			switch ( _nextActiveState ) {
				case ActiveState.Enabling:
					Log.Debug( $"call {this.GetAboutName()}.{ActiveState.Enabling}" );
					switch ( _activeState ) {
						case ActiveState.Enabling:
							_nextActiveState = null;
							await UniTaskUtility.WaitWhile( cancel, () => _activeState == ActiveState.Enabling );
							return;

						case ActiveState.Enabled:
							_nextActiveState = null;
							return;

						case ActiveState.Disabling:
							await UniTaskUtility.WaitWhile( cancel, () => _activeState == ActiveState.Disabling );
							await RunActiveEvent();
							return;

						case ActiveState.Disabled:
							Log.Debug( $"run {this.GetAboutName()}.{ActiveState.Enabling}" );
							_nextActiveState = null;
							_activeState = ActiveState.Enabling;
							try {
								await _enableEvent.Run( cancel );
							} catch {
								_activeState = ActiveState.Disabled;
								throw;
							}
							_activeState = ActiveState.Enabled;
							if ( _runState == RunState.Update )	{ _runState = RunState.BeforeUpdate; }
							return;
					}
					return;

				case ActiveState.Disabling:
					Log.Debug( $"call {this.GetAboutName()}.{ActiveState.Disabling}" );
					switch ( _activeState ) {
						case ActiveState.Disabling:
							_nextActiveState = null;
							await UniTaskUtility.WaitWhile( cancel, () => _activeState == ActiveState.Disabling );
							return;

						case ActiveState.Disabled:
							_nextActiveState = null;
							return;

						case ActiveState.Enabling:
							await UniTaskUtility.WaitWhile( cancel, () => _activeState == ActiveState.Enabling );
							await RunActiveEvent();
							return;

						case ActiveState.Enabled:
							Log.Debug( $"run {this.GetAboutName()}.{ActiveState.Disabling}" );
							_nextActiveState = null;
							_activeState = ActiveState.Disabling;
							try {
								await _disableEvent.Run( cancel );
							} catch {
								_activeState = ActiveState.Enabled;
								throw;
							}
							_activeState = ActiveState.Disabled;
							return;
					}
					return;

				case ActiveState.Enabled:
				case ActiveState.Disabled:
					throw new ArgumentOutOfRangeException( $"{_nextActiveState}", $"実行不可能な型を指定" );
			}
		}


		public async UniTask RunProcessStateEvent( RanState state ) {
			switch ( state ) {
				case RanState.Loading:
					await _loadEvent.Run( _owner._activeAsyncCancel );
					return;

				case RanState.Initializing:
					await _initializeEvent.Run( _owner._activeAsyncCancel );
					return;

				case RanState.FixedUpdate:
					if ( !_isActive )	{ return; }
					switch ( _runState ) {
						case RunState.BeforeUpdate:
						case RunState.Update:
							_fixedUpdateDeltaEvent.Run();
							return;
					}
					return;

				case RanState.Update:
					if ( !_isActive )	{ return; }
					switch ( _runState ) {
						case RunState.BeforeUpdate:
						case RunState.Update:
							RunStateEvent( RunState.Update ).Forget();
							_updateDeltaEvent.Run();
							return;
					}
					return;

				case RanState.LateUpdate:
					if ( !_isActive )	{ return; }
					switch ( _runState ) {
						case RunState.BeforeUpdate:
						case RunState.Update:
							_lateUpdateDeltaEvent.Run();
							return;
					}
					return;

				case RanState.Finalizing:
					await _finalizeEvent.Run( _owner._inActiveAsyncCancel );
					return;

				case RanState.None:
				case RanState.Creating:
				case RanState.Created:
				case RanState.Loaded:
				case RanState.Initialized:
				case RanState.Finalized:
					throw new ArgumentOutOfRangeException( $"{state}", $"実行不可能な型を指定" );
			}
		}


		public override string ToString()
			=> $"{this.GetAboutName()}( {_runState}, {_activeState}, next:{_nextActiveState} )";
	}
}