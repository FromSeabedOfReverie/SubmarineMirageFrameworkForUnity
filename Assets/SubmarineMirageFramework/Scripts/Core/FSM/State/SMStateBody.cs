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
	using Utility;
	using Debug;



	public class SMStateBody : SMStandardBase {
		public SMState _state		{ get; private set; }
		public ISMFSMOwner _owner	{ get; private set; }
		public SMFSMBody _fsmBody		{ get; private set; }

		[SMShowLine] public SMStateRunState _ranState			{ get; set; }
		[SMShowLine] public SMStateUpdateState _updatedState	{ get; set; }
		[SMShowLine] public bool _isUpdating					{ get; set; }

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