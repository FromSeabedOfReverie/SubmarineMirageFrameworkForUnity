//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Base {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using MultiEvent;
	using Task;
	using FSM.Modifyler;
	using FSM.State.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class BaseSMSingleFSM<TOwner, TState> : SMFSM
		where TOwner : IBaseSMFSMOwner
		where TState : BaseSMState
	{
		[SMHide] public override bool _isInitialized	=> _owner._isInitialized;
		[SMHide] public override bool _isOperable		=> _owner._isOperable;
		[SMHide] public override bool _isFinalizing		=> _owner._isFinalizing;
		[SMHide] public override bool _isActive			=> _owner._isActive;

		[SMHide] public override SMMultiAsyncEvent _selfInitializeEvent	=> _owner._selfInitializeEvent;
		[SMHide] public override SMMultiAsyncEvent _initializeEvent		=> _owner._initializeEvent;
		[SMHide] public override SMMultiSubject _enableEvent			=> _owner._enableEvent;
		[SMHide] public override SMMultiSubject _fixedUpdateEvent		=> _owner._fixedUpdateEvent;
		[SMHide] public override SMMultiSubject _updateEvent			=> _owner._updateEvent;
		[SMHide] public override SMMultiSubject _lateUpdateEvent		=> _owner._lateUpdateEvent;
		[SMHide] public override SMMultiSubject _disableEvent			=> _owner._disableEvent;
		[SMHide] public override SMMultiAsyncEvent _finalizeEvent		=> _owner._finalizeEvent;

		[SMHide] public override SMTaskCanceler _asyncCancelerOnDispose	=> _owner._asyncCancelerOnDispose;

		[SMHide] protected TOwner _owner	{ get; private set; }
		[SMShowLine] public TState _state	{ get; set; }
		public readonly Dictionary<Type, TState> _states = new Dictionary<Type, TState>();
		public Type _startStateType	{ get; protected set; }


		public BaseSMSingleFSM( IEnumerable<TState> states, Type startStateType = null ) {
			_states = states.ToDictionary( state => state.GetType() );
			_startStateType = startStateType ?? _states.First().Value.GetType();


			_body._setEvent.AddFirst( ( topFSMOwner, fsmOwner ) => {
				_owner = (TOwner)fsmOwner;
			} );
			_body._setEvent.AddLast( ( topFSMOwner, fsmOwner ) => {
				_states.ForEach( pair => pair.Value._body.Set( topFSMOwner, this ) );

				_selfInitializeEvent.AddLast( _body._registerEventName, async canceler => {
					await _states.Select( pair => pair.Value._body.SelfInitialize() );
				} );
				_initializeEvent.AddLast( _body._registerEventName, async canceler => {
					await _states.Select( pair => pair.Value._body.Initialize() );
				} );
				_finalizeEvent.AddFirst( _body._registerEventName, async canceler => {
					_state = null;
					await _states.Select( pair => pair.Value._body.Finalize() );
				} );

				_enableEvent.AddLast( _body._registerEventName ).Subscribe( _ =>	_state?._body.Enable() );
				_disableEvent.AddLast( _body._registerEventName ).Subscribe( _ =>	_state?._body.Disable() );

				_fixedUpdateEvent.AddLast( _body._registerEventName ).Subscribe( _ =>	_state?._body.FixedUpdate() );
				_updateEvent.AddLast( _body._registerEventName ).Subscribe( _ =>		_state?._body.Update() );
				_lateUpdateEvent.AddLast( _body._registerEventName ).Subscribe( _ =>	_state?._body.LateUpdate() );

				_body._modifyler.Register( new InitialEnterSMSingleFSM<TOwner, TState>( _startStateType ) );
			} );


			_disposables.AddLast( () => {
				_states.ForEach( pair => pair.Value.Dispose() );
				_states.Clear();
				_state = null;
			} );
		}



		public TState GetState( Type stateType )
			=> _states.GetOrDefault( stateType );



		public UniTask ChangeState( Type stateType )
			=> _body._modifyler.RegisterAndRun( new ChangeStateSMSingleFSM<TOwner, TState>( stateType ) );

		public UniTask ChangeState<T>() where T : TState
			=> ChangeState( typeof( T ) );


		public override UniTask FinalExit()
			=> _body._modifyler.RegisterAndRun( new FinalExitSMSingleFSM<TOwner, TState>() );



		public override void SetToString() {
			base.SetToString();
			_toStringer.SetValue( nameof( _states ), i => _toStringer.DefaultValue( _states, i, true ) );
		}
	}
}