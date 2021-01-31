//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using System;
	using System.Collections.Generic;
	using KoganeUnityLib;
	using FSM.Base;
	using FSM.State.Base;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMInternalFSM<TOwner, TOwnerFSM, TState, TEnum> : BaseSMSingleFSM<TOwnerFSM, TState>
		where TOwner : IBaseSMFSMOwner
		where TOwnerFSM : SMFSM, IBaseSMFSMOwner
		where TState : BaseSMState
		where TEnum : Enum
	{
		[SMHide] public TOwner _topOwner	{ get; private set; }
		[SMHide] public TOwnerFSM _fsm => _owner;
		[SMShowLine] public TEnum _fsmType	{ get; private set; }


		public SMInternalFSM( IEnumerable<TState> states, Type baseStateType, Type startStateType = null )
			: base( states, startStateType )
		{
			_states.ForEach( pair => {
				var type = pair.Value.GetType();
				if ( !type.IsInheritance( baseStateType ) ) {
					throw new InvalidOperationException( $"基盤状態が違う、状態を指定 : {type}, {baseStateType}" );
				}
			} );

			_body._setEvent.AddFirst( ( topFSMOwner, fsmOwner ) => {
				_topOwner = (TOwner)topFSMOwner;
			} );
		}

		public override void SetFSMType( Enum fsmType )
			=> _fsmType = (TEnum)fsmType;
	}
}