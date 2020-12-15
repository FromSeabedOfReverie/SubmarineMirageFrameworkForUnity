//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.FSM {
	using Cysharp.Threading.Tasks;
	using Base;
	using Task;


	// TODO : コメント追加、整頓


	public interface ISMState<TFSM, TOwner> : ISMStandardBase
		where TFSM : ISMFSM
		where TOwner : ISMFSMOwner<TFSM>
	{
		TFSM _fsm		{ get; }
		TOwner _owner	{ get; }

		SMFSMRunState _runState	{ get; }
		bool _isActive	{ get; }
		SMTaskCanceler _activeAsyncCanceler	{ get; }

		void Set( TOwner owner );
		void StopActiveAsync();
		UniTask RunStateEvent( SMFSMRunState state );
		UniTask ChangeActive( bool isActive );
		UniTask RunBehaviourStateEvent( SMTaskRunState state );
	}
}