//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using Task;


	// TODO : コメント追加、整頓


	public interface ISMFSMOwner<TFSM> : ISMBehaviour where TFSM : ISMFSM {
		TFSM _fsm { get; }
	}
}