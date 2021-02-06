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
	using FSM.Base;
	using FSM.State.Base;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMFSM<TOwner, TFSM, TState> : BaseSMFSM
		where TOwner : IBaseSMFSMOwner
		where TFSM : BaseSMFSM
		where TState : BaseSMState
	{
		[SMHide] public new TOwner _owner		=> (TOwner)base._owner;
		[SMShowLine] public new TState _state	=> (TState)base._state;



		public override void Setup( IBaseSMFSMOwner owner, IEnumerable<BaseSMState> states,
									Type baseStateType = null, Type startStateType = null,
									bool isInitialEnter = true
		) {
			baseStateType = baseStateType ?? typeof( TState );
			_body.Setup( owner, states, baseStateType, startStateType, isInitialEnter );
		}


		public static TFSM Generate( IBaseSMFSMOwner owner,
										SMFSMGenerateList<TState> generateList,
										Func<TFSM> generateFSMEvent = null
		) {
			generateFSMEvent = generateFSMEvent ?? ( () => typeof( TFSM ).Create<TFSM>() );

			TFSM first = null;
			TFSM last = null;
			generateList.ForEach( data => {
				var current = generateFSMEvent();
				data.CreateStates();
				current.Setup(
					owner, data._states, data._baseStateType, data._startStateType, data._isInitialEnter );

				if ( first == null )	{ first = current; }
				last?._body.Link( current._body );
				last = current;
			} );

			return first;
		}


		public UniTask ChangeState<T>() where T : TState
			=> ChangeState( typeof( T ) );


		public IEnumerable<TFSM> GetFSMs()
			=> _body.GetBrothers()
				.Select( fsm => (TFSM)fsm._fsm );

		public TFSM GetFSM( Type baseStateType )
			=> (TFSM)_body.GetFSM( baseStateType )?._fsm;

		public TFSM GetFSM<T>() where T : TState
			=> GetFSM( typeof( T ) );


		public IEnumerable<TState> GetStates()
			=> _body.GetStates()
				.Select( s => (TState)s._state );

		public TState GetState( Type stateType )
			=> (TState)_body.GetState( stateType )?._state;

		public T GetState<T>() where T : TState
			=> (T)GetState( typeof( T ) );
	}
}