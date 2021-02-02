//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task {
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using MultiEvent;
	using Task.Base;
	using Extension;
	using Utility;
	using Debug;
	using Debug.ToString;


	// TODO : コメント追加、整頓


	public abstract class SMMonoBehaviour : MonoBehaviourSMExtension, ISMBehaviour {
		[SMHide] public SMMultiDisposable _disposables	{ get; private set; } = new SMMultiDisposable();
		[SMShowLine] public bool _isDispose => _disposables._isDispose;
		[SMHide] public SMToStringer _toStringer	{ get; private set; }

		public virtual SMTaskType _type => SMTaskType.Work;
		public virtual SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;

		public SMBehaviourBody _body	{ get; private set; }
		[SMHide] public SMObject _object	=> _body._objectBody._object;

		[SMHide] public bool _isInitialized	=> _body._isInitialized;
		[SMHide] public bool _isOperable	=> _body._isOperable;
		[SMHide] public bool _isFinalizing	=> _body._isFinalizing;
		[SMHide] public bool _isActive		=> _body._isActive;

		[SMHide] public SMMultiAsyncEvent _selfInitializeEvent	=> _body._selfInitializeEvent;
		[SMHide] public SMMultiAsyncEvent _initializeEvent		=> _body._initializeEvent;
		[SMHide] public SMMultiSubject _enableEvent				=> _body._enableEvent;
		[SMHide] public SMMultiSubject _fixedUpdateEvent		=> _body._fixedUpdateEvent;
		[SMHide] public SMMultiSubject _updateEvent				=> _body._updateEvent;
		[SMHide] public SMMultiSubject _lateUpdateEvent			=> _body._lateUpdateEvent;
		[SMHide] public SMMultiSubject _disableEvent			=> _body._disableEvent;
		[SMHide] public SMMultiAsyncEvent _finalizeEvent		=> _body._finalizeEvent;

		[SMHide] public SMAsyncCanceler _asyncCancelerOnDisable	=> _body._asyncCancelerOnDisable;
		[SMHide] public SMAsyncCanceler _asyncCancelerOnDispose	=> _body._asyncCancelerOnDispose;


		protected override void Awake() {
			base.Awake();

			_toStringer = new SMToStringer( this );
			SetToString();
			_body = new SMBehaviourBody( this );

			_disposables.AddLast( () => {
				_toStringer.Dispose();
				_body.Dispose();
			} );
		}

		public override void Dispose() => _disposables.Dispose();

		public abstract void Create();



		public static T Generate<T>() where T : ISMBehaviour
			=> SMBehaviourBody.Generate<T>();

		public static ISMBehaviour Generate( Type type )
			=> SMBehaviourBody.Generate( type );



		public void DestroyObject()
			=> _object.Destroy();

		public void ChangeActiveObject( bool isActive )
			=> _object.ChangeActive( isActive );

		public void ChangeParent( Transform parent, bool isWorldPositionStays = true )
			=> _object.ChangeParent( parent, isWorldPositionStays );


		public T AddBehaviour<T>() where T : SMMonoBehaviour
			=> _object.AddBehaviour<T>();

		public SMMonoBehaviour AddBehaviour( Type type )
			=> _object.AddBehaviour( type );



		public T GetBehaviour<T>() where T : ISMBehaviour
			=> _body.GetBehaviour<T>();

		public ISMBehaviour GetBehaviour( Type type )
			=> _body.GetBehaviour( type );

		public IEnumerable<T> GetBehaviours<T>() where T : ISMBehaviour
			=> _body.GetBehaviours<T>();

		public IEnumerable<ISMBehaviour> GetBehaviours( Type type )
			=> _body.GetBehaviours( type );


		public T GetBehaviourInParent<T>() where T : ISMBehaviour
			=> _object.GetBehaviourInParent<T>();

		public ISMBehaviour GetBehaviourInParent( Type type )
			=> _object.GetBehaviourInParent( type );

		public IEnumerable<T> GetBehavioursInParent<T>() where T : ISMBehaviour
			=> _object.GetBehavioursInParent<T>();

		public IEnumerable<ISMBehaviour> GetBehavioursInParent( Type type )
			=> _object.GetBehavioursInParent( type );


		public T GetBehaviourInChildren<T>() where T : ISMBehaviour
			=> _object.GetBehaviourInChildren<T>();

		public ISMBehaviour GetBehaviourInChildren( Type type )
			=> _object.GetBehaviourInChildren( type );

		public IEnumerable<T> GetBehavioursInChildren<T>() where T : ISMBehaviour
			=> _object.GetBehavioursInChildren<T>();

		public IEnumerable<ISMBehaviour> GetBehavioursInChildren( Type type )
			=> _object.GetBehavioursInChildren( type );


		
		public virtual void SetToString() {}
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