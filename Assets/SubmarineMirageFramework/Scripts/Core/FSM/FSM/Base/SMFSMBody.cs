//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Base {
	using UniRx;
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Base;
	using MultiEvent;
	using Task;
	using FSM.Modifyler.Base;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMFSMBody : SMStandardBase {
		[SMHide] public bool _isInitialized	=> _fsm._isInitialized;
		[SMHide] public bool _isOperable	=> _fsm._isOperable;
		[SMHide] public bool _isFinalizing	=> _fsm._isFinalizing;
		[SMHide] public bool _isActive		=> _fsm._isActive;

		[SMHide] public SMMultiAsyncEvent _selfInitializeEvent	=> _fsm._selfInitializeEvent;
		[SMHide] public SMMultiAsyncEvent _initializeEvent		=> _fsm._initializeEvent;
		[SMHide] public SMMultiSubject _enableEvent				=> _fsm._enableEvent;
		[SMHide] public SMMultiSubject _fixedUpdateEvent		=> _fsm._fixedUpdateEvent;
		[SMHide] public SMMultiSubject _updateEvent				=> _fsm._updateEvent;
		[SMHide] public SMMultiSubject _lateUpdateEvent			=> _fsm._lateUpdateEvent;
		[SMHide] public SMMultiSubject _disableEvent			=> _fsm._disableEvent;
		[SMHide] public SMMultiAsyncEvent _finalizeEvent		=> _fsm._finalizeEvent;

		[SMHide] public readonly SMMultiEvent<IBaseSMFSMOwner, IBaseSMFSMOwner> _setEvent
			= new SMMultiEvent<IBaseSMFSMOwner, IBaseSMFSMOwner>();

		[SMHide] public readonly SMTaskCanceler _asyncCancelerOnDisableAndExit = new SMTaskCanceler();
		[SMHide] public SMTaskCanceler _asyncCancelerOnDispose	=> _fsm._asyncCancelerOnDispose;

		[SMHide] public SMFSM _fsm	{ get; private set; }
		public SMFSMModifyler _modifyler	{ get; private set; }
		public string _registerEventName	{ get; private set; }
		public bool _isInitialEntered	{ get; set; }


		public SMFSMBody( SMFSM fsm ) {
			_fsm = fsm;
			_registerEventName = _fsm.GetAboutName();
			_modifyler = new SMFSMModifyler( this );

			_setEvent.AddLast( ( topFSMOwner, fsmOwner ) => {
				_disableEvent.AddLast( _registerEventName ).Subscribe( _ => {
					_modifyler.Reset();
					StopAsyncOnDisableAndExit();
				} );
				_updateEvent.AddLast( _registerEventName ).Subscribe( _ => {
					_modifyler.Run().Forget();
				} );
			} );

			_disposables.AddLast( () => {
				_modifyler.Dispose();
				_asyncCancelerOnDisableAndExit.Dispose();
				_setEvent.Dispose();
			} );
		}

		public void Set( IBaseSMFSMOwner topFSMOwner, IBaseSMFSMOwner fsmOwner )
			=> _setEvent.Run( topFSMOwner, fsmOwner );


		public void StopAsyncOnDisableAndExit() {
			_fsm._asyncCancelerOnDisableAndExit.Cancel();
		}
	}
}