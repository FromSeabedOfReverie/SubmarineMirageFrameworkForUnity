//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Base {
	using System;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using SubmarineMirage.Base;
	using Event;
	using FSM.State.Base;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class BaseSMFSM : SMStandardBase {
		[SMShowLine] public SMFSMBody _body	{ get; protected set; }
		public IBaseSMFSMOwner _owner	=> _body._owner;
		public BaseSMState _state		=> _body._stateBody?._state;

		public Type _fsmType	=> _body._baseStateType;

		public bool _isInitialEntered	=> _body._isInitialEntered;
		public bool _isInitialized		=> _body._isInitialized;
		public bool _isOperable		=> _body._isOperable;
		public bool _isFinalizing		=> _body._isFinalizing;
		public bool _isActive			=> _body._isActive;

		public SMAsyncEvent _selfInitializeEvent	=> _body._selfInitializeEvent;
		public SMAsyncEvent _initializeEvent		=> _body._initializeEvent;
		public SMSubject _enableEvent				=> _body._enableEvent;
		public SMSubject _fixedUpdateEvent		=> _body._fixedUpdateEvent;
		public SMSubject _updateEvent				=> _body._updateEvent;
		public SMSubject _lateUpdateEvent			=> _body._lateUpdateEvent;
		public SMSubject _disableEvent			=> _body._disableEvent;
		public SMAsyncEvent _finalizeEvent		=> _body._finalizeEvent;

		public SMAsyncCanceler _asyncCancelerOnDisableAndExit	=> _body._asyncCancelerOnDisableAndExit;
		public SMAsyncCanceler _asyncCancelerOnDispose			=> _body._asyncCancelerOnDispose;



		public BaseSMFSM() {
			_body = new SMFSMBody( this );
			_disposables.AddLast( () => {
				_body.Dispose();
			} );
		}

		public override void Dispose() => base.Dispose();

		public abstract void Setup( IBaseSMFSMOwner owner, IEnumerable<BaseSMState> states,
									Type baseStateType = null, Type startStateType = null,
									bool isInitialEnter = true
		);


		public UniTask InitialEnter( bool isRunSelfOnly = false )
			=> _body.InitialEnter( isRunSelfOnly );

		public UniTask FinalExit( bool isRunSelfOnly = false )
			=> _body.FinalExit( isRunSelfOnly );

		public UniTask ChangeState( Type stateType )
			=> _body.ChangeState( stateType );
	}
}