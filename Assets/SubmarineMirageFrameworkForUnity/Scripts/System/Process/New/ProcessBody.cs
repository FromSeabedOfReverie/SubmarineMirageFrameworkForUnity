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
	using KoganeUnityLib;
	using MultiEvent;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using UnityObject = UnityEngine.Object;


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
		public ActiveState? _nextActiveState	{ get; private set; } = ActiveState.Enabling;
		public bool _isInitialized => _ranState >= RanState.Initialized;
		public bool _isActive => _activeState == ActiveState.Enabled;

		IProcess _owner;
		MonoBehaviourProcess _monoOwner;

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
			if ( _owner is MonoBehaviourProcess )	{ _monoOwner = (MonoBehaviourProcess)_owner; }

			_belongSceneName = (
				owner._lifeSpan == LifeSpan.Forever ?	FOREVER_SCENE_NAME :
				_monoOwner != null ?					_monoOwner.gameObject.scene.name :
														SceneManager.s_instance._currentSceneName
			);

			if ( _monoOwner != null ) {
				if ( owner._lifeSpan == LifeSpan.Forever ) {
					UnityObject.DontDestroyOnLoad( _monoOwner.gameObject );
				}
				if ( !_monoOwner.isActiveAndEnabled )	{ _nextActiveState = ActiveState.Disabling; }
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
			_disposables.AddLast( () => Log.Debug( $"ProcessBody.Dispose()" ) );
		}

		void SetActiveAsyncCancelerDisposable() {
			_disposables.AddFirst( "_activeAsyncCanceler", () => {
				Log.Debug( $"ProcessBody._activeAsyncCanceler dispose" );
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
			switch ( state ) {
				case RanState.Creating:
					if ( _activeState != ActiveState.Disabled )	{ return; }
					switch ( _ranState ) {
						case RanState.None:
							Log.Debug( $"Run {state}" );
							_ranState = RanState.Creating;
							try {
								await UniTaskUtility.Delay( _activeAsyncCancel, 1 );
							} catch {
								_ranState = RanState.None;
								throw;
							}
							_owner.Create();
							_ranState = RanState.Created;
							return;
					}
					return;

				case RanState.Loading:
					if ( _activeState != ActiveState.Disabled )		{ return; }
					if ( _nextActiveState != ActiveState.Enabling )	{ return; }
					switch ( _ranState ) {
						case RanState.Created:
							Log.Debug( $"Run {state}" );
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
					if ( _activeState != ActiveState.Disabled )		{ return; }
					if ( _nextActiveState != ActiveState.Enabling )	{ return; }
					switch ( _ranState ) {
						case RanState.Loaded:
							Log.Debug( $"Run {state}" );
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
							Log.Debug( $"Run {state}" );
							_fixedUpdateEvent.Run();
							return;
					}
					return;

				case RanState.Update:
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
							Log.Debug( $"Run {state}" );
							_updateEvent.Run();
							return;
					}
					return;

				case RanState.LateUpdate:
					if ( _activeState != ActiveState.Enabled )	{ return; }
					if ( _nextActiveState.HasValue )			{ return; }
					switch ( _ranState ) {
						case RanState.Update:
							_ranState = RanState.LateUpdate;
							break;
					}
					switch ( _ranState ) {
						case RanState.LateUpdate:
							Log.Debug( $"Run {state}" );
							_lateUpdateEvent.Run();
							return;
					}
					return;

				case RanState.Finalizing:
					switch ( _ranState ) {
						case RanState.Initialized:
						case RanState.FixedUpdate:
						case RanState.Update:
						case RanState.LateUpdate:
							Log.Debug( $"check disable Want {state}" );
							await ChangeActive( false );
							break;
					}
					switch ( _ranState ) {
						case RanState.Finalizing:
						case RanState.Finalized:
							return;
					}
					StopActiveAsync();
					Log.Debug( $"Run {state}" );
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
								throw;
							}
							break;
					}
					_ranState = RanState.Finalized;
					Dispose();
					if ( _owner._type != Type.DontWork ) {
//						CoreProcessManager.s_instance.Unregister( _owner );
					}
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
			var state = isActive ? ActiveState.Enabling : ActiveState.Disabling;
			_nextActiveState = state;
			await RunActiveEvent();
		}


		async UniTask RunActiveEvent() {
			if ( !_nextActiveState.HasValue )	{ return; }

			var lastRanState = _ranState;
			switch ( _ranState ) {
				case RanState.None:
				case RanState.Creating:
				case RanState.Loading:
				case RanState.Initializing:
					await UniTaskUtility.WaitWhile( _activeAsyncCancel, () => _ranState == lastRanState );
					await RunActiveEvent();
					return;

				case RanState.Created:
				case RanState.Loaded:
				case RanState.Initialized:
					break;
				case RanState.FixedUpdate:
				case RanState.Update:
				case RanState.LateUpdate:
					break;

				case RanState.Finalizing:
				case RanState.Finalized:
					return;
			}

			switch ( _nextActiveState ) {
				case ActiveState.Enabling:
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
							ChangeActiveOfGameObject( true );
							Log.Debug( $"check initialize Want {_nextActiveState}" );
							var lastNextActiveState = _nextActiveState;
							await RunStateEvent( RanState.Loading );
							await RunStateEvent( RanState.Initializing );
							await UniTaskUtility.WaitWhile( _activeAsyncCancel, () => !_isInitialized );
							if ( lastNextActiveState != _nextActiveState ) {
								await RunActiveEvent();
								return;
							}
							Log.Debug( $"Run {_nextActiveState}" );
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
					switch ( _activeState ) {
						case ActiveState.Disabling:
							_nextActiveState = null;
							await UniTaskUtility.WaitWhile(
								_activeAsyncCancel, () => _activeState == ActiveState.Disabling );
							return;

						case ActiveState.Disabled:
							_nextActiveState = null;
							return;

						case ActiveState.Enabling:
							await UniTaskUtility.WaitWhile(
								_activeAsyncCancel, () => _activeState == ActiveState.Enabling );
							await RunActiveEvent();
							return;

						case ActiveState.Enabled:
							Log.Debug( $"Run {_nextActiveState}" );
							_nextActiveState = null;
							_activeState = ActiveState.Disabling;
							StopActiveAsync();
							try {
								await _disableEvent.Run( _inActiveAsyncCancel );
							} catch {
								_activeState = ActiveState.Enabled;
								throw;
							}
							ChangeActiveOfGameObject( false );
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


		void ChangeActiveOfGameObject( bool isActive, bool isChange = true ) {
// TODO : 呼び出し元で、isChangeを指定する
			if ( _monoOwner == null || !isChange )	{ return; }
			_monoOwner.gameObject.SetActive( isActive );
		}


		public override string ToString() => this.ToDeepString();
	}
}