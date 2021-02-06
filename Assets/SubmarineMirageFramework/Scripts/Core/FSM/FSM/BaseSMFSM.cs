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
	using MultiEvent;
	using FSM.State.Base;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class BaseSMFSM : SMStandardBase {
		public SMFSMBody _body	{ get; protected set; }
		[SMHide] public IBaseSMFSMOwner _owner	=> _body._owner;
		[SMHide] public BaseSMState _state		=> _body._stateBody?._state;

		[SMHide] public Type _fsmType	=> _body._baseStateType;

		[SMHide] public bool _isInitialEntered	=> _body._isInitialEntered;
		[SMHide] public bool _isInitialized		=> _body._isInitialized;
		[SMHide] public bool _isOperable		=> _body._isOperable;
		[SMHide] public bool _isFinalizing		=> _body._isFinalizing;
		[SMHide] public bool _isActive			=> _body._isActive;

		[SMHide] public SMMultiAsyncEvent _selfInitializeEvent	=> _body._selfInitializeEvent;
		[SMHide] public SMMultiAsyncEvent _initializeEvent		=> _body._initializeEvent;
		[SMHide] public SMMultiSubject _enableEvent				=> _body._enableEvent;
		[SMHide] public SMMultiSubject _fixedUpdateEvent		=> _body._fixedUpdateEvent;
		[SMHide] public SMMultiSubject _updateEvent				=> _body._updateEvent;
		[SMHide] public SMMultiSubject _lateUpdateEvent			=> _body._lateUpdateEvent;
		[SMHide] public SMMultiSubject _disableEvent			=> _body._disableEvent;
		[SMHide] public SMMultiAsyncEvent _finalizeEvent		=> _body._finalizeEvent;

		[SMHide] public SMAsyncCanceler _asyncCancelerOnDisableAndExit	=> _body._asyncCancelerOnDisableAndExit;
		[SMHide] public SMAsyncCanceler _asyncCancelerOnDispose			=> _body._asyncCancelerOnDispose;



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