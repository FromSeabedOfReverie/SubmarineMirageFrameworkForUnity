//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestTask
namespace SubmarineMirage.Task {
	using Cysharp.Threading.Tasks;
	using Event;
	using Service;
	using Utility;
	using Debug;



	public abstract class SMTask : SMLinkNode {
		[SMShowLine] public new SMTask _previous {
			get => base._previous as SMTask;
			set => base._previous = value;
		}
		[SMShowLine] public new SMTask _next {
			get => base._next as SMTask;
			set => base._next = value;
		}
		public SMTaskManager _taskManager	{ protected get; set; }

		[SMShowLine] public virtual SMTaskRunType _type	=> SMTaskRunType.Parallel;
		[SMShowLine] public SMTaskRunState _ranState	{ get; set; }
		public SMTaskActiveState _activeState			{ get; set; }

		[SMShow] public bool _isInitialized				{ get; set; }
		[SMShow] public bool _isFinalize				=> _ranState >= SMTaskRunState.FinalDisable;
		[SMShow] public bool _isOperable				=> _isInitialized && !_isFinalize;
		[SMShowLine] public bool _isActive				=> _activeState == SMTaskActiveState.Enable;
		[SMShow] public bool _isRequestInitialEnable	{ get; set; } = true;
// TODO : SMBehaviour実装後に、_isCanActive判定を全面的に見直す
		[SMShow] public virtual bool _isCanActive		=> true;

		public SMAsyncEvent _selfInitializeEvent	{ get; private set; } = new SMAsyncEvent();
		public SMAsyncEvent _initializeEvent		{ get; private set; } = new SMAsyncEvent();
		public SMAsyncEvent _finalizeEvent			{ get; private set; } = new SMAsyncEvent();
		public SMSubject _enableEvent				{ get; private set; } = new SMSubject();
		public SMSubject _disableEvent				{ get; private set; } = new SMSubject();
		public SMSubject _fixedUpdateEvent			{ get; private set; } = new SMSubject();
		public SMSubject _updateEvent				{ get; private set; } = new SMSubject();
		public SMSubject _lateUpdateEvent			{ get; private set; } = new SMSubject();

		public SMAsyncCanceler _asyncCancelerOnDisable	{ get; private set; } = new SMAsyncCanceler();
		public SMAsyncCanceler _asyncCancelerOnDispose	{ get; private set; } = new SMAsyncCanceler();



#region ToString
		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _id ), i => StringSMUtility.IndentSpace( i ) +
				$"{_id} ↑{_previous?._id} ↓{_next?._id}" );
			_toStringer.Add( nameof( _taskManager ),			i => _taskManager?.ToLineString() );
			_toStringer.Add( nameof( _selfInitializeEvent ),	i => _selfInitializeEvent._isRunning.ToString() );
			_toStringer.Add( nameof( _initializeEvent ),		i => _initializeEvent._isRunning.ToString() );
			_toStringer.Add( nameof( _finalizeEvent ),			i => _finalizeEvent._isRunning.ToString() );

			_toStringer.SetLineValue( nameof( _isActive ), () => _isActive ? "◯" : "×" );
		}
#endregion



		public SMTask( bool isRegister = true, bool isAdjustRun = true ) {
			_asyncCancelerOnDisable.Cancel( false );
			_taskManager = SMServiceLocator.Resolve<SMTaskManager>();

			_disposables.AddFirst( () => {
#if TestTask
				SMLog.Debug( $"{nameof( SMTask )}.{nameof( Dispose )} : start\n{this}" );
#endif
				_ranState = SMTaskRunState.Dispose;
				_isRequestInitialEnable = false;

				_asyncCancelerOnDisable.Dispose();
				_asyncCancelerOnDispose.Dispose();

				_selfInitializeEvent.Dispose();
				_initializeEvent.Dispose();
				_finalizeEvent.Dispose();
				_enableEvent.Dispose();
				_disableEvent.Dispose();
				_fixedUpdateEvent.Dispose();
				_updateEvent.Dispose();
				_lateUpdateEvent.Dispose();

				_taskManager.Unregister( this ).Forget();
#if TestTask
				SMLog.Debug( $"{nameof( SMTask )}.{nameof( Dispose )} : end\n{this}" );
#endif
			} );

			if ( isRegister && _taskManager != null ) {
				_taskManager.Register( this, isAdjustRun ).Forget();
			}
		}

		public abstract void Create();

		public override void Dispose() => base.Dispose();



		public void Link( SMTask add ) {
			CheckDisposeError( $"{nameof( Link )}( {add._id} )" );

			base.Link( add );
		}

		public new void Unlink()
			=> base.Unlink();



		public async UniTask Destroy() {
			CheckDisposeError( nameof( Destroy ) );
			await _taskManager.DestroyTask( this );
		}

		public virtual async UniTask ChangeActive( bool isActive ) {
			CheckDisposeError( $"{nameof( ChangeActive )}( {isActive} )" );
			await _taskManager.ChangeActiveTask( this, isActive );
		}
	}
}