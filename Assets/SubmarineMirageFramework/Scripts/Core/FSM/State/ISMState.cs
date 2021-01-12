//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using Cysharp.Threading.Tasks;
	using Base;
	using MultiEvent;
	using Task;



	// TODO : コメント追加、整頓



	public interface ISMState<TFSM> : IBaseSMState
		where TFSM : ISMInternalFSM
	{
		TFSM _fsm			{ get; }

		void Set( TFSM fsm );
	}
}