//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Process.New {
	using System.Linq;
	using System.Threading;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx.Async;
	using MultiEvent;
	using Extension;


	// TODO : コメント追加、整頓


	public abstract class MonoBehaviourProcess : MonoBehaviourExtension, IProcess {
		public virtual ProcessBody.Type _type => ProcessBody.Type.Work;
		public virtual ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.InScene;

		public ProcessHierarchy _hierarchy	{ get; set; }
		public ProcessBody _body			{ get; private set; }

		public bool _isInitialized	=> _body._isInitialized;
		public bool _isActive		=> _body._isActive;

		public MultiAsyncEvent _loadEvent		=> _body._loadEvent;
		public MultiAsyncEvent _initializeEvent	=> _body._initializeEvent;
		public MultiAsyncEvent _enableEvent		=> _body._enableEvent;
		public MultiSubject _fixedUpdateEvent	=> _body._fixedUpdateEvent;
		public MultiSubject _updateEvent		=> _body._updateEvent;
		public MultiSubject _lateUpdateEvent	=> _body._lateUpdateEvent;
		public MultiAsyncEvent _disableEvent	=> _body._disableEvent;
		public MultiAsyncEvent _finalizeEvent	=> _body._finalizeEvent;

		public MultiSubject _activeAsyncCancelEvent		=> _body._activeAsyncCancelEvent;
		public CancellationToken _activeAsyncCancel		=> _body._activeAsyncCancel;
		public CancellationToken _inActiveAsyncCancel	=> _body._inActiveAsyncCancel;

		public MultiDisposable _disposables	=> _body._disposables;


		public void Constructor() {
			_body = new ProcessBody(
				this,
				isActiveAndEnabled ? ProcessBody.ActiveState.Enabling : ProcessBody.ActiveState.Disabling
			);
		}

#if DEVELOP
		protected
#endif
		void OnDestroy() => Dispose();

		public void Dispose() => _body?.Dispose();

		public abstract void Create();


		public void StopActiveAsync() => _body.StopActiveAsync();




		public T GetProcess<T>() where T : MonoBehaviourProcess
			=> _hierarchy.GetProcess<T>();

		public MonoBehaviourProcess GetProcess( System.Type type )
			=> (MonoBehaviourProcess)_hierarchy.GetProcess( type );


		public List<T> GetProcesses<T>() where T : MonoBehaviourProcess
			=> _hierarchy.GetProcesses<T>();

		public List<MonoBehaviourProcess> GetProcesses( System.Type type ) {
			return _hierarchy.GetProcesses( type )
				.Select( p => (MonoBehaviourProcess)p )
				.ToList();
		}


		public T GetProcessInParent<T>() where T : MonoBehaviourProcess
			=> _hierarchy.GetProcessInParent<T>();

		public MonoBehaviourProcess GetProcessInParent( System.Type type )
			=> (MonoBehaviourProcess)_hierarchy.GetProcessInParent( type );


		public List<T> GetProcessesInParent<T>() where T : MonoBehaviourProcess
			=> _hierarchy.GetProcessesInParent<T>();

		public List<MonoBehaviourProcess> GetProcessesInParent( System.Type type ) {
			return _hierarchy.GetProcessesInParent( type )
				.Select( p => (MonoBehaviourProcess)p )
				.ToList();
		}


		public T GetProcessInChildren<T>() where T : MonoBehaviourProcess
			=> _hierarchy.GetProcessInChildren<T>();

		public MonoBehaviourProcess GetProcessInChildren( System.Type type )
			=> (MonoBehaviourProcess)_hierarchy.GetProcessInChildren( type );


		public List<T> GetProcessesInChildren<T>() where T : MonoBehaviourProcess
			=> _hierarchy.GetProcessesInChildren<T>();

		public List<MonoBehaviourProcess> GetProcessesInChildren( System.Type type ) {
			return _hierarchy.GetProcessesInChildren( type )
				.Select( p => (MonoBehaviourProcess)p )
				.ToList();
		}


		public T AddProcess<T>() where T : MonoBehaviourProcess
			=> _hierarchy.AddProcess<T>();

		public MonoBehaviourProcess AddProcess( System.Type type )
			=> _hierarchy.AddProcess( type );


		public void DestroyHierarchy()
			=> _hierarchy.Destroy();

		public void ChangeParent( Transform parent, bool isWorldPositionStays )
			=> _hierarchy.ChangeParent( parent, isWorldPositionStays );




		public async UniTask RunStateEvent( ProcessBody.RanState state )
			=> await _body.RunStateEvent( state );


		public async UniTask ChangeActive( bool isActive )
			=> await _body.ChangeActive( isActive );

		public async UniTask RunActiveEvent()
			=> await _body.RunActiveEvent();


		public override string ToString()
			=> $"{this.GetAboutName()}( {_type}, {_lifeSpan}, {_hierarchy._owner} )";


#if DEVELOP
		protected void Awake() {}
		protected void Start() {}
		protected void OnEnable() {}
		protected void OnDisable() {}
		protected void FixedUpdate() {}
		protected void Update() {}
		protected void LateUpdate() {}
#endif
	}
}