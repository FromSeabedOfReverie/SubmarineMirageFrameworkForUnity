//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Behaviour {
	using System;
	using Cysharp.Threading.Tasks;
	using UnityEngine;
	using KoganeUnityLib;
	using Base;
	using Event;
	using Task;
	using Scene;
	using Extension;
	using Utility;
	using Debug;
	using Debug.ToString;



	public abstract class SMBehaviour : MonoBehaviourSMExtension, ISMStandardBase {
		[SMShow] public virtual SMTaskRunType _type => SMTaskRunType.Parallel;

		[SMShow] public SMBehaviourBody _body	{ get; private set; }
		protected SMScene _scene				=> _body._scene;

		public bool _isInitialized	=> _body._isInitialized;
		public bool _isOperable		=> _body._isOperable;
		public bool _isActive		=> _body._isActive;

		public SMAsyncEvent _selfInitializeEvent	=> _body._selfInitializeEvent;
		public SMAsyncEvent _initializeEvent		=> _body._initializeEvent;
		public SMSubject _enableEvent				=> _body._enableEvent;
		public SMSubject _fixedUpdateEvent			=> _body._fixedUpdateEvent;
		public SMSubject _updateEvent				=> _body._updateEvent;
		public SMSubject _lateUpdateEvent			=> _body._lateUpdateEvent;
		public SMSubject _disableEvent				=> _body._disableEvent;
		public SMAsyncEvent _finalizeEvent			=> _body._finalizeEvent;

		public SMAsyncCanceler _asyncCancelerOnDisable	=> _body._asyncCancelerOnDisable;
		public SMAsyncCanceler _asyncCancelerOnDispose	=> _body._asyncCancelerOnDispose;

		public SMDisposable _disposables	{ get; private set; } = new SMDisposable();
		[SMShowLine] public bool _isDispose => _disposables._isDispose;
		public SMToStringer _toStringer	{ get; private set; }



		public static T Generate<T>( SMScene scene ) where T : SMBehaviour
			=> Generate( typeof( T ), scene ) as T;

		public static SMBehaviour Generate( Type type, SMScene scene ) {
			if ( !type.IsInheritance( typeof( SMBehaviour ) ) ) {
				throw new InvalidOperationException(
					$"{nameof( SMBehaviour )}でない為、作成不可 : {type.GetAboutName()}" );
			}
			var go = new GameObject( type.GetAboutName() );
			var b = go.AddComponent( type ) as SMBehaviour;
			b.Constructor( scene );
			return b;
		}

		public static GameObject Instantiate( GameObject original, SMScene scene, Vector3? position = null,
												Quaternion? rotation = null
		) {
			var go = (
				position.HasValue	? original.Instantiate( position.Value, rotation.Value )
									: original.Instantiate()
			);
			scene.MoveGameObject( go );
			go.GetComponentsInChildren<SMBehaviour>().ForEach( b => {
				b.Constructor( scene );
			} );
			return go;
		}



		protected override void Awake() {
			base.Awake();

			_toStringer = new SMToStringer( this );
			SetToString();

			_disposables.AddLast( () => {
				_toStringer.Dispose();
				_body?.Dispose();
			} );
		}

		public void Constructor( SMScene scene ) {
			_body = new SMBehaviourBody( this, scene );
		}

		public abstract void Create();

		public override void Dispose() => _disposables.Dispose();



		public UniTask DestroyObject()
			=> _body.DestroyObject();

		public UniTask ChangeActiveObject( bool isActive )
			=> _body.ChangeActiveObject( isActive );

		public UniTask ChangeParentObject( Transform parent, bool isWorldPositionStays = true )
			=> _body.ChangeParentObject( parent, isWorldPositionStays );



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