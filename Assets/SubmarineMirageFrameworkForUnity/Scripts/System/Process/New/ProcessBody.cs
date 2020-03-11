//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process.New {
	using System;
	using System.Threading;
	using UniRx.Async;
	using MultiEvent;
	using Extension;


	// TODO : コメント追加、整頓


	public class ProcessBody : IDisposable {
		public enum RanState {
			None,
			Create,
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
		ActiveState _activeState;
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

		CancellationTokenSource _activeAsyncCanceler = new CancellationTokenSource();
		public CancellationToken _activeAsyncCancel => _activeAsyncCanceler.Token;
		CancellationTokenSource _finalizeAsyncCanceler = new CancellationTokenSource();
		public CancellationToken _finalizeAsyncCancel => _finalizeAsyncCanceler.Token;


		public ProcessBody( IProcess owner ) {
			_owner = owner;
			CoreProcessManager.s_instance.Register( owner ).Forget();
		}


		public async UniTask RunStateEvent( RanState state ) {
			if ( !_isActive ) {
// TODO : 非活動中の状態遷移どうする？
			}

			switch ( state ) {
				case RanState.Create:
					switch ( _ranState ) {
						case RanState.None:
							_ranState = RanState.Create;
							_owner.Create();
							break;
					}
					break;

				case RanState.Loading:
					switch ( _ranState ) {
						case RanState.Create:
						case RanState.Loading:
							_ranState = RanState.Loading;
							await _loadEvent.Invoke( _activeAsyncCancel );
							_ranState = RanState.Loaded;
							break;
					}
					break;

				case RanState.Initializing:
					switch ( _ranState ) {
						case RanState.Loaded:
						case RanState.Initializing:
							_ranState = RanState.Initializing;
							await _initializeEvent.Invoke( _activeAsyncCancel );
							_ranState = RanState.Initialized;
							break;
					}
					break;

				case RanState.FixedUpdate:
					switch ( _ranState ) {
						case RanState.Initialized:
							_ranState = RanState.FixedUpdate;
							break;
					}
					switch ( _ranState ) {
						case RanState.FixedUpdate:
						case RanState.Update:
						case RanState.LateUpdate:
							_fixedUpdateEvent.Invoke();
							break;
					}
					break;

				case RanState.Update:
					switch ( _ranState ) {
						case RanState.FixedUpdate:
							_ranState = RanState.Update;
							break;
					}
					switch ( _ranState ) {
						case RanState.Update:
						case RanState.LateUpdate:
							_updateEvent.Invoke();
							break;
					}
					break;

				case RanState.LateUpdate:
					switch ( _ranState ) {
						case RanState.Update:
							_ranState = RanState.LateUpdate;
							break;
					}
					switch ( _ranState ) {
						case RanState.LateUpdate:
							_lateUpdateEvent.Invoke();
							break;
					}
					break;

				case RanState.Finalizing:
					switch ( _ranState ) {
						case RanState.Loading:
						case RanState.Loaded:
						case RanState.Initializing:
						case RanState.Initialized:
						case RanState.FixedUpdate:
						case RanState.Update:
						case RanState.LateUpdate:
						case RanState.Finalizing:
							_ranState = RanState.Finalizing;
							await _finalizeEvent.Invoke( _finalizeAsyncCancel );
							break;
					}
					_ranState = RanState.Finalized;
					Dispose();
					break;
			}
		}


		public async UniTask RunActiveEvent( ActiveState newState ) {
			switch ( newState ) {
				case ActiveState.Enabling:
					switch ( _activeState ) {
						case ActiveState.Disabled:
							_activeState = ActiveState.Enabling;
							await RunStateEvent( RanState.Loading );
							await RunStateEvent( RanState.Initializing );
							await _enableEvent.Invoke( _activeAsyncCancel );
							_activeState = ActiveState.Enabled;
							break;
					}
					break;

				case ActiveState.Disabling:
					switch ( _activeState ) {
						case ActiveState.Enabled:
							_activeState = ActiveState.Disabling;
							StopActiveAsync();
							await _disableEvent.Invoke( _finalizeAsyncCancel );
							_activeState = ActiveState.Disabled;
							break;
					}
					break;
			}
		}


		public void StopActiveAsync() {
			_activeAsyncCanceler.Cancel();
			_activeAsyncCanceler.Dispose();
			_activeAsyncCanceler = new CancellationTokenSource();
		}


		public void Dispose() {
			_activeAsyncCanceler.Cancel();
			_finalizeAsyncCanceler.Cancel();
			_activeAsyncCanceler.Dispose();
			_finalizeAsyncCanceler.Dispose();

			_loadEvent.Dispose();
			_initializeEvent.Dispose();
			_enableEvent.Dispose();
			_fixedUpdateEvent.Dispose();
			_updateEvent.Dispose();
			_lateUpdateEvent.Dispose();
			_disableEvent.Dispose();
			_finalizeEvent.Dispose();
		}


		public override string ToString() => this.ToDeepString();


		~ProcessBody() => Dispose();
	}
}