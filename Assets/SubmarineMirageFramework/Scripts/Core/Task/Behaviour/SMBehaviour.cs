//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using SubmarineMirage.Base;
	using MultiEvent;
	using Task.Base;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using Debug.ToString;


	// TODO : コメント追加、整頓


	public abstract class SMBehaviour : MonoBehaviourSMExtension, ISMStandardBase {
		public virtual SMTaskType _type => SMTaskType.Work;

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

		[SMHide] public SMMultiDisposable _disposables	{ get; private set; } = new SMMultiDisposable();
		[SMShowLine] public bool _isDispose => _disposables._isDispose;
		[SMHide] public SMToStringer _toStringer	{ get; private set; }



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



		public static T Generate<T>( SMScene scene ) where T : SMBehaviour
			=> (T)Generate( typeof( T ), scene );

		public static SMBehaviour Generate( Type type, SMScene scene ) {
			if ( !type.IsInheritance( typeof( SMBehaviour ) ) ) {
				throw new InvalidOperationException(
					$"{nameof( SMBehaviour )}でない為、作成不可 : {type.GetAboutName()}" );
			}
			var go = new GameObject( type.GetAboutName() );
			var b = (SMBehaviour)go.AddComponent( type );
			new SMGroup( scene._groupManagerBody, go, new SMBehaviour[] { b } );
			return b;
		}



		public void DestroyObject()
			=> _object.Destroy();

		public void ChangeActiveObject( bool isActive )
			=> _object.ChangeActive( isActive );

		public void ChangeParent( Transform parent, bool isWorldPositionStays = true )
			=> _object.ChangeParent( parent, isWorldPositionStays );


		public T AddBehaviour<T>() where T : SMBehaviour
			=> _object.AddBehaviour<T>();

		public SMBehaviour AddBehaviour( Type type )
			=> _object.AddBehaviour( type );



		public T GetBehaviour<T>() where T : SMBehaviour
			=> (T)GetBehaviour( typeof( T ) );

		public SMBehaviour GetBehaviour( Type type )
			=> GetBehaviours( type ).FirstOrDefault();

		public IEnumerable<T> GetBehaviours<T>() where T : SMBehaviour
			=> GetBehaviours( typeof( T ) ).Select( b => (T)b );

		public IEnumerable<SMBehaviour> GetBehaviours( Type type )
			=> _body.GetBrothers()
				.Select( b => b._behaviour )
				.Where( b => b.GetType().IsInheritance( type ) );


		public T GetBehaviourInParent<T>() where T : SMBehaviour
			=> _object.GetBehaviourInParent<T>();

		public SMBehaviour GetBehaviourInParent( Type type )
			=> _object.GetBehaviourInParent( type );

		public IEnumerable<T> GetBehavioursInParent<T>() where T : SMBehaviour
			=> _object.GetBehavioursInParent<T>();

		public IEnumerable<SMBehaviour> GetBehavioursInParent( Type type )
			=> _object.GetBehavioursInParent( type );


		public T GetBehaviourInChildren<T>() where T : SMBehaviour
			=> _object.GetBehaviourInChildren<T>();

		public SMBehaviour GetBehaviourInChildren( Type type )
			=> _object.GetBehaviourInChildren( type );

		public IEnumerable<T> GetBehavioursInChildren<T>() where T : SMBehaviour
			=> _object.GetBehavioursInChildren<T>();

		public IEnumerable<SMBehaviour> GetBehavioursInChildren( Type type )
			=> _object.GetBehavioursInChildren( type );


		
		public virtual void SetToString() {}

		public override string ToString( int indent, bool isUseHeadIndent = true )
			=> _toStringer.Run( indent, isUseHeadIndent );

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