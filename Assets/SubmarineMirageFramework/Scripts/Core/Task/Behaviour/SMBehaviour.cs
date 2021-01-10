//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestBehaviour
//#define TestBehaviourModifyler
namespace SubmarineMirage.Task.Behaviour {
	using Base;
	using MultiEvent;
	using Modifyler;
	using Object;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class SMBehaviour : SMStandardBase, ISMBehaviour {
		public virtual SMTaskType _type => SMTaskType.Work;
		public virtual SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;

		public SMObject _object			{ get; set; }
		public SMBehaviourBody _body	{ get; protected set; }
		[SMHide] public SMBehaviourModifyler _modifyler => _body._modifyler;
		[SMShowLine] public ISMBehaviour _previous	{ get; set; }	// 常に無
		[SMShowLine] public ISMBehaviour _next		{ get; set; }	// 常に無

		[SMHide] public bool _isInitialized	=> _body._isInitialized;
		[SMHide] public bool _isOperable	=> _body._isOperable;
		[SMHide] public bool _isActive		=> _body._isActive;

		[SMHide] public SMMultiAsyncEvent _selfInitializeEvent	=> _body._selfInitializeEvent;
		[SMHide] public SMMultiAsyncEvent _initializeEvent		=> _body._initializeEvent;
		[SMHide] public SMMultiSubject _enableEvent				=> _body._enableEvent;
		[SMHide] public SMMultiSubject _fixedUpdateEvent		=> _body._fixedUpdateEvent;
		[SMHide] public SMMultiSubject _updateEvent				=> _body._updateEvent;
		[SMHide] public SMMultiSubject _lateUpdateEvent			=> _body._lateUpdateEvent;
		[SMHide] public SMMultiSubject _disableEvent			=> _body._disableEvent;
		[SMHide] public SMMultiAsyncEvent _finalizeEvent		=> _body._finalizeEvent;

		[SMHide] public SMTaskCanceler _asyncCancelerOnDisable	=> _body._asyncCancelerOnDisable;
		[SMHide] public SMTaskCanceler _asyncCancelerOnDispose	=> _body._asyncCancelerOnDispose;


		protected SMBehaviour( bool isDebug = false ) {
			_body = new SMBehaviourBody( this, _type != SMTaskType.DontWork );
			_object = new SMObject( null, new ISMBehaviour[] { this }, null
#if TestBehaviourModifyler
				, isDebug
#endif
			);
#if TestBehaviour
			SMLog.Debug( $"{nameof( SMBehaviour )}() : {this}" );
#endif
			_disposables.AddLast( _body );
		}

		public abstract void Create();

		public override void Dispose() => base.Dispose();


		public void DestroyObject() => _object.Destroy();

		public void ChangeActiveObject( bool isActive ) => _object.ChangeActive( isActive );

		public void StopAsyncOnDisable() => _body.StopAsyncOnDisable();


		public override void SetToString() {
			base.SetToString();
			SMBehaviourBody.SetBehaviourToString( this );
		}
	}
}