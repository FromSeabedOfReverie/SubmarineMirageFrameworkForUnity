//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTask
namespace SubmarineMirage.SMTask {
	using System;
	using Cysharp.Threading.Tasks;
	using MultiEvent;
	using UTask;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMBehaviourBody : IDisposableExtension {
		static uint s_idCount;
		public uint _id			{ get; private set; }

		public SMTaskRanState _ranState				{ get; private set; }
		public SMTaskActiveState _activeState		{ get; private set; }
		public SMTaskActiveState? _nextActiveState	{ get; private set; }

		public bool _isInitialized =>	_ranState >= SMTaskRanState.Initialized;
		public bool _isActive =>		_activeState == SMTaskActiveState.Enabled;

		ISMBehaviour _owner;

		public readonly MultiAsyncEvent _loadEvent = new MultiAsyncEvent();
		public readonly MultiAsyncEvent _initializeEvent = new MultiAsyncEvent();
		public readonly MultiAsyncEvent _enableEvent = new MultiAsyncEvent();
		public readonly MultiSubject _fixedUpdateEvent = new MultiSubject();
		public readonly MultiSubject _updateEvent = new MultiSubject();
		public readonly MultiSubject _lateUpdateEvent = new MultiSubject();
		public readonly MultiAsyncEvent _disableEvent = new MultiAsyncEvent();
		public readonly MultiAsyncEvent _finalizeEvent = new MultiAsyncEvent();

		public readonly UTaskCanceler _activeAsyncCanceler = new UTaskCanceler();
		public readonly UTaskCanceler _inActiveAsyncCanceler = new UTaskCanceler();

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public SMBehaviourBody( ISMBehaviour owner, SMTaskActiveState nextActiveState ) {
			_id = ++s_idCount;

			_owner = owner;
			_nextActiveState = nextActiveState;

			_disposables.AddLast(
				_activeAsyncCanceler,
				_inActiveAsyncCanceler
			);
			_disposables.AddLast(
				_loadEvent,
				_initializeEvent,
				_enableEvent,
				_fixedUpdateEvent,
				_updateEvent,
				_lateUpdateEvent,
				_disableEvent,
				_finalizeEvent
			);
			_disposables.AddLast( () => UnLink() );
#if TestSMTask
			_disposables.AddLast( () =>
				Log.Debug( $"{nameof( SMBehaviourBody )}.{nameof( Dispose )} : {this}" )
			);
			Log.Debug( $"{nameof( SMBehaviourBody )}() : {this}" );
#endif
		}

		~SMBehaviourBody() => Dispose();

		public void Dispose() => _disposables.Dispose();

		void UnLink() {
#if TestSMTask
			Log.Debug( $"{nameof( SMBehaviourBody )}.{nameof( UnLink )} : start\n{_owner}" );
#endif
			if ( _owner._previous != null )	{ _owner._previous._next = _owner._next; }
			if ( _owner._next != null )		{ _owner._next._previous = _owner._previous; }
			_owner._previous = null;
			_owner._next = null;
#if TestSMTask
			Log.Debug( $"{nameof( SMBehaviourBody )}.{nameof( UnLink )} : end\n{_owner}" );
#endif
		}


		public void StopActiveAsync() => _activeAsyncCanceler.Cancel();


		public async UniTask RunStateEvent( SMTaskRanState state ) {
			switch ( state ) {
				case SMTaskRanState.Creating:
					if ( _activeState != SMTaskActiveState.Disabled )	{ return; }
					switch ( _ranState ) {
						case SMTaskRanState.None:
#if TestSMTask
							Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}\n{this}" );
#endif
							_ranState = SMTaskRanState.Creating;
							try {
// TODO : awaitが不要な事を、FSM等実装後に確認後、状態をCreateのみに、修正
//								await UTask.NextFrame( _activeAsyncCanceler );
								_owner.Create();
							} catch {
								_ranState = SMTaskRanState.None;
								throw;
							}
							_ranState = SMTaskRanState.Created;
							return;
					}
					return;

				case SMTaskRanState.Loading:
					if ( _owner._type == SMTaskType.DontWork )				{ return; }
					if ( _activeState != SMTaskActiveState.Disabled )		{ return; }
					if ( _nextActiveState != SMTaskActiveState.Enabling )	{ return; }
					switch ( _ranState ) {
						case SMTaskRanState.Created:
#if TestSMTask
							Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}\n{this}" );
#endif
							_ranState = SMTaskRanState.Loading;
							try {
								await _loadEvent.Run( _activeAsyncCanceler );
							} catch {
								_ranState = SMTaskRanState.Created;
								throw;
							}
							_ranState = SMTaskRanState.Loaded;
							return;
					}
					return;

				case SMTaskRanState.Initializing:
					if ( _owner._type == SMTaskType.DontWork )				{ return; }
					if ( _activeState != SMTaskActiveState.Disabled )		{ return; }
					if ( _nextActiveState != SMTaskActiveState.Enabling )	{ return; }
					switch ( _ranState ) {
						case SMTaskRanState.Loaded:
#if TestSMTask
							Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}\n{this}" );
#endif
							_ranState = SMTaskRanState.Initializing;
							try {
								await _initializeEvent.Run( _activeAsyncCanceler );
							} catch {
								_ranState = SMTaskRanState.Loaded;
								throw;
							}
							_ranState = SMTaskRanState.Initialized;
							return;
					}
					return;

