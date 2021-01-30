//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM.State {
	using Task;
	using FSM.Base;
	using FSM.State.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class SMState<TOwner, TFSM> : BaseSMState
		where TOwner : IBaseSMFSMOwner
		where TFSM : SMFSM
	{
		[SMHide] public override SMTaskCanceler _asyncCancelerOnDisableAndExit
			=> _fsm._asyncCancelerOnDisableAndExit;
		[SMHide] public override SMTaskCanceler _asyncCancelerOnDispose	=> _fsm._asyncCancelerOnDispose;

		[SMHide] public TOwner _topOwner	{ get; private set; }
		[SMHide] public TFSM _fsm	{ get; private set; }


		public override void Set( IBaseSMFSMOwner topOwner, SMFSM fsm ) {
			_topOwner = (TOwner)topOwner;
			_fsm = (TFSM)fsm;
		}
	}
}