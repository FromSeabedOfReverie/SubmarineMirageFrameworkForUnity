//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTask
//#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask {
	using Cysharp.Threading.Tasks;
	using MultiEvent;
	using UTask;
	using Modifyler;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class SMBehaviour : ISMBehaviour {
		public virtual SMTaskType _type => SMTaskType.Work;
		public virtual SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;

		public uint _id => _body._id;
		public SMObject _object			{ get; set; }
		public SMBehaviourBody _body	{ get; protected set; }
		public SMBehaviourModifyler _modifyler => _body._modifyler;
		public ISMBehaviour _previous	{ get; set; }	// 常に無
		public ISMBehaviour _next		{ get; set; }	// 常に無

		public bool _isInitialized =>	_body._isInitialized;
		public bool _isOperable =>		_body._isOperable;
		public bool _isActive =>		_body._isActive;
		public bool _isDispose =>		_body._isDispose;

		public MultiAsyncEvent _selfInitializeEvent	=> _body._selfInitializeEvent;
		public MultiAsyncEvent _initializeEvent		=> _body._initializeEvent;
		public MultiSubject _enableEvent			=> _body._enableEvent;
		public MultiSubject _fixedUpdateEvent		=> _body._fixedUpdateEvent;
		public MultiSubject _updateEvent			=> _body._updateEvent;
		public MultiSubject _lateUpdateEvent		=> _body._lateUpdateEvent;
		public MultiSubject _disableEvent			=> _body._disableEvent;
		public MultiAsyncEvent _finalizeEvent		=> _body._finalizeEvent;

		public UTaskCanceler _asyncCancelerOnDisable	=> _body._asyncCancelerOnDisable;
		public UTaskCanceler _asyncCancelerOnDispose	=> _body._asyncCancelerOnDispose;

		public MultiDisposable _disposables	=> _body._disposables;


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
		}

		~SMBehaviour() => Dispose();

		public void Dispose() => _body.Dispose();

		public abstract void Create();


		public void DestroyObject() => _object.Destroy();

		public void ChangeActiveObject( bool isActive ) => _object.ChangeActive( isActive );

		public void StopAsyncOnDisable() => _body.StopAsyncOnDisable();

		public override string ToString() => SMBehaviourBody.BehaviourToString( this );

		public string ToLineString() => SMBehaviourBody.BehaviourToLineString( this );
	}
}