				case SMTaskRanState.FixedUpdate:
					if ( _owner._type == SMTaskType.DontWork )			{ return; }
					if ( _activeState != SMTaskActiveState.Enabled )	{ return; }
					if ( _nextActiveState.HasValue )					{ return; }
					switch ( _ranState ) {
						case SMTaskRanState.Initialized:
							_ranState = SMTaskRanState.FixedUpdate;
							break;
					}
					switch ( _ranState ) {
						case SMTaskRanState.FixedUpdate:
						case SMTaskRanState.Update:
						case SMTaskRanState.LateUpdate:
#if TestSMTask
							Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}\n{this}" );
#endif
							_fixedUpdateEvent.Run();
							return;
					}
					return;

				case SMTaskRanState.Update:
					if ( _owner._type == SMTaskType.DontWork )			{ return; }
					if ( _activeState != SMTaskActiveState.Enabled )	{ return; }
					if ( _nextActiveState.HasValue )					{ return; }
					switch ( _ranState ) {
						case SMTaskRanState.FixedUpdate:
							_ranState = SMTaskRanState.Update;
							break;
					}
					switch ( _ranState ) {
						case SMTaskRanState.Update:
						case SMTaskRanState.LateUpdate:
#if TestSMTask
							Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}\n{this}" );
#endif
							_updateEvent.Run();
							return;
					}
					return;

				case SMTaskRanState.LateUpdate:
					if ( _owner._type == SMTaskType.DontWork )			{ return; }
					if ( _activeState != SMTaskActiveState.Enabled )	{ return; }
					if ( _nextActiveState.HasValue )					{ return; }
					switch ( _ranState ) {
						case SMTaskRanState.Update:
							_ranState = SMTaskRanState.LateUpdate;
							break;
					}
					switch ( _ranState ) {
						case SMTaskRanState.LateUpdate:
#if TestSMTask
							Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}\n{this}" );
#endif
							_lateUpdateEvent.Run();
							return;
					}
					return;

				case SMTaskRanState.Finalizing:
					if ( _owner._type == SMTaskType.DontWork )	{ return; }
					switch ( _ranState ) {
						case SMTaskRanState.Finalizing:
						case SMTaskRanState.Finalized:
							return;
					}
					var lastRanState = _ranState;
					_ranState = SMTaskRanState.Finalizing;
					switch ( lastRanState ) {
						case SMTaskRanState.Initialized:
						case SMTaskRanState.FixedUpdate:
						case SMTaskRanState.Update:
						case SMTaskRanState.LateUpdate:
#if TestSMTask
							Log.Debug( string.Join( "\n",
								$"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state} : check disable",
								$"{this}"
							) );
#endif
							await ChangeActive( false );
							break;
					}
					switch ( lastRanState ) {
						case SMTaskRanState.Finalizing:
						case SMTaskRanState.Finalized:
							return;
					}
					StopActiveAsync();
					if ( _ranState != SMTaskRanState.Finalizing )	{ lastRanState = _ranState; }
					// 非同期停止時に、catchで状態が変わる為、1フレーム待機
					await UTask.NextFrame( _inActiveAsyncCanceler );
					_ranState = SMTaskRanState.Finalizing;
