//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.State {
	using Cysharp.Threading.Tasks;
	using Base;
	using Event;
	using Extension;
	using Utility;
	using Debug;



	public class SMStateBody : SMStandardBase {
		public SMState _state		{ get; private set; }
		public ISMFSMOwner _owner	{ get; private set; }
		public SMFSMBody _fsmBody		{ get; private set; }

		[SMShowLine] public SMStateRunState _ranState		{ get; set; }
		[SMShowLine] public SMStateActiveState _activeState	{ get; set; }
		public bool _isActive => _activeState == SMStateActiveState.Enable;
		[SMShowLine] public bool _isUpdating				{ get; set; }

		public readonly SMAsyncEvent _selfInitializeEvent = new SMAsyncEvent();
		public readonly SMAsyncEvent _initializeEvent = new SMAsyncEvent();
		public readonly SMSubject _enableEvent = new SMSubject();
		public readonly SMSubject _fixedUpdateEvent = new SMSubject();
		public readonly SMSubject _updateEvent = new SMSubject();
		public readonly SMSubject _lateUpdateEvent = new SMSubject();
		public readonly SMSubject _disableEvent = new SMSubject();
		public readonly SMAsyncEvent _finalizeEvent = new SMAsyncEvent();

		public readonly SMAsyncEvent _enterEvent = new SMAsyncEvent();
		public readonly SMAsyncEvent _updateAsyncEvent = new SMAsyncEvent();
		public readonly SMAsyncEvent _exitEvent = new SMAsyncEvent();

		public SMAsyncCanceler _asyncCancelerOnDisableAndExit	=> _fsmBody._asyncCancelerOnDisableAndExit;
		public SMAsyncCanceler _asyncCancelerOnDispose			=> _fsmBody._asyncCancelerOnDispose;



		public SMStateBody( SMState state ) {
			_state = state;

			_disposables.AddLast( () => {
				_ranState = SMStateRunState.Exit;
				_activeState = SMStateActiveState.Disable;
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


		public void Setup( ISMFSMOwner owner, SMFSM fsm ) {
			_owner = owner;
			_fsmBody = fsm._body;
		}



		public UniTask SelfInitialize()
			=> _selfInitializeEvent.Run( _asyncCancelerOnDispose );

		public UniTask Initialize()
			=> _initializeEvent.Run( _asyncCancelerOnDispose );

		public UniTask Finalize()
			=> _finalizeEvent.Run( _asyncCancelerOnDispose );



		public void Enable() {
			if ( _isActive )	{ return; }

			_enableEvent.Run();
			_activeState = SMStateActiveState.Enable;
		}

		public void Disable() {
			if ( !_isActive )	{ return; }

			_disableEvent.Run();
			_activeState = SMStateActiveState.Disable;
		}



		public void FixedUpdate() {
			if ( !_isActive )	{ return; }
			if ( _ranState < SMStateRunState.UpdateEnter )	{ return; }

			_fixedUpdateEvent.Run();

			if ( _ranState == SMStateRunState.UpdateEnter ) {
				_ranState = SMStateRunState.FixedUpdate;
//				SMLog.Debug( $"{_state.GetAboutName()}.{nameof( FixedUpdate )} : First" );
			}
		}

		public void Update() {
			if ( !_isActive )	{ return; }
			if ( _ranState < SMStateRunState.FixedUpdate )	{ return; }

			_updateEvent.Run();

			if ( _ranState == SMStateRunState.FixedUpdate ) {
				_ranState = SMStateRunState.Update;
//				SMLog.Debug( $"{_state.GetAboutName()}.{nameof( Update )} : First" );
			}
		}

		public void LateUpdate() {
			if ( !_isActive )	{ return; }
			if ( _ranState < SMStateRunState.Update )	{ return; }

			_lateUpdateEvent.Run();

			if ( _ranState == SMStateRunState.Update ) {
				_ranState = SMStateRunState.LateUpdate;
//				SMLog.Debug( $"{_state.GetAboutName()}.{nameof( LateUpdate )} : First" );
			}
		}



		public async UniTask Enter() {
			if ( _ranState != SMStateRunState.Exit )	{ return; }

			await _enterEvent.Run( _asyncCancelerOnDisableAndExit );
			_ranState = SMStateRunState.Enter;
		}

		public async UniTask Exit() {
			if ( _ranState == SMStateRunState.Exit )	{ return; }

			await _exitEvent.Run( _asyncCancelerOnDisableAndExit );
			_ranState = SMStateRunState.Exit;
		}

		public void UpdateAsync() {
			if ( _ranState != SMStateRunState.Enter )	{ return; }
			if ( _isUpdating )	{ return; }

			UTask.Void( async () => {
				try {
					_ranState = SMStateRunState.UpdateEnter;
					_isUpdating = true;
					await _updateAsyncEvent.Run( _asyncCancelerOnDisableAndExit );
				} finally {
					_isUpdating = false;
				}
			} );
		}
	}
}