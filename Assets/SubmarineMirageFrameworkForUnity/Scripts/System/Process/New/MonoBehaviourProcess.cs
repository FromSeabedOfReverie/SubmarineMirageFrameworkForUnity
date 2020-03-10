//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Process.New {
	using System.Threading;
	using MultiEvent;
	using Extension;


	// TODO : コメント追加、整頓


	public abstract class MonoBehaviourProcess : MonoBehaviourExtension, IProcess {
		public CoreProcessManager.ExecutedState _executedState	{ get; set; }
		public virtual CoreProcessManager.ProcessType _type => CoreProcessManager.ProcessType.Work;
		public virtual CoreProcessManager.ProcessLifeSpan _lifeSpan
			=> CoreProcessManager.ProcessLifeSpan.InScene;

		public bool _isInitialized	{ get; set; }
		public bool _isActive		{ get; set; }
		
		CancellationTokenSource _activeAsyncCanceler = new CancellationTokenSource();
		public CancellationToken _activeAsyncCancel => _activeAsyncCanceler.Token;
		CancellationTokenSource _finalizeAsyncCanceler = new CancellationTokenSource();
		public CancellationToken _finalizeAsyncCancel => _finalizeAsyncCanceler.Token;

		public MultiAsyncEvent _loadEvent		{ get; protected set; }
		public MultiAsyncEvent _initializeEvent	{ get; protected set; }
		public MultiAsyncEvent _enableEvent		{ get; protected set; }
		public MultiSubject _fixedUpdateEvent	{ get; protected set; }
		public MultiSubject _updateEvent		{ get; protected set; }
		public MultiSubject _lateUpdateEvent	{ get; protected set; }
		public MultiAsyncEvent _disableEvent	{ get; protected set; }
		public MultiAsyncEvent _finalizeEvent	{ get; protected set; }


		protected void Awake() {

// TODO : 非活動中は、どうする？
//			disable中は、登録後、呼戻さない？
//			_isActive判定、enable、disableイベント呼び出し等、含む
			if ( !isActiveAndEnabled )	{ return; }

			_loadEvent = new MultiAsyncEvent();
			_initializeEvent = new MultiAsyncEvent();
			_enableEvent = new MultiAsyncEvent();
			_fixedUpdateEvent = new MultiSubject();
			_updateEvent = new MultiSubject();
			_lateUpdateEvent = new MultiSubject();
			_disableEvent = new MultiAsyncEvent();
			_finalizeEvent = new MultiAsyncEvent();

			CoreProcessManager.s_instance.Register( this );
		}


		public abstract void Create();


		public void StopActiveAsync() {
			_activeAsyncCanceler.Cancel();
			_activeAsyncCanceler.Dispose();
			_activeAsyncCanceler = new CancellationTokenSource();
		}


		public virtual void Dispose() {
			_activeAsyncCanceler.Cancel();
			_finalizeAsyncCanceler.Cancel();
			_activeAsyncCanceler.Dispose();
			_finalizeAsyncCanceler.Dispose();

			_loadEvent.Dispose();
			_initializeEvent.Dispose();
			_enableEvent.Dispose();
			_fixedUpdateEvent.Dispose();
			_updateEvent.Dispose();
			_lateUpdateEvent.Dispose();
			_disableEvent.Dispose();
			_finalizeEvent.Dispose();
		}


		public override string ToString() {
			return this.ToDeepString();
		}


		protected void OnDestroy() {
			Dispose();
		}


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