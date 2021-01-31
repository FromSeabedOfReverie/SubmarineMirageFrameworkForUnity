//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.State.Base {
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Base;
	using MultiEvent;
	using Task;
	using FSM.Base;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public class BaseSMStateBody : SMStandardBase {
		[SMShowLine] public SMStateRunState _ranState	{ get; set; }
		[SMShowLine] public SMStateUpdateState _updatedState	{ get; set; }
		[SMShowLine] public bool _isUpdating	{ get; set; }

		[SMHide] public readonly SMMultiAsyncEvent _selfInitializeEvent = new SMMultiAsyncEvent();
		[SMHide] public readonly SMMultiAsyncEvent _initializeEvent = new SMMultiAsyncEvent();
		[SMHide] public readonly SMMultiSubject _enableEvent = new SMMultiSubject();
		[SMHide] public readonly SMMultiSubject _fixedUpdateEvent = new SMMultiSubject();
		[SMHide] public readonly SMMultiSubject _updateEvent = new SMMultiSubject();
		[SMHide] public readonly SMMultiSubject _lateUpdateEvent = new SMMultiSubject();
		[SMHide] public readonly SMMultiSubject _disableEvent = new SMMultiSubject();
		[SMHide] public readonly SMMultiAsyncEvent _finalizeEvent = new SMMultiAsyncEvent();

		[SMHide] public readonly SMMultiAsyncEvent _enterEvent = new SMMultiAsyncEvent();
		[SMHide] public readonly SMMultiAsyncEvent _updateAsyncEvent = new SMMultiAsyncEvent();
		[SMHide] public readonly SMMultiAsyncEvent _exitEvent = new SMMultiAsyncEvent();

		[SMHide] public readonly SMMultiEvent<IBaseSMFSMOwner, SMFSM> _setEvent
			= new SMMultiEvent<IBaseSMFSMOwner, SMFSM>();

		[SMHide] public SMTaskCanceler _asyncCancelerOnDisableAndExit	=> _state._asyncCancelerOnDisableAndExit;
		[SMHide] public SMTaskCanceler _asyncCancelerOnDispose			=> _state._asyncCancelerOnDispose;

		[SMHide] public BaseSMState _state	{ get; private set; }



		public BaseSMStateBody( BaseSMState state ) {
			_state = state;

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

				_setEvent.Dispose();
			} );
		}

		public void Set( IBaseSMFSMOwner topOwner, SMFSM fsm )
			=> _setEvent.Run( topOwner, fsm );



		public UniTask SelfInitialize()
			=> _selfInitializeEvent.Run( _asyncCancelerOnDispose );

		public UniTask Initialize()
			=> _initializeEvent.Run( _asyncCancelerOnDispose );

		public UniTask Finalize()
			=> _finalizeEvent.Run( _asyncCancelerOnDispose );



		public void Enable()
			=> _enableEvent.Run();

		public void Disable() {
			_updatedState = SMStateUpdateState.Disable;
			_disableEvent.Run();
		}



		public void FixedUpdate() {
			if ( _ranState != SMStateRunState.Update )			{ return; }
			if ( _updatedState < SMStateUpdateState.Disable )	{ return; }

			_fixedUpdateEvent.Run();

			if ( _updatedState == SMStateUpdateState.Disable ) {
				_updatedState = SMStateUpdateState.FixedUpdate;
			}
		}

		public void Update() {
			if ( _ranState != SMStateRunState.Update )				{ return; }
			if ( _updatedState < SMStateUpdateState.FixedUpdate )	{ return; }

			_updateEvent.Run();

			if ( _updatedState == SMStateUpdateState.FixedUpdate ) {
				_updatedState = SMStateUpdateState.Update;
			}
		}

		public void LateUpdate() {
			if ( _ranState != SMStateRunState.Update )			{ return; }
			if ( _updatedState < SMStateUpdateState.Update )	{ return; }

			_lateUpdateEvent.Run();

			if ( _updatedState == SMStateUpdateState.Update ) {
				_updatedState = SMStateUpdateState.LateUpdate;
			}
		}



		public async UniTask Enter() {
			if ( _ranState != SMStateRunState.Exit )	{ return; }

			await _enterEvent.Run( _asyncCancelerOnDisableAndExit );
			_ranState = SMStateRunState.Enter;
		}

		public async UniTask Exit() {
			if ( _ranState == SMStateRunState.Exit )	{ return; }

			_updatedState = SMStateUpdateState.Disable;
			_ranState = SMStateRunState.Enter;

			await _exitEvent.Run( _asyncCancelerOnDisableAndExit );
			_ranState = SMStateRunState.Exit;
		}

		public void UpdateAsync() {
			if ( _ranState == SMStateRunState.Exit )	{ return; }
			if ( _isUpdating )	{ return; }

			UTask.Void( async () => {
				try {
					_ranState = SMStateRunState.Update;
					_isUpdating = true;
					await _updateAsyncEvent.Run( _asyncCancelerOnDisableAndExit );
				} finally {
					_isUpdating = false;
				}
			} );
		}
	}
}