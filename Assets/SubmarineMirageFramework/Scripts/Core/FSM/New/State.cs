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
	using Process.New;
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


		async UniTask WaitChangeActive() {
			switch ( _owner._body._activeState ) {
				case ActiveState.Enabling:
				case ActiveState.Disabling:
					var lastActiveState = _owner._body._activeState;
					Log.Debug( "wait owner change active" );
					await UniTaskUtility.WaitWhile(
						_fsm._changeStateAsyncCancel, () => lastActiveState == _owner._body._activeState );
					Log.Debug( "end wait owner change active" );
					break;
			}
		}


		public async UniTask RunStateEvent( RunState state ) {
			Log.Debug( $"call RunStateEvent : {state}" );

			switch ( state ) {
				case RunState.Entering:
					switch ( _runState ) {
						case RunState.Exited:
							await WaitChangeActive();
							switch ( _runState ) {
								case RunState.Entering:
								case RunState.Entered:
									return;
							}
							Log.Debug( "RunStateEvent : Entering" );
							_runState = RunState.Entering;
							try {
								await _enterEvent.Run( _fsm._changeStateAsyncCancel );
							} catch {
								_runState = RunState.Exited;
								throw;
							}
							_runState = RunState.Entered;
							if ( _owner._isActive ) {
								await ChangeActive( _fsm._changeStateAsyncCancel, true, false );
							}
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
							StopActiveAsync();
							var lastRunState = _runState;
							// 非同期停止時に、catchで状態が変わる為、1フレーム待機
							await UniTaskUtility.Yield( _fsm._changeStateAsyncCancel );
							await ChangeActive( _fsm._changeStateAsyncCancel, false, false );
							switch ( _runState ) {
								case RunState.Exiting:
								case RunState.Exited:
									return;
							}
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


		public async UniTask ChangeActive( CancellationToken cancel, bool isActive, bool isWait ) {
			_nextActiveState = isActive ? ActiveState.Enabling : ActiveState.Disabling;
			await RunActiveEvent( cancel, isWait );
		}

		async UniTask RunActiveEvent( CancellationToken cancel, bool isWait ) {
			if ( !_nextActiveState.HasValue )		{ return; }

			if ( isWait && _runState != RunState.Update && _runState != RunState.BeforeUpdate ) {
				Log.Debug( "wait _isChangingState" );
				await UniTaskUtility.WaitWhile( cancel,
					() => isWait && _runState != RunState.Update && _runState != RunState.BeforeUpdate );
				Log.Debug( "end wait _isChangingState" );
			}

			switch ( _nextActiveState ) {
				case ActiveState.Enabling:
					Log.Debug( $"call {this.GetAboutName()}.{ActiveState.Enabling}" );
					switch ( _activeState ) {
						case ActiveState.Enabling:
							_nextActiveState = null;
							await UniTaskUtility.WaitWhile(
								cancel, () => _activeState == ActiveState.Enabling );
							return;

						case ActiveState.Enabled:
							_nextActiveState = null;
							return;

						case ActiveState.Disabling:
							await UniTaskUtility.WaitWhile(
								cancel, () => _activeState == ActiveState.Disabling );
							await RunActiveEvent( cancel, isWait );
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
							await UniTaskUtility.WaitWhile(
								cancel, () => _activeState == ActiveState.Disabling );
							return;

						case ActiveState.Disabled:
							_nextActiveState = null;
							return;

						case ActiveState.Enabling:
							await UniTaskUtility.WaitWhile(
								cancel, () => _activeState == ActiveState.Enabling );
							await RunActiveEvent( cancel, isWait );
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


		public async UniTask RunProcessStateEvent( CancellationToken cancel, RanState state ) {
			switch ( state ) {
				case RanState.Loading:
					await _loadEvent.Run( cancel );
					return;

				case RanState.Initializing:
					await _initializeEvent.Run( cancel );
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
					await _finalizeEvent.Run( cancel );
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