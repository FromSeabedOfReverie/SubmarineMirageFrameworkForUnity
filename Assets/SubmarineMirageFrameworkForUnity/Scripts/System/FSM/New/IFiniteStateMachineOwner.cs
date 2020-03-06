//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.FSM.New {


	// TODO : コメント追加、整頓


	public interface IFiniteStateMachineOwner<TFSM> where TFSM : IFiniteStateMachine {
		TFSM _fsm { get; }
	}
}