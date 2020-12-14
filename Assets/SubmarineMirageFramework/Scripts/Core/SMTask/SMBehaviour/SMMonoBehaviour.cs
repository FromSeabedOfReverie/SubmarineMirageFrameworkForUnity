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
	using Debug.ToString;


	// TODO : コメント追加、整頓


	public abstract class SMMonoBehaviour : MonoBehaviourSMExtension, ISMBehaviour {
		[Hide] public MultiDisposable _disposables	{ get; private set; } = new MultiDisposable();
		[ShowLine] public bool _isDispose => _disposables._isDispose;
		[Hide] public SMToStringer _toStringer	{ get; private set; }

		public virtual SMTaskType _type => SMTaskType.Work;
		public virtual SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;

		public SMObject _object			{ get; set; }
		public SMBehaviourBody _body	{ get; private set; }
		[Hide] public SMBehaviourModifyler _modifyler => _body?._modifyler;
		[ShowLine] public ISMBehaviour _previous	{ get; set; }
		[ShowLine] public ISMBehaviour _next		{ get; set; }

		[Hide] public bool _isInitialized	=> _body?._isInitialized ?? false;
		[Hide] public bool _isOperable		=> _body?._isOperable ?? false;
		[Hide] public bool _isActive		=> _body?._isActive ?? false;

		[Hide] public MultiAsyncEvent _selfInitializeEvent	=> _body?._selfInitializeEvent;
		[Hide] public MultiAsyncEvent _initializeEvent		=> _body?._initializeEvent;
		[Hide] public MultiSubject _enableEvent				=> _body?._enableEvent;
		[Hide] public MultiSubject _fixedUpdateEvent		=> _body?._fixedUpdateEvent;
		[Hide] public MultiSubject _updateEvent				=> _body?._updateEvent;
		[Hide] public MultiSubject _lateUpdateEvent			=> _body?._lateUpdateEvent;
		[Hide] public MultiSubject _disableEvent			=> _body?._disableEvent;
		[Hide] public MultiAsyncEvent _finalizeEvent		=> _body?._finalizeEvent;

		[Hide] public UTaskCanceler _asyncCancelerOnDisable	=> _body?._asyncCancelerOnDisable;
		[Hide] public UTaskCanceler _asyncCancelerOnDispose	=> _body?._asyncCancelerOnDispose;


		protected override void Awake() {
			base.Awake();
			_toStringer = new SMToStringer( this );
			SetToString();
			_disposables.AddLast( () => {
				if ( _body != null )	{ _body.Dispose(); }
				else					{ Destroy( this ); }
			} );
		}

		public void Constructor() {
			_body = new SMBehaviourBody( this, isActiveAndEnabled );
#if TestSMTask
			_disposables.AddLast( () =>
				Log.Debug( $"{nameof( SMMonoBehaviour )}.{nameof( Dispose )} : {this}" )
			);
			Log.Debug( $"{nameof( SMMonoBehaviour )}.{nameof( Constructor )} : {this}" );
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