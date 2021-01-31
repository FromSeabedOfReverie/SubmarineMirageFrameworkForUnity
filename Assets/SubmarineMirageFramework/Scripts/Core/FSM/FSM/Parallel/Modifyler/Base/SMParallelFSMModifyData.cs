//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler.Base {
	using System;
	using FSM.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMParallelFSMModifyData<TOwner, TInternalFSM, TEnum> : SMFSMModifyData
		where TOwner : IBaseSMFSMOwner
		where TInternalFSM : SMFSM
		where TEnum : Enum
	{
		[SMHide] protected SMParallelFSM<TOwner, TInternalFSM, TEnum> _owner	{ get; private set; }


		public override void Set( SMFSMBody owner ) {
			base.Set( owner );
			_owner = (SMParallelFSM<TOwner, TInternalFSM, TEnum>)owner._fsm;
		}
	}
}