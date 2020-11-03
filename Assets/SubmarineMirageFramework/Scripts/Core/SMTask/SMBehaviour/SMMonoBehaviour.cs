//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTask
namespace SubmarineMirage.SMTask {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using MultiEvent;
	using UTask;
	using Modifyler;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class SMMonoBehaviour : MonoBehaviourExtension, ISMBehaviour {
		public virtual SMTaskType _type => SMTaskType.Work;
		public virtual SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;

		public uint _id => _body?._id ?? 0;
		public SMObject _object			{ get; set; }
		public SMBehaviourBody _body	{ get; private set; }
		public SMBehaviourModifyler _modifyler => _body?._modifyler;
		public ISMBehaviour _previous	{ get; set; }
		public ISMBehaviour _next		{ get; set; }

		public bool _isInitialized =>	_body?._isInitialized ?? false;
		public bool _isActive =>		_body?._isActive ?? false;
		public bool _isDispose =>		_body?._isDispose ?? false;

		public MultiAsyncEvent _selfInitializeEvent	=> _body?._selfInitializeEvent;
		public MultiAsyncEvent _initializeEvent		=> _body?._initializeEvent;
		public MultiSubject _enableEvent			=> _body?._enableEvent;
		public MultiSubject _fixedUpdateEvent		=> _body?._fixedUpdateEvent;
		public MultiSubject _updateEvent			=> _body?._updateEvent;
		public MultiSubject _lateUpdateEvent		=> _body?._lateUpdateEvent;
		public MultiSubject _disableEvent			=> _body?._disableEvent;
		public MultiAsyncEvent _finalizeEvent		=> _body?._finalizeEvent;

		public UTaskCanceler _asyncCancelerOnDisable	=> _body?._asyncCancelerOnDisable;
		public UTaskCanceler _asyncCancelerOnDispose	=> _body?._asyncCancelerOnDispose;

		public MultiDisposable _disposables	=> _body?._disposables;


		public void Constructor() {
			_body = new SMBehaviourBody(
				this,
				isActiveAndEnabled ? SMTaskActiveState.Enable : SMTaskActiveState.Disable
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
			if ( _body != null )	{ _body.Dispose(); }
			else					{ Destroy( this ); }
		}

		public abstract void Create();

		public void DestroyObject() => _object.Destroy();

		public void ChangeActiveObject( bool isActive ) => _object.ChangeActive( isActive );

		public void StopAsyncOnDisable() => _body?.StopAsyncOnDisable();


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



		public void ChangeParent( Transform parent, bool isWorldPositionStays = true )
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