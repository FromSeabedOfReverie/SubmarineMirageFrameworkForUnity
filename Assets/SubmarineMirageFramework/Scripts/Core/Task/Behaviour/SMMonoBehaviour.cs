//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestBehaviour
namespace SubmarineMirage.Task.Behaviour {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using MultiEvent;
	using Modifyler;
	using Object;
	using Extension;
	using Debug;
	using Debug.ToString;


	// TODO : コメント追加、整頓


	public abstract class SMMonoBehaviour : MonoBehaviourSMExtension, ISMBehaviour {
		[SMHide] public SMMultiDisposable _disposables	{ get; private set; } = new SMMultiDisposable();
		[SMShowLine] public bool _isDispose => _disposables._isDispose;
		[SMHide] public SMToStringer _toStringer	{ get; private set; }

		public virtual SMTaskType _type => SMTaskType.Work;
		public virtual SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;

		public SMObject _object			{ get; set; }
		public SMBehaviourBody _body	{ get; private set; }
		[SMHide] public SMBehaviourModifyler _modifyler => _body?._modifyler;
		[SMShowLine] public ISMBehaviour _previous	{ get; set; }
		[SMShowLine] public ISMBehaviour _next		{ get; set; }

		[SMHide] public bool _isInitialized	=> _body?._isInitialized ?? false;
		[SMHide] public bool _isOperable	=> _body?._isOperable ?? false;
		[SMHide] public bool _isFinalizing	=> _body?._isFinalizing ?? false;
		[SMHide] public bool _isActive		=> _body?._isActive ?? false;

		[SMHide] public SMMultiAsyncEvent _selfInitializeEvent	=> _body?._selfInitializeEvent;
		[SMHide] public SMMultiAsyncEvent _initializeEvent		=> _body?._initializeEvent;
		[SMHide] public SMMultiSubject _enableEvent				=> _body?._enableEvent;
		[SMHide] public SMMultiSubject _fixedUpdateEvent		=> _body?._fixedUpdateEvent;
		[SMHide] public SMMultiSubject _updateEvent				=> _body?._updateEvent;
		[SMHide] public SMMultiSubject _lateUpdateEvent			=> _body?._lateUpdateEvent;
		[SMHide] public SMMultiSubject _disableEvent			=> _body?._disableEvent;
		[SMHide] public SMMultiAsyncEvent _finalizeEvent		=> _body?._finalizeEvent;

		[SMHide] public SMTaskCanceler _asyncCancelerOnDisable	=> _body?._asyncCancelerOnDisable;
		[SMHide] public SMTaskCanceler _asyncCancelerOnDispose	=> _body?._asyncCancelerOnDispose;


		protected override void Awake() {
			base.Awake();
			_toStringer = new SMToStringer( this );
			SetToString();
			_disposables.AddLast( () => {
				_toStringer.Dispose();
				_body?.Dispose();
			} );
		}

		public void Constructor() {
			_body = new SMBehaviourBody(
				this,
				_type != SMTaskType.DontWork && SMBehaviourApplyer.IsActiveInHierarchyAndMonoBehaviour( this )
			);
#if TestBehaviour
			_disposables.AddLast( () =>
				SMLog.Debug( $"{nameof( SMMonoBehaviour )}.{nameof( Dispose )} : {this}" )
			);
			SMLog.Debug( $"{nameof( SMMonoBehaviour )}.{nameof( Constructor )} : {this}" );
#endif
		}

		public override void Dispose() => _disposables.Dispose();

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


		
		public virtual void SetToString() => SMBehaviourBody.SetBehaviourToString( this );
		public override string ToString( int indent ) => _toStringer.Run( indent );
		public override string ToLineString( int indent = 0 ) => _toStringer.RunLine( indent );


#if DEVELOP
		protected void Start() {}
		protected void OnEnable() {}
		protected void OnDisable() {}
		protected void FixedUpdate() {}
		protected void Update() {}
		protected void LateUpdate() {}
#endif
	}
}