//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.State.Base {
	using SubmarineMirage.Base;
	using Event;
	using FSM.Base;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class BaseSMState : SMStandardBase {
		[SMShow] public SMStateBody _body	{ get; private set; }
		public IBaseSMFSMOwner _owner	=> _body._owner;
		public BaseSMFSM _fsm			=> _body._fsmBody._fsm;

		public SMStateRunState _ranState			=> _body._ranState;
		public SMStateUpdateState _updatedState	=> _body._updatedState;
		public bool _isUpdating					=> _body._isUpdating;

		public SMAsyncEvent _selfInitializeEvent	=> _body._selfInitializeEvent;
		public SMAsyncEvent _initializeEvent		=> _body._initializeEvent;
		public SMSubject _enableEvent				=> _body._enableEvent;
		public SMSubject _fixedUpdateEvent		=> _body._fixedUpdateEvent;
		public SMSubject _updateEvent				=> _body._updateEvent;
		public SMSubject _lateUpdateEvent			=> _body._lateUpdateEvent;
		public SMSubject _disableEvent			=> _body._disableEvent;
		public SMAsyncEvent _finalizeEvent		=> _body._finalizeEvent;

		public SMAsyncEvent _enterEvent		=> _body._enterEvent;
		public SMAsyncEvent _updateAsyncEvent	=> _body._updateAsyncEvent;
		public SMAsyncEvent _exitEvent		=> _body._exitEvent;

		public SMAsyncCanceler _asyncCancelerOnDisableAndExit	=> _body._asyncCancelerOnDisableAndExit;
		public SMAsyncCanceler _asyncCancelerOnDispose			=> _body._asyncCancelerOnDispose;



		public BaseSMState() {
			_body = new SMStateBody( this );

			_disposables.AddLast( () => {
				_body.Dispose();
			} );
		}

		public override void Dispose() => base.Dispose();
	}
}