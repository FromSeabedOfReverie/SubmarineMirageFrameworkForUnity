//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask {
	using Cysharp.Threading.Tasks;
	using MultiEvent;
	using UTask;
	using Extension;


	// TODO : コメント追加、整頓


	public abstract class SMBehaviour : ISMBehaviour {
		public virtual SMTaskType _type => SMTaskType.Work;
		public virtual SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;

		public SMObject _object			{ get; set; }
		public SMBehaviourBody _body	{ get; protected set; }
		public uint _id => _body?._id ?? 0;
		public ISMBehaviour _previous	{ get; set; }	// 常に無
		public ISMBehaviour _next		{ get; set; }	// 常に無

		public bool _isInitialized	=> _body._isInitialized;
		public bool _isActive		=> _body._isActive;

		public MultiAsyncEvent _loadEvent		=> _body._loadEvent;
		public MultiAsyncEvent _initializeEvent	=> _body._initializeEvent;
		public MultiAsyncEvent _enableEvent		=> _body._enableEvent;
		public MultiSubject _fixedUpdateEvent	=> _body._fixedUpdateEvent;
		public MultiSubject _updateEvent		=> _body._updateEvent;
		public MultiSubject _lateUpdateEvent	=> _body._lateUpdateEvent;
		public MultiAsyncEvent _disableEvent	=> _body._disableEvent;
		public MultiAsyncEvent _finalizeEvent	=> _body._finalizeEvent;

		public UTaskCanceler _activeAsyncCanceler	=> _body._activeAsyncCanceler;
		public UTaskCanceler _inActiveAsyncCanceler	=> _body._inActiveAsyncCanceler;

		public MultiDisposable _disposables	=> _body._disposables;


		protected SMBehaviour() {
			_body = new SMBehaviourBody( this, SMTaskActiveState.Enabling );
			_object = new SMObject( null, new ISMBehaviour[] { this }, null );
		}

		~SMBehaviour() => Dispose();

		public void Dispose() => _body.Dispose();

		public abstract void Create();

		public void DestroyObject() => _object.Destroy();

		public void StopActiveAsync() => _body.StopActiveAsync();


		public async UniTask RunStateEvent( SMTaskRanState state )
			=> await _body.RunStateEvent( state );

		public async UniTask ChangeActive( bool isActive )
			=> await _body.ChangeActive( isActive );

		public async UniTask RunActiveEvent()
			=> await _body.RunActiveEvent();


		public override string ToString() => string.Join( "\n",
			$"{this.GetAboutName()}(",
			$"    {nameof( _type )} : {_type}",
			$"    {nameof( _lifeSpan )} : {_lifeSpan}",
			string.Join( "",
				$"    {nameof( _object._owner )} : ",
				$"{( _object._owner != null ? _object._owner.ToString() : "null" )}",
				$"( {_object._id} )"
			),
			$"    {nameof( _previous )} : {_previous?._id}",
			$"    {nameof( _next )} : {_next?._id}",
			$"    {nameof( _body )} : {_body}",
			")"
		);
	}
}