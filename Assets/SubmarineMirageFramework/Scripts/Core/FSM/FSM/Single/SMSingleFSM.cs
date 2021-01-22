//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using System;
	using System.Collections.Generic;
	using FSM.Base;
	using FSM.State.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMSingleFSM<TOwner, TState> : BaseSMSingleFSM<TOwner, TState>
		where TOwner : IBaseSMFSMOwner
		where TState : BaseSMState
	{
		public new TOwner _owner => base._owner;


		public SMSingleFSM( TOwner owner, IEnumerable<TState> states, Type startStateType = null )
			: base( states, startStateType )
		{
			Set( owner, owner );
		}
	}
}