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

		public SMObject _object			{ get; set; }
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


		public T GetBehaviour<T>() where T : SMMonoBehaviour
			=> _object.GetBehaviour<T>();

		public SMMonoBehaviour GetBehaviour( Type type )
			=> (SMMonoBehaviour)_object.GetBehaviour( type );


		public IEnumerable<T> GetBehaviours<T>() where T : SMMonoBehaviour
			=> _object.GetBehaviours<T>();

		public IEnumerable<SMMonoBehaviour> GetBehaviours( Type type )
			=> _object.GetBehaviours( type )
				.Select( b => (SMMonoBehaviour)b );


		public T GetBehaviourInParent<T>() where T : SMMonoBehaviour
			=> _object.GetBehaviourInParent<T>();

		public SMMonoBehaviour GetBehaviourInParent( Type type )
			=> (SMMonoBehaviour)_object.GetBehaviourInParent( type );


		public IEnumerable<T> GetBehavioursInParent<T>() where T : SMMonoBehaviour
			=> _object.GetBehavioursInParent<T>();

		public IEnumerable<SMMonoBehaviour> GetBehavioursInParent( Type type )
			=> _object.GetBehavioursInParent( type )
				.Select( b => (SMMonoBehaviour)b );


		public T GetBehaviourInChildren<T>() where T : SMMonoBehaviour
			=> _object.GetBehaviourInChildren<T>();

		public SMMonoBehaviour GetBehaviourInChildren( Type type )
			=> (SMMonoBehaviour)_object.GetBehaviourInChildren( type );


		public IEnumerable<T> GetBehavioursInChildren<T>() where T : SMMonoBehaviour
			=> _object.GetBehavioursInChildren<T>();

		public IEnumerable<SMMonoBehaviour> GetBehavioursInChildren( Type type )
			=> _object.GetBehavioursInChildren( type )
				.Select( b => (SMMonoBehaviour)b );


		public T AddBehaviour<T>() where T : SMMonoBehaviour
			=> _object.AddBehaviour<T>();

		public SMMonoBehaviour AddBehaviour( Type type )
			=> _object.AddBehaviour( type );


		public void DestroyObject()
			=> _object.Destroy();

		public void ChangeParent( Transform parent, bool isWorldPositionStays )
			=> _object.ChangeParent( parent, isWorldPositionStays );




		public async UniTask RunStateEvent( SMTaskRanState state )
			=> await _body.RunStateEvent( state );


		public async UniTask ChangeActive( bool isActive )
			=> await _body.ChangeActive( isActive );

		public async UniTask RunActiveEvent()
			=> await _body.RunActiveEvent();


		public override string ToString()
			=> $"{this.GetAboutName()}( {_type}, {_lifeSpan}, {_object._owner} )";


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