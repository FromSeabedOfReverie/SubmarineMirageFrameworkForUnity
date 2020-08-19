//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestSMTask
namespace SubmarineMirage.SMTask {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using MultiEvent;
	using UTask;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class SMMonoBehaviour : MonoBehaviourExtension, ISMBehaviour {
		public virtual SMTaskType _type => SMTaskType.Work;
		public virtual SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;

		public SMObject _object			{ get; set; }
		public SMBehaviourBody _body	{ get; private set; }
		public uint _id => _body?._id ?? 0;
		public ISMBehaviour _previous	{ get; set; }
		public ISMBehaviour _next		{ get; set; }

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

		public UTaskCanceler _activeAsyncCanceler	=> _body._activeAsyncCanceler;
		public UTaskCanceler _inActiveAsyncCanceler	=> _body._inActiveAsyncCanceler;

		public MultiDisposable _disposables	=> _body._disposables;


		public void Constructor() {
			_body = new SMBehaviourBody(
				this,
				isActiveAndEnabled ? SMTaskActiveState.Enabling : SMTaskActiveState.Disabling
			);
#if TestSMTask
			_disposables.AddLast( () =>
				Log.Debug( $"{nameof( SMMonoBehaviour )}.{nameof( Dispose )} : {this}" )
			);
			Log.Debug( $"{nameof( SMMonoBehaviour )}.{nameof( Constructor )} : {this}" );
#endif
		}

#if DEVELOP
		protected
#endif
		void OnDestroy() => Dispose();

		public void Dispose() {
			_body?.Dispose();
			if ( _object != null && _object._behaviour == null )	{ _object.Dispose(); }
		}

		public abstract void Create();

		public void DestroyObject() => _object.Destroy();

		public void StopActiveAsync() => _body.StopActiveAsync();


		public UniTask RunStateEvent( SMTaskRanState state ) => _body.RunStateEvent( state );

		public UniTask ChangeActive( bool isActive ) => _body.ChangeActive( isActive );

		public UniTask RunActiveEvent() => _body.RunActiveEvent();



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



		public void ChangeParent( Transform parent, bool isWorldPositionStays )
			=> _object.ChangeParent( parent, isWorldPositionStays );


		public override string ToString() => SMBehaviourBody.BehaviourToString( this );

		public string ToLineString() => SMBehaviourBody.BehaviourToLineString( this );


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