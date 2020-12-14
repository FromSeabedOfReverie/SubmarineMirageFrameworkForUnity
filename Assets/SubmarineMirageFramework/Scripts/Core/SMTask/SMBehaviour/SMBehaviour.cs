//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTask
//#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask {
	using Base;
	using MultiEvent;
	using UTask;
	using Modifyler;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class SMBehaviour : SMStandardBase, ISMBehaviour {
		public virtual SMTaskType _type => SMTaskType.Work;
		public virtual SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;

		public SMObject _object			{ get; set; }
		public SMBehaviourBody _body	{ get; protected set; }
		[Hide] public SMBehaviourModifyler _modifyler => _body._modifyler;
		[ShowLine] public ISMBehaviour _previous	{ get; set; }	// 常に無
		[ShowLine] public ISMBehaviour _next		{ get; set; }	// 常に無

		[Hide] public bool _isInitialized	=> _body._isInitialized;
		[Hide] public bool _isOperable		=> _body._isOperable;
		[Hide] public bool _isActive		=> _body._isActive;

		[Hide] public MultiAsyncEvent _selfInitializeEvent	=> _body._selfInitializeEvent;
		[Hide] public MultiAsyncEvent _initializeEvent		=> _body._initializeEvent;
		[Hide] public MultiSubject _enableEvent				=> _body._enableEvent;
		[Hide] public MultiSubject _fixedUpdateEvent		=> _body._fixedUpdateEvent;
		[Hide] public MultiSubject _updateEvent				=> _body._updateEvent;
		[Hide] public MultiSubject _lateUpdateEvent			=> _body._lateUpdateEvent;
		[Hide] public MultiSubject _disableEvent			=> _body._disableEvent;
		[Hide] public MultiAsyncEvent _finalizeEvent		=> _body._finalizeEvent;

		[Hide] public UTaskCanceler _asyncCancelerOnDisable	=> _body._asyncCancelerOnDisable;
		[Hide] public UTaskCanceler _asyncCancelerOnDispose	=> _body._asyncCancelerOnDispose;


		protected SMBehaviour( bool isDebug = false ) {
			_body = new SMBehaviourBody( this, true );
			_object = new SMObject( null, new ISMBehaviour[] { this }, null
#if TestSMTaskModifyler
				, isDebug
#endif
			);
#if TestSMTask
			Log.Debug( $"{nameof( SMBehaviour )}() : {this}" );
#endif
			_disposables.AddLast( _body );
		}

		public abstract void Create();


		public void DestroyObject() => _object.Destroy();

		public void ChangeActiveObject( bool isActive ) => _object.ChangeActive( isActive );

		public void StopAsyncOnDisable() => _body.StopAsyncOnDisable();


		public override void SetToString() {
			base.SetToString();
			SMBehaviourBody.SetBehaviourToString( this );
		}
	}
}