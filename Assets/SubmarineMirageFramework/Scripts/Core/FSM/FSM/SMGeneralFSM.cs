//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using System;


// TODO : コメント追加、整頓


	public class SMGeneralFSM<TOwner, TState> :
		SMInternalFSM<SMGeneralFSM<TOwner, TState>, TOwner, TState>
		where TOwner : ISMParallelFSMOwner< SMGeneralFSM<TOwner, TState> >
		where TState : class, ISMState<SMGeneralFSM<TOwner, TState>, TOwner>
	{
		public SMGeneralFSM( TOwner owner, TState[] states, Type startState = null )
			: base( owner, states, startState )
		{
		}
	}
}