//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task {
	using System;
	using SubmarineMirage.Base;
	using MultiEvent;
	using Task.Base;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMBehaviour : SMStandardBase, ISMBehaviour {
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



		public SMBehaviour() {
			_body = new SMBehaviourBody( this );

			_disposables.AddLast( () => {
				_body.Dispose();
			} );
		}

		public void Setup() {
			var group = new SMGroup( null, new ISMBehaviour[] { this } );
			_body._objectBody = group._body._objectBody;
		}

		public override void Dispose() => base.Dispose();

		public abstract void Create();



		public static T Generate<T>() where T : ISMBehaviour
			=> SMBehaviourBody.Generate<T>();

		public static ISMBehaviour Generate( Type type )
			=> SMBehaviourBody.Generate( type );



		public void DestroyObject() => _object.Destroy();

		public void ChangeActiveObject( bool isActive ) => _object.ChangeActive( isActive );
	}
}