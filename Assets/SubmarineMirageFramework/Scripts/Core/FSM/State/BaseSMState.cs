//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.State.Base {
	using MultiEvent;
	using Task;
	using FSM.Base;
	using FSM.Modifyler.Base;
	using FSM.State.Modifyler.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class BaseSMState : BaseSMFSMModifylerOwner<BaseSMState, SMStateModifyler, SMStateModifyData>
	{
		[SMShowLine] public SMStateRunState _ranState	{ get; set; }
		public bool _isUpdating	{ get; set; }

		public readonly SMMultiAsyncEvent _selfInitializeEvent = new SMMultiAsyncEvent();
		public readonly SMMultiAsyncEvent _initializeEvent = new SMMultiAsyncEvent();
		public readonly SMMultiSubject _enableEvent = new SMMultiSubject();
		public readonly SMMultiSubject _fixedUpdateEvent = new SMMultiSubject();
		public readonly SMMultiSubject _updateEvent = new SMMultiSubject();
		public readonly SMMultiSubject _lateUpdateEvent = new SMMultiSubject();
		public readonly SMMultiSubject _disableEvent = new SMMultiSubject();
		public readonly SMMultiAsyncEvent _finalizeEvent = new SMMultiAsyncEvent();

		public readonly SMMultiAsyncEvent _enterEvent = new SMMultiAsyncEvent();
		public readonly SMMultiAsyncEvent _updateAsyncEvent = new SMMultiAsyncEvent();
		public readonly SMMultiAsyncEvent _exitEvent = new SMMultiAsyncEvent();

		public readonly SMTaskCanceler _asyncCancelerOnDisableAndExit = new SMTaskCanceler();
		public readonly SMTaskCanceler _asyncCancelerOnDispose = new SMTaskCanceler();



		public BaseSMState() {
			_modifyler = new SMStateModifyler( this );

			_disposables.AddLast( () => {
				_ranState = SMStateRunState.Exit;
				_isUpdating = false;

				_asyncCancelerOnDisableAndExit.Dispose();
				_asyncCancelerOnDispose.Dispose();

				_selfInitializeEvent.Dispose();
				_initializeEvent.Dispose();
				_enableEvent.Dispose();
				_fixedUpdateEvent.Dispose();
				_updateEvent.Dispose();
				_lateUpdateEvent.Dispose();
				_disableEvent.Dispose();
				_finalizeEvent.Dispose();

				_enterEvent.Dispose();
				_updateAsyncEvent.Dispose();
				_exitEvent.Dispose();
			} );
		}

		public override void Dispose() => base.Dispose();

		public abstract void Set( SMFSM fsm );
	}
}