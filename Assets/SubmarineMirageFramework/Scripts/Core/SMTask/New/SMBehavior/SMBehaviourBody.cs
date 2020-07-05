//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask {
	using System;
	using System.Threading;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using MultiEvent;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMBehaviourBody : IDisposableExtension {
		public SMTaskRanState _ranState	{ get; private set; }
		public SMTaskActiveState _activeState	{ get; private set; }
		public SMTaskActiveState? _nextActiveState	{ get; private set; }
		public bool _isInitialized => _ranState >= SMTaskRanState.Initialized;
		public bool _isActive => _activeState == SMTaskActiveState.Enabled;

		ISMBehaviour _owner;

		public readonly MultiAsyncEvent _loadEvent = new MultiAsyncEvent();
		public readonly MultiAsyncEvent _initializeEvent = new MultiAsyncEvent();
		public readonly MultiAsyncEvent _enableEvent = new MultiAsyncEvent();
		public readonly MultiSubject _fixedUpdateEvent = new MultiSubject();
		public readonly MultiSubject _updateEvent = new MultiSubject();
		public readonly MultiSubject _lateUpdateEvent = new MultiSubject();
		public readonly MultiAsyncEvent _disableEvent = new MultiAsyncEvent();
		public readonly MultiAsyncEvent _finalizeEvent = new MultiAsyncEvent();

		public readonly MultiSubject _activeAsyncCancelEvent = new MultiSubject();
		CancellationTokenSource _activeAsyncCanceler = new CancellationTokenSource();
		public CancellationToken _activeAsyncCancel => _activeAsyncCanceler.Token;
		CancellationTokenSource _inActiveAsyncCanceler = new CancellationTokenSource();
		public CancellationToken _inActiveAsyncCancel => _inActiveAsyncCanceler.Token;

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public SMBehaviourBody( ISMBehaviour owner, SMTaskActiveState nextActiveState ) {
			_owner = owner;
			_nextActiveState = nextActiveState;

			SetActiveAsyncCancelerDisposable();
			_disposables.AddLast( () => {
				_inActiveAsyncCanceler.Cancel();
				_inActiveAsyncCanceler.Dispose();
			} );
			_disposables.AddLast(
				_loadEvent,
				_initializeEvent,
				_enableEvent,
				_fixedUpdateEvent,
				_updateEvent,
				_lateUpdateEvent,
				_disableEvent,
				_finalizeEvent,
				_activeAsyncCancelEvent
			);
			_disposables.AddLast( () => UnLink() );
			_disposables.AddLast( () => Log.Debug( $"Dispose Body : {_owner.GetAboutName()}" ) );
		}

		void SetActiveAsyncCancelerDisposable() {
			_disposables.AddFirst( "_activeAsyncCanceler", () => {
				_activeAsyncCanceler.Cancel();
				_activeAsyncCanceler.Dispose();
			} );
		}

		~SMBehaviourBody() => Dispose();

		public void Dispose() => _disposables.Dispose();

		void UnLink() {
			if ( _owner._previous != null )	{ _owner._previous._next = _owner._next; }
			if ( _owner._next != null )		{ _owner._next._previous = _owner._previous; }
			_owner._previous = null;
			_owner._next = null;
		}



		public void StopActiveAsync() {
			_disposables.Remove( "_activeAsyncCanceler" );
			_activeAsyncCanceler = new CancellationTokenSource();
			SetActiveAsyncCancelerDisposable();
			_activeAsyncCancelEvent.Run();
		}


		public async UniTask RunStateEvent( SMTaskRanState state ) {
			switch ( state ) {
				case SMTaskRanState.Creating:
					if ( _activeState != SMTaskActiveState.Disabled )	{ return; }
					switch ( _ranState ) {
						case SMTaskRanState.None:
							Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}" );
							_ranState = SMTaskRanState.Creating;
							try {
// TODO : awaitが不要な事を、FSM等実装後に確認後、状態をCreateのみに、修正
//								await UniTaskUtility.Yield( _activeAsyncCancel );
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
							Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}" );
							_ranState = SMTaskRanState.Loading;
							try {
								await _loadEvent.Run( _activeAsyncCancel );
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
							Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}" );
							_ranState = SMTaskRanState.Initializing;
							try {
								await _initializeEvent.Run( _activeAsyncCancel );
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
							Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}" );
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
							Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}" );
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
							Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}" );
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
							Log.Debug(
								$"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state} : check disable" );
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
					await UniTaskUtility.Yield( _inActiveAsyncCancel );
					_ranState = SMTaskRanState.Finalizing;
					Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}" );
					switch ( lastRanState ) {
						case SMTaskRanState.Loading:
						case SMTaskRanState.Loaded:
						case SMTaskRanState.Initializing:
						case SMTaskRanState.Initialized:
						case SMTaskRanState.FixedUpdate:
						case SMTaskRanState.Update:
						case SMTaskRanState.LateUpdate:
							try {
								await _finalizeEvent.Run( _inActiveAsyncCancel );
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
					await UniTaskUtility.WaitWhile( _activeAsyncCancel, () => _ranState == lastRanState );
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
					Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunActiveEvent)} : {_nextActiveState} : call" );
					switch ( _activeState ) {
						case SMTaskActiveState.Enabling:
							_nextActiveState = null;
							await UniTaskUtility.WaitWhile(
								_activeAsyncCancel, () => _activeState == SMTaskActiveState.Enabling );
							return;

						case SMTaskActiveState.Enabled:
							_nextActiveState = null;
							return;

						case SMTaskActiveState.Disabling:
							await UniTaskUtility.WaitWhile(
								_activeAsyncCancel, () => _activeState == SMTaskActiveState.Disabling );
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
							Log.Debug(
								$"{_owner.GetAboutName()}.{nameof(RunActiveEvent)} : {_nextActiveState}" );
							_nextActiveState = null;
							_activeState = SMTaskActiveState.Enabling;
							try {
								await _enableEvent.Run( _activeAsyncCancel );
							} catch {
								_activeState = SMTaskActiveState.Disabled;
								throw;
							}
							_activeState = SMTaskActiveState.Enabled;
							return;
					}
					return;

				case SMTaskActiveState.Disabling:
					Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunActiveEvent)} : {_nextActiveState} : call" );
					switch ( _activeState ) {
						case SMTaskActiveState.Disabling:
							_nextActiveState = null;
							await UniTaskUtility.WaitWhile(
								_inActiveAsyncCancel, () => _activeState == SMTaskActiveState.Disabling );
							return;

						case SMTaskActiveState.Disabled:
							_nextActiveState = null;
							return;

						case SMTaskActiveState.Enabling:
							await UniTaskUtility.WaitWhile(
								_inActiveAsyncCancel, () => _activeState == SMTaskActiveState.Enabling );
							await RunActiveEvent();
							return;

						case SMTaskActiveState.Enabled:
							StopActiveAsync();
							Log.Debug(
								$"{_owner.GetAboutName()}.{nameof(RunActiveEvent)} : {_nextActiveState}" );
							_nextActiveState = null;
							_activeState = SMTaskActiveState.Disabling;
							try {
								await _disableEvent.Run( _inActiveAsyncCancel );
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


		public override string ToString() => this.ToDeepString();
	}
}