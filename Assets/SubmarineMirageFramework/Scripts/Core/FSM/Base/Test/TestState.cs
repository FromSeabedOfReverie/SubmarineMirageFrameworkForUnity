//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSMTest {
	using MultiEvent;
	using Task;



	// TODO : コメント追加、整頓



	public abstract class BaseSMState : BaseSMFSMModifylerOwner<BaseSMState, SMStateModifyler, SMStateModifyData>
	{
		public FSM.SMFSMRunState _runState	{ get; set; }
		public SMMultiAsyncEvent _enterEvent		{ get; private set; }
		public SMMultiAsyncEvent _updateAsyncEvent	{ get; private set; }
		public SMMultiAsyncEvent _exitEvent		{ get; private set; }
		public SMTaskCanceler _asyncCancelerOnChangeOrDisable	{ get; private set; }
		public abstract void Set( BaseSMFSM fsm );
	}



	public abstract class SMState<TFSM> : BaseSMState
		where TFSM : BaseSMFSM
	{
		public TFSM _fsm	{ get; private set; }
		public override void Set( BaseSMFSM fsm ) {
			_fsm = (TFSM)fsm;
		}
	}
}