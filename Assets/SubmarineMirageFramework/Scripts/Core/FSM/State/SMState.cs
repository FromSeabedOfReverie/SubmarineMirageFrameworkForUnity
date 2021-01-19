//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.State {
	using FSM;



	// TODO : コメント追加、整頓



	public abstract class SMState<TFSM> : BaseSMState
		where TFSM : BaseSMFSM
	{
		public override bool _isInitialized	=> _fsm._isInitialized;
		public override bool _isOperable	=> _fsm._isOperable;
		public override bool _isFinalizing	=> _fsm._isFinalizing;
		public override bool _isActive		=> _fsm._isActive;

		public TFSM _fsm		{ get; private set; }


		public override void Set( BaseSMFSM fsm ) {
			_fsm = (TFSM)fsm;
		}
	}
}