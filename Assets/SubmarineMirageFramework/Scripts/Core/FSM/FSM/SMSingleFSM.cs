//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.FSM {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Base;
	using MultiEvent;
	using Task;
	using State;
	using Modifyler;
	using Utility;
	using Debug;



// TODO : コメント追加、整頓



	public abstract class SMSingleFSM<TOwner, TFSM, TState> : BaseSMFSM
		where TOwner : ISMFSMOwner<TFSM>
		where TFSM : BaseSMFSM
		where TState : BaseSMState
	{
		public override SMMultiAsyncEvent _selfInitializeEvent	=> _owner._selfInitializeEvent;
		public override SMMultiAsyncEvent _initializeEvent		=> _owner._initializeEvent;
		public override SMMultiSubject _enableEvent				=> _owner._enableEvent;
		public override SMMultiSubject _fixedUpdateEvent		=> _owner._fixedUpdateEvent;
		public override SMMultiSubject _updateEvent				=> _owner._updateEvent;
		public override SMMultiSubject _lateUpdateEvent			=> _owner._lateUpdateEvent;
		public override SMMultiSubject _disableEvent			=> _owner._disableEvent;
		public override SMMultiAsyncEvent _finalizeEvent		=> _owner._finalizeEvent;

		public TOwner _owner	{ get; private set; }
		public TState _state	{ get; set; }
		public readonly Dictionary<Type, TState> _states = new Dictionary<Type, TState>();

		public override BaseSMState _rawState {
			get { return _state; }
			set { _state = (TState)value; }
		}


		public SMSingleFSM( TOwner owner, IEnumerable<TState> states ) {
			_owner = owner;
			states.ForEach( s => {
				s.Set( this );
				_states[s.GetType()] = s;
			} );
		}


		public async UniTask ChangeState( Type stateType ) {
			if ( !( stateType is TState ) )	{ throw new Exception( "違うステートを入れた" ); }
			_modifyler.Register( new ChangeStateSMFSM( _states.GetOrDefault( stateType ) ) );
			await _modifyler.WaitRunning();
		}

		public async UniTask ChangeState<T>() where T : TState
			=> await ChangeState( typeof( T ) );
	}
}