//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.FSM {
	using System;
	using System.Collections.Generic;
	using Task;
	using Task.Behaviour;
	using State;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMSingleFSM<TOwner, TState> : SMFSM<TOwner, TState>
		where TOwner : IBaseSMFSMOwner, ISMBehaviour
		where TState : BaseSMState
	{
		public override SMTaskRunState _taskRanState => _owner._body._ranState;


		public SMSingleFSM( TOwner owner, IEnumerable<TState> states, Type startState = null )
			: base( states, typeof( TState ), startState )
		{
			Set( owner );
		}
	}
}