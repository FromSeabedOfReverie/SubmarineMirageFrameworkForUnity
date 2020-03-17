//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process.New {
	using System.Threading;
	using UniRx.Async;
	using MultiEvent;
	using Scene;
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
		public static readonly string FOREVER_SCENE_NAME = LifeSpan.Forever.ToString();

		public RanState _ranState	{ get; private set; }
		public ActiveState _activeState	{ get; private set; }
		public bool _isInitialized => _ranState >= RanState.Initialized;
		public bool _isActive => _activeState == ActiveState.Enabled;

		IProcess _owner;

		public string _belongSceneName	{ get; private set; }

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
		CancellationTokenSource _inActiveAsyncCanceler = new CancellationTokenSource();
		public CancellationToken _inActiveAsyncCancel => _inActiveAsyncCanceler.Token;

		public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();


		public ProcessBody( IProcess owner ) {
			_owner = owner;

			if ( owner._lifeSpan == LifeSpan.Forever ) {
				_belongSceneName = FOREVER_SCENE_NAME;
			} else if ( _owner is MonoBehaviourProcess ) {
				var mono = (MonoBehaviourProcess)_owner;
				_belongSceneName = mono.gameObject.scene.name;
			} else {
				_belongSceneName = SceneManager.s_instance._currentSceneName;
			}

			if ( _owner._type == Type.DontWork ) {
				RunStateEvent( RanState.Creating ).Forget();
			} else {
//				CoreProcessManager.s_instance.Register( owner ).Forget();
			}

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
				_finalizeEvent
			);
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
		}


		public async UniTask RunStateEvent( RanState state ) {
//			Log.Debug( $"{_owner.GetAboutName()} {state} want" );

			switch ( state ) {
				case RanState.Creating:
					if ( _isActive )	{ break; }
					switch ( _ranState ) {
						case RanState.None:
							_ranState = RanState.Creating;
							try {
								await UniTaskUtility.Delay( _activeAsyncCancel, 1 );
							} catch {
								_ranState = RanState.None;
							}
							_owner.Create();
							_ranState = RanState.Created;
							break;
					}
					break;

				case RanState.Loading:
					if ( _isActive )	{ break; }
					switch ( _ranState ) {
						case RanState.Created:
							_ranState = RanState.Loading;
							try {
								await _loadEvent.Run( _activeAsyncCancel );
							} catch {
								_ranState = RanState.Created;
							}
							_ranState = RanState.Loaded;
							break;
					}
					break;

				case RanState.Initializing:
					if ( _isActive )	{ break; }
					switch ( _ranState ) {
						case RanState.Loaded:
							_ranState = RanState.Initializing;
							try {
								await _initializeEvent.Run( _activeAsyncCancel );
							} catch {
								_ranState = RanState.Loaded;
							}
							_ranState = RanState.Initialized;
							break;
					}
					break;

				case RanState.FixedUpdate:
					if ( !_isActive )	{ break; }
					switch ( _ranState ) {
						case RanState.Initialized:
							_ranState = RanState.FixedUpdate;
							break;
					}
					switch ( _ranState ) {
						case RanState.FixedUpdate:
						case RanState.Update:
						case RanState.LateUpdate:
							_fixedUpdateEvent.Run();
							break;
					}
					break;

				case RanState.Update:
					if ( !_isActive )	{ break; }
					switch ( _ranState ) {
						case RanState.FixedUpdate:
							_ranState = RanState.Update;
							break;
					}
					switch ( _ranState ) {
						case RanState.Update:
						case RanState.LateUpdate:
							_updateEvent.Run();
							break;
					}
					break;

				case RanState.LateUpdate:
					if ( !_isActive )	{ break; }
					switch ( _ranState ) {
						case RanState.Update:
							_ranState = RanState.LateUpdate;
							break;
					}
					switch ( _ranState ) {
						case RanState.LateUpdate:
							_lateUpdateEvent.Run();
							break;
					}
					break;

				case RanState.Finalizing:
					if ( _isActive )	{ break; }
					switch ( _ranState ) {
						case RanState.Loading:
						case RanState.Loaded:
						case RanState.Initializing:
						case RanState.Initialized:
						case RanState.FixedUpdate:
						case RanState.Update:
						case RanState.LateUpdate:
							var lastRanState = _ranState;
							_ranState = RanState.Finalizing;
							try {
								await _finalizeEvent.Run( _inActiveAsyncCancel );
							} catch {
								_ranState = lastRanState;
							}
							break;
					}
					_ranState = RanState.Finalized;
					Dispose();
					if ( _owner._type != Type.DontWork ) {
//						CoreProcessManager.s_instance.Unregister( _owner );
					}
					break;
			}
		}


		public async UniTask RunActiveEvent( ActiveState newState ) {
			switch ( newState ) {
				case ActiveState.Enabling:
					switch ( _activeState ) {
						case ActiveState.Disabled:
							_activeState = ActiveState.Enabling;
							try {
								await RunStateEvent( RanState.Loading );
								await RunStateEvent( RanState.Initializing );
								await _enableEvent.Run( _activeAsyncCancel );
							} catch {
								_activeState = ActiveState.Disabled;
							}
							_activeState = ActiveState.Enabled;
							break;
					}
					break;

				case ActiveState.Disabling:
					switch ( _activeState ) {
						case ActiveState.Enabled:
							_activeState = ActiveState.Disabling;
							StopActiveAsync();
							try {
								await _disableEvent.Run( _inActiveAsyncCancel );
							} catch {
								_activeState = ActiveState.Enabled;
							}
							_activeState = ActiveState.Disabled;
							break;
					}
					break;
			}
		}


		public override string ToString() => this.ToDeepString();
	}
}