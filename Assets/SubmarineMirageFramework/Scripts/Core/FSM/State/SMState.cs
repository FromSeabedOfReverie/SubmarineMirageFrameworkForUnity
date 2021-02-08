//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.State {
	using FSM.Base;
	using FSM.State.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMState<TOwner, TFSM> : BaseSMState
		where TOwner : IBaseSMFSMOwner
		where TFSM : BaseSMFSM
	{
		public new TOwner _owner	=> (TOwner)base._owner;
		public new TFSM _fsm		=> (TFSM)base._fsm;


		public SMState() {
		}
	}
}