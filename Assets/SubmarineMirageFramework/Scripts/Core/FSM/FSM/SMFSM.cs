//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Base;
	using Event;
	using FSM.State;
	using Extension;
	using Utility;
	using Debug;



	public class SMFSM : SMStandardBase {
		[SMShowLine] public SMFSMBody _body	{ get; protected set; }
		public ISMFSMOwner _owner	=> _body._owner;
		public SMState _state	=> _body._stateBody?._state;

		public Type _fsmType	=> _body._baseStateType;

		public bool _isInitialEntered	=> _body._isInitialEntered;
		public bool _isInitialized		=> _body._isInitialized;
		public bool _isOperable			=> _body._isOperable;
		public bool _isFinalizing		=> _body._isFinalizing;
		public bool _isActive			=> _body._isActive;

		public SMAsyncEvent _selfInitializeEvent	=> _body._selfInitializeEvent;
		public SMAsyncEvent _initializeEvent		=> _body._initializeEvent;
		public SMSubject _enableEvent				=> _body._enableEvent;
		public SMSubject _fixedUpdateEvent			=> _body._fixedUpdateEvent;
		public SMSubject _updateEvent				=> _body._updateEvent;
		public SMSubject _lateUpdateEvent			=> _body._lateUpdateEvent;
		public SMSubject _disableEvent				=> _body._disableEvent;
		public SMAsyncEvent _finalizeEvent			=> _body._finalizeEvent;

		public SMAsyncCanceler _asyncCancelerOnDisableAndExit	=> _body._asyncCancelerOnDisableAndExit;
		public SMAsyncCanceler _asyncCancelerOnDispose			=> _body._asyncCancelerOnDispose;



		public SMFSM() {
			_body = new SMFSMBody( this );
			_disposables.AddLast( () => {
				_body.Dispose();
			} );
		}

		public override void Dispose() => base.Dispose();


		public void Setup( ISMFSMOwner owner, IEnumerable<SMState> states,
							Type baseStateType = null, Type startStateType = null,
							bool isInitialEnter = true, bool isLockBeforeInitialize = false
		) => _body.Setup(
			owner, states, baseStateType, startStateType, isInitialEnter, isLockBeforeInitialize
		);


		public static SMFSM Generate( ISMFSMOwner owner, SMFSMGenerateList generateList ) {
			SMFSM first = null;
			SMFSM last = null;
			generateList.ForEach( data => {
				var current = typeof( SMFSM ).Create<SMFSM>();
				data.CreateStates();
				current.Setup(
					owner, data._states, data._baseStateType, data._startStateType,
					data._isInitialEnter, data._isLockBeforeInitialize
				);

				if ( first == null )	{ first = current; }
				last?._body.Link( current._body );
				last = current;
			} );

			return first;
		}



		public UniTask InitialEnter( bool isRunSelfOnly = false )
			=> _body.InitialEnter( isRunSelfOnly );

		public UniTask FinalExit( bool isRunSelfOnly = false )
			=> _body.FinalExit( isRunSelfOnly );

		public UniTask ChangeState( Type stateType )
			=> _body.ChangeState( stateType );

		public UniTask ChangeState<T>() where T : SMState
			=> ChangeState( typeof( T ) );




		public IEnumerable<SMFSM> GetFSMs()
			=> _body.GetBrothers()
				.Select( fsm => fsm._fsm );

		public SMFSM GetFSM( Type baseStateType )
			=> _body.GetFSM( baseStateType )?._fsm;

		public SMFSM GetFSM<T>() where T : SMState
			=> GetFSM( typeof( T ) );


		public IEnumerable<SMState> GetStates()
			=> _body.GetStates()
				.Select( s => s._state );

		public SMState GetState( Type stateType )
			=> _body.GetState( stateType )?._state;

		public T GetState<T>() where T : SMState
			=> (T)GetState( typeof( T ) );
	}
}