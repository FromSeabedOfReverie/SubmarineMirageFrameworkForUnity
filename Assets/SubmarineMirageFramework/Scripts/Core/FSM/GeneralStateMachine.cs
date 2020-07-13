//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using System;


// TODO : コメント追加、整頓


	public class GeneralStateMachine<TOwner, TState> :
		FiniteStateMachine<GeneralStateMachine<TOwner, TState>, TOwner, TState>
		where TOwner : IFiniteStateMachineOwner< GeneralStateMachine<TOwner, TState> >
		where TState : class, IState<GeneralStateMachine<TOwner, TState>, TOwner>
	{
		public GeneralStateMachine( TOwner owner, TState[] states, Type startState = null )
			: base( owner, states, startState )
		{
		}
	}
}