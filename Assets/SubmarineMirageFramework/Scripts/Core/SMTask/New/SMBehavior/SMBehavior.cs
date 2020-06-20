//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.SMTask {
	using System.Threading;
	using UniRx.Async;
	using MultiEvent;
	using Extension;


	// TODO : コメント追加、整頓


	public abstract class SMBehavior : ISMBehavior {
		public virtual SMTaskType _type => SMTaskType.Work;
		public virtual SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.InScene;

		public SMHierarchy _hierarchy	{ get; set; }
		public SMBehaviorBody _body		{ get; protected set; }
		public ISMBehavior _previous	{ get; set; }
		public ISMBehavior _next		{ get; set; }

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

		public MultiSubject _activeAsyncCancelEvent		=> _body._activeAsyncCancelEvent;
		public CancellationToken _activeAsyncCancel		=> _body._activeAsyncCancel;
		public CancellationToken _inActiveAsyncCancel	=> _body._inActiveAsyncCancel;

		public MultiDisposable _disposables	=> _body._disposables;


		protected SMBehavior() {
			_body = new SMBehaviorBody( this, SMTaskActiveState.Enabling );
			_hierarchy = new SMHierarchy( null, new ISMBehavior[] { this }, null );
		}

		~SMBehavior() => Dispose();

		public void Dispose() => _body.Dispose();

		public abstract void Create();


		public void StopActiveAsync() => _body.StopActiveAsync();


		public void DestroyHierarchy()
			=> _hierarchy.Destroy();


		public async UniTask RunStateEvent( SMTaskRanState state )
			=> await _body.RunStateEvent( state );


		public async UniTask ChangeActive( bool isActive )
			=> await _body.ChangeActive( isActive );

		public async UniTask RunActiveEvent()
			=> await _body.RunActiveEvent();


		public override string ToString()
			=> $"{this.GetAboutName()}( {_type}, {_lifeSpan} )";
	}
}