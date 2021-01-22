//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.State.Base {
	using SubmarineMirage.Base;
	using MultiEvent;
	using Task;
	using FSM.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class BaseSMState : SMStandardBase {
		[SMShowLine] public SMStateRunState _ranState	{ get; set; }
		[SMShowLine] public SMStateUpdateState _updatedState	{ get; set; }
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

		[SMHide] public abstract SMTaskCanceler _asyncCancelerOnDisableAndExit	{ get; }
		[SMHide] public abstract SMTaskCanceler _asyncCancelerOnDispose	{ get; }



		public BaseSMState() {
			_disposables.AddLast( () => {
				_ranState = SMStateRunState.Exit;
				_updatedState = SMStateUpdateState.Disable;
				_isUpdating = false;

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

		public abstract void Set( IBaseSMFSMOwner topOwner, SMFSM fsm );
	}
}