#if TestSMTask
					Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}\n{this}" );
#endif
					switch ( lastRanState ) {
						case SMTaskRanState.Loading:
						case SMTaskRanState.Loaded:
						case SMTaskRanState.Initializing:
						case SMTaskRanState.Initialized:
						case SMTaskRanState.FixedUpdate:
						case SMTaskRanState.Update:
						case SMTaskRanState.LateUpdate:
							try {
								await _finalizeEvent.Run( _inActiveAsyncCanceler );
							} catch {
								_ranState = lastRanState;
								throw;
							}
							break;
					}
					_ranState = SMTaskRanState.Finalized;
					Dispose();
					return;

				case SMTaskRanState.None:
				case SMTaskRanState.Created:
				case SMTaskRanState.Loaded:
				case SMTaskRanState.Initialized:
				case SMTaskRanState.Finalized:
					throw new ArgumentOutOfRangeException(
						$"{state}", $"実行状態に、実行後の型を指定した為、実行不可" );
			}
		}


		public async UniTask ChangeActive( bool isActive ) {
			if ( _owner._type == SMTaskType.DontWork )	{ return; }
			_nextActiveState = isActive ? SMTaskActiveState.Enabling : SMTaskActiveState.Disabling;
			await RunActiveEvent();
		}

		public async UniTask RunActiveEvent() {
			if ( _owner._type == SMTaskType.DontWork )	{ return; }
			if ( !_nextActiveState.HasValue )			{ return; }

			switch ( _ranState ) {
				case SMTaskRanState.None:
				case SMTaskRanState.Creating:
				case SMTaskRanState.Loading:
				case SMTaskRanState.Initializing:
					var lastRanState = _ranState;
					await UTask.WaitWhile( _activeAsyncCanceler, () => _ranState == lastRanState );
					await RunActiveEvent();
					return;

				case SMTaskRanState.Created:
				case SMTaskRanState.Loaded:
				case SMTaskRanState.Initialized:
				case SMTaskRanState.FixedUpdate:
				case SMTaskRanState.Update:
				case SMTaskRanState.LateUpdate:
					break;

				case SMTaskRanState.Finalizing:
					if ( _nextActiveState == SMTaskActiveState.Disabling ) {
						break;
					} else {
						_nextActiveState = SMTaskActiveState.Disabling;
						return;
					}
				case SMTaskRanState.Finalized:
					return;
			}

			switch ( _nextActiveState ) {
				case SMTaskActiveState.Enabling:
#if TestSMTask
					Log.Debug( string.Join( "\n",
						$"{_owner.GetAboutName()}.{nameof(RunActiveEvent)} : {_nextActiveState} : call",
						$"{this}"
					) );
#endif
					switch ( _activeState ) {
						case SMTaskActiveState.Enabling:
							_nextActiveState = null;
							await UTask.WaitWhile(
								_activeAsyncCanceler, () => _activeState == SMTaskActiveState.Enabling );
							return;

						case SMTaskActiveState.Enabled:
							_nextActiveState = null;
							return;

						case SMTaskActiveState.Disabling:
							await UTask.WaitWhile(
								_activeAsyncCanceler, () => _activeState == SMTaskActiveState.Disabling );
							await RunActiveEvent();
							return;

						case SMTaskActiveState.Disabled:
							var lastNextActiveState = _nextActiveState;
							await RunStateEvent( SMTaskRanState.Loading );
							await RunStateEvent( SMTaskRanState.Initializing );
							if ( lastNextActiveState != _nextActiveState ) {
								await RunActiveEvent();
								return;
							}
							if ( !_isInitialized ) { return; }
#if TestSMTask
							Log.Debug(
								$"{_owner.GetAboutName()}.{nameof(RunActiveEvent)} : {_nextActiveState}\n{this}" );
#endif
							_nextActiveState = null;
							_activeState = SMTaskActiveState.Enabling;
							try {
								await _enableEvent.Run( _activeAsyncCanceler );
							} catch {
								_activeState = SMTaskActiveState.Disabled;
								throw;
							}
							_activeState = SMTaskActiveState.Enabled;
							return;
					}
					return;

				case SMTaskActiveState.Disabling:
#if TestSMTask
					Log.Debug( string.Join( "\n",
						$"{_owner.GetAboutName()}.{nameof(RunActiveEvent)} : {_nextActiveState} : call",
						$"{this}"
					) );
#endif
					switch ( _activeState ) {
						case SMTaskActiveState.Disabling:
							_nextActiveState = null;
							await UTask.WaitWhile(
								_inActiveAsyncCanceler, () => _activeState == SMTaskActiveState.Disabling );
							return;

						case SMTaskActiveState.Disabled:
							_nextActiveState = null;
							return;

						case SMTaskActiveState.Enabling:
							await UTask.WaitWhile(
								_inActiveAsyncCanceler, () => _activeState == SMTaskActiveState.Enabling );
							await RunActiveEvent();
							return;

						case SMTaskActiveState.Enabled:
							StopActiveAsync();
#if TestSMTask
							Log.Debug(
								$"{_owner.GetAboutName()}.{nameof(RunActiveEvent)} : {_nextActiveState}\n{this}" );
#endif
							_nextActiveState = null;
							_activeState = SMTaskActiveState.Disabling;
							try {
								await _disableEvent.Run( _inActiveAsyncCanceler );
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
					throw new ArgumentOutOfRangeException(
						$"{_nextActiveState}", $"活動状態に、実行後の型を指定した為、実行不可" );
			}
		}



		public override string ToString() {
			var result = string.Join( "\n",
				$"    {nameof( SMBehaviourBody )}(",
				$"        {nameof( _id )} : {_id} / {s_idCount}",
				$"        {nameof( _owner )} : {_owner.GetAboutName()}( {_owner._id} )",
				$"        {nameof( _ranState )} : {_ranState}",
				$"        {nameof( _activeState )} : {_activeState}",
				$"        {nameof( _nextActiveState )} : {_nextActiveState}",
				$"        {nameof( _isInitialized )} : {_isInitialized}",
				$"        {nameof( _isActive )} : {_isActive}",
				"\n"
			);

			var isCancel = _activeAsyncCanceler._disposables._isDispose
				? true
				: _activeAsyncCanceler.ToToken().IsCancellationRequested;
			result += $"        {nameof( _activeAsyncCanceler )}.Cancel : {isCancel}\n";

			isCancel = _inActiveAsyncCanceler._disposables._isDispose
				? true
				: _inActiveAsyncCanceler.ToToken().IsCancellationRequested;
			result += $"        {nameof( _inActiveAsyncCanceler )}.Cancel : {isCancel}\n";

			result += string.Join( "\n",
				$"        {nameof( _disposables._isDispose )} : {_disposables._isDispose}",
				"",
				$"        {nameof( _loadEvent )}._isRunning : {_loadEvent._isRunning}",
				$"        {nameof( _initializeEvent )}._isRunning : {_initializeEvent._isRunning}",
				$"        {nameof( _enableEvent )}._isRunning : {_enableEvent._isRunning}",
				$"        {nameof( _fixedUpdateEvent )}.Count : {_fixedUpdateEvent._events.Count}",
				$"        {nameof( _updateEvent )}.Count : {_updateEvent._events.Count}",
				$"        {nameof( _lateUpdateEvent )}.Count : {_lateUpdateEvent._events.Count}",
				$"        {nameof( _disableEvent )}._isRunning : {_disableEvent._isRunning}",
				$"        {nameof( _finalizeEvent )}._isRunning : {_finalizeEvent._isRunning}",
				"    )"
			);

			return result;
		}
	}
}