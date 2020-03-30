//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process.New {
	using System.Linq;
	using System.Threading;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx.Async;
	using KoganeUnityLib;
	using MultiEvent;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class MonoBehaviourProcess : MonoBehaviourExtension, IProcess {
		public virtual ProcessBody.Type _type => ProcessBody.Type.Work;
		public virtual ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.InScene;

		readonly List<MonoBehaviourProcess> _parents = new List<MonoBehaviourProcess>();
		readonly List<MonoBehaviourProcess> _children = new List<MonoBehaviourProcess>();
		readonly List<MonoBehaviourProcess> _brothers = new List<MonoBehaviourProcess>();

		public string _belongSceneName	{ get; private set; }

		public ProcessBody _process	{ get; private set; }

		public bool _isInitialized => _process._isInitialized;
		public bool _isActive => _process._isActive;

		public MultiAsyncEvent _loadEvent => _process._loadEvent;
		public MultiAsyncEvent _initializeEvent => _process._initializeEvent;
		public MultiAsyncEvent _enableEvent => _process._enableEvent;
		public MultiSubject _fixedUpdateEvent => _process._fixedUpdateEvent;
		public MultiSubject _updateEvent => _process._updateEvent;
		public MultiSubject _lateUpdateEvent => _process._lateUpdateEvent;
		public MultiAsyncEvent _disableEvent => _process._disableEvent;
		public MultiAsyncEvent _finalizeEvent => _process._finalizeEvent;

		public CancellationToken _activeAsyncCancel => _process._activeAsyncCancel;
		public CancellationToken _inActiveAsyncCancel => _process._inActiveAsyncCancel;

		public MultiDisposable _disposables => _process._disposables;


#if DEVELOP
		protected
#endif
		void Awake() {
			if ( _type != ProcessBody.Type.DontWork ) {
				SetupParents();
				SetupBrothers();
			}

			_belongSceneName = _lifeSpan == ProcessBody.LifeSpan.Forever ?
				ProcessBody.FOREVER_SCENE_NAME : gameObject.scene.name;
			if ( _lifeSpan == ProcessBody.LifeSpan.Forever )	{ DontDestroyOnLoad( gameObject ); }
			var nextActiveState = isActiveAndEnabled ?
				ProcessBody.ActiveState.Enabling : ProcessBody.ActiveState.Disabling;

			_process = new ProcessBody( this, nextActiveState );

			if ( _type != ProcessBody.Type.DontWork ) {
				_disposables.AddLast( () => {
					_parents.ForEach( p => p._children.Remove( this ) );
					_parents.Clear();
					_brothers.ForEach( p => p._children.Remove( this ) );
					_brothers.Clear();
				} );
			}
			if ( _type == ProcessBody.Type.DontWork ) {
				RunStateEvent( ProcessBody.RanState.Creating ).Forget();
			} else if ( _parents.IsEmpty() ) {
//				CoreProcessManager.s_instance.Register( this ).Forget();
//				_disposables.AddLast( () => CoreProcessManager.s_instance.Unregister( this ) );
			}
		}

#if DEVELOP
		protected
#endif
		void OnDestroy() {
			Log.Debug("OnDestroy");
			Dispose();
		}

		public void Dispose() => _process.Dispose();

		public abstract void Create();


/*
TODO : OnEnable、OnDisableは廃止し、必ずProcess経由で、ChangeActiveさせる
		protected void OnEnable() {
			Log.Debug( "OnEnable" );
			_process.ChangeActive( true, false ).Forget();
		}

		protected void OnDisable() {
			Log.Debug( "OnDisable" );
			_process.ChangeActive( false, false ).Forget();
		}
*/


		public void StopActiveAsync() => _process.StopActiveAsync();


		void SetupParents() {
			var parents = this.GetComponentsInParentUntilOneHierarchy<MonoBehaviourProcess>( true );
			while ( _parents.IsEmpty() && !parents.IsEmpty() ) {
				var nextParents = new List<MonoBehaviourProcess>();
				parents.ForEach( p => {
					if ( p._type != ProcessBody.Type.DontWork ) {
						_parents.Add( p );
						p._children.Add( this );
					} else {
						nextParents.Add(
							p.GetComponentsInParentUntilOneHierarchy<MonoBehaviourProcess>( true ) );
					}
				} );
				parents = nextParents;
			}
		}

		void SetupBrothers() {
			GetComponents<MonoBehaviourProcess>()
				.Where( p => p != this )
				.Where( p => p._type != ProcessBody.Type.DontWork )
				.ForEach( p => _brothers.Add( p ) );
		}

		public void ChangeParent( Transform parent, bool isWorldPositionStays ) {
			transform.SetParent( parent, isWorldPositionStays );
			_parents.ForEach( p => p._children.Remove( this ) );
			_parents.Clear();
			SetupParents();
		}


		public async UniTask RunStateEvent( ProcessBody.RanState state )
			=> await _process.RunStateEvent( state );

		public async UniTask RunStateEventOfBrothersAndChildren( ProcessBody.RanState state ) {
			foreach ( var p in _brothers ) {
				await p.RunStateEvent( state );
			}
			foreach ( var p in _children ) {
				await p.RunStateEvent( state );
			}
		}


		public async UniTask ChangeActive( bool isActive ) {
			_process._changeActiveOfOwner = isActiveOwner => {
				_process._changeActiveOfOwner = null;
				gameObject.SetActive( isActiveOwner );
			};
			await _process.ChangeActive( isActive );
		}

		public async UniTask ChangeActiveOfBrothersAndChildren( bool isActive ) {
			foreach ( var p in _brothers ) {
				await p._process.ChangeActive( isActive );
			}
			foreach ( var p in _children ) {
				await p._process.ChangeActive( isActive );
			}
		}


		public override string ToString() => this.ToDeepString();


#if DEVELOP
		protected void Start() {}
		protected void OnEnable() {}
		protected void FixedUpdate() {}
		protected void Update() {}
		protected void LateUpdate() {}
		protected void OnDisable() {}
#endif
	}
}