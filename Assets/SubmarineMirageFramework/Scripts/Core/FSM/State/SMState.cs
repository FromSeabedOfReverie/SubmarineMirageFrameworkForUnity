//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using System;
	using Cysharp.Threading.Tasks;
	using Base;
	using MultiEvent;
	using Task;
	using Modifyler;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class SMState<TFSM>
		: BaseSMFSMModifylerOwner<SMStateModifyler>, ISMState<TFSM>
		where TFSM : ISMInternalFSM
	{
		public TFSM _fsm		{ get; private set; }

		[SMShowLine] public SMFSMRunState _runState	{ get; set; } = SMFSMRunState.Exit;

		public SMMultiAsyncEvent _selfInitializeEvent	{ get; private set; } = new SMMultiAsyncEvent();
		public SMMultiAsyncEvent _initializeEvent		{ get; private set; } = new SMMultiAsyncEvent();
		public SMMultiSubject _enableEvent				{ get; private set; } = new SMMultiSubject();
		public SMMultiSubject _fixedUpdateEvent			{ get; private set; } = new SMMultiSubject();
		public SMMultiSubject _updateEvent				{ get; private set; } = new SMMultiSubject();
		public SMMultiSubject _lateUpdateEvent			{ get; private set; } = new SMMultiSubject();
		public SMMultiSubject _disableEvent				{ get; private set; } = new SMMultiSubject();
		public SMMultiAsyncEvent _finalizeEvent			{ get; private set; } = new SMMultiAsyncEvent();

		public SMMultiAsyncEvent _enterEvent		{ get; private set; } = new SMMultiAsyncEvent();
		public SMMultiAsyncEvent _updateAsyncEvent	{ get; private set; } = new SMMultiAsyncEvent();
		public SMMultiAsyncEvent _exitEvent			{ get; private set; } = new SMMultiAsyncEvent();

		public SMTaskCanceler _asyncCancelerOnChangeOrDisable	{ get; private set; } = new SMTaskCanceler();


		public SMState() {
			_disposables.AddLast( () => {
				_asyncCancelerOnChangeOrDisable.Dispose();

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

		public void Set( TFSM fsm ) {
			_fsm = fsm;
		}


		public void StopActiveAsync() => _asyncCancelerOnChangeOrDisable.Cancel();
	}
}