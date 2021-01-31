//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.Modifyler.Base {
	using FSM.Base;
	using FSM.State.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMSingleFSMModifyData<TOwner, TState> : SMFSMModifyData
		where TOwner : IBaseSMFSMOwner
		where TState : BaseSMState
	{
		[SMHide] protected BaseSMSingleFSM<TOwner, TState> _owner	{ get; private set; }


		public override void Set( SMFSMBody owner ) {
			base.Set( owner );
			_owner = (BaseSMSingleFSM<TOwner, TState>)owner._fsm;
		}
	}
}