//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.New {
	using System;
	using System.Threading;
	using UniRx.Async;
	using MultiEvent;
	using Process.New;
	using Extension;
	using Debug;
	using RanState = Process.New.ProcessBody.RanState;
	using ActiveState = Process.New.ProcessBody.ActiveState;


	// TODO : コメント追加、整頓


	public abstract class State<TFSM, TOwner> : IState<TFSM, TOwner>
		where TFSM : IFiniteStateMachine
		where TOwner : IFiniteStateMachineOwner<TFSM>
	{
		public TFSM _fsm		{ get; private set; }
		public TOwner _owner	{ get; private set; }
		public bool _isActive => _owner._isActive;
		public FiniteStateMachineRunState _runState	{ get; private set; } = FiniteStateMachineRunState.Exit;

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

		CancellationTokenSource _internalActiveAsyncCanceler = new CancellationTokenSource();
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
				_internalActiveAsyncCanceler.Cancel();
				_internalActiveAsyncCanceler.Dispose();
				_activeAsyncCanceler.Cancel();
				_activeAsyncCanceler.Dispose();
			} );
		}

		public void Set( TOwner owner ) {
			_fsm = owner._fsm;
			_owner = owner;
			StopActiveAsync();
		}

		public void Dispose() => _disposables.Dispose();

		~State() => Dispose();


		public void StopActiveAsync() {
			Log.Debug( "StopActiveAsync()" );
			_disposables.Remove( "_activeAsyncCanceler" );
			_internalActiveAsyncCanceler = new CancellationTokenSource();
			_activeAsyncCanceler = _internalActiveAsyncCanceler.Token.Add( _owner._activeAsyncCancel );
			SetActiveAsyncCancelerDisposable();
		}


		public async UniTask RunStateEvent( FiniteStateMachineRunState state ) {
			switch ( state ) {
				case FiniteStateMachineRunState.Enter:
					_runState = FiniteStateMachineRunState.Enter;
					await _enterEvent.Run( _fsm._changeStateAsyncCancel );
					return;

				case FiniteStateMachineRunState.Update:
					_runState = FiniteStateMachineRunState.Update;
					if ( _isActive ) {
						Log.Debug( "Run _updateEvent" );
						await _updateEvent.Run( _activeAsyncCancel );
					}
					return;

				case FiniteStateMachineRunState.Exit:
					_runState = FiniteStateMachineRunState.Exit;
					StopActiveAsync();
					await _exitEvent.Run( _fsm._changeStateAsyncCancel );
					return;
			}
		}


		public async UniTask RunProcessStateEvent( RanState state, CancellationToken cancel = default ) {
			switch ( state ) {
				case RanState.Loading:
					await _loadEvent.Run( cancel );
					return;

				case RanState.Initializing:
					await _initializeEvent.Run( cancel );
					return;

				case RanState.FixedUpdate:
					if ( _runState == FiniteStateMachineRunState.Update ) {
						_fixedUpdateDeltaEvent.Run();
					}
					return;

				case RanState.Update:
					if ( _runState == FiniteStateMachineRunState.Update ) {
						_updateDeltaEvent.Run();
					}
					return;

				case RanState.LateUpdate:
					if ( _runState == FiniteStateMachineRunState.Update ) {
						_lateUpdateDeltaEvent.Run();
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
					throw new ArgumentOutOfRangeException(
						$"{state}", $"実行不可能な型を指定した為、実行不可" );
			}
		}


		public async UniTask RunProcessActiveEvent( ActiveState state, CancellationToken cancel ) {
			switch ( state ) {
				case ActiveState.Enabling:
					await _enableEvent.Run( cancel );
					if ( _runState == FiniteStateMachineRunState.Update ) {
						Log.Debug( "Run _updateEvent" );
// TODO : _activeAsyncCancelが、owner由来でキャンセルされた場合、リセットされず即停止する
						_updateEvent.Run( _activeAsyncCancel ).Forget();
					}
					return;

				case ActiveState.Disabling:
					StopActiveAsync();
					await _disableEvent.Run( cancel );
					return;

				case ActiveState.Enabled:
				case ActiveState.Disabled:
					throw new ArgumentOutOfRangeException(
						$"{state}", $"実行不可能な型を指定した為、実行不可" );
			}
		}


		public override string ToString()
			=> $"{this.GetAboutName()}( _runState : {_runState} )";
	}
}