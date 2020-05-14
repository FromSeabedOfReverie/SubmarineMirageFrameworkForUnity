//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process.New {
	using System;
	using System.Threading;
	using UniRx.Async;
	using KoganeUnityLib;
	using MultiEvent;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class ProcessBody : IDisposableExtension {
		public enum RanState {
			None,
			Creating,
			Created,
			Loading,
			Loaded,
			Initializing,
			Initialized,
			FixedUpdate,
			Update,
			LateUpdate,
			Finalizing,
			Finalized,
		}
		public enum ActiveState {
			Disabled,
			Disabling,
			Enabled,
			Enabling,
		}
		public enum Type {
			DontWork,
			Work,
			FirstWork,
		}
		public enum LifeSpan {
			InScene,
			Forever,
		}

		public RanState _ranState	{ get; private set; }
		public ActiveState _activeState	{ get; private set; }
		public ActiveState? _nextActiveState	{ get; private set; }
		public bool _isInitialized => _ranState >= RanState.Initialized;
		public bool _isActive => _activeState == ActiveState.Enabled;

		IProcess _owner;

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


		public ProcessBody( IProcess owner, ActiveState nextActiveState ) {
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
			_disposables.AddLast( () => Log.Debug( $"Dispose Body : {_owner.GetAboutName()}" ) );
		}

		void SetActiveAsyncCancelerDisposable() {
			_disposables.AddFirst( "_activeAsyncCanceler", () => {
				_activeAsyncCanceler.Cancel();
				_activeAsyncCanceler.Dispose();
			} );
		}

		public void Dispose() => _disposables.Dispose();

		~ProcessBody() => Dispose();


		public void StopActiveAsync() {
			_disposables.Remove( "_activeAsyncCanceler" );
			_activeAsyncCanceler = new CancellationTokenSource();
			SetActiveAsyncCancelerDisposable();
			_activeAsyncCancelEvent.Run();
		}


		public async UniTask RunStateEvent( RanState state ) {
			switch ( state ) {
				case RanState.Creating:
					if ( _activeState != ActiveState.Disabled )	{ return; }
					switch ( _ranState ) {
						case RanState.None:
							Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}" );
							_ranState = RanState.Creating;
							try {
// TODO : awaitが不要な事を、FSM等実装後に確認後、状態をCreateのみに、修正
//								await UniTaskUtility.Yield( _activeAsyncCancel );
								_owner.Create();
							} catch {
								_ranState = RanState.None;
								throw;
							}
							_ranState = RanState.Created;
							return;
					}
					return;

				case RanState.Loading:
					if ( _owner._type == Type.DontWork )			{ return; }
					if ( _activeState != ActiveState.Disabled )		{ return; }
					if ( _nextActiveState != ActiveState.Enabling )	{ return; }
					switch ( _ranState ) {
						case RanState.Created:
							Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}" );
							_ranState = RanState.Loading;
							try {
								await _loadEvent.Run( _activeAsyncCancel );
							} catch {
								_ranState = RanState.Created;
								throw;
							}
							_ranState = RanState.Loaded;
							return;
					}
					return;

				case RanState.Initializing:
					if ( _owner._type == Type.DontWork )			{ return; }
					if ( _activeState != ActiveState.Disabled )		{ return; }
					if ( _nextActiveState != ActiveState.Enabling )	{ return; }
					switch ( _ranState ) {
						case RanState.Loaded:
							Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}" );
							_ranState = RanState.Initializing;
							try {
								await _initializeEvent.Run( _activeAsyncCancel );
							} catch {
								_ranState = RanState.Loaded;
								throw;
							}
							_ranState = RanState.Initialized;
							return;
					}
					return;

				case RanState.FixedUpdate:
					if ( _owner._type == Type.DontWork )		{ return; }
					if ( _activeState != ActiveState.Enabled )	{ return; }
					if ( _nextActiveState.HasValue )			{ return; }
					switch ( _ranState ) {
						case RanState.Initialized:
							_ranState = RanState.FixedUpdate;
							break;
					}
					switch ( _ranState ) {
						case RanState.FixedUpdate:
						case RanState.Update:
						case RanState.LateUpdate:
							Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}" );
							_fixedUpdateEvent.Run();
							return;
					}
					return;

				case RanState.Update:
					if ( _owner._type == Type.DontWork )		{ return; }
					if ( _activeState != ActiveState.Enabled )	{ return; }
					if ( _nextActiveState.HasValue )			{ return; }
					switch ( _ranState ) {
						case RanState.FixedUpdate:
							_ranState = RanState.Update;
							break;
					}
					switch ( _ranState ) {
						case RanState.Update:
						case RanState.LateUpdate:
							Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}" );
							_updateEvent.Run();
							return;
					}
					return;

				case RanState.LateUpdate:
					if ( _owner._type == Type.DontWork )		{ return; }
					if ( _activeState != ActiveState.Enabled )	{ return; }
					if ( _nextActiveState.HasValue )			{ return; }
					switch ( _ranState ) {
						case RanState.Update:
							_ranState = RanState.LateUpdate;
							break;
					}
					switch ( _ranState ) {
						case RanState.LateUpdate:
							Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}" );
							_lateUpdateEvent.Run();
							return;
					}
					return;

				case RanState.Finalizing:
					if ( _owner._type == Type.DontWork )	{ return; }
					switch ( _ranState ) {
						case RanState.Finalizing:
						case RanState.Finalized:
							return;
					}
					var lastRanState = _ranState;
					_ranState = RanState.Finalizing;
					switch ( lastRanState ) {
						case RanState.Initialized:
						case RanState.FixedUpdate:
						case RanState.Update:
						case RanState.LateUpdate:
							Log.Debug(
								$"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state} : check disable" );
							await ChangeActive( false );
							break;
					}
					switch ( lastRanState ) {
						case RanState.Finalizing:
						case RanState.Finalized:
							return;
					}
					StopActiveAsync();
					if ( _ranState != RanState.Finalizing )	{ lastRanState = _ranState; }
					// 非同期停止時に、catchで状態が変わる為、1フレーム待機
					await UniTaskUtility.Yield( _inActiveAsyncCancel );
					_ranState = RanState.Finalizing;
					Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunStateEvent)} : {state}" );
					switch ( lastRanState ) {
						case RanState.Loading:
						case RanState.Loaded:
						case RanState.Initializing:
						case RanState.Initialized:
						case RanState.FixedUpdate:
						case RanState.Update:
						case RanState.LateUpdate:
							try {
								await _finalizeEvent.Run( _inActiveAsyncCancel );
							} catch {
								_ranState = lastRanState;
								throw;
							}
							break;
					}
					_ranState = RanState.Finalized;
					Dispose();
					return;

				case RanState.None:
				case RanState.Created:
				case RanState.Loaded:
				case RanState.Initialized:
				case RanState.Finalized:
					throw new ArgumentOutOfRangeException(
						$"{state}", $"実行状態に、実行後の型を指定した為、実行不可" );
			}
		}


		public async UniTask ChangeActive( bool isActive ) {
			if ( _owner._type == Type.DontWork )	{ return; }
			_nextActiveState = isActive ? ActiveState.Enabling : ActiveState.Disabling;
			await RunActiveEvent();
		}

		public async UniTask RunActiveEvent() {
			if ( _owner._type == Type.DontWork )	{ return; }
			if ( !_nextActiveState.HasValue )		{ return; }

			switch ( _ranState ) {
				case RanState.None:
				case RanState.Creating:
				case RanState.Loading:
				case RanState.Initializing:
					var lastRanState = _ranState;
					await UniTaskUtility.WaitWhile( _activeAsyncCancel, () => _ranState == lastRanState );
					await RunActiveEvent();
					return;

				case RanState.Created:
				case RanState.Loaded:
				case RanState.Initialized:
				case RanState.FixedUpdate:
				case RanState.Update:
				case RanState.LateUpdate:
					break;

				case RanState.Finalizing:
					if ( _nextActiveState == ActiveState.Disabling ) {
						break;
					} else {
						_nextActiveState = ActiveState.Disabling;
						return;
					}
				case RanState.Finalized:
					return;
			}

			switch ( _nextActiveState ) {
				case ActiveState.Enabling:
					Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunActiveEvent)} : {_nextActiveState} : call" );
					switch ( _activeState ) {
						case ActiveState.Enabling:
							_nextActiveState = null;
							await UniTaskUtility.WaitWhile(
								_activeAsyncCancel, () => _activeState == ActiveState.Enabling );
							return;

						case ActiveState.Enabled:
							_nextActiveState = null;
							return;

						case ActiveState.Disabling:
							await UniTaskUtility.WaitWhile(
								_activeAsyncCancel, () => _activeState == ActiveState.Disabling );
							await RunActiveEvent();
							return;

						case ActiveState.Disabled:
							var lastNextActiveState = _nextActiveState;
							await RunStateEvent( RanState.Loading );
							await RunStateEvent( RanState.Initializing );
							if ( lastNextActiveState != _nextActiveState ) {
								await RunActiveEvent();
								return;
							}
							if ( !_isInitialized ) { return; }
							Log.Debug(
								$"{_owner.GetAboutName()}.{nameof(RunActiveEvent)} : {_nextActiveState}" );
							_nextActiveState = null;
							_activeState = ActiveState.Enabling;
							try {
								await _enableEvent.Run( _activeAsyncCancel );
							} catch {
								_activeState = ActiveState.Disabled;
								throw;
							}
							_activeState = ActiveState.Enabled;
							return;
					}
					return;

				case ActiveState.Disabling:
					Log.Debug( $"{_owner.GetAboutName()}.{nameof(RunActiveEvent)} : {_nextActiveState} : call" );
					switch ( _activeState ) {
						case ActiveState.Disabling:
							_nextActiveState = null;
							await UniTaskUtility.WaitWhile(
								_inActiveAsyncCancel, () => _activeState == ActiveState.Disabling );
							return;

						case ActiveState.Disabled:
							_nextActiveState = null;
							return;

						case ActiveState.Enabling:
							await UniTaskUtility.WaitWhile(
								_inActiveAsyncCancel, () => _activeState == ActiveState.Enabling );
							await RunActiveEvent();
							return;

						case ActiveState.Enabled:
							StopActiveAsync();
							Log.Debug(
								$"{_owner.GetAboutName()}.{nameof(RunActiveEvent)} : {_nextActiveState}" );
							_nextActiveState = null;
							_activeState = ActiveState.Disabling;
							try {
								await _disableEvent.Run( _inActiveAsyncCancel );
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
					throw new ArgumentOutOfRangeException(
						$"{_nextActiveState}", $"活動状態に、実行後の型を指定した為、実行不可" );
			}
		}


		public override string ToString() => this.ToDeepString();
	}
}