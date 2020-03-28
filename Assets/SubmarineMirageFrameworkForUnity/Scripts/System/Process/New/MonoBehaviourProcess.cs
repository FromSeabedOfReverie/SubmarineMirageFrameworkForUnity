//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process.New {
	using System.Threading;
	using UniRx.Async;
	using MultiEvent;
	using Extension;


	// TODO : コメント追加、整頓


	public abstract class MonoBehaviourProcess : MonoBehaviourExtension, IProcess {
		public virtual ProcessBody.Type _type => ProcessBody.Type.Work;
		public virtual ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.InScene;

		public ProcessBody _process	{ get; private set; }

		public string _belongSceneName => _process._belongSceneName;

		public bool _isInitialized => _process._isInitialized;
		public bool _isActive => _process._isActive;
		
		public MultiAsyncEvent _loadEvent => _process._loadEvent;
		public MultiAsyncEvent _initializeEvent => _process._initializeEvent;
		public MultiAsyncEvent _enableEvent => _process._enableEvent;
		public MultiSubject _fixedUpdateEvent => _process._fixedUpdateEvent;
		public MultiSubject _updateEvent => _process._updateEvent;
		public MultiSubject _lateUpdateEvent => _process._lateUpdateEvent;
		public MultiAsyncEvent _disableEvent => _process._disableEvent;
		public MultiAsyncEvent _finalizeEvent => _process._finalizeEvent;

		public CancellationToken _activeAsyncCancel => _process._activeAsyncCancel;
		public CancellationToken _inActiveAsyncCancel => _process._inActiveAsyncCancel;

		public MultiDisposable _disposables => _process._disposables;


		protected void Awake() => _process = new ProcessBody( this );

		protected void OnDestroy() {
			Debug.Log.Debug("OnDestroy");
			Dispose();
		}

		public void Dispose() => _process.Dispose();

		public abstract void Create();


/*
TODO : OnEnable、OnDisableは廃止し、必ずProcess経由で、ChangeActiveさせる
		protected void OnEnable() {
			Debug.Log.Debug( "OnEnable" );
			_process.ChangeActive( true, false ).Forget();
		}

		protected void OnDisable() {
			Debug.Log.Debug( "OnDisable" );
			_process.ChangeActive( false, false ).Forget();
		}
*/


		public void StopActiveAsync() => _process.StopActiveAsync();


		public async UniTask RunStateEvent( ProcessBody.RanState state, bool isRunStateEventOfChildren = false )
			=> await _process.RunStateEvent( state, isRunStateEventOfChildren );


		public async UniTask ChangeActive( bool isActive )
			=> await _process.ChangeActive( isActive, true );


		public override string ToString() => this.ToDeepString();


#if DEVELOP
		protected void Start() {}
		protected void OnEnable() {}
		protected void FixedUpdate() {}
		protected void Update() {}
		protected void LateUpdate() {}
		protected void OnDisable() {}
#endif
	}
}