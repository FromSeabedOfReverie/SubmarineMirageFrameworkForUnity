//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask {
	using System;
	using System.Linq;
	using System.Threading;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx.Async;
	using MultiEvent;
	using Extension;


	// TODO : コメント追加、整頓


	public abstract class SMMonoBehaviour : MonoBehaviourExtension, ISMBehavior {
		public virtual SMTaskType _type => SMTaskType.Work;
		public virtual SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;

		public SMHierarchy _hierarchy	{ get; set; }
		public SMBehaviorBody _body		{ get; private set; }
		public ISMBehavior _previous	{ get; set; }
		public ISMBehavior _next		{ get; set; }

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
			_body = new SMBehaviorBody(
				this,
				isActiveAndEnabled ? SMTaskActiveState.Enabling : SMTaskActiveState.Disabling
			);
		}

#if DEVELOP
		protected
#endif
		void OnDestroy() => Dispose();

		public void Dispose() => _body?.Dispose();

		public abstract void Create();


		public void StopActiveAsync() => _body.StopActiveAsync();




		public T GetProcess<T>() where T : SMMonoBehaviour
			=> _hierarchy.GetProcess<T>();

		public SMMonoBehaviour GetProcess( Type type )
			=> (SMMonoBehaviour)_hierarchy.GetProcess( type );


		public IEnumerable<T> GetProcesses<T>() where T : SMMonoBehaviour
			=> _hierarchy.GetProcesses<T>();

		public IEnumerable<SMMonoBehaviour> GetProcesses( Type type ) {
			return _hierarchy.GetProcesses( type )
				.Select( p => (SMMonoBehaviour)p );
		}


		public T GetProcessInParent<T>() where T : SMMonoBehaviour
			=> _hierarchy.GetProcessInParent<T>();

		public SMMonoBehaviour GetProcessInParent( Type type )
			=> (SMMonoBehaviour)_hierarchy.GetProcessInParent( type );


		public IEnumerable<T> GetProcessesInParent<T>() where T : SMMonoBehaviour
			=> _hierarchy.GetProcessesInParent<T>();

		public IEnumerable<SMMonoBehaviour> GetProcessesInParent( Type type ) {
			return _hierarchy.GetProcessesInParent( type )
				.Select( p => (SMMonoBehaviour)p );
		}


		public T GetProcessInChildren<T>() where T : SMMonoBehaviour
			=> _hierarchy.GetProcessInChildren<T>();

		public SMMonoBehaviour GetProcessInChildren( Type type )
			=> (SMMonoBehaviour)_hierarchy.GetProcessInChildren( type );


		public IEnumerable<T> GetProcessesInChildren<T>() where T : SMMonoBehaviour
			=> _hierarchy.GetProcessesInChildren<T>();

		public IEnumerable<SMMonoBehaviour> GetProcessesInChildren( Type type ) {
			return _hierarchy.GetProcessesInChildren( type )
				.Select( p => (SMMonoBehaviour)p );
		}


		public T AddProcess<T>() where T : SMMonoBehaviour
			=> _hierarchy.AddProcess<T>();

		public SMMonoBehaviour AddProcess( Type type )
			=> _hierarchy.AddProcess( type );


		public void DestroyHierarchy()
			=> _hierarchy.Destroy();

		public void ChangeParent( Transform parent, bool isWorldPositionStays )
			=> _hierarchy.ChangeParent( parent, isWorldPositionStays );




		public async UniTask RunStateEvent( SMTaskRanState state )
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