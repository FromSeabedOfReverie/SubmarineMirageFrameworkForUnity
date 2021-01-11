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


	public abstract class SMState<TFSM, TOwner>
		: BaseSMFSMModifylerOwner<SMStateModifyler>, ISMState<TFSM, TOwner>
		where TFSM : ISMFSM
		where TOwner : ISMFSMOwner<TFSM>
	{
		public TFSM _fsm		{ get; private set; }
		public TOwner _owner	{ get; private set; }

		[SMShowLine] public SMFSMRunState _runState	{ get; private set; } = SMFSMRunState.Exit;

		public SMMultiAsyncEvent _selfInitializeEvent	{ get; private set; } = new SMMultiAsyncEvent();
		public SMMultiAsyncEvent _initializeEvent		{ get; private set; } = new SMMultiAsyncEvent();
		public SMMultiSubject _enableEvent				{ get; private set; } = new SMMultiSubject();
		public SMMultiSubject _fixedUpdateEvent			{ get; private set; } = new SMMultiSubject();
		public SMMultiSubject _updateEvent				{ get; private set; } = new SMMultiSubject();
		public SMMultiSubject _lateUpdateEvent			{ get; private set; } = new SMMultiSubject();
		public SMMultiSubject _disableEvent				{ get; private set; } = new SMMultiSubject();
		public SMMultiAsyncEvent _finalizeEvent			{ get; private set; } = new SMMultiAsyncEvent();

		public readonly SMMultiAsyncEvent _enterEvent = new SMMultiAsyncEvent();
		public readonly SMMultiAsyncEvent _updateAsyncEvent = new SMMultiAsyncEvent();
		public readonly SMMultiAsyncEvent _exitEvent = new SMMultiAsyncEvent();

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

		public void Set( TOwner owner ) {
			_owner = owner;
			_fsm = _owner._fsm;

			_asyncCancelerOnChangeOrDisable.Dispose();
			_asyncCancelerOnChangeOrDisable = _owner._asyncCancelerOnDisable.CreateChild();
		}


		public void StopActiveAsync() => _asyncCancelerOnChangeOrDisable.Cancel();
	}
}