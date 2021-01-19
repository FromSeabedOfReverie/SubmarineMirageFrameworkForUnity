//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.FSM {
	using System;
	using System.Collections.Generic;
	using KoganeUnityLib;
	using State;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMInternalFSM<TOwner, TState> : SMFSM<TOwner, TState>
		where TOwner : BaseSMFSM, IBaseSMFSMOwner
		where TState : BaseSMState
	{
		public SMInternalFSM( IEnumerable<TState> states, Type baseStateType, Type startStateType = null )
			: base( states, startStateType )
		{
			_states.ForEach( pair => {
				var type = pair.Value.GetType();
				if ( type.IsInheritance( baseStateType ) ) {
					throw new InvalidOperationException( $"基盤状態が違う、状態を指定 : {type}, {baseStateType}" );
				}
			} );
		}
	}
}