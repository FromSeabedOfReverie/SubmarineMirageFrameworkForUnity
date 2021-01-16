//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.State {
	using MultiEvent;
	using Task;
	using Base.Modifyler;
	using Modifyler;
	using FSM;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class BaseSMState : BaseSMFSMModifylerOwner<BaseSMState, SMStateModifyler, SMStateModifyData>
	{
		[SMShowLine] public SMStateRunState _ranState	{ get; set; }

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


		public BaseSMState() {
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

		public abstract void Set( BaseSMFSM fsm );

		public void StopActiveAsync() => _asyncCancelerOnChangeOrDisable.Cancel();
	}